// DetectMarker.cpp : Defines the entry point for the console application.
//

//#include "stdafx.h"
#include <opencv2/highgui.hpp>
#include <opencv2/aruco.hpp>
#include <iostream>
#include <opencv2/opencv.hpp>

#include "Pipe.h"


using namespace std;
using namespace cv;

Pipe myPipe;

namespace {
	const char* about = "Basic marker detection";
	const char* keys =
		"{d        |       | dictionary: DICT_4X4_50=0, DICT_4X4_100=1, DICT_4X4_250=2,"
		"DICT_4X4_1000=3, DICT_5X5_50=4, DICT_5X5_100=5, DICT_5X5_250=6, DICT_5X5_1000=7, "
		"DICT_6X6_50=8, DICT_6X6_100=9, DICT_6X6_250=10, DICT_6X6_1000=11, DICT_7X7_50=12,"
		"DICT_7X7_100=13, DICT_7X7_250=14, DICT_7X7_1000=15, DICT_ARUCO_ORIGINAL = 16}"
		"{v        |       | Input from video file, if ommited, input comes from camera }"
		"{ci       | 0     | Camera id if input doesnt come from video (-v) }"
		"{c        |       | Camera intrinsic parameters. Needed for camera pose }"
		"{l        | 0.1   | Marker side lenght (in meters). Needed for correct scale in camera pose }"
		"{dp       |       | File of marker detector parameters }"
		"{r        |       | show rejected candidates too }";
}
static Point origin;
int TranslatePoint(const Point originPt, const Point videoPt, Point *xyPt);

void CallBackFunc(int event, int x, int y, int flags, void* ptr);
int ConvertVideo2Stage(float xscale, float yscale, Point videoPt, Point2f * stagePt);
int TranslatePoint(const Point originPt, const Point videoPt, Point *xyPt)
{
	xyPt->x = videoPt.x - originPt.x;
	xyPt->y = videoPt.y - originPt.y;
	xyPt->y = -xyPt->y;
	return 0;
};
/**
*/
static bool readCameraParameters(string filename, Mat &camMatrix, Mat &distCoeffs) {
	FileStorage fs(filename, FileStorage::READ);
	if (!fs.isOpened())
		return false;
	fs["camera_matrix"] >> camMatrix;
	fs["distortion_coefficients"] >> distCoeffs;
	return true;
}

static bool averagePointFromCorners(vector< vector< Point2f > > corners, OutputArray ids, vector< Point2f > & average)
{
	if (corners.size() <= 0)
	{
		return false;
	}
	Mat id = ids.getMat();
	map <int, Point2f> mapAverage;
	Point2f avept(0.0, 0.0);
	for (int i = 0; i < corners.size(); i++)
	{
		Point2f pt(0.0, 0.0);
		for (int j = 0; j < corners[i].size(); j++)
		{
			pt.x = pt.x + corners[i][j].x;
			pt.y = pt.y + corners[i][j].y;
		}
		avept.x = pt.x / corners[i].size();
		avept.y = pt.y / corners[i].size();
		int ids = id.at<int>(0, i);
		mapAverage.insert(pair <int, Point2f>(id.at<int>(0,i),avept));
	}

	map <int, Point2f>::const_iterator pos = mapAverage.begin();

	for (int i = 0; i < mapAverage.size(); i++)
	{

		average.push_back(pos->second);
		pos++;
	}
	return true;
}

static bool drawXYAxis(Mat image, vector< Point2f > & average)
{
	if (average.size() <= 0)
	{
		return false;
	}

	int thickness = 1;
	int lineType = 8;
	Point2f start = average[0];
	Point2f end = average[1];
	line(image, start, end, Scalar(0, 200, 0), thickness, lineType);

	start = average[0];
	end = average[2];
	line(image, start, end, Scalar(0, 200, 0), thickness, lineType);



	return true;
}
/**
*/
static bool readDetectorParameters(string filename, Ptr<aruco::DetectorParameters> &params) {
	FileStorage fs(filename, FileStorage::READ);
	if (!fs.isOpened())
		return false;
	fs["adaptiveThreshWinSizeMin"] >> params->adaptiveThreshWinSizeMin;
	fs["adaptiveThreshWinSizeMax"] >> params->adaptiveThreshWinSizeMax;
	fs["adaptiveThreshWinSizeStep"] >> params->adaptiveThreshWinSizeStep;
	fs["adaptiveThreshConstant"] >> params->adaptiveThreshConstant;
	fs["minMarkerPerimeterRate"] >> params->minMarkerPerimeterRate;
	fs["maxMarkerPerimeterRate"] >> params->maxMarkerPerimeterRate;
	fs["polygonalApproxAccuracyRate"] >> params->polygonalApproxAccuracyRate;
	fs["minCornerDistanceRate"] >> params->minCornerDistanceRate;
	fs["minDistanceToBorder"] >> params->minDistanceToBorder;
	fs["minMarkerDistanceRate"] >> params->minMarkerDistanceRate;
	fs["cornerRefinementMethod"] >> params->cornerRefinementMethod;
	fs["cornerRefinementWinSize"] >> params->cornerRefinementWinSize;
	fs["cornerRefinementMaxIterations"] >> params->cornerRefinementMaxIterations;
	fs["cornerRefinementMinAccuracy"] >> params->cornerRefinementMinAccuracy;
	fs["markerBorderBits"] >> params->markerBorderBits;
	fs["perspectiveRemovePixelPerCell"] >> params->perspectiveRemovePixelPerCell;
	fs["perspectiveRemoveIgnoredMarginPerCell"] >> params->perspectiveRemoveIgnoredMarginPerCell;
	fs["maxErroneousBitsInBorderRate"] >> params->maxErroneousBitsInBorderRate;
	fs["minOtsuStdDev"] >> params->minOtsuStdDev;
	fs["errorCorrectionRate"] >> params->errorCorrectionRate;
	return true;
}


int main(int argc, char *argv[])
{
	myPipe.CreateSecurity();
	myPipe.CreatePipe();

	static Point videoPt;
	CommandLineParser parser(argc, argv, keys);
	parser.about(about);

	if (argc < 2) {
		parser.printMessage();
		return 0;
	}

	int dictionaryId = parser.get<int>("d");
	bool showRejected = parser.has("r");
	bool estimatePose = parser.has("c");
	float markerLength = parser.get<float>("l");


	Ptr<aruco::DetectorParameters> detectorParams = aruco::DetectorParameters::create();
	if (parser.has("dp")) {
		bool readOk = readDetectorParameters(parser.get<string>("dp"), detectorParams);
		if (!readOk) {
			cerr << "Invalid detector parameters file" << endl;
			return 0;
		}
	}
	detectorParams->cornerRefinementMethod = aruco::CORNER_REFINE_SUBPIX; // do corner refinement in markers

	int camId = parser.get<int>("ci");

	String video;
	if (parser.has("v")) {
		video = parser.get<String>("v");
	}

	if (!parser.check()) {
		parser.printErrors();
		return 0;
	}

	Ptr<aruco::Dictionary> dictionary =
		aruco::getPredefinedDictionary(aruco::PREDEFINED_DICTIONARY_NAME(dictionaryId));

	Mat camMatrix, distCoeffs;
	if (estimatePose) {
		bool readOk = readCameraParameters(parser.get<string>("c"), camMatrix, distCoeffs);
		if (!readOk) {
			cerr << "Invalid camera file" << endl;
			return 0;
		}
	}

	VideoCapture inputVideo;
	int waitTime;
	if (!video.empty()) {
		inputVideo.open(video);
		waitTime = 0;
	}
	else {
		inputVideo.open(camId);
		waitTime = 10;
	}

	double totalTime = 0;
	int totalIterations = 0;

	while (inputVideo.grab()) {
		Mat image, imageCopy;
		inputVideo.retrieve(image);

		double tick = (double)getTickCount();

		vector< int > ids;
		vector< vector< Point2f > > corners, rejected;
		vector< Vec3d > rvecs, tvecs;
		vector< Point2f > average;

		// detect markers and estimate pose
		aruco::detectMarkers(image, dictionary, corners, ids, detectorParams, rejected);
		averagePointFromCorners(corners, ids, average);
		if (estimatePose && ids.size() > 0)
			aruco::estimatePoseSingleMarkers(corners, markerLength, camMatrix, distCoeffs, rvecs,
				tvecs);

		double currentTime = ((double)getTickCount() - tick) / getTickFrequency();
		totalTime += currentTime;
		totalIterations++;
		//if (totalIterations % 30 == 0) {
		//	cout << "Detection Time = " << currentTime * 1000 << " ms "
		//		<< "(Mean = " << 1000 * totalTime / double(totalIterations) << " ms)" << endl;
		//}

		// draw results
		image.copyTo(imageCopy);
		if (ids.size() > 0) {
			aruco::drawDetectedMarkers(imageCopy, corners, ids);

			if (estimatePose) {
				for (unsigned int i = 0; i < ids.size(); i++)
					aruco::drawAxis(imageCopy, camMatrix, distCoeffs, rvecs[i], tvecs[i],
						markerLength * 0.5f);
			}
		}

		if (showRejected && rejected.size() > 0)
			aruco::drawDetectedMarkers(imageCopy, rejected, noArray(), Scalar(100, 0, 255));
		
		if (ids.size() > 3) {
			drawXYAxis(imageCopy, average);
		}

		imshow("out", imageCopy);
		char key = (char)waitKey(waitTime);
		if (key == 27)
		{
			string command;
			myPipe.Connect();
			myPipe.Read(command);
			myPipe.Write("end");
			myPipe.Disconnect();
			break;
		}
		

		if (average.size() > 3)
		{
			vector<Point2f> pts_corners;

			pts_corners.push_back(Point2f(average[0]));
			pts_corners.push_back(Point2f(average[1]));
			pts_corners.push_back(Point2f(average[2]));
			pts_corners.push_back(Point2f(average[3]));

			vector<Point2f> pts_dst;

			pts_dst.push_back(Point2f(80, 400));
			pts_dst.push_back(Point2f(80, 80));
			pts_dst.push_back(Point2f(500, 400));
			pts_dst.push_back(Point2f(500, 80));
			origin = pts_dst[0];

			// Calculate Homography
			Mat h = findHomography(pts_corners, pts_dst);
//			Mat h = getPerspectiveTransform(pts_corners, pts_dst);

			Mat im_out;
			// Warp source image to destination based on homography
			warpPerspective(imageCopy, im_out, h, imageCopy.size());
			
			imshow("XYView", im_out);

			Point2f stagePt;
			setMouseCallback("XYView", CallBackFunc, &stagePt);

			pts_corners.clear();
			pts_dst.clear();
		}
	}

	return 0;
}

int ConvertVideo2Stage(float xscale, float yscale, Point videoPt, Point2f * stagePt)
{

	stagePt->x = videoPt.x*xscale;
	stagePt->y = videoPt.y*yscale;
	cout << "Left button of the mouse is clicked - STAGE position (" << stagePt->x << ", " << stagePt->y << ")" << endl;

	return 0;
}
void CallBackFunc(int event, int x, int y, int flags, void* ptr)
{
	if (event == EVENT_LBUTTONDOWN)
	{
		float xrefdist = 6.9375;  // inch
		float yrefdist = 5.875;   // inch

		float xscale = 0.0;       // pixel/inch
		float yscale = 0.0;       // pixel/inch
		string myString;
		string outstring1;

		Point videoPt;
		videoPt.x = x;
		videoPt.y = y;

		Point p;
		TranslatePoint(origin, videoPt, &p);
		cout << "Left button of the mouse is clicked - position (" << p.x << ", " << p.y << ")" << endl;
		
		xscale = abs(xrefdist / (80 - 500));
		yscale = abs(yrefdist / (80 - 400));
		Point2f *stagePt = (Point2f*)ptr;
		ConvertVideo2Stage(xscale, -yscale, p, stagePt);
		
		myPipe.Connect();
		myPipe.Read(myString);

		outstring1 = to_string(stagePt->x) + ',' + to_string(stagePt->y);
		myPipe.Write(outstring1);
		myPipe.Disconnect();

	}

	if (event == EVENT_LBUTTONDOWN)
	{

	}
	//else if(event == EVENT_RBUTTONDOWN)
	//{
	//	cout << "Right button of the mouse is clicked - position (" << x << ", " << y << ")" << endl;
	//}
	//else if(event == EVENT_MBUTTONDOWN)
	//{
	//	cout << "Middle button of the mouse is clicked - position (" << x << ", " << y << ")" << endl;
	//}
	//else if (event == EVENT_MOUSEMOVE)
	//{
	//	cout << "Mouse move over the window - position (" << x << ", " << y << ")" << endl;
	//}
}