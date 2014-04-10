#include "Log.h"

void Log::LogMessage(const char* message)
{
	LogMessage(message, true);
}

void Log::LogMessage(const char* message, bool newLine)
{
	std::cout << message;
	if (newLine)
		std::cout << std::endl;
}

void Log::LogMessage(std::string str)
{
	LogMessage(str.c_str());
}

void Log::LogMessage(std::string str, bool newLine)
{
	LogMessage(str.c_str(), newLine);
}

void Log::LogWin32Error(int error)
{
	LPTSTR errorText = NULL;

	FormatMessageA( FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_IGNORE_INSERTS,
		NULL,   
		error,
		MAKELANGID(LANG_ENGLISH, SUBLANG_ENGLISH_US),
		(LPTSTR)&errorText,  
		0, 
		NULL);  

	if (errorText != NULL)
	{
		std::stringstream ss;
		ss << " Error: (" << error << ") " << errorText;
		LogMessage(ss.str());

		LocalFree(errorText);
		errorText = NULL;
	}
}

void Log::LogWin32Error(const char* methodName, int error)
{
	LogMessage(methodName, false);
	LogWin32Error(error);
}

void Log::LogMethod(const char* methodName)
{
	LogMessage(methodName);
}

void Log::LogMethod(const char* methodName, const char* extraInfo)
{
	std::stringstream ss;
	ss << methodName << " " << extraInfo;
	LogMessage(ss.str());
}