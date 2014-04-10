#pragma once
#include <IOSTREAM>
#include <OSTREAM>
#include <WINDOWS.H>
#include <STRING>
#include <SSTREAM>

class Log
{
public:
	static void LogMessage(const char* message);
	static void LogMessage(const char*, bool newLine);
	static void LogMessage(std::string str);
	static void LogMessage(std::string str, bool newLine);
	static void LogWin32Error(int error);
	static void LogWin32Error(const char* methodName, int error);
	static void LogMethod(const char* methodName);
	static void LogMethod(const char* methodName, const char* extraInfo);
};