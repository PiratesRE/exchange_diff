using System;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.HttpProxy
{
	internal class PstProviderAnchorMailbox : UserBasedAnchorMailbox
	{
		public PstProviderAnchorMailbox(string pstFilePath, IRequestContext requestContext) : base(AnchorSource.GenericAnchorHint, pstFilePath, requestContext)
		{
		}

		protected override ADRawEntry LoadADRawEntry()
		{
			ADRawEntry ret = DirectoryHelper.InvokeAccountForest(base.RequestContext.LatencyTracker, () => HttpProxyBackEndHelper.GetOrganizationMailboxInClosestSite(OrganizationId.ForestWideOrgId, OrganizationCapability.PstProvider));
			return base.CheckForNullAndThrowIfApplicable<ADRawEntry>(ret);
		}
	}
}
