using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace day12;

public struct Point
{
	public int X, Y;

	public Point(int x, int y)
	{
		Y = y;
		X = x;
	}
}

[DebuggerDisplay("[{Height}] ({X}, {Y}) = {Value}")]
public class Node : IComparable
{
	public char Height;
	public int Value = int.MaxValue;
	public bool Visited = false;
	public List<Node> Links = new List<Node>();
	public int X;
	public int Y;

	public Node(char height, int x, int y)
	{
		Height = height;
		X = x;
		Y = y;
	}

	public int CompareTo(object? obj)
	{
		return Value.CompareTo(((Node)obj).Value);
	}
}
