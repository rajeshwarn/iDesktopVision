#pragma once
#include "Client.h"
#include <Windows.h>
#include <objidl.h>
#include <Unknwn.h>
#include <gdiplus.h>

#pragma comment (lib,"Gdiplus.lib")


class Desktop2
{
public:
	Desktop2(Client* client);
	~Desktop2();

	Client* client;
	u_int quality;
	

	Gdiplus::Bitmap* backBitmap;
	ULONG_PTR gdiplusToken;
	GUID qualityGuid;
	HDC hDesktopDC;
	HDC hCaptureDC;
	CLSID* encoderPngClsid;
	CLSID* encoderJpegClsid;
	Gdiplus::EncoderParameters* jpegEncoderParameters;

	void CheckDifference(Gdiplus::Bitmap* inputBitmap, Gdiplus::Bitmap* bitmapOut);
	u_int64 Hash(char* bytes, u_int len);
	Gdiplus::Bitmap* TakeScreenshot(int x, int y, int width, int height);
	u_int SerializeBitmap(Gdiplus::Bitmap* bitmap, char** bytes);
	void ScanDesktop();
	int GetEncoderClsid(WCHAR *format, CLSID *pClsid);
};