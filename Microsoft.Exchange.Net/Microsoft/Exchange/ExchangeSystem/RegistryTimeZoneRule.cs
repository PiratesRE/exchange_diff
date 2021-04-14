using System;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.ExchangeSystem
{
	internal sealed class RegistryTimeZoneRule
	{
		public RegistryTimeZoneRule(NativeMethods.SystemTime start, REG_TIMEZONE_INFO regTimeZoneInfo)
		{
			this.Start = new DateTime((int)start.Year, (int)start.Month, (int)start.Day, (int)start.Hour, (int)start.Minute, (int)start.Second, (int)start.Milliseconds);
			this.RegTimeZoneInfo = regTimeZoneInfo;
		}

		public RegistryTimeZoneRule(DateTime start, REG_TIMEZONE_INFO regTimeZoneInfo)
		{
			this.Start = start;
			this.RegTimeZoneInfo = regTimeZoneInfo;
		}

		public RegistryTimeZoneRule(int year, REG_TIMEZONE_INFO regTimeZoneInfo) : this(new DateTime(year, 1, 1), regTimeZoneInfo)
		{
		}

		public readonly DateTime Start;

		public readonly REG_TIMEZONE_INFO RegTimeZoneInfo;
	}
}
