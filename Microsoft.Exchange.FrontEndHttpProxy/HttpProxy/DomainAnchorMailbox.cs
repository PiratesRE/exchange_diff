using System;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.HttpProxy
{
	internal class DomainAnchorMailbox : UserBasedAnchorMailbox
	{
		public DomainAnchorMailbox(string domain, IRequestContext requestContext) : this(AnchorSource.Domain, domain, requestContext)
		{
		}

		protected DomainAnchorMailbox(AnchorSource anchorSource, object sourceObject, IRequestContext requestContext) : base(AnchorSource.Domain, sourceObject, requestContext)
		{
		}

		public virtual string Domain
		{
			get
			{
				return (string)base.SourceObject;
			}
		}

		public override string GetOrganizationNameForLogging()
		{
			string organizationNameForLogging = base.GetOrganizationNameForLogging();
			if (string.IsNullOrEmpty(organizationNameForLogging))
			{
				return this.Domain;
			}
			return organizationNameForLogging;
		}

		protected override ADRawEntry LoadADRawEntry()
		{
			IRecipientSession session = this.GetDomainRecipientSession();
			ADRawEntry ret = DirectoryHelper.InvokeAccountForest(base.RequestContext.LatencyTracker, () => HttpProxyBackEndHelper.GetDefaultOrganizationMailbox(session, this.ToCookieKey()));
			return base.CheckForNullAndThrowIfApplicable<ADRawEntry>(ret);
		}

		protected IRecipientSession GetDomainRecipientSession()
		{
			IRecipientSession result = null;
			try
			{
				Guid externalOrgId = new Guid(this.Domain);
				result = DirectoryHelper.GetRecipientSessionFromExternalDirectoryOrganizationId(base.RequestContext.LatencyTracker, externalOrgId);
			}
			catch (FormatException)
			{
				result = DirectoryHelper.GetRecipientSessionFromDomain(base.RequestContext.LatencyTracker, this.Domain, false);
			}
			return result;
		}
	}
}
