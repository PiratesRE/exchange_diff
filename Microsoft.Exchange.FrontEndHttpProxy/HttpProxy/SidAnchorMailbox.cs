using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.HttpProxy
{
	internal class SidAnchorMailbox : UserBasedAnchorMailbox
	{
		public SidAnchorMailbox(SecurityIdentifier sid, IRequestContext requestContext) : base(AnchorSource.Sid, sid, requestContext)
		{
		}

		public SidAnchorMailbox(string sid, IRequestContext requestContext) : this(new SecurityIdentifier(sid), requestContext)
		{
		}

		public SecurityIdentifier Sid
		{
			get
			{
				return (SecurityIdentifier)base.SourceObject;
			}
		}

		public OrganizationId OrganizationId { get; set; }

		public string SmtpOrLiveId { get; set; }

		public string PartitionId { get; set; }

		public override string GetOrganizationNameForLogging()
		{
			if (this.OrganizationId != null)
			{
				return this.OrganizationId.GetFriendlyName();
			}
			return base.GetOrganizationNameForLogging();
		}

		protected override ADRawEntry LoadADRawEntry()
		{
			IRecipientSession session = null;
			if (this.OrganizationId != null)
			{
				session = DirectoryHelper.GetRecipientSessionFromOrganizationId(base.RequestContext.LatencyTracker, this.OrganizationId);
			}
			else if (this.PartitionId != null)
			{
				session = DirectoryHelper.GetRecipientSessionFromPartition(base.RequestContext.LatencyTracker, this.PartitionId);
			}
			else if (this.SmtpOrLiveId != null)
			{
				session = DirectoryHelper.GetRecipientSessionFromSmtpOrLiveId(base.RequestContext.LatencyTracker, this.SmtpOrLiveId, false);
			}
			else
			{
				session = DirectoryHelper.GetRootOrgRecipientSession();
			}
			ADRawEntry ret = DirectoryHelper.InvokeAccountForest(base.RequestContext.LatencyTracker, () => session.FindADRawEntryBySid(this.Sid, this.PropertySet));
			return base.CheckForNullAndThrowIfApplicable<ADRawEntry>(ret);
		}
	}
}
