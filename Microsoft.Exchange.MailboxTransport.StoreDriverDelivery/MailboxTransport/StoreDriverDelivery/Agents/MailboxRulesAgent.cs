using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.MailboxRules;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Data.Transport.StoreDriver;
using Microsoft.Exchange.Data.Transport.StoreDriverDelivery;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;
using Microsoft.Exchange.MailboxTransport.StoreDriver.Shared;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;
using Microsoft.Exchange.Transport.Configuration;
using Microsoft.Exchange.Transport.MailboxRules;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal sealed class MailboxRulesAgent : StoreDriverDeliveryAgent, IDisposeTrackable, IDisposable
	{
		public MailboxRulesAgent(SmtpServer server)
		{
			this.server = (server as StoreDriverServer);
			if (this.server == null)
			{
				throw new ArgumentException("The instance of the SmtpServer is not of the expected type.", "server");
			}
			base.OnPromotedMessage += this.OnPromotedMessageHandler;
			base.OnCreatedMessage += this.OnCreatedMessageHandler;
			base.OnDeliveredMessage += this.OnDeliveredMessageHandler;
			base.OnCompletedMessage += this.OnCompletedMessageHandler;
			this.disposeTracker = ((IDisposeTrackable)this).GetDisposeTracker();
			this.stopwatch = new Stopwatch();
		}

		public void OnPromotedMessageHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs args)
		{
			using (new MailboxRulesPerformanceTracker(this.stopwatch))
			{
				StoreDriverDeliveryEventArgsImpl storeDriverDeliveryEventArgsImpl = (StoreDriverDeliveryEventArgsImpl)args;
				if (storeDriverDeliveryEventArgsImpl.IsPublicFolderRecipient)
				{
					MailboxRulesAgent.tracer.TraceDebug(0L, "Will not process rules in transport for public folder.");
				}
				else
				{
					this.testMessageConfig = this.GetTestMessageConfig(storeDriverDeliveryEventArgsImpl);
					this.evaluationContext = this.CreateEvaluationContext(storeDriverDeliveryEventArgsImpl);
					this.evaluationContext.PropertiesForAllMessageCopies = storeDriverDeliveryEventArgsImpl.PropertiesForAllMessageCopies;
					this.evaluationContext.PropertiesForDelegateForward = storeDriverDeliveryEventArgsImpl.PropertiesForDelegateForward;
					this.evaluationContext.SharedPropertiesBetweenAgents = storeDriverDeliveryEventArgsImpl.SharedPropertiesBetweenAgents;
					this.evaluationContext.ShouldSkipMoveRule = storeDriverDeliveryEventArgsImpl.ShouldSkipMoveRule;
					this.evaluationContext.SenderAddress = this.GetSenderSmtpAddress(storeDriverDeliveryEventArgsImpl);
					if (!this.ShouldProcessRuleInTransport(storeDriverDeliveryEventArgsImpl))
					{
						this.evaluationContext.TraceDebug("Skipping inbox rules processing.");
						this.HandleTestMessage(storeDriverDeliveryEventArgsImpl);
					}
					else
					{
						this.SetCalculatedProperties(storeDriverDeliveryEventArgsImpl.ReplayItem);
						RuleEvaluator ruleEvaluator = new RuleEvaluator(this.evaluationContext);
						this.evaluationResult = ruleEvaluator.Evaluate();
						storeDriverDeliveryEventArgsImpl.SharedPropertiesBetweenAgents = this.evaluationContext.SharedPropertiesBetweenAgents;
						if (this.testMessageConfig.SuppressDelivery)
						{
							this.HandleTestMessage(storeDriverDeliveryEventArgsImpl);
						}
						this.evaluationResult.Execute(ExecutionStage.OnPromotedMessage);
						if (this.evaluationResult.TargetFolder != null)
						{
							this.evaluationContext.TraceDebug(string.Format("Target folder name: {0}, ID: {1}.", this.evaluationResult.TargetFolder.DisplayName, this.evaluationResult.TargetFolder.Id));
							MailboxSession mailboxSession = this.evaluationContext.StoreSession as MailboxSession;
							if (mailboxSession != null)
							{
								StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
								if (!this.evaluationResult.TargetFolder.Id.ObjectId.Equals(defaultFolderId))
								{
									this.evaluationContext.TraceDebug("Setting delivery folder.");
									storeDriverDeliveryEventArgsImpl.SetDeliveryFolder(this.evaluationResult.TargetFolder);
								}
								else
								{
									this.evaluationContext.TraceDebug("DeliverToFolder is same as inbox folder.");
								}
							}
							if (storeDriverDeliveryEventArgsImpl.DeliverToFolder == null || storeDriverDeliveryEventArgsImpl.DeliverToFolder.Equals(this.evaluationResult.TargetFolder.Id))
							{
								storeDriverDeliveryEventArgsImpl.RetentionPolicyTag = this.evaluationResult.TargetFolder.GetValueOrDefault<object>(StoreObjectSchema.PolicyTag);
								storeDriverDeliveryEventArgsImpl.RetentionPeriod = this.evaluationResult.TargetFolder.GetValueOrDefault<object>(StoreObjectSchema.RetentionPeriod);
								storeDriverDeliveryEventArgsImpl.RetentionFlags = this.evaluationResult.TargetFolder.GetValueOrDefault<object>(StoreObjectSchema.RetentionFlags);
								storeDriverDeliveryEventArgsImpl.ArchiveTag = this.evaluationResult.TargetFolder.GetValueOrDefault<object>(StoreObjectSchema.ArchiveTag);
								storeDriverDeliveryEventArgsImpl.ArchivePeriod = this.evaluationResult.TargetFolder.GetValueOrDefault<object>(StoreObjectSchema.ArchivePeriod);
								storeDriverDeliveryEventArgsImpl.CompactDefaultRetentionPolicy = this.evaluationResult.TargetFolder.GetValueOrDefault<object>(FolderSchema.RetentionTagEntryId);
							}
						}
						else
						{
							this.HandleTestMessage(storeDriverDeliveryEventArgsImpl);
							SmtpResponse smtpResponseForBounceCode = this.GetSmtpResponseForBounceCode(this.evaluationResult.BounceCode, storeDriverDeliveryEventArgsImpl);
							if (!this.evaluationResult.FolderResults.Any((FolderEvaluationResult folderResult) => folderResult.WorkItems.Any((WorkItem workItem) => workItem is DelegateWorkItem)) || !ObjectClass.IsMeetingMessage(storeDriverDeliveryEventArgsImpl.ReplayItem.ClassName) || !MeetingMessage.IsDelegateOnlyFromSession(storeDriverDeliveryEventArgsImpl.MailboxSession))
							{
								throw new SmtpResponseException(smtpResponseForBounceCode, base.Name);
							}
							storeDriverDeliveryEventArgsImpl.ShouldCreateItemForDelete = true;
							storeDriverDeliveryEventArgsImpl.BounceSmtpResponse = smtpResponseForBounceCode;
							storeDriverDeliveryEventArgsImpl.BounceSource = base.Name + ".DelegateAccess";
						}
					}
				}
			}
		}

		public void OnCreatedMessageHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs args)
		{
			if (this.evaluationResult != null)
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest(40592U);
				using (new MailboxRulesPerformanceTracker(this.stopwatch))
				{
					StoreDriverDeliveryEventArgsImpl storeDriverDeliveryEventArgsImpl = (StoreDriverDeliveryEventArgsImpl)args;
					this.evaluationResult.Context.FinalDeliveryFolderId = storeDriverDeliveryEventArgsImpl.DeliverToFolder;
					this.evaluationResult.Context.DeliveredMessage = storeDriverDeliveryEventArgsImpl.MessageItem;
					this.evaluationResult.Execute(ExecutionStage.OnCreatedMessage);
				}
			}
		}

		public void OnDeliveredMessageHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs args)
		{
			if (this.evaluationResult != null)
			{
				using (new MailboxRulesPerformanceTracker(this.stopwatch))
				{
					StoreDriverDeliveryEventArgsImpl storeDriverDeliveryEventArgsImpl = (StoreDriverDeliveryEventArgsImpl)args;
					this.evaluationResult.Context.DeliveredMessage = storeDriverDeliveryEventArgsImpl.MessageItem;
					this.evaluationResult.Context.PropertiesForAllMessageCopies = storeDriverDeliveryEventArgsImpl.PropertiesForAllMessageCopies;
					this.evaluationResult.Context.PropertiesForDelegateForward = storeDriverDeliveryEventArgsImpl.PropertiesForDelegateForward;
					this.evaluationResult.Execute(ExecutionStage.OnDeliveredMessage);
					this.evaluationContext.UpdateDeferredError();
					this.HandleTestMessage(storeDriverDeliveryEventArgsImpl);
					List<string> errorRecords = ((RuleEvaluationContext)this.evaluationResult.Context).ErrorRecords;
					storeDriverDeliveryEventArgsImpl.AddDeliveryErrors(errorRecords);
				}
			}
		}

		public void OnCompletedMessageHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs args)
		{
			using (new MailboxRulesPerformanceTracker(this.stopwatch))
			{
				this.DisposeAgentState();
			}
			long num = Math.Max(0L, this.stopwatch.ElapsedMilliseconds);
			MailboxRulesAgent.processingTime.AddValue(num);
			MSExchangeStoreDriver.MailboxRulesMilliseconds90thPercentile.RawValue = MailboxRulesAgent.processingTime.PercentileQuery(90.0);
			MailboxRulesAgent.averageMilliseconds.Update((float)num);
			MSExchangeStoreDriver.MailboxRulesMilliseconds.RawValue = (long)MailboxRulesAgent.averageMilliseconds.Value;
		}

		public void Dispose()
		{
			if (this.disposed)
			{
				return;
			}
			this.DisposeAgentState();
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
				this.disposeTracker = null;
			}
			this.disposed = true;
			GC.SuppressFinalize(this);
		}

		DisposeTracker IDisposeTrackable.GetDisposeTracker()
		{
			return DisposeTracker.Get<MailboxRulesAgent>(this);
		}

		void IDisposeTrackable.SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		private static bool IsJournalNdrWithSkipRulesStamped(DeliverableMailItem mailItem)
		{
			return null != mailItem.Message.MimeDocument.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-JournalNdr-Skip-TransportMailboxRules");
		}

		private static bool IsSigned(string messageClass)
		{
			return !string.IsNullOrEmpty(messageClass) && (messageClass.Equals("IPM.Note.Secure.Sign", StringComparison.InvariantCultureIgnoreCase) || messageClass.Equals("IPM.Note.SMIME.MultipartSigned", StringComparison.InvariantCultureIgnoreCase) || messageClass.EndsWith("SMIME.Signed", StringComparison.InvariantCultureIgnoreCase) || messageClass.EndsWith("SMIME.MultipartSigned", StringComparison.InvariantCultureIgnoreCase) || messageClass.EndsWith("SMIME.SignedEncrypted", StringComparison.InvariantCultureIgnoreCase));
		}

		private static bool IsReadReceipt(string messageClass)
		{
			return !string.IsNullOrEmpty(messageClass) && messageClass.StartsWith("REPORT", StringComparison.InvariantCultureIgnoreCase) && messageClass.EndsWith("IPNRN", StringComparison.InvariantCultureIgnoreCase);
		}

		private bool ShouldProcessRuleInTransport(StoreDriverDeliveryEventArgsImpl deliveryEventArguments)
		{
			MailboxSession mailboxSession = deliveryEventArguments.MailboxSession;
			IExchangePrincipal mailboxOwner = mailboxSession.MailboxOwner;
			int serverVersion = mailboxOwner.MailboxInfo.Location.ServerVersion;
			if (serverVersion < Server.E14MinVersion)
			{
				this.evaluationContext.TraceDebug<int>("Will not process rules in transport for old server with version number {0}.", serverVersion);
				return false;
			}
			if (string.IsNullOrEmpty(mailboxOwner.MailboxInfo.Location.ServerFqdn))
			{
				this.evaluationContext.TraceError("Server Fqdn is empty.");
				return false;
			}
			DeliverableMailItem mailItem = deliveryEventArguments.MailItem;
			Header header = mailItem.Message.MimeDocument.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-Unjournal-Processed");
			Header header2 = mailItem.Message.MimeDocument.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-Unjournal-ProcessedNdr");
			if (header != null || header2 != null)
			{
				this.evaluationContext.TraceError("This is an unwrapped message from legacy archive journals {0},{1}.");
				return false;
			}
			if (MailboxRulesAgent.IsJournalNdrWithSkipRulesStamped(mailItem))
			{
				this.evaluationContext.TraceError("Message is destined to journal ndr mailbox. We dont journal messages to journal ndr mailbox as there is a potential of a loop PS 685340, skipping.");
				return false;
			}
			return true;
		}

		private void DisposeAgentState()
		{
			if (this.evaluationContext != null)
			{
				this.evaluationContext.Dispose();
				this.evaluationContext = null;
			}
			if (this.evaluationResult != null)
			{
				this.evaluationResult.Dispose();
				this.evaluationResult = null;
			}
		}

		private SmtpResponse GetSmtpResponseForBounceCode(RuleAction.Bounce.BounceCode? bounceCode, StoreDriverDeliveryEventArgsImpl arguments)
		{
			if (bounceCode == null)
			{
				this.ClearRecipientDsnRequestedFlag(arguments);
				return MailboxRulesAgent.MessageDeletedByRule;
			}
			RuleAction.Bounce.BounceCode value = bounceCode.Value;
			if (value == RuleAction.Bounce.BounceCode.TooLarge)
			{
				return MailboxRulesAgent.MessageTooBig;
			}
			if (value == RuleAction.Bounce.BounceCode.FormsMismatch)
			{
				return MailboxRulesAgent.FormsMismatch;
			}
			if (value != RuleAction.Bounce.BounceCode.AccessDenied)
			{
				return MailboxRulesAgent.NotAuthorized;
			}
			return MailboxRulesAgent.AccessDenied;
		}

		private void ClearRecipientDsnRequestedFlag(StoreDriverDeliveryEventArgsImpl deliveryEventArguments)
		{
			deliveryEventArguments.MailRecipient.DsnRequested = DsnRequestedFlags.Never;
		}

		private void HandleTestMessage(StoreDriverDeliveryEventArgsImpl deliveryEventArguments)
		{
			if (!this.testMessageConfig.IsTestMessage)
			{
				return;
			}
			RuleEvaluationContext ruleEvaluationContext;
			if (this.evaluationResult != null)
			{
				ruleEvaluationContext = (this.evaluationResult.Context as RuleEvaluationContext);
			}
			else
			{
				ruleEvaluationContext = this.evaluationContext;
			}
			EmailMessage emailMessage = ruleEvaluationContext.GenerateTraceReport(this.testMessageConfig.ReportToAddress);
			if (emailMessage != null)
			{
				MailboxRulesAgent.tracer.TraceDebug(0L, "Submit trace report message");
				this.server.SubmitMessage(deliveryEventArguments.MailItemDeliver.MbxTransportMailItem, emailMessage, deliveryEventArguments.MailItemDeliver.MbxTransportMailItem.OrganizationId, deliveryEventArguments.MailItemDeliver.MbxTransportMailItem.ExternalOrganizationId, false);
			}
			if (this.testMessageConfig.SuppressDelivery)
			{
				MailboxRulesAgent.tracer.TraceDebug(0L, "Delete test message");
				this.ClearRecipientDsnRequestedFlag(deliveryEventArguments);
				throw new SmtpResponseException(MailboxRulesAgent.MessageDeletedByRule, base.Name);
			}
		}

		private RuleEvaluationContext CreateEvaluationContext(StoreDriverDeliveryEventArgsImpl deliveryEventArguments)
		{
			MailboxSession mailboxSession = deliveryEventArguments.MailboxSession;
			string className = deliveryEventArguments.ReplayItem.ClassName;
			StoreObjectId storeObjectId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
			bool flag = deliveryEventArguments.DeliverToFolder != null;
			if (!string.IsNullOrEmpty(className))
			{
				StoreObjectId receiveFolder = mailboxSession.GetReceiveFolder(className);
				if (receiveFolder != null && !receiveFolder.Equals(storeObjectId))
				{
					deliveryEventArguments.ShouldRunMailboxRulesBasedOnDeliveryFolder = true;
					deliveryEventArguments.DeliverToFolder = receiveFolder;
				}
			}
			bool flag2 = false;
			if (deliveryEventArguments.ShouldRunMailboxRulesBasedOnDeliveryFolder && deliveryEventArguments.DeliverToFolder != null && !deliveryEventArguments.DeliverToFolder.Equals(storeObjectId))
			{
				flag2 = true;
				storeObjectId = StoreId.GetStoreObjectId(deliveryEventArguments.DeliverToFolder);
			}
			bool processingTestMessage = this.testMessageConfig.ReportToAddress.IsValidAddress && (this.testMessageConfig.LogTypes & LogTypesEnum.InboxRules) != LogTypesEnum.None;
			long mimeStreamLength = deliveryEventArguments.MailItem.MimeStreamLength;
			Folder folder = null;
			RuleEvaluationContext ruleEvaluationContext = null;
			try
			{
				folder = Folder.Bind(mailboxSession, storeObjectId, RuleEvaluationContextBase.AdditionalFolderProperties);
				ruleEvaluationContext = RuleEvaluationContext.Create(this.server, folder, deliveryEventArguments.ReplayItem, mailboxSession, (string)deliveryEventArguments.MailRecipient.Email, deliveryEventArguments.ADRecipientCache, mimeStreamLength, processingTestMessage, this.testMessageConfig.ShouldExecuteDisabledAndInErrorRules, deliveryEventArguments.MailItemDeliver);
				ruleEvaluationContext.TraceDebug(string.Format("Initial folder name: {0}, ID: {1}, requested by previous agent: {2}, overridden by receive-folder table: {3}", new object[]
				{
					folder.DisplayName,
					folder.Id,
					flag,
					flag2
				}));
			}
			finally
			{
				if (ruleEvaluationContext == null && folder != null)
				{
					folder.Dispose();
					folder = null;
				}
			}
			return ruleEvaluationContext;
		}

		private TestMessageConfig GetTestMessageConfig(StoreDriverDeliveryEventArgsImpl deliveryEventArguments)
		{
			MbxTransportMailItem mbxTransportMailItem = deliveryEventArguments.MailItemDeliver.MbxTransportMailItem;
			return new TestMessageConfig(mbxTransportMailItem.Message);
		}

		private string GetSenderSmtpAddress(StoreDriverDeliveryEventArgsImpl deliveryEventArguments)
		{
			return deliveryEventArguments.MailItemDeliver.MbxTransportMailItem.MimeSender.ToString();
		}

		private void SetCalculatedProperties(MessageItem replayItem)
		{
			string text = replayItem[StoreObjectSchema.ItemClass] as string;
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			bool flag = MailboxRulesAgent.IsSigned(text);
			replayItem[MessageItemSchema.IsSigned] = flag;
			bool flag2 = MailboxRulesAgent.IsReadReceipt(text);
			replayItem[MessageItemSchema.IsReadReceipt] = flag2;
		}

		public static readonly SmtpResponse MessageDeletedByRule = new SmtpResponse("250", "2.1.5", new string[]
		{
			"StoreDriver.Rules; message is deleted by mailbox rules"
		});

		public static readonly SmtpResponse MessageTooBig = new SmtpResponse("550", "5.2.3", new string[]
		{
			"StoreDriver.Rules; message is too big"
		});

		public static readonly SmtpResponse FormsMismatch = new SmtpResponse("550", "5.6.5", new string[]
		{
			"StoreDriver.Rules; message is not of the right message class"
		});

		public static readonly SmtpResponse AccessDenied = new SmtpResponse("550", "5.7.1", new string[]
		{
			"StoreDriver.Rules; message is denied by mailbox rules"
		});

		public static readonly SmtpResponse NotAuthorized = new SmtpResponse("550", "5.2.1", new string[]
		{
			"StoreDriver.Rules; message is not authorized by mailbox rules"
		});

		private static readonly Microsoft.Exchange.Diagnostics.Trace tracer = ExTraceGlobals.MailboxRuleTracer;

		private static readonly PercentileCounter processingTime = new PercentileCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(5.0), 10L, 1000L);

		private static RunningAverageFloat averageMilliseconds = new RunningAverageFloat(500);

		private DisposeTracker disposeTracker;

		private StoreDriverServer server;

		private RuleEvaluationContext evaluationContext;

		private MailboxEvaluationResult evaluationResult;

		private TestMessageConfig testMessageConfig;

		private bool disposed;

		private Stopwatch stopwatch;
	}
}
