#pragma once
#include <Winsock2.h> //or the compiler bitches
#include "Util.h"
#include "Client.h"
#include "Explorer.h"
#include "Desktop.h"
#include <STRING>
#include <IOSTREAM> 

class Slave
{
public:
	Slave(std::string ip, USHORT port);
	void PerformPacketLoop();

private:
	Client _client;
	Explorer _explorer;
	Desktop _desktop;
};