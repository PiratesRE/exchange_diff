using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class PhotoPerformanceMarkers
	{
		public const string ADHandlerTotal = "ADHandlerTotal";

		public const string ADHandlerReadPhoto = "ADHandlerReadPhoto";

		public const string FileSystemHandlerTotal = "FileSystemHandlerTotal";

		public const string FileSystemHandlerReadPhoto = "FileSystemHandlerReadPhoto";

		public const string FileSystemHandlerReadThumbprint = "FileSystemHandlerReadThumbprint";

		public const string MailboxHandlerTotal = "MailboxHandlerTotal";

		public const string MailboxHandlerOpenMailbox = "MailboxHandlerOpenMailbox";

		public const string MailboxHandlerReadPhoto = "MailboxHandlerReadPhoto";

		public const string MailboxPhotoHandlerComputeTargetPrincipal = "MailboxPhotoHandlerComputeTargetPrincipal";

		public const string MailboxPhotoReaderFindPhoto = "MailboxPhotoReaderFindPhoto";

		public const string MailboxPhotoReaderBindPhotoItem = "MailboxPhotoReaderBindPhotoItem";

		public const string MailboxPhotoReaderReadStream = "MailboxPhotoReaderReadStream";

		public const string CachingHandlerTotal = "CachingHandlerTotal";

		public const string CachingHandlerCachePhoto = "CachingHandlerCachePhoto";

		public const string CachingHandlerCacheNegativePhoto = "CachingHandlerCacheNegativePhoto";

		public const string GetPersonFromPersonId = "GetPersonFromPersonId";

		public const string FindEmailAddressFromADObjectId = "FindEmailAddressFromADObjectId";

		public const string FindPersonIdByEmailAddress = "FindPersonIdByEmailAddress";

		public const string GetPhotoStreamFromPerson = "GetPhotoStreamFromPerson";

		public const string GetPersonaPhotoTotal = "GetPersonaPhotoTotal";

		public const string GetUserPhotoTotal = "GetUserPhotoTotal";

		public const string QueryResolveTargetInDirectory = "QueryResolveTargetInDirectory";

		public const string ProxyRequest = "ProxyRequest";

		public const string LocalAuthorization = "LocalAuthorization";

		public const string GetUserPhotoQueryCreation = "QueryCreation";

		public const string CreateClientContext = "CreateClientContext";

		public const string ProxyOverRestService = "ProxyOverRestService";

		public const string ADHandlerPhotoServed = "ADHandlerPhotoServed";

		public const string FileSystemHandlerPhotoServed = "FileSystemHandlerPhotoServed";

		public const string MailboxHandlerPhotoServed = "MailboxHandlerPhotoServed";

		public const string ADHandlerError = "ADHandlerError";

		public const string FileSystemHandlerError = "FileSystemHandlerError";

		public const string MailboxHandlerError = "MailboxHandlerError";

		public const string ADHandlerPhotoAvailable = "ADHandlerPhotoAvailable";

		public const string FileSystemHandlerPhotoAvailable = "FileSystemHandlerPhotoAvailable";

		public const string MailboxHandlerPhotoAvailable = "MailboxHandlerPhotoAvailable";

		public const string ADFallbackPhotoServed = "ADFallbackPhotoServed";

		public const string GarbageCollection = "GarbageCollection";

		public const string HttpHandlerTotal = "HttpHandlerTotal";

		public const string HttpHandlerSendRequestAndGetResponse = "HttpHandlerSendRequestAndGetResponse";

		public const string HttpHandlerGetAndReadResponseStream = "HttpHandlerGetAndReadResponseStream";

		public const string HttpHandlerLocateService = "HttpHandlerLocateService";

		public const string HttpHandlerError = "HttpHandlerError";

		public const string RouterTotal = "RouterTotal";

		public const string RouterLookupTargetInDirectory = "RouterLookupTargetInDirectory";

		public const string RouterCheckTargetMailboxOnThisServer = "RouterCheckTargetMailboxOnThisServer";

		public const string PrivateHandlerTotal = "PrivateHandlerTotal";

		public const string ADHandlerProcessed = "ADHandlerProcessed";

		public const string FileSystemHandlerProcessed = "FileSystemHandlerProcessed";

		public const string MailboxHandlerProcessed = "MailboxHandlerProcessed";

		public const string CachingHandlerProcessed = "CachingHandlerProcessed";

		public const string HttpHandlerProcessed = "HttpHandlerProcessed";

		public const string PrivateHandlerProcessed = "PrivateHandlerProcessed";

		public const string PrivateHandlerServedContent = "PrivateHandlerServedContent";

		public const string PrivateHandlerServedRedirect = "PrivateHandlerServedRedirect";

		public const string PrivateHandlerReadPhotoStream = "PrivateHandlerReadPhotoStream";

		public const string LocalForestPhotoServiceLocatorLocateServer = "LocalForestPhotoServiceLocatorLocateServer";

		public const string LocalForestPhotoServiceLocatorGetPhotoServiceUri = "LocalForestPhotoServiceLocatorGetPhotoServiceUri";

		public const string WrongRoutingDetectedThenFallbackToOtherServer = "WrongRoutingDetectedThenFallbackToOtherServer";

		public const string PhotoRequestorWriterResolveIdentity = "PhotoRequestorWriterResolveIdentity";

		public const string RemoteForestHandlerProcessed = "RemoteForestHandlerProcessed";

		public const string RemoteForestHandlerTotal = "RemoteForestHandlerTotal";

		public const string RemoteForestHandlerServed = "RemoteForestHandlerServed";

		public const string RemoteForestHandlerError = "RemoteForestHandlerError";

		public const string MiscRoutingAndDiscovery = "MiscRoutingAndDiscovery";
	}
}
