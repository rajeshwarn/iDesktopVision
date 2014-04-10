#include "Explorer.h"

void Explorer::SendPathContent(const std::string& path)
{
	SendPathContentPacket packet(path, GetPathListing(path));
	(*_client).SendPacket(packet);

	return;
}

INT64 Explorer::GetFileSize(const std::string& path)
{
	WIN32_FILE_ATTRIBUTE_DATA fad;
	if (!GetFileAttributesEx(path.c_str(), GetFileExInfoStandard, &fad))
		return -1; //idc 'bout the error >:(

	if (fad.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
		return 0;

	LARGE_INTEGER size;
	size.HighPart = fad.nFileSizeHigh;
	size.LowPart = fad.nFileSizeLow;
	return size.QuadPart;
}

std::map<std::string, INT64> Explorer::GetPathListing(const std::string& directory)
{
	_currentDirectory = directory;

	WIN32_FIND_DATA ffd;
	HANDLE hFind;
	std::map<std::string, INT64> pathListing;
	std::set<std::string> files;

	std::string copy = directory;
	if (*copy.rbegin() != '\\')
		copy.append("\\");
	if ((hFind = FindFirstFile(copy.append("*").c_str(), &ffd)) == INVALID_HANDLE_VALUE)
	{
		Log::LogWin32Error("GetPathListing() FindFirstFile", GetLastError());
		return pathListing;
	}

	do
	{
		if ((ffd.cFileName[0] == '.' && ffd.cFileName[1] == 0) || (ffd.cFileName[0] == '.' && ffd.cFileName[1] == '.'))
			continue; //idk random lol

		if (ffd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
		{
			pathListing[ffd.cFileName] = -1;
		}
		else
		{
			LARGE_INTEGER size;
			size.HighPart = ffd.nFileSizeHigh;
			size.LowPart = ffd.nFileSizeLow;
			pathListing[ffd.cFileName] = size.QuadPart;
		}
	} while (FindNextFile(hFind, &ffd));

	FindClose(hFind);
	
	return pathListing;
}

void Explorer::SendFile(const std::string& path, UINT bufSize)
{
	INT64 fileSize = GetFileSize(path);
	if (fileSize < 1)
	{ //Directory or error
		if (fileSize == -1)// Error :(
			Log::LogWin32Error("SendFile() GetFileAttributesEx", GetLastError());

		return;
	}

	HANDLE hFile;
	if ((hFile = CreateFile(path.c_str(), GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL)) == INVALID_HANDLE_VALUE)
	{
		Log::LogWin32Error("SendFile() CreateFile", GetLastError());
		return;
	}

	char* buf = new char[bufSize];
	DWORD bytesRead;
	UINT64 totalRead = 0;
	DWORD toRead;
	while ((toRead = fileSize - totalRead) > 0)
	{
		toRead = toRead > bufSize ? bufSize : toRead;
		if (ReadFile(hFile, buf, toRead, &bytesRead, NULL) == false)
		{
			Log::LogWin32Error("SendFile() ReadFile", GetLastError());
			return;
		}

		SendFilePacket packet(buf, totalRead, (UINT64)bytesRead, path);
		(*_client).SendPacket(packet);

		totalRead += bytesRead;
	}
	CloseHandle(hFile);

	SendFilePacket packet(" ", 0, 0, path);
	(*_client).SendPacket(packet);

	delete[] buf;
}

void Explorer::ReceiveFile(SendFilePacket& packet)
{
	std::string path = packet.GetPath();
	path = path.substr(path.find_last_of('\\'));

	std::cout << path;
}