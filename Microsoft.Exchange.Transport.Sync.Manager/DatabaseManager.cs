using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DatabaseManager
	{
		internal DatabaseManager(Guid databaseGuid, byte databaseLookupIndex)
		{
			this.coreDatabaseManager = new DatabaseManager.CoreDatabaseManager();
			this.databaseGuid = databaseGuid;
			this.enabled = false;
			this.databaseLookupIndex = databaseLookupIndex;
			this.systemMailboxName = "SystemMailbox{" + databaseGuid + "}";
			this.mailboxTableManager = new MailboxTableManager(databaseGuid);
			this.mailboxManager = new MailboxManager(databaseGuid);
			this.lastCompletedPollingStartTime = null;
			this.lastTwoWayPollingStartTime = null;
		}

		internal static DatabaseManager.MailboxCrawler MailboxCrawlerInstance
		{
			get
			{
				return DatabaseManager.mailboxCrawler;
			}
			set
			{
				DatabaseManager.mailboxCrawler = value;
			}
		}

		internal ICoreDatabaseManager CoreDatabaseManagerClass
		{
			set
			{
				this.coreDatabaseManager = value;
			}
		}

		internal ExDateTime? StartTimeOfLastTwoWayPolling
		{
			set
			{
				this.lastTwoWayPollingStartTime = value;
			}
		}

		internal ExDateTime? LastCompletedPollingStartTime
		{
			set
			{
				this.lastCompletedPollingStartTime = value;
			}
		}

		internal bool Enabled
		{
			get
			{
				return this.enabled;
			}
		}

		internal Guid DatabaseGuid
		{
			get
			{
				return this.databaseGuid;
			}
		}

		internal Guid SystemMailboxGuid
		{
			get
			{
				return this.systemMailboxGuid;
			}
			set
			{
				this.systemMailboxGuid = value;
			}
		}

		internal SubscriptionCacheManager SubscriptionCacheManager
		{
			get
			{
				return this.subscriptionCacheManager;
			}
			set
			{
				SubscriptionCacheManager subscriptionCacheManager = this.subscriptionCacheManager;
				this.subscriptionCacheManager = value;
			}
		}

		internal MailboxTableManager MailboxTableManager
		{
			get
			{
				return this.mailboxTableManager;
			}
		}

		internal byte DatabaseLookupIndex
		{
			get
			{
				return this.databaseLookupIndex;
			}
		}

		internal static void AddMailboxCrawlerDiagnosticInfoTo(XElement parentElement)
		{
			DatabaseManager.mailboxCrawler.AddBasicDiagnosticInfoTo(parentElement);
		}

		internal static void AddMailboxCrawlerQueueEntriesDiagnosticInfoTo(XElement parentElement)
		{
			DatabaseManager.mailboxCrawler.AddVerboseDiagnosticInfoTo(parentElement);
		}

		internal bool Initialize()
		{
			lock (this.syncObject)
			{
				if (this.enabled)
				{
					return true;
				}
				if (!this.InitializeSystemMailboxGuid())
				{
					return false;
				}
				if (!this.coreDatabaseManager.StartCacheManager(this))
				{
					ContentAggregationConfig.SyncLogSession.LogError((TSLID)241UL, DatabaseManager.Tracer, "Database initialize failed for database {0}. Could not start the cache manager.", new object[]
					{
						this.databaseGuid
					});
					return false;
				}
				this.mailboxPollingTimer = new GuardedTimer(new TimerCallback(this.MailboxPolling), null, ContentAggregationConfig.DelayBeforeMailboxTablePollingStarts, TimeSpan.FromMilliseconds(-1.0));
				ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)242UL, DatabaseManager.Tracer, "Database {0} has been successfully initialized for sync processing.", new object[]
				{
					this.databaseGuid
				});
				this.enabled = true;
			}
			return true;
		}

		internal void Shutdown()
		{
			lock (this.syncObject)
			{
				if (this.enabled)
				{
					this.mailboxPollingTimer.Dispose(false);
					this.mailboxPollingTimer = null;
					this.lastCompletedPollingStartTime = null;
					this.lastTwoWayPollingStartTime = null;
					this.coreDatabaseManager.ShutdownCacheManager(this);
					ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)243UL, DatabaseManager.Tracer, "Database {0} has been shutdown for sync processing.", new object[]
					{
						this.databaseGuid
					});
					this.enabled = false;
				}
			}
		}

		internal bool TryRebuildMailbox(Guid userMailboxGuid, out bool hasCacheChanged)
		{
			hasCacheChanged = false;
			ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)244UL, DatabaseManager.Tracer, Guid.Empty, userMailboxGuid, "TryRebuildMailbox called", new object[0]);
			try
			{
				this.RebuildMailbox(userMailboxGuid, out hasCacheChanged);
			}
			catch (CacheTransientException)
			{
				ContentAggregationConfig.SyncLogSession.LogError((TSLID)245UL, DatabaseManager.Tracer, Guid.Empty, userMailboxGuid, "TryRebuildMailbox: Failed due to transient exception in database {0}.", new object[]
				{
					this.databaseGuid
				});
				return false;
			}
			catch (CachePermanentException)
			{
				ContentAggregationConfig.SyncLogSession.LogError((TSLID)246UL, DatabaseManager.Tracer, Guid.Empty, userMailboxGuid, "TryRebuildMailbox: Failed due to permanent exception in database {0}.", new object[]
				{
					this.databaseGuid
				});
				return false;
			}
			if (hasCacheChanged)
			{
				ManagerPerfCounterHandler.Instance.IncrementCacheMessagesRebuildRepaired();
			}
			return true;
		}

		internal void RebuildMailbox(Guid userMailboxGuid, out bool hasCacheChanged)
		{
			hasCacheChanged = false;
			IList<AggregationSubscription> list = null;
			Guid? guid = null;
			Guid? guid2 = null;
			ExDateTime? exDateTime = null;
			lock (this.syncObject)
			{
				if (!this.enabled)
				{
					ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)100UL, DatabaseManager.Tracer, Guid.Empty, userMailboxGuid, "RebuildMailbox: Database {0} is disabled. Ignoring rebuild request.", new object[]
					{
						this.databaseGuid
					});
					return;
				}
				MailboxSession userMailboxSession = this.mailboxManager.GetUserMailboxSession(userMailboxGuid);
				if (userMailboxSession == null)
				{
					try
					{
						this.subscriptionCacheManager.DeleteCacheMessage(userMailboxGuid);
						hasCacheChanged = true;
					}
					catch (CacheNotFoundException)
					{
					}
					return;
				}
				using (userMailboxSession)
				{
					ExDateTime utcNow = ExDateTime.UtcNow;
					ExDateTime? mailboxSubscriptionListTimestamp = this.mailboxTableManager.GetMailboxSubscriptionListTimestamp(userMailboxSession);
					this.mailboxManager.CrawlUserMailbox(userMailboxSession, out list, out guid, out guid2);
					if (mailboxSubscriptionListTimestamp == null)
					{
						ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)247UL, DatabaseManager.Tracer, Guid.Empty, userMailboxGuid, "RebuildMailbox: New mailbox found in database {0}, with {1} subscriptions.", new object[]
						{
							this.databaseGuid,
							(list == null) ? 0 : list.Count
						});
						exDateTime = new ExDateTime?((list != null && list.Count > 0) ? utcNow : MailboxTableManager.MinSystemDateTimeValue);
						this.mailboxTableManager.SetMailboxSubscriptionListTimestamp(userMailboxSession, exDateTime.Value);
					}
					else
					{
						ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)248UL, DatabaseManager.Tracer, Guid.Empty, userMailboxGuid, "RebuildMailbox: Mailbox was last updated at {0} and has {1} subscriptions.", new object[]
						{
							mailboxSubscriptionListTimestamp.Value.ToString("MM/dd/yyyy hh:mm:ss.fff"),
							(list == null) ? 0 : list.Count
						});
						if (list == null || list.Count == 0)
						{
							exDateTime = new ExDateTime?(MailboxTableManager.MinSystemDateTimeValue);
							this.mailboxTableManager.SetMailboxSubscriptionListTimestamp(userMailboxSession, exDateTime.Value);
						}
						else
						{
							exDateTime = mailboxSubscriptionListTimestamp;
						}
					}
					if (list == null || list.Count == 0)
					{
						this.ConsistencyCheckCrawl(userMailboxGuid, userMailboxSession, out list);
						if (list != null && list.Count > 0)
						{
							exDateTime = new ExDateTime?(utcNow);
							this.mailboxTableManager.SetMailboxSubscriptionListTimestamp(userMailboxSession, ExDateTime.UtcNow);
						}
					}
				}
				foreach (AggregationSubscription subscription in list)
				{
					if (guid2 != null)
					{
						MrsAdapter.UpdateAndCheckMrsJob(ContentAggregationConfig.SyncLogSession, subscription, this.DatabaseGuid, guid2.Value);
					}
				}
				this.subscriptionCacheManager.UpdateCacheMessageForCrawl(list, userMailboxGuid, exDateTime.Value, guid.Value, guid2.Value, out hasCacheChanged);
			}
			if (hasCacheChanged)
			{
				ManagerPerfCounterHandler.Instance.IncrementCacheMessagesRebuildRepaired();
			}
		}

		internal bool IsUserMailbox(Guid mailboxGuid)
		{
			return mailboxGuid != this.systemMailboxGuid;
		}

		internal void ScheduleMailboxCrawl(Guid mailboxGuid)
		{
			DatabaseManager.mailboxCrawler.EnqueueMailboxCrawl(this.databaseGuid, mailboxGuid);
		}

		internal void MailboxPolling(object state)
		{
			SubscriptionCacheManager subscriptionCacheManager = null;
			lock (this.syncObject)
			{
				if (!this.enabled || this.mailboxPollingTimer == null)
				{
					return;
				}
				subscriptionCacheManager = this.SubscriptionCacheManager;
			}
			bool subscriptionQueueBuildupDone = subscriptionCacheManager.SubscriptionQueueBuildupDone;
			ExDateTime utcNow = ExDateTime.UtcNow;
			ExDateTime? baselineTime = null;
			bool flag2 = subscriptionQueueBuildupDone && this.lastTwoWayPollingStartTime != null && this.lastTwoWayPollingStartTime != null && this.lastTwoWayPollingStartTime.Value + ContentAggregationConfig.MailboxTableTwoWayPollingInterval > utcNow;
			if (flag2)
			{
				baselineTime = this.lastCompletedPollingStartTime - TimeSpan.FromSeconds(1.0);
				ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)249UL, DatabaseManager.Tracer, "Mailbox Polling: Using baseline incremental polling {0} on database {1}. StartTimeOfLastTwoWayPolling: {2}. ProcessingTime: {3}", new object[]
				{
					baselineTime.Value.ToString("MM/dd/yyyy hh:mm:ss.fff"),
					this.databaseGuid,
					(this.lastTwoWayPollingStartTime != null) ? this.lastTwoWayPollingStartTime.Value.ToString("MM/dd/yyyy hh:mm:ss.fff") : string.Empty,
					utcNow.ToString("MM/dd/yyyy hh:mm:ss.fff")
				});
			}
			else
			{
				ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)250UL, DatabaseManager.Tracer, "Mailbox Polling: Using two-way polling on database {0}. StartTimeOfLastTwoWayPolling: {1}. ProcessingTime: {2}", new object[]
				{
					this.databaseGuid,
					(this.lastTwoWayPollingStartTime != null) ? this.lastTwoWayPollingStartTime.Value.ToString("MM/dd/yyyy hh:mm:ss.fff") : "null",
					utcNow.ToString("MM/dd/yyyy hh:mm:ss.fff")
				});
			}
			Dictionary<Guid, Pair<ExDateTime?, ExDateTime?>> mailboxTableChanges = this.MailboxTableManager.GetAllMailboxes(baselineTime, this);
			if (mailboxTableChanges != null)
			{
				bool flag3 = true;
				if (!flag2)
				{
					flag3 = subscriptionCacheManager.EnumerateSubscriptionsInCache(false, delegate(Guid mailboxGuid, SubscriptionCacheMessage cacheMessage, Exception exception)
					{
						if (cacheMessage != null)
						{
							if (!mailboxTableChanges.ContainsKey(mailboxGuid))
							{
								Pair<ExDateTime?, ExDateTime?> value = new Pair<ExDateTime?, ExDateTime?>(null, new ExDateTime?(cacheMessage.SubscriptionListTimestamp));
								mailboxTableChanges[mailboxGuid] = value;
								return;
							}
							if (mailboxTableChanges[mailboxGuid].First == cacheMessage.SubscriptionListTimestamp)
							{
								mailboxTableChanges.Remove(mailboxGuid);
								return;
							}
							Pair<ExDateTime?, ExDateTime?> value2 = new Pair<ExDateTime?, ExDateTime?>(mailboxTableChanges[mailboxGuid].First, new ExDateTime?(cacheMessage.SubscriptionListTimestamp));
							mailboxTableChanges.Remove(mailboxGuid);
							mailboxTableChanges[mailboxGuid] = value2;
							return;
						}
						else
						{
							if (exception is CacheTransientException)
							{
								ContentAggregationConfig.SyncLogSession.LogError((TSLID)251UL, DatabaseManager.Tracer, "Mailbox Polling: Cache load failed transiently for mailbox {0} on database {1}. Crawl not scheduled.", new object[]
								{
									mailboxGuid,
									this.databaseGuid
								});
								mailboxTableChanges.Remove(mailboxGuid);
								return;
							}
							if (exception is CachePermanentException)
							{
								ContentAggregationConfig.SyncLogSession.LogError((TSLID)252UL, DatabaseManager.Tracer, "Mailbox Polling: Cache load failed permanently for mailbox {0} on database {1}. Crawl scheduled.", new object[]
								{
									mailboxGuid,
									this.databaseGuid
								});
							}
							return;
						}
					});
					if (!flag3)
					{
						ContentAggregationConfig.SyncLogSession.LogError((TSLID)253UL, DatabaseManager.Tracer, "Mailbox polling: Cache enumeration failed for database {0}. No crawls scheduled.", new object[]
						{
							this.databaseGuid
						});
					}
				}
				if (flag3)
				{
					this.lastCompletedPollingStartTime = new ExDateTime?(utcNow);
					if (subscriptionQueueBuildupDone && !flag2)
					{
						ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)1440UL, DatabaseManager.Tracer, "Mailbox Polling: Updating the last 2 way polling start time to: " + utcNow, new object[0]);
						this.lastTwoWayPollingStartTime = new ExDateTime?(utcNow);
					}
					ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)254UL, DatabaseManager.Tracer, "Mailbox polling: polling completed for database {0}, with {1} changes to be scheduled.", new object[]
					{
						this.databaseGuid,
						mailboxTableChanges.Count
					});
					foreach (KeyValuePair<Guid, Pair<ExDateTime?, ExDateTime?>> keyValuePair in mailboxTableChanges)
					{
						this.ScheduleMailboxCrawl(keyValuePair.Key);
					}
				}
				this.RescheduleMailboxPolling(flag3);
				return;
			}
			ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)1439UL, DatabaseManager.Tracer, "Mailbox Polling: Exiting and scheduling a retry as there are no mailbox table changes.", new object[0]);
			this.RescheduleMailboxPolling(false);
		}

		internal bool InitializeSystemMailboxGuid()
		{
			Guid guid;
			if (!this.coreDatabaseManager.FindSystemMailboxGuid(this.systemMailboxName, out guid))
			{
				ContentAggregationConfig.SyncLogSession.LogError((TSLID)255UL, DatabaseManager.Tracer, "Database initialize failed for database {0}. Could not find the system mailbox Guid.", new object[]
				{
					this.databaseGuid
				});
				return false;
			}
			this.systemMailboxGuid = guid;
			return true;
		}

		internal XElement GetDiagnosticInfo(SyncDiagnosticMode mode)
		{
			XElement xelement = new XElement("Database");
			xelement.Add(new XElement("databaseId", this.databaseGuid));
			xelement.Add(new XElement("enabled", this.Enabled));
			xelement.Add(new XElement("lastCompletedPollingStartTime", (this.lastCompletedPollingStartTime != null) ? this.lastCompletedPollingStartTime.Value.ToString("o") : string.Empty));
			xelement.Add(new XElement("lastTwoWayPollingStartTime", (this.lastTwoWayPollingStartTime != null) ? this.lastTwoWayPollingStartTime.Value.ToString("o") : string.Empty));
			xelement.Add(new XElement("nextPollingStartTime", (this.nextPollingStartTime != null) ? this.nextPollingStartTime.Value.ToString("o") : string.Empty));
			if (this.subscriptionCacheManager != null)
			{
				this.subscriptionCacheManager.AddDiagnosticInfoTo(xelement);
			}
			return xelement;
		}

		private bool TryTriggerRecrawlOnException(Guid userMailboxGuid, MailboxSession userMailboxSession)
		{
			bool result;
			try
			{
				MailboxSession userMailboxSession2 = this.mailboxManager.GetUserMailboxSession(userMailboxGuid);
				if (userMailboxSession2 != null)
				{
					using (userMailboxSession2)
					{
						this.mailboxTableManager.SetMailboxSubscriptionListTimestamp(userMailboxSession2, ExDateTime.UtcNow);
						goto IL_40;
					}
				}
				this.mailboxTableManager.SetMailboxSubscriptionListTimestamp(userMailboxSession, ExDateTime.UtcNow);
				IL_40:
				result = true;
			}
			catch (CacheTransientException)
			{
				result = false;
			}
			catch (CachePermanentException)
			{
				result = false;
			}
			return result;
		}

		private void ConsistencyCheckCrawl(Guid userMailboxGuid, MailboxSession userMailboxSession, out IList<AggregationSubscription> foundSubscriptions)
		{
			try
			{
				Guid? guid;
				Guid? guid2;
				this.mailboxManager.CrawlUserMailbox(userMailboxSession, out foundSubscriptions, out guid, out guid2);
			}
			catch (CacheTransientException)
			{
				this.TryTriggerRecrawlOnException(userMailboxGuid, userMailboxSession);
				throw;
			}
			catch (CachePermanentException)
			{
				this.TryTriggerRecrawlOnException(userMailboxGuid, userMailboxSession);
				throw;
			}
		}

		private void RescheduleMailboxPolling(bool successful)
		{
			lock (this.syncObject)
			{
				if (this.enabled && this.mailboxPollingTimer != null)
				{
					TimeSpan timeSpan;
					if (successful)
					{
						timeSpan = ContentAggregationConfig.MailboxTablePollingInterval;
					}
					else
					{
						timeSpan = ContentAggregationConfig.MailboxTableRetryPollingInterval;
					}
					this.nextPollingStartTime = new ExDateTime?(ExDateTime.UtcNow + timeSpan);
					ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)491UL, DatabaseManager.Tracer, "Mailbox polling: successful:{0} next poll will begin at {1}.", new object[]
					{
						successful,
						this.nextPollingStartTime.Value.ToString("o")
					});
					this.mailboxPollingTimer.Change(timeSpan, TimeSpan.FromMilliseconds(-1.0));
				}
			}
		}

		private static readonly Trace Tracer = ExTraceGlobals.DatabaseManagerTracer;

		private static DatabaseManager.MailboxCrawler mailboxCrawler = new DatabaseManager.MailboxCrawler();

		private Guid databaseGuid;

		private object syncObject = new object();

		private string systemMailboxName;

		private Guid systemMailboxGuid;

		private bool enabled;

		private ICoreDatabaseManager coreDatabaseManager;

		private SubscriptionCacheManager subscriptionCacheManager;

		private MailboxTableManager mailboxTableManager;

		private MailboxManager mailboxManager;

		private byte databaseLookupIndex;

		private GuardedTimer mailboxPollingTimer;

		private ExDateTime? lastCompletedPollingStartTime;

		private ExDateTime? lastTwoWayPollingStartTime;

		private ExDateTime? nextPollingStartTime;

		internal sealed class MailboxCrawler
		{
			public MailboxCrawler() : this(ContentAggregationConfig.CacheRepairEnabled, ContentAggregationConfig.DelayBeforeRepairThreadStarts, ContentAggregationConfig.MaxCacheMessageRepairAttempts)
			{
			}

			internal MailboxCrawler(bool enabled, TimeSpan delayBeforeCrawlThreadStarts, int maxCacheMessageCrawlAttempts)
			{
				this.crawlEnqueued = false;
				this.mailboxesToBeCrawled = new QueuedDictionary<KeyedPair<Pair<Guid, Guid>, int>>();
				this.enabled = enabled;
				this.delayBeforeCrawlThreadStarts = delayBeforeCrawlThreadStarts;
				this.maxCacheMessageCrawlAttempts = maxCacheMessageCrawlAttempts;
				this.currentlyProcessedMailbox = null;
				this.crawlWorkerThread = 0;
			}

			private bool IsCrawlInProgress
			{
				get
				{
					bool result;
					lock (this.crawlThreadLock)
					{
						result = (this.crawlEnqueued || this.currentlyProcessedMailbox != null);
					}
					return result;
				}
			}

			public void StartCrawl()
			{
				lock (this.crawlThreadLock)
				{
					this.enabled = true;
					if (this.mailboxesToBeCrawled.Count > 0 && !this.crawlEnqueued)
					{
						if (this.crawlWorkerTimer == null)
						{
							this.crawlWorkerTimer = new Timer(new TimerCallback(this.CrawlWorker), null, this.delayBeforeCrawlThreadStarts, TimeSpan.FromMilliseconds(-1.0));
						}
						else
						{
							this.crawlWorkerTimer.Change(this.delayBeforeCrawlThreadStarts, TimeSpan.FromMilliseconds(-1.0));
						}
						ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)256UL, DatabaseManager.Tracer, "StartCrawl: Starting crawl thread.", new object[0]);
						this.crawlEnqueued = true;
					}
				}
			}

			public void StopCrawl()
			{
				ManualResetEvent manualResetEvent = null;
				try
				{
					lock (this.crawlThreadLock)
					{
						this.enabled = false;
						if (this.crawlWorkerTimer != null)
						{
							manualResetEvent = new ManualResetEvent(false);
							this.crawlWorkerTimer.Dispose(manualResetEvent);
							this.crawlWorkerTimer = null;
						}
						this.mailboxesToBeCrawled.Clear();
						this.crawlEnqueued = false;
					}
					ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)257UL, DatabaseManager.Tracer, "StopCrawl: Waiting for Crawl Worker to shutdown.", new object[0]);
					if (manualResetEvent != null)
					{
						manualResetEvent.WaitOne();
					}
				}
				finally
				{
					if (manualResetEvent != null)
					{
						manualResetEvent.Close();
						manualResetEvent = null;
					}
				}
				ContentAggregationConfig.SyncLogSession.LogInformation((TSLID)258UL, DatabaseManager.Tracer, "StopCrawl: Stopped crawl timer, crawl thread done as well.", new object[0]);
			}

			public void EnqueueMailboxCrawl(Guid databaseGuid, Guid mailboxGuid)
			{
				SyncUtilities.ThrowIfGuidEmpty("databaseGuid", databaseGuid);
				SyncUtilities.ThrowIfGuidEmpty("mailboxGuid", mailboxGuid);
				if (!DataAccessLayer.Initialized)
				{
					ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)394UL, DatabaseManager.Tracer, Guid.Empty, mailboxGuid, "EnqueueMailboxCrawl: Crawl for mailbox in database {0} skipped. DAL not initialized.", new object[]
					{
						databaseGuid
					});
					return;
				}
				DatabaseManager databaseManager = DataAccessLayer.GetDatabaseManager(databaseGuid);
				if (databaseManager == null || !databaseManager.Enabled)
				{
					ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)259UL, DatabaseManager.Tracer, Guid.Empty, mailboxGuid, "EnqueueMailboxCrawl: Crawl for mailbox in database {0} skipped. Database disabled.", new object[]
					{
						databaseGuid
					});
					return;
				}
				bool flag = false;
				Pair<Guid, Guid> first = new Pair<Guid, Guid>(databaseGuid, mailboxGuid);
				KeyedPair<Pair<Guid, Guid>, int> entry = new KeyedPair<Pair<Guid, Guid>, int>(first, 1);
				lock (this.crawlThreadLock)
				{
					if (!this.enabled)
					{
						ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)1278UL, DatabaseManager.Tracer, Guid.Empty, mailboxGuid, "EnqueueMailboxCrawl: Crawl for mailbox in database {0} skipped. Crawler is shutting down.", new object[]
						{
							databaseGuid
						});
						return;
					}
					if (this.currentlyProcessedMailbox != null && this.currentlyProcessedMailbox.First.First == databaseGuid && this.currentlyProcessedMailbox.First.Second == mailboxGuid && this.crawlWorkerThread == Thread.CurrentThread.ManagedThreadId)
					{
						ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)261UL, DatabaseManager.Tracer, Guid.Empty, mailboxGuid, "EnqueueMailboxCrawl: Ignoring reentrant mailbox enqueue in database {0}.", new object[]
						{
							databaseGuid
						});
					}
					else if (this.mailboxesToBeCrawled.Contains(entry))
					{
						ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)262UL, DatabaseManager.Tracer, Guid.Empty, mailboxGuid, "EnqueueMailboxCrawl: Mailbox in database {0} is already enqueued for crawl.", new object[]
						{
							databaseGuid
						});
					}
					else
					{
						ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)260UL, DatabaseManager.Tracer, Guid.Empty, mailboxGuid, "EnqueueMailboxCrawl: Will enqueue crawl for mailbox in database {0}.", new object[]
						{
							databaseGuid
						});
						ManagerPerfCounterHandler.Instance.IncrementMailboxesToBeRebuilt();
						this.mailboxesToBeCrawled.Enqueue(entry);
						flag = !this.crawlEnqueued;
					}
				}
				if (flag)
				{
					this.StartCrawl();
				}
			}

			internal bool CheckIsCrawlInProgress()
			{
				return this.IsCrawlInProgress;
			}

			internal KeyedPair<Pair<Guid, Guid>, int>[] GetCrawlEntriesCopy()
			{
				KeyedPair<Pair<Guid, Guid>, int>[] result;
				lock (this.crawlThreadLock)
				{
					KeyedPair<Pair<Guid, Guid>, int>[] array = new KeyedPair<Pair<Guid, Guid>, int>[this.mailboxesToBeCrawled.Count];
					int num = 0;
					foreach (KeyedPair<Pair<Guid, Guid>, int> keyedPair in this.mailboxesToBeCrawled)
					{
						array[num++] = keyedPair;
					}
					result = array;
				}
				return result;
			}

			internal KeyedPair<Pair<Guid, Guid>, int> GetCurrentlyProcessedEntryCopy()
			{
				KeyedPair<Pair<Guid, Guid>, int> result;
				lock (this.crawlThreadLock)
				{
					if (this.currentlyProcessedMailbox == null)
					{
						result = null;
					}
					else
					{
						KeyedPair<Pair<Guid, Guid>, int> keyedPair = new KeyedPair<Pair<Guid, Guid>, int>(new Pair<Guid, Guid>(this.currentlyProcessedMailbox.First.First, this.currentlyProcessedMailbox.First.Second), this.currentlyProcessedMailbox.Second);
						result = keyedPair;
					}
				}
				return result;
			}

			internal void AddBasicDiagnosticInfoTo(XElement parentElement)
			{
				lock (this.crawlThreadLock)
				{
					parentElement.Add(new XElement("isMailboxCrawlInProgress", this.IsCrawlInProgress));
					parentElement.Add(new XElement("totalMailboxesToBeCrawled", this.mailboxesToBeCrawled.Count));
				}
			}

			internal void AddVerboseDiagnosticInfoTo(XElement parentElement)
			{
				lock (this.crawlThreadLock)
				{
					foreach (KeyedPair<Pair<Guid, Guid>, int> keyedPair in this.mailboxesToBeCrawled)
					{
						XElement content = new XElement("MailboxCrawlEntry", new object[]
						{
							new XElement("databaseId", keyedPair.First.First),
							new XElement("mailboxId", keyedPair.First.Second),
							new XElement("attemptNumber", keyedPair.Second)
						});
						parentElement.Add(content);
					}
				}
			}

			internal void CrawlNow()
			{
				this.CrawlWorker(null);
			}

			private void CrawlWorker(object objectState)
			{
				ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)263UL, DatabaseManager.Tracer, "CrawlWorker: Starting crawl of cache entries.", new object[0]);
				int num = 0;
				int num2 = 0;
				try
				{
					this.crawlWorkerThread = Thread.CurrentThread.ManagedThreadId;
					for (;;)
					{
						Guid guid = Guid.Empty;
						Guid guid2 = Guid.Empty;
						int num3 = 0;
						lock (this.crawlThreadLock)
						{
							if (!this.enabled)
							{
								ContentAggregationConfig.SyncLogSession.LogInformation((TSLID)1279UL, DatabaseManager.Tracer, "CrawlWorker: Crawl is shutting down. {0} cache entries were successfully crawled out of {1} attempted ones.", new object[]
								{
									num2,
									num
								});
								break;
							}
							if (this.mailboxesToBeCrawled.Count == 0)
							{
								ContentAggregationConfig.SyncLogSession.LogInformation((TSLID)264UL, DatabaseManager.Tracer, "CrawlWorker: Crawl is done. {0} cache entries were successfully crawled out of {1} attempted ones.", new object[]
								{
									num2,
									num
								});
								break;
							}
							this.currentlyProcessedMailbox = this.mailboxesToBeCrawled.Dequeue();
							guid = this.currentlyProcessedMailbox.First.First;
							guid2 = this.currentlyProcessedMailbox.First.Second;
							num3 = this.currentlyProcessedMailbox.Second;
						}
						try
						{
							ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)265UL, DatabaseManager.Tracer, Guid.Empty, guid2, "CrawlWorker: Attempting to crawl cache entry for mailbox in database {0} for the #{1} attempt.", new object[]
							{
								guid,
								num3
							});
							DatabaseManager databaseManager = DataAccessLayer.GetDatabaseManager(guid);
							if (databaseManager == null)
							{
								ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)266UL, DatabaseManager.Tracer, Guid.Empty, guid2, "CrawlWorker: Skipping crawl for mailbox in database {0} as could not get the database manager.", new object[]
								{
									guid
								});
							}
							else
							{
								SubscriptionCacheManager subscriptionCacheManager = databaseManager.SubscriptionCacheManager;
								if (subscriptionCacheManager == null)
								{
									ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)203UL, DatabaseManager.Tracer, Guid.Empty, guid2, "CrawlWorker: Skipping crawl for mailbox in database {0} as could not get the cache manager.", new object[]
									{
										guid
									});
								}
								else
								{
									bool flag2 = false;
									bool flag3 = false;
									bool flag4 = false;
									try
									{
										try
										{
											bool flag5 = false;
											databaseManager.RebuildMailbox(guid2, out flag5);
											flag2 = true;
										}
										catch (CacheCorruptException)
										{
											subscriptionCacheManager.DeleteCacheMessage(guid2);
											flag3 = true;
											flag4 = false;
										}
									}
									catch (CacheTransientException)
									{
										flag3 = true;
										flag4 = true;
									}
									catch (CachePermanentException)
									{
										flag3 = false;
										flag4 = false;
									}
									num++;
									if (flag2)
									{
										num2++;
									}
									else
									{
										if (flag3)
										{
											if (num3 < this.maxCacheMessageCrawlAttempts)
											{
												ContentAggregationConfig.SyncLogSession.LogInformation((TSLID)267UL, DatabaseManager.Tracer, Guid.Empty, guid2, "CrawlWorker: Enqueing for crawl after a failed attempt for mailbox in database {0}.", new object[]
												{
													guid
												});
												KeyedPair<Pair<Guid, Guid>, int> entry = new KeyedPair<Pair<Guid, Guid>, int>(new Pair<Guid, Guid>(guid, guid2), num3 + 1);
												lock (this.crawlThreadLock)
												{
													this.currentlyProcessedMailbox = null;
													if (!this.mailboxesToBeCrawled.Contains(entry))
													{
														this.mailboxesToBeCrawled.Enqueue(entry);
													}
												}
												ManagerPerfCounterHandler.Instance.IncrementMailboxesToBeRebuilt();
											}
											else
											{
												ContentAggregationConfig.SyncLogSession.LogError((TSLID)268UL, DatabaseManager.Tracer, Guid.Empty, guid2, "CrawlWorker: Will not try to crawl again for mailbox in database {0} as we hit the maximum attempt of retries.", new object[]
												{
													guid
												});
											}
										}
										else
										{
											ContentAggregationConfig.SyncLogSession.LogError((TSLID)269UL, DatabaseManager.Tracer, Guid.Empty, guid2, "CrawlWorker: Skipping crawl for mailbox in database {0}.", new object[]
											{
												guid
											});
										}
										if (flag4)
										{
											ContentAggregationConfig.SyncLogSession.LogInformation((TSLID)270UL, DatabaseManager.Tracer, Guid.Empty, null, "CrawlWorker: Backing off crawl ", new object[0]);
											break;
										}
									}
								}
							}
						}
						finally
						{
							ManagerPerfCounterHandler.Instance.DecrementMailboxesToBeRebuilt();
							lock (this.crawlThreadLock)
							{
								this.currentlyProcessedMailbox = null;
							}
						}
					}
				}
				finally
				{
					bool flag8 = false;
					lock (this.crawlThreadLock)
					{
						this.crawlEnqueued = false;
						flag8 = (this.mailboxesToBeCrawled.Count > 0);
						this.crawlWorkerThread = 0;
					}
					if (flag8)
					{
						this.StartCrawl();
					}
				}
			}

			private object crawlThreadLock = new object();

			private Timer crawlWorkerTimer;

			private bool crawlEnqueued;

			private QueuedDictionary<KeyedPair<Pair<Guid, Guid>, int>> mailboxesToBeCrawled;

			private KeyedPair<Pair<Guid, Guid>, int> currentlyProcessedMailbox;

			private int crawlWorkerThread;

			private bool enabled;

			private TimeSpan delayBeforeCrawlThreadStarts;

			private int maxCacheMessageCrawlAttempts;
		}

		private class CoreDatabaseManager : ICoreDatabaseManager
		{
			public bool FindSystemMailboxGuid(string systemMailboxName, out Guid systemMailboxGuid)
			{
				ExchangePrincipal exchangePrincipal = null;
				Exception ex = null;
				systemMailboxGuid = default(Guid);
				try
				{
					ADSystemMailbox adsystemMailbox = this.FindSystemMailbox(systemMailboxName);
					if (adsystemMailbox == null)
					{
						ContentAggregationConfig.SyncLogSession.LogError((TSLID)271UL, DatabaseManager.Tracer, "Unable to find the system mailbox.", new object[0]);
						return false;
					}
					exchangePrincipal = ExchangePrincipal.FromADSystemMailbox(ADSessionSettings.FromRootOrgScopeSet(), adsystemMailbox, LocalServer.GetServer());
				}
				catch (DataValidationException ex2)
				{
					ex = ex2;
				}
				catch (ObjectNotFoundException ex3)
				{
					ex = ex3;
				}
				catch (DataSourceOperationException ex4)
				{
					ex = ex4;
				}
				catch (DataSourceTransientException ex5)
				{
					ex = ex5;
				}
				if (ex != null)
				{
					ContentAggregationConfig.SyncLogSession.LogError((TSLID)272UL, DatabaseManager.Tracer, "{0}: Unable to find valid system mailbox. Exception: {1}", new object[]
					{
						this,
						ex
					});
					return false;
				}
				systemMailboxGuid = exchangePrincipal.MailboxInfo.MailboxGuid;
				return true;
			}

			public bool StartCacheManager(DatabaseManager databaseManager)
			{
				SubscriptionCacheManager subscriptionCacheManager;
				bool flag = SubscriptionCacheManager.TryCreateCacheManager(databaseManager, new EventHandler<SubscriptionInformation>(DataAccessLayer.OnSubscriptionAddedHandler), new EventHandler<SubscriptionInformation>(DataAccessLayer.OnSubscriptionRemovedHandler), false, out subscriptionCacheManager);
				if (flag)
				{
					databaseManager.SubscriptionCacheManager = subscriptionCacheManager;
				}
				return flag;
			}

			public void ShutdownCacheManager(DatabaseManager databaseManager)
			{
				databaseManager.subscriptionCacheManager.Shutdown();
				databaseManager.subscriptionCacheManager = null;
			}

			private ADSystemMailbox FindSystemMailbox(string systemMailboxName)
			{
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 1777, "FindSystemMailbox", "f:\\15.00.1497\\sources\\dev\\transportSync\\src\\Manager\\Database\\DatabaseManager.cs");
				ADRecipient[] array = null;
				try
				{
					array = tenantOrRootOrgRecipientSession.Find(null, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, systemMailboxName), null, 1);
					if (array.Length != 1 || !(array[0] is ADSystemMailbox))
					{
						ContentAggregationConfig.SyncLogSession.LogError((TSLID)273UL, DatabaseManager.Tracer, "Found {0} mailboxes named '{1}' in the AD", new object[]
						{
							array.Length,
							systemMailboxName
						});
						return null;
					}
				}
				catch (DataSourceTransientException ex)
				{
					ContentAggregationConfig.SyncLogSession.LogError((TSLID)274UL, DatabaseManager.Tracer, "Exception on looking up the mailbox '{0}' in AD : {1}", new object[]
					{
						systemMailboxName,
						ex
					});
					return null;
				}
				catch (DataSourceOperationException ex2)
				{
					ContentAggregationConfig.SyncLogSession.LogError((TSLID)275UL, DatabaseManager.Tracer, "Exception on looking up the mailbox '{0}' in AD : {1}", new object[]
					{
						systemMailboxName,
						ex2
					});
					return null;
				}
				return (ADSystemMailbox)array[0];
			}
		}
	}
}
