using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

interface Item
{
	public string Name { get; set; }
	
	public long Size { get; set; }
}

class DirItem : Item
{
	public string Name { get; set; }
	
	public List<Item> Items { get; set; }
	
	public long Size { get; set; }
}

class FileItem : Item
{
	public string Name { get; set; }
	public long Size { get; set; }
}
