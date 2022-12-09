using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using Spectre.Console;

var part1Expect = 7L;
var part2Expect = 19L;

long GetStartSlow(string data, int length)
{
	var i = -1;
	var positions = data.Select(x => 1u << (x - 'a')).ToArray();
	do
	{
		var bits = positions.Skip(++i).Take(length).Aggregate(0u, (total, next) => total | next);
		var result = BitOperations.PopCount(bits);

		if (result == length) 
			return i + length;
	} while(true);
}

long FindDistinctSpanEnd(string data, int length)
{
	var hits = new bool[256];
	var startPos = 0;
	var endPos = 0;

	// run until the sliding window size is `length` chars.
	while (endPos - startPos < length)
	{
		var endChar = data[endPos];
		if (hits[endChar])
		{
			while (true)
			{
				var startChar = data[startPos];
				startPos++;
				if (startChar != endChar)
				{
					hits[startChar] = false;
				}
				else
				{
					endPos++;
					break;
				}
			}
		}
		else
		{
			hits[endChar] = true;
			endPos++;
		}
	}

	return endPos;
}


long GetPart1(string[] data)
{
	var starts = data.Select(x => FindDistinctSpanEnd(x, 4)).ToArray();
	
	Console.WriteLine(string.Join(", ", starts));
	
	return starts.First();
}

long GetPart2(string[] data)
{
	var starts = data.Select(x => FindDistinctSpanEnd(x, 14)).ToArray();
	
	Console.WriteLine(string.Join(", ", starts));

	return starts.First();
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
