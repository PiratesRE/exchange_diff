using System;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public class DiagnosticQueryTableFormatter : DiagnosticQueryStringFormatter
	{
		private DiagnosticQueryTableFormatter(DiagnosticQueryResults results) : base(results)
		{
		}

		public static DiagnosticQueryTableFormatter Create(DiagnosticQueryResults results)
		{
			return new DiagnosticQueryTableFormatter(results);
		}

		public static StringBuilder FormatException(DiagnosticQueryException e)
		{
			StringBuilder stringBuilder = new StringBuilder(e.Message.Length);
			string name = e.GetType().Name;
			string value = new string('-', name.Length);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine(name);
			stringBuilder.AppendLine(value);
			stringBuilder.AppendLine(e.Message);
			return stringBuilder;
		}

		protected override void WriteHeader()
		{
			if (base.Results.Names.Count > 0)
			{
				base.Builder.AppendLine();
				for (int i = 0; i < base.Results.Names.Count; i++)
				{
					string format = string.Format("{{0,-{0}}} ", base.Results.Widths[i]);
					base.Builder.AppendFormat(format, base.Results.Names[i]);
				}
				base.Builder.AppendLine();
				for (int j = 0; j < base.Results.Names.Count; j++)
				{
					base.Builder.AppendFormat("{0} ", new string('-', (int)base.Results.Widths[j]));
				}
				base.Builder.AppendLine();
			}
		}

		protected override void WriteValues()
		{
			foreach (object[] array in base.Results.Values)
			{
				for (int i = 0; i < array.Length; i++)
				{
					string format = string.Format("{{0,-{0}}} ", base.Results.Widths[i]);
					base.Builder.AppendFormat(format, DiagnosticQueryFormatter<StringBuilder>.FormatValue(array[i]));
				}
				base.Builder.AppendLine();
			}
		}
	}
}
