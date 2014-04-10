#include "Util.h"

const int Util::_bufSize = 32767;

std::string Util::GetComputername()
{
	char buffer[_bufSize];
	DWORD size = _bufSize;
	if (GetComputerName(buffer, &size) == FALSE)
	{
		Log::LogWin32Error("Util::GetComputername()", GetLastError());
		return NULL;
	}
	
	return std::string(buffer);
}

std::string Util::GetUsername()
{
	char buffer[_bufSize];
	DWORD size = _bufSize;
	if (GetUserName(buffer, &size) == FALSE)
	{
		Log::LogWin32Error("Util::GetUsername()", GetLastError());
		return NULL;
	}

	return std::string(buffer);
}

std::string Util::GetOSVersion()
{
	typedef void (WINAPI *PGNSI)(LPSYSTEM_INFO);
	typedef bool (WINAPI *PGPI)(DWORD, DWORD, DWORD, DWORD, PDWORD);

	std::stringstream sstream;

	OSVERSIONINFOEX osvi = {0};
	SYSTEM_INFO si = {0};
	PGNSI pGNSI;
	PGPI pGPI;
	DWORD dwType;

	osvi.dwOSVersionInfoSize = sizeof(OSVERSIONINFOEX);

	if (GetVersionEx((OSVERSIONINFO*)&osvi) == FALSE)
		return NULL;

	pGNSI = (PGNSI)GetProcAddress(GetModuleHandle(TEXT("kernel32.dll")), "GetNativeSystemInfo");
	if (pGNSI == NULL)
		GetSystemInfo(&si);
	else
		pGNSI(&si);

	sstream << "Microsoft ";

	if (osvi.dwMajorVersion == 6)
	{
		if (osvi.dwMinorVersion == 0)
		{
			sstream << "Windows Vista ";
		}
		else if (osvi.dwMinorVersion == 1)
		{
			sstream << "Windows 7 ";
		}
		else if (osvi.dwMinorVersion == 2)
		{
			sstream << "Windows 8 ";
		}

		pGPI = (PGPI)GetProcAddress(GetModuleHandle(TEXT("kernel32.dll")), "GetProductInfo");
		pGPI(osvi.dwMajorVersion, osvi.dwMinorVersion, 0, 0, &dwType);

		switch (dwType)
		{
		case 0x00000001:
			sstream << "Ultimate Edition";
			break;
		case 0x00000030:
			sstream << "Professional Edition";
			break;
		case 0x00000003:
			sstream << "Home Premium Edition";
			break;
		case 0x00000002:
			sstream << "Home Basic Edition";
			break;
		case 0x00000004:
			sstream << "Enterprise Edition";
			break;
		case 0x00000006:
			sstream << "Business Edition";
			break;
		case 0x0000000B:
			sstream << "Starter Edition";
			break;
		default:
			sstream << "Unknown Edition";
		}
	}

	if (osvi.dwMajorVersion == 5)
	{
		sstream << "Windows XP";
		if (osvi.dwMinorVersion == 2)
			sstream << " Professional x64 Edition";
	}

	if (strlen(osvi.szCSDVersion) > 0)
	{
		sstream << " " << osvi.szCSDVersion;
	}

	sstream << " (build " << osvi.dwBuildNumber << ")";

	if (osvi.dwMajorVersion >= 6)
	{
		sstream << (si.wProcessorArchitecture == 9 ? ", 64-bit" : si.wProcessorArchitecture == 0 ? ", 86-bit" : ", Unkown Architecture");
	}

	return sstream.str();
}