using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Protocols
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class WellKnownHeader
	{
		public const string XEXTClientName = "X-EXT-ClientName";

		public const string XEXTCorrelationId = "X-EXT-CorrelationId";

		public const string XEXTProxyBegin = "X-EXT-ProxyBegin";

		public const string XEXTACSBegin = "X-EXT-ACSBegin";

		public const string XEXTACSEnd = "X-EXT-ACSEnd";

		public const string XOWACorrelationId = "X-OWA-CorrelationId";

		public const string XSuiteServiceProxyOrigin = "X-SuiteServiceProxyOrigin";

		public const string XOWAClientBegin = "X-OWA-ClientBegin";

		public const string XFrontEndBegin = "X-FrontEnd-Begin";

		public const string XFrontEndEnd = "X-FrontEnd-End";

		public const string XBackEndBegin = "X-BackEnd-Begin";

		public const string XBackEndEnd = "X-BackEnd-End";

		public const string XFrontEndHandlerBegin = "X-FrontEnd-Handler-Begin";

		public const string XFromBackEndClientConnection = "X-FromBackEnd-ClientConnection";

		public const string XFromBackEndServerAffinity = "X-FromBackend-ServerAffinity";

		public const string XAuthError = "X-Auth-Error";

		public const string XAutoDiscoveryError = "X-AutoDiscovery-Error";

		public const string OwaEcpUpnAnchorMailbox = "X-UpnAnchorMailbox";

		public const string HttpProxyInfo = "Via";

		public const string XOwaExplicitLogonUser = "X-OWA-ExplicitLogonUser";

		public static readonly string MsExchProxyUri = "msExchProxyUri";

		public static readonly string XIsFromCafe = "X-IsFromCafe";

		public static readonly string XIsFromBackend = "X-IsFromBackend";

		public static readonly string XSourceCafeServer = "X-SourceCafeServer";

		public static readonly string XBackendHeaderPrefix = "X-Backend-Diag-";

		public static readonly string XFEServer = "X-FEServer";

		public static readonly string XBEServer = "X-BEServer";

		public static readonly string XCalculatedBETarget = "X-CalculatedBETarget";

		public static readonly string XTargetServer = "X-TargetServer";

		public static readonly string XRequestId = "X-RequestId";

		public static readonly string XForwardedFor = "X-Forwarded-For";

		public static readonly string AnchorMailbox = "X-AnchorMailbox";

		public static readonly string ExternalDirectoryObjectId = "X-ExternalDirectoryObjectId";

		public static readonly string PublicFolderMailbox = "X-PublicFolderMailbox";

		public static readonly string TargetDatabase = "X-TargetDatabase";

		public static readonly string ClientVersion = "X-ExchClientVersion";

		public static readonly string WLIDMemberName = "X-WLID-MemberName";

		public static readonly string LiveIdPuid = "RPSPUID";

		public static readonly string LiveIdMemberName = "RPSMemberName";

		public static readonly string AcceptEncoding = "Accept-Encoding";

		public static readonly string TestBackEndUrlRequest = "TestBackEndUrl";

		public static readonly string CafeErrorCode = "X-CasErrorCode";

		public static readonly string CaptureResponseId = "CaptureResponseId";

		public static readonly string Probe = "X-ProbeType";

		public static readonly string LocalProbeHeaderValue = "X-MS-ClientAccess-LocalProbe";

		public static readonly string Authorization = "Authorization";

		public static readonly string Authentication = "WWW-Authenticate";

		public static readonly string E14DiagInfo = "X-DiagInfo";

		public static readonly string BEServerRoutingError = "X-BEServerRoutingError";

		public static readonly string XDBMountedOnServer = "X-DBMountedOnServer";

		public static readonly string Pragma = "Pragma";

		public static readonly string MailboxDatabaseGuid = "X-DatabaseGuid";

		public static readonly string PreAuthRequest = "X-AuthenticateOnly";

		public static readonly string RpcHttpProxyLogonUserName = "X-RpcHttpProxyLogonUserName";

		public static readonly string RpcHttpProxyServerTarget = "X-RpcHttpProxyServerTarget";

		public static readonly string RpcHttpProxyAssociationGuid = "X-AssociationGuid";

		public static readonly string GenericAnchorHint = "X-GenericAnchorHint";

		public static readonly string FrontEndToBackEndTimeout = "X-FeToBeTimeout";

		public static readonly string EWSTargetVersion = "X-EWS-TargetVersion";

		public static readonly string XCafeLastRetryHeaderKey = "X-Cafe-Last-Retry";

		public static readonly string BackEndOriginatingMailboxDatabase = "X-BackEndOriginatingMailboxDatabase";

		public static readonly string CmdletProxyIsOn = "proxy";

		public static readonly string MissingDirectoryUserObjectHint = "X-MissingDirectoryUserObjectHint";

		public static readonly string OrganizationContext = "X-OrganizationContext";

		public static readonly string MSDiagnostics = "X-MS-Diagnostics";

		public static readonly string PrimaryIdentity = "X-PrimaryIdentity";

		public static readonly string XOWAErrorMessageID = "X-OWAErrorMessageID";

		public static readonly string StrictTransportSecurity = "Strict-Transport-Security";

		public static readonly string XErrorTrace = "X-ErrorTrace";

		public static readonly string XRealm = "X-Realm";
	}
}
