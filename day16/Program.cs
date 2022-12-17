using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using Spectre.Console;
using day16;

var part1Expect = 1651L;
var part2Expect = 1707L;

int Dist(Valve[] graph, int start, int end)
{
	if (start == end) return 0;

	var dist = graph.ToDictionary(x => x.Id, x => int.MaxValue);
	var queue = graph.ToDictionary(x => x.Id, x => int.MaxValue);

	dist[start] = 0;
	queue[start] = 0;

	while (queue.Any())
	{
		var curId = queue.MinBy(x => x.Value).Key;
		var curValve = graph[curId];
		queue.Remove(curId);
		
		foreach(var neighbor in curValve.Neighbors)
		{
			var newDist = dist[curId] + 1;
			if (newDist < dist[neighbor])
			{
				dist[neighbor] = newDist;
				queue[neighbor] = newDist;
			}
		}
	}

	return dist[end];
}

long GetPart1(string[] data)
{
	var maxMoves = 30;
	var startPos = "AA";
	
	var inputs = data.Select(x => RawValve.Parse(x)).OrderByDescending(x => x.Rate).ToArray();
	var names = inputs.Select((v, Id) => (v.Name, Id)).ToDictionary(k => k.Name, v => v.Id);
	var valves = inputs
		.Select(x => new Valve()
		{
			Name = x.Name,
			Id = names[x.Name],
			Rate = x.Rate,
			Neighbors = x.InitialConnections.Select(y => names[y]).ToArray()
		}).ToArray();

	var count = valves.Length;
	
	// Distances to every location on the map.
	var distances = new int[count * count];
	for (int y = 0; y < count; y++)
	{
		for (int x = 0; x < count; x++)
		{ 
			distances[x + y * count] = Dist(valves, x, y);
		}
	}

	// Valves we care about

	Stack<(int, string)> Visits = new();
	
	var maxValve = valves.Count(x => x.Rate > 0);
	var cache = new Dictionary<string, long>();
	var hits = 0;
	var misses = 0;
	
	var result = Maximize(valves.Take(maxValve), maxMoves, valves[names[startPos]]);
	return result;

	long Maximize(IEnumerable<Valve> maxValves, int remainingTime, Valve curValve)
	{
		var key = $"{string.Join(' ', maxValves.Select(x => x.Id))}-{remainingTime}-{curValve.Id}";
		if (cache.TryGetValue(key, out var res))
		{
			hits++;
			return res;
		}

		misses++;

		long max = 0;
		int idx = 0;

		foreach (var valve in maxValves)
		{
			int timeLeft = remainingTime - distances[curValve.Id + valve.Id * count] - 1;
			if (timeLeft > 0)
			{
				var amount = valve.Rate * timeLeft;
				var plus = Maximize(maxValves.SkipAt(idx), timeLeft, valve);

				max = Math.Max(max, amount + plus);
			}

			idx++;
		}

		cache[key] = max;
		return max;
	}
}


long GetPart2(string[] data)
{
	var maxMoves = 26;
	var startPos = "AA";

	var inputs = data.Select(x => RawValve.Parse(x)).OrderByDescending(x => x.Rate).ToArray();
	var names = inputs.Select((v, Id) => (v.Name, Id)).ToDictionary(k => k.Name, v => v.Id);
	var valves = inputs
		.Select(x => new Valve() {Name = x.Name, Id = names[x.Name], Rate = x.Rate, Neighbors = x.InitialConnections.Select(y => names[y]).ToArray()}).ToArray();

	var count = valves.Length;

	// Distances to every location on the map.
	var distances = new int[count * count];
	for (int y = 0; y < count; y++)
	{
		for (int x = 0; x < count; x++)
		{
			distances[x + y * count] = Dist(valves, x, y);
		}
	}

	// Valves we care about

	Stack<(int, string)> Visits = new();

	var maxValve = valves.Count(x => x.Rate > 0);
	var cache = new Dictionary<string, long>();
	var hits = 0;
	var misses = 0;

	var result = Maximize(valves.Take(maxValve), new[] {maxMoves, maxMoves}, new [] { valves[names[startPos]], valves[names[startPos]]}, 0);
	return result;

	long Maximize(IEnumerable<Valve> maxValves, int[] remainingTime, Valve[] curValve, int depth)
	{
		
		var key = $"{string.Join(' ', maxValves.Select(x => x.Id))}-{string.Join(' ', remainingTime)}-{string.Join(' ', curValve.Select(x => x.Id))}-{depth}";
		if (cache.TryGetValue(key, out var res))
		{
			hits++;
			return res;
		}

		misses++;

		long max = 0;
		int idx = 0;
		int player = depth % 2;
		var timeLeft = remainingTime.ToArray();
		var nextValve = curValve.ToArray();
		
		foreach (var valve in maxValves)
		{
			timeLeft[player] = remainingTime[player] - distances[curValve[player].Id + valve.Id * count] - 1;
			if (timeLeft[player] > 0)
			{
				var amount = valve.Rate * timeLeft[player];
				nextValve[player] = valve;
				var plus = Maximize(maxValves.SkipAt(idx), timeLeft, nextValve, depth+1);

				max = Math.Max(max, amount + plus);
			}

			idx++;
			
			if (depth == 0)
			{
				Console.WriteLine($"{valve.Name} - {max}  --- Hits: {hits}   Misses: {misses}");
			}
		}

		cache[key] = max;
		return max;
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

