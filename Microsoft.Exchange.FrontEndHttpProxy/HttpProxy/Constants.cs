using System;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.Net.Protocols;

namespace Microsoft.Exchange.HttpProxy
{
	internal static class Constants
	{
		public static readonly string ExtensionGif = ".gif";

		public static readonly string ExtensionJpg = ".jpg";

		public static readonly string ExtensionCss = ".css";

		public static readonly string ExtensionXap = ".xap";

		public static readonly string ExtensionJs = ".js";

		public static readonly string ExtensionWav = ".wav";

		public static readonly string ExtensionMp3 = ".mp3";

		public static readonly string ExtensionHtm = ".htm";

		public static readonly string ExtensionHtml = ".html";

		public static readonly string ExtensionPng = ".png";

		public static readonly string ExtensionMSI = ".msi";

		public static readonly string ExtensionICO = ".ico";

		public static readonly string ExtensionManifest = ".manifest";

		public static readonly string ExtensionTtf = ".ttf";

		public static readonly string ExtensionEot = ".eot";

		public static readonly string ExtensionChromeWebApp = ".crx";

		public static readonly string ExtensionAxd = ".axd";

		public static readonly string ExtensionSvg = ".svg";

		public static readonly string ExtensionWoff = ".woff";

		public static readonly string ExtensionAspx = ".aspx";

		public static readonly string ExtensionOwa = ".owa";

		public static readonly string MsExchProxyUri = Constants.MsExchProxyUri;

		public static readonly string XIsFromCafe = Constants.XIsFromCafe;

		public static readonly string XSourceCafeServer = Constants.XSourceCafeServer;

		public static readonly string XBackendHeaderPrefix = Constants.XBackendHeaderPrefix;

		public static readonly string XRequestId = WellKnownHeader.XRequestId;

		public static readonly string AnchorMailboxHeaderName = WellKnownHeader.AnchorMailbox;

		public static readonly string ExternalDirectoryObjectIdHeaderName = WellKnownHeader.ExternalDirectoryObjectId;

		public static readonly string TargetDatabaseHeaderName = WellKnownHeader.TargetDatabase;

		public static readonly string ClientVersionHeaderName = WellKnownHeader.ClientVersion;

		public static readonly string BEServerExceptionHeaderName = "X-BEServerException";

		public static readonly string IllegalCrossServerConnectionExceptionType = "Microsoft.Exchange.Data.Storage.IllegalCrossServerConnectionException";

		public static readonly string BEServerRoutingErrorHeaderName = WellKnownHeader.BEServerRoutingError;

		public static readonly string WLIDMemberNameHeaderName = Constants.WLIDMemberNameHeaderName;

		public static readonly string WLIDOrganizationContextHeaderName = "WLID-OrganizationContext";

		public static readonly string LiveIdEnvironment = "RPSEnv";

		public static readonly string LiveIdPuid = WellKnownHeader.LiveIdPuid;

		public static readonly string OrgIdPuid = "RPSOrgIdPUID";

		public static readonly string LiveIdMemberName = Constants.LiveIdMemberName;

		public static readonly string AcceptEncodingHeaderName = WellKnownHeader.AcceptEncoding;

		public static readonly string TestBackEndUrlRequestHeaderKey = WellKnownHeader.TestBackEndUrlRequest;

		public static readonly string CafeErrorCodeHeaderName = WellKnownHeader.CafeErrorCode;

		public static readonly string CaptureResponseIdHeaderKey = WellKnownHeader.CaptureResponseId;

		public static readonly string ProbeHeaderName = Constants.ProbeHeaderName;

		public static readonly string LocalProbeHeaderValue = WellKnownHeader.LocalProbeHeaderValue;

		public static readonly string AuthorizationHeader = WellKnownHeader.Authorization;

		public static readonly string AuthenticationHeader = WellKnownHeader.Authentication;

		public static readonly string FrontEndToBackEndTimeout = WellKnownHeader.FrontEndToBackEndTimeout;

		public static readonly string BEResourcePath = "X-BEResourcePath";

		public static readonly string OriginatingClientIpHeader = Constants.OriginatingClientIpHeader;

		public static readonly string OriginatingClientPortHeader = Constants.OriginatingClientPortHeader;

		public static readonly string VDirObjectID = "X-vDirObjectId";

		public static readonly string MissingDirectoryUserObjectHeader = WellKnownHeader.MissingDirectoryUserObjectHint;

		public static readonly string OrganizationContextHeader = WellKnownHeader.OrganizationContext;

		public static readonly string RequestCompletedHttpContextKeyName = "RequestCompleted";

		public static readonly string LatencyTrackerContextKeyName = "LatencyTracker";

		public static readonly string TraceContextKey = "TraceContext";

		public static readonly string RequestIdHttpContextKeyName = "LogRequestId";

		public static readonly string CallerADRawEntryKeyName = "CallerADRawEntry";

		public static readonly string MissingDirectoryUserObjectKey = "MissingDirectoryUserObject";

		public static readonly string OrganizationContextKey = "OrganizationContext";

		public static readonly string WLIDMemberName = "WLID-MemberName";

		public static readonly string GzipHeaderValue = "gzip";

		public static readonly string DeflateHeaderValue = "deflate";

		public static readonly string IsFromCafeHeaderValue = Constants.IsFromCafeHeaderValue;

		public static readonly string SpnPrefixForHttp = "HTTP/";

		public static readonly string NegotiatePackageValue = "Negotiate";

		public static readonly string NtlmPackageValue = "NTLM";

		public static readonly string KerberosPackageValue = "Kerberos";

		public static readonly string PrefixForKerbAuthBlob = "Negotiate ";

		public static readonly string RPSBackEndServerCookieName = "MS-WSMAN";

		public static readonly string LiveIdRPSAuth = "RPSAuth";

		public static readonly string LiveIdRPSSecAuth = "RPSSecAuth";

		public static readonly string LiveIdRPSTAuth = "RPSTAuth";

		public static readonly string BEResource = "X-BEResource";

		public static readonly string AnonResource = "X-AnonResource";

		public static readonly string AnonResourceBackend = "X-AnonResource-Backend";

		public static readonly string BackEndOverrideCookieName = "X-BackEndOverrideCookie";

		public static readonly string PreferServerAffinityHeader = "X-PreferServerAffinity";

		public static readonly Regex GuidAtDomainRegex = Constants.GuidAtDomainRegex;

		public static readonly Regex AddressRegex = new Regex("(?<address>[A-Z0-9._%+-]+@[A-Z0-9.-]+\\.[A-Z]{2,4})", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public static readonly Regex SidRegex = new Regex("(?<sid>S-1-5-\\d{2}-\\d{9,}-\\d{9,}-\\d{9,}-\\d{4,})@(?<domain>[\\S.]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public static readonly Regex SidOnlyRegex = new Regex("(?<sid>S-1-5-\\d{2}-\\d{9,}-\\d{9,}-\\d{9,}-\\d{4,})", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public static readonly Regex ExchClientVerRegex = new Regex("(?<major>\\d{2})\\.(?<minor>\\d{1,})\\.(?<build>\\d{1,})\\.(?<revision>\\d{1,})", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public static readonly Regex NoLeadingZeroRegex = new Regex("0*([0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public static readonly Regex NoRevisionNumberRegex = new Regex("^([0-9]+\\.[0-9]+\\.[0-9]+)(\\.[0-9]+)*", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public static readonly string CertificateValidationComponentId = "ClientAccessFrontEnd";

		public static readonly string NotImplementedStatusDescription = "Not implemented";

		public static readonly string IntegratedAuthPath = "/integrated";

		public static readonly string IntegratedAuthPathWithTrailingSlash = Constants.IntegratedAuthPath + "/";

		public static readonly string OMAPath = "/oma";

		public static readonly string RequestIdKeyForIISLogs = "&cafeReqId=";

		public static readonly string CorrelationIdKeyForIISLogs = "&CorrelationID=";

		public static readonly string ISO8601DateTimeMsPattern = "yyyy-MM-ddTHH:mm:ss.fff";

		public static readonly string HealthCheckPage = "HealthCheck.htm";

		public static readonly string HealthCheckPageResponse = "200 OK";

		public static readonly string OutlookDomain = "outlook.com";

		public static readonly string Office365Domain = "outlook.office365.com";

		public static readonly string ServerKerberosAuthenticationFailureErrorCode = HttpProxySubErrorCode.ServerKerberosAuthenticationFailure.ToString();

		public static readonly string InvalidOAuthTokenErrorCode = HttpProxySubErrorCode.InvalidOAuthToken.ToString();

		public static readonly string ClientDisconnectErrorCode = HttpProxySubErrorCode.ClientDisconnect.ToString();

		public static readonly string InternalServerErrorStatusCode = HttpStatusCode.InternalServerError.ToString();
	}
}
