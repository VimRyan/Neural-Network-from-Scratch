using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network {
    internal class images {
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
}
