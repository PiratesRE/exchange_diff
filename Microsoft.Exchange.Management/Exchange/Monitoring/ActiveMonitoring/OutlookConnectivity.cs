using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring.Management.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	internal static class OutlookConnectivity
	{
		internal static ProbeIdentity ResolveIdentity(string identityParameter, bool isDcOrDedicated)
		{
			IEnumerable<ProbeIdentity> enumerable = OutlookConnectivity.AllProbes;
			ProbeIdentity probeIdentity = null;
			if ((!MonitoringItemIdentity.MonitorIdentityId.IsValidFormat(identityParameter) || !ExchangeComponent.WellKnownComponents.ContainsKey(MonitoringItemIdentity.MonitorIdentityId.GetHealthSet(identityParameter))) && !MonitoringItemIdentity.MonitorIdentityId.IsValidFormat(identityParameter = "\\" + identityParameter))
			{
				throw new ArgumentException(Strings.InvalidOutlookProbeIdentity(identityParameter));
			}
			string healthSetLookup = MonitoringItemIdentity.MonitorIdentityId.GetHealthSet(identityParameter);
			if (!string.IsNullOrEmpty(healthSetLookup))
			{
				if (ExchangeComponent.WellKnownComponents.ContainsKey(healthSetLookup))
				{
					enumerable = from probe in enumerable
					where probe.Component.Name.Equals(healthSetLookup, StringComparison.InvariantCultureIgnoreCase)
					select probe;
				}
				else
				{
					identityParameter = "\\" + identityParameter;
				}
			}
			string monitor = MonitoringItemIdentity.MonitorIdentityId.GetMonitor(identityParameter);
			string targetResource = MonitoringItemIdentity.MonitorIdentityId.GetTargetResource(identityParameter);
			foreach (ProbeIdentity probeIdentity2 in enumerable)
			{
				if (probeIdentity2.Name.IndexOf(monitor, StringComparison.InvariantCultureIgnoreCase) >= 0)
				{
					if (probeIdentity != null)
					{
						throw new ArgumentException(Strings.AmbiguousOutlookProbeIdentity(identityParameter, probeIdentity.ToString(), probeIdentity2.ForTargetResource(targetResource).ToString()));
					}
					probeIdentity = probeIdentity2.ForTargetResource(targetResource);
				}
			}
			if (probeIdentity != null && isDcOrDedicated && probeIdentity.Name.IndexOf("CTP", StringComparison.InvariantCultureIgnoreCase) >= 0)
			{
				probeIdentity = null;
			}
			if (probeIdentity == null)
			{
				throw new ArgumentException(Strings.OutlookProbeIdentityNotFound(identityParameter));
			}
			return probeIdentity;
		}

		internal const string RpcArchiveScenario = "OutlookLogonToArchiveRpc";

		internal const string MailboxRpcScenario = "Rpc";

		internal const string MapiHttpArchiveScenario = "OutlookLogonToArchiveMapiHttp";

		public const string RpcProxyApplicationPoolName = "MSExchangeRpcProxyAppPool";

		public const string RpcProxyFrontEndApplicationPoolName = "MSExchangeRpcProxyFrontEndAppPool";

		public static readonly ProbeIdentity DeepTest = ProbeIdentity.Create(ExchangeComponent.OutlookProtocol, ProbeType.DeepTest, "Rpc", null);

		public static readonly ProbeIdentity MapiHttpDeepTest = ProbeIdentity.Create(ExchangeComponent.OutlookMapiHttpProtocol, ProbeType.DeepTest, null, null);

		public static readonly ProbeIdentity RpcSelfTest = ProbeIdentity.Create(ExchangeComponent.OutlookProtocol, ProbeType.SelfTest, "Rpc", null);

		public static readonly ProbeIdentity MapiHttpSelfTest = ProbeIdentity.Create(ExchangeComponent.OutlookMapiHttpProtocol, ProbeType.SelfTest, null, null);

		public static readonly ProbeIdentity ProxyTest = ProbeIdentity.Create(ExchangeComponent.OutlookProxy, ProbeType.ProxyTest, null, "MSExchangeRpcProxyFrontEndAppPool");

		public static readonly ProbeIdentity Ctp = ProbeIdentity.Create(ExchangeComponent.Outlook, ProbeType.Ctp, "Rpc", null);

		public static readonly ProbeIdentity MapiHttpCtp = ProbeIdentity.Create(ExchangeComponent.OutlookMapiHttp, ProbeType.Ctp, null, null);

		public static readonly ProbeIdentity ArchiveCtp = ProbeIdentity.Create(ExchangeComponent.Compliance, ProbeType.Ctp, "OutlookLogonToArchiveRpc", null);

		public static readonly ProbeIdentity MapiHttpArchiveCtp = ProbeIdentity.Create(ExchangeComponent.Compliance, ProbeType.Ctp, "OutlookLogonToArchiveMapiHttp", null);

		public static readonly ProbeIdentity[] AllProbes = new ProbeIdentity[]
		{
			OutlookConnectivity.DeepTest,
			OutlookConnectivity.RpcSelfTest,
			OutlookConnectivity.ProxyTest,
			OutlookConnectivity.Ctp,
			OutlookConnectivity.ArchiveCtp,
			OutlookConnectivity.MapiHttpDeepTest,
			OutlookConnectivity.MapiHttpSelfTest,
			OutlookConnectivity.MapiHttpCtp,
			OutlookConnectivity.MapiHttpArchiveCtp
		};
	}
}
