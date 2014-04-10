#pragma once
#include <MAP>
#include <SET>
#include "Client.h"

class Explorer
{
public:
	Explorer(Client* client) : _client(client) {}

	void SendPathContent(const std::string& path);
	void SendFile(const std::string& path, UINT bufSize);
	void ReceiveFile(SendFilePacket& packet);

private:
	Client* _client;

	INT64 GetFileSize(const std::string& path);
	std::map<std::string, INT64> GetPathListing(const std::string& directory);
	
	std::string _currentDirectory;
};