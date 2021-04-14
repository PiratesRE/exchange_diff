using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.IsMemberOfProvider;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.MailboxRules;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxTransport.StoreDriver.Shared;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;
using Microsoft.Exchange.MailboxTransport.StoreDriverDelivery;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Transport.MailboxRules
{
	internal abstract class RuleEvaluationContext : RuleEvaluationContextBase
	{
		internal StoreDriverServer Server
		{
			get
			{
				return this.server;
			}
		}

		protected RuleEvaluationContext(Folder folder, MessageItem message, StoreSession session, ProxyAddress recipient, ADRecipientCache<TransportMiniRecipient> recipientCache, long mimeSize, MailItemDeliver mailItemDeliver) : base(folder, message, session, recipient, recipientCache, mimeSize, Microsoft.Exchange.Transport.MailboxRules.RuleConfig.Instance, ExTraceGlobals.MailboxRuleTracer)
		{
			MailboxSession mailboxSession = session as MailboxSession;
			this.initialFolder = folder;
			this.mailItemDeliver = mailItemDeliver;
			if (mailboxSession != null)
			{
				object obj = mailboxSession.Mailbox.TryGetProperty(MailboxSchema.MailboxOofState);
				if (obj is PropertyError)
				{
					this.IsOof = mailboxSession.IsMailboxOof();
				}
				else
				{
					this.IsOof = (bool)obj;
				}
			}
			base.LimitChecker = new StoreDriverLimitChecker(this);
		}

		protected RuleEvaluationContext(RuleEvaluationContext parentContext) : base(parentContext)
		{
			this.mailItemDeliver = parentContext.mailItemDeliver;
		}

		protected RuleEvaluationContext()
		{
			this.traceFormatter = new TraceFormatter(false);
			this.tracer = ExTraceGlobals.MailboxRuleTracer;
			base.LimitChecker = new StoreDriverLimitChecker(this);
		}

		public override string DefaultDomainName
		{
			get
			{
				return Components.Configuration.FirstOrgAcceptedDomainTable.DefaultDomainName;
			}
		}

		public override IsMemberOfResolver<string> IsMemberOfResolver
		{
			get
			{
				return Components.MailboxRulesIsMemberOfResolverComponent.IsMemberOfResolver;
			}
		}

		public override string LocalServerFqdn
		{
			get
			{
				return Components.Configuration.LocalServer.TransportServer.Fqdn;
			}
		}

		public override IPAddress LocalServerNetworkAddress
		{
			get
			{
				return StoreDriverDelivery.LocalIP.AddressList[0];
			}
		}

		public override ExEventLog.EventTuple OofHistoryCorruption
		{
			get
			{
				return MailboxTransportEventLogConstants.Tuple_OofHistoryCorruption;
			}
		}

		public override ExEventLog.EventTuple OofHistoryFolderMissing
		{
			get
			{
				return MailboxTransportEventLogConstants.Tuple_OofHistoryFolderMissing;
			}
		}

		public DeliveryPriority Priority
		{
			get
			{
				return this.mailItemDeliver.MailItemWrapper.DeliveryPriority;
			}
		}

		public string PrioritizationReason
		{
			get
			{
				return this.mailItemDeliver.MailItemWrapper.PrioritizationReason;
			}
		}

		public MimePart RootPart
		{
			get
			{
				return this.mailItemDeliver.MbxTransportMailItem.RootPart;
			}
		}

		public MbxTransportMailItem MbxTransportMailItem
		{
			get
			{
				if (this.mailItemDeliver != null)
				{
					return this.mailItemDeliver.MbxTransportMailItem;
				}
				return null;
			}
		}

		public override List<KeyValuePair<string, string>> ExtraTrackingEventData
		{
			get
			{
				if (this.mailItemDeliver.ExtraTrackingEventData == null)
				{
					this.mailItemDeliver.ExtraTrackingEventData = new List<KeyValuePair<string, string>>();
				}
				return this.mailItemDeliver.ExtraTrackingEventData;
			}
		}

		public static RuleEvaluationContext Create(StoreDriverServer server, Folder folder, MessageItem message, StoreSession session, string recipientAddress, ADRecipientCache<TransportMiniRecipient> recipientCache, long mimeSize, bool processingTestMessage, bool shouldExecuteDisabledAndInErrorRules, MailItemDeliver mailItemDeliver)
		{
			return new MessageContext(folder, message, session, new SmtpProxyAddress(recipientAddress, true), recipientCache, mimeSize, mailItemDeliver)
			{
				server = server,
				traceFormatter = new TraceFormatter(processingTestMessage),
				ShouldExecuteDisabledAndInErrorRules = shouldExecuteDisabledAndInErrorRules
			};
		}

		public override MessageItem CreateMessageItem(PropertyDefinition[] prefetchProperties)
		{
			return MessageItem.CreateInMemory(prefetchProperties);
		}

		public virtual void UpdateDeferredError()
		{
			if (this.daeMessageEntryIds == null || this.daeMessageEntryIds.Count == 0)
			{
				return;
			}
			base.DeliveredMessage.Load(DeferredError.EntryId);
			byte[] deliveredMessageEntryId = base.DeliveredMessage[StoreObjectSchema.EntryId] as byte[];
			if (deliveredMessageEntryId == null)
			{
				base.TraceDebug("Delivered Message EntryId is null");
				return;
			}
			using (List<byte[]>.Enumerator enumerator = this.daeMessageEntryIds.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					byte[] daeEntryId = enumerator.Current;
					if (daeEntryId != null)
					{
						Exception argument;
						if (!RuleUtil.TryRunStoreCode(delegate
						{
							StoreObjectId storeObjectId = StoreObjectId.FromProviderSpecificId(daeEntryId);
							if (storeObjectId != null)
							{
								using (Item item = Microsoft.Exchange.Data.Storage.Item.Bind(this.StoreSession, storeObjectId))
								{
									item.OpenAsReadWrite();
									item[ItemSchema.OriginalMessageEntryId] = deliveredMessageEntryId;
									item.Save(SaveMode.NoConflictResolution);
								}
							}
						}, out argument))
						{
							base.TraceDebug<byte[], Exception>("Can't set pr_dam_original_entryid on DeferredError message id {0}, and the exception is: {1}", daeEntryId, argument);
						}
					}
				}
			}
		}

		public string GetTraces()
		{
			string result;
			using (Stream stream = new MemoryStream())
			{
				this.traceFormatter.CopyDataTo(stream);
				stream.Position = 0L;
				using (TextReader textReader = new StreamReader(stream))
				{
					string text = textReader.ReadToEnd();
					result = text;
				}
			}
			return result;
		}

		public override ISubmissionItem GenerateSubmissionItem(MessageItem item, WorkItem workItem)
		{
			return new SmtpSubmissionItem(this, item, workItem);
		}

		public EmailMessage GenerateTraceReport(SmtpAddress reportToAddress)
		{
			if (base.ProcessingTestMessage && reportToAddress.IsValidAddress)
			{
				EmailMessage traceReport = EmailMessage.Create();
				RoutingAddress address = GlobalConfigurationBase<MicrosoftExchangeRecipient, MicrosoftExchangeRecipientConfiguration>.Instance.Address;
				traceReport.From = new EmailRecipient(null, (string)address);
				traceReport.To.Add(new EmailRecipient(null, (string)reportToAddress));
				traceReport.Subject = "Tracing Report: " + base.Message.Subject;
				using (Stream contentWriteStream = traceReport.Body.GetContentWriteStream())
				{
					this.traceFormatter.CopyDataTo(contentWriteStream);
					contentWriteStream.Flush();
				}
				List<Exception> list = new List<Exception>();
				Exception item;
				if (!RuleUtil.TryRunStoreCode(delegate
				{
					this.AddRuleDumpAttachment(traceReport, this.initialFolder, "mailbox-rules.xml");
				}, out item))
				{
					list.Add(item);
				}
				if (!RuleUtil.TryRunStoreCode(delegate
				{
					MailboxSession mailboxSession = this.StoreSession as MailboxSession;
					StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
					if (defaultFolderId != this.initialFolder.Id.ObjectId)
					{
						using (Folder folder = Folder.Bind(mailboxSession, defaultFolderId))
						{
							this.AddRuleDumpAttachment(traceReport, folder, "inbox-rules.xml");
						}
					}
				}, out item))
				{
					list.Add(item);
				}
				if (!RuleUtil.TryRunStoreCode(delegate
				{
					this.AddOofHistoryAttachment(traceReport, "automatic-reply-history.xml");
				}, out item))
				{
					list.Add(item);
				}
				if (list.Count > 0)
				{
					Attachment attachment = traceReport.Attachments.Add("inboxrule-report-exceptions.txt");
					using (Stream contentWriteStream2 = attachment.GetContentWriteStream())
					{
						using (TextWriter textWriter = new StreamWriter(contentWriteStream2))
						{
							foreach (Exception ex in list)
							{
								textWriter.WriteLine(ex.ToString());
								textWriter.WriteLine();
							}
						}
					}
				}
				return traceReport;
			}
			return null;
		}

		public override Folder GetDeletedItemsFolder()
		{
			MailboxSession mailboxSession = base.StoreSession as MailboxSession;
			return base.OpenFolder(mailboxSession.GetDefaultFolderId(DefaultFolderType.DeletedItems));
		}

		public override void SetMailboxOwnerAsSender(MessageItem message)
		{
			MailboxSession mailboxSession = base.StoreSession as MailboxSession;
			Participant participant = new Participant(mailboxSession.MailboxOwner);
			message.Sender = participant;
			message.From = participant;
		}

		public override ExTimeZone DetermineRecipientTimeZone()
		{
			if (this.timeZoneRetrieved)
			{
				base.TraceDebug<ExTimeZone>("TimeZone retrieved before, returning it. TimeZone: {0}", this.timeZone);
				return this.timeZone;
			}
			MailboxSession mailboxSession = base.StoreSession as MailboxSession;
			if (!this.CheckMailboxSessionForTimeZone(mailboxSession))
			{
				return this.timeZone;
			}
			bool found = false;
			bool flag = false;
			MessageStatus messageStatus = this.RunUnderStorageExceptionHandler(delegate
			{
				found = this.TryFindOwaTimeZone(mailboxSession, out this.timeZone);
			});
			if (found)
			{
				this.timeZoneRetrieved = true;
				base.TraceDebug<ProxyAddress, ExTimeZone>("Found OWA user TimeZone configuration, using it. Recipient: {0}, TimeZone: {1}", base.Recipient, this.timeZone);
				return this.timeZone;
			}
			if (MessageStatus.Success != messageStatus)
			{
				base.TraceDebug<ProxyAddress, Exception>("Unable to retrieve OWA user configuration, trying to get Outlook configuration next. Recipient: {0}, Exception: {2}", base.Recipient, messageStatus.Exception);
				switch (messageStatus.Action)
				{
				case MessageAction.NDR:
				case MessageAction.Throw:
					flag = true;
					break;
				}
			}
			byte[] blob = null;
			messageStatus = this.RunUnderStorageExceptionHandler(delegate
			{
				found = this.TryFindOutlookTimeZone(mailboxSession, out blob);
			});
			if (!found)
			{
				if (MessageStatus.Success != messageStatus)
				{
					base.TraceDebug<ProxyAddress, Exception>("Unable to retrieve Outlook user configuration. Using server TimeZone instead. Recipient: {0}, Exception: {2}", base.Recipient, messageStatus.Exception);
					if (flag)
					{
						switch (messageStatus.Action)
						{
						case MessageAction.NDR:
						case MessageAction.Throw:
							this.timeZoneRetrieved = true;
							base.TraceDebug("Setting timeZoneRetrieved to true due to double permanent storage exception encountered.");
							break;
						}
					}
				}
				this.timeZone = ExTimeZone.CurrentTimeZone;
				base.TraceDebug<ProxyAddress, ExTimeZone>("Neither OWA nor Outlook user TimeZone configuration were found, Using server TimeZone instead. Recipient: {0}, TimeZone: {1}", base.Recipient, this.timeZone);
				return this.timeZone;
			}
			this.timeZoneRetrieved = true;
			if (this.TryParseTimeZoneBlob(blob, string.Empty, out this.timeZone))
			{
				base.TraceDebug<ProxyAddress, ExTimeZone>("Found Outlook user TimeZone configuration, using it. Recipient: {0}, TimeZone: {1}", base.Recipient, this.timeZone);
				return this.timeZone;
			}
			this.timeZone = ExTimeZone.CurrentTimeZone;
			base.TraceDebug<ProxyAddress, ExTimeZone>("Outlook user TimeZone blob could not be parsed in O12 TimeZone format. Using server TimeZone instead. Recipient: {0}, TimeZone: {1}", base.Recipient, this.timeZone);
			return this.timeZone;
		}

		public override void LogEvent(ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs)
		{
			StoreDriverDeliveryDiagnostics.LogEvent(tuple, periodicKey, messageArgs);
		}

		public override void MarkRuleInError(Rule rule, RuleAction.Type actionType, int actionIndex, DeferredError.RuleError errorCode)
		{
			base.MarkRuleInError(rule, actionType, actionIndex, errorCode);
			using (DeferredError deferredError = DeferredError.Create(base.StoreSession as MailboxSession, base.CurrentFolder.StoreObjectId, rule.Provider, rule.ID, actionType, actionIndex, errorCode))
			{
				byte[] array = deferredError.Save();
				if (array != null)
				{
					if (this.daeMessageEntryIds == null)
					{
						this.daeMessageEntryIds = new List<byte[]>();
					}
					this.daeMessageEntryIds.Add(array);
				}
			}
		}

		protected virtual MessageStatus RunUnderStorageExceptionHandler(StoreDriverDelegate action)
		{
			return StorageExceptionHandler.RunUnderExceptionHandler(this.mailItemDeliver, action);
		}

		protected virtual bool TryFindOwaTimeZone(MailboxSession mailboxSession, out ExTimeZone timeZone)
		{
			return TimeZoneSettings.TryFindOwaTimeZone(mailboxSession, out timeZone);
		}

		protected virtual bool TryFindOutlookTimeZone(MailboxSession mailboxSession, out byte[] blob)
		{
			return TimeZoneSettings.TryFindOutlookTimeZone(mailboxSession, out blob);
		}

		protected virtual bool TryParseTimeZoneBlob(byte[] blob, string displayName, out ExTimeZone timeZone)
		{
			return O12TimeZoneFormatter.TryParseTimeZoneBlob(blob, displayName, out timeZone);
		}

		protected virtual bool CheckMailboxSessionForTimeZone(MailboxSession mailboxSession)
		{
			if (mailboxSession == null)
			{
				this.timeZoneRetrieved = true;
				this.timeZone = ExTimeZone.CurrentTimeZone;
				base.TraceDebug<Type, ExTimeZone>("Session is not MailboxSession, using server time zone instead. SessionType: {0}, TimeZone: {1}", base.StoreSession.GetType(), this.timeZone);
				return false;
			}
			return true;
		}

		private void AddRuleDumpAttachment(EmailMessage traceReport, Folder folder, string attachmentFileName)
		{
			Attachment attachment = traceReport.Attachments.Add(attachmentFileName);
			using (Stream contentWriteStream = attachment.GetContentWriteStream())
			{
				MailboxSession session = base.StoreSession as MailboxSession;
				using (RuleWriter ruleWriter = new RuleWriter(session, folder, contentWriteStream))
				{
					ruleWriter.WriteRules();
				}
			}
		}

		private void AddOofHistoryAttachment(EmailMessage traceReport, string attachmentFileName)
		{
			Attachment attachment = traceReport.Attachments.Add(attachmentFileName);
			using (Stream contentWriteStream = attachment.GetContentWriteStream())
			{
				using (StreamWriter streamWriter = new StreamWriter(contentWriteStream))
				{
					StoreSession storeSession = base.StoreSession;
					using (OofHistory oofHistory = new OofHistory(null, null, this))
					{
						if (oofHistory.TryInitialize())
						{
							oofHistory.DumpHistory(streamWriter);
						}
						else
						{
							streamWriter.WriteLine("Automatic reply history does not exist.");
						}
					}
				}
			}
		}

		private List<byte[]> daeMessageEntryIds;

		private Folder initialFolder;

		private bool timeZoneRetrieved;

		private ExTimeZone timeZone;

		private MailItemDeliver mailItemDeliver;

		protected StoreDriverServer server;
	}
}
