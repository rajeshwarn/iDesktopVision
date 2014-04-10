#include "Desktop2.h"

using namespace Gdiplus;

Desktop2::Desktop2(Client* client)
{
	this->client = client;

	Gdiplus::GdiplusStartupInput gdiplusStartupInput;
	Gdiplus::GdiplusStartup(&gdiplusToken, &gdiplusStartupInput, NULL);

	qualityGuid.Data1 = 0x1d5be4b5;
	qualityGuid.Data2 = 0xfa4a;
	qualityGuid.Data3 = 0x452d;
	qualityGuid.Data4[0] = 0x9C;
	qualityGuid.Data4[1] = 0xDD;
	qualityGuid.Data4[2] = 0x5D;
	qualityGuid.Data4[3] = 0xB3;
	qualityGuid.Data4[4] = 0x51;
	qualityGuid.Data4[5] = 0x05;
	qualityGuid.Data4[6] = 0xE7;
	qualityGuid.Data4[7] = 0xEB;

	hDesktopDC = GetDC(GetDesktopWindow());
	hCaptureDC = CreateCompatibleDC(hDesktopDC);

	encoderPngClsid = new CLSID;
	encoderJpegClsid = new CLSID;
	GetEncoderClsid(L"image/png", encoderPngClsid);
	GetEncoderClsid(L"image/jpeg", encoderJpegClsid);

	jpegEncoderParameters = new Gdiplus::EncoderParameters;
	jpegEncoderParameters->Count = 1;
	jpegEncoderParameters->Parameter[0].Guid = qualityGuid;
	jpegEncoderParameters->Parameter[0].Type = 4;
	jpegEncoderParameters->Parameter[0].NumberOfValues = 1;
}

Desktop2::~Desktop2()
{
	GdiplusShutdown(gdiplusToken);

	delete encoderPngClsid;
	delete encoderJpegClsid;
	delete jpegEncoderParameters;

	ReleaseDC(GetDesktopWindow(), hDesktopDC);
	DeleteDC(hCaptureDC);
}

void Desktop2::CheckDifference(Bitmap* inputBitmap, Bitmap* bitmapOut)
{
	if (backBitmap == NULL || inputBitmap == NULL)
	{
		bitmapOut = backBitmap;
		return;
	}

	int width = inputBitmap->GetWidth();
	int height = inputBitmap->GetHeight();
	Rect rect = Rect(0, 0, width, height);

	BitmapData inputData, backData;
	inputBitmap->LockBits(&rect, ImageFlagsReadOnly, inputBitmap->GetPixelFormat(), &inputData);
	backBitmap->LockBits(&rect, ImageFlagsReadOnly, backBitmap->GetPixelFormat(), &backData);

	int* pInput = (int*)inputData.Scan0;
	int* pBack = (int*)backData.Scan0;

	int leftX = width + 1, rightX = -1;
	int topY = -1, bottomY = height + 1;
	for (int x = 0; x < width; x++)
	{
		for (int y = 0; y < height; y++)
		{
			if (*pInput != *pBack)
			{
				if (x > rightX)
					rightX = x;

				if (x < leftX)
					leftX = x;

				if (y > topY)
					topY = y;

				if (y < bottomY)
					bottomY = y;
			}
		}
	}

	bitmapOut = inputBitmap->Clone(leftX, topY, rightX - leftX, topY - bottomY, inputBitmap->GetPixelFormat());

	inputBitmap->UnlockBits(&inputData);
	backBitmap->UnlockBits(&backData);
}

u_int64 Desktop2::Hash(char* bytes, u_int len)
{
	u_int64 h = 0xcbf29ce484222325;

	for (u_int i = 0; i < len; ++i)
		h = (h ^ bytes[i]) * 0x100000001b3;

	return h;
}

Bitmap* Desktop2::TakeScreenshot(int x, int y, int width, int height)
{
	HBITMAP hCaptureBitmap = CreateCompatibleBitmap(hDesktopDC, width, height);
	SelectObject(hCaptureDC, hCaptureBitmap);
	BitBlt(hCaptureDC, 0, 0, width, height, hDesktopDC, x, y, SRCCOPY);

	Bitmap* bitmap = Bitmap::FromHBITMAP(hCaptureBitmap, NULL);
	DeleteObject(hCaptureBitmap);

	return bitmap;
}

u_int Desktop2::SerializeBitmap(Bitmap* bitmap, char** bytes)
{
	LARGE_INTEGER liZero = { 0 };
	ULARGE_INTEGER pos = { 0 };
	STATSTG stg = { 0 };

	IStream* istream;
	CreateStreamOnHGlobal(NULL, TRUE, &istream);

	if (quality < 100)
	{
		jpegEncoderParameters->Parameter[0].Value = &quality;
		bitmap->Save(istream, encoderJpegClsid, jpegEncoderParameters);
	}
	else
	{
		bitmap->Save(istream, encoderPngClsid);
	}

	istream->Seek(liZero, STREAM_SEEK_SET, &pos);
	istream->Stat(&stg, STATFLAG_NONAME);

	ULONG cbRead;
	*bytes = new char[stg.cbSize.QuadPart];
	istream->Read(*bytes, stg.cbSize.QuadPart, &cbRead);
	istream->Release();

	return cbRead;
}

int Desktop2::GetEncoderClsid(WCHAR *format, CLSID *pClsid)
{
	unsigned int num = 0, size = 0;
	Gdiplus::GetImageEncodersSize(&num, &size);
	if (size == 0) return -1;
	Gdiplus::ImageCodecInfo *pImageCodecInfo = (Gdiplus::ImageCodecInfo*)(malloc(size));
	if (pImageCodecInfo == NULL) return -1;
	Gdiplus::GetImageEncoders(num, size, pImageCodecInfo);
	for (unsigned int j = 0; j < num; ++j)
	{
		if (wcscmp(pImageCodecInfo[j].MimeType, format) == 0){
			*pClsid = pImageCodecInfo[j].Clsid;
			free(pImageCodecInfo);
			return j;
		}
	}
	free(pImageCodecInfo);
	return -1;
}