using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public struct StartStopKey
	{
		public StartStopKey(bool inclusive, IList<object> values)
		{
			this.values = values;
			this.inclusive = inclusive;
		}

		public StartStopKey(bool inclusive, params object[] values)
		{
			this = new StartStopKey(inclusive, (IList<object>)values);
		}

		public static void AppendKeyValuesToStringBuilder(StringBuilder sb, StringFormatOptions formatOptions, IList<object> keyValues)
		{
			int num = (keyValues != null) ? keyValues.Count : 0;
			for (int i = 0; i < num; i++)
			{
				if (i != 0)
				{
					sb.Append(", ");
				}
				sb.Append("[");
				if ((formatOptions & StringFormatOptions.SkipParametersData) == StringFormatOptions.None)
				{
					if ((formatOptions & StringFormatOptions.IncludeDetails) == StringFormatOptions.IncludeDetails || !(keyValues[i] is byte[]) || ((byte[])keyValues[i]).Length <= 32)
					{
						sb.AppendAsString(keyValues[i]);
					}
					else
					{
						sb.Append("<long blob>");
					}
				}
				else
				{
					sb.AppendAsString((keyValues[i] != null) ? "X" : "Null");
				}
				sb.Append("]");
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.values == null || this.values.Count == 0;
			}
		}

		public int Count
		{
			get
			{
				if (this.values != null)
				{
					return this.values.Count;
				}
				return 0;
			}
		}

		public IList<object> Values
		{
			get
			{
				return this.values;
			}
		}

		public bool Inclusive
		{
			get
			{
				return this.Count == 0 || this.inclusive;
			}
		}

		internal static int CommonKeyPrefix(StartStopKey startKey, StartStopKey stopKey, CompareInfo compareInfo)
		{
			return StartStopKey.CommonKeyPrefix(startKey.Values, stopKey.Values, compareInfo);
		}

		internal static int CommonKeyPrefix(IList<object> first, IList<object> second, CompareInfo compareInfo)
		{
			if (!object.ReferenceEquals(first, second))
			{
				int num = Math.Min((first == null) ? 0 : first.Count, (second == null) ? 0 : second.Count);
				for (int i = 0; i < num; i++)
				{
					if (!ValueHelper.ValuesEqual(first[i], second[i], compareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth))
					{
						return i;
					}
				}
				return num;
			}
			if (first != null)
			{
				return first.Count;
			}
			return 0;
		}

		internal void AppendToStringBuilder(StringBuilder sb, StringFormatOptions formatOptions)
		{
			if (this.IsEmpty)
			{
				sb.Append("<Empty>");
				return;
			}
			sb.Append(this.inclusive ? "inclusive:[" : "exclusive:[");
			StartStopKey.AppendKeyValuesToStringBuilder(sb, formatOptions, this.values);
			sb.Append("]");
		}

		public new string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			this.AppendToStringBuilder(stringBuilder, StringFormatOptions.IncludeDetails);
			return stringBuilder.ToString();
		}

		public static readonly StartStopKey Empty = default(StartStopKey);

		private readonly IList<object> values;

		private bool inclusive;
	}
}
