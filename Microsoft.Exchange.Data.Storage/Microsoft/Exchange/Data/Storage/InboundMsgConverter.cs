using System;
using System.IO;
using Microsoft.Exchange.Data.ContentTypes.Tnef;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.MsgStorage.Internal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class InboundMsgConverter : AbstractInboundConverter
	{
		internal InboundMsgConverter(InboundMessageWriter writer) : base(writer, new AbstractInboundConverter.WriteablePropertyPromotionRule())
		{
		}

		private void ConvertMessage()
		{
			this.isTextBodyFound = false;
			this.isHtmlBodyFound = false;
			this.isRtfBodyFound = false;
			this.isRtfInSync = false;
			this.PromotePropertyList();
			this.SetBody();
		}

		protected override void PromoteBodyProperty(StorePropertyDefinition property)
		{
			if (property == InternalSchema.TextBody)
			{
				this.isTextBodyFound = true;
				return;
			}
			if (property == InternalSchema.HtmlBody)
			{
				this.isHtmlBodyFound = true;
				return;
			}
			if (property == InternalSchema.RtfBody)
			{
				this.isRtfBodyFound = true;
				return;
			}
			if (property == InternalSchema.RtfInSync)
			{
				this.isRtfInSync = this.InternalReadBool();
				base.SetProperty(InternalSchema.RtfInSync, this.isRtfInSync);
			}
		}

		private void SetBody()
		{
			Stream stream = null;
			try
			{
				Charset charset = null;
				Charset.TryGetCharset(this.internetCpid, out charset);
				string targetCharsetName = (charset != null) ? charset.Name : null;
				BodyWriteConfiguration bodyWriteConfiguration;
				if (this.isRtfBodyFound && (!this.isHtmlBodyFound || this.isRtfInSync))
				{
					stream = this.InternalGetValueReadStream(TnefPropertyTag.RtfCompressed);
					bodyWriteConfiguration = new BodyWriteConfiguration(BodyFormat.ApplicationRtf);
					bodyWriteConfiguration.SetTargetFormat(BodyFormat.ApplicationRtf, targetCharsetName, BodyCharsetFlags.PreserveUnicode);
				}
				else if (this.isHtmlBodyFound)
				{
					if (charset == null)
					{
						charset = Charset.DefaultWebCharset;
					}
					stream = this.InternalGetValueReadStream(TnefPropertyTag.BodyHtmlB);
					bodyWriteConfiguration = new BodyWriteConfiguration(BodyFormat.TextHtml, charset.Name);
					bodyWriteConfiguration.SetTargetFormat(BodyFormat.TextHtml, charset.Name, BodyCharsetFlags.PreserveUnicode);
				}
				else
				{
					if (this.isTextBodyFound)
					{
						stream = this.InternalGetValueReadStream(TnefPropertyTag.BodyW);
					}
					bodyWriteConfiguration = new BodyWriteConfiguration(BodyFormat.TextPlain, ConvertUtils.UnicodeCharset.Name);
					if (charset != null)
					{
						bodyWriteConfiguration.SetTargetFormat(BodyFormat.TextPlain, charset.Name);
					}
					bodyWriteConfiguration.SetTargetFormat(BodyFormat.TextPlain, targetCharsetName, BodyCharsetFlags.PreserveUnicode);
				}
				base.CoreItem.CharsetDetector.DetectionOptions = base.MessageWriter.ConversionOptions.DetectionOptions;
				using (Stream stream2 = base.CoreItem.Body.OpenWriteStream(bodyWriteConfiguration))
				{
					if (stream != null)
					{
						Util.StreamHandler.CopyStreamData(stream, stream2);
					}
				}
			}
			finally
			{
				if (stream != null)
				{
					stream.Dispose();
				}
			}
		}

		protected override bool CanPromoteMimeOnlyProperties()
		{
			return true;
		}

		public void ConvertToItem(Stream msgStorageStream)
		{
			MsgStorageReader msgStorageReader = null;
			try
			{
				msgStorageReader = new MsgStorageReader(msgStorageStream);
				this.InternalConvertToItem(msgStorageReader);
			}
			finally
			{
				if (msgStorageReader != null)
				{
					msgStorageReader.Dispose();
				}
			}
		}

		private void InternalConvertToItem(MsgStorageReader reader)
		{
			CoreObject.GetPersistablePropertyBag(base.CoreItem).SaveFlags |= (PropertyBagSaveFlags.IgnoreMapiComputedErrors | PropertyBagSaveFlags.IgnoreUnresolvedHeaders);
			this.reader = reader;
			this.ConvertMessage();
			for (int num = 0; num != this.reader.RecipientCount; num++)
			{
				this.ConvertRecipient(num);
			}
			for (int num2 = 0; num2 != this.reader.AttachmentCount; num2++)
			{
				this.ConvertAttachment(num2);
			}
		}

		private void ConvertRecipient(int recipientIndex)
		{
			try
			{
				this.reader.OpenRecipient(recipientIndex);
				base.MessageWriter.StartNewRecipient();
				this.PromotePropertyList();
				base.MessageWriter.EndRecipient();
			}
			catch (MsgStorageNotFoundException)
			{
				StorageGlobals.ContextTraceError<int>(ExTraceGlobals.CcInboundTnefTracer, "InboundMsgConverter::RecipientNotFound (index = {0})", recipientIndex);
			}
		}

		private void ConvertAttachment(int attachmentIndex)
		{
			try
			{
				this.reader.OpenAttachment(attachmentIndex);
				base.MessageWriter.StartNewAttachment(this.reader.PropertyReader.AttachMethod);
				this.PromotePropertyList();
				base.MessageWriter.EndAttachment();
			}
			catch (MsgStorageNotFoundException)
			{
				StorageGlobals.ContextTraceError<int>(ExTraceGlobals.CcInboundTnefTracer, "InboundMsgConverter::AttachmentNotFound (index = {0})", attachmentIndex);
			}
		}

		protected override void PromoteAttachDataObject()
		{
			if (this.PropertyReader.AttachMethod == 5)
			{
				using (MsgStorageReader embeddedMessageReader = this.PropertyReader.GetEmbeddedMessageReader())
				{
					using (InboundMessageWriter inboundMessageWriter = base.MessageWriter.OpenAttachedMessageWriter())
					{
						InboundMsgConverter inboundMsgConverter = new InboundMsgConverter(inboundMessageWriter);
						inboundMsgConverter.InternalConvertToItem(embeddedMessageReader);
						inboundMessageWriter.Commit();
					}
					return;
				}
			}
			if (this.PropertyReader.AttachMethod == 6)
			{
				using (Stream valueReadStream = this.PropertyReader.GetValueReadStream())
				{
					using (Stream stream = base.MessageWriter.OpenOleAttachmentDataStream())
					{
						Util.StreamHandler.CopyStreamData(valueReadStream, stream);
					}
				}
			}
		}

		protected override void PromoteInternetCpidProperty()
		{
			this.internetCpid = (int)this.PropertyReader.ReadValue();
			base.SetProperty(InternalSchema.InternetCpid, this.internetCpid);
		}

		private void PromotePropertyList()
		{
			while (this.PropertyReader.ReadNextProperty())
			{
				NativeStorePropertyDefinition nativeStorePropertyDefinition = this.CreatePropertyDefinition();
				if (nativeStorePropertyDefinition != null)
				{
					AbstractInboundConverter.IPromotionRule propertyPromotionRule = base.GetPropertyPromotionRule(nativeStorePropertyDefinition);
					if (propertyPromotionRule != null)
					{
						propertyPromotionRule.PromoteProperty(this, nativeStorePropertyDefinition);
					}
				}
			}
		}

		protected override bool IsLargeValue()
		{
			return this.PropertyReader.IsLargeValue;
		}

		protected override Stream OpenValueReadStream(out int skipTrailingNulls)
		{
			TnefPropertyType propertyType = this.PropertyReader.PropertyType;
			skipTrailingNulls = ((propertyType == TnefPropertyType.Unicode) ? 2 : 0);
			return this.PropertyReader.GetValueReadStream();
		}

		private NativeStorePropertyDefinition CreatePropertyDefinition()
		{
			TnefNameId? namedProperty = this.PropertyReader.IsNamedProperty ? new TnefNameId?(this.PropertyReader.PropertyNameId) : null;
			return base.CreatePropertyDefinition(this.PropertyReader.PropertyTag, namedProperty);
		}

		protected override object ReadValue()
		{
			if (this.IsLargeValue())
			{
				StorageGlobals.ContextTraceError(ExTraceGlobals.CcInboundTnefTracer, "InboundMsgConverter::ReadValue: large property value");
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent);
			}
			return ExTimeZoneHelperForMigrationOnly.ToExDateTimeIfObjectIsDateTime(this.PropertyReader.ReadValue());
		}

		private bool InternalReadBool()
		{
			return (bool)this.PropertyReader.ReadValue();
		}

		private Stream InternalGetValueReadStream(TnefPropertyTag propertyTag)
		{
			return this.PropertyReader.GetValueReadStream(propertyTag);
		}

		private MsgStoragePropertyReader PropertyReader
		{
			get
			{
				return this.reader.PropertyReader;
			}
		}

		private bool isRtfInSync;

		private bool isTextBodyFound;

		private bool isRtfBodyFound;

		private bool isHtmlBodyFound;

		private int internetCpid;

		private MsgStorageReader reader;
	}
}
