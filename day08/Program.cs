using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using day8;
using Spectre.Console;

var part1Expect = 21L;
var part2Expect = 8L;

long GetPart1(string[] data)
{
	var w = data[0].Length;
	var h = data.Length;

	var items = data
		.Select(x => x.Select(y => (byte)(y - '0')))
		.SelectMany(x => x)
		.ToArray();

	var visible = new bool[w * h];
	
	for (var y = 0; y < h; y++)
	{
		var span = items.AsSpan(y * w, w);
		var visSpan = visible.AsSpan(y * w, w);
		
		var left = 0;
		var right = w - 1;
		var lmin = -1;
		var rmin = -1;
		
		while (left < w)
		{
			if (span[left] > lmin) { lmin = span[left]; visSpan[left] = true; }
			if (span[right] > rmin) { rmin = span[right]; visSpan[right] = true; }

			left++;
			right--;
		}
	}

	for (var x = 0; x < w; x++)
	{
		var top = x;
		var bottom = x + w * h - h;
		var tmin = -1;
		var bmin = -1;
		
		while (bottom > 0)
		{
			if (items[top] > tmin) { tmin = items[top]; visible[top] = true; }
			if (items[bottom] > bmin) { bmin = items[bottom]; visible[bottom] = true; }

			top += w;
			bottom -= w;
		}
	}
	
	return visible.Count(x => x);
}

long GetPart2(string[] data)
{
	var w = data[0].Length;
	var h = data.Length;

	var items = data
		.Select(x => x.Select(y => (byte)(y - '0')))
		.SelectMany(x => x)
		.ToArray();

	var score = new long[w * h];

	for (var y = 0; y < h; y++)
	{
		var rowScore = Score(items.Skip(w * y).Take(w), w);
		var rowScore2 = Score(items.Skip(w * y).Take(w).Reverse(), w).Reverse().ToArray();
		var dest = score.AsSpan(w * y, w);
		
		for (int x = 0; x < w; x++)
		{
			dest[x] = rowScore[x] * rowScore2[x];
		}
	}
	
	for (var x = 0; x < w; x++)
	{
		var rowScore = Score(items.Every(x, w), h);
		var rowScore2 = Score(items.Every(x, w).Reverse(), h).Reverse().ToArray();
		for (int y = 0; y < h; y++)
		{
			score[x + y * w] *= rowScore[y] * rowScore2[y];
		}
	}

	return score.Max();

	long[] Score(IEnumerable<byte> input, int len)
	{
		var stack = new Stack<(byte value, int pos)>();
		var result = new long[len];
		
		foreach(var item in input.Select((value, pos) => (value, pos)))
		{
			while (stack.Any() && stack.First().value <= item.value)
			{
				var sitem = stack.Pop();
				result[sitem.pos] = item.pos - sitem.pos;
			}
			
			stack.Push(item);
		}

		while (stack.Any())
		{
			var sitem = stack.Pop();
			result[sitem.pos] = len - sitem.pos - 1;
		}

		return result;
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

