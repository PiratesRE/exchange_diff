using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.InfoWorker.Common.Availability;

namespace Microsoft.Exchange.InfoWorker.Common.MailTips
{
	internal sealed class MailTipsPerRequesterPermissionMap
	{
		internal ExternalClientContext ExternalClientContext { get; private set; }

		internal MailTipsPerRequesterPermissionMap(ClientContext clientContext, int recipientCount, Trace tracer, int traceId)
		{
			this.ExternalClientContext = (clientContext as ExternalClientContext);
			if (this.ExternalClientContext != null)
			{
				this.permissionMap = new Dictionary<OrganizationId, MailTipsPermission>(recipientCount);
			}
			this.tracer = tracer;
			this.traceId = traceId;
		}

		internal MailTipsPermission Lookup(RecipientData recipientData)
		{
			if (this.ExternalClientContext == null)
			{
				this.tracer.TraceDebug<object, EmailAddress>((long)this.traceId, "{0}: InternalMailTipsPermission used for {1} because requester is not external", TraceContext.Get(), recipientData.EmailAddress);
				return MailTipsPermission.AllAccess;
			}
			if (recipientData.IsEmpty)
			{
				this.tracer.TraceDebug<object, EmailAddress>((long)this.traceId, "{0}: InternalMailTipsPermission used for {1} because recipient did not resolve in AD", TraceContext.Get(), recipientData.EmailAddress);
				return MailTipsPermission.AllAccess;
			}
			MailTipsPermission mailTipsPermission;
			if (this.permissionMap.TryGetValue(recipientData.OrganizationId, out mailTipsPermission))
			{
				return mailTipsPermission;
			}
			OrganizationRelationship organizationRelationship = FreeBusyPermission.GetOrganizationRelationship(recipientData.OrganizationId, this.ExternalClientContext.EmailAddress.Domain);
			if (organizationRelationship == null || !organizationRelationship.Enabled)
			{
				this.tracer.TraceDebug<object, OrganizationId, string>((long)this.traceId, "{0}: No organization relationship found in organization {1} for domain {2}", TraceContext.Get(), recipientData.OrganizationId, this.ExternalClientContext.EmailAddress.Domain);
				return MailTipsPermission.NoAccess;
			}
			bool requesterInAccessScope = false;
			if (organizationRelationship.MailTipsAccessScope == null)
			{
				requesterInAccessScope = true;
			}
			else if (organizationRelationship.MailTipsAccessEnabled && organizationRelationship.MailTipsAccessLevel != MailTipsAccessLevel.None)
			{
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(recipientData.OrganizationId), 127, "Lookup", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\MailTips\\MailTipsPerRequesterPermissionMap.cs");
				ADGroup adgroup = tenantOrRootOrgRecipientSession.Read(organizationRelationship.MailTipsAccessScope) as ADGroup;
				if (adgroup == null)
				{
					this.tracer.TraceError<object, OrganizationId, ADObjectId>((long)this.traceId, "{0}: OrganizationRelationship for organization {1} has invalid MailTipsAccessScope {2} which cannot be resolved in ad as an ADGroup", TraceContext.Get(), recipientData.OrganizationId, organizationRelationship.MailTipsAccessScope);
				}
				else if (adgroup.ContainsMember(recipientData.Id, false))
				{
					this.tracer.TraceDebug((long)this.traceId, "{0}: {1} is a member of MailTipsAccessScope {2} for OrganizationRelationship of organization {3}", new object[]
					{
						TraceContext.Get(),
						recipientData.EmailAddress,
						organizationRelationship.MailTipsAccessScope,
						recipientData.OrganizationId
					});
					requesterInAccessScope = true;
				}
				else
				{
					this.tracer.TraceDebug((long)this.traceId, "{0}: {1} is not a member of MailTipsAccessScope {2} for OrganizationRelationship of organization {3}", new object[]
					{
						TraceContext.Get(),
						recipientData.EmailAddress,
						organizationRelationship.MailTipsAccessScope,
						recipientData.OrganizationId
					});
				}
			}
			mailTipsPermission = new MailTipsPermission(organizationRelationship.MailTipsAccessEnabled, organizationRelationship.MailTipsAccessLevel, requesterInAccessScope);
			this.permissionMap[recipientData.OrganizationId] = mailTipsPermission;
			return mailTipsPermission;
		}

		private Dictionary<OrganizationId, MailTipsPermission> permissionMap;

		private Trace tracer;

		private int traceId;
	}
}
