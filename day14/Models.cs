using System.Diagnostics;
using System.Linq;

namespace day14;

[DebuggerDisplay("({X},{Y})")]
public struct Point
{
	public int X;
	public int Y;

	public Point(int x, int y)
	{
		X = x;
		Y = y;
	}
	
	public Point(string csv)
	{
		var temp = csv.Split(",");
		X = int.Parse(temp[0]);
		Y = int.Parse(temp[1]);
	}
}

public struct Line
{
	public Point[] Points;

	public Line(Point[] points)
	{
		Points = points;
	}
}
