using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Rpc.QueueViewer;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Exchange.Transport.MessageDepot;
using Microsoft.Exchange.Transport.QueueViewer;
using Microsoft.Exchange.Transport.ShadowRedundancy;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Transport.RemoteDelivery
{
	internal sealed class QueueManager : ITransportComponent
	{
		public static ExEventLog EventLogger
		{
			get
			{
				return QueueManager.eventLogger;
			}
		}

		public static int InstanceCountersLength
		{
			get
			{
				if (QueueManager.instanceCountersLength == -1)
				{
					QueueManager.instanceCountersLength = QueueManager.priorityBasedInstanceCounterNames.Length + (QueueManager.includeRiskBasedCounters ? QueueManager.riskBasedInstanceCounterNames.Length : 0);
				}
				return QueueManager.instanceCountersLength;
			}
		}

		public PoisonMessageQueue PoisonMessageQueue
		{
			get
			{
				return this.poisonMessageQueue;
			}
		}

		public QueuingPerfCountersInstance PerfCountersTotal
		{
			get
			{
				return this.queuingPerfCountersTotalInstance;
			}
		}

		internal static Dictionary<DeliveryPriority, int> PriorityToInstanceIndexMap
		{
			get
			{
				return QueueManager.priorityToInstanceIndexMap;
			}
		}

		public static void StartUpdateAllQueues()
		{
			TimeSpan timeSpan = DateTime.UtcNow - QueueManager.lastQueueUpdate;
			if (timeSpan > Components.Configuration.AppConfig.QueueConfiguration.MaxUpdateQueueBlockedInterval)
			{
				throw new QueueManager.QueueUpdateBlockedException("Queue is not updated for " + timeSpan);
			}
			if (!QueueManager.updateAllQueuesPendingOrBusy)
			{
				QueueManager.updateAllQueuesPendingOrBusy = true;
				ThreadPool.QueueUserWorkItem(new WaitCallback(QueueManager.UpdateAllQueuesCallback));
			}
		}

		public static void UpdateAllQueuesCallback(object state)
		{
			if (Interlocked.CompareExchange(ref QueueManager.busyUpdateAllQueues, 1, 0) == 0)
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest(3708169533U);
				try
				{
					QueueManager.lastQueueUpdate = DateTime.UtcNow;
					Components.CategorizerComponent.UpdateSubmitQueue();
					Components.RemoteDeliveryComponent.UpdateQueues();
					Components.ShadowRedundancyComponent.ShadowRedundancyManager.UpdateQueues();
				}
				finally
				{
					Interlocked.Exchange(ref QueueManager.busyUpdateAllQueues, 0);
					QueueManager.updateAllQueuesPendingOrBusy = false;
				}
			}
		}

		public static bool ShouldDehydrateMessage(RoutedMessageQueue routedMessageQueue, RoutedMailItem routedMailItem, out bool shouldAttemptConnection)
		{
			shouldAttemptConnection = true;
			if (routedMessageQueue.Suspended)
			{
				ExTraceGlobals.QueuingTracer.TraceDebug<long, Guid, string>(0L, "Message {0} may be committed and dehydrated because queue {1} ({2}) is frozen", routedMailItem.RecordId, routedMessageQueue.Key.NextHopConnector, routedMessageQueue.Key.NextHopDomain);
				shouldAttemptConnection = false;
				return true;
			}
			if (routedMessageQueue.RetryConnectionScheduled)
			{
				ExTraceGlobals.QueuingTracer.TraceDebug<long, Guid, string>(0L, "Message {0} may be committed and dehydrated because queue {1} ({2}) is in retry", routedMailItem.RecordId, routedMessageQueue.Key.NextHopConnector, routedMessageQueue.Key.NextHopDomain);
				shouldAttemptConnection = false;
				return true;
			}
			if (Components.ResourceManager.ShouldShrinkDownMemoryCaches)
			{
				ExTraceGlobals.QueuingTracer.TraceDebug<long>(0L, "Message {0} may be committed and dehydrated because of the high memory pressure.", routedMailItem.RecordId);
				shouldAttemptConnection = true;
				return true;
			}
			return false;
		}

		public static bool FreezeQueue(QueueIdentity queueIdentity)
		{
			if (!queueIdentity.IsFullySpecified)
			{
				throw new QueueViewerException(QVErrorCode.QV_E_INCOMPLETE_IDENTITY);
			}
			if (queueIdentity.Type == QueueType.Delivery)
			{
				lock (Components.RemoteDeliveryComponent.SyncQueues)
				{
					List<RoutedMessageQueue> list = Components.RemoteDeliveryComponent.FindByQueueIdentity(queueIdentity);
					if (list.Count == 0)
					{
						return false;
					}
					if (list.Count > 1)
					{
						throw new QueueViewerException(QVErrorCode.QV_E_MULTIPLE_IDENTITY_MATCH);
					}
					RoutedMessageQueue routedMessageQueue = list[0];
					routedMessageQueue.Suspended = true;
					routedMessageQueue.ResetScheduledCallback();
					goto IL_EE;
				}
			}
			if (queueIdentity.Type == QueueType.Submission)
			{
				if (Components.MessageDepotComponent.Enabled)
				{
					Components.ProcessingSchedulerComponent.ProcessingSchedulerAdmin.Pause();
				}
				else
				{
					Components.CategorizerComponent.SubmitMessageQueue.Suspended = true;
				}
			}
			else if (queueIdentity.Type == QueueType.Unreachable)
			{
				RemoteDeliveryComponent.UnreachableMessageQueue.Suspended = true;
			}
			else
			{
				if (queueIdentity.Type != QueueType.Shadow)
				{
					throw new QueueViewerException(QVErrorCode.QV_E_INVALID_OPERATION);
				}
				QueueManager.SuspendShadowQueue(queueIdentity);
			}
			IL_EE:
			ExTraceGlobals.QueuingTracer.TraceDebug<QueueIdentity>(0L, "Queue {0} frozen by the admin", queueIdentity);
			return true;
		}

		public static bool UnfreezeQueue(QueueIdentity queueIdentity)
		{
			if (!queueIdentity.IsFullySpecified)
			{
				throw new QueueViewerException(QVErrorCode.QV_E_INCOMPLETE_IDENTITY);
			}
			if (queueIdentity.Type == QueueType.Delivery)
			{
				RoutedMessageQueue routedMessageQueue;
				lock (Components.RemoteDeliveryComponent.SyncQueues)
				{
					List<RoutedMessageQueue> list = Components.RemoteDeliveryComponent.FindByQueueIdentity(queueIdentity);
					if (list.Count == 0)
					{
						return false;
					}
					if (list.Count > 1)
					{
						throw new QueueViewerException(QVErrorCode.QV_E_MULTIPLE_IDENTITY_MATCH);
					}
					routedMessageQueue = list[0];
					routedMessageQueue.Suspended = false;
				}
				Components.RemoteDeliveryComponent.ConnectionManager.CreateConnectionIfNecessary(routedMessageQueue);
			}
			else if (queueIdentity.Type == QueueType.Submission)
			{
				if (Components.MessageDepotComponent.Enabled)
				{
					Components.ProcessingSchedulerComponent.ProcessingSchedulerAdmin.Resume();
				}
				else
				{
					Components.CategorizerComponent.SubmitMessageQueue.Suspended = false;
				}
			}
			else if (queueIdentity.Type == QueueType.Unreachable)
			{
				RemoteDeliveryComponent.UnreachableMessageQueue.Suspended = false;
			}
			else
			{
				if (queueIdentity.Type != QueueType.Shadow)
				{
					throw new QueueViewerException(QVErrorCode.QV_E_INVALID_OPERATION);
				}
				QueueManager.ResumeShadowQueue(queueIdentity);
			}
			ExTraceGlobals.QueuingTracer.TraceDebug<QueueIdentity>(0L, "Queue {0} unfrozen by the admin", queueIdentity);
			return true;
		}

		public static bool RetryQueue(QueueIdentity queueIdentity, bool resubmit)
		{
			return QueueManager.RetryQueueAsync(queueIdentity, resubmit).Result;
		}

		public static async Task<bool> RetryQueueAsync(QueueIdentity queueIdentity, bool resubmit)
		{
			if (!queueIdentity.IsFullySpecified)
			{
				throw new QueueViewerException(QVErrorCode.QV_E_INCOMPLETE_IDENTITY);
			}
			if (queueIdentity.Type == QueueType.Delivery)
			{
				RoutedMessageQueue routedMessageQueue;
				lock (Components.RemoteDeliveryComponent.SyncQueues)
				{
					List<RoutedMessageQueue> list = Components.RemoteDeliveryComponent.FindByQueueIdentity(queueIdentity);
					if (list.Count == 0)
					{
						return false;
					}
					if (list.Count > 1)
					{
						throw new QueueViewerException(QVErrorCode.QV_E_MULTIPLE_IDENTITY_MATCH);
					}
					routedMessageQueue = list[0];
					if (routedMessageQueue.Suspended)
					{
						return true;
					}
					routedMessageQueue.ResetScheduledCallback();
				}
				if (resubmit)
				{
					ExTraceGlobals.QueuingTracer.TraceDebug<QueueIdentity>(0L, "Queue {0} resubmitted by the admin", queueIdentity);
					await routedMessageQueue.ResubmitAsync(ResubmitReason.Admin, null);
				}
				else
				{
					ExTraceGlobals.QueuingTracer.TraceDebug<QueueIdentity>(0L, "Queue {0} forced into immediate retry by the admin", queueIdentity);
					Components.RemoteDeliveryComponent.ConnectionManager.CreateConnectionIfNecessary(routedMessageQueue);
				}
			}
			else if (queueIdentity.Type == QueueType.Unreachable)
			{
				if (!resubmit)
				{
					throw new QueueViewerException(QVErrorCode.QV_E_INVALID_OPERATION);
				}
				ExTraceGlobals.QueuingTracer.TraceDebug<QueueIdentity>(0L, "Queue {0} resubmitted by the admin", queueIdentity);
				RemoteDeliveryComponent.UnreachableMessageQueue.Resubmit(ResubmitReason.Admin, null);
			}
			else
			{
				if (queueIdentity.Type != QueueType.Shadow)
				{
					throw new QueueViewerException(QVErrorCode.QV_E_INVALID_OPERATION);
				}
				ExTraceGlobals.QueuingTracer.TraceDebug<QueueIdentity, bool>(0L, "Retry-Queue {0} -Resubmit:${1}", queueIdentity, resubmit);
				List<ShadowMessageQueue> list2 = Components.ShadowRedundancyComponent.ShadowRedundancyManager.FindByQueueIdentity(queueIdentity);
				if (list2.Count == 0)
				{
					return false;
				}
				if (list2.Count > 1)
				{
					throw new QueueViewerException(QVErrorCode.QV_E_MULTIPLE_IDENTITY_MATCH);
				}
				if (resubmit)
				{
					list2[0].Resubmit(ResubmitReason.Admin);
				}
				else
				{
					list2[0].ScheduleImmediateHeartbeat();
				}
			}
			return true;
		}

		public static bool FreezeMessage(MessageIdentity mailItemId)
		{
			if (!mailItemId.QueueIdentity.IsFullySpecified)
			{
				throw new QueueViewerException(QVErrorCode.QV_E_INCOMPLETE_IDENTITY);
			}
			if (mailItemId.QueueIdentity.Type == QueueType.Submission)
			{
				bool flag = Components.MessageDepotComponent.Enabled ? QueueManager.SuspendMessageInMessageDepot(mailItemId.InternalId) : Components.CategorizerComponent.SubmitMessageQueue.SuspendMailItem(mailItemId.InternalId);
				if (!flag && Components.CategorizerComponent.CatContainsMailItem(mailItemId.InternalId))
				{
					throw new QueueViewerException(QVErrorCode.QV_E_INVALID_OPERATION);
				}
				return flag;
			}
			else
			{
				if (mailItemId.QueueIdentity.Type != QueueType.Delivery && mailItemId.QueueIdentity.Type != QueueType.Unreachable && mailItemId.QueueIdentity.Type != QueueType.Shadow)
				{
					throw new QueueViewerException(QVErrorCode.QV_E_INVALID_OPERATION);
				}
				TransportMailItem mailItem = Components.RemoteDeliveryComponent.GetMailItem(mailItemId);
				if (mailItem == null)
				{
					mailItem = Components.ShadowRedundancyComponent.ShadowRedundancyManager.GetMailItem(mailItemId);
					if (mailItem == null)
					{
						return false;
					}
				}
				bool flag2 = false;
				NextHopSolution nextHopSolution = null;
				lock (mailItem)
				{
					if (mailItemId.QueueIdentity.Type != QueueType.Shadow)
					{
						if (!mailItem.IsActive)
						{
							return false;
						}
						if (!mailItem.QueuedForDelivery)
						{
							throw new QueueViewerException(QVErrorCode.QV_E_INVALID_OPERATION);
						}
					}
					else if (mailItem.IsRowDeleted)
					{
						return false;
					}
					if (!QueueManager.TryGetMailItemSolution(mailItem, mailItemId.QueueIdentity, out nextHopSolution))
					{
						return false;
					}
					if (nextHopSolution.IsInactive)
					{
						return false;
					}
					if (nextHopSolution.DeliveryStatus == DeliveryStatus.InDelivery && nextHopSolution.IsDeletedByAdmin)
					{
						throw new QueueViewerException(QVErrorCode.QV_E_INVALID_OPERATION);
					}
					if (nextHopSolution.AdminActionStatus == AdminActionStatus.None)
					{
						nextHopSolution.AdminActionStatus = AdminActionStatus.Suspended;
						mailItem.CommitLazy();
						flag2 = true;
						QueueManager.RemoveItemFromDeliveryConditionManager(nextHopSolution, mailItem.Priority);
					}
				}
				if (flag2)
				{
					ExTraceGlobals.QueuingTracer.TraceDebug<MessageIdentity>(0L, "Message {0} frozen by the admin", mailItemId);
				}
				return true;
			}
		}

		public static QueuingPerfCountersInstance GetTotalPerfCounters()
		{
			return QueuingPerfCounters.GetInstance("_Total");
		}

		public static IEnumerable<int> GetInstanceCounterIndex(RiskLevel riskLevel, DeliveryPriority priority)
		{
			List<int> list = new List<int>();
			list.Add(QueueManager.priorityToInstanceIndexMap[priority]);
			int item;
			if (QueueManager.priorityToTotalExcludingPriorityNoneInstanceIndexMap.TryGetValue(priority, out item))
			{
				list.Add(item);
			}
			if (QueueManager.includeRiskBasedCounters)
			{
				list.Add(QueueManager.riskToInstanceIndexMap[riskLevel]);
				if (QueueManager.riskToHighAndBulkRiskTotalInstanceIndexMap.TryGetValue(riskLevel, out item))
				{
					list.Add(item);
				}
				if (QueueManager.riskToNormalAndLowRiskTotalInstanceIndexMap.TryGetValue(riskLevel, out item))
				{
					list.Add(item);
				}
				Tuple<RiskLevel, DeliveryPriority> key = new Tuple<RiskLevel, DeliveryPriority>(riskLevel, priority);
				if (QueueManager.riskAndPriorityInstanceIndexMap.TryGetValue(key, out item))
				{
					list.Add(item);
				}
			}
			return list;
		}

		public static void PreAsyncRetryQueueValidate(QueueIdentity queueIdentity, bool resubmit)
		{
			if (!queueIdentity.IsFullySpecified)
			{
				throw new QueueViewerException(QVErrorCode.QV_E_INCOMPLETE_IDENTITY);
			}
			if (queueIdentity.Type == QueueType.Delivery)
			{
				lock (Components.RemoteDeliveryComponent.SyncQueues)
				{
					List<RoutedMessageQueue> list = Components.RemoteDeliveryComponent.FindByQueueIdentity(queueIdentity);
					if (list.Count == 0)
					{
						throw new QueueViewerException(QVErrorCode.QV_E_OBJECT_NOT_FOUND);
					}
					if (list.Count > 1)
					{
						throw new QueueViewerException(QVErrorCode.QV_E_MULTIPLE_IDENTITY_MATCH);
					}
					return;
				}
			}
			if (queueIdentity.Type == QueueType.Unreachable)
			{
				ExTraceGlobals.QueuingTracer.TraceDebug<bool, bool>(0L, "Unreachable Queue can only be resubmitted while not being suspended: Resubmit = {0}, Suspended = {1}", resubmit, RemoteDeliveryComponent.UnreachableMessageQueue.Suspended);
				if (!resubmit)
				{
					throw new QueueViewerException(QVErrorCode.QV_E_INVALID_OPERATION);
				}
			}
			else
			{
				if (queueIdentity.Type != QueueType.Shadow)
				{
					throw new QueueViewerException(QVErrorCode.QV_E_INVALID_OPERATION);
				}
				ExTraceGlobals.QueuingTracer.TraceDebug<QueueIdentity, bool>(0L, "Retry-Queue {0} -Resubmit:${1}", queueIdentity, resubmit);
				List<ShadowMessageQueue> list2 = Components.ShadowRedundancyComponent.ShadowRedundancyManager.FindByQueueIdentity(queueIdentity);
				if (list2.Count == 0)
				{
					throw new QueueViewerException(QVErrorCode.QV_E_OBJECT_NOT_FOUND);
				}
				if (list2.Count > 1)
				{
					throw new QueueViewerException(QVErrorCode.QV_E_MULTIPLE_IDENTITY_MATCH);
				}
			}
		}

		public void Load()
		{
			if (this.queuingPerfCountersTotalInstance == null)
			{
				this.queuingPerfCountersTotalInstance = QueueManager.GetTotalPerfCounters();
				int num = QueueManager.includeRiskBasedCounters ? (QueueManager.riskBasedInstanceCounterNames.Length + QueueManager.priorityBasedInstanceCounterNames.Length) : QueueManager.priorityBasedInstanceCounterNames.Length;
				this.queuingPerfCountersInstances = new QueuingPerfCountersInstance[num];
				for (int i = 0; i < QueueManager.priorityBasedInstanceCounterNames.Length; i++)
				{
					this.queuingPerfCountersInstances[i] = QueuingPerfCounters.GetInstance(QueueManager.priorityBasedInstanceCounterNames[i]);
				}
				if (QueueManager.includeRiskBasedCounters)
				{
					for (int j = 0; j < QueueManager.riskBasedInstanceCounterNames.Length; j++)
					{
						this.queuingPerfCountersInstances[j + QueueManager.priorityBasedInstanceCounterNames.Length] = QueuingPerfCounters.GetInstance(QueueManager.riskBasedInstanceCounterNames[j]);
					}
				}
				Components.RemoteDeliveryComponent.SetPerfCounters(this.queuingPerfCountersTotalInstance);
			}
			if (this.poisonMessageQueue == null)
			{
				this.poisonMessageQueue = PoisonMessageQueue.Instance;
			}
			this.submissionPerfCounterWrapper = new QueueManager.SubmissionPerfCounterWrapper(this.queuingPerfCountersTotalInstance, Components.TransportAppConfig.QueueConfiguration.RecentPerfCounterTrackingInterval, Components.TransportAppConfig.QueueConfiguration.RecentPerfCounterTrackingBucketSize);
			this.LoadMessageQueues();
			Components.RoutingComponent.MailRouter.RoutingTablesChanged += UnreachableMessageQueue.Instance.RoutingTablesChangedHandler;
			this.queuedRecipientsByAge = new QueuedRecipientsByAgePerfCountersWrapper(Components.TransportAppConfig.QueueConfiguration.QueuedRecipientsByAgeTrackingEnabled);
			SubmitMessageQueue.Instance.OnAcquire += this.GetQueuedRecipientsByAge().TrackEnteringSubmissionQueue;
			SubmitMessageQueue.Instance.OnRelease += this.GetQueuedRecipientsByAge().TrackExitingSubmissionQueue;
			SubmitMessageQueue.Instance.OnAcquire += new Action<TransportMailItem>(this.UpdatePerfCountersOnEnterSubmissionQueue);
			SubmitMessageQueue.Instance.OnRelease += new Action<TransportMailItem>(this.UpdatePerfCountersOnExitSubmissionQueue);
			Components.RemoteDeliveryComponent.OnAcquireRoutedMailItem += this.GetQueuedRecipientsByAge().TrackEnteringDeliveryQueue;
			Components.RemoteDeliveryComponent.OnReleaseRoutedMailItem += this.GetQueuedRecipientsByAge().TrackExitingDeliveryQueue;
		}

		public void Unload()
		{
			if (UnreachableMessageQueue.Instance != null)
			{
				Components.RoutingComponent.MailRouter.RoutingTablesChanged -= UnreachableMessageQueue.Instance.RoutingTablesChangedHandler;
			}
			this.queuedRecipientsByAge.Reset();
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		public bool UnfreezeMessage(MessageIdentity mailItemId)
		{
			if (!mailItemId.QueueIdentity.IsFullySpecified)
			{
				throw new QueueViewerException(QVErrorCode.QV_E_INCOMPLETE_IDENTITY);
			}
			switch (mailItemId.QueueIdentity.Type)
			{
			case QueueType.Delivery:
			case QueueType.Unreachable:
				return QueueManager.UnfreezeRemoteDeliveryMessage(mailItemId);
			case QueueType.Poison:
				return this.UnfreezePoisonMessage(mailItemId);
			case QueueType.Submission:
				return (Components.MessageDepotComponent.Enabled ? QueueManager.ResumeMessageInMessageDepot(mailItemId.InternalId) : Components.CategorizerComponent.SubmitMessageQueue.ResumeMailItem(mailItemId.InternalId)) || Components.CategorizerComponent.CatContainsMailItem(mailItemId.InternalId);
			case QueueType.Shadow:
				return QueueManager.UnfreezeShadowMessage(mailItemId);
			default:
				throw new QueueViewerException(QVErrorCode.QV_E_INVALID_OPERATION);
			}
		}

		public bool DeleteMessage(MessageIdentity mailItemId, bool withNDR)
		{
			if (!mailItemId.QueueIdentity.IsFullySpecified)
			{
				throw new QueueViewerException(QVErrorCode.QV_E_INCOMPLETE_IDENTITY);
			}
			if (mailItemId.QueueIdentity.Type == QueueType.Delivery || mailItemId.QueueIdentity.Type == QueueType.Unreachable)
			{
				return QueueManager.DeleteRemoteDeliveryMessage(mailItemId, withNDR);
			}
			if (mailItemId.QueueIdentity.Type == QueueType.Shadow)
			{
				return QueueManager.DeleteShadowMessage(mailItemId);
			}
			if (mailItemId.QueueIdentity.Type == QueueType.Poison)
			{
				return this.DeletePoisonMessage(mailItemId);
			}
			if (mailItemId.QueueIdentity.Type != QueueType.Submission)
			{
				throw new QueueViewerException(QVErrorCode.QV_E_INVALID_OPERATION);
			}
			bool flag = Components.MessageDepotComponent.Enabled ? QueueManager.DeleteMessageFromMessageDepot(mailItemId.InternalId, withNDR) : Components.CategorizerComponent.SubmitMessageQueue.DeleteMailItem(mailItemId.InternalId, withNDR);
			if (!flag && Components.CategorizerComponent.CatContainsMailItem(mailItemId.InternalId))
			{
				throw new QueueViewerException(QVErrorCode.QV_E_INVALID_OPERATION);
			}
			return flag;
		}

		public void RedirectMessage(MultiValuedProperty<Fqdn> targetServers)
		{
			List<string> targetHosts = (from server in targetServers
			select server.Domain).ToList<string>();
			if (!targetHosts.All(new Func<string, bool>(Components.RoutingComponent.MailRouter.IsHubTransportServer)))
			{
				throw new QueueViewerException(QVErrorCode.QV_E_INVALID_SERVER_COLLECTION);
			}
			if (this.pendingRedirectMessageTask != null && !this.pendingRedirectMessageTask.IsCompleted)
			{
				throw new QueueViewerException(QVErrorCode.QV_E_REDIRECT_MESSAGE_IN_PROGRESS);
			}
			List<string> resubmitQueueNames = new List<string>();
			List<RoutedMessageQueue> resubmitQueues = new List<RoutedMessageQueue>();
			long resubmitCount = 0L;
			QueueManager.FreezeQueue(QueueIdentity.SubmissionQueueIdentity);
			Task<int>[] resubmitTasks;
			try
			{
				RemoteDeliveryComponent.UnreachableMessageQueue.Resubmit(ResubmitReason.Redirect, null);
				resubmitQueueNames.Add(Strings.LatencyComponentSubmissionQueue);
				resubmitQueueNames.Add(Strings.LatencyComponentUnreachableQueue);
				resubmitCount = (Components.MessageDepotComponent.Enabled ? QueueManager.RedirectMessagesInMessageDepot(targetHosts) : QueueManager.RedirectMessagesInSubmissionQueue(targetHosts));
				lock (Components.RemoteDeliveryComponent.SyncQueues)
				{
					foreach (RoutedMessageQueue routedMessageQueue in from queue in Components.RemoteDeliveryComponent.GetQueueArray()
					where !queue.IsEmpty
					select queue)
					{
						resubmitQueues.Add(routedMessageQueue);
						routedMessageQueue.AddReference();
						resubmitQueueNames.Add(string.Format("'{0}:{1}:{2}'", routedMessageQueue.Key.NextHopType, routedMessageQueue.Key.NextHopDomain, routedMessageQueue.Key.NextHopConnector));
					}
				}
				resubmitTasks = (from q in resubmitQueues
				select q.ResubmitAsync(ResubmitReason.Redirect, delegate(TransportMailItem tmi)
				{
					QueueManager.RedirectMessage(tmi, targetHosts);
				})).ToArray<Task<int>>();
			}
			catch
			{
				QueueManager.UnfreezeQueue(QueueIdentity.SubmissionQueueIdentity);
				throw;
			}
			this.pendingRedirectMessageTask = Task.WhenAll<int>(resubmitTasks).ContinueWith(delegate(Task<int[]> t)
			{
				QueueManager.UnfreezeQueue(QueueIdentity.SubmissionQueueIdentity);
				resubmitQueues.ForEach(delegate(RoutedMessageQueue queue)
				{
					queue.ReleaseReference();
				});
				resubmitCount = resubmitTasks.Aggregate(resubmitCount, (long current, Task<int> resubmitTask) => current + (long)resubmitTask.Result);
				QueueManager.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RedirectMessageStarted, null, new object[]
				{
					string.Join(",", resubmitQueueNames.ToArray()),
					string.Join(",", targetHosts.ToArray()),
					resubmitCount
				});
			});
		}

		public bool SetMessage(MessageIdentity mailItemId, ExtensibleMessageInfo properties, bool resubmit)
		{
			if (!mailItemId.QueueIdentity.IsFullySpecified)
			{
				throw new QueueViewerException(QVErrorCode.QV_E_INCOMPLETE_IDENTITY);
			}
			if (mailItemId.QueueIdentity.Type == QueueType.Shadow || mailItemId.QueueIdentity.Type == QueueType.Poison)
			{
				throw new QueueViewerException(QVErrorCode.QV_E_INVALID_OPERATION);
			}
			if (mailItemId.QueueIdentity.Type == QueueType.Delivery || mailItemId.QueueIdentity.Type == QueueType.Unreachable)
			{
				return QueueManager.SetDeliveryMessage(mailItemId, properties, resubmit);
			}
			bool flag;
			if (mailItemId.QueueIdentity.Type != QueueType.Submission || Components.CategorizerComponent.SubmitMessageQueue.UpdateMailItem(mailItemId.InternalId, properties, out flag))
			{
				return true;
			}
			if (flag)
			{
				throw new QueueViewerException(QVErrorCode.QV_E_MESSAGE_NOT_SUSPENDED);
			}
			throw new QueueViewerException(QVErrorCode.QV_E_OBJECT_NOT_FOUND);
		}

		public bool ReadMessageBody(MessageIdentity mailItemId, byte[] buffer, int position, int count, out int bytesRead)
		{
			if (!mailItemId.QueueIdentity.IsFullySpecified)
			{
				throw new QueueViewerException(QVErrorCode.QV_E_INCOMPLETE_IDENTITY);
			}
			if (mailItemId.QueueIdentity.Type == QueueType.Delivery || mailItemId.QueueIdentity.Type == QueueType.Unreachable)
			{
				return QueueManager.ReadRemoteDeliveryMessageBody(mailItemId, buffer, position, count, out bytesRead);
			}
			if (mailItemId.QueueIdentity.Type == QueueType.Submission)
			{
				bool flag2;
				bool flag;
				if (Components.MessageDepotComponent.Enabled)
				{
					flag = QueueManager.ReadMessageBodyFromMessageDepot(mailItemId.InternalId, buffer, position, count, out bytesRead, out flag2);
				}
				else
				{
					flag = Components.CategorizerComponent.SubmitMessageQueue.ReadMessageBody(mailItemId.InternalId, buffer, position, count, out bytesRead, out flag2);
				}
				if (flag2)
				{
					throw new QueueViewerException(QVErrorCode.QV_E_MESSAGE_NOT_SUSPENDED);
				}
				if (!flag && Components.CategorizerComponent.CatContainsMailItem(mailItemId.InternalId))
				{
					throw new QueueViewerException(QVErrorCode.QV_E_INVALID_OPERATION);
				}
				return flag;
			}
			else
			{
				if (mailItemId.QueueIdentity.Type == QueueType.Poison)
				{
					return this.ReadPoisonMessageBody(mailItemId, buffer, position, count, out bytesRead);
				}
				throw new QueueViewerException(QVErrorCode.QV_E_INVALID_OPERATION);
			}
		}

		public ExtensibleMessageInfo[] GetMessageInfoPage(PagingEngine<ExtensibleMessageInfo, ExtensibleMessageInfoSchema> pagingEngine, out int matchCount, out int pageOffset)
		{
			MessageInfoFactory messageInfoFactory = new MessageInfoFactory(pagingEngine.IncludeDetails, pagingEngine.IncludeComponentLatencyInfo);
			matchCount = 0;
			pageOffset = 0;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			IMessageDepotQueueViewer messageDepotQueueViewer = null;
			bool enabled = Components.MessageDepotQueueViewerComponent.Enabled;
			if (enabled)
			{
				messageDepotQueueViewer = Components.MessageDepotQueueViewerComponent.MessageDepotQueueViewer;
			}
			TransportMailItem transportMailItem = null;
			TransportMailItem transportMailItem2 = null;
			TransportMailItem transportMailItem3 = null;
			CategorizerItem categorizerItem = null;
			IMessageDepotItemWrapper messageDepotItemWrapper = null;
			ICollection<NextHopSolution> collection = null;
			if (pagingEngine.IdentitySearch)
			{
				MessageIdentity messageIdentity = (MessageIdentity)pagingEngine.IdentitySearchValue;
				if (messageIdentity.QueueIdentity.IsEmpty)
				{
					throw new QueueViewerException(QVErrorCode.QV_E_INVALID_IDENTITY_FOR_EQUALITY);
				}
				transportMailItem2 = Components.ShadowRedundancyComponent.ShadowRedundancyManager.GetMailItem(messageIdentity);
				if (transportMailItem2 != null)
				{
					collection = QueueManager.GetMailItemSolutions(transportMailItem2, messageIdentity.QueueIdentity);
				}
				transportMailItem = Components.RemoteDeliveryComponent.GetMailItem(messageIdentity);
				if (transportMailItem != null && collection == null)
				{
					collection = QueueManager.GetMailItemSolutions(transportMailItem, messageIdentity.QueueIdentity);
				}
				if (transportMailItem2 == null && transportMailItem == null)
				{
					if (enabled)
					{
						if (QueueManager.TryGetMailItemByIdFromMessageDepot(messageIdentity, out messageDepotItemWrapper) && messageDepotItemWrapper.State == MessageDepotItemState.Poisoned)
						{
							transportMailItem3 = (TransportMailItem)messageDepotItemWrapper.Item.MessageObject;
							messageDepotItemWrapper = null;
						}
					}
					else
					{
						categorizerItem = Components.CategorizerComponent.GetCategorizerItemById(messageIdentity);
						if (categorizerItem == null)
						{
							transportMailItem3 = this.poisonMessageQueue[messageIdentity];
						}
					}
				}
				if (transportMailItem == null && transportMailItem3 == null && categorizerItem == null && transportMailItem2 == null && messageDepotItemWrapper == null)
				{
					stopwatch.Stop();
					ExTraceGlobals.QueuingTracer.TraceDebug<long, long>(0L, "Return 0 ExtensibleMessageInfo elements in {0}ms [{1} ticks]", stopwatch.ElapsedMilliseconds, stopwatch.ElapsedTicks);
					return QueueManager.emptyMessageInfoResult;
				}
			}
			int num;
			bool flag;
			ExtensibleMessageInfo[] page;
			do
			{
				matchCount = 0;
				num = 0;
				pagingEngine.ResetResultSet();
				QueueManager.MultiSolutionMessageFilter multiSolutionMessageFilter = new QueueManager.MultiSolutionMessageFilter(messageInfoFactory, pagingEngine, collection, true);
				if (pagingEngine.IdentitySearch)
				{
					multiSolutionMessageFilter.Visit(transportMailItem2);
				}
				else
				{
					Components.ShadowRedundancyComponent.ShadowRedundancyManager.VisitMailItems(new Func<TransportMailItem, bool>(multiSolutionMessageFilter.Visit));
				}
				matchCount += multiSolutionMessageFilter.MatchCount;
				num += multiSolutionMessageFilter.TotalCount;
				if (matchCount < pagingEngine.PageSize)
				{
					QueueManager.MultiSolutionMessageFilter multiSolutionMessageFilter2 = new QueueManager.MultiSolutionMessageFilter(messageInfoFactory, pagingEngine, collection, false);
					if (pagingEngine.IdentitySearch)
					{
						multiSolutionMessageFilter2.Visit(transportMailItem);
					}
					else
					{
						Components.RemoteDeliveryComponent.VisitMailItems(new Func<TransportMailItem, bool>(multiSolutionMessageFilter2.Visit));
					}
					matchCount += multiSolutionMessageFilter2.MatchCount;
					num += multiSolutionMessageFilter2.TotalCount;
				}
				if (matchCount < pagingEngine.PageSize)
				{
					QueueManager.PoisonMessageFilter poisonMessageFilter = new QueueManager.PoisonMessageFilter(messageInfoFactory, pagingEngine);
					if (pagingEngine.IdentitySearch)
					{
						poisonMessageFilter.Visit(transportMailItem3);
						matchCount += poisonMessageFilter.MatchCount;
						num += poisonMessageFilter.TotalCount;
					}
					else if (enabled)
					{
						QueueManager.PoisonMessageDepotItemFilter poisonMessageDepotItemFilter = new QueueManager.PoisonMessageDepotItemFilter(messageInfoFactory, pagingEngine);
						messageDepotQueueViewer.VisitMailItems(new Func<IMessageDepotItemWrapper, bool>(poisonMessageDepotItemFilter.Visit));
						matchCount += poisonMessageDepotItemFilter.MatchCount;
						num += poisonMessageDepotItemFilter.TotalCount;
					}
					else
					{
						this.poisonMessageQueue.VisitMailItems(new Func<TransportMailItem, bool>(poisonMessageFilter.Visit));
						matchCount += poisonMessageFilter.MatchCount;
						num += poisonMessageFilter.TotalCount;
					}
				}
				if (matchCount < pagingEngine.PageSize)
				{
					if (enabled)
					{
						QueueManager.SubmissionMessageDepotItemFilter submissionMessageDepotItemFilter = new QueueManager.SubmissionMessageDepotItemFilter(messageInfoFactory, pagingEngine);
						if (pagingEngine.IdentitySearch)
						{
							submissionMessageDepotItemFilter.Visit(messageDepotItemWrapper);
						}
						else
						{
							messageDepotQueueViewer.VisitMailItems(new Func<IMessageDepotItemWrapper, bool>(submissionMessageDepotItemFilter.Visit));
						}
						matchCount += submissionMessageDepotItemFilter.MatchCount;
						num += submissionMessageDepotItemFilter.TotalCount;
					}
					else
					{
						QueueManager.CatogorizerMessageFilter catogorizerMessageFilter = new QueueManager.CatogorizerMessageFilter(messageInfoFactory, pagingEngine);
						if (pagingEngine.IdentitySearch)
						{
							catogorizerMessageFilter.Visit(categorizerItem);
						}
						else
						{
							Components.CategorizerComponent.VisitCategorizerItems(new Func<CategorizerItem, bool>(catogorizerMessageFilter.Visit), true);
						}
						matchCount += catogorizerMessageFilter.MatchCount;
						num += catogorizerMessageFilter.TotalCount;
					}
				}
				page = pagingEngine.GetPage(matchCount, out pageOffset, out flag);
			}
			while (flag);
			stopwatch.Stop();
			ExTraceGlobals.QueuingTracer.TraceDebug<int, long, long>(0L, "Processed {0} TransportMailItem elements in {1} ms [{2} ticks]", num, stopwatch.ElapsedMilliseconds, stopwatch.ElapsedTicks);
			ExTraceGlobals.QueuingTracer.TraceDebug<int>(0L, "Return {0} ExtensibleMessageInfo elements", page.Length);
			return page;
		}

		public ExtensibleQueueInfo[] GetQueueInfoPage(PagingEngine<ExtensibleQueueInfo, ExtensibleQueueInfoSchema> pagingEngine, out int totalCount, out int pageOffset)
		{
			totalCount = 0;
			pageOffset = 0;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			int num = 0;
			RoutedMessageQueue[] array = null;
			ShadowMessageQueue[] array2 = null;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			long num2;
			if (Components.MessageDepotQueueViewerComponent.Enabled)
			{
				IMessageDepotQueueViewer messageDepotQueueViewer = Components.MessageDepotQueueViewerComponent.MessageDepotQueueViewer;
				num2 = messageDepotQueueViewer.GetCount(MessageDepotItemStage.Submission, MessageDepotItemState.Poisoned);
			}
			else
			{
				num2 = (long)this.PoisonMessageQueue.Count;
			}
			if (pagingEngine.IdentitySearch)
			{
				QueueIdentity queueIdentity = (QueueIdentity)pagingEngine.IdentitySearchValue;
				if (queueIdentity == QueueIdentity.PoisonQueueIdentity)
				{
					if (num2 > 0L)
					{
						flag = true;
						num = 1;
					}
				}
				else if (queueIdentity == QueueIdentity.SubmissionQueueIdentity)
				{
					flag2 = true;
					num = 1;
				}
				else if (queueIdentity == QueueIdentity.UnreachableQueueIdentity)
				{
					if (RemoteDeliveryComponent.UnreachableMessageQueue.CountNotDeleted > 0 || RemoteDeliveryComponent.UnreachableMessageQueue.Suspended)
					{
						flag3 = true;
						num = 1;
					}
				}
				else if (queueIdentity.Type == QueueType.Shadow)
				{
					List<ShadowMessageQueue> list = Components.ShadowRedundancyComponent.ShadowRedundancyManager.FindByQueueIdentity(queueIdentity);
					if (list.Count > 0)
					{
						array2 = list.ToArray();
						num += array2.Length;
					}
				}
				else
				{
					List<RoutedMessageQueue> list2 = Components.RemoteDeliveryComponent.FindByQueueIdentity(queueIdentity);
					if (list2.Count > 0)
					{
						array = list2.ToArray();
						num += array.Length;
					}
				}
				if (num == 0)
				{
					return new List<ExtensibleQueueInfo>(0).ToArray();
				}
			}
			else
			{
				array = Components.RemoteDeliveryComponent.GetQueueArray();
				array2 = Components.ShadowRedundancyComponent.ShadowRedundancyManager.GetQueueArray();
				num = array.Length + array2.Length;
				flag2 = true;
				num++;
				if (num2 > 0L)
				{
					flag = true;
					num++;
				}
				UnreachableMessageQueue unreachableMessageQueue = RemoteDeliveryComponent.UnreachableMessageQueue;
				if (unreachableMessageQueue.CountNotDeleted > 0 || unreachableMessageQueue.Suspended)
				{
					flag3 = true;
					num++;
				}
			}
			bool flag4 = false;
			ExtensibleQueueInfo[] page;
			do
			{
				totalCount = 0;
				pagingEngine.ResetResultSet();
				if (array != null)
				{
					foreach (RoutedMessageQueue routedMessageQueue in array)
					{
						if (routedMessageQueue.IsAdminVisible)
						{
							ExtensibleQueueInfo queueInfo = QueueInfoFactory.NewDeliveryQueueInfo(routedMessageQueue);
							bool flag5 = QueueManager.ProcessQueueInfo(pagingEngine, queueInfo, ref totalCount);
							if (!flag5)
							{
								break;
							}
						}
					}
				}
				if (array2 != null)
				{
					foreach (ShadowMessageQueue shadowMessageQueue in array2)
					{
						ExtensibleQueueInfo queueInfo2 = QueueInfoFactory.NewShadowQueueInfo(shadowMessageQueue);
						bool flag6 = QueueManager.ProcessQueueInfo(pagingEngine, queueInfo2, ref totalCount);
						if (!flag6)
						{
							break;
						}
					}
				}
				if (flag)
				{
					ExtensibleQueueInfo queueInfo3 = QueueInfoFactory.NewPoisonQueueInfo();
					QueueManager.ProcessQueueInfo(pagingEngine, queueInfo3, ref totalCount);
				}
				if (flag2)
				{
					ExtensibleQueueInfo queueInfo4 = QueueInfoFactory.NewSubmissionQueueInfo();
					QueueManager.ProcessQueueInfo(pagingEngine, queueInfo4, ref totalCount);
				}
				if (flag3)
				{
					ExtensibleQueueInfo queueInfo5 = QueueInfoFactory.NewUnreachableQueueInfo();
					QueueManager.ProcessQueueInfo(pagingEngine, queueInfo5, ref totalCount);
				}
				page = pagingEngine.GetPage(totalCount, out pageOffset, out flag4);
			}
			while (flag4);
			stopwatch.Stop();
			ExTraceGlobals.QueuingTracer.TraceDebug<int, long, long>(0L, "Processed {0} Queue objects in {1} ms [{2} ticks]", num, stopwatch.ElapsedMilliseconds, stopwatch.ElapsedTicks);
			ExTraceGlobals.QueuingTracer.TraceDebug<int>(0L, "Return {0} ExtensibleQueueInfo elements", page.Length);
			return page;
		}

		public void LoadMessageQueues()
		{
			foreach (RoutedQueueBase routedQueueBase in Components.MessagingDatabase.Queues)
			{
				if (routedQueueBase.NextHopType.DeliveryType == DeliveryType.Undefined)
				{
					SubmitMessageQueue.LoadInstance(routedQueueBase);
				}
				else if (routedQueueBase.NextHopType.DeliveryType == DeliveryType.Unreachable)
				{
					UnreachableMessageQueue.LoadInstance(routedQueueBase);
				}
				else if (routedQueueBase.NextHopType.DeliveryType == DeliveryType.ShadowRedundancy)
				{
					Components.ShadowRedundancyComponent.ShadowRedundancyManager.LoadQueue(routedQueueBase);
				}
				else
				{
					Components.RemoteDeliveryComponent.LoadQueue(routedQueueBase);
				}
			}
			if (SubmitMessageQueue.Instance == null)
			{
				SubmitMessageQueue.CreateInstance();
			}
			if (UnreachableMessageQueue.Instance == null)
			{
				UnreachableMessageQueue.CreateInstance();
			}
		}

		public void UpateInstanceCounter(RiskLevel riskLevel, DeliveryPriority priority, Action<QueuingPerfCountersInstance> updateCounter)
		{
			IEnumerable<int> instanceCounterIndex = QueueManager.GetInstanceCounterIndex(riskLevel, priority);
			foreach (int num in instanceCounterIndex)
			{
				updateCounter(this.queuingPerfCountersInstances[num]);
			}
			updateCounter(this.queuingPerfCountersTotalInstance);
		}

		public void UpdateAllInstanceCounters(int[] instanceValues, Action<QueuingPerfCountersInstance, int> updateCounterWithValue)
		{
			this.UpdateAllInstanceCounters(instanceValues, updateCounterWithValue, true);
		}

		public void UpdateAllInstanceCounters(int[] instanceValues, Action<QueuingPerfCountersInstance, int> updateCounterWithValue, bool updateTotal)
		{
			if (instanceValues == null || instanceValues.Length != QueueManager.InstanceCountersLength)
			{
				throw new InvalidOperationException(string.Format("instanceValues does not have the right array length. Expected: {0}, Actual: {1}", QueueManager.InstanceCountersLength, (instanceValues != null) ? instanceValues.Length : 0));
			}
			for (int i = 0; i < QueueManager.InstanceCountersLength; i++)
			{
				updateCounterWithValue(this.queuingPerfCountersInstances[i], instanceValues[i]);
			}
			if (updateTotal)
			{
				updateCounterWithValue(this.queuingPerfCountersTotalInstance, this.GetTotalFromInstance(instanceValues));
			}
		}

		public int GetTotalFromInstance(IList<int> instanceValues)
		{
			if (instanceValues == null || instanceValues.Count != QueueManager.InstanceCountersLength)
			{
				throw new InvalidOperationException(string.Format("instanceValues does not have the right array length. Expected: {0}, Actual: {1}", QueueManager.InstanceCountersLength, (instanceValues != null) ? instanceValues.Count : 0));
			}
			return QueueManager.priorityToInstanceIndexMap.Sum((KeyValuePair<DeliveryPriority, int> priorityIndex) => instanceValues[priorityIndex.Value]);
		}

		public void UpdatePerfCountersOnEnterSubmissionQueue(IQueueItem item)
		{
			if (this.queuingPerfCountersTotalInstance != null)
			{
				this.submissionPerfCounterWrapper.OnEnterSubmissionQueue(this.queuingPerfCountersTotalInstance);
			}
		}

		public void UpdatePerfCountersOnExpireFromSubmissionQueue(IQueueItem item)
		{
			if (this.queuingPerfCountersTotalInstance != null)
			{
				this.submissionPerfCounterWrapper.OnExpireFromSubmissionQueue(this.queuingPerfCountersTotalInstance);
			}
		}

		public void UpdatePerfCountersOnLockExpiredInSubmissionQueue()
		{
			if (this.queuingPerfCountersTotalInstance != null)
			{
				this.queuingPerfCountersTotalInstance.SubmissionQueueLocksExpiredTotal.Increment();
			}
		}

		public void UpdatePerfCountersOnLockExpiredInDeliveryQueue()
		{
			if (this.queuingPerfCountersTotalInstance != null)
			{
				this.queuingPerfCountersTotalInstance.LocksExpiredInDeliveryTotal.Increment();
			}
		}

		public void UpdatePerfCountersOnExitSubmissionQueue(IQueueItem item)
		{
			if (this.queuingPerfCountersTotalInstance != null)
			{
				this.submissionPerfCounterWrapper.OnExitSubmissionQueue(this.queuingPerfCountersTotalInstance);
			}
		}

		public void UpdatePerfCountersOnMessageBifurcatedInCategorizer()
		{
			if (this.queuingPerfCountersTotalInstance != null)
			{
				this.submissionPerfCounterWrapper.OnMessageBifurcatedInCategorizer(this.queuingPerfCountersTotalInstance);
			}
		}

		public void UpdatePerfCountersOnLeavingCategorizer()
		{
			if (this.queuingPerfCountersTotalInstance != null)
			{
				this.submissionPerfCounterWrapper.OnLeavingCategorizer(this.queuingPerfCountersTotalInstance);
			}
		}

		public void UpdatePerfCountersOnMessageDeferredFromCategorizer()
		{
			if (this.queuingPerfCountersTotalInstance != null)
			{
				this.submissionPerfCounterWrapper.OnMessageDeferredFromCategorizer(this.queuingPerfCountersTotalInstance);
			}
		}

		public QueuedRecipientsByAgePerfCountersWrapper GetQueuedRecipientsByAge()
		{
			if (this.queuedRecipientsByAge == null)
			{
				throw new InvalidOperationException("cannot acquire queuedRecipientsByAge before loading the component");
			}
			return this.queuedRecipientsByAge;
		}

		public void UpdatePerfCountersOnMessagesResubmittedFromCategorizer()
		{
			if (this.queuingPerfCountersTotalInstance != null)
			{
				this.submissionPerfCounterWrapper.OnMessagesResubmittedFromCategorizer(this.queuingPerfCountersTotalInstance);
			}
		}

		public void TimeUpdatePerfCounters()
		{
			this.submissionPerfCounterWrapper.OnTimedUpdate(this.queuingPerfCountersTotalInstance);
		}

		internal static bool TryGetMailItemByIdFromMessageDepot(long mailItemId, out IMessageDepotItemWrapper item)
		{
			TransportMessageId messageId = new TransportMessageId(mailItemId.ToString(CultureInfo.CurrentCulture));
			return Components.MessageDepotQueueViewerComponent.MessageDepotQueueViewer.TryGet(messageId, out item);
		}

		private static bool SuspendMessageInMessageDepot(long internalMessageId)
		{
			IMessageDepotItemWrapper messageDepotItemWrapper;
			if (!QueueManager.TryGetMailItemByIdFromMessageDepot(internalMessageId, out messageDepotItemWrapper))
			{
				return false;
			}
			IMessageDepotItem item = messageDepotItemWrapper.Item;
			TransportMailItem transportMailItem = (TransportMailItem)item.MessageObject;
			if (transportMailItem == null)
			{
				return false;
			}
			try
			{
				Components.MessageDepotQueueViewerComponent.MessageDepotQueueViewer.Suspend(item.Id);
				transportMailItem.Suspend();
			}
			catch (MessageDepotPermanentException)
			{
				return false;
			}
			return true;
		}

		private static bool ResumeMessageInMessageDepot(long internalMessageId)
		{
			IMessageDepotItemWrapper messageDepotItemWrapper;
			if (!QueueManager.TryGetMailItemByIdFromMessageDepot(internalMessageId, out messageDepotItemWrapper))
			{
				return false;
			}
			IMessageDepotItem item = messageDepotItemWrapper.Item;
			TransportMailItem transportMailItem = (TransportMailItem)item.MessageObject;
			if (transportMailItem == null)
			{
				return false;
			}
			try
			{
				Components.MessageDepotQueueViewerComponent.MessageDepotQueueViewer.Resume(item.Id);
				transportMailItem.Resume();
			}
			catch (MessageDepotPermanentException)
			{
				return false;
			}
			return true;
		}

		private static bool DeleteMessageFromMessageDepot(long internalMessageId, bool withNdr)
		{
			IMessageDepotItemWrapper messageDepotItemWrapper;
			if (!QueueManager.TryGetMailItemByIdFromMessageDepot(internalMessageId, out messageDepotItemWrapper))
			{
				return false;
			}
			IMessageDepotQueueViewer messageDepotQueueViewer = Components.MessageDepotQueueViewerComponent.MessageDepotQueueViewer;
			try
			{
				messageDepotQueueViewer.Remove(messageDepotItemWrapper.Item.Id, withNdr);
			}
			catch (MessageDepotPermanentException)
			{
				return false;
			}
			return true;
		}

		private static bool ReadMessageBodyFromMessageDepot(long internalMessageId, byte[] buffer, int position, int count, out int bytesRead, out bool foundNotSuspended)
		{
			bytesRead = 0;
			foundNotSuspended = false;
			IMessageDepotItemWrapper messageDepotItemWrapper;
			if (!QueueManager.TryGetMailItemByIdFromMessageDepot(internalMessageId, out messageDepotItemWrapper))
			{
				return false;
			}
			foundNotSuspended = (messageDepotItemWrapper.State == MessageDepotItemState.Suspended);
			if (foundNotSuspended)
			{
				return false;
			}
			TransportMailItem transportMailItem = (TransportMailItem)messageDepotItemWrapper.Item.MessageObject;
			Stream stream;
			if (ExportStream.TryCreate(transportMailItem, transportMailItem.Recipients, false, out stream))
			{
				using (stream)
				{
					stream.Position = (long)position;
					bytesRead = stream.Read(buffer, 0, count);
				}
			}
			return true;
		}

		private static bool UnfreezeRemoteDeliveryMessage(MessageIdentity mailItemId)
		{
			bool flag = false;
			DateTime utcNow = DateTime.UtcNow;
			bool flag2 = false;
			NextHopSolution nextHopSolution = null;
			TransportMailItem mailItem = Components.RemoteDeliveryComponent.GetMailItem(mailItemId);
			if (mailItem == null)
			{
				return false;
			}
			lock (mailItem)
			{
				if (!mailItem.IsActive)
				{
					return false;
				}
				if (!mailItem.QueuedForDelivery)
				{
					throw new QueueViewerException(QVErrorCode.QV_E_INVALID_OPERATION);
				}
				if (!QueueManager.TryGetMailItemSolution(mailItem, mailItemId.QueueIdentity, out nextHopSolution))
				{
					return false;
				}
				if (nextHopSolution.IsInactive)
				{
					return false;
				}
				if (nextHopSolution.DeliveryStatus == DeliveryStatus.InDelivery && nextHopSolution.IsDeletedByAdmin)
				{
					throw new QueueViewerException(QVErrorCode.QV_E_INVALID_OPERATION);
				}
				if (nextHopSolution.AdminActionStatus == AdminActionStatus.Suspended)
				{
					nextHopSolution.AdminActionStatus = AdminActionStatus.None;
					flag2 = true;
					flag = (nextHopSolution.DeliveryStatus == DeliveryStatus.DequeuedAndDeferred);
					if (flag)
					{
						nextHopSolution.DeliveryStatus = DeliveryStatus.Enqueued;
						nextHopSolution.DeferUntil = utcNow;
					}
					else
					{
						QueueManager.AddItemToDeliveryConditionManager(nextHopSolution);
					}
					mailItem.CommitLazy();
				}
			}
			if (flag2)
			{
				if (flag)
				{
					RoutedMessageQueue queue = Components.RemoteDeliveryComponent.GetQueue(nextHopSolution.NextHopSolutionKey);
					queue.UpdateNextActivationTime(utcNow);
					Components.RemoteDeliveryComponent.ConnectionManager.CreateConnectionIfNecessary(queue);
				}
				ExTraceGlobals.QueuingTracer.TraceDebug<MessageIdentity>(0L, "Message {0} has been unfrozen by the admin", mailItemId);
			}
			return true;
		}

		private static bool UnfreezeShadowMessage(MessageIdentity mailItemId)
		{
			bool flag = false;
			NextHopSolution nextHopSolution = null;
			TransportMailItem mailItem = Components.ShadowRedundancyComponent.ShadowRedundancyManager.GetMailItem(mailItemId);
			if (mailItem == null)
			{
				return false;
			}
			lock (mailItem)
			{
				if (mailItem.IsRowDeleted)
				{
					return false;
				}
				if (!QueueManager.TryGetMailItemSolution(mailItem, mailItemId.QueueIdentity, out nextHopSolution))
				{
					return false;
				}
				if (nextHopSolution.AdminActionStatus == AdminActionStatus.Suspended)
				{
					nextHopSolution.AdminActionStatus = AdminActionStatus.None;
					flag = true;
					mailItem.CommitLazy();
				}
			}
			if (flag)
			{
				ExTraceGlobals.QueuingTracer.TraceDebug<MessageIdentity>(0L, "Shadow Message '{0}' has been unfrozen by the admin", mailItemId);
			}
			return true;
		}

		private static bool DeleteRemoteDeliveryMessage(MessageIdentity mailItemId, bool withNDR)
		{
			TransportMailItem item = null;
			if (!QueueManager.RunActionOnSolutionInDelivery(mailItemId, delegate(TransportMailItem mailItem, NextHopSolution nextHopSolution)
			{
				RemoteMessageQueue queue = QueueManager.GetQueue(mailItemId);
				RoutedMailItem routedMailItem = null;
				try
				{
					routedMailItem = QueueManager.FindMailItem(queue, mailItemId, true);
					item = mailItem;
					nextHopSolution.AdminActionStatus = (withNDR ? AdminActionStatus.PendingDeleteWithNDR : AdminActionStatus.PendingDeleteWithOutNDR);
					IList<MailRecipient> recipientsToBeResubmitted = new List<MailRecipient>();
					bool flag;
					bool flag2;
					mailItem.Ack(AckStatus.Fail, AckReason.MessageDeletedByAdmin, null, nextHopSolution.Recipients, nextHopSolution.AdminActionStatus, null, null, recipientsToBeResubmitted, out flag, out flag2);
					QueueManager.PostProcessDeletedItem(nextHopSolution);
					nextHopSolution.DeliveryStatus = DeliveryStatus.Complete;
					if (withNDR)
					{
						Components.DsnGenerator.GenerateDSNs(mailItem, nextHopSolution.Recipients);
					}
					MessageTrackingLog.TrackRelayedAndFailed(MessageTrackingSource.ADMIN, mailItem, nextHopSolution.Recipients, null);
				}
				catch (Exception)
				{
					if (routedMailItem != null && nextHopSolution.DeliveryStatus != DeliveryStatus.Complete)
					{
						queue.Enqueue(routedMailItem);
					}
					throw;
				}
			}))
			{
				return false;
			}
			if (item.Status == Status.Complete)
			{
				item.ReleaseFromRemoteDelivery();
				Components.RemoteDeliveryComponent.ReleaseMailItem(item);
			}
			ExTraceGlobals.QueuingTracer.TraceDebug<MessageIdentity, bool>(0L, "Message {0} deleted by the admin, NDR={1}", mailItemId, withNDR);
			return true;
		}

		private static bool RunActionOnSolutionInDelivery(MessageIdentity mailItemId, Action<TransportMailItem, NextHopSolution> action)
		{
			TransportMailItem mailItem = Components.RemoteDeliveryComponent.GetMailItem(mailItemId);
			if (mailItem == null)
			{
				return false;
			}
			lock (mailItem)
			{
				if (!mailItem.IsActive)
				{
					return false;
				}
				if (!mailItem.QueuedForDelivery)
				{
					throw new QueueViewerException(QVErrorCode.QV_E_INVALID_OPERATION);
				}
				NextHopSolution nextHopSolution;
				if (!QueueManager.TryGetMailItemSolution(mailItem, mailItemId.QueueIdentity, out nextHopSolution))
				{
					return false;
				}
				if (nextHopSolution.IsInactive || nextHopSolution.IsDeletedByAdmin)
				{
					return false;
				}
				if (nextHopSolution.DeliveryStatus == DeliveryStatus.InDelivery)
				{
					throw new QueueViewerException(QVErrorCode.QV_E_INVALID_OPERATION);
				}
				action(mailItem, nextHopSolution);
			}
			return true;
		}

		private static bool DeleteShadowMessage(MessageIdentity mailItemId)
		{
			ShadowRedundancyManager shadowRedundancyManager = Components.ShadowRedundancyComponent.ShadowRedundancyManager;
			List<ShadowMessageQueue> list = shadowRedundancyManager.FindByQueueIdentity(mailItemId.QueueIdentity);
			if (list.Count > 1)
			{
				throw new QueueViewerException(QVErrorCode.QV_E_MULTIPLE_IDENTITY_MATCH);
			}
			if (list.Count == 0 || list[0].IsEmpty)
			{
				return false;
			}
			TransportMailItem mailItem = shadowRedundancyManager.GetMailItem(mailItemId.InternalId);
			if (mailItem == null)
			{
				return false;
			}
			bool flag = list[0].Discard(mailItem.ShadowMessageId, DiscardReason.DeletedByAdmin);
			if (flag)
			{
				ExTraceGlobals.QueuingTracer.TraceDebug<MessageIdentity>(0L, "Shadow Message '{0}' deleted by the admin.", mailItemId);
			}
			return flag;
		}

		private static void PostProcessDeletedItem(NextHopSolution nextHopSolution)
		{
			RemoteMessageQueue remoteMessageQueue;
			if (nextHopSolution.NextHopSolutionKey.Equals(NextHopSolutionKey.Unreachable))
			{
				remoteMessageQueue = RemoteDeliveryComponent.UnreachableMessageQueue;
			}
			else
			{
				remoteMessageQueue = Components.RemoteDeliveryComponent.GetQueue(nextHopSolution.NextHopSolutionKey);
				if (remoteMessageQueue == null)
				{
					return;
				}
			}
			if (nextHopSolution.DeferUntil != DateTime.MinValue)
			{
				nextHopSolution.DeferUntil = DateTime.MinValue;
				remoteMessageQueue.UpdateNextActivationTime(DateTime.UtcNow);
			}
		}

		private static bool ReadRemoteDeliveryMessageBody(MessageIdentity mailItemId, byte[] buffer, int position, int count, out int bytesRead)
		{
			bytesRead = 0;
			TransportMailItem mailItem = Components.RemoteDeliveryComponent.GetMailItem(mailItemId);
			if (mailItem == null)
			{
				return false;
			}
			lock (mailItem)
			{
				if (!mailItem.IsActive)
				{
					return false;
				}
				if (!mailItem.QueuedForDelivery)
				{
					throw new QueueViewerException(QVErrorCode.QV_E_INVALID_OPERATION);
				}
				NextHopSolution nextHopSolution;
				if (!QueueManager.TryGetMailItemSolution(mailItem, mailItemId.QueueIdentity, out nextHopSolution))
				{
					return false;
				}
				if (nextHopSolution.IsInactive)
				{
					return false;
				}
				if (nextHopSolution.AdminActionStatus != AdminActionStatus.Suspended)
				{
					throw new QueueViewerException(QVErrorCode.QV_E_MESSAGE_NOT_SUSPENDED);
				}
				if (nextHopSolution.DeliveryStatus == DeliveryStatus.InDelivery)
				{
					throw new QueueViewerException(QVErrorCode.QV_E_INVALID_OPERATION);
				}
				Stream stream;
				if (ExportStream.TryCreate(mailItem, nextHopSolution.Recipients, false, out stream))
				{
					using (stream)
					{
						stream.Position = (long)position;
						bytesRead = stream.Read(buffer, 0, count);
					}
				}
			}
			if (bytesRead > 0)
			{
				ExTraceGlobals.QueuingTracer.TraceDebug<int, MessageIdentity>(0L, "{0} bytes of message {1} read by the admin", bytesRead, mailItemId);
			}
			return true;
		}

		private static List<NextHopSolution> GetMailItemSolutions(TransportMailItem mailItem, QueueIdentity queueIdentity)
		{
			List<NextHopSolution> list = new List<NextHopSolution>();
			Dictionary<NextHopSolutionKey, NextHopSolution> nextHopSolutions = mailItem.NextHopSolutions;
			if (queueIdentity.Type == QueueType.Unreachable)
			{
				NextHopSolution nextHopSolution;
				if (nextHopSolutions.TryGetValue(NextHopSolutionKey.Unreachable, out nextHopSolution))
				{
					list.Add(nextHopSolution);
				}
			}
			else
			{
				if (queueIdentity.Type == QueueType.Shadow)
				{
					List<ShadowMessageQueue> list2 = Components.ShadowRedundancyComponent.ShadowRedundancyManager.FindByQueueIdentity(queueIdentity);
					using (List<ShadowMessageQueue>.Enumerator enumerator = list2.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ShadowMessageQueue shadowMessageQueue = enumerator.Current;
							NextHopSolution nextHopSolution;
							if (nextHopSolutions.TryGetValue(shadowMessageQueue.Key, out nextHopSolution))
							{
								list.Add(nextHopSolution);
							}
						}
						return list;
					}
				}
				List<RoutedMessageQueue> list3 = Components.RemoteDeliveryComponent.FindByQueueIdentity(queueIdentity);
				foreach (RoutedMessageQueue routedMessageQueue in list3)
				{
					NextHopSolution nextHopSolution;
					if (nextHopSolutions.TryGetValue(routedMessageQueue.Key, out nextHopSolution) && nextHopSolution.DeliveryStatus != DeliveryStatus.Complete)
					{
						list.Add(nextHopSolution);
					}
				}
			}
			return list;
		}

		private static bool TryGetMailItemSolution(TransportMailItem transportMailItem, QueueIdentity queueIdentity, out NextHopSolution solution)
		{
			solution = null;
			List<NextHopSolution> mailItemSolutions = QueueManager.GetMailItemSolutions(transportMailItem, queueIdentity);
			if (mailItemSolutions.Count == 0)
			{
				return false;
			}
			if (mailItemSolutions.Count > 1)
			{
				throw new QueueViewerException(QVErrorCode.QV_E_MULTIPLE_IDENTITY_MATCH);
			}
			solution = mailItemSolutions[0];
			return true;
		}

		private static bool ProcessQueueInfo(PagingEngine<ExtensibleQueueInfo, ExtensibleQueueInfoSchema> pagingEngine, ExtensibleQueueInfo queueInfo, ref int totalCount)
		{
			if (pagingEngine.ApplyFilterConditions(queueInfo))
			{
				totalCount++;
				if (pagingEngine.ApplyBookmarkConditions(queueInfo))
				{
					return pagingEngine.AddToResultSet(queueInfo);
				}
			}
			return true;
		}

		private static bool SuspendShadowQueue(QueueIdentity queueIdentity)
		{
			return QueueManager.InternalSuspendResumeShadowQueue(queueIdentity, true);
		}

		private static bool ResumeShadowQueue(QueueIdentity queueIdentity)
		{
			return QueueManager.InternalSuspendResumeShadowQueue(queueIdentity, false);
		}

		private static bool InternalSuspendResumeShadowQueue(QueueIdentity queueIdentity, bool suspend)
		{
			lock (Components.ShadowRedundancyComponent.ShadowRedundancyManager.SyncQueues)
			{
				List<ShadowMessageQueue> list = Components.ShadowRedundancyComponent.ShadowRedundancyManager.FindByQueueIdentity(queueIdentity);
				if (list.Count == 0)
				{
					return false;
				}
				if (list.Count > 1)
				{
					throw new QueueViewerException(QVErrorCode.QV_E_MULTIPLE_IDENTITY_MATCH);
				}
				list[0].Suspended = suspend;
			}
			return true;
		}

		private static long RedirectMessagesInMessageDepot(List<string> targetHosts)
		{
			long count = 0L;
			Components.MessageDepotQueueViewerComponent.MessageDepotQueueViewer.VisitMailItems(delegate(IMessageDepotItemWrapper item)
			{
				if (item.State != MessageDepotItemState.Poisoned && item.State != MessageDepotItemState.Expiring)
				{
					TransportMailItem transportMailItem = (TransportMailItem)item.Item.MessageObject;
					transportMailItem.MoveToHosts = targetHosts;
					count += 1L;
				}
				return true;
			});
			return count;
		}

		private static long RedirectMessagesInSubmissionQueue(List<string> targetHosts)
		{
			long count = 0L;
			Components.CategorizerComponent.SubmitMessageQueue.ForEach(delegate(IQueueItem item)
			{
				QueueManager.RedirectMessage(item as TransportMailItem, targetHosts);
				count += 1L;
			}, true);
			return count;
		}

		private static void RedirectMessage(TransportMailItem mailItem, List<string> targetHosts)
		{
			ArgumentValidator.ThrowIfNull("mailItem", mailItem);
			mailItem.MoveToHosts = targetHosts;
		}

		private static void RemoveItemFromDeliveryConditionManager(NextHopSolution nextHopSolution, DeliveryPriority priority)
		{
			if (nextHopSolution != null && nextHopSolution.AccessToken != null && nextHopSolution.AccessToken.Validate(nextHopSolution.AccessToken.Condition))
			{
				nextHopSolution.AccessToken.Return(true);
				return;
			}
			if (nextHopSolution != null && nextHopSolution.DeliveryStatus != DeliveryStatus.InDelivery && nextHopSolution.CurrentCondition != null)
			{
				Components.RemoteDeliveryComponent.ConditionManager.MoveLockedToDisabled(nextHopSolution.CurrentCondition, nextHopSolution.NextHopSolutionKey);
			}
		}

		private static void AddItemToDeliveryConditionManager(NextHopSolution nextHopSolution)
		{
			if (nextHopSolution.CurrentCondition != null)
			{
				Components.RemoteDeliveryComponent.ConditionManager.AddToLocked(nextHopSolution.CurrentCondition, nextHopSolution.NextHopSolutionKey);
			}
		}

		private static bool SetDeliveryMessage(MessageIdentity mailItemId, ExtensibleMessageInfo properties, bool resubmit)
		{
			bool result = false;
			RoutedMailItem routedMailItem = null;
			QueueManager.RunActionOnSolutionInDelivery(mailItemId, delegate(TransportMailItem mailItem, NextHopSolution solution)
			{
				RemoteMessageQueue queue = QueueManager.GetQueue(mailItemId);
				if (!queue.Suspended && solution.AdminActionStatus != AdminActionStatus.Suspended)
				{
					throw new QueueViewerException(QVErrorCode.QV_E_MESSAGE_NOT_SUSPENDED);
				}
				try
				{
					routedMailItem = QueueManager.FindMailItem(queue, mailItemId, resubmit);
					if (routedMailItem.UpdateProperties(properties, resubmit))
					{
						result = true;
					}
				}
				finally
				{
					if (routedMailItem != null && !result)
					{
						queue.Enqueue(routedMailItem);
					}
				}
			});
			return result;
		}

		private static RemoteMessageQueue GetQueue(MessageIdentity mailItemId)
		{
			if (mailItemId.QueueIdentity.Type == QueueType.Unreachable)
			{
				return RemoteDeliveryComponent.UnreachableMessageQueue;
			}
			List<RoutedMessageQueue> list = Components.RemoteDeliveryComponent.FindByQueueIdentity(mailItemId.QueueIdentity);
			if (list.Count != 1)
			{
				throw new QueueViewerException(QVErrorCode.QV_E_INCOMPLETE_IDENTITY);
			}
			return list[0];
		}

		private static RoutedMailItem FindMailItem(RemoteMessageQueue queue, MessageIdentity mailItemId, bool dequeue)
		{
			RoutedMailItem routedMailItem = null;
			queue.DequeueItem(delegate(IQueueItem item)
			{
				if (((RoutedMailItem)item).RecordId != mailItemId.InternalId)
				{
					return DequeueMatchResult.Continue;
				}
				routedMailItem = (RoutedMailItem)item;
				if (dequeue)
				{
					return DequeueMatchResult.DequeueAndBreak;
				}
				return DequeueMatchResult.Break;
			});
			if (routedMailItem == null)
			{
				throw new QueueViewerException(QVErrorCode.QV_E_INVALID_SERVER_DATA);
			}
			return routedMailItem;
		}

		private bool UnfreezePoisonMessage(MessageIdentity mailItemId)
		{
			TransportMailItem transportMailItem = null;
			if (Components.MessageDepotComponent.Enabled)
			{
				IMessageDepotItemWrapper messageDepotItemWrapper = null;
				if (QueueManager.TryGetMailItemByIdFromMessageDepot(mailItemId.InternalId, out messageDepotItemWrapper))
				{
					IMessageDepotItem item = messageDepotItemWrapper.Item;
					transportMailItem = (TransportMailItem)item.MessageObject;
					if (!QueueManager.DeleteMessageFromMessageDepot(mailItemId.InternalId, false))
					{
						return false;
					}
				}
			}
			else
			{
				transportMailItem = this.poisonMessageQueue.Extract(mailItemId);
			}
			if (transportMailItem != null)
			{
				transportMailItem.BumpExpirationTime();
				transportMailItem.PoisonCount = 0;
				MessageTrackingLog.TrackResubmit(MessageTrackingSource.QUEUE, transportMailItem, transportMailItem, "resubmitting from poison");
				Components.CategorizerComponent.EnqueueSubmittedMessage(transportMailItem);
				ExTraceGlobals.QueuingTracer.TraceDebug<MessageIdentity>(0L, "Message {0} was removed from the poison queue and submitted to the Categorizer", mailItemId);
				return true;
			}
			return false;
		}

		private bool DeletePoisonMessage(MessageIdentity mailItemId)
		{
			TransportMailItem transportMailItem = this.poisonMessageQueue.Extract(mailItemId);
			if (transportMailItem != null)
			{
				transportMailItem.Ack(AckStatus.Fail, AckReason.PoisonMessageDeletedByAdmin, transportMailItem.Recipients, null);
				MessageTrackingLog.TrackPoisonMessageDeleted(MessageTrackingSource.ADMIN, null, transportMailItem);
				transportMailItem.ReleaseFromActiveMaterializedLazy();
				ExTraceGlobals.QueuingTracer.TraceDebug<MessageIdentity>(0L, "Message {0} was deleted from the poison queue", mailItemId);
				return true;
			}
			return false;
		}

		private bool ReadPoisonMessageBody(MessageIdentity mailItemId, byte[] buffer, int position, int count, out int bytesRead)
		{
			bytesRead = 0;
			TransportMailItem transportMailItem = this.poisonMessageQueue[mailItemId];
			if (transportMailItem != null)
			{
				Stream stream;
				if (transportMailItem.TryCreateExportStream(out stream))
				{
					using (stream)
					{
						stream.Position = (long)position;
						bytesRead = stream.Read(buffer, 0, count);
					}
				}
				if (bytesRead > 0)
				{
					ExTraceGlobals.QueuingTracer.TraceDebug<int, MessageIdentity>(0L, "{0} bytes of message {1} read by the admin", bytesRead, mailItemId);
				}
				return true;
			}
			return false;
		}

		private const string TotalPerfCounterInstanceName = "_Total";

		private static readonly ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.QueuingTracer.Category, TransportEventLog.GetEventSource());

		private static readonly Dictionary<DeliveryPriority, int> priorityToInstanceIndexMap = new Dictionary<DeliveryPriority, int>
		{
			{
				DeliveryPriority.High,
				0
			},
			{
				DeliveryPriority.Normal,
				1
			},
			{
				DeliveryPriority.Low,
				2
			},
			{
				DeliveryPriority.None,
				3
			}
		};

		private static readonly Dictionary<DeliveryPriority, int> priorityToTotalExcludingPriorityNoneInstanceIndexMap = new Dictionary<DeliveryPriority, int>
		{
			{
				DeliveryPriority.High,
				4
			},
			{
				DeliveryPriority.Normal,
				4
			},
			{
				DeliveryPriority.Low,
				4
			}
		};

		private static readonly Dictionary<RiskLevel, int> riskToInstanceIndexMap = new Dictionary<RiskLevel, int>
		{
			{
				RiskLevel.High,
				5
			},
			{
				RiskLevel.Bulk,
				6
			},
			{
				RiskLevel.Normal,
				7
			},
			{
				RiskLevel.Low,
				8
			}
		};

		private static readonly Dictionary<RiskLevel, int> riskToHighAndBulkRiskTotalInstanceIndexMap = new Dictionary<RiskLevel, int>
		{
			{
				RiskLevel.High,
				9
			},
			{
				RiskLevel.Bulk,
				9
			}
		};

		private static readonly Dictionary<RiskLevel, int> riskToNormalAndLowRiskTotalInstanceIndexMap = new Dictionary<RiskLevel, int>
		{
			{
				RiskLevel.Normal,
				10
			},
			{
				RiskLevel.Low,
				10
			}
		};

		private static readonly Dictionary<Tuple<RiskLevel, DeliveryPriority>, int> riskAndPriorityInstanceIndexMap = new Dictionary<Tuple<RiskLevel, DeliveryPriority>, int>
		{
			{
				new Tuple<RiskLevel, DeliveryPriority>(RiskLevel.Normal, DeliveryPriority.Normal),
				11
			},
			{
				new Tuple<RiskLevel, DeliveryPriority>(RiskLevel.Normal, DeliveryPriority.Low),
				12
			},
			{
				new Tuple<RiskLevel, DeliveryPriority>(RiskLevel.Normal, DeliveryPriority.None),
				13
			},
			{
				new Tuple<RiskLevel, DeliveryPriority>(RiskLevel.Low, DeliveryPriority.Normal),
				14
			},
			{
				new Tuple<RiskLevel, DeliveryPriority>(RiskLevel.Low, DeliveryPriority.Low),
				15
			},
			{
				new Tuple<RiskLevel, DeliveryPriority>(RiskLevel.Low, DeliveryPriority.None),
				16
			}
		};

		private static readonly string[] priorityBasedInstanceCounterNames = new string[]
		{
			Strings.HighPriority,
			Strings.NormalPriority,
			Strings.LowPriority,
			Strings.NonePriority,
			Strings.TotalExcludingPriorityNone
		};

		private static readonly string[] riskBasedInstanceCounterNames = new string[]
		{
			Strings.HighRisk,
			Strings.BulkRisk,
			Strings.NormalRisk,
			Strings.LowRisk,
			Strings.HighAndBulkRisk,
			Strings.NormalAndLowRisk,
			Strings.NormalRiskNormalPriority,
			Strings.NormalRiskLowPriority,
			Strings.NormalRiskNonePriority,
			Strings.LowRiskNormalPriority,
			Strings.LowRiskLowPriority,
			Strings.LowRiskNonePriority
		};

		private static ExtensibleMessageInfo[] emptyMessageInfoResult = new ExtensibleMessageInfo[0];

		private static int busyUpdateAllQueues;

		private static bool updateAllQueuesPendingOrBusy;

		private static DateTime lastQueueUpdate = DateTime.UtcNow;

		private static bool includeRiskBasedCounters = VariantConfiguration.InvariantNoFlightingSnapshot.Transport.RiskBasedCounters.Enabled;

		private static int instanceCountersLength = -1;

		private PoisonMessageQueue poisonMessageQueue;

		private QueuingPerfCountersInstance queuingPerfCountersTotalInstance;

		private QueuingPerfCountersInstance[] queuingPerfCountersInstances;

		private QueueManager.SubmissionPerfCounterWrapper submissionPerfCounterWrapper;

		private QueuedRecipientsByAgePerfCountersWrapper queuedRecipientsByAge;

		private Task pendingRedirectMessageTask;

		private class SubmissionPerfCounterWrapper
		{
			public SubmissionPerfCounterWrapper(QueuingPerfCountersInstance queuingPerfCounter, TimeSpan recentInterval, TimeSpan recentBucketSize)
			{
				this.percentMessageCompleting = new SlidingPercentageCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(1.0));
				this.percentMessageDeferred = new SlidingPercentageCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(1.0));
				this.percentMessageResubmitted = new SlidingPercentageCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(1.0));
				queuingPerfCounter.MessagesCompletingCategorization.RawValue = (long)((int)this.percentMessageCompleting.GetSlidingPercentage());
				this.messagesSubmittedRecently = new SlidingTotalCounter(recentInterval, recentBucketSize);
			}

			public void OnEnterSubmissionQueue(QueuingPerfCountersInstance queuingPerfCounter)
			{
				queuingPerfCounter.SubmissionQueueLength.Increment();
				queuingPerfCounter.MessagesSubmittedTotal.Increment();
				this.IncrementDenominator(this.percentMessageCompleting, queuingPerfCounter.MessagesCompletingCategorization);
				this.messagesSubmittedRecently.AddValue(1L);
				queuingPerfCounter.MessagesSubmittedRecently.RawValue = this.messagesSubmittedRecently.Sum;
			}

			public void OnExpireFromSubmissionQueue(QueuingPerfCountersInstance queuingPerfCounter)
			{
				queuingPerfCounter.SubmissionQueueItemsExpiredTotal.Increment();
				this.OnLeavingCategorizer(queuingPerfCounter);
			}

			public void OnExitSubmissionQueue(QueuingPerfCountersInstance queuingPerfCounter)
			{
				queuingPerfCounter.SubmissionQueueLength.Decrement();
				this.IncrementDenominator(this.percentMessageDeferred, queuingPerfCounter.MessagesDeferredDuringCategorization);
				this.IncrementDenominator(this.percentMessageResubmitted, queuingPerfCounter.MessagesResubmittedDuringCategorization);
			}

			public void OnMessageBifurcatedInCategorizer(QueuingPerfCountersInstance queuingPerfCounter)
			{
				this.IncrementDenominator(this.percentMessageCompleting, queuingPerfCounter.MessagesCompletingCategorization);
				this.IncrementDenominator(this.percentMessageDeferred, queuingPerfCounter.MessagesDeferredDuringCategorization);
				this.IncrementDenominator(this.percentMessageResubmitted, queuingPerfCounter.MessagesResubmittedDuringCategorization);
			}

			public void OnLeavingCategorizer(QueuingPerfCountersInstance queuingPerfCounter)
			{
				this.IncrementNumerator(this.percentMessageCompleting, queuingPerfCounter.MessagesCompletingCategorization);
			}

			public void OnMessageDeferredFromCategorizer(QueuingPerfCountersInstance queuingPerfCounter)
			{
				this.IncrementNumerator(this.percentMessageDeferred, queuingPerfCounter.MessagesDeferredDuringCategorization);
			}

			public void OnMessagesResubmittedFromCategorizer(QueuingPerfCountersInstance queuingPerfCounter)
			{
				this.IncrementNumerator(this.percentMessageResubmitted, queuingPerfCounter.MessagesResubmittedDuringCategorization);
			}

			public void OnTimedUpdate(QueuingPerfCountersInstance queuingPerfCounter)
			{
				queuingPerfCounter.MessagesSubmittedRecently.RawValue = this.messagesSubmittedRecently.Sum;
			}

			private void IncrementDenominator(SlidingPercentageCounter slidingCounter, ExPerformanceCounter perfCounter)
			{
				int val = (int)slidingCounter.AddDenominator(1L);
				perfCounter.RawValue = (long)Math.Min(val, 1000);
			}

			private void IncrementNumerator(SlidingPercentageCounter slidingCounter, ExPerformanceCounter perfCounter)
			{
				int val = (int)slidingCounter.AddNumerator(1L);
				perfCounter.RawValue = (long)Math.Min(val, 1000);
			}

			private const int Infinity = 1000;

			private SlidingPercentageCounter percentMessageCompleting;

			private SlidingPercentageCounter percentMessageDeferred;

			private SlidingPercentageCounter percentMessageResubmitted;

			private SlidingTotalCounter messagesSubmittedRecently;
		}

		private abstract class MessageFilter<T>
		{
			protected MessageFilter(MessageInfoFactory messageInfoFactory, PagingEngine<ExtensibleMessageInfo, ExtensibleMessageInfoSchema> pagingEngine)
			{
				if (messageInfoFactory == null)
				{
					throw new ArgumentNullException("messageInfoFactory");
				}
				if (pagingEngine == null)
				{
					throw new ArgumentNullException("pagingEngine");
				}
				this.messageInfoFactory = messageInfoFactory;
				this.pagingEngine = pagingEngine;
			}

			public PagingEngine<ExtensibleMessageInfo, ExtensibleMessageInfoSchema> PagingEngine
			{
				get
				{
					return this.pagingEngine;
				}
			}

			public int MatchCount
			{
				get
				{
					return this.matchCount;
				}
				protected set
				{
					this.matchCount = value;
				}
			}

			public int TotalCount
			{
				get
				{
					return this.totalCount;
				}
				protected set
				{
					this.totalCount = value;
				}
			}

			protected MessageInfoFactory MessageInfoFactory
			{
				get
				{
					return this.messageInfoFactory;
				}
			}

			protected ExtensibleMessageInfo MessageInfoToRecycle
			{
				get
				{
					return this.messageInfoToRecycle;
				}
				set
				{
					this.messageInfoToRecycle = value;
				}
			}

			public virtual bool Visit(T item)
			{
				if (item == null)
				{
					return true;
				}
				this.TotalCount++;
				ExtensibleMessageInfo extensibleMessageInfo = this.CreateMessageInfo(item);
				if (extensibleMessageInfo == null)
				{
					return true;
				}
				this.RegisterForRecycling(extensibleMessageInfo);
				if (this.PagingEngine.ApplyFilterConditions(extensibleMessageInfo))
				{
					this.MatchCount++;
					if (this.PagingEngine.ApplyBookmarkConditions(extensibleMessageInfo))
					{
						return this.AddToResultSetAndBlockRecycling(extensibleMessageInfo);
					}
				}
				return true;
			}

			public void Visit(IEnumerable<T> items)
			{
				if (items == null)
				{
					return;
				}
				foreach (T item in items)
				{
					if (!this.Visit(item))
					{
						break;
					}
				}
			}

			protected abstract ExtensibleMessageInfo CreateMessageInfo(T item);

			protected bool AddToResultSetAndBlockRecycling(ExtensibleMessageInfo messageInfo)
			{
				bool result = this.PagingEngine.AddToResultSet(messageInfo);
				this.messageInfoToRecycle = null;
				return result;
			}

			protected void RegisterForRecycling(ExtensibleMessageInfo messageInfo)
			{
				if (messageInfo != null)
				{
					if (this.messageInfoToRecycle == null)
					{
						this.messageInfoToRecycle = messageInfo;
						return;
					}
					if (!object.ReferenceEquals(messageInfo, this.messageInfoToRecycle))
					{
						throw new InvalidOperationException("Available message info instance was not recycled.");
					}
				}
			}

			private MessageInfoFactory messageInfoFactory;

			private PagingEngine<ExtensibleMessageInfo, ExtensibleMessageInfoSchema> pagingEngine;

			private ExtensibleMessageInfo messageInfoToRecycle;

			private int totalCount;

			private int matchCount;
		}

		private class MessageDepotItemFilter : QueueManager.MessageFilter<IMessageDepotItemWrapper>
		{
			public MessageDepotItemFilter(MessageInfoFactory messageInfoFactory, PagingEngine<ExtensibleMessageInfo, ExtensibleMessageInfoSchema> pagingEngine) : base(messageInfoFactory, pagingEngine)
			{
			}

			public override bool Visit(IMessageDepotItemWrapper item)
			{
				return item == null || base.Visit(item);
			}

			protected override ExtensibleMessageInfo CreateMessageInfo(IMessageDepotItemWrapper item)
			{
				return base.MessageInfoFactory.NewMessageDepotItemMessageInfo(item, base.MessageInfoToRecycle);
			}
		}

		private sealed class PoisonMessageDepotItemFilter : QueueManager.MessageDepotItemFilter
		{
			public PoisonMessageDepotItemFilter(MessageInfoFactory messageInfoFactory, PagingEngine<ExtensibleMessageInfo, ExtensibleMessageInfoSchema> pagingEngine) : base(messageInfoFactory, pagingEngine)
			{
			}

			public override bool Visit(IMessageDepotItemWrapper item)
			{
				return item == null || item.Item == null || item.State != MessageDepotItemState.Poisoned || item.Item.Stage != MessageDepotItemStage.Submission || base.Visit(item);
			}
		}

		private sealed class SubmissionMessageDepotItemFilter : QueueManager.MessageDepotItemFilter
		{
			public SubmissionMessageDepotItemFilter(MessageInfoFactory messageInfoFactory, PagingEngine<ExtensibleMessageInfo, ExtensibleMessageInfoSchema> pagingEngine) : base(messageInfoFactory, pagingEngine)
			{
			}

			public override bool Visit(IMessageDepotItemWrapper itemWrapper)
			{
				return itemWrapper == null || itemWrapper.Item == null || itemWrapper.Item.Stage != MessageDepotItemStage.Submission || (itemWrapper.State != MessageDepotItemState.Ready && itemWrapper.State != MessageDepotItemState.Deferred && itemWrapper.State != MessageDepotItemState.Expiring && itemWrapper.State != MessageDepotItemState.Processing && itemWrapper.State != MessageDepotItemState.Suspended) || base.Visit(itemWrapper);
			}
		}

		private sealed class CatogorizerMessageFilter : QueueManager.MessageFilter<CategorizerItem>
		{
			public CatogorizerMessageFilter(MessageInfoFactory messageInfoFactory, PagingEngine<ExtensibleMessageInfo, ExtensibleMessageInfoSchema> pagingEngine) : base(messageInfoFactory, pagingEngine)
			{
			}

			public override bool Visit(CategorizerItem item)
			{
				return item == null || item.TransportMailItem == null || item.TransportMailItem.RecordId == 0L || base.Visit(item);
			}

			protected override ExtensibleMessageInfo CreateMessageInfo(CategorizerItem item)
			{
				return base.MessageInfoFactory.NewCategorizerMessageInfo(item, base.MessageInfoToRecycle);
			}
		}

		private sealed class PoisonMessageFilter : QueueManager.MessageFilter<TransportMailItem>
		{
			public PoisonMessageFilter(MessageInfoFactory messageInfoFactory, PagingEngine<ExtensibleMessageInfo, ExtensibleMessageInfoSchema> pagingEngine) : base(messageInfoFactory, pagingEngine)
			{
			}

			protected override ExtensibleMessageInfo CreateMessageInfo(TransportMailItem item)
			{
				return base.MessageInfoFactory.NewPoisonMessageInfo(item, base.MessageInfoToRecycle);
			}
		}

		private sealed class MultiSolutionMessageFilter : QueueManager.MessageFilter<TransportMailItem>
		{
			public MultiSolutionMessageFilter(MessageInfoFactory messageInfoFactory, PagingEngine<ExtensibleMessageInfo, ExtensibleMessageInfoSchema> pagingEngine, IEnumerable<NextHopSolution> nextHopSolutions, bool processShadowSolutions) : base(messageInfoFactory, pagingEngine)
			{
				this.nextHopSolutions = nextHopSolutions;
				this.processShadowSolutions = processShadowSolutions;
			}

			public override bool Visit(TransportMailItem mailItem)
			{
				if (mailItem == null)
				{
					return true;
				}
				if (mailItem.IsHeartbeat)
				{
					return true;
				}
				base.TotalCount++;
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				IEnumerable<NextHopSolution> enumerable = this.nextHopSolutions ?? mailItem.NextHopSolutions.Values;
				foreach (NextHopSolution nextHopSolution in enumerable)
				{
					bool flag4 = false;
					DeliveryType deliveryType = nextHopSolution.NextHopSolutionKey.NextHopType.DeliveryType;
					ExtensibleMessageInfo extensibleMessageInfo;
					if (deliveryType == DeliveryType.ShadowRedundancy)
					{
						if (!this.processShadowSolutions)
						{
							continue;
						}
						extensibleMessageInfo = base.MessageInfoFactory.NewShadowMessageInfo(mailItem, nextHopSolution, base.MessageInfoToRecycle);
					}
					else
					{
						if (this.processShadowSolutions)
						{
							continue;
						}
						extensibleMessageInfo = base.MessageInfoFactory.NewMessageInfo(mailItem, nextHopSolution, base.MessageInfoToRecycle);
					}
					if (extensibleMessageInfo != null)
					{
						base.RegisterForRecycling(extensibleMessageInfo);
						if (flag || base.PagingEngine.ApplyFilterConditions(extensibleMessageInfo, out flag4))
						{
							base.MatchCount++;
							flag = base.PagingEngine.FilterUsesOnlyBasicFields;
							bool flag5 = true;
							if (!flag3)
							{
								if (flag2 || base.PagingEngine.ApplyBookmarkConditions(extensibleMessageInfo, out flag5))
								{
									flag2 = flag5;
									if (!base.AddToResultSetAndBlockRecycling(extensibleMessageInfo))
									{
										return false;
									}
								}
								else
								{
									flag3 = flag5;
								}
							}
						}
						else if (flag4)
						{
							break;
						}
					}
				}
				return true;
			}

			protected override ExtensibleMessageInfo CreateMessageInfo(TransportMailItem item)
			{
				throw new NotSupportedException("This method should not be called.");
			}

			private readonly bool processShadowSolutions;

			private IEnumerable<NextHopSolution> nextHopSolutions;
		}

		private class QueueUpdateBlockedException : Exception
		{
			public QueueUpdateBlockedException(string message) : base(message)
			{
			}
		}
	}
}
