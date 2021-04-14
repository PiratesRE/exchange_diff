using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.EventLog;
using Microsoft.Exchange.Search.Fast;
using Microsoft.Exchange.Search.Mdb;
using Microsoft.Exchange.Search.OperatorSchema;
using Microsoft.Exchange.Search.Performance;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Search.Engine
{
	internal class SearchFeedingController : Executable
	{
		internal SearchFeedingController(ISearchServiceConfig config, MdbInfo mdbInfo, IIndexStatusStore indexStatusStore, IIndexManager indexManager, IDocumentTracker documentTracker) : base(config)
		{
			Util.ThrowOnNullArgument(config, "config");
			Util.ThrowOnNullArgument(mdbInfo, "mdbInfo");
			Util.ThrowOnNullArgument(indexStatusStore, "indexStatusStore");
			Util.ThrowOnNullArgument(indexManager, "indexManager");
			base.DiagnosticsSession.ComponentName = "SearchFeedingController";
			base.DiagnosticsSession.Tracer = ExTraceGlobals.MdbFeedingControllerTracer;
			this.MdbInfo = mdbInfo;
			base.InstanceName = mdbInfo.Name;
			this.mdbFeedingPerfCounters = this.GetMdbFeedingPerfCounterInstance(mdbInfo.Name);
			if (this.mdbFeedingPerfCounters != null)
			{
				this.mdbFeedingPerfCounters.Reset();
			}
			this.IndexSystemName = FastIndexVersion.GetIndexSystemName(mdbInfo.Guid);
			this.feeders = new HashSet<IFeeder>();
			this.indexStatusStore = indexStatusStore;
			this.fastIndexStatus = new FastIndexStatus(this.IndexSystemName, indexManager);
			this.documentTracker = documentTracker;
			this.failedItemStorage = Factory.Current.CreateFailedItemStorage(base.Config, this.IndexSystemName);
		}

		internal MdbInfo MdbInfo { get; private set; }

		internal string IndexSystemName { get; private set; }

		internal ISubmitDocument FastFeeder
		{
			[DebuggerStepThrough]
			get
			{
				return this.fastFeeder;
			}
		}

		internal IWatermarkStorage WatermarkStorage
		{
			[DebuggerStepThrough]
			get
			{
				return this.watermarkStorage;
			}
		}

		public override XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			List<XElement> list = new List<XElement>(10);
			list.Add(new XElement("MdbName", this.MdbInfo.Name));
			list.Add(new XElement("MdbGuid", this.MdbInfo.Guid));
			list.Add(new XElement("OwningServer", this.MdbInfo.OwningServer));
			if (this.MdbInfo.DatabaseCopies != null)
			{
				XElement xelement = new XElement("Servers");
				foreach (MdbCopy mdbCopy in this.MdbInfo.DatabaseCopies)
				{
					XElement xelement2 = new XElement("Server");
					xelement2.Add(new XElement("Name", mdbCopy.Name));
					xelement2.Add(new XElement("ActivationPreference", mdbCopy.ActivationPreference));
					xelement2.Add(new XElement("MaxSchemaVersion", mdbCopy.SchemaVersion));
					xelement.Add(xelement2);
				}
				list.Add(xelement);
			}
			list.Add(new XElement("ActivationPreference", this.MdbInfo.ActivationPreference));
			list.Add(new XElement("PreferredActiveCopy", this.MdbInfo.PreferredActiveCopy));
			list.Add(new XElement("QueryVersion", this.MdbInfo.CatalogVersion.QueryVersion));
			list.Add(new XElement("FeedingVersion", this.MdbInfo.CatalogVersion.FeedingVersion));
			list.Add(new XElement("MaxSupportedVersion", this.MdbInfo.MaxSupportedVersion));
			if (base.Config.DisableFeeders)
			{
				list.Add(new XElement("Disabled", "True"));
			}
			lock (this.feedersLock)
			{
				foreach (IFeeder feeder in this.feeders)
				{
					Executable executable = (Executable)feeder;
					list.Add(executable.GetDiagnosticInfo(parameters));
				}
			}
			if (this.documentTracker != null)
			{
				list.Add(this.documentTracker.GetDiagnosticInfo(parameters));
			}
			list.Add(new XElement("GracefulDegradationEnabled", base.Config.GracefulDegradationEnabled));
			list.Add(new XElement("Config-DisableInstantSearch", ((SearchConfig)base.Config).DisableInstantSearch));
			list.Add(new XElement("Current-DisableInstantSearch", !this.MdbInfo.IsInstantSearchEnabled));
			list.Add(new XElement("Config-DisableRefiners", ((SearchConfig)base.Config).DisableRefiners));
			list.Add(new XElement("Current-DisableRefiners", !this.MdbInfo.IsRefinersEnabled));
			if (!this.MdbInfo.MountedOnLocalServer)
			{
				list.Add(new XElement("AutoSuspendCatalog", ((SearchConfig)base.Config).AutoSuspendCatalog));
				list.Add(new XElement("IsCatalogSuspended", this.MdbInfo.IsCatalogSuspended));
			}
			list.Add(new XElement("NumberOfItems", this.MdbInfo.NumberOfItems));
			XElement diagnosticInfo = base.GetDiagnosticInfo(parameters);
			diagnosticInfo.AddFirst(list);
			return diagnosticInfo;
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SearchFeedingController>(this);
		}

		internal void RecrawlMailbox(Guid mailboxGuid)
		{
			CrawlerMailboxIterator crawlerMailboxIterator = Factory.Current.CreateCrawlerMailboxIterator(this.MdbInfo);
			IEnumerable<MailboxInfo> mailboxes = crawlerMailboxIterator.GetMailboxes(mailboxGuid);
			foreach (MailboxInfo mailboxInfo in mailboxes)
			{
				MailboxCrawlerState mailboxState = new MailboxCrawlerState(mailboxInfo.MailboxNumber, -2, 0)
				{
					RecrawlMailbox = true
				};
				IAsyncResult asyncResult = this.watermarkStorage.BeginSetMailboxCrawlerState(mailboxState, null, null);
				this.watermarkStorage.EndSetMailboxCrawlerState(asyncResult);
			}
		}

		internal long QueryItemsCount()
		{
			return this.failedItemStorage.GetItemsCount(Guid.Empty);
		}

		internal void PublishCatalogState()
		{
			base.DiagnosticsSession.SetCounterRawValue(this.mdbFeedingPerfCounters.InstantSearchEnabled, this.MdbInfo.IsInstantSearchEnabled ? 1L : 0L);
			base.DiagnosticsSession.SetCounterRawValue(this.mdbFeedingPerfCounters.CatalogSuspended, this.MdbInfo.IsCatalogSuspended ? 1L : 0L);
		}

		internal void InitializeFastFeederAndWatermarkStorage()
		{
			if (this.watermarkStorage == null)
			{
				if (this.documentTracker != null)
				{
					this.documentTracker.Initialize(this.failedItemStorage);
				}
				int num = this.CalculateNumberOfFeederSessions();
				if (this.mdbFeedingPerfCounters != null)
				{
					base.DiagnosticsSession.SetCounterRawValue(this.mdbFeedingPerfCounters.FeedingSessions, (long)num);
				}
				this.fastFeeder = Factory.Current.CreateFastFeeder(base.Config, FlowDescriptor.GetIndexingFlowDescriptor(base.Config).DisplayName, this.IndexSystemName, this.MdbInfo.Name, num);
				this.fastFeeder.Tracker = this.documentTracker;
				this.watermarkStorage = Factory.Current.CreateWatermarkStorage(this.fastFeeder, base.Config, this.IndexSystemName);
			}
		}

		protected override void InternalExecutionStart()
		{
			base.DiagnosticsSession.TraceDebug<MdbInfo>("Starting feeding controller for {0}", this.MdbInfo);
			ContentIndexStatusType state = ContentIndexStatusType.Disabled;
			IndexStatusErrorCode indexStatusErrorCode = IndexStatusErrorCode.IndexNotEnabled;
			int? failureCode = null;
			string failureReason = null;
			string text;
			if (!base.Config.DisableFeeders && (state = this.fastIndexStatus.GetIndexStatus(out indexStatusErrorCode, out text, out failureCode, out failureReason)) == ContentIndexStatusType.Healthy)
			{
				this.MdbInfo.UpdateDatabaseLocationInfo();
				base.DiagnosticsSession.TraceDebug<MdbInfo, string, string>("{0} mounted on {1} {2}", this.MdbInfo, this.MdbInfo.OwningServer, this.MdbInfo.MountedOnLocalServer ? "(local)" : "(remote)");
				this.InitializeFastFeederAndWatermarkStorage();
				this.MdbInfo.CatalogVersion = this.watermarkStorage.GetVersionInfo();
				this.DetermineFeederStateAndStartFeeders();
				base.DiagnosticsSession.SetCounterRawValue(this.mdbFeedingPerfCounters.NumberOfItems, this.MdbInfo.NumberOfItems);
				return;
			}
			if (indexStatusErrorCode == IndexStatusErrorCode.CatalogCorruption)
			{
				throw new FeedingSkippedForCorruptionException(this.MdbInfo, state, indexStatusErrorCode, failureCode, failureReason);
			}
			throw new FeedingSkippedException(this.MdbInfo, state, indexStatusErrorCode);
		}

		protected override void InternalExecutionFinish()
		{
			HashSet<IFeeder> hashSet;
			List<WaitHandle> list;
			lock (this.feedersLock)
			{
				hashSet = new HashSet<IFeeder>(this.feeders);
				list = new List<WaitHandle>(this.feeders.Count);
				if (this.feeders.Count == 0)
				{
					this.doneDisposingChildrenEvent.Set();
				}
				foreach (IFeeder feeder in hashSet)
				{
					if (feeder.AsyncResult != null)
					{
						list.Add(feeder.AsyncResult.AsyncWaitHandle);
					}
				}
			}
			foreach (IFeeder feeder2 in hashSet)
			{
				if (feeder2.AsyncResult != null)
				{
					feeder2.CancelExecute();
				}
			}
			foreach (WaitHandle waitHandle in list)
			{
				waitHandle.WaitOne(base.Config.MaxOperationTimeout);
			}
			this.UpdateIndexStatus(UpdateIndexStatusSource.FeedingCompletes);
			base.DiagnosticsSession.LogEvent(MSExchangeFastSearchEventLogConstants.Tuple_SearchIndexingStopSuccess, new object[]
			{
				base.InstanceName
			});
		}

		protected override void Dispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				if (this.doneDisposingChildrenEvent != null)
				{
					this.doneDisposingChildrenEvent.WaitOne();
					this.doneDisposingChildrenEvent.Dispose();
					this.doneDisposingChildrenEvent = null;
				}
				if (this.watermarkStorage != null)
				{
					this.watermarkStorage.Dispose();
				}
				if (this.failedItemStorage != null)
				{
					this.failedItemStorage.Dispose();
				}
				if (this.fastFeeder != null)
				{
					this.fastFeeder.Dispose();
				}
				if (this.watermarkInitializationCounter != null)
				{
					this.watermarkInitializationCounter.Dispose();
				}
			}
			base.Dispose(calledFromDispose);
		}

		protected virtual MdbPerfCountersInstance GetMdbFeedingPerfCounterInstance(string databaseName)
		{
			return MdbPerfCounters.GetInstance(databaseName);
		}

		private void StartFeeders()
		{
			this.doneDisposingChildrenEvent.Reset();
			if (!base.Config.DisableCrawlerFeeder)
			{
				this.CreateAndStartFeeder(FeederType.Crawler);
			}
			this.CreateAndStartFeeder(FeederType.Notifications);
			if (!base.Config.DisableRetryFeeder)
			{
				this.CreateAndStartFeeder(FeederType.Retry);
			}
			base.DiagnosticsSession.LogEvent(MSExchangeFastSearchEventLogConstants.Tuple_SearchIndexingStartSuccess, new object[]
			{
				base.InstanceName
			});
		}

		private void CreateAndStartFeeder(FeederType feederType)
		{
			IFeeder feeder;
			lock (this.feedersLock)
			{
				if (base.Stopping)
				{
					return;
				}
				switch (feederType)
				{
				case FeederType.Notifications:
					feeder = Factory.Current.CreateNotificationsFeeder(this.mdbFeedingPerfCounters, this.MdbInfo, base.Config, this.fastFeeder, this.watermarkStorage, this.indexStatusStore);
					break;
				case FeederType.Crawler:
					feeder = Factory.Current.CreateCrawlerFeeder(this.mdbFeedingPerfCounters, this.MdbInfo, base.Config, this.fastFeeder, this.watermarkStorage, this.failedItemStorage, this.indexStatusStore);
					break;
				case FeederType.Retry:
					feeder = Factory.Current.CreateRetryFeeder(this.mdbFeedingPerfCounters, this.MdbInfo, base.Config, this.fastFeeder, this.failedItemStorage, this.watermarkStorage, this.indexStatusStore);
					break;
				default:
					throw new InvalidOperationException("feederType");
				}
				if (!this.feeders.Add(feeder))
				{
					throw new InvalidOperationException("Unable to add the feeder to the collection.");
				}
			}
			feeder.BeginExecute(new AsyncCallback(this.ExecuteComplete), feeder);
		}

		private void ExecuteComplete(IAsyncResult asyncResult)
		{
			IFeeder feeder = (IFeeder)asyncResult.AsyncState;
			bool flag = true;
			try
			{
				feeder.EndExecute(asyncResult);
				if (feeder.FeederType == FeederType.Crawler && base.Config.FlushAfterCrawl)
				{
					LazyAsyncResult lazyAsyncResult = (LazyAsyncResult)asyncResult;
					string a = lazyAsyncResult.Result as string;
					if (a == "CrawlComplete")
					{
						IndexManager.Instance.FlushCatalog(this.MdbInfo.IndexSystemName);
					}
				}
			}
			catch (Exception ex)
			{
				base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Failures, "Source Feeder [{0} : {1}] failed with exception: {2}", new object[]
				{
					feeder.GetDiagnosticComponentName(),
					feeder.InstanceName,
					ex
				});
				base.CompleteExecute(ex);
				flag = false;
			}
			if (flag)
			{
				RegisteredWaitHandleWrapper.RegisterWaitForSingleObject(base.StopEvent, CallbackWrapper.WaitOrTimerCallback(new WaitOrTimerCallback(this.CompleteDelayedRestartCallback)), feeder, base.Config.FeederRestartInterval, true);
				return;
			}
			this.CleanupFeeder(feeder);
		}

		private void CompleteDelayedRestartCallback(object state, bool timerFired)
		{
			IFeeder feeder = (IFeeder)state;
			this.CleanupFeeder(feeder);
			if (timerFired)
			{
				this.CreateAndStartFeeder(feeder.FeederType);
			}
		}

		private void CleanupFeeder(IFeeder feeder)
		{
			try
			{
				feeder.Dispose();
			}
			finally
			{
				lock (this.feedersLock)
				{
					this.feeders.Remove(feeder);
				}
				if (base.Stopping && this.feeders.Count == 0)
				{
					this.doneDisposingChildrenEvent.Set();
				}
			}
		}

		private int CalculateNumberOfFeederSessions()
		{
			int result;
			using (INotificationsEventSource notificationsEventSource = Factory.Current.CreateNotificationsEventSource(this.MdbInfo))
			{
				int num = base.Config.MinFeederSessions;
				if (!this.MdbInfo.MountedOnLocalServer)
				{
					long networkLatency = notificationsEventSource.GetNetworkLatency(5);
					int num2 = (int)(networkLatency / (long)base.Config.ExtraFeederSessionsLatency);
					base.DiagnosticsSession.TraceDebug<long, int>("Network Latency: {0} ms, requesting {1} extra sessions", networkLatency, num2);
					num += num2;
				}
				num = Math.Min(num, base.Config.MaxFeederSessions);
				base.DiagnosticsSession.TraceDebug<int>("Sessions needed: {0}", num);
				result = num;
			}
			return result;
		}

		private IndexStatus UpdateIndexStatus(UpdateIndexStatusSource location)
		{
			int? num = null;
			string text = null;
			ContentIndexStatusType contentIndexStatusType;
			IndexStatusErrorCode indexStatusErrorCode;
			string text2;
			if (base.Config.DisableFeeders)
			{
				base.DiagnosticsSession.TraceDebug<MdbInfo>("Feeding is disabled for {0}", this.MdbInfo);
				contentIndexStatusType = ContentIndexStatusType.Disabled;
				indexStatusErrorCode = IndexStatusErrorCode.IndexNotEnabled;
				text2 = null;
			}
			else
			{
				contentIndexStatusType = this.fastIndexStatus.GetIndexStatus(out indexStatusErrorCode, out text2, out num, out text);
				if (contentIndexStatusType == ContentIndexStatusType.Healthy)
				{
					Exception ex = ((LazyAsyncResult)base.AsyncResult).Result as Exception;
					if (ex != null)
					{
						Exception ex2;
						if (Util.TryGetExceptionOrInnerOfType<CatalogReseedException>(ex, out ex2))
						{
							base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Failures, "The feeding of Mdb: {0} failed with {1}", new object[]
							{
								this.MdbInfo,
								ex2
							});
							contentIndexStatusType = ContentIndexStatusType.FailedAndSuspended;
							indexStatusErrorCode = ((CatalogReseedException)ex2).OriginalErrorCode;
							goto IL_3E6;
						}
						if (Util.TryGetExceptionOrInnerOfType<MapiExceptionMdbOffline>(ex, out ex2))
						{
							base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Failures, "The feeding of Mdb: {0} failed with {1}", new object[]
							{
								this.MdbInfo,
								ex2
							});
							contentIndexStatusType = ContentIndexStatusType.Failed;
							indexStatusErrorCode = IndexStatusErrorCode.DatabaseOffline;
							goto IL_3E6;
						}
						if (Util.TryGetExceptionOrInnerOfType<MapiExceptionNetworkError>(ex, out ex2))
						{
							base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Failures, "The feeding of Mdb: {0} failed with {1}", new object[]
							{
								this.MdbInfo,
								ex2
							});
							contentIndexStatusType = ContentIndexStatusType.Failed;
							indexStatusErrorCode = IndexStatusErrorCode.MapiNetworkError;
							goto IL_3E6;
						}
						base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Failures, "The feeding of Mdb: {0} failed with {1}", new object[]
						{
							this.MdbInfo,
							ex
						});
						contentIndexStatusType = ContentIndexStatusType.Failed;
						indexStatusErrorCode = IndexStatusErrorCode.InternalError;
						goto IL_3E6;
					}
					else
					{
						using (INotificationsEventSource notificationsEventSource = Factory.Current.CreateNotificationsEventSource(this.MdbInfo))
						{
							NotificationsEventSourceInfo notificationsEventSourceInfo = new NotificationsEventSourceInfo(this.watermarkStorage, notificationsEventSource, base.DiagnosticsSession, this.MdbInfo);
							base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "Watermarks Information for Mdb [{0}] - NotificationsWatermark: {1}; HasCrawlerWatermarks: {2}; FirstEvent: {3}; LastEvent: {4}, MaxEvents: {5}", new object[]
							{
								this.MdbInfo,
								notificationsEventSourceInfo.NotificationsWatermark,
								this.watermarkStorage.HasCrawlerWatermarks(),
								notificationsEventSourceInfo.FirstEvent,
								notificationsEventSourceInfo.LastEvent,
								base.Config.MaxEventsToBeHandledByNotificationFeeder
							});
							if (notificationsEventSourceInfo.NotificationsWatermark > notificationsEventSourceInfo.LastEvent)
							{
								base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "For mdb {0} NotificationsWatermarks came after LastEvent : {1} , {2}", new object[]
								{
									this.MdbInfo,
									notificationsEventSourceInfo.NotificationsWatermark,
									notificationsEventSourceInfo.LastEvent
								});
							}
							if (notificationsEventSourceInfo.NotificationsWatermark > 0L && !this.watermarkStorage.HasCrawlerWatermarks() && ((notificationsEventSourceInfo.FirstEvent == 1L && notificationsEventSourceInfo.LastEvent - notificationsEventSourceInfo.NotificationsWatermark <= (long)base.Config.MaxEventsToBeHandledByNotificationFeeder) || notificationsEventSourceInfo.FirstEvent != 1L))
							{
								base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "Catalog is being set to Healthy for mdb {0}", new object[]
								{
									this.MdbInfo
								});
								contentIndexStatusType = ContentIndexStatusType.Healthy;
							}
							else
							{
								base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Warnings, "Catalog is Unknown as condition were not met for mdb {0}", new object[]
								{
									this.MdbInfo
								});
								contentIndexStatusType = ContentIndexStatusType.Unknown;
							}
							goto IL_3E6;
						}
					}
				}
				if (indexStatusErrorCode == IndexStatusErrorCode.CatalogCorruption)
				{
					base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Failures, "Catalog is corrupt and needs reseed for mdb {0}. Failure code: '{1}'. Failure reason: '{2}'", new object[]
					{
						this.MdbInfo,
						num,
						text
					});
					contentIndexStatusType = ContentIndexStatusType.FailedAndSuspended;
					switch (location)
					{
					case UpdateIndexStatusSource.FeedingStarts:
						indexStatusErrorCode = IndexStatusErrorCode.CatalogCorruptionWhenFeedingStarts;
						break;
					case UpdateIndexStatusSource.FeedingCompletes:
						indexStatusErrorCode = IndexStatusErrorCode.CatalogCorruptionWhenFeedingCompletes;
						break;
					default:
						throw new ArgumentException(string.Format("UpdateIndexStatusSource {0} doesn't match any method calling source", location));
					}
				}
				else if (this.MdbInfo.ShouldAutomaticallySuspendCatalog && contentIndexStatusType == ContentIndexStatusType.Suspended)
				{
					base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Warnings, "Catalog is automatically suspended for mdb {0}", new object[]
					{
						this.MdbInfo
					});
					contentIndexStatusType = ContentIndexStatusType.AutoSuspended;
				}
				else if (contentIndexStatusType == ContentIndexStatusType.Seeding)
				{
					base.DiagnosticsSession.Assert(!string.IsNullOrEmpty(text2), "SeedingSource must be present", new object[0]);
				}
			}
			IL_3E6:
			IndexStatus indexStatus = this.indexStatusStore.SetIndexStatus(this.MdbInfo.Guid, contentIndexStatusType, indexStatusErrorCode, this.MdbInfo.CatalogVersion, text2);
			if (this.mdbFeedingPerfCounters != null)
			{
				base.DiagnosticsSession.SetCounterRawValue(this.mdbFeedingPerfCounters.IndexingStatus, (long)indexStatus.IndexingState);
			}
			return indexStatus;
		}

		private bool SchemaUpgradeCrawlRequired()
		{
			bool flag;
			VersionInfo versionInfo;
			if (this.MdbInfo.CatalogVersion.IsUpgrading)
			{
				int feedingVersion = this.MdbInfo.CatalogVersion.FeedingVersion;
				VersionInfo latest = VersionInfo.Latest;
				if (feedingVersion <= latest.FeedingVersion)
				{
					if (base.Config.SchemaUpgradingEnabled)
					{
						base.DiagnosticsSession.LogPeriodicEvent(MSExchangeFastSearchEventLogConstants.Tuple_ResumeSchemaUpdate, this.MdbInfo.Name, new object[]
						{
							this.MdbInfo,
							this.MdbInfo.CatalogVersion,
							VersionInfo.Latest
						});
						return true;
					}
					return false;
				}
				else
				{
					int feedingVersion2 = this.MdbInfo.CatalogVersion.FeedingVersion;
					VersionInfo latest2 = VersionInfo.Latest;
					int num = Math.Min(feedingVersion2, latest2.FeedingVersion);
					int queryVersion = this.MdbInfo.CatalogVersion.QueryVersion;
					VersionInfo latest3 = VersionInfo.Latest;
					int num2 = Math.Min(queryVersion, latest3.QueryVersion);
					flag = false;
					versionInfo = VersionInfo.FromVersions(num2, num);
					base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Failures, "Index system for database {0} does not support version {1}. Downgrading to  {2}", new object[]
					{
						this.MdbInfo,
						this.MdbInfo.CatalogVersion,
						versionInfo
					});
				}
			}
			else
			{
				int num3 = Math.Min(this.MdbInfo.MaxSupportedVersion, base.Config.TargetSchemaVersion.FeedingVersion);
				if (this.MdbInfo.CatalogVersion.FeedingVersion < num3 && base.Config.SchemaUpgradingEnabled)
				{
					if (!this.MdbInfo.PreferredActiveCopy)
					{
						return false;
					}
					flag = true;
					versionInfo = VersionInfo.FromVersions(this.MdbInfo.CatalogVersion.QueryVersion, num3);
					base.DiagnosticsSession.LogPeriodicEvent(MSExchangeFastSearchEventLogConstants.Tuple_SchemaUpdateIsAvailable, this.MdbInfo.Name, new object[]
					{
						this.MdbInfo,
						this.MdbInfo.CatalogVersion,
						versionInfo
					});
				}
				else
				{
					int feedingVersion3 = this.MdbInfo.CatalogVersion.FeedingVersion;
					VersionInfo latest4 = VersionInfo.Latest;
					if (feedingVersion3 <= latest4.FeedingVersion)
					{
						return false;
					}
					flag = false;
					versionInfo = VersionInfo.Latest;
					base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Failures, "Index system for database {0} does not support version {1}. Downgrading to VersionInfo.Latest: {2}", new object[]
					{
						this.MdbInfo,
						this.MdbInfo.CatalogVersion,
						VersionInfo.Latest
					});
				}
			}
			this.upgradeVersion = versionInfo;
			this.upgradeInitializationCounter = InterlockedCounter.Create(1, (int value) => value == 0, new Action(this.UpgradeVersion));
			if (flag)
			{
				base.DiagnosticsSession.LogPeriodicEvent(MSExchangeFastSearchEventLogConstants.Tuple_InitiateSchemaUpdate, this.MdbInfo.Name, new object[]
				{
					this.MdbInfo,
					this.MdbInfo.CatalogVersion,
					VersionInfo.Latest
				});
				this.watermarkInitializationCounter.Increment();
				CrawlerMailboxIterator crawlerMailboxIterator = Factory.Current.CreateCrawlerMailboxIterator(this.MdbInfo);
				foreach (MailboxInfo mailboxInfo in crawlerMailboxIterator.GetMailboxes())
				{
					this.upgradeInitializationCounter.Increment();
					this.watermarkStorage.BeginSetMailboxCrawlerState(new MailboxCrawlerState(mailboxInfo.MailboxNumber, -1, 0), new AsyncCallback(this.EndSetMailboxCrawlerState), new SearchFeedingController.EndSetMailboxCrawlerStateParameters(mailboxInfo, this.upgradeInitializationCounter));
				}
			}
			this.upgradeInitializationCounter.Decrement();
			return flag;
		}

		private void UpgradeVersion()
		{
			this.MdbInfo.CatalogVersion = this.upgradeVersion;
			this.watermarkStorage.BeginSetVersionInfo(this.upgradeVersion, new AsyncCallback(this.EndSetVersionInfo), null);
		}

		private void DetermineFeederStateAndStartFeeders()
		{
			this.watermarkInitializationCounter = InterlockedCounter.Create(1, (int value) => value == 0);
			NotificationsEventSourceInfo notificationsEventSourceInfo;
			using (INotificationsEventSource notificationsEventSource = Factory.Current.CreateNotificationsEventSource(this.MdbInfo))
			{
				notificationsEventSourceInfo = new NotificationsEventSourceInfo(this.watermarkStorage, notificationsEventSource, base.DiagnosticsSession, this.MdbInfo);
			}
			ICollection<MailboxCrawlerState> mailboxesForCrawling = this.watermarkStorage.GetMailboxesForCrawling();
			base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "OnStart: The current state storage values for Mdb [{0}] are: Number of unprocessed events: {1}, Age of oldest unprocessed event (TimeSpan): {2}, Last event: {3} Number of mailboxes to crawl: {4}", new object[]
			{
				this.MdbInfo,
				notificationsEventSourceInfo.WatermarkDelta,
				notificationsEventSourceInfo.CurrentEventAge,
				notificationsEventSourceInfo.LastEvent,
				mailboxesForCrawling.Count
			});
			if (notificationsEventSourceInfo.LastEvent < 1L)
			{
				throw new DatabaseInitializationException(Strings.ReadLastEventFailure(this.MdbInfo.ToString(), notificationsEventSourceInfo.LastEvent));
			}
			if (notificationsEventSourceInfo.NotificationsWatermark >= 0L)
			{
				if (notificationsEventSourceInfo.NotificationsWatermark < notificationsEventSourceInfo.FirstEvent)
				{
					base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Warnings, "Missing events for Mdb [{0}]. Watermark {1} is less than first reliable event {2}. Requesting reseed.", new object[]
					{
						this.MdbInfo,
						notificationsEventSourceInfo.NotificationsWatermark,
						notificationsEventSourceInfo.FirstEvent
					});
					throw new CatalogReseedException(Strings.MissingNotifications(this.MdbInfo.ToString()), IndexStatusErrorCode.EventsMissingWithNotificationsWatermark);
				}
				if (!this.SchemaUpgradeCrawlRequired() && !this.MdbInfo.PreferredActiveCopy)
				{
					foreach (MailboxCrawlerState mailboxCrawlerState in mailboxesForCrawling)
					{
						if (!mailboxCrawlerState.RecrawlMailbox && (!ExEnvironment.IsTest || !base.Config.AllowCrawlOnPassiveForTest))
						{
							base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Warnings, "Crawling is suppressed and the catalog needs reseed for mdb {0}", new object[]
							{
								this.MdbInfo
							});
							throw new CatalogReseedException(Strings.ReseedOnPassiveServer(this.MdbInfo.ToString()), IndexStatusErrorCode.CrawlOnNonPreferredActiveWithNotificationsWatermark);
						}
					}
				}
				this.UpdateIndexStatus(UpdateIndexStatusSource.FeedingStarts);
			}
			else
			{
				this.PrepareToCrawl(notificationsEventSourceInfo);
			}
			this.watermarkInitializationCounter.Decrement();
			this.watermarkInitializationCounter.Wait(base.Config.DocumentFeederProcessingTimeout);
			this.StartFeeders();
		}

		private void PrepareToCrawl(NotificationsEventSourceInfo ciWatermarkInfo)
		{
			base.DiagnosticsSession.TraceDebug<long>("Last event for this database: {0}", ciWatermarkInfo.LastEvent);
			long notificationWatermarkToSet;
			int lastDocumentIdIndexed;
			if (ciWatermarkInfo.FirstEvent == 1L && ciWatermarkInfo.LastEvent < (long)base.Config.MaxEventsToBeHandledByNotificationFeeder)
			{
				notificationWatermarkToSet = ciWatermarkInfo.FirstEvent;
				lastDocumentIdIndexed = int.MaxValue;
			}
			else
			{
				if (!this.MdbInfo.PreferredActiveCopy && (!ExEnvironment.IsTest || !base.Config.AllowCrawlOnPassiveForTest))
				{
					base.DiagnosticsSession.TraceDebug("Not mounted on the preferred active server. Initiating reseed.", new object[0]);
					StringBuilder stringBuilder = new StringBuilder();
					if (this.MdbInfo.DatabaseCopies != null)
					{
						foreach (MdbCopy mdbCopy in this.MdbInfo.DatabaseCopies)
						{
							stringBuilder.AppendFormat("({0},{1})", mdbCopy.Name, mdbCopy.ActivationPreference);
						}
					}
					base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "For Mdb [{0}] - ActivationPreference: {1}; ActiveServer: {2}; HostingServers: {3}", new object[]
					{
						this.MdbInfo,
						this.MdbInfo.ActivationPreference,
						this.MdbInfo.OwningServer,
						stringBuilder.ToString()
					});
					throw new CatalogReseedException(Strings.ReseedOnPassiveServer(this.MdbInfo.ToString()), IndexStatusErrorCode.CrawlOnNonPreferredActiveWithTooManyNotificationEvents);
				}
				notificationWatermarkToSet = ciWatermarkInfo.LastEvent;
				lastDocumentIdIndexed = -1;
			}
			this.MdbInfo.CatalogVersion = VersionInfo.FromVersions(this.MdbInfo.MaxSupportedVersion, this.MdbInfo.MaxSupportedVersion);
			this.watermarkInitializationCounter.Increment();
			this.watermarkStorage.BeginSetVersionInfo(this.MdbInfo.CatalogVersion, new AsyncCallback(this.EndSetVersionInfo), null);
			this.UpdateIndexStatus(UpdateIndexStatusSource.FeedingStarts);
			CrawlerMailboxIterator crawlerMailboxIterator = Factory.Current.CreateCrawlerMailboxIterator(this.MdbInfo);
			this.watermarkInitializationCounter.Increment();
			InterlockedCounter interlockedCounter = InterlockedCounter.Create(1, (int value) => value == 0, delegate()
			{
				this.watermarkStorage.BeginSetNotificationsWatermark(notificationWatermarkToSet, new AsyncCallback(this.EndSetNotificationWatermark), null);
			});
			foreach (MailboxInfo mailboxInfo in crawlerMailboxIterator.GetMailboxes())
			{
				interlockedCounter.Increment();
				this.watermarkStorage.BeginSetMailboxCrawlerState(new MailboxCrawlerState(mailboxInfo.MailboxNumber, lastDocumentIdIndexed, 0), new AsyncCallback(this.EndSetMailboxCrawlerState), new SearchFeedingController.EndSetMailboxCrawlerStateParameters(mailboxInfo, interlockedCounter));
			}
			interlockedCounter.Decrement();
		}

		private void EndSetMailboxCrawlerState(IAsyncResult ar)
		{
			SearchFeedingController.EndSetMailboxCrawlerStateParameters endSetMailboxCrawlerStateParameters = (SearchFeedingController.EndSetMailboxCrawlerStateParameters)ar.AsyncState;
			try
			{
				this.watermarkStorage.EndSetMailboxCrawlerState(ar);
				endSetMailboxCrawlerStateParameters.InterlockedCounter.Decrement();
			}
			catch (ComponentException ex)
			{
				base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Failures, "Failed to initialize crawler watermark for {0}/{1}, exception: {2}", new object[]
				{
					endSetMailboxCrawlerStateParameters.MailboxInfo.MailboxGuid,
					this.MdbInfo,
					ex
				});
				base.CompleteExecute(ex);
			}
		}

		private void EndSetNotificationWatermark(IAsyncResult ar)
		{
			try
			{
				this.watermarkStorage.EndSetNotificationsWatermark(ar);
				this.UpdateIndexStatus(UpdateIndexStatusSource.FeedingStarts);
				this.watermarkInitializationCounter.Decrement();
				IndexManager.Instance.FlushCatalog(this.IndexSystemName);
			}
			catch (ComponentException ex)
			{
				base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Failures, "Failed to initialize notification watermark for {0}, exception: {1}", new object[]
				{
					this.MdbInfo,
					ex
				});
				base.CompleteExecute(ex);
			}
		}

		private void EndSetVersionInfo(IAsyncResult ar)
		{
			try
			{
				this.watermarkStorage.EndSetVersionInfo(ar);
				this.watermarkInitializationCounter.Decrement();
			}
			catch (ComponentException ex)
			{
				base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Failures, "Failed to bump versioning info for {0}, exception: {1}", new object[]
				{
					this.MdbInfo,
					ex
				});
				base.CompleteExecute(ex);
			}
		}

		private readonly MdbPerfCountersInstance mdbFeedingPerfCounters;

		private readonly HashSet<IFeeder> feeders;

		private readonly object feedersLock = new object();

		private readonly FastIndexStatus fastIndexStatus;

		private readonly IDocumentTracker documentTracker;

		private readonly IIndexStatusStore indexStatusStore;

		private ISubmitDocument fastFeeder;

		private IWatermarkStorage watermarkStorage;

		private IFailedItemStorage failedItemStorage;

		private ManualResetEvent doneDisposingChildrenEvent = new ManualResetEvent(true);

		private InterlockedCounter.EventCounter watermarkInitializationCounter;

		private InterlockedCounter upgradeInitializationCounter;

		private VersionInfo upgradeVersion;

		private class EndSetMailboxCrawlerStateParameters
		{
			public EndSetMailboxCrawlerStateParameters(MailboxInfo mailboxInfo, InterlockedCounter counter)
			{
				this.MailboxInfo = mailboxInfo;
				this.InterlockedCounter = counter;
			}

			public MailboxInfo MailboxInfo { get; private set; }

			public InterlockedCounter InterlockedCounter { get; private set; }
		}
	}
}
