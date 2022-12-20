using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Spectre.Console;

var part1Expect = 3L;
var part2Expect = 1623178306L;

List<long> Decode(long[] data, int rounds)
{
	var o = data.Select((x, y) => new Point() {Id = y, Value = x});
	var original = o.ToArray();
	var decoded = original.ToList();
	var count = data.Length;

	for (int i=0; i<rounds; i++)
	foreach (var p in original)
	{
		long current = decoded.IndexOf(p);
		decoded.RemoveAt((int)current);

		current += p.Value;
		current %= decoded.Count;
		while (current < 0) current += decoded.Count;
		
		decoded.Insert((int)current, p);
	}

	return decoded.Select(x => x.Value).ToList();
}


long GetPart1(string[] strdata)
{
	var data = strdata.Select(x => long.Parse(x)).ToArray();
	var decoded = Decode(data, 1);

	var zero = decoded.IndexOf(0);

	var one = decoded[(zero + 1000) % data.Length];
	var two = decoded[(zero + 2000) % data.Length];
	var thr = decoded[(zero + 3000) % data.Length];
	
	return one + two + thr;
}

long GetPart2(string[] strdata)
{
	var key = 811589153L;
	var data = strdata.Select(x => long.Parse(x) * key).ToArray();
	var decoded = Decode(data, 10);

	var zero = decoded.IndexOf(0);

	var one = decoded[(zero + 1000) % data.Length];
	var two = decoded[(zero + 2000) % data.Length];
	var thr = decoded[(zero + 3000) % data.Length];
	
	return one + two + thr;
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

[DebuggerDisplay("{Value}")]
class Point
{
	public long Value;
	public int Id;
}
