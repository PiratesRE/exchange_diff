using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	internal sealed class IntAppSettingsEntry : AppSettingsEntry<int>
	{
		public IntAppSettingsEntry(string name, int defaultValue, Trace tracer) : base(name, defaultValue, tracer)
		{
		}

		protected override bool TryParseValue(string inputValue, out int outputValue)
		{
			return int.TryParse(inputValue, out outputValue);
		}
	}
}
