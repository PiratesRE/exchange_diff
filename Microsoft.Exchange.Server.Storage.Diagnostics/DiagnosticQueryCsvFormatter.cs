using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public class DiagnosticQueryCsvFormatter : DiagnosticQueryStringFormatter
	{
		private DiagnosticQueryCsvFormatter(DiagnosticQueryResults results) : base(results)
		{
		}

		private IList<object[]> Values
		{
			get
			{
				if (base.Results.Values.Count == 0)
				{
					string[] emptyStringArray = DiagnosticQueryCsvFormatter.GetEmptyStringArray(base.Results.Names.Count);
					return new object[][]
					{
						emptyStringArray
					};
				}
				return base.Results.Values;
			}
		}

		public static DiagnosticQueryCsvFormatter Create(DiagnosticQueryResults results)
		{
			return new DiagnosticQueryCsvFormatter(results);
		}

		protected override void WriteHeader()
		{
			if (base.Results.Names.Count > 0)
			{
				for (int i = 0; i < base.Results.Names.Count; i++)
				{
					string format = string.Format("{0}\"{{0}}\"", (i == 0) ? string.Empty : ",");
					base.Builder.AppendFormat(format, base.Results.Names[i]);
				}
				base.Builder.AppendLine();
			}
		}

		protected override void WriteValues()
		{
			foreach (object[] array in this.Values)
			{
				for (int i = 0; i < array.Length; i++)
				{
					string format = string.Format("{0}\"{{0}}\"", (i == 0) ? string.Empty : ",");
					string text = array[i] as string;
					if (text != null)
					{
						base.Builder.AppendFormat(format, DiagnosticQueryFormatter<StringBuilder>.FormatValue(text.Replace("\"", "\"\"")));
					}
					else
					{
						base.Builder.AppendFormat(format, DiagnosticQueryFormatter<StringBuilder>.FormatValue(array[i]));
					}
				}
				base.Builder.AppendLine();
			}
		}

		private static string[] GetEmptyStringArray(int numberOfValues)
		{
			char[] array = new char[]
			{
				','
			};
			return new string(array[0], numberOfValues - 1).Split(array, StringSplitOptions.None);
		}
	}
}
