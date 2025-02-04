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

var part1Expect = 157L;
var part2Expect = 70L;

var data =
	(from line in File.ReadAllLines("input.txt")
	 select line).ToArray();


long GetPart1(string[] data)
{
	var priority = data
		.Select(x => x.Take(x.Length / 2).Intersect(x.TakeLast(x.Length / 2)).ToList())
		.SelectMany(x => x)
		.Select(x => x switch
		{
			(>= 'a') and (<= 'z') => x - 'a' + 1,
			_ => x - 'A' + 27
		});
	
	return priority.Sum();
}

long GetPart2(string[] data)
{
	IEnumerable<char> GetPriority()
	{
		var pos = 0;
		while (pos < data.Length)
		{
			yield return data[pos].Intersect(data[pos + 1]).Intersect(data[pos + 2]).First();
			pos += 3;
		}
	}
	
	var total = GetPriority().Select(x => x switch
	{
		(>= 'a') and (<= 'z') => x - 'a' + 1,
		_ => x - 'A' + 27
	});
	
	return total.Sum();
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
