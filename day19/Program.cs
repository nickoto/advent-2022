using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Spectre.Console;

var part1Expect = 33L;
var part2Expect = 56L * 62L;


long CalculateQuality(Blueprint bp, int maxTime, Action<int> progress)
{
	Int128 robots = 1;
	Int128 resources = 0;

	Int128 maxOre = (new[] {bp.Clay.Ore, bp.Geode.Ore, bp.Ore.Ore, bp.Obsidian.Ore}).Max();
	Int128 maxClay = (new[] {bp.Clay.Clay, bp.Geode.Clay, bp.Ore.Clay, bp.Obsidian.Clay}).Max();
	Int128 maxObsidian = (new[] {bp.Clay.Obsidian, bp.Geode.Obsidian, bp.Ore.Obsidian, bp.Obsidian.Obsidian}).Max();
	Int128 maxGeode = int.MaxValue;

	Int128 maxRobots = maxOre | maxClay << 32 | maxObsidian << 64 | maxGeode << 96;
	
	var best = 0;
	var maxTimePerGRobot = Enumerable.Repeat(0, 1024).ToArray();
	
	Calculate(bp, robots, resources, maxRobots, maxTime, maxTimePerGRobot, -1, ref best, progress);

	return best;
}

void Calculate(Blueprint bp, Int128 robots, Int128 resources, Int128 maxRobots, int time, int[] maxTimePerGRobot, int buy, ref int best, Action<int> progress)
{
	var geodes = resources.Int(3);
	var grobots = robots.Int(3);

	if (time == 0)
	{
		int n = Math.Max(best, geodes);
		if (n != best) { best = n; progress(n); }
		return;
	}

	if (time < maxTimePerGRobot[grobots])
	{
		return;
	}

	resources += robots;
	if (buy >= 0)
	{
		Int128 ore = 0;
		Int128 clay = 0;
		Int128 obsidian = 0;
		
		Int128 cost = 0;
		if (buy == 0) { ore += bp.Ore.Ore; }
		if (buy == 1) { ore += bp.Clay.Ore; }
		if (buy == 2) { ore += bp.Obsidian.Ore; clay += bp.Obsidian.Clay;}
		if (buy == 3) { ore += bp.Geode.Ore; obsidian += bp.Geode.Obsidian; }

		cost = ore + (clay << 32) + (obsidian << 64);
		resources -= cost;
		
		Int128 temp = 1;
		temp <<= 32 * buy;
		robots += temp;
	}
	
	if (resources.Int(0) >= bp.Geode.Ore && resources.Int(2) >= bp.Geode.Obsidian)
	{
		maxTimePerGRobot[grobots] = Math.Max(maxTimePerGRobot[grobots], time - 3);
		Calculate(bp, robots, resources, maxRobots, time - 1, maxTimePerGRobot, 3, ref best, progress);
	}
	if (robots.Int(2) < maxRobots.Int(2))
	{
		if (resources.Int(0) >= bp.Obsidian.Ore && resources.Int(1) >= bp.Obsidian.Clay)
		{
			Calculate(bp, robots, resources, maxRobots, time - 1, maxTimePerGRobot, 2, ref best, progress);
		}
	}
	if (robots.Int(1) < maxRobots.Int(1))
	{
		if (resources.Int(0) >= bp.Clay.Ore)
		{
			Calculate(bp, robots, resources, maxRobots, time - 1, maxTimePerGRobot, 1, ref best, progress);
		}
	}
	if (robots.Int(0) < maxRobots.Int(0))
	{
		if (resources.Int(0) >= bp.Ore.Ore)
		{
			Calculate(bp, robots, resources, maxRobots, time - 1, maxTimePerGRobot, 0, ref best, progress);
		}
	}

	
	Calculate(bp, robots, resources, maxRobots, time - 1, maxTimePerGRobot, -1, ref best, progress);
}


long[] Run(string[] data, int maxTime)
{
	var blueprints = data.Select(Blueprint.Parse).ToArray();
	var results = new long[blueprints.Length];

	AnsiConsole.Progress()
		.AutoRefresh(true) // Turn off auto refresh
		.AutoClear(false)   // Do not remove the task list when done
		.HideCompleted(false)   // Hide tasks as they are completed
		.Columns(new ProgressColumn[] 
		{
			new TaskDescriptionColumn(),    // Task description
			new ProgressBarColumn(),        // Progress bar
			new PercentageColumn(),         // Percentage
			new RemainingTimeColumn(),      // Remaining time
			new SpinnerColumn(),            // Spinner
		})
		.Start(ctx =>
		{
			var tasks = new List<ProgressTask>();
			foreach (var line in data)
			{
				var task = ctx.AddTask(line);
				tasks.Add(task);

				task.IsIndeterminate = false;
			}
			
			Parallel.ForEach(blueprints, (bp, DirectoryInfo, index) =>
			{
				tasks[(int)index].IsIndeterminate = true;
				tasks[(int)index].StartTask();

				results[index] = CalculateQuality(bp, maxTime, (x) =>
				{
					tasks[(int)index].Value = x;
				});

				tasks[(int)index].Increment(100);
				tasks[(int)index].StopTask();
			});
		});


	return results;
}

long GetPart1(string[] data)
{
	 return Run(data, 24).Select((x,id) => (x, id: id + 1)).Select(x => x.x * x.id).Sum();
}

long GetPart2(string[] data)
{
	var result = Run(data.Take(3).ToArray(), 32);
	Console.WriteLine(string.Join(", ", result));
	return result.Aggregate(1L, (x,y) => x * y);
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

class Cost
{
	public int Ore { get; set; }
	public int Clay { get; set; }
	public int Obsidian { get; set; }
}

class Blueprint
{
	private static Regex Parser = new Regex(@"Blueprint (\d*): \D*(\d*)\D*(\d*)\D*(\d*)\D*(\d*)\D*(\d*)\D*(\d*)\D*$", RegexOptions.Compiled); 
	public static Blueprint Parse(string line)
	{
		var match = Parser.Match(line);

		return new Blueprint()
		{
			Id = int.Parse(match.Groups[1].Value),
			Ore = new Cost() {Ore = int.Parse(match.Groups[2].Value)},
			Clay = new Cost() {Ore = int.Parse(match.Groups[3].Value)},
			Obsidian = new Cost() {Ore = int.Parse(match.Groups[4].Value), Clay = int.Parse(match.Groups[5].Value)},
			Geode = new Cost() {Ore = int.Parse(match.Groups[6].Value), Obsidian = int.Parse(match.Groups[7].Value)},
		};
	}
	
	public int Id { get; set; }
	public Cost Ore { get; set; }
	public Cost Clay { get; set; }
	public Cost Obsidian { get; set; }
	public Cost Geode { get; set; }
}

public static class Helper {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Int(this Int128 val, byte off)
	{
		return (int)(val >> (off * 32) & 0xffffffff);
	}
}
