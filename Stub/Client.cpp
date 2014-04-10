#include "Client.h"

Client::Client(const char* szIP, u_short port)
{
	int error;
	if ((error = WSAStartup(MAKEWORD(2, 2), &_localWSAData)) != 0) //Error :(
	{
		Log::LogWin32Error("Client::Client(const char*, unsigned short) WSAStartup", error);
		return;
	}

	_addrIn.sin_family = AF_INET; //IPv4 InterClient
	_addrIn.sin_port = htons(port);
	_addrIn.sin_addr.S_un.S_addr = inet_addr(szIP);

	if ((_socket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP)) == INVALID_SOCKET)
	{
		Log::LogWin32Error("Client::Client(const char*, unsigned short) WSAStartup", WSAGetLastError());
		return;
	}

	Log::LogMethod("Client(const char*, unsigned short)", "Constructed succesfully :D");
}


Client::~Client()
{
	closesocket(_socket);

	if (WSACleanup() == SOCKET_ERROR)
	{
		Log::LogWin32Error("Client::~Client() WSACleanup", WSAGetLastError());
		return;
	}

	Log::LogMethod("Client::~Client()", "Destructed succesfully :D");
}

bool Client::Connect()
{
	if (connect(_socket, (const sockaddr*)&_addrIn, sizeof(_addrIn)) == SOCKET_ERROR)
	{
		int err = WSAGetLastError();
		if (err == 10056)
		{
			if ((_socket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP)) == INVALID_SOCKET)
			{
				Log::LogWin32Error("Client::Client(const char*, unsigned short) WSAStartup", WSAGetLastError());
				return false;
			}

			return Connect();
		}

		std::cout <<  err << std::endl;
		Log::LogWin32Error("Client::Connect() connect", err);
		return false;
	}

	Log::LogMessage("Client::Connect() Connected :D");

	return true;
}

int Client::SendBlocking(const std::string& str)
{
	return SendBlocking(str.c_str(), 0, str.length());
}

int Client::SendBlocking(const char* buf, u_int count)
{
	return SendBlocking(buf, 0, count);
}

int Client::SendBlocking(const char* buf, u_int offset, u_int count)
{
	int sent;
	if ((sent = send(_socket, buf + offset, count, NULL)) == SOCKET_ERROR)
	{
		Log::LogWin32Error("Client::SendBlocking(const char*, u_int, u_int) send", WSAGetLastError());
		while (Connect() == false) Sleep(5);
	}

	return sent;
}

int Client::ReceiveBlocking(std::string* str, u_int count)
{
	char* buffer = new char(count);
	int received = ReceiveBlocking(buffer, 0, count);

	*str = buffer;
	delete[] buffer;

	return received;
}

int Client::ReceiveBlocking(char* buf, u_int count)
{
	return ReceiveBlocking(buf, 0, count);
}

int Client::ReceiveBlocking(char* buf, u_int offset, u_int count)
{
	int received;
	if ((received = recv(_socket, buf + offset, count, NULL)) == SOCKET_ERROR)
	{
		Log::LogWin32Error("Client::ReceiveBlocking(const char*, u_int, u_int) recv", WSAGetLastError());
		while (Connect() == false) Sleep(5);
	}

	return received;
}

void Client::SendPacket(Packet& packet)
{
	((Packet)packet).WritePacket(*this);
	packet.WritePacket(*this);

	std::cout << "Packet: " << std::hex << (int)packet.GetId() << " sent." << std::dec << std::endl;
}

void Client::ReceivePacket(Packet& packet)
{
	packet.ReadPacket(*this);
	std::cout << "Packet: " << std::hex << (int)packet.GetId() << " received." << std::dec << std::endl;
}

void Client::WriteBytes(const char* buf, u_int count)
{
	if (count == 0)
		return;

	u_int sent = 0;
	while ((sent += SendBlocking(buf, sent, count - sent)) < count);
}

void Client::ReadBytes(char* buf, u_int count)
{
	u_int received = 0;
	while ((received += ReceiveBlocking(buf, received, count - received)) < count);
}

void Client::WriteInt(int value)
{
	char buf[4];
	*(int*)buf = value;
	WriteBytes(buf, 4);
}

void Client::ReadInt(int* value)
{
	char buf[4];
	ReadBytes(buf, 4);
	if (value != NULL)
	*value = *(int*)buf;
}

void Client::WriteInt64(INT64 value)
{
	char buf[8];
	*(INT64*)buf = value;
	WriteBytes(buf, 8);
}

void Client::ReadInt64(INT64* value)
{
	char buf[8];
	ReadBytes(buf, 8);
	*value = *(INT64*)buf;
}

void Client::WriteString(const std::string& str)
{
	int length = str.length();
	WriteInt(length);
	WriteBytes(str.c_str(), length);
}

void Client::ReadString(std::string& str)
{
	int length;
	ReadInt(&length);

	char* buf = new char[length + 1];
	buf[length] = 0;
	ReadBytes(buf, length);

	str = std::string(buf);	
	delete[] buf;
}

void Client::WriteMapStringInt64(std::map<std::string, INT64>& stringLongs)
{
	WriteInt(stringLongs.size());
	for (std::map<std::string, INT64>::iterator i = stringLongs.begin(); i != stringLongs.end(); i++)
	{
		WriteString(i->first);
		WriteInt64(i->second);
	}
}

void Client::ReadMapStringInt64(std::map<std::string, INT64>& stringLongs)
{
	int count;
	ReadInt(&count);
	for (int i = 0; i < count; i++)
	{
		std::string path;
		INT64 size;
		ReadString(path);
		ReadInt64(&size);

		stringLongs[path] = size;
	}
}

void Client::WriteSize(const Size& size)
{
	WriteInt(size.Width);
	WriteInt(size.Height);
}

void Client::ReadSize(Size* size)
{
	ReadInt(&size->Width);
	ReadInt(&size->Height);
}

void Client::WritePoint(const Point& point)
{
	WriteInt(point.X);
	WriteInt(point.Y);
}

void Client::ReadPoint(Point* point)
{
	ReadInt(&point->X);
	ReadInt(&point->Y);
}