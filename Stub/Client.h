#pragma once
#include "Size.h"
#include <WINSOCK2.H>
#include <STRING>
#include <MAP>
#include "Log.h"
#include "Packet.h"

class Packet;

class Client
{
public:
	Client(const char* szIP, u_short port);
	~Client();
	
	bool Connect();
	int SendBlocking(const std::string& str);
	int SendBlocking(const char* buf, u_int count);
	int SendBlocking(const char* buf, u_int offset, u_int count);

	int ReceiveBlocking(std::string* str, u_int count);
	int ReceiveBlocking(char* buf, u_int count);
	int ReceiveBlocking(char* buf, u_int offset, u_int count);

	void SendPacket(Packet& packet);
	void ReceivePacket(Packet& packet);

	void WriteBytes(const char* buf, u_int count);
	void ReadBytes(char* buf, u_int count);

	void WriteInt(int value);
	void ReadInt(int* value);
	void WriteInt64(INT64 value);
	void ReadInt64(INT64* value);
	void WriteString(const std::string& str);
	void ReadString(std::string& str);
	void WriteMapStringInt64(std::map<std::string, INT64>& stringLongs);
	void ReadMapStringInt64(std::map<std::string, INT64>& stringLongs);
	void WriteSize(const Size& size);
	void ReadSize(Size* size);
	void WritePoint(const Point& point);
	void ReadPoint(Point* point);


private:
	SOCKET			_socket;
	WSAData			_localWSAData;
	SOCKADDR_IN		_addrIn;
	WSAOVERLAPPED	_sendOverlapped;
	WSABUF			_sendWSABuf;
	DWORD			_sendBytes;

	
};

