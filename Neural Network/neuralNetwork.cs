using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network {
    internal class neuralNetwork {
        public int epochs;

        //these arrays store the image data
        int[,] trainImages;
        int[,] testImages;
        int[] trainLabels;
        int[] testLabels;

        // Variables that dictate the amount of nodes each layer has  
        static int layer1count = 100;
        static int outputLayerCount = 10;
        static int inputLayerCount = 784;

        int trainMax, testMax;

        //arrays that are used to do the calculations

        double[,] weight1Old;
        double[,] weight1;

        double[] firstLayer;

        double[] layer1BiasOld;
        double[] layer1Bias;

        double[] S;

        double[,] weight2Old;
        double[,] weight2;

        double[] outLayerBiasOld;
        double[] outLayerBias;

        double[] outputLayer;
        double[] finalOutput;
        double[] truth;


        const double learn = 0.005;

        /*
         * Constructor
         * Needs an images object to get all the images 
         * The size of the input image
         * And the amount of train and test images
         */
        public neuralNetwork(images im, int x, int trainM, int testM) {
            epochs = 0;
            trainImages = im.trainImages; testImages = im.testImages;
            trainLabels = im.trainLabels; testLabels = im.testLabels;
            trainMax = trainM; testMax = testM;
            inputLayerCount = x;

            // wO w 
            weight1Old = new double[inputLayerCount, layer1count];
            weight1 = new double[inputLayerCount, layer1count];

            // y
            firstLayer = new double[layer1count];

            // bO b
            layer1BiasOld = new double[layer1count];
            layer1Bias = new double[layer1count];

            // s 
            S = new double[layer1count];

            // uO u
            weight2Old = new double[layer1count, outputLayerCount];
            weight2 = new double[layer1count, outputLayerCount];

            // cO c
            outLayerBiasOld = new double[outputLayerCount];
            outLayerBias = new double[outputLayerCount];

            // r
            outputLayer = new double[outputLayerCount];

            // z
            finalOutput = new double[outputLayerCount];

            // t
            truth = new double[outputLayerCount];
            fillArrays();
        }

        /*
         * Fills the the initial arrays with values so calculations can be done
         */
        private void fillArrays() {
            var rand = new Random();

            for (int i = 0; i < inputLayerCount; i++) {
                for (int j = 0; j < layer1count; j++) {
                    //wO[i, j] = rand.NextDouble() - 0.5;
                    weight1Old[i, j] = rand.NextDouble() - 0.5;
                }
            }

            for (int i = 0; i < layer1count; i++) {
                //bO[i] = rand.NextDouble() - 0.5;
                layer1BiasOld[i] = rand.NextDouble() - 0.5;
                for (int j = 0; j < outputLayerCount; j++) {
                    //uO[i, j] = rand.NextDouble() - 0.5;
                    weight2Old[i, j] = rand.NextDouble() - 0.5;
                }
            }

            for (int i = 0; i < outputLayerCount; i++) {
                //cO[i] = rand.NextDouble() - 0.5;
                outLayerBiasOld[i] = rand.NextDouble() - 0.5;

            }
        }


        /*
         * Calculates one Epoch
         * Includes feed foward and back propagation
         */
        public void startEpoch() {
            epochs += 1;


            Parallel.For(0, trainMax, k => {
                double addVar = 0;

                /*
                 * Start of feed forward calculations
                 */

                //Fill the truth array
                for (int i = 0; i < outputLayerCount; i++) {
                    truth[i] = 0;
                }
                truth[trainLabels[k]] = 1;

                //Calculate the values of the nodes for the first layer
                for (int i = 0; i < layer1count; i++) {
                    S[i] = layer1BiasOld[i];
                    for (int j = 0; j < inputLayerCount; j++) {
                        //Parallel.For(0, 784, j => {
                        S[i] += weight1Old[j, i] * trainImages[k, j];
                    }
                    firstLayer[i] = 1 / (1 + Math.Exp(-S[i]));
                }

                //Calculate the values the output 
                for (int i = 0; i < outputLayerCount; i++) {
                    outputLayer[i] = outLayerBiasOld[i];
                    for (int j = 0; j < layer1count; j++) {
                        outputLayer[i] += firstLayer[j] * weight2Old[j, i];
                    }
                    finalOutput[i] = 1 / (1 + Math.Exp(-outputLayer[i]));
                }

                /*
                 * Start of back propagation calculations
                 */

                //fill the second weights
                for (int i = 0; i < layer1count; i++) {
                    for (int j = 0; j < outputLayerCount; j++) {
                        weight2[i, j] = weight2Old[i, j] - learn * ((finalOutput[j] - truth[j])
                        * finalOutput[j] * (1 - finalOutput[j]) * firstLayer[i]);
                    }
                }

                //fill the first weights
                for (int i = 0; i < inputLayerCount; i++) {
                    for (int j = 0; j < layer1count; j++) {
                        //Parallel.For(0, 50, j => {
                        weight1[i, j] = weight1Old[i, j];
                        addVar = 0;
                        for (int b = 0; b < 10; b++) {
                            addVar += (finalOutput[b] - truth[b]) * finalOutput[b] * (1 - finalOutput[b])
                            * weight2Old[j, b] * firstLayer[j] * (1 - firstLayer[j]) * trainImages[k, i];
                        }
                        weight1[i, j] -= learn * addVar;
                    }
                }

                //fill the layer one bias array
                for (int i = 0; i < layer1count; i++) {
                    layer1Bias[i] = layer1BiasOld[i];
                    addVar = 0;
                    for (int j = 0; j < 10; j++) {
                        addVar += (finalOutput[j] - truth[j]) * finalOutput[j] * (1 - finalOutput[j])
                        * weight2Old[i, j] * firstLayer[i] * (1 - firstLayer[i]);
                    }
                    layer1Bias[i] -= learn * addVar;
                }

                //fill the output layer bias array
                for (int i = 0; i < outputLayerCount; i++) {
                    outLayerBias[i] = outLayerBiasOld[i] - learn *
                    ((finalOutput[i] - truth[i]) * finalOutput[i] * (1 - finalOutput[i]));
                }

                //Fill the old arrays with new values
                for (int i = 0; i < inputLayerCount; i++) {
                    for (int j = 0; j < layer1count; j++) {
                        weight1Old[i, j] = weight1[i, j];
                    }
                }

                for (int i = 0; i < 50; i++) {
                    for (int j = 0; j < 10; j++) {
                        weight2Old[i, j] = weight2[i, j];
                    }
                }

                for (int i = 0; i < 50; i++) {
                    layer1BiasOld[i] = layer1Bias[i];
                }

                for (int i = 0; i < 10; i++) {
                    outLayerBiasOld[i] = outLayerBias[i];
                }


            });

        }


        /*
         * Calculate the error rate based on the current neural network
         */
        public double calcError() {
            double val;
            double correctPredicts = 0;
            int index;

            for (int x = 0; x < testMax; x++) {
                double[] midLayer = new double[layer1count];
                double[] output = new double[outputLayerCount];

                for (int i = 0; i < layer1count; i++) {
                    midLayer[i] = layer1BiasOld[i];
                    for (int j = 0; j < inputLayerCount; j++) {
                        midLayer[i] += weight1[j, i] * testImages[x, j];
                    }
                }

                for (int i = 0; i < layer1count; i++) {
                    midLayer[i] = 1 / (1 + Math.Exp(-midLayer[i]));
                }

                for (int i = 0; i < outputLayerCount; i++) {
                    output[i] = outLayerBias[i];
                    for (int j = 0; j < layer1count; j++) {
                        output[i] += weight2[j, i] * midLayer[j];
                    }
                }

                for (int i = 0; i < outputLayerCount; i++) {
                    output[i] = 1 / (1 + Math.Exp(-output[i]));
                }

                //check if the prediction is correct
                val = output[0];
                index = 0;
                for (int i = 1; i < 10; i++) {
                    if (val < output[i]) {
                        index = i;
                        val = output[i];
                    }
                }


                if (index == testLabels[x]) {
                    correctPredicts++;
                }

            }

            val = (correctPredicts / testMax) * 100;

            return val;
        }

        /*
         * Makes a prediction based on the passed image
         */
        public double[] makePrediction(int[] currentImage) {
            double[] output = new double[outputLayerCount];
            double[] midLayer = new double[layer1count];

            for (int i = 0; i < layer1count; i++) {
                midLayer[i] = 0;
                for (int j = 0; j < inputLayerCount; j++) {
                    midLayer[i] += weight1[j, i] * currentImage[j];
                }
            }

            for (int i = 0; i < layer1count; i++) {
                midLayer[i] = 1 / (1 + Math.Exp(-midLayer[i]));
            }

            for (int i = 0; i < outputLayerCount; i++) {
                output[i] = 0;
                for (int j = 0; j < layer1count; j++) {
                    output[i] += weight2[j, i] * midLayer[j];
                }
            }

            for (int i = 0; i < outputLayerCount; i++) {
                output[i] = 1 / (1 + Math.Exp(-output[i]));
            }

            return output;
        }
    }
}
