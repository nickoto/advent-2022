using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Spectre.Console;

var part1Expect = 95437L;
var part2Expect = 24933642L;

DirItem ParseDirectory(IEnumerator<string> enumerator, string name)
{
	var ret = new DirItem {Name = name, Items = new()};
	var skip = false;

	while (skip || enumerator.MoveNext())
	{
		skip = false;
		
		if (!enumerator.Current.StartsWith("$"))
		{
			throw new InvalidOperationException();
		}

		var cmdLine = enumerator.Current.Split(" ");
		if (cmdLine[1] == "cd")
		{
			if (cmdLine[2] == "..")
			{
				break;
			}
			
			ret.Items.Add(ParseDirectory(enumerator, cmdLine[2]));
		}
		else if (cmdLine[1] == "ls")
		{
			while (enumerator.MoveNext())
			{
				cmdLine = enumerator.Current.Split(" ");
				if (cmdLine[0] == "$")
				{
					skip = true;
					break;
				}
				
				if (cmdLine[0] == "dir")
				{
					continue;
				}

				ret.Items.Add(new FileItem(){Name = cmdLine[1], Size = long.Parse(cmdLine[0])});
			}
		}
		else
		{
			throw new InvalidOperationException();
		}
	}

	ret.Size = ret.Items.Select(x => x.Size).Sum();
	
	return ret;
}

long FindTotalSize(long max, DirItem dir, string indent = "")
{
	var total = 0l;
	
	foreach (var item in dir.Items)
	{
		if (item is DirItem subDir)
		{
			if (item.Size <= max) total += item.Size;
			total += FindTotalSize(max, subDir, indent + " ");
		}
	}

	return total;
}

long FindClosestSize(long minSize, DirItem dir)
{
	var found = long.MaxValue;
	
	foreach (var item in dir.Items)
	{
		if (item is DirItem subDir)
		{
			if (item.Size >= minSize && item.Size < found)
			{
				found = item.Size;
			}

			var sub = FindClosestSize(minSize, subDir);
			if (sub >= minSize && sub < found)
			{
				found = sub;
			}
		}
	}

	return found;
}


long GetPart1(string[] data)
{
	var enumerator = data.AsEnumerable().GetEnumerator();
	enumerator.MoveNext(); // $ cd /
	
	var dir = ParseDirectory(enumerator, "/");
	
	return FindTotalSize(100000, dir);
}

long GetPart2(string[] data)
{
	var enumerator = data.AsEnumerable().GetEnumerator();
	enumerator.MoveNext(); // $ cd /
	
	var dir = ParseDirectory(enumerator, "/");

	var diskSize = 70000000;
	var needed = 30000000;
	var used = dir.Size;
	var findDirSize = needed - (diskSize - used);
	
	return FindClosestSize(findDirSize, dir);
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

