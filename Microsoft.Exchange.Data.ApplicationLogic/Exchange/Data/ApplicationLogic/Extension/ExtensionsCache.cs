using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal class ExtensionsCache
	{
		internal static ExtensionsCache Singleton
		{
			get
			{
				return ExtensionsCache.singleton;
			}
		}

		internal bool SkipSubmitUpdateQueryForTest { get; set; }

		internal int SubmitCount { get; set; }

		internal int Size { get; set; }

		internal DateTime LastCacheCleanupTime { get; set; }

		internal int GetUpdatesCount { get; set; }

		internal TokenRenewSubmitter TokenRenewSubmitter
		{
			get
			{
				return this.tokenRenewSubmitter;
			}
		}

		internal int QueryQueueCount
		{
			get
			{
				return this.queryQueue.Count;
			}
		}

		internal ExtensionsCache()
		{
			this.LastCacheCleanupTime = DateTime.UtcNow;
			this.extensionsDictionary.OnReplaced += this.OnReplaced;
		}

		internal ExtensionsCache(OmexWebServiceUrlsCache urlsCache) : this()
		{
			if (urlsCache == null)
			{
				throw new ArgumentNullException("urlsCache");
			}
			this.urlsCache = urlsCache;
			this.tokenRenewSubmitter = new TokenRenewSubmitter(urlsCache);
		}

		internal bool TryGetEntry(string marketplaceAssetID, out ExtensionsCacheEntry extensionCacheEntry)
		{
			ExtensionsCacheEntry extensionsCacheEntry = null;
			if (this.extensionsDictionary.TryGetValue(marketplaceAssetID, out extensionsCacheEntry) && !InstalledExtensionTable.IsUpdateCheckTimeExpired(extensionsCacheEntry.LastUpdateCheckTime))
			{
				extensionCacheEntry = extensionsCacheEntry;
			}
			else
			{
				extensionCacheEntry = null;
			}
			return extensionCacheEntry != null;
		}

		internal int Count
		{
			get
			{
				return this.extensionsDictionary.Count;
			}
		}

		private void OnReplaced(object sender, MruDictionaryElementReplacedEventArgs<string, ExtensionsCacheEntry> eventArgs)
		{
			if (eventArgs.OldKeyValuePair.Value != null && eventArgs.NewKeyValuePair.Value != null)
			{
				lock (this.extensionsDictionary.SyncRoot)
				{
					this.Size -= eventArgs.OldKeyValuePair.Value.Size;
				}
			}
		}

		private void CleanupCache()
		{
			lock (this.extensionsDictionary.SyncRoot)
			{
				if (this.LastCacheCleanupTime.AddDays(1.0) < DateTime.UtcNow)
				{
					List<ExtensionsCacheEntry> list = new List<ExtensionsCacheEntry>(this.extensionsDictionary.Count);
					this.Size = 0;
					foreach (KeyValuePair<string, ExtensionsCacheEntry> keyValuePair in this.extensionsDictionary)
					{
						ExtensionsCacheEntry value = keyValuePair.Value;
						if (value.LastUpdateCheckTime.AddDays(14.0) < DateTime.UtcNow)
						{
							list.Add(value);
						}
						else
						{
							this.Size += value.Size;
						}
					}
					foreach (ExtensionsCacheEntry extensionsCacheEntry in list)
					{
						ExtensionsCache.Tracer.TraceDebug<string>(0L, "ExtensionsCache.CleanupCache: Removing Extension {0}.", extensionsCacheEntry.MarketplaceAssetID);
						this.extensionsDictionary.Remove(extensionsCacheEntry.MarketplaceAssetID);
					}
					ExtensionsCache.Tracer.TraceDebug<int>(0L, "ExtensionsCache.CleanupCache: Current cache size {0}.", this.Size);
				}
			}
		}

		internal bool TryGetExtensionUpdate(ExtensionData extensionData, out byte[] manifestBytes)
		{
			manifestBytes = null;
			this.CleanupCache();
			ExtensionsCacheEntry extensionsCacheEntry = null;
			if (this.extensionsDictionary.TryGetValue(extensionData.MarketplaceAssetID, out extensionsCacheEntry))
			{
				if (extensionData.ExtensionId != extensionsCacheEntry.ExtensionID)
				{
					ExtensionsCache.Tracer.TraceError<string, string, string>(0L, "ExtensionsCache.TryGetExtensionUpdate: Extension {0} ExtensionID property {1} does not match cache entry value {2}.", extensionData.MarketplaceAssetID, extensionData.ExtensionId, extensionsCacheEntry.ExtensionID);
					ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_MismatchedCacheMailboxExtensionId, extensionData.MarketplaceAssetID, new object[]
					{
						"ProcessUpdates",
						extensionData.MarketplaceAssetID,
						extensionData.ExtensionId,
						extensionsCacheEntry.ExtensionID
					});
				}
				else if (extensionData.Version != null && extensionsCacheEntry.RequestedCapabilities != null && extensionsCacheEntry.Manifest != null && extensionData.Version < extensionsCacheEntry.Version && ExtensionData.CompareCapabilities(extensionsCacheEntry.RequestedCapabilities.Value, extensionData.RequestedCapabilities.Value) <= 0 && GetUpdates.IsValidUpdateState(new OmexConstants.AppState?(extensionsCacheEntry.State)))
				{
					manifestBytes = extensionsCacheEntry.Manifest;
				}
			}
			return manifestBytes != null;
		}

		internal void SubmitUpdateQuery(ICollection<ExtensionData> extensions, UpdateQueryContext queryContext)
		{
			if (extensions == null)
			{
				throw new ArgumentNullException("extensions");
			}
			if (extensions.Count == 0)
			{
				throw new ArgumentException("extensions must contain one or more extensions");
			}
			if (this.SkipSubmitUpdateQueryForTest)
			{
				this.SubmitCount = 0;
			}
			Dictionary<string, UpdateRequestAsset> dictionary = new Dictionary<string, UpdateRequestAsset>(extensions.Count);
			foreach (ExtensionData extensionData in extensions)
			{
				if (extensionData.Version == null)
				{
					ExtensionsCache.Tracer.TraceDebug<string>(0L, "ExtensionsCache.SubmitUpdateQuery: Extension {0} not added to query list because version is invalid", extensionData.MarketplaceAssetID);
					ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_InvalidVersionSubmitUpdateQuery, extensionData.MarketplaceAssetID, new object[]
					{
						"ProcessUpdates",
						ExtensionDiagnostics.GetLoggedMailboxIdentifier(queryContext.ExchangePrincipal),
						extensionData.MarketplaceAssetID
					});
				}
				else
				{
					if (extensionData.Scope == null)
					{
						throw new ArgumentNullException("extensionData.Scope");
					}
					if (extensionData.RequestedCapabilities == null)
					{
						throw new ArgumentNullException("extensionData.RequestedCapabilities");
					}
					ExtensionsCacheEntry extensionsCacheEntry = null;
					if (this.extensionsDictionary.TryGetValue(extensionData.MarketplaceAssetID, out extensionsCacheEntry) && !InstalledExtensionTable.IsUpdateCheckTimeExpired(extensionsCacheEntry.LastUpdateCheckTime) && extensionsCacheEntry.Version == extensionData.Version)
					{
						ExtensionsCache.Tracer.TraceDebug<string>(0L, "ExtensionsCache.SubmitUpdateQuery: Extension {0} not added to query list because version matches recent cache entry", extensionData.MarketplaceAssetID);
					}
					else
					{
						UpdateRequestAsset updateRequestAsset = null;
						if (dictionary.TryGetValue(extensionData.MarketplaceAssetID, out updateRequestAsset))
						{
							ExtensionsCache.Tracer.TraceDebug<string, string, string>(0L, "ExtensionsCache.SubmitUpdateQuery: Extension {0} not added to query list because asset with same MarketplaceAssetID is already in list. ExtensionIds with same asset id: {1} {2}", extensionData.MarketplaceAssetID, extensionData.ExtensionId, updateRequestAsset.ExtensionID);
						}
						else
						{
							dictionary.Add(extensionData.MarketplaceAssetID, new UpdateRequestAsset
							{
								MarketplaceContentMarket = extensionData.MarketplaceContentMarket,
								ExtensionID = extensionData.ExtensionId,
								MarketplaceAssetID = extensionData.MarketplaceAssetID,
								RequestedCapabilities = extensionData.RequestedCapabilities.Value,
								Version = extensionData.Version,
								DisableReason = extensionData.DisableReason,
								Enabled = extensionData.Enabled,
								Scope = extensionData.Scope.Value,
								Etoken = extensionData.Etoken
							});
						}
					}
				}
			}
			if (dictionary.Count == 0)
			{
				ExtensionsCache.Tracer.TraceDebug(0L, "ExtensionsCache.SubmitUpdateQuery: UpdateRequestAssets count is 0. Updates query will not be started.");
				return;
			}
			queryContext.UpdateRequestAssets = dictionary;
			queryContext.DeploymentId = ExtensionDataHelper.GetDeploymentId(queryContext.Domain);
			this.QueueQueryItem(queryContext);
		}

		internal void QueueQueryItem(UpdateQueryContext queryContext)
		{
			GetUpdates getUpdates = null;
			lock (this.queryQueueLockObject)
			{
				if (this.queryQueue.Count > 500)
				{
					ExtensionsCache.Tracer.TraceError<IExchangePrincipal>(0L, "Query for {0} not added to the query queue because queue is full.", queryContext.ExchangePrincipal);
					ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_ExtensionUpdateQueryMaxExceeded, null, new object[]
					{
						"ProcessUpdates",
						ExtensionDiagnostics.GetLoggedMailboxIdentifier(queryContext.ExchangePrincipal)
					});
					return;
				}
				ExtensionsCache.Tracer.TraceDebug<IExchangePrincipal>(0L, "Adding query for {0} to the query queue.", queryContext.ExchangePrincipal);
				this.queryQueue.Enqueue(queryContext);
				if (this.GetUpdatesCount < 50)
				{
					getUpdates = new GetUpdates(this.urlsCache, this);
					this.GetUpdatesCount++;
					ExtensionsCache.Tracer.TraceDebug<int>(0L, "Creating a new instance of GetUpdates. GetUpdates Count {0}", this.GetUpdatesCount);
				}
				else
				{
					ExtensionsCache.Tracer.TraceDebug<int>(0L, "Too many GetUpdates commands. Query will be handled from pool. GetUpdates Count {0}", this.GetUpdatesCount);
				}
			}
			if (getUpdates != null)
			{
				this.ExecuteUpdateQuery(getUpdates);
			}
		}

		internal void ExecuteUpdateQuery(GetUpdates getUpdates)
		{
			UpdateQueryContext updateQueryContext = null;
			lock (this.queryQueueLockObject)
			{
				if (this.queryQueue.Count > 0)
				{
					updateQueryContext = this.queryQueue.Dequeue();
				}
				else
				{
					this.GetUpdatesCount--;
					if (this.GetUpdatesCount < 0)
					{
						throw new InvalidOperationException("GetUpdatesCount can't be less than 0.");
					}
					ExtensionsCache.Tracer.TraceDebug<int>(0L, "Query queue is empty. GetUpdates Count {0}", this.GetUpdatesCount);
				}
			}
			if (updateQueryContext != null)
			{
				ExtensionsCache.Tracer.TraceDebug<IExchangePrincipal>(0L, "Starting query for {0}.", updateQueryContext.ExchangePrincipal);
				if (this.SkipSubmitUpdateQueryForTest)
				{
					this.SubmitCount = updateQueryContext.UpdateRequestAssets.Count;
					return;
				}
				getUpdates.Execute(updateQueryContext);
			}
		}

		internal void Update(AppStateResponseAsset appStateResponseAsset)
		{
			if (appStateResponseAsset == null)
			{
				throw new ArgumentNullException("appStateResponseAsset");
			}
			if (appStateResponseAsset.State == null)
			{
				throw new ArgumentNullException("appStateResponseAsset.State");
			}
			if (appStateResponseAsset.Version == null)
			{
				throw new ArgumentNullException("appStateResponseAsset.Version");
			}
			byte[] manifest = null;
			RequestedCapabilities? requestedCapabilities = null;
			ExtensionsCacheEntry extensionsCacheEntry = null;
			if (this.extensionsDictionary.TryGetValue(appStateResponseAsset.MarketplaceAssetID, out extensionsCacheEntry) && extensionsCacheEntry.Version == appStateResponseAsset.Version)
			{
				ExtensionsCache.Tracer.TraceDebug<string>(0L, "ExtensionsCache.Update: Since version is unchanged, getting properties from extension entry {0} for add", appStateResponseAsset.MarketplaceAssetID);
				requestedCapabilities = extensionsCacheEntry.RequestedCapabilities;
				if (GetUpdates.IsValidUpdateState(new OmexConstants.AppState?(appStateResponseAsset.State.Value)))
				{
					manifest = extensionsCacheEntry.Manifest;
				}
				else
				{
					manifest = null;
				}
			}
			ExtensionsCache.Tracer.TraceDebug<string>(0L, "ExtensionsCache.Update: Adding extension {0} from AppStateResponse", appStateResponseAsset.MarketplaceAssetID);
			ExtensionsCacheEntry entry = new ExtensionsCacheEntry(appStateResponseAsset.MarketplaceAssetID, appStateResponseAsset.ExtensionID, appStateResponseAsset.Version, requestedCapabilities, appStateResponseAsset.State.Value, manifest);
			this.AddExtension(entry);
		}

		internal void Add(ExtensionData extensionData, OmexConstants.AppState state)
		{
			byte[] manifestBytes = extensionData.GetManifestBytes();
			if (manifestBytes == null || manifestBytes.Length == 0)
			{
				throw new ArgumentNullException("extensionData Manifest");
			}
			if (extensionData.Version == null)
			{
				throw new ArgumentNullException("extensionData Version");
			}
			if (extensionData.RequestedCapabilities == null)
			{
				throw new ArgumentNullException("extensionData RequestedCapabilities");
			}
			ExtensionsCache.Tracer.TraceDebug<string>(0L, "ExtensionsCache.Add: Adding Extension {0} from ExtensionData", extensionData.MarketplaceAssetID);
			ExtensionsCacheEntry entry = new ExtensionsCacheEntry(extensionData.MarketplaceAssetID, extensionData.ExtensionId, extensionData.Version, new RequestedCapabilities?(extensionData.RequestedCapabilities.Value), state, manifestBytes);
			this.AddExtension(entry);
		}

		private void AddExtension(ExtensionsCacheEntry entry)
		{
			lock (this.extensionsDictionary.SyncRoot)
			{
				this.MaintainCacheSize(entry);
				this.extensionsDictionary.Add(entry.MarketplaceAssetID, entry);
			}
		}

		private void MaintainCacheSize(ExtensionsCacheEntry entry)
		{
			int num = this.Size + entry.Size;
			if (num > 512000)
			{
				List<ExtensionsCacheEntry> list = new List<ExtensionsCacheEntry>();
				foreach (KeyValuePair<string, ExtensionsCacheEntry> keyValuePair in this.extensionsDictionary)
				{
					ExtensionsCacheEntry value = keyValuePair.Value;
					list.Add(value);
					num -= value.Size;
					if (num <= 460800)
					{
						break;
					}
				}
				foreach (ExtensionsCacheEntry extensionsCacheEntry in list)
				{
					ExtensionsCache.Tracer.TraceDebug<string>(0L, "ExtensionsCache.MaintainCacheSize: Removing Extension {0}.", extensionsCacheEntry.MarketplaceAssetID);
					this.extensionsDictionary.Remove(extensionsCacheEntry.MarketplaceAssetID);
				}
				ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_ExtensionsCacheReachedMaxSize, null, new object[]
				{
					"ProcessUpdates"
				});
			}
			this.Size = num;
			ExtensionsCache.Tracer.TraceDebug<int>(0L, "ExtensionsCache.MaintainCacheSize: Current cache size {0}.", this.Size);
		}

		internal static string BuildClientInfoString(string clientInfoStringPrefix)
		{
			return clientInfoStringPrefix + "ExtensionUpdate";
		}

		private const string ExtensionUpdateClientInfoPart = "ExtensionUpdate";

		private const string ScenarioProcessUpdates = "ProcessUpdates";

		private const int MaxEntryCount = 500000;

		private const int QueryQueueMaxCount = 500;

		internal const int GetUpdatesMaxCount = 50;

		internal const int MaxCacheSize = 512000;

		internal const int ReduceCacheSize = 460800;

		internal const int CleanupFrequencyDays = 1;

		internal const int CleanupLastUpdateCheckThresholdDays = 14;

		private static ExtensionsCache singleton = new ExtensionsCache(OmexWebServiceUrlsCache.Singleton);

		private static readonly Trace Tracer = ExTraceGlobals.ExtensionTracer;

		private MruDictionary<string, ExtensionsCacheEntry> extensionsDictionary = new MruDictionary<string, ExtensionsCacheEntry>(500000, StringComparer.Ordinal, null);

		private OmexWebServiceUrlsCache urlsCache;

		private TokenRenewSubmitter tokenRenewSubmitter;

		private object queryQueueLockObject = new object();

		private Queue<UpdateQueryContext> queryQueue = new Queue<UpdateQueryContext>(500);
	}
}
