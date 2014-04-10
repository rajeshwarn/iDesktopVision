#pragma once
#include <WINDOWS.H>
#include <STRING>
#include <SSTREAM>
#include "Log.h"

class Util
{
public:
	static std::string GetComputername();
	static std::string GetUsername();
	static std::string GetOSVersion();

private:
	static const int _bufSize;
};