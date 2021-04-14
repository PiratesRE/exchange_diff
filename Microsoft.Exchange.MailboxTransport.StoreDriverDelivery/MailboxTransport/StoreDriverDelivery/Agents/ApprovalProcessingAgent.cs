using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Approval;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.StoreDriver;
using Microsoft.Exchange.Data.Transport.StoreDriverDelivery;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxTransport.StoreDriver.Shared;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Configuration;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Win32;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class ApprovalProcessingAgent : StoreDriverDeliveryAgent
	{
		public ApprovalProcessingAgent(SmtpServer server, MSExchangeTransportApprovalInstance perfCounter)
		{
			this.server = (server as StoreDriverServer);
			if (this.server == null)
			{
				throw new ArgumentException("The instance of the SmtpServer is not of the expected type.", "server");
			}
			this.perfCounter = perfCounter;
			base.OnCreatedMessage += this.OnCreatedMessageHandler;
			base.OnDeliveredMessage += this.OnDeliveredMessageHandler;
		}

		public void OnCreatedMessageHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs args)
		{
			StoreDriverDeliveryEventArgsImpl storeDriverDeliveryEventArgsImpl = (StoreDriverDeliveryEventArgsImpl)args;
			MailRecipient mailRecipient = storeDriverDeliveryEventArgsImpl.MailRecipient;
			DeliverableMailItem mailItem = storeDriverDeliveryEventArgsImpl.MailItem;
			MbxTransportMailItem mbxTransportMailItem = storeDriverDeliveryEventArgsImpl.MailItemDeliver.MbxTransportMailItem;
			MessageItem messageItem = storeDriverDeliveryEventArgsImpl.MessageItem;
			bool flag = false;
			bool flag2 = ApprovalInitiation.IsArbitrationMailbox(mbxTransportMailItem.ADRecipientCache, mailRecipient.Email);
			if (!flag2 && string.Equals(messageItem.ClassName, "IPM.Note.Microsoft.Approval.Request.Recall", StringComparison.OrdinalIgnoreCase))
			{
				flag = true;
			}
			EmailMessage message = mailItem.Message;
			TestMessageConfig testMessageConfig = new TestMessageConfig(message);
			if (testMessageConfig.IsTestMessage && (testMessageConfig.LogTypes & LogTypesEnum.Arbitration) != LogTypesEnum.None)
			{
				EmailMessage emailMessage = ArbitrationMailboxReport.GenerateContentReport(new SmtpAddress(mailRecipient.Email.ToString()), testMessageConfig.ReportToAddress, messageItem.Session, flag2);
				if (emailMessage != null)
				{
					ApprovalProcessingAgent.diag.TraceDebug(0L, "Submit arbitration mailbox content report message");
					this.server.SubmitMessage(mbxTransportMailItem, emailMessage, mbxTransportMailItem.OrganizationId, mbxTransportMailItem.ExternalOrganizationId, false);
				}
				else
				{
					ApprovalProcessingAgent.diag.TraceDebug(0L, "Failed to generate arbitration mailbox content report");
				}
				throw new SmtpResponseException(AckReason.ApprovalUpdateSuccess, base.Name);
			}
			if (!flag)
			{
				if (flag2)
				{
					ApprovalEngine approvalEngineInstance = ApprovalEngine.GetApprovalEngineInstance(message, (RoutingAddress)message.From.SmtpAddress, mailRecipient.Email, messageItem, mbxTransportMailItem, ApprovalProcessingAgent.MessageItemCreationDelegate);
					ApprovalEngine.ApprovalProcessResults resultInfo = approvalEngineInstance.ProcessMessage();
					this.HandleResults(resultInfo, messageItem, mbxTransportMailItem, mailRecipient);
				}
				return;
			}
			if (!MultilevelAuth.IsInternalMail(message))
			{
				return;
			}
			if (ApprovalRequestUpdater.Result.InvalidUpdateMessage == ApprovalRequestUpdater.TryUpdateExistingApprovalRequest(messageItem))
			{
				throw new SmtpResponseException(AckReason.ApprovalInvalidMessage);
			}
			throw new SmtpResponseException(AckReason.ApprovalUpdateSuccess, base.Name);
		}

		public void OnDeliveredMessageHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs args)
		{
			StoreDriverDeliveryEventArgsImpl storeDriverDeliveryEventArgsImpl = (StoreDriverDeliveryEventArgsImpl)args;
			MailRecipient mailRecipient = storeDriverDeliveryEventArgsImpl.MailRecipient;
			MbxTransportMailItem mbxTransportMailItem = storeDriverDeliveryEventArgsImpl.MailItemDeliver.MbxTransportMailItem;
			if (!ApprovalInitiation.IsArbitrationMailbox(mbxTransportMailItem.ADRecipientCache, mailRecipient.Email))
			{
				return;
			}
			DeliverableMailItem mailItem = storeDriverDeliveryEventArgsImpl.MailItem;
			MessageItem messageItem = storeDriverDeliveryEventArgsImpl.MessageItem;
			EmailMessage message = mailItem.Message;
			int? messageLocaleId = (storeDriverDeliveryEventArgsImpl.ReplayItem != null) ? storeDriverDeliveryEventArgsImpl.ReplayItem.GetValueAsNullable<int>(MessageItemSchema.MessageLocaleId) : null;
			this.CheckArbitrationMailboxCapacity(messageItem.Session as MailboxSession, mbxTransportMailItem.OrganizationId);
			ApprovalEngine approvalEngineInstance = ApprovalEngine.GetApprovalEngineInstance(message, (RoutingAddress)message.From.SmtpAddress, mailRecipient.Email, messageItem, mbxTransportMailItem, ApprovalProcessingAgent.MessageItemCreationDelegate);
			if (approvalEngineInstance.CreateAndSubmitApprovalRequests(messageLocaleId) != ApprovalEngine.ProcessResult.InitiationMessageOk)
			{
				throw new SmtpResponseException(AckReason.ApprovalInvalidMessage);
			}
		}

		internal static int ReadTestRegistryValue(string keyName, int defaultValue)
		{
			try
			{
				object value = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Exchange_Test\\v15\\BCM", keyName, defaultValue);
				if (value is int)
				{
					return (int)value;
				}
			}
			catch (SecurityException)
			{
			}
			catch (IOException)
			{
			}
			return defaultValue;
		}

		private void CheckArbitrationMailboxCapacity(MailboxSession session, OrganizationId orgId)
		{
			if (!VariantConfiguration.InvariantNoFlightingSnapshot.MailboxTransport.CheckArbitrationMailboxCapacity.Enabled || session == null)
			{
				return;
			}
			object obj = session.Mailbox.TryGetProperty(MailboxSchema.QuotaProhibitReceive);
			object obj2 = session.Mailbox.TryGetProperty(MailboxSchema.QuotaUsedExtended);
			if (obj is PropertyError || obj2 is PropertyError)
			{
				ApprovalProcessingAgent.diag.TraceDebug<Mailbox, object, object>(0L, "Property error getting quota/usage of {0}. quota={1}, used={2}", session.Mailbox, obj, obj2);
				return;
			}
			int num = (int)obj;
			if (num < 0)
			{
				return;
			}
			ulong num2 = (ulong)((long)num * 1024L);
			ulong num3 = (ulong)((long)obj2);
			if (0.7 * num2 < num3)
			{
				bool flag = false;
				lock (ApprovalProcessingAgent.organizationsAlertedForQuotaSyncLock)
				{
					if (ApprovalProcessingAgent.organizationsAlertedForQuota == null || ApprovalProcessingAgent.organizationsAlertedForQuotLastRefreshed.Add(ApprovalProcessingAgent.organizationsAlertedForQuotaRefreshPeriod) < ExDateTime.UtcNow || ApprovalProcessingAgent.organizationsAlertedForQuota.Count > 500)
					{
						ApprovalProcessingAgent.organizationsAlertedForQuota = new HashSet<OrganizationId>();
						ApprovalProcessingAgent.organizationsAlertedForQuotLastRefreshed = ExDateTime.UtcNow;
					}
					flag = ApprovalProcessingAgent.organizationsAlertedForQuota.Add(orgId);
				}
				if (flag)
				{
					StoreDriverDeliveryDiagnostics.LogEvent(orgId, MailboxTransportEventLogConstants.Tuple_ApprovalArbitrationMailboxQuota, null, new object[]
					{
						session.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(),
						orgId,
						num3,
						num2
					});
					EventNotificationItem.Publish(ExchangeComponent.MailboxTransport.Name, "ApprovalArbitrationMailboxQuota", null, string.Format("The arbitration mailbox is approaching its quota. Org ID: {0}, Primary Smtp Address: {1}", orgId, session.MailboxOwner.MailboxInfo.PrimarySmtpAddress), ResultSeverityLevel.Warning, false);
				}
			}
		}

		private void HandleResults(ApprovalEngine.ApprovalProcessResults resultInfo, MessageItem messageItem, MbxTransportMailItem rmi, MailRecipient recipient)
		{
			if (resultInfo.ProcessResults == ApprovalEngine.ProcessResult.InitiationMessageOk)
			{
				messageItem.VotingInfo.MessageCorrelationBlob = resultInfo.ApprovalTrackingBlob;
				messageItem[MessageItemSchema.ApprovalRequestMessageId] = resultInfo.ApprovalRequestMessageId;
				messageItem[MessageItemSchema.ApprovalDecisionMakersNdred] = NdrOofHandler.FormatNdrOofProperty(resultInfo.TotalDecisionMakers, 0, 0);
				messageItem[MessageItemSchema.ApprovalStatus] = ApprovalStatus.Unhandled;
				this.perfCounter.InitiationMessages.Increment();
				return;
			}
			if (resultInfo.ProcessResults == ApprovalEngine.ProcessResult.UnauthorizedMessage)
			{
				throw new SmtpResponseException(AckReason.ApprovalUnAuthorizedMessage);
			}
			if (resultInfo.ProcessResults == ApprovalEngine.ProcessResult.InitiationMessageDuplicate)
			{
				recipient.DsnRequested = DsnRequestedFlags.Never;
				throw new SmtpResponseException(AckReason.ApprovalDuplicateInitiation, base.Name);
			}
			if (resultInfo.ProcessResults == ApprovalEngine.ProcessResult.DecisionMarked)
			{
				this.perfCounter.DecisionUsed.Increment();
				this.perfCounter.DecisionMessages.Increment();
				this.perfCounter.DecisionInitiationMessageSearchTimeMilliseconds.IncrementBy(resultInfo.InitiationMessageSearchTimeMilliseconds);
				recipient.DsnRequested = DsnRequestedFlags.Never;
				throw new SmtpResponseException(AckReason.ApprovalDecisionSuccsess, base.Name);
			}
			if (resultInfo.ProcessResults == ApprovalEngine.ProcessResult.InitiationNotFoundForDecision || resultInfo.ProcessResults == ApprovalEngine.ProcessResult.DecisionAlreadyMade)
			{
				byte[] conversationIndex = messageItem.ConversationIndex;
				string threadIndex = Convert.ToBase64String(ConversationIndex.CreateFromParent(conversationIndex).ToByteArray());
				this.SendNotifications(recipient.Email, (RoutingAddress)rmi.Message.Sender.SmtpAddress, rmi, threadIndex, messageItem.ConversationTopic, resultInfo.ExistingDecisionMakerAddress, resultInfo.ExistingApprovalStatus, resultInfo.ExistingDecisionTime);
				this.perfCounter.DecisionMessages.Increment();
				this.perfCounter.DecisionInitiationMessageSearchTimeMilliseconds.IncrementBy(resultInfo.InitiationMessageSearchTimeMilliseconds);
				recipient.DsnRequested = DsnRequestedFlags.Never;
				throw new SmtpResponseException(AckReason.ApprovalDecisionSuccsess, base.Name);
			}
			if (resultInfo.ProcessResults == ApprovalEngine.ProcessResult.NdrOrOofUpdated || resultInfo.ProcessResults == ApprovalEngine.ProcessResult.NdrOrOofUpdateSkipped || resultInfo.ProcessResults == ApprovalEngine.ProcessResult.InitiationNotFoundForNdrOrOof || resultInfo.ProcessResults == ApprovalEngine.ProcessResult.NdrOrOofInvalid)
			{
				this.perfCounter.TotalNdrOofHandled.Increment();
				if (resultInfo.ProcessResults != ApprovalEngine.ProcessResult.NdrOrOofInvalid)
				{
					this.perfCounter.TotalSearchesForInitiationBasedOnNdrAndOof.Increment();
					this.perfCounter.NdrOofInitiationMessageSearchTimeMilliseconds.IncrementBy(resultInfo.InitiationMessageSearchTimeMilliseconds);
				}
				if (resultInfo.ProcessResults == ApprovalEngine.ProcessResult.NdrOrOofUpdated)
				{
					this.perfCounter.TotalNdrOofUpdated.Increment();
				}
				recipient.DsnRequested = DsnRequestedFlags.Never;
				throw new SmtpResponseException(AckReason.ApprovalNdrOofUpdateSuccess, base.Name);
			}
			throw new SmtpResponseException(AckReason.ApprovalInvalidMessage);
		}

		private void SendNotifications(RoutingAddress from, RoutingAddress recipient, MbxTransportMailItem rmi, string threadIndex, string threadTopic, string existingDecisionMakerAddress, ApprovalStatus? existingApprovalStatus, ExDateTime? existingDecisionTime)
		{
			HeaderList headers = rmi.RootPart.Headers;
			Header acceptLanguageHeader = headers.FindFirst("Accept-Language");
			Header contentLanguageHeader = headers.FindFirst(HeaderId.ContentLanguage);
			string decisionMakerDisplayName = existingDecisionMakerAddress;
			bool? flag = null;
			if (existingApprovalStatus != null)
			{
				if ((existingApprovalStatus.Value & ApprovalStatus.Approved) == ApprovalStatus.Approved)
				{
					flag = new bool?(true);
				}
				else if ((existingApprovalStatus.Value & ApprovalStatus.Rejected) == ApprovalStatus.Rejected)
				{
					flag = new bool?(false);
				}
			}
			if (!string.IsNullOrEmpty(existingDecisionMakerAddress))
			{
				ADNotificationAdapter.TryRunADOperation(delegate()
				{
					IRecipientSession recipientSession = ApprovalProcessor.CreateRecipientSessionFromSmtpAddress(existingDecisionMakerAddress);
					ADRawEntry adrawEntry = recipientSession.FindByProxyAddress(new SmtpProxyAddress(existingDecisionMakerAddress, true), ApprovalProcessingAgent.DisplayNameProperty);
					if (adrawEntry != null)
					{
						string text = (string)adrawEntry[ADRecipientSchema.DisplayName];
						if (!string.IsNullOrEmpty(text))
						{
							decisionMakerDisplayName = text;
						}
					}
				}, 1);
			}
			ApprovalProcessingAgent.diag.TraceDebug<bool?, string>(0L, "Generating conflict notification. Decision='{0}', DecisionMaker='{1}'", flag, decisionMakerDisplayName);
			EmailMessage emailMessage = NotificationGenerator.GenerateDecisionNotTakenNotification(from, recipient, rmi.Subject, threadIndex, threadTopic, decisionMakerDisplayName, flag, existingDecisionTime, acceptLanguageHeader, contentLanguageHeader, rmi.TransportSettings.InternalDsnDefaultLanguage);
			if (emailMessage != null)
			{
				this.server.SubmitMessage(rmi, emailMessage, rmi.OrganizationId, rmi.ExternalOrganizationId, false);
			}
		}

		private const double ArbitrationMailboxPercentageFullBeforeAlert = 0.7;

		private const int OrganizationsAlertedForQuotaMaxSize = 500;

		private static readonly Trace diag = ExTraceGlobals.ApprovalAgentTracer;

		private static readonly ApprovalEngine.ApprovalRequestCreateDelegate MessageItemCreationDelegate = new ApprovalEngine.ApprovalRequestCreateDelegate(MessageItemApprovalRequest.Create);

		private static readonly ADPropertyDefinition[] DisplayNameProperty = new ADPropertyDefinition[]
		{
			ADRecipientSchema.DisplayName
		};

		private static readonly TimeSpan organizationsAlertedForQuotaRefreshPeriod = TimeSpan.FromMinutes((double)ApprovalProcessingAgent.ReadTestRegistryValue("ArbitrationMailboxQuotaAlertRefresh", 1440));

		private static HashSet<OrganizationId> organizationsAlertedForQuota;

		private static ExDateTime organizationsAlertedForQuotLastRefreshed;

		private static object organizationsAlertedForQuotaSyncLock = new object();

		private StoreDriverServer server;

		private MSExchangeTransportApprovalInstance perfCounter;
	}
}
