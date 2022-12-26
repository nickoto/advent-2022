using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Spectre.Console;

var part1Expect = 110L;
var part2Expect = 20L;

long GetPart1(string[] data)
{
	var w = data[0].Length;
	var h = data.Length;
	var map = new HashSet<UInt64>();

	var r = new Random();

	for (int i = 0; i < 10_000_000; i++)
	{
		var xx = r.Next(int.MinValue, int.MaxValue);
		var yy = r.Next(int.MinValue, int.MaxValue);
		var temp = FromPos(ToPos(xx, yy));

		if (xx != temp.x) throw new InvalidDataException();
		if (yy != temp.y) throw new InvalidDataException();
	}

	for (int y = 0; y < h; y++)
	{
		var line = data[y];
		for (int x = 0; x < w; x++)
		{
			if (line[x] == '#') map.Add(ToPos(x, y));
		}
	}
	
	var hits = new bool[3, 3];
	var tests = new List<Func<bool[,], int, int, (bool, UInt64)>>();
	
	tests.Add((h, x,y) => {
		if (!h[0, 0] && !h[1, 0] && !h[2, 0])
		{
			return (true, ToPos(x, y - 1));
		}

		return (false, 0);
	});
	tests.Add((h, x,y) => {
		if (!h[0, 2] && !h[1, 2] && !h[2, 2])
		{
			return (true, ToPos(x, y + 1));
		}

		return (false, 0);
	});
	tests.Add((h, x,y) => {
		if (!h[0, 0] && !h[0, 1] && !h[0, 2])
		{
			return (true, ToPos(x - 1, y));
		}

		return (false, 0);
	});
	tests.Add((h, x,y) => {
		if (!h[2, 0] && !h[2, 1] && !h[2, 2])
		{
			return (true, ToPos(x + 1, y));
		}

		return (false, 0);
	});
	tests.Add((h, x, y) => (true, ToPos(x, y)));

	
	for (var round = 0; round < 10; round++)
	{
		var newElfPos = new Dictionary<UInt64, UInt64>();
		var oldElfPos = new Dictionary<UInt64, UInt64>();
		
		foreach (var elf in map)
		{
			var (x, y) = FromPos(elf);
			
			for (int i = 0; i < 9; i++)
			{
				var xx = i % 3;
				var yy = i / 3;
				hits[xx, yy] = map.Contains(ToPos(x + xx - 1, y + yy - 1));
			}
			
			UInt64 newPos = 0;

			if (hits.Cast<bool>().Count(x => x) == 1)
			{
				newPos = ToPos(x, y);
			}
			else 
			{
				var result = tests.Select(f => f(hits, x, y)).First(x => x.Item1);
				newPos = result.Item2;
			}

			if (oldElfPos.TryGetValue(newPos, out var oldElf))
			{
				// Neither moves if there was someone already there.
				// and the old elf has to mvoe back to its original pos.
				newElfPos[elf] = elf;
				newElfPos[oldElf] = oldElf;
			}
			else
			{
				newElfPos[elf] = newPos;
				oldElfPos[newPos] = elf;
			}
		}

		var f = tests.First();
		tests.RemoveAt(0);
		tests.Insert(3, f);
		
		map = newElfPos.Values.ToHashSet();
	}

	var minx = int.MaxValue;
	var maxx = int.MinValue;
	var miny = int.MaxValue;
	var maxy = int.MinValue;
	
	foreach (var item in map.Select(FromPos))
	{
		minx = Math.Min(minx, item.x);
		miny = Math.Min(miny, item.y);
		maxx = Math.Max(maxx, item.x);
		maxy = Math.Max(maxy, item.y);
	}
	
	return Math.Abs(maxx - minx + 1) * Math.Abs(maxy - miny + 1) - map.Count;
}

long GetPart2(string[] data)
{
	var w = data[0].Length;
	var h = data.Length;
	var map = new HashSet<UInt64>();

	var r = new Random();

	for (int i = 0; i < 10_000_000; i++)
	{
		var xx = r.Next(int.MinValue, int.MaxValue);
		var yy = r.Next(int.MinValue, int.MaxValue);
		var temp = FromPos(ToPos(xx, yy));

		if (xx != temp.x) throw new InvalidDataException();
		if (yy != temp.y) throw new InvalidDataException();
	}

	for (int y = 0; y < h; y++)
	{
		var line = data[y];
		for (int x = 0; x < w; x++)
		{
			if (line[x] == '#') map.Add(ToPos(x, y));
		}
	}
	
	var hits = new bool[3, 3];
	var tests = new List<Func<bool[,], int, int, (bool, UInt64)>>();
	
	tests.Add((h, x,y) => {
		if (!h[0, 0] && !h[1, 0] && !h[2, 0])
		{
			return (true, ToPos(x, y - 1));
		}

		return (false, 0);
	});
	tests.Add((h, x,y) => {
		if (!h[0, 2] && !h[1, 2] && !h[2, 2])
		{
			return (true, ToPos(x, y + 1));
		}

		return (false, 0);
	});
	tests.Add((h, x,y) => {
		if (!h[0, 0] && !h[0, 1] && !h[0, 2])
		{
			return (true, ToPos(x - 1, y));
		}

		return (false, 0);
	});
	tests.Add((h, x,y) => {
		if (!h[2, 0] && !h[2, 1] && !h[2, 2])
		{
			return (true, ToPos(x + 1, y));
		}

		return (false, 0);
	});
	tests.Add((h, x, y) => (true, ToPos(x, y)));

	var round = 0;
	while(true)
	{
		round++;
		var newElfPos = new Dictionary<UInt64, UInt64>();
		var oldElfPos = new Dictionary<UInt64, UInt64>();
		
		foreach (var elf in map)
		{
			var (x, y) = FromPos(elf);
			
			for (int i = 0; i < 9; i++)
			{
				var xx = i % 3;
				var yy = i / 3;
				hits[xx, yy] = map.Contains(ToPos(x + xx - 1, y + yy - 1));
			}
			
			UInt64 newPos = 0;

			if (hits.Cast<bool>().Count(x => x) == 1)
			{
				newPos = ToPos(x, y);
			}
			else 
			{
				var result = tests.Select(f => f(hits, x, y)).First(x => x.Item1);
				newPos = result.Item2;
			}

			if (oldElfPos.TryGetValue(newPos, out var oldElf))
			{
				// Neither moves if there was someone already there.
				// and the old elf has to mvoe back to its original pos.
				newElfPos[elf] = elf;
				newElfPos[oldElf] = oldElf;
			}
			else
			{
				newElfPos[elf] = newPos;
				oldElfPos[newPos] = elf;
			}
		}

		var f = tests.First();
		tests.RemoveAt(0);
		tests.Insert(3, f);
		
		var newMap = newElfPos.Values.ToHashSet();

		if (newMap.Except(map).Any())
		{
			map = newMap;
			continue;
		}

		break;
	}
	
	return round;
}

UInt64 ToPos(int x, int y)
{
	return unchecked(((UInt64)x << 32) | (UInt32)y);
}

(int x, int y) FromPos(UInt64 v)
{
	var x = v >> 32;
	var y = v & 0xffffffff;

	return (unchecked((int)x), unchecked((int)y));
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

