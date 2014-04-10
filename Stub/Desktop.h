#pragma once
#include "Client.h"

#include <windows.h>
#include <objidl.h>
#include <Unknwn.h>
#include <gdiplus.h>
#pragma comment (lib,"Gdiplus.lib")

class Desktop
{
public:
	Desktop(Client* client);
	~Desktop();

	void SendAcknowledge(Size blocks, Size blockSize, ULONG quality);
	void SendScreenBlock(Point block, ULONG* oldHash, ULONG quality);
	void Start();
	void Stop();
	static DWORD WINAPI ThreadStub(LPVOID lpParam);

private:
	void DesktopThread();
	void TakeScreenshot(char** bmp, int* len, Point block, ULONG quality);
	int GetEncoderClsid(WCHAR *format, CLSID *pClsid);
	ULONG Hash(const char* input, UINT size);
	void ClearCache();

	Client* _client;

	ULONG _quality;

	BOOL _stopped;
	HANDLE _hThread;
	ULONG* _hashCache;

	Size _blocks;
	Size _blockSize;

	ULONG_PTR _gdiplusToken;

	GUID qualityGuid;
	HDC hDesktopDC;
	HDC hCaptureDC;
	CLSID* encoderPngClsid;
	CLSID* encoderJpegClsid;
	Gdiplus::EncoderParameters* jpegEncoderParameters;
};