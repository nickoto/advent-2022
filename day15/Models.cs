using System;

namespace day15;

public struct Sensor
{
	public Sensor(string s)
	{
		var temp = s.Split(' ');
		SX = int.Parse(temp[2].Split('=')[1][..^1]);
		SY = int.Parse(temp[3].Split('=')[1][..^1]);
		BX = int.Parse(temp[8].Split('=')[1][..^1]);
		BY = int.Parse(temp[9].Split('=')[1]);
		Dist = Math.Abs(SX - BX) + Math.Abs(SY - BY);
	}
	
	// Sensor at x=2, y=18: closest beacon is at x=-2, y=15
	public readonly int SX;
	public readonly int SY;
	public readonly int BX;
	public readonly int BY;
	public readonly int Dist;
}
