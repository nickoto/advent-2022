using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using day11;
using Spectre.Console;

var part1Expect = 10605L;
var part2Expect = 2713310158L;

IEnumerable<Monkey> ParseMonkeys(string[] data)
{
	var iterator = data.AsEnumerable().GetEnumerator();
	while (iterator.MoveNext())
	{
		yield return Monkey.Parse(iterator);
	}
}

long GetPart1(string[] data)
{
	var monkeys = ParseMonkeys(data).ToArray();

	for (int iteration = 0; iteration < 20; iteration++)
	{
		Run();
	}
	
	var topMonkeys = monkeys.OrderByDescending(monkey => monkey.Inspections).Take(2).ToArray();

	return (topMonkeys[0].Inspections * topMonkeys[1].Inspections);

	void Run()
	{
		foreach (var monkey in monkeys)
		{
			monkey.Inspections += monkey.Items.Count;
			var items = monkey.Items;
			monkey.Items = new List<Int128>();
			
			foreach (var worry in items)
			{
				var newWorry = monkey.Operation(worry) / 3;
				var newMonkey = (newWorry % monkey.DivisibleBy == 0) ? monkey.ThrowTrue : monkey.ThrowFalse;
				
				monkeys[newMonkey].Items.Add(newWorry);
			}
		}
	}
}

long GetPart2(string[] data)
{
	var monkeys = ParseMonkeys(data).ToArray();
	Int128 gcd = 1;
	foreach (var m in monkeys)
	{
		gcd *= m.DivisibleBy;
	}
	
	for (int iteration = 0; iteration < 10000; iteration++)
	{
		Run();
	}
	
	var topMonkeys = monkeys.OrderByDescending(monkey => monkey.Inspections).Take(2).ToArray();

	return topMonkeys[0].Inspections * topMonkeys[1].Inspections;

	void Run()
	{
		foreach (var monkey in monkeys)
		{
			monkey.Inspections += monkey.Items.Count;
			var items = monkey.Items;
			monkey.Items = new List<Int128>();
			
			foreach (var worry in items)
			{
				var newWorry = monkey.Operation(worry) % gcd;
				
				var newMonkey = (newWorry % monkey.DivisibleBy == 0) ? monkey.ThrowTrue : monkey.ThrowFalse;
				
				monkeys[newMonkey].Items.Add(newWorry);
			}
		}
	}}


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

