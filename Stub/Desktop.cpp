#include "Desktop.h"
#include "Util.h"
#include "lz4.h"
#include <d3d9.h>

Desktop::Desktop(Client* client)
{
	_client = client;
	_hashCache = NULL;
	
	Gdiplus::GdiplusStartupInput gdiplusStartupInput;
	Gdiplus::GdiplusStartup(&_gdiplusToken, &gdiplusStartupInput, NULL);

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

Desktop::~Desktop()
{
	if (_hashCache != NULL)
		delete _hashCache;

	Gdiplus::GdiplusShutdown(_gdiplusToken);

	delete encoderPngClsid;
	delete encoderJpegClsid;
	delete jpegEncoderParameters;

	ReleaseDC(GetDesktopWindow(), hDesktopDC);
	DeleteDC(hCaptureDC);

	Stop();
}

void Desktop::SendAcknowledge(Size blocks, Size blockSize, ULONG quality)
{
	Stop();

	_blocks = blocks;		
	_blockSize = blockSize;
	_quality = quality;

	if (_hashCache != NULL)
	{
		delete _hashCache;
		_hashCache = NULL;
	}

	_hashCache = new ULONG[blocks.Width*blocks.Height];

	AckScreensharePacket packet(blockSize, blocks);
	(*_client).SendPacket(packet);

	Start();
}

void Desktop::SendScreenBlock(Point block, ULONG* oldHash, ULONG quality)
{
	char* bmp = NULL;
	char* buffer = NULL;
	int len;
	
	TakeScreenshot(&bmp, &len, block, quality);
	ULONG hash;
	if ((hash = Hash(bmp, len)) == *oldHash)
	{
		if (bmp != NULL)
			delete[] bmp;

		return;
	}
	
	*oldHash = hash;
	SendScreenshotBlockPacket packet(block, bmp, len, hash);
	//if (len > 0 && quality < 80) //compressing doesn't really work at high quality pictures (95%)
	//{
	//	buffer = new char[(int)(len*1.05)]; //incrompessible data is 0.4% larger, 105% should be safe

	//	int comp = LZ4_compress(bmp, buffer, len);
	//	packet = SendScreenshotBlockPacket(block, buffer, comp, hash, true, len);
	//}
	//else
	//{
	//	packet = SendScreenshotBlockPacket(block, bmp, len, hash);
	//}
	
	(*_client).SendPacket(packet);

	if (bmp != NULL)
		delete[] bmp; //TakeScreenshot allocated it, how nice :D

	//if (buffer != NULL)
	//	delete[] buffer; // Used when compressing is applied 
}

void Desktop::Start()
{
	_stopped = false;
	_hThread = CreateThread(NULL, 0, ThreadStub, this, 0, NULL);
}

void Desktop::Stop()
{
	_stopped = true;

	if (_hThread != NULL)
		TerminateThread(_hThread, 0);
}

DWORD WINAPI Desktop::ThreadStub(LPVOID lpParam)
{
	while (((Desktop*)lpParam)->_stopped == false)
		((Desktop*)lpParam)->DesktopThread();

	return 0;
}

void Desktop::DesktopThread()
{
	for (int x = 0; x < _blocks.Width; ++x)
	{
		for (int y = 0; y < _blocks.Height; ++y)
		{
			ULONG* cache = &_hashCache[x*_blocks.Height + y];
			SendScreenBlock(Point(x, y), cache, _quality);
		}
	}
}

void Desktop::TakeScreenshot(char** bmp, int* len, Point block, ULONG quality)
{
	HBITMAP hCaptureBitmap = CreateCompatibleBitmap(hDesktopDC, _blockSize.Width, _blockSize.Height);
	SelectObject(hCaptureDC, hCaptureBitmap);
	BitBlt(hCaptureDC, 0, 0, _blockSize.Width, _blockSize.Height, hDesktopDC, _blockSize.Width * block.X, _blockSize.Height * block.Y, SRCCOPY);

	Gdiplus::Bitmap* bitmap = Gdiplus::Bitmap::FromHBITMAP(hCaptureBitmap, NULL);
	LARGE_INTEGER liZero = { 0 };
	ULARGE_INTEGER pos = { 0 };
	STATSTG stg = { 0 };

	IStream* istream;
	CreateStreamOnHGlobal(NULL, TRUE, &istream);

	CLSID encoderClsid;
	if (quality > 100) quality = 100;
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

	*bmp = new char[stg.cbSize.QuadPart];
	istream->Read(*bmp, stg.cbSize.QuadPart, (ULONG*)len);
	istream->Release();

	DeleteObject(hCaptureBitmap);
}



int Desktop::GetEncoderClsid(WCHAR *format, CLSID *pClsid)
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

ULONG Desktop::Hash(const char* input, UINT size)
{
	unsigned long hash = 5381;
	int c;

	for (int i = 0; i < size; c = *input++, i++)
		hash = ((hash << 5) + hash) + c; /* hash * 33 + c */

	return hash;
}

void Desktop::ClearCache()
{
	if (_hashCache == NULL)
		_hashCache = new ULONG[_blocks.Width*_blocks.Height];

	for (int x = 0; x < _blocks.Width; ++x)
	{
		for (int y = 0; y < _blocks.Height; ++y)
		{
			_hashCache[x*_blocks.Height + y] = -1;
		}
	}
}