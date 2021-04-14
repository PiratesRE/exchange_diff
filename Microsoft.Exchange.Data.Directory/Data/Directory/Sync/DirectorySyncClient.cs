using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DebuggerStepThrough]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	public class DirectorySyncClient : ClientBase<IDirectorySync>, IDirectorySync
	{
		public DirectorySyncClient()
		{
		}

		public DirectorySyncClient(string endpointConfigurationName) : base(endpointConfigurationName)
		{
		}

		public DirectorySyncClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		public DirectorySyncClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		public DirectorySyncClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
		{
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		NewCookieResponse IDirectorySync.NewCookie(NewCookieRequest request)
		{
			return base.Channel.NewCookie(request);
		}

		public byte[] NewCookie(string serviceInstance, SyncOptions options, string[] alwaysReturnProperties)
		{
			NewCookieResponse newCookieResponse = ((IDirectorySync)this).NewCookie(new NewCookieRequest
			{
				serviceInstance = serviceInstance,
				options = options,
				alwaysReturnProperties = alwaysReturnProperties
			});
			return newCookieResponse.NewCookieResult;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		Task<NewCookieResponse> IDirectorySync.NewCookieAsync(NewCookieRequest request)
		{
			return base.Channel.NewCookieAsync(request);
		}

		public Task<NewCookieResponse> NewCookieAsync(string serviceInstance, SyncOptions options, string[] alwaysReturnProperties)
		{
			return ((IDirectorySync)this).NewCookieAsync(new NewCookieRequest
			{
				serviceInstance = serviceInstance,
				options = options,
				alwaysReturnProperties = alwaysReturnProperties
			});
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		NewCookie2Response IDirectorySync.NewCookie2(NewCookie2Request request)
		{
			return base.Channel.NewCookie2(request);
		}

		public byte[] NewCookie2(int schemaRevision, string serviceInstance, SyncOptions options, string[] objectClassesOfInterest, string[] propertiesOfInterest, string[] linkClassesOfInterest, string[] alwaysReturnProperties)
		{
			NewCookie2Response newCookie2Response = ((IDirectorySync)this).NewCookie2(new NewCookie2Request
			{
				schemaRevision = schemaRevision,
				serviceInstance = serviceInstance,
				options = options,
				objectClassesOfInterest = objectClassesOfInterest,
				propertiesOfInterest = propertiesOfInterest,
				linkClassesOfInterest = linkClassesOfInterest,
				alwaysReturnProperties = alwaysReturnProperties
			});
			return newCookie2Response.NewCookie2Result;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		Task<NewCookie2Response> IDirectorySync.NewCookie2Async(NewCookie2Request request)
		{
			return base.Channel.NewCookie2Async(request);
		}

		public Task<NewCookie2Response> NewCookie2Async(int schemaRevision, string serviceInstance, SyncOptions options, string[] objectClassesOfInterest, string[] propertiesOfInterest, string[] linkClassesOfInterest, string[] alwaysReturnProperties)
		{
			return ((IDirectorySync)this).NewCookie2Async(new NewCookie2Request
			{
				schemaRevision = schemaRevision,
				serviceInstance = serviceInstance,
				options = options,
				objectClassesOfInterest = objectClassesOfInterest,
				propertiesOfInterest = propertiesOfInterest,
				linkClassesOfInterest = linkClassesOfInterest,
				alwaysReturnProperties = alwaysReturnProperties
			});
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		GetChangesResponse IDirectorySync.GetChanges(GetChangesRequest request)
		{
			return base.Channel.GetChanges(request);
		}

		public DirectoryChanges GetChanges(byte[] lastCookie)
		{
			GetChangesResponse changes = ((IDirectorySync)this).GetChanges(new GetChangesRequest
			{
				lastCookie = lastCookie
			});
			return changes.GetChangesResult;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		Task<GetChangesResponse> IDirectorySync.GetChangesAsync(GetChangesRequest request)
		{
			return base.Channel.GetChangesAsync(request);
		}

		public Task<GetChangesResponse> GetChangesAsync(byte[] lastCookie)
		{
			return ((IDirectorySync)this).GetChangesAsync(new GetChangesRequest
			{
				lastCookie = lastCookie
			});
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		PublishResponse IDirectorySync.Publish(PublishRequest request)
		{
			return base.Channel.Publish(request);
		}

		public UpdateResultCode[] Publish(ServicePublication[] publications)
		{
			PublishResponse publishResponse = ((IDirectorySync)this).Publish(new PublishRequest
			{
				publications = publications
			});
			return publishResponse.PublishResult;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		Task<PublishResponse> IDirectorySync.PublishAsync(PublishRequest request)
		{
			return base.Channel.PublishAsync(request);
		}

		public Task<PublishResponse> PublishAsync(ServicePublication[] publications)
		{
			return ((IDirectorySync)this).PublishAsync(new PublishRequest
			{
				publications = publications
			});
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		GetContextResponse IDirectorySync.GetContext(GetContextRequest request)
		{
			return base.Channel.GetContext(request);
		}

		public DirectoryObjectsAndLinks GetContext(byte[] lastCookie, string contextId, byte[] lastPageToken)
		{
			GetContextResponse context = ((IDirectorySync)this).GetContext(new GetContextRequest
			{
				lastCookie = lastCookie,
				contextId = contextId,
				lastPageToken = lastPageToken
			});
			return context.GetContextResult;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		Task<GetContextResponse> IDirectorySync.GetContextAsync(GetContextRequest request)
		{
			return base.Channel.GetContextAsync(request);
		}

		public Task<GetContextResponse> GetContextAsync(byte[] lastCookie, string contextId, byte[] lastPageToken)
		{
			return ((IDirectorySync)this).GetContextAsync(new GetContextRequest
			{
				lastCookie = lastCookie,
				contextId = contextId,
				lastPageToken = lastPageToken
			});
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		GetDirectoryObjectsResponse IDirectorySync.GetDirectoryObjects(GetDirectoryObjectsRequest request)
		{
			return base.Channel.GetDirectoryObjects(request);
		}

		public DirectoryObjectsAndLinks GetDirectoryObjects(byte[] lastGetChangesCookieOrGetContextPageToken, DirectoryObjectIdentity[] objectIdentities, GetDirectoryObjectsOptions? options, byte[] lastPageToken)
		{
			GetDirectoryObjectsResponse directoryObjects = ((IDirectorySync)this).GetDirectoryObjects(new GetDirectoryObjectsRequest
			{
				lastGetChangesCookieOrGetContextPageToken = lastGetChangesCookieOrGetContextPageToken,
				objectIdentities = objectIdentities,
				options = options,
				lastPageToken = lastPageToken
			});
			return directoryObjects.GetDirectoryObjectsResult;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		Task<GetDirectoryObjectsResponse> IDirectorySync.GetDirectoryObjectsAsync(GetDirectoryObjectsRequest request)
		{
			return base.Channel.GetDirectoryObjectsAsync(request);
		}

		public Task<GetDirectoryObjectsResponse> GetDirectoryObjectsAsync(byte[] lastGetChangesCookieOrGetContextPageToken, DirectoryObjectIdentity[] objectIdentities, GetDirectoryObjectsOptions? options, byte[] lastPageToken)
		{
			return ((IDirectorySync)this).GetDirectoryObjectsAsync(new GetDirectoryObjectsRequest
			{
				lastGetChangesCookieOrGetContextPageToken = lastGetChangesCookieOrGetContextPageToken,
				objectIdentities = objectIdentities,
				options = options,
				lastPageToken = lastPageToken
			});
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		MergeCookiesResponse IDirectorySync.MergeCookies(MergeCookiesRequest request)
		{
			return base.Channel.MergeCookies(request);
		}

		public DirectoryChanges MergeCookies(byte[] lastGetChangesCookie, byte[] lastGetContextPageToken, byte[] lastMergeCookie)
		{
			MergeCookiesResponse mergeCookiesResponse = ((IDirectorySync)this).MergeCookies(new MergeCookiesRequest
			{
				lastGetChangesCookie = lastGetChangesCookie,
				lastGetContextPageToken = lastGetContextPageToken,
				lastMergeCookie = lastMergeCookie
			});
			return mergeCookiesResponse.MergeCookiesResult;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		Task<MergeCookiesResponse> IDirectorySync.MergeCookiesAsync(MergeCookiesRequest request)
		{
			return base.Channel.MergeCookiesAsync(request);
		}

		public Task<MergeCookiesResponse> MergeCookiesAsync(byte[] lastGetChangesCookie, byte[] lastGetContextPageToken, byte[] lastMergeCookie)
		{
			return ((IDirectorySync)this).MergeCookiesAsync(new MergeCookiesRequest
			{
				lastGetChangesCookie = lastGetChangesCookie,
				lastGetContextPageToken = lastGetContextPageToken,
				lastMergeCookie = lastMergeCookie
			});
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		GetDirSyncDrainageStatusResponse IDirectorySync.GetDirSyncDrainageStatus(GetDirSyncDrainageStatusRequest request)
		{
			return base.Channel.GetDirSyncDrainageStatus(request);
		}

		public DirSyncDrainageCode[] GetDirSyncDrainageStatus(ContextDirSyncStatus[] contextDirSyncStatusList, byte[] getChangesCookie)
		{
			GetDirSyncDrainageStatusResponse dirSyncDrainageStatus = ((IDirectorySync)this).GetDirSyncDrainageStatus(new GetDirSyncDrainageStatusRequest
			{
				contextDirSyncStatusList = contextDirSyncStatusList,
				getChangesCookie = getChangesCookie
			});
			return dirSyncDrainageStatus.GetDirSyncDrainageStatusResult;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		Task<GetDirSyncDrainageStatusResponse> IDirectorySync.GetDirSyncDrainageStatusAsync(GetDirSyncDrainageStatusRequest request)
		{
			return base.Channel.GetDirSyncDrainageStatusAsync(request);
		}

		public Task<GetDirSyncDrainageStatusResponse> GetDirSyncDrainageStatusAsync(ContextDirSyncStatus[] contextDirSyncStatusList, byte[] getChangesCookie)
		{
			return ((IDirectorySync)this).GetDirSyncDrainageStatusAsync(new GetDirSyncDrainageStatusRequest
			{
				contextDirSyncStatusList = contextDirSyncStatusList,
				getChangesCookie = getChangesCookie
			});
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		UpdateCookieResponse IDirectorySync.UpdateCookie(UpdateCookieRequest request)
		{
			return base.Channel.UpdateCookie(request);
		}

		public byte[] UpdateCookie(byte[] getChangesCookie, int? schemaRevision, SyncOptions? options, string[] objectClassesOfInterest, string[] propertiesOfInterest, string[] linkClassesOfInterest, string[] alwaysReturnProperties)
		{
			UpdateCookieResponse updateCookieResponse = ((IDirectorySync)this).UpdateCookie(new UpdateCookieRequest
			{
				getChangesCookie = getChangesCookie,
				schemaRevision = schemaRevision,
				options = options,
				objectClassesOfInterest = objectClassesOfInterest,
				propertiesOfInterest = propertiesOfInterest,
				linkClassesOfInterest = linkClassesOfInterest,
				alwaysReturnProperties = alwaysReturnProperties
			});
			return updateCookieResponse.UpdateCookieResult;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		Task<UpdateCookieResponse> IDirectorySync.UpdateCookieAsync(UpdateCookieRequest request)
		{
			return base.Channel.UpdateCookieAsync(request);
		}

		public Task<UpdateCookieResponse> UpdateCookieAsync(byte[] getChangesCookie, int? schemaRevision, SyncOptions? options, string[] objectClassesOfInterest, string[] propertiesOfInterest, string[] linkClassesOfInterest, string[] alwaysReturnProperties)
		{
			return ((IDirectorySync)this).UpdateCookieAsync(new UpdateCookieRequest
			{
				getChangesCookie = getChangesCookie,
				schemaRevision = schemaRevision,
				options = options,
				objectClassesOfInterest = objectClassesOfInterest,
				propertiesOfInterest = propertiesOfInterest,
				linkClassesOfInterest = linkClassesOfInterest,
				alwaysReturnProperties = alwaysReturnProperties
			});
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		GetCookieUpdateStatusResponse IDirectorySync.GetCookieUpdateStatus(GetCookieUpdateStatusRequest request)
		{
			return base.Channel.GetCookieUpdateStatus(request);
		}

		public CookieUpdateStatus GetCookieUpdateStatus(byte[] getChangesCookie)
		{
			GetCookieUpdateStatusResponse cookieUpdateStatus = ((IDirectorySync)this).GetCookieUpdateStatus(new GetCookieUpdateStatusRequest
			{
				getChangesCookie = getChangesCookie
			});
			return cookieUpdateStatus.GetCookieUpdateStatusResult;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		Task<GetCookieUpdateStatusResponse> IDirectorySync.GetCookieUpdateStatusAsync(GetCookieUpdateStatusRequest request)
		{
			return base.Channel.GetCookieUpdateStatusAsync(request);
		}

		public Task<GetCookieUpdateStatusResponse> GetCookieUpdateStatusAsync(byte[] getChangesCookie)
		{
			return ((IDirectorySync)this).GetCookieUpdateStatusAsync(new GetCookieUpdateStatusRequest
			{
				getChangesCookie = getChangesCookie
			});
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		FilterAndGetContextRecoveryInfoResponse IDirectorySync.FilterAndGetContextRecoveryInfo(FilterAndGetContextRecoveryInfoRequest request)
		{
			return base.Channel.FilterAndGetContextRecoveryInfo(request);
		}

		public ContextRecoveryInfo FilterAndGetContextRecoveryInfo(byte[] getChangesCookie, string contextId)
		{
			FilterAndGetContextRecoveryInfoResponse filterAndGetContextRecoveryInfoResponse = ((IDirectorySync)this).FilterAndGetContextRecoveryInfo(new FilterAndGetContextRecoveryInfoRequest
			{
				getChangesCookie = getChangesCookie,
				contextId = contextId
			});
			return filterAndGetContextRecoveryInfoResponse.FilterAndGetContextRecoveryInfoResult;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		Task<FilterAndGetContextRecoveryInfoResponse> IDirectorySync.FilterAndGetContextRecoveryInfoAsync(FilterAndGetContextRecoveryInfoRequest request)
		{
			return base.Channel.FilterAndGetContextRecoveryInfoAsync(request);
		}

		public Task<FilterAndGetContextRecoveryInfoResponse> FilterAndGetContextRecoveryInfoAsync(byte[] getChangesCookie, string contextId)
		{
			return ((IDirectorySync)this).FilterAndGetContextRecoveryInfoAsync(new FilterAndGetContextRecoveryInfoRequest
			{
				getChangesCookie = getChangesCookie,
				contextId = contextId
			});
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		EstimateBacklogResponse IDirectorySync.EstimateBacklog(EstimateBacklogRequest request)
		{
			return base.Channel.EstimateBacklog(request);
		}

		public BacklogEstimateBatch EstimateBacklog(byte[] latestGetChangesCookie, byte[] lastPageToken)
		{
			EstimateBacklogResponse estimateBacklogResponse = ((IDirectorySync)this).EstimateBacklog(new EstimateBacklogRequest
			{
				latestGetChangesCookie = latestGetChangesCookie,
				lastPageToken = lastPageToken
			});
			return estimateBacklogResponse.EstimateBacklogResult;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		Task<EstimateBacklogResponse> IDirectorySync.EstimateBacklogAsync(EstimateBacklogRequest request)
		{
			return base.Channel.EstimateBacklogAsync(request);
		}

		public Task<EstimateBacklogResponse> EstimateBacklogAsync(byte[] latestGetChangesCookie, byte[] lastPageToken)
		{
			return ((IDirectorySync)this).EstimateBacklogAsync(new EstimateBacklogRequest
			{
				latestGetChangesCookie = latestGetChangesCookie,
				lastPageToken = lastPageToken
			});
		}
	}
}
