using System;
using System.Net;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriver;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.StoreDriver
{
	[Serializable]
	internal abstract class SubmissionInfo
	{
		protected SubmissionInfo(string serverDN, string serverFqdn, IPAddress networkAddress, Guid mdbGuid, bool isShadowSupported, DateTime originalCreateTime, string mailboxHopLatency)
		{
			this.serverDN = serverDN;
			this.serverFqdn = serverFqdn;
			this.networkAddress = networkAddress;
			this.mdbGuid = mdbGuid;
			this.isShadowSupported = isShadowSupported;
			this.originalCreateTime = originalCreateTime;
			this.mailboxHopLatency = mailboxHopLatency;
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

		public bool IsShadowSupported
		{
			get
			{
				return this.isShadowSupported;
			}
		}

		public DateTime OriginalCreateTime
		{
			get
			{
				return this.originalCreateTime;
			}
		}

		public string MailboxHopLatency
		{
			get
			{
				return this.mailboxHopLatency;
			}
		}

		public long ContentHash
		{
			get
			{
				return this.contentHash;
			}
			set
			{
				this.contentHash = value;
			}
		}

		public abstract bool IsShadowSubmission { get; }

		internal string DatabaseName { get; set; }

		public abstract SubmissionItem CreateSubmissionItem(MailItemSubmitter context);

		public abstract OrganizationId GetOrganizationId();

		public abstract SenderGuidTraceFilter GetTraceFilter();

		public abstract string GetPoisonId();

		protected static readonly Trace Diag = ExTraceGlobals.MapiSubmitTracer;

		private readonly string serverDN;

		private readonly string serverFqdn;

		private readonly IPAddress networkAddress;

		private readonly Guid mdbGuid;

		private readonly bool isShadowSupported;

		private long contentHash;

		private DateTime originalCreateTime;

		private string mailboxHopLatency;

		internal enum Event
		{
			StoreDriverPoisonMessage,
			StoreDriverPoisonMessageInSubmission,
			FailedToGenerateNdrInSubmission,
			InvalidSender
		}
	}
}
