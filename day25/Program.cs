using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Spectre.Console;

var part1Expect = 4890L;
var part2Expect = 0L;

long GetPart1(string[] data)
{
	var values = data.Select(Parse).ToArray();
	
	var result = values.Sum();
	
	Console.WriteLine(ToSnafu(result));
	return result;

	Int64 Parse(string s)
	{
		Int64 v = 1;
		Int64 r = 0;
		
		foreach (var c in s.Reverse())
		{
			Int64 x = c switch
			{
				'2' => 2,
				'1' => 1,
				'0' => 0,
				'-' => -1,
				'=' => -2,
				_ => throw new InvalidDataException()
			};

			r += x * v;
			v *= 5;
		}

		return r;
	}

	string ToSnafu(Int64 value)
	{
		Stack<char> result = new();
		var carry = 0;
		do
		{
			var j = (value % 5) switch
			{
				0 => '0',
				1 => '1',
				2 => '2',
				3 => '=',
				4 => '-'
			};
			
			result.Push(j);
			carry = (j == '-' || j == '=') ? 1 : 0;
			value = value / 5 + carry;
		} while (value > 0);
		
		return string.Join("", result);
	}
	
}

long GetPart2(string[] data)
{
	return 0L;
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

