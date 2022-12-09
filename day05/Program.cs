using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using Spectre.Console;

var part1Expect = "CMZ";
var part2Expect = "MCD";

var instructionParser = new Regex(@"move (\d*) from (\d*) to (\d*)", RegexOptions.Compiled);

List<Stack<char>> ParseStacks(IEnumerator<string> enumerator)
{
	var inputStacks = new Stack<string>();
	enumerator.MoveNext();

	while (true)
	{
		var line = enumerator.Current;
		if (line[1] == '1')
		{
			// Welcome to the stack identifier line.
			break;
		}
		
		inputStacks.Push(line);
		enumerator.MoveNext();
	}

	var stackCount = enumerator.Current.Trim().Split("  ").Length;
	enumerator.MoveNext();
	
	var stacks = new List<Stack<char>>();
	for(int i=0; i<stackCount; i++) 
		stacks.Add(new Stack<char>());

	while (inputStacks.Any())
	{
		var level = inputStacks.Pop();
		for (int i = 0; i < stackCount; i++)
		{
			var letter = level[i * 4 + 1];
			if (letter > 65)
			{
				stacks[i].Push(letter);
			}
		}
	}

	return stacks;
}

string Run(string[] data, bool keepOrder)
{
	var enumerator = data.AsEnumerable().GetEnumerator();
	var stacks = ParseStacks(enumerator);
	
	while (enumerator.MoveNext())
	{
		var instruction = instructionParser.Match(enumerator.Current);
		var count = int.Parse(instruction.Groups[1].Value);
		var from = int.Parse(instruction.Groups[2].Value) - 1;
		var to = int.Parse(instruction.Groups[3].Value) - 1;

		if (keepOrder)
		{ 
			foreach (var item in stacks[from].Take(count).Reverse().ToList())
			{
				stacks[from].Pop();
				stacks[to].Push(item);
			}
		}
		else
		{
			for (int i = 0; i < count; i++)
			{
				stacks[to].Push(stacks[from].Pop());
			}
		}
	}
	
	return string.Join("", stacks.Select(x => x.FirstOrDefault()));
}

string GetPart1(string[] data)
{
	return Run(data, false);
}

string GetPart2(string[] data)
{
	return Run(data, true);
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
AnsiConsole.MarkupLineInterpolated($"[[[aqua]{stopwatch.ElapsedMilliseconds} ms[/]]] Part 2: [aqua]{part2Result}[/]");
