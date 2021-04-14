using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	internal sealed class StringAppSettingsEntry : AppSettingsEntry<string>
	{
		public StringAppSettingsEntry(string name, string defaultValue, Trace tracer) : base(name, defaultValue, tracer)
		{
		}

		protected override bool TryParseValue(string inputValue, out string outputValue)
		{
			outputValue = inputValue;
			return true;
		}
	}
}
