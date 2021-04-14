using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport.Common;
using Microsoft.Exchange.Transport.Storage;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Transport.RemoteDelivery
{
	internal class RemoteDeliveryComponent : IStartableTransportComponent, ITransportComponent, IDiagnosable, IQueueQuotaObservableComponent
	{
		public RemoteDeliveryComponent()
		{
			this.connectionManager = new ConnectionManager();
			if (MultiTenantTransport.MultiTenancyEnabled)
			{
				this.healthTracker = new RemoteDeliveryHealthTracker(Components.TransportAppConfig.RemoteDelivery.RefreshIntervalToUpdateHealth, Components.TransportAppConfig.RemoteDelivery.MessageThresholdToUpdateHealthCounters, new RemoteDeliveryHealthPerformanceCountersWrapper());
			}
		}

		public event Action<TransportMailItem> OnAcquire;

		public event Action<TransportMailItem> OnRelease;

		public event Action<RoutedMailItem> OnAcquireRoutedMailItem;

		public event Action<RoutedMailItem> OnReleaseRoutedMailItem;

		public static UnreachableMessageQueue UnreachableMessageQueue
		{
			get
			{
				return UnreachableMessageQueue.Instance;
			}
		}

		public string CurrentState
		{
			get
			{
				return string.Empty;
			}
		}

		public ConnectionManager ConnectionManager
		{
			get
			{
				return this.connectionManager;
			}
		}

		public WaitConditionManager ConditionManager
		{
			get
			{
				return this.conditionManager;
			}
		}

		public int TotalQueuedMessages
		{
			get
			{
				int count;
				lock (this.allMailItems)
				{
					count = this.allMailItems.Count;
				}
				return count;
			}
		}

		public int? MessagesCompletingCategorization
		{
			get
			{
				if (this.totalPerfCountersInstance == null)
				{
					return null;
				}
				return new int?((int)this.totalPerfCountersInstance.MessagesCompletingCategorization.RawValue);
			}
		}

		public object SyncQueues
		{
			get
			{
				return this.syncObject;
			}
		}

		public bool IsPaused
		{
			get
			{
				return this.paused;
			}
		}

		public void Load()
		{
			if (Components.IsBridgehead && !Components.Configuration.LocalServer.TransportServer.MaxOutboundConnections.IsUnlimited && (Components.TransportAppConfig.ThrottlingConfig.DeliveryTenantThrottlingEnabled || Components.TransportAppConfig.ThrottlingConfig.DeliverySenderThrottlingEnabled))
			{
				this.conditionManager = new MultiQueueWaitConditionManager(Components.Configuration.LocalServer.TransportServer.MaxOutboundConnections.Value, Components.TransportAppConfig.ThrottlingConfig.GetConfig(false), new CostFactory(), null, null, ExTraceGlobals.QueuingTracer, new GetQueueDelegate(this.GetQueue));
			}
			this.endToEndLatencyBuckets = new E2ELatencyBucketsPerfCountersWrapper();
			ExTraceGlobals.GeneralTracer.TraceDebug(0L, "Loaded remote delivery component.");
		}

		public void Unload()
		{
			this.endToEndLatencyBuckets.Reset();
			ExTraceGlobals.GeneralTracer.TraceDebug(0L, "Unloaded remote delivery component.");
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		public void Start(bool initiallyPaused, ServiceState targetRunningState)
		{
			ExTraceGlobals.GeneralTracer.TraceDebug(0L, "Starting remote delivery component.");
			if (initiallyPaused)
			{
				this.Pause();
			}
			ExTraceGlobals.GeneralTracer.TraceDebug(0L, "Started remote delivery component.");
		}

		public void Stop()
		{
			ExTraceGlobals.GeneralTracer.TraceDebug(0L, "Stoping remote delivery component.");
			this.Pause();
			ExTraceGlobals.GeneralTracer.TraceDebug(0L, "Stopped remote delivery component.");
		}

		public virtual void Pause()
		{
			this.paused = true;
			ExTraceGlobals.PickupTracer.TraceDebug(0L, "Paused remote delivery component.");
		}

		public virtual void Continue()
		{
			this.paused = false;
			ExTraceGlobals.PickupTracer.TraceDebug(0L, "Resumed remote delivery component.");
		}

		public void SetPerfCounters(QueuingPerfCountersInstance totalCounters)
		{
			this.totalPerfCountersInstance = totalCounters;
		}

		public RoutedMessageQueue GetQueue(NextHopSolutionKey key)
		{
			RoutedMessageQueue result;
			if (!this.rmq.TryGetValue(key, out result))
			{
				ExTraceGlobals.QueuingTracer.TraceDebug<Guid, string>(0L, "Queue {0} ({1}) not found", key.NextHopConnector, key.NextHopDomain);
				return null;
			}
			return result;
		}

		public TransportMailItem GetMailItem(long mailItemId)
		{
			TransportMailItem result;
			lock (this.allMailItems)
			{
				TransportMailItem transportMailItem;
				result = ((!this.allMailItems.TryGetValue(mailItemId, out transportMailItem)) ? null : transportMailItem);
			}
			return result;
		}

		public RoutedMessageQueue[] GetQueueArray()
		{
			RoutedMessageQueue[] result;
			lock (this.SyncQueues)
			{
				RoutedMessageQueue[] array = new RoutedMessageQueue[this.rmq.Count];
				this.rmq.Values.CopyTo(array, 0);
				result = array;
			}
			return result;
		}

		public E2ELatencyBucketsPerfCountersWrapper GetEndToEndLatencyBuckets()
		{
			if (this.endToEndLatencyBuckets == null)
			{
				throw new InvalidOperationException("RemoteDeliveryComponent has not been loaded yet");
			}
			return this.endToEndLatencyBuckets;
		}

		public void VisitMailItems(Func<TransportMailItem, bool> visitor)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException("visitor");
			}
			lock (this.allMailItems)
			{
				foreach (TransportMailItem arg in this.allMailItems.Values)
				{
					if (!visitor(arg))
					{
						break;
					}
				}
			}
		}

		public void UpdateQueues()
		{
			RemoteDeliveryComponent.UnreachableMessageQueue.TimedUpdate();
			IEnumerable<RoutedMessageQueue> source = this.UpdateRoutedMessageQueues();
			foreach (RoutedMessageQueue routedMessageQueue in from queue in source
			orderby queue.TotalConnections, queue.LastDeliveryTime
			select queue)
			{
				this.connectionManager.CreateConnectionIfNecessary(routedMessageQueue);
			}
		}

		public void QueueMessageForNextHop(TransportMailItem transportMailItem)
		{
			this.AcquireMailItem(transportMailItem);
			bool flag = true;
			Dictionary<NextHopSolutionKey, NextHopSolution> nextHopSolutions = transportMailItem.NextHopSolutions;
			this.UpdateCounters(transportMailItem, delegate(QueuingPerfCountersInstance c)
			{
				c.ItemsQueuedForDeliveryTotal.Increment();
			});
			foreach (KeyValuePair<NextHopSolutionKey, NextHopSolution> keyValuePair in nextHopSolutions)
			{
				if (keyValuePair.Key.NextHopType.DeliveryType != DeliveryType.ShadowRedundancy)
				{
					RoutedMailItem routedMailItem = new RoutedMailItem(transportMailItem, keyValuePair.Key);
					LatencyTracker.BeginTrackLatency(LatencyTracker.GetDeliveryQueueLatencyComponent(keyValuePair.Key.NextHopType.DeliveryType), routedMailItem.LatencyTracker);
					if (keyValuePair.Key.NextHopType.DeliveryType == DeliveryType.Unreachable)
					{
						Components.ShadowRedundancyComponent.ShadowRedundancyManager.NotifyMailItemPreEnqueuing(transportMailItem, RemoteDeliveryComponent.UnreachableMessageQueue);
						RemoteDeliveryComponent.UnreachableMessageQueue.Enqueue(routedMailItem);
						flag = true;
						ExTraceGlobals.QueuingTracer.TraceDebug<long>(0L, "Message {0} enqueued in the unreachable queue", transportMailItem.RecordId);
					}
					else
					{
						RoutedMessageQueue routedMessageQueue;
						lock (this.SyncQueues)
						{
							if (!this.rmq.TryGetValue(keyValuePair.Key, out routedMessageQueue))
							{
								routedMessageQueue = RoutedMessageQueue.NewQueue(keyValuePair.Key, this.conditionManager, string.IsNullOrEmpty(keyValuePair.Key.OverrideSource) ? null : routedMailItem.OrganizationId);
								this.AddToQueueAddRemoveTrace("[CreateQueue] Thread={0}, QueueId={1}, Key={2}, NextHopDomain={3}", new object[]
								{
									Thread.CurrentThread.ManagedThreadId,
									routedMessageQueue.Id,
									routedMessageQueue.Key,
									routedMessageQueue.NextHopDomain
								});
								this.SendWatsonOnDuplicate(routedMessageQueue);
								this.rmq[keyValuePair.Key] = routedMessageQueue;
								this.ids[routedMessageQueue.Id] = routedMessageQueue;
								routedMessageQueue.OnAcquire += this.OnAcquireInternal;
								routedMessageQueue.OnRelease += this.OnReleaseInternal;
							}
							routedMessageQueue.AddReference();
						}
						Components.ShadowRedundancyComponent.ShadowRedundancyManager.NotifyMailItemPreEnqueuing(transportMailItem, routedMessageQueue);
						routedMessageQueue.Enqueue(routedMailItem);
						routedMailItem.UpdateE2ELatencyBucketsOnEnqueue();
						routedMessageQueue.ReleaseReference();
						bool flag3;
						if (!QueueManager.ShouldDehydrateMessage(routedMessageQueue, routedMailItem, out flag3))
						{
							flag = false;
						}
						if (routedMessageQueue.TotalConnections == 0 && flag3)
						{
							this.ConnectionManager.CreateConnectionIfNecessary(routedMessageQueue, ((IQueueItem)transportMailItem).Priority);
						}
					}
				}
			}
			if (flag)
			{
				try
				{
					transportMailItem.CommitLazyAndDehydrateMessageIfPossible(Breadcrumb.DehydrateOnRoutingDone);
				}
				catch (EsentErrorException arg)
				{
					ExTraceGlobals.QueuingTracer.TraceError<int, EsentErrorException>(0L, "Dehydration attempt for {0} failed with {1}", transportMailItem.GetHashCode(), arg);
				}
			}
		}

		private void SendWatsonOnDuplicate(RoutedMessageQueue routedMessageQueue)
		{
			if (this.ids.ContainsKey(routedMessageQueue.Id))
			{
				this.AddToQueueAddRemoveTrace("[Duplicate] Thread={0}, QueueId={1}, Key={2}, NextHopDomain={3}", new object[]
				{
					Thread.CurrentThread.ManagedThreadId,
					routedMessageQueue.Id,
					routedMessageQueue.Key,
					routedMessageQueue.NextHopDomain
				});
				foreach (RoutedMessageQueue routedMessageQueue2 in this.ids.Values)
				{
					this.AddToQueueAddRemoveTrace("[LookById] QueueId={0}, Key={1}, NextHopDomain={2}", new object[]
					{
						routedMessageQueue2.Id,
						routedMessageQueue2.Key,
						routedMessageQueue2.NextHopDomain
					});
				}
				foreach (RoutedMessageQueue routedMessageQueue3 in this.rmq.Values)
				{
					this.AddToQueueAddRemoveTrace("[LookByNextHop] QueueId={0}, Key={1}, NextHopDomain={2}", new object[]
					{
						routedMessageQueue3.Id,
						routedMessageQueue3.Key,
						routedMessageQueue3.NextHopDomain
					});
				}
				string stackTrace = Environment.StackTrace;
				ExWatson.SendGenericWatsonReport("E12", ExWatson.ApplicationVersion.ToString(), ExWatson.AppName, "15.00.1497.012", Assembly.GetExecutingAssembly().GetName().Name, "System.InvalidOperationException", stackTrace, stackTrace.GetHashCode().ToString(), "RemoteDeliveryComponent.SendWatsonOnDuplicate", string.Format("Duplicate queue found. More info:{0}{1}", Environment.NewLine, this.queueAddRemoveTrace));
			}
		}

		private void AddToQueueAddRemoveTrace(string format, params object[] args)
		{
			this.queueAddRemoveTrace.AppendLine(string.Format("[{0}]{1}", DateTime.UtcNow, string.Format(format, args)));
		}

		public void AcquireMailItem(TransportMailItem mailItem)
		{
			if (this.OnAcquire != null)
			{
				this.OnAcquire(mailItem);
			}
			mailItem.SetQueuedForDelivery(true);
			lock (this.allMailItems)
			{
				TransportHelpers.AttemptAddToDictionary<long, TransportMailItem>(this.allMailItems, mailItem.RecordId, mailItem, null);
			}
			this.UpdateCountersForAcquireMailItem(mailItem);
		}

		public void ReleaseMailItem(TransportMailItem mailItem)
		{
			lock (this.allMailItems)
			{
				this.allMailItems.Remove(mailItem.RecordId);
			}
			mailItem.SetQueuedForDelivery(false);
			if (this.OnRelease != null)
			{
				this.OnRelease(mailItem);
			}
			this.UpdateCountersForReleasedMailItem(mailItem);
		}

		private void UpdateCountersForAcquireMailItem(TransportMailItem mailItem)
		{
			if (this.totalPerfCountersInstance != null)
			{
				this.totalPerfCountersInstance.MessagesQueuedForDelivery.Increment();
				this.totalPerfCountersInstance.MessagesQueuedForDeliveryTotal.Increment();
			}
		}

		private void UpdateCountersForReleasedMailItem(TransportMailItem mailItem)
		{
			if (this.totalPerfCountersInstance != null)
			{
				this.totalPerfCountersInstance.MessagesQueuedForDelivery.Decrement();
				this.totalPerfCountersInstance.MessagesCompletedDeliveryTotal.Increment();
			}
		}

		private void UpdateCounters(TransportMailItem item, Action<QueuingPerfCountersInstance> updateCounter)
		{
			if (updateCounter == null)
			{
				throw new InvalidOperationException("No update action provided");
			}
			foreach (KeyValuePair<NextHopSolutionKey, NextHopSolution> keyValuePair in item.NextHopSolutions)
			{
				Components.QueueManager.UpateInstanceCounter(keyValuePair.Key.RiskLevel, item.Priority, updateCounter);
			}
		}

		public virtual void CommitLazyAndDehydrateMessages()
		{
			this.VisitMailItems(delegate(TransportMailItem mailItem)
			{
				mailItem.CommitLazyAndDehydrateMessageIfPossible(Breadcrumb.DehydrateOnBackPressure);
				return true;
			});
		}

		public List<RoutedMessageQueue> FindByQueueIdentity(QueueIdentity queueIdentity)
		{
			if (!queueIdentity.IsLocal || queueIdentity.Type != QueueType.Delivery)
			{
				return new List<RoutedMessageQueue>(0);
			}
			if (queueIdentity.RowId > 0L)
			{
				List<RoutedMessageQueue> list = new List<RoutedMessageQueue>(1);
				RoutedMessageQueue routedMessageQueue = this.FindById(queueIdentity.RowId);
				if (routedMessageQueue != null && routedMessageQueue.IsAdminVisible)
				{
					list.Add(routedMessageQueue);
				}
				return list;
			}
			return this.FindByDomain(queueIdentity.NextHopDomain);
		}

		public void LoadQueue(RoutedQueueBase queueStorage)
		{
			RoutedMessageQueue routedMessageQueue = RoutedMessageQueue.LoadQueue(queueStorage, this.conditionManager);
			if (routedMessageQueue != null)
			{
				routedMessageQueue.OnAcquire += this.OnAcquireInternal;
				routedMessageQueue.OnRelease += this.OnReleaseInternal;
				this.AddToQueueAddRemoveTrace("[LoadQueue] Thread={0}, QueueId={1}, Key={2}, NextHopDomain={3}", new object[]
				{
					Thread.CurrentThread.ManagedThreadId,
					routedMessageQueue.Id,
					routedMessageQueue.Key,
					routedMessageQueue.NextHopDomain
				});
				this.SendWatsonOnDuplicate(routedMessageQueue);
				this.ids[routedMessageQueue.Id] = routedMessageQueue;
				this.rmq[routedMessageQueue.Key] = routedMessageQueue;
			}
		}

		private void OnAcquireInternal(RoutedMailItem routedMailItem)
		{
			if (this.OnAcquireRoutedMailItem != null)
			{
				this.OnAcquireRoutedMailItem(routedMailItem);
			}
		}

		private void OnReleaseInternal(RoutedMailItem routedMailItem)
		{
			if (this.OnReleaseRoutedMailItem != null)
			{
				this.OnReleaseRoutedMailItem(routedMailItem);
			}
		}

		private RoutedMessageQueue FindById(long id)
		{
			RoutedMessageQueue result;
			lock (this.SyncQueues)
			{
				RoutedMessageQueue routedMessageQueue;
				result = (this.ids.TryGetValue(id, out routedMessageQueue) ? routedMessageQueue : null);
			}
			return result;
		}

		private List<RoutedMessageQueue> FindByDomain(string domain)
		{
			List<RoutedMessageQueue> list = new List<RoutedMessageQueue>();
			lock (this.SyncQueues)
			{
				foreach (RoutedMessageQueue routedMessageQueue in this.ids.Values)
				{
					if (string.Equals(domain, routedMessageQueue.NextHopDomain, StringComparison.OrdinalIgnoreCase) && routedMessageQueue.IsAdminVisible)
					{
						list.Add(routedMessageQueue);
					}
				}
			}
			return list;
		}

		private IEnumerable<RoutedMessageQueue> UpdateRoutedMessageQueues()
		{
			RoutedMessageQueue[] queues = this.GetQueueArray();
			int[] totalExternalActiveRemoteDelivery = new int[QueueManager.InstanceCountersLength];
			int[] totalInternalActiveRemoteDelivery = new int[QueueManager.InstanceCountersLength];
			int[] totalExternalRetryRemoteDelivery = new int[QueueManager.InstanceCountersLength];
			int[] totalInternalRetryRemoteDelivery = new int[QueueManager.InstanceCountersLength];
			int[] totalActiveMailboxDelivery = new int[QueueManager.InstanceCountersLength];
			int[] totalRetryMailboxDelivery = new int[QueueManager.InstanceCountersLength];
			int[] totalActiveNonSmtpDelivery = new int[QueueManager.InstanceCountersLength];
			int[] totalRetryNonSmtpDelivery = new int[QueueManager.InstanceCountersLength];
			int[] maxInternalQueueLength = new int[QueueManager.InstanceCountersLength];
			int[] maxExternalQueueLength = new int[QueueManager.InstanceCountersLength];
			int maxTotalInternalQueueLength = 0;
			int maxTotalInternalUnlockedQueueLength = 0;
			int maxTotalExternalQueueLength = 0;
			int maxTotalExternalUnlockedQueueLength = 0;
			bool healthUpdateNeeded = this.healthTracker != null && this.healthTracker.StartRefresh();
			for (int i = 0; i < queues.Length; i++)
			{
				queues[i].UpdateQueue();
				int[] activeCountForPerfCounter;
				int[] retryCountForPerfCounter;
				queues[i].GetQueueCounts(out activeCountForPerfCounter, out retryCountForPerfCounter);
				if (NextHopType.IsMailboxDeliveryType(queues[i].Key.NextHopType.DeliveryType))
				{
					totalActiveMailboxDelivery = totalActiveMailboxDelivery.Zip(activeCountForPerfCounter, (int a, int b) => a + b).ToArray<int>();
					totalRetryMailboxDelivery = totalRetryMailboxDelivery.Zip(retryCountForPerfCounter, (int a, int b) => a + b).ToArray<int>();
				}
				else if (queues[i].Key.NextHopType.DeliveryType == DeliveryType.NonSmtpGatewayDelivery)
				{
					totalActiveNonSmtpDelivery = totalActiveNonSmtpDelivery.Zip(activeCountForPerfCounter, (int a, int b) => a + b).ToArray<int>();
					totalRetryNonSmtpDelivery = totalRetryNonSmtpDelivery.Zip(retryCountForPerfCounter, (int a, int b) => a + b).ToArray<int>();
				}
				else if (queues[i].Key.NextHopType.DeliveryType != DeliveryType.Heartbeat)
				{
					if (queues[i].Key.NextHopType.NextHopCategory == NextHopCategory.External)
					{
						totalExternalActiveRemoteDelivery = totalExternalActiveRemoteDelivery.Zip(activeCountForPerfCounter, (int a, int b) => a + b).ToArray<int>();
						totalExternalRetryRemoteDelivery = totalExternalRetryRemoteDelivery.Zip(retryCountForPerfCounter, (int a, int b) => a + b).ToArray<int>();
					}
					else
					{
						totalInternalActiveRemoteDelivery = totalInternalActiveRemoteDelivery.Zip(activeCountForPerfCounter, (int a, int b) => a + b).ToArray<int>();
						totalInternalRetryRemoteDelivery = totalInternalRetryRemoteDelivery.Zip(retryCountForPerfCounter, (int a, int b) => a + b).ToArray<int>();
					}
				}
				if (queues[i].Key.NextHopType.NextHopCategory == NextHopCategory.External)
				{
					maxTotalExternalQueueLength = Math.Max(maxTotalExternalQueueLength, Components.QueueManager.GetTotalFromInstance(activeCountForPerfCounter) + Components.QueueManager.GetTotalFromInstance(retryCountForPerfCounter));
					maxTotalExternalUnlockedQueueLength = Math.Max(maxTotalExternalUnlockedQueueLength, queues[i].ActiveCount);
					maxExternalQueueLength = maxExternalQueueLength.Zip(activeCountForPerfCounter.Zip(retryCountForPerfCounter, (int a, int b) => a + b), (int a, int b) => Math.Max(a, b)).ToArray<int>();
				}
				else
				{
					maxTotalInternalQueueLength = Math.Max(maxTotalInternalQueueLength, Components.QueueManager.GetTotalFromInstance(activeCountForPerfCounter) + Components.QueueManager.GetTotalFromInstance(retryCountForPerfCounter));
					maxTotalInternalUnlockedQueueLength = Math.Max(maxTotalInternalUnlockedQueueLength, queues[i].ActiveCount);
					maxInternalQueueLength = maxInternalQueueLength.Zip(activeCountForPerfCounter.Zip(retryCountForPerfCounter, (int a, int b) => a + b), (int a, int b) => Math.Max(a, b)).ToArray<int>();
				}
				if (healthUpdateNeeded)
				{
					this.healthTracker.UpdateHealthUsingQueueData(queues[i]);
				}
				if (queues[i].CanResubmit(Components.TransportAppConfig.RemoteDelivery.MaxIdleTimeBeforeResubmit))
				{
					queues[i].ResetScheduledCallback();
					queues[i].Resubmit(ResubmitReason.Inactivity, null);
				}
				else if (queues[i].ActiveQueueLength > 0)
				{
					yield return queues[i];
				}
				else if (queues[i].CanBeDeleted(Components.Configuration.LocalServer.TransportServer.QueueMaxIdleTime))
				{
					lock (this.SyncQueues)
					{
						if (queues[i].CanBeDeleted(Components.Configuration.LocalServer.TransportServer.QueueMaxIdleTime))
						{
							this.AddToQueueAddRemoveTrace("[RemoveQueue_NextHop] Thread={0}, QueueId={1}, Key={2}, NextHopDomain={3}", new object[]
							{
								Thread.CurrentThread.ManagedThreadId,
								queues[i].Id,
								queues[i].Key,
								queues[i].NextHopDomain
							});
							this.rmq.Remove(queues[i].Key);
							queues[i].OnAcquire -= this.OnAcquireInternal;
							queues[i].OnRelease -= this.OnReleaseInternal;
							queues[i].ResetScheduledCallback();
							queues[i].Delete();
							this.AddToQueueAddRemoveTrace("[RemoveQueue_Ids] Thread={0}, QueueId={1}, Key={2}, NextHopDomain={3}", new object[]
							{
								Thread.CurrentThread.ManagedThreadId,
								queues[i].Id,
								queues[i].Key,
								queues[i].NextHopDomain
							});
							this.ids.Remove(queues[i].Id);
						}
					}
				}
			}
			if (healthUpdateNeeded)
			{
				this.healthTracker.CompleteRefresh();
			}
			if (this.totalPerfCountersInstance != null)
			{
				Components.QueueManager.UpdateAllInstanceCounters(totalActiveMailboxDelivery, delegate(QueuingPerfCountersInstance c, int v)
				{
					c.ActiveMailboxDeliveryQueueLength.RawValue = (long)v;
				});
				Components.QueueManager.UpdateAllInstanceCounters(totalRetryMailboxDelivery, delegate(QueuingPerfCountersInstance c, int v)
				{
					c.RetryMailboxDeliveryQueueLength.RawValue = (long)v;
				});
				Components.QueueManager.UpdateAllInstanceCounters(totalActiveNonSmtpDelivery, delegate(QueuingPerfCountersInstance c, int v)
				{
					c.ActiveNonSmtpDeliveryQueueLength.RawValue = (long)v;
				});
				Components.QueueManager.UpdateAllInstanceCounters(totalRetryNonSmtpDelivery, delegate(QueuingPerfCountersInstance c, int v)
				{
					c.RetryNonSmtpDeliveryQueueLength.RawValue = (long)v;
				});
				Components.QueueManager.UpdateAllInstanceCounters(totalInternalActiveRemoteDelivery, delegate(QueuingPerfCountersInstance c, int v)
				{
					c.InternalActiveRemoteDeliveryQueueLength.RawValue = (long)v;
				});
				Components.QueueManager.UpdateAllInstanceCounters(totalExternalActiveRemoteDelivery, delegate(QueuingPerfCountersInstance c, int v)
				{
					c.ExternalActiveRemoteDeliveryQueueLength.RawValue = (long)v;
				});
				Components.QueueManager.UpdateAllInstanceCounters(totalExternalRetryRemoteDelivery, delegate(QueuingPerfCountersInstance c, int v)
				{
					c.ExternalRetryRemoteDeliveryQueueLength.RawValue = (long)v;
				});
				Components.QueueManager.UpdateAllInstanceCounters(totalInternalRetryRemoteDelivery, delegate(QueuingPerfCountersInstance c, int v)
				{
					c.InternalRetryRemoteDeliveryQueueLength.RawValue = (long)v;
				});
				Components.QueueManager.UpdateAllInstanceCounters(totalActiveMailboxDelivery.Zip(totalRetryMailboxDelivery, (int a, int b) => a + b).Zip(totalActiveNonSmtpDelivery, (int a, int b) => a + b).Zip(totalRetryNonSmtpDelivery, (int a, int b) => a + b).Zip(totalInternalActiveRemoteDelivery, (int a, int b) => a + b).Zip(totalInternalRetryRemoteDelivery, (int a, int b) => a + b).ToArray<int>(), delegate(QueuingPerfCountersInstance c, int v)
				{
					c.InternalAggregateDeliveryQueueLength.RawValue = (long)v;
				}, true);
				Components.QueueManager.UpdateAllInstanceCounters(totalExternalActiveRemoteDelivery.Zip(totalExternalRetryRemoteDelivery, (int a, int b) => a + b).ToArray<int>(), delegate(QueuingPerfCountersInstance c, int v)
				{
					c.ExternalAggregateDeliveryQueueLength.RawValue = (long)v;
				}, true);
				Components.QueueManager.UpdateAllInstanceCounters(maxExternalQueueLength, delegate(QueuingPerfCountersInstance c, int v)
				{
					c.ExternalLargestDeliveryQueueLength.RawValue = (long)v;
				}, false);
				Components.QueueManager.UpdateAllInstanceCounters(maxInternalQueueLength, delegate(QueuingPerfCountersInstance c, int v)
				{
					c.InternalLargestDeliveryQueueLength.RawValue = (long)v;
				}, false);
				this.totalPerfCountersInstance.InternalLargestDeliveryQueueLength.RawValue = (long)maxTotalInternalQueueLength;
				this.totalPerfCountersInstance.ExternalLargestDeliveryQueueLength.RawValue = (long)maxTotalExternalQueueLength;
				this.totalPerfCountersInstance.InternalLargestUnlockedDeliveryQueueLength.RawValue = (long)maxTotalInternalUnlockedQueueLength;
				this.totalPerfCountersInstance.ExternalLargestUnlockedDeliveryQueueLength.RawValue = (long)maxTotalExternalUnlockedQueueLength;
			}
			yield break;
		}

		string IDiagnosable.GetDiagnosticComponentName()
		{
			return "RemoteDelivery";
		}

		XElement IDiagnosable.GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement xelement = new XElement(((IDiagnosable)this).GetDiagnosticComponentName());
			bool flag = parameters.Argument.IndexOf("verbose", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag2 = parameters.Argument.IndexOf("conditionalQueuing", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag3 = parameters.Argument.IndexOf("config", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag4 = parameters.Argument.IndexOf("diversity", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag5 = (!flag3 && !flag4) || parameters.Argument.IndexOf("help", StringComparison.OrdinalIgnoreCase) != -1;
			if (flag3)
			{
				XElement xelement2 = new XElement("config");
				xelement2.Add(TransportAppConfig.GetDiagnosticInfoForType(Components.TransportAppConfig.RemoteDelivery));
				xelement2.Add(TransportAppConfig.GetDiagnosticInfoForType(Components.TransportAppConfig.DeliveryQueuePrioritizationConfiguration));
				xelement2.Add(TransportAppConfig.GetDiagnosticInfoForType(Components.TransportAppConfig.ThrottlingConfig));
				xelement.Add(xelement2);
			}
			if (flag2 && this.conditionManager != null)
			{
				xelement.Add(this.conditionManager.GetDiagnosticInfo(flag));
			}
			if (flag)
			{
				RoutedMessageQueue[] queueArray = this.GetQueueArray();
				XElement xelement3 = new XElement("queues");
				foreach (RoutedMessageQueue routedMessageQueue in queueArray)
				{
					xelement3.Add(routedMessageQueue.GetDiagnosticInfo(true, flag2));
				}
				xelement.Add(xelement3);
				if (this.healthTracker != null)
				{
					xelement.Add(this.healthTracker.GetDiagnosticInfo());
				}
			}
			if (flag5)
			{
				xelement.Add(new XElement("help", "Supported arguments: config, conditionalQueuing, verbose, diversity:" + QueueDiversity.UsageString));
			}
			if (flag4)
			{
				string requestArgument = parameters.Argument.Substring(parameters.Argument.IndexOf("diversity", StringComparison.OrdinalIgnoreCase) + "diversity".Length);
				this.GetDiversityDiagnosticInfo(xelement, requestArgument);
			}
			return xelement;
		}

		private void GetDiversityDiagnosticInfo(XElement deliveryElement, string requestArgument)
		{
			QueueDiversity queueDiversity;
			string text;
			if (QueueDiversity.TryParse(requestArgument, false, out queueDiversity, out text))
			{
				QueueType type = queueDiversity.QueueId.Type;
				if (type != QueueType.Delivery)
				{
					if (type == QueueType.Unreachable)
					{
						deliveryElement.Add(queueDiversity.GetDiagnosticInfo(UnreachableMessageQueue.Instance));
					}
					else
					{
						deliveryElement.Add(queueDiversity.GetComponentAdvice());
					}
				}
				else if (this.ids.ContainsKey(queueDiversity.QueueId.RowId))
				{
					deliveryElement.Add(queueDiversity.GetDiagnosticInfo(this.ids[queueDiversity.QueueId.RowId]));
				}
				else
				{
					text = string.Format("Remote Queue doesn't have queue id = '{0}'", queueDiversity.QueueId.RowId);
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				deliveryElement.Add(new XElement("Error", text));
			}
		}

		private readonly Dictionary<NextHopSolutionKey, RoutedMessageQueue> rmq = new Dictionary<NextHopSolutionKey, RoutedMessageQueue>();

		private readonly Dictionary<long, RoutedMessageQueue> ids = new Dictionary<long, RoutedMessageQueue>();

		private readonly object syncObject = new object();

		private readonly StringBuilder queueAddRemoveTrace = new StringBuilder();

		private ConnectionManager connectionManager;

		private MultiQueueWaitConditionManager conditionManager;

		private QueuingPerfCountersInstance totalPerfCountersInstance;

		private E2ELatencyBucketsPerfCountersWrapper endToEndLatencyBuckets;

		private RemoteDeliveryHealthTracker healthTracker;

		private Dictionary<long, TransportMailItem> allMailItems = new Dictionary<long, TransportMailItem>();

		private bool paused;
	}
}
