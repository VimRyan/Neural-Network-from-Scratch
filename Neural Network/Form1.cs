namespace Neural_Network {
    
    
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();

            //these will load 4 arrays with the image data
            im.trainImages = OpenByteFile(trainImagePath, trainMax);
            im.testImages = OpenByteFile(testImagePath, testMax);
            im.trainLabels = openLabelFile(trainLabelsPath, trainMax);
            im.testLabels = openLabelFile(testLabelsPath, testMax);

            numericUpDown1.Maximum = trainMax - 1;
            displayImage(radioButton1.Checked ? true : false, (int)numericUpDown1.Value);
            numericUpDown1.Maximum = trainMax - 1;

            network = new neuralNetwork(im);
        }

        private const int trainMax = 60_000;
        private const int testMax = 10_000;
        neuralNetwork network;
        readonly images im = new(trainMax, testMax, 784);

        //Theses are the file paths to access the mnist images
        //Image files contain the images themselves
        //Label files contain what the value of each picture is
        private const String testImagePath = "..\\..\\..\\t10k-images.idx3-ubyte";
        private const String trainImagePath = "..\\..\\..\\train-images.idx3-ubyte";
        private const String testLabelsPath = "..\\..\\..\\t10k-labels.idx1-ubyte";
        private const String trainLabelsPath = "..\\..\\..\\train-labels.idx1-ubyte";



        int imageTruth;
        int[] currentImage = new int[784];

        /**
        * This function will open the the image file the contains the data for the images.
        * The file does not contain an image, it contains the values of each pixel for each image.
        * It reads in the file and returns a 2d array that contains the data
        * data[0,...] contains the values for image 0
        * data[1,...] contains the values for image 1
        */
        private int[,] OpenByteFile(String filePath, int imageAmount) {

            byte[] temp;
            int[,] data = new int[imageAmount, 28 * 28];
            int offset = 0;

            BinaryReader images = new BinaryReader(new FileStream(filePath, FileMode.Open));

            images.ReadBytes(16);
            temp = images.ReadBytes((int)images.BaseStream.Length);
            // textBox1.Text += filePath + imageAmount + "             ";

            for (int x = 0; x < imageAmount; x++) {
                for (int y = 0; y < 28 * 28; y++) {
                    data[x, y] = temp[offset];
                    offset++;
                }
            }

            images.Close();
            return data;
        }

        /**
         * Opens a label file and returns an array with the labels for each image
         * data[0] contains the label for image 0
         * data[1] contains the label for image 1
         */
        private int[] openLabelFile(String filePath, int imageAmount) {
            byte[] temp;
            int[] data = new int[imageAmount];
            //int offset = 0;

            BinaryReader images = new BinaryReader(new FileStream(filePath, FileMode.Open));

            images.ReadBytes(8);
            temp = images.ReadBytes((int)images.BaseStream.Length);
            //  textBox1.Text += filePath + imageAmount + "\n";

            for (int i = 0; i < imageAmount; i++) {
                data[i] = temp[i];
            }

            images.Close();
            return data;
        }

        /**
         * Displays an image based on the passed parameters
         * ImageType indicates if its a test image or a training image
         * ImageVal is the position in the array
         */
        private void displayImage(bool imageType, int imageVal) {
            Bitmap picImage = new Bitmap(28, 28);
            Color pix;
            int val = 0;
            int[,] images;


            if (imageType) {
                images = im.trainImages;
                imageTruth = im.trainLabels[imageVal];
                label4.Text = "Image Truth: " + imageTruth.ToString();
            } else {
                images = im.testImages;
                imageTruth = im.testLabels[imageVal];
                label4.Text = "Image Truth: " + imageTruth.ToString();
            }

            for (int i = 0; i < 28; i++) {
                for (int j = 0; j < 28; j++) {
                    currentImage[val] = images[imageVal, val];
                    pix = Color.FromArgb(images[imageVal, val], images[imageVal, val], images[imageVal, val]);
                    picImage.SetPixel(j, i, pix);
                    val++;
                }
            }
            pictureBox1.Image = makeBig(makeBig(picImage));

        }
        
        /*
         * Increases the size of the passed bitmap
         */
        private Bitmap makeBig(Bitmap image) {
            Bitmap newBitmap = new Bitmap(image.Width * 3, image.Height * 3);
            Color pix;

            for (int i = 0; i < image.Width; i++) {
                for (int j = 0; j < image.Height; j++) {
                    pix = image.GetPixel(i, j);

                    newBitmap.SetPixel((i * 3), (j * 3), pix); newBitmap.SetPixel((i * 3) + 1, (j * 3), pix); newBitmap.SetPixel((i * 3) + 2, (j * 3), pix);
                    newBitmap.SetPixel((i * 3), (j * 3) + 1, pix); newBitmap.SetPixel((i * 3) + 1, (j * 3) + 1, pix); newBitmap.SetPixel((i * 3) + 2, (j * 3) + 1, pix);
                    newBitmap.SetPixel((i * 3), (j * 3) + 2, pix); newBitmap.SetPixel((i * 3) + 1, (j * 3) + 2, pix); newBitmap.SetPixel((i * 3) + 2, (j * 3) + 2, pix);
                }
            }

            return newBitmap;

        }

        /*
         * The functions below add functionality to the buttons
         */
        private void radioButton1_CheckedChanged(object sender, EventArgs e) {
            numericUpDown1.Maximum = trainMax - 1;
            displayImage(true, (int)numericUpDown1.Value);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e) {
            numericUpDown1.Maximum = testMax - 1;
            displayImage(false, (int)numericUpDown1.Value);
        }

        private void NumericEnterVal(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                if (!int.TryParse(numericUpDown1.Value.ToString(), out int value)) {
                    numericUpDown1.Value = (int)numericUpDown1.Value;
                    throw new Exception("Error entered value is not an int.");
                }

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e) {
           
            if (!int.TryParse(numericUpDown1.Value.ToString(), out int value)) {
                throw new Exception("Error: Entered value is not an int.");
            }

            displayImage(radioButton1.Checked ? true : false, (int)numericUpDown1.Value);

        }

        /*
         * Starts one epoch
         * Training takes a while to complete
         */
        private void button1_Click(object sender, EventArgs e) {
            //start the training loop
            displayMessage("This might take a while");
            network.startEpoch();

            displayMessage("Done");
        }

        /*
         * Makes a prediction for the current image
         */
        private void button2_Click(object sender, EventArgs e) {

        }

        /*
         * Creates a form that displays the passed string
         */
        private void displayMessage(string msg) {
            Form temp = new Form() { Size = new Size(400, 100) };
            temp.Text = msg;
            temp.Show();
        }

    }
    
    
    public class images {
        //Arrays that will store all test and training images and labels
        public int[,] trainImages;
        public int[,] testImages;
        public int[] trainLabels;
        public int[] testLabels;

        public images(int trainMax, int testMax, int pixelCount) {
            trainImages = new int[trainMax, 28 * 28];
            testImages = new int[testMax, 28 * 28];
            trainLabels = new int[trainMax];
            testLabels = new int[testMax];
        }

    }

    class neuralNetwork {
        int epochs;

        public int[,] trainImages;
        public int[,] testImages;
        public int[] trainLabels;
        public int[] testLabels;

        double[,] wO = new double[784, 50]; double[,] w = new double[784, 50];
        double[] bO = new double[50]; double[] b = new double[50];
        double[,] uO = new double[50, 10]; double[,] u = new double[50, 10];
        double[] cO = new double[10]; double[] c = new double[10];
        //   double[] zO = new double[10];      
        double[] z = new double[10];
        double[] s = new double[50]; double[] y = new double[50];
        double[] r = new double[10];
        int[] t = new int[10];

        private const double learn = 0.005;


        public neuralNetwork(images im) {
            epochs = 0;
            trainImages = im.trainImages;
            testImages = im.testImages;
            trainLabels = im.trainLabels;
            testLabels = im.testLabels;
        }

        /*
         */
        
        public void startEpoch() {
            
        }
    }

}