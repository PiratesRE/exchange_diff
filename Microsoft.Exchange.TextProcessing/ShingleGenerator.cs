using System;

namespace Microsoft.Exchange.TextProcessing
{
	internal class ShingleGenerator
	{
		public static string[] CollectShingles(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return new string[0];
			}
			string[] array = text.Split(ShingleGenerator.delimiters, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length < 2)
			{
				return new string[0];
			}
			string[] array2 = new string[array.Length - 1];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = array[i] + " " + array[i + 1];
			}
			return array2;
		}

		private static readonly char[] delimiters = new char[]
		{
			' ',
			'\t',
			'\r',
			'\n'
		};
	}
}
