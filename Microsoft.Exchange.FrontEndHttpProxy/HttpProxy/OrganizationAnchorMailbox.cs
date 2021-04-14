using System;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.HttpProxy
{
	internal class OrganizationAnchorMailbox : UserBasedAnchorMailbox
	{
		public OrganizationAnchorMailbox(OrganizationId orgId, IRequestContext requestContext) : base(AnchorSource.OrganizationId, orgId, requestContext)
		{
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return (OrganizationId)base.SourceObject;
			}
		}

		public override string ToString()
		{
			return string.Format("{0}~{1}", base.AnchorSource, this.ToCookieKey());
		}

		public override string ToCookieKey()
		{
			string arg = string.Empty;
			if (this.OrganizationId.ConfigurationUnit != null)
			{
				arg = this.OrganizationId.ConfigurationUnit.Parent.Name;
			}
			return string.Format("{0}@{1}", OrganizationAnchorMailbox.OrganizationAnchor, arg);
		}

		public override string GetOrganizationNameForLogging()
		{
			return this.OrganizationId.GetFriendlyName();
		}

		protected override ADRawEntry LoadADRawEntry()
		{
			IRecipientSession session = DirectoryHelper.GetRecipientSessionFromOrganizationId(base.RequestContext.LatencyTracker, this.OrganizationId);
			ADRawEntry ret = DirectoryHelper.InvokeAccountForest(base.RequestContext.LatencyTracker, () => HttpProxyBackEndHelper.GetDefaultOrganizationMailbox(session, this.ToCookieKey()));
			return base.CheckForNullAndThrowIfApplicable<ADRawEntry>(ret);
		}

		private static readonly string OrganizationAnchor = "OrganizationAnchor";
	}
}
