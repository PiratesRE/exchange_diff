using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.CalendarDiagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct CalendarVersionStoreQueryPolicy
	{
		public TimeSpan WaitTimeForPopulation
		{
			get
			{
				return this.waitTime;
			}
		}

		public bool WaitForPopulation
		{
			get
			{
				return !this.WaitTimeForPopulation.Equals(TimeSpan.Zero);
			}
		}

		public CalendarVersionStoreQueryPolicy(TimeSpan waitTimeForPopulation)
		{
			this.waitTime = waitTimeForPopulation;
		}

		private TimeSpan waitTime;

		public static readonly TimeSpan DefaultWaitTimeForPopulation = TimeSpan.FromMinutes(2.0);
	}
}
