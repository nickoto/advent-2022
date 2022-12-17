using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Spectre.Console;

var part1Expect = 3068L;
var part2Expect = 1_514_285_714_288L; // 1_000_000_000_000

void DumpWell(List<byte> well)
{
	for (int i = well.Count; i > 0; i--)
	{
		var row = well[i - 1];
		Console.Write("|");
		for (int bit = 0; bit < 7; bit++)
		{
			if ((row & (1 << bit)) != 0) Console.Write("#");
			else Console.Write(".");
		}

		Console.WriteLine("|");
	}
		
	Console.WriteLine("+-------+\n");
}

long GetPart1(string[] data)
{
	var pieces = new[] {new byte[] {0xf}, new byte[] {0x2, 0x7, 0x2}, new byte[] {0x7, 0x4, 0x4}, new byte[] {0x1, 0x1, 0x1, 0x1}, new byte[] {0x3, 0x3}};
	var pieceWidth = new[] { 4, 3, 3, 1, 2 };
	var width = 7;
	var height = 0;
	var round = 0;
	var well = new List<byte> { 0x7f };
	var jetPattern = data[0];
	var jetIndex = 0;

	while (round < 2022)
	{
		var piece = pieces[round % pieces.Length];
		var pw = pieceWidth[round % pieces.Length];
		var ph = piece.Length;
		round++;

		var x = 2;
		var y = height + 4;
		var t = -1;

		while(well.Count <= y + 5) well.Add(0);
		
		while (true)
		{
			t++;

			// Push then fall.
			if (t % 2 == 0) // push
			{
				var jet = jetPattern[jetIndex % jetPattern.Length];
				jetIndex++;
				
				var newx = jet == '<' ? x - 1 : x + 1;

				if (newx < 0) continue;
				if (newx + pw > width) continue;
				if (DoesPieceOverlap(piece, pw, ph, newx, y, well)) continue;
				
				x = newx;
			}
			else
			{
				var newy = y - 1;
				if (DoesPieceOverlap(piece, pw, ph, x, newy, well)) break;
				y = newy;
			}
		}

		// Add the piece to the well.
		for (var cy = 0; cy < piece.Length; cy++)
		{
			var wb = well[y + cy];
			well[y + cy] = (byte)(wb | (piece[cy] << x));
		}
		
		height = well.Count - 1;
		while (well[height] == 0) height--;

		/*
		DumpWell();
		Console.WriteLine(round);
		Console.Write("");
		*/
	}

	return height; // Don't include the floor.

	bool DoesPieceOverlap(byte[] piece, int pw, int ph, int px, int py, List<byte> well)
	{
		for (int i = 0; i < ph; i++)
		{
			var bWell = well[i + py];
			var bPiece = piece[i] << px;

			if ((bWell | bPiece) != (bWell ^ bPiece))
				return true;
		}
		
		return false;
	}
}

long GetPart2(string[] data)
{
	var bigNum = 200_000;
	
	var pieces = new[] {new byte[] {0xf}, new byte[] {0x2, 0x7, 0x2}, new byte[] {0x7, 0x4, 0x4}, new byte[] {0x1, 0x1, 0x1, 0x1}, new byte[] {0x3, 0x3}};
	var pieceWidth = new[] { 4, 3, 3, 1, 2 };
	var width = 7;
	var height = 0;
	var round = 0;
	var well = new List<byte>(bigNum) { 0x7f };
	var heights = new List<int>(bigNum);
	var jetPattern = data[0];
	var jetIndex = 0;

	while (round < bigNum)
	{
		var piece = pieces[round % pieces.Length];
		var pw = pieceWidth[round % pieces.Length];
		var ph = piece.Length;
		round++;

		var x = 2;
		var y = height + 4;
		var t = -1;

		while(well.Count <= y + 5) well.Add(0);
		
		while (true)
		{
			t++;

			// Push then fall.
			if (t % 2 == 0) // push
			{
				var jet = jetPattern[jetIndex % jetPattern.Length];
				jetIndex++;
				
				var newx = jet == '<' ? x - 1 : x + 1;

				if (newx < 0) continue;
				if (newx + pw > width) continue;
				if (DoesPieceOverlap(piece, pw, ph, newx, y, well)) continue;
				
				x = newx;
			}
			else
			{
				var newy = y - 1;
				if (DoesPieceOverlap(piece, pw, ph, x, newy, well)) break;
				y = newy;
			}
		}

		// Add the piece to the well.
		for (var cy = 0; cy < piece.Length; cy++)
		{
			var wb = well[y + cy];
			well[y + cy] = (byte)(wb | (piece[cy] << x));
		}
		
		height = well.Count - 1;
		while (well[height] == 0) height--;

		heights.Add(height);
	}
	
	var stop = false;
	var start = 0;
	var span = 0;

	while(!stop)
	{
		start++;
		var startV = heights[start];
		var modValue = startV;
		for (int i = start + 1; i < heights.Count - 10; i++)
		{
			var val = heights[i] % modValue;
			if (val == 0)
			{
				for (int j = 1; j < i - start; j++)
				{
					var l = heights[start + j] % modValue;
					var r = heights[i + j] % modValue;
					if (l == r)
					{
						stop = true;
						continue;
					}

					stop = false;
					break;
				}

				if (stop)
				{
					span = i - start;
					break;
				}
			}
		}
	}
	
	// We need 1_000_000_000_000
	var need = 1_000_000_000_000;
		need -= start;
			
	var repeatCount = need / span;
	var spanHeight = heights[start + span] - heights[start];
	var heightRepeat = repeatCount * spanHeight;
	var remainder = need % span;

	var totalHeight = heightRepeat + heights[start] + heights[start + (int)remainder] - heights[start];

	Console.WriteLine($"Repeating spans start at: {start}");
	Console.WriteLine($"Span: {span}");
	Console.WriteLine($"Height of span {spanHeight}");
	Console.WriteLine($"Height of repeats {heightRepeat} ({repeatCount} repeats)");
	Console.WriteLine($"Total height {totalHeight}");
	
	return totalHeight - 1; //Remove the floor!

	bool DoesPieceOverlap(byte[] piece, int pw, int ph, int px, int py, List<byte> well)
	{
		for (int i = 0; i < ph; i++)
		{
			var bWell = well[i + py];
			var bPiece = piece[i] << px;

			if ((bWell | bPiece) != (bWell ^ bPiece))
				return true;
		}
		
		return false;
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

