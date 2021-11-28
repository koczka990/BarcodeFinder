using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarcodeFinder
{
    class Finder
    {
        const int WHITEBLACKTHRESHOLD = 10;
        //public Rect FindBarcode(Mat frame)
        //{
        //    var indexer = frame.GetGenericIndexer<Vec3b>();
        //    var rows = new List<int>();
        //    var columns = new List<int>();
        //    for(int y = 0; y < frame.Height; y++){
        //        int blackPixelsInaRow = 0;
        //        int currentnumOfBlackPixels = 0;
        //        int whitePixelsInaRow = 0;
        //        int currentnumOfWhitePixels = 0;
        //        bool whitePrev = false;
        //        bool blackPrev = false;
        //        for(int x = 0; x < frame.Width; x++)
        //        {
        //            if(closeToWhite(indexer[y,x], WHITEBLACKTHRESHOLD)){
                        
        //                if (white)
        //                {
        //                    n += 1;
        //                }
        //            }
        //            if(closeToBlack(indexer[y,x], WHITEBLACKTHRESHOLD))
        //            {

        //            }
        //        }
        //    }
        //}

        private bool closeToWhite(Vec3b color, int v)
        {
            if (color.Item0 > 255 - v && color.Item1 > 255 - v && color.Item2 > 255 - v) return true;
            return false;
        }

        private bool closeToBlack(Vec3b color, int v)
        {
            if (color.Item0 < v && color.Item1 < v && color.Item2 < v) return true;
            return false;
        }

        public Mat createGrayScaleImage(Mat original)
        {
            Mat output = new Mat(original.Rows, original.Cols, MatType.CV_8UC3);
            var outputIndexer = output.GetGenericIndexer<Vec3b>();
            var indexer = original.GetGenericIndexer<Vec3b>();
            for(int y = 2; y < original.Height-2; y++)
            {
                for(int x = 2; x < original.Width-2; x++)
                {
                    byte value = (byte) ((double)255 * chanceOfBarcode(original, new Rect(x - 2, y - 2, 5, 5)));
                    outputIndexer[y, x] = new Vec3b(value, value, value);
                }
            }
            return output;
        }

        private double chanceOfBarcode(Mat image, Rect r)
        {
            double all = r.Width * r.Height;
            double sum = 0;
            var indexer = image.GetGenericIndexer<Vec3b>();
            for (int y = r.Y; y < r.Height; y++)
            {
                for (int x = r.Y; x < r.Width; x++)
                {
                    Vec3b color = indexer[y, x];
                    if (closeToBlack(color, 100) || closeToWhite(color, 100)) sum += 1;
                }
            }
            
            return sum / all;
        }
    }
}
