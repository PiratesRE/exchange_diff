using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.MailboxTransport.ContentAggregation;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Rpc.Submission;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;
using Microsoft.Exchange.Transport.Sync.Worker.Health;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Transport.Sync.Worker.Throttling
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SyncStoreLoadManager : DisposeTrackableBase
	{
		protected SyncStoreLoadManager(SyncLogSession syncLogSession)
		{
			ResourceLoadDelayInfo.Initialize();
			this.resourceMonitoringDictionary = new Dictionary<Guid, SyncResource>();
			this.monitoredResourcesResourcesKey = new Dictionary<string, ResourceKey[]>();
			this.syncLogSession = syncLogSession;
			this.sleepWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
			this.cloudLatencyAverage = new RunningAverageFloat(SyncStoreLoadManager.NumberOfPercentWIInStoreSamples);
			this.storeLatencyAverage = new RunningAverageFloat(SyncStoreLoadManager.NumberOfPercentWIInStoreSamples);
			this.storeCloudRatioAverage = new RunningAverageFloat(SyncStoreLoadManager.NumberOfPercentWIInStoreSamples);
			this.syncBudget = this.AcquireUnthrottledBudget(SyncStoreLoadManager.TransportSyncBudgetKey);
			this.isRunning = true;
			this.cloudLatencyAverage.Update(100f);
			this.storeLatencyAverage.Update(10f);
			this.storeCloudRatioAverage.Update(1f);
			this.randomDelay = new Random();
		}

		internal static SyncStoreLoadManager Singleton
		{
			get
			{
				return SyncStoreLoadManager.singleton;
			}
			set
			{
				SyncStoreLoadManager.singleton = value;
			}
		}

		internal static SyncStoreLoadManager Instance
		{
			get
			{
				SyncStoreLoadManager result;
				lock (SyncStoreLoadManager.syncObject)
				{
					if (SyncStoreLoadManager.singleton == null)
					{
						throw new InvalidOperationException("Instance can be accessed only after Create.");
					}
					result = SyncStoreLoadManager.singleton;
				}
				return result;
			}
		}

		internal static float StoreLatencyAverage
		{
			get
			{
				return SyncStoreLoadManager.Instance.storeLatencyAverage.Value;
			}
		}

		internal static float CloudLatencyAverage
		{
			get
			{
				return SyncStoreLoadManager.Instance.cloudLatencyAverage.Value;
			}
		}

		internal static float StoreCloudRatioAverage
		{
			get
			{
				return SyncStoreLoadManager.Instance.storeCloudRatioAverage.Value;
			}
		}

		protected SyncLogSession SyncLogSession
		{
			get
			{
				return this.syncLogSession;
			}
		}

		internal static bool Sleep(int sleepTime)
		{
			return SyncStoreLoadManager.Instance.sleepWaitHandle.WaitOne(sleepTime);
		}

		internal static void StartExecution()
		{
			SyncStoreLoadManager.Instance.isRunning = true;
			SyncStoreLoadManager.Instance.sleepWaitHandle.Reset();
			SyncStoreLoadManager.Instance.SyncLogSession.LogVerbose((TSLID)1241UL, ExTraceGlobals.SchedulerTracer, "StartExection of SyncStoreLoadManager!", new object[0]);
		}

		internal static void StopExecution()
		{
			SyncStoreLoadManager.Instance.isRunning = false;
			SyncStoreLoadManager.Instance.sleepWaitHandle.Set();
			SyncStoreLoadManager.Instance.SyncLogSession.LogVerbose((TSLID)1242UL, ExTraceGlobals.SchedulerTracer, "StopExecution of SyncStoreLoadManager!", new object[0]);
		}

		internal static void RecordPercentTimeInStore(float storeLatency, float cloudLatency, float storeCloudRatio)
		{
			SyncStoreLoadManager.Instance.storeLatencyAverage.Update(storeLatency);
			SyncStoreLoadManager.Instance.cloudLatencyAverage.Update(cloudLatency);
			SyncStoreLoadManager.Instance.storeCloudRatioAverage.Update(storeCloudRatio);
		}

		internal static void TrackMailItem(TransportMailItem transportMailItem, Guid userMailboxGuid, Guid subscriptionGuid, string cloudId)
		{
			SyncTransportServer orCreateSyncTransportServer = SyncStoreLoadManager.Instance.GetOrCreateSyncTransportServer();
			orCreateSyncTransportServer.TrackMailItem(transportMailItem, userMailboxGuid, subscriptionGuid, cloudId);
		}

		internal static void ThrottleAndExecuteStoreCall(MailboxSession mailboxSession, EventHandler<RoundtripCompleteEventArgs> roundtripComplete, string methodName, Action method)
		{
			SyncStoreLoadManager.PerformXsoOperation(mailboxSession, methodName, method, roundtripComplete);
		}

		internal static Folder FolderBind(MailboxSession mailboxSession, StoreId storeId, PropertyDefinition[] properties, EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			Folder folder = null;
			SyncStoreLoadManager.PerformXsoOperation(mailboxSession, "FolderBind", delegate()
			{
				folder = Folder.Bind(mailboxSession, storeId, properties);
			}, roundtripComplete);
			return folder;
		}

		internal static void FolderSave(Folder folder, MailboxSession mailboxSession, EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			SyncStoreLoadManager.PerformXsoOperation(mailboxSession, "FolderSave", delegate()
			{
				folder.Save();
			}, roundtripComplete);
		}

		internal static void FolderLoad(Folder folder, MailboxSession mailboxSession, PropertyDefinition[] properties, EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			SyncStoreLoadManager.PerformXsoOperation(mailboxSession, "FolderLoad", delegate()
			{
				folder.Load(properties);
			}, roundtripComplete);
		}

		internal static Folder FolderCreate(MailboxSession mailboxSession, StoreObjectId storeId, StoreObjectType objectType, string objectName, CreateMode createMode, EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			Folder folder = null;
			SyncStoreLoadManager.PerformXsoOperation(mailboxSession, "FolderCreate", delegate()
			{
				folder = Folder.Create(mailboxSession, storeId, objectType, objectName, createMode);
			}, roundtripComplete);
			return folder;
		}

		internal static Item ItemBind(MailboxSession mailboxSession, StoreId storeId, PropertyDefinition[] properties, EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			Item item = null;
			SyncStoreLoadManager.PerformXsoOperation(mailboxSession, "ItemBind", delegate()
			{
				item = Item.Bind(mailboxSession, storeId, properties);
			}, roundtripComplete);
			return item;
		}

		internal static ConflictResolutionResult ItemSave(Item item, MailboxSession mailboxSession, SaveMode saveMode, EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			ConflictResolutionResult result = null;
			SyncStoreLoadManager.PerformXsoOperation(mailboxSession, "ItemSave", delegate()
			{
				result = item.Save(saveMode);
			}, roundtripComplete);
			return result;
		}

		internal static void ItemLoad(Item item, MailboxSession mailboxSession, PropertyDefinition[] properties, EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			SyncStoreLoadManager.PerformXsoOperation(mailboxSession, "ItemLoad", delegate()
			{
				item.Load(properties);
			}, roundtripComplete);
		}

		internal static MessageItem MessageItemCreateAggregated(MailboxSession mailboxSession, StoreId storeId, EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			MessageItem item = null;
			SyncStoreLoadManager.PerformXsoOperation(mailboxSession, "MessageItemCreateAggregated", delegate()
			{
				item = MessageItem.CreateAggregated(mailboxSession, storeId);
			}, roundtripComplete);
			return item;
		}

		internal static ConflictResolutionResult ContactSave(Contact contact, MailboxSession mailboxSession, SaveMode saveMode, EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			ConflictResolutionResult result = null;
			SyncStoreLoadManager.PerformXsoOperation(mailboxSession, "ContactSave", delegate()
			{
				result = contact.Save(saveMode);
			}, roundtripComplete);
			return result;
		}

		internal static void ContactLoad(Contact contact, MailboxSession mailboxSession, PropertyDefinition[] properties, EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			SyncStoreLoadManager.PerformXsoOperation(mailboxSession, "ContactLoad", delegate()
			{
				contact.Load(properties);
			}, roundtripComplete);
		}

		internal static Contact ContactCreate(MailboxSession mailboxSession, StoreId storeId, EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			Contact contact = null;
			SyncStoreLoadManager.PerformXsoOperation(mailboxSession, "ContactCreate", delegate()
			{
				contact = Contact.Create(mailboxSession, storeId);
			}, roundtripComplete);
			return contact;
		}

		internal static AggregationSubscription LoadSubscription(MailboxSession mailboxSession, StoreId messageId, AggregationSubscriptionType subscriptionType, EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			AggregationSubscription subscription = null;
			SyncStoreLoadManager.PerformXsoOperation(mailboxSession, "LoadSubscription", delegate()
			{
				subscription = SubscriptionManager.LoadSubscription(mailboxSession, messageId, subscriptionType);
			}, roundtripComplete);
			return subscription;
		}

		internal static bool TrySaveSubscription(MailboxSession mailboxSession, AggregationSubscription subscription, EventHandler<RoundtripCompleteEventArgs> roundtripComplete, out Exception exception)
		{
			SubscriptionMailboxSession subMailboxSession = new SubscriptionMailboxSession(mailboxSession);
			Exception saveException = null;
			if (SyncStoreLoadManager.TryPerformXsoOperation(mailboxSession, "SaveSubscription", delegate
			{
				SubscriptionManager.Instance.TrySaveSubscription(subMailboxSession, subscription, out saveException);
			}, roundtripComplete, out exception))
			{
				exception = saveException;
			}
			return exception != null;
		}

		internal static bool TryDeleteSubscription(MailboxSession mailboxSession, Guid subscriptionGuid, EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			bool deleted = true;
			Exception ex;
			bool flag = SyncStoreLoadManager.TryPerformXsoOperation(mailboxSession, "TryDeleteSubscription", delegate
			{
				deleted = SubscriptionManager.TryDeleteSubscription(mailboxSession, subscriptionGuid);
			}, roundtripComplete, out ex);
			return flag && deleted;
		}

		internal static bool TryDeleteSubscription(MailboxSession mailboxSession, AggregationSubscription subscription, EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			bool deleted = true;
			Exception ex;
			bool flag = SyncStoreLoadManager.TryPerformXsoOperation(mailboxSession, "TryDeleteSubscription", delegate
			{
				deleted = SubscriptionManager.TryDeleteSubscription(mailboxSession, subscription);
			}, roundtripComplete, out ex);
			return flag && deleted;
		}

		internal static SyncStateStorage SyncStateStorageCreate(MailboxSession mailboxSession, string protocol, string deviceType, string deviceId, StateStorageFeatures features, EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			SyncStateStorage syncStateStorage = null;
			SyncStoreLoadManager.PerformXsoOperation(mailboxSession, "SyncStateStorageCreate", delegate()
			{
				syncStateStorage = SyncStateStorage.Create(mailboxSession, new DeviceIdentity(deviceId, deviceType, protocol), features, null);
			}, roundtripComplete);
			return syncStateStorage;
		}

		internal static SyncStateStorage SyncStateStorageBind(MailboxSession mailboxSession, string protocol, string deviceType, string deviceId, EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			SyncStateStorage syncStateStorage = null;
			SyncStoreLoadManager.PerformXsoOperation(mailboxSession, "SyncStateStorageBind", delegate()
			{
				syncStateStorage = SyncStateStorage.Bind(mailboxSession, new DeviceIdentity(deviceId, deviceType, protocol), null);
			}, roundtripComplete);
			return syncStateStorage;
		}

		internal static bool SyncStateStorageDelete(MailboxSession mailboxSession, string protocol, string deviceType, string deviceId, EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			bool deleteResult = false;
			SyncStoreLoadManager.PerformXsoOperation(mailboxSession, "SyncStateStorageDelete", delegate()
			{
				deleteResult = SyncStateStorage.DeleteSyncStateStorage(mailboxSession, new DeviceIdentity(deviceId, deviceType, protocol), null);
			}, roundtripComplete);
			return deleteResult;
		}

		internal static CustomSyncState SyncStateStorageGetCustomSyncState(Guid databaseGuid, SyncStateStorage syncStateStorage, SyncStateInfo syncStateInfo, EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			CustomSyncState customSyncState = null;
			SyncStoreLoadManager.PerformXsoOperation(databaseGuid, syncStateStorage, "SyncStateStorageGetCustomSyncState", delegate()
			{
				customSyncState = syncStateStorage.GetCustomSyncState(syncStateInfo, new PropertyDefinition[0]);
			}, roundtripComplete);
			return customSyncState;
		}

		internal static CustomSyncState SyncStateStorageCreateCustomSyncState(Guid databaseGuid, SyncStateStorage syncStateStorage, SyncStateInfo syncStateInfo, EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			CustomSyncState customSyncState = null;
			SyncStoreLoadManager.PerformXsoOperation(databaseGuid, syncStateStorage, "SyncStateStorageCreateCustomSyncState", delegate()
			{
				customSyncState = syncStateStorage.CreateCustomSyncState(syncStateInfo);
			}, roundtripComplete);
			return customSyncState;
		}

		internal static void Link(MailboxSession mailboxSession, BulkAutomaticLink bulkAutomaticLink, Contact contact, EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			SyncStoreLoadManager.PerformXsoOperation(mailboxSession, "Link", delegate()
			{
				bulkAutomaticLink.Link(contact);
			}, roundtripComplete);
		}

		internal static void NotifyContactSaved(MailboxSession mailboxSession, BulkAutomaticLink bulkAutomaticLink, Contact contact, EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			SyncStoreLoadManager.PerformXsoOperation(mailboxSession, "NotifyContactSaved", delegate()
			{
				bulkAutomaticLink.NotifyContactSaved(contact);
			}, roundtripComplete);
		}

		internal static void Create(SyncLogSession syncLogSession)
		{
			if (SyncStoreLoadManager.singleton != null)
			{
				throw new InvalidOperationException("Create should be called only once.");
			}
			SyncStoreLoadManager.singleton = new SyncStoreLoadManager(syncLogSession);
		}

		internal virtual void EnableLoadTrackingOnSession(MailboxSession mailboxSession)
		{
			SyncUtilities.ThrowIfArgumentNull("mailboxSession", mailboxSession);
			mailboxSession.AccountingObject = this.syncBudget;
		}

		internal bool TryAcceptWorkItem(AggregationWorkItem workItem, out SubscriptionSubmissionResult result)
		{
			SyncUtilities.ThrowIfArgumentNull("workItem", workItem);
			result = SubscriptionSubmissionResult.Success;
			IEnumerable<SyncResource> orCreateSyncResources = this.GetOrCreateSyncResources(workItem);
			List<SyncResource> list = new List<SyncResource>();
			foreach (SyncResource syncResource in orCreateSyncResources)
			{
				if (!syncResource.TryAcceptWorkItem(workItem, out result))
				{
					this.SyncLogSession.LogVerbose((TSLID)1351UL, ExTraceGlobals.SchedulerTracer, "AcceptCheck: WI could not be accepted on Resource {0} - {1}.", new object[]
					{
						syncResource.ResourceId,
						syncResource.GetType().ToString()
					});
					foreach (SyncResource syncResource2 in list)
					{
						syncResource2.RemoveWorkItem(workItem);
					}
					return false;
				}
				list.Add(syncResource);
				this.SyncLogSession.LogVerbose((TSLID)1022UL, ExTraceGlobals.SchedulerTracer, "AcceptCheck: WI can be accepted on Resource {0} - {1}.", new object[]
				{
					syncResource.ResourceId,
					syncResource.GetType().ToString()
				});
			}
			this.SyncLogSession.LogVerbose((TSLID)1525UL, ExTraceGlobals.SchedulerTracer, "AcceptCheck: WI Accepted on all Resources.", new object[0]);
			return true;
		}

		internal void RemoveWorkItem(AggregationWorkItem workItem)
		{
			SyncUtilities.ThrowIfArgumentNull("workItem", workItem);
			IEnumerable<SyncResource> orCreateSyncResources = this.GetOrCreateSyncResources(workItem);
			foreach (SyncResource syncResource in orCreateSyncResources)
			{
				syncResource.RemoveWorkItem(workItem);
			}
		}

		protected SyncDB GetOrCreateSyncDB(Guid databaseGuid)
		{
			SyncUtilities.ThrowIfGuidEmpty("databaseGuid", databaseGuid);
			SyncResource syncResource;
			if (!this.resourceMonitoringDictionary.TryGetValue(databaseGuid, out syncResource))
			{
				lock (SyncStoreLoadManager.syncObject)
				{
					if (!this.resourceMonitoringDictionary.TryGetValue(databaseGuid, out syncResource))
					{
						syncResource = this.CreateSyncDB(databaseGuid);
						this.resourceMonitoringDictionary.Add(databaseGuid, syncResource);
					}
				}
			}
			return (SyncDB)syncResource;
		}

		protected virtual SyncDB CreateSyncDB(Guid databaseGuid)
		{
			return SyncDB.CreateSyncDB(databaseGuid, this.SyncLogSession);
		}

		protected SyncMailboxServer GetOrCreateSyncMailboxServer(Guid mailboxServerGuid, string mailboxServer)
		{
			SyncUtilities.ThrowIfGuidEmpty("mailboxServerGuid", mailboxServerGuid);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("mailboxServer", mailboxServer);
			SyncResource syncResource;
			if (!this.resourceMonitoringDictionary.TryGetValue(mailboxServerGuid, out syncResource))
			{
				lock (SyncStoreLoadManager.syncObject)
				{
					if (!this.resourceMonitoringDictionary.TryGetValue(mailboxServerGuid, out syncResource))
					{
						syncResource = this.CreateSyncMailboxServer(mailboxServerGuid, mailboxServer);
						this.resourceMonitoringDictionary.Add(mailboxServerGuid, syncResource);
					}
				}
			}
			return (SyncMailboxServer)syncResource;
		}

		protected virtual SyncMailboxServer CreateSyncMailboxServer(Guid mailboxServerGuid, string mailboxServer)
		{
			return SyncMailboxServer.CreateSyncMailboxServer(mailboxServerGuid, mailboxServer, this.SyncLogSession);
		}

		protected SyncTransportServer GetOrCreateSyncTransportServer()
		{
			SyncResource syncResource;
			if (!this.resourceMonitoringDictionary.TryGetValue(SyncStoreLoadManager.TransportServerMonitorKey, out syncResource))
			{
				lock (SyncStoreLoadManager.syncObject)
				{
					if (!this.resourceMonitoringDictionary.TryGetValue(SyncStoreLoadManager.TransportServerMonitorKey, out syncResource))
					{
						syncResource = this.CreateSyncTransportServer(AggregationConfiguration.Instance.MaxPendingMessagesInTransportQueueForTheServer, AggregationConfiguration.Instance.MaxPendingMessagesInTransportQueuePerUser);
						this.resourceMonitoringDictionary.Add(SyncStoreLoadManager.TransportServerMonitorKey, syncResource);
					}
				}
			}
			return (SyncTransportServer)syncResource;
		}

		protected virtual SyncTransportServer CreateSyncTransportServer(int maxPendingMessages, int maxPendingMessagesPerUser)
		{
			return SyncTransportServer.CreateSyncTransportServer(this.SyncLogSession, maxPendingMessages, maxPendingMessagesPerUser);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				lock (SyncStoreLoadManager.syncObject)
				{
					this.syncBudget.Dispose();
					this.syncBudget = null;
					this.resourceMonitoringDictionary.Clear();
					this.resourceMonitoringDictionary = null;
					this.sleepWaitHandle.Close();
					this.sleepWaitHandle = null;
					SyncStoreLoadManager.singleton = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SyncStoreLoadManager>(this);
		}

		private static ThrottlingInfo ThrottleStoreCall(IExchangePrincipal exchangePrincipal, Guid databaseGuid)
		{
			return SyncStoreLoadManager.ThrottleStoreCall(exchangePrincipal.MailboxInfo.Location.ServerGuid, exchangePrincipal.MailboxInfo.Location.ServerFqdn, databaseGuid);
		}

		private static ThrottlingInfo ThrottleStoreCall(Guid mailboxServerGuid, string mailboxServer, Guid databaseGuid)
		{
			IEnumerable<SyncResource> orCreateSyncResources = SyncStoreLoadManager.Instance.GetOrCreateSyncResources(mailboxServerGuid, mailboxServer, databaseGuid);
			ResourceKey[] orCreateSyncResourceKeys = SyncStoreLoadManager.Instance.GetOrCreateSyncResourceKeys(orCreateSyncResources);
			ResourceKey resourceKey = null;
			SyncResourceMonitorType monitor = SyncResourceMonitorType.Unknown;
			int num = 0;
			bool flag = true;
			TimeSpan minValue = TimeSpan.MinValue;
			ThrottlingInfo throttlingInfo = new ThrottlingInfo();
			while (flag)
			{
				if (!SyncStoreLoadManager.Instance.isRunning)
				{
					SyncStoreLoadManager.Instance.SyncLogSession.LogVerbose((TSLID)1244UL, ExTraceGlobals.SchedulerTracer, "Work item stopping retry for unhealthy resource due to SyncStoreLoadManager StopExecution!", new object[0]);
					return throttlingInfo;
				}
				num = SyncStoreLoadManager.Instance.GetDelay(orCreateSyncResourceKeys, out resourceKey);
				if (num > 0)
				{
					SyncStoreLoadManager.Instance.syncBudget.ResetWorkAccomplished();
				}
				bool flag2;
				bool flag3;
				monitor = SyncStoreLoadManager.Instance.IsAnyResourceUnhealthyOrUnknown(orCreateSyncResources, out flag2, out flag3);
				if (minValue >= AggregationConfiguration.Instance.SyncDurationThreshold)
				{
					SyncStoreLoadManager.Instance.SyncLogSession.LogVerbose((TSLID)1522UL, ExTraceGlobals.SchedulerTracer, "Work Item has exceeded the SyncDuration Threshold!", new object[0]);
					throw new SyncStoreUnhealthyException(databaseGuid, num);
				}
				Stopwatch stopwatch = Stopwatch.StartNew();
				if (num == 0)
				{
					SyncStoreLoadManager.Instance.SyncLogSession.LogDebugging((TSLID)1531UL, ExTraceGlobals.SchedulerTracer, "All Resources Healthy!", new object[]
					{
						num
					});
					stopwatch.Stop();
					break;
				}
				if (resourceKey != null)
				{
					SyncStoreLoadManager.Instance.SyncLogSession.LogDebugging((TSLID)1467UL, ExTraceGlobals.SchedulerTracer, "The offending resource {0} has delay of {1} milliseconds.", new object[]
					{
						resourceKey,
						num
					});
				}
				if (num == 2147483647)
				{
					SyncStoreLoadManager.Instance.SyncLogSession.LogDebugging((TSLID)1532UL, ExTraceGlobals.SchedulerTracer, "Delay for DB {0} and MailboxServer {1} is Int32.MaxValue.", new object[]
					{
						databaseGuid,
						mailboxServerGuid
					});
				}
				if (num > SyncStoreLoadManager.MaxBackOffValue)
				{
					SyncStoreLoadManager.Instance.SyncLogSession.LogDebugging((TSLID)1533UL, ExTraceGlobals.SchedulerTracer, "Backoff Delay is {0}. The value will be adjusted to {1}", new object[]
					{
						num,
						SyncStoreLoadManager.MaxBackOffValue
					});
					num = SyncStoreLoadManager.MaxBackOffValue;
				}
				SyncStoreLoadManager.Instance.SyncLogSession.LogDebugging((TSLID)1352UL, ExTraceGlobals.SchedulerTracer, "Budget snapshot for DB {0} and MailboxServer {1}: {2}", new object[]
				{
					databaseGuid,
					mailboxServerGuid,
					SyncStoreLoadManager.Instance.syncBudget.ToString()
				});
				if (SyncStoreLoadManager.BackOffOverride > 0)
				{
					num = SyncStoreLoadManager.BackOffOverride;
				}
				else if (flag3)
				{
					num = SyncStoreLoadManager.BackOffForUnknownHealth;
				}
				ResourceKey resourceKey2 = null;
				foreach (SyncResource syncResource in orCreateSyncResources)
				{
					SyncResourceMonitor[] resourceMonitors = syncResource.GetResourceMonitors();
					foreach (SyncResourceMonitor syncResourceMonitor in resourceMonitors)
					{
						if (syncResourceMonitor.ResourceKey.Equals(resourceKey))
						{
							int suggestedDelay = syncResource.GetSuggestedDelay(num);
							int num2 = SyncStoreLoadManager.Instance.GetRandomDelay(suggestedDelay);
							SyncStoreLoadManager.Instance.SyncLogSession.LogVerbose((TSLID)1534UL, ExTraceGlobals.SchedulerTracer, "Current delay for {0} is {1}. Suggested Delay is {2}. Random delay is {3}", new object[]
							{
								resourceKey,
								num,
								suggestedDelay,
								num2
							});
							num = num2;
							syncResource.UpdateDelay(num);
							resourceKey2 = syncResourceMonitor.ResourceKey;
							monitor = syncResourceMonitor.SyncResourceMonitorType;
							break;
						}
						SyncStoreLoadManager.Instance.SyncLogSession.LogDebugging((TSLID)1535UL, ExTraceGlobals.SchedulerTracer, "Delay not updated for {0}", new object[]
						{
							syncResourceMonitor.ResourceKey
						});
					}
				}
				if (resourceKey2 == null)
				{
					SyncStoreLoadManager.Instance.SyncLogSession.LogDebugging((TSLID)1536UL, ExTraceGlobals.SchedulerTracer, "No backoff applied. Could not find the offending resource!", new object[0]);
					stopwatch.Stop();
					break;
				}
				ResourceLoadState health;
				if (!flag2 && !flag3)
				{
					SyncStoreLoadManager.Instance.SyncLogSession.LogVerbose((TSLID)1537UL, ExTraceGlobals.SchedulerTracer, "Resource Fair {0} : Sleeping for {1} milliseconds, then accepting!", new object[]
					{
						resourceKey2,
						num
					});
					health = ResourceLoadState.Overloaded;
					flag = false;
				}
				else if (flag3)
				{
					SyncStoreLoadManager.Instance.SyncLogSession.LogVerbose((TSLID)1538UL, ExTraceGlobals.SchedulerTracer, "Resource Unknown {0} : Sleeping for {1} milliseconds, then accepting!", new object[]
					{
						resourceKey2,
						num
					});
					health = ResourceLoadState.Unknown;
					flag = false;
				}
				else
				{
					SyncStoreLoadManager.Instance.SyncLogSession.LogVerbose((TSLID)1539UL, ExTraceGlobals.SchedulerTracer, "Resource Unhealthy {0} : Sleeping for {1} milliseconds, then retrying", new object[]
					{
						resourceKey2,
						num
					});
					health = ResourceLoadState.Critical;
				}
				SyncStoreLoadManager.Sleep(num);
				stopwatch.Stop();
				minValue.Add(stopwatch.Elapsed);
				throttlingInfo.Add(monitor, health, num);
			}
			return throttlingInfo;
		}

		private static bool TryPerformXsoOperation(MailboxSession mailboxSession, string xsoOperationName, Action xsoOperation, EventHandler<RoundtripCompleteEventArgs> roundtripComplete, out Exception exception)
		{
			exception = null;
			try
			{
				SyncStoreLoadManager.PerformXsoOperation(mailboxSession, xsoOperationName, xsoOperation, roundtripComplete);
			}
			catch (SyncStoreUnhealthyException ex)
			{
				exception = ex;
			}
			return exception != null;
		}

		private static void PerformXsoOperation(MailboxSession mailboxSession, string xsoOperationName, Action xsoOperation, EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			SyncStoreLoadManager.PerformXsoOperation(mailboxSession.MdbGuid, mailboxSession.MailboxOwner, xsoOperationName, xsoOperation, roundtripComplete);
		}

		private static void PerformXsoOperation(Guid databaseGuid, SyncStateStorage syncStateStorage, string xsoOperationName, Action xsoOperation, EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			SyncStoreLoadManager.PerformXsoOperation(databaseGuid, syncStateStorage.MailboxOwner, xsoOperationName, xsoOperation, roundtripComplete);
		}

		private static void PerformXsoOperation(Guid databaseGuid, IExchangePrincipal exchangePrincipal, string xsoOperationName, Action xsoOperation, EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			bool roundtripSuccessful = false;
			try
			{
				xsoOperation();
				roundtripSuccessful = true;
			}
			finally
			{
				stopwatch.Stop();
				ThrottlingInfo throttlingInfo = SyncStoreLoadManager.ThrottleStoreCall(exchangePrincipal, databaseGuid);
				if (roundtripComplete != null)
				{
					SyncDB orCreateSyncDB = SyncStoreLoadManager.Instance.GetOrCreateSyncDB(databaseGuid);
					orCreateSyncDB.NotifyStoreRoundtripComplete(xsoOperationName, roundtripComplete, new RoundtripCompleteEventArgs(stopwatch.Elapsed, throttlingInfo, roundtripSuccessful));
				}
			}
		}

		private IBudget AcquireUnthrottledBudget(string budgetKey)
		{
			return StandardBudget.Acquire(new UnthrottledBudgetKey(budgetKey, BudgetType.ResourceTracking));
		}

		private int GetRandomDelay(int backoffDelay)
		{
			int result;
			lock (SyncStoreLoadManager.syncObject)
			{
				result = this.randomDelay.Next(1, backoffDelay);
			}
			return result;
		}

		private SyncResourceMonitorType IsAnyResourceUnhealthyOrUnknown(IEnumerable<SyncResource> syncResources, out bool isAnyResourceUnhealthy, out bool isAnyResourceUnknown)
		{
			isAnyResourceUnhealthy = false;
			isAnyResourceUnknown = false;
			List<SyncResourceMonitor> list = new List<SyncResourceMonitor>();
			foreach (SyncResource syncResource in syncResources)
			{
				list.AddRange(syncResource.GetResourceMonitors());
			}
			return SyncResourceMonitor.IsAnyResourceUnhealthyOrUnknown(null, list, out isAnyResourceUnhealthy, out isAnyResourceUnknown);
		}

		private IEnumerable<SyncResource> GetOrCreateSyncResources(AggregationWorkItem workItem)
		{
			List<SyncResource> list = (List<SyncResource>)this.GetOrCreateSyncResources(workItem.MailboxServerGuid, workItem.MailboxServer, workItem.DatabaseGuid);
			if (workItem.AggregationType == AggregationType.Aggregation)
			{
				list.Add(this.GetOrCreateSyncTransportServer());
			}
			return list;
		}

		private IEnumerable<SyncResource> GetOrCreateSyncResources(Guid mailboxServerGuid, string mailboxServer, Guid databaseGuid)
		{
			return new List<SyncResource>
			{
				this.GetOrCreateSyncMailboxServer(mailboxServerGuid, mailboxServer),
				this.GetOrCreateSyncDB(databaseGuid)
			};
		}

		private ResourceKey[] GetOrCreateSyncResourceKeys(IEnumerable<SyncResource> syncResources)
		{
			ResourceKey[] array = null;
			string combinedResourcesId = this.GetCombinedResourcesId(syncResources);
			lock (SyncStoreLoadManager.syncObject)
			{
				if (this.monitoredResourcesResourcesKey.ContainsKey(combinedResourcesId))
				{
					array = this.monitoredResourcesResourcesKey[combinedResourcesId];
				}
				else
				{
					array = this.GetSyncResourcesKey(syncResources);
					this.monitoredResourcesResourcesKey.Add(combinedResourcesId, array);
				}
			}
			return array;
		}

		private string GetCombinedResourcesId(IEnumerable<SyncResource> syncResources)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (SyncResource syncResource in syncResources)
			{
				string resourceId = syncResource.ResourceId;
				stringBuilder.Append(resourceId);
			}
			return stringBuilder.ToString();
		}

		private ResourceKey[] GetSyncResourcesKey(IEnumerable<SyncResource> syncResources)
		{
			List<ResourceKey> list = new List<ResourceKey>();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (SyncResource syncResource in syncResources)
			{
				SyncResourceMonitor[] resourceMonitors = syncResource.GetResourceMonitors();
				foreach (SyncResourceMonitor syncResourceMonitor in resourceMonitors)
				{
					ResourceKey resourceKey = syncResourceMonitor.ResourceKey;
					if (resourceKey != null)
					{
						list.Add(resourceKey);
						stringBuilder.Append(resourceKey);
						stringBuilder.Append("/");
					}
				}
			}
			SyncStoreLoadManager.Instance.SyncLogSession.LogDebugging((TSLID)1524UL, ExTraceGlobals.SchedulerTracer, "GetSyncResourcesKey Count {0} - value {1}", new object[]
			{
				list.Count,
				stringBuilder
			});
			return list.ToArray();
		}

		private int GetDelay(ResourceKey[] syncResourceKeys, out ResourceKey offendingResource)
		{
			offendingResource = null;
			DelayInfo delay = ResourceLoadDelayInfo.GetDelay(this.syncBudget, SyncStoreLoadManager.workloadSettings, syncResourceKeys, true);
			if (delay.Delay > TimeSpan.Zero)
			{
				ResourceLoadDelayInfo resourceLoadDelayInfo = delay as ResourceLoadDelayInfo;
				if (resourceLoadDelayInfo != null)
				{
					offendingResource = resourceLoadDelayInfo.ResourceKey;
				}
			}
			if (delay.Delay.TotalMilliseconds < 2147483647.0)
			{
				return (int)delay.Delay.TotalMilliseconds;
			}
			return int.MaxValue;
		}

		private static readonly Guid TransportServerMonitorKey = Guid.NewGuid();

		private static readonly string TransportSyncBudgetKey = "TransportSyncBudget";

		private static readonly WorkloadSettings workloadSettings = new WorkloadSettings(WorkloadType.TransportSync, true);

		private static readonly object syncObject = new object();

		private static readonly ushort NumberOfPercentWIInStoreSamples = 100;

		private static readonly int BackOffForUnknownHealth = 10;

		private static readonly int MaxBackOffValue = 1000;

		private static readonly int BackOffOverride = 0;

		private static SyncStoreLoadManager singleton = null;

		private SyncLogSession syncLogSession;

		private RunningAverageFloat storeLatencyAverage;

		private RunningAverageFloat cloudLatencyAverage;

		private RunningAverageFloat storeCloudRatioAverage;

		private Dictionary<string, ResourceKey[]> monitoredResourcesResourcesKey;

		private EventWaitHandle sleepWaitHandle;

		private Dictionary<Guid, SyncResource> resourceMonitoringDictionary;

		private IBudget syncBudget;

		private Random randomDelay;

		private volatile bool isRunning;
	}
}
