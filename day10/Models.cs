using System;

namespace day10;

public class CPU
{
	public long X = 1;
	public long Cycle = 0;
	public long IP = 0;

	public void NOP()
	{
		Cycle += 1;
		OnCycle(this);
		IP++;
	}

	public void AddX(long op)
	{
		Cycle += 1;
		OnCycle(this);
		Cycle += 1;
		OnCycle(this);
		X += op;
		IP++;
	}

	public Action<CPU> OnCycle;
}
