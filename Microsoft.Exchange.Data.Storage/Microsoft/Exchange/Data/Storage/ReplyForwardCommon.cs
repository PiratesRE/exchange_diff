using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class ReplyForwardCommon
	{
		internal ReplyForwardCommon(Item originalItem, Item newItem, ReplyForwardConfiguration parameters, bool decodeSmime)
		{
			Util.ThrowOnNullArgument(parameters, "parameters");
			if (decodeSmime && ObjectClass.IsSmime(originalItem.ClassName))
			{
				if (parameters.ConversionOptionsForSmime == null || parameters.ConversionOptionsForSmime.IgnoreImceaDomain || parameters.ConversionOptionsForSmime.ImceaEncapsulationDomain == null)
				{
					throw new InvalidOperationException("Cannot decode SMIME without valid ConversionOptionsForSmime");
				}
				this.originalItem = originalItem;
				MessageItem messageItem = originalItem as MessageItem;
				if (messageItem != null)
				{
					Item item = messageItem.FetchSmimeContent(parameters.ConversionOptionsForSmime.ImceaEncapsulationDomain);
					if (item != null)
					{
						this.originalItem = item;
						this.originalItem[InternalSchema.NormalizedSubjectInternal] = messageItem.GetValueOrDefault<string>(InternalSchema.NormalizedSubjectInternal, string.Empty);
						this.originalItem[InternalSchema.Sender] = messageItem.Sender;
						this.originalItem[InternalSchema.From] = messageItem.From;
					}
				}
			}
			else
			{
				this.originalItem = originalItem;
			}
			this.newItem = newItem;
			this.parameters = parameters;
			this.culture = ReplyForwardUtils.CalculateReplyForwardCulture(parameters.Culture, newItem);
			if (this.culture == null)
			{
				throw new InvalidOperationException("Forward message culture is unknown");
			}
			this.FetchPropertiesFromOriginalItem();
			if (originalItem is MessageItem)
			{
				if (originalItem.MapiMessage != null)
				{
					SetReadFlags readFlag = parameters.ShouldSuppressReadReceipt ? SetReadFlags.SuppressReceipt : SetReadFlags.None;
					try
					{
						StoreSession session = originalItem.Session;
						bool flag = false;
						try
						{
							if (session != null)
							{
								session.BeginMapiCall();
								session.BeginServerHealthCall();
								flag = true;
							}
							if (StorageGlobals.MapiTestHookBeforeCall != null)
							{
								StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
							}
							originalItem.MapiMessage.SetReadFlag(readFlag);
						}
						catch (MapiPermanentException ex)
						{
							throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSetReadFlags, ex, session, this, "{0}. MapiException = {1}.", new object[]
							{
								string.Format("ReplyForwardCommon::ctor.", new object[0]),
								ex
							});
						}
						catch (MapiRetryableException ex2)
						{
							throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSetReadFlags, ex2, session, this, "{0}. MapiException = {1}.", new object[]
							{
								string.Format("ReplyForwardCommon::ctor.", new object[0]),
								ex2
							});
						}
						finally
						{
							try
							{
								if (session != null)
								{
									session.EndMapiCall();
									if (flag)
									{
										session.EndServerHealthCall();
									}
								}
							}
							finally
							{
								if (StorageGlobals.MapiTestHookAfterCall != null)
								{
									StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
								}
							}
						}
					}
					catch (StoragePermanentException)
					{
					}
					catch (StorageTransientException)
					{
					}
				}
				(this.originalItem as MessageItem).IsRead = true;
			}
			else if (this.originalItem is CalendarItemOccurrence)
			{
				this.parentPropValues[ParentPropertyIndex.IsRecurring] = true;
				this.parentPropValues[ParentPropertyIndex.AppointmentRecurring] = false;
				this.parentPropValues[ParentPropertyIndex.RecurrenceType] = 0;
				this.parentPropValues[ParentPropertyIndex.TimeZoneBlob] = new PropertyError(InternalSchema.TimeZoneBlob, PropertyErrorCode.NotFound);
				this.parentPropValues[ParentPropertyIndex.AppointmentRecurrenceBlob] = new PropertyError(InternalSchema.AppointmentRecurrenceBlob, PropertyErrorCode.NotFound);
				this.parentPropValues[ParentPropertyIndex.IsException] = true;
				if (string.IsNullOrEmpty(this.parentPropValues[ParentPropertyIndex.RecurrencePattern] as string))
				{
					CalendarItemOccurrence calendarItemOccurrence = this.originalItem as CalendarItemOccurrence;
					this.parentPropValues[ParentPropertyIndex.RecurrencePattern] = calendarItemOccurrence.OccurrencePropertyBag.MasterCalendarItem.GenerateWhen();
				}
			}
			string valueOrDefault = originalItem.GetValueOrDefault<string>(InternalSchema.ItemClass, string.Empty);
			this.originalItemSigned = (ObjectClass.IsOfClass(valueOrDefault, "IPM.Note.Secure.Sign") || ObjectClass.IsSmimeClearSigned(valueOrDefault));
			if (!this.originalItemSigned)
			{
				this.originalItemEncrypted = (ObjectClass.IsOfClass(valueOrDefault, "IPM.Note.Secure") || ObjectClass.IsSmime(valueOrDefault));
			}
			string valueOrDefault2 = originalItem.GetValueOrDefault<string>(InternalSchema.ContentClass, string.Empty);
			this.originalItemIrm = ObjectClass.IsRightsManagedContentClass(valueOrDefault2);
		}

		protected CultureInfo Culture
		{
			get
			{
				return this.culture;
			}
		}

		protected BodyFormat Format
		{
			get
			{
				return this.parameters.TargetFormat;
			}
		}

		protected string BodyPrefix
		{
			get
			{
				return this.parameters.BodyPrefix;
			}
		}

		protected bool IsResourceDelegationMessage
		{
			get
			{
				return ForwardCreationFlags.ResourceDelegationMessage == (this.parameters.ForwardCreationFlags & ForwardCreationFlags.ResourceDelegationMessage);
			}
		}

		protected bool TreatAsMeetingMessage
		{
			get
			{
				return ForwardCreationFlags.TreatAsMeetingMessage == (this.parameters.ForwardCreationFlags & ForwardCreationFlags.TreatAsMeetingMessage);
			}
		}

		internal static HeaderFooterFormat GetSupportedPrefixFormat(BodyFormat format)
		{
			if (format != BodyFormat.TextPlain)
			{
				return HeaderFooterFormat.Html;
			}
			return HeaderFooterFormat.Text;
		}

		internal static void CopyBodyWithPrefix(Body sourceBody, Body targetBody, ReplyForwardConfiguration configuration, BodyConversionCallbacks callbacks)
		{
			ReplyForwardCommon.CheckRtf(sourceBody.RawFormat, targetBody.RawFormat);
			BodyReadConfiguration configuration2 = new BodyReadConfiguration(sourceBody.RawFormat, sourceBody.RawCharset.Name);
			BodyWriteConfiguration bodyWriteConfiguration = new BodyWriteConfiguration(sourceBody.RawFormat, sourceBody.RawCharset.Name);
			bodyWriteConfiguration.SetTargetFormat(configuration.TargetFormat, sourceBody.Charset);
			if (!string.IsNullOrEmpty(configuration.BodyPrefix))
			{
				bodyWriteConfiguration.AddInjectedText(configuration.BodyPrefix, null, configuration.BodyPrefixFormat);
			}
			if (callbacks.HtmlCallback != null || callbacks.RtfCallback != null)
			{
				bodyWriteConfiguration.HtmlCallback = callbacks.HtmlCallback;
				bodyWriteConfiguration.RtfCallback = callbacks.RtfCallback;
				if (!configuration.ShouldSkipFilterHtmlOnBodyWrite)
				{
					bodyWriteConfiguration.HtmlFlags = HtmlStreamingFlags.FilterHtml;
				}
			}
			using (Stream stream = sourceBody.OpenReadStream(configuration2))
			{
				using (Stream stream2 = targetBody.OpenWriteStream(bodyWriteConfiguration))
				{
					Util.StreamHandler.CopyStreamData(stream, stream2);
				}
			}
		}

		private static bool ParticipantIsSender(StoreSession session, Participant participant)
		{
			if (session is MailboxSession)
			{
				if (Participant.HasSameEmail(participant, new Participant((session as MailboxSession).MailboxOwner)))
				{
					return true;
				}
			}
			else if (session is PublicFolderSession && Participant.HasSameEmail(participant, new Participant((session as PublicFolderSession).MailboxPrincipal)))
			{
				return true;
			}
			return false;
		}

		internal static void BuildReplyRecipientsFromMessage(MessageItem message, MessageItem sourceMessage, bool isReplyAll, bool shouldUseSender, bool useReplyTo)
		{
			if (!useReplyTo || sourceMessage.ReplyTo == null || sourceMessage.ReplyTo.Count == 0)
			{
				if (shouldUseSender && sourceMessage.Sender != null)
				{
					if (!isReplyAll || !ReplyForwardCommon.ParticipantIsSender(message.Session, sourceMessage.Sender))
					{
						message.Recipients.Add(sourceMessage.Sender, RecipientItemType.To);
					}
				}
				else if (sourceMessage.From != null && (!isReplyAll || !ReplyForwardCommon.ParticipantIsSender(message.Session, sourceMessage.From)))
				{
					message.Recipients.Add(sourceMessage.From, RecipientItemType.To);
				}
			}
			else
			{
				foreach (Participant participant in sourceMessage.ReplyTo)
				{
					if (!isReplyAll || !ReplyForwardCommon.ParticipantIsSender(message.Session, participant))
					{
						message.Recipients.Add(participant, RecipientItemType.To);
					}
				}
			}
			if (!isReplyAll)
			{
				return;
			}
			bool flag = message.Recipients.Count == 0;
			foreach (Recipient recipient in sourceMessage.Recipients)
			{
				if (!message.Recipients.Contains(recipient.Participant) && (!ReplyForwardCommon.ParticipantIsSender(message.Session, recipient.Participant) || (flag && sourceMessage.Recipients.Count <= 1)) && recipient.RecipientItemType != RecipientItemType.Unknown && recipient.RecipientItemType != RecipientItemType.Bcc)
				{
					message.Recipients.Add(recipient.Participant, recipient.RecipientItemType);
				}
			}
		}

		internal static void UpdateXLoop(Item original, Item newItem, string newXLoop)
		{
			if (!string.IsNullOrEmpty(newXLoop))
			{
				string[] valueOrDefault = original.GetValueOrDefault<string[]>(InternalSchema.XLoop, null);
				string[] array;
				if (valueOrDefault == null)
				{
					array = new string[]
					{
						newXLoop
					};
				}
				else
				{
					array = new string[valueOrDefault.Length + 1];
					valueOrDefault.CopyTo(array, 0);
					array[array.Length - 1] = newXLoop;
				}
				newItem[InternalSchema.XLoop] = array;
			}
		}

		internal void PopulateProperties()
		{
			this.PopulateProperties(true);
		}

		internal void PopulateProperties(bool populateContents)
		{
			this.UpdateNewItemProperties();
			this.BuildSubject();
			if (populateContents)
			{
				this.PopulateContents();
			}
		}

		internal void PopulateContents()
		{
			BodyConversionCallbacks callbacks = this.GetCallbacks();
			this.BuildBody(callbacks);
			this.BuildAttachments(callbacks, this.parameters.ConversionOptionsForSmime);
		}

		protected static void CheckRtf(BodyFormat sourceFormat, BodyFormat targetFormat)
		{
			if (targetFormat == BodyFormat.ApplicationRtf && sourceFormat == BodyFormat.TextPlain)
			{
				throw new InvalidOperationException(ServerStrings.ExBodyFormatConversionNotSupported(sourceFormat.ToString() + "->" + targetFormat.ToString()));
			}
		}

		protected abstract void BuildSubject();

		protected abstract void BuildAttachments(BodyConversionCallbacks callbacks, InboundConversionOptions optionsForSmime);

		protected virtual BodyConversionCallbacks GetCallbacks()
		{
			return this.GetCallbacksInternal(this.originalItem.Body, this.originalItem.AttachmentCollection);
		}

		protected virtual void BuildBody(BodyConversionCallbacks callbacks)
		{
			ReplyForwardCommon.CopyBodyWithPrefix(this.originalItem.Body, this.newItem.Body, this.parameters, callbacks);
		}

		protected void CopyAttachments(BodyConversionCallbacks callbacks, AttachmentCollection sourceCollection, AttachmentCollection targetCollection, bool copyInlinesOnly, bool targetIsPlainText, InboundConversionOptions optionsForSmime)
		{
			ReadOnlyCollection<AttachmentLink> readOnlyCollection = null;
			bool flag = true;
			if (callbacks.HtmlCallback != null && callbacks.HtmlCallback.AttachmentListInitialized)
			{
				readOnlyCollection = callbacks.HtmlCallback.AttachmentLinks;
				flag = (flag && callbacks.HtmlCallback.ClearInlineOnUnmarkedAttachments);
			}
			if (callbacks.RtfCallback != null && callbacks.RtfCallback.AttachmentListInitialized)
			{
				readOnlyCollection = callbacks.RtfCallback.AttachmentLinks;
				flag = (flag && callbacks.RtfCallback.ClearInlineOnUnmarkedAttachments);
			}
			if (readOnlyCollection == null)
			{
				if (copyInlinesOnly)
				{
					return;
				}
				using (IEnumerator<AttachmentHandle> enumerator = sourceCollection.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						AttachmentHandle handle = enumerator.Current;
						using (Attachment attachment = sourceCollection.Open(handle, null))
						{
							if (!attachment.IsCalendarException)
							{
								using (Attachment attachment2 = attachment.CreateCopy(targetCollection, new BodyFormat?(this.parameters.TargetFormat)))
								{
									attachment2.IsInline = false;
									attachment2.Save();
								}
							}
						}
					}
					return;
				}
			}
			foreach (AttachmentLink attachmentLink in readOnlyCollection)
			{
				if (!copyInlinesOnly || attachmentLink.IsInline(flag))
				{
					using (Attachment attachment3 = sourceCollection.Open(attachmentLink.AttachmentId, null))
					{
						if (!copyInlinesOnly || !(attachment3 is ReferenceAttachment))
						{
							using (Attachment attachment4 = attachment3.CreateCopy(targetCollection, new BodyFormat?(this.parameters.TargetFormat)))
							{
								attachmentLink.MakeAttachmentChanges(attachment4, flag);
								if (targetIsPlainText)
								{
									attachment4.IsInline = false;
								}
								attachment4.Save();
							}
						}
					}
				}
			}
		}

		protected virtual void UpdateNewItemProperties()
		{
			IExchangePrincipal exchangePrincipal = (this.newItem.Session == null) ? null : this.newItem.Session.MailboxOwner;
			MailboxSession mailboxSession = this.newItem.Session as MailboxSession;
			if (mailboxSession != null && exchangePrincipal != null)
			{
				PostItem postItem = this.newItem as PostItem;
				if (postItem != null)
				{
					postItem.Sender = new Participant(exchangePrincipal);
				}
				else
				{
					((MessageItem)this.newItem).Sender = new Participant(exchangePrincipal);
				}
			}
			string text = null;
			string valueOrDefault = this.originalItem.GetValueOrDefault<string>(InternalSchema.InternetMessageId);
			if (!string.IsNullOrEmpty(valueOrDefault))
			{
				text = valueOrDefault;
			}
			string text2 = null;
			valueOrDefault = this.originalItem.GetValueOrDefault<string>(InternalSchema.InReplyTo);
			if (!string.IsNullOrEmpty(valueOrDefault))
			{
				text2 = valueOrDefault;
			}
			this.newItem.SafeSetProperty(InternalSchema.InReplyTo, text);
			StringBuilder stringBuilder = new StringBuilder(128);
			valueOrDefault = this.originalItem.GetValueOrDefault<string>(InternalSchema.InternetReferences);
			if (!string.IsNullOrEmpty(valueOrDefault))
			{
				stringBuilder.Append(valueOrDefault);
			}
			else if (text2 != null)
			{
				stringBuilder.Append(text2);
			}
			if (text != null)
			{
				if (stringBuilder.Length + 1 + text.Length > 32768 || stringBuilder.Length == 0)
				{
					stringBuilder.Clear();
					stringBuilder.Append(text);
				}
				else
				{
					stringBuilder.Append(',');
					stringBuilder.Append(text);
				}
			}
			this.newItem.SafeSetProperty(InternalSchema.InternetReferences, stringBuilder.ToString());
			this.newItem.SafeSetProperty(InternalSchema.Categories, StoreObject.SafePropertyValue(this.originalItem.TryGetProperty(InternalSchema.Categories), typeof(string[]), null));
			byte[] parentBytes = this.originalItem.TryGetProperty(InternalSchema.ConversationIndex) as byte[];
			ConversationIndex conversationIndex = ConversationIndex.CreateFromParent(parentBytes);
			this.newItem.SafeSetProperty(InternalSchema.ConversationIndex, conversationIndex.ToByteArray());
			ConversationId valueOrDefault2 = this.originalItem.GetValueOrDefault<ConversationId>(InternalSchema.ConversationFamilyId);
			this.newItem.SafeSetProperty(InternalSchema.ConversationFamilyId, valueOrDefault2);
			object propValue = this.originalItem.TryGetProperty(InternalSchema.SupportsSideConversation);
			this.newItem.SafeSetProperty(InternalSchema.SupportsSideConversation, propValue);
			ConversationCreatorSidCalculatorFactory conversationCreatorSidCalculatorFactory = new ConversationCreatorSidCalculatorFactory(XSOFactory.Default);
			IConversationCreatorSidCalculator conversationCreatorSidCalculator;
			byte[] propValue2;
			if (conversationCreatorSidCalculatorFactory.TryCreate(mailboxSession, exchangePrincipal, out conversationCreatorSidCalculator) && conversationCreatorSidCalculator.TryCalculateOnReply(conversationIndex, out propValue2))
			{
				this.newItem.SafeSetProperty(ItemSchema.ConversationCreatorSID, propValue2);
			}
			bool? valueAsNullable = this.originalItem.GetValueAsNullable<bool>(InternalSchema.ConversationIndexTracking);
			if (valueAsNullable != null && valueAsNullable.Value)
			{
				this.newItem.SafeSetProperty(InternalSchema.ConversationIndexTracking, true);
			}
			this.newItem.SafeSetProperty(InternalSchema.ConversationTopic, StoreObject.SafePropertyValue(this.originalItem.TryGetProperty(InternalSchema.ConversationTopic), typeof(string), StoreObject.SafePropertyValue(this.originalItem.TryGetProperty(InternalSchema.NormalizedSubjectInternal), typeof(string), null)));
			this.newItem.SafeSetProperty(InternalSchema.Sensitivity, StoreObject.SafePropertyValue(this.originalItem.TryGetProperty(InternalSchema.Sensitivity), typeof(Sensitivity), Sensitivity.Normal));
			this.newItem.SafeSetProperty(InternalSchema.OriginalSensitivity, StoreObject.SafePropertyValue(this.originalItem.TryGetProperty(InternalSchema.OriginalSensitivity), typeof(Sensitivity), Sensitivity.Normal));
			this.newItem.SafeSetProperty(InternalSchema.IsReadReceiptRequested, false);
			this.newItem.SafeSetProperty(InternalSchema.IsDeliveryReceiptRequested, false);
			this.newItem.SafeSetProperty(InternalSchema.IsReplyRequested, StoreObject.SafePropertyValue(this.originalItem.TryGetProperty(InternalSchema.IsReplyRequested), typeof(bool), null));
			this.newItem.SafeSetProperty(InternalSchema.IsResponseRequested, StoreObject.SafePropertyValue(this.originalItem.TryGetProperty(InternalSchema.IsResponseRequested), typeof(bool), null));
			this.newItem.SafeSetProperty(InternalSchema.NativeBlockStatus, StoreObject.SafePropertyValue(this.originalItem.TryGetProperty(InternalSchema.NativeBlockStatus), typeof(int), null));
			object obj = this.originalItem.TryGetProperty(InternalSchema.IconIndex);
			if (!PropertyError.IsPropertyError(obj))
			{
				IconIndex iconIndex = (IconIndex)obj;
				if (iconIndex != IconIndex.Default)
				{
					iconIndex = ReplyForwardCommon.GetIconIndexForNewItem(iconIndex);
					this.newItem.SafeSetProperty(InternalSchema.IconIndex, iconIndex);
				}
			}
			ReplyForwardCommon.UpdateXLoop(this.originalItem, this.newItem, this.parameters.XLoop);
		}

		protected BodyConversionCallbacks GetCallbacksInternal(Body defaultItemBody, AttachmentCollection defaultAttachmentCollection)
		{
			BodyConversionCallbacks result = default(BodyConversionCallbacks);
			if (this.parameters.HtmlCallbacks != null)
			{
				result.HtmlCallback = this.parameters.HtmlCallbacks;
			}
			else
			{
				DefaultHtmlCallbacks defaultHtmlCallbacks = new DefaultHtmlCallbacks(defaultAttachmentCollection.CoreAttachmentCollection, defaultItemBody, true);
				if (this.parameters.ConversionOptionsForSmime != null && !this.parameters.ConversionOptionsForSmime.IgnoreImceaDomain && this.parameters.ConversionOptionsForSmime.ImceaEncapsulationDomain != null)
				{
					defaultHtmlCallbacks.SetContentIdDomain(this.parameters.ConversionOptionsForSmime.ImceaEncapsulationDomain);
				}
				defaultHtmlCallbacks.ClearingEmptyLinks = true;
				defaultHtmlCallbacks.RemoveLinksToNonImageAttachments = true;
				result.HtmlCallback = defaultHtmlCallbacks;
			}
			if (this.parameters.RtfCallbacks != null)
			{
				result.RtfCallback = this.parameters.RtfCallbacks;
			}
			else if (this.parameters.TargetFormat == BodyFormat.ApplicationRtf && this.originalItem.Body.Format == BodyFormat.TextHtml)
			{
				result.RtfCallback = new DefaultRtfCallbacks(defaultAttachmentCollection.CoreAttachmentCollection, defaultItemBody, true);
			}
			return result;
		}

		private static IconIndex GetIconIndexForNewItem(IconIndex iconIndex)
		{
			if (iconIndex <= IconIndex.BaseMail)
			{
				if (iconIndex != IconIndex.PostItem && iconIndex != IconIndex.BaseMail)
				{
					return iconIndex;
				}
			}
			else
			{
				switch (iconIndex)
				{
				case IconIndex.MailReplied:
				case IconIndex.MailForwarded:
					break;
				default:
					switch (iconIndex)
					{
					case IconIndex.MailEncryptedReplied:
					case IconIndex.MailEncryptedForwarded:
					case IconIndex.MailEncryptedRead:
						return IconIndex.MailEncrypted;
					case IconIndex.MailSmimeSignedReplied:
					case IconIndex.MailSmimeSignedForwarded:
					case IconIndex.MailSmimeSignedRead:
						return IconIndex.MailSmimeSigned;
					default:
						switch (iconIndex)
						{
						case IconIndex.MailIrmForwarded:
						case IconIndex.MailIrmReplied:
							return IconIndex.MailIrm;
						default:
							return iconIndex;
						}
						break;
					}
					break;
				}
			}
			return IconIndex.Default;
		}

		private void FetchPropertiesFromOriginalItem()
		{
			this.parentPropDefinitions = null;
			if (this.TreatAsMeetingMessage || this.originalItem is MeetingRequest || this.originalItem is MeetingCancellation || this.originalItem is CalendarItemBase)
			{
				this.parentPropDefinitions = CalendarItemProperties.MeetingReplyForwardProperties;
				this.originalItem.Load(this.parentPropDefinitions);
				this.parentPropValues = this.originalItem.GetProperties(this.parentPropDefinitions);
			}
		}

		internal const int MaxStringPropValueSize = 32768;

		internal const char NewLine = '\n';

		internal const int LastActionIndex = 0;

		internal const int LastIconIndexIndex = 1;

		internal const int ParentItemIdBase64Index = 2;

		internal const int MessageStatusCount = 3;

		protected Item newItem;

		protected Item originalItem;

		protected PropertyDefinition[] parentPropDefinitions;

		protected object[] parentPropValues;

		protected bool originalItemSigned;

		protected bool originalItemEncrypted;

		protected bool originalItemIrm;

		protected ReplyForwardConfiguration parameters;

		private CultureInfo culture;
	}
}
