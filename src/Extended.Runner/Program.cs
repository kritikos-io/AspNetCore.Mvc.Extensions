namespace Kritikos.Extended.Runner
{
	using System;

	public sealed class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			var par = string.Join(' ', args);
			Console.WriteLine($"Arguments: {par}");
		}
	}
}
