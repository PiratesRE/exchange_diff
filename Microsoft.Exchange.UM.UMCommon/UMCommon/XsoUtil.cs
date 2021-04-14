using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal static class XsoUtil
	{
		public static MeetingResponse RespondToMeetingRequest(CalendarItemBase calendarItem, ResponseType responseType)
		{
			if (calendarItem.IsCancelled)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, 0, "Trying to respond to a cancelled meeting - {0}", new object[]
				{
					calendarItem.StoreObjectId
				});
				throw new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(calendarItem.StoreObjectId));
			}
			return calendarItem.RespondToMeetingRequest(responseType);
		}

		internal static string CombineConfigurationNames(string c1, string c2)
		{
			string text = string.Empty;
			if (!string.IsNullOrEmpty(c1))
			{
				text += c1.TrimEnd(new char[]
				{
					'.'
				});
			}
			if (!string.IsNullOrEmpty(c2))
			{
				text = text + "." + c2.TrimStart(new char[]
				{
					'.'
				});
			}
			return text.TrimStart(new char[]
			{
				'.'
			}).TrimEnd(new char[]
			{
				'.'
			});
		}

		internal static OutboundConversionOptions GetOutboundConversionOptions(UMRecipient user)
		{
			AcceptedDomain defaultAcceptedDomain = Utils.GetDefaultAcceptedDomain(user.ADRecipient);
			return new OutboundConversionOptions(defaultAcceptedDomain.DomainName.ToString())
			{
				UserADSession = ADRecipientLookupFactory.CreateFromUmUser(user).ScopedRecipientSession
			};
		}

		internal static void AddHiddenAttachment(AttachmentCollection attachmentCollection, ITempFile tmp, string attachmentName, string contentType)
		{
			using (FileStream fileStream = new FileStream(tmp.FilePath, FileMode.Open, FileAccess.Read))
			{
				XsoUtil.AddAttachment(attachmentCollection, fileStream, attachmentName, contentType, true);
			}
		}

		internal static void AddAttachment(AttachmentCollection attachmentCollection, ITempFile tmp, string attachmentName, string contentType)
		{
			using (FileStream fileStream = new FileStream(tmp.FilePath, FileMode.Open, FileAccess.Read))
			{
				XsoUtil.AddAttachment(attachmentCollection, fileStream, attachmentName, contentType, false);
			}
		}

		internal static void AddAttachment(AttachmentCollection attachmentCollection, Stream sourceStream, string attachmentName, string contentType)
		{
			XsoUtil.AddAttachment(attachmentCollection, sourceStream, attachmentName, contentType, false);
		}

		internal static void AddHiddenAttachment(AttachmentCollection attachmentCollection, Stream sourceStream, string attachmentName, string contentType)
		{
			XsoUtil.AddAttachment(attachmentCollection, sourceStream, attachmentName, contentType, true);
		}

		private static void AddAttachment(AttachmentCollection attachmentCollection, Stream sourceStream, string attachmentName, string contentType, bool markHidden)
		{
			using (StreamAttachment streamAttachment = (StreamAttachment)attachmentCollection.Create(AttachmentType.Stream))
			{
				streamAttachment.FileName = attachmentName;
				streamAttachment[AttachmentSchema.DisplayName] = attachmentName;
				streamAttachment.ContentType = contentType;
				using (Stream contentStream = streamAttachment.GetContentStream())
				{
					CommonUtil.CopyStream(sourceStream, contentStream);
				}
				streamAttachment[AttachmentSchema.AttachCalendarHidden] = markHidden;
				streamAttachment.Save();
			}
		}

		internal static void AppendVoiceClass(MessageItem msg)
		{
			if (msg != null)
			{
				string className = msg.ClassName;
				if (className != null && !XsoUtil.IsPureVoice(className) && !XsoUtil.IsMixedVoice(className))
				{
					msg.ClassName = className + ".Microsoft.Voicemail";
				}
			}
		}

		internal static void UpdateAttachementOrder(MessageItem newMsg, string attachName)
		{
			if (newMsg != null)
			{
				newMsg.Load(new PropertyDefinition[]
				{
					MessageItemSchema.VoiceMessageAttachmentOrder
				});
				string text = (string)XsoUtil.SafeGetProperty(newMsg, MessageItemSchema.VoiceMessageAttachmentOrder, string.Empty);
				if (!string.IsNullOrEmpty(text))
				{
					newMsg[MessageItemSchema.VoiceMessageAttachmentOrder] = text + ';' + attachName;
					return;
				}
				newMsg[MessageItemSchema.VoiceMessageAttachmentOrder] = attachName;
			}
		}

		internal static AttachmentHandle FindFirstAttachmentByContentType(MessageItem item, string contentType, int maxCount)
		{
			return XsoUtil.FindAttachment(item, maxCount, (Attachment attachment) => string.Equals(attachment.ContentType, contentType, StringComparison.OrdinalIgnoreCase));
		}

		internal static AttachmentHandle FindAttachmentByName(MessageItem item, string name, int maxCount)
		{
			return XsoUtil.FindAttachment(item, maxCount, (Attachment attachment) => string.Equals(attachment.FileName, name, StringComparison.OrdinalIgnoreCase));
		}

		internal static UndeleteContext Delete(MessageItem msg, MailboxSession store)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, null, "XSOUtil::Delete.", new object[0]);
			msg.Load(new PropertyDefinition[]
			{
				StoreObjectSchema.ParentItemId,
				StoreObjectSchema.SearchKey
			});
			UndeleteContext result = new UndeleteContext((StoreObjectId)msg[StoreObjectSchema.ParentItemId], (byte[])msg[StoreObjectSchema.SearchKey]);
			OperationResult operationResult = store.Delete(DeleteItemFlags.MoveToDeletedItems, new StoreId[]
			{
				msg.Id
			}).OperationResult;
			if (operationResult != OperationResult.Succeeded)
			{
				throw new DeleteFailedException();
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, null, "XSOUtil::Delete successfully delete message.", new object[0]);
			return result;
		}

		internal static StoreObjectId Undelete(UndeleteContext uc, MailboxSession store)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, null, "XSOUtil::Undelete.", new object[0]);
			ComparisonFilter condition = new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.SearchKey, uc.SearchKey);
			XsoUtil.DoUndelete(uc, store, condition);
			return XsoUtil.FindUndeletedItemId(uc, store, condition).ObjectId;
		}

		internal static void RemoveFromCalendar(StoreObjectId objectId, MailboxSession store)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, null, "XSOUtil::RemoveFromCalendar.", new object[0]);
			using (MeetingCancellation meetingCancellation = MeetingCancellation.Bind(store, objectId))
			{
				using (CalendarItemBase correlatedItem = meetingCancellation.GetCorrelatedItem())
				{
					if (correlatedItem != null)
					{
						store.Delete(DeleteItemFlags.MoveToDeletedItems, new StoreId[]
						{
							correlatedItem.Id
						});
					}
				}
			}
		}

		internal static void BuildParticipantStrings(IAttendeeCollection attendees, out string required, out string optional)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, null, "XSOUtil::BuildToListString.", new object[0]);
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			foreach (Attendee attendee in attendees)
			{
				if (attendee.AttendeeType == AttendeeType.Required)
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(";");
					}
					stringBuilder.Append(attendee.Participant.DisplayName);
				}
				else if (attendee.AttendeeType == AttendeeType.Optional)
				{
					if (stringBuilder2.Length != 0)
					{
						stringBuilder2.Append(";");
					}
					stringBuilder2.Append(attendee.Participant.DisplayName);
				}
			}
			required = stringBuilder.ToString();
			optional = stringBuilder2.ToString();
		}

		internal static void BuildParticipantStrings(RecipientCollection recips, out string to, out string cc)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, null, "XSOUtil::BuildToListString.", new object[0]);
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			foreach (Recipient recipient in recips)
			{
				if (recipient.RecipientItemType == RecipientItemType.To)
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(";");
					}
					stringBuilder.Append(XsoUtil.GetParticipantName(recipient.Participant));
				}
				else if (recipient.RecipientItemType == RecipientItemType.Cc)
				{
					if (stringBuilder2.Length != 0)
					{
						stringBuilder2.Append(";");
					}
					stringBuilder2.Append(XsoUtil.GetParticipantName(recipient.Participant));
				}
			}
			to = stringBuilder.ToString();
			cc = stringBuilder2.ToString();
		}

		internal static PhoneNumber GetSenderTelephoneNumber(IStorePropertyBag message)
		{
			string text = (string)XsoUtil.SafeGetProperty(message, MessageItemSchema.SenderTelephoneNumber, null);
			PhoneNumber result;
			if (!PhoneNumber.TryParse(text, out result))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, null, "XSOUtil::GetSenderTelephoneNumber: '{0}' is empty or invalid, returning null", new object[]
				{
					text
				});
				result = null;
			}
			return result;
		}

		internal static void SetSubscriberAccessSenderProperties(Item message, UMSubscriber subscriber)
		{
			ContactInfo contact = new ADContactInfo((ADUser)subscriber.ADRecipient);
			PhoneNumber callerId;
			ExAssert.RetailAssert(PhoneNumber.TryParse(subscriber.Extension, out callerId), "UMSubscriber.Extension:{0} is invalid!", new object[]
			{
				subscriber.Extension
			});
			XsoUtil.SetVoiceMessageSenderProperties(message, contact, subscriber.DialPlan, callerId);
		}

		internal static void SetVoiceMessageSenderProperties(Item message, ContactInfo contact, UMDialPlan dialPlan, PhoneNumber callerId)
		{
			PhoneNumber pstnCallbackTelephoneNumber = callerId.GetPstnCallbackTelephoneNumber(contact, dialPlan);
			message[MessageItemSchema.VoiceMessageSenderName] = (contact.DisplayName ?? string.Empty);
			message[MessageItemSchema.SenderTelephoneNumber] = callerId.ToDial;
			message[MessageItemSchema.PstnCallbackTelephoneNumber] = pstnCallbackTelephoneNumber.ToDial;
		}

		internal static object SafeGetProperty(IStorePropertyBag message, PropertyDefinition propertyDefinition, object defaultValue)
		{
			if (message == null)
			{
				return defaultValue;
			}
			object obj = message.TryGetProperty(propertyDefinition);
			if (obj == null || obj is PropertyError)
			{
				return defaultValue;
			}
			return obj;
		}

		internal static object CheckPropertyValue(object propertyValue, object defaultValue)
		{
			if (propertyValue == null || propertyValue is PropertyError)
			{
				return defaultValue;
			}
			return propertyValue;
		}

		internal static object[][] GetItemView(Folder folder, int rowCount, ItemQueryType flags, QueryFilter queryFilter, SortBy[] sortColumns, params PropertyDefinition[] dataColumns)
		{
			object[][] rows;
			using (QueryResult queryResult = folder.ItemQuery(flags, queryFilter, sortColumns, dataColumns))
			{
				rows = queryResult.GetRows(rowCount);
			}
			return rows;
		}

		internal static string GetAttachmentOrderString(Item item)
		{
			item.Load(new PropertyDefinition[]
			{
				MessageItemSchema.VoiceMessageAttachmentOrder
			});
			string text = ((string)XsoUtil.SafeGetProperty(item, MessageItemSchema.VoiceMessageAttachmentOrder, string.Empty)).Trim();
			if (string.IsNullOrEmpty(text))
			{
				text = XsoUtil.AttachmentOrderFromAttachments(item);
			}
			return text;
		}

		internal static bool IsValidProtectedAudioAttachment(Attachment attachment)
		{
			if (attachment == null)
			{
				return false;
			}
			if (string.IsNullOrEmpty(attachment.FileName) || string.IsNullOrEmpty(attachment.FileExtension))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, null, "IsValidAudioAttachment returning false because one of attachment name={0} or ext={1} is NULL or Empty.", new object[]
				{
					attachment.FileName,
					attachment.FileExtension
				});
				return false;
			}
			if (!string.Equals(attachment.FileExtension, ".umrmmp3", StringComparison.OrdinalIgnoreCase) && !string.Equals(attachment.FileExtension, ".umrmwav", StringComparison.OrdinalIgnoreCase) && !string.Equals(attachment.FileExtension, ".umrmwma", StringComparison.OrdinalIgnoreCase))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, null, "IsValidAudioAttachment returning false because the extension={0} is neither protected versions of WMA nor WAV nor MP3.", new object[]
				{
					attachment.FileExtension
				});
				return false;
			}
			return true;
		}

		internal static bool IsValidAudioAttachment(Attachment attachment)
		{
			if (attachment == null)
			{
				return false;
			}
			if (string.IsNullOrEmpty(attachment.FileName) || string.IsNullOrEmpty(attachment.FileExtension))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, null, "IsValidAudioAttachment returning false because one of attachment name={0} or ext={1} is NULL or Empty.", new object[]
				{
					attachment.FileName,
					attachment.FileExtension
				});
				return false;
			}
			if (!string.Equals(attachment.FileExtension, ".wma", StringComparison.OrdinalIgnoreCase) && !string.Equals(attachment.FileExtension, ".wav", StringComparison.OrdinalIgnoreCase) && !string.Equals(attachment.FileExtension, ".mp3", StringComparison.OrdinalIgnoreCase))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, null, "IsValidAudioAttachment returning false because the extension={0} is neither WMA nor WAV nor MP3.", new object[]
				{
					attachment.FileExtension
				});
				return false;
			}
			return true;
		}

		internal static bool IsMissedCall(string itemClass)
		{
			bool result = false;
			if (itemClass != null)
			{
				result = ObjectClass.IsOfClass(itemClass, "IPM.Note.Microsoft.Missed.Voice");
			}
			return result;
		}

		internal static bool IsPureVoice(string itemClass)
		{
			bool result = false;
			if (itemClass != null)
			{
				result = itemClass.StartsWith("IPM.Note.Microsoft.Voicemail.UM", StringComparison.OrdinalIgnoreCase);
			}
			return result;
		}

		internal static bool IsProtectedVoicemail(string itemClass)
		{
			bool result = false;
			if (itemClass != null)
			{
				result = itemClass.StartsWith("IPM.Note.rpmsg.Microsoft.Voicemail.UM", StringComparison.OrdinalIgnoreCase);
			}
			return result;
		}

		internal static bool IsMixedVoice(string itemClass)
		{
			bool result = false;
			if (itemClass != null)
			{
				result = itemClass.EndsWith("Microsoft.Voicemail", StringComparison.OrdinalIgnoreCase);
			}
			return result;
		}

		internal static bool IsMailboxQuotaExceeded(UMMailboxRecipient user, uint maxMessageSize)
		{
			bool result;
			try
			{
				if (user == null || maxMessageSize <= 0U)
				{
					PIIMessage data = PIIMessage.Create(PIIType._EmailAddress, user.MailAddress);
					CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, null, data, "IsMailboxQuotaExceeded called maxMessageSize={0} for user _EmailAddress.", new object[]
					{
						maxMessageSize
					});
					result = false;
				}
				else
				{
					UMMailboxRecipient.MailboxSessionLock mailboxSessionLock2;
					UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = mailboxSessionLock2 = user.CreateSessionLock();
					try
					{
						result = XsoUtil.IsOverReceiveQuota(mailboxSessionLock.Session.Mailbox, (ulong)maxMessageSize);
					}
					finally
					{
						if (mailboxSessionLock2 != null)
						{
							((IDisposable)mailboxSessionLock2).Dispose();
						}
					}
				}
			}
			catch (StorageTransientException ex)
			{
				PIIMessage data2 = PIIMessage.Create(PIIType._EmailAddress, user.MailAddress);
				CallIdTracer.TraceError(ExTraceGlobals.XsoTracer, null, data2, "IsMailboxQuotaExceeded: Failed to retrieve quota for _EmailAddress. Exception: {0}.", new object[]
				{
					ex
				});
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_FailedToRetrieveMailboxData, null, new object[]
				{
					CallId.Id,
					user.MailAddress,
					CommonUtil.ToEventLogString(Utils.ConcatenateMessagesOnException(ex))
				});
				result = false;
			}
			catch (StoragePermanentException ex2)
			{
				PIIMessage data3 = PIIMessage.Create(PIIType._EmailAddress, user.MailAddress);
				CallIdTracer.TraceError(ExTraceGlobals.XsoTracer, null, data3, "IsMailboxQuotaExceeded: Failed to retrieve quota for _EmailAddress. Exception: {0}.", new object[]
				{
					ex2
				});
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_FailedToRetrieveMailboxData, null, new object[]
				{
					CallId.Id,
					user.MailAddress,
					CommonUtil.ToEventLogString(Utils.ConcatenateMessagesOnException(ex2))
				});
				result = false;
			}
			return result;
		}

		internal static void SetUMOutlookUIFlags(ADUser user, CommonConstants.UMOutlookUIFlags flags)
		{
			ExchangePrincipal mailboxOwner = ExchangePrincipal.FromADUser(user, RemotingOptions.AllowCrossSite);
			MailboxSession mailboxSession = null;
			using (MailboxSession mailboxSession = MailboxSessionEstablisher.OpenAsAdmin(mailboxOwner, CultureInfo.InvariantCulture, "Client=UM;Action=SetOutlookFlags"))
			{
				try
				{
					using (UserConfiguration folderConfiguration = mailboxSession.UserConfigurationManager.GetFolderConfiguration("UMOLK.UserOptions", UserConfigurationTypes.Dictionary, mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox)))
					{
						IDictionary dictionary = folderConfiguration.GetDictionary();
						dictionary["outlookFlags"] = (int)flags;
						folderConfiguration.Save();
					}
				}
				catch (ObjectNotFoundException)
				{
					XsoUtil.RebuildOutlookConfiguration(mailboxSession, flags);
				}
				catch (CorruptDataException)
				{
					XsoUtil.RebuildOutlookConfiguration(mailboxSession, flags);
				}
				catch (InvalidOperationException)
				{
					XsoUtil.RebuildOutlookConfiguration(mailboxSession, flags);
				}
			}
		}

		internal static CommonConstants.UMOutlookUIFlags GetUMOutlookUIFlags(ADUser user)
		{
			ExchangePrincipal mailboxOwner = ExchangePrincipal.FromADUser(user, RemotingOptions.AllowCrossSite);
			MailboxSession mailboxSession = null;
			CommonConstants.UMOutlookUIFlags result;
			using (MailboxSession mailboxSession = MailboxSessionEstablisher.OpenAsAdmin(mailboxOwner, CultureInfo.InvariantCulture, "Client=UM;Action=GetOutlookFlags"))
			{
				try
				{
					using (UserConfiguration folderConfiguration = mailboxSession.UserConfigurationManager.GetFolderConfiguration("UMOLK.UserOptions", UserConfigurationTypes.Dictionary, mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox)))
					{
						IDictionary dictionary = folderConfiguration.GetDictionary();
						CommonConstants.UMOutlookUIFlags umoutlookUIFlags = (CommonConstants.UMOutlookUIFlags)dictionary["outlookFlags"];
						result = umoutlookUIFlags;
					}
				}
				catch (ObjectNotFoundException)
				{
					XsoUtil.RebuildOutlookConfiguration(mailboxSession, CommonConstants.UMOutlookUIFlags.None);
					result = CommonConstants.UMOutlookUIFlags.None;
				}
				catch (CorruptDataException)
				{
					XsoUtil.RebuildOutlookConfiguration(mailboxSession, CommonConstants.UMOutlookUIFlags.None);
					result = CommonConstants.UMOutlookUIFlags.None;
				}
				catch (InvalidOperationException)
				{
					XsoUtil.RebuildOutlookConfiguration(mailboxSession, CommonConstants.UMOutlookUIFlags.None);
					result = CommonConstants.UMOutlookUIFlags.None;
				}
				catch (InvalidCastException)
				{
					XsoUtil.RebuildOutlookConfiguration(mailboxSession, CommonConstants.UMOutlookUIFlags.None);
					result = CommonConstants.UMOutlookUIFlags.None;
				}
			}
			return result;
		}

		internal static bool IsOverReceiveQuota(Mailbox mailbox, ulong additionalBytes)
		{
			mailbox.Load(new PropertyDefinition[]
			{
				MailboxSchema.QuotaProhibitReceive,
				MailboxSchema.QuotaUsedExtended
			});
			int num = (int)XsoUtil.SafeGetProperty(mailbox, MailboxSchema.QuotaProhibitReceive, -1);
			if (num < 0)
			{
				return false;
			}
			ulong num2 = (ulong)((long)num);
			ulong num3 = (ulong)((long)XsoUtil.SafeGetProperty(mailbox, MailboxSchema.QuotaUsedExtended, 0));
			ulong num4 = num3 / 1024UL;
			return num2 <= checked(num4 + additionalBytes / 1024UL);
		}

		internal static bool IsOverSendQuota(Mailbox mailbox, ulong additionalBytes)
		{
			mailbox.Load(new PropertyDefinition[]
			{
				MailboxSchema.QuotaProhibitSend,
				MailboxSchema.QuotaUsedExtended
			});
			int num = (int)XsoUtil.SafeGetProperty(mailbox, MailboxSchema.QuotaProhibitSend, -1);
			if (num < 0)
			{
				return false;
			}
			ulong num2 = (ulong)((long)num);
			ulong num3 = (ulong)((long)XsoUtil.SafeGetProperty(mailbox, MailboxSchema.QuotaUsedExtended, 0));
			ulong num4 = num3 / 1024UL;
			return num2 <= checked(num4 + additionalBytes / 1024UL);
		}

		internal static StoreObjectId GetDraftsFolderId(MailboxSession session)
		{
			return session.GetDefaultFolderId(DefaultFolderType.Drafts) ?? session.GetDefaultFolderId(DefaultFolderType.Outbox);
		}

		internal static void LogMailboxConnectionFailureException(Exception e, ExchangePrincipal mailboxPrincipal)
		{
			if (XsoUtil.IsMailboxConnectionFailureException(e))
			{
				XsoUtil.LogMailboxException(e, mailboxPrincipal);
			}
		}

		internal static void LogMailboxException(Exception e, ExchangePrincipal mailboxPrincipal)
		{
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_MailboxAccessFailure, mailboxPrincipal.MailboxInfo.MailboxDatabase.ToString(), new object[]
			{
				mailboxPrincipal.MailboxInfo.DisplayName,
				mailboxPrincipal.MailboxInfo.Location.ServerFqdn,
				mailboxPrincipal.MailboxInfo.MailboxDatabase.ToString(),
				CommonUtil.ToEventLogString(Utils.ConcatenateMessagesOnException(e))
			});
		}

		internal static bool IsMailboxConnectionFailureException(Exception e)
		{
			return e is ConnectionFailedPermanentException || e is ConnectionFailedTransientException;
		}

		internal static string GetProperty(object[] result, StorePropertyDefinition propId, StorePropertyDefinition[] propertyDefinitions)
		{
			int num = Array.IndexOf<StorePropertyDefinition>(propertyDefinitions, propId);
			object obj = result[num];
			string result2 = null;
			if (obj != null && !(obj is PropertyError))
			{
				string text = obj as string;
				if (text != null)
				{
					result2 = Utils.TrimSpaces(text);
				}
			}
			return result2;
		}

		internal static T GetProperty<T>(object[] result, StorePropertyDefinition propId, StorePropertyDefinition[] propertyDefinitions)
		{
			return XsoUtil.GetProperty<T>(result, propId, propertyDefinitions, default(T));
		}

		internal static T GetProperty<T>(object[] result, StorePropertyDefinition propId, StorePropertyDefinition[] propertyDefinitions, T defaultValue)
		{
			int num = Array.IndexOf<StorePropertyDefinition>(propertyDefinitions, propId);
			object obj = result[num];
			if (obj == null || obj is PropertyError)
			{
				return defaultValue;
			}
			return (T)((object)obj);
		}

		internal static MessageItem CreateTemporaryMessage(MailboxSession session, Folder folder, int retentionDays)
		{
			MessageItem messageItem = MessageItem.Create(session, folder.Id);
			PolicyTagHelper.SetRetentionProperties(messageItem, ExDateTime.UtcNow.AddDays((double)retentionDays), retentionDays);
			return messageItem;
		}

		private static void RebuildOutlookConfiguration(MailboxSession session, CommonConstants.UMOutlookUIFlags flags)
		{
			session.UserConfigurationManager.DeleteFolderConfigurations(session.GetDefaultFolderId(DefaultFolderType.Inbox), new string[]
			{
				"UMOLK.UserOptions"
			});
			using (UserConfiguration userConfiguration = session.UserConfigurationManager.CreateFolderConfiguration("UMOLK.UserOptions", UserConfigurationTypes.Dictionary, session.GetDefaultFolderId(DefaultFolderType.Inbox)))
			{
				IDictionary dictionary = userConfiguration.GetDictionary();
				dictionary["outlookFlags"] = (int)flags;
				userConfiguration.Save();
			}
		}

		private static string GetParticipantName(Participant participant)
		{
			if (!string.IsNullOrEmpty(participant.DisplayName))
			{
				return participant.DisplayName;
			}
			return participant.EmailAddress;
		}

		private static string AttachmentOrderFromAttachments(Item item)
		{
			StringBuilder stringBuilder = new StringBuilder(32);
			if (!XsoUtil.IsPureVoice(item.ClassName) && !XsoUtil.IsMixedVoice(item.ClassName) && !XsoUtil.IsProtectedVoicemail(item.ClassName))
			{
				int num = 0;
				foreach (AttachmentHandle handle in item.AttachmentCollection)
				{
					using (Attachment attachment = item.AttachmentCollection.Open(handle))
					{
						if (XsoUtil.IsValidAudioAttachment(attachment))
						{
							if (num > 0)
							{
								stringBuilder.Append(';');
							}
							stringBuilder.Append(attachment.FileName);
							num++;
						}
					}
				}
			}
			return stringBuilder.ToString();
		}

		private static void DoUndelete(UndeleteContext uc, MailboxSession store, ComparisonFilter condition)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, null, "XSOUtil::DoUndelete.", new object[0]);
			using (Folder folder = Folder.Bind(store, store.GetDefaultFolderId(DefaultFolderType.DeletedItems)))
			{
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, null, new PropertyDefinition[]
				{
					ItemSchema.Id
				}))
				{
					queryResult.SeekToCondition(SeekReference.OriginBeginning, condition);
					object[][] rows = queryResult.GetRows(1);
					if (1 != rows.Length)
					{
						throw new UndeleteNotFoundException();
					}
					CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, null, "XSOUtil::DoUndelete found message to undelete.", new object[0]);
					StoreObjectId[] ids = new StoreObjectId[]
					{
						((VersionedId)rows[0][0]).ObjectId
					};
					OperationResult operationResult = store.Move(uc.ParentFolderId, ids).OperationResult;
					if (operationResult != OperationResult.Succeeded)
					{
						throw new UndeleteFailedException();
					}
					CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, null, "XSOUtil::DoUndelete successfully undeleted item.", new object[0]);
				}
			}
		}

		private static VersionedId FindUndeletedItemId(UndeleteContext uc, MailboxSession store, ComparisonFilter condition)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, null, "XSOUtil::FindUndeletedItem.", new object[0]);
			VersionedId result;
			using (Folder folder = Folder.Bind(store, uc.ParentFolderId))
			{
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, null, new PropertyDefinition[]
				{
					ItemSchema.Id
				}))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, null, "XSOUtil::FindUndeletedItemId looking for undeleted item in original folder.", new object[0]);
					queryResult.SeekToCondition(SeekReference.OriginBeginning, condition);
					object[][] rows = queryResult.GetRows(1);
					if (1 != rows.Length)
					{
						throw new UndeleteNotFoundException();
					}
					CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, null, "XSOUtil::FindUndeletedItemId successfully found undeleted item id.", new object[0]);
					result = (VersionedId)rows[0][0];
				}
			}
			return result;
		}

		private static AttachmentHandle FindAttachment(MessageItem item, int maxCount, XsoUtil.AttachmentComparerDelegate comparer)
		{
			if (maxCount < 1)
			{
				throw new ArgumentException("maxCount");
			}
			AttachmentHandle result = null;
			foreach (AttachmentHandle attachmentHandle in item.AttachmentCollection)
			{
				if (maxCount-- == 0)
				{
					CallIdTracer.TraceWarning(ExTraceGlobals.XsoTracer, null, "XSOUtil::FindAttachment: Could not find attachment.", new object[0]);
					break;
				}
				using (Attachment attachment = item.AttachmentCollection.Open(attachmentHandle))
				{
					if (comparer(attachment))
					{
						result = attachmentHandle;
						break;
					}
				}
			}
			return result;
		}

		internal static readonly NativeStorePropertyDefinition ReminderOffsetTimeIntervalPropId = GuidIdPropertyDefinition.CreateCustom("ReminderOffsetTimeInternal", typeof(int), XsoUtil.PSETIDCommon, 34049, PropertyFlags.None);

		private static readonly Guid PSETIDCommon = new Guid("{00062008-0000-0000-c000-000000000046}");

		private delegate bool AttachmentComparerDelegate(Attachment attachment);
	}
}
