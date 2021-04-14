using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Internal.MExRuntime;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Data.Transport.StoreDriver;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxTransport.StoreDriver;
using Microsoft.Exchange.MailboxTransport.StoreDriver.Shared;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.Common;
using Microsoft.Exchange.Transport.Internal;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class MailItemDeliver : DisposeTrackableBase, IMessageConverter
	{
		public MailItemDeliver(MbxTransportMailItem mailItem, ulong sessionId)
		{
			bool flag = false;
			try
			{
				this.Thread = Thread.CurrentThread;
				this.mbxTransportMailItem = mailItem;
				this.sessionId = sessionId;
				this.sessionSourceContext = StoreDriverDelivery.GenerateSessionSourceContext(sessionId, mailItem.SessionStartTime);
				this.wasSessionOpenedForLastRecipient = false;
				this.currentItem = new DeliveryItem(this);
				if (this.MbxTransportMailItem != null && this.MbxTransportMailItem.RootPart != null)
				{
					Header header = this.MbxTransportMailItem.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-Journal-Report");
					this.isJournalReport = (header != null);
				}
				else
				{
					this.isJournalReport = false;
				}
				if (this.currentItem.DisposeTracker != null)
				{
					this.currentItem.DisposeTracker.AddExtraDataWithStackTrace("MailItemDeliver owns currentItem at");
				}
				this.deliveredRecipients = new List<string>();
				this.deliveredRecipientsMailboxInfo = new List<string>();
				this.duplicateRecipients = new List<string>();
				this.breadcrumb = new MailItemDeliver.Breadcrumb();
				MailItemDeliver.deliveryHistory.Drop(this.breadcrumb);
				this.breadcrumb.Sender = this.MbxTransportMailItem.MimeSender.ToString();
				this.breadcrumb.MessageId = this.MbxTransportMailItem.InternetMessageId;
				this.breadcrumb.MailboxDatabase = this.MbxTransportMailItem.DatabaseName;
				this.breadcrumb.LatencyTracker = this.MbxTransportMailItem.LatencyTracker;
				MailItemDeliver.Diag.TraceDebug<long>(0L, "Delivering mailitem {0}", this.mbxTransportMailItem.RecordId);
				try
				{
					this.mexSession = MExEvents.GetExecutionContext(StoreDriverDeliveryServer.GetInstance(this.MbxTransportMailItem.OrganizationId));
					this.mexSession.Dispatcher.OnAgentInvokeEnd += this.AgentInvokeEndHandler;
				}
				catch (DataSourceTransientException exception)
				{
					throw new RetryException(new MessageStatus(MessageAction.RetryQueue, new SmtpResponse("432", "4.3.2", new string[]
					{
						"storedriver agent context transient failure"
					}), exception));
				}
				this.agentLatencyTracker = new AgentLatencyTracker(this.mexSession);
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					this.Dispose();
				}
			}
		}

		public int DeliveredRecipients
		{
			get
			{
				return this.deliveredRecipients.Count;
			}
		}

		public string Description
		{
			get
			{
				return "Deliver";
			}
		}

		public bool IsOutbound
		{
			get
			{
				return false;
			}
		}

		public string MessageClass
		{
			get
			{
				return this.messageClass;
			}
		}

		public MessageItem ReplayItem
		{
			get
			{
				return this.replayItem;
			}
		}

		public IDeliveryItem DeliveryItem
		{
			get
			{
				return this.currentItem;
			}
			set
			{
				this.currentItem = value;
			}
		}

		public MailRecipient Recipient
		{
			get
			{
				return this.recipient;
			}
			internal set
			{
				this.recipient = value;
			}
		}

		public bool IsPublicFolderRecipient { get; internal set; }

		public Guid RecipientMailboxGuid { get; set; }

		public bool IsJournalReport
		{
			get
			{
				return this.isJournalReport;
			}
		}

		public TimeSpan? RetryInterval
		{
			get
			{
				return this.retryInterval;
			}
		}

		public MbxMailItemWrapper MailItemWrapper
		{
			get
			{
				return this.mailItemWrapper;
			}
		}

		public Trace Tracer
		{
			get
			{
				return ExTraceGlobals.StoreDriverDeliveryTracer;
			}
		}

		internal static DeliveryRecipientThreadMap RecipientThreadMap
		{
			get
			{
				return MailItemDeliver.recipientThreadMap;
			}
		}

		internal string DeliverToFolderName
		{
			set
			{
				this.deliverToFolderName = value;
			}
		}

		internal ulong SessionId
		{
			get
			{
				return this.sessionId;
			}
		}

		internal MbxTransportMailItem MbxTransportMailItem
		{
			get
			{
				return this.mbxTransportMailItem;
			}
		}

		internal bool WasSessionOpenedForLastRecipient
		{
			get
			{
				return this.wasSessionOpenedForLastRecipient;
			}
			set
			{
				this.wasSessionOpenedForLastRecipient = value;
			}
		}

		internal List<KeyValuePair<string, string>> ExtraTrackingEventData { get; set; }

		internal ExDateTime RecipientStartTime { get; private set; }

		internal MailItemDeliver.DeliveryStage Stage
		{
			get
			{
				return this.deliveryStage;
			}
			set
			{
				this.deliveryStage = value;
			}
		}

		internal MailItemDeliver.Breadcrumb DeliveryBreadcrumb
		{
			get
			{
				return this.breadcrumb;
			}
			set
			{
				this.breadcrumb = value;
			}
		}

		internal int? DatabaseHealthMeasureToLog
		{
			get
			{
				return this.databaseHealthMeasureToLog;
			}
			set
			{
				this.databaseHealthMeasureToLog = value;
			}
		}

		internal StoreDriverDeliveryEventArgsImpl EventArguments
		{
			get
			{
				return this.eventArguments;
			}
		}

		internal Thread Thread { get; private set; }

		public void DeliverToRecipients()
		{
			PerformanceContext performanceContext = new PerformanceContext(PerformanceContext.Current);
			this.recipient = this.MbxTransportMailItem.GetNextRecipient();
			try
			{
				while (this.recipient != null)
				{
					TraceHelper.StoreDriverDeliveryTracer.TracePass<MailRecipient>(TraceHelper.MessageProbeActivityId, 0L, "Delivery: processing started for recipient {0}.", this.recipient);
					this.breadcrumb.RecipientCount++;
					MessageStatus messageStatus = this.DeliverToRecipient();
					if (messageStatus.NDRForAllRecipients)
					{
						this.FailAllRecipients(messageStatus);
						TraceHelper.StoreDriverDeliveryTracer.TraceFail(TraceHelper.MessageProbeActivityId, 0L, "Delivery: processing failed NDR being generated for all recipients.");
						break;
					}
					TraceHelper.StoreDriverDeliveryTracer.TracePass<MailRecipient>(TraceHelper.MessageProbeActivityId, 0L, "Delivery: processing completed for recipient {0}.", this.recipient);
					this.recipient = this.MbxTransportMailItem.GetNextRecipient();
				}
			}
			finally
			{
				this.breadcrumb.DeliveredToAll = ExDateTime.UtcNow.Subtract(this.breadcrumb.RecordCreation);
				TimeSpan timeSpan = PerformanceContext.Current.Latency - performanceContext.Latency;
				LatencyTracker.TrackExternalComponentLatency(LatencyComponent.StoreDriverDeliveryAD, this.MbxTransportMailItem.LatencyTracker, timeSpan);
				this.breadcrumb.LdapLatency = timeSpan;
				string arg = this.MbxTransportMailItem.InternetMessageId ?? "NoMessageId";
				MailItemDeliver.Diag.TraceDebug<TimeSpan, string>(0L, "Ldap latency {0} recorded for message {1}", timeSpan, arg);
				MailItemDeliver.Diag.TraceDebug<TimeSpan, string>(0L, "RPC latency {0} recorded for message {1}", this.rpcLatency, arg);
				LatencyTracker.TrackExternalComponentLatency(LatencyComponent.StoreDriverDeliveryStore, this.MbxTransportMailItem.LatencyTracker, this.storeLatency);
				MailItemDeliver.Diag.TraceDebug<TimeSpan, string>(0L, "Store latency {0} recorded for message {1}", this.storeLatency, arg);
			}
			this.WriteDeliveredAndProcessedTrackingLog();
		}

		public MessageStatus DeliverToRecipient()
		{
			MessageStatus messageStatus = null;
			this.RecipientStartTime = ExDateTime.UtcNow;
			string exceptionAgentName = string.Empty;
			using (ActivityScope activityScope = ActivityContext.Start(this))
			{
				activityScope.Component = "MailItemDeliver";
				try
				{
					MSExchangeStoreDriver.PendingDeliveries.Increment();
					this.deliveryStage = this.breadcrumb.SetStage(MailItemDeliver.DeliveryStage.Start);
					StoreDriverDeliveryPerfCounters.Instance.IncrementDeliveryAttempt(false);
					StoreDriverDatabasePerfCounters.IncrementDeliveryAttempt(this.mbxTransportMailItem.DatabaseGuid.ToString(), false);
					messageStatus = StorageExceptionHandler.RunUnderExceptionHandler(this, delegate
					{
						try
						{
							this.DeliverMessageForRecipient();
						}
						catch (StoreDriverAgentRaisedException ex)
						{
							exceptionAgentName = ex.AgentName;
							throw;
						}
					});
					if (!string.IsNullOrEmpty(exceptionAgentName))
					{
						this.recipient.ExtendedProperties.SetValue<string>("ExceptionAgentName", exceptionAgentName);
					}
					this.AcknowledgeDeliveryStatusForRecipient(messageStatus);
					if (this.IsPermanentFailure(messageStatus.Action) && !string.IsNullOrEmpty(exceptionAgentName))
					{
						MailItemDeliver.UpdateAgentDeliveryFailureStatistics(exceptionAgentName);
					}
					StoreDriverDeliveryPerfCounters.Instance.AddDeliveryLatencySample(ExDateTime.UtcNow - this.RecipientStartTime, false);
				}
				finally
				{
					MSExchangeStoreDriver.PendingDeliveries.Decrement();
					messageStatus = this.RaiseOnCompletedMessage();
					this.DecrementThrottlingForCurrentRecipient();
					this.DisposeCurrentMessageItem();
				}
			}
			MSExchangeStoreDriver.RecipientsDelivered.Increment();
			return messageStatus;
		}

		public void SetDeliveryFolder(Folder folder)
		{
			ArgumentValidator.ThrowIfNull("folder", folder);
			if (folder.Id == null || folder.Id.ObjectId == null)
			{
				throw new ArgumentException("Folder ID is null or incomplete", "folder");
			}
			this.currentItem.DeliverToFolder = folder.Id;
			this.deliverToFolderName = folder.DisplayName;
			string text = null;
			if (folder.Id.ObjectId.Equals(this.currentItem.MailboxSession.GetDefaultFolderId(DefaultFolderType.DeletedItems)))
			{
				text = "Deleted Items";
			}
			else if (folder.Id.ObjectId.Equals(this.currentItem.MailboxSession.GetDefaultFolderId(DefaultFolderType.JunkEmail)))
			{
				text = "Junk Email";
			}
			if (text != null && this.deliverToFolderName != null && !text.Equals(this.deliverToFolderName, StringComparison.Ordinal))
			{
				this.deliverToFolderName = string.Format(CultureInfo.InvariantCulture, "{0}<{1}>", new object[]
				{
					this.deliverToFolderName,
					text
				});
			}
		}

		public void LogMessage(Exception exception)
		{
			if (exception is StoragePermanentException && exception.InnerException != null && exception.InnerException is MapiExceptionDuplicateDelivery)
			{
				return;
			}
			if (exception is SmtpResponseException)
			{
				return;
			}
			ItemConversion.SaveFailedMimeDocument(this.MbxTransportMailItem.Message, exception, Components.Configuration.LocalServer.ContentConversionTracingPath);
		}

		public void AddDeliveryErrors(List<string> errorRecords)
		{
			if (errorRecords == null || errorRecords.Count == 0)
			{
				return;
			}
			if (this.deliveryErrors == null)
			{
				this.deliveryErrors = new List<string>(errorRecords.Count);
			}
			foreach (string item in errorRecords)
			{
				this.deliveryErrors.Add(item);
			}
		}

		internal static XElement GetDiagnosticInfo()
		{
			XElement xelement = new XElement("DeliveryHistory");
			foreach (MailItemDeliver.Breadcrumb breadcrumb in ((IEnumerable<MailItemDeliver.Breadcrumb>)MailItemDeliver.deliveryHistory))
			{
				xelement.Add(breadcrumb.GetDiagnosticInfo());
			}
			return xelement;
		}

		internal void AddRpcLatency(TimeSpan additionalLatency, string rpcType)
		{
			this.rpcLatency += additionalLatency;
			this.breadcrumb.RpcLatency = this.rpcLatency;
			MailItemDeliver.Diag.TraceDebug(0L, "{0} latency {1} added to for message {2}, making total RPC latency {3}", new object[]
			{
				rpcType,
				additionalLatency,
				this.MbxTransportMailItem.InternetMessageId ?? "NoMessageId",
				this.rpcLatency
			});
		}

		internal void BeginTrackLatency(LatencyComponent component)
		{
			if (this.MbxTransportMailItem != null)
			{
				LatencyTracker.BeginTrackLatency(component, this.MbxTransportMailItem.LatencyTracker);
			}
		}

		internal TimeSpan EndTrackLatency(LatencyComponent component)
		{
			if (this.mbxTransportMailItem != null)
			{
				return LatencyTracker.EndTrackLatency(component, this.mbxTransportMailItem.LatencyTracker, true);
			}
			return TimeSpan.Zero;
		}

		internal void CreateSession(DeliverableItem item)
		{
			this.currentItem.CreateSession(this.recipient, this.deliveryFlags, item, this.recipientLanguages);
			this.breadcrumb.MailboxServer = this.GetMailboxServerName();
		}

		internal bool LoadMessageForAgentEventsRetry()
		{
			try
			{
				if (this.currentItem.DeliverToFolder == null)
				{
					this.currentItem.DeliverToFolder = this.currentItem.MailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
				}
				this.currentItem.LoadMailboxMessage(this.replayItem.InternetMessageId);
				ExTraceGlobals.FaultInjectionTracer.TraceTest(64128U);
				ExTraceGlobals.FaultInjectionTracer.TraceTest(2265328957U);
			}
			catch (UnexpectedMessageCountException arg)
			{
				MailItemDeliver.Diag.TraceWarning<UnexpectedMessageCountException>(0L, "Unable to load the message to allow delivery agent event retry. Error {0}.", arg);
				return false;
			}
			catch (System.Data.ObjectNotFoundException arg2)
			{
				MailItemDeliver.Diag.TraceDebug<System.Data.ObjectNotFoundException>(0L, "Unable to load the message to allow delivery agent event retry. This operation will be retried. Error {0}.", arg2);
				return false;
			}
			return true;
		}

		internal void CreateMessage(DeliverableItem item)
		{
			if (this.IsPublicFolderRecipient)
			{
				this.currentItem.CreatePublicFolderMessage(this.recipient, item);
				return;
			}
			this.currentItem.CreateMailboxMessage(this.originalReceivedTime != null);
		}

		internal TimeSpan RaiseEvent(string deliveryEventBindings, LatencyComponent eventComponent)
		{
			DateTime utcNow = DateTime.UtcNow;
			TimeSpan result;
			try
			{
				this.agentLatencyTracker.BeginTrackLatency(eventComponent, this.MbxTransportMailItem.LatencyTracker);
				MExEvents.RaiseEvent(this.mexSession, deliveryEventBindings, new object[]
				{
					MailItemDeliver.eventSource,
					this.eventArguments
				});
				try
				{
					ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(3540397373U, deliveryEventBindings);
				}
				catch (LocalizedException actualAgentException)
				{
					throw new StoreDriverAgentRaisedException("Fault injection.", actualAgentException);
				}
			}
			finally
			{
				result = DateTime.UtcNow - utcNow;
				this.agentLatencyTracker.EndTrackLatency();
				this.MbxTransportMailItem.TrackAgentInfo();
			}
			return result;
		}

		internal void PromotePropertiesToItem()
		{
			if (this.spamConfidenceLevel != null)
			{
				this.currentItem.SetProperty(ItemSchema.SpamConfidenceLevel, this.spamConfidenceLevel);
			}
			if (this.originalReceivedTime != null)
			{
				this.currentItem.SetProperty(ItemSchema.ReceivedTime, this.originalReceivedTime.Value.ToUniversalTime());
			}
			this.PromoteDrmPropertiesToItem();
			byte[] array = null;
			if (!string.IsNullOrEmpty(this.recipient.ORcpt))
			{
				array = RedirectionHistory.GenerateRedirectionHistoryFromOrcpt(this.recipient.ORcpt);
			}
			if (array != null)
			{
				MailItemDeliver.Diag.TraceDebug<string>(0L, "Promote Redirection History {0}", this.recipient.Email.ToString());
				this.currentItem.SetProperty(RecipientSchema.RedirectionHistory, array);
			}
		}

		internal void ExtractCulture()
		{
			ADRecipientCache<TransportMiniRecipient> adrecipientCache = this.MbxTransportMailItem.ADRecipientCache;
			ProxyAddress proxyAddress = new SmtpProxyAddress((string)this.recipient.Email, true);
			TransportMiniRecipient data = adrecipientCache.FindAndCacheRecipient(proxyAddress).Data;
			if (data != null)
			{
				this.recipientLanguages = data.Languages;
			}
		}

		internal void DeliverItem()
		{
			ExTraceGlobals.FaultInjectionTracer.TraceTest(55936U);
			if (this.eventArguments.PropertiesForAllMessageCopies != null)
			{
				foreach (KeyValuePair<PropertyDefinition, object> keyValuePair in this.eventArguments.PropertiesForAllMessageCopies)
				{
					this.currentItem.Message.SetOrDeleteProperty(keyValuePair.Key, keyValuePair.Value);
				}
			}
			this.BeginTrackLatency(LatencyComponent.StoreDriverDeliveryRpc);
			try
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(2227580221U, this.currentItem.Message.ConversationTopic);
				ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(61072U, this.currentItem.Session.MdbGuid.ToString());
				ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(36496U, this.currentItem.Session.ServerFullyQualifiedDomainName);
				ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(44688U, (this.currentItem.MailboxSession != null) ? this.currentItem.MailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString() : null);
				this.currentItem.Deliver(ProxyAddress.Parse(ProxyAddressPrefix.Smtp.PrimaryPrefix, (string)this.recipient.Email));
			}
			catch (StoragePermanentException exception)
			{
				this.LogMessage(exception);
				throw;
			}
			finally
			{
				TimeSpan additionalLatency = this.EndTrackLatency(LatencyComponent.StoreDriverDeliveryRpc);
				this.AddRpcLatency(additionalLatency, "Deliver");
			}
		}

		internal void CreateReplay()
		{
			if (this.replayCreated)
			{
				return;
			}
			try
			{
				this.BeginTrackLatency(LatencyComponent.StoreDriverDeliveryContentConversion);
				this.GetInboundConversionOptions();
				this.GetSpamConfidenceLevel();
				this.GetOriginalReceivedTime();
				this.GetDeliveryFlags();
				this.GetVirusScanningStamps();
				this.CreateReplayItem();
				this.replayCreated = true;
			}
			finally
			{
				this.breadcrumb.ContentConversionLatency = this.EndTrackLatency(LatencyComponent.StoreDriverDeliveryContentConversion);
				MailItemDeliver.Diag.TraceDebug<TimeSpan, string>(0L, "Content conversion latency {0} for message {1}", this.breadcrumb.ContentConversionLatency, this.MbxTransportMailItem.InternetMessageId ?? "NoMessageId");
			}
			this.PromoteDrmPropertiesToReplayItem();
		}

		internal void ClearRetryOnDuplicateDelivery()
		{
			if (this.recipient != null)
			{
				this.recipient.ExtendedProperties.Remove("Microsoft.Exchange.Transport.MailboxTransport.RetryOnDuplicateDelivery ");
			}
		}

		internal void RaiseOnDeliveredEvent()
		{
			this.deliveryStage = this.breadcrumb.SetStage(MailItemDeliver.DeliveryStage.OnDeliveredEvent);
			try
			{
				this.RaiseEvent("OnDeliveredMessage", LatencyComponent.StoreDriverOnDeliveredMessage);
				this.deliveryStage = this.breadcrumb.SetStage(MailItemDeliver.DeliveryStage.Done);
			}
			finally
			{
				TraceHelper.StoreDriverDeliveryTracer.TracePass<MailRecipient>(TraceHelper.MessageProbeActivityId, 0L, "Delivery: OnDeliveredMessage complete for recipient {0}.", this.recipient);
			}
		}

		internal void AddRecipientInfoForDeliveredEvent()
		{
			this.AddDeliveryErrors(ref this.deliveredRecipientsErrors, this.deliveryErrors);
			MailItemDeliver.AddFolderNameIfNeeded(ref this.deliveredRecipientsFolderNames, this.deliverToFolderName, this.deliveredRecipients.Count);
			this.deliveredRecipients.Add(this.recipient.Email.ToString());
			this.deliveredRecipientsMailboxInfo.Add(this.RecipientMailboxGuid.ToString());
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MailItemDeliver>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (this.currentItem != null && this.currentItem.DisposeTracker != null)
			{
				this.currentItem.DisposeTracker.AddExtraDataWithStackTrace(string.Format(CultureInfo.InvariantCulture, "MailItemDeliver.InternalDispose({0}) called with stack", new object[]
				{
					disposing
				}));
			}
			if (disposing)
			{
				try
				{
					if (this.currentItem != null)
					{
						this.currentItem.Dispose();
						this.currentItem = null;
					}
					if (this.replayItem != null)
					{
						this.replayItem.Dispose();
						this.replayItem = null;
					}
					if (this.agentLatencyTracker != null)
					{
						this.agentLatencyTracker.Dispose();
						this.agentLatencyTracker = null;
					}
				}
				finally
				{
					if (this.mexSession != null)
					{
						MExEvents.FreeExecutionContext(this.mexSession);
						this.mexSession = null;
					}
				}
			}
		}

		private static bool IsPublicFolder(MailRecipient recipient)
		{
			DeliverableItem deliverableItem = RecipientItem.Create(recipient) as DeliverableItem;
			return deliverableItem.RecipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.PublicFolder;
		}

		private static void AddFolderNameIfNeeded(ref List<string> folderNameList, string folderName, int count)
		{
			if (folderNameList == null)
			{
				if (string.IsNullOrEmpty(folderName))
				{
					return;
				}
				folderNameList = new List<string>(count);
				while (count > 0)
				{
					folderNameList.Add(null);
					count--;
				}
			}
			folderNameList.Add(folderName);
		}

		private static string CombineDeliveryErrors(List<string> errors)
		{
			string result = null;
			if (errors != null && errors.Count > 0)
			{
				result = string.Join(" ", errors.ToArray());
			}
			return result;
		}

		private static void UpdateAgentDeliveryFailureStatistics(string exceptionAgentName)
		{
			StoreDriverDeliveryAgentPerfCounters.IncrementAgentDeliveryAttempt(exceptionAgentName);
			StoreDriverDeliveryAgentPerfCounters.IncrementAgentDeliveryFailure(exceptionAgentName);
			StoreDriverDeliveryAgentPerfCounters.RefreshAgentDeliveryPercentCounter(exceptionAgentName);
		}

		private void DeliverMessageForRecipient()
		{
			this.Initialize();
			ExTraceGlobals.FaultInjectionTracer.TraceTest(57488U);
			IDeliveryProcessor deliveryProcessor = DeliveryProcessorFactory.Create(this);
			this.deliveryStage = this.breadcrumb.SetStage(MailItemDeliver.DeliveryStage.OnInitializedEvent);
			deliveryProcessor.Initialize();
			TraceHelper.StoreDriverDeliveryTracer.TracePass<MailRecipient>(TraceHelper.MessageProbeActivityId, 0L, "Delivery: OnInitializedMessage complete for recipient {0}.", this.recipient);
			DeliverableItem item = deliveryProcessor.CreateSession();
			TraceHelper.StoreDriverDeliveryTracer.TracePass<MailRecipient>(TraceHelper.MessageProbeActivityId, 0L, "Delivery: OnPromotedMessage complete for recipient {0}.", this.recipient);
			deliveryProcessor.CreateMessage(item);
			TraceHelper.StoreDriverDeliveryTracer.TracePass<MailRecipient>(TraceHelper.MessageProbeActivityId, 0L, "Delivery: OnCreatedMessage complete for recipient {0}.", this.recipient);
			deliveryProcessor.DeliverMessage();
		}

		private void AcknowledgeDeliveryStatusForRecipient(MessageStatus messageStatus)
		{
			if (messageStatus.Action != MessageAction.Retry)
			{
				this.ClearRetryOnDuplicateDelivery();
			}
			if (messageStatus.Action == MessageAction.Success)
			{
				this.ProcessDeliveryForRecipient();
				TraceHelper.StoreDriverDeliveryTracer.TracePass<MailRecipient, MessageAction>(TraceHelper.MessageProbeActivityId, 0L, "Delivery: Processing complete for recipient {0} Action: {1}.", this.recipient, messageStatus.Action);
				return;
			}
			TraceHelper.StoreDriverDeliveryTracer.TraceFail<MailRecipient, MessageAction>(TraceHelper.MessageProbeActivityId, 0L, "Delivery: Processing complete for recipient {0} Action: {1}.", this.recipient, messageStatus.Action);
			StoreDriverDeliveryDiagnostics.RecordExceptionForDiagnostics(messageStatus, this);
			switch (messageStatus.Action)
			{
			case MessageAction.Retry:
				if (this.deliveryStage != MailItemDeliver.DeliveryStage.OnDeliveredEvent || !(messageStatus.Exception is StoreDriverAgentTransientException))
				{
					ExTraceGlobals.FaultInjectionTracer.TraceTest(65168U);
					this.ClearRetryOnDuplicateDelivery();
					this.AckRetryRecipient(messageStatus.Response);
					if (messageStatus.RetryInterval != null && (this.retryInterval == null || messageStatus.RetryInterval.Value < this.retryInterval.Value))
					{
						this.retryInterval = messageStatus.RetryInterval;
					}
					MSExchangeStoreDriver.DeliveryRetry.Increment();
					this.IncrementDeliveryFailure(messageStatus.Action);
					return;
				}
				ExTraceGlobals.FaultInjectionTracer.TraceTest(50816U);
				MailItemDeliver.Diag.TraceDebug(0L, "Store Driver Agent transient exception thrown during the OnDeliveredMessage event. The message was delivered but is being retried to complete message processing.");
				if (this.recipient.ExtendedProperties.GetValue<bool>("Microsoft.Exchange.Transport.MailboxTransport.RetryOnDuplicateDelivery ", false))
				{
					this.ProcessDuplicateDeliveryForRecipient();
					return;
				}
				this.recipient.ExtendedProperties.SetValue<bool>("Microsoft.Exchange.Transport.MailboxTransport.RetryOnDuplicateDelivery ", true);
				MailItemDeliver.Diag.TraceDebug(0L, "RetryOnDuplicateDelivery set by delivery processing for incoming message.");
				this.ProcessDeliveryForRecipient();
				return;
			case MessageAction.RetryQueue:
				ExTraceGlobals.FaultInjectionTracer.TraceTest(61584U);
				this.IncrementDeliveryFailure(messageStatus.Action);
				throw new RetryException(messageStatus, this.FormatStoreDriverContext());
			case MessageAction.NDR:
				ExTraceGlobals.FaultInjectionTracer.TraceTest(48784U);
				this.AckFailedRecipient(messageStatus.Response);
				this.IncrementDeliveryFailure(messageStatus.Action);
				return;
			case MessageAction.Reroute:
				ExTraceGlobals.FaultInjectionTracer.TraceTest(2445684029U);
				this.AckRerouteRecipient(messageStatus.Response);
				this.recipient.SmtpResponse = messageStatus.Response;
				MSExchangeStoreDriver.DeliveryReroute.Increment();
				this.IncrementDeliveryFailure(messageStatus.Action);
				return;
			case MessageAction.LogDuplicate:
				ExTraceGlobals.FaultInjectionTracer.TraceTest(47744U);
				this.ProcessDuplicateDeliveryForRecipient();
				return;
			case MessageAction.LogProcess:
				this.AckDeliveredRecipient();
				this.AddProcessedRecipient((string)this.recipient.Email, messageStatus);
				MSExchangeStoreDriver.SuccessfulDeliveries.Increment();
				MSExchangeStoreDriver.BytesDelivered.IncrementBy(this.MbxTransportMailItem.MimeSize);
				return;
			}
			throw new InvalidOperationException("Unexpected message action!");
		}

		private MessageStatus RaiseOnCompletedMessage()
		{
			string exceptionAgentName = string.Empty;
			MessageStatus messageStatus = StorageExceptionHandler.RunUnderExceptionHandler(this, delegate
			{
				try
				{
					this.RaiseEvent("OnCompletedMessage", LatencyComponent.StoreDriverOnCompletedMessage);
				}
				catch (StoreDriverAgentRaisedException ex)
				{
					exceptionAgentName = ex.AgentName;
					throw;
				}
			});
			ExTraceGlobals.FaultInjectionTracer.TraceTest<MessageAction>(49504U, messageStatus.Action);
			if (messageStatus.Action != MessageAction.Success)
			{
				MailItemDeliver.Diag.TraceDebug<SmtpResponse>(0L, "OnCompletedMessage failed: {0}", messageStatus.Response);
				if (this.IsPermanentFailure(messageStatus.Action) && !string.IsNullOrEmpty(exceptionAgentName))
				{
					MailItemDeliver.UpdateAgentDeliveryFailureStatistics(exceptionAgentName);
				}
			}
			return messageStatus;
		}

		private void DisposeCurrentMessageItem()
		{
			if (this.currentItem != null)
			{
				if (this.currentItem.DisposeTracker != null)
				{
					this.currentItem.DisposeTracker.AddExtraDataWithStackTrace("MailItemDeliver.DeliverToRecipient disposing currentItem with stack");
				}
				if (this.currentItem.Session != null)
				{
					this.AddStoreLatency(this.currentItem.Session.GetStoreCumulativeRPCStats().timeInServer);
				}
				this.currentItem.DisposeMessageAndSession();
			}
		}

		private string FormatStoreDriverContext()
		{
			string text = null;
			if ((this.deliveryStage == MailItemDeliver.DeliveryStage.OnInitializedEvent || this.deliveryStage == MailItemDeliver.DeliveryStage.OnPromotedEvent || this.deliveryStage == MailItemDeliver.DeliveryStage.OnCreatedEvent || this.deliveryStage == MailItemDeliver.DeliveryStage.OnDeliveredEvent) && this.mexSession != null)
			{
				text = this.mexSession.LastAgentName;
			}
			string text2 = this.deliveryStage.ToString();
			string result;
			if (!string.IsNullOrEmpty(text))
			{
				result = string.Format(CultureInfo.InvariantCulture, "[Stage: {0}][Agent: {1}]", new object[]
				{
					text2,
					text
				});
			}
			else
			{
				result = string.Format(CultureInfo.InvariantCulture, "[Stage: {0}]", new object[]
				{
					text2
				});
			}
			return result;
		}

		private void AgentInvokeEndHandler(object dispatcher, MExSession session)
		{
			StoreDriverDeliveryAgentPerfCounters.IncrementAgentDeliveryAttempt(session.CurrentAgent.Name);
			StoreDriverDeliveryAgentPerfCounters.RefreshAgentDeliveryPercentCounter(session.CurrentAgent.Name);
		}

		private void FailAllRecipients(MessageStatus deliveryStatus)
		{
			MailItemDeliver.Diag.TraceDebug<string>(0L, "Fail all recipients with the same response {0}", deliveryStatus.Response.EnhancedStatusCode);
			this.recipient = this.MbxTransportMailItem.GetNextRecipient();
			while (this.recipient != null)
			{
				this.AckFailedRecipient(deliveryStatus.Response);
				MSExchangeStoreDriver.RecipientsDelivered.Increment();
				this.IncrementDeliveryFailure(MessageAction.NDR);
				this.recipient = this.MbxTransportMailItem.GetNextRecipient();
			}
		}

		private void AddProcessedRecipient(string recipient, MessageStatus status)
		{
			if (this.processedRecipients == null)
			{
				this.processedRecipients = new List<string>(1);
				this.processedRecipientsSources = new List<string>(1);
			}
			this.processedRecipients.Add(recipient);
			string item = string.Format("{0};{1}", this.FormatStoreDriverContext(), status.Response.StatusText[0]);
			this.processedRecipientsSources.Add(item);
		}

		private void PromoteDrmPropertiesToItem()
		{
			string text;
			if (this.recipient.ExtendedProperties.TryGetValue<string>("Microsoft.Exchange.RightsManagement.B2BDRMLicense", out text) && !string.IsNullOrEmpty(text))
			{
				MailItemDeliver.Diag.TraceDebug<RoutingAddress>(0L, "Promote DRMServerLicenseCompressed property for the B2B case. Recipient {0}", this.recipient.Email);
				using (Stream stream = this.currentItem.OpenPropertyStream(MessageItemSchema.DRMServerLicenseCompressed, PropertyOpenMode.Create))
				{
					DrmEmailCompression.CompressUseLicense(text, stream);
				}
			}
			ReadOnlyCollection<byte> readOnlyCollection;
			int num;
			if (this.recipient.ExtendedProperties.TryGetListValue<byte>("Microsoft.Exchange.RightsManagement.DRMLicense", out readOnlyCollection) && readOnlyCollection != null)
			{
				MailItemDeliver.Diag.TraceDebug<RoutingAddress>(0L, "Promote License for recipient {0}", this.recipient.Email);
				byte[][] array = new byte[][]
				{
					new byte[readOnlyCollection.Count]
				};
				readOnlyCollection.CopyTo(array[0], 0);
				this.currentItem.SetProperty(MessageItemSchema.DRMLicense, array);
			}
			else if (this.recipient.ExtendedProperties.TryGetValue<int>("Microsoft.Exchange.RightsManagement.DRMFailure", out num))
			{
				MailItemDeliver.Diag.TraceDebug<RoutingAddress>(0L, "Promote DRMPrelicenseFailure property for recipient {0}", this.recipient.Email);
				this.currentItem.SetProperty(MessageItemSchema.DRMPrelicenseFailure, num);
			}
			int num2;
			if (this.recipient.ExtendedProperties.TryGetValue<int>("Microsoft.Exchange.RightsManagement.DRMRights", out num2))
			{
				MailItemDeliver.Diag.TraceDebug<RoutingAddress>(0L, "Promote DRMRights property for recipient {0}", this.recipient.Email);
				this.currentItem.SetProperty(MessageItemSchema.DRMRights, num2);
			}
			DateTime dateTime;
			if (this.recipient.ExtendedProperties.TryGetValue<DateTime>("Microsoft.Exchange.RightsManagement.DRMExpiryTime", out dateTime))
			{
				MailItemDeliver.Diag.TraceDebug<RoutingAddress>(0L, "Promote DRMExpiryTime property for recipient {0}", this.recipient.Email);
				this.currentItem.SetProperty(MessageItemSchema.DRMExpiryTime, new ExDateTime(ExTimeZone.TimeZoneFromKind(DateTimeKind.Utc), dateTime));
			}
			ReadOnlyCollection<byte> readOnlyCollection2;
			if (this.recipient.ExtendedProperties.TryGetListValue<byte>("Microsoft.Exchange.RightsManagement.DRMPropsSignature", out readOnlyCollection2))
			{
				MailItemDeliver.Diag.TraceDebug<RoutingAddress>(0L, "Promote DRMPropsSignature property for recipient {0}", this.recipient.Email);
				byte[] array2 = new byte[readOnlyCollection2.Count];
				readOnlyCollection2.CopyTo(array2, 0);
				this.currentItem.SetProperty(MessageItemSchema.DRMPropsSignature, array2);
			}
		}

		private void GetInboundConversionOptions()
		{
			this.conversionOptions = new InboundConversionOptions(Components.Configuration.FirstOrgAcceptedDomainTable.DefaultDomainName);
			this.conversionOptions.IsSenderTrusted = MultilevelAuth.IsInternalMail(this.MbxTransportMailItem);
			this.conversionOptions.ServerSubmittedSecurely = MultilevelAuth.IsAuthenticated(this.MbxTransportMailItem);
			this.conversionOptions.ClearCategories = this.MbxTransportMailItem.TransportSettings.ClearCategories;
			this.conversionOptions.Limits.MimeLimits = MimeLimits.Unlimited;
			this.conversionOptions.PreserveReportBody = this.MbxTransportMailItem.TransportSettings.PreserveReportBodypart;
			this.conversionOptions.ConvertReportToMessage = this.MbxTransportMailItem.TransportSettings.ConvertReportToMessage;
			this.conversionOptions.LogDirectoryPath = Components.Configuration.LocalServer.ContentConversionTracingPath;
			this.conversionOptions.DetectionOptions.PreferredInternetCodePageForShiftJis = Components.TransportAppConfig.ContentConversion.PreferredInternetCodePageForShiftJis;
			this.conversionOptions.HeaderPromotion = Configuration.TransportConfigObject.HeaderPromotionModeSetting;
			if (this.MbxTransportMailItem.TransportSettings.OpenDomainRoutingEnabled)
			{
				this.conversionOptions.RecipientCache = new EmptyRecipientCache();
				this.conversionOptions.UserADSession = null;
			}
			else
			{
				this.conversionOptions.RecipientCache = this.MbxTransportMailItem.ADRecipientCache;
				this.conversionOptions.UserADSession = this.MbxTransportMailItem.ADRecipientCache.ADSession;
			}
			this.conversionOptions.TreatInlineDispositionAsAttachment = Components.TransportAppConfig.ContentConversion.TreatInlineDispositionAsAttachment;
			EmailRecipient sender = this.MbxTransportMailItem.Message.Sender;
			if (sender != null)
			{
				RoutingAddress smtpAddress = new RoutingAddress(sender.SmtpAddress);
				if (smtpAddress.IsValid)
				{
					RemoteDomainEntry domainContentConfig = ContentConverter.GetDomainContentConfig(smtpAddress, this.MbxTransportMailItem.OrganizationId);
					Charset defaultCharset = null;
					if (domainContentConfig != null)
					{
						this.conversionOptions.DetectionOptions.PreferredInternetCodePageForShiftJis = (int)domainContentConfig.PreferredInternetCodePageForShiftJis;
						if (domainContentConfig.RequiredCharsetCoverage != null)
						{
							this.conversionOptions.DetectionOptions.RequiredCoverage = domainContentConfig.RequiredCharsetCoverage.Value;
						}
						if (Charset.TryGetCharset(domainContentConfig.CharacterSet, out defaultCharset))
						{
							this.conversionOptions.DefaultCharset = defaultCharset;
						}
					}
				}
			}
		}

		private void GetSpamConfidenceLevel()
		{
			try
			{
				Header header = this.MbxTransportMailItem.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-SCL");
				int num;
				if (header != null && int.TryParse(header.Value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out num))
				{
					num = ((num < -1) ? -1 : ((num > 9) ? 9 : num));
					this.spamConfidenceLevel = num;
				}
			}
			catch (ExchangeDataException)
			{
			}
		}

		private void PromoteDrmPropertiesToReplayItem()
		{
			if (this.replayItem == null)
			{
				throw new InvalidOperationException("replay item must be created first.");
			}
			string text;
			if (this.MbxTransportMailItem.ExtendedProperties.TryGetValue<string>("Microsoft.Exchange.RightsManagement.TransportDecryptionUL", out text) && !string.IsNullOrEmpty(text) && !DrmClientUtils.IsCachingOfLicenseDisabled(text))
			{
				MailItemDeliver.Diag.TraceDebug(0L, "Promote DRMServerLicenseCompressed property");
				using (Stream stream = this.replayItem.OpenPropertyStream(MessageItemSchema.DRMServerLicenseCompressed, PropertyOpenMode.Create))
				{
					DrmEmailCompression.CompressUseLicense(text, stream);
				}
			}
			if (this.replayItem.IsRestricted && this.replayItem.IconIndex == IconIndex.Default)
			{
				this.replayItem.IconIndex = IconIndex.MailIrm;
			}
		}

		private void GetOriginalReceivedTime()
		{
			Header header = this.MbxTransportMailItem.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-Original-Received-Time");
			DateTime value;
			if (header != null && Util.TryParseOrganizationalMessageArrivalTime(header.Value, out value))
			{
				this.originalReceivedTime = new DateTime?(value);
			}
		}

		private void AckDeliveredRecipient()
		{
			MailItemDeliver.Diag.TracePfd<int, string>(0L, "PFD ESD {0} Ack Delivered Recipient {1}", 31899, this.recipient.Email.ToString());
			this.MbxTransportMailItem.AckRecipient(AckStatus.Success, SmtpResponse.Empty);
		}

		private void AckFailedRecipient(SmtpResponse response)
		{
			MailItemDeliver.Diag.TraceDebug<string, string>(0L, "Ack Failed Recipient {0} SmtpResponse: {1}", this.recipient.Email.ToString(), response.EnhancedStatusCode);
			SmtpResponse smtpResponse = this.FormatOneLineResponse(response);
			this.MbxTransportMailItem.AckRecipient(AckStatus.Fail, smtpResponse);
		}

		private void AckRetryRecipient(SmtpResponse response)
		{
			MailItemDeliver.Diag.TraceDebug<string>(0L, "Ack Retry Recipient {0}", this.recipient.Email.ToString());
			SmtpResponse smtpResponse = this.FormatOneLineResponse(response);
			this.MbxTransportMailItem.AckRecipient(AckStatus.Retry, smtpResponse);
		}

		private void AckRerouteRecipient(SmtpResponse response)
		{
			MailItemDeliver.Diag.TraceDebug<string>(0L, "Ack Rerouted Recipient {0}", this.recipient.Email.ToString());
			this.MbxTransportMailItem.AckRecipient(AckStatus.Resubmit, response);
		}

		private void AckDuplicateRecipient()
		{
			MailItemDeliver.Diag.TraceDebug<string>(0L, "Ack Duplicate Recipient {0}", this.recipient.Email.ToString());
			this.MbxTransportMailItem.AckRecipient(AckStatus.SuccessNoDsn, SmtpResponse.Empty);
			MailItemDeliver.AddFolderNameIfNeeded(ref this.duplicateRecipientsFolderNames, this.deliverToFolderName, this.duplicateRecipients.Count);
			this.duplicateRecipients.Add(this.recipient.Email.ToString());
		}

		private SmtpResponse FormatOneLineResponse(SmtpResponse response)
		{
			string text = this.FormatStoreDriverContext();
			string text2 = string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
			{
				SmtpResponseGenerator.FlattenStatusText(response),
				text
			});
			return new SmtpResponse(response.StatusCode, response.EnhancedStatusCode, new string[]
			{
				text2
			});
		}

		private void AddDeliveryErrors(ref List<string> errorCollection, List<string> newErrors)
		{
			if (newErrors == null || newErrors.Count == 0)
			{
				return;
			}
			if (errorCollection == null)
			{
				errorCollection = new List<string>(newErrors.Count);
			}
			errorCollection.AddRange(newErrors);
		}

		private string GetMailboxServerName()
		{
			return StoreDriverDelivery.MailboxServerName ?? this.MbxTransportMailItem.DatabaseName;
		}

		private void WriteDeliveredAndProcessedTrackingLog()
		{
			this.MbxTransportMailItem.FinalizeDeliveryLatencyTracking(LatencyComponent.StoreDriverDelivery);
			if (this.ExtraTrackingEventData == null)
			{
				this.ExtraTrackingEventData = new List<KeyValuePair<string, string>>();
			}
			string databaseName = this.MbxTransportMailItem.DatabaseName;
			if (!string.IsNullOrEmpty(databaseName))
			{
				this.ExtraTrackingEventData.Add(new KeyValuePair<string, string>("MailboxDatabaseName", databaseName));
			}
			string value = MailItemDeliver.CombineDeliveryErrors(this.deliveredRecipientsErrors);
			if (!string.IsNullOrEmpty(value))
			{
				this.ExtraTrackingEventData.Add(new KeyValuePair<string, string>("DeliveryErrors", value));
			}
			if (this.databaseHealthMeasureToLog != null)
			{
				int value2 = this.databaseHealthMeasureToLog.Value;
				if (value2 != 100)
				{
					this.ExtraTrackingEventData.Add(new KeyValuePair<string, string>("DatabaseHealth", string.Format("{0:D}", value2)));
				}
			}
			if (ObjectClass.IsDsnNegative(this.messageClass))
			{
				this.ExtraTrackingEventData.Add(new KeyValuePair<string, string>("NDR", "1"));
			}
			this.ExtraTrackingEventData.Add(new KeyValuePair<string, string>("Mailboxes", string.Join(";", this.deliveredRecipientsMailboxInfo)));
			this.ExtraTrackingEventData.Add(new KeyValuePair<string, string>("ToEntity", RoutingEndpoint.Hosted.ToString()));
			Header header = this.MbxTransportMailItem.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-FromEntityHeader");
			if (header != null && !string.IsNullOrEmpty(header.Value))
			{
				this.ExtraTrackingEventData.Add(new KeyValuePair<string, string>("FromEntity", header.Value));
			}
			string value3 = this.MbxTransportMailItem.ExtendedProperties.GetValue<string>("Microsoft.Exchange.Transport.DeliveryQueueMailboxSubComponent", LatencyComponent.None.ToString());
			LatencyComponent previousHopSubComponent;
			if (!Enum.TryParse<LatencyComponent>(value3, out previousHopSubComponent))
			{
				previousHopSubComponent = LatencyComponent.None;
			}
			long value4 = this.MbxTransportMailItem.ExtendedProperties.GetValue<long>("Microsoft.Exchange.Transport.DeliveryQueueMailboxSubComponentLatency", 0L);
			TimeSpan previousHopSubComponentLatency = TimeSpan.FromSeconds((double)value4);
			LatencyHeaderManager.HandleLatencyHeaders(this.MbxTransportMailItem.TransportSettings.InternalSMTPServers, this.MbxTransportMailItem.RootPart.Headers, this.MbxTransportMailItem.DateReceived, LatencyComponent.DeliveryQueueMailbox, previousHopSubComponent, previousHopSubComponentLatency);
			LatencyFormatter latencyFormatter = new LatencyFormatter(this.MbxTransportMailItem, Components.Configuration.LocalServer.TransportServer.Fqdn, true);
			string arg = "ClientSubmitTime:";
			if (this.MbxTransportMailItem.ClientSubmitTime != DateTime.MinValue)
			{
				arg = string.Format(CultureInfo.InvariantCulture, "ClientSubmitTime:{0:yyyy-MM-ddTHH\\:mm\\:ss.fffZ}", new object[]
				{
					this.MbxTransportMailItem.ClientSubmitTime
				});
			}
			string sourceContext = string.Format("{0};{1}", this.sessionSourceContext, arg);
			string value5 = this.MbxTransportMailItem.ExtendedProperties.GetValue<string>("Microsoft.Exchange.Transport.MailboxTransport.SmtpInClientHostname", string.Empty);
			if (this.deliveredRecipients.Count != 0)
			{
				MailItemDeliver.Diag.TraceDebug<int>(0L, "Writing message tracking delivered entry for {0} recipients", this.deliveredRecipients.Count);
				SystemProbe.TracePass<int>(this.MbxTransportMailItem, "StoreDriver", "Message delivered for {0} recipients", this.deliveredRecipients.Count);
				MessageTrackingLog.TrackDelivered(MessageTrackingSource.STOREDRIVER, this.MbxTransportMailItem, this.deliveredRecipients, this.deliveredRecipientsFolderNames, value5, this.GetMailboxServerName(), latencyFormatter, sourceContext, this.ExtraTrackingEventData);
				ExTraceGlobals.FaultInjectionTracer.TraceTest(54880U);
			}
			if (this.duplicateRecipients.Count != 0)
			{
				MailItemDeliver.Diag.TraceDebug<int>(0L, "Writing message tracking duplicate delivery entry for {0} recipients", this.duplicateRecipients.Count);
				SystemProbe.TracePass<int>(this.MbxTransportMailItem, "StoreDriver", "Message duplicate delivery for {0} recipients", this.duplicateRecipients.Count);
				MessageTrackingLog.TrackDuplicateDelivery(MessageTrackingSource.STOREDRIVER, this.MbxTransportMailItem, this.duplicateRecipients, this.duplicateRecipientsFolderNames, value5, this.GetMailboxServerName(), latencyFormatter, sourceContext, this.ExtraTrackingEventData);
			}
			if (this.processedRecipients != null)
			{
				MailItemDeliver.Diag.TraceDebug<int>(0L, "Writing message tracking processed entry for {0} recipients", this.processedRecipients.Count);
				SystemProbe.TracePass<int>(this.MbxTransportMailItem, "StoreDriver", "Message processed for {0} recipients", this.processedRecipients.Count);
				MessageTrackingLog.TrackProcessed(MessageTrackingSource.STOREDRIVER, this.MbxTransportMailItem, this.processedRecipients, this.processedRecipientsSources, latencyFormatter, this.sessionSourceContext, this.ExtraTrackingEventData);
			}
		}

		private void Initialize()
		{
			if (this.initialized)
			{
				this.eventArguments.ResetPerRecipientState();
				this.currentItem.DeliverToFolder = null;
				this.deliverToFolderName = null;
				this.deliveryErrors = null;
				return;
			}
			this.eventArguments = new StoreDriverDeliveryEventArgsImpl(this);
			this.messageClass = this.MbxTransportMailItem.Message.MapiMessageClass;
			this.mailItemWrapper = new MbxMailItemWrapper(this.MbxTransportMailItem);
			this.messageClass = this.MbxTransportMailItem.Message.MapiMessageClass;
			this.initialized = true;
		}

		private void GetDeliveryFlags()
		{
			this.deliveryFlags = OpenTransportSessionFlags.OpenForNormalMessageDelivery;
			if (!MultilevelAuth.IsInternalMail(this.MbxTransportMailItem))
			{
				return;
			}
			if (ObjectClass.IsOfClass(this.messageClass, "IPM.Note.StorageQuotaWarning"))
			{
				this.deliveryFlags = OpenTransportSessionFlags.OpenForQuotaMessageDelivery;
				return;
			}
			if (ObjectClass.IsDsn(this.messageClass))
			{
				this.deliveryFlags = OpenTransportSessionFlags.OpenForSpecialMessageDelivery;
				return;
			}
			if (ObjectClass.IsOfClass(this.messageClass, "IPM.Note.Microsoft.Voicemail.UM.CA") || ObjectClass.IsOfClass(this.messageClass, "IPM.Note.rpmsg.Microsoft.Voicemail.UM.CA") || ObjectClass.IsOfClass(this.messageClass, "IPM.Note.rpmsg.Microsoft.Voicemail.UM") || ObjectClass.IsOfClass(this.messageClass, "IPM.Note.Microsoft.Partner.UM") || ObjectClass.IsOfClass(this.messageClass, "IPM.Note.Microsoft.Fax.CA"))
			{
				this.deliveryFlags = OpenTransportSessionFlags.OpenForSpecialMessageDelivery;
			}
		}

		private void GetVirusScanningStamps()
		{
			Header[] array = this.MbxTransportMailItem.RootPart.Headers.FindAll("X-MS-Exchange-Organization-AVStamp-Mailbox");
			if (array != null && array.Length > 0)
			{
				this.virusScanningStamps = new string[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					this.virusScanningStamps[i] = array[i].Value;
				}
			}
		}

		private void CreateReplayItem()
		{
			MailItemDeliver.Diag.TraceDebug(0L, "Create Content Conversion Replay Item");
			if (this.replayItem != null)
			{
				this.replayItem.Dispose();
				this.replayItem = null;
			}
			this.replayItem = MessageItem.CreateInMemory(StoreObjectSchema.ContentConversionProperties);
			this.replayItem.SetNoMessageDecoding(true);
			ItemConversion.ConvertAnyMimeToItem(this.replayItem, this.MbxTransportMailItem.Message, this.conversionOptions);
			this.replayItem.Save(SaveMode.NoConflictResolution);
		}

		private void ProcessDuplicateDelivery()
		{
			StorageExceptionHandler.RunUnderExceptionHandler(this, delegate
			{
				if (this.replayItem != null && !this.replayItem.WasDeliveredViaBcc)
				{
					MailItemDeliver.Diag.TraceDebug((long)this.GetHashCode(), "Duplicate delivery happened, and the new item was not BCCed.");
					this.RemoveBccFlagsFromDuplicates();
				}
			});
		}

		private void RemoveBccFlagsFromDuplicates()
		{
			MailboxSession mailboxSession = this.currentItem.Session as MailboxSession;
			if (mailboxSession == null)
			{
				MailItemDeliver.Diag.TraceDebug<string>((long)this.GetHashCode(), "BCC duplicate detection not supported for: {0}", this.currentItem.Session.GetType().FullName);
				return;
			}
			if (string.IsNullOrEmpty(this.replayItem.InternetMessageId))
			{
				return;
			}
			IEnumerable<IStorePropertyBag> enumerable = AllItemsFolderHelper.FindItemsFromInternetId(mailboxSession, this.replayItem.InternetMessageId, ItemQueryType.NoNotifications, new PropertyDefinition[]
			{
				ItemSchema.Id
			});
			int num = 0;
			foreach (IStorePropertyBag storePropertyBag in enumerable)
			{
				num++;
				if (num >= MailItemDeliver.MaxDuplicatesChecked)
				{
					break;
				}
				StoreObjectId objectId = ((VersionedId)storePropertyBag[ItemSchema.Id]).ObjectId;
				using (MessageItem messageItem = MessageItem.Bind(this.currentItem.Session, objectId, new PropertyDefinition[]
				{
					MessageItemSchema.MessageBccMe
				}))
				{
					if (messageItem == null)
					{
						MailItemDeliver.Diag.TraceDebug((long)this.GetHashCode(), "Duplicate item was not found, presumably the user deleted it.");
					}
					else
					{
						bool valueOrDefault = messageItem.GetValueOrDefault<bool>(MessageItemSchema.MessageBccMe);
						if (valueOrDefault)
						{
							MailItemDeliver.Diag.TraceDebug((long)this.GetHashCode(), "Existing duplicate item was BCC, new non-BCC state must take precedence.");
							messageItem.OpenAsReadWrite();
							messageItem[MessageItemSchema.MessageBccMe] = false;
							ConflictResolutionResult conflictResolutionResult = messageItem.Save(SaveMode.ResolveConflicts);
							MailItemDeliver.Diag.TraceDebug<SaveResult>((long)this.GetHashCode(), "Existing duplicate item saved: {0}", conflictResolutionResult.SaveStatus);
						}
						else
						{
							MailItemDeliver.Diag.TraceDebug((long)this.GetHashCode(), "Existing duplicate item was not BCC");
						}
					}
				}
			}
		}

		private void AddStoreLatency(TimeSpan additionalLatency)
		{
			this.storeLatency += additionalLatency;
			this.breadcrumb.StoreLatency = this.storeLatency;
			MailItemDeliver.Diag.TraceDebug<TimeSpan, string, TimeSpan>(0L, "Store time in server {0} added to store latency for message {1}, making total store latency {2}", additionalLatency, this.MbxTransportMailItem.InternetMessageId ?? "NoMessageId", this.storeLatency);
		}

		private void DecrementThrottlingForCurrentRecipient()
		{
			ulong smtpSessionId = 0UL;
			if (this.MbxTransportMailItem.ExtendedProperties.TryGetValue<ulong>("Microsoft.Exchange.Transport.SmtpInSessionId", out smtpSessionId))
			{
				DeliveryThrottling.Instance.DecrementRecipient((long)smtpSessionId, this.recipient.Email);
				return;
			}
			MailItemDeliver.Diag.TraceWarning<string>(0L, "SmtpInSessionId is not found in mailitem properties for message {0}. Throttling value will be decremented only on SMTP disconnect", this.MbxTransportMailItem.InternetMessageId ?? "NoMessageId");
		}

		private void IncrementDeliveryFailure(MessageAction messageAction)
		{
			StoreDriverDeliveryPerfCounters.Instance.IncrementDeliveryFailure(this.IsPermanentFailure(messageAction), false);
			StoreDriverDatabasePerfCounters.IncrementDeliveryFailure(this.MbxTransportMailItem.DatabaseGuid.ToString(), false);
		}

		private bool IsPermanentFailure(MessageAction messageAction)
		{
			switch (messageAction)
			{
			case MessageAction.NDR:
				return true;
			}
			return false;
		}

		private void ProcessDeliveryForRecipient()
		{
			this.AckDeliveredRecipient();
			MSExchangeStoreDriver.SuccessfulDeliveries.Increment();
			MSExchangeStoreDriver.BytesDelivered.IncrementBy(this.MbxTransportMailItem.MimeSize);
		}

		private void ProcessDuplicateDeliveryForRecipient()
		{
			this.AckDuplicateRecipient();
			MSExchangeStoreDriver.DuplicateDelivery.Increment();
			this.IncrementDeliveryFailure(MessageAction.Success);
			this.ProcessDuplicateDelivery();
		}

		private const int DeliverRecords = 128;

		private static readonly int MaxDuplicatesChecked = 5;

		private static readonly Trace Diag = ExTraceGlobals.MapiDeliverTracer;

		private static DeliveryRecipientThreadMap recipientThreadMap = new DeliveryRecipientThreadMap(MailItemDeliver.Diag);

		private static Breadcrumbs<MailItemDeliver.Breadcrumb> deliveryHistory = new Breadcrumbs<MailItemDeliver.Breadcrumb>(128);

		private static StoreDriverEventSource eventSource = new StoreDriverEventSourceImpl();

		private readonly ulong sessionId;

		private readonly string sessionSourceContext;

		private InboundConversionOptions conversionOptions;

		private object spamConfidenceLevel;

		private DateTime? originalReceivedTime;

		private MultiValuedProperty<CultureInfo> recipientLanguages;

		private string messageClass;

		private OpenTransportSessionFlags deliveryFlags;

		private IDeliveryItem currentItem;

		private List<string> deliveredRecipients;

		private List<string> deliveredRecipientsFolderNames;

		private List<string> deliveredRecipientsErrors;

		private List<string> deliveredRecipientsMailboxInfo;

		private List<string> duplicateRecipients;

		private List<string> duplicateRecipientsFolderNames;

		private List<string> processedRecipients;

		private List<string> processedRecipientsSources;

		private MessageItem replayItem;

		private MailRecipient recipient;

		private bool isJournalReport;

		private string[] virusScanningStamps;

		private bool initialized;

		private bool replayCreated;

		private MailItemDeliver.Breadcrumb breadcrumb;

		private IMExSession mexSession;

		private AgentLatencyTracker agentLatencyTracker;

		private StoreDriverDeliveryEventArgsImpl eventArguments;

		private string deliverToFolderName;

		private List<string> deliveryErrors;

		private TimeSpan? retryInterval;

		private bool wasSessionOpenedForLastRecipient;

		private int? databaseHealthMeasureToLog;

		private MailItemDeliver.DeliveryStage deliveryStage;

		private TimeSpan storeLatency;

		private TimeSpan rpcLatency;

		private MbxTransportMailItem mbxTransportMailItem;

		private MbxMailItemWrapper mailItemWrapper;

		internal enum DeliveryStage
		{
			NotStarted,
			Start,
			OnInitializedEvent,
			CreateReplay,
			CreateSession,
			OnPromotedEvent,
			CreateMessage,
			PromoteProperties,
			OnCreatedEvent,
			PostCreate,
			Delivery,
			OnDeliveredEvent,
			Done
		}

		internal class Breadcrumb
		{
			internal Breadcrumb()
			{
				this.RecordCreation = ExDateTime.UtcNow;
				this.DeliveredToAll = TimeSpan.MinValue;
				this.MailboxServer = "?";
			}

			internal ExDateTime RecordCreation { get; set; }

			internal string Sender { get; set; }

			internal string MessageId { get; set; }

			internal int RecipientCount { get; set; }

			internal TimeSpan DeliveredToAll { get; set; }

			internal string MailboxDatabase { get; set; }

			internal string MailboxServer { get; set; }

			internal LatencyTracker LatencyTracker { get; set; }

			internal TimeSpan RpcLatency { get; set; }

			internal TimeSpan StoreLatency { get; set; }

			internal TimeSpan LdapLatency { get; set; }

			internal TimeSpan ContentConversionLatency { get; set; }

			public override string ToString()
			{
				return string.Format("Created: {0}, MbxDatabase: {1}, MbxServer: {2}, Sender: {3}, MessageId: {4}, RecipientCount: {5}, DeliveryStage: {6}, Completed: {7}, Elapsed: {8}, RpcLatency: {9}, StoreLatency: {10}, LdapLatency: {11}, ContentConversionLatency: {12}", new object[]
				{
					this.RecordCreation,
					this.MailboxDatabase,
					this.MailboxServer,
					this.Sender,
					this.MessageId,
					this.RecipientCount,
					this.deliveryStage,
					(this.DeliveredToAll == TimeSpan.MinValue) ? "No" : "Yes",
					(this.DeliveredToAll == TimeSpan.MinValue) ? ExDateTime.UtcNow.Subtract(this.RecordCreation) : this.DeliveredToAll,
					this.RpcLatency,
					this.StoreLatency,
					this.LdapLatency,
					this.ContentConversionLatency
				});
			}

			public XElement GetDiagnosticInfo()
			{
				XElement xelement = new XElement("Delivery", new object[]
				{
					new XElement("creationTimestamp", this.RecordCreation),
					new XElement("mailboxDatabase", this.MailboxDatabase),
					new XElement("mailboxServer", this.MailboxServer),
					new XElement("sender", this.Sender),
					new XElement("recipientCount", this.RecipientCount),
					new XElement("deliveryStage", this.deliveryStage),
					new XElement("completed", this.DeliveredToAll != TimeSpan.MinValue),
					new XElement("elapsed", (this.DeliveredToAll == TimeSpan.MinValue) ? ExDateTime.UtcNow.Subtract(this.RecordCreation) : this.DeliveredToAll),
					new XElement("rpcLatency", this.RpcLatency),
					new XElement("storeLatency", this.StoreLatency),
					new XElement("ldapLatency", this.LdapLatency),
					new XElement("contentConversionLatency", this.ContentConversionLatency)
				});
				xelement.Add(LatencyFormatter.GetDiagnosticInfo(this.LatencyTracker));
				xelement.SetAttributeValue("messageId", this.MessageId);
				return xelement;
			}

			internal MailItemDeliver.DeliveryStage SetStage(MailItemDeliver.DeliveryStage deliveryStage)
			{
				this.deliveryStage = deliveryStage;
				return deliveryStage;
			}

			private MailItemDeliver.DeliveryStage deliveryStage;
		}
	}
}
