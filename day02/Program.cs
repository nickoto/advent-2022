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

var part1Expect = 15L;
var part2Expect = 12L;

var data =
	(from line in File.ReadAllLines("input.txt")
	 select line).ToArray();

long Score(long you, long me)
{
	var wld = 2 - ((you - me + 4) % 3);
	return me + 1 + wld * 3;
}

long GetPart1(string[] data)
{
	var result = data
		.Select(x => new {l = x[0] - 'A', r = (x[2] - 'X')})
		.Select(x => Score(x.l, x.r))
		.Sum();
	
	return result;
}

long GetPart2(string[] data)
{
	var result = data
		.Select(x => new {l = x[0] - 'A', r = (x[0] + x[2] - 'X') % 3})
		.Select(x => Score(x.l, x.r))
		.Sum();

	return result;
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
