using System;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	internal sealed class ColumnIdParser
	{
		private ColumnIdParser()
		{
		}

		public static string GetString(ColumnId value)
		{
			return ColumnIdParser.columnId.GetString((int)value);
		}

		public static ColumnId Parse(string value)
		{
			object obj = ColumnIdParser.columnId.Parse(value);
			if (obj == null)
			{
				return ColumnId.Count;
			}
			return (ColumnId)obj;
		}

		private static FastEnumParser columnId = new FastEnumParser(typeof(ColumnId));
	}
}
