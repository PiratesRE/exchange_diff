using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public static class QuarantinedJobs
	{
		private static IDictionary<Guid, FailureRec> Jobs
		{
			get
			{
				return QuarantinedJobs.quarantinedJobs.Value;
			}
		}

		public static bool Enabled
		{
			get
			{
				return QuarantinedJobs.enabled.Value;
			}
		}

		public static void Add(Guid requestGuid, Exception ex)
		{
			if (!QuarantinedJobs.Enabled)
			{
				return;
			}
			JobQuarantineProvider.Instance.QuarantineJob(requestGuid, ex);
			lock (QuarantinedJobs.syncRoot)
			{
				QuarantinedJobs.Jobs.Add(requestGuid, FailureRec.Create(ex));
			}
		}

		public static void Remove(Guid requestGuid)
		{
			if (!QuarantinedJobs.Enabled)
			{
				return;
			}
			JobQuarantineProvider.Instance.UnquarantineJob(requestGuid);
			lock (QuarantinedJobs.syncRoot)
			{
				QuarantinedJobs.Jobs.Remove(requestGuid);
			}
		}

		public static bool Contains(Guid requestGuid)
		{
			if (!QuarantinedJobs.Enabled)
			{
				return false;
			}
			bool result;
			lock (QuarantinedJobs.syncRoot)
			{
				result = QuarantinedJobs.Jobs.ContainsKey(requestGuid);
			}
			return result;
		}

		public static FailureRec Get(Guid requestGuid)
		{
			if (!QuarantinedJobs.Enabled)
			{
				return null;
			}
			FailureRec result;
			lock (QuarantinedJobs.syncRoot)
			{
				FailureRec failureRec = null;
				QuarantinedJobs.Jobs.TryGetValue(requestGuid, out failureRec);
				result = failureRec;
			}
			return result;
		}

		private static readonly object syncRoot = new object();

		private static readonly Lazy<bool> enabled = new Lazy<bool>(() => ConfigBase<MRSConfigSchema>.GetConfig<bool>("QuarantineEnabled"));

		private static readonly Lazy<IDictionary<Guid, FailureRec>> quarantinedJobs = new Lazy<IDictionary<Guid, FailureRec>>(delegate()
		{
			if (QuarantinedJobs.Enabled)
			{
				return JobQuarantineProvider.Instance.GetQuarantinedJobs();
			}
			return null;
		});
	}
}
