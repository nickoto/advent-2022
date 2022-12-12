using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;


using System.Text.RegularExpressions;

namespace day11;

public class Monkey
{
	public int Id;
	public List<Int128> Items;
	public Func<Int128, Int128> Operation;
	public Int128 DivisibleBy;
	public int ThrowTrue;
	public int ThrowFalse;
	public long Inspections;
	
	public static Monkey Parse(IEnumerator<string> input)
	{
		var ret = new Monkey();

		// Monkey: n
		ret.Id = int.Parse(input.Current.Split(" ")[1].TrimEnd(':'));
		input.MoveNext();
		
		// Starting items: x, y, z
		ret.Items = input.Current.Split(":")[1].Split(", ").Select(x => Int128.Parse(x)).ToList();
		input.MoveNext();
		
		// Operation:
		var rhs = input.Current.Split(" = ")[1];
		var bits = rhs.Split(' ');
		Int128.TryParse(bits[2], out Int128 value);
		
		ret.Operation = (bits[1], bits[2]) switch
		{
			("+", "old") => (Int128 worry) => worry + worry,
			("+", _) => (Int128 worry) => worry + value,
			("*", "old") => (Int128 worry) => worry * worry,
			("*", _) => (Int128 worry) => worry * value,
			_ => throw new DataException()
		};
		input.MoveNext();

		ret.DivisibleBy = Int128.Parse(input.Current.Split(" ").Last());
		input.MoveNext();

		ret.ThrowTrue = int.Parse(input.Current.Split(" ").Last());
		input.MoveNext();

		ret.ThrowFalse = int.Parse(input.Current.Split(" ").Last());
		input.MoveNext();

		return ret;
	}
}
