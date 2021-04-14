using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Search.Core.Diagnostics
{
	internal struct OperatorTimingEntry
	{
		public static string SerializeList(ICollection<OperatorTimingEntry> entries, List<string> entryNames)
		{
			StringBuilder stringBuilder = new StringBuilder(30 * entries.Count);
			foreach (OperatorTimingEntry operatorTimingEntry in entries)
			{
				if (entryNames.Count == 0 || entryNames.Contains(operatorTimingEntry.Name))
				{
					stringBuilder.AppendFormat("{0}{1},{2},{3}", new object[]
					{
						(stringBuilder.Length == 0) ? string.Empty : ";",
						operatorTimingEntry.Name,
						(int)operatorTimingEntry.Location,
						operatorTimingEntry.Elapsed
					});
				}
			}
			return stringBuilder.ToString();
		}

		public static List<OperatorTimingEntry> DeserializeList(string serializedString)
		{
			string[] array = serializedString.Split(OperatorTimingEntry.ListSplitSeparators, StringSplitOptions.RemoveEmptyEntries);
			List<OperatorTimingEntry> list = new List<OperatorTimingEntry>(array.Length);
			foreach (string text in array)
			{
				string[] array3 = text.Split(OperatorTimingEntry.FieldSplitSeparators, StringSplitOptions.RemoveEmptyEntries);
				if (array3.Length != 3)
				{
					throw new FormatException("Wrong number of fields");
				}
				OperatorTimingEntry item = new OperatorTimingEntry
				{
					Name = array3[0],
					Location = (OperatorLocation)int.Parse(array3[1]),
					Elapsed = long.Parse(array3[2])
				};
				list.Add(item);
			}
			return list;
		}

		public string Name;

		public OperatorLocation Location;

		public long Elapsed;

		private static readonly char[] ListSplitSeparators = new char[]
		{
			';'
		};

		private static readonly char[] FieldSplitSeparators = new char[]
		{
			','
		};
	}
}
