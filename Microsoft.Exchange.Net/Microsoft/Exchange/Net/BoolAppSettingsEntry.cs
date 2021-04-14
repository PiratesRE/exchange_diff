using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	internal sealed class BoolAppSettingsEntry : AppSettingsEntry<bool>
	{
		public BoolAppSettingsEntry(string name, bool defaultValue, Trace tracer) : base(name, defaultValue, tracer)
		{
		}

		protected override bool TryParseValue(string inputValue, out bool outputValue)
		{
			int num;
			if (int.TryParse(inputValue, out num))
			{
				outputValue = (num != 0);
				return true;
			}
			bool flag;
			if (bool.TryParse(inputValue, out flag))
			{
				outputValue = flag;
				return true;
			}
			outputValue = false;
			return false;
		}
	}
}
