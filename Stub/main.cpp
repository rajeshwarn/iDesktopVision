#include "Slave.h"
#pragma comment(lib, "ws2_32.lib")

int main(int args, char* argv[])
{
	Slave slave("82.72.201.150", 2345);
	slave.PerformPacketLoop();

	getchar();
	return 0;
}