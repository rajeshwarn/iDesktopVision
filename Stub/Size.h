#pragma once

struct Size
{
	Size()
	{
		Width = 0;
		Height = 0;
	}

	Size(int width, int height)
	{
		Width = width;
		Height = height;
	}

	int Width;
	int Height;
};