using System;
using System.Net;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.HttpProxy.Routing;
using Microsoft.Exchange.HttpProxy.Routing.RoutingKeys;

namespace Microsoft.Exchange.HttpProxy
{
	internal class ExternalDirectoryObjectIdAnchorMailbox : UserBasedAnchorMailbox
	{
		public ExternalDirectoryObjectIdAnchorMailbox(string externalDirectoryObjectId, OrganizationId organizationId, IRequestContext requestContext) : base(AnchorSource.ExternalDirectoryObjectId, externalDirectoryObjectId, requestContext)
		{
			this.externalDirectoryObjectId = externalDirectoryObjectId;
			this.organizationId = organizationId;
			base.NotFoundExceptionCreator = delegate()
			{
				string message = string.Format("Cannot find mailbox by ExternalDirectoryObjectId={0} in organizationId={1}.", this.externalDirectoryObjectId, this.organizationId);
				return new HttpProxyException(HttpStatusCode.NotFound, HttpProxySubErrorCode.MailboxExternalDirectoryObjectIdNotFound, message);
			};
		}

		protected override ADRawEntry LoadADRawEntry()
		{
			IRecipientSession recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.organizationId), 62, "LoadADRawEntry", "f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\AnchorMailbox\\ExternalDirectoryObjectIdAnchorMailbox.cs");
			ADRawEntry ret = DirectoryHelper.InvokeAccountForest(base.RequestContext.LatencyTracker, () => recipientSession.FindADUserByExternalDirectoryObjectId(this.externalDirectoryObjectId));
			return base.CheckForNullAndThrowIfApplicable<ADRawEntry>(ret);
		}

		protected override IRoutingKey GetRoutingKey()
		{
			return new ExternalDirectoryObjectIdRoutingKey(Guid.Parse(this.externalDirectoryObjectId), Guid.Parse(this.organizationId.ToExternalDirectoryOrganizationId()));
		}

		private readonly string externalDirectoryObjectId;

		private readonly OrganizationId organizationId;
	}
}
