using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Spectre.Console;
using Spectre.Console.Rendering;

var part1Expect = 152L;
var part2Expect = 301L;

long GetPart1(string[] data)
{
	var parsed = new Dictionary<string, Func<long>>();
	foreach (var item in data.Select(s => s.Split(": ")))
	{
		Func<long> fn;
		var v = item[1].Split(" ");
		if (v.Length == 1)
		{
			var value = long.Parse(v[0]);
			fn = () => value;
		}
		else
		{
			fn = v[1][0] switch
			{
				'+' => () => parsed[v[0]]() + parsed[v[2]](), 
				'-' => () => parsed[v[0]]() - parsed[v[2]](), 
				'*' => () => parsed[v[0]]() * parsed[v[2]](), 
				'/' => () => parsed[v[0]]() / parsed[v[2]](), 
				_ => throw new InvalidDataException()
			};
		}

		parsed[item[0]] = fn;
	}

	return parsed["root"]();
}

long GetPart2(string[] data)
{
	var parsed = data.Select(x => x.Split(": ")).ToDictionary(k => k[0], v => v[1]);

	Simplify("root");
	
	var root = parsed["root"];
	var eq = root.Split(" ");

	var lhs = eq[0];
	var rhs = eq[2];

	if (eq[0][0] >= 'a' && eq[0][0] <= 'z')
	{
		(lhs, rhs) = (rhs, lhs);
	}

	var expect = Int128.Parse(lhs);

	return (long)Solve(expect, rhs);
	
	(Int128 value, bool humn) Simplify(string key)
	{
		var value = parsed[key];
		var eq = value.Split(" ");

		if (eq.Length == 1)
		{
			return (Int128.Parse(eq[0]), key == "humn");
		}

		var lhs = Simplify(eq[0]);
		var rhs = Simplify(eq[2]);
		var total = eq[1][0] switch
		{
			'+' => lhs.value + rhs.value,
			'-' => lhs.value - rhs.value,
			'*' => lhs.value * rhs.value,
			'/' => lhs.value / rhs.value,
			_ => throw new InvalidDataException()
		};
		
		if (!lhs.humn && !rhs.humn)
		{
			parsed[key] = total.ToString();
			return (total, false);
		}

		if (lhs.humn)
		{ 
			parsed[key] = $"{eq[0]} {eq[1]} {rhs.value}";
			return (0, true);
		}
		
		parsed[key] = $"{lhs.value} {eq[1]} {eq[2]}";
		return (0, true);
	}

	string Dump(string key, int depth = 0)
	{
		var value = parsed[key];
		var eq = value.Split(" ");

		if (key == "humn")
		{
			return key;
		}
		
		if (eq.Length == 1)
		{
			return value;
		}

		var lhs = (eq[0][0] >= 'a' && eq[0][0] <= 'z') ? $"({Dump(eq[0])})" : eq[0];
		var rhs = (eq[2][0] >= 'a' && eq[2][0] <= 'z') ? $"({Dump(eq[2])})" : eq[2];

		return $"{lhs} {eq[1]} {rhs}";
	}

	Int128 Solve(Int128 expect, string key)
	{
		var next = parsed[key];

		if (key == "humn")
		{
			return expect;
		}

		var eq = next.Split(" ");
		var lhs = eq[0];
		var rhs = eq[2];

		if (eq[0][0] >= 'a' && eq[0][0] <= 'z')
		{
			var value = Int128.Parse(rhs);

			switch(eq[1][0])
			{
				case '+':
					expect -= value;
					break;
				
				case '-':
					expect += value;
					break;
				
				case '*':
					expect /= value;
					break;
				
				case '/':
					expect *= value;
					break;
			};
			
			return Solve(expect, lhs);
		}
		else
		{
			var value = Int128.Parse(lhs);

			switch(eq[1][0])
			{
				case '+':
					expect -= value;
					break;
				
				case '-':
					(expect, value) = (value, expect);
					expect -= value;
					break;
				
				case '*':
					expect /= value;
					break;
				
				case '/':
					throw new InvalidDataException();
			};

			return Solve(expect, rhs);
		}
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

