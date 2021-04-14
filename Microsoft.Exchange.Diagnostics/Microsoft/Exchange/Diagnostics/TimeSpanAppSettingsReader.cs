using System;

namespace Microsoft.Exchange.Diagnostics
{
	public sealed class TimeSpanAppSettingsReader : AppSettingsReader<TimeSpan>
	{
		public TimeSpanAppSettingsReader(string name, TimeSpanUnits unit, TimeSpan defaultValue) : base(name, defaultValue)
		{
			this.unit = unit;
		}

		public static bool TryParseStringValue(string inputValue, TimeSpanUnits unit, out TimeSpan outputValue)
		{
			int num;
			if (int.TryParse(inputValue, out num))
			{
				switch (unit)
				{
				case TimeSpanUnits.Seconds:
					outputValue = TimeSpan.FromSeconds((double)num);
					return true;
				case TimeSpanUnits.Minutes:
					outputValue = TimeSpan.FromMinutes((double)num);
					return true;
				case TimeSpanUnits.Hours:
					outputValue = TimeSpan.FromHours((double)num);
					return true;
				case TimeSpanUnits.Days:
					outputValue = TimeSpan.FromDays((double)num);
					return true;
				}
			}
			outputValue = TimeSpan.Zero;
			return false;
		}

		protected override bool TryParseValue(string inputValue, out TimeSpan outputValue)
		{
			return TimeSpanAppSettingsReader.TryParseStringValue(inputValue, this.unit, out outputValue);
		}

		private TimeSpanUnits unit;
	}
}
