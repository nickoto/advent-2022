using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using day10;
using Spectre.Console;

var part1Expect = 13140L;
var part2Expect = 0L;

void Run(CPU cpu, string[] data)
{
	foreach (var act in data.Select(x => x.Split(' ')))
	{
		switch (act[0])
		{
			case "noop":
				cpu.NOP();
				break;
			case "addx":
				cpu.AddX(long.Parse(act[1]));
				break;
			default:
				throw new EvaluateException();
		}
	}
}

long GetPart1(string[] data)
{
	var cpu = new CPU();
	var total = 0L;

	cpu.OnCycle = (cpu) =>
	{
		if (cpu.Cycle % 40 == 20)
		{
			total += cpu.Cycle * cpu.X;
		}
	};

	Run(cpu, data);
	
	return total;
}

long GetPart2(string[] data)
{
	var cpu = new CPU();
	var total = 0L;
	var original = Console.BackgroundColor;
	
	cpu.OnCycle = (cpu) =>
	{
		var pos = (cpu.Cycle - 1) % 40;
		if (pos == 0)
		{
			Console.WriteLine();
		}

		if (Math.Abs(cpu.X - pos) <= 1)
		{
			Console.CursorLeft = (int)pos;
			Console.BackgroundColor = ConsoleColor.Cyan;
			Console.Write(" ");
			Console.BackgroundColor = original;
		}
	};

	Run(cpu, data);
	Console.WriteLine();

	return 0;
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

