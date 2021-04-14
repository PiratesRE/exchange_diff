using System;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.HttpProxy.Common
{
	internal static class HttpProxySettings
	{
		public static string Prefix(string appSettingName)
		{
			return "HttpProxy." + appSettingName;
		}

		public static readonly BoolAppSettingsEntry CafeV2Enabled = new BoolAppSettingsEntry(HttpProxySettings.Prefix("CafeV2Enabled"), VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Cafe.CafeV2.Enabled, ExTraceGlobals.VerboseTracer);

		public static readonly BoolAppSettingsEntry AddressFinderEnabled = new BoolAppSettingsEntry(HttpProxySettings.Prefix("AddressFinderEnabled"), HttpProxySettings.CafeV2Enabled.Value, ExTraceGlobals.VerboseTracer);

		public static readonly bool AnonymousRequestFilterEnabled = new BoolAppSettingsEntry(HttpProxySettings.Prefix("AnonymousRequestFilterEnabled"), true, ExTraceGlobals.VerboseTracer).Value;

		public static readonly BoolAppSettingsEntry ProxyAssistantEnabled = new BoolAppSettingsEntry(HttpProxySettings.Prefix("ProxyAssistantEnabled"), HttpProxySettings.CafeV2Enabled.Value, ExTraceGlobals.VerboseTracer);

		public static readonly BoolAppSettingsEntry NativeProxyEnabled = new BoolAppSettingsEntry("NativeHttpProxy.Enabled", HttpProxySettings.CafeV2Enabled.Value, ExTraceGlobals.VerboseTracer);

		public static readonly BoolAppSettingsEntry RouteSelectorEnabled = new BoolAppSettingsEntry(HttpProxySettings.Prefix("RouteSelectorEnabled"), HttpProxySettings.CafeV2Enabled.Value, ExTraceGlobals.VerboseTracer);

		public static readonly BoolAppSettingsEntry DiagnosticsEnabled = new BoolAppSettingsEntry(HttpProxySettings.Prefix("DiagnosticsEnabled"), HttpProxySettings.CafeV2Enabled.Value, ExTraceGlobals.VerboseTracer);

		public static readonly BoolAppSettingsEntry RouteRefresherEnabled = new BoolAppSettingsEntry(HttpProxySettings.Prefix("RouteRefresherEnabled"), HttpProxySettings.CafeV2Enabled.Value, ExTraceGlobals.VerboseTracer);

		public static readonly BoolAppSettingsEntry PartitionedRoutingEnabled = new BoolAppSettingsEntry(HttpProxySettings.Prefix("PartitionedRoutingEnabled"), VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Cafe.PartitionedRouting.Enabled, ExTraceGlobals.VerboseTracer);

		public static readonly EnumAppSettingsEntry<ProtocolType> ProtocolTypeAppSetting = new EnumAppSettingsEntry<ProtocolType>(HttpProxySettings.Prefix("ProtocolType"), ProtocolType.Unknown, ExTraceGlobals.VerboseTracer);

		public static readonly BoolAppSettingsEntry OnlyProxySecureConnectionsAppSetting = new BoolAppSettingsEntry(HttpProxySettings.Prefix("OnlyProxySecureConnections"), false, ExTraceGlobals.VerboseTracer);

		public static readonly BoolAppSettingsEntry NegativeAnchorMailboxCacheEnabled = new BoolAppSettingsEntry(HttpProxySettings.Prefix("NegativeAnchorMailboxCacheEnabled"), false, ExTraceGlobals.VerboseTracer);

		public static readonly BoolAppSettingsEntry TrustClientXForwardedFor = new BoolAppSettingsEntry(HttpProxySettings.Prefix("TrustClientXForwardedFor"), VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Cafe.TrustClientXForwardedFor.Enabled, ExTraceGlobals.VerboseTracer);
	}
}
