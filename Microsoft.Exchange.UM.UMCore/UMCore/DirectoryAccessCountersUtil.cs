using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class DirectoryAccessCountersUtil
	{
		protected DirectoryAccessCountersUtil(BaseUMCallSession vo)
		{
			this.session = vo;
		}

		protected BaseUMCallSession Session
		{
			get
			{
				return this.session;
			}
		}

		internal void Increment(DirectoryAccessCountersUtil.DirectoryAccessCounter counterName)
		{
			foreach (ExPerformanceCounter counter in this.GetCounters(counterName))
			{
				this.Session.IncrementCounter(counter);
			}
		}

		internal void IncrementSingleCounter(DirectoryAccessCountersUtil.DirectoryAccessCounter counterName)
		{
			this.Session.IncrementCounter(this.GetSingleCounter(counterName));
		}

		protected abstract List<ExPerformanceCounter> GetCounters(DirectoryAccessCountersUtil.DirectoryAccessCounter counter);

		protected abstract ExPerformanceCounter GetSingleCounter(DirectoryAccessCountersUtil.DirectoryAccessCounter counter);

		private BaseUMCallSession session;

		internal enum DirectoryAccessCounter
		{
			DirectoryAccess,
			DirectoryAccessedByExtension,
			DirectoryAccessedByDialByName,
			DirectoryAccessedSuccessfullyByDialByName,
			DirectoryAccessedBySpokenName,
			DirectoryAccessedSuccessfullyBySpokenName
		}
	}
}
