using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using Spectre.Console;

var part1Expect = 6032L;
var part2Expect = 5031L;

(byte[,] Map, Dir[] Steps, int Width, int Height) Parse(string[] data)
{
	var height = data.Length - 2;
	var width = data.SkipLast(2).Select(x => x.Length).Max();

	var map = new byte[width, height];
	var regex = new Regex(@"(\d+)(\D?)", RegexOptions.Compiled);
	var stepMatch = regex.Matches(data.Last());
	
	var steps = stepMatch.Select(x =>
	{
		var dist = int.Parse(x.Groups[1].Value);
		var group = x.Groups.Count switch
		{
			3 => x.Groups[2].Value.FirstOrDefault(),
			_ => ' '
		};
		return new Dir(dist, group);
	}).ToArray();

	foreach (var (line, y) in data.SkipLast(2).Select((line, y) => (line, y)))
	{
		foreach (var (c, x) in line.Select((c, x) => (c, x)))
		{
			map[x, y] = c switch
			{
				' ' => 0,
				'.' => 1,
				'#' => 2,
				_ => throw new InvalidDataException()
			};
		}
	}
	
	return (map, steps, width, height);
}

int Wrap(int v, int max) => (v % max + max) % max;

// This is horrible - theres some matrix you could use
(int Face, int Rotation) RotateFromFace(int pf, int dx, int dy)
{
	return (pf, dx, dy) switch
	{
		(0, 0, -1) => (5, 1),
		(0, 1, 0) => (1, 0),
		(0, 0, 1) => (2, 0),
		(0, -1, 0) => (3, 2),
		
		(1, 0, -1) => (5, 0),
		(1, 1, 0) => (4, 2),
		(1, 0, 1) => (2, 1),
		(1, -1, 0) => (0, 0),

		(2, 0, -1) => (0, 0),
		(2, 1, 0) => (1, 3),
		(2, 0, 1) => (4, 0),
		(2, -1, 0) => (3, 3),

		(3, 0, -1) => (2, 1),
		(3, 1, 0) => (4, 0),
		(3, 0, 1) => (5, 0),
		(3, -1, 0) => (0, 2),
		
		(4, 0, -1) => (2, 0),
		(4, 1, 0) => (1, 2),
		(4, 0, 1) => (5, 1),
		(4, -1, 0) => (3, 0),
		
		(5, 0, -1) => (3, 0),
		(5, 1, 0) => (4, 3),
		(5, 0, 1) => (1, 0),
		(5, -1, 0) => (0, 3),
		
		_ => throw new InvalidOperationException()
	};
}

long GetPart1(string[] data)
{
	var (map, steps, width, height) = Parse(data);

	// Figure out the initial position.
	int x = 0, y = 0;

	// Move along.
	while (map[x, y] == 0) x++;

	// Initial direction is facing right
	int dx = 1, dy = 0;
	
	foreach (var step in steps)
	{
		for (int d = 0; d < step.Dist; d++)
		{
			int newx = x;
			int newy = y;

			do
			{
				newx = Wrap(newx + dx, width);
				newy = Wrap(newy + dy, height);

				if (map[newx, newy] == 0)
				{
					continue; // lost in space continue until we wrap around.
				}
				if (map[newx, newy] == 1)
				{
					x = newx;
					y = newy;
					break;
				}
				if (map[newx, newy] == 2)
				{
					break;
				}
			} while (true);
		}

		switch (step.Rotate)
		{
			case 'L':
				(dx, dy) = (dy, dx);
				dy *= -1;
				break;
			case 'R':
				(dx, dy) = (dy, dx);
				dx *= -1;
				break;
		}
	}

	var facing = (dx, dy) switch
	{
		(1, 0) => 0,
		(0, 1) => 1,
		(-1, 0) => 2,
		(0, -1) => 3,
		_ => throw new InvalidOperationException()
	};

	return (y + 1) * 1000 + (x + 1) * 4 + facing;
}

long GetPart2(string[] data)
{
	var (map, steps, width, height) = Parse(data);

	// Figure out the initial position.
	int x = 0, y = 0, face = 0;
	int faceSize = width / 3;

	// Turn the damn thing into a set of faces.
	var faces = Enumerable.Repeat(() => new byte[faceSize, faceSize], 6).Select(x => x()).ToArray();
	var coords = new[] {(faceSize * 1, 0), (faceSize * 2, 0), (faceSize, faceSize), (0, faceSize*2), (faceSize * 1, faceSize * 2), (0, faceSize * 3)};
	for (int i = 0; i < 6; i++)
	{
		var c = coords[i];
		var f = faces[i];
		for (int cy = 0; cy < faceSize; cy++) 
		for (int cx = 0; cx < faceSize; cx++)
		{
			f[cx, cy] = map[cx + c.Item1, cy + c.Item2];
		}
	}
	
	// Initial direction is facing right from the first face.
	int dx = 1, dy = 0;
	int faceMax = faceSize - 1;
	
	foreach (var step in steps)
	{
		for (int d = 0; d < step.Dist; d++)
		{
			int nextX = x + dx;
			int nextY = y + dy;
			int nextDx = dx;
			int nextDy = dy;
			int nextFace = face;

			if (nextX < 0 || nextY < 0 || nextX >= faceSize || nextY >= faceSize)
			{
				var (_nextFace, nextRotation) = RotateFromFace(nextFace, nextDx, nextDy);

				nextFace = _nextFace;
				
				// Rotate several times because it's just easier.
				for (int i = 0; i < nextRotation; i++)
				{
					(nextDx, nextDy) = (nextDy * -1, nextDx);
					(nextX, nextY) = (faceMax - nextY, nextX); 
				}

				Console.Write($"From {x,3}, {y,3} on {face} to {nextX,3}, {nextY,3} on {nextFace} --- ");
				
				nextX = Wrap2(nextX, faceMax);
				nextY = Wrap2(nextY, faceMax);

				Console.WriteLine($"{nextX}, {nextY}");
			}

			int Wrap2(int delta, int max)
			{
				if (delta > max) return 0;
				if (delta < 0) return max;
				return delta;
			}

			if (faces[nextFace][nextX, nextY] == 2)
			{
				break;
			}

			x = nextX;
			y = nextY;
			dx = nextDx;
			dy = nextDy;
			face = nextFace;
		}
		
		switch (step.Rotate)
		{
			case 'L':
				(dx, dy) = (dy, dx);
				dy *= -1;
				break;
			case 'R':
				(dx, dy) = (dy, dx);
				dx *= -1;
				break;
		}
	}

	var facing = (dx, dy) switch
	{
		(1, 0) => 0,
		(0, 1) => 1,
		(-1, 0) => 2,
		(0, -1) => 3,
		_ => throw new InvalidOperationException()
	};

	var system = coords[face];
	
	return (y + system.Item2 + 1) * 1000 + (x + system.Item1 + 1) * 4 + facing;
}


// -----


var stopwatch = Stopwatch.StartNew();
var sampleData = 
	(from line in File.ReadAllLines("sample.txt")
		select line).ToArray();

var inputData =
	(from line in File.ReadAllLines("input.txt")
	 select line).ToArray();

/*
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
*/
stopwatch = Stopwatch.StartNew();
var part2Result = GetPart2(inputData);
AnsiConsole.MarkupLineInterpolated($"[[[aqua]{stopwatch.ElapsedMilliseconds} ms[/]]] Part 2: [aqua]{part2Result}[/]\n");

public record Dir(int Dist, char? Rotate);
