using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Spectre.Console;

var stopwatch = Stopwatch.StartNew();

var testData = 
	(from line in File.ReadAllLines("sample.txt")
		select line).ToArray();

var part1Expect = 24000L;
var part2Expect = 45000L;

var data =
	(from line in File.ReadAllLines("input.txt")
	 select line).ToArray();


long GetPart1(string[] data)
{
	var max = 0L;
	var total = 0L;
	
	foreach(var line in data)
	{
		if (string.IsNullOrWhiteSpace(line))
		{ 
			total = 0;
			continue;
		}

		total += long.Parse(line);
		if (total > max)
		{
			max = total;
		}
	}

	return max;
}

long GetPart2(string[] data)
{
	var total = 0L;
	var totals = new List<long>();
	
	foreach(var line in data)
	{
		if (string.IsNullOrWhiteSpace(line))
		{ 
			totals.Add(total);
			total = 0;
			continue;
		}

		total += long.Parse(line);
	}
	
	totals.Add(total);

	totals.Sort();

	return totals.TakeLast(3).Sum();
}

AnsiConsole.MarkupLineInterpolated($"[[[aqua]{stopwatch.ElapsedMilliseconds} ms[/]]] Pre-compute\n");


stopwatch = Stopwatch.StartNew();
var part1TestResult = GetPart1(testData);
AnsiConsole.MarkupLineInterpolated($"[[[aqua]{stopwatch.ElapsedMilliseconds} ms[/]]] Part 1 (Test): [aqua]{part1TestResult}[/]");
if (part1TestResult != part1Expect)
{
	AnsiConsole.MarkupLineInterpolated($"[[[red]Failed[/]]] Result: [aqua]{part1TestResult}[/]  Expected: [aqua]{part1Expect}[/]");
	return;
}

AnsiConsole.MarkupLineInterpolated($"[[[green]Passed[/]]] Result: [aqua]{part1TestResult}[/]\n");


stopwatch = Stopwatch.StartNew();
var part1Result = GetPart1(data);
AnsiConsole.MarkupLineInterpolated($"[[[aqua]{stopwatch.ElapsedMilliseconds} ms[/]]] Part 1: [aqua]{part1Result}[/]\n");


stopwatch = Stopwatch.StartNew();
var part2TestResult = GetPart2(testData);
AnsiConsole.MarkupLineInterpolated($"[[[aqua]{stopwatch.ElapsedMilliseconds} ms[/]]] Part 2 (Test): [aqua]{part2TestResult}[/]");
if (part2TestResult != part2Expect)
{
	AnsiConsole.MarkupLineInterpolated($"[[[red]Failed[/]]] Result: [aqua]{part2TestResult}[/]  Expected: [aqua]{part2Expect}[/]");
	return;
}

AnsiConsole.MarkupLineInterpolated($"[[[green]Passed[/]]] Result: [aqua]{part2TestResult}[/]\n");


stopwatch = Stopwatch.StartNew();
var part2Result = GetPart2(data);
AnsiConsole.MarkupLineInterpolated($"[[[aqua]{stopwatch.ElapsedMilliseconds} ms[/]]] Part 2: [aqua]{part2Result}[/]");
