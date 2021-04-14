using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Groups;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage.GroupMailbox.Escalation
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class GroupEscalation
	{
		public static bool IsEscalationEnabled()
		{
			if (GroupEscalation.escalationComponentState == null)
			{
				GroupEscalation.escalationComponentState = new GroupEscalation.EscalationComponentState?((GroupEscalation.EscalationComponentState)Util.GetRegistryValueOrDefault(GroupEscalation.RegistryKeysLocation, GroupEscalation.GroupEscalationEnabledValueName, 1, GroupEscalation.Tracer));
			}
			return GroupEscalation.escalationComponentState != GroupEscalation.EscalationComponentState.Disabled;
		}

		public GroupEscalation(IXSOFactory xsoFactory, IGroupEscalationFlightInfo groupEscalationFlightInfo, IMailboxUrls mailboxUrls)
		{
			Util.ThrowOnNullArgument(groupEscalationFlightInfo, "groupEscalationFlightInfo");
			this.xsoFactory = xsoFactory;
			this.groupEscalationFlightInfo = groupEscalationFlightInfo;
			this.mailboxUrls = mailboxUrls;
		}

		public bool EscalateItem(IMessageItem item, IMailboxSession mailboxSession, out bool isTransientError, bool isFromYammer = false)
		{
			Util.ThrowOnNullArgument(item, "item");
			Util.ThrowOnNullArgument(mailboxSession, "mailboxSession");
			isTransientError = false;
			string yammerEscalateAddress;
			List<string> escalatedUsersList = this.GetEscalatedUsersList(mailboxSession, out yammerEscalateAddress);
			bool flag = GroupEscalation.ShouldEscalateToYammer(isFromYammer, yammerEscalateAddress, item.ClassName);
			if (escalatedUsersList.Count < 1 && !flag)
			{
				GroupEscalation.Tracer.TraceDebug((long)this.GetHashCode(), "GroupEscalation.EscalateItem: Skipping Escalation as there are no users in need to escalate");
				return true;
			}
			bool result = false;
			IExtensibleLogger logger = GroupEscalateItemDiagnosticsFrameFactory.Default.CreateLogger(mailboxSession.MailboxGuid, mailboxSession.OrganizationId);
			IGroupEscalateItemPerformanceTracker groupEscalateItemPerformanceTracker = GroupEscalateItemDiagnosticsFrameFactory.Default.CreatePerformanceTracker(mailboxSession);
			using (GroupEscalateItemDiagnosticsFrameFactory.Default.CreateDiagnosticsFrame("XSO", "EscalateItem", logger, groupEscalateItemPerformanceTracker))
			{
				try
				{
					if (item.From != null)
					{
						groupEscalateItemPerformanceTracker.OriginalMessageSender = (item.From.EmailAddress ?? string.Empty);
						groupEscalateItemPerformanceTracker.OriginalMessageSenderRecipientType = ((item.From.Origin != null) ? item.From.Origin.ToString() : string.Empty);
						groupEscalateItemPerformanceTracker.OriginalMessageClass = (item.ClassName ?? string.Empty);
						groupEscalateItemPerformanceTracker.OriginalMessageId = ((item.Id != null) ? item.Id.ToBase64String() : string.Empty);
						groupEscalateItemPerformanceTracker.OriginalInternetMessageId = (item.InternetMessageId ?? string.Empty);
					}
					StoreObjectId escalationFolderId = GroupEscalation.GetEscalationFolderId(mailboxSession);
					using (IMessageItem messageItem = this.CreateEscalationMessage(mailboxSession, escalationFolderId, item))
					{
						ExDateTime? valueOrDefault = item.PropertyBag.GetValueOrDefault<ExDateTime?>(ItemSchema.SentTime);
						if (valueOrDefault != null)
						{
							messageItem[InternalSchema.OriginalSentTimeForEscalation] = valueOrDefault;
						}
						else
						{
							this.TraceAndLogError(logger, "EscalateItem", "GroupEscalation.EscalateItem: Couldn't find client submit time.");
						}
						IExchangePrincipal mailboxOwner = mailboxSession.MailboxOwner;
						Participant participant = new Participant(mailboxOwner);
						if (!ObjectClass.IsMeetingMessage(messageItem.ClassName))
						{
							messageItem.Sender = participant;
						}
						messageItem.SuppressAllAutoResponses();
						messageItem.IsGroupEscalationMessage = true;
						Participant[] array = new Participant[messageItem.Recipients.Count];
						int num = 0;
						foreach (Recipient recipient in messageItem.Recipients)
						{
							recipient[ItemSchema.Responsibility] = false;
							array[num] = recipient.Participant;
							num++;
						}
						groupEscalateItemPerformanceTracker.ParticipantsInOriginalMessage = array.Length;
						List<Participant> list = new List<Participant>(array);
						Stopwatch stopwatch = new Stopwatch();
						stopwatch.Start();
						if (!participant.ExistIn(array))
						{
							Recipient recipient2 = messageItem.Recipients.Add(participant, RecipientItemType.To);
							list.Add(participant);
							recipient2[ItemSchema.Responsibility] = false;
							groupEscalateItemPerformanceTracker.IsGroupParticipantAddedToParticipants = true;
						}
						stopwatch.Stop();
						groupEscalateItemPerformanceTracker.EnsureGroupParticipantAddedMilliseconds = stopwatch.ElapsedMilliseconds;
						stopwatch.Reset();
						stopwatch.Start();
						bool flag2 = false;
						foreach (string emailAddress in escalatedUsersList)
						{
							Participant participant2 = new Participant.Builder
							{
								EmailAddress = emailAddress,
								RoutingType = "EX"
							}.ToParticipant();
							if (!participant2.ExistIn(array))
							{
								Recipient recipient3 = messageItem.Recipients.Add(participant2, RecipientItemType.Bcc);
								recipient3[ItemSchema.Responsibility] = true;
								flag2 = true;
								groupEscalateItemPerformanceTracker.IncrementParticipantsAddedToEscalatedMessage();
							}
							else
							{
								groupEscalateItemPerformanceTracker.IncrementParticipantsSkippedInEscalatedMessage();
							}
						}
						stopwatch.Stop();
						groupEscalateItemPerformanceTracker.DedupeParticipantsMilliseconds = stopwatch.ElapsedMilliseconds;
						groupEscalateItemPerformanceTracker.HasEscalatedUser = flag2;
						if (!flag2 && !flag)
						{
							GroupEscalation.Tracer.TraceDebug((long)this.GetHashCode(), "GroupEscalation.EscalateItem: Skipping Escalation as there are no additional users in need to escalate");
							return true;
						}
						messageItem.MarkRecipientAsSubmitted(list.ToArray());
						if (flag)
						{
							this.EscalateMessageToYammer(mailboxSession, logger, groupEscalateItemPerformanceTracker, yammerEscalateAddress, flag2, escalationFolderId, messageItem);
						}
						if (flag2)
						{
							this.InsertUnsubscribeLinkToBody(groupEscalateItemPerformanceTracker, logger, mailboxOwner, item, messageItem);
							this.SendEscalateMessage(messageItem);
						}
						result = true;
					}
				}
				catch (StoragePermanentException arg)
				{
					this.TraceAndLogError(logger, "EscalateItem", string.Format("GroupEscalation.EscalateItem: Escalation failed. Got store permanent exception: {0}", arg));
				}
				catch (StorageTransientException arg2)
				{
					this.TraceAndLogError(logger, "EscalateItem", string.Format("GroupEscalation.EscalateItem: Escalation failed. Got store transient exception: {0}", arg2));
					isTransientError = true;
				}
				catch (Exception arg3)
				{
					this.TraceAndLogError(logger, "EscalateItem", string.Format("GroupEscalation.EscalateItem: Escalation failed. Got exception: {0}", arg3));
					throw;
				}
			}
			return result;
		}

		private static StoreObjectId GetEscalationFolderId(IMailboxSession mailboxSession)
		{
			return mailboxSession.GetDefaultFolderId(DefaultFolderType.TemporarySaves) ?? mailboxSession.CreateDefaultFolder(DefaultFolderType.TemporarySaves);
		}

		private static bool ShouldEscalateToYammer(bool isFromYammer, string yammerEscalateAddress, string className)
		{
			return !string.IsNullOrEmpty(yammerEscalateAddress) && !isFromYammer && className == "IPM.Note";
		}

		private IMessageItem CreateEscalationMessage(IMailboxSession mailboxSession, StoreId escalationFolderId, IMessageItem originalMessage)
		{
			IMessageItem messageItem = this.xsoFactory.Create(mailboxSession, escalationFolderId);
			messageItem.Load(InternalSchema.ContentConversionProperties);
			CoreItem.CopyItemContentExcept(originalMessage.CoreItem, messageItem.CoreItem, GroupEscalation.PropertiesToExcludeFromCopy);
			messageItem.CharsetDetector.DetectionOptions = originalMessage.CharsetDetector.DetectionOptions;
			messageItem.SaveFlags |= (originalMessage.SaveFlags | PropertyBagSaveFlags.IgnoreMapiComputedErrors | PropertyBagSaveFlags.IgnoreAccessDeniedErrors);
			return messageItem;
		}

		private void EscalateMessageToYammer(IMailboxSession mailboxSession, IExtensibleLogger logger, IGroupEscalateItemPerformanceTracker performanceTracker, string yammerEscalateAddress, bool hasEscalatedUser, StoreObjectId draftsFolderId, IMessageItem escalatedMessage)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			performanceTracker.EscalateToYammer = true;
			GroupEscalation.Tracer.TraceDebug((long)this.GetHashCode(), "GroupEscalation.EscalateItem: Escalating message to yammer group: " + yammerEscalateAddress);
			if (hasEscalatedUser)
			{
				try
				{
					using (IMessageItem messageItem = this.CreateEscalationMessage(mailboxSession, draftsFolderId, escalatedMessage))
					{
						this.SendEscalateMessageToYammer(messageItem, yammerEscalateAddress, mailboxSession);
					}
					goto IL_7D;
				}
				catch (Exception arg)
				{
					this.TraceAndLogError(logger, "EscalateItem", string.Format("GroupEscalation.EscalateItem: Escalation to yammer failed. Got exception: {0}", arg));
					goto IL_7D;
				}
			}
			this.SendEscalateMessageToYammer(escalatedMessage, yammerEscalateAddress, mailboxSession);
			IL_7D:
			stopwatch.Stop();
			performanceTracker.SendToYammerMilliseconds = stopwatch.ElapsedMilliseconds;
		}

		private void SendEscalateMessageToYammer(IMessageItem escalatedMessage, string yammerEscalateAddress, IMailboxSession mailboxSession)
		{
			foreach (Recipient recipient in escalatedMessage.Recipients)
			{
				recipient.Submitted = true;
				recipient[ItemSchema.Responsibility] = false;
			}
			this.SetUniqueBody(escalatedMessage, mailboxSession);
			Participant.Builder builder = new Participant.Builder
			{
				EmailAddress = yammerEscalateAddress,
				RoutingType = "SMTP"
			};
			Recipient recipient2 = escalatedMessage.Recipients.Add(builder.ToParticipant(), RecipientItemType.Bcc);
			recipient2[ItemSchema.Responsibility] = true;
			this.SendEscalateMessage(escalatedMessage);
		}

		private void SetUniqueBody(IMessageItem escalatedMessage, IMailboxSession mailboxSession)
		{
			StoreId messageStoreId = this.GetMessageStoreId(escalatedMessage.InReplyTo, mailboxSession);
			if (messageStoreId != null)
			{
				using (IMessageItem messageItem = this.xsoFactory.BindToMessage(mailboxSession, messageStoreId, null))
				{
					BodyDiffer bodyDiffer = new BodyDiffer(messageItem.Body.GetConversationBodyScanner(), escalatedMessage.Body.GetConversationBodyScanner());
					FragmentInfo uniqueBodyPart = bodyDiffer.UniqueBodyPart;
					using (TextWriter textWriter = escalatedMessage.Body.OpenTextWriter(new BodyWriteConfiguration(BodyFormat.TextHtml)))
					{
						using (HtmlWriter htmlWriter = new HtmlWriter(textWriter))
						{
							uniqueBodyPart.WriteHtml(htmlWriter);
						}
					}
				}
			}
		}

		private StoreId GetMessageStoreId(string internetId, IMailboxSession session)
		{
			if (string.IsNullOrEmpty(internetId))
			{
				return null;
			}
			IStorePropertyBag[] array = AllItemsFolderHelper.FindItemsFromInternetId((MailboxSession)session, internetId, new StorePropertyDefinition[]
			{
				CoreItemSchema.Id
			});
			if (array == null || array.Length == 0)
			{
				return null;
			}
			IStorePropertyBag storePropertyBag = array[0];
			if (storePropertyBag == null)
			{
				return null;
			}
			VersionedId valueOrDefault = storePropertyBag.GetValueOrDefault<VersionedId>(CoreItemSchema.Id, null);
			if (valueOrDefault == null)
			{
				return null;
			}
			return valueOrDefault.ObjectId;
		}

		internal static void Enable()
		{
			GroupEscalation.escalationComponentState = new GroupEscalation.EscalationComponentState?(GroupEscalation.EscalationComponentState.Enabled);
		}

		internal static void Disable()
		{
			GroupEscalation.escalationComponentState = new GroupEscalation.EscalationComponentState?(GroupEscalation.EscalationComponentState.Disabled);
		}

		protected virtual List<string> GetEscalatedUsersList(IMailboxSession session, out string yammerEscalateAddress)
		{
			ProxyAddress proxyAddress = ProxyAddress.Parse(session.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString());
			if (proxyAddress is InvalidProxyAddress)
			{
				GroupEscalation.Tracer.TraceError<string>((long)this.GetHashCode(), "GroupEscalation.GetEscalatedUsersList: Escalation failed. Could not parse smtp address: {0}", session.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString());
				yammerEscalateAddress = string.Empty;
				return new List<string>();
			}
			GroupMailboxLocator groupMailboxLocator = GroupMailboxLocator.Instantiate(session.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid), proxyAddress);
			yammerEscalateAddress = groupMailboxLocator.GetYammerGroupAddress();
			EscalationGetter escalationGetter = new EscalationGetter(groupMailboxLocator, session);
			return escalationGetter.Execute();
		}

		protected virtual void SendEscalateMessage(IMessageItem escalatedMessage)
		{
			escalatedMessage.SendWithoutSavingMessage();
		}

		private void TraceAndLogError(IExtensibleLogger logger, string context, string errorMessage)
		{
			GroupEscalation.Tracer.TraceError((long)this.GetHashCode(), errorMessage);
			logger.LogEvent(new SchemaBasedLogEvent<GroupEscalateItemLogSchema.Error>
			{
				{
					GroupEscalateItemLogSchema.Error.Context,
					context
				},
				{
					GroupEscalateItemLogSchema.Error.Exception,
					errorMessage
				}
			});
		}

		private void InsertUnsubscribeLinkToBody(IGroupEscalateItemPerformanceTracker performanceTracker, IExtensibleLogger logger, IExchangePrincipal groupExchangePrincipal, IMessageItem originalItem, IMessageItem escalatedMessage)
		{
			performanceTracker.UnsubscribeUrlInserted = false;
			if (!this.groupEscalationFlightInfo.IsGroupEscalationFooterEnabled())
			{
				GroupEscalation.Tracer.TraceDebug((long)this.GetHashCode(), "GroupEscalation.InsertUnsubscribeLinkToBody: Skipping inserting link as the GroupsDeepLink flight isn't enabled");
				return;
			}
			GroupEscalationFooter groupEscalationFooter = new GroupEscalationFooter(groupExchangePrincipal.MailboxInfo.DisplayName, this.GetPreferredCulture(groupExchangePrincipal), new EscalationLinkBuilder(groupExchangePrincipal, this.mailboxUrls));
			try
			{
				bool unsubscribeUrlInserted = groupEscalationFooter.InsertFooterToTheBody(originalItem, escalatedMessage);
				performanceTracker.UnsubscribeUrlInserted = unsubscribeUrlInserted;
			}
			catch (Exception ex)
			{
				this.TraceAndLogError(logger, "EscalateItem", string.Format("GroupEscalation.InsertUnsubscribeLinkToBody: Escalation footer not inserted because of exception:{0}", ex.ToString()));
			}
			performanceTracker.BuildUnsubscribeUrlMilliseconds = groupEscalationFooter.LastLinkBuildTimeMs;
			performanceTracker.LinkBodySize = groupEscalationFooter.LastBodySizeBytes;
			performanceTracker.LinkOnBodyDetectionMilliseconds = groupEscalationFooter.LastLinkOnBodyDetectionTimeMs;
			performanceTracker.LinkInsertOnBodyMilliseconds = groupEscalationFooter.LastLinkInsertOnBodyTimeMs;
		}

		private CultureInfo GetPreferredCulture(IExchangePrincipal groupExchangePrincipal)
		{
			CultureInfo preferredCultureInfo = ClientCultures.GetPreferredCultureInfo(groupExchangePrincipal.PreferredCultures);
			return preferredCultureInfo ?? new CultureInfo("en-US");
		}

		private const string OperationContext = "XSO";

		private const string OperationEscalateItem = "EscalateItem";

		private static readonly Microsoft.Exchange.Diagnostics.Trace Tracer = ExTraceGlobals.COWGroupMessageEscalationTracer;

		internal static readonly string RegistryKeysLocation = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Groups";

		internal static readonly string GroupEscalationEnabledValueName = "GroupEscalationEnabled";

		private static readonly HashSet<NativeStorePropertyDefinition> PropertiesToExcludeFromCopy = new HashSet<NativeStorePropertyDefinition>
		{
			InternalSchema.EntryId,
			InternalSchema.ChangeKey,
			InternalSchema.ParentEntryId,
			InternalSchema.StoreEntryId,
			InternalSchema.StoreRecordKey,
			InternalSchema.OriginalEntryId
		};

		private static GroupEscalation.EscalationComponentState? escalationComponentState;

		private IXSOFactory xsoFactory;

		private IGroupEscalationFlightInfo groupEscalationFlightInfo;

		private IMailboxUrls mailboxUrls;

		private enum EscalationComponentState
		{
			Disabled,
			Enabled
		}
	}
}
