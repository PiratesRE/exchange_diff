using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal sealed class GetUpdates
	{
		internal GetUpdates(Dictionary<string, UpdateRequestAsset> updateRequestAssets, ExtensionsCache extensionsCache, UpdateQueryContext updateQueryContext)
		{
			this.updateRequestAssets = updateRequestAssets;
			this.extensionsCache = extensionsCache;
			this.queryContext = updateQueryContext;
		}

		internal GetUpdates(OmexWebServiceUrlsCache urlsCache, ExtensionsCache extensionsCache)
		{
			if (urlsCache == null)
			{
				throw new ArgumentNullException("urlsCache");
			}
			if (extensionsCache == null)
			{
				throw new ArgumentNullException("extensionsCache");
			}
			this.urlsCache = urlsCache;
			this.extensionsCache = extensionsCache;
		}

		internal void Execute(UpdateQueryContext queryContext)
		{
			if (queryContext == null)
			{
				throw new ArgumentNullException("queryContext");
			}
			if (queryContext.UpdateRequestAssets == null)
			{
				throw new ArgumentNullException("QueryContext.UpdateRequestAssets");
			}
			if (queryContext.UpdateRequestAssets.Count == 0)
			{
				throw new ArgumentException("QueryContext.UpdateRequestAssets must include 1 or more extensions");
			}
			this.queryContext = queryContext;
			if (queryContext.UpdateRequestAssets.Count > 100)
			{
				GetUpdates.Tracer.TraceError<int, int>(0L, "GetUpdates.Execute: Too many extensions passed. Passed: {0}  Supported: {1}", queryContext.UpdateRequestAssets.Count, 100);
				string[] array = queryContext.UpdateRequestAssets.Keys.ToArray<string>();
				for (int i = 100; i < array.Length; i++)
				{
					GetUpdates.Tracer.TraceDebug<string>(0L, "GetUpdates.Execute: Too many extensions. Removing {0}.", array[i]);
					queryContext.UpdateRequestAssets.Remove(array[i]);
				}
				ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_TooManyExtensionsForAutomaticUpdate, null, new object[]
				{
					"ProcessUpdates",
					this.GetLoggedMailboxIdentifier()
				});
			}
			GetUpdates.Tracer.TraceDebug<int>(0L, "GetUpdates.Execute: Getting update information for {0} extensions.", queryContext.UpdateRequestAssets.Count);
			this.updateRequestAssets = queryContext.UpdateRequestAssets;
			GetAppState getAppState = new GetAppState(this.urlsCache);
			using (ActivityContext.SuppressThreadScope())
			{
				getAppState.Execute(queryContext.UpdateRequestAssets.Values, queryContext.DeploymentId, new BaseAsyncCommand.GetLoggedMailboxIdentifierCallback(this.GetLoggedMailboxIdentifier), new GetAppState.SuccessCallback(this.GetAppStateSuccessCallback), new BaseAsyncCommand.FailureCallback(this.GetAppStateFailureCallback));
			}
		}

		internal void GetAppStateSuccessCallback(List<AppStateResponseAsset> appStateResponses, Uri appStateUri)
		{
			GetUpdates.Tracer.TraceDebug<int>(0L, "GetUpdates.GetAppStateSuccessCallback called with {0} responses", appStateResponses.Count);
			this.extensionUpdates = new List<ExtensionData>();
			foreach (AppStateResponseAsset appStateResponseAsset in appStateResponses)
			{
				string value = string.Empty;
				OmexConstants.AppState valueOrDefault = appStateResponseAsset.State.GetValueOrDefault();
				OmexConstants.AppState? appState;
				if (appState == null)
				{
					goto IL_8B;
				}
				switch (valueOrDefault)
				{
				case OmexConstants.AppState.Killed:
				case OmexConstants.AppState.OK:
					break;
				case OmexConstants.AppState.Withdrawn:
					value = "3.1";
					break;
				case OmexConstants.AppState.Flagged:
					value = "3.2";
					break;
				case OmexConstants.AppState.WithdrawingSoon:
					value = "3.3";
					break;
				default:
					goto IL_8B;
				}
				IL_91:
				if (!(appStateResponseAsset.State != OmexConstants.AppState.Killed))
				{
					continue;
				}
				string text = string.Empty;
				if (string.IsNullOrWhiteSpace(appStateResponseAsset.ExtensionID))
				{
					UpdateRequestAsset updateRequestAsset;
					if (this.updateRequestAssets.TryGetValue(appStateResponseAsset.MarketplaceAssetID, out updateRequestAsset))
					{
						text = updateRequestAsset.ExtensionID;
					}
				}
				else
				{
					text = appStateResponseAsset.ExtensionID;
				}
				if (!string.IsNullOrWhiteSpace(text))
				{
					this.appStatuses.Add(text, value);
					continue;
				}
				continue;
				IL_8B:
				value = "3.0";
				goto IL_91;
			}
			this.downloadQueue = this.BuildDownloadQueue(appStateResponses, appStateUri, this.extensionUpdates);
			if (this.downloadQueue.Count > 0)
			{
				this.downloadApp = new DownloadApp(OmexWebServiceUrlsCache.Singleton);
				this.ExecuteDownload(this.downloadApp);
				return;
			}
			if (this.extensionUpdates.Count > 0)
			{
				this.InstallExtensionUpdates(this.extensionUpdates, this.queryContext);
				return;
			}
			this.ExecuteNextUpdateQuery();
		}

		internal Queue<UpdateRequestAsset> BuildDownloadQueue(List<AppStateResponseAsset> appStateResponses, Uri appStateUri, List<ExtensionData> updates)
		{
			Queue<UpdateRequestAsset> queue = new Queue<UpdateRequestAsset>(appStateResponses.Count);
			foreach (AppStateResponseAsset appStateResponseAsset in appStateResponses)
			{
				UpdateRequestAsset updateRequestAsset = null;
				if (!this.updateRequestAssets.TryGetValue(appStateResponseAsset.MarketplaceAssetID, out updateRequestAsset))
				{
					GetUpdates.Tracer.TraceError<string, string>(0L, "GetUpdates.GetAppStateSuccessCallback: Asset returned in AppState response will not be downloaded. Asset ID does not match any value requested.  Response Asset ID: {0} Response Extension ID: {1}", appStateResponseAsset.MarketplaceAssetID, appStateResponseAsset.ExtensionID);
					ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_InvalidAssetIDReturnedByAppState, appStateResponseAsset.MarketplaceAssetID, new object[]
					{
						"ProcessUpdates",
						this.GetLoggedMailboxIdentifier(),
						appStateResponseAsset.MarketplaceAssetID,
						appStateUri
					});
				}
				else if (!this.CacheSatisfiesRequest(updateRequestAsset, updates))
				{
					if (appStateResponseAsset.ExtensionID != updateRequestAsset.ExtensionID)
					{
						GetUpdates.Tracer.TraceError<string, string, string>(0L, "GetUpdates.GetAppStateSuccessCallback: Asset returned in AppState response will not be downloaded. Extension ID does not match expected value.  Asset ID: {0} Response: {1} Expected: {2}", appStateResponseAsset.MarketplaceAssetID, appStateResponseAsset.ExtensionID, updateRequestAsset.ExtensionID);
						ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_MismatchedExtensionIDReturnedByAppState, appStateResponseAsset.MarketplaceAssetID, new object[]
						{
							"ProcessUpdates",
							this.GetLoggedMailboxIdentifier(),
							appStateResponseAsset.MarketplaceAssetID,
							appStateUri,
							updateRequestAsset.ExtensionID,
							appStateResponseAsset.ExtensionID
						});
					}
					else if (appStateResponseAsset.State != null && appStateResponseAsset.Version != null && (appStateResponseAsset.Version == updateRequestAsset.Version || !GetUpdates.IsValidUpdateState(appStateResponseAsset.State)))
					{
						this.extensionsCache.Update(appStateResponseAsset);
					}
					else if (GetUpdates.IsValidUpdateState(appStateResponseAsset.State) && appStateResponseAsset.Version != null && appStateResponseAsset.Version > updateRequestAsset.Version)
					{
						GetUpdates.Tracer.TraceDebug<string>(0L, "GetUpdates.GetAppStateSuccessCallback: Add asset {0} to download queue", appStateResponseAsset.MarketplaceAssetID);
						updateRequestAsset.State = appStateResponseAsset.State.Value;
						queue.Enqueue(updateRequestAsset);
					}
					else
					{
						GetUpdates.Tracer.TraceDebug<string, OmexConstants.AppState?, Version>(0L, "GetUpdates.GetAppStateSuccessCallback: Asset returned in AppState response will not be downloaded. Asset ID: {0} State: {1} Version: {2}", appStateResponseAsset.MarketplaceAssetID, appStateResponseAsset.State, appStateResponseAsset.Version);
					}
				}
			}
			return queue;
		}

		public static bool IsValidUpdateState(OmexConstants.AppState? state)
		{
			return state != null && (state.Value == OmexConstants.AppState.OK || state.Value == OmexConstants.AppState.Flagged || state.Value == OmexConstants.AppState.Undefined);
		}

		internal bool CacheSatisfiesRequest(UpdateRequestAsset requestAsset, List<ExtensionData> updates)
		{
			bool result = false;
			ExtensionsCacheEntry extensionsCacheEntry = null;
			if (this.extensionsCache.TryGetEntry(requestAsset.MarketplaceAssetID, out extensionsCacheEntry) && requestAsset.Version <= extensionsCacheEntry.Version && extensionsCacheEntry.Manifest != null)
			{
				if (requestAsset.ExtensionID != extensionsCacheEntry.ExtensionID)
				{
					GetUpdates.Tracer.TraceError<string, string, string>(0L, "GetUpdates.CacheSatisfiesRequest: Asset {0} extension ID {1} does not match the cache entry {2}", requestAsset.MarketplaceAssetID, requestAsset.ExtensionID, extensionsCacheEntry.ExtensionID);
					ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_MismatchedCacheMailboxExtensionId, requestAsset.MarketplaceAssetID, new object[]
					{
						"ProcessUpdates",
						this.GetLoggedMailboxIdentifier(),
						requestAsset.MarketplaceAssetID,
						requestAsset.ExtensionID,
						extensionsCacheEntry.ExtensionID
					});
					result = true;
				}
				else if (requestAsset.Version == extensionsCacheEntry.Version)
				{
					GetUpdates.Tracer.TraceDebug<string>(0L, "GetUpdates.CacheSatisfiesRequest: Asset {0} version matches the cache entry", requestAsset.MarketplaceAssetID);
					result = true;
				}
				else if (requestAsset.Version < extensionsCacheEntry.Version && extensionsCacheEntry.Manifest != null)
				{
					if (ExtensionData.CompareCapabilities(extensionsCacheEntry.RequestedCapabilities.Value, requestAsset.RequestedCapabilities) > 0)
					{
						GetUpdates.Tracer.TraceDebug<string, RequestedCapabilities, RequestedCapabilities>(0L, "GetUpdates.CacheSatisfiesRequest: Asset cache entry requires more capabilities than installed asset.  Asset ID: {0} Update: {1} Installed: {2}", requestAsset.MarketplaceAssetID, extensionsCacheEntry.RequestedCapabilities.Value, requestAsset.RequestedCapabilities);
						ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_MoreCapabilitiesSkipUpdate, requestAsset.MarketplaceAssetID, new object[]
						{
							"ProcessUpdates",
							this.GetLoggedMailboxIdentifier(),
							requestAsset.MarketplaceAssetID,
							requestAsset.RequestedCapabilities,
							extensionsCacheEntry.RequestedCapabilities.Value
						});
						result = true;
					}
					else if (!GetUpdates.IsValidUpdateState(new OmexConstants.AppState?(extensionsCacheEntry.State)))
					{
						GetUpdates.Tracer.TraceDebug<string, OmexConstants.AppState>(0L, "GetUpdates.CacheSatisfiesRequest: Asset {0} cache entry AppState {1} is not valid for updates", requestAsset.MarketplaceAssetID, extensionsCacheEntry.State);
						ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_InvalidStateSkipUpdate, requestAsset.MarketplaceAssetID, new object[]
						{
							"ProcessUpdates",
							this.GetLoggedMailboxIdentifier(),
							requestAsset.MarketplaceAssetID,
							extensionsCacheEntry.State
						});
						result = true;
					}
					else
					{
						GetUpdates.Tracer.TraceDebug<string>(0L, "GetUpdates.CacheSatisfiesRequest: Asset {0} will be updated from the cache", requestAsset.MarketplaceAssetID);
						ExtensionData updatedExtension = null;
						Exception ex = InstalledExtensionTable.RunClientExtensionAction(delegate
						{
							updatedExtension = ExtensionData.ParseOsfManifest(extensionsCacheEntry.Manifest, extensionsCacheEntry.Manifest.Length, requestAsset.MarketplaceAssetID, requestAsset.MarketplaceContentMarket, ExtensionType.MarketPlace, requestAsset.Scope, requestAsset.Enabled, requestAsset.DisableReason, string.Empty, requestAsset.Etoken);
						});
						if (ex == null)
						{
							updates.Add(updatedExtension);
							result = true;
						}
						else
						{
							GetUpdates.Tracer.TraceError<string, Exception>(0L, "GetUpdates.CacheSatisfiesRequest: Parse of manifest failed for extension {0}. Exception: {1}", requestAsset.MarketplaceAssetID, ex);
							ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_CachedManifestParseFailed, requestAsset.MarketplaceAssetID, new object[]
							{
								"ProcessUpdates",
								requestAsset.MarketplaceAssetID,
								ExtensionDiagnostics.GetLoggedExceptionString(ex)
							});
						}
					}
				}
			}
			return result;
		}

		internal void GetAppStateFailureCallback(Exception exception)
		{
			GetUpdates.Tracer.TraceDebug<Exception>(0L, "GetUpdates.GetAppStateFailureCallback called with exception: {0}", exception);
			this.ExecuteNextUpdateQuery();
		}

		internal void CreateEmptyDataStructuresForTest()
		{
			this.downloadQueue = new Queue<UpdateRequestAsset>();
			this.extensionUpdates = new List<ExtensionData>();
		}

		internal void ExecuteDownload(DownloadApp downloadApp)
		{
			if (this.downloadQueue.Count > 0)
			{
				UpdateRequestAsset updateRequestAsset = this.downloadQueue.Dequeue();
				if (this.CacheSatisfiesRequest(updateRequestAsset, this.extensionUpdates))
				{
					GetUpdates.Tracer.TraceDebug<string>(0L, "GetUpdates.ExecuteDownload: Asset {0} satisfied from cache; getting next asset to download", updateRequestAsset.MarketplaceAssetID);
					this.ExecuteDownload(downloadApp);
					return;
				}
				GetUpdates.Tracer.TraceDebug<string>(0L, "GetUpdates.ExecuteDownload: Downloading {0}", updateRequestAsset.MarketplaceAssetID);
				this.currentDownloadAppId = updateRequestAsset.ExtensionID;
				downloadApp.Execute(updateRequestAsset, this.queryContext.DeploymentId, new BaseAsyncCommand.GetLoggedMailboxIdentifierCallback(this.GetLoggedMailboxIdentifier), new DownloadApp.SuccessCallback(this.DownloadAppSuccessCallback), new BaseAsyncCommand.FailureCallback(this.DownloadAppFailureCallback));
				return;
			}
			else
			{
				GetUpdates.Tracer.TraceDebug(0L, "GetUpdates.ExecuteDownload: Downloads complete.");
				if (this.extensionUpdates.Count > 0)
				{
					this.InstallExtensionUpdates(this.extensionUpdates, this.queryContext);
					return;
				}
				this.ExecuteNextUpdateQuery();
				return;
			}
		}

		private void DownloadAppFailureCallback(Exception exception)
		{
			GetUpdates.Tracer.TraceError<string, Exception>(0L, "GetUpdates.DownloadAppFailureCallback for app {0} called with exception: {1}", this.currentDownloadAppId, exception);
			string value = "1.0";
			if (exception is WebException)
			{
				WebException ex = (WebException)exception;
				if (ex.Status == WebExceptionStatus.ProtocolError)
				{
					HttpWebResponse httpWebResponse = ex.Response as HttpWebResponse;
					if (httpWebResponse != null && httpWebResponse.StatusCode == HttpStatusCode.Forbidden)
					{
						value = "1.2";
					}
				}
			}
			this.appStatuses[this.currentDownloadAppId] = value;
			this.ExecuteDownload(this.downloadApp);
		}

		private void DownloadAppSuccessCallback(ExtensionData extensionData, Uri downloadAppUri)
		{
			GetUpdates.Tracer.TraceDebug<string>(0L, "GetUpdates.DownloadAppSuccessCallback called for asset {0}", extensionData.MarketplaceAssetID);
			UpdateRequestAsset updateRequestAsset = null;
			if (this.IsValidUpdate(extensionData, downloadAppUri, out updateRequestAsset))
			{
				this.extensionsCache.Add(extensionData, updateRequestAsset.State);
				this.extensionUpdates.Add(extensionData);
			}
			this.ExecuteDownload(this.downloadApp);
		}

		private string GetLoggedMailboxIdentifier()
		{
			return ExtensionDiagnostics.GetLoggedMailboxIdentifier(this.queryContext.ExchangePrincipal);
		}

		internal bool IsValidUpdate(ExtensionData extensionData, Uri downloadAppUri, out UpdateRequestAsset requestAsset)
		{
			bool flag = false;
			bool flag2 = false;
			UpdateRequestAsset updateRequestAsset = null;
			if (!this.updateRequestAssets.TryGetValue(extensionData.MarketplaceAssetID, out updateRequestAsset))
			{
				GetUpdates.Tracer.TraceError<string, string>(0L, "GetUpdates.IsValidUpdate: Asset returned in AppState response will not be downloaded. Asset ID does not match any value requested.  Response Asset ID: {0} Response Extension ID: {1}", extensionData.MarketplaceAssetID, extensionData.ExtensionId);
				ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_InvalidAssetIDReturnedInDownload, extensionData.MarketplaceAssetID, new object[]
				{
					"ProcessUpdates",
					this.GetLoggedMailboxIdentifier(),
					extensionData.MarketplaceAssetID,
					downloadAppUri
				});
			}
			else if (extensionData.ExtensionId != updateRequestAsset.ExtensionID)
			{
				GetUpdates.Tracer.TraceError<string, string, string>(0L, "GetUpdates.IsValidUpdate: Asset returned in download response will not be installed. Extension ID does not match expected value.  Asset ID: {0} Response: {1} Expected: {2}", extensionData.MarketplaceAssetID, extensionData.ExtensionId, updateRequestAsset.ExtensionID);
				ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_MismatchedExtensionIDReturnedInDownload, extensionData.MarketplaceAssetID, new object[]
				{
					"ProcessUpdates",
					this.GetLoggedMailboxIdentifier(),
					extensionData.MarketplaceAssetID,
					downloadAppUri,
					updateRequestAsset.ExtensionID,
					extensionData.ExtensionId
				});
			}
			else if (extensionData.Version == null || extensionData.Version <= updateRequestAsset.Version)
			{
				GetUpdates.Tracer.TraceError<string, string, Version>(0L, "GetUpdates.IsValidUpdate: Asset returned in download response will not be installed. Version is not newer.  Asset ID: {0} Response: {1} Request: {2}", extensionData.MarketplaceAssetID, extensionData.VersionAsString, updateRequestAsset.Version);
				ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_OldVersionReturnedInDownload, extensionData.MarketplaceAssetID, new object[]
				{
					"ProcessUpdates",
					this.GetLoggedMailboxIdentifier(),
					extensionData.MarketplaceAssetID,
					downloadAppUri,
					updateRequestAsset.Version,
					extensionData.VersionAsString
				});
			}
			else if (ExtensionData.CompareCapabilities(extensionData.RequestedCapabilities.Value, updateRequestAsset.RequestedCapabilities) > 0)
			{
				GetUpdates.Tracer.TraceError<string, RequestedCapabilities?, RequestedCapabilities>(0L, "GetUpdates.IsValidUpdate: Asset update requires more capabilities than installed asset.  Asset ID: {0} Update: {1} Installed: {2}", extensionData.MarketplaceAssetID, extensionData.RequestedCapabilities, updateRequestAsset.RequestedCapabilities);
				flag2 = true;
				ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_MoreCapabilitiesReturnedInDownload, extensionData.MarketplaceAssetID, new object[]
				{
					"ProcessUpdates",
					this.GetLoggedMailboxIdentifier(),
					extensionData.MarketplaceAssetID,
					downloadAppUri,
					updateRequestAsset.RequestedCapabilities,
					extensionData.RequestedCapabilities.Value
				});
			}
			else if (!InstalledExtensionTable.ValidateAndRemoveManifestSignature(extensionData.Manifest, extensionData.ExtensionId, false))
			{
				GetUpdates.Tracer.TraceError<string>(0L, "GetUpdates.IsValidUpdate: Asset doesn't have valid Signature. Asset ID: {0}.", extensionData.MarketplaceAssetID);
				ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_InvalidAssetSignatureReturnedInDownload, extensionData.MarketplaceAssetID, new object[]
				{
					"ProcessUpdates",
					this.GetLoggedMailboxIdentifier(),
					extensionData.MarketplaceAssetID,
					downloadAppUri
				});
			}
			else if (!ExtensionData.ValidateManifestSize((long)extensionData.Manifest.OuterXml.Length, false))
			{
				GetUpdates.Tracer.TraceError<string>(0L, "GetUpdates.IsValidUpdate: Asset exceeds the allowed maximum size. Asset ID: {0}.", extensionData.MarketplaceAssetID);
				ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_ManifestExceedsAllowedSize, extensionData.MarketplaceAssetID, new object[]
				{
					"ProcessUpdates",
					this.GetLoggedMailboxIdentifier(),
					extensionData.Manifest.OuterXml.Length,
					256
				});
			}
			else
			{
				flag = true;
			}
			if (!flag)
			{
				string value = "1.0";
				if (flag2)
				{
					value = "1.1";
				}
				this.appStatuses[extensionData.ExtensionId] = value;
			}
			requestAsset = (flag ? updateRequestAsset : null);
			return flag;
		}

		internal void InstallExtensionUpdates(List<ExtensionData> updates, UpdateQueryContext queryContext)
		{
			GetUpdates.Tracer.TraceDebug<int>(0L, "GetUpdates.InstallExtensionUpdates: Installing {0} extensions.", updates.Count);
			ExtensionData currentExtensionData = null;
			Exception ex = InstalledExtensionTable.RunClientExtensionAction(delegate
			{
				using (MailboxSession mailboxSession = MailboxSession.OpenAsSystemService(queryContext.ExchangePrincipal, queryContext.CultureInfo, queryContext.ClientInfoString))
				{
					using (InstalledExtensionTable installedExtensionTable = InstalledExtensionTable.CreateInstalledExtensionTable(queryContext.Domain, queryContext.IsUserScope, queryContext.OrgEmptyMasterTableCache, mailboxSession))
					{
						foreach (ExtensionData currentExtensionData in updates)
						{
							currentExtensionData = currentExtensionData;
							GetUpdates.Tracer.TraceDebug<string>(0L, "GetUpdates.InstallExtensionUpdates: Installing asset {0}", currentExtensionData.MarketplaceAssetID);
							installedExtensionTable.AddExtension(currentExtensionData, true);
							currentExtensionData = null;
						}
						installedExtensionTable.SaveXML();
					}
				}
			});
			string text = (currentExtensionData == null) ? string.Empty : currentExtensionData.MarketplaceAssetID;
			if (ex != null)
			{
				GetUpdates.Tracer.TraceError<string, Exception>(0L, "GetUpdates.InstallExtensionUpdates: Installation failed for extension {0}. Exception: {1}", text, ex);
				ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_ExtensionUpdateFailed, null, new object[]
				{
					"UpdateExtension",
					ExtensionDiagnostics.GetLoggedMailboxIdentifier(queryContext.ExchangePrincipal),
					text,
					ExtensionDiagnostics.GetLoggedExceptionString(ex)
				});
			}
			else
			{
				ExtensionDiagnostics.LogToDatacenterOnly(ApplicationLogicEventLogConstants.Tuple_ExtensionUpdateSuccess, null, new object[]
				{
					"UpdateExtension",
					ExtensionDiagnostics.GetLoggedMailboxIdentifier(queryContext.ExchangePrincipal),
					text
				});
			}
			this.ExecuteNextUpdateQuery();
		}

		private void ExecuteNextUpdateQuery()
		{
			this.WriteAppStatuesToMailbox();
			this.appStatuses.Clear();
			this.queryContext = null;
			this.downloadQueue = null;
			this.updateRequestAssets = null;
			this.downloadApp = null;
			this.extensionUpdates = null;
			this.extensionsCache.ExecuteUpdateQuery(this);
		}

		private void WriteAppStatuesToMailbox()
		{
			if (this.appStatuses.Count == 0)
			{
				return;
			}
			GetUpdates.Tracer.TraceDebug<int>(0L, "GetUpdates.ConfigAppStatus: Config app status for {0} extensions.", this.appStatuses.Count);
			Exception ex = InstalledExtensionTable.RunClientExtensionAction(delegate
			{
				using (MailboxSession mailboxSession = MailboxSession.OpenAsSystemService(this.queryContext.ExchangePrincipal, this.queryContext.CultureInfo, this.queryContext.ClientInfoString))
				{
					using (InstalledExtensionTable installedExtensionTable = InstalledExtensionTable.CreateInstalledExtensionTable(this.queryContext.Domain, this.queryContext.IsUserScope, this.queryContext.OrgEmptyMasterTableCache, mailboxSession))
					{
						foreach (KeyValuePair<string, string> keyValuePair in this.appStatuses)
						{
							installedExtensionTable.ConfigureAppStatus(keyValuePair.Key, keyValuePair.Value);
						}
						installedExtensionTable.SaveXML();
					}
				}
			});
			if (ex != null)
			{
				GetUpdates.Tracer.TraceError<Exception>(0L, "GetUpdates.ConfigAppStatus: Config app status failed. Exception: {0}", ex);
				ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_FailedToConfigAppStatus, null, new object[]
				{
					"ProcessUpdates",
					ExtensionDiagnostics.GetLoggedMailboxIdentifier(this.queryContext.ExchangePrincipal),
					ExtensionDiagnostics.GetLoggedExceptionString(ex)
				});
			}
		}

		private const string ScenarioProcessUpdates = "ProcessUpdates";

		private const string ScenarioNameUpdateExtension = "UpdateExtension";

		private const string ScenarioGetUpdates = "GetUpdates";

		private static readonly Trace Tracer = ExTraceGlobals.ExtensionTracer;

		private OmexWebServiceUrlsCache urlsCache;

		private ExtensionsCache extensionsCache;

		private UpdateQueryContext queryContext;

		private Queue<UpdateRequestAsset> downloadQueue;

		private Dictionary<string, UpdateRequestAsset> updateRequestAssets;

		private Dictionary<string, string> appStatuses = new Dictionary<string, string>();

		private DownloadApp downloadApp;

		private List<ExtensionData> extensionUpdates;

		private string currentDownloadAppId;

		internal delegate void ExecuteDownloadCallback(DownloadApp downloadApp);
	}
}
