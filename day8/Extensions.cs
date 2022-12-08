using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace day8;

public static class Extensions
{
	public static IEnumerable<T> Every<T>(this T[] source, int start, int skip)
	{
		var count = source.Count();

		for (int i = start; i < count; i += skip)
		{
			yield return source[i];
		}
	}
}
