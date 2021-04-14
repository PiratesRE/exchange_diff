using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class EnterpriseRedirectionTarget : RedirectionTarget
	{
		public override RedirectionTarget.ResultSet GetForCallAnsweringCall(UMRecipient user, IRoutingContext context)
		{
			ValidateArgument.NotNull(user, "user");
			ValidateArgument.NotNull(context, "context");
			ADUser aduser = user.ADRecipient as ADUser;
			ExAssert.RetailAssert(aduser != null, "Recipient is not ADUser");
			if (user.RequiresLegacyRedirectForCallAnswering)
			{
				return this.GetLegacyRedirectionTarget(user, context);
			}
			return this.GetBackEndBrickRedirectionTarget(aduser, context);
		}

		public override RedirectionTarget.ResultSet GetForSubscriberAccessCall(UMRecipient user, IRoutingContext context)
		{
			ValidateArgument.NotNull(user, "user");
			ValidateArgument.NotNull(context, "context");
			ADUser aduser = user.ADRecipient as ADUser;
			ExAssert.RetailAssert(aduser != null, "Recipient is not ADUser");
			if (user.RequiresLegacyRedirectForSubscriberAccess)
			{
				return this.GetLegacyRedirectionTarget(user, context);
			}
			return this.GetBackEndBrickRedirectionTarget(aduser, context);
		}

		public override RedirectionTarget.ResultSet GetForNonUserSpecificCall(OrganizationId orgId, IRoutingContext context)
		{
			ValidateArgument.NotNull(orgId, "orgId");
			ValidateArgument.NotNull(context, "context");
			ADUser organizationMailboxForRouting = GrammarMailboxFileStore.GetOrganizationMailboxForRouting(orgId);
			if (organizationMailboxForRouting == null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "GetForNonUserSpecificCall: failed to find E15 org mailbox", new object[0]);
				throw CallRejectedException.Create(Strings.NoGrammarCapableMailbox(orgId.ToString(), context.CallId), CallEndingReason.NoOrgMailboxFound, null, new object[0]);
			}
			return this.GetBackEndBrickRedirectionTarget(organizationMailboxForRouting, context);
		}

		private RedirectionTarget.ResultSet GetLegacyRedirectionTarget(UMRecipient user, IRoutingContext context)
		{
			string text = null;
			IRedirectTargetChooser redirectTargetChooser = new LegacyUMServerChooser(context.DialPlan, context.IsSecuredCall, user);
			int port;
			if (!redirectTargetChooser.GetTargetServer(out text, out port))
			{
				redirectTargetChooser.HandleServerNotFound();
				throw CallRejectedException.Create(Strings.NoValidLegacyServer(context.CallId, redirectTargetChooser.SubscriberLogId), CallEndingReason.UserRoutingIssue, null, new object[0]);
			}
			return new RedirectionTarget.ResultSet(RouterUtils.GetRedirectContactUri(context.RequestUriOfCall, context.RoutingHelper, text, port, context.IsSecuredCall ? TransportParameter.Tls : TransportParameter.Tcp, string.Empty), text, port);
		}

		private RedirectionTarget.ResultSet GetBackEndBrickRedirectionTarget(ADUser user, IRoutingContext context)
		{
			ActiveManager cachingActiveManagerInstance = ActiveManager.GetCachingActiveManagerInstance();
			DatabaseLocationInfo databaseLocationInfo = null;
			if (user.Database != null)
			{
				databaseLocationInfo = cachingActiveManagerInstance.GetServerForDatabase(user.Database.ObjectGuid, true);
			}
			if (databaseLocationInfo == null || string.IsNullOrEmpty(databaseLocationInfo.ServerFqdn))
			{
				throw CallRejectedException.Create(Strings.ErrorLookingUpActiveMailboxServer(user.DistinguishedName, context.CallId), CallEndingReason.ADError, null, new object[0]);
			}
			string text;
			int port;
			this.GetRoutingInformation(databaseLocationInfo.ServerFqdn, context.IsSecuredCall, out text, out port);
			return new RedirectionTarget.ResultSet(RouterUtils.GetRedirectContactUri(context.RequestUriOfCall, context.RoutingHelper, text, port, context.IsSecuredCall ? TransportParameter.Tls : TransportParameter.Tcp, context.TenantGuid.ToString()), text, port);
		}

		protected void GetRoutingInformation(string serverFqdn, bool isSecuredCall, out string routingFqdn, out int routingPort)
		{
			routingFqdn = serverFqdn;
			routingPort = 0;
			ADTopologyLookup adtopologyLookup = ADTopologyLookup.CreateLocalResourceForestLookup();
			Server serverFromName = adtopologyLookup.GetServerFromName(serverFqdn);
			if (serverFromName != null && serverFromName.IsUnifiedMessagingServer)
			{
				UMServer umserver = new UMServer(serverFromName);
				routingPort = (isSecuredCall ? umserver.SipTlsListeningPort : umserver.SipTcpListeningPort);
				return;
			}
			throw CallRejectedException.Create(Strings.UMServerNotFoundinAD(serverFqdn), CallEndingReason.ADError, null, new object[0]);
		}
	}
}
