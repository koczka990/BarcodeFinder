using OpenCvSharp;
using System;

namespace BarcodeFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            var cap = new VideoCapture(0);
            Finder finder = new Finder();
            while (true)
            {
                var frame = new Mat();
                cap.Read(frame);
                Mat gray = finder.createGrayScaleImage(frame);
                Cv2.ImShow("output", gray);
                Cv2.ImShow("original", frame);
                //drawBarcodeBoundingBox(frame);
                if ((char)Cv2.WaitKey(24) == 27) break;

            }
        }
    }
}