using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using day14;
using Spectre.Console;

var part1Expect = 24L;
var part2Expect = 93L;

Line[] ParseInput(string[] data)
{
	return data
		.Select(x => x.Split(" -> ").Select(x => new Point(x)).ToArray())
		.Select(x => new Line(x))
		.ToArray();
}

void Draw(byte[] map, int width, int x1, int y1, int x2, int y2)
{
	if (x1 == x2)
	{
		if (y1 > y2) (y1, y2) = (y2, y1);
		for (int y = y1; y <= y2; y++)
		{
			map[width * y + x1] = 1;
		}
	}
	else
	{
		if (x1 > x2) (x1, x2) = (x2, x1);
		int y = y1 * width;
		for (int x = x1; x <= x2; x++)
		{
			map[y + x] = 1;
		}
	}
}

long Simulate(string[] data, bool hasFloor)
{
	var lines = ParseInput(data);
	var points = lines.Select(x => x.Points).SelectMany(x => x);
	var minx = int.MaxValue;
	var maxx = int.MinValue;
	var miny = int.MaxValue;
	var maxy = int.MinValue;
	foreach (var point in points)
	{
		minx = Math.Min(point.X, minx);
		miny = Math.Min(point.Y, miny);
		maxx = Math.Max(point.X, maxx);
		maxy = Math.Max(point.Y, maxy);
	}

	// Add a buffer
	maxx += (maxy - miny) * 3; minx -= (maxy - miny) * 3;
	maxy += 3; miny = 0;
	
	int width = maxx - minx;
	int height = maxy - miny;

	var map = new byte[width * height];

	foreach (var line in lines)
	{
		foreach (var pair in line.Points.Zip(line.Points.Skip(1), (p1, p2) => (p1, p2)))
		{
			Draw(map, width, pair.p1.X - minx, pair.p1.Y - miny, pair.p2.X - minx, pair.p2.Y - miny);
		}
	}

	if (hasFloor)
	{
		Draw(map, width, 0, height - 1, width - 1, height - 1);
	}
	
	// run the simulation.
	var dropPoint = 500 - minx;
	var stopped = 0;
	var visits = new Stack<Point>();
	visits.Push(new Point(dropPoint, 0));
	
	while (visits.Any())
	{
		var last = visits.Peek();
		var next = last with {Y = last.Y + 1};

		if (next.Y >= height)
		{
			break;
		}
		
		if (Map(next.X, next.Y) == 0)
		{
			visits.Push(next);
		}
		else if (Map(next.X - 1, next.Y) == 0)
		{
			visits.Push(new Point(next.X - 1, next.Y));
		}
		else if (Map(next.X + 1, next.Y) == 0)
		{
			visits.Push(new Point(next.X + 1, next.Y));
		}
		else
		{
			stopped++;
			map[last.X + last.Y * width] = 2;
			visits.Pop();
		}
	}

	byte Map(int x, int y)
	{
		return map[x + y * width];
	}

	void DumpMap()
	{
		Console.SetCursorPosition(0, 1);
		var c = 0;
		foreach (var b in map)
		{
			if ((c++) % width == 0)
			{
				Console.WriteLine();			
			}
		
			Console.Write(b switch { 0 => '.', 1 => '#', 2 => 'o' });
		}
	}
	
	
	return stopped;
}

long GetPart1(string[] data)
{
	return Simulate(data, false);
}

long GetPart2(string[] data)
{
	return Simulate(data, true);
}


// -----


var stopwatch = Stopwatch.StartNew();
var sampleData = 
	(from line in File.ReadAllLines("sample.txt")
		select line).ToArray();

var inputData =
	(from line in File.ReadAllLines("input.txt")
	 select line).ToArray();

AnsiConsole.MarkupLineInterpolated($"[[[aqua]{stopwatch.ElapsedMilliseconds} ms[/]]] Pre-compute\n");


stopwatch = Stopwatch.StartNew();
var part1TestResult = GetPart1(sampleData);
AnsiConsole.MarkupLineInterpolated($"[[[aqua]{stopwatch.ElapsedMilliseconds} ms[/]]] Part 1 (Test): [aqua]{part1TestResult}[/]");
if (part1TestResult != part1Expect)
{
	AnsiConsole.MarkupLineInterpolated($"[[[red]Failed[/]]] Result: [aqua]{part1TestResult}[/]  Expected: [aqua]{part1Expect}[/]");
	return;
}

AnsiConsole.MarkupLineInterpolated($"[[[green]Passed[/]]] Result: [aqua]{part1TestResult}[/]\n");


stopwatch = Stopwatch.StartNew();
var part1Result = GetPart1(inputData);
AnsiConsole.MarkupLineInterpolated($"[[[aqua]{stopwatch.ElapsedMilliseconds} ms[/]]] Part 1: [aqua]{part1Result}[/]\n");


stopwatch = Stopwatch.StartNew();
var part2TestResult = GetPart2(sampleData);
AnsiConsole.MarkupLineInterpolated($"[[[aqua]{stopwatch.ElapsedMilliseconds} ms[/]]] Part 2 (Test): [aqua]{part2TestResult}[/]");
if (part2TestResult != part2Expect)
{
	AnsiConsole.MarkupLineInterpolated($"[[[red]Failed[/]]] Result: [aqua]{part2TestResult}[/]  Expected: [aqua]{part2Expect}[/]");
	return;
}

AnsiConsole.MarkupLineInterpolated($"[[[green]Passed[/]]] Result: [aqua]{part2TestResult}[/]\n");

stopwatch = Stopwatch.StartNew();
var part2Result = GetPart2(inputData);
AnsiConsole.MarkupLineInterpolated($"[[[aqua]{stopwatch.ElapsedMilliseconds} ms[/]]] Part 2: [aqua]{part2Result}[/]\n");

