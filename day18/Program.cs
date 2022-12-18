using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Spectre.Console;

var part1Expect = 64L;
var part2Expect = 58L;

Point[] Parse(string[] data)
{
	return data.Select(x =>
	{
		var p = x.Split(",").Select(int.Parse).ToList();
		return new Point(p[0], p[1], p[2]);
	}).ToArray();
}

void UpdateAdjacency(int maxX, int maxY, int maxZ, int[,,] grid)
{
	var ix = maxX - 1;
	var iy = maxY - 1;
	var iz = maxZ - 1;
	
	for (int x = 0; x < maxX; x++)
	for (int y = 0; y < maxY; y++)
	for (int z = 0; z < maxZ; z++)
	{
		var value = grid[x, y, z] & 0xffff;
		
		if (value == 0)
			continue;
		
		if (x > 0 && grid[x - 1, y, z] != 0) value--;
		if (y > 0 && grid[x, y - 1, z] != 0) value--;
		if (z > 0 && grid[x, y, z - 1] != 0) value--;
		if (x < ix && grid[x + 1, y, z] != 0) value--;
		if (y < iy && grid[x, y + 1, z] != 0) value--;
		if (z < iz && grid[x, y, z + 1] != 0) value--;

		grid[x, y, z] = value;
	}
}

void Dump(int maxX, int maxY, int maxZ, int[,,] grid)
{
	for (int z = 0; z < maxZ; z++)
	{
		for (int y = 0; y < maxY; y++)
		{
			for (int x = 0; x < maxX; x++)
			{
				Console.Write($"{grid[x,y,z],6}");
			}
			Console.WriteLine();
		}
		Console.WriteLine();
	}
}

void FloodFillInner(int maxX, int maxY, int maxZ, int[,,] grid)
{
	var queue = new Queue<(int X, int Y, int Z)>();
	queue.Enqueue((0, 0, 0));

	var ix = maxX - 1;
	var iy = maxY - 1;
	var iz = maxZ - 1;

	while (queue.TryDequeue(out var p))
	{
		if (grid[p.X, p.Y, p.Z] != 0) 
			continue;
		
		grid[p.X, p.Y, p.Z] = 0x10000;
		
		if (p.X > 0 && grid[p.X - 1, p.Y, p.Z] == 0) queue.Enqueue((p.X - 1, p.Y, p.Z));
		if (p.Y > 0 && grid[p.X, p.Y - 1, p.Z] == 0) queue.Enqueue((p.X, p.Y - 1, p.Z));
		if (p.Z > 0 && grid[p.X, p.Y, p.Z - 1] == 0) queue.Enqueue((p.X, p.Y, p.Z - 1));
		if (p.X < ix && grid[p.X + 1, p.Y, p.Z] == 0) queue.Enqueue((p.X + 1, p.Y, p.Z));
		if (p.Y < iy && grid[p.X, p.Y + 1, p.Z] == 0) queue.Enqueue((p.X, p.Y + 1, p.Z));
		if (p.Z < iz && grid[p.X, p.Y, p.Z + 1] == 0) queue.Enqueue((p.X, p.Y, p.Z + 1));
	}
	
	// Now invert it so the insides of everything are flood filled.
	for (int x = 0; x < maxX; x++)
	for (int y = 0; y < maxY; y++)
	for (int z = 0; z < maxZ; z++)
	{
		grid[x, y, z] ^= 0x10000;
	}
}

long GetPart1(string[] data)
{
	var points = Parse(data);
	int minX = int.MaxValue, minY = int.MaxValue, minZ = int.MaxValue;
	int maxX = int.MinValue, maxY = int.MinValue, maxZ = int.MinValue;

	foreach (var point in points)
	{
		minX = Math.Min(point.X, minX); minY = Math.Min(point.Y, minY); minZ = Math.Min(point.Z, minZ);
		maxX = Math.Max(point.X, maxX); maxY = Math.Max(point.Y, maxY); maxZ = Math.Max(point.Z, maxZ);
	}

	maxX += 2; maxY += 2; maxZ += 2;
	
	AnsiConsole.MarkupLine($"Grid Size = [cyan]{maxX} x {maxY} x {maxZ}[/]");
	var grid = new int[maxX,maxY,maxZ];
	foreach (var point in points) { grid[point.X, point.Y, point.Z] = 0x8006; }

	UpdateAdjacency(maxX, maxY, maxZ, grid);

	return grid.Cast<int>().Select(x => x & 0xf).Sum();
}

long GetPart2(string[] data)
{
	var points = Parse(data);
	int minX = int.MaxValue, minY = int.MaxValue, minZ = int.MaxValue;
	int maxX = int.MinValue, maxY = int.MinValue, maxZ = int.MinValue;

	foreach (var point in points)
	{
		minX = Math.Min(point.X, minX); minY = Math.Min(point.Y, minY); minZ = Math.Min(point.Z, minZ);
		maxX = Math.Max(point.X, maxX); maxY = Math.Max(point.Y, maxY); maxZ = Math.Max(point.Z, maxZ);
	}

	maxX += 2; maxY += 2; maxZ += 2;
	
	AnsiConsole.MarkupLine($"Grid Size = [cyan]{maxX} x {maxY} x {maxZ}[/]");
	var grid = new int[maxX,maxY,maxZ];
	foreach (var point in points) { grid[point.X, point.Y, point.Z] = 0x8006; }

	FloodFillInner(maxX, maxY, maxZ, grid);
	UpdateAdjacency(maxX, maxY, maxZ, grid);
	
	return grid.Cast<int>().Select(x => x & 0xf).Sum();
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

//----------

public record Point(int X, int Y, int Z);

