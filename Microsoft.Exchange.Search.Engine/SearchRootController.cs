using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Exchange.Search.Core;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.Core.RpcEndpoint;
using Microsoft.Exchange.Search.EventLog;
using Microsoft.Exchange.Search.Fast;
using Microsoft.Exchange.Search.Mdb;
using Microsoft.Exchange.Search.OperatorSchema;
using Microsoft.Exchange.Search.Performance;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Mapi;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Win32;

namespace Microsoft.Exchange.Search.Engine
{
	internal sealed class SearchRootController : Executable
	{
		public SearchRootController(ISearchServiceConfig config, SearchServiceRpcServer rpcServiceEndpoint) : base(config)
		{
			Util.ThrowOnNullArgument(config, "config");
			base.DiagnosticsSession.ComponentName = "SearchRootController";
			base.DiagnosticsSession.Tracer = ExTraceGlobals.EngineTracer;
			this.eventTimer = new GuardedTimer(new TimerCallback(this.TimerCallback));
			this.breadCrumbs = new Breadcrumbs<string>(256);
			foreach (string text in MdbPerfCounters.GetInstanceNames())
			{
				if (!StringComparer.OrdinalIgnoreCase.Equals(MdbPerfCounters.TotalInstance.Name, text))
				{
					MdbPerfCounters.ResetInstance(text);
					MdbPerfCounters.RemoveInstance(text);
				}
			}
			if (rpcServiceEndpoint != null)
			{
				rpcServiceEndpoint.RecordDocumentProcessingHandler = new SearchServiceRpcServer.HandleRecordDocumentProcessing(this.RecordDocumentProcessing);
				rpcServiceEndpoint.RecordDocumentFailureHandler = new SearchServiceRpcServer.HandleRecordDocumentFailure(this.RecordDocumentFailure);
				rpcServiceEndpoint.UpdateIndexSystemsHandler = new Action(this.UpdateIndexSystems);
				rpcServiceEndpoint.ResumeIndexingHandler = new Action<Guid>(this.ResumeIndexing);
				rpcServiceEndpoint.RebuildIndexSystemHandler = new Action<Guid>(this.RebuildIndexSystem);
			}
		}

		internal int FeedingControllerCount
		{
			get
			{
				return this.feedingInstancePerMdb.Count;
			}
		}

		internal IIndexStatusStore IndexStatusStore
		{
			get
			{
				return this.indexStatusStore;
			}
		}

		public override XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			return this.GetDiagnosticInfo(parameters, null);
		}

		public XElement GetBreadCrumbs()
		{
			return new XElement("SearchRootControllerBreadCrumbs", this.breadCrumbs.ToString());
		}

		public XElement GetDiagnosticInfo(DiagnosableParameters parameters, string remainingArgs)
		{
			XElement diagnosticInfo = base.GetDiagnosticInfo(parameters);
			if (base.Config.GracefulDegradationEnabled)
			{
				diagnosticInfo.Add(new XElement("RecentGracefulDegradationExecutionTime", GracefulDegradationManager.RecentGracefulDegradationExecutionTime));
			}
			lock (this.dictionaryLock)
			{
				Guid key;
				SearchRootController.FeedingControllerContext feedingControllerContext;
				if (Guid.TryParse(remainingArgs, out key) && this.feedingInstancePerMdb.TryGetValue(key, out feedingControllerContext) && feedingControllerContext.FeedingController != null)
				{
					diagnosticInfo.Add(feedingControllerContext.FeedingController.GetDiagnosticInfo(parameters));
					return diagnosticInfo;
				}
				remainingArgs = (string.IsNullOrEmpty(remainingArgs) ? null : remainingArgs);
				foreach (SearchRootController.FeedingControllerContext feedingControllerContext2 in this.feedingInstancePerMdb.Values)
				{
					if ((remainingArgs == null || string.Equals(feedingControllerContext2.DatabaseInfo.Name, remainingArgs, StringComparison.OrdinalIgnoreCase)) && feedingControllerContext2.FeedingController != null)
					{
						diagnosticInfo.Add(feedingControllerContext2.FeedingController.GetDiagnosticInfo(parameters));
					}
				}
			}
			return diagnosticInfo;
		}

		public MdbInfo FindMdbInfo(string database)
		{
			if (string.IsNullOrEmpty(database))
			{
				return null;
			}
			lock (this.dictionaryLock)
			{
				Guid key;
				SearchRootController.FeedingControllerContext feedingControllerContext;
				if (Guid.TryParse(database, out key) && this.feedingInstancePerMdb.TryGetValue(key, out feedingControllerContext) && feedingControllerContext.FeedingController != null)
				{
					return feedingControllerContext.DatabaseInfo;
				}
				foreach (SearchRootController.FeedingControllerContext feedingControllerContext2 in this.feedingInstancePerMdb.Values)
				{
					if (string.Equals(feedingControllerContext2.DatabaseInfo.Name, database, StringComparison.OrdinalIgnoreCase) && feedingControllerContext2.FeedingController != null)
					{
						return feedingControllerContext2.DatabaseInfo;
					}
				}
			}
			return null;
		}

		public sealed override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SearchRootController>(this);
		}

		internal static bool ShouldWaitForConfigureMountPointsPostReInstall()
		{
			if (!ExEnvironment.IsTest && VariantConfiguration.InvariantNoFlightingSnapshot.Search.WaitForMountPoints.Enabled)
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\autoreseed", false))
				{
					if (registryKey != null)
					{
						object value = registryKey.GetValue("ConfigureMountPointsPostReInstall");
						if (value != null && Convert.ToInt32(value) == 0)
						{
							return false;
						}
					}
					return true;
				}
				return false;
			}
			return false;
		}

		internal XElement RecrawlMailbox(DiagnosableParameters parameters, string remainingArgs)
		{
			string[] array = remainingArgs.Split(null, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length != 2)
			{
				throw new ArgumentException("Expected mdbGuid { mailboxGuid | *}");
			}
			Guid mdbGuid = new Guid(array[0]);
			Guid mailboxGuid = (array[1] == "*") ? Guid.Empty : new Guid(array[1]);
			this.RecrawlMailbox(mdbGuid, mailboxGuid);
			return new XElement("RecrawlMailbox");
		}

		internal void CalculateCatalogs(IEnumerable<MdbInfo> allMDBs, List<MdbInfo> effective, HashSet<string> catalogs, out List<MdbInfo> indexSystemToUpdate, out List<string> catalogsToSuspend, out List<string> catalogsToRemove)
		{
			this.DropBreadCrumb("CalculateCatalogs");
			indexSystemToUpdate = new List<MdbInfo>();
			catalogsToSuspend = new List<string>();
			catalogsToRemove = new List<string>();
			Dictionary<Guid, SearchRootController.FeedingControllerContext> dictionary = new Dictionary<Guid, SearchRootController.FeedingControllerContext>();
			lock (this.dictionaryLock)
			{
				foreach (KeyValuePair<Guid, SearchRootController.FeedingControllerContext> keyValuePair in this.feedingInstancePerMdb)
				{
					MdbInfo databaseInfo = keyValuePair.Value.DatabaseInfo;
					if (!effective.Contains(databaseInfo))
					{
						dictionary.Add(keyValuePair.Key, keyValuePair.Value);
						base.DiagnosticsSession.TraceDebug<MdbInfo>("Orphaned SearchFeedingControllers is updated: {0}", keyValuePair.Value.DatabaseInfo);
					}
				}
			}
			this.CleanupOrphanedFeedingControllers(dictionary);
			List<string> list = null;
			lock (this.catalogsToRebuild)
			{
				if (this.catalogsToRebuild.Count > 0)
				{
					list = new List<string>(this.catalogsToRebuild);
				}
			}
			foreach (string text in catalogs)
			{
				bool flag3 = true;
				bool flag4 = false;
				foreach (MdbInfo mdbInfo in allMDBs)
				{
					if (FastIndexVersion.MatchIndexSystemName(text, mdbInfo.Guid, ref flag4))
					{
						flag3 = false;
						if (list != null && list.Contains(text))
						{
							indexSystemToUpdate.Add(mdbInfo);
							catalogsToRemove.Add(text);
							base.DiagnosticsSession.TraceDebug<MdbInfo>("List of the index systems to rebuild is updated: {0}", mdbInfo);
							this.DropBreadCrumb("Remove/Rebuild catalog: " + mdbInfo.Guid.ToString());
							break;
						}
						string rootDirectory = this.indexManager.GetRootDirectory(text);
						string desiredCatalogFolder = mdbInfo.DesiredCatalogFolder;
						if (!flag4 && !StringComparer.OrdinalIgnoreCase.Equals(rootDirectory, desiredCatalogFolder))
						{
							flag4 = true;
							indexSystemToUpdate.Add(mdbInfo);
							this.DropBreadCrumb("Update index system for database: " + mdbInfo.Guid.ToString());
							base.DiagnosticsSession.TraceDebug<MdbInfo>("List of the index systems to update is updated: {0}", mdbInfo);
							break;
						}
						break;
					}
				}
				if (flag3)
				{
					Guid indexSystemDatabaseGuid = FastIndexVersion.GetIndexSystemDatabaseGuid(text);
					if (indexSystemDatabaseGuid == Guid.Empty)
					{
						base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "GetIndexSystemDatabaseGuid found index system with invalid name format: {0}", new object[]
						{
							text
						});
					}
					if (base.Config.RemoveOrphanedCatalogs && (indexSystemDatabaseGuid == Guid.Empty || !this.mdbWatcher.Exists(indexSystemDatabaseGuid)))
					{
						catalogsToRemove.Add(text);
					}
					else
					{
						catalogsToSuspend.Add(text);
					}
				}
				else if (flag4)
				{
					catalogsToSuspend.Add(text);
					base.DiagnosticsSession.TraceDebug<string>("List of the index systems to suspend is updated: {0}", text);
				}
			}
			catalogs.ExceptWith(catalogsToSuspend);
			catalogs.ExceptWith(catalogsToRemove);
			if (list != null)
			{
				lock (this.catalogsToRebuild)
				{
					this.catalogsToRebuild.ExceptWith(list);
				}
			}
			List<MdbInfo> list2 = new List<MdbInfo>();
			lock (this.dictionaryLock)
			{
				foreach (MdbInfo mdbInfo2 in allMDBs)
				{
					SearchRootController.FeedingControllerContext feedingControllerContext;
					if (this.feedingInstancePerMdb.TryGetValue(mdbInfo2.Guid, out feedingControllerContext))
					{
						mdbInfo2.IsCatalogSuspended = feedingControllerContext.DatabaseInfo.IsCatalogSuspended;
						mdbInfo2.IsInstantSearchEnabled = feedingControllerContext.DatabaseInfo.IsInstantSearchEnabled;
						mdbInfo2.IsRefinersEnabled = feedingControllerContext.DatabaseInfo.IsRefinersEnabled;
					}
					else
					{
						mdbInfo2.IsInstantSearchEnabled = !base.Config.DisableInstantSearch;
						mdbInfo2.IsRefinersEnabled = !base.Config.DisableRefiners;
						if (catalogs.Contains(mdbInfo2.IndexSystemName))
						{
							list2.Add(mdbInfo2);
						}
						else
						{
							mdbInfo2.IsCatalogSuspended = base.Config.AutoSuspendCatalog;
						}
					}
				}
			}
			foreach (MdbInfo mdbInfo3 in list2)
			{
				mdbInfo3.IsCatalogSuspended = IndexManager.Instance.IsCatalogSuspended(mdbInfo3.Guid);
			}
			this.UpdateNumberOfItems(effective, catalogs);
		}

		internal void RebuildIndexSystem(Guid databaseGuid)
		{
			this.DropBreadCrumb("RebuildIndexSystem: " + databaseGuid.ToString());
			lock (this.catalogsToRebuild)
			{
				this.catalogsToRebuild.Add(FastIndexVersion.GetIndexSystemName(databaseGuid));
			}
			this.ResetTimer();
		}

		internal void CalculateCatalogsConfiguration(IEnumerable<MdbInfo> allMDBs)
		{
			GracefulDegradationManager.DetermineFeatureStatusAndUpdate(allMDBs, base.Config, base.DiagnosticsSession);
		}

		internal void UpdateCatalogs(IEnumerable<MdbInfo> allMDBs, List<MdbInfo> effective, HashSet<string> catalogs, List<MdbInfo> indexSystemToUpdate, List<string> catalogsToSuspend, List<string> catalogsToRemove)
		{
			this.DropBreadCrumb("UpdateCatalogs");
			bool flag = false;
			try
			{
				foreach (string text in catalogsToSuspend)
				{
					this.flowManager.RemoveFlowsForIndexSystem(text);
					if (this.indexManager.SuspendCatalog(text))
					{
						flag = true;
					}
				}
				foreach (string text2 in catalogsToRemove)
				{
					this.flowManager.RemoveFlowsForIndexSystem(text2);
					this.indexManager.RemoveCatalog(text2);
					flag = true;
				}
			}
			finally
			{
				if (flag)
				{
					this.indexManager.UpdateConfiguration();
				}
			}
			HashSet<Guid> hashSet = new HashSet<Guid>();
			flag = false;
			try
			{
				SearchConfig.RefreshSearchConfigCache(true);
				foreach (MdbInfo mdbInfo in allMDBs)
				{
					bool flag2 = false;
					string desiredCatalogFolder = mdbInfo.DesiredCatalogFolder;
					string indexSystemName = mdbInfo.IndexSystemName;
					if (!string.IsNullOrEmpty(desiredCatalogFolder) && !Directory.Exists(desiredCatalogFolder))
					{
						base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "AutoSuspend: {0} since catalogfolderpath '{1}' does not exist on the server.", new object[]
						{
							indexSystemName,
							desiredCatalogFolder
						});
						flag2 = true;
					}
					RefinerUsage refinerUsage = 0;
					SearchConfig searchConfig = (SearchConfig)Factory.Current.CreateSearchServiceConfig(mdbInfo.Guid);
					if (!searchConfig.DisableRefiners)
					{
						refinerUsage |= 2;
					}
					mdbInfo.IsInstantSearchEnabled = !searchConfig.DisableInstantSearch;
					if (!searchConfig.DisableInstantSearch)
					{
						refinerUsage |= 1;
					}
					if (!catalogs.Contains(indexSystemName))
					{
						if (flag2)
						{
							hashSet.Add(mdbInfo.Guid);
						}
						else
						{
							this.indexManager.CreateCatalog(indexSystemName, desiredCatalogFolder, mdbInfo.MountedOnLocalServer, refinerUsage);
							flag = true;
						}
					}
					else
					{
						if (searchConfig.AutoSuspendCatalog && effective.Contains(mdbInfo) && !mdbInfo.MountedOnLocalServer && !mdbInfo.IsSuspended && mdbInfo.NumberOfCopies >= 3)
						{
							mdbInfo.ShouldAutomaticallySuspendCatalog = true;
						}
						bool flag3 = mdbInfo.NotIndexed != IndexStatusErrorCode.Unknown;
						bool flag4 = mdbInfo.IsSuspended || mdbInfo.ShouldAutomaticallySuspendCatalog || flag3 || flag2;
						if (this.indexManager.EnsureCatalog(indexSystemName, mdbInfo.MountedOnLocalServer && !flag3, flag4, refinerUsage))
						{
							if (!flag3)
							{
								indexSystemToUpdate.Add(mdbInfo);
							}
							flag = true;
							mdbInfo.IsCatalogSuspended = ((!mdbInfo.MountedOnLocalServer || flag3) && flag4);
						}
					}
				}
			}
			finally
			{
				if (flag)
				{
					this.indexManager.UpdateConfiguration();
				}
			}
			this.flowManager.EnsureIndexingFlow();
			foreach (MdbInfo mdbInfo2 in allMDBs)
			{
				if (!hashSet.Contains(mdbInfo2.Guid))
				{
					this.flowManager.EnsureQueryFlows(FastIndexVersion.GetIndexSystemName(mdbInfo2.Guid));
				}
			}
			lock (this.dictionaryLock)
			{
				if (!base.Stopping)
				{
					foreach (MdbInfo mdbInfo3 in effective)
					{
						SearchRootController.FeedingControllerContext feedingControllerContext;
						if (this.feedingInstancePerMdb.TryGetValue(mdbInfo3.Guid, out feedingControllerContext))
						{
							if (this.feedingInstanceNeedToStart.Contains(mdbInfo3.Guid))
							{
								this.StartFeedingController(feedingControllerContext);
							}
							else
							{
								feedingControllerContext.DatabaseInfo.IsCatalogSuspended = mdbInfo3.IsCatalogSuspended;
								feedingControllerContext.DatabaseInfo.IsInstantSearchEnabled = mdbInfo3.IsInstantSearchEnabled;
								feedingControllerContext.DatabaseInfo.ShouldAutomaticallySuspendCatalog = mdbInfo3.ShouldAutomaticallySuspendCatalog;
								if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Search.EnableDynamicActivationPreference.Enabled)
								{
									base.DiagnosticsSession.TraceDebug("VariantConfiguration EnableDynamicActivationPreference is True", new object[0]);
									feedingControllerContext.DatabaseInfo.ActivationPreference = mdbInfo3.ActivationPreference;
									feedingControllerContext.DatabaseInfo.PreferredActiveCopy = mdbInfo3.PreferredActiveCopy;
									feedingControllerContext.DatabaseInfo.DatabaseCopies = mdbInfo3.DatabaseCopies;
								}
								if (indexSystemToUpdate.Contains(mdbInfo3))
								{
									this.feedingInstancePerMdb[mdbInfo3.Guid].RestartNowEvent.Set();
								}
							}
							try
							{
								((SearchFeedingController)feedingControllerContext.FeedingController).PublishCatalogState();
								continue;
							}
							catch (InvalidCastException)
							{
								continue;
							}
						}
						feedingControllerContext = new SearchRootController.FeedingControllerContext(mdbInfo3);
						this.feedingInstancePerMdb[mdbInfo3.Guid] = feedingControllerContext;
						this.CreateFeedingController(feedingControllerContext);
						this.StartFeedingController(feedingControllerContext);
					}
				}
			}
		}

		internal void UpdateNumberOfItems(List<MdbInfo> effective, HashSet<string> catalogs)
		{
			this.DropBreadCrumb("UpdateNumberOfItems");
			int num = 0;
			long num2 = 0L;
			List<MdbInfo> list = new List<MdbInfo>();
			List<MdbInfo> list2 = new List<MdbInfo>();
			List<MdbInfo> list3 = new List<MdbInfo>();
			Dictionary<Guid, SearchRootController.FeedingControllerContext> dictionary = new Dictionary<Guid, SearchRootController.FeedingControllerContext>();
			lock (this.dictionaryLock)
			{
				this.feedingInstanceNeedToStart.Clear();
				foreach (MdbInfo mdbInfo in effective)
				{
					SearchRootController.FeedingControllerContext feedingControllerContext;
					if (this.feedingInstancePerMdb.TryGetValue(mdbInfo.Guid, out feedingControllerContext))
					{
						if (feedingControllerContext.DatabaseInfo.NumberOfItemsTimeStamp >= DateTime.UtcNow - base.Config.NumberOfItemsUpdateInterval)
						{
							mdbInfo.NumberOfItems = feedingControllerContext.DatabaseInfo.NumberOfItems;
							if (mdbInfo.MountedOnLocalServer)
							{
								num++;
								num2 += mdbInfo.NumberOfItems;
							}
						}
						else if (catalogs.Contains(mdbInfo.IndexSystemName))
						{
							if (!mdbInfo.IsCatalogSuspended)
							{
								list.Add(mdbInfo);
								if (!dictionary.ContainsKey(mdbInfo.Guid))
								{
									dictionary.Add(mdbInfo.Guid, feedingControllerContext);
								}
							}
							else
							{
								mdbInfo.NumberOfItems = feedingControllerContext.DatabaseInfo.NumberOfItems;
							}
						}
					}
					else if (catalogs.Contains(mdbInfo.IndexSystemName))
					{
						if (!mdbInfo.IsCatalogSuspended)
						{
							list2.Add(mdbInfo);
							list.Add(mdbInfo);
						}
						else
						{
							list3.Add(mdbInfo);
						}
					}
					else
					{
						mdbInfo.NumberOfItemsTimeStamp = DateTime.MinValue;
					}
				}
			}
			lock (this.dictionaryLock)
			{
				foreach (MdbInfo mdbInfo2 in list2)
				{
					SearchRootController.FeedingControllerContext feedingControllerContext2 = new SearchRootController.FeedingControllerContext(mdbInfo2);
					this.feedingInstancePerMdb[mdbInfo2.Guid] = feedingControllerContext2;
					this.CreateFeedingController(feedingControllerContext2);
					this.feedingInstanceNeedToStart.Add(mdbInfo2.Guid);
					if (!dictionary.ContainsKey(mdbInfo2.Guid))
					{
						dictionary.Add(mdbInfo2.Guid, feedingControllerContext2);
					}
				}
			}
			foreach (MdbInfo mdbInfo3 in list)
			{
				SearchRootController.FeedingControllerContext feedingControllerContext3;
				if (dictionary.TryGetValue(mdbInfo3.Guid, out feedingControllerContext3))
				{
					try
					{
						mdbInfo3.NumberOfItems = ((SearchFeedingController)feedingControllerContext3.FeedingController).QueryItemsCount();
						mdbInfo3.NumberOfItemsTimeStamp = DateTime.UtcNow;
						feedingControllerContext3.DatabaseInfo.NumberOfItems = mdbInfo3.NumberOfItems;
						feedingControllerContext3.DatabaseInfo.NumberOfItemsTimeStamp = mdbInfo3.NumberOfItemsTimeStamp;
					}
					catch (OperationFailedException)
					{
						mdbInfo3.NumberOfItems = feedingControllerContext3.DatabaseInfo.NumberOfItems;
					}
					catch (InvalidCastException)
					{
					}
					base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "{0}: Number of items is updated. The value is {1}.", new object[]
					{
						mdbInfo3,
						mdbInfo3.NumberOfItems
					});
					if (mdbInfo3.MountedOnLocalServer)
					{
						num++;
						num2 += mdbInfo3.NumberOfItems;
					}
				}
			}
			if (num != 0)
			{
				long numberOfItems = num2 / (long)num;
				foreach (MdbInfo mdbInfo4 in list3)
				{
					mdbInfo4.NumberOfItems = numberOfItems;
					mdbInfo4.NumberOfItemsTimeStamp = DateTime.UtcNow;
				}
			}
		}

		protected override void InternalExecutionStart()
		{
			this.DropBreadCrumb("InternalExecutionStart");
			this.indexManager = Factory.Current.CreateIndexManager();
			this.indexStatusStore = Factory.Current.CreateIndexStatusStore();
			this.flowManager = Factory.Current.CreateFlowManager();
			this.mdbWatcher = Factory.Current.CreateMdbWatcher();
			this.mdbWatcher.Changed += this.MdbChangedCallback;
			this.ResetTimer();
		}

		protected override void InternalExecutionFinish()
		{
			this.DropBreadCrumb("InternalExecutionFinish");
			if (this.mdbWatcher != null)
			{
				this.mdbWatcher.Changed -= this.MdbChangedCallback;
				this.mdbWatcher.Dispose();
				this.mdbWatcher = null;
			}
			if (this.eventTimer != null)
			{
				this.eventTimer.Pause();
				this.eventTimer.Dispose(false);
			}
			lock (this.dictionaryLock)
			{
				if (this.feedingInstancePerMdb.Count > 0)
				{
					this.CancelFeedingControllers(this.feedingInstancePerMdb);
				}
				else
				{
					this.doneDisposingChildrenEvent.Set();
				}
			}
		}

		protected override void Dispose(bool calledFromDispose)
		{
			this.DropBreadCrumb("Dispose");
			if (calledFromDispose)
			{
				if (base.AsyncResult != null)
				{
					base.AsyncResult.AsyncWaitHandle.WaitOne(base.Config.MaxOperationTimeout);
				}
				this.doneDisposingChildrenEvent.WaitOne();
				this.doneDisposingChildrenEvent.Dispose();
			}
			base.Dispose(calledFromDispose);
		}

		private void CancelFeedingControllers(Dictionary<Guid, SearchRootController.FeedingControllerContext> feedingControllerContexts)
		{
			Dictionary<Guid, SearchRootController.FeedingControllerContext> dictionary = new Dictionary<Guid, SearchRootController.FeedingControllerContext>(feedingControllerContexts);
			foreach (Guid key in dictionary.Keys)
			{
				MdbInfo databaseInfo = dictionary[key].DatabaseInfo;
				this.feedingInstancePerMdb[key].IsCanceled = true;
				this.feedingInstancePerMdb[key].RestartNowEvent.Set();
				MdbPerfCounters.ResetInstance(databaseInfo.Name);
				MdbPerfCounters.RemoveInstance(databaseInfo.Name);
				IExecutable feedingController = dictionary[key].FeedingController;
				if (feedingController.AsyncResult != null)
				{
					base.DiagnosticsSession.TraceDebug<MdbInfo>("Cancelling Feeding Controller for {0}", databaseInfo);
					feedingController.CancelExecute();
				}
			}
		}

		private void MdbChangedCallback(object sender, EventArgs e)
		{
			this.DropBreadCrumb("MdbChangedCallback");
			this.ResetTimer();
			this.newEvents = true;
		}

		private void TimerCallback(object state)
		{
			try
			{
				this.DropBreadCrumb("TimerCallback");
				this.isTimerCallbackScheduled = false;
				ExEnvironment.Reset();
				if (SearchRootController.ShouldWaitForConfigureMountPointsPostReInstall())
				{
					base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Failures, "The registry key/value indicating that the Database information is accurate was either not found or indicates that Setup is still running. Delaying startup for approximately {0} minutes.", new object[]
					{
						base.Config.SyncMdbsInterval
					});
					base.DiagnosticsSession.LogEvent(MSExchangeFastSearchEventLogConstants.Tuple_ServiceStartupDelayed, new object[0]);
				}
				else
				{
					IMdbCollection databases = this.mdbWatcher.GetDatabases();
					this.newEvents = false;
					MdbInfo[] array;
					MdbInfo[] effectiveDatabases = this.GetEffectiveDatabases(databases, out array);
					try
					{
						databases.UpdateDatabasesCopyStatusInfo();
						this.SyncMDBs(databases.Databases, effectiveDatabases);
					}
					finally
					{
						foreach (MdbInfo mdbInfo in array)
						{
							this.indexStatusStore.SetIndexStatus(mdbInfo.Guid, ContentIndexStatusType.Disabled, mdbInfo.NotIndexed, mdbInfo.CatalogVersion, null);
						}
					}
					if (this.newEvents)
					{
						this.DropBreadCrumb("New Events Available");
						this.ResetTimer();
					}
					this.DropBreadCrumb("TimerCallback Completed");
				}
			}
			catch (Exception ex)
			{
				base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Failures, "SyncMDBs() failed with exception {0}. Retrying in {1}s", new object[]
				{
					ex,
					base.Config.DelayForDatabaseChanged.TotalSeconds
				});
				this.DropBreadCrumb("TimerCallback Failed");
				this.ResetTimer();
			}
		}

		private void ResetTimer()
		{
			this.DropBreadCrumb("ResetTimer");
			lock (this.eventTimer)
			{
				if (!this.isTimerCallbackScheduled)
				{
					try
					{
						this.eventTimer.Change(base.Config.DelayForDatabaseChanged, base.Config.SyncMdbsInterval);
						this.isTimerCallbackScheduled = true;
					}
					catch (ObjectDisposedException arg)
					{
						base.DiagnosticsSession.TraceError<ObjectDisposedException>("ResetTimer failed: {0}", arg);
					}
				}
			}
		}

		private void SyncMDBs(IEnumerable<MdbInfo> allMDBs, IEnumerable<MdbInfo> effectiveMDBs)
		{
			if (base.Stopping)
			{
				return;
			}
			this.DropBreadCrumb("SyncMDBs");
			List<MdbInfo> effective = new List<MdbInfo>(effectiveMDBs);
			HashSet<string> catalogs = this.indexManager.GetCatalogs();
			List<MdbInfo> indexSystemToUpdate;
			List<string> catalogsToSuspend;
			List<string> catalogsToRemove;
			this.CalculateCatalogs(allMDBs, effective, catalogs, out indexSystemToUpdate, out catalogsToSuspend, out catalogsToRemove);
			this.CalculateCatalogsConfiguration(allMDBs);
			this.UpdateCatalogs(allMDBs, effective, catalogs, indexSystemToUpdate, catalogsToSuspend, catalogsToRemove);
		}

		private MdbInfo[] GetEffectiveDatabases(IMdbCollection mdbCollection, out MdbInfo[] indexDisabledDatabases)
		{
			this.DropBreadCrumb("GetEffectiveDatabases");
			IEnumerable<MdbInfo> databases = mdbCollection.Databases;
			List<MdbInfo> list = new List<MdbInfo>();
			List<MdbInfo> list2 = new List<MdbInfo>();
			if (base.Config.DatabaseOverride.Count > 0)
			{
				using (IEnumerator<MdbInfo> enumerator = databases.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MdbInfo mdbInfo = enumerator.Current;
						if (base.Config.DatabaseOverride.Contains(mdbInfo.Guid))
						{
							list.Add(mdbInfo);
						}
						else
						{
							mdbInfo.NotIndexed = IndexStatusErrorCode.CatalogExcluded;
							list2.Add(mdbInfo);
						}
					}
					goto IL_E0;
				}
			}
			mdbCollection.UpdateDatabasesIndexStatusInfo(base.Config.NumberOfCopiesToIndexPerDatabase);
			foreach (MdbInfo mdbInfo2 in databases)
			{
				if (mdbInfo2.NotIndexed == IndexStatusErrorCode.Unknown)
				{
					list.Add(mdbInfo2);
				}
				else
				{
					list2.Add(mdbInfo2);
				}
			}
			IL_E0:
			indexDisabledDatabases = list2.ToArray();
			return list.ToArray();
		}

		private void CleanupOrphanedFeedingControllers(Dictionary<Guid, SearchRootController.FeedingControllerContext> orphaned)
		{
			this.DropBreadCrumb("CleanupOrphanedFeedingControllers");
			lock (this.dictionaryLock)
			{
				this.CancelFeedingControllers(orphaned);
			}
		}

		private void CreateFeedingController(SearchRootController.FeedingControllerContext context)
		{
			this.DropBreadCrumb("CreateFeedingController: " + context.DatabaseInfo.Guid.ToString());
			if (!base.Stopping && this.feedingInstancePerMdb.ContainsKey(context.DatabaseInfo.Guid) && !this.feedingInstancePerMdb[context.DatabaseInfo.Guid].IsCanceled)
			{
				base.DiagnosticsSession.TraceDebug<MdbInfo>("Creating Feeding Controller for {0}", context.DatabaseInfo);
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					IExecutable executable = Factory.Current.CreateFeedingController(Factory.Current.CreateSearchServiceConfig(context.DatabaseInfo.Guid), context.DatabaseInfo, this.indexStatusStore, this.indexManager, context.Tracker);
					disposeGuard.Add<IExecutable>(executable);
					this.feedingInstancePerMdb[context.DatabaseInfo.Guid].FeedingController = executable;
					disposeGuard.Success();
				}
			}
		}

		private void StartFeedingController(SearchRootController.FeedingControllerContext context)
		{
			this.DropBreadCrumb("StartFeedingController: " + context.DatabaseInfo.Guid.ToString());
			bool flag = false;
			try
			{
				if (!base.Stopping && this.feedingInstancePerMdb.ContainsKey(context.DatabaseInfo.Guid) && !this.feedingInstancePerMdb[context.DatabaseInfo.Guid].IsCanceled)
				{
					using (DisposeGuard disposeGuard = default(DisposeGuard))
					{
						IExecutable feedingController = context.FeedingController;
						disposeGuard.Add<IExecutable>(feedingController);
						base.DiagnosticsSession.TraceDebug<MdbInfo>("Starting Feeding Controller for {0}", context.DatabaseInfo);
						feedingController.BeginExecute(new AsyncCallback(this.ExecuteComplete), context);
						flag = true;
						disposeGuard.Success();
					}
				}
			}
			finally
			{
				if (!flag)
				{
					this.DropBreadCrumb("StartFeedingController Failed: " + context.DatabaseInfo.Guid.ToString());
					this.feedingInstancePerMdb.Remove(context.DatabaseInfo.Guid);
					context.RestartNowEvent.Close();
					if (base.Stopping && this.feedingInstancePerMdb.Count == 0)
					{
						this.doneDisposingChildrenEvent.Set();
					}
				}
			}
		}

		private void ExecuteComplete(IAsyncResult asyncResult)
		{
			SearchRootController.FeedingControllerContext feedingControllerContext = (SearchRootController.FeedingControllerContext)asyncResult.AsyncState;
			IExecutable feedingController = feedingControllerContext.FeedingController;
			MdbInfo databaseInfo = feedingControllerContext.DatabaseInfo;
			Exception ex = null;
			try
			{
				base.DiagnosticsSession.TraceDebug<MdbInfo>("Calling EndExecute on the FeedingController for Mdb {0}", databaseInfo);
				feedingController.EndExecute(asyncResult);
			}
			catch (Exception ex2)
			{
				ex = ex2;
			}
			finally
			{
				this.LogResultOnCompleteFeedingController(feedingController, ex);
				lock (this.dictionaryLock)
				{
					TimeSpan timeout = base.Config.FeederRestartInterval;
					if (ex != null)
					{
						bool flag2 = true;
						bool flag3 = true;
						Exception ex3;
						if (Util.TryGetExceptionOrInnerOfType<CatalogReseedException>(ex, out ex3) || Util.TryGetExceptionOrInnerOfType<FeedingSkippedException>(ex, out ex3) || Util.TryGetExceptionOrInnerOfType<FeedingSkippedForCorruptionException>(ex, out ex3))
						{
							flag2 = false;
							flag3 = false;
						}
						else if (Util.TryGetExceptionOrInnerOfType<MapiExceptionMdbOffline>(ex, out ex3) || Util.TryGetExceptionOrInnerOfType<MapiExceptionNetworkError>(ex, out ex3) || Util.TryGetExceptionOrInnerOfType<DatabaseInitializationException>(ex, out ex3))
						{
							flag2 = false;
							flag3 = false;
							timeout = base.Config.DatabaseFailureRestartInterval;
						}
						else if (Util.TryGetExceptionOrInnerOfType<FastDocumentTimeoutException>(ex, out ex3))
						{
							flag3 = false;
						}
						else if (Util.TryGetExceptionOrInnerOfType<DocumentValidationException>(ex, out ex3))
						{
							flag2 = false;
							if (!base.Config.MonitorDocumentValidationFailures)
							{
								flag3 = false;
								timeout = base.Config.DatabaseFailureRestartInterval;
							}
						}
						else if (Util.TryGetExceptionOrInnerOfType<DocumentFeederLostCallbackException>(ex, out ex3))
						{
							flag2 = false;
							flag3 = true;
							timeout = base.Config.DatabaseFailureRestartInterval;
						}
						if (flag2)
						{
							feedingControllerContext.Tracker.MarkCurrentlyTrackedDocumentsAsPoison();
						}
						if (flag3 && !ExEnvironment.IsTestProcess)
						{
							bool flag4 = false;
							if (base.Config.FeedingControllerFailureWhitelist != null)
							{
								string text = ex.ToString();
								foreach (string text2 in base.Config.FeedingControllerFailureWhitelist)
								{
									if (Regex.IsMatch(text, text2, RegexOptions.IgnoreCase) || text.IndexOf(text2, StringComparison.OrdinalIgnoreCase) >= 0)
									{
										flag4 = true;
										break;
									}
								}
							}
							if (!flag4)
							{
								EventNotificationItem.Publish(ExchangeComponent.Search.Name, "SearchFeedingControllerFailure", databaseInfo.Name, Strings.SearchFeedingControllerException(databaseInfo.Name, ex.ToString()), ResultSeverityLevel.Error, false);
							}
						}
					}
					if (!base.Stopping && !feedingControllerContext.IsCanceled)
					{
						feedingControllerContext.RestartNowEvent.Reset();
					}
					if (this.mdbWatcher != null && !this.mdbWatcher.Exists(feedingControllerContext.DatabaseInfo.Guid))
					{
						feedingControllerContext.IsCanceled = true;
					}
					RegisteredWaitHandleWrapper.RegisterWaitForSingleObject(feedingControllerContext.RestartNowEvent, CallbackWrapper.WaitOrTimerCallback(new WaitOrTimerCallback(this.CompleteDelayedRestartCallback)), feedingControllerContext, timeout, true);
				}
			}
		}

		private void CompleteDelayedRestartCallback(object state, bool timerFired)
		{
			SearchRootController.FeedingControllerContext feedingControllerContext = (SearchRootController.FeedingControllerContext)state;
			this.DropBreadCrumb("CompleteDelayedRestartCallback: " + feedingControllerContext.DatabaseInfo.Guid.ToString());
			base.DiagnosticsSession.TraceDebug<string, bool>("Disposing SearchFeedingController for Mdb {0}; timerFired={1}", feedingControllerContext.DatabaseInfo.Name, timerFired);
			feedingControllerContext.FeedingController.Dispose();
			lock (this.dictionaryLock)
			{
				this.CreateFeedingController(feedingControllerContext);
				this.StartFeedingController(feedingControllerContext);
			}
		}

		private void LogResultOnCompleteFeedingController(IExecutable feedingInstance, Exception result)
		{
			this.DropBreadCrumb("LogResultOnCompleteFeedingController: " + feedingInstance.InstanceName);
			if (result == null)
			{
				base.DiagnosticsSession.TraceDebug<string>("FeedingController for '{0}' completed without error.", feedingInstance.InstanceName);
				return;
			}
			base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Failures, "FeedingController for '{0}' completed with exception: {1}", new object[]
			{
				feedingInstance.InstanceName,
				result
			});
			base.DiagnosticsSession.LogEvent(MSExchangeFastSearchEventLogConstants.Tuple_SearchIndexingUnexpectedException, new object[]
			{
				feedingInstance.InstanceName,
				result
			});
		}

		private void RecordDocumentProcessing(Guid mdbGuid, Guid instance, Guid correlationId, long docId)
		{
			SearchRootController.FeedingControllerContext feedingControllerContext;
			lock (this.dictionaryLock)
			{
				if (!this.feedingInstancePerMdb.TryGetValue(mdbGuid, out feedingControllerContext))
				{
					base.DiagnosticsSession.TraceError<Guid>("Received an invalid Mdb Guid on the Document Tracking RPC API: {0}", mdbGuid);
					return;
				}
			}
			feedingControllerContext.Tracker.RecordDocumentProcessing(instance, correlationId, docId);
		}

		private void RecordDocumentFailure(Guid mdbGuid, Guid correlationId, long docId, string errorMessage)
		{
			SearchRootController.FeedingControllerContext feedingControllerContext;
			lock (this.dictionaryLock)
			{
				if (!this.feedingInstancePerMdb.TryGetValue(mdbGuid, out feedingControllerContext))
				{
					base.DiagnosticsSession.TraceError<Guid>("Received an invalid Mdb Guid on the Document Failure RPC API: {0}", mdbGuid);
					return;
				}
			}
			feedingControllerContext.Tracker.MarkDocumentAsPoison(docId);
			base.DiagnosticsSession.LogEvent(MSExchangeFastSearchEventLogConstants.Tuple_SevereDocumentFailure, new object[]
			{
				mdbGuid,
				docId,
				correlationId,
				errorMessage
			});
		}

		private void UpdateIndexSystems()
		{
			this.DropBreadCrumb("UpdateIndexSystems");
			this.ResetTimer();
		}

		private void ResumeIndexing(Guid databaseGuid)
		{
			this.DropBreadCrumb("ResumeIndexing:" + databaseGuid.ToString());
			lock (this.dictionaryLock)
			{
				SearchRootController.FeedingControllerContext feedingControllerContext;
				if (this.feedingInstancePerMdb.TryGetValue(databaseGuid, out feedingControllerContext))
				{
					feedingControllerContext.RestartNowEvent.Set();
				}
			}
		}

		private void RecrawlMailbox(Guid mdbGuid, Guid mailboxGuid)
		{
			this.DropBreadCrumb("RecrawlMailbox:" + mailboxGuid.ToString());
			SearchRootController.FeedingControllerContext feedingControllerContext;
			lock (this.dictionaryLock)
			{
				if (!this.feedingInstancePerMdb.TryGetValue(mdbGuid, out feedingControllerContext))
				{
					throw new ArgumentException("MDB guid not found");
				}
			}
			((SearchFeedingController)feedingControllerContext.FeedingController).RecrawlMailbox(mailboxGuid);
		}

		private void DropBreadCrumb(string breadCrumb)
		{
			this.breadCrumbs.Drop(string.Format("{0}, {1}, {2}", DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss.fff"), Thread.CurrentThread.ManagedThreadId, breadCrumb));
		}

		internal const string SearchFeedingControllerFailure = "SearchFeedingControllerFailure";

		private const string HARegistrySubKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\autoreseed";

		private readonly Dictionary<Guid, SearchRootController.FeedingControllerContext> feedingInstancePerMdb = new Dictionary<Guid, SearchRootController.FeedingControllerContext>(48);

		private readonly object dictionaryLock = new object();

		private readonly ManualResetEvent doneDisposingChildrenEvent = new ManualResetEvent(false);

		private readonly HashSet<string> catalogsToRebuild = new HashSet<string>();

		private IFlowManager flowManager;

		private IIndexManager indexManager;

		private IIndexStatusStore indexStatusStore;

		private IMdbWatcher mdbWatcher;

		private GuardedTimer eventTimer;

		private List<Guid> feedingInstanceNeedToStart = new List<Guid>();

		private Breadcrumbs<string> breadCrumbs;

		private volatile bool newEvents;

		private bool isTimerCallbackScheduled;

		private sealed class FeedingControllerContext
		{
			internal FeedingControllerContext(MdbInfo mdbInfo)
			{
				this.databaseInfo = mdbInfo;
				this.restartNowEvent = new ManualResetEvent(true);
				this.tracker = Factory.Current.CreateDocumentTracker();
			}

			internal MdbInfo DatabaseInfo
			{
				get
				{
					return this.databaseInfo;
				}
			}

			internal EventWaitHandle RestartNowEvent
			{
				get
				{
					return this.restartNowEvent;
				}
			}

			internal IDocumentTracker Tracker
			{
				get
				{
					return this.tracker;
				}
			}

			internal IExecutable FeedingController { get; set; }

			internal bool IsCanceled { get; set; }

			private readonly MdbInfo databaseInfo;

			private readonly ManualResetEvent restartNowEvent;

			private readonly IDocumentTracker tracker;
		}
	}
}
