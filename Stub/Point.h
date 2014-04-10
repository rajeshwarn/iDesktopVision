#pragma once

struct Point
{
	Point()
	{
		X = 0;
		Y = 0;
	}

	Point(int x, int y)
	{
		X = x;
		Y = y;
	}

	int X;
	int Y;
};