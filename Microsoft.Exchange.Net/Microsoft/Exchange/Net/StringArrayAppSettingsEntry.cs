using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	internal sealed class StringArrayAppSettingsEntry : AppSettingsEntry<string[]>
	{
		public StringArrayAppSettingsEntry(string name, string[] defaultValue, Trace tracer) : base(name, defaultValue, tracer)
		{
		}

		protected override bool TryParseValue(string inputValue, out string[] outputValue)
		{
			outputValue = inputValue.Split(StringArrayAppSettingsEntry.separator, StringSplitOptions.RemoveEmptyEntries);
			return true;
		}

		private static char[] separator = new char[]
		{
			','
		};
	}
}
