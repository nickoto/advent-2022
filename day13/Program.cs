using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Spectre.Console;
using System.Text.Json;

var part1Expect = 13L;
var part2Expect = 140L;

List<object> ParseLine(string line)
{
	List<object> top = new();
	var stack = new Stack<List<object>>();
	var collector = string.Empty;
	
	foreach (var c in line)
	{
		switch (c)
		{
			case '[':
				stack.Push(new());
				break;
			
			case ']':
				if (collector.Any())
				{
					stack.Peek().Add(int.Parse(collector));
					collector = string.Empty;
				}

				top = stack.Pop();

				if (stack.Any())
				{
					stack.Peek().Add(top);
				}
				break;
			
			case ',':
				if (collector.Any())
				{
					stack.Peek().Add(int.Parse(collector));
					collector = string.Empty;
				}
				break;

			default:
				if (c < '0' || c > '9') throw new InvalidOperationException();
				collector += c;
				break;
		}
	}

	return top;
}

IEnumerable<(List<object>, List<object>)> ParseData(IEnumerable<string> data)
{
	using var enumerator = data.GetEnumerator();

	while (enumerator.MoveNext())
	{
		Console.WriteLine("Compare");
		Console.WriteLine(enumerator.Current);
		var l = ParseLine(enumerator.Current);
		enumerator.MoveNext();
		Console.WriteLine("vs");
		Console.WriteLine(enumerator.Current);
		var r = ParseLine(enumerator.Current);
		enumerator.MoveNext();

		yield return (l, r);
		Console.WriteLine();
	}
}

int Compare(object l, object r)
{
	return CompareIn(l, r);
}

string Label(object l)
{
	if (l is int i)
		return i.ToString();

	if (l is List<object> list)
	{
		if (list.Any())
		{
			return $"[{list.Count} items]";
		}

		return "[]";
	}

	return "?";
}

int CompareIn(object l, object r)
{
	Console.WriteLine($" - Compare {Label(l)} vs {Label(r)}");
		
	if (l is int lInt && r is int rInt)
	{
		var result = lInt.CompareTo(rInt);
			
		if (result < 0) AnsiConsole.MarkupLine("   - Left is smaller so inputs are [green]in order[/]");
		if (result > 0) AnsiConsole.MarkupLine("   - Left is smaller so inputs are [red]not in order[/]");
			
		return result;
	}

	List<object> lList, rList;

	if (l is List<object> ll)
	{
		lList = ll;
	}
	else
	{
		Console.WriteLine($"   - Mixed types; convert left to [{(int)l}]");
		lList = new List<object>() {(int)l};
	};

	if (r is List<object> rl)
	{
		rList = rl;
	}
	else
	{
		Console.WriteLine($"   - Mixed types; convert right to [{(int)r}]");
		rList = new List<object>() {(int)r};
	};

	using var lEnum = lList.GetEnumerator();
	using var rEnum = rList.GetEnumerator();

	while (true)
	{
		bool lMore = lEnum.MoveNext();
		bool rMore = rEnum.MoveNext();

		if (lMore == false && rMore == false)
		{
			return 0;
		}
			
		if (lMore == false)
		{
			AnsiConsole.MarkupLine("   - Left ran out of items so inputs are [green]in order[/]");
			return -1;
		}

		if (rMore == false)
		{
			AnsiConsole.MarkupLine("   - Left ran out of items so inputs are [red]not in order[/]");
			return 1;
		}

		var cmp = Compare(lEnum.Current, rEnum.Current);
		if (cmp != 0)
			return cmp;
	}
}

string Encode(object o)
{
	if (o is int i)
	{
		return i.ToString();
	}
	
	if (o is List<object> l)
	{
		return $"[{string.Join(",", l.Select(x => Encode(x)))}]";
	}

	throw new Exception();
}

long GetPart1(string[] data)
{
	var result =
		ParseData(data)
			.Select((pair, index) => (pair, index: index + 1))
			.Where(x => Compare(x.pair.Item1, x.pair.Item2) < 0)
			.Select(x => x.index)
			.Sum();
	
	return result;
}

long GetPart2(string[] data)
{
	var result =
		data.Concat(new List<string>() {"[[2]]", "[[6]]"})
			.Where(l => !string.IsNullOrEmpty(l))
			.Select(ParseLine)
			.Select((line, index) => (line, index: index + 1))
			.ToList();
	
	result.Sort((l, r) => Compare(l.line, r.line));

	var s1 = 0;
	var s2 = 0;
	var p1 = result.Count;
	var p2 = result.Count - 1;
	var pos = 0;
	
	foreach (var x in result)
	{
		Console.WriteLine(Encode(x.line));
		pos++;
		
		if (x.index == p1){ s1 = pos; }
		if (x.index == p2){ s2 = pos; }
	}
	
	return s1 * s2;
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

