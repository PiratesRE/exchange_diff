using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.ContentTypes.Tnef;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ItemToTnefConverter : IDisposeTrackable, IDisposable
	{
		internal ItemToTnefConverter(Item itemIn, OutboundAddressCache addressCache, Stream mimeOut, OutboundConversionOptions options, ConversionLimitsTracker limits, TnefType tnefType, string tnefCorrelationKey, bool parsingEmbeddedItem) : this(itemIn, addressCache, options, limits, tnefType, parsingEmbeddedItem)
		{
			this.tnefCorrelationKey = tnefCorrelationKey;
			Charset itemWindowsCharset = ConvertUtils.GetItemWindowsCharset(this.item, options);
			this.tnefWriter = new ItemToTnefConverter.TnefContentWriter(mimeOut, itemWindowsCharset);
			this.propertyChecker = new TnefPropertyChecker(tnefType, parsingEmbeddedItem, options);
			this.disposeTracker = this.GetDisposeTracker();
		}

		internal ItemToTnefConverter(Item itemIn, OutboundAddressCache addressCache, ItemToTnefConverter.TnefContentWriter writer, OutboundConversionOptions options, ConversionLimitsTracker limits, TnefType tnefType, bool parsingEmbeddedItem) : this(itemIn, addressCache, options, limits, tnefType, parsingEmbeddedItem)
		{
			this.tnefWriter = writer;
			this.tnefCorrelationKey = null;
			this.propertyChecker = new TnefPropertyChecker(tnefType, parsingEmbeddedItem, options);
			this.disposeTracker = this.GetDisposeTracker();
		}

		private ItemToTnefConverter(Item itemIn, OutboundAddressCache addressCache, OutboundConversionOptions options, ConversionLimitsTracker limits, TnefType tnefType, bool parsingEmbeddedItem)
		{
			if (options.FilterAttachmentHandler != null)
			{
				throw new NotSupportedException("FilterAttachmentHandler is not supported in ItemToTnefConverter");
			}
			if (options.FilterBodyHandler != null)
			{
				throw new NotSupportedException("FilterBodyHandler is not supported in ItemToTnefConverter");
			}
			this.item = itemIn;
			this.addressCache = addressCache;
			this.options = options;
			this.limitsTracker = limits;
			this.isEmbeddedItem = parsingEmbeddedItem;
			this.tnefType = tnefType;
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ItemToTnefConverter>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal ConversionResult Convert()
		{
			ExTimeZone exTimeZone = this.item.PropertyBag.ExTimeZone;
			this.item.PropertyBag.ExTimeZone = ExTimeZone.UtcTimeZone;
			this.conversionResult = new ConversionResult();
			try
			{
				this.WriteMessageAttributes();
				this.WriteRecipientTable();
				this.WriteMapiProperties();
				this.WriteAttachments();
				this.tnefWriter.Flush();
			}
			finally
			{
				this.item.PropertyBag.ExTimeZone = exTimeZone;
			}
			return this.conversionResult;
		}

		private static bool IsCompleteParticipant(Participant participant)
		{
			return participant != null && participant.ValidationStatus == ParticipantValidationStatus.NoError && participant.RoutingType != null && participant.EmailAddress != null;
		}

		private static HashSet<NativeStorePropertyDefinition> CreateExcludedPropertiesSet()
		{
			return new HashSet<NativeStorePropertyDefinition>
			{
				InternalSchema.HtmlBody,
				InternalSchema.TextBody,
				InternalSchema.RtfBody,
				InternalSchema.RtfSyncBodyCrc,
				InternalSchema.RtfSyncBodyCount,
				InternalSchema.RtfSyncBodyTag,
				InternalSchema.RtfSyncPrefixCount,
				InternalSchema.RtfSyncTrailingCount,
				InternalSchema.RtfInSync,
				InternalSchema.TnefCorrelationKey,
				InternalSchema.PredictedActionsInternal,
				InternalSchema.GroupingActionsDeprecated,
				InternalSchema.PredictedActionsSummaryDeprecated,
				InternalSchema.NeedGroupExpansion
			};
		}

		private static HashSet<NativeStorePropertyDefinition> CreateBodyPropertiesSet()
		{
			return new HashSet<NativeStorePropertyDefinition>
			{
				InternalSchema.HtmlBody,
				InternalSchema.RtfBody
			};
		}

		private void WriteMessageAttribute(TnefAttributeTag attribute, TnefPropertyTag tnefProperty, StorePropertyDefinition property)
		{
			object obj = this.item.TryGetProperty(property);
			PropertyError propertyError = obj as PropertyError;
			if (propertyError == null)
			{
				if (attribute == TnefAttributeTag.OriginalMessageClass || attribute == TnefAttributeTag.MessageClass)
				{
					string value = obj as string;
					if (string.IsNullOrEmpty(value))
					{
						return;
					}
				}
				this.tnefWriter.StartAttribute(attribute, TnefAttributeLevel.Message);
				this.tnefWriter.WriteProperty(tnefProperty, obj);
				return;
			}
			if (PropertyError.IsPropertyValueTooBig(propertyError))
			{
				try
				{
					using (Stream stream = this.item.OpenPropertyStream(property, PropertyOpenMode.ReadOnly))
					{
						using (Stream stream2 = this.tnefWriter.OpenAttributeStream(attribute, TnefAttributeLevel.Message))
						{
							Util.StreamHandler.CopyStreamData(stream, stream2);
						}
					}
				}
				catch (ObjectNotFoundException)
				{
				}
			}
		}

		private void WriteMessageAttributes()
		{
			this.WriteMessageAttribute(TnefAttributeTag.MessageId, TnefPropertyTag.SearchKey, InternalSchema.SearchKey);
			this.WriteMessageAttribute(TnefAttributeTag.Priority, TnefPropertyTag.Importance, InternalSchema.Importance);
			this.WriteMessageAttribute(TnefAttributeTag.DateSent, TnefPropertyTag.ClientSubmitTime, InternalSchema.SentTime);
			this.WriteMessageAttribute(TnefAttributeTag.DateModified, TnefPropertyTag.LastModificationTime, InternalSchema.LastModifiedTime);
			if (this.isEmbeddedItem)
			{
				this.WriteMessageAttribute(TnefAttributeTag.DateReceived, TnefPropertyTag.MessageDeliveryTime, InternalSchema.ReceivedTime);
				this.WriteMessageAttribute(TnefAttributeTag.MessageStatus, TnefPropertyTag.MessageFlags, InternalSchema.Flags);
			}
			if (this.tnefType == TnefType.LegacyTnef)
			{
				this.WriteMessageAttribute(TnefAttributeTag.MessageClass, TnefPropertyTag.MessageClassW, InternalSchema.ItemClass);
				this.WriteMessageAttribute(TnefAttributeTag.OriginalMessageClass, TnefPropertyTag.OrigMessageClassW, InternalSchema.OrigMessageClass);
				this.WriteMessageAttribute(TnefAttributeTag.Subject, TnefPropertyTag.SubjectA, InternalSchema.Subject);
				this.WriteMessageAttribute(TnefAttributeTag.ConversationId, TnefPropertyTag.ConversationKey, InternalSchema.ConversationKey);
				this.WriteMessageAttribute(TnefAttributeTag.DateStart, TnefPropertyTag.StartDate, InternalSchema.StartTime);
				this.WriteMessageAttribute(TnefAttributeTag.DateEnd, TnefPropertyTag.EndDate, InternalSchema.EndTime);
				this.WriteMessageAttribute(TnefAttributeTag.AidOwner, TnefPropertyTag.OwnerApptId, InternalSchema.OwnerAppointmentID);
				this.WriteMessageAttribute(TnefAttributeTag.RequestResponse, TnefPropertyTag.ResponseRequested, InternalSchema.IsResponseRequested);
				if (this.isEmbeddedItem)
				{
					this.WriteMessageAttribute(TnefAttributeTag.ParentId, TnefPropertyTag.ParentEntryId, InternalSchema.ParentEntryId);
					this.WriteTnefParticipant(TnefAttributeTag.From, ConversionItemParticipants.ParticipantIndex.Sender);
					this.WriteTnefParticipant(TnefAttributeTag.SentFor, ConversionItemParticipants.ParticipantIndex.From);
					this.WriteAttOwnerTnef();
				}
			}
		}

		private void WriteAttOwnerTnef()
		{
			if (this.addressCache.Participants[ConversionItemParticipants.ParticipantIndex.ReceivedRepresenting] == null)
			{
				this.WriteTnefParticipant(TnefAttributeTag.Owner, ConversionItemParticipants.ParticipantIndex.From);
				return;
			}
			this.WriteTnefParticipant(TnefAttributeTag.Owner, ConversionItemParticipants.ParticipantIndex.ReceivedRepresenting);
		}

		private void WriteAttachmentAttributes(Attachment attachment)
		{
			this.tnefWriter.StartAttribute(TnefAttributeTag.AttachRenderData, TnefAttributeLevel.Attachment);
			object obj = attachment.TryGetProperty(InternalSchema.AttachMethod);
			if (obj is PropertyError)
			{
				StorageGlobals.ContextTraceError<string>(ExTraceGlobals.CcOutboundTnefTracer, "ItemToTnefConverter::WriteAttachmentAttributes: unable to get attach method, {0}.", ((PropertyError)obj).PropertyErrorDescription);
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent);
			}
			this.tnefWriter.WriteProperty(TnefPropertyTag.AttachMethod, (int)obj);
			obj = attachment.TryGetProperty(InternalSchema.RenderingPosition);
			if (obj is PropertyError)
			{
				obj = -1;
			}
			this.tnefWriter.WriteProperty(TnefPropertyTag.RenderingPosition, (int)obj);
			obj = attachment.TryGetProperty(InternalSchema.AttachEncoding);
			if (!(obj is PropertyError))
			{
				this.tnefWriter.WriteProperty(TnefPropertyTag.AttachEncoding, (byte[])obj);
			}
			this.tnefWriter.StartAttribute(TnefAttributeTag.AttachTitle, TnefAttributeLevel.Attachment);
			this.tnefWriter.WriteProperty(TnefPropertyTag.AttachFilenameA, attachment.DisplayName ?? string.Empty);
			this.tnefWriter.StartAttribute(TnefAttributeTag.AttachCreateDate, TnefAttributeLevel.Attachment);
			this.tnefWriter.WriteProperty(TnefPropertyTag.CreationTime, attachment.CreationTime);
			this.tnefWriter.StartAttribute(TnefAttributeTag.AttachModifyDate, TnefAttributeLevel.Attachment);
			this.tnefWriter.WriteProperty(TnefPropertyTag.LastModificationTime, attachment.LastModifiedTime);
			object obj2 = attachment.TryGetProperty(InternalSchema.AttachRendering);
			if (!(obj2 is PropertyError))
			{
				this.tnefWriter.StartAttribute(TnefAttributeTag.AttachMetaFile, TnefAttributeLevel.Attachment);
				this.tnefWriter.WriteProperty(TnefPropertyTag.AttachRendering, obj2);
			}
		}

		private bool WriteMessageProperty(NativeStorePropertyDefinition property)
		{
			long num;
			return this.WriteMessageProperty(property, out num);
		}

		private bool WriteMessageProperty(NativeStorePropertyDefinition property, out long totalBytesRead)
		{
			totalBytesRead = 0L;
			object obj = this.item.PropertyBag.TryGetProperty(property);
			PropertyError propertyError = obj as PropertyError;
			if (propertyError == null)
			{
				this.tnefWriter.WriteProperty(property, obj);
				if (ItemToTnefConverter.bodyProperties.Contains(property))
				{
					byte[] array = obj as byte[];
					if (array != null)
					{
						totalBytesRead = (long)array.Length;
					}
				}
				return true;
			}
			return PropertyError.IsPropertyValueTooBig(propertyError) && this.WritePropertyStreamData(this.item.PropertyBag, property, out totalBytesRead);
		}

		private bool WritePropertyStreamData(PersistablePropertyBag propertyBag, NativeStorePropertyDefinition property)
		{
			long num;
			return this.WritePropertyStreamData(propertyBag, property, out num);
		}

		private bool WritePropertyStreamData(PersistablePropertyBag propertyBag, NativeStorePropertyDefinition property, out long totalBytesRead)
		{
			totalBytesRead = 0L;
			try
			{
				using (Stream stream = propertyBag.OpenPropertyStream(property, PropertyOpenMode.ReadOnly))
				{
					using (Stream stream2 = this.tnefWriter.StartStreamProperty(property))
					{
						totalBytesRead = Util.StreamHandler.CopyStreamData(stream, stream2);
					}
				}
			}
			catch (ObjectNotFoundException)
			{
			}
			return false;
		}

		private void WriteMessageBody()
		{
			this.limitsTracker.CountMessageBody();
			BodyFormat rawFormat = this.item.Body.RawFormat;
			long bodySize = 0L;
			if (rawFormat == BodyFormat.TextHtml)
			{
				this.WriteMessageProperty(InternalSchema.HtmlBody, out bodySize);
				this.conversionResult.BodySize = bodySize;
				return;
			}
			this.WriteMessageProperty(InternalSchema.RtfSyncBodyCrc);
			this.WriteMessageProperty(InternalSchema.RtfSyncBodyCount);
			this.WriteMessageProperty(InternalSchema.RtfSyncBodyTag);
			this.WriteMessageProperty(InternalSchema.RtfInSync);
			this.WriteMessageProperty(InternalSchema.RtfSyncPrefixCount);
			this.WriteMessageProperty(InternalSchema.RtfSyncTrailingCount);
			if (rawFormat == BodyFormat.ApplicationRtf)
			{
				this.WriteMessageProperty(InternalSchema.RtfBody, out bodySize);
				this.conversionResult.BodySize = bodySize;
				return;
			}
			if (rawFormat == BodyFormat.TextPlain)
			{
				if (!this.item.Body.IsBodyDefined)
				{
					return;
				}
				using (Stream textStream = this.item.OpenPropertyStream(InternalSchema.TextBody, PropertyOpenMode.ReadOnly))
				{
					if (this.tnefType == TnefType.SummaryTnef)
					{
						using (Stream stream = this.tnefWriter.StartStreamProperty(InternalSchema.TextBody))
						{
							this.conversionResult.BodySize = Util.StreamHandler.CopyStreamData(textStream, stream);
						}
						textStream.Position = 0L;
					}
					using (Stream tnefStream = this.tnefWriter.StartStreamProperty(InternalSchema.RtfBody))
					{
						int inCodePage = this.item.Body.RawCharset.CodePage;
						ConvertUtils.CallCts(ExTraceGlobals.CcOutboundTnefTracer, "ItemToTnefConverter::WriteMessageBody", ServerStrings.ConversionBodyConversionFailed, delegate
						{
							TextToRtf textToRtf = new TextToRtf();
							textToRtf.InputEncoding = Charset.GetEncoding(inCodePage);
							using (Stream stream2 = new ConverterStream(textStream, textToRtf, ConverterStreamAccess.Read))
							{
								using (ConverterStream converterStream = new ConverterStream(stream2, new RtfToRtfCompressed(), ConverterStreamAccess.Read))
								{
									Util.StreamHandler.CopyStreamData(converterStream, tnefStream);
								}
							}
						});
					}
				}
			}
		}

		private void WriteTnefParticipant(TnefAttributeTag legacyTnefAttribute, ConversionItemParticipants.ParticipantIndex index)
		{
			Participant participant = this.addressCache.Participants[index];
			if (ItemToTnefConverter.IsCompleteParticipant(participant))
			{
				this.tnefWriter.StartAttribute(legacyTnefAttribute, TnefAttributeLevel.Message);
				EmbeddedParticipantProperty embeddedParticipantProperty = ConversionItemParticipants.GetEmbeddedParticipantProperty(index);
				this.tnefWriter.WriteProperty(embeddedParticipantProperty.EmailAddressPropertyDefinition, participant.EmailAddress);
				this.tnefWriter.WriteProperty(embeddedParticipantProperty.RoutingTypePropertyDefinition, participant.RoutingType);
				this.tnefWriter.WriteProperty(embeddedParticipantProperty.DisplayNamePropertyDefinition, string.IsNullOrEmpty(participant.DisplayName) ? participant.EmailAddress : participant.DisplayName);
			}
		}

		private void WriteTnefCorrelationKeyProperty()
		{
			if (this.tnefCorrelationKey != null)
			{
				byte[] array = new byte[this.tnefCorrelationKey.Length + 1];
				CTSGlobals.AsciiEncoding.GetBytes(this.tnefCorrelationKey, 0, this.tnefCorrelationKey.Length, array, 0);
				this.tnefWriter.WriteProperty(TnefPropertyTag.TnefCorrelationKey, array);
			}
		}

		private void WriteMapiProperties()
		{
			this.tnefWriter.StartAttribute(TnefAttributeTag.MapiProperties, TnefAttributeLevel.Message);
			this.WriteTnefCorrelationKeyProperty();
			this.WriteMessageBody();
			foreach (NativeStorePropertyDefinition nativeStorePropertyDefinition in this.addressCache.Properties.AllNativeProperties)
			{
				object obj = this.addressCache.Properties.TryGetProperty(nativeStorePropertyDefinition);
				if (obj != null && !(obj is PropertyError))
				{
					this.tnefWriter.WriteProperty(nativeStorePropertyDefinition, obj);
				}
			}
			foreach (NativeStorePropertyDefinition property in this.item.AllNativeProperties)
			{
				if (!ItemToTnefConverter.excludedMapiProperties.Contains(property) && !ConversionAddressCache.IsAnyCacheProperty(property) && this.propertyChecker.IsItemPropertyWritable(property))
				{
					this.WriteMessageProperty(property);
				}
			}
			if (!this.isEmbeddedItem && ObjectClass.IsMdn(this.item.ClassName) && this.item.Session is MailboxSession)
			{
				this.AppendTimeZoneInfo();
			}
		}

		private void AppendTimeZoneInfo()
		{
			MailboxSession mailboxSession = this.item.Session as MailboxSession;
			if (mailboxSession == null)
			{
				return;
			}
			ExTimeZone timeZone;
			byte[] timeZoneBlob;
			if (TimeZoneSettings.TryFindOwaTimeZone(mailboxSession, out timeZone))
			{
				timeZoneBlob = O12TimeZoneFormatter.GetTimeZoneBlob(timeZone);
				this.tnefWriter.WriteProperty(InternalSchema.TimeZoneDefinitionStart, timeZoneBlob);
				return;
			}
			if (TimeZoneSettings.TryFindOutlookTimeZone(mailboxSession, out timeZoneBlob))
			{
				this.tnefWriter.WriteProperty(InternalSchema.TimeZoneDefinitionStart, timeZoneBlob);
			}
		}

		private long WriteAttachDataObj(Attachment attachment)
		{
			OleAttachment oleAttachment = attachment as OleAttachment;
			long result = 0L;
			if (oleAttachment != null)
			{
				byte[] array = MimeConstants.IID_IStorage.ToByteArray();
				using (Stream stream = this.tnefWriter.StartStreamProperty(InternalSchema.AttachDataObj))
				{
					this.tnefWriter.StreamPropertyData(array, 0, array.Length);
					using (Stream contentStream = oleAttachment.GetContentStream(PropertyOpenMode.ReadOnly))
					{
						result = Util.StreamHandler.CopyStreamData(contentStream, stream);
					}
				}
			}
			return result;
		}

		private long WriteAttachmentProperties(Attachment attachment)
		{
			long result = 0L;
			this.tnefWriter.StartAttribute(TnefAttributeTag.Attachment, TnefAttributeLevel.Attachment);
			foreach (NativeStorePropertyDefinition nativeStorePropertyDefinition in attachment.AllNativeProperties)
			{
				if (nativeStorePropertyDefinition.Equals(InternalSchema.AttachDataObj))
				{
					result = this.WriteAttachDataObj(attachment);
				}
				else if (!nativeStorePropertyDefinition.Equals(InternalSchema.AttachDataBin) && this.propertyChecker.IsAttachmentPropertyWritable(nativeStorePropertyDefinition))
				{
					object obj = attachment.TryGetProperty(nativeStorePropertyDefinition);
					PropertyError propertyError = obj as PropertyError;
					if (propertyError != null && PropertyError.IsPropertyValueTooBig(propertyError))
					{
						this.WritePropertyStreamData(attachment.PropertyBag, nativeStorePropertyDefinition);
					}
					else if (propertyError == null)
					{
						obj = ExTimeZoneHelperForMigrationOnly.ToLegacyUtcIfDateTime(obj);
						this.tnefWriter.WriteProperty(nativeStorePropertyDefinition, obj);
					}
				}
			}
			ItemAttachment itemAttachment = attachment as ItemAttachment;
			if (itemAttachment != null)
			{
				result = this.WriteAttachedItem(itemAttachment);
			}
			return result;
		}

		private long WriteAttachedItem(ItemAttachment attachment)
		{
			Item item = ConvertUtils.OpenAttachedItem(attachment);
			long result = 0L;
			if (item != null)
			{
				try
				{
					this.limitsTracker.StartEmbeddedMessage();
					Charset itemWindowsCharset = ConvertUtils.GetItemWindowsCharset(item, this.options);
					using (ItemToTnefConverter.TnefContentWriter embeddedMessageWriter = this.tnefWriter.GetEmbeddedMessageWriter(itemWindowsCharset))
					{
						OutboundAddressCache outboundAddressCache = new OutboundAddressCache(this.options, this.limitsTracker);
						outboundAddressCache.CopyDataFromItem(item);
						if (this.tnefType == TnefType.LegacyTnef && this.options.ResolveRecipientsInAttachedMessages)
						{
							outboundAddressCache.Resolve();
						}
						using (ItemToTnefConverter itemToTnefConverter = new ItemToTnefConverter(item, outboundAddressCache, embeddedMessageWriter, this.options, this.limitsTracker, this.tnefType, true))
						{
							ConversionResult conversionResult = itemToTnefConverter.Convert();
							result = conversionResult.BodySize + conversionResult.AccumulatedAttachmentSize;
						}
					}
					this.limitsTracker.EndEmbeddedMessage();
				}
				finally
				{
					item.Dispose();
				}
			}
			return result;
		}

		private long WriteAttachment(Item item, Attachment attachment)
		{
			this.limitsTracker.CountMessageAttachment();
			this.WriteAttachmentAttributes(attachment);
			long result = 0L;
			StreamAttachment streamAttachment = attachment as StreamAttachment;
			if (streamAttachment != null)
			{
				using (Stream stream = this.tnefWriter.OpenAttributeStream(TnefAttributeTag.AttachData, TnefAttributeLevel.Attachment))
				{
					object obj = attachment.TryGetProperty(InternalSchema.AttachDataBin);
					PropertyError propertyError = obj as PropertyError;
					if (propertyError != null)
					{
						using (Stream stream2 = streamAttachment.TryGetRawContentStream(PropertyOpenMode.ReadOnly))
						{
							if (stream2 != null)
							{
								result = Util.StreamHandler.CopyStreamData(stream2, stream);
							}
							goto IL_88;
						}
					}
					byte[] array = (byte[])obj;
					result = (long)array.Length;
					stream.Write(array, 0, array.Length);
					IL_88:;
				}
			}
			this.WriteAttachmentProperties(attachment);
			return result;
		}

		private void WriteAttachments()
		{
			if (this.item.AttachmentCollection != null)
			{
				int num = 0;
				long num2 = 0L;
				this.item.CoreItem.OpenAttachmentCollection();
				foreach (AttachmentHandle handle in this.item.CoreItem.AttachmentCollection)
				{
					num++;
					using (Attachment attachment = this.item.AttachmentCollection.Open(handle, InternalSchema.ContentConversionProperties))
					{
						using (StorageGlobals.SetTraceContext(attachment))
						{
							StorageGlobals.ContextTracePfd(ExTraceGlobals.CcPFDTracer, "Starting attachment (ItemToTnefConverter.WriteAttachments)");
							num2 += this.WriteAttachment(this.item, attachment);
							StorageGlobals.ContextTracePfd(ExTraceGlobals.CcPFDTracer, "Finishing attachment (ItemToTnefConverter.WriteAttachments)");
						}
					}
				}
				this.conversionResult.AttachmentCount = num;
				this.conversionResult.AccumulatedAttachmentSize = num2;
			}
		}

		private void WriteRecipient(ConversionRecipientEntry recipient)
		{
			if (recipient.Participant == null)
			{
				return;
			}
			RecipientItemType recipientItemType = recipient.RecipientItemType;
			if (!ConvertUtils.IsRecipientTransmittable(recipientItemType) && this.tnefType != TnefType.SummaryTnef)
			{
				return;
			}
			this.tnefWriter.StartRow();
			this.tnefWriter.WriteProperty(TnefPropertyTag.RecipientType, (int)MapiUtil.RecipientItemTypeToMapiRecipientType(recipientItemType, false));
			Participant participant = recipient.Participant;
			this.tnefWriter.WriteProperty(TnefPropertyTag.DisplayNameW, participant.DisplayName ?? string.Empty);
			this.tnefWriter.WriteProperty(TnefPropertyTag.EmailAddressW, participant.EmailAddress ?? string.Empty);
			this.tnefWriter.WriteProperty(TnefPropertyTag.AddrtypeW, participant.RoutingType ?? string.Empty);
			foreach (NativeStorePropertyDefinition nativeStorePropertyDefinition in recipient.AllNativeProperties)
			{
				if (nativeStorePropertyDefinition != null && nativeStorePropertyDefinition != InternalSchema.RecipientType && nativeStorePropertyDefinition != InternalSchema.DisplayName && nativeStorePropertyDefinition != InternalSchema.EmailAddress && nativeStorePropertyDefinition != InternalSchema.AddrType && this.propertyChecker.IsRecipientPropertyWritable(nativeStorePropertyDefinition))
				{
					this.WriteRecipientProperty(recipient, nativeStorePropertyDefinition);
				}
			}
		}

		private bool WriteRecipientProperty(ConversionRecipientEntry recipient, NativeStorePropertyDefinition property)
		{
			object obj = recipient.TryGetProperty(property);
			if (!PropertyError.IsPropertyError(obj))
			{
				obj = ExTimeZoneHelperForMigrationOnly.ToLegacyUtcIfDateTime(obj);
				this.tnefWriter.WriteProperty(property, obj);
				return true;
			}
			return false;
		}

		private void WriteRecipientTable()
		{
			if (this.tnefType != TnefType.LegacyTnef || this.isEmbeddedItem)
			{
				this.tnefWriter.StartAttribute(TnefAttributeTag.RecipientTable, TnefAttributeLevel.Message);
				List<ConversionRecipientEntry> recipients = this.addressCache.Recipients;
				if (recipients != null)
				{
					this.conversionResult.RecipientCount = recipients.Count;
					foreach (ConversionRecipientEntry conversionRecipientEntry in recipients)
					{
						if (!(this.item is MessageItem) || this.options.DemoteBcc || conversionRecipientEntry.RecipientItemType != RecipientItemType.Bcc || ((conversionRecipientEntry.Participant.GetValueOrDefault<bool>(ParticipantSchema.IsRoom, false) || conversionRecipientEntry.Participant.GetValueOrDefault<bool>(ParticipantSchema.IsResource, false)) && ObjectClass.IsMeetingMessage(this.item.ClassName)))
						{
							this.WriteRecipient(conversionRecipientEntry);
						}
					}
				}
			}
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.tnefWriter != null)
				{
					this.tnefWriter.Dispose();
					this.tnefWriter = null;
				}
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
				}
			}
		}

		private const int TnefVersionValue = 65536;

		private const int Attach_OLE = 6;

		private static readonly HashSet<NativeStorePropertyDefinition> excludedMapiProperties = ItemToTnefConverter.CreateExcludedPropertiesSet();

		private static readonly HashSet<NativeStorePropertyDefinition> bodyProperties = ItemToTnefConverter.CreateBodyPropertiesSet();

		private Item item;

		private ItemToTnefConverter.TnefContentWriter tnefWriter;

		private OutboundConversionOptions options;

		private ConversionLimitsTracker limitsTracker;

		private TnefType tnefType;

		private bool isEmbeddedItem;

		private string tnefCorrelationKey;

		private OutboundAddressCache addressCache;

		private TnefPropertyChecker propertyChecker;

		private DisposeTracker disposeTracker;

		private ConversionResult conversionResult;

		internal class TnefContentWriterPropertyStream : StreamBase
		{
			internal TnefContentWriterPropertyStream(ItemToTnefConverter.TnefContentWriter writer) : base(StreamBase.Capabilities.Writable)
			{
				this.writer = writer;
			}

			public override DisposeTracker GetDisposeTracker()
			{
				return DisposeTracker.Get<ItemToTnefConverter.TnefContentWriterPropertyStream>(this);
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				base.CheckDisposed("Write");
				this.writer.StreamPropertyData(buffer, offset, count);
			}

			private ItemToTnefConverter.TnefContentWriter writer;
		}

		internal class TnefContentWriterAttributeStream : StreamBase
		{
			internal TnefContentWriterAttributeStream(ItemToTnefConverter.TnefContentWriter writer) : base(StreamBase.Capabilities.Writable)
			{
				this.writer = writer;
			}

			public override DisposeTracker GetDisposeTracker()
			{
				return DisposeTracker.Get<ItemToTnefConverter.TnefContentWriterAttributeStream>(this);
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				base.CheckDisposed("Write");
				this.writer.StreamAttributeData(buffer, offset, count);
			}

			private ItemToTnefConverter.TnefContentWriter writer;
		}

		internal class TnefContentWriter : IDisposeTrackable, IDisposable
		{
			internal TnefContentWriter(Stream outStream, Charset charset) : this(ItemToTnefConverter.TnefContentWriter.CreateTnefWriter(outStream, charset), charset)
			{
			}

			private TnefContentWriter(TnefWriter writer, Charset charset)
			{
				this.tnefWriter = writer;
				this.charset = charset;
				this.isNewRow = false;
				this.newTnefAttribute = TnefAttributeTag.Null;
				this.currentTnefAttribute = TnefAttributeTag.Null;
				this.newTnefAttributeLevel = TnefAttributeLevel.Message;
				this.streamProperty = null;
				this.tnefWriter.SetMessageCodePage(charset.CodePage);
				this.disposeTracker = this.GetDisposeTracker();
			}

			public virtual DisposeTracker GetDisposeTracker()
			{
				return DisposeTracker.Get<ItemToTnefConverter.TnefContentWriter>(this);
			}

			public void SuppressDisposeTracker()
			{
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Suppress();
				}
			}

			public void Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			internal void StartAttribute(TnefAttributeTag tag, TnefAttributeLevel level)
			{
				this.newTnefAttribute = tag;
				this.newTnefAttributeLevel = level;
			}

			internal Stream OpenAttributeStream(TnefAttributeTag tag, TnefAttributeLevel level)
			{
				this.StartAttribute(tag, level);
				return new ItemToTnefConverter.TnefContentWriterAttributeStream(this);
			}

			internal void StartRow()
			{
				this.isNewRow = true;
			}

			internal Stream StartStreamProperty(NativeStorePropertyDefinition property)
			{
				this.streamProperty = property;
				return new ItemToTnefConverter.TnefContentWriterPropertyStream(this);
			}

			internal void WriteProperty(NativeStorePropertyDefinition property, object propertyValue)
			{
				if (propertyValue == null)
				{
					return;
				}
				TnefPropertyTag propertyTag = this.StartProperty(property);
				if (this.IsMultivalued(property.MapiPropertyType))
				{
					using (IEnumerator enumerator = ((Array)propertyValue).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							object propertyValue2 = enumerator.Current;
							this.WriteTnefWriterPropertyValue(propertyTag, propertyValue2);
						}
						return;
					}
				}
				this.WriteTnefWriterPropertyValue(propertyTag, propertyValue);
			}

			internal void WriteProperty(TnefPropertyTag property, object propertyValue)
			{
				if (propertyValue == null)
				{
					return;
				}
				this.StartProperty(property);
				if (this.IsMultivalued(property))
				{
					using (IEnumerator enumerator = ((Array)propertyValue).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							object propertyValue2 = enumerator.Current;
							this.WriteTnefWriterPropertyValue(property, propertyValue2);
						}
						return;
					}
				}
				this.WriteTnefWriterPropertyValue(property, propertyValue);
			}

			internal void StreamPropertyData(byte[] data, int offset, int length)
			{
				if (this.streamProperty != null)
				{
					this.StartProperty(this.streamProperty);
					this.streamProperty = null;
				}
				this.tnefWriter.WritePropertyRawValue(data, offset, length);
			}

			internal void StreamAttributeData(byte[] data, int offset, int length)
			{
				this.CheckStartTnefAttribute();
				this.tnefWriter.WriteAttributeRawValue(data, offset, length);
			}

			internal void Flush()
			{
				this.tnefWriter.Flush();
			}

			internal ItemToTnefConverter.TnefContentWriter GetEmbeddedMessageWriter(Charset charset)
			{
				this.tnefWriter.StartProperty(TnefPropertyTag.AttachDataObj);
				TnefWriter embeddedMessageWriter = this.tnefWriter.GetEmbeddedMessageWriter();
				ItemToTnefConverter.TnefContentWriter.InitializeWriter(embeddedMessageWriter, charset);
				return new ItemToTnefConverter.TnefContentWriter(embeddedMessageWriter, this.charset);
			}

			private static TnefWriter CreateTnefWriter(Stream outStream, Charset charset)
			{
				Random random = new Random((int)ExDateTime.UtcNow.UtcTicks);
				TnefWriter tnefWriter = new TnefWriter(outStream, (short)random.Next(32767));
				ItemToTnefConverter.TnefContentWriter.InitializeWriter(tnefWriter, charset);
				return tnefWriter;
			}

			private static void InitializeWriter(TnefWriter writer, Charset charset)
			{
				writer.WriteTnefVersion();
				try
				{
					writer.WriteOemCodePage(charset.CodePage);
				}
				catch (ArgumentException innerException)
				{
					StorageGlobals.ContextTraceError<int>(ExTraceGlobals.CcOutboundTnefTracer, "TnefContentWriter::InitializeWriter: invalid codepage, {0}.", charset.CodePage);
					throw new ConversionFailedException(ConversionFailureReason.CorruptContent, innerException);
				}
			}

			private static TnefPropertyTag GetTnefPropertyTag(PropertyTagPropertyDefinition property)
			{
				return (int)property.PropertyTag;
			}

			private static TnefPropertyTag GetTnefPropertyTag(GuidNamePropertyDefinition property)
			{
				return (int)((PropType)(-2147483648) | property.MapiPropertyType);
			}

			private static TnefPropertyTag GetTnefPropertyTag(GuidIdPropertyDefinition property)
			{
				return (int)((PropType)(-2147483648) | property.MapiPropertyType);
			}

			private void StartProperty(TnefPropertyTag tag)
			{
				this.CheckStartTnefAttribute();
				this.streamProperty = null;
				this.tnefWriter.StartProperty(tag);
			}

			private TnefPropertyTag StartProperty(NativeStorePropertyDefinition property)
			{
				this.CheckStartTnefAttribute();
				this.streamProperty = null;
				TnefPropertyTag tnefPropertyTag;
				switch (property.SpecifiedWith)
				{
				case PropertyTypeSpecifier.PropertyTag:
					tnefPropertyTag = ItemToTnefConverter.TnefContentWriter.GetTnefPropertyTag((PropertyTagPropertyDefinition)property);
					this.tnefWriter.StartProperty(tnefPropertyTag);
					break;
				case PropertyTypeSpecifier.GuidString:
				{
					GuidNamePropertyDefinition guidNamePropertyDefinition = (GuidNamePropertyDefinition)property;
					tnefPropertyTag = ItemToTnefConverter.TnefContentWriter.GetTnefPropertyTag(guidNamePropertyDefinition);
					this.tnefWriter.StartProperty(tnefPropertyTag, guidNamePropertyDefinition.Guid, guidNamePropertyDefinition.PropertyName);
					break;
				}
				case PropertyTypeSpecifier.GuidId:
				{
					GuidIdPropertyDefinition guidIdPropertyDefinition = (GuidIdPropertyDefinition)property;
					tnefPropertyTag = ItemToTnefConverter.TnefContentWriter.GetTnefPropertyTag(guidIdPropertyDefinition);
					this.tnefWriter.StartProperty(tnefPropertyTag, guidIdPropertyDefinition.Guid, guidIdPropertyDefinition.Id);
					break;
				}
				default:
					throw new InvalidOperationException(string.Format("Invalid native property specifier: {0}", property.SpecifiedWith));
				}
				return tnefPropertyTag;
			}

			private void WriteTnefWriterPropertyValue(TnefPropertyTag propertyTag, object propertyValue)
			{
				TnefPropertyType tnefType = propertyTag.TnefType;
				if (tnefType == TnefPropertyType.AppTime || tnefType == (TnefPropertyType)4103)
				{
					DateTime dateTime = (DateTime)Util.Date1601Utc.ToUtc();
					try
					{
						dateTime = ConvertUtils.GetDateTimeFromOADate((double)propertyValue);
					}
					catch (ArgumentException arg)
					{
						StorageGlobals.ContextTraceError<double, ArgumentException>(ExTraceGlobals.CcOutboundTnefTracer, "TnefContentWriter::WriteTnefWriterPropertyValue: ArgumentException processing date {0}, {1}.", (double)propertyValue, arg);
					}
					propertyValue = dateTime;
				}
				propertyValue = ExTimeZoneHelperForMigrationOnly.ToLegacyUtcIfDateTime(propertyValue);
				if (propertyValue is DateTime)
				{
					DateTime dateTime2 = (DateTime)propertyValue;
					if ((ExDateTime)dateTime2 < Util.Date1601Utc)
					{
						propertyValue = (DateTime)Util.Date1601Utc;
					}
				}
				this.tnefWriter.WritePropertyValue(propertyValue);
			}

			private void CheckStartTnefAttribute()
			{
				if (this.newTnefAttribute != TnefAttributeTag.Null && this.newTnefAttribute != this.currentTnefAttribute)
				{
					this.tnefWriter.StartAttribute(this.newTnefAttribute, this.newTnefAttributeLevel);
					this.currentTnefAttribute = this.newTnefAttribute;
				}
				if (this.isNewRow)
				{
					this.tnefWriter.StartRow();
					this.isNewRow = false;
				}
			}

			private bool IsMultivalued(PropType propType)
			{
				return ((long)propType & 4096L) != 0L;
			}

			private bool IsMultivalued(TnefPropertyTag propTag)
			{
				return ((long)propTag & 4096L) != 0L;
			}

			private void Dispose(bool disposing)
			{
				if (disposing)
				{
					if (this.tnefWriter != null)
					{
						this.tnefWriter.Close();
						this.tnefWriter = null;
					}
					if (this.disposeTracker != null)
					{
						this.disposeTracker.Dispose();
					}
				}
			}

			private readonly DisposeTracker disposeTracker;

			private Charset charset;

			private TnefWriter tnefWriter;

			private TnefAttributeTag newTnefAttribute;

			private TnefAttributeLevel newTnefAttributeLevel;

			private TnefAttributeTag currentTnefAttribute;

			private NativeStorePropertyDefinition streamProperty;

			private bool isNewRow;
		}
	}
}
