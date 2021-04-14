using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum PhotosLoggingMetadata
	{
		[DisplayName("GPP", "ISP")]
		IsSelfPhoto,
		[DisplayName("GPP", "TgEm")]
		TargetEmailAddress,
		[DisplayName("GPP", "PhSz")]
		PhotoSize,
		[DisplayName("GPP", "SCH")]
		ServerCacheHit,
		[DisplayName("GPP", "ADP")]
		ADHandlerProcessed,
		[DisplayName("GPP", "FSP")]
		FileSystemHandlerProcessed,
		[DisplayName("GPP", "MbxP")]
		MailboxHandlerProcessed,
		[DisplayName("GPP", "CaP")]
		CachingHandlerProcessed,
		[DisplayName("GPP", "HttpP")]
		HttpHandlerProcessed,
		[DisplayName("GPP", "PrvP")]
		PrivateHandlerProcessed,
		[DisplayName("GPP", "PrvSC")]
		PrivateHandlerServedContent,
		[DisplayName("GPP", "PrvSR")]
		PrivateHandlerServedRedirect,
		[DisplayName("GPP", "ADT")]
		ADHandlerTotalTime,
		[DisplayName("GPP", "ADC")]
		ADHandlerTotalLdapCount,
		[DisplayName("GPP", "ADL")]
		ADHandlerTotalLdapLatency,
		[DisplayName("GPP", "ADRdPT")]
		ADHandlerReadPhotoTime,
		[DisplayName("GPP", "ADRdPC")]
		ADHandlerReadPhotoLdapCount,
		[DisplayName("GPP", "ADRdPL")]
		ADHandlerReadPhotoLdapLatency,
		[DisplayName("GPP", "FST")]
		FileSystemHandlerTotalTime,
		[DisplayName("GPP", "FSCpu")]
		FileSystemHandlerTotalCpuTime,
		[DisplayName("GPP", "FSRdPT")]
		FileSystemHandlerReadPhotoTime,
		[DisplayName("GPP", "FSRdPCpu")]
		FileSystemHandlerReadPhotoCpuTime,
		[DisplayName("GPP", "FSRdThT")]
		FileSystemHandlerReadThumbprintTime,
		[DisplayName("GPP", "FSRdThCpu")]
		FileSystemHandlerReadThumbprintCpuTime,
		[DisplayName("GPP", "MbxT")]
		MailboxHandlerTotalTime,
		[DisplayName("GPP", "MbxRpcC")]
		MailboxHandlerTotalStoreRpcCount,
		[DisplayName("GPP", "MbxRpcL")]
		MailboxHandlerTotalStoreRpcLatency,
		[DisplayName("GPP", "MbxOpMT")]
		MailboxHandlerOpenMailboxTime,
		[DisplayName("GPP", "MbxOpMRpcC")]
		MailboxHandlerOpenMailboxStoreRpcCount,
		[DisplayName("GPP", "MbxOpMRpcL")]
		MailboxHandlerOpenMailboxStoreRpcLatency,
		[DisplayName("GPP", "MbxRdPT")]
		MailboxHandlerReadPhotoTime,
		[DisplayName("GPP", "MbxRdPRpcC")]
		MailboxHandlerReadPhotoStoreRpcCount,
		[DisplayName("GPP", "MbxRdPRpcL")]
		MailboxHandlerReadPhotoStoreRpcLatency,
		[DisplayName("GPP", "MbxRFdPT")]
		MailboxPhotoReaderFindPhotoTime,
		[DisplayName("GPP", "MbxRFdPRpcC")]
		MailboxPhotoReaderFindPhotoStoreRpcCount,
		[DisplayName("GPP", "MbxRFdPRpcL")]
		MailboxPhotoReaderFindPhotoStoreRpcLatency,
		[DisplayName("GPP", "MbxRBdPT")]
		MailboxPhotoReaderBindPhotoItemTime,
		[DisplayName("GPP", "MbxRBdPRpcC")]
		MailboxPhotoReaderBindPhotoItemStoreRpcCount,
		[DisplayName("GPP", "MbxRBdPRpcL")]
		MailboxPhotoReaderBindPhotoItemStoreRpcLatency,
		[DisplayName("GPP", "MbxRRdST")]
		MailboxPhotoReaderReadStreamTime,
		[DisplayName("GPP", "MbxRRdSRpcC")]
		MailboxPhotoReaderReadStreamStoreRpcCount,
		[DisplayName("GPP", "MbxRRdSRpcL")]
		MailboxPhotoReaderReadStreamStoreRpcLatency,
		[DisplayName("GPP", "CaT")]
		CachingHandlerTotalTime,
		[DisplayName("GPP", "CaCpu")]
		CachingHandlerTotalCpuTime,
		[DisplayName("GPP", "CaPT")]
		CachingHandlerCachePhotoTime,
		[DisplayName("GPP", "CaPCpu")]
		CachingHandlerCachePhotoCpuTime,
		[DisplayName("GPP", "CaNPT")]
		CachingHandlerCacheNegativePhotoTime,
		[DisplayName("GPP", "CaNPCpu")]
		CachingHandlerCacheNegativePhotoCpuTime,
		[DisplayName("GPP", "LdPT")]
		GetPersonFromPersonIdTime,
		[DisplayName("GPP", "LdPRpcC")]
		GetPersonFromPersonIdStoreRpcCount,
		[DisplayName("GPP", "LdPRpcL")]
		GetPersonFromPersonIdStoreRpcLatency,
		[DisplayName("GPP", "FdEmADT")]
		FindEmailAddressFromADObjectIdTime,
		[DisplayName("GPP", "FdEmADC")]
		FindEmailAddressFromADObjectIdLdapCount,
		[DisplayName("GPP", "FdEmADL")]
		FindEmailAddressFromADObjectIdLdapLatency,
		[DisplayName("GPP", "FdPT")]
		FindPersonIdByEmailAddressTime,
		[DisplayName("GPP", "FdPRpcC")]
		FindPersonIdByEmailAddressStoreRpcCount,
		[DisplayName("GPP", "FdPRpcL")]
		FindPersonIdByEmailAddressStoreRpcLatency,
		[DisplayName("GPP", "LdPST")]
		GetPhotoStreamFromPersonTime,
		[DisplayName("GPP", "LdPSRpcC")]
		GetPhotoStreamFromPersonStoreRpcCount,
		[DisplayName("GPP", "LdPSRpcL")]
		GetPhotoStreamFromPersonStoreRpcLatency,
		[DisplayName("GPP", "GPPT")]
		GetPersonaPhotoTotalTime,
		[DisplayName("GPP", "GPPADC")]
		GetPersonaPhotoTotalLdapCount,
		[DisplayName("GPP", "GPPADL")]
		GetPersonaPhotoTotalLdapLatency,
		[DisplayName("GPP", "GPPRpcC")]
		GetPersonaPhotoTotalStoreRpcCount,
		[DisplayName("GPP", "GPPRpcL")]
		GetPersonaPhotoTotalStoreRpcLatency,
		[DisplayName("GPP", "GUPT")]
		GetUserPhotoTotalTime,
		[DisplayName("GPP", "GUPADC")]
		GetUserPhotoTotalLdapCount,
		[DisplayName("GPP", "GUPADL")]
		GetUserPhotoTotalLdapLatency,
		[DisplayName("GPP", "GUPRpcC")]
		GetUserPhotoTotalStoreRpcCount,
		[DisplayName("GPP", "GUPRpcL")]
		GetUserPhotoTotalStoreRpcLatency,
		[DisplayName("GPP", "PxyT")]
		ProxyRequestTime,
		[DisplayName("GPP", "QRsT")]
		GetUserPhotoQueryResolveTargetInDirectoryTime,
		[DisplayName("GPP", "QRsADC")]
		GetUserPhotoQueryResolveTargetInDirectoryLdapCount,
		[DisplayName("GPP", "QRsADL")]
		GetUserPhotoQueryResolveTargetInDirectoryLdapLatency,
		[DisplayName("GPP", "LAzT")]
		LocalAuthorizationTime,
		[DisplayName("GPP", "LAzADC")]
		LocalAuthorizationLdapCount,
		[DisplayName("GPP", "LAzADL")]
		LocalAuthorizationLdapLatency,
		[DisplayName("GPP", "GUPFail")]
		GetUserPhotoFailed,
		[DisplayName("GPP", "GPPFail")]
		GetPersonaPhotoFailed,
		[DisplayName("GPP", "QryCT")]
		GetUserPhotoQueryCreationTime,
		[DisplayName("GPP", "QryCADC")]
		GetUserPhotoQueryCreationLdapCount,
		[DisplayName("GPP", "QryCADL")]
		GetUserPhotoQueryCreationLdapLatency,
		[DisplayName("GPP", "MRD")]
		MiscRoutingAndDiscovery,
		[DisplayName("GPP", "CrtCliCtxT")]
		CreateClientContextTime,
		[DisplayName("GPP", "CrtCliCtxADC")]
		CreateClientContextLdapCount,
		[DisplayName("GPP", "CrtCliCtxADL")]
		CreateClientContextLdapLatency,
		[DisplayName("GPP", "PxyRest")]
		RestProxy,
		[DisplayName("GPP", "MbxCompEPT")]
		MailboxPhotoHandlerComputeTargetPrincipalTime,
		[DisplayName("GPP", "MbxCompEPC")]
		MailboxPhotoHandlerComputeTargetPrincipalLdapCount,
		[DisplayName("GPP", "MbxCompEPL")]
		MailboxPhotoHandlerComputeTargetPrincipalLdapLatency,
		[DisplayName("GPP", "ADSer")]
		ADHandlerPhotoServed,
		[DisplayName("GPP", "ADFSer")]
		ADFallbackPhotoServed,
		[DisplayName("GPP", "FSSer")]
		FileSystemHandlerPhotoServed,
		[DisplayName("GPP", "MbxSer")]
		MailboxHandlerPhotoServed,
		[DisplayName("GPP", "ADHErr")]
		ADHandlerError,
		[DisplayName("GPP", "FSHErr")]
		FileSystemHandlerError,
		[DisplayName("GPP", "MbxHErr")]
		MailboxHandlerError,
		[DisplayName("GPP", "ADPhAvail")]
		ADHandlerPhotoAvailabile,
		[DisplayName("GPP", "FSPhAvail")]
		FileSystemHandlerPhotoAvailabile,
		[DisplayName("GPP", "MbxPhAvail")]
		MailboxHandlerPhotoAvailabile,
		[DisplayName("GPP", "HttpT")]
		HttpHandlerTotalTime,
		[DisplayName("GPP", "HttpADC")]
		HttpHandlerTotalLdapCount,
		[DisplayName("GPP", "HttpADL")]
		HttpHandlerTotalLdapLatency,
		[DisplayName("GPP", "HttpSendNGetRespT")]
		HttpHandlerSendRequestAndGetResponseTime,
		[DisplayName("GPP", "HttpSendNGetRespADC")]
		HttpHandlerSendRequestAndGetResponseLdapCount,
		[DisplayName("GPP", "HttpSendNGetRespADL")]
		HttpHandlerSendRequestAndGetResponseLdapLatency,
		[DisplayName("GPP", "HttpRdRespT")]
		HttpHandlerGetAndReadResponseStreamTime,
		[DisplayName("GPP", "HttpLocSvcT")]
		HttpHandlerLocateServiceTime,
		[DisplayName("GPP", "HttpLocSvcADC")]
		HttpHandlerLocateServiceLdapCount,
		[DisplayName("GPP", "HttpLocSvcADL")]
		HttpHandlerLocateServiceLdapLatency,
		[DisplayName("GPP", "RouterT")]
		RouterTotalTime,
		[DisplayName("GPP", "RouterADC")]
		RouterTotalLdapCount,
		[DisplayName("GPP", "RouterADL")]
		RouterTotalLdapLatency,
		[DisplayName("GPP", "RouterLkTargetDirT")]
		RouterLookupTargetInDirectoryTime,
		[DisplayName("GPP", "RouterLkTargetDirADC")]
		RouterLookupTargetInDirectoryLdapCount,
		[DisplayName("GPP", "RouterLkTargetDirADL")]
		RouterLookupTargetInDirectoryLdapLatency,
		[DisplayName("GPP", "RouterChkTargetMbxThisServerT")]
		RouterCheckTargetMailboxOnThisServerTime,
		[DisplayName("GPP", "RouterChkTargetMbxThisServerADC")]
		RouterCheckTargetMailboxOnThisServerLdapCount,
		[DisplayName("GPP", "RouterChkTargetMbxThisServerADL")]
		RouterCheckTargetMailboxOnThisServerLdapLatency,
		[DisplayName("GPP", "ExeV1Impl")]
		ExecutedV1Implementation,
		[DisplayName("GPP", "ExeV2Impl")]
		ExecutedV2Implementation,
		[DisplayName("GPP", "Fallback")]
		FallbackToV1Implementation,
		[DisplayName("GPP", "PrvT")]
		PrivateHandlerTotalTime,
		[DisplayName("GPP", "PrvRpcC")]
		PrivateHandlerTotalStoreRpcCount,
		[DisplayName("GPP", "PrvRpcL")]
		PrivateHandlerTotalStoreRpcLatency,
		[DisplayName("GPP", "PrvRdPhT")]
		PrivateHandlerReadPhotoStreamTime,
		[DisplayName("GPP", "PrvRdPhRpcC")]
		PrivateHandlerReadPhotoStreamStoreRpcCount,
		[DisplayName("GPP", "PrvRdPhRpcL")]
		PrivateHandlerReadPhotoStreamStoreRpcLatency,
		[DisplayName("GPP", "LocFrstPhSvcLocLocServerTime")]
		LocalForestPhotoServiceLocatorLocateServerTime,
		[DisplayName("GPP", "LocFrstPhSvcLocLocServerADC")]
		LocalForestPhotoServiceLocatorLocateServerLdapCount,
		[DisplayName("GPP", "LocFrstPhSvcLocLocServerADL")]
		LocalForestPhotoServiceLocatorLocateServerLdapLatency,
		[DisplayName("GPP", "LocFrstPhSvcLocGetPhSvcUriTime")]
		LocalForestPhotoServiceLocatorGetPhotoServiceUriTime,
		[DisplayName("GPP", "LocFrstPhSvcLocGetPhSvcUriADC")]
		LocalForestPhotoServiceLocatorGetPhotoServiceUriLdapCount,
		[DisplayName("GPP", "LocFrstPhSvcLocGetPhSvcUriADL")]
		LocalForestPhotoServiceLocatorGetPhotoServiceUriLdapLatency,
		[DisplayName("GPP", "WrgRtFallbackOtherSrv")]
		WrongRoutingDetectedThenFallbackToOtherServer,
		[DisplayName("GPP", "PhRqHdler")]
		ServedByPhotoRequestHandler,
		[DisplayName("GPP", "RFP")]
		RemoteForestHandlerProcessed,
		RemoteForestHandlerServed,
		[DisplayName("GPP", "RFT")]
		RemoteForestHandlerTotalTime,
		[DisplayName("GPP", "RFADC")]
		RemoteForestHandlerTotalLdapCount,
		[DisplayName("GPP", "RFADT")]
		RemoteForestHandlerTotalLdapLatency,
		[DisplayName("GPP", "RFHErr")]
		RemoteForestHandlerError,
		[DisplayName("GPP", "RCT")]
		ResponseContentType
	}
}
