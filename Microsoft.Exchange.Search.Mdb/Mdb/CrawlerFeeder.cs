using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.EventLog;
using Microsoft.Exchange.Search.OperatorSchema;
using Microsoft.Exchange.Search.Performance;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Search.Mdb
{
	internal class CrawlerFeeder : Executable, IFeeder, IExecutable, IDiagnosable, IDisposable
	{
		public CrawlerFeeder(MdbPerfCountersInstance mdbFeedingPerfCounters, MdbInfo mdbInfo, ISearchServiceConfig config, CrawlerMailboxIterator mailboxIterator, ICrawlerItemIterator<int> itemIterator, IWatermarkStorage stateStorage, IFailedItemStorage failedItemStorage, ISubmitDocument indexFeeder, IIndexStatusStore indexStatusStore) : base(config)
		{
			Util.ThrowOnNullArgument(mdbFeedingPerfCounters, "mdbFeedingPerfCounters");
			Util.ThrowOnNullArgument(mdbInfo, "mdbInfo");
			Util.ThrowOnNullArgument(config, "config");
			Util.ThrowOnNullArgument(mailboxIterator, "mailboxIterator");
			Util.ThrowOnNullArgument(itemIterator, "itemIterator");
			Util.ThrowOnNullArgument(stateStorage, "stateStorage");
			Util.ThrowOnNullArgument(indexFeeder, "indexFeeder");
			Util.ThrowOnNullArgument(indexStatusStore, "indexStatusStore");
			this.crawlingActivityId = Guid.NewGuid();
			base.DiagnosticsSession.ComponentName = "CrawlerFeeder";
			base.DiagnosticsSession.Tracer = ExTraceGlobals.MdbCrawlerFeederTracer;
			this.mailboxIterator = mailboxIterator;
			this.itemIterator = itemIterator;
			this.stateStorage = stateStorage;
			this.failedItemStorage = failedItemStorage;
			this.indexFeeder = indexFeeder;
			this.indexStatusStore = indexStatusStore;
			this.mdbFeedingPerfCounters = mdbFeedingPerfCounters;
			this.MdbInfo = mdbInfo;
			this.documentQueueManager = new QueueManager<MdbItemIdentity>(base.Config.QueueSize, base.Config.QueueSize, null);
			this.watermarkManager = new CrawlerWatermarkManager(base.Config.QueueSize);
			this.loopDelay = base.Config.CrawlerRateLoopDelay;
			base.DiagnosticsSession.SetCounterRawValue(this.mdbFeedingPerfCounters.MailboxesLeftToCrawl, 0L);
		}

		internal event EventHandler Failed;

		public FeederType FeederType
		{
			get
			{
				return FeederType.Crawler;
			}
		}

		internal Dictionary<int, List<MailboxCrawlerState>> PendingWatermarkUpdates
		{
			get
			{
				return this.pendingWatermarkUpdates;
			}
		}

		internal HashSet<int> PendingWatermarkDeletes
		{
			get
			{
				return this.pendingWatermarkDeletes;
			}
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<CrawlerFeeder>(this);
		}

		public override XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement diagnosticInfo = base.GetDiagnosticInfo(parameters);
			diagnosticInfo.Add(new XElement("MailboxesLeftToCrawl", (int)this.mdbFeedingPerfCounters.MailboxesLeftToCrawl.RawValue));
			diagnosticInfo.Add(new XElement("MailboxesLeftToRecrawl", (int)this.mdbFeedingPerfCounters.MailboxesLeftToRecrawl.RawValue));
			return diagnosticInfo;
		}

		internal void FinishSetMailboxCrawlerState(IAsyncResult ar)
		{
			MailboxCrawlerState mailboxCrawlerState = (MailboxCrawlerState)ar.AsyncState;
			bool flag = base.TryRunUnderExceptionHandler(delegate()
			{
				this.stateStorage.EndSetMailboxCrawlerState(ar);
			}, CrawlerFeeder.ErrorAccessingStateStorage);
			bool flag2 = false;
			lock (this.pendingWatermarkUpdatesLock)
			{
				if (this.pendingWatermarkUpdates.ContainsKey(mailboxCrawlerState.MailboxNumber))
				{
					List<MailboxCrawlerState> list = this.pendingWatermarkUpdates[mailboxCrawlerState.MailboxNumber];
					list.Remove(mailboxCrawlerState);
					if (list.Count > 0)
					{
						return;
					}
				}
				if (this.pendingWatermarkDeletes.Remove(mailboxCrawlerState.MailboxNumber) && mailboxCrawlerState.RawState != 2147483647)
				{
					flag2 = true;
				}
				if (mailboxCrawlerState.RawState == 2147483647)
				{
					this.pendingWatermarkUpdates.Remove(mailboxCrawlerState.MailboxNumber);
				}
			}
			if (flag && flag2)
			{
				this.UpdateMailboxCrawlerWatermark(mailboxCrawlerState.MailboxNumber, int.MaxValue, 0);
			}
		}

		protected override void InternalExecutionStart()
		{
			using (IDisposable disposable = this.activeThreadCount.AcquireReference())
			{
				if (disposable != null)
				{
					ICollection<MailboxCrawlerState> mailboxesForCrawling = this.stateStorage.GetMailboxesForCrawling();
					int num;
					int num2;
					int num3;
					bool flag;
					bool flag2;
					this.GetMailboxCrawlCount(mailboxesForCrawling, out num, out num2, out num3, out flag, out flag2);
					this.mailboxCrawlerStates = new Dictionary<int, MailboxCrawlerState>(flag2 ? num : (num2 + num3));
					foreach (MailboxCrawlerState mailboxCrawlerState in mailboxesForCrawling)
					{
						if (flag2 && !mailboxCrawlerState.RecrawlMailbox && !this.MdbInfo.CatalogVersion.IsUpgrading)
						{
							this.mailboxCrawlerStates.Add(mailboxCrawlerState.MailboxNumber, mailboxCrawlerState);
						}
						else if (!flag2 && (mailboxCrawlerState.RecrawlMailbox || (this.MdbInfo.CatalogVersion.IsUpgrading && base.Config.SchemaUpgradingEnabled)))
						{
							this.mailboxCrawlerStates.Add(mailboxCrawlerState.MailboxNumber, mailboxCrawlerState);
						}
					}
					this.throttlingManager = Factory.Current.CreateFeederRateThrottlingManager(base.Config, this.MdbInfo, flag2 ? FeederRateThrottlingManager.ThrottlingRateExecutionType.Fast : FeederRateThrottlingManager.ThrottlingRateExecutionType.LowResource);
					if (flag)
					{
						base.DiagnosticsSession.LogPeriodicEvent(MSExchangeFastSearchEventLogConstants.Tuple_SuspendSchemaUpdate, this.MdbInfo.Name, new object[]
						{
							this.MdbInfo,
							this.MdbInfo.CatalogVersion,
							VersionInfo.Latest
						});
					}
					base.DiagnosticsSession.SetCounterRawValue(this.mdbFeedingPerfCounters.MailboxesLeftToCrawl, (long)(num + num3));
					base.DiagnosticsSession.SetCounterRawValue(this.mdbFeedingPerfCounters.MailboxesLeftToRecrawl, (long)num2);
					if (num == 0 && num2 == 0 && num3 == 0)
					{
						base.DiagnosticsSession.TraceDebug<MdbInfo>("(MDB {0}): No mailboxes to crawl.", this.MdbInfo);
						if (this.MdbInfo.CatalogVersion.IsUpgrading && !flag)
						{
							this.FinalizeSchemaUpdate();
						}
						lock (CrawlerFeeder.lastIdleLoggingTime)
						{
							DateTime d;
							if (!CrawlerFeeder.lastIdleLoggingTime.TryGetValue(this.MdbInfo.Name, out d) || DateTime.UtcNow - d > base.Config.CrawlerLoggingInterval)
							{
								CrawlerFeeder.lastIdleLoggingTime[this.MdbInfo.Name] = DateTime.UtcNow;
								base.DiagnosticsSession.LogCrawlerInfo(DiagnosticsLoggingTag.Informational, CrawlerFeeder.CrawlerEvent.NoMailboxesToCrawl.ToString(), this.MdbInfo.Name, null, "{0}:{1}", new object[]
								{
									this.MdbInfo.CatalogVersion.FeedingVersion,
									this.MdbInfo.CatalogVersion.QueryVersion
								});
							}
						}
						base.CompleteExecute(null);
					}
					else
					{
						bool flag4 = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Search.RequireMountedForCrawl.Enabled ? (!this.MdbInfo.MountedOnLocalServer) : (!this.MdbInfo.PreferredActiveCopy);
						if (num > 0 && flag4 && (!ExEnvironment.IsTest || !base.Config.AllowCrawlOnPassiveForTest))
						{
							base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Warnings, "Crawling on passive copy and the catalog needs reseed for mdb {0}", new object[]
							{
								this.MdbInfo
							});
							base.CompleteExecute(new CatalogReseedException(IndexStatusErrorCode.CrawlOnPassive));
						}
						else
						{
							this.UpdateCrawlingStatus();
							this.documentEnumerator = this.GetDocuments().GetEnumerator();
							if (!base.Stopping)
							{
								this.MoveToNextDocument();
								this.CollectDocumentsAtRateStart();
							}
						}
					}
				}
			}
		}

		protected override void InternalExecutionFinish()
		{
			this.activeThreadCount.DisableAddRef();
		}

		protected override void Dispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				if (!this.activeThreadCount.TryWaitForZero(base.Config.MaxOperationTimeout))
				{
					base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Warnings, "Waiting for threads to stop: Did not shut down in a timely manner.", new object[0]);
				}
				if (this.documentEnumerator != null)
				{
					this.documentEnumerator.Dispose();
					this.documentEnumerator = null;
				}
			}
			base.Dispose(calledFromDispose);
		}

		protected virtual StoreSession GetMailboxSession(MailboxInfo mailbox)
		{
			return XsoUtil.TranslateXsoExceptionsWithReturnValue<StoreSession>(base.DiagnosticsSession, Strings.ConnectionToMailboxFailed(mailbox.MailboxGuid), XsoUtil.XsoExceptionHandlingFlags.DoNotExpectObjectNotFound | XsoUtil.XsoExceptionHandlingFlags.DoNotExpectCorruptData, delegate()
			{
				ExchangePrincipal exchangePrincipal;
				try
				{
					exchangePrincipal = XsoUtil.GetExchangePrincipal(this.Config, this.MdbInfo, mailbox.MailboxGuid);
				}
				catch (ObjectNotFoundException ex)
				{
					this.DiagnosticsSession.TraceDebug<MdbInfo, MailboxInfo, ObjectNotFoundException>("(MDB {0}): Skipping mailbox: {1}, reason: {2}", this.MdbInfo, mailbox, ex);
					this.DiagnosticsSession.LogCrawlerInfo(DiagnosticsLoggingTag.Informational, CrawlerFeeder.CrawlerEvent.MailboxSkipped.ToString(), this.MdbInfo.Name, mailbox.MailboxGuid.ToString(), ex.ToString(), new object[0]);
					return null;
				}
				catch (MailboxInfoStaleException ex2)
				{
					this.DiagnosticsSession.TraceDebug<MdbInfo, MailboxInfo, MailboxInfoStaleException>("(MDB {0}): Skipping mailbox: {1}, reason: {2}", this.MdbInfo, mailbox, ex2);
					this.DiagnosticsSession.LogCrawlerInfo(DiagnosticsLoggingTag.Informational, CrawlerFeeder.CrawlerEvent.MailboxSkippedInfoStale.ToString(), this.MdbInfo.Name, mailbox.MailboxGuid.ToString(), ex2.ToString(), new object[0]);
					return null;
				}
				Exception ex3 = null;
				try
				{
					return XsoUtil.GetStoreSession(this.Config, exchangePrincipal, mailbox.IsPublicFolderMailbox, "Client=CI;Action=CrawlerFeeder");
				}
				catch (MailboxUnavailableException ex4)
				{
					ex3 = ex4;
				}
				catch (AccountDisabledException ex5)
				{
					ex3 = ex5;
				}
				catch (WrongServerException ex6)
				{
					ex3 = ex6;
				}
				catch (ObjectNotFoundException ex7)
				{
					ex3 = ex7;
				}
				catch (MailboxInfoStaleException ex8)
				{
					if (!this.Config.ReadFromPassiveEnabled || this.MdbInfo.IsLagCopy)
					{
						throw;
					}
					this.DiagnosticsSession.TraceDebug<MdbInfo, MailboxInfo, MailboxInfoStaleException>("(MDB {0}): Skipping mailbox: {1}, reason: {2}", this.MdbInfo, mailbox, ex8);
					ex3 = ex8;
				}
				this.DiagnosticsSession.LogCrawlerInfo(DiagnosticsLoggingTag.Informational, CrawlerFeeder.CrawlerEvent.MailboxSkippedUnavailable.ToString(), this.MdbInfo.Name, mailbox.MailboxGuid.ToString(), ex3.ToString(), new object[0]);
				return null;
			});
		}

		private IEnumerable<MdbItemIdentity> GetDocuments()
		{
			List<MailboxInfo> mailboxList = new List<MailboxInfo>();
			Dictionary<int, MailboxInfo> mailboxesInStore = new Dictionary<int, MailboxInfo>();
			foreach (MailboxInfo mailboxInfo in this.mailboxIterator.GetMailboxes())
			{
				mailboxList.Add(mailboxInfo);
				mailboxesInStore.Add(mailboxInfo.MailboxNumber, mailboxInfo);
			}
			foreach (int num in this.mailboxCrawlerStates.Keys)
			{
				if (base.Stopping)
				{
					yield break;
				}
				if (!mailboxesInStore.ContainsKey(num))
				{
					this.UpdateMailboxCrawlerWatermark(num, int.MaxValue, 0);
				}
			}
			foreach (MailboxInfo mailbox in mailboxList)
			{
				if (base.Stopping)
				{
					yield break;
				}
				MailboxCrawlerState mailboxCrawlerState;
				if (!this.mailboxCrawlerStates.TryGetValue(mailbox.MailboxNumber, out mailboxCrawlerState))
				{
					base.DiagnosticsSession.LogCrawlerInfo(DiagnosticsLoggingTag.Informational, CrawlerFeeder.CrawlerEvent.MailboxSkippedRemoved.ToString(), this.MdbInfo.Name, mailbox.MailboxGuid.ToString(), string.Empty, new object[0]);
				}
				else
				{
					base.DiagnosticsSession.TraceDebug<MdbInfo, MailboxInfo>("(MDB {0}): Starting to crawl mailbox: {1}", this.MdbInfo, mailbox);
					FailedItemParameters failedItemParameters = new FailedItemParameters(FailureMode.Permanent, FieldSet.None)
					{
						MailboxGuid = new Guid?(mailbox.MailboxGuid)
					};
					long failedItemsCount = this.TryGetFailedDocumentsCount(failedItemParameters);
					long itemsCount = this.TryGetItemsCount(mailbox.MailboxGuid);
					base.DiagnosticsSession.LogCrawlerInfo(DiagnosticsLoggingTag.Informational, CrawlerFeeder.CrawlerEvent.MailboxStarted.ToString(), this.MdbInfo.Name, mailbox.MailboxGuid.ToString(), "{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}:{10}", new object[]
					{
						mailbox.IsArchive,
						mailbox.IsPublicFolderMailbox,
						mailbox.IsSharedMailbox,
						mailbox.IsTeamSiteMailbox,
						mailbox.IsModernGroupMailbox,
						this.MdbInfo.CatalogVersion.QueryVersion,
						this.MdbInfo.CatalogVersion.FeedingVersion,
						this.MdbInfo.CatalogVersion.IsUpgrading,
						this.mdbFeedingPerfCounters.MailboxesLeftToCrawl.RawValue,
						failedItemsCount,
						itemsCount
					});
					using (StoreSession session = this.GetMailboxSession(mailbox))
					{
						if (session != null)
						{
							goto IL_474;
						}
						int num2 = mailboxCrawlerState.AttemptCount + 1;
						int num3 = (num2 < MailboxCrawlerState.MaxCrawlAttemptCount) ? -3 : -4;
						base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "Failed to get a session for Mailbox: {0}, Attempt Count: {1}, MailboxCrawlState: {2}, MailboxGuid: {3}", new object[]
						{
							mailbox.DisplayName,
							num2,
							(num3 == -4) ? "PermanentFailure" : "RetriableFailure",
							mailbox.MailboxGuid
						});
						this.UpdateMailboxCrawlerWatermark(mailbox.MailboxNumber, num3, num2);
					}
					continue;
					IL_474:
					MdbItemIdentity lastItem = null;
					StoreSession session;
					foreach (MdbItemIdentity item in this.itemIterator.GetItems(session, mailboxCrawlerState.LastDocumentIdIndexed, int.MaxValue))
					{
						lastItem = item;
						yield return item;
					}
					if (lastItem != null)
					{
						base.DiagnosticsSession.TraceDebug<MdbInfo, int, int>("(MDB {0}): Mailbox ({1}) is done. Mark the last document id = {2}", this.MdbInfo, mailbox.MailboxNumber, lastItem.DocumentId);
						this.watermarkManager.SetLast(mailbox.MailboxNumber, lastItem.DocumentId);
						long num4 = this.TryGetFailedDocumentsCount(failedItemParameters);
						failedItemParameters.FailureMode = FailureMode.Transient;
						long num5 = this.TryGetFailedDocumentsCount(failedItemParameters);
						base.DiagnosticsSession.LogCrawlerInfo(DiagnosticsLoggingTag.Informational, CrawlerFeeder.CrawlerEvent.MailboxCompleted.ToString(), this.MdbInfo.Name, mailbox.MailboxGuid.ToString(), "{0}:{1}", new object[]
						{
							num4,
							num5
						});
					}
					else
					{
						base.DiagnosticsSession.TraceDebug<MdbInfo, int>("(MDB {0}): Mailbox ({1}) is empty. Mark state completed", this.MdbInfo, mailbox.MailboxNumber);
						this.UpdateMailboxCrawlerWatermark(mailbox.MailboxNumber, int.MaxValue, 0);
						base.DiagnosticsSession.LogCrawlerInfo(DiagnosticsLoggingTag.Informational, CrawlerFeeder.CrawlerEvent.MailboxCompletedEmpty.ToString(), this.MdbInfo.Name, mailbox.MailboxGuid.ToString(), string.Empty, new object[0]);
					}
				}
			}
			yield break;
		}

		private bool MoveToNextDocument()
		{
			bool result;
			using (ExPerfTrace.RelatedActivity(this.crawlingActivityId))
			{
				bool flag;
				if (!base.TryRunUnderExceptionHandler<bool>(() => this.documentEnumerator.MoveNext(), out flag, CrawlerFeeder.FailedToCrawlMailbox))
				{
					this.currentDocument = null;
					result = false;
				}
				else
				{
					this.currentDocument = (flag ? this.documentEnumerator.Current : null);
					result = flag;
				}
			}
			return result;
		}

		private bool CollectDocuments(ref int documentsToQueue)
		{
			bool flag = false;
			int num = documentsToQueue;
			bool result;
			using (IDisposable disposable = this.activeThreadCount.AcquireReference())
			{
				if (disposable == null)
				{
					result = flag;
				}
				else
				{
					MdbItemIdentity mdbItemIdentity = null;
					try
					{
						while (!base.Stopping && documentsToQueue != 0)
						{
							mdbItemIdentity = this.currentDocument;
							if (mdbItemIdentity == null)
							{
								break;
							}
							if (!this.documentQueueManager.Enqueue(mdbItemIdentity))
							{
								base.DiagnosticsSession.TraceDebug<MdbInfo>("(MDB {0}):Document queue is full", this.MdbInfo);
								break;
							}
							documentsToQueue--;
							if (!this.MoveToNextDocument())
							{
								base.DiagnosticsSession.TraceDebug<MdbInfo>("(MDB {0}): No more documents", this.MdbInfo);
								break;
							}
						}
					}
					finally
					{
						documentsToQueue = num - documentsToQueue;
						bool crawlerFeederCollectDocumentsVerifyPendingWatermarks = base.Config.CrawlerFeederCollectDocumentsVerifyPendingWatermarks;
						bool flag2 = false;
						if (crawlerFeederCollectDocumentsVerifyPendingWatermarks)
						{
							lock (this.pendingWatermarkUpdates)
							{
								foreach (List<MailboxCrawlerState> list in this.pendingWatermarkUpdates.Values)
								{
									if (list.Count != 0)
									{
										flag2 = true;
										break;
									}
								}
								flag2 |= (this.pendingWatermarkDeletes.Count != 0);
							}
						}
						if (num != 0 && mdbItemIdentity == null && this.documentQueueManager.Length == 0 && this.documentQueueManager.OutstandingLength == 0 && (!crawlerFeederCollectDocumentsVerifyPendingWatermarks || (crawlerFeederCollectDocumentsVerifyPendingWatermarks && !flag2)))
						{
							base.DiagnosticsSession.TraceDebug<MdbInfo>("(MDB {0}): Crawling has completed", this.MdbInfo);
							base.DiagnosticsSession.LogCrawlerInfo(DiagnosticsLoggingTag.Informational, CrawlerFeeder.CrawlerEvent.CrawlCompleted.ToString(), this.MdbInfo.Name, null, "{0}:{1}:{2}", new object[]
							{
								this.mdbFeedingPerfCounters.MailboxesLeftToCrawl.RawValue,
								this.MdbInfo.CatalogVersion.FeedingVersion,
								this.MdbInfo.CatalogVersion.QueryVersion
							});
							base.CompleteExecute("CrawlComplete");
							flag = true;
						}
						else
						{
							this.SendDocuments();
						}
					}
					result = flag;
				}
			}
			return result;
		}

		private void SendDocuments()
		{
			lock (this.documentQueueManager)
			{
				IEnumerable<MdbItemIdentity> enumerable;
				if (this.documentQueueManager.Dequeue(out enumerable))
				{
					foreach (MdbItemIdentity mdbItemIdentity in enumerable)
					{
						if (base.Stopping)
						{
							this.documentQueueManager.Remove(mdbItemIdentity);
						}
						else
						{
							if (this.mdbFeedingPerfCounters != null)
							{
								base.DiagnosticsSession.IncrementCounter(this.mdbFeedingPerfCounters.NumberOfDocumentsSentForProcessingCrawler);
							}
							this.watermarkManager.Add(mdbItemIdentity);
							IFastDocument fastDocument = this.indexFeeder.CreateFastDocument(DocumentOperation.Insert);
							this.indexFeeder.DocumentHelper.PopulateFastDocumentForIndexing(fastDocument, this.MdbInfo.CatalogVersion.FeedingVersion, mdbItemIdentity.MailboxGuid, mdbItemIdentity.MailboxNumber, false, !this.MdbInfo.IsLagCopy, mdbItemIdentity.DocumentId, mdbItemIdentity);
							try
							{
								this.indexFeeder.BeginSubmitDocument(fastDocument, new AsyncCallback(this.DocumentCompleteCallback), mdbItemIdentity);
							}
							catch (ObjectDisposedException result)
							{
								base.DiagnosticsSession.TraceError<MdbInfo>("(MDB {0}): FastFeeder has been disposed", this.MdbInfo);
								base.CompleteExecute(result);
								break;
							}
						}
					}
				}
			}
		}

		private void DocumentCompleteCallback(IAsyncResult asyncResult)
		{
			using (ExPerfTrace.RelatedActivity(this.crawlingActivityId))
			{
				using (IDisposable disposable = this.activeThreadCount.AcquireReference())
				{
					MdbItemIdentity mdbItemIdentity = (MdbItemIdentity)asyncResult.AsyncState;
					try
					{
						if (!this.indexFeeder.EndSubmitDocument(asyncResult))
						{
							base.CompleteExecute(null);
							return;
						}
						if (this.mdbFeedingPerfCounters != null)
						{
							base.DiagnosticsSession.IncrementCounter(this.mdbFeedingPerfCounters.NumberOfDocumentsIndexedCrawler);
							base.DiagnosticsSession.IncrementCounter(this.mdbFeedingPerfCounters.NumberOfDocumentsProcessed);
						}
					}
					catch (Exception ex)
					{
						EventHandler failed = this.Failed;
						if (failed != null)
						{
							failed(this, new EventArgs());
						}
						base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Failures, "Document failure: ID: {0}, Failure: {1}", new object[]
						{
							mdbItemIdentity,
							ex
						});
						base.CompleteExecute(ex);
						return;
					}
					if (disposable != null)
					{
						try
						{
							MailboxCrawlerState mailboxCrawlerState;
							if (this.watermarkManager.TryComplete(mdbItemIdentity, out mailboxCrawlerState))
							{
								this.UpdateMailboxCrawlerWatermark(mailboxCrawlerState.MailboxNumber, mailboxCrawlerState.LastDocumentIdIndexed, 0);
							}
						}
						finally
						{
							this.documentQueueManager.Remove(mdbItemIdentity);
						}
					}
				}
			}
		}

		private void CollectDocumentsAtRate(object state, bool timerFired)
		{
			if (!timerFired)
			{
				return;
			}
			using (ExPerfTrace.RelatedActivity(this.crawlingActivityId))
			{
				ExDateTime utcNow = ExDateTime.UtcNow;
				TimeSpan timeSpan = utcNow.Subtract(this.lastCollectionTime);
				int num = (this.rate > 0.0) ? Math.Max((int)(timeSpan.Duration().TotalSeconds * this.rate), 1) : 0;
				if (this.mdbFeedingPerfCounters != null)
				{
					base.DiagnosticsSession.SetCounterRawValue(this.mdbFeedingPerfCounters.AverageAttemptedCrawlerRate, (long)((double)num / timeSpan.Duration().TotalSeconds));
				}
				bool flag = this.CollectDocuments(ref num);
				if (this.mdbFeedingPerfCounters != null)
				{
					base.DiagnosticsSession.SetCounterRawValue(this.mdbFeedingPerfCounters.AverageCrawlerRate, (long)((double)num / timeSpan.Duration().TotalSeconds));
				}
				this.lastCollectionTime = utcNow;
				this.rate = this.throttlingManager.ThrottlingRateContinue(this.rate);
				if (!flag)
				{
					try
					{
						RegisteredWaitHandleWrapper.RegisterWaitForSingleObject(base.StopEvent, CallbackWrapper.WaitOrTimerCallback(new WaitOrTimerCallback(this.CollectDocumentsAtRate)), state, this.loopDelay.Duration(), true);
					}
					catch (ObjectDisposedException result)
					{
						base.DiagnosticsSession.TraceError<MdbInfo>("(MDB {0}): CrawlerFeeder has been disposed", this.MdbInfo);
						base.CompleteExecute(result);
					}
				}
			}
		}

		private void CollectDocumentsAtRateStart()
		{
			this.lastCollectionTime = ExDateTime.UtcNow.Subtract(this.loopDelay);
			this.rate = this.throttlingManager.ThrottlingRateStart();
			RegisteredWaitHandleWrapper.RegisterWaitForSingleObject(base.StopEvent, CallbackWrapper.WaitOrTimerCallback(new WaitOrTimerCallback(this.CollectDocumentsAtRate)), null, this.loopDelay.Duration(), true);
		}

		private void UpdateMailboxCrawlerWatermark(int mailboxNumber, int mailboxState, int attemptCount = 0)
		{
			MailboxCrawlerState mailboxCrawlerState = new MailboxCrawlerState(mailboxNumber, mailboxState, attemptCount);
			lock (this.pendingWatermarkUpdatesLock)
			{
				if (mailboxState == 2147483647)
				{
					if (this.pendingWatermarkUpdates.ContainsKey(mailboxNumber) && this.pendingWatermarkUpdates[mailboxNumber].Count > 0)
					{
						this.pendingWatermarkDeletes.Add(mailboxNumber);
						return;
					}
				}
				else if (this.pendingWatermarkUpdates.ContainsKey(mailboxNumber))
				{
					this.pendingWatermarkUpdates[mailboxNumber].Add(mailboxCrawlerState);
				}
				else
				{
					this.pendingWatermarkUpdates.Add(mailboxNumber, new List<MailboxCrawlerState>(1)
					{
						mailboxCrawlerState
					});
				}
			}
			Exception objectDiposedExceptionInsideHandler = null;
			bool flag2 = base.TryRunUnderExceptionHandler(delegate()
			{
				try
				{
					this.stateStorage.BeginSetMailboxCrawlerState(mailboxCrawlerState, new AsyncCallback(this.FinishSetMailboxCrawlerState), mailboxCrawlerState);
				}
				catch (ObjectDisposedException objectDiposedExceptionInsideHandler)
				{
					objectDiposedExceptionInsideHandler = objectDiposedExceptionInsideHandler;
				}
			}, CrawlerFeeder.ErrorAccessingStateStorage);
			if (objectDiposedExceptionInsideHandler == null)
			{
				if (flag2 && (mailboxState == 2147483647 || mailboxState == -4) && this.mailboxCrawlerStates.ContainsKey(mailboxNumber))
				{
					base.DiagnosticsSession.DecrementCounter(this.mdbFeedingPerfCounters.MailboxesLeftToCrawl);
					if (!this.mailboxCrawlerStates[mailboxNumber].RecrawlMailbox)
					{
						this.UpdateCrawlingStatus();
					}
				}
				return;
			}
			base.DiagnosticsSession.TraceError<MdbInfo>("(MDB {0}): CrawlerFeeder has been disposed", this.MdbInfo);
			base.CompleteExecute(objectDiposedExceptionInsideHandler);
		}

		private long TryGetFailedDocumentsCount(FailedItemParameters parameters)
		{
			long result;
			try
			{
				result = this.failedItemStorage.GetFailedItemsCount(parameters);
			}
			catch (Exception)
			{
				result = -1L;
			}
			return result;
		}

		private long TryGetItemsCount(Guid mailboxGuid)
		{
			long result;
			try
			{
				result = this.failedItemStorage.GetItemsCount(mailboxGuid);
			}
			catch (Exception)
			{
				result = -1L;
			}
			return result;
		}

		private void FinalizeSchemaUpdate()
		{
			if (base.TryRunUnderExceptionHandler(delegate()
			{
				this.stateStorage.BeginSetVersionInfo(VersionInfo.Latest, new AsyncCallback(this.FinishSetVersionInfo), null);
			}, CrawlerFeeder.ErrorAccessingStateStorage))
			{
				base.DiagnosticsSession.LogPeriodicEvent(MSExchangeFastSearchEventLogConstants.Tuple_FinishSchemaUpdate, this.MdbInfo.Name, new object[]
				{
					this.MdbInfo,
					VersionInfo.Latest
				});
			}
		}

		private void FinishSetVersionInfo(IAsyncResult ar)
		{
			base.TryRunUnderExceptionHandler(delegate()
			{
				this.stateStorage.EndSetVersionInfo(ar);
			}, CrawlerFeeder.ErrorAccessingStateStorage);
		}

		private void GetMailboxCrawlCount(ICollection<MailboxCrawlerState> mailboxesToCrawl, out int crawlCount, out int recrawlCount, out int upgradeCount, out bool suspendSchemaUpgrade, out bool crawling)
		{
			crawlCount = 0;
			recrawlCount = 0;
			upgradeCount = 0;
			suspendSchemaUpgrade = false;
			crawling = false;
			foreach (MailboxCrawlerState mailboxCrawlerState in mailboxesToCrawl)
			{
				if (mailboxCrawlerState.RecrawlMailbox)
				{
					recrawlCount++;
				}
				else if (!this.MdbInfo.CatalogVersion.IsUpgrading)
				{
					crawlCount++;
					crawling = true;
				}
				else if (base.Config.SchemaUpgradingEnabled)
				{
					upgradeCount++;
				}
				else
				{
					suspendSchemaUpgrade = true;
				}
			}
		}

		private void UpdateCrawlingStatus()
		{
			int num = (int)this.mdbFeedingPerfCounters.MailboxesLeftToCrawl.RawValue;
			bool crawlerFeederUpdateCrawlingStatusResetCache = base.Config.CrawlerFeederUpdateCrawlingStatusResetCache;
			if (crawlerFeederUpdateCrawlingStatusResetCache && num == 0)
			{
				this.stateStorage.RefreshCachedCrawlerWatermarks();
				if (this.stateStorage.HasCrawlerWatermarks())
				{
					ICollection<MailboxCrawlerState> mailboxesForCrawling = this.stateStorage.GetMailboxesForCrawling();
					int num2;
					int num3;
					int num4;
					bool flag;
					bool flag2;
					this.GetMailboxCrawlCount(mailboxesForCrawling, out num2, out num3, out num4, out flag, out flag2);
					num = num2 + num4;
				}
			}
			if (num == 0 && base.Config.SchemaUpgradingEnabled)
			{
				this.FinalizeSchemaUpdate();
				this.MdbInfo.CatalogVersion = VersionInfo.Latest;
				base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "SchemaUpgradeComplete", new object[]
				{
					this.MdbInfo.Guid,
					this.MdbInfo.CatalogVersion
				});
			}
			IndexStatus indexStatus;
			if (num > 0)
			{
				indexStatus = this.indexStatusStore.SetIndexStatus(this.MdbInfo.Guid, num, this.MdbInfo.CatalogVersion);
			}
			else
			{
				indexStatus = this.indexStatusStore.SetIndexStatus(this.MdbInfo.Guid, ContentIndexStatusType.Healthy, IndexStatusErrorCode.Success, this.MdbInfo.CatalogVersion, null);
			}
			if (this.mdbFeedingPerfCounters != null)
			{
				base.DiagnosticsSession.SetCounterRawValue(this.mdbFeedingPerfCounters.IndexingStatus, (long)indexStatus.IndexingState);
			}
		}

		public const string CrawlComplete = "CrawlComplete";

		protected readonly MdbInfo MdbInfo;

		private static readonly LocalizedString ErrorAccessingStateStorage = Strings.ErrorAccessingStateStorage;

		private static readonly LocalizedString FailedToCrawlMailbox = Strings.FailedToCrawlMailbox;

		private static Dictionary<string, DateTime> lastIdleLoggingTime = new Dictionary<string, DateTime>();

		private readonly IWatermarkStorage stateStorage;

		private readonly IFailedItemStorage failedItemStorage;

		private readonly ISubmitDocument indexFeeder;

		private readonly CrawlerMailboxIterator mailboxIterator;

		private readonly ICrawlerItemIterator<int> itemIterator;

		private readonly MdbPerfCountersInstance mdbFeedingPerfCounters;

		private readonly QueueManager<MdbItemIdentity> documentQueueManager;

		private readonly CrawlerWatermarkManager watermarkManager;

		private readonly RefCount activeThreadCount = new RefCount();

		private readonly Guid crawlingActivityId;

		private readonly IIndexStatusStore indexStatusStore;

		private readonly TimeSpan loopDelay;

		private readonly Dictionary<int, List<MailboxCrawlerState>> pendingWatermarkUpdates = new Dictionary<int, List<MailboxCrawlerState>>();

		private readonly HashSet<int> pendingWatermarkDeletes = new HashSet<int>();

		private readonly object pendingWatermarkUpdatesLock = new object();

		private IEnumerator<MdbItemIdentity> documentEnumerator;

		private MdbItemIdentity currentDocument;

		private Dictionary<int, MailboxCrawlerState> mailboxCrawlerStates;

		private ExDateTime lastCollectionTime;

		private IFeederRateThrottlingManager throttlingManager;

		private double rate;

		private enum CrawlerEvent
		{
			NoMailboxesToCrawl = 1,
			MailboxStarted,
			MailboxSkipped,
			MailboxSkippedInfoStale,
			MailboxSkippedUnavailable,
			MailboxSkippedRemoved,
			MailboxCompleted,
			MailboxCompletedEmpty,
			CrawlCompleted
		}
	}
}
