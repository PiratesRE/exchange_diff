using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal static class ExtensionMethods
	{
		public static List<TOutput> ConvertAll<TInput, TOutput>(this ICollection<TInput> inputCollection, Converter<TInput, TOutput> converter)
		{
			List<TOutput> list = null;
			if (inputCollection != null)
			{
				list = new List<TOutput>(inputCollection.Count);
				foreach (TInput input in inputCollection)
				{
					list.Add(converter(input));
				}
			}
			return list;
		}

		public static List<TOutput> ConvertAll<TInput, TOutput>(this IEnumerable<TInput> inputCollection, Converter<TInput, TOutput> converter)
		{
			List<TOutput> list = null;
			if (inputCollection != null)
			{
				list = new List<TOutput>();
				foreach (TInput input in inputCollection)
				{
					list.Add(converter(input));
				}
			}
			return list;
		}

		public static string ToString<T>(this IList<T> list, string delimiter)
		{
			ValidateArgument.NotNull(delimiter, "delimiter");
			StringBuilder stringBuilder = new StringBuilder();
			foreach (T t in list)
			{
				object value = t;
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(delimiter);
				}
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}
	}
}
