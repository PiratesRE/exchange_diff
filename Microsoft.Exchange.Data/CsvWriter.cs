using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Exchange.Data
{
	internal static class CsvWriter
	{
		public static void WriteCsvLine(this StreamWriter streamWriter, IEnumerable<string> columnData)
		{
			bool flag = false;
			foreach (string text in columnData)
			{
				bool flag2;
				bool flag3;
				if (string.IsNullOrEmpty(text))
				{
					flag2 = false;
					flag3 = false;
				}
				else
				{
					flag2 = text.Contains("\"");
					flag3 = (flag2 || text.Contains(",") || text.Contains("\r") || text.Contains("\n") || char.IsWhiteSpace(text[0]) || char.IsWhiteSpace(text[text.Length - 1]));
				}
				if (flag)
				{
					streamWriter.Write(',');
				}
				if (flag3)
				{
					streamWriter.Write('"');
				}
				if (flag2)
				{
					streamWriter.Write(text.Replace("\"", "\"\""));
				}
				else
				{
					streamWriter.Write(text);
				}
				if (flag3)
				{
					streamWriter.Write('"');
				}
				flag = true;
			}
			streamWriter.WriteLine();
		}
	}
}
