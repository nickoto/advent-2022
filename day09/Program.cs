using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using Spectre.Console;

var part1Expect = 13L;
var part2Expect = 1L;

long GetTravel(string[] data, int depth)
{
	var pieces = new Point[depth+1];
	var tailHits = new HashSet<Point>();
	var c = 0;
	
	foreach (var line in data)
	{
		var dir = line[0];
		var dist = int.Parse(line.Substring(2));
		
		for (int i = 0; i < dist; i++)
		{
			var head = pieces[0];
			head = dir switch
			{
				'D' => head with {Y = head.Y + 1},
				'U' => head with {Y = head.Y - 1},
				'L' => head with {X = head.X - 1},
				'R' => head with {X = head.X + 1},
			};

			pieces[0] = head;

			for (int d = 0; d < depth; d++)
			{
				head = pieces[d];
				var tail = pieces[d + 1];
				if (Dist(head, tail) > 1)
				{
					var move = Move(head.X - tail.X, head.Y - tail.Y);
					tail = new Point(tail.X + move.X, tail.Y + move.Y);
				}

				pieces[d + 1] = tail;
			}

			tailHits.Add(pieces.Last());
		}
	}

	return tailHits.Count();	
}

long GetPart1(string[] data)
{
	return GetTravel(data, 1);
}

long GetPart2(string[] data)
{
	return GetTravel(data, 9);
}

static int Dist(Point a, Point b)
{
	int Sq(int a) { return a * a; }
	return (int)Math.Sqrt(Sq(b.X - a.X) + Sq(b.Y - a.Y));
}

static (int X, int Y) Move(int dx, int dy)
{
	return (One(dx), One(dy));
	
	int One(int d)
	{
		return Convert.ToByte(d > 0) - Convert.ToByte(d < 0);
	}
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

