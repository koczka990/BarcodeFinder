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
                //var greyFrame = new Mat();
                //Cv2.CvtColor(frame, greyFrame, ColorConversionCodes.BGR2GRAY);
                //var threshFrame = new Mat();
                //Cv2.Threshold(greyFrame, threshFrame, 125, 255, ThresholdTypes.Binary);
                //Cv2.ImShow("Frame", frame);
                //Cv2.ImShow("GreyFrame", greyFrame);
                //Cv2.ImShow("Thresholded", threshFrame);

            }
        }

        static void drawBarcodeBoundingBox(Mat frame)
        {
            var grayFrame = new Mat();
            Cv2.CvtColor(frame, grayFrame, ColorConversionCodes.BGR2GRAY);
            Cv2.ImShow("gray", grayFrame);

            var clahe = Cv2.CreateCLAHE(2.0, new Size(8,8));
            clahe.Apply(grayFrame, grayFrame);
            Cv2.ImShow("clahe", grayFrame);

            var laplacian = new Mat();
            Cv2.Laplacian(grayFrame, laplacian, MatType.CV_8U, 3, 1, 0);
            Cv2.ImShow("Laplacian", laplacian);

            var blurred = new Mat();
            Cv2.BilateralFilter(laplacian, blurred, 13, 50, 50);
            Cv2.ImShow("blurred", blurred);

            var thresh = new Mat();
            Cv2.Threshold(blurred, thresh, 55, 255, ThresholdTypes.Binary);
            Cv2.ImShow("Thresholded", thresh);

            var kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(9, 9));
            var closed = new Mat();
            Cv2.MorphologyEx(thresh, closed, MorphTypes.Close, kernel);
            Cv2.Erode(closed, closed, new Mat(), null, 4);
            Cv2.Dilate(closed, closed, new Mat(), null, 4);
            Cv2.ImShow("After Morphology", closed);

            var cnts = new Mat[50];
            Cv2.FindContours(closed, out cnts, new Mat(), RetrievalModes.External, ContourApproximationModes.ApproxSimple);
            foreach(Mat m in cnts)
            {
                var r = new Rect();
                r = m.BoundingRect();
                frame.Rectangle(r, new Scalar (0,0,255), 2);
            }
            Cv2.ImShow("OUTPUT", frame);
        }
    }
}





//# find contours left in the image
//(_, cnts, _) = cv2.findContours(closed.copy(), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
//c = sorted(cnts, key = cv2.contourArea, reverse = True)[0]
//rect = cv2.minAreaRect(c)
//box = np.int0(cv2.boxPoints(rect))
//cv2.drawContours(image, [box], -1, (0, 255, 0), 3)
//print(box)
//cv2.imshow("found barcode", image)
//cv2.waitKey(0)
//retval = cv2.imwrite("found.jpg", image)
