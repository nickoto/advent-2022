using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace day16;

// Valve OS has flow rate=0; tunnels lead to valves EE, CL

public class RawValve
{
	private static Regex Parser = new Regex(@"Valve (\w\w) has flow rate=(\d*); tunnels? leads? to valves? (.*)", RegexOptions.Compiled);
	
	public static RawValve Parse(string line)
	{
		var result = Parser.Match(line);

		return new RawValve()
		{
			Name = result.Groups[1].Value, 
			Rate = int.Parse(result.Groups[2].Value), 
			InitialConnections = result.Groups[3].Value.Split(", ")
		};
	}
	
	public string Name;
	public long Rate;
	public string[] InitialConnections;
	public RawValve[] Connections;
}

public class Valve
{
	public string Name;
	public int Id;
	public long Rate;
	public int[] Neighbors;
}

public static class EnumerableExtensions
{
	public static IEnumerable<T> SkipAt<T>(this IEnumerable<T> self, int idx)
	{
		int i = 0;
		foreach (var item in self)
		{
			if (i != idx)
			{
				yield return item;
			}

			i++;
		}
	}
}
