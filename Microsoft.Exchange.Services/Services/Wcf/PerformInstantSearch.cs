using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using System.Web.Caching;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class PerformInstantSearch : SingleStepServiceCommand<PerformInstantSearchRequest, PerformInstantSearchResponse>
	{
		internal static InstantSearchManager GetManagerForCaller(CallContext callContext, string deviceId)
		{
			string key = PerformInstantSearch.BuildCacheKey(callContext, deviceId);
			InstantSearchManager instantSearchManager = HttpRuntime.Cache.Get(key) as InstantSearchManager;
			if (instantSearchManager == null)
			{
				InstantSearchManager instantSearchManager2 = null;
				try
				{
					instantSearchManager2 = new InstantSearchManager(() => MailboxSession.Open(callContext.MailboxIdentityPrincipal, callContext.EffectiveCaller.ClientSecurityContext, callContext.ClientCulture, "Client=WebServices;Action=PerformInstantSearch"));
					lock (PerformInstantSearch.searchManagerCreationMutex)
					{
						instantSearchManager = (HttpRuntime.Cache.Get(key) as InstantSearchManager);
						if (instantSearchManager == null)
						{
							HttpRuntime.Cache.Insert(key, instantSearchManager2, null, Cache.NoAbsoluteExpiration, PerformInstantSearch.searchManagerExpiration, CacheItemPriority.NotRemovable, new CacheItemRemovedCallback(PerformInstantSearch.OnCacheEntryExpired));
							instantSearchManager = instantSearchManager2;
							instantSearchManager2 = null;
						}
					}
				}
				finally
				{
					if (instantSearchManager2 != null)
					{
						instantSearchManager2.Dispose();
						instantSearchManager2 = null;
					}
				}
			}
			return instantSearchManager;
		}

		internal static void RemoveManagerForCaller(CallContext callContext, string deviceId)
		{
			string key = PerformInstantSearch.BuildCacheKey(callContext, deviceId);
			lock (PerformInstantSearch.searchManagerCreationMutex)
			{
				HttpRuntime.Cache.Remove(key);
			}
		}

		private static string BuildCacheKey(CallContext callContext, string deviceId)
		{
			return string.Format("InstantSearchManager_{0}_{1}_{2}_{3}_{4}", new object[]
			{
				callContext.EffectiveCaller.ObjectGuid,
				callContext.OriginalCallerContext.IdentifierString,
				callContext.MailboxIdentityPrincipal.MailboxInfo.MailboxGuid,
				callContext.LogonType,
				deviceId
			});
		}

		private static void OnCacheEntryExpired(string key, object value, CacheItemRemovedReason reason)
		{
			InstantSearchManager instantSearchManager = value as InstantSearchManager;
			if (instantSearchManager != null)
			{
				instantSearchManager.Dispose();
			}
		}

		public PerformInstantSearch(CallContext callContext, PerformInstantSearchRequest request) : base(callContext, request)
		{
			ServiceCommandBase.ThrowIfNull(request, "request", "ServiceCommand::PerformInstantSearch");
			OwsLogRegistry.Register("PerformInstantSearch", typeof(PerformInstantSearchMetaData), new Type[0]);
			this.instantSearchRequest = request;
			this.instantSearchManager = PerformInstantSearch.GetManagerForCaller(callContext, request.DeviceId);
			this.notificationHandler = new NullInstantSearchNotificationHandler();
			this.searchPerfMarkers = new SearchPerfMarkerContainer();
			this.searchPerfMarkers.SetPerfMarker(InstantSearchPerfKey.ServiceCommandInvocationTimeStamp);
		}

		public PerformInstantSearch(CallContext callContext, InstantSearchManager instantSearchManager, IInstantSearchNotificationHandler notificationHandler, PerformInstantSearchRequest request) : base(callContext, request)
		{
			ServiceCommandBase.ThrowIfNull(instantSearchManager, "instantSearchManager", "ServiceCommand::PerformInstantSearch");
			ServiceCommandBase.ThrowIfNull(notificationHandler, "notificationHandler", "ServiceCommand::PerformInstantSearch");
			ServiceCommandBase.ThrowIfNull(request, "request", "ServiceCommand::PerformInstantSearch");
			OwsLogRegistry.Register("PerformInstantSearch", typeof(PerformInstantSearchMetaData), new Type[0]);
			this.instantSearchRequest = request;
			this.instantSearchManager = instantSearchManager;
			this.notificationHandler = notificationHandler;
			this.searchPerfMarkers = new SearchPerfMarkerContainer();
			this.searchPerfMarkers.SetPerfMarker(InstantSearchPerfKey.ServiceCommandInvocationTimeStamp);
		}

		public PerformInstantSearch(CallContext callContext, Action<InstantSearchPayloadType> searchPayloadCallback, PerformInstantSearchRequest request) : base(callContext, request)
		{
			ServiceCommandBase.ThrowIfNull(searchPayloadCallback, "searchPayloadCallback", "ServiceCommand::PerformInstantSearch");
			ServiceCommandBase.ThrowIfNull(request, "request", "ServiceCommand::PerformInstantSearch");
			this.instantSearchRequest = request;
			if (this.instantSearchRequest.FolderScope == null || this.instantSearchRequest.FolderScope.Length == 0)
			{
				throw new ServiceArgumentException((CoreResources.IDs)3784063568U, CoreResources.FolderScopeNotSpecified);
			}
			this.instantSearchManager = PerformInstantSearch.GetManagerForCaller(callContext, request.DeviceId);
			this.notificationHandler = new SimpleInstantSearchNotificationHandler(searchPayloadCallback);
			this.searchPerfMarkers = new SearchPerfMarkerContainer();
			this.searchPerfMarkers.SetPerfMarker(InstantSearchPerfKey.ServiceCommandInvocationTimeStamp);
		}

		internal override ServiceResult<PerformInstantSearchResponse> Execute()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			ServiceResult<PerformInstantSearchResponse> result;
			try
			{
				List<StoreId> folderScope = this.GetFolderScope();
				bool flag = false;
				InstantSearchSession orCreateSession = this.instantSearchManager.GetOrCreateSession(this.instantSearchRequest.SearchSessionId, folderScope, this.instantSearchRequest.ItemType, this.instantSearchRequest.SearchRequestId, this.instantSearchRequest.SuggestionSources, out flag);
				PerformInstantSearchResponse value = orCreateSession.PerformInstantSearch(this.instantSearchRequest, base.CallContext, folderScope, this.searchPerfMarkers, this.notificationHandler);
				result = new ServiceResult<PerformInstantSearchResponse>(value);
			}
			finally
			{
				InstantSearchSession.SafeLogData(base.CallContext, PerformInstantSearchMetaData.ApplicationID, this.instantSearchRequest.ApplicationId);
				InstantSearchSession.SafeLogData(base.CallContext, PerformInstantSearchMetaData.InternalExecuteTime, stopwatch.ElapsedMilliseconds);
			}
			return result;
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return base.Result.Value;
		}

		private List<StoreId> GetFolderScope()
		{
			List<StoreId> list = new List<StoreId>(this.instantSearchRequest.FolderScope.Length);
			foreach (FolderId folderId in this.instantSearchRequest.FolderScope)
			{
				if (folderId == null)
				{
					throw new ServiceArgumentException((CoreResources.IDs)3784063568U, CoreResources.InstantSearchNullFolderId);
				}
				StoreObjectId item = IdConverter.EwsIdToFolderId(folderId.Id);
				list.Add(item);
			}
			return list;
		}

		private static readonly TimeSpan searchManagerExpiration = TimeSpan.FromMinutes(10.0);

		private static object searchManagerCreationMutex = new object();

		private readonly InstantSearchManager instantSearchManager;

		private readonly IInstantSearchNotificationHandler notificationHandler;

		public readonly PerformInstantSearchRequest instantSearchRequest;

		private readonly SearchPerfMarkerContainer searchPerfMarkers;
	}
}
