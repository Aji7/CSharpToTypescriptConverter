namespace CSharpToTypescriptConverter;

internal class Program
{
	static void Main(string[] args)
	{
		Console.WriteLine("This program accepts a C# object definition and outputs its typescript equivalent.");
		Console.WriteLine("You may paste it directly to the console or type it line by line. Hit Ctrl + Z to denote the end of the definition.");

		var converter = new TypescriptConverter();

		do
		{
			Console.WriteLine("\nEnter a valid C# object definition:\n");

			var csharpClass = Console.In.ReadToEnd();

			Console.WriteLine("\nEquivalent interface/s in Typescript: \n");
			Console.WriteLine(converter.ConvertToTypescriptInterface(csharpClass));

			Console.Write("Convert another? (Y/N): ");
		}
		while (string.Equals(Console.ReadLine(), "Y", StringComparison.InvariantCultureIgnoreCase));
	}
}