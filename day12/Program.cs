using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using day12;
using Spectre.Console;

var part1Expect = 31L;
var part2Expect = 29L;

(char[] Map, int Width, int Height, Point Start, Point End) ParseMap(string[] data)
{
	var map = string.Join("", data).ToArray();
	var width = data[0].Length; 
	var height = data.Length;
	var start = Array.IndexOf(map, 'S');
	var end = Array.IndexOf(map, 'E');


	map[start] = (char)('a');
	map[end] = (char)('z');
	
	return (map, width, height, new Point(start % width, start / width), new Point(end % width, end / width));
}

long GetPart1(string[] data)
{
	var info = ParseMap(data);
	var nodes = info.Map.Select((x, o) => new Node(x, o % info.Width, o / info.Width)).ToArray();
	var toVisit = new List<Node>();
	
	for (int y = 0; y < info.Height; y++)
	{
		for (int x = 0; x < info.Width; x++)
		{
			var node = nodes[x + y * info.Width];
			if (x > 0) AddIf(node, nodes[Offset(x - 1, y)]);
			if (x < info.Width - 1) AddIf(node, nodes[Offset(x + 1, y)]);
			if (y > 0) AddIf(node, nodes[Offset(x, y - 1)]);
			if (y < info.Height - 1) AddIf(node, nodes[Offset(x, y + 1)]);
		}
	}

	void AddIf(Node from, Node to)
	{
		if (to.Height <= from.Height + 1)
		{
			from.Links.Add(to);
		}
	}

	toVisit.Add(nodes[Offset(info.Start.X, info.Start.Y)]);
	toVisit[0].Value = 0;

	while (true)
	{
		if (!toVisit.Any())
			break;
		
		// Pick the shortest one next.
		var cur = toVisit.First();
		toVisit.RemoveAt(0);

		if (cur.Visited) continue;

		if (cur.Value == int.MaxValue)
			throw new InvalidOperationException();
		
		var next = cur.Value + 1;
		cur.Visited = true;
		
		foreach (var node in cur.Links.Where(n => !n.Visited))
		{
			if (node.Value > next)
				node.Value = next;
			
			var index = toVisit.BinarySearch(node);
			if (index < 0) index = ~index;
			toVisit.Insert(index, node);
		}
	}
	
	var end = nodes[Offset(info.End.X, info.End.Y)];
	return end.Value;
	
	int Offset(int x, int y) => x + info.Width * y;
}

long GetPart2(string[] data)
{
	var info = ParseMap(data);
	var nodes = info.Map.Select((x, o) => new Node(x, o % info.Width, o / info.Width)).ToArray();
	var toVisit = new List<Node>();
	
	for (int y = 0; y < info.Height; y++)
	{
		for (int x = 0; x < info.Width; x++)
		{
			var node = nodes[x + y * info.Width];
			if (x > 0) AddIf(node, nodes[Offset(x - 1, y)]);
			if (x < info.Width - 1) AddIf(node, nodes[Offset(x + 1, y)]);
			if (y > 0) AddIf(node, nodes[Offset(x, y - 1)]);
			if (y < info.Height - 1) AddIf(node, nodes[Offset(x, y + 1)]);
		}
	}

	void AddIf(Node from, Node to)
	{
		if (to.Height <= from.Height + 1)
		{
			from.Links.Add(to);
		}
	}

	toVisit.Add(nodes[Offset(info.Start.X, info.Start.Y)]);
	toVisit[0].Value = 0;

	while (true)
	{
		if (!toVisit.Any())
			break;
		
		// Pick the shortest one next.
		var cur = toVisit.First();
		toVisit.RemoveAt(0);

		if (cur.Visited) continue;

		if (cur.Value == int.MaxValue)
			throw new InvalidOperationException();
		
		var next = cur.Value + 1;
		cur.Visited = true;
		
		foreach (var node in cur.Links.Where(n => !n.Visited))
		{
			if (node.Value > next)
				node.Value = next;

			if (node.Height == 'a')
				node.Value = 0;

			var index = toVisit.BinarySearch(node);
			if (index < 0) index = ~index;
			toVisit.Insert(index, node);
		}
	}
	
	var end = nodes[Offset(info.End.X, info.End.Y)];
	return end.Value;
	
	int Offset(int x, int y) => x + info.Width * y;
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

