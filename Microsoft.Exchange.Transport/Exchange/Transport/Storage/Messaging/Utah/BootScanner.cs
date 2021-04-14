using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport.Internal.MExRuntime;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.Extensibility;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Exchange.Transport.RemoteDelivery;
using Microsoft.Exchange.Transport.ShadowRedundancy;

namespace Microsoft.Exchange.Transport.Storage.Messaging.Utah
{
	internal class BootScanner : IBootLoader, IStartableTransportComponent, ITransportComponent
	{
		public event Action OnBootLoadCompleted;

		public string CurrentState
		{
			get
			{
				return null;
			}
		}

		protected SegmentedSlidingCounter RecentPoisonMessagesCounter
		{
			get
			{
				return this.recentPoisonMessagesCounter;
			}
		}

		public void SetLoadTimeDependencies(ExEventLog eventLogger, IMessagingDatabase database, ShadowRedundancyComponent shadowRedundancyComponent, PoisonMessage poisonComponent, ICategorizer categorizerComponent, QueueManager queueManagerComponent, IBootLoaderConfig bootLoaderConfiguration)
		{
			this.database = database;
			this.eventLogger = eventLogger;
			this.shadowRedundancyComponent = shadowRedundancyComponent;
			this.poisonComponent = poisonComponent;
			this.categorizerComponent = categorizerComponent;
			this.queueManagerComponent = queueManagerComponent;
			this.configuration = bootLoaderConfiguration;
		}

		public void Load()
		{
			this.OnBootLoadCompleted += this.database.BootLoadCompleted;
			if (this.configuration.PoisonCountPublishingEnabled)
			{
				int poisonCountLookbackHours = this.configuration.PoisonCountLookbackHours;
				TimeSpan[] array = new TimeSpan[poisonCountLookbackHours];
				for (int i = 0; i < poisonCountLookbackHours; i++)
				{
					array[i] = TimeSpan.FromHours(1.0);
				}
				this.recentPoisonMessagesCounter = new SegmentedSlidingCounter(array, TimeSpan.FromMinutes(5.0));
			}
		}

		public void Unload()
		{
			if (this.poisonMessageCountTimer != null)
			{
				this.poisonMessageCountTimer.Dispose(true);
			}
		}

		public string OnUnhandledException(Exception e)
		{
			string text = this.CreateLoadedMessagesReport();
			this.EventLogLoadedMessages(text);
			StringBuilder stringBuilder = new StringBuilder("The following messages were loaded at startup before Transport crashed: ");
			stringBuilder.Append(text);
			return stringBuilder.ToString();
		}

		public void Start(bool initiallyPaused, ServiceState targetRunningState)
		{
			this.targetRunningState = targetRunningState;
			if (!initiallyPaused && this.ShouldExecute())
			{
				this.Start();
			}
		}

		public void Stop()
		{
			this.Stop(false);
		}

		public void Pause()
		{
			this.Stop(true);
		}

		public void Continue()
		{
			if (this.ShouldExecute())
			{
				this.Start();
			}
		}

		protected virtual bool IsPoison(TransportMailItem mailItem, out bool newPoisonMessage)
		{
			return this.poisonComponent.HandlePoison(mailItem, out newPoisonMessage);
		}

		protected virtual void SendToPoisonQueue(TransportMailItem mailItem)
		{
			if (Components.MessageDepotComponent.Enabled)
			{
				MessageDepotMailItem item = new MessageDepotMailItem(mailItem);
				Components.MessageDepotComponent.MessageDepot.Add(item);
				return;
			}
			this.queueManagerComponent.PoisonMessageQueue.Enqueue(mailItem);
		}

		protected virtual void SendToSubmissionQueue(TransportMailItem categorizerMailItem)
		{
			LatencyTracker.TrackPreProcessLatency(LatencyComponent.ServiceRestart, categorizerMailItem.LatencyTracker, categorizerMailItem.DateReceived);
			this.categorizerComponent.EnqueueSubmittedMessage(categorizerMailItem);
			ExTraceGlobals.StorageTracer.TraceDebug(categorizerMailItem.MsgId, "Bootloader submitted the message to categorizer.");
		}

		protected virtual void SendToShadowQueue(TransportMailItem shadowRedundancyMailItem)
		{
			this.shadowRedundancyComponent.ShadowRedundancyManager.ProcessMailItemOnStartup(shadowRedundancyMailItem);
			ExTraceGlobals.StorageTracer.TraceDebug(shadowRedundancyMailItem.MsgId, "Bootloader submitted the message to shadow manager.");
		}

		protected virtual void SendToDiscardPendingQueue(TransportMailItem mailItem)
		{
			this.shadowRedundancyComponent.ShadowRedundancyManager.EnqueueDiscardPendingMailItem(mailItem);
			ExTraceGlobals.StorageTracer.TraceDebug(mailItem.MsgId, "Bootloader moved message to the discard queue.");
		}

		protected virtual TransportMailItem MoveUndeliveredRecipientsToNewClone(TransportMailItem mailItem)
		{
			return this.shadowRedundancyComponent.ShadowRedundancyManager.MoveUndeliveredRecipientsToNewClone(mailItem);
		}

		protected virtual void NotifyShadowManagerMailItemIsPoison(TransportMailItem mailItem)
		{
			this.shadowRedundancyComponent.ShadowRedundancyManager.NotifyMailItemPoison(mailItem);
		}

		protected virtual void LogPoisonMessageCount(int poisonMessageCount)
		{
			this.poisonComponent.LogPoisonMessageCount(poisonMessageCount);
		}

		protected virtual IMExSession CreateMExSession()
		{
			return StorageAgentMExEvents.GetExecutionContext();
		}

		protected virtual void RaiseOnMessageLoadEvent(IMExSession session, TransportMailItem mailItem)
		{
			TransportMailItemWrapper mailItem2 = new TransportMailItemWrapper(mailItem, true);
			OnLoadedMessageEventSource onLoadedMessageEventSource = new OnLoadedMessageEventSource(mailItem);
			OnLoadedMessageEventArgs onLoadedMessageEventArgs = new OnLoadedMessageEventArgs(mailItem2);
			StorageAgentMExEvents.RaiseEvent(session, "OnLoadedMessage", new object[]
			{
				onLoadedMessageEventSource,
				onLoadedMessageEventArgs
			});
		}

		protected virtual void CloseMExSession(IMExSession session)
		{
			session.Close();
		}

		protected virtual void ExtendMailItemExpiry(TransportMailItem mailItem)
		{
			mailItem.SetExpirationTime(DateTime.UtcNow + this.configuration.MessageExpirationGracePeriod);
		}

		protected virtual DateTime GetMailItemExpiry(TransportMailItem mailItem)
		{
			return mailItem.Expiry;
		}

		private void HandlePoison(TransportMailItem mailItem, bool newPoisonMessage)
		{
			LatencyTracker.TrackPreProcessLatency(LatencyComponent.ServiceRestart, mailItem.LatencyTracker, mailItem.DateReceived);
			ExTraceGlobals.StorageTracer.TraceDebug(mailItem.MsgId, "Poison message detected.");
			mailItem.DropBreadcrumb(Breadcrumb.MailItemPoison);
			if (newPoisonMessage)
			{
				MessageTrackingLog.TrackPoisonMessage(MessageTrackingSource.POISONMESSAGE, mailItem);
			}
			if (this.IsPoisonMessageTooOld(mailItem))
			{
				this.DeletePoisonMessage(mailItem);
				return;
			}
			this.SendToPoisonQueue(mailItem);
			if (mailItem.IsShadowed())
			{
				this.NotifyShadowManagerMailItemIsPoison(mailItem);
				mailItem.CommitLazy();
			}
		}

		protected virtual void DeletePoisonMessage(TransportMailItem mailItem)
		{
			mailItem.Ack(AckStatus.Fail, AckReason.PoisonMessageExpired, mailItem.Recipients, null);
			MessageTrackingLog.TrackPoisonMessageDeleted(MessageTrackingSource.BOOTLOADER, "PoisonExpired", mailItem);
			mailItem.ReleaseFromActiveMaterializedLazy();
			ExTraceGlobals.QueuingTracer.TraceDebug<string>(0L, "Poison message {0} was deleted by the Bootscanner", mailItem.InternetMessageId);
		}

		private bool IsPoisonMessageTooOld(TransportMailItem mailItem)
		{
			return mailItem.DateReceived < DateTime.UtcNow - this.configuration.PoisonMessageRetentionPeriod;
		}

		private void HandleStrandedMessage(TransportMailItem mailItem, IMExSession session)
		{
			this.poisonComponent.SetMessageContext(mailItem, MessageProcessingSource.BootLoader);
			this.RaiseOnMessageLoadEvent(session, mailItem);
			if (!mailItem.IsActive && !mailItem.IsDiscardPending)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<string, string>(0L, "Bootscanner skipping item {0} with subject {1} because it was deleted by an agent", mailItem.InternetMessageId, mailItem.Subject);
				MessageTrackingLog.TrackExpiredMessageDropped(MessageTrackingSource.BOOTLOADER, mailItem, mailItem.Recipients.AllUnprocessed.ToList<MailRecipient>(), AckReason.MessageNotActive);
				return;
			}
			ExTraceGlobals.StorageTracer.TraceDebug<string, string>(0L, "Bootscanner handling item {0} with subject {1}", mailItem.InternetMessageId, mailItem.Subject);
			this.TrackLoadedMessage(mailItem);
			if (mailItem.IsHeartbeat)
			{
				ExTraceGlobals.StorageTracer.TraceDebug(mailItem.MsgId, "Bootloader found a heartbeat message.");
				this.SendToShadowQueue(mailItem);
				return;
			}
			if (mailItem.IsDiscardPending)
			{
				this.SendToDiscardPendingQueue(mailItem);
				if (!mailItem.IsShadow())
				{
					return;
				}
			}
			if (mailItem.IsShadow())
			{
				this.ProcessShadowItem(mailItem);
				return;
			}
			this.ProcessMessageForSubmission(mailItem);
		}

		private void Start()
		{
			if (this.thread != null)
			{
				if (!this.stopRequested)
				{
					return;
				}
				this.thread.Join();
				this.thread = null;
			}
			this.stopRequested = false;
			if (this.workCompleted)
			{
				return;
			}
			this.thread = new Thread(new ThreadStart(this.BackgroundScanner));
			this.thread.Start();
		}

		private bool ShouldExecute()
		{
			return this.targetRunningState == ServiceState.Active || this.targetRunningState == ServiceState.Draining;
		}

		private void Stop(bool async)
		{
			this.stopRequested = true;
			if (!async && this.thread != null)
			{
				this.thread.Join();
				this.thread = null;
			}
		}

		private void ProcessMessageForSubmission(TransportMailItem categorizerMailItem)
		{
			DateTime mailItemExpiry = this.GetMailItemExpiry(categorizerMailItem);
			if (DateTime.UtcNow > mailItemExpiry + this.configuration.MessageDropTimeout)
			{
				List<MailRecipient> list = categorizerMailItem.Recipients.AllUnprocessed.ToList<MailRecipient>();
				foreach (MailRecipient mailRecipient in list)
				{
					mailRecipient.Ack(AckStatus.SuccessNoDsn, AckReason.MessageTooOld);
				}
				categorizerMailItem.ReleaseFromActive();
				categorizerMailItem.CommitLazy();
				ExTraceGlobals.StorageTracer.TraceDebug<string, DateTime>(0L, "Bootscanner dropped expired mail item {0} with expiry time {1}", categorizerMailItem.InternetMessageId, mailItemExpiry);
				MessageTrackingLog.TrackExpiredMessageDropped(MessageTrackingSource.BOOTLOADER, categorizerMailItem, list, AckReason.MessageTooOld);
				return;
			}
			if (mailItemExpiry < DateTime.UtcNow)
			{
				this.ExtendMailItemExpiry(categorizerMailItem);
			}
			this.SendToSubmissionQueue(categorizerMailItem);
		}

		private void ProcessShadowItem(TransportMailItem mailItem)
		{
			TransportMailItem transportMailItem = null;
			if (mailItem.Status != Status.Complete)
			{
				transportMailItem = this.MoveUndeliveredRecipientsToNewClone(mailItem);
			}
			ExTraceGlobals.StorageTracer.TraceDebug<string>(mailItem.MsgId, "Bootloader found a {0} message.", mailItem.IsHeartbeat ? "heartbeat" : "shadow");
			this.SendToShadowQueue(mailItem);
			if (transportMailItem != null)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<long>(mailItem.MsgId, "Bootloader loaded an hybrid shadow message and moved the undelivered items to a new message with id {0}.", transportMailItem.MsgId);
				this.ProcessMessageForSubmission(transportMailItem);
			}
		}

		private void TrackLoadedMessage(TransportMailItem mailItem)
		{
			if (this.lastMessagesLoaded.Count == 10)
			{
				this.lastMessagesLoaded.Dequeue();
			}
			this.lastMessagesLoaded.Enqueue(mailItem);
		}

		private void AddLoadedMessageToReport(TransportMailItem mailItem, StringBuilder sb)
		{
			sb.AppendFormat("From: {0}", mailItem.Message.From.SmtpAddress);
			sb.AppendLine();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (MailRecipient mailRecipient in mailItem.Recipients)
			{
				stringBuilder.AppendFormat("{0}, ", mailRecipient.Email.ToString());
			}
			if (stringBuilder.Length > 1)
			{
				stringBuilder.Remove(stringBuilder.Length - 2, 2);
			}
			sb.AppendFormat("To: {0}", stringBuilder);
			sb.AppendLine();
			sb.AppendFormat("Subject: {0}", mailItem.Message.Subject);
			sb.AppendLine();
			sb.AppendFormat("Message ID: {0}", mailItem.Message.MessageId);
			sb.AppendLine();
		}

		private string CreateLoadedMessagesReport()
		{
			if (this.lastMessagesLoaded.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
				foreach (TransportMailItem transportMailItem in this.lastMessagesLoaded)
				{
					if (transportMailItem.IsActive)
					{
						this.AddLoadedMessageToReport(transportMailItem, stringBuilder);
						stringBuilder.AppendLine();
					}
				}
				return stringBuilder.ToString();
			}
			return null;
		}

		private void EventLogLoadedMessages(string loadedMessagesReport)
		{
			if (!string.IsNullOrEmpty(loadedMessagesReport))
			{
				this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_LastMessagesLoadedByBootScanner, null, new object[]
				{
					loadedMessagesReport
				});
			}
		}

		private void BackgroundScanner()
		{
			int num = 0;
			int num2 = 0;
			this.CacheUnprocessedMessageIds();
			IMExSession session = this.CreateMExSession();
			string text;
			string text2;
			this.GetMessageCountDetails(out text, out text2);
			this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_StartScanForMessages, null, new object[]
			{
				text2
			});
			ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "Starting to scan for active messages.{0}", text2);
			Stopwatch stopwatch = Stopwatch.StartNew();
			foreach (byte b in from i in this.unprocessedMessageIds.Keys
			orderby i
			select i)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<byte>(0L, "Starting to scan for active messages priority {0}.", b);
				foreach (MailItemAndRecipients mailItemAndRecipients in this.database.GetMessages(this.unprocessedMessageIds[b]))
				{
					TransportMailItem transportMailItem = TransportMailItem.NewMailItem(mailItemAndRecipients.MailItem, LatencyComponent.ServiceRestart);
					foreach (IMailRecipientStorage recipStorage in mailItemAndRecipients.Recipients)
					{
						transportMailItem.AddRecipient(recipStorage);
					}
					stopwatch.Stop();
					if (this.configuration.BootLoaderMessageTrackingEnabled)
					{
						MessageTrackingLog.TrackLoadedMessage(MessageTrackingSource.BOOTLOADER, MessageTrackingEvent.LOAD, transportMailItem);
					}
					bool newPoisonMessage;
					if (this.IsPoison(transportMailItem, out newPoisonMessage))
					{
						this.HandlePoison(transportMailItem, newPoisonMessage);
						num++;
						if (this.configuration.PoisonCountPublishingEnabled)
						{
							this.recentPoisonMessagesCounter.AddEventsAt(transportMailItem.DateReceived, 1L);
						}
					}
					else
					{
						this.HandleStrandedMessage(transportMailItem, session);
					}
					num2++;
					this.perfCounters.BootloaderOutstandingItems.Decrement();
					this.perfCounters.BootloadedItemCount.Increment();
					this.perfCounters.BootloadedItemAverageLatency.IncrementBy(stopwatch.ElapsedTicks);
					this.perfCounters.BootloadedItemAverageLatencyBase.Increment();
					stopwatch.Restart();
					if (this.stopRequested)
					{
						break;
					}
				}
				this.GetMessageCountDetails(out text, out text2);
				ExTraceGlobals.StorageTracer.TraceDebug<byte, string, string>(0L, "Finished scan for active messages priority {0}. Processed({1}), Unprocessed({2})", b, text, text2);
				this.eventLogger.LogEvent(this.stopRequested ? TransportEventLogConstants.Tuple_StopScanForMessages : TransportEventLogConstants.Tuple_EndScanForMessages, null, new object[]
				{
					text,
					text2
				});
				if (this.stopRequested)
				{
					break;
				}
			}
			this.CloseMExSession(session);
			ExTraceGlobals.StorageTracer.TraceDebug<string, int, int>(0L, "Bootscanner {0}. {1} items completed; {2} poison.", this.stopRequested ? "was stopped" : "has completed", num2, num);
			this.LogPoisonMessageCount(num);
			this.workCompleted = !this.stopRequested;
			if (this.workCompleted)
			{
				if (this.configuration.PoisonCountPublishingEnabled)
				{
					this.StartPoisonCountTimer();
				}
				this.OnBootLoadCompleted();
			}
		}

		private void CacheUnprocessedMessageIds()
		{
			if (this.unprocessedMessageIds == null)
			{
				MessagingDatabaseResultStatus messagingDatabaseResultStatus = this.database.ReadUnprocessedMessageIds(out this.unprocessedMessageIds);
				if (messagingDatabaseResultStatus != MessagingDatabaseResultStatus.Complete)
				{
					throw new InvalidOperationException("Could not get complete list of unprocessed message Ids");
				}
				this.perfCounters.BootloaderOutstandingItems.RawValue = (long)this.unprocessedMessageIds.Values.Sum((List<long> i) => i.Count);
			}
		}

		private void GetMessageCountDetails(out string processedCountDescription, out string unprocessedCountDescription)
		{
			Dictionary<byte, int> dictionary = this.unprocessedMessageIds.ToDictionary((KeyValuePair<byte, List<long>> o) => o.Key, (KeyValuePair<byte, List<long>> o) => o.Value.Count((long msgId) => msgId == 0L));
			processedCountDescription = string.Format("{0}({1})", dictionary.Values.Sum(), string.Join(",", from i in dictionary
			select string.Format("P{0}={1}", i.Key, i.Value)));
			unprocessedCountDescription = string.Format("{0}({1})", this.unprocessedMessageIds.Values.Sum((List<long> o) => o.Count) - dictionary.Values.Sum(), string.Join(",", from pc in dictionary
			select string.Format("P{0}={1}", pc.Key, this.unprocessedMessageIds[pc.Key].Count - pc.Value)));
		}

		protected virtual void StartPoisonCountTimer()
		{
			this.poisonMessageCountTimer = new GuardedTimer(new TimerCallback(this.PublishPoisonCount), null, TimeSpan.FromMinutes(5.0));
		}

		protected void PublishPoisonCount(object state)
		{
			this.perfCounters.BootloadedRecentPoisonMessageCount.RawValue = this.recentPoisonMessagesCounter.TimedUpdate();
		}

		private const int MaxLoadedMessagesToTrack = 10;

		protected IMessagingDatabase database;

		private readonly DatabasePerfCountersInstance perfCounters = DatabasePerfCounters.GetInstance("other");

		private Thread thread;

		private volatile bool stopRequested;

		private ExEventLog eventLogger;

		private ShadowRedundancyComponent shadowRedundancyComponent;

		private PoisonMessage poisonComponent;

		private QueueManager queueManagerComponent;

		private ICategorizer categorizerComponent;

		private bool workCompleted;

		private Dictionary<byte, List<long>> unprocessedMessageIds;

		private Queue<TransportMailItem> lastMessagesLoaded = new Queue<TransportMailItem>();

		private ServiceState targetRunningState;

		private IBootLoaderConfig configuration;

		private SegmentedSlidingCounter recentPoisonMessagesCounter;

		private GuardedTimer poisonMessageCountTimer;
	}
}
