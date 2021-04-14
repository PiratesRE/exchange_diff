using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.ContentTypes.Tnef;
using Microsoft.Exchange.Data.MsgStorage.Internal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class OutboundMsgConverter
	{
		internal OutboundMsgConverter(OutboundConversionOptions options)
		{
			this.options = options;
		}

		private MsgStorageWriter PropertyWriter
		{
			get
			{
				return this.writer;
			}
		}

		internal void ConvertItemToMsgStorage(Item item, Stream outStream)
		{
			MsgStorageWriter msgStorageWriter = null;
			try
			{
				msgStorageWriter = new MsgStorageWriter(outStream);
				ConversionLimitsTracker conversionLimitsTracker = new ConversionLimitsTracker(this.options.Limits);
				conversionLimitsTracker.CountMessageBody();
				this.InternalConvertItemToMsgStorage(item, msgStorageWriter, conversionLimitsTracker);
			}
			finally
			{
				if (msgStorageWriter != null)
				{
					msgStorageWriter.Dispose();
				}
			}
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
				InternalSchema.StoreSupportMask,
				InternalSchema.AttachNum,
				InternalSchema.ObjectType,
				InternalSchema.EntryId,
				InternalSchema.RecordKey,
				InternalSchema.StoreEntryId,
				InternalSchema.StoreRecordKey,
				InternalSchema.ParentEntryId,
				InternalSchema.SourceKey,
				InternalSchema.CreatorEntryId,
				InternalSchema.LastModifierEntryId,
				InternalSchema.MdbProvider,
				InternalSchema.MappingSignature,
				InternalSchema.UrlCompName,
				InternalSchema.UrlCompNamePostfix,
				InternalSchema.MID,
				InternalSchema.Associated,
				InternalSchema.Size,
				InternalSchema.SentMailSvrEId,
				InternalSchema.SentMailEntryId,
				InternalSchema.AttachSize
			};
		}

		private static HashSet<NativeStorePropertyDefinition> CreateRecipientExcludedPropertiesSet()
		{
			return new HashSet<NativeStorePropertyDefinition>
			{
				InternalSchema.BusinessPhoneNumber,
				InternalSchema.OfficeLocation,
				InternalSchema.MobilePhone,
				InternalSchema.RowId
			};
		}

		private void InternalConvertItemToMsgStorage(Item item, MsgStorageWriter writer, ConversionLimitsTracker limitsTracker)
		{
			this.item = item;
			this.writer = writer;
			this.limitsTracker = limitsTracker;
			this.addressCache = new OutboundAddressCache(this.options, limitsTracker);
			ExTimeZone exTimeZone = this.item.PropertyBag.ExTimeZone;
			this.item.PropertyBag.ExTimeZone = ExTimeZone.UtcTimeZone;
			try
			{
				this.addressCache.CopyDataFromItem(this.item);
				this.WriteMessageProperties();
				this.WriteRecipientTable();
				this.WriteAttachments();
				this.writer.Flush();
			}
			finally
			{
				this.item.PropertyBag.ExTimeZone = exTimeZone;
			}
		}

		private void WriteMessageProperties()
		{
			this.WriteProperty(InternalSchema.StoreSupportMask, 265849);
			this.WritePropertyIfMissing(InternalSchema.MapiHasAttachment, this.item.PropertyBag, delegate
			{
				foreach (AttachmentHandle attachmentHandle in this.item.AttachmentCollection)
				{
					using (Attachment attachment = this.item.AttachmentCollection.Open(attachmentHandle, null))
					{
						if (!attachment.IsInline || attachmentHandle.AttachMethod == 7)
						{
							return true;
						}
					}
				}
				return false;
			});
			this.WritePropertyIfMissing(InternalSchema.DisplayToInternal, this.item.PropertyBag, () => this.item.PropertyBag.TryGetProperty(InternalSchema.DisplayTo));
			this.WritePropertyIfMissing(InternalSchema.DisplayCcInternal, this.item.PropertyBag, () => this.item.PropertyBag.TryGetProperty(InternalSchema.DisplayCc));
			this.WritePropertyIfMissing(InternalSchema.DisplayBccInternal, this.item.PropertyBag, () => this.item.PropertyBag.TryGetProperty(InternalSchema.DisplayBcc));
			this.WriteMessageBody();
			foreach (NativeStorePropertyDefinition nativeStorePropertyDefinition in this.addressCache.Properties.AllNativeProperties)
			{
				object obj = this.addressCache.Properties.TryGetProperty(nativeStorePropertyDefinition);
				if (obj != null && !(obj is PropertyError))
				{
					this.WriteProperty(nativeStorePropertyDefinition, obj);
				}
			}
			foreach (NativeStorePropertyDefinition property in this.item.PropertyBag.AllNativeProperties)
			{
				if (!OutboundMsgConverter.excludedPropertySet.Contains(property) && !ConversionAddressCache.IsAnyCacheProperty(property))
				{
					this.WriteProperty(property, this.item.PropertyBag);
				}
			}
		}

		private void WriteProperty(NativeStorePropertyDefinition property, PersistablePropertyBag propertyBag)
		{
			object obj = propertyBag.TryGetProperty(property);
			PropertyError propertyError = obj as PropertyError;
			if (propertyError == null)
			{
				this.WriteProperty(property, obj);
				return;
			}
			if (PropertyError.IsPropertyValueTooBig(propertyError))
			{
				this.StreamProperty(property, propertyBag);
			}
		}

		private void WritePropertyIfMissing(NativeStorePropertyDefinition property, PersistablePropertyBag propertyBag, OutboundMsgConverter.ComputeValueDelegate computeValue)
		{
			PropertyError propertyError = ((IDirectPropertyBag)propertyBag).GetValue(property) as PropertyError;
			if (propertyError != null && !PropertyError.IsPropertyValueTooBig(propertyError))
			{
				object obj = computeValue();
				if (!(obj is PropertyError))
				{
					this.WriteProperty(property, obj);
				}
			}
		}

		private void WriteProperty(NativeStorePropertyDefinition property, object value)
		{
			value = ExTimeZoneHelperForMigrationOnly.ToLegacyUtcIfDateTime(value);
			switch (property.SpecifiedWith)
			{
			case PropertyTypeSpecifier.PropertyTag:
			{
				PropertyTagPropertyDefinition propertyTagPropertyDefinition = (PropertyTagPropertyDefinition)property;
				TnefPropertyTag propertyTag = (int)propertyTagPropertyDefinition.PropertyTag;
				this.PropertyWriter.WriteProperty(propertyTag, value);
				return;
			}
			case PropertyTypeSpecifier.GuidString:
			{
				GuidNamePropertyDefinition guidNamePropertyDefinition = (GuidNamePropertyDefinition)property;
				TnefPropertyType propertyType = (TnefPropertyType)guidNamePropertyDefinition.MapiPropertyType;
				this.PropertyWriter.WriteProperty(guidNamePropertyDefinition.Guid, guidNamePropertyDefinition.PropertyName, propertyType, value);
				return;
			}
			case PropertyTypeSpecifier.GuidId:
			{
				GuidIdPropertyDefinition guidIdPropertyDefinition = (GuidIdPropertyDefinition)property;
				TnefPropertyType propertyType2 = (TnefPropertyType)guidIdPropertyDefinition.MapiPropertyType;
				this.PropertyWriter.WriteProperty(guidIdPropertyDefinition.Guid, guidIdPropertyDefinition.Id, propertyType2, value);
				return;
			}
			default:
				throw new InvalidOperationException(string.Format("Invalid native property specifier: {0}", property.SpecifiedWith));
			}
		}

		private void StreamProperty(NativeStorePropertyDefinition property, PersistablePropertyBag propertyBag)
		{
			Stream stream = null;
			try
			{
				stream = propertyBag.OpenPropertyStream(property, PropertyOpenMode.ReadOnly);
				this.StreamProperty(property, stream);
			}
			catch (ObjectNotFoundException)
			{
			}
			finally
			{
				if (stream != null)
				{
					stream.Dispose();
				}
			}
		}

		private void StreamProperty(NativeStorePropertyDefinition property, Stream propertyStream)
		{
			Stream stream = null;
			try
			{
				switch (property.SpecifiedWith)
				{
				case PropertyTypeSpecifier.PropertyTag:
				{
					PropertyTagPropertyDefinition propertyTagPropertyDefinition = (PropertyTagPropertyDefinition)property;
					TnefPropertyTag propertyTag = (int)propertyTagPropertyDefinition.PropertyTag;
					stream = this.PropertyWriter.OpenPropertyStream(propertyTag);
					break;
				}
				case PropertyTypeSpecifier.GuidString:
				{
					GuidNamePropertyDefinition guidNamePropertyDefinition = (GuidNamePropertyDefinition)property;
					TnefPropertyType propertyType = (TnefPropertyType)guidNamePropertyDefinition.MapiPropertyType;
					stream = this.PropertyWriter.OpenPropertyStream(guidNamePropertyDefinition.Guid, guidNamePropertyDefinition.PropertyName, propertyType);
					break;
				}
				case PropertyTypeSpecifier.GuidId:
				{
					GuidIdPropertyDefinition guidIdPropertyDefinition = (GuidIdPropertyDefinition)property;
					TnefPropertyType propertyType2 = (TnefPropertyType)guidIdPropertyDefinition.MapiPropertyType;
					stream = this.PropertyWriter.OpenPropertyStream(guidIdPropertyDefinition.Guid, guidIdPropertyDefinition.Id, propertyType2);
					break;
				}
				default:
					throw new InvalidOperationException(string.Format("Invalid native property specifier: {0}", property.SpecifiedWith));
				}
				Util.StreamHandler.CopyStreamData(propertyStream, stream);
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
				}
			}
		}

		private void WriteMessageBody()
		{
			if (this.options.FilterBodyHandler != null && !this.options.FilterBodyHandler(this.item))
			{
				return;
			}
			this.limitsTracker.CountMessageBody();
			BodyFormat rawFormat = this.item.Body.RawFormat;
			if (rawFormat == BodyFormat.TextHtml)
			{
				this.WriteProperty(InternalSchema.HtmlBody, this.item.PropertyBag);
				return;
			}
			this.WriteProperty(InternalSchema.RtfSyncBodyCrc, this.item.PropertyBag);
			this.WriteProperty(InternalSchema.RtfSyncBodyCount, this.item.PropertyBag);
			this.WriteProperty(InternalSchema.RtfSyncBodyTag, this.item.PropertyBag);
			this.WriteProperty(InternalSchema.RtfInSync, this.item.PropertyBag);
			this.WriteProperty(InternalSchema.RtfSyncPrefixCount, this.item.PropertyBag);
			this.WriteProperty(InternalSchema.RtfSyncTrailingCount, this.item.PropertyBag);
			if (!this.item.Body.IsBodyDefined)
			{
				return;
			}
			if (rawFormat == BodyFormat.TextPlain)
			{
				this.WriteProperty(InternalSchema.TextBody, this.item.PropertyBag);
			}
			BodyReadConfiguration configuration = new BodyReadConfiguration(BodyFormat.ApplicationRtf);
			using (Stream stream = this.item.Body.OpenReadStream(configuration))
			{
				using (Stream stream2 = this.PropertyWriter.OpenPropertyStream(TnefPropertyTag.RtfCompressed))
				{
					Util.StreamHandler.CopyStreamData(stream, stream2);
				}
			}
		}

		private void WriteAttachment(Attachment attachment, int attachNumber)
		{
			this.limitsTracker.CountMessageAttachment();
			this.PropertyWriter.StartAttachment();
			this.WriteProperty(InternalSchema.AttachNum, attachNumber);
			foreach (NativeStorePropertyDefinition nativeStorePropertyDefinition in attachment.AllNativeProperties)
			{
				if (!nativeStorePropertyDefinition.Equals(InternalSchema.AttachDataObj) && !nativeStorePropertyDefinition.Equals(InternalSchema.AttachDataBin) && !OutboundMsgConverter.excludedPropertySet.Contains(nativeStorePropertyDefinition))
				{
					this.WriteProperty(nativeStorePropertyDefinition, attachment.PropertyBag);
				}
			}
			switch (attachment.AttachmentType)
			{
			case AttachmentType.Stream:
				this.WriteProperty(InternalSchema.AttachDataBin, attachment.PropertyBag);
				return;
			case AttachmentType.EmbeddedMessage:
				this.WriteAttachedItem((ItemAttachment)attachment);
				return;
			case AttachmentType.Ole:
				this.StreamProperty(InternalSchema.AttachDataObj, attachment.PropertyBag);
				break;
			case AttachmentType.Reference:
				break;
			default:
				return;
			}
		}

		private void WriteAttachedItem(ItemAttachment itemAttachment)
		{
			using (Item item = itemAttachment.GetItem(InternalSchema.ContentConversionProperties))
			{
				this.limitsTracker.StartEmbeddedMessage();
				using (MsgStorageWriter embeddedMessageWriter = this.PropertyWriter.GetEmbeddedMessageWriter())
				{
					OutboundMsgConverter outboundMsgConverter = new OutboundMsgConverter(this.options);
					outboundMsgConverter.InternalConvertItemToMsgStorage(item, embeddedMessageWriter, this.limitsTracker);
				}
				this.limitsTracker.EndEmbeddedMessage();
			}
		}

		private void WriteAttachments()
		{
			if (this.item.AttachmentCollection != null)
			{
				int num = 0;
				this.item.CoreItem.OpenAttachmentCollection();
				foreach (AttachmentHandle handle in this.item.CoreItem.AttachmentCollection)
				{
					using (Attachment attachment = this.item.AttachmentCollection.Open(handle, InternalSchema.ContentConversionProperties))
					{
						if (this.options.FilterAttachmentHandler == null || this.options.FilterAttachmentHandler(this.item, attachment))
						{
							using (StorageGlobals.SetTraceContext(attachment))
							{
								StorageGlobals.ContextTracePfd(ExTraceGlobals.CcPFDTracer, "Starting attachment (OutboundMsgStorage.WriteAttachments)");
								this.WriteAttachment(attachment, num++);
								StorageGlobals.ContextTracePfd(ExTraceGlobals.CcPFDTracer, "Finishing attachment (OutboundMsgStorage.WriteAttachments)");
							}
						}
					}
				}
			}
		}

		private void WriteRecipient(ConversionRecipientEntry recipient, int recipientIndex)
		{
			if (recipient.Participant == null)
			{
				return;
			}
			this.PropertyWriter.StartRecipient();
			this.writer.WriteProperty(TnefPropertyTag.Rowid, recipientIndex);
			RecipientItemType recipientItemType = recipient.RecipientItemType;
			this.writer.WriteProperty(TnefPropertyTag.RecipientType, (int)MapiUtil.RecipientItemTypeToMapiRecipientType(recipientItemType, false));
			foreach (NativeStorePropertyDefinition nativeStorePropertyDefinition in recipient.AllNativeProperties)
			{
				if (nativeStorePropertyDefinition != null && !OutboundMsgConverter.recipientExcludedPropertySet.Contains(nativeStorePropertyDefinition))
				{
					this.WriteRecipientProperty(recipient, nativeStorePropertyDefinition);
				}
			}
		}

		private void WriteRecipientProperty(ConversionRecipientEntry recipient, NativeStorePropertyDefinition property)
		{
			object obj = recipient.TryGetProperty(property);
			if (!PropertyError.IsPropertyError(obj))
			{
				this.WriteProperty(property, obj);
			}
		}

		private void WriteRecipientTable()
		{
			List<ConversionRecipientEntry> recipients = this.addressCache.Recipients;
			if (recipients != null)
			{
				int num = 0;
				foreach (ConversionRecipientEntry recipient in recipients)
				{
					this.WriteRecipient(recipient, num++);
				}
			}
		}

		private static HashSet<NativeStorePropertyDefinition> excludedPropertySet = OutboundMsgConverter.CreateExcludedPropertiesSet();

		private static HashSet<NativeStorePropertyDefinition> recipientExcludedPropertySet = OutboundMsgConverter.CreateRecipientExcludedPropertiesSet();

		private Item item;

		private MsgStorageWriter writer;

		private OutboundConversionOptions options;

		private ConversionLimitsTracker limitsTracker;

		private OutboundAddressCache addressCache;

		private enum StoreSupportMaskValues
		{
			None,
			StoreEntryIdUnique,
			StoreReadOnly,
			StoreSearchOk = 4,
			StoreModifyOk = 8,
			StoreCreateOk = 16,
			StoreAttachOk = 32,
			StoreOleOk = 64,
			StoreSubmitOk = 128,
			StoreNotifyOk = 256,
			StoreMVPropsOk = 512,
			StoreCategorizeOk = 1024,
			StoreRtfOk = 2048,
			StoreRestrictionOk = 4096,
			StoreSortOk = 8192,
			StorePublicFolders = 16384,
			StoreUncompressedRtf = 32768,
			StoreHtmlOk = 65536,
			StoreAnsiOk = 131072,
			StoreUnicodeOk = 262144,
			StoreLocalStore = 524288,
			Default = 265849
		}

		private delegate object ComputeValueDelegate();
	}
}
