using System;
using System.Globalization;
using System.Net;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.StoreDriver
{
	[Serializable]
	internal class ShadowSubmissionInfo : SubmissionInfo
	{
		public ShadowSubmissionInfo(string serverDN, string serverFqdn, IPAddress networkAddress, Guid mdbGuid, bool isShadowSupported, DateTime originalCreateTime, string mailboxHopLatency, string internetMessageId, string sender, long contentHash) : base(serverDN, serverFqdn, networkAddress, mdbGuid, isShadowSupported, originalCreateTime, mailboxHopLatency)
		{
			this.internetMessageId = internetMessageId;
			this.sender = sender;
			base.ContentHash = contentHash;
		}

		public string InternetMessageId
		{
			get
			{
				return this.internetMessageId;
			}
		}

		public string Sender
		{
			get
			{
				return this.sender;
			}
		}

		public override bool IsShadowSubmission
		{
			get
			{
				return true;
			}
		}

		public override SubmissionItem CreateSubmissionItem(MailItemSubmitter context)
		{
			throw new NotImplementedException();
		}

		public override OrganizationId GetOrganizationId()
		{
			OrganizationId result;
			ADOperationResult adoperationResult = MultiTenantTransport.TryGetOrganizationId(new RoutingAddress(this.sender), out result);
			bool succeeded = adoperationResult.Succeeded;
			return result;
		}

		public override SenderGuidTraceFilter GetTraceFilter()
		{
			return default(SenderGuidTraceFilter);
		}

		public override string GetPoisonId()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
			{
				this.sender,
				this.internetMessageId
			});
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Sender {0}, InternetMessageId {1}", new object[]
			{
				this.sender,
				this.internetMessageId
			});
		}

		private readonly string internetMessageId;

		private readonly string sender;
	}
}
