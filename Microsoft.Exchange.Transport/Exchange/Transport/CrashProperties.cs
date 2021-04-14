using System;

namespace Microsoft.Exchange.Transport
{
	internal class CrashProperties
	{
		public CrashProperties(double crashCount, DateTime lastCrashTime)
		{
			this.crashCount = crashCount;
			this.lastCrashTime = lastCrashTime;
		}

		internal double CrashCount
		{
			get
			{
				return this.crashCount;
			}
		}

		internal DateTime LastCrashTime
		{
			get
			{
				return this.lastCrashTime;
			}
		}

		private readonly double crashCount;

		private readonly DateTime lastCrashTime;
	}
}
