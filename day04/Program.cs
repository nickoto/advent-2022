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

var part1Expect = 2L;
var part2Expect = 4L;

var inputData =
	(from line in File.ReadAllLines("input.txt")
	 select line).ToArray();

(int, int) AssignmentPair(string assignment)
{
	var temp = assignment.Split("-");
	return (int.Parse(temp[0]), int.Parse(temp[1]));
}

bool PairOverlapsEntirely(string pair)
{
	var groups = pair.Split(",");
	var left = AssignmentPair(groups[0]);
	var right = AssignmentPair(groups[1]);

	return left.Item1 <= right.Item1 && left.Item2 >= right.Item2 ||
	       right.Item1 <= left.Item1 && right.Item2 >= left.Item2;
}

bool PairOverlapsPartially(string pair)
{
	var groups = pair.Split(",");
	var left = AssignmentPair(groups[0]);
	var right = AssignmentPair(groups[1]);

	return
		right.Item1 >= left.Item1 && right.Item1 <= left.Item2 ||
		left.Item1 >= right.Item1 && left.Item1 <= right.Item2;
}

long GetPart1(string[] data)
{
	return data
		.Select(PairOverlapsEntirely)
		.Count(x => x);
}

long GetPart2(string[] data)
{
	return data
		.Select(PairOverlapsPartially)
		.Count(x => x);}

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
var part1Result = GetPart1(inputData);
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
var part2Result = GetPart2(inputData);
AnsiConsole.MarkupLineInterpolated($"[[[aqua]{stopwatch.ElapsedMilliseconds} ms[/]]] Part 2: [aqua]{part2Result}[/]");
