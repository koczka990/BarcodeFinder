using OpenCvSharp;
using System;

namespace BarcodeFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            var image = Cv2.ImRead("C:\\Users\\koczk\\AAAAA\\BME\\5.felev\\témalabor\\barcode_test\\BarcodeFinder\\BarcodeFinder\\pictures\\barcode2.png");
            
            Finder finder = new Finder();
            //Mat gray = finder.createGrayScaleFromG(image);
            //Mat gray = finder.createGrayScale(image);
            //Mat gray = finder.createGrayScaleImage(image);
            //Cv2.ImShow("output", gray);

            //Cv2.ImShow("original", image);
            finder.ScharrImage(image);
            //Mat hehe = finder.createGrayScaleImage(image);
            //Cv2.ImShow("output", image);
            //Cv2.ImShow("hehe", hehe);
            Cv2.WaitKey();


            //var cap = new VideoCapture(0);
            //while (true)
            //{
            //    var frame = new Mat();
            //    cap.Read(frame);
            //    Mat gray = finder.createGrayScaleFromG(frame);
            //    Cv2.ImShow("output", gray);
            //    Cv2.ImShow("original", frame);
            //    if ((char)Cv2.WaitKey(24) == 27) break;
            //}
        }
    }
}