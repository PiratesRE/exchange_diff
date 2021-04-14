using System;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class DataCenterRedirectionTarget : RedirectionTarget
	{
		public override RedirectionTarget.ResultSet GetForCallAnsweringCall(UMRecipient user, IRoutingContext context)
		{
			this.ValidateUserArgs(user, context);
			return this.GetBackEndBrickRedirectionTarget(user.ADRecipient as ADUser, context);
		}

		public override RedirectionTarget.ResultSet GetForSubscriberAccessCall(UMRecipient user, IRoutingContext context)
		{
			this.ValidateUserArgs(user, context);
			return this.GetBackEndBrickRedirectionTarget(user.ADRecipient as ADUser, context);
		}

		public override RedirectionTarget.ResultSet GetForNonUserSpecificCall(OrganizationId orgId, IRoutingContext context)
		{
			ValidateArgument.NotNull(orgId, "orgId");
			ValidateArgument.NotNull(context, "context");
			ADUser organizationMailboxForRouting = GrammarMailboxFileStore.GetOrganizationMailboxForRouting(orgId);
			if (organizationMailboxForRouting == null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "GetForNonUserSpecificCall: failed to find E15 org mailbox", new object[0]);
				return this.HandleNoOrgMailboxForRouting(context);
			}
			return this.GetBackEndBrickRedirectionTarget(organizationMailboxForRouting, context);
		}

		protected virtual RedirectionTarget.ResultSet GetBackEndBrickRedirectionTarget(ADUser user, IRoutingContext context)
		{
			string text = null;
			try
			{
				using (MailboxServerLocator mbxLocator = this.InvokeWithStopwatch<MailboxServerLocator>("new MailboxServerLocator", () => MailboxServerLocator.Create(user.Database.ObjectGuid, user.PrimarySmtpAddress.Domain, user.Database.PartitionFQDN)))
				{
					try
					{
						IAsyncResult asyncResult = this.InvokeWithStopwatch<IAsyncResult>("MailboxLocator.BeginGetServer", () => mbxLocator.BeginGetServer(null, null));
						BackEndServer backEndServer = this.InvokeWithStopwatch<BackEndServer>("MailboxLocator.EndGetServer", delegate
						{
							asyncResult.AsyncWaitHandle.WaitOne();
							return mbxLocator.EndGetServer(asyncResult);
						});
						CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "GetBackEndBrickRedirectionTarget: MailboxServerLocator returned server {0}", new object[]
						{
							backEndServer.Fqdn
						});
						if (backEndServer.IsE15OrHigher)
						{
							text = backEndServer.Fqdn;
						}
						else
						{
							CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "GetBackEndBrickRedirectionTarget: legacy server in local forest", new object[0]);
							using (UMRecipient umrecipient = new UMRecipient(user))
							{
								return this.GetLocalForestLegacyRedirectionTarget(umrecipient, context);
							}
						}
					}
					catch (RemoteForestDownLevelServerException)
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "GetBackEndBrickRedirectionTarget: legacy server in remote forest", new object[0]);
						return this.GetRemoteForestLegacyRedirectionTarget(context);
					}
				}
			}
			catch (Exception ex)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "GetBackEndBrickRedirectionTarget: failed with exception {0}", new object[]
				{
					ex
				});
				throw;
			}
			if (text == null)
			{
				throw CallRejectedException.Create(Strings.ErrorLookingUpActiveMailboxServer(user.DistinguishedName, context.CallId), CallEndingReason.ADError, null, new object[0]);
			}
			string text2;
			int port;
			this.GetRoutingInformation(context.TenantGuid, text, context.IsSecuredCall, out text2, out port);
			return new RedirectionTarget.ResultSet(RouterUtils.GetRedirectContactUri(context.RequestUriOfCall, context.RoutingHelper, text2, port, context.IsSecuredCall ? TransportParameter.Tls : TransportParameter.Tcp, context.TenantGuid.ToString()), text2, port);
		}

		protected virtual RedirectionTarget.ResultSet HandleNoOrgMailboxForRouting(IRoutingContext context)
		{
			throw CallRejectedException.Create(Strings.NoGrammarCapableMailbox(context.TenantGuid.ToString(), context.CallId), CallEndingReason.NoOrgMailboxFound, null, new object[0]);
		}

		protected void ValidateUserArgs(UMRecipient recipient, IRoutingContext context)
		{
			ValidateArgument.NotNull(recipient, "recipient");
			ValidateArgument.NotNull(context, "context");
			ADUser aduser = recipient.ADRecipient as ADUser;
			ExAssert.RetailAssert(aduser != null, "Recipient is not ADUser");
			if (aduser.Database == null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "ValidateUserArgs: user database property is null", new object[0]);
			}
		}

		protected void GetRoutingInformation(Guid orgId, string serverFqdn, bool isSecuredCall, out string routingFqdn, out int routingPort)
		{
			routingPort = 5063;
			routingFqdn = serverFqdn;
			string text;
			int num;
			if (Utils.RunningInTestMode && this.GetRoutingFqdnAndPort(orgId, serverFqdn, out text, out num))
			{
				routingFqdn = text;
				routingPort = num;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "GetRoutingInformation: routing fqdn is {0}", new object[]
			{
				routingFqdn
			});
		}

		protected bool GetRoutingFqdnAndPort(Guid tenantGuid, string serverFqdn, out string routingFqdn, out int routingPort)
		{
			routingFqdn = null;
			routingPort = 0;
			ADTopologyLookup adtopologyLookup = ADTopologyLookup.CreateTenantResourceForestLookup(tenantGuid);
			Server serverFromName = adtopologyLookup.GetServerFromName(serverFqdn);
			if (serverFromName == null || !serverFromName.IsUnifiedMessagingServer)
			{
				throw CallRejectedException.Create(Strings.UMServerNotFoundinAD(serverFqdn), CallEndingReason.ADError, null, new object[0]);
			}
			UMServer umserver = new UMServer(serverFromName);
			if (umserver.ExternalHostFqdn == null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "GetRoutingHostFqdnAndPort: ExternalHostFqdn is null", new object[0]);
				return false;
			}
			routingFqdn = umserver.ExternalHostFqdn.ToString();
			routingPort = umserver.SipTlsListeningPort;
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "GetRoutingHostFqdnAndPort: ExternalHostFQDN = '{0}'  port = '{1}'", new object[]
			{
				routingFqdn,
				routingPort
			});
			return true;
		}

		protected virtual RedirectionTarget.ResultSet GetLocalForestLegacyRedirectionTarget(UMRecipient recipient, IRoutingContext context)
		{
			throw CallRejectedException.Create(Strings.LegacyMailboxesNotSupported(context.TenantGuid.ToString(), context.CallId), CallEndingReason.UnsupportedRequest, null, new object[0]);
		}

		protected virtual RedirectionTarget.ResultSet GetRemoteForestLegacyRedirectionTarget(IRoutingContext context)
		{
			throw CallRejectedException.Create(Strings.LegacyMailboxesNotSupported(context.TenantGuid.ToString(), context.CallId), CallEndingReason.UnsupportedRequest, null, new object[0]);
		}

		protected T InvokeWithStopwatch<T>(string operationName, Func<T> func)
		{
			return this.latencyStopwatch.Invoke<T>(operationName, func);
		}

		private LatencyStopwatch latencyStopwatch = new LatencyStopwatch();
	}
}
