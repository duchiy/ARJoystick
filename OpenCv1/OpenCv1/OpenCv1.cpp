// OpenCv1.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "opencv2\opencv.hpp"

using namespace cv;

int _tmain(int argc, _TCHAR* argv[])
{
	Mat mat = imread("C:\\Photos\\LarissaMarcia.jpg", CV_LOAD_IMAGE_UNCHANGED);
	Mat matBlurr = imread("", CV_LOAD_IMAGE_UNCHANGED);
	Mat matOut = imread("", CV_LOAD_IMAGE_UNCHANGED);
	GaussianBlur(mat, matBlurr, Size(5, 5), 5, 5);
	addWeighted(mat, 1.6, matBlurr, -0.5, 0, matOut);
	imwrite("C:\\Photos\\LarissaMarcia2.jpg", matOut);

	return 0;
}

