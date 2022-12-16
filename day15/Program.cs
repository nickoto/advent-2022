using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using day15;
using Spectre.Console;

var part1Expect = 26L;
var part2Expect = 56000011L;

// Sensor at x=2, y=18: closest beacon is at x=-2, y=15

Sensor[] ParseSensors(IEnumerable<string> data)
{
	return data.Select(x => new Sensor(x)).ToArray();
}

List<(int, int)> GetRangesAtLine(Sensor[] sensors, int line)
{
	var ranges = new List<(int, int)>();

	foreach (var sensor in sensors)
	{
		var dist = Math.Abs(sensor.SY - line);
		if (dist > sensor.Dist)
			continue;

		var calc = sensor.Dist - dist;
		ranges.Add((sensor.SX - calc, sensor.SX + calc));
	}

	ranges.Sort((l, r) => l.Item1.CompareTo(r.Item1));

	return ranges;
}

long GetPart1(string[] data)
{
	var checkLine = int.Parse(data[0]);
	var sensors = ParseSensors(data.Skip(1));
	
	var ranges = GetRangesAtLine(sensors, checkLine);

	var min = int.MaxValue / 2;
	var max = int.MinValue / 2;
	var total = 0;
	
	foreach (var range in ranges)
	{
		if (range.Item1 > max + 1)
		{
			var d = Math.Max(max - min, 0);
			if (d > 0) total += d;
			
			min = range.Item1;
			max = range.Item2;
		}
		else
		{
			max = Math.Max(max, range.Item2);
		}
	}

	{
		var d = Math.Max(max - min, 0);
		if (d > 0) total += d;
	}
	
	return total;
}

long GetPart2(string[] data)
{
	var checkLine = int.Parse(data[0]);
	var sensors = ParseSensors(data.Skip(1));
	
	for (int i = 0; i <= checkLine * 2; i++)
	{
		var ranges = GetRangesAtLine(sensors, i);
		var x = MergeRanges(ranges).ToArray();

		if (x.Length > 1)
		{
			Console.WriteLine($"Found? {x[0].Item1} - {x[0].Item2} ~ {x[1].Item1} - {x[1].Item2}");
			return (x[0].Item2 + 1) * 4000000U + i;
		}
	}
		
	return 0L;
	
	IEnumerable<(int, int)> MergeRanges(IEnumerable<(int, int)> ranges)
	{
		var min = int.MaxValue / 2;
		var max = int.MinValue / 2;
		
		foreach (var range in ranges)
		{
			if (range.Item1 > max + 1)
			{
				var d = Math.Max(max - min, 0);
				if (d > 0) yield return (min, max);
			
				min = range.Item1;
				max = range.Item2;
			}
			else
			{
				max = Math.Max(max, range.Item2);
			}
		}

		yield return (min, max);
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

