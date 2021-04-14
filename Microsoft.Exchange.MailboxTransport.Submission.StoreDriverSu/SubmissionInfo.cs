using System;
using System.Net;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	[Serializable]
	internal abstract class SubmissionInfo
	{
		protected SubmissionInfo(string serverDN, string serverFqdn, IPAddress networkAddress, Guid mdbGuid, string databaseName, DateTime originalCreateTime, TenantPartitionHint tenantHint, string mailboxHopLatency, LatencyTracker latencyTracker, bool shouldDeprioritize, bool shouldThrottle)
		{
			this.serverDN = serverDN;
			this.serverFqdn = serverFqdn;
			this.networkAddress = networkAddress;
			this.mdbGuid = mdbGuid;
			this.databaseName = databaseName;
			this.originalCreateTime = originalCreateTime;
			this.tenantHint = tenantHint;
			this.mailboxHopLatency = mailboxHopLatency;
			this.latencyTracker = latencyTracker;
			this.shouldDeprioritize = shouldDeprioritize;
			this.shouldThrottle = shouldThrottle;
		}

		public string MailboxServerDN
		{
			get
			{
				return this.serverDN;
			}
		}

		public string MailboxFqdn
		{
			get
			{
				return this.serverFqdn;
			}
		}

		public IPAddress NetworkAddress
		{
			get
			{
				return this.networkAddress;
			}
		}

		public Guid MdbGuid
		{
			get
			{
				return this.mdbGuid;
			}
		}

		public DateTime OriginalCreateTime
		{
			get
			{
				return this.originalCreateTime;
			}
		}

		public TenantPartitionHint TenantHint
		{
			get
			{
				return this.tenantHint;
			}
		}

		public string MailboxHopLatency
		{
			get
			{
				return this.mailboxHopLatency;
			}
		}

		public LatencyTracker LatencyTracker
		{
			get
			{
				return this.latencyTracker;
			}
		}

		public string DatabaseName
		{
			get
			{
				return this.databaseName;
			}
		}

		public bool ShouldDeprioritize
		{
			get
			{
				return this.shouldDeprioritize;
			}
		}

		public bool ShouldThrottle
		{
			get
			{
				return this.shouldThrottle;
			}
		}

		public abstract SubmissionItem CreateSubmissionItem(MailItemSubmitter context);

		public abstract OrganizationId GetOrganizationId();

		public abstract SenderGuidTraceFilter GetTraceFilter();

		public abstract SubmissionPoisonContext GetPoisonContext();

		public abstract void LogEvent(SubmissionInfo.Event submissionInfoEvent);

		public abstract void LogEvent(SubmissionInfo.Event submissionInfoEvent, Exception exception);

		private readonly string serverDN;

		private readonly string serverFqdn;

		private readonly IPAddress networkAddress;

		private readonly Guid mdbGuid;

		private readonly string databaseName;

		private readonly DateTime originalCreateTime;

		private readonly TenantPartitionHint tenantHint;

		private readonly string mailboxHopLatency;

		private readonly LatencyTracker latencyTracker;

		private readonly bool shouldDeprioritize;

		private readonly bool shouldThrottle;

		internal enum Event
		{
			StoreDriverSubmissionPoisonMessage,
			StoreDriverSubmissionPoisonMessageInSubmission,
			FailedToGenerateNdrInSubmission,
			InvalidSender
		}
	}
}
