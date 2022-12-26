using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Spectre.Console;
using day24;

var part1Expect = 18L;
var part2Expect = 54L;

(int MapWidth, int MapHeight, byte[,] Map) Preprocess(string[] data)
{
	var mapWidth = data[0].Length;
	var mapHeight = data.Length;
	
	var map = new byte[mapWidth, mapHeight];
	
	for (int my = 0; my < mapHeight; my++)
	{
		var line = data[my];
		for (int mx = 0; mx < mapWidth; mx++)
		{
			map[mx, my] = line[mx] switch
			{
				'#' => 0x10,
				'.' => 0,
				'^' => 0x1,
				'v' => 0x2,
				'<' => 0x4,
				'>' => 0x8,
				_ => throw new InvalidDataException()
			};
		}
	}

	return (mapWidth, mapHeight, map);
}

long GetPart1(int mapWidth, int mapHeight, byte[,] initialMap)
{
	var maps = new List<byte[,]>();
	maps.Add(initialMap);
	
	int maxw = mapWidth - 2;
	int maxh = mapHeight - 2;

	byte[,] MapForTurn(int turn)
	{
		while (maps.Count <= turn)
		{
			var map = maps.Last();
			var newMap = new byte[mapWidth, mapHeight];

			// Run the wind.
			for (var y = 1; y < mapHeight - 1; y++)
			{
				for (var x = 1; x < mapWidth - 1; x++)
				{
					var value = map[x, y];
					if ((value & 0x1) != 0)
					{
						Or(x, y - 1, 0x1); // Up
					}
					if ((value & 0x2) != 0)
					{
						Or(x, y + 1, 0x2); // Down
					}
					if ((value & 0x4) != 0)
					{
						Or(x - 1, y, 0x4); // Left
					}
					if ((value & 0x8) != 0)
					{
						Or(x + 1, y, 0x8); // Right
					}
				}
			}

			maps.Add(newMap);
			
			void Or(int x, int y, byte v)
			{
				x = ((x + maxw - 1) % maxw) + 1;
				y = ((y + maxh - 1) % maxh) + 1;

				if (x < 0 || y < 0) throw new InvalidDataException();

				newMap[x, y] += v;
			}
		}

		return maps[turn];
	}

	var searches = new Queue<SearchNode>();
	var visits = new HashSet<Int64>();
	
	searches.Enqueue(new SearchNode(1, 0, 0));
	
	while (true)
	{
		var nextNode = searches.Dequeue();
		
		if (FindExit(nextNode, mapWidth - 2, mapHeight - 1, 1, 0))
		{
			return nextNode.Turn;
		}
	}
	
	bool FindExit(SearchNode node, int toX, int toY, int fromX, int fromY)
	{
		var turn = node.Turn + 1;
		var map = MapForTurn(turn);
		
		if (node.X == toX && node.Y == toY)
		{
			// Adjacent to the exit, so we're done.
			return true;
		}
		
		if (node.X <= 0 || node.Y <= 0 || node.X >= (mapWidth - 1) || node.Y >= (mapHeight - 1))
		{
			if (!(node.X == fromX && node.Y == fromY))
			{
				return false;
			}
		}
		
		// Dump(map, mapWidth, mapHeight, node);

		if (map[node.X, node.Y] == 0) Enqueue(node with { Turn = turn });
		if (node.X > 1 && map[node.X - 1, node.Y] == 0) Enqueue(new SearchNode(node.X - 1, node.Y, turn));
		if (node.Y > 1 && map[node.X, node.Y - 1] == 0) Enqueue(new SearchNode(node.X, node.Y - 1, turn));
		if (node.X < mapWidth - 1 && map[node.X + 1, node.Y] == 0) Enqueue(new SearchNode(node.X + 1, node.Y, turn));		
		if (node.Y < mapHeight - 1 && map[node.X, node.Y + 1] == 0) Enqueue(new SearchNode(node.X, node.Y + 1, turn));
		
		return false;
	}

	void Enqueue(SearchNode node)
	{
		var key = (node.Turn * 1L) | ((Int64)node.X << 32) | ((Int64)node.Y << 48);
		if (visits.Contains(key))
			return;

		visits.Add(key);
		searches.Enqueue(node);
	}
}

void Dump(byte[,] map, int w, int h, SearchNode n)
{
	Console.Clear();

	if (n.X == w - 1)
		throw new InvalidDataException();

	for (int y = 0; y < h; y++)
	{
		for (int x = 0; x < w; x++)
		{
			var b = map[x, y];
			Console.Write(b switch
			{
				0 => '.',
				0x1 => '^',
				0x2 => 'v',
				0x4 => '<',
				0x8 => '>',
				0xff => '#',
				_ => (char)('8' + b)
			});
		}
		
		Console.WriteLine();
	}
	
	Console.WriteLine($"Depth: {n.Turn}");
	
	Console.SetCursorPosition(n.X, n.Y);
	Console.Write("@");
	
	Thread.Sleep(200);
}

long GetPart2(int mapWidth, int mapHeight, byte[,] map)
{
	var turnMax = 0;
	var maps = new List<byte[,]>(); 
	maps.Add(map);
	
	int maxw = mapWidth - 2;
	int maxh = mapHeight - 2;

	byte[,] MapForTurn(int turn)
	{
		while (maps.Count <= turn)
		{
			var map = maps.Last();
			var newMap = new byte[mapWidth, mapHeight];

			// Run the wind.
			for (int y = 1; y < mapHeight - 1; y++)
			{
				for (int x = 1; x < mapWidth - 1; x++)
				{
					var value = map[x, y];
					if ((value & 0x1) != 0)
					{
						Or(x, y - 1, 0x1);
					}
					if ((value & 0x2) != 0)
					{
						Or(x, y + 1, 0x2);
					}
					if ((value & 0x4) != 0)
					{
						Or(x - 1, y, 0x4);
					}
					if ((value & 0x8) != 0)
					{
						Or(x + 1, y, 0x8);
					}
				}
			}

			maps.Add(newMap);
		
			void Or(int x, int y, byte v)
			{
				x = (x + maxw - 1) % maxw + 1;
				y = (y + maxh - 1) % maxh + 1;

				if (x < 0 || y < 0) throw new InvalidDataException();

				newMap[x, y] |= v;
			}
		}

		return maps[turn];
	}

	var searches = new Queue<SearchNode>();
	var visits = new HashSet<Int64>();
	
	searches.Enqueue(new SearchNode(1, 0, 0));
	
	while (true)
	{
		var nextNode = searches.Dequeue();
		
		if (FindExit(nextNode, mapWidth - 2, mapHeight - 1, 1, 0))
		{
			visits.Clear();
			searches.Clear();
			searches.Enqueue(nextNode);
			break;
		}
	}
	Console.WriteLine($"Trip 1 - {searches.Peek().Turn} - @ {searches.Peek().X},{searches.Peek().Y}");
	
	while (true)
	{
		var nextNode = searches.Dequeue();
		
		if (FindExit(nextNode, 1, 0, mapWidth - 2, mapHeight - 1))
		{
			visits.Clear();
			searches.Clear();
			searches.Enqueue(nextNode);
			break;
		}
	}
	Console.WriteLine($"Trip 2 - {searches.Peek().Turn} - @ {searches.Peek().X},{searches.Peek().Y}");

	while (true)
	{
		var nextNode = searches.Dequeue();
		
		if (FindExit(nextNode, mapWidth - 2, mapHeight - 1, 1, 0))
		{
			visits.Clear();
			searches.Clear();
			searches.Enqueue(nextNode);
			break;
		}
	}
	Console.WriteLine($"Trip 3 - {searches.Peek().Turn} - @ {searches.Peek().X},{searches.Peek().Y}");

	return searches.Peek().Turn;
	
	
	bool FindExit(SearchNode node, int toX, int toY, int fromX, int fromY)
	{
		var turn = node.Turn + 1;

		var map = MapForTurn(turn);

		// Dump(map, mapWidth, mapHeight, node);
		
		if (node.X == toX && node.Y == toY)
		{
			// Adjacent to the exit, so we're done.
			return true;
		}
		
		if (node.X <= 0 || node.Y <= 0 || node.X >= (mapWidth - 1) || node.Y >= (mapHeight - 1))
		{
			if (!(node.X == fromX && node.Y == fromY))
			{
				return false;
			}
		}
		
		if (map[node.X, node.Y] == 0) Enqueue(new SearchNode(node.X, node.Y, turn));
		if (node.X >= 1 && map[node.X - 1, node.Y] == 0) Enqueue(new SearchNode(node.X - 1, node.Y, turn));
		if (node.Y >= 1 && map[node.X, node.Y - 1] == 0) Enqueue(new SearchNode(node.X, node.Y - 1, turn));
		if (node.X < mapWidth - 1 && map[node.X + 1, node.Y] == 0) Enqueue(new SearchNode(node.X + 1, node.Y, turn));		
		if (node.Y < mapHeight - 1 && map[node.X, node.Y + 1] == 0) Enqueue(new SearchNode(node.X, node.Y + 1, turn));
		
		return false;
	}

	void Enqueue(SearchNode node)
	{
		var key = (Int64)node.Turn | (Int64)node.X << 32 | (Int64)node.Y << 48;
		if (visits.Contains(key))
			return;

		visits.Add(key);
		searches.Enqueue(node);
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

var sample = Preprocess(sampleData);
var input = Preprocess(inputData);

AnsiConsole.MarkupLineInterpolated($"[[[aqua]{stopwatch.ElapsedMilliseconds} ms[/]]] Pre-compute\n");


stopwatch = Stopwatch.StartNew();
var part1TestResult = GetPart1(sample.MapWidth, sample.MapHeight, sample.Map);
AnsiConsole.MarkupLineInterpolated($"[[[aqua]{stopwatch.ElapsedMilliseconds} ms[/]]] Part 1 (Test): [aqua]{part1TestResult}[/]");
if (part1TestResult != part1Expect)
{
	AnsiConsole.MarkupLineInterpolated($"[[[red]Failed[/]]] Result: [aqua]{part1TestResult}[/]  Expected: [aqua]{part1Expect}[/]");
	return;
}

AnsiConsole.MarkupLineInterpolated($"[[[green]Passed[/]]] Result: [aqua]{part1TestResult}[/]\n");


stopwatch = Stopwatch.StartNew();
var part1Result = GetPart1(input.MapWidth, input.MapHeight, input.Map);
AnsiConsole.MarkupLineInterpolated($"[[[aqua]{stopwatch.ElapsedMilliseconds} ms[/]]] Part 1: [aqua]{part1Result}[/]\n");


stopwatch = Stopwatch.StartNew();
var part2TestResult = GetPart2(sample.MapWidth, sample.MapHeight, sample.Map);
AnsiConsole.MarkupLineInterpolated($"[[[aqua]{stopwatch.ElapsedMilliseconds} ms[/]]] Part 2 (Test): [aqua]{part2TestResult}[/]");
if (part2TestResult != part2Expect)
{
	AnsiConsole.MarkupLineInterpolated($"[[[red]Failed[/]]] Result: [aqua]{part2TestResult}[/]  Expected: [aqua]{part2Expect}[/]");
	return;
}

AnsiConsole.MarkupLineInterpolated($"[[[green]Passed[/]]] Result: [aqua]{part2TestResult}[/]\n");

stopwatch = Stopwatch.StartNew();
var part2Result = GetPart2(input.MapWidth, input.MapHeight, input.Map);
AnsiConsole.MarkupLineInterpolated($"[[[aqua]{stopwatch.ElapsedMilliseconds} ms[/]]] Part 2: [aqua]{part2Result}[/]\n");

