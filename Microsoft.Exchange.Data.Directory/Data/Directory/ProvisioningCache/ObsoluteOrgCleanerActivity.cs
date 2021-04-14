using System;
using System.Threading;

namespace Microsoft.Exchange.Data.Directory.ProvisioningCache
{
	internal class ObsoluteOrgCleanerActivity : Activity
	{
		public ObsoluteOrgCleanerActivity(ProvisioningCache cache) : base(cache)
		{
		}

		public override string Name
		{
			get
			{
				return "Expired organizations cleaner";
			}
		}

		protected override void InternalExecute()
		{
			while (!base.GotStopSignalFromTestCode)
			{
				Thread.Sleep(ObsoluteOrgCleanerActivity.cleanUpInterval);
				base.ProvisioningCache.ClearExpireOrganizations();
			}
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		internal override void StopExecute()
		{
			base.StopExecute();
			if (base.GotStopSignalFromTestCode && base.AsyncThread.ThreadState != ThreadState.Stopped && base.AsyncThread.ThreadState != ThreadState.Aborted)
			{
				base.AsyncThread.Abort();
				base.AsyncThread.Join();
			}
		}

		private static readonly TimeSpan cleanUpInterval = new TimeSpan(2, 0, 0);
	}
}
