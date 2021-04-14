using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.ContentTypes.Tnef;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class InboundTnefConverter : AbstractInboundConverter
	{
		internal InboundTnefConverter(InboundMessageWriter writer) : base(writer, new AbstractInboundConverter.TransmittablePropertyPromotionRule())
		{
		}

		internal void ConvertToItem(Stream tnefStream, int primaryCodepage, bool isSummaryTnef)
		{
			base.SetProperty(InternalSchema.SendRichInfo, true);
			TnefReader tnefReader = new TnefReader(tnefStream, primaryCodepage, TnefComplianceMode.Loose);
			this.ConvertToItem(tnefReader, isSummaryTnef);
		}

		private void ConvertToItem(TnefReader reader, bool isSummaryTnef)
		{
			if (this.IsReplicationMessage)
			{
				PersistablePropertyBag persistablePropertyBag = CoreObject.GetPersistablePropertyBag(base.MessageWriter.CoreItem);
				if (persistablePropertyBag != null)
				{
					persistablePropertyBag.Context.IsValidationDisabled = true;
				}
			}
			this.reader = reader;
			this.isSummaryTnef = isSummaryTnef;
			this.isRecipientTablePromoted = false;
			while (this.reader.ReadNextAttribute())
			{
				this.CheckTnefComplianceStatus();
				this.ReadAttribute();
			}
			this.ProcessEndTnef();
			PropertyBagSaveFlags propertyBagSaveFlags = PropertyBagSaveFlags.IgnoreMapiComputedErrors | base.ConversionOptions.GetSaveFlags(base.IsTopLevelMessage);
			CoreObject.GetPersistablePropertyBag(base.CoreItem).SaveFlags |= propertyBagSaveFlags;
		}

		private void ReadAttribute()
		{
			TnefPropertyTag @null = TnefPropertyTag.Null;
			if (base.CurrentComponentType == ConversionComponentType.FileAttachment)
			{
				TnefAttributeTag attributeTag = this.reader.AttributeTag;
				if (attributeTag <= TnefAttributeTag.AttachModifyDate)
				{
					if (attributeTag != TnefAttributeTag.AttachTitle)
					{
						switch (attributeTag)
						{
						case TnefAttributeTag.AttachCreateDate:
						case TnefAttributeTag.AttachModifyDate:
							break;
						default:
							goto IL_71;
						}
					}
				}
				else
				{
					switch (attributeTag)
					{
					case TnefAttributeTag.AttachData:
					case TnefAttributeTag.AttachMetaFile:
						break;
					case (TnefAttributeTag)426000:
						goto IL_71;
					default:
						if (attributeTag != TnefAttributeTag.AttachTransportFilename && attributeTag != TnefAttributeTag.Attachment)
						{
							goto IL_71;
						}
						break;
					}
				}
				this.ParseAttachmentProperties();
				goto IL_77;
				IL_71:
				this.EndAttachment();
			}
			IL_77:
			if (base.CurrentComponentType == ConversionComponentType.Message)
			{
				TnefAttributeTag attributeTag2 = this.reader.AttributeTag;
				if (attributeTag2 <= TnefAttributeTag.RequestResponse)
				{
					if (attributeTag2 <= TnefAttributeTag.Body)
					{
						if (attributeTag2 <= TnefAttributeTag.Subject)
						{
							if (attributeTag2 != TnefAttributeTag.From && attributeTag2 != TnefAttributeTag.Subject)
							{
								goto IL_211;
							}
						}
						else
						{
							switch (attributeTag2)
							{
							case TnefAttributeTag.MessageId:
							case TnefAttributeTag.ParentId:
							case TnefAttributeTag.ConversationId:
								break;
							default:
								if (attributeTag2 != TnefAttributeTag.Body)
								{
									goto IL_211;
								}
								break;
							}
						}
					}
					else if (attributeTag2 <= TnefAttributeTag.DateReceived)
					{
						switch (attributeTag2)
						{
						case TnefAttributeTag.DateStart:
						case TnefAttributeTag.DateEnd:
							break;
						default:
							switch (attributeTag2)
							{
							case TnefAttributeTag.DateSent:
							case TnefAttributeTag.DateReceived:
								break;
							default:
								goto IL_211;
							}
							break;
						}
					}
					else if (attributeTag2 != TnefAttributeTag.DateModified && attributeTag2 != TnefAttributeTag.RequestResponse)
					{
						goto IL_211;
					}
				}
				else if (attributeTag2 <= TnefAttributeTag.MessageStatus)
				{
					if (attributeTag2 <= TnefAttributeTag.AidOwner)
					{
						if (attributeTag2 != TnefAttributeTag.Priority && attributeTag2 != TnefAttributeTag.AidOwner)
						{
							goto IL_211;
						}
					}
					else
					{
						switch (attributeTag2)
						{
						case TnefAttributeTag.Owner:
						case TnefAttributeTag.SentFor:
						case TnefAttributeTag.Delegate:
							break;
						default:
							if (attributeTag2 != TnefAttributeTag.MessageStatus)
							{
								goto IL_211;
							}
							this.ParseMapiProperties(true);
							return;
						}
					}
				}
				else if (attributeTag2 <= TnefAttributeTag.OriginalMessageClass)
				{
					switch (attributeTag2)
					{
					case TnefAttributeTag.AttachRenderData:
						this.StartNewAttachment();
						this.ParseAttachRenderData();
						return;
					case TnefAttributeTag.MapiProperties:
						break;
					case TnefAttributeTag.RecipientTable:
						this.ParseRecipientTable();
						return;
					case TnefAttributeTag.Attachment:
					case (TnefAttributeTag)430086:
						goto IL_211;
					case TnefAttributeTag.OemCodepage:
						base.SetProperty(InternalSchema.Codepage, this.reader.MessageCodepage);
						return;
					default:
						if (attributeTag2 != TnefAttributeTag.OriginalMessageClass)
						{
							goto IL_211;
						}
						break;
					}
				}
				else if (attributeTag2 != TnefAttributeTag.MessageClass)
				{
					if (attributeTag2 != TnefAttributeTag.TnefVersion)
					{
						goto IL_211;
					}
					return;
				}
				this.ParseMapiProperties(false);
				return;
				IL_211:
				StorageGlobals.ContextTraceDebug<uint>(ExTraceGlobals.CcInboundTnefTracer, "Unknown TNEF attribute encountered: 0x{0:X}. Ignoring.", (uint)this.reader.AttributeTag);
			}
		}

		private void ParseMapiProperties(bool forceTransmittable)
		{
			TnefPropertyReader propertyReader = this.reader.PropertyReader;
			while (propertyReader.ReadNextProperty())
			{
				this.CheckTnefComplianceStatus();
				this.ParseTnefProperty(propertyReader, forceTransmittable);
			}
		}

		private void ParseTnefProperty(TnefPropertyReader propertyReader, bool forceTransmittable)
		{
			this.CheckTnefComplianceStatus();
			TnefNameId? namedProperty = propertyReader.IsNamedProperty ? new TnefNameId?(propertyReader.PropertyNameId) : null;
			NativeStorePropertyDefinition nativeStorePropertyDefinition = base.CreatePropertyDefinition(propertyReader.PropertyTag, namedProperty);
			if (nativeStorePropertyDefinition == null)
			{
				return;
			}
			AbstractInboundConverter.IPromotionRule propertyPromotionRule = base.GetPropertyPromotionRule(nativeStorePropertyDefinition);
			if (propertyPromotionRule != null)
			{
				propertyPromotionRule.PromoteProperty(this, nativeStorePropertyDefinition);
			}
		}

		protected override bool IsLargeValue()
		{
			return !this.PropertyReader.PropertyTag.IsMultiValued && this.PropertyReader.IsLargeValue;
		}

		protected override object ReadValue()
		{
			if (this.IsLargeValue())
			{
				StorageGlobals.ContextTraceError(ExTraceGlobals.CcInboundTnefTracer, "InboundTnefConverter::ReadValue: large property value");
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent);
			}
			if (this.PropertyReader.IsMultiValuedProperty)
			{
				Type elementType = InternalSchema.ClrTypeFromPropTagType(ConvertUtils.GetPropertyBaseType(this.PropertyReader.PropertyTag));
				List<object> list = new List<object>();
				while (this.PropertyReader.ReadNextValue())
				{
					if (this.PropertyReader.IsLargeValue)
					{
						throw new ConversionFailedException(ConversionFailureReason.CorruptContent, ServerStrings.LargeMultivaluedPropertiesNotSupportedInTNEF, null);
					}
					object obj = this.ReadSingleValue();
					if (obj == null)
					{
						throw new ConversionFailedException(ConversionFailureReason.CorruptContent, ServerStrings.InvalidTnef, null);
					}
					list.Add(obj);
				}
				Array array = Array.CreateInstance(elementType, list.Count);
				for (int i = 0; i < array.Length; i++)
				{
					array.SetValue(list[i], i);
				}
				return array;
			}
			return this.ReadSingleValue();
		}

		private object ReadSingleValue()
		{
			TnefPropertyType tnefType = this.PropertyReader.PropertyTag.TnefType;
			if (tnefType == TnefPropertyType.AppTime || tnefType == (TnefPropertyType)4103)
			{
				double num = 0.0;
				DateTime dateTime = this.PropertyReader.ReadValueAsDateTime();
				try
				{
					num = ConvertUtils.GetOADate(dateTime);
				}
				catch (OverflowException arg)
				{
					StorageGlobals.ContextTraceError<DateTime, OverflowException>(ExTraceGlobals.CcInboundTnefTracer, "InboundTnefConverter::ReadPropertyReaderValue: OverflowException processing date {0}, {1}.", dateTime, arg);
				}
				return num;
			}
			return ExTimeZoneHelperForMigrationOnly.ToExDateTimeIfObjectIsDateTime(this.PropertyReader.ReadValue());
		}

		protected override Stream OpenValueReadStream(out int skipTrailingNulls)
		{
			TnefPropertyType tnefType = this.PropertyReader.PropertyTag.TnefType;
			TnefPropertyType tnefPropertyType = tnefType;
			switch (tnefPropertyType)
			{
			case TnefPropertyType.String8:
			{
				skipTrailingNulls = 1;
				TextToText textToText = new TextToText(TextToTextConversionMode.ConvertCodePageOnly);
				textToText.InputEncoding = this.GetString8Encoding();
				textToText.OutputEncoding = ConvertUtils.UnicodeEncoding;
				return new ConverterStream(this.PropertyReader.GetRawValueReadStream(), textToText, ConverterStreamAccess.Read);
			}
			case TnefPropertyType.Unicode:
				skipTrailingNulls = 2;
				return this.PropertyReader.GetRawValueReadStream();
			default:
				if (tnefPropertyType == TnefPropertyType.Binary)
				{
					skipTrailingNulls = 0;
					return this.PropertyReader.GetRawValueReadStream();
				}
				StorageGlobals.ContextTraceError(ExTraceGlobals.CcInboundTnefTracer, "InboundTnefConverter::StreamLargeProperty: only supports binary and string properties.");
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent);
			}
		}

		private void ParseRecipientTable()
		{
			string valueOrDefault = base.CoreItem.PropertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass);
			if (base.ConversionOptions.IsSenderTrusted || !base.MessageWriter.IsTopLevelWriter || ObjectClass.IsNonSendableWithRecipients(valueOrDefault) || ObjectClass.IsDsn(valueOrDefault))
			{
				TnefPropertyReader propertyReader = this.reader.PropertyReader;
				while (propertyReader.ReadNextRow())
				{
					this.NewRecipient();
					while (propertyReader.ReadNextProperty())
					{
						this.ParseTnefProperty(propertyReader, false);
					}
					this.EndRecipient();
					this.isRecipientTablePromoted = true;
				}
			}
		}

		private void NewRecipient()
		{
			base.MessageWriter.StartNewRecipient();
		}

		private void EndRecipient()
		{
			base.MessageWriter.EndRecipient();
		}

		private void StartNewAttachment()
		{
			base.MessageWriter.StartNewAttachment();
		}

		private void EndAttachment()
		{
			base.MessageWriter.EndAttachment();
		}

		private void ParseAttachRenderData()
		{
			TnefPropertyReader propertyReader = this.reader.PropertyReader;
			while (propertyReader.ReadNextProperty())
			{
				if (propertyReader.PropertyTag != TnefPropertyTag.AttachMethod)
				{
					this.ParseTnefProperty(propertyReader, false);
				}
			}
		}

		private void ParseAttachmentProperties()
		{
			TnefPropertyReader propertyReader = this.reader.PropertyReader;
			while (propertyReader.ReadNextProperty())
			{
				this.ParseTnefProperty(propertyReader, false);
			}
		}

		protected override void PromoteAttachDataObject()
		{
			if (this.PropertyReader.IsEmbeddedMessage)
			{
				using (TnefReader embeddedMessageReader = this.PropertyReader.GetEmbeddedMessageReader())
				{
					using (InboundMessageWriter inboundMessageWriter = base.MessageWriter.OpenAttachedMessageWriter())
					{
						if (this.IsReplicationMessage)
						{
							inboundMessageWriter.ForceParticipantResolution = this.ResolveParticipantsOnAttachments;
						}
						else
						{
							inboundMessageWriter.ForceParticipantResolution = base.ConversionOptions.ResolveRecipientsInAttachedMessages;
						}
						new InboundTnefConverter(inboundMessageWriter)
						{
							IsReplicationMessage = this.IsReplicationMessage
						}.ConvertToItem(embeddedMessageReader, this.isSummaryTnef);
						inboundMessageWriter.Commit();
					}
					return;
				}
			}
			if (this.PropertyReader.ObjectIid == MimeConstants.IID_IStorage)
			{
				using (Stream rawValueReadStream = this.PropertyReader.GetRawValueReadStream())
				{
					using (Stream stream = base.MessageWriter.OpenOleAttachmentDataStream())
					{
						Util.StreamHandler.CopyStreamData(rawValueReadStream, stream, null, 0, 131072);
					}
				}
			}
		}

		protected override void PromoteInternetCpidProperty()
		{
			int num = this.PropertyReader.ReadValueAsInt32();
			this.internetCpid = num;
			base.SetProperty(InternalSchema.InternetCpid, num);
		}

		protected override bool CanPromoteMimeOnlyProperties()
		{
			return !base.IsTopLevelMessage;
		}

		protected override void PromoteMessageClass()
		{
			string text = (string)this.ReadValue();
			base.SetProperty(InternalSchema.ItemClass, text);
			if (ObjectClass.IsOfClass(text, "IPM.Replication"))
			{
				base.ConversionOptions.ClearCategories = false;
				if (base.IsTopLevelMessage && this.isSummaryTnef && base.ConversionOptions.IsSenderTrusted)
				{
					base.MessageWriter.SuppressLimitChecks();
				}
			}
		}

		protected override void PromoteBodyProperty(StorePropertyDefinition property)
		{
			if (base.CurrentComponentType != ConversionComponentType.Message)
			{
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent);
			}
			if (property == InternalSchema.RtfInSync)
			{
				base.PromoteProperty(property, false);
				return;
			}
			if (this.promotedBodyProperty == null)
			{
				this.promotedBodyProperty = property;
				this.StreamBody(property, this.PropertyReader);
				return;
			}
			if (property == InternalSchema.TextBody)
			{
				if (this.isSummaryTnef)
				{
					base.MessageWriter.DeleteMessageProperty(InternalSchema.RtfBody);
				}
				this.promotedBodyProperty = InternalSchema.TextBody;
				this.StreamBody(property, this.PropertyReader);
			}
			if (property == InternalSchema.HtmlBody)
			{
				this.StreamBody(property, this.PropertyReader);
			}
			if (property == InternalSchema.RtfBody)
			{
				if (this.promotedBodyProperty == InternalSchema.TextBody && this.isSummaryTnef)
				{
					return;
				}
				this.StreamBody(property, this.PropertyReader);
			}
		}

		private void StreamBody(StorePropertyDefinition property, TnefPropertyReader reader)
		{
			Charset charset = null;
			if (this.internetCpid != 0)
			{
				ConvertUtils.TryGetValidCharset(this.internetCpid, out charset);
			}
			string text = (charset != null) ? charset.Name : null;
			BodyWriteConfiguration bodyWriteConfiguration = null;
			if (property == InternalSchema.TextBody)
			{
				bodyWriteConfiguration = new BodyWriteConfiguration(BodyFormat.TextPlain, ConvertUtils.UnicodeCharset.Name);
				bodyWriteConfiguration.SetTargetFormat(BodyFormat.TextPlain, text);
			}
			else if (property == InternalSchema.HtmlBody)
			{
				bodyWriteConfiguration = new BodyWriteConfiguration(BodyFormat.TextHtml, text);
				bodyWriteConfiguration.SetTargetFormat(BodyFormat.TextHtml, text);
			}
			else if (property == InternalSchema.RtfBody)
			{
				bodyWriteConfiguration = new BodyWriteConfiguration(BodyFormat.ApplicationRtf);
				bodyWriteConfiguration.SetTargetFormat(BodyFormat.ApplicationRtf, text);
			}
			int trailingNulls;
			using (Stream stream = this.OpenValueReadStream(out trailingNulls))
			{
				base.CoreItem.CharsetDetector.DetectionOptions = base.ConversionOptions.DetectionOptions;
				using (Stream stream2 = base.CoreItem.Body.InternalOpenWriteStream(bodyWriteConfiguration, null))
				{
					Util.StreamHandler.CopyStreamData(stream, stream2, null, trailingNulls, 65536);
				}
			}
		}

		private void ProcessEndTnef()
		{
			ConversionComponentType currentComponentType = base.CurrentComponentType;
			if (currentComponentType == ConversionComponentType.FileAttachment)
			{
				this.EndAttachment();
			}
			this.CheckTnefComplianceStatus();
		}

		internal void Undo()
		{
			if (base.MessageWriter != null)
			{
				base.MessageWriter.UndoTnef();
			}
		}

		private void CheckTnefComplianceStatus()
		{
			TnefComplianceStatus complianceStatus = this.reader.ComplianceStatus;
			if ((complianceStatus & ~(TnefComplianceStatus.InvalidAttributeChecksum | TnefComplianceStatus.InvalidMessageCodepage | TnefComplianceStatus.InvalidDate)) != TnefComplianceStatus.Compliant)
			{
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent, ServerStrings.ConversionCorruptTnef((int)complianceStatus), null);
			}
		}

		private Encoding GetString8Encoding()
		{
			if (this.string8Encoding == null)
			{
				Charset defaultWindowsCharset;
				if (!ConvertUtils.TryGetValidCharset(this.reader.MessageCodepage, out defaultWindowsCharset))
				{
					defaultWindowsCharset = Charset.DefaultWindowsCharset;
				}
				this.string8Encoding = defaultWindowsCharset.GetEncoding();
			}
			return this.string8Encoding;
		}

		internal bool IsRecipientTablePromoted
		{
			get
			{
				return this.isRecipientTablePromoted;
			}
		}

		internal StorePropertyDefinition PromotedBodyProperty
		{
			get
			{
				return this.promotedBodyProperty;
			}
		}

		internal TnefPropertyReader PropertyReader
		{
			get
			{
				return this.reader.PropertyReader;
			}
		}

		internal bool ResolveParticipantsOnAttachments
		{
			get
			{
				return this.resolveParticipantsOnAttachments;
			}
			set
			{
				this.resolveParticipantsOnAttachments = value;
			}
		}

		internal bool IsReplicationMessage
		{
			get
			{
				return this.isReplicationMessage;
			}
			set
			{
				this.isReplicationMessage = value;
			}
		}

		private TnefReader reader;

		private Encoding string8Encoding;

		private int internetCpid;

		private bool isSummaryTnef;

		private bool isReplicationMessage;

		private StorePropertyDefinition promotedBodyProperty;

		private bool isRecipientTablePromoted;

		private bool resolveParticipantsOnAttachments;
	}
}
