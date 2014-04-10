#include "Packet.h"

const byte Packet::Id							= 0xFF;
const byte RequestInfoPacket::Id				= 0x01;
const byte SendInfoPacket::Id					= 0x02;

const byte AckScreensharePacket::Id				= 0x10;
const byte SckScreensharePacket::Id				= 0x11;
const byte RequestScreenshotBlockPacket::Id		= 0x12;
const byte SendScreenshotBlockPacket::Id		= 0x13;
const byte ClickPacket::Id						= 0x14;

const byte RequestPathContentPacket::Id			= 0x20;
const byte SendPathContentPacket::Id			= 0x21;
const byte RequestFilePacket::Id				= 0x22;
const byte SendFilePacket::Id					= 0x23;

Packet::Packet() 
{ 
	_id = Packet::Id;
	_name = std::string();
	_size = 0;
}

Packet::Packet(byte id)
{
	_id = id;
	_size = 0;
}

void Packet::WritePacket(Client& client)
{
	char buf[5];
	*buf = _id;
	*(u_int*)(buf + 1) = _size;

	client.WriteBytes(buf, 5);
}

void Packet::ReadPacket(Client& client)
{
	char buf[5];
	client.ReadBytes(buf, 5);

	_id = *buf;
	_size = *(u_int*)(buf + 1);
}



void SendInfoPacket::WritePacket(Client& client)
{
	client.WriteString(_computerName);
	client.WriteString(_userName);
	client.WriteString(_os);
}

void RequestPathContentPacket::ReadPacket(Client& client)
{
	client.ReadString(_path);
}

void SendPathContentPacket::WritePacket(Client& client)
{
	client.WriteString(_path);
	client.WriteMapStringInt64(_listing);
}

void RequestFilePacket::ReadPacket(Client& client)
{
	client.ReadString(_path);
}

void SendFilePacket::WritePacket(Client& client)
{
	client.WriteInt64(_count);
	client.WriteInt64(_offset);
	client.WriteInt(_count);
	client.WriteBytes(_data, _count);
	client.WriteString(_path);
}

void SendFilePacket::ReadPacket(Client& client)
{
	client.ReadInt64((INT64*)&_count);
	client.ReadInt64((INT64*)&_offset);

	int length;
	client.ReadInt(&length);
	client.ReadBytes(_data, length);
	client.ReadString(_path);
}

void SckScreensharePacket::WritePacket(Client& client) { }

void SckScreensharePacket::ReadPacket(Client& client)
{
	client.ReadSize(&_size);
	client.ReadInt((int*)&_quality);
}

void AckScreensharePacket::WritePacket(Client& client)
{
	client.WriteSize(_blockSize);
	client.WriteSize(_blocks);
}

void AckScreensharePacket::ReadPacket(Client& client)
{

}

void RequestScreenshotBlockPacket::ReadPacket(Client& client)
{
	client.ReadPoint(&_block);
	client.ReadInt((int*)&_hash);
	client.ReadInt((int*)&_quality);
}

void SendScreenshotBlockPacket::WritePacket(Client& client)
{
	client.WritePoint(_block);
	client.WriteInt(_len);
	client.WriteBytes(_bytes, _len);
	client.WriteInt(_hash);
	
	char buf[1];
	buf[0] = _compressed;
	client.WriteBytes(buf, 1);
	client.WriteInt(_size);
}

void ClickPacket::WritePacket(Client& client) { }

void ClickPacket::ReadPacket(Client& client)
{
	client.ReadPoint(&_point);

	int i;
	client.ReadInt(&i);
	_buttons = (MouseButtons)i;

	char buf[1];
	client.ReadBytes(buf, 1);
	_pressEvent = (PressEvent)buf[0];
}