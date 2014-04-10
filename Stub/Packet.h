#pragma once
#include "Point.h"
#include "Client.h"
#include "Size.h"
#include <MAP>
#include "MouseButtons.h"
#include "PressEvent.h"

class Client;
class Packet
{
public:
	static const byte Id;
	
	Packet();
	Packet(byte id);
	virtual ~Packet() { }

	inline byte GetId() { return _id; }
	inline u_int GetSize() { return _size; }
	inline void SetSize(u_int size) { _size = size; }

	virtual void WritePacket(Client& client);
	virtual void ReadPacket(Client& client);
		
private:
	byte _id;
	std::string _name;
	u_int _size;
};

class RequestInfoPacket : public Packet
{
public:
	static const byte Id;

	RequestInfoPacket() : Packet(Id) {}
};

class SendInfoPacket : public Packet
{
public:
	static const byte Id;

	SendInfoPacket() : Packet(Id) {}
	SendInfoPacket(const std::string& computerName, const std::string& userName, const std::string& os) : Packet(Id)
	{
		_computerName = computerName;
		_userName = userName;
		_os = os;

		SetSize(computerName.length() + userName.length() + os.length() + 3 * sizeof(u_int));
	}

	virtual void WritePacket(Client& client);

private:
	std::string _computerName;
	std::string _userName;
	std::string _os;
};


class AckScreensharePacket : public Packet
{
public:
	static const byte Id;

	AckScreensharePacket(Size& blockSize, Size& blocks) : Packet(AckScreensharePacket::Id) 
	{ 
		_blockSize = blockSize;
		_blocks = blocks;

		SetSize(4 * 4);
	}

	virtual void WritePacket(Client& client);
	virtual void ReadPacket(Client& client);

private:
	Size _blockSize;
	Size _blocks;
};

class SckScreensharePacket : public Packet
{
public:
	static const byte Id;
	
	SckScreensharePacket() : Packet(Id) { }
	inline Size GetSize() { return _size; }
	inline ULONG GetQuality() { return _quality; }

	virtual void WritePacket(Client& client);
	virtual void ReadPacket(Client& client);

private:
	Size _size;
	ULONG _quality;
};

class RequestScreenshotBlockPacket : public Packet
{
public:
	static const byte Id;

	RequestScreenshotBlockPacket() : Packet (Id) {}

	inline Point GetBlock() { return _block; }
	inline ULONG GetHash() { return _hash; }
	inline ULONG GetQuality() { return _quality; }

	virtual void ReadPacket(Client& client);

private:
	Point _block;
	ULONG _hash;
	ULONG _quality;
};

class SendScreenshotBlockPacket : public Packet
{
public:
	static const byte Id;

	SendScreenshotBlockPacket() : Packet(Id) {}
	SendScreenshotBlockPacket(const Point& block, const char* bytes, u_int len, ULONG hash, bool compressed = false, int size = 0) : Packet(Id)
	{
		_block = block;
		_bytes = bytes;
		_len = len;
		_hash = hash;
		_compressed = compressed;
		_size = size;

		SetSize(len + 32);
	}

	virtual void WritePacket(Client& client);

private:
	Point _block;
	const char* _bytes;
	u_int _len;
	ULONG _hash;
	bool _compressed;
	int _size;
};

class ClickPacket : public Packet
{
public:
	static const byte Id;

	ClickPacket() : Packet(Id) {}

	inline Point GetPoint() { return _point; }
	inline MouseButtons GetButtons() { return _buttons; }
	inline PressEvent GetEvent() { return _pressEvent; }

	virtual void ReadPacket(Client& client);
	virtual void WritePacket(Client& client);

private:
	Point _point;
	MouseButtons _buttons;
	PressEvent _pressEvent;
};

class RequestPathContentPacket : public Packet
{
public:
	static const byte Id;

	RequestPathContentPacket() : Packet(Id) {}
	RequestPathContentPacket(std::string path) : Packet(Id), _path(path) { SetSize(path.length() + sizeof(int)); }

	inline std::string GetPath() { return _path; }

	virtual void ReadPacket(Client& client);

private:
	std::string _path;
};

class SendPathContentPacket : public Packet
{
public:
	static const byte Id;

	SendPathContentPacket() : Packet(Id) {}
	SendPathContentPacket(std::string path, std::map<std::string, INT64> listing) : Packet(Id), _path(path), _listing(listing)
	{
		u_int size = path.length() + sizeof(int);
		for (std::map<std::string, INT64>::iterator i = listing.begin(); i != listing.end(); i++)
			size += i->first.size() + 8;

		SetSize(size);
	}

	inline std::string GetPath() { return _path; }
	inline std::map<std::string, INT64> GetListing() { return _listing; }

	virtual void WritePacket(Client& client);

private:
	std::string _path;
	std::map<std::string, INT64> _listing;
};

class RequestFilePacket : public Packet
{
public:
	static const byte Id;

	RequestFilePacket() : Packet(Id) {}
	inline std::string GetPath() { return _path;  }

	virtual void ReadPacket(Client& client);

private:
	std::string _path;
};

class SendFilePacket : public Packet
{
public:
	static const byte Id;

	SendFilePacket() : Packet(Id){}
	SendFilePacket(char* data, UINT64 offset, UINT64 count, const std::string& path) : Packet(Id)
	{
		_data = data;
		_offset = offset;
		_count = count;
		_path = path;

		unsigned int size = 4 * 2 + 2 * 8 + count + path.length();
		SetSize(size);
	}

	inline char* GetData() { return _data; }
	inline UINT64 GetOffset() { return _offset; }
	inline UINT64 GetCount() { return _count; }
	inline std::string GetPath() { return _path; }

	virtual void WritePacket(Client& client);
	virtual void ReadPacket(Client& client);

private:
	char* _data;
	UINT64 _offset;
	UINT64 _count;
	std::string _path;
};