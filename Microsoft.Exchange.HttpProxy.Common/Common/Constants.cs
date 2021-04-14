using System;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Net.Protocols;

namespace Microsoft.Exchange.HttpProxy.Common
{
	internal static class Constants
	{
		public const string AnonymousRequestFilterModuleLoggingKey = "AnonymousRequestFilterModule";

		public const string CalendarString = "calendar";

		public const string WLIDMemberName = "WLID-MemberName";

		public const string WLIDOrganizationContextHeaderName = "WLID-OrganizationContext";

		public const string NativeProxyTargetServer = "X-ProxyTargetServer";

		public const string NativeProxyTargetServerVersion = "X-ProxyTargetServerVersion";

		public const string NativeProxyTargetUrlAbsPath = "X-ProxyTargetUrlAbsPath";

		public const string RoutingKeys = "RoutingKeys";

		public const string EwsODataPath = "/ews/odata/";

		public const string OAuthMetadataPath = "/metadata/";

		public const string AutoDiscoverV2Path = "/autodiscover.json";

		public const string AutoDiscoverV2Version1Path = "/autodiscover.json/v1.0";

		public const string ODataPath = "/odata/";

		public const string EwsGetUserPhotoPath = "/ews/exchange.asmx/s/GetUserPhoto";

		public const string BackEndOverrideCookieName = "X-BackEndOverrideCookie";

		public const string PreferServerAffinityHeader = "X-PreferServerAffinity";

		public const string WsSecurityAddressEnd = "wssecurity";

		public const string PartnerAuthAddressEnd = "wssecurity/symmetrickey";

		public const string X509CertAuthAddressEnd = "wssecurity/x509cert";

		public static readonly Regex GuidAtDomainRegex = new Regex("^(?<mailboxguid>[A-Fa-f0-9]{32}|({|\\()?[A-Fa-f0-9]{8}-([A-Fa-f0-9]{4}-){3}[A-Fa-f0-9]{12}(}|\\))?|^({)?[0xA-Fa-f0-9]{3,10}(, {0,1}[0xA-Fa-f0-9]{3,6}){2}, {0,1}({)([0xA-Fa-f0-9]{3,4}, {0,1}){7}[0xA-Fa-f0-9]{3,4}(}}))(@(?<domain>[\\S.]+))*", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public static readonly Regex UsersEntityRegex = new Regex("/Users(\\('|/)(?<address>.+?)(['\\)/]|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public static readonly string ProbeHeaderName = WellKnownHeader.Probe;

		public static readonly string WLIDMemberNameHeaderName = WellKnownHeader.WLIDMemberName;

		public static readonly string XBackendHeaderPrefix = WellKnownHeader.XBackendHeaderPrefix;

		public static readonly string MsExchMonString = "MSEXCHMON";

		public static readonly string ActiveMonProbe = "ACTIVEMONITORING";

		public static readonly string LamProbe = "AMProbe";

		public static readonly string EasProbe = "TestActiveSyncConnectivity";

		public static readonly string MsExchProxyUri = WellKnownHeader.MsExchProxyUri;

		public static readonly string XIsFromCafe = WellKnownHeader.XIsFromCafe;

		public static readonly string XSourceCafeServer = WellKnownHeader.XSourceCafeServer;

		public static readonly string IsFromCafeHeaderValue = "1";

		public static readonly string LiveIdMemberName = WellKnownHeader.LiveIdMemberName;

		public static readonly string OriginatingClientIpHeader = "X-Forwarded-For";

		public static readonly string OriginatingClientPortHeader = "X-Forwarded-Port";
	}
}
