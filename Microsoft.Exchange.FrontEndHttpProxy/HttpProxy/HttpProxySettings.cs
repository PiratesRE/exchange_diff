using System;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.HttpProxy
{
	internal static class HttpProxySettings
	{
		public static string Prefix(string appSettingName)
		{
			return HttpProxySettings.Prefix(appSettingName);
		}

		private static int GetBufferBoundary(BufferPoolCollection.BufferSize bufferSize)
		{
			int num = 1024;
			string text = bufferSize.ToString();
			char c = text[text.Length - 1];
			if (c == 'M')
			{
				num *= 1024;
			}
			else if (c != 'K')
			{
				throw new ArgumentException(string.Format("BufferSize format for BufferSize value {0} is not supported", bufferSize));
			}
			return Convert.ToInt32(text.Substring(4, text.Length - 5)) * num;
		}

		public static readonly BoolAppSettingsEntry WriteDiagnosticHeaders = new BoolAppSettingsEntry(HttpProxySettings.Prefix("WriteDiagnosticHeaders"), true, ExTraceGlobals.VerboseTracer);

		public static readonly BoolAppSettingsEntry UseDefaultWebProxy = new BoolAppSettingsEntry(HttpProxySettings.Prefix("UseDefaultWebProxy"), false, ExTraceGlobals.VerboseTracer);

		public static readonly BoolAppSettingsEntry UseSmartBufferSizing = new BoolAppSettingsEntry(HttpProxySettings.Prefix("UseSmartBufferSizing"), true, ExTraceGlobals.VerboseTracer);

		public static readonly EnumAppSettingsEntry<BufferPoolCollection.BufferSize> RequestBufferSize = new EnumAppSettingsEntry<BufferPoolCollection.BufferSize>(HttpProxySettings.Prefix("RequestBufferSize"), BufferPoolCollection.BufferSize.Size32K, ExTraceGlobals.VerboseTracer);

		public static readonly EnumAppSettingsEntry<BufferPoolCollection.BufferSize> MinimumRequestBufferSize = new EnumAppSettingsEntry<BufferPoolCollection.BufferSize>(HttpProxySettings.Prefix("MinimumRequestBufferSize"), BufferPoolCollection.BufferSize.Size4K, ExTraceGlobals.VerboseTracer);

		public static readonly EnumAppSettingsEntry<BufferPoolCollection.BufferSize> ResponseBufferSize = new EnumAppSettingsEntry<BufferPoolCollection.BufferSize>(HttpProxySettings.Prefix("ResponseBufferSize"), BufferPoolCollection.BufferSize.Size32K, ExTraceGlobals.VerboseTracer);

		public static readonly EnumAppSettingsEntry<BufferPoolCollection.BufferSize> MinimumResponseBufferSize = new EnumAppSettingsEntry<BufferPoolCollection.BufferSize>(HttpProxySettings.Prefix("MinimumResponseBufferSize"), BufferPoolCollection.BufferSize.Size4K, ExTraceGlobals.VerboseTracer);

		public static readonly LazyMember<long> RequestBufferBoundary = new LazyMember<long>(() => (long)HttpProxySettings.GetBufferBoundary(HttpProxySettings.RequestBufferSize.Value));

		public static readonly LazyMember<long> ResponseBufferBoundary = new LazyMember<long>(() => (long)HttpProxySettings.GetBufferBoundary(HttpProxySettings.ResponseBufferSize.Value));

		public static readonly BoolAppSettingsEntry TestBackEndSupportEnabled = new BoolAppSettingsEntry(HttpProxySettings.Prefix("TestBackEndSupportEnabled"), false, ExTraceGlobals.VerboseTracer);

		public static readonly IntAppSettingsEntry SerializeClientAccessContext = new IntAppSettingsEntry(HttpProxySettings.Prefix("SerializeClientAccessContext"), 0, ExTraceGlobals.VerboseTracer);

		public static readonly BoolAppSettingsEntry DFPOWAVdirProxyEnabled = new BoolAppSettingsEntry("DFPOWAProxyEnabled", false, ExTraceGlobals.VerboseTracer);

		public static readonly StringAppSettingsEntry CaptureResponsesLocation = new StringAppSettingsEntry(HttpProxySettings.Prefix("CaptureResponsesLocation"), string.Empty, ExTraceGlobals.VerboseTracer);

		public static readonly IntAppSettingsEntry ServicePointConnectionLimit = new IntAppSettingsEntry(HttpProxySettings.Prefix("ServicePointConnectionLimit"), 65000, ExTraceGlobals.VerboseTracer);

		public static readonly IntAppSettingsEntry DelayProbeResponseSeconds = new IntAppSettingsEntry(HttpProxySettings.Prefix("DelayProbeResponseSeconds"), 0, ExTraceGlobals.VerboseTracer);

		public static readonly BoolAppSettingsEntry DetailedLatencyTracingEnabled = new BoolAppSettingsEntry(HttpProxySettings.Prefix("DetailedLatencyTracingEnabled"), false, ExTraceGlobals.VerboseTracer);

		public static readonly IntAppSettingsEntry MaxRetryOnError = new IntAppSettingsEntry(HttpProxySettings.Prefix("MaxRetryOnError"), VariantConfiguration.InvariantNoFlightingSnapshot.Cafe.RetryOnError.Enabled ? 2 : 0, ExTraceGlobals.VerboseTracer);

		public static readonly BoolAppSettingsEntry RetryOnConnectivityErrorEnabled = new BoolAppSettingsEntry(HttpProxySettings.Prefix("RetryOnConnectivityErrorEnabled"), false, ExTraceGlobals.VerboseTracer);

		public static readonly IntAppSettingsEntry DelayOnRetryOnError = new IntAppSettingsEntry(HttpProxySettings.Prefix("DelayOnRetryOnError"), 5000, ExTraceGlobals.VerboseTracer);

		public static readonly BoolAppSettingsEntry MailboxServerLocatorSharedCacheEnabled = new BoolAppSettingsEntry(HttpProxySettings.Prefix("MailboxServerLocatorSharedCacheEnabled"), HttpProxyGlobals.IsMultitenant && VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Cafe.MailboxServerSharedCache.Enabled, ExTraceGlobals.VerboseTracer);

		public static readonly StringAppSettingsEntry EnableLiveIdBasicBEAuthVersion = new StringAppSettingsEntry("LiveIdBasicAuthModule.EnableBEAuthVersion", string.Empty, ExTraceGlobals.VerboseTracer);

		public static readonly StringAppSettingsEntry EnableOAuthBEAuthVersion = new StringAppSettingsEntry("OAuthHttpModule.EnableBEAuthVersion", string.Empty, ExTraceGlobals.VerboseTracer);

		public static readonly BoolAppSettingsEntry AnchorMailboxSharedCacheEnabled = new BoolAppSettingsEntry(HttpProxySettings.Prefix("AnchorMailboxSharedCacheEnabled"), HttpProxyGlobals.IsMultitenant && VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Cafe.AnchorMailboxSharedCache.Enabled, ExTraceGlobals.VerboseTracer);

		public static readonly IntAppSettingsEntry GlobalSharedCacheRpcTimeout = new IntAppSettingsEntry(HttpProxySettings.Prefix("GlobalSharedCacheRpcTimeout"), 2000, ExTraceGlobals.VerboseTracer);

		public static readonly StringAppSettingsEntry EnableLiveIdCookieBEAuthVersion = new StringAppSettingsEntry("LiveIdCookieAuthModule.EnableBEAuthVersion", string.Empty, ExTraceGlobals.VerboseTracer);

		public static readonly EnumAppSettingsEntry<ProxyRequestHandler.SupportBackEndCookie> SupportBackEndCookie = new EnumAppSettingsEntry<ProxyRequestHandler.SupportBackEndCookie>(HttpProxySettings.Prefix("SupportBackEndCookie"), VariantConfiguration.InvariantNoFlightingSnapshot.Cafe.UseResourceForest.Enabled ? ProxyRequestHandler.SupportBackEndCookie.All : ProxyRequestHandler.SupportBackEndCookie.V1, ExTraceGlobals.VerboseTracer);

		public static readonly BoolAppSettingsEntry CafeV1RUMEnabled = new BoolAppSettingsEntry(HttpProxySettings.Prefix("CafeV1RUMEnabled"), VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Cafe.CafeV1RUM.Enabled, ExTraceGlobals.VerboseTracer);

		public static readonly StringAppSettingsEntry EnableRpsTokenBEAuthVersion = new StringAppSettingsEntry("RpsTokenAuthModule.EnableBEAuthVersion", string.Empty, ExTraceGlobals.VerboseTracer);
	}
}
