using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarcodeFinder
{
    class Finder
    {
        const int WHITEBLACKTHRESHOLD = 10;

        public void ScharrImage(Mat original)
        {
            Mat gray = new Mat();
            Cv2.CvtColor(original, gray, ColorConversionCodes.BGR2GRAY);

            Mat gradX = new Mat();
            Mat gradY = new Mat();

            Cv2.Scharr(gray, gradX, MatType.CV_8UC1, 0, 1);
            Cv2.Scharr(gray, gradY, MatType.CV_8UC1, 1, 0);
            //Cv2.Erode(gradY, gradY, new Mat());



            Mat prob = probabilityBarcode(gradY, gradY.Width/200, 200, 0.2);
            //Cv2.ImShow("grad", prob);

            var kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(21, 7));
            Mat closed = new Mat();
            Cv2.MorphologyEx(prob, closed, MorphTypes.Close, kernel);
            //Cv2.ImShow("Closed", closed);
            
            Cv2.Erode(closed, closed, new Mat(), null, 30);
            Cv2.Dilate(closed, closed, new Mat(), null, 30);
            //Cv2.ImShow("eroded", closed);

            Mat[] contours;
            Cv2.FindContours(closed, out contours, new Mat(), RetrievalModes.External, ContourApproximationModes.ApproxSimple);
            Cv2.DrawContours(original, contours, -1, new Scalar(255, 0, 255), 2);
            //Cv2.ImShow("output", original);
            //Mat blurred = new Mat();
            //Cv2.Blur(gradY, blurred, new Size(9, 9));
            //Cv2.ImShow("blurred", blurred);

            //Cv2.Threshold(blurred, blurred, 155, 255, ThresholdTypes.Binary);
            //Cv2.ImShow("Thresholded", blurred);

            //Cv2.Dilate(prob, prob, new Mat(), null, 5);

            //Mat grad = gradX + gradY;
            //Cv2.ImShow("gradX", gradX);
            //Cv2.ImShow("gradY", gradY);
            //Cv2.ImShow("PROB", prob);
            //Cv2.ImShow("grad", grad);
        }

        private Mat probabilityBarcode(Mat image, int r, double minVal, double minProb)
        {
            Mat output = new Mat(image.Size(), MatType.CV_8UC1, new Scalar(0));
            for (int y = 0; y < image.Height - r; y += r)
            {
                for(int x = 0; x < image.Width - r; x += r)
                {
                    Rect rect = new Rect(x, y, r, r);
                    int sum = 0;
                    var indexer = image.GetGenericIndexer<byte>();
                    for(int ry = y; ry < y+r; ry++)
                    {
                        for(int rx = x; rx < x+r; rx++)
                        {
                            if (indexer[ry, rx] > minVal) sum += 1;
                        }
                    }
                    
                    double prob = sum / (double)(r * r);
                    if (prob > minProb) output.Rectangle(rect, 255, -1);
                    //else { output.Rectangle(rect, 0, -1); }
                }
            }
            return output;
        }

        public Mat createGrayScale(Mat original)
        {
            Mat output = new Mat(original.Size(), MatType.CV_8UC3, new Scalar(0,0,0));
            var outputIndexer = output.GetGenericIndexer<Vec3b>();

            var indexer = original.GetGenericIndexer<Vec3b>();
            for(int y = 0; y < original.Height; y++)
            {
                for(int x = 0; x < original.Width; x++)
                {
                    if (closeToWhite(indexer[y, x], 100))
                    {
                        outputIndexer[y, x] = new Vec3b(0,255,255);
                    }
                    else if (closeToBlack(indexer[y, x], 55))
                    {
                        outputIndexer[y, x] = new Vec3b(255, 0, 255);
                    }
                    else
                    {
                        outputIndexer[y, x] = indexer[y, x];
                    }
                }
            }

            return output;
        }
        public Mat createGrayScaleFromG(Mat original)
        {
            Mat output = new Mat(original.Size(), MatType.CV_8UC3, new Scalar(0,0,0));
            var outputIndexer = output.GetGenericIndexer<Vec3b>();

            Cv2.CvtColor(original, original, ColorConversionCodes.BGR2GRAY);

            var indexer = original.GetGenericIndexer<byte>();
            for(int y = 0; y < original.Height; y++)
            {
                for(int x = 0; x < original.Width; x++)
                {
                    if (indexer[y,x] > 205)
                    {
                        outputIndexer[y, x] = new Vec3b(0,255,255);
                    }else if(indexer[y,x] < 45)
                    {
                        outputIndexer[y, x] = new Vec3b(255, 0, 255);
                    }
                    else
                    {
                        byte value = indexer[y, x];
                        outputIndexer[y, x] = new Vec3b(value, value, value);
                    }
                }
            }

            return output;
        }

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
            for (int y = 2; y < original.Height - 2; y++)
            {
                for (int x = 2; x < original.Width - 2; x++)
                {
                    byte value = (byte)((double)255 * chanceOfBarcode(original, new Rect(x - 2, y - 2, 5, 5)));
                    outputIndexer[y, x] = new Vec3b(value, value, value);
                }
            }
            return output;
        }

        private double chanceOfBarcode(Mat image, Rect r)
        {
            double all = r.Width * r.Height;
            double sum = 0;
            double sumWhite = 0;
            double sumBlack = 0;
            var indexer = image.GetGenericIndexer<Vec3b>();
            for (int y = r.Y; y < r.Bottom; y++)
            {
                for (int x = r.Y; x < r.Right; x++)
                {
                    Vec3b color = indexer[y, x];
                    if (closeToWhite(color, 100)) sumWhite += 1;
                    else if (closeToBlack(color, 55)) sumBlack += 1;
                }
            }

            if (sumWhite / sumBlack > 0.8) return (sumWhite + sumBlack) / all;
            return 0;
        }
    }
}
