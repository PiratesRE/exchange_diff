using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.Services.Diagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PhotoRequestPerformanceLogger : IPerformanceDataLogger
	{
		internal PhotoRequestPerformanceLogger(RequestDetailsLogger logger, ITracer tracer)
		{
			if (logger == null)
			{
				throw new ArgumentNullException("logger");
			}
			if (tracer == null)
			{
				throw new ArgumentNullException("tracer");
			}
			this.logger = logger;
			this.tracer = tracer;
		}

		public void Log(string marker, string counter, TimeSpan dataPoint)
		{
			this.tracer.TracePerformance<string, string, double>((long)this.GetHashCode(), "{0}.{1}={2}ms", marker, counter, dataPoint.TotalMilliseconds);
			this.MapToMetadataAndLog(marker, counter, dataPoint.TotalMilliseconds);
		}

		public void Log(string marker, string counter, uint dataPoint)
		{
			this.tracer.TracePerformance<string, string, uint>((long)this.GetHashCode(), "{0}.{1}={2}", marker, counter, dataPoint);
			this.MapToMetadataAndLog(marker, counter, dataPoint);
		}

		public void Log(string marker, string counter, string dataPoint)
		{
			this.tracer.TracePerformance<string, string, string>((long)this.GetHashCode(), "{0}.{1}={2}", marker, counter, dataPoint);
			this.MapToMetadataAndLog(marker, counter, dataPoint);
		}

		private void MapToMetadataAndLog(string marker, string counter, object dataPoint)
		{
			PhotosLoggingMetadata photosLoggingMetadata;
			if (!PhotoRequestPerformanceLogger.MarkerAndCounterToMetadataMap.TryGetValue(Tuple.Create<string, string>(marker, counter), out photosLoggingMetadata))
			{
				return;
			}
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.logger, photosLoggingMetadata, dataPoint);
		}

		private const string ElapsedTime = "ElapsedTime";

		private const string CpuTime = "CpuTime";

		private const string LdapCount = "LdapCount";

		private const string LdapLatency = "LdapLatency";

		private const string StoreRpcCount = "StoreRpcCount";

		private const string StoreRpcLatency = "StoreRpcLatency";

		private static readonly Dictionary<Tuple<string, string>, PhotosLoggingMetadata> MarkerAndCounterToMetadataMap = new Dictionary<Tuple<string, string>, PhotosLoggingMetadata>
		{
			{
				Tuple.Create<string, string>("ADHandlerTotal", "ElapsedTime"),
				PhotosLoggingMetadata.ADHandlerTotalTime
			},
			{
				Tuple.Create<string, string>("ADHandlerTotal", "LdapCount"),
				PhotosLoggingMetadata.ADHandlerTotalLdapCount
			},
			{
				Tuple.Create<string, string>("ADHandlerTotal", "LdapLatency"),
				PhotosLoggingMetadata.ADHandlerTotalLdapLatency
			},
			{
				Tuple.Create<string, string>("ADHandlerReadPhoto", "ElapsedTime"),
				PhotosLoggingMetadata.ADHandlerReadPhotoTime
			},
			{
				Tuple.Create<string, string>("ADHandlerReadPhoto", "LdapCount"),
				PhotosLoggingMetadata.ADHandlerReadPhotoLdapCount
			},
			{
				Tuple.Create<string, string>("ADHandlerReadPhoto", "LdapLatency"),
				PhotosLoggingMetadata.ADHandlerReadPhotoLdapLatency
			},
			{
				Tuple.Create<string, string>("FileSystemHandlerTotal", "ElapsedTime"),
				PhotosLoggingMetadata.FileSystemHandlerTotalTime
			},
			{
				Tuple.Create<string, string>("FileSystemHandlerTotal", "CpuTime"),
				PhotosLoggingMetadata.FileSystemHandlerTotalCpuTime
			},
			{
				Tuple.Create<string, string>("FileSystemHandlerReadPhoto", "ElapsedTime"),
				PhotosLoggingMetadata.FileSystemHandlerReadPhotoTime
			},
			{
				Tuple.Create<string, string>("FileSystemHandlerReadPhoto", "CpuTime"),
				PhotosLoggingMetadata.FileSystemHandlerReadPhotoCpuTime
			},
			{
				Tuple.Create<string, string>("FileSystemHandlerReadThumbprint", "ElapsedTime"),
				PhotosLoggingMetadata.FileSystemHandlerReadThumbprintTime
			},
			{
				Tuple.Create<string, string>("FileSystemHandlerReadThumbprint", "CpuTime"),
				PhotosLoggingMetadata.FileSystemHandlerReadThumbprintCpuTime
			},
			{
				Tuple.Create<string, string>("MailboxHandlerTotal", "ElapsedTime"),
				PhotosLoggingMetadata.MailboxHandlerTotalTime
			},
			{
				Tuple.Create<string, string>("MailboxHandlerTotal", "StoreRpcCount"),
				PhotosLoggingMetadata.MailboxHandlerTotalStoreRpcCount
			},
			{
				Tuple.Create<string, string>("MailboxHandlerTotal", "StoreRpcLatency"),
				PhotosLoggingMetadata.MailboxHandlerTotalStoreRpcLatency
			},
			{
				Tuple.Create<string, string>("MailboxHandlerOpenMailbox", "ElapsedTime"),
				PhotosLoggingMetadata.MailboxHandlerOpenMailboxTime
			},
			{
				Tuple.Create<string, string>("MailboxHandlerOpenMailbox", "StoreRpcCount"),
				PhotosLoggingMetadata.MailboxHandlerOpenMailboxStoreRpcCount
			},
			{
				Tuple.Create<string, string>("MailboxHandlerOpenMailbox", "StoreRpcLatency"),
				PhotosLoggingMetadata.MailboxHandlerOpenMailboxStoreRpcLatency
			},
			{
				Tuple.Create<string, string>("MailboxHandlerReadPhoto", "ElapsedTime"),
				PhotosLoggingMetadata.MailboxHandlerReadPhotoTime
			},
			{
				Tuple.Create<string, string>("MailboxHandlerReadPhoto", "StoreRpcCount"),
				PhotosLoggingMetadata.MailboxHandlerReadPhotoStoreRpcCount
			},
			{
				Tuple.Create<string, string>("MailboxHandlerReadPhoto", "StoreRpcLatency"),
				PhotosLoggingMetadata.MailboxHandlerReadPhotoStoreRpcLatency
			},
			{
				Tuple.Create<string, string>("MailboxPhotoReaderFindPhoto", "ElapsedTime"),
				PhotosLoggingMetadata.MailboxPhotoReaderFindPhotoTime
			},
			{
				Tuple.Create<string, string>("MailboxPhotoReaderFindPhoto", "StoreRpcCount"),
				PhotosLoggingMetadata.MailboxPhotoReaderFindPhotoStoreRpcCount
			},
			{
				Tuple.Create<string, string>("MailboxPhotoReaderFindPhoto", "StoreRpcLatency"),
				PhotosLoggingMetadata.MailboxPhotoReaderFindPhotoStoreRpcLatency
			},
			{
				Tuple.Create<string, string>("MailboxPhotoReaderBindPhotoItem", "ElapsedTime"),
				PhotosLoggingMetadata.MailboxPhotoReaderBindPhotoItemTime
			},
			{
				Tuple.Create<string, string>("MailboxPhotoReaderBindPhotoItem", "StoreRpcCount"),
				PhotosLoggingMetadata.MailboxPhotoReaderBindPhotoItemStoreRpcCount
			},
			{
				Tuple.Create<string, string>("MailboxPhotoReaderBindPhotoItem", "StoreRpcLatency"),
				PhotosLoggingMetadata.MailboxPhotoReaderBindPhotoItemStoreRpcLatency
			},
			{
				Tuple.Create<string, string>("MailboxPhotoReaderReadStream", "ElapsedTime"),
				PhotosLoggingMetadata.MailboxPhotoReaderReadStreamTime
			},
			{
				Tuple.Create<string, string>("MailboxPhotoReaderReadStream", "StoreRpcCount"),
				PhotosLoggingMetadata.MailboxPhotoReaderReadStreamStoreRpcCount
			},
			{
				Tuple.Create<string, string>("MailboxPhotoReaderReadStream", "StoreRpcLatency"),
				PhotosLoggingMetadata.MailboxPhotoReaderReadStreamStoreRpcLatency
			},
			{
				Tuple.Create<string, string>("CachingHandlerTotal", "ElapsedTime"),
				PhotosLoggingMetadata.CachingHandlerTotalTime
			},
			{
				Tuple.Create<string, string>("CachingHandlerTotal", "CpuTime"),
				PhotosLoggingMetadata.CachingHandlerTotalCpuTime
			},
			{
				Tuple.Create<string, string>("CachingHandlerCachePhoto", "ElapsedTime"),
				PhotosLoggingMetadata.CachingHandlerCachePhotoTime
			},
			{
				Tuple.Create<string, string>("CachingHandlerCachePhoto", "CpuTime"),
				PhotosLoggingMetadata.CachingHandlerCachePhotoCpuTime
			},
			{
				Tuple.Create<string, string>("CachingHandlerCacheNegativePhoto", "ElapsedTime"),
				PhotosLoggingMetadata.CachingHandlerCacheNegativePhotoTime
			},
			{
				Tuple.Create<string, string>("CachingHandlerCacheNegativePhoto", "CpuTime"),
				PhotosLoggingMetadata.CachingHandlerCacheNegativePhotoCpuTime
			},
			{
				Tuple.Create<string, string>("GetPersonFromPersonId", "ElapsedTime"),
				PhotosLoggingMetadata.GetPersonFromPersonIdTime
			},
			{
				Tuple.Create<string, string>("GetPersonFromPersonId", "StoreRpcCount"),
				PhotosLoggingMetadata.GetPersonFromPersonIdStoreRpcCount
			},
			{
				Tuple.Create<string, string>("GetPersonFromPersonId", "StoreRpcLatency"),
				PhotosLoggingMetadata.GetPersonFromPersonIdStoreRpcLatency
			},
			{
				Tuple.Create<string, string>("FindEmailAddressFromADObjectId", "ElapsedTime"),
				PhotosLoggingMetadata.FindEmailAddressFromADObjectIdTime
			},
			{
				Tuple.Create<string, string>("FindEmailAddressFromADObjectId", "LdapCount"),
				PhotosLoggingMetadata.FindEmailAddressFromADObjectIdLdapCount
			},
			{
				Tuple.Create<string, string>("FindEmailAddressFromADObjectId", "LdapLatency"),
				PhotosLoggingMetadata.FindEmailAddressFromADObjectIdLdapLatency
			},
			{
				Tuple.Create<string, string>("FindPersonIdByEmailAddress", "ElapsedTime"),
				PhotosLoggingMetadata.FindPersonIdByEmailAddressTime
			},
			{
				Tuple.Create<string, string>("FindPersonIdByEmailAddress", "StoreRpcCount"),
				PhotosLoggingMetadata.FindPersonIdByEmailAddressStoreRpcCount
			},
			{
				Tuple.Create<string, string>("FindPersonIdByEmailAddress", "StoreRpcLatency"),
				PhotosLoggingMetadata.FindPersonIdByEmailAddressStoreRpcLatency
			},
			{
				Tuple.Create<string, string>("GetPhotoStreamFromPerson", "ElapsedTime"),
				PhotosLoggingMetadata.GetPhotoStreamFromPersonTime
			},
			{
				Tuple.Create<string, string>("GetPhotoStreamFromPerson", "StoreRpcCount"),
				PhotosLoggingMetadata.GetPhotoStreamFromPersonStoreRpcCount
			},
			{
				Tuple.Create<string, string>("GetPhotoStreamFromPerson", "StoreRpcLatency"),
				PhotosLoggingMetadata.GetPhotoStreamFromPersonStoreRpcLatency
			},
			{
				Tuple.Create<string, string>("GetPersonaPhotoTotal", "ElapsedTime"),
				PhotosLoggingMetadata.GetPersonaPhotoTotalTime
			},
			{
				Tuple.Create<string, string>("GetPersonaPhotoTotal", "LdapCount"),
				PhotosLoggingMetadata.GetPersonaPhotoTotalLdapCount
			},
			{
				Tuple.Create<string, string>("GetPersonaPhotoTotal", "LdapLatency"),
				PhotosLoggingMetadata.GetPersonaPhotoTotalLdapLatency
			},
			{
				Tuple.Create<string, string>("GetPersonaPhotoTotal", "StoreRpcCount"),
				PhotosLoggingMetadata.GetPersonaPhotoTotalStoreRpcCount
			},
			{
				Tuple.Create<string, string>("GetPersonaPhotoTotal", "StoreRpcLatency"),
				PhotosLoggingMetadata.GetPersonaPhotoTotalStoreRpcLatency
			},
			{
				Tuple.Create<string, string>("ProxyRequest", "ElapsedTime"),
				PhotosLoggingMetadata.ProxyRequestTime
			},
			{
				Tuple.Create<string, string>("QueryResolveTargetInDirectory", "ElapsedTime"),
				PhotosLoggingMetadata.GetUserPhotoQueryResolveTargetInDirectoryTime
			},
			{
				Tuple.Create<string, string>("QueryResolveTargetInDirectory", "LdapCount"),
				PhotosLoggingMetadata.GetUserPhotoQueryResolveTargetInDirectoryLdapCount
			},
			{
				Tuple.Create<string, string>("QueryResolveTargetInDirectory", "LdapLatency"),
				PhotosLoggingMetadata.GetUserPhotoQueryResolveTargetInDirectoryLdapLatency
			},
			{
				Tuple.Create<string, string>("LocalAuthorization", "ElapsedTime"),
				PhotosLoggingMetadata.LocalAuthorizationTime
			},
			{
				Tuple.Create<string, string>("LocalAuthorization", "LdapCount"),
				PhotosLoggingMetadata.LocalAuthorizationLdapCount
			},
			{
				Tuple.Create<string, string>("LocalAuthorization", "LdapLatency"),
				PhotosLoggingMetadata.LocalAuthorizationLdapLatency
			},
			{
				Tuple.Create<string, string>("GetUserPhotoTotal", "ElapsedTime"),
				PhotosLoggingMetadata.GetUserPhotoTotalTime
			},
			{
				Tuple.Create<string, string>("GetUserPhotoTotal", "LdapCount"),
				PhotosLoggingMetadata.GetUserPhotoTotalLdapCount
			},
			{
				Tuple.Create<string, string>("GetUserPhotoTotal", "LdapLatency"),
				PhotosLoggingMetadata.GetUserPhotoTotalLdapLatency
			},
			{
				Tuple.Create<string, string>("GetUserPhotoTotal", "StoreRpcCount"),
				PhotosLoggingMetadata.GetUserPhotoTotalStoreRpcCount
			},
			{
				Tuple.Create<string, string>("GetUserPhotoTotal", "StoreRpcLatency"),
				PhotosLoggingMetadata.GetUserPhotoTotalStoreRpcLatency
			},
			{
				Tuple.Create<string, string>("QueryCreation", "ElapsedTime"),
				PhotosLoggingMetadata.GetUserPhotoQueryCreationTime
			},
			{
				Tuple.Create<string, string>("QueryCreation", "LdapCount"),
				PhotosLoggingMetadata.GetUserPhotoQueryCreationLdapCount
			},
			{
				Tuple.Create<string, string>("QueryCreation", "LdapLatency"),
				PhotosLoggingMetadata.GetUserPhotoQueryCreationLdapLatency
			},
			{
				Tuple.Create<string, string>("CreateClientContext", "ElapsedTime"),
				PhotosLoggingMetadata.CreateClientContextTime
			},
			{
				Tuple.Create<string, string>("CreateClientContext", "LdapCount"),
				PhotosLoggingMetadata.CreateClientContextLdapCount
			},
			{
				Tuple.Create<string, string>("CreateClientContext", "LdapLatency"),
				PhotosLoggingMetadata.CreateClientContextLdapLatency
			},
			{
				Tuple.Create<string, string>("ProxyOverRestService", string.Empty),
				PhotosLoggingMetadata.RestProxy
			},
			{
				Tuple.Create<string, string>("MailboxPhotoHandlerComputeTargetPrincipal", "ElapsedTime"),
				PhotosLoggingMetadata.MailboxPhotoHandlerComputeTargetPrincipalTime
			},
			{
				Tuple.Create<string, string>("MailboxPhotoHandlerComputeTargetPrincipal", "LdapCount"),
				PhotosLoggingMetadata.MailboxPhotoHandlerComputeTargetPrincipalLdapCount
			},
			{
				Tuple.Create<string, string>("MailboxPhotoHandlerComputeTargetPrincipal", "LdapLatency"),
				PhotosLoggingMetadata.MailboxPhotoHandlerComputeTargetPrincipalLdapLatency
			},
			{
				Tuple.Create<string, string>("ADHandlerPhotoServed", string.Empty),
				PhotosLoggingMetadata.ADHandlerPhotoServed
			},
			{
				Tuple.Create<string, string>("FileSystemHandlerPhotoServed", string.Empty),
				PhotosLoggingMetadata.FileSystemHandlerPhotoServed
			},
			{
				Tuple.Create<string, string>("MailboxHandlerPhotoServed", string.Empty),
				PhotosLoggingMetadata.MailboxHandlerPhotoServed
			},
			{
				Tuple.Create<string, string>("ADFallbackPhotoServed", string.Empty),
				PhotosLoggingMetadata.ADFallbackPhotoServed
			},
			{
				Tuple.Create<string, string>("ADHandlerError", string.Empty),
				PhotosLoggingMetadata.ADHandlerError
			},
			{
				Tuple.Create<string, string>("FileSystemHandlerError", string.Empty),
				PhotosLoggingMetadata.FileSystemHandlerError
			},
			{
				Tuple.Create<string, string>("MailboxHandlerError", string.Empty),
				PhotosLoggingMetadata.MailboxHandlerError
			},
			{
				Tuple.Create<string, string>("ADHandlerPhotoAvailable", string.Empty),
				PhotosLoggingMetadata.ADHandlerPhotoAvailabile
			},
			{
				Tuple.Create<string, string>("FileSystemHandlerPhotoAvailable", string.Empty),
				PhotosLoggingMetadata.FileSystemHandlerPhotoAvailabile
			},
			{
				Tuple.Create<string, string>("MailboxHandlerPhotoAvailable", string.Empty),
				PhotosLoggingMetadata.MailboxHandlerPhotoAvailabile
			},
			{
				Tuple.Create<string, string>("HttpHandlerTotal", "ElapsedTime"),
				PhotosLoggingMetadata.HttpHandlerTotalTime
			},
			{
				Tuple.Create<string, string>("HttpHandlerTotal", "LdapCount"),
				PhotosLoggingMetadata.HttpHandlerTotalLdapCount
			},
			{
				Tuple.Create<string, string>("HttpHandlerTotal", "LdapLatency"),
				PhotosLoggingMetadata.HttpHandlerTotalLdapLatency
			},
			{
				Tuple.Create<string, string>("HttpHandlerSendRequestAndGetResponse", "ElapsedTime"),
				PhotosLoggingMetadata.HttpHandlerSendRequestAndGetResponseTime
			},
			{
				Tuple.Create<string, string>("HttpHandlerSendRequestAndGetResponse", "LdapCount"),
				PhotosLoggingMetadata.HttpHandlerSendRequestAndGetResponseLdapCount
			},
			{
				Tuple.Create<string, string>("HttpHandlerSendRequestAndGetResponse", "LdapLatency"),
				PhotosLoggingMetadata.HttpHandlerSendRequestAndGetResponseLdapLatency
			},
			{
				Tuple.Create<string, string>("HttpHandlerGetAndReadResponseStream", "ElapsedTime"),
				PhotosLoggingMetadata.HttpHandlerGetAndReadResponseStreamTime
			},
			{
				Tuple.Create<string, string>("HttpHandlerLocateService", "ElapsedTime"),
				PhotosLoggingMetadata.HttpHandlerLocateServiceTime
			},
			{
				Tuple.Create<string, string>("HttpHandlerLocateService", "LdapCount"),
				PhotosLoggingMetadata.HttpHandlerLocateServiceLdapCount
			},
			{
				Tuple.Create<string, string>("HttpHandlerLocateService", "LdapLatency"),
				PhotosLoggingMetadata.HttpHandlerLocateServiceLdapLatency
			},
			{
				Tuple.Create<string, string>("RouterTotal", "ElapsedTime"),
				PhotosLoggingMetadata.RouterTotalTime
			},
			{
				Tuple.Create<string, string>("RouterTotal", "LdapCount"),
				PhotosLoggingMetadata.RouterTotalLdapCount
			},
			{
				Tuple.Create<string, string>("RouterTotal", "LdapLatency"),
				PhotosLoggingMetadata.RouterTotalLdapLatency
			},
			{
				Tuple.Create<string, string>("RouterLookupTargetInDirectory", "ElapsedTime"),
				PhotosLoggingMetadata.RouterLookupTargetInDirectoryTime
			},
			{
				Tuple.Create<string, string>("RouterLookupTargetInDirectory", "LdapCount"),
				PhotosLoggingMetadata.RouterLookupTargetInDirectoryLdapCount
			},
			{
				Tuple.Create<string, string>("RouterLookupTargetInDirectory", "LdapLatency"),
				PhotosLoggingMetadata.RouterLookupTargetInDirectoryLdapLatency
			},
			{
				Tuple.Create<string, string>("RouterCheckTargetMailboxOnThisServer", "ElapsedTime"),
				PhotosLoggingMetadata.RouterCheckTargetMailboxOnThisServerTime
			},
			{
				Tuple.Create<string, string>("RouterCheckTargetMailboxOnThisServer", "LdapCount"),
				PhotosLoggingMetadata.RouterCheckTargetMailboxOnThisServerLdapCount
			},
			{
				Tuple.Create<string, string>("RouterCheckTargetMailboxOnThisServer", "LdapLatency"),
				PhotosLoggingMetadata.RouterCheckTargetMailboxOnThisServerLdapLatency
			},
			{
				Tuple.Create<string, string>("PrivateHandlerTotal", "ElapsedTime"),
				PhotosLoggingMetadata.PrivateHandlerTotalTime
			},
			{
				Tuple.Create<string, string>("PrivateHandlerTotal", "StoreRpcCount"),
				PhotosLoggingMetadata.PrivateHandlerTotalStoreRpcCount
			},
			{
				Tuple.Create<string, string>("PrivateHandlerTotal", "StoreRpcLatency"),
				PhotosLoggingMetadata.PrivateHandlerTotalStoreRpcLatency
			},
			{
				Tuple.Create<string, string>("PrivateHandlerReadPhotoStream", "ElapsedTime"),
				PhotosLoggingMetadata.PrivateHandlerReadPhotoStreamTime
			},
			{
				Tuple.Create<string, string>("PrivateHandlerReadPhotoStream", "StoreRpcCount"),
				PhotosLoggingMetadata.PrivateHandlerReadPhotoStreamStoreRpcCount
			},
			{
				Tuple.Create<string, string>("PrivateHandlerReadPhotoStream", "StoreRpcLatency"),
				PhotosLoggingMetadata.PrivateHandlerReadPhotoStreamStoreRpcLatency
			},
			{
				Tuple.Create<string, string>("PrivateHandlerServedContent", string.Empty),
				PhotosLoggingMetadata.PrivateHandlerServedContent
			},
			{
				Tuple.Create<string, string>("PrivateHandlerServedRedirect", string.Empty),
				PhotosLoggingMetadata.PrivateHandlerServedRedirect
			},
			{
				Tuple.Create<string, string>("ADHandlerProcessed", string.Empty),
				PhotosLoggingMetadata.ADHandlerProcessed
			},
			{
				Tuple.Create<string, string>("CachingHandlerProcessed", string.Empty),
				PhotosLoggingMetadata.CachingHandlerProcessed
			},
			{
				Tuple.Create<string, string>("FileSystemHandlerProcessed", string.Empty),
				PhotosLoggingMetadata.FileSystemHandlerProcessed
			},
			{
				Tuple.Create<string, string>("MailboxHandlerProcessed", string.Empty),
				PhotosLoggingMetadata.MailboxHandlerProcessed
			},
			{
				Tuple.Create<string, string>("HttpHandlerProcessed", string.Empty),
				PhotosLoggingMetadata.HttpHandlerProcessed
			},
			{
				Tuple.Create<string, string>("PrivateHandlerProcessed", string.Empty),
				PhotosLoggingMetadata.PrivateHandlerProcessed
			},
			{
				Tuple.Create<string, string>("LocalForestPhotoServiceLocatorLocateServer", "ElapsedTime"),
				PhotosLoggingMetadata.LocalForestPhotoServiceLocatorLocateServerTime
			},
			{
				Tuple.Create<string, string>("LocalForestPhotoServiceLocatorLocateServer", "LdapCount"),
				PhotosLoggingMetadata.LocalForestPhotoServiceLocatorLocateServerLdapCount
			},
			{
				Tuple.Create<string, string>("LocalForestPhotoServiceLocatorLocateServer", "LdapLatency"),
				PhotosLoggingMetadata.LocalForestPhotoServiceLocatorLocateServerLdapLatency
			},
			{
				Tuple.Create<string, string>("LocalForestPhotoServiceLocatorGetPhotoServiceUri", "ElapsedTime"),
				PhotosLoggingMetadata.LocalForestPhotoServiceLocatorGetPhotoServiceUriTime
			},
			{
				Tuple.Create<string, string>("LocalForestPhotoServiceLocatorGetPhotoServiceUri", "LdapCount"),
				PhotosLoggingMetadata.LocalForestPhotoServiceLocatorGetPhotoServiceUriLdapCount
			},
			{
				Tuple.Create<string, string>("LocalForestPhotoServiceLocatorGetPhotoServiceUri", "LdapLatency"),
				PhotosLoggingMetadata.LocalForestPhotoServiceLocatorGetPhotoServiceUriLdapLatency
			},
			{
				Tuple.Create<string, string>("WrongRoutingDetectedThenFallbackToOtherServer", string.Empty),
				PhotosLoggingMetadata.WrongRoutingDetectedThenFallbackToOtherServer
			},
			{
				Tuple.Create<string, string>("MiscRoutingAndDiscovery", string.Empty),
				PhotosLoggingMetadata.MiscRoutingAndDiscovery
			},
			{
				Tuple.Create<string, string>("RemoteForestHandlerServed", string.Empty),
				PhotosLoggingMetadata.RemoteForestHandlerServed
			},
			{
				Tuple.Create<string, string>("RemoteForestHandlerProcessed", string.Empty),
				PhotosLoggingMetadata.RemoteForestHandlerProcessed
			},
			{
				Tuple.Create<string, string>("RemoteForestHandlerError", string.Empty),
				PhotosLoggingMetadata.RemoteForestHandlerError
			},
			{
				Tuple.Create<string, string>("RemoteForestHandlerTotal", "ElapsedTime"),
				PhotosLoggingMetadata.RemoteForestHandlerTotalTime
			},
			{
				Tuple.Create<string, string>("RemoteForestHandlerTotal", "LdapCount"),
				PhotosLoggingMetadata.RemoteForestHandlerTotalLdapCount
			},
			{
				Tuple.Create<string, string>("RemoteForestHandlerTotal", "LdapLatency"),
				PhotosLoggingMetadata.RemoteForestHandlerTotalLdapLatency
			}
		};

		private readonly RequestDetailsLogger logger;

		private readonly ITracer tracer;
	}
}
