using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal class ResourceEventCounterCrashInfo
	{
		public ResourceEventCounterCrashInfo(SortedSet<DateTime> crashTimes, bool isPoisonNdrSent)
		{
			this.crashTimes = crashTimes;
			this.IsPoisonNdrSent = isPoisonNdrSent;
		}

		public bool IsPoisonNdrSent { get; set; }

		public SortedSet<DateTime> CrashTimes
		{
			get
			{
				return this.crashTimes;
			}
		}

		private readonly SortedSet<DateTime> crashTimes = new SortedSet<DateTime>();
	}
}
