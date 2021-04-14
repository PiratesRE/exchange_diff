using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	internal sealed class TimeSpanAppSettingsEntry : AppSettingsEntry<TimeSpan>
	{
		public TimeSpanAppSettingsEntry(string name, TimeSpanUnit unit, TimeSpan defaultValue, Trace tracer) : base(name, defaultValue, tracer)
		{
			this.unit = unit;
		}

		protected override bool TryParseValue(string inputValue, out TimeSpan outputValue)
		{
			int num;
			if (int.TryParse(inputValue, out num))
			{
				switch (this.unit)
				{
				case TimeSpanUnit.Seconds:
					outputValue = TimeSpan.FromSeconds((double)num);
					return true;
				case TimeSpanUnit.Minutes:
					outputValue = TimeSpan.FromMinutes((double)num);
					return true;
				case TimeSpanUnit.Hours:
					outputValue = TimeSpan.FromHours((double)num);
					return true;
				case TimeSpanUnit.Days:
					outputValue = TimeSpan.FromDays((double)num);
					return true;
				}
			}
			outputValue = TimeSpan.Zero;
			return false;
		}

		private TimeSpanUnit unit;
	}
}
