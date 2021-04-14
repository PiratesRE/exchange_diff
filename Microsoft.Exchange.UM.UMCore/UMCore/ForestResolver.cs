using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class ForestResolver : ICallHandler
	{
		public void HandleCall(CafeRoutingContext context)
		{
			ValidateArgument.NotNull(context, "RoutingContext");
			context.Tracer.Trace("ForestResolver : TryHandleCall", new object[0]);
			if (!CommonConstants.UseDataCenterCallRouting)
			{
				context.Tracer.Trace("ForestResolver: Enterprise Call", new object[0]);
				context.ScopedADConfigurationSession = ADSystemConfigurationLookupFactory.CreateFromRootOrg(true);
				return;
			}
			if (context.IsAccessProxyCall)
			{
				this.HandleLyncAPCall(context);
				return;
			}
			this.HandleSBCCall(context);
		}

		private void HandleLyncAPCall(CafeRoutingContext context)
		{
			context.Tracer.Trace("ForestResolver : HandleLyncAPCall. RequestUri={0}", new object[]
			{
				context.CallInfo.RequestUri
			});
			string parameterValue = context.CallInfo.RequestUri.FindParameter("ms-organization");
			bool allowMultipleEntries = context.CallInfo.DiversionInfo.Count > 0 || context.CallInfo.RequestUri.UserParameter != UserParameter.Phone;
			string[] array = SipRoutingHelper.ParseMsOrganizationParameter(parameterValue, context.CallInfo.RequestUri.User, allowMultipleEntries);
			string text;
			OrganizationId organizationIdFromTenantDomainList = SipRoutingHelper.GetOrganizationIdFromTenantDomainList(array, out text);
			if (null != organizationIdFromTenantDomainList)
			{
				context.Tracer.Trace("ForestResolver : HandleLyncAPCall: Organization {0} is hosted in this pod.", new object[]
				{
					organizationIdFromTenantDomainList
				});
				context.ScopedADConfigurationSession = ADSystemConfigurationLookupFactory.CreateFromOrganizationId(organizationIdFromTenantDomainList);
				context.TenantGuid = context.ScopedADConfigurationSession.GetExternalDirectoryOrganizationId();
				return;
			}
			throw CallRejectedException.Create(Strings.InvalidRequest, CallEndingReason.MsOrganizationCannotFindTenant, "Value: {0}.", new object[]
			{
				array[0]
			});
		}

		private void HandleSBCCall(CafeRoutingContext context)
		{
			PlatformSignalingHeader platformSignalingHeader = null;
			string g = string.Empty;
			RouterUtils.TryGetHeader(context.CallInfo.RemoteHeaders, "To", out platformSignalingHeader);
			PlatformSipUri platformSipUri = platformSignalingHeader.ParseUri();
			context.Tracer.Trace("ForestResolver : HandleSBCCall: ToUri recvd = {0}", new object[]
			{
				platformSipUri
			});
			int num = platformSipUri.Host.IndexOf('.');
			if (num <= 0)
			{
				throw CallRejectedException.Create(Strings.ToHeaderDoesNotContainTenantGuid(context.CallInfo.CallId, platformSipUri.ToString()), CallEndingReason.OnPremRoutingNotCorrectlyConfigured, null, new object[0]);
			}
			g = platformSipUri.Host.Substring(0, num);
			Guid empty = Guid.Empty;
			if (!GuidHelper.TryParseGuid(g, out empty))
			{
				throw CallRejectedException.Create(Strings.ToHeaderDoesNotContainTenantGuid(context.CallInfo.CallId, platformSipUri.ToString()), CallEndingReason.OnPremRoutingNotCorrectlyConfigured, null, new object[0]);
			}
			string text = string.Format(CultureInfo.InvariantCulture, context.CallRouterConfiguration.UMForwardingAddressTemplate, new object[]
			{
				empty.ToString("D")
			});
			context.Tracer.Trace("ForestResolver : HandleSBCCall: ToUri Host recvd = {0}", new object[]
			{
				platformSipUri.Host
			});
			context.Tracer.Trace("ForestResolver : HandleSBCCall: tmpHost= {0}", new object[]
			{
				text
			});
			if (!string.Equals(text, platformSipUri.Host, StringComparison.OrdinalIgnoreCase))
			{
				throw CallRejectedException.Create(Strings.ToHeaderDoesNotContainTenantGuid(context.CallInfo.CallId, platformSipUri.ToString()), CallEndingReason.OnPremRoutingNotCorrectlyConfigured, null, new object[0]);
			}
			try
			{
				IADSystemConfigurationLookup scopedADConfigurationSession = null;
				if (!this.TryLookupTenantByTenantGuid(context, empty, out scopedADConfigurationSession))
				{
					context.Tracer.Trace("ForestResolver : HandleSBCCall: Could not map tenant guid '{0}' to a tenant", new object[]
					{
						empty
					});
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_CallRouterUnableToMapGatewayToForest, null, new object[]
					{
						context.CallInfo.CallId,
						empty.ToString("D")
					});
					throw CallRejectedException.Create(Strings.PartnerGatewayNotFoundError, CallEndingReason.GatewaylookupIssue, null, new object[0]);
				}
				context.Tracer.Trace("ForestResolver : HandleSBCCall: Tenant '{0}' found", new object[]
				{
					empty
				});
				context.ScopedADConfigurationSession = scopedADConfigurationSession;
				context.TenantGuid = empty;
			}
			catch (Exception ex)
			{
				context.Tracer.Trace("ForestResolver : HandleSBCCall: Lookup for tenant object with guid '{0}' failed: '{1}'", new object[]
				{
					empty,
					ex
				});
				throw;
			}
		}

		private bool TryLookupTenantByTenantGuid(CafeRoutingContext context, Guid guid, out IADSystemConfigurationLookup scopedADLookup)
		{
			scopedADLookup = null;
			try
			{
				scopedADLookup = ADSystemConfigurationLookupFactory.CreateFromTenantGuid(guid);
			}
			catch (InvalidTenantGuidException ex)
			{
				context.Tracer.Trace("ForestResolver : TryLookupTenantByTenantGuid: InvalidTenantGuidException for '{0}': '{1}'", new object[]
				{
					guid,
					ex
				});
			}
			return scopedADLookup != null;
		}
	}
}
