using System;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.HttpProxy.Common
{
	internal static class AutodiscoverEwsWebConfiguration
	{
		public static readonly bool SoapEndpointEnabled = new BoolAppSettingsEntry("SoapEndpointEnabled", false, ExTraceGlobals.VerboseTracer).Value;

		public static readonly bool WsSecurityEndpointEnabled = new BoolAppSettingsEntry("WsSecurityEndpointEnabled", false, ExTraceGlobals.VerboseTracer).Value;

		public static readonly bool WsSecuritySymmetricKeyEndpointEnabled = new BoolAppSettingsEntry("WsSecuritySymmetricKeyEndpointEnabled", false, ExTraceGlobals.VerboseTracer).Value;

		public static readonly bool WsSecurityX509CertEndpointEnabled = new BoolAppSettingsEntry("WsSecurityX509CertEndpointEnabled", false, ExTraceGlobals.VerboseTracer).Value;
	}
}
