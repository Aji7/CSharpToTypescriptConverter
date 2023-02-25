using System.Text.RegularExpressions;

namespace CSharpToTypescriptConverter;

internal class TypescriptConverter
{
	internal string ConvertToTypescriptInterface(string csharpClass)
	{
		var rows = csharpClass
			.Replace("public ", "")
			.Replace("int", "number")
			.Replace("long", "number")
			.Replace("class", "export interface")
			.Replace(" { get; set; }", "")
			.Replace("?", "")
			.Trim()
			.Split("\n")
			.ToList();

		for (int i = 0; i < rows.Count; i++)
		{
			if (rows[i].Contains("List"))
			{
				rows[i] = this.FormatTypescriptCollection(rows[i]);
			}
			else if (rows[i].Contains("number"))
			{
				rows[i] = this.FormatTypescriptProperty(rows[i], "number");
			}
			else if (rows[i].Contains("string"))
			{
				rows[i] = this.FormatTypescriptProperty(rows[i], "string");
			}
		}

		this.ExtractNestedClasses(rows);

		var tsInterface = "";
		foreach (var row in rows)
		{
			if (row.Contains("export"))
			{
				tsInterface += row.TrimEnd();
				tsInterface += row.Contains('{') ? "\n" : " {\n";
				continue;
			}

			if (row.Contains('{'))
			{
				continue;
			}

			tsInterface += $"{row}\n";
		}

		return tsInterface;
	}

	private string FormatTypescriptProperty(string row, string tsType)
	{
		var tabs = row[..row.IndexOf(tsType)];
		var propertyName = row.Trim().Split(" ")[1];

		return $"{tabs}{char.ToLower(propertyName[0])}{propertyName[1..]}: {tsType};";
	}

	private string FormatTypescriptCollection(string row)
	{
		var tabs = row[..row.IndexOf("List")];
		var typeAndPropertyName = row
			.Replace("<", "")
			.Replace(">", "")
			.Trim()
			.Split(" ");

		var nestLevel = Regex.Matches(typeAndPropertyName[0], "List").Count;
		var typeName = typeAndPropertyName[0].Replace("List", "");
		var propertyName = typeAndPropertyName[1];

		return $"{tabs}{char.ToLower(propertyName[0])}{propertyName[1..]}: {typeName}{string.Concat(Enumerable.Repeat("[]", nestLevel))};";
	}

	private void ExtractNestedClasses(List<string> rows)
	{
		var nestedClasses = new List<List<string>>();
		var firstNestedClassIndex = -1;

		for (int i = rows.Count - 2; i >= 0; i--)
		{
			if (!rows[i].Contains('}'))
			{
				continue;
			}

			var closingBracketIndex = i;

			while (!rows[i].Contains("export"))
			{
				i--;
			}

			nestedClasses.Add(rows.GetRange(i, closingBracketIndex - i + 1));
			firstNestedClassIndex = i;
		}

		if (firstNestedClassIndex != -1)
		{
			rows.RemoveRange(firstNestedClassIndex, rows.Count - firstNestedClassIndex);
			rows.Add("}");
		}

		foreach (var nestedClass in nestedClasses)
		{
			for (int i = 0; i < nestedClass.Count; i++)
			{
				nestedClass[i] = nestedClass[i][1..];
			}

			rows.AddRange(nestedClass);
		}
	}
}
