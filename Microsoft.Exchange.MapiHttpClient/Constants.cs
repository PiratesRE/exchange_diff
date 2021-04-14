using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class Constants
	{
		internal const string MapiHttpProtocolSequence = "MapiHttp";

		internal const string HttpVerbPost = "POST";

		internal const string HttpVerbGet = "GET";

		internal const string ContentTypeApplicationMapiHttp = "application/mapi-http";

		internal const string ContentTypeLegacyApplicationOctetStream = "application/octet-stream";

		internal const string ContentTypeTextHtml = "text/html";

		internal const string AcceptApplicationMapiHttp = "application/mapi-http";

		internal const string AcceptLegacyApplicationOctetStream = "application/octet-stream";

		internal const string AcceptApplicationWildcard = "application/*";

		internal const string AcceptWildcard = "*/*";

		internal const string RequestTypeUnknown = "Unknown";

		internal const string RequestTypePing = "PING";

		internal const string HeaderClientInfo = "X-ClientInfo";

		internal const string HeaderRequestType = "X-RequestType";

		internal const string HeaderRequestId = "X-RequestId";

		internal const string HeaderResponseCode = "X-ResponseCode";

		internal const string HeaderServiceCode = "X-ServiceCode";

		internal const string HeaderExpirationInfo = "X-ExpirationInfo";

		internal const string HeaderFailureLID = "X-FailureLID";

		internal const string HeaderFailureDescription = "X-FailureDescription";

		internal const string HeaderDelayRequest = "X-DelayRequest";

		internal const string HeaderDelayResponse = "X-DelayResponse";

		internal const string HeaderPendingPeriod = "X-PendingPeriod";

		internal const string HeaderSourceCafeServer = "X-SourceCafeServer";

		internal const string HeaderClientApplication = "X-ClientApplication";

		internal const string HeaderServerApplication = "X-ServerApplication";

		internal const string HeaderContentType = "Content-Type";

		internal const string HeaderStartTime = "X-StartTime";

		internal const string HeaderElapsedTime = "X-ElapsedTime";

		internal const string HeaderTunnelExpirationTime = "X-TunnelExpirationTime";

		internal const string CookieContextHandle = "MapiContext";

		internal const string CookieSequence = "MapiSequence";

		internal const string RequestTypeEmsmdbConnect = "Connect";

		internal const string RequestTypeEmsmdbDisconnect = "Disconnect";

		internal const string RequestTypeEmsmdbExecute = "Execute";

		internal const string RequestTypeEmsmdbNotificationWait = "NotificationWait";

		internal const string RequestTypeEmsmdbDummy = "Dummy";

		internal const string RequestTypeLegacyEmsmdbConnect = "EcDoConnectEx";

		internal const string RequestTypeLegacyEmsmdbDisconnect = "EcDoDisconnect";

		internal const string RequestTypeLegacyEmsmdbExecute = "EcDoRpcExt2";

		internal const string RequestTypeLegacyEmsmdbNotificationWait = "EcDoAsyncWaitEx";

		internal const string RequestTypeLegacyEmsmdbDummy = "EcDoDummy";

		internal const string RequestTypeNspiBind = "Bind";

		internal const string RequestTypeNspiUnbind = "Unbind";

		internal const string RequestTypeNspiUpdateStat = "UpdateStat";

		internal const string RequestTypeNspiQueryRows = "QueryRows";

		internal const string RequestTypeNspiQueryColumns = "QueryColumns";

		internal const string RequestTypeNspiSeekEntries = "SeekEntries";

		internal const string RequestTypeNspiResortRestriction = "ResortRestriction";

		internal const string RequestTypeNspiDNToEph = "DNToMId";

		internal const string RequestTypeNspiCompareMIds = "CompareMIds";

		internal const string RequestTypeNspiCompareDNTs = "CompareDNTs";

		internal const string RequestTypeNspiCompareMinIds = "CompareMinIds";

		internal const string RequestTypeNspiGetSpecialTable = "GetSpecialTable";

		internal const string RequestTypeNspiGetTemplateInfo = "GetTemplateInfo";

		internal const string RequestTypeNspiModLinkAtt = "ModLinkAtt";

		internal const string RequestTypeNspiResolveNames = "ResolveNames";

		internal const string RequestTypeNspiGetMatches = "GetMatches";

		internal const string RequestTypeNspiGetPropList = "GetPropList";

		internal const string RequestTypeNspiGetProps = "GetProps";

		internal const string RequestTypeNspiModProps = "ModProps";

		internal const string RequestTypeRfriGetMailboxUrl = "GetMailboxUrl";

		internal const string RequestTypeRfriGetAddressBookUrl = "GetAddressBookUrl";

		internal const string RequestTypeLegacyRfriGetNspiUrl = "GetNspiUrl";

		internal const string MetaTagProcessing = "PROCESSING";

		internal const string MetaTagPending = "PENDING";

		internal const string MetaTagDone = "DONE";

		internal const string ClientUserAgent = "MapiHttpClient";

		internal const string ServerApplication = "Exchange";

		internal const string ClientApplication = "MapiHttpClient";

		internal const string ProtocolSequencePrefix = "MapiHttp:";

		internal const int HttpStatusCodeRoutingError = 555;

		internal const int ExtendedBufferHeaderSize = 8;

		internal const int AuxInSize = 4104;

		internal const int AuxOutSize = 4104;

		internal const int MaxRequestSize = 268288;

		internal const int MaxResponseSize = 1054720;

		internal const int ReadResponseFragmentSize = 32768;

		internal const int CompletedAsyncOperationTrackingCount = 16;

		internal const int FailedAsyncOperationTrackingCount = 16;

		internal const int ServicePointConnectionLimit = 65000;

		internal const string TrustEntireForwardedForConfigurationKey = "TrustEntireForwardedFor";

		internal const string UseBufferedReadStreamConfigurationKey = "UseBufferedReadStream";

		internal const string ClientTunnelExpirationTimeConfigurationKey = "ClientTunnelExpirationTime";

		internal static readonly TimeSpan ClientTunnelExpirationTimeDefault = TimeSpan.FromMinutes(30.0);

		internal static readonly TimeSpan ClientTunnelExpirationTimeMin = TimeSpan.Zero;

		internal static readonly TimeSpan ClientTunnelExpirationTimeMax = TimeSpan.FromDays(1.0);

		internal static readonly TimeSpan MaxDelayRequest = TimeSpan.FromMinutes(5.0);

		internal static readonly TimeSpan MaxDelayResponse = TimeSpan.FromMinutes(5.0);

		internal static readonly TimeSpan DefaultPendingPeriod = TimeSpan.FromSeconds(15.0);

		internal static readonly TimeSpan HttpRequestTimeout = TimeSpan.FromSeconds(6.0 * Constants.DefaultPendingPeriod.TotalSeconds);

		internal static readonly TimeSpan MinPendingPeriod = TimeSpan.FromSeconds(5.0);

		internal static readonly TimeSpan SessionContextIdleTimeout = TimeSpan.FromMinutes(15.0);

		internal static readonly TimeSpan SessionContextRefreshInterval = TimeSpan.FromMilliseconds(Constants.SessionContextIdleTimeout.TotalMilliseconds / 2.0);

		internal static readonly TimeSpan UserContextIdleTimeout = TimeSpan.FromMinutes(15.0);

		internal static readonly TimeSpan MinExpirationInfo = TimeSpan.FromSeconds(5.0);

		internal static readonly TimeSpan MaxExpirationInfo = TimeSpan.FromHours(2.0);

		internal static readonly byte[] ProcessingMarker = Encoding.UTF8.GetBytes(string.Format("{0}\r\n", "PROCESSING"));

		internal static readonly byte[] PendingMarker = Encoding.UTF8.GetBytes(string.Format("{0}\r\n", "PENDING"));

		internal static readonly byte[] DoneMarker = Encoding.UTF8.GetBytes(string.Format("{0}\r\n\r\n", "DONE"));

		internal static readonly byte[] DoneMarkerNoEmptyLine = Encoding.UTF8.GetBytes(string.Format("{0}\r\n", "DONE"));

		internal static readonly int DefaultHttpPort = new Uri("http://www.contoso.com/").Port;

		internal static readonly int DefaultHttpsPort = new Uri("https://www.contoso.com/").Port;

		internal static readonly int BackEndHttpsPort = 444;
	}
}
