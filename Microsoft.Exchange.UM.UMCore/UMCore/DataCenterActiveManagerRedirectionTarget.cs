using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class DataCenterActiveManagerRedirectionTarget : DataCenterLegacySupportRedirectionTarget
	{
		protected override RedirectionTarget.ResultSet GetBackEndBrickRedirectionTarget(ADUser user, IRoutingContext context)
		{
			DatabaseLocationInfo databaseLocationInfo = null;
			if (user.Database != null)
			{
				try
				{
					ActiveManager activeManager = base.InvokeWithStopwatch<ActiveManager>("ActiveManager.GetCachingActiveManagerInstance", () => ActiveManager.GetCachingActiveManagerInstance());
					databaseLocationInfo = base.InvokeWithStopwatch<DatabaseLocationInfo>("ActiveManager.GetServerForDatabase", () => activeManager.GetServerForDatabase(user.Database.ObjectGuid, true));
					goto IL_D1;
				}
				catch (DatabaseNotFoundException)
				{
					PIIMessage data = PIIMessage.Create(PIIType._PII, user.DistinguishedName);
					CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, data, "GetBackEndBrickRedirectionTarget: ActiveManager threw DatabaseNotFoundException for user '_PII'", new object[0]);
					databaseLocationInfo = null;
					goto IL_D1;
				}
			}
			PIIMessage data2 = PIIMessage.Create(PIIType._PII, user.DistinguishedName);
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, data2, "GetBackEndBrickRedirectionTarget: Database property is null for user '_PII'", new object[0]);
			IL_D1:
			if (databaseLocationInfo == null || string.IsNullOrEmpty(databaseLocationInfo.ServerFqdn))
			{
				PIIMessage data3 = PIIMessage.Create(PIIType._PII, user.DistinguishedName);
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, data3, "GetBackEndBrickRedirectionTarget: mbx db not found in local forest using ActiveManager for user '_PII'", new object[0]);
				throw CallRejectedException.Create(Strings.ErrorLookingUpActiveMailboxServer(user.DistinguishedName, context.CallId), CallEndingReason.ADError, null, new object[0]);
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "GetBackEndBrickRedirectionTarget: ActiveManager determined server fqdn is {0}", new object[]
			{
				databaseLocationInfo.ServerFqdn
			});
			if (!CommonUtil.IsServerCompatible(databaseLocationInfo.ServerVersion))
			{
				using (UMRecipient umrecipient = new UMRecipient(user))
				{
					return this.GetLocalForestLegacyRedirectionTarget(umrecipient, context);
				}
			}
			string text;
			int port;
			base.GetRoutingInformation(context.TenantGuid, databaseLocationInfo.ServerFqdn, context.IsSecuredCall, out text, out port);
			return new RedirectionTarget.ResultSet(RouterUtils.GetRedirectContactUri(context.RequestUriOfCall, context.RoutingHelper, text, port, context.IsSecuredCall ? TransportParameter.Tls : TransportParameter.Tcp, context.TenantGuid.ToString()), text, port);
		}
	}
}
