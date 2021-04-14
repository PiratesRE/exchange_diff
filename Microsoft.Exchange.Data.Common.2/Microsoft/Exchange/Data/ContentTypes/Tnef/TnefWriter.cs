using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.ContentTypes.iCalendar;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Internal;

namespace Microsoft.Exchange.Data.ContentTypes.Tnef
{
	public class TnefWriter : IDisposable
	{
		public TnefWriter(Stream outputStream, short attachmentKey) : this(outputStream, attachmentKey, 0, TnefWriterFlags.NoStandardAttributes)
		{
		}

		public TnefWriter(Stream outputStream, short attachmentKey, int messageCodePage) : this(outputStream, attachmentKey, messageCodePage, (TnefWriterFlags)0)
		{
		}

		public TnefWriter(Stream outputStream, short attachmentKey, int messageCodePage, TnefWriterFlags flags)
		{
			this.canFlush = true;
			base..ctor();
			if (outputStream == null)
			{
				throw new ArgumentNullException("outputStream");
			}
			if (!outputStream.CanWrite)
			{
				throw new NotSupportedException(TnefStrings.StreamDoesNotSupportWrite);
			}
			this.flags = flags;
			this.outputStream = outputStream;
			this.writeStream = outputStream;
			if (!outputStream.CanSeek)
			{
				this.writeStream = ApplicationServices.Provider.CreateTemporaryStorage();
			}
			this.unicodeEncoder = Encoding.Unicode.GetEncoder();
			this.writeBuffer = new byte[4096];
			this.charBuffer = new char[1024];
			this.byteBuffer = new byte[1024];
			this.fabricatedBuffer = new byte[512];
			this.attachmentKey = attachmentKey;
			this.directWrite = true;
			this.WriteDword(574529400);
			this.WriteWord(this.attachmentKey);
			this.totalLength += 6;
			if (!this.outputStream.CanSeek)
			{
				this.FlushWriteStreamToOutput();
			}
			this.writeState = TnefWriter.WriteState.BeforeAttribute;
			if ((this.flags & TnefWriterFlags.NoStandardAttributes) == (TnefWriterFlags)0)
			{
				this.WriteTnefVersion();
				this.WriteOemCodePage(messageCodePage);
			}
			else
			{
				this.SetMessageCodePage(messageCodePage);
			}
			if (this.string8Encoder == null)
			{
				this.string8Encoder = Charset.DefaultWindowsCharset.GetEncoding().GetEncoder();
			}
		}

		private TnefWriter(TnefWriter parent, int messageCodePage)
		{
			this.canFlush = true;
			base..ctor();
			this.parent = parent;
			this.parent.Child = this;
			this.flags = parent.flags;
			this.attachmentKey = parent.attachmentKey;
			this.unicodeEncoder = parent.unicodeEncoder;
			this.SetMessageCodePage(messageCodePage);
			this.outputStream = parent.outputStream;
			this.streamOffset = parent.streamOffset;
			this.writeStream = parent.writeStream;
			this.writeStreamOffset = parent.writeStreamOffset;
			this.writeBuffer = parent.writeBuffer;
			this.writeOffset = parent.writeOffset;
			this.charBuffer = parent.charBuffer;
			this.byteBuffer = parent.byteBuffer;
			this.fabricatedBuffer = parent.fabricatedBuffer;
			this.directWrite = true;
			this.WriteDword(574529400);
			this.WriteWord(this.attachmentKey);
			this.totalLength += 6;
			this.writeState = TnefWriter.WriteState.BeforeAttribute;
			if ((this.flags & TnefWriterFlags.NoStandardAttributes) == (TnefWriterFlags)0)
			{
				this.WriteTnefVersion();
				this.WriteOemCodePage(messageCodePage);
			}
			else
			{
				this.SetMessageCodePage(messageCodePage);
			}
			if (this.string8Encoder == null)
			{
				this.string8Encoder = Charset.DefaultWindowsCharset.GetEncoding().GetEncoder();
			}
		}

		public int MessageCodePage
		{
			get
			{
				return this.messageCodePage;
			}
		}

		private int StreamOffset
		{
			get
			{
				return this.streamOffset + this.writeOffset;
			}
		}

		public void Flush()
		{
			this.AssertGoodToUse(true);
			if (this.writeState > TnefWriter.WriteState.BeforeAttribute)
			{
				this.EndAttribute();
			}
			if (this.parent == null)
			{
				this.FlushBuffer();
			}
		}

		public void Close()
		{
			this.Dispose();
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.disposing = true;
				if (this.outputStream == null)
				{
					return;
				}
				if (this.canFlush)
				{
					if (this.Child != null)
					{
						if (this.Child is TnefWriterStreamWrapper)
						{
							(this.Child as TnefWriterStreamWrapper).Dispose();
						}
						else if (this.Child is TnefWriter)
						{
							(this.Child as TnefWriter).Dispose();
						}
					}
					this.Flush();
					if (this.parent != null)
					{
						this.parent.streamOffset = this.streamOffset;
						this.parent.attributeLength += this.totalLength;
						this.parent.valueLength += this.totalLength;
						TnefWriter tnefWriter = this.parent;
						tnefWriter.checksum += this.checksum;
						this.parent.writeOffset = this.writeOffset;
						this.parent.Child = null;
					}
					else
					{
						if (!this.outputStream.CanSeek)
						{
							this.writeStream.Dispose();
						}
						this.outputStream.Dispose();
					}
				}
			}
			this.outputStream = null;
			this.writeStream = null;
			this.writeBuffer = null;
			this.byteBuffer = null;
			this.charBuffer = null;
			this.fabricatedBuffer = null;
			this.unicodeEncoder = null;
			this.string8Encoder = null;
		}

		public void WriteTnefVersion()
		{
			this.AssertGoodToUse(true);
			TnefBitConverter.GetBytes(this.byteBuffer, 0, 65536);
			this.StartAttribute(TnefAttributeTag.TnefVersion, TnefAttributeLevel.Message);
			this.WriteAttributeRawValue(this.byteBuffer, 0, 4);
			this.EndAttribute();
		}

		public void WriteOemCodePage(int messageCodePage)
		{
			this.AssertGoodToUse(true);
			this.SetMessageCodePage(messageCodePage);
			TnefBitConverter.GetBytes(this.byteBuffer, 0, this.messageCodePage);
			TnefBitConverter.GetBytes(this.byteBuffer, 4, 0);
			this.StartAttribute(TnefAttributeTag.OemCodepage, TnefAttributeLevel.Message);
			this.WriteAttributeRawValue(this.byteBuffer, 0, 8);
			this.EndAttribute();
		}

		public void SetMessageCodePage(int messageCodePage)
		{
			if (this.messageCodePage != messageCodePage)
			{
				Encoding encoding;
				if (messageCodePage == 0)
				{
					encoding = Charset.DefaultWindowsCharset.GetEncoding();
					messageCodePage = CodePageMap.GetCodePage(encoding);
				}
				else
				{
					encoding = Charset.GetEncoding(messageCodePage);
				}
				int codePage = CodePageMap.GetCodePage(encoding);
				if (TnefCommon.IsUnicodeCodepage(codePage))
				{
					messageCodePage = 1252;
					encoding = Charset.GetEncoding(messageCodePage);
				}
				this.string8Encoder = encoding.GetEncoder();
				this.messageCodePage = messageCodePage;
			}
		}

		public void WriteAttribute(TnefReader reader)
		{
			this.AssertGoodToUse(true);
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (reader.InputStream == null)
			{
				throw new ObjectDisposedException("TnefReader");
			}
			if (reader.ReadStateValue != TnefReader.ReadState.BeginAttribute)
			{
				throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationMustBeAtTheBeginningOfAttribute);
			}
			this.StartAttribute(reader.AttributeTag, reader.AttributeLevel);
			int count;
			while ((count = reader.ReadAttributeRawValue(this.byteBuffer, 0, this.byteBuffer.Length)) != 0)
			{
				this.WriteAttributeRawValue(this.byteBuffer, 0, count);
			}
			this.EndAttribute();
		}

		public void WriteProperty(TnefPropertyReader propertyReader)
		{
			this.AssertGoodToUse(true);
			if (propertyReader.Reader == null)
			{
				throw new ArgumentException("TnefPropertyReader object is not properly initialized", "propertyReader");
			}
			TnefReader reader = propertyReader.Reader;
			if (reader.InputStream == null)
			{
				throw new ObjectDisposedException("TnefReader");
			}
			if (reader.ReadStateValue != TnefReader.ReadState.BeginProperty && (reader.ReadStateValue != TnefReader.ReadState.BeginPropertyValue || reader.PropertyTag.IsMultiValued))
			{
				throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationMustBeAtTheBeginningOfProperty);
			}
			if (!reader.PropertyTag.IsNamed)
			{
				this.StartProperty(reader.PropertyTag);
			}
			else if (reader.PropertyNameId.Kind == TnefNameIdKind.Id)
			{
				this.StartProperty(reader.PropertyTag, reader.PropertyNameId.PropertySetGuid, reader.PropertyNameId.Id);
			}
			else
			{
				this.StartProperty(reader.PropertyTag, reader.PropertyNameId.PropertySetGuid, reader.PropertyNameId.Name);
			}
			if (reader.PropertyTag.IsMultiValued)
			{
				while (reader.ReadNextPropertyValue())
				{
					this.StartPropertyValue();
					int count;
					while ((count = reader.ReadPropertyRawValue(this.byteBuffer, 0, this.byteBuffer.Length, false)) != 0)
					{
						this.WritePropertyRawValue(this.byteBuffer, 0, count);
					}
				}
			}
			else if (reader.PropertyTag.TnefType != TnefPropertyType.Null)
			{
				this.StartPropertyValue();
				if (reader.PropertyTag.TnefType == TnefPropertyType.Object)
				{
					this.WriteGuid(reader.PropertyValueOleIID);
				}
				int count;
				while ((count = reader.ReadPropertyRawValue(this.byteBuffer, 0, this.byteBuffer.Length, false)) != 0)
				{
					this.WritePropertyRawValue(this.byteBuffer, 0, count);
				}
			}
			this.EndProperty();
		}

		public void WriteAllProperties(TnefPropertyReader reader)
		{
			while (reader.ReadNextProperty())
			{
				this.WriteProperty(reader);
			}
		}

		public void StartAttribute(TnefAttributeTag tag, TnefAttributeLevel level)
		{
			this.AssertGoodToUse(true);
			if (this.writeState != TnefWriter.WriteState.BeforeAttribute)
			{
				this.EndAttribute();
			}
			this.attributeLevel = level;
			this.attributeTag = tag;
			this.legacyAttribute = this.IsLegacyAttribute();
			this.directWrite = true;
			this.WriteByte((byte)this.attributeLevel);
			this.WriteDword((int)this.attributeTag);
			this.attributeLengthOffset = this.StreamOffset;
			this.WriteDword(0);
			this.attributeLength = 0;
			this.attributeStartChecksum = this.checksum;
			this.totalLength += 9;
			this.writeState = TnefWriter.WriteState.BeginAttribute;
		}

		public void WriteAttributeRawValue(byte[] buffer, int offset, int count)
		{
			this.AssertGoodToUse(true);
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset > buffer.Length || offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", TnefStrings.OffsetOutOfRange);
			}
			if (count > buffer.Length || count < 0)
			{
				throw new ArgumentOutOfRangeException("count", TnefStrings.CountOutOfRange);
			}
			if (count + offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("count", TnefStrings.CountTooLarge);
			}
			if (this.writeState == TnefWriter.WriteState.BeginAttribute)
			{
				this.writeState = TnefWriter.WriteState.WriteAttributeValue;
			}
			if (this.writeState != TnefWriter.WriteState.WriteAttributeValue)
			{
				throw new InvalidOperationException(TnefStrings.WriterInvalidOperation);
			}
			this.writeState = TnefWriter.WriteState.WriteAttributeValue;
			this.WriteBinary(buffer, offset, count);
		}

		public void StartRow()
		{
			this.AssertGoodToUse(true);
			if (this.attributeTag != TnefAttributeTag.RecipientTable)
			{
				throw new InvalidOperationException(TnefStrings.WriterInvalidOperationStartRowNotInRecipientTable);
			}
			if (this.writeState != TnefWriter.WriteState.BeforeRow)
			{
				if (this.writeState == TnefWriter.WriteState.BeginAttribute)
				{
					this.rowCountOffset = this.StreamOffset;
					this.WriteDword(0);
					this.rowCount = 0;
					this.writeState = TnefWriter.WriteState.BeforeRow;
				}
				else
				{
					if (this.writeState == TnefWriter.WriteState.BeforeAttribute)
					{
						throw new InvalidOperationException(TnefStrings.WriterInvalidOperation);
					}
					this.EndRow();
				}
			}
			this.rowCount++;
			this.propertyCountOffset = this.StreamOffset;
			this.WriteDword(0);
			this.propertyCount = 0;
			this.writeState = TnefWriter.WriteState.BeforeProperty;
		}

		private void PrepareToStartProperty(TnefPropertyTag tag, bool nameAvailable)
		{
			if (tag.IsNamed != nameAvailable)
			{
				throw new InvalidOperationException(tag.IsNamed ? TnefStrings.WriterInvalidOperationStartNamedPropertyNoName : TnefStrings.WriterInvalidOperationStartNormalPropertyWithName);
			}
			if (this.legacyAttribute && tag.IsNamed)
			{
				throw new NotSupportedException(TnefStrings.WriterInvalidOperationNamedPropertyInLegacyAttribute);
			}
			if (this.writeState != TnefWriter.WriteState.BeforeProperty)
			{
				if (this.writeState == TnefWriter.WriteState.BeginAttribute && this.attributeTag != TnefAttributeTag.RecipientTable)
				{
					if (!this.legacyAttribute)
					{
						this.propertyCountOffset = this.StreamOffset;
						this.WriteDword(0);
						this.propertyCount = 0;
					}
					else
					{
						this.StartLegacyAttribute();
					}
					this.writeState = TnefWriter.WriteState.BeforeProperty;
				}
				else
				{
					if (this.writeState < TnefWriter.WriteState.BeforeProperty)
					{
						throw new InvalidOperationException(TnefStrings.WriterInvalidOperation);
					}
					this.EndProperty();
				}
			}
			this.propertyTag = tag;
			this.propertyCount++;
			this.valueCount = 0;
			this.valueLength = 0;
			this.directWrite = true;
			this.string8AsUnicode = false;
			TnefPropertyType valueTnefType = tag.ValueTnefType;
			if (valueTnefType <= TnefPropertyType.Unicode)
			{
				switch (valueTnefType)
				{
				case TnefPropertyType.Null:
					if (tag.IsMultiValued)
					{
						throw new NotSupportedException(TnefStrings.WriterInvalidOperationInvalidPropertyType);
					}
					this.valueFixedLength = 0;
					return;
				case TnefPropertyType.I2:
					this.valueFixedLength = 2;
					return;
				case TnefPropertyType.Long:
					this.valueFixedLength = 4;
					return;
				case TnefPropertyType.R4:
					this.valueFixedLength = 4;
					return;
				case TnefPropertyType.Double:
					this.valueFixedLength = 8;
					return;
				case TnefPropertyType.Currency:
					this.valueFixedLength = 8;
					return;
				case TnefPropertyType.AppTime:
					this.valueFixedLength = 8;
					return;
				case (TnefPropertyType)8:
				case (TnefPropertyType)9:
				case (TnefPropertyType)12:
				case (TnefPropertyType)14:
				case (TnefPropertyType)15:
				case (TnefPropertyType)16:
				case (TnefPropertyType)17:
				case (TnefPropertyType)18:
				case (TnefPropertyType)19:
					goto IL_209;
				case TnefPropertyType.Error:
					this.valueFixedLength = 4;
					return;
				case TnefPropertyType.Boolean:
					this.valueFixedLength = 2;
					return;
				case TnefPropertyType.Object:
					if (tag.IsMultiValued)
					{
						throw new NotSupportedException(TnefStrings.WriterInvalidOperationMvObject);
					}
					this.valueFixedLength = 0;
					return;
				case TnefPropertyType.I8:
					this.valueFixedLength = 8;
					return;
				default:
					switch (valueTnefType)
					{
					case TnefPropertyType.String8:
					case TnefPropertyType.Unicode:
						break;
					default:
						goto IL_209;
					}
					break;
				}
			}
			else
			{
				if (valueTnefType == TnefPropertyType.SysTime)
				{
					this.valueFixedLength = 8;
					return;
				}
				if (valueTnefType == TnefPropertyType.ClassId)
				{
					this.valueFixedLength = 16;
					return;
				}
				if (valueTnefType != TnefPropertyType.Binary)
				{
					goto IL_209;
				}
			}
			this.valueFixedLength = 0;
			return;
			IL_209:
			throw new NotSupportedException(TnefStrings.WriterInvalidOperationInvalidPropertyType);
		}

		private void CompleteStartProperty()
		{
			if (this.propertyTag.IsMultiValued)
			{
				this.valueCountOffset = this.StreamOffset;
				this.WriteDword(0);
			}
			else if (this.propertyTag.ValueTnefType == TnefPropertyType.String8 || this.propertyTag.ValueTnefType == TnefPropertyType.Unicode || this.propertyTag.ValueTnefType == TnefPropertyType.Binary || this.propertyTag.ValueTnefType == TnefPropertyType.Object)
			{
				this.WriteDword(1);
			}
			this.writeState = TnefWriter.WriteState.BeforePropertyValue;
		}

		public void StartProperty(TnefPropertyTag tag)
		{
			this.AssertGoodToUse(true);
			this.PrepareToStartProperty(tag, false);
			if (!this.legacyAttribute)
			{
				this.WriteDword(tag);
				this.CompleteStartProperty();
				return;
			}
			this.StartLegacyAttributeProperty();
			this.writeState = TnefWriter.WriteState.BeforePropertyValue;
		}

		public void StartProperty(TnefPropertyTag tag, Guid propSetGuid, string name)
		{
			this.AssertGoodToUse(true);
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length > 10238)
			{
				throw new ArgumentException(TnefStrings.WriterPropertyNameEmptyOrTooLong, "name");
			}
			this.PrepareToStartProperty(tag, true);
			this.WriteDword(tag);
			this.WriteGuid(propSetGuid);
			this.WriteDword(1);
			int num = this.StreamOffset;
			this.WriteDword(0);
			this.WriteUnicodeString(name, true);
			this.WriteWord(0);
			int num2 = this.StreamOffset - num - 4;
			if (num2 % 4 != 0)
			{
				this.WriteBinary(TnefCommon.Padding, 0, (4 - num2 % 4) % 4);
			}
			this.ReWriteDword(num, num2);
			this.CompleteStartProperty();
		}

		public void StartProperty(TnefPropertyTag tag, Guid propSetGuid, int nameId)
		{
			this.AssertGoodToUse(true);
			this.PrepareToStartProperty(tag, true);
			this.WriteDword(tag);
			this.WriteGuid(propSetGuid);
			this.WriteDword(0);
			this.WriteDword(nameId);
			this.CompleteStartProperty();
		}

		public void StartPropertyValue()
		{
			this.AssertGoodToUse(true);
			if (this.writeState <= TnefWriter.WriteState.BeforeProperty)
			{
				throw new InvalidOperationException(TnefStrings.WriterInvalidOperation);
			}
			if (!this.propertyTag.IsMultiValued)
			{
				if (this.valueCount != 0)
				{
					throw new InvalidOperationException(TnefStrings.WriterInvalidOperationMoreThanOneValueForSingleValuedProperty);
				}
			}
			else if (this.writeState >= TnefWriter.WriteState.BeginPropertyValue)
			{
				this.EndPropertyValue();
			}
			if (!this.legacyAttribute && (this.propertyTag.ValueTnefType == TnefPropertyType.String8 || this.propertyTag.ValueTnefType == TnefPropertyType.Unicode || this.propertyTag.ValueTnefType == TnefPropertyType.Binary || this.propertyTag.ValueTnefType == TnefPropertyType.Object))
			{
				this.valueLengthOffset = this.StreamOffset;
				this.WriteDword(0);
			}
			this.valueAsText = true;
			this.valueCount++;
			this.valueLength = 0;
			this.writeState = TnefWriter.WriteState.BeginPropertyValue;
		}

		public void WritePropertyValue(bool value)
		{
			this.AssertGoodToUse(true);
			if (this.writeState != TnefWriter.WriteState.BeginPropertyValue)
			{
				this.StartPropertyValue();
			}
			if (this.propertyTag.ValueTnefType != TnefPropertyType.Boolean)
			{
				throw new InvalidOperationException(TnefStrings.WriterInvalidOperationInvalidValueType);
			}
			this.WriteWord(value ? 1 : 0);
			this.EndPropertyValue();
		}

		public void WritePropertyValue(short value)
		{
			this.AssertGoodToUse(true);
			if (this.writeState != TnefWriter.WriteState.BeginPropertyValue)
			{
				this.StartPropertyValue();
			}
			if (this.propertyTag.ValueTnefType == TnefPropertyType.I2 || this.propertyTag.ValueTnefType == TnefPropertyType.Boolean)
			{
				this.WriteWord(value);
			}
			else if (this.propertyTag.ValueTnefType == TnefPropertyType.Long)
			{
				this.WriteDword((int)value);
			}
			else
			{
				if (this.propertyTag.ValueTnefType != TnefPropertyType.I8)
				{
					throw new InvalidOperationException(TnefStrings.WriterInvalidOperationInvalidValueType);
				}
				this.WriteQword((long)value);
			}
			this.EndPropertyValue();
		}

		public void WritePropertyValue(int value)
		{
			this.AssertGoodToUse(true);
			if (this.writeState != TnefWriter.WriteState.BeginPropertyValue)
			{
				this.StartPropertyValue();
			}
			if (this.propertyTag.ValueTnefType == TnefPropertyType.Long || this.propertyTag.ValueTnefType == TnefPropertyType.Error)
			{
				this.WriteDword(value);
			}
			else if ((this.propertyTag.ValueTnefType == TnefPropertyType.Boolean || this.propertyTag.ValueTnefType == TnefPropertyType.I2) && !this.propertyTag.IsMultiValued)
			{
				if (!this.legacyAttribute)
				{
					this.valueFixedLength = 4;
					this.WriteDword(value);
				}
				else
				{
					this.WriteWord((short)value);
				}
			}
			else
			{
				if (this.propertyTag.ValueTnefType != TnefPropertyType.I8)
				{
					throw new InvalidOperationException(TnefStrings.WriterInvalidOperationInvalidValueType);
				}
				this.WriteQword((long)value);
			}
			this.EndPropertyValue();
		}

		public void WritePropertyValue(long value)
		{
			this.AssertGoodToUse(true);
			if (this.writeState != TnefWriter.WriteState.BeginPropertyValue)
			{
				this.StartPropertyValue();
			}
			if (this.propertyTag.ValueTnefType == TnefPropertyType.I8 || this.propertyTag.ValueTnefType == TnefPropertyType.Currency || this.propertyTag.ValueTnefType == TnefPropertyType.SysTime)
			{
				this.WriteQword(value);
				this.EndPropertyValue();
				return;
			}
			throw new InvalidOperationException(TnefStrings.WriterInvalidOperationInvalidValueType);
		}

		public void WritePropertyValue(float value)
		{
			this.AssertGoodToUse(true);
			if (this.writeState != TnefWriter.WriteState.BeginPropertyValue)
			{
				this.StartPropertyValue();
			}
			if (this.propertyTag.ValueTnefType == TnefPropertyType.R4)
			{
				this.WriteFloat(value);
			}
			else
			{
				if (this.propertyTag.ValueTnefType != TnefPropertyType.Double)
				{
					throw new InvalidOperationException(TnefStrings.WriterInvalidOperationInvalidValueType);
				}
				this.WriteDouble((double)value);
			}
			this.EndPropertyValue();
		}

		public void WritePropertyValue(double value)
		{
			this.AssertGoodToUse(true);
			if (this.writeState != TnefWriter.WriteState.BeginPropertyValue)
			{
				this.StartPropertyValue();
			}
			if (this.propertyTag.ValueTnefType == TnefPropertyType.Double || this.propertyTag.ValueTnefType == TnefPropertyType.AppTime)
			{
				this.WriteDouble(value);
				this.EndPropertyValue();
				return;
			}
			throw new InvalidOperationException(TnefStrings.WriterInvalidOperationInvalidValueType);
		}

		public void WritePropertyValue(Guid value)
		{
			this.AssertGoodToUse(true);
			if (this.writeState != TnefWriter.WriteState.BeginPropertyValue)
			{
				this.StartPropertyValue();
			}
			if (this.propertyTag.ValueTnefType != TnefPropertyType.ClassId && this.propertyTag.ValueTnefType != TnefPropertyType.Object)
			{
				throw new InvalidOperationException(TnefStrings.WriterInvalidOperationInvalidValueType);
			}
			this.WriteGuid(value);
			if (this.propertyTag.ValueTnefType == TnefPropertyType.Object)
			{
				this.writeState = TnefWriter.WriteState.WritePropertyValue;
				this.valueAsText = false;
				return;
			}
			this.EndPropertyValue();
		}

		public void WritePropertyValue(DateTime value)
		{
			this.AssertGoodToUse(true);
			if (this.writeState != TnefWriter.WriteState.BeginPropertyValue)
			{
				this.StartPropertyValue();
			}
			if (this.propertyTag.ValueTnefType == TnefPropertyType.SysTime)
			{
				this.WriteQword((value == DateTime.MinValue) ? 0L : value.ToFileTimeUtc());
			}
			else
			{
				if (this.propertyTag.ValueTnefType != TnefPropertyType.AppTime)
				{
					throw new InvalidOperationException(TnefStrings.WriterInvalidOperationInvalidValueType);
				}
				this.WriteDouble((value == DateTime.MinValue) ? 0.0 : TnefWriter.ToOADate(value));
			}
			this.EndPropertyValue();
		}

		public void WritePropertyValue(string value)
		{
			this.AssertGoodToUse(true);
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (this.writeState != TnefWriter.WriteState.BeginPropertyValue)
			{
				this.StartPropertyValue();
			}
			if (this.propertyTag.ValueTnefType == TnefPropertyType.String8)
			{
				this.WriteString8String(value, true);
				this.WriteByte(0);
			}
			else
			{
				if (this.propertyTag.ValueTnefType != TnefPropertyType.Unicode)
				{
					throw new InvalidOperationException(TnefStrings.WriterInvalidOperationInvalidValueType);
				}
				this.WriteUnicodeString(value, true);
				this.WriteWord(0);
			}
			this.valueAsText = false;
			this.EndPropertyValue();
		}

		public void WritePropertyValue(byte[] value)
		{
			this.AssertGoodToUse(true);
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (this.writeState != TnefWriter.WriteState.BeginPropertyValue)
			{
				this.StartPropertyValue();
			}
			this.WritePropertyRawValue(value, 0, value.Length);
			this.EndPropertyValue();
		}

		public void WritePropertyValue(object value)
		{
			this.AssertGoodToUse(true);
			if (value == null)
			{
				throw new ArgumentNullException("object");
			}
			Type type = value.GetType();
			switch (TnefWriter.GetTypeCode(value))
			{
			case TypeCode.Boolean:
				this.WritePropertyValue((bool)value);
				return;
			case TypeCode.Char:
				throw new NotSupportedException("Char type is not supported");
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
				this.WritePropertyValue((short)value);
				return;
			case TypeCode.UInt16:
				this.WritePropertyValue((short)((ushort)value));
				return;
			case TypeCode.Int32:
				this.WritePropertyValue((int)value);
				return;
			case TypeCode.UInt32:
				this.WritePropertyValue((int)((uint)value));
				return;
			case TypeCode.Int64:
				this.WritePropertyValue((long)value);
				return;
			case TypeCode.UInt64:
				this.WritePropertyValue((long)((ulong)value));
				return;
			case TypeCode.Single:
				this.WritePropertyValue((float)value);
				return;
			case TypeCode.Double:
				this.WritePropertyValue((double)value);
				return;
			case TypeCode.Decimal:
				throw new NotSupportedException("Decimal type is not supported");
			case TypeCode.DateTime:
				this.WritePropertyValue((DateTime)value);
				return;
			case TypeCode.String:
				this.WritePropertyValue((string)value);
				return;
			}
			if (type == typeof(Guid))
			{
				this.WritePropertyValue((Guid)value);
				return;
			}
			if (type == typeof(byte[]))
			{
				this.WritePropertyValue((byte[])value);
				return;
			}
			if (type.GetTypeInfo().IsEnum)
			{
				if (typeof(short).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
				{
					this.WritePropertyValue((short)value);
					return;
				}
				if (typeof(int).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
				{
					this.WritePropertyValue((int)value);
					return;
				}
				if (typeof(long).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
				{
					this.WritePropertyValue((long)value);
					return;
				}
				throw new NotSupportedException("enum is not supported");
			}
			else
			{
				TextReader textReader = value as TextReader;
				if (textReader != null)
				{
					this.WritePropertyValue(textReader);
					return;
				}
				Stream stream = value as Stream;
				if (stream != null)
				{
					this.WritePropertyValue(stream);
					return;
				}
				throw new NotSupportedException(TnefStrings.WriterInvalidOperationInvalidValueType);
			}
		}

		public TnefWriter GetEmbeddedMessageWriter()
		{
			this.AssertGoodToUse(true);
			return this.GetEmbeddedMessageWriter(this.messageCodePage);
		}

		public TnefWriter GetEmbeddedMessageWriter(int messageCodePage)
		{
			this.AssertGoodToUse(true);
			if (this.writeState != TnefWriter.WriteState.BeginPropertyValue && this.writeState != TnefWriter.WriteState.WritePropertyValue)
			{
				this.StartPropertyValue();
			}
			if (this.writeState == TnefWriter.WriteState.BeginPropertyValue)
			{
				this.WriteGuid(TnefCommon.MessageIID);
			}
			return new TnefWriter(this, messageCodePage);
		}

		public Stream GetRawPropertyValueWriteStream()
		{
			this.AssertGoodToUse(true);
			if (this.writeState != TnefWriter.WriteState.BeginPropertyValue)
			{
				this.StartPropertyValue();
			}
			return new TnefWriterStreamWrapper(this);
		}

		private void CopyStreamContentAsRawValue(Stream stream)
		{
			int count;
			while ((count = stream.Read(this.byteBuffer, 0, this.byteBuffer.Length)) != 0)
			{
				this.WriteBinary(this.byteBuffer, 0, count);
			}
		}

		public void WritePropertyValue(Stream stream)
		{
			this.AssertGoodToUse(true);
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (this.writeState != TnefWriter.WriteState.BeginPropertyValue)
			{
				this.StartPropertyValue();
			}
			if (this.propertyTag.ValueTnefType != TnefPropertyType.Binary && this.propertyTag.ValueTnefType != TnefPropertyType.Object)
			{
				throw new InvalidOperationException(TnefStrings.WriterInvalidOperationInvalidValueType);
			}
			this.CopyStreamContentAsRawValue(stream);
			this.EndPropertyValue();
		}

		public void WritePropertyValue(Guid iid, Stream stream)
		{
			this.AssertGoodToUse(true);
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (this.writeState != TnefWriter.WriteState.BeginPropertyValue)
			{
				this.StartPropertyValue();
			}
			if (this.propertyTag.ValueTnefType == TnefPropertyType.Object)
			{
				this.WriteGuid(iid);
				this.CopyStreamContentAsRawValue(stream);
				this.EndPropertyValue();
				return;
			}
			throw new InvalidOperationException(TnefStrings.WriterInvalidOperationNotObjectProperty);
		}

		public void WritePropertyValue(TextReader reader)
		{
			this.AssertGoodToUse(true);
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (this.writeState != TnefWriter.WriteState.BeginPropertyValue)
			{
				this.StartPropertyValue();
			}
			if (this.propertyTag.ValueTnefType == TnefPropertyType.String8)
			{
				this.WriteString8TextReader(reader, true);
				this.WriteByte(0);
			}
			else
			{
				if (this.propertyTag.ValueTnefType != TnefPropertyType.Unicode)
				{
					throw new InvalidOperationException(TnefStrings.WriterInvalidOperationNotStringProperty);
				}
				this.WriteUnicodeTextReader(reader, true);
				this.WriteWord(0);
			}
			this.valueAsText = false;
			this.EndPropertyValue();
		}

		public void WritePropertyTextValue(char[] buffer, int offset, int count)
		{
			this.AssertGoodToUse(true);
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset > buffer.Length || offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", TnefStrings.OffsetOutOfRange);
			}
			if (count > buffer.Length || count < 0)
			{
				throw new ArgumentOutOfRangeException("count", TnefStrings.CountOutOfRange);
			}
			if (count + offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("count", TnefStrings.CountTooLarge);
			}
			if (this.writeState < TnefWriter.WriteState.BeginPropertyValue)
			{
				this.StartPropertyValue();
			}
			if (this.writeState == TnefWriter.WriteState.BeginPropertyValue)
			{
				this.valueAsText = true;
			}
			if (!this.valueAsText)
			{
				throw new NotSupportedException(TnefStrings.WriterInvalidOperationTextAfterRawData);
			}
			if (this.propertyTag.ValueTnefType == TnefPropertyType.String8)
			{
				this.WriteString8Text(buffer, offset, count, false);
			}
			else
			{
				if (this.propertyTag.ValueTnefType != TnefPropertyType.Unicode)
				{
					throw new InvalidOperationException(TnefStrings.WriterInvalidOperationInvalidValueType);
				}
				this.WriteUnicodeText(buffer, offset, count, false);
			}
			this.writeState = TnefWriter.WriteState.WritePropertyValue;
		}

		public void WritePropertyRawValue(byte[] buffer, int offset, int count)
		{
			this.WritePropertyRawValueImpl(buffer, offset, count, false);
		}

		internal void WritePropertyRawValueImpl(byte[] buffer, int offset, int count, bool fromWrapper)
		{
			this.AssertGoodToUse(!fromWrapper);
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset > buffer.Length || offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", TnefStrings.OffsetOutOfRange);
			}
			if (count > buffer.Length || count < 0)
			{
				throw new ArgumentOutOfRangeException("count", TnefStrings.CountOutOfRange);
			}
			if (count + offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("count", TnefStrings.CountTooLarge);
			}
			if (this.writeState < TnefWriter.WriteState.BeginPropertyValue)
			{
				this.StartPropertyValue();
			}
			if (this.valueFixedLength != 0 && this.valueLength + count > this.valueFixedLength)
			{
				throw new NotSupportedException(TnefStrings.WriterInvalidOperationValueTooLongForType);
			}
			if (this.legacyAttribute && this.string8AsUnicode)
			{
				throw new NotSupportedException(TnefStrings.WriterInvalidOperationUnicodeRawValueForLegacyAttribute);
			}
			if (this.writeState == TnefWriter.WriteState.BeginPropertyValue)
			{
				this.valueAsText = false;
			}
			if (this.valueAsText)
			{
				throw new NotSupportedException(TnefStrings.WriterInvalidOperationRawDataAfterText);
			}
			this.WriteBinary(buffer, offset, count);
			this.writeState = TnefWriter.WriteState.WritePropertyValue;
		}

		public void WriteProperty(TnefPropertyTag tag, bool value)
		{
			this.StartProperty(tag);
			this.WritePropertyValue(value);
		}

		public void WriteProperty(TnefPropertyTag tag, int value)
		{
			this.StartProperty(tag);
			this.WritePropertyValue(value);
		}

		public void WriteProperty(TnefPropertyTag tag, short value)
		{
			this.StartProperty(tag);
			this.WritePropertyValue(value);
		}

		public void WriteProperty(TnefPropertyTag tag, long value)
		{
			this.StartProperty(tag);
			this.WritePropertyValue(value);
		}

		public void WriteProperty(TnefPropertyTag tag, float value)
		{
			this.StartProperty(tag);
			this.WritePropertyValue(value);
		}

		public void WriteProperty(TnefPropertyTag tag, double value)
		{
			this.StartProperty(tag);
			this.WritePropertyValue(value);
		}

		public void WriteProperty(TnefPropertyTag tag, string value)
		{
			this.StartProperty(tag);
			this.WritePropertyValue(value);
		}

		public void WriteProperty(TnefPropertyTag tag, Guid value)
		{
			this.StartProperty(tag);
			this.WritePropertyValue(value);
		}

		public void WriteProperty(TnefPropertyTag tag, DateTime value)
		{
			this.StartProperty(tag);
			this.WritePropertyValue(value);
		}

		public void WriteProperty(TnefPropertyTag tag, byte[] value)
		{
			this.StartProperty(tag);
			this.WritePropertyValue(value);
		}

		public void WriteProperty(TnefPropertyTag tag, object value)
		{
			this.StartProperty(tag);
			if (tag.TnefType != TnefPropertyType.Null || value != null)
			{
				this.WritePropertyValue(value);
			}
		}

		public void WriteProperty(TnefPropertyTag tag, Stream stream)
		{
			this.StartProperty(tag);
			this.WritePropertyValue(stream);
		}

		public void WriteProperty(TnefPropertyTag tag, Guid guid, Stream stream)
		{
			this.StartProperty(tag);
			this.WritePropertyValue(guid, stream);
		}

		public void WriteProperty(TnefPropertyTag tag, TextReader reader)
		{
			this.StartProperty(tag);
			this.WritePropertyValue(reader);
		}

		private void EndAttribute()
		{
			if (this.writeState != TnefWriter.WriteState.BeginAttribute && this.writeState != TnefWriter.WriteState.WriteAttributeValue)
			{
				if (this.writeState >= TnefWriter.WriteState.BeforePropertyValue)
				{
					this.EndProperty();
				}
				if (!this.legacyAttribute)
				{
					if (this.writeState == TnefWriter.WriteState.BeforeProperty)
					{
						if (this.attributeTag == TnefAttributeTag.RecipientTable)
						{
							this.EndRow();
						}
						else if (this.propertyCount != 0)
						{
							this.ReWriteDword(this.propertyCountOffset, this.propertyCount);
						}
					}
					if (this.writeState == TnefWriter.WriteState.BeforeRow && this.rowCount != 0)
					{
						this.ReWriteDword(this.rowCountOffset, this.rowCount);
					}
				}
				else
				{
					this.CompleteLegacyAttribute();
				}
			}
			int num = this.attributeLength;
			ushort num2 = this.checksum - this.attributeStartChecksum;
			this.WriteWord((short)num2);
			if (num != 0)
			{
				this.totalLength += num;
				this.ReWriteDword(this.attributeLengthOffset, num);
			}
			this.totalLength += 2;
			if (this.parent == null && !this.outputStream.CanSeek)
			{
				this.FlushWriteStreamToOutput();
			}
			this.writeState = TnefWriter.WriteState.BeforeAttribute;
		}

		private void FlushWriteStreamToOutput()
		{
			this.canFlush = false;
			if (this.writeStream.Length != 0L)
			{
				this.writeStream.Position = 0L;
				int num;
				while ((num = this.writeStream.Read(this.byteBuffer, 0, this.byteBuffer.Length)) != 0)
				{
					this.outputStream.Write(this.byteBuffer, 0, num);
					this.writeStreamOffset += num;
				}
				this.writeStream.Position = 0L;
				this.writeStream.SetLength(0L);
			}
			if (this.writeOffset != 0)
			{
				this.outputStream.Write(this.writeBuffer, 0, this.writeOffset);
				this.streamOffset += this.writeOffset;
				this.writeStreamOffset += this.writeOffset;
				this.writeOffset = 0;
			}
			this.canFlush = true;
		}

		private void EndRow()
		{
			if (this.writeState >= TnefWriter.WriteState.BeforePropertyValue)
			{
				this.EndProperty();
			}
			if (this.propertyCount != 0)
			{
				this.ReWriteDword(this.propertyCountOffset, this.propertyCount);
			}
			this.writeState = TnefWriter.WriteState.BeforeRow;
		}

		private void EndProperty()
		{
			if (this.writeState >= TnefWriter.WriteState.BeginPropertyValue)
			{
				this.EndPropertyValue();
			}
			if (!this.legacyAttribute)
			{
				if (this.propertyTag.IsMultiValued && this.valueCount != 0)
				{
					this.ReWriteDword(this.valueCountOffset, this.valueCount);
				}
				int num = (4 - this.valueCount * this.valueFixedLength % 4) % 4;
				if (num != 0)
				{
					this.WriteBinary(TnefCommon.Padding, 0, num);
				}
			}
			this.writeState = TnefWriter.WriteState.BeforeProperty;
		}

		private void EndPropertyValue()
		{
			if (this.valueFixedLength != 0 && this.valueLength != this.valueFixedLength)
			{
				if (!this.disposing || this.valueLength > this.valueFixedLength)
				{
					throw new NotSupportedException(TnefStrings.WriterInvalidOperationValueSizeInvalidForType);
				}
				while (this.valueLength != this.valueFixedLength)
				{
					this.WriteByte(0);
				}
			}
			if (this.valueAsText)
			{
				if (this.propertyTag.ValueTnefType == TnefPropertyType.String8)
				{
					this.WriteString8Text(this.charBuffer, 0, 0, true);
					this.WriteByte(0);
				}
				else if (this.propertyTag.ValueTnefType == TnefPropertyType.Unicode)
				{
					this.WriteUnicodeText(this.charBuffer, 0, 0, true);
					this.WriteWord(0);
				}
			}
			else if (this.legacyAttribute && this.hexEncodeBinary)
			{
				this.directWrite = true;
				this.WriteByte(0);
			}
			if (!this.legacyAttribute && (this.propertyTag.ValueTnefType == TnefPropertyType.String8 || this.propertyTag.ValueTnefType == TnefPropertyType.Unicode || this.propertyTag.ValueTnefType == TnefPropertyType.Binary || this.propertyTag.ValueTnefType == TnefPropertyType.Object))
			{
				int num = this.valueLength;
				int num2 = (4 - num % 4) % 4;
				if (num2 != 0)
				{
					this.WriteBinary(TnefCommon.Padding, 0, num2);
				}
				if (num != 0)
				{
					this.ReWriteDword(this.valueLengthOffset, num);
				}
			}
			this.writeState = TnefWriter.WriteState.BeforePropertyValue;
		}

		private void WriteByte(byte value)
		{
			if (!this.directWrite)
			{
				this.EnsureFabricatedSpace(1);
				this.fabricatedBuffer[this.fabricatedOffset++] = value;
				this.valueLength++;
				return;
			}
			this.EnsureSpace(1);
			this.writeBuffer[this.writeOffset++] = value;
			this.Checksum(this.writeBuffer, this.writeOffset - 1, 1);
			this.valueLength++;
			this.attributeLength++;
		}

		private void WriteWord(short value)
		{
			if (!this.directWrite)
			{
				this.EnsureFabricatedSpace(2);
				TnefBitConverter.GetBytes(this.fabricatedBuffer, this.fabricatedOffset, value);
				this.fabricatedOffset += 2;
				this.valueLength += 2;
				return;
			}
			this.EnsureSpace(2);
			TnefBitConverter.GetBytes(this.writeBuffer, this.writeOffset, value);
			this.Checksum(this.writeBuffer, this.writeOffset, 2);
			this.writeOffset += 2;
			this.valueLength += 2;
			this.attributeLength += 2;
		}

		private void WriteDword(int value)
		{
			if (!this.directWrite)
			{
				this.EnsureFabricatedSpace(4);
				TnefBitConverter.GetBytes(this.fabricatedBuffer, this.fabricatedOffset, value);
				this.fabricatedOffset += 4;
				this.valueLength += 4;
				return;
			}
			this.EnsureSpace(4);
			TnefBitConverter.GetBytes(this.writeBuffer, this.writeOffset, value);
			this.Checksum(this.writeBuffer, this.writeOffset, 4);
			this.writeOffset += 4;
			this.valueLength += 4;
			this.attributeLength += 4;
		}

		private void WriteQword(long value)
		{
			if (!this.directWrite)
			{
				this.EnsureFabricatedSpace(8);
				TnefBitConverter.GetBytes(this.fabricatedBuffer, this.fabricatedOffset, value);
				this.fabricatedOffset += 8;
				this.valueLength += 8;
				return;
			}
			this.EnsureSpace(8);
			TnefBitConverter.GetBytes(this.writeBuffer, this.writeOffset, value);
			this.Checksum(this.writeBuffer, this.writeOffset, 8);
			this.writeOffset += 8;
			this.valueLength += 8;
			this.attributeLength += 8;
		}

		private void WriteFloat(float value)
		{
			this.EnsureSpace(4);
			TnefBitConverter.GetBytes(this.writeBuffer, this.writeOffset, value);
			this.Checksum(this.writeBuffer, this.writeOffset, 4);
			this.writeOffset += 4;
			this.valueLength += 4;
			this.attributeLength += 4;
		}

		private void WriteDouble(double value)
		{
			this.EnsureSpace(8);
			TnefBitConverter.GetBytes(this.writeBuffer, this.writeOffset, value);
			this.Checksum(this.writeBuffer, this.writeOffset, 8);
			this.writeOffset += 8;
			this.valueLength += 8;
			this.attributeLength += 8;
		}

		private void WriteGuid(Guid value)
		{
			this.EnsureSpace(16);
			TnefBitConverter.GetBytes(this.writeBuffer, this.writeOffset, value);
			this.Checksum(this.writeBuffer, this.writeOffset, 16);
			this.writeOffset += 16;
			this.valueLength += 16;
			this.attributeLength += 16;
		}

		private void WriteUnicodeString(string value, bool flush)
		{
			int num = 0;
			while (num != value.Length)
			{
				int num2 = Math.Min(this.charBuffer.Length, value.Length - num);
				value.CopyTo(num, this.charBuffer, 0, num2);
				num += num2;
				this.WriteUnicodeText(this.charBuffer, 0, num2, flush && num == value.Length);
			}
		}

		private void WriteString8String(string value, bool flush)
		{
			int num = 0;
			while (num != value.Length)
			{
				int num2 = Math.Min(this.charBuffer.Length, value.Length - num);
				value.CopyTo(num, this.charBuffer, 0, num2);
				num += num2;
				this.WriteString8Text(this.charBuffer, 0, num2, flush && num == value.Length);
			}
		}

		private void WriteUnicodeTextReader(TextReader reader, bool flush)
		{
			int count;
			while ((count = reader.Read(this.charBuffer, 0, this.charBuffer.Length)) != 0)
			{
				this.WriteUnicodeText(this.charBuffer, 0, count, false);
			}
			if (flush)
			{
				this.WriteUnicodeText(this.charBuffer, 0, 0, true);
			}
		}

		private void WriteString8TextReader(TextReader reader, bool flush)
		{
			int count;
			while ((count = reader.Read(this.charBuffer, 0, this.charBuffer.Length)) != 0)
			{
				this.WriteString8Text(this.charBuffer, 0, count, false);
			}
			if (flush)
			{
				this.WriteString8Text(this.charBuffer, 0, 0, true);
			}
		}

		private void WriteUnicodeText(char[] buffer, int offset, int count, bool flush)
		{
			bool flag = false;
			int num = 0;
			while (count != 0 || (flush && !flag))
			{
				this.EnsureSpace(128);
				int num2;
				int num3;
				this.unicodeEncoder.Convert(buffer, offset, count, this.writeBuffer, this.writeOffset, this.writeBuffer.Length - this.writeOffset, flush, out num2, out num3, out flag);
				this.Checksum(this.writeBuffer, this.writeOffset, num3);
				this.writeOffset += num3;
				num += num3;
				offset += num2;
				count -= num2;
			}
			this.valueLength += num;
			this.attributeLength += num;
		}

		private void WriteString8Text(char[] buffer, int offset, int count, bool flush)
		{
			bool flag = false;
			int num = 0;
			if (!this.directWrite)
			{
				while (count != 0 || (flush && !flag))
				{
					this.EnsureFabricatedSpace(16);
					int num2;
					int num3;
					this.string8Encoder.Convert(buffer, offset, count, this.fabricatedBuffer, this.fabricatedOffset, this.fabricatedBuffer.Length - this.fabricatedOffset, flush, out num2, out num3, out flag);
					this.fabricatedOffset += num3;
					num += num3;
					offset += num2;
					count -= num2;
				}
				this.valueLength += num;
				return;
			}
			while (count != 0 || (flush && !flag))
			{
				this.EnsureSpace(128);
				int num2;
				int num3;
				this.string8Encoder.Convert(buffer, offset, count, this.writeBuffer, this.writeOffset, this.writeBuffer.Length - this.writeOffset, flush, out num2, out num3, out flag);
				this.Checksum(this.writeBuffer, this.writeOffset, num3);
				this.writeOffset += num3;
				num += num3;
				offset += num2;
				count -= num2;
			}
			this.valueLength += num;
			this.attributeLength += num;
		}

		private void WriteBinary(byte[] buffer, int offset, int count)
		{
			if (!this.directWrite)
			{
				if (this.hexEncodeBinary)
				{
					this.HexEncodeBinary(buffer, offset, count);
					return;
				}
				this.EnsureFabricatedSpace(count);
				Buffer.BlockCopy(buffer, offset, this.fabricatedBuffer, this.fabricatedOffset, count);
				this.fabricatedOffset += count;
				this.valueLength += count;
				return;
			}
			else
			{
				this.Checksum(buffer, offset, count);
				this.valueLength += count;
				this.attributeLength += count;
				if (this.writeOffset + count > this.writeBuffer.Length)
				{
					this.FlushBuffer();
					this.DirectWrite(buffer, offset, count);
					this.streamOffset += count;
					return;
				}
				Buffer.BlockCopy(buffer, offset, this.writeBuffer, this.writeOffset, count);
				this.writeOffset += count;
				return;
			}
		}

		private void HexEncodeBinary(byte[] buffer, int offset, int count)
		{
			int num = 0;
			while (count != 0)
			{
				this.EnsureSpace(Math.Min(128, count * 2));
				int num2 = Math.Min(count * 2, this.writeBuffer.Length - this.writeOffset) & -2;
				for (int i = 0; i < num2; i += 2)
				{
					this.writeBuffer[this.writeOffset + i] = TnefCommon.HexDigit[buffer[offset] >> 4];
					this.writeBuffer[this.writeOffset + i + 1] = TnefCommon.HexDigit[(int)(buffer[offset] & 15)];
					offset++;
				}
				this.Checksum(this.writeBuffer, this.writeOffset, num2);
				this.writeOffset += num2;
				num += num2;
				count -= num2 / 2;
			}
			this.valueLength += num;
			this.attributeLength += num;
		}

		private void Checksum(byte[] buffer, int offset, int count)
		{
			while (count-- > 0)
			{
				this.checksum += (ushort)buffer[offset++];
			}
		}

		private void EnsureSpace(int count)
		{
			if (this.writeOffset + count > this.writeBuffer.Length)
			{
				this.FlushBuffer();
			}
		}

		private void FlushBuffer()
		{
			int num = this.writeOffset;
			this.writeOffset = 0;
			this.DirectWrite(this.writeBuffer, 0, num);
			this.streamOffset += num;
		}

		private void DirectWrite(byte[] buffer, int offset, int count)
		{
			this.canFlush = false;
			this.writeStream.Write(buffer, offset, count);
			this.canFlush = true;
		}

		private void ReWriteDword(int streamOffset, int value)
		{
			int num = this.StreamOffset - streamOffset;
			if (num <= this.writeOffset)
			{
				TnefBitConverter.GetBytes(this.writeBuffer, this.writeOffset - num, value);
				this.Checksum(this.writeBuffer, this.writeOffset - num, 4);
				return;
			}
			TnefBitConverter.GetBytes(this.byteBuffer, 0, value);
			this.Checksum(this.byteBuffer, 0, 4);
			this.canFlush = false;
			this.writeStream.Position = (long)(streamOffset - this.writeStreamOffset);
			this.writeStream.Write(this.byteBuffer, 0, 4);
			this.writeStream.Position = (long)(this.streamOffset - this.writeStreamOffset);
			this.canFlush = true;
		}

		private void EnsureFabricatedSpace(int count)
		{
			if (this.fabricatedOffset + count > this.fabricatedBuffer.Length)
			{
				if (this.fabricatedOffset + count > 32768)
				{
					throw new NotSupportedException(TnefStrings.WriterNotSupportedLegacyAttributeTooLong);
				}
				int num = this.fabricatedBuffer.Length * 2;
				while (num - this.fabricatedOffset < count)
				{
					num *= 2;
				}
				byte[] dst = new byte[num];
				Buffer.BlockCopy(this.fabricatedBuffer, 0, dst, 0, this.fabricatedOffset);
				this.fabricatedBuffer = dst;
			}
		}

		private bool IsLegacyAttribute()
		{
			return this.attributeTag != TnefAttributeTag.MapiProperties && this.attributeTag != TnefAttributeTag.Attachment && this.attributeTag != TnefAttributeTag.RecipientTable;
		}

		private void StartLegacyAttribute()
		{
			this.fabricatedOffset = 0;
			TnefAttributeTag tnefAttributeTag = this.attributeTag;
			if (tnefAttributeTag != TnefAttributeTag.From)
			{
				switch (tnefAttributeTag)
				{
				case TnefAttributeTag.Owner:
				case TnefAttributeTag.SentFor:
					break;
				default:
					if (tnefAttributeTag != TnefAttributeTag.AttachRenderData)
					{
						return;
					}
					this.renderingPositionOffset = -1;
					this.attachMethodOffset = -1;
					this.attachEncodingOffset = -1;
					return;
				}
			}
			this.entryIdOffset = -1;
			this.nameOffset = -1;
			this.addrTypeOffset = -1;
			this.addressOffset = -1;
		}

		private void StartLegacyAttributeProperty()
		{
			this.directWrite = false;
			this.string8AsUnicode = false;
			this.hexEncodeBinary = false;
			TnefAttributeTag tnefAttributeTag = this.attributeTag;
			if (tnefAttributeTag <= TnefAttributeTag.DateModified)
			{
				if (tnefAttributeTag <= TnefAttributeTag.AttachTitle)
				{
					if (tnefAttributeTag <= TnefAttributeTag.From)
					{
						if (tnefAttributeTag != TnefAttributeTag.Null)
						{
							if (tnefAttributeTag == TnefAttributeTag.From)
							{
								if (this.propertyTag.Id == TnefPropertyId.SenderEntryId)
								{
									if (this.entryIdOffset != -1)
									{
										throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
									}
									if (this.propertyTag.TnefType != TnefPropertyType.Binary)
									{
										throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
									}
									this.entryIdOffset = this.fabricatedOffset;
									return;
								}
								else if (this.propertyTag.Id == TnefPropertyId.SenderName)
								{
									if (this.nameOffset != -1)
									{
										throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
									}
									if (this.propertyTag.TnefType != TnefPropertyType.String8 && this.propertyTag.TnefType != TnefPropertyType.Unicode)
									{
										throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
									}
									if (this.propertyTag.TnefType == TnefPropertyType.Unicode)
									{
										this.string8AsUnicode = true;
										this.propertyTag = TnefPropertyTag.SenderNameA;
									}
									this.nameOffset = this.fabricatedOffset;
									return;
								}
								else if (this.propertyTag.Id == TnefPropertyId.SenderAddrtype)
								{
									if (this.addrTypeOffset != -1)
									{
										throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
									}
									if (this.propertyTag.TnefType != TnefPropertyType.String8 && this.propertyTag.TnefType != TnefPropertyType.Unicode)
									{
										throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
									}
									if (this.propertyTag.TnefType == TnefPropertyType.Unicode)
									{
										this.string8AsUnicode = true;
										this.propertyTag = TnefPropertyTag.SenderAddrtypeA;
									}
									this.addrTypeOffset = this.fabricatedOffset;
									return;
								}
								else
								{
									if (this.propertyTag.Id != TnefPropertyId.SenderEmailAddress)
									{
										throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
									}
									if (this.addressOffset != -1)
									{
										throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
									}
									if (this.propertyTag.TnefType != TnefPropertyType.String8 && this.propertyTag.TnefType != TnefPropertyType.Unicode)
									{
										throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
									}
									if (this.propertyTag.TnefType == TnefPropertyType.Unicode)
									{
										this.string8AsUnicode = true;
										this.propertyTag = TnefPropertyTag.SenderEmailAddressA;
									}
									this.addressOffset = this.fabricatedOffset;
									return;
								}
							}
						}
					}
					else if (tnefAttributeTag != TnefAttributeTag.Subject)
					{
						switch (tnefAttributeTag)
						{
						case TnefAttributeTag.MessageId:
							if (this.propertyTag.Id != TnefPropertyId.SearchKey)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
							}
							if (this.attributeLength != 0)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
							}
							if (this.propertyTag.TnefType != TnefPropertyType.Binary)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
							}
							this.hexEncodeBinary = true;
							return;
						case TnefAttributeTag.ParentId:
							if (this.propertyTag.Id != TnefPropertyId.ParentKey)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
							}
							if (this.attributeLength != 0)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
							}
							if (this.propertyTag.TnefType != TnefPropertyType.Binary)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
							}
							this.hexEncodeBinary = true;
							return;
						case TnefAttributeTag.ConversationId:
							if (this.propertyTag.Id != TnefPropertyId.ConversationKey)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
							}
							if (this.attributeLength != 0)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
							}
							if (this.propertyTag.TnefType != TnefPropertyType.Binary)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
							}
							this.hexEncodeBinary = true;
							return;
						default:
							if (tnefAttributeTag == TnefAttributeTag.AttachTitle)
							{
								if (this.propertyTag.Id != TnefPropertyId.AttachFilename)
								{
									throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
								}
								if (this.attributeLength != 0)
								{
									throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
								}
								if (this.propertyTag.TnefType != TnefPropertyType.String8 && this.propertyTag.TnefType != TnefPropertyType.Unicode)
								{
									throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
								}
								if (this.propertyTag.TnefType == TnefPropertyType.Unicode)
								{
									this.string8AsUnicode = true;
									this.propertyTag = TnefPropertyTag.AttachFilenameA;
								}
								this.directWrite = true;
								return;
							}
							break;
						}
					}
					else
					{
						if (this.propertyTag.Id != TnefPropertyId.Subject)
						{
							throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
						}
						if (this.attributeLength != 0)
						{
							throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
						}
						if (this.propertyTag.TnefType != TnefPropertyType.String8 && this.propertyTag.TnefType != TnefPropertyType.Unicode)
						{
							throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
						}
						if (this.propertyTag.TnefType == TnefPropertyType.Unicode)
						{
							this.string8AsUnicode = true;
							this.propertyTag = TnefPropertyTag.SubjectA;
						}
						this.directWrite = true;
						return;
					}
				}
				else if (tnefAttributeTag <= TnefAttributeTag.DateEnd)
				{
					if (tnefAttributeTag != TnefAttributeTag.Body)
					{
						switch (tnefAttributeTag)
						{
						case TnefAttributeTag.DateStart:
							if (this.propertyTag.Id != TnefPropertyId.StartDate)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
							}
							if (this.fabricatedOffset != 0)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
							}
							if (this.propertyTag.TnefType != TnefPropertyType.SysTime)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
							}
							return;
						case TnefAttributeTag.DateEnd:
							if (this.propertyTag.Id != TnefPropertyId.EndDate)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
							}
							if (this.fabricatedOffset != 0)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
							}
							if (this.propertyTag.TnefType != TnefPropertyType.SysTime)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
							}
							return;
						}
					}
					else
					{
						if (this.propertyTag.Id != TnefPropertyId.Body)
						{
							throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
						}
						if (this.attributeLength != 0)
						{
							throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
						}
						if (this.propertyTag.TnefType != TnefPropertyType.String8 && this.propertyTag.TnefType != TnefPropertyType.Unicode)
						{
							throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
						}
						if (this.propertyTag.TnefType == TnefPropertyType.Unicode)
						{
							this.string8AsUnicode = true;
							this.propertyTag = TnefPropertyTag.BodyA;
						}
						this.directWrite = true;
						return;
					}
				}
				else
				{
					switch (tnefAttributeTag)
					{
					case TnefAttributeTag.DateSent:
						if (this.propertyTag.Id != TnefPropertyId.ClientSubmitTime)
						{
							throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
						}
						if (this.fabricatedOffset != 0)
						{
							throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
						}
						if (this.propertyTag.TnefType != TnefPropertyType.SysTime)
						{
							throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
						}
						return;
					case TnefAttributeTag.DateReceived:
						if (this.propertyTag.Id != TnefPropertyId.MessageDeliveryTime)
						{
							throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
						}
						if (this.fabricatedOffset != 0)
						{
							throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
						}
						if (this.propertyTag.TnefType != TnefPropertyType.SysTime)
						{
							throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
						}
						return;
					default:
						switch (tnefAttributeTag)
						{
						case TnefAttributeTag.AttachCreateDate:
							if (this.propertyTag.Id != TnefPropertyId.CreationTime)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
							}
							if (this.fabricatedOffset != 0)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
							}
							if (this.propertyTag.TnefType != TnefPropertyType.SysTime)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
							}
							return;
						case TnefAttributeTag.AttachModifyDate:
							if (this.propertyTag.Id != TnefPropertyId.LastModificationTime)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
							}
							if (this.fabricatedOffset != 0)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
							}
							if (this.propertyTag.TnefType != TnefPropertyType.SysTime)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
							}
							return;
						default:
							if (tnefAttributeTag == TnefAttributeTag.DateModified)
							{
								if (this.propertyTag.Id != TnefPropertyId.LastModificationTime)
								{
									throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
								}
								if (this.fabricatedOffset != 0)
								{
									throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
								}
								if (this.propertyTag.TnefType != TnefPropertyType.SysTime)
								{
									throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
								}
								return;
							}
							break;
						}
						break;
					}
				}
			}
			else if (tnefAttributeTag <= TnefAttributeTag.MessageStatus)
			{
				if (tnefAttributeTag <= TnefAttributeTag.Priority)
				{
					if (tnefAttributeTag != TnefAttributeTag.RequestResponse)
					{
						if (tnefAttributeTag == TnefAttributeTag.Priority)
						{
							if (this.propertyTag.Id != TnefPropertyId.Importance)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
							}
							if (this.fabricatedOffset != 0)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
							}
							if (this.propertyTag.TnefType != TnefPropertyType.Long)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
							}
							return;
						}
					}
					else
					{
						if (this.propertyTag.Id != TnefPropertyId.ResponseRequested)
						{
							throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
						}
						if (this.fabricatedOffset != 0)
						{
							throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
						}
						if (this.propertyTag.TnefType != TnefPropertyType.Boolean)
						{
							throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
						}
						this.directWrite = true;
						return;
					}
				}
				else if (tnefAttributeTag != TnefAttributeTag.AidOwner)
				{
					switch (tnefAttributeTag)
					{
					case TnefAttributeTag.Owner:
						if (this.propertyTag.Id == TnefPropertyId.SentRepresentingEntryId)
						{
							if (this.entryIdOffset != -1)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
							}
							if (this.propertyTag.TnefType != TnefPropertyType.Binary)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
							}
							this.entryIdOffset = this.fabricatedOffset;
							return;
						}
						else if (this.propertyTag.Id == TnefPropertyId.SentRepresentingName)
						{
							if (this.nameOffset != -1)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
							}
							if (this.propertyTag.TnefType != TnefPropertyType.String8 && this.propertyTag.TnefType != TnefPropertyType.Unicode)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
							}
							if (this.propertyTag.TnefType == TnefPropertyType.Unicode)
							{
								this.string8AsUnicode = true;
								this.propertyTag = TnefPropertyTag.SentRepresentingNameA;
							}
							this.nameOffset = this.fabricatedOffset;
							return;
						}
						else if (this.propertyTag.Id == TnefPropertyId.SentRepresentingAddrtype)
						{
							if (this.addrTypeOffset != -1)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
							}
							if (this.propertyTag.TnefType != TnefPropertyType.String8 && this.propertyTag.TnefType != TnefPropertyType.Unicode)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
							}
							if (this.propertyTag.TnefType == TnefPropertyType.Unicode)
							{
								this.string8AsUnicode = true;
								this.propertyTag = TnefPropertyTag.SentRepresentingAddrtypeA;
							}
							this.addrTypeOffset = this.fabricatedOffset;
							return;
						}
						else if (this.propertyTag.Id == TnefPropertyId.SentRepresentingEmailAddress)
						{
							if (this.addressOffset != -1)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
							}
							if (this.propertyTag.TnefType != TnefPropertyType.String8 && this.propertyTag.TnefType != TnefPropertyType.Unicode)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
							}
							if (this.propertyTag.TnefType == TnefPropertyType.Unicode)
							{
								this.string8AsUnicode = true;
								this.propertyTag = TnefPropertyTag.SentRepresentingEmailAddressA;
							}
							this.addressOffset = this.fabricatedOffset;
							return;
						}
						else if (this.propertyTag.Id == TnefPropertyId.RcvdRepresentingEntryId)
						{
							if (this.entryIdOffset != -1)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
							}
							if (this.propertyTag.TnefType != TnefPropertyType.Binary)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
							}
							this.entryIdOffset = this.fabricatedOffset;
							return;
						}
						else if (this.propertyTag.Id == TnefPropertyId.RcvdRepresentingName)
						{
							if (this.nameOffset != -1)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
							}
							if (this.propertyTag.TnefType != TnefPropertyType.String8 && this.propertyTag.TnefType != TnefPropertyType.Unicode)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
							}
							if (this.propertyTag.TnefType == TnefPropertyType.Unicode)
							{
								this.string8AsUnicode = true;
								this.propertyTag = TnefPropertyTag.RcvdRepresentingNameA;
							}
							this.nameOffset = this.fabricatedOffset;
							return;
						}
						else if (this.propertyTag.Id == TnefPropertyId.RcvdRepresentingAddrtype)
						{
							if (this.addrTypeOffset != -1)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
							}
							if (this.propertyTag.TnefType != TnefPropertyType.String8 && this.propertyTag.TnefType != TnefPropertyType.Unicode)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
							}
							if (this.propertyTag.TnefType == TnefPropertyType.Unicode)
							{
								this.string8AsUnicode = true;
								this.propertyTag = TnefPropertyTag.RcvdRepresentingAddrtypeA;
							}
							this.addrTypeOffset = this.fabricatedOffset;
							return;
						}
						else
						{
							if (this.propertyTag.Id != TnefPropertyId.RcvdRepresentingEmailAddress)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
							}
							if (this.addressOffset != -1)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
							}
							if (this.propertyTag.TnefType != TnefPropertyType.String8 && this.propertyTag.TnefType != TnefPropertyType.Unicode)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
							}
							if (this.propertyTag.TnefType == TnefPropertyType.Unicode)
							{
								this.string8AsUnicode = true;
								this.propertyTag = TnefPropertyTag.RcvdRepresentingEmailAddressA;
							}
							this.addressOffset = this.fabricatedOffset;
							return;
						}
						break;
					case TnefAttributeTag.SentFor:
						if (this.propertyTag.Id == TnefPropertyId.SentRepresentingEntryId)
						{
							if (this.entryIdOffset != -1)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
							}
							if (this.propertyTag.TnefType != TnefPropertyType.Binary)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
							}
							this.entryIdOffset = this.fabricatedOffset;
							return;
						}
						else if (this.propertyTag.Id == TnefPropertyId.SentRepresentingName)
						{
							if (this.nameOffset != -1)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
							}
							if (this.propertyTag.TnefType != TnefPropertyType.String8 && this.propertyTag.TnefType != TnefPropertyType.Unicode)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
							}
							if (this.propertyTag.TnefType == TnefPropertyType.Unicode)
							{
								this.string8AsUnicode = true;
								this.propertyTag = TnefPropertyTag.SentRepresentingNameA;
							}
							this.nameOffset = this.fabricatedOffset;
							return;
						}
						else if (this.propertyTag.Id == TnefPropertyId.SentRepresentingAddrtype)
						{
							if (this.addrTypeOffset != -1)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
							}
							if (this.propertyTag.TnefType != TnefPropertyType.String8 && this.propertyTag.TnefType != TnefPropertyType.Unicode)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
							}
							if (this.propertyTag.TnefType == TnefPropertyType.Unicode)
							{
								this.string8AsUnicode = true;
								this.propertyTag = TnefPropertyTag.SentRepresentingAddrtypeA;
							}
							this.addrTypeOffset = this.fabricatedOffset;
							return;
						}
						else
						{
							if (this.propertyTag.Id != TnefPropertyId.SentRepresentingEmailAddress)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
							}
							if (this.addressOffset != -1)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
							}
							if (this.propertyTag.TnefType != TnefPropertyType.String8 && this.propertyTag.TnefType != TnefPropertyType.Unicode)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
							}
							if (this.propertyTag.TnefType == TnefPropertyType.Unicode)
							{
								this.string8AsUnicode = true;
								this.propertyTag = TnefPropertyTag.SentRepresentingEmailAddressA;
							}
							this.addressOffset = this.fabricatedOffset;
							return;
						}
						break;
					case TnefAttributeTag.Delegate:
						if (this.propertyTag.Id != TnefPropertyId.Delegation)
						{
							throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
						}
						if (this.attributeLength != 0)
						{
							throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
						}
						if (this.propertyTag.TnefType != TnefPropertyType.Binary)
						{
							throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
						}
						this.directWrite = true;
						return;
					default:
						if (tnefAttributeTag == TnefAttributeTag.MessageStatus)
						{
							if (this.propertyTag.Id != TnefPropertyId.MessageFlags)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
							}
							if (this.fabricatedOffset != 0)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
							}
							if (this.propertyTag.TnefType != TnefPropertyType.Long)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
							}
							return;
						}
						break;
					}
				}
				else
				{
					if (this.propertyTag.Id != TnefPropertyId.OwnerApptId)
					{
						throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
					}
					if (this.fabricatedOffset != 0)
					{
						throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
					}
					if (this.propertyTag.TnefType != TnefPropertyType.Long)
					{
						throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
					}
					this.directWrite = true;
					return;
				}
			}
			else if (tnefAttributeTag <= TnefAttributeTag.OemCodepage)
			{
				switch (tnefAttributeTag)
				{
				case TnefAttributeTag.AttachData:
					if (this.propertyTag.Id == TnefPropertyId.AttachData)
					{
						if (this.attributeLength != 0)
						{
							throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
						}
						if (this.propertyTag.TnefType != TnefPropertyType.Binary && this.propertyTag.TnefType != TnefPropertyType.Object)
						{
							throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
						}
						this.directWrite = true;
						return;
					}
					else
					{
						if (this.propertyTag.Id != TnefPropertyId.AttachTag)
						{
							throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
						}
						if (this.fabricatedOffset != 0)
						{
							throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
						}
						if (this.propertyTag.TnefType != TnefPropertyType.Binary)
						{
							throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
						}
						return;
					}
					break;
				case (TnefAttributeTag)426000:
					break;
				case TnefAttributeTag.AttachMetaFile:
					if (this.propertyTag.Id != TnefPropertyId.AttachRendering)
					{
						throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
					}
					if (this.attributeLength != 0)
					{
						throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
					}
					if (this.propertyTag.TnefType != TnefPropertyType.Binary)
					{
						throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
					}
					this.directWrite = true;
					return;
				default:
					switch (tnefAttributeTag)
					{
					case TnefAttributeTag.AttachTransportFilename:
						if (this.propertyTag.Id != TnefPropertyId.AttachTransportName)
						{
							throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
						}
						if (this.attributeLength != 0)
						{
							throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
						}
						if (this.propertyTag.TnefType != TnefPropertyType.String8 && this.propertyTag.TnefType != TnefPropertyType.Unicode)
						{
							throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
						}
						if (this.propertyTag.TnefType == TnefPropertyType.Unicode)
						{
							this.string8AsUnicode = true;
							this.propertyTag = TnefPropertyTag.AttachTransportNameA;
						}
						this.directWrite = true;
						return;
					case TnefAttributeTag.AttachRenderData:
						if (this.propertyTag.Id == TnefPropertyId.RenderingPosition)
						{
							if (this.renderingPositionOffset != -1)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
							}
							if (this.propertyTag.TnefType != TnefPropertyType.Long)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
							}
							this.renderingPositionOffset = this.fabricatedOffset;
							return;
						}
						else if (this.propertyTag.Id == TnefPropertyId.AttachMethod)
						{
							if (this.attachMethodOffset != -1)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
							}
							if (this.propertyTag.TnefType != TnefPropertyType.Long)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
							}
							this.attachMethodOffset = this.fabricatedOffset;
							return;
						}
						else
						{
							if (this.propertyTag.Id != TnefPropertyId.AttachEncoding)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
							}
							if (this.attachEncodingOffset != -1)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
							}
							if (this.propertyTag.TnefType != TnefPropertyType.Binary)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
							}
							this.attachEncodingOffset = this.fabricatedOffset;
							return;
						}
						break;
					default:
						if (tnefAttributeTag != TnefAttributeTag.OemCodepage)
						{
						}
						break;
					}
					break;
				}
			}
			else if (tnefAttributeTag != TnefAttributeTag.OriginalMessageClass)
			{
				if (tnefAttributeTag == TnefAttributeTag.MessageClass)
				{
					if (this.propertyTag.Id != TnefPropertyId.MessageClass)
					{
						throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
					}
					if (this.fabricatedOffset != 0)
					{
						throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
					}
					if (this.propertyTag.TnefType != TnefPropertyType.String8 && this.propertyTag.TnefType != TnefPropertyType.Unicode)
					{
						throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
					}
					if (this.propertyTag.TnefType == TnefPropertyType.Unicode)
					{
						this.string8AsUnicode = true;
						this.propertyTag = TnefPropertyTag.MessageClassA;
						return;
					}
					return;
				}
			}
			else
			{
				if (this.propertyTag.Id != TnefPropertyId.OrigMessageClass)
				{
					throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
				}
				if (this.fabricatedOffset != 0)
				{
					throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
				}
				if (this.propertyTag.TnefType != TnefPropertyType.String8 && this.propertyTag.TnefType != TnefPropertyType.Unicode)
				{
					throw new NotSupportedException(TnefStrings.WriterNotSupportedInvalidPropertyType);
				}
				if (this.propertyTag.TnefType == TnefPropertyType.Unicode)
				{
					this.string8AsUnicode = true;
					this.propertyTag = TnefPropertyTag.OrigMessageClassA;
					return;
				}
				return;
			}
			throw new NotSupportedException(TnefStrings.WriterNotSupportedCannotAddAnyPropertyToAttribute);
		}

		private void CompleteLegacyAttribute()
		{
			this.directWrite = true;
			TnefAttributeTag tnefAttributeTag = this.attributeTag;
			if (tnefAttributeTag <= TnefAttributeTag.DateModified)
			{
				if (tnefAttributeTag <= TnefAttributeTag.DateEnd)
				{
					if (tnefAttributeTag != TnefAttributeTag.From)
					{
						switch (tnefAttributeTag)
						{
						case TnefAttributeTag.DateStart:
						case TnefAttributeTag.DateEnd:
							break;
						default:
							return;
						}
					}
					else
					{
						if (this.nameOffset == -1 || this.addrTypeOffset == -1 || this.addressOffset == -1)
						{
							if (this.entryIdOffset != -1)
							{
								this.CrackEntryId();
							}
							if (this.nameOffset == -1 || this.addrTypeOffset == -1 || this.addressOffset == -1)
							{
								if (!this.disposing)
								{
									throw new NotSupportedException(TnefStrings.WriterNotSupportedNotEnoughInformationForAttribute);
								}
								return;
							}
						}
						int num = TnefCommon.StrZLength(this.fabricatedBuffer, this.nameOffset, this.fabricatedOffset);
						int num2 = TnefCommon.StrZLength(this.fabricatedBuffer, this.addrTypeOffset, this.fabricatedOffset);
						int num3 = TnefCommon.StrZLength(this.fabricatedBuffer, this.addressOffset, this.fabricatedOffset);
						if (num != 0 && num2 != 0 && num3 != 0)
						{
							short num4 = (short)(num + 1);
							if ((num4 & 1) != 0)
							{
								num4 += 1;
							}
							short num5 = (short)(num2 + 1 + num3 + 1);
							short value = 16 + num4 + num5;
							this.WriteWord(4);
							this.WriteWord(value);
							this.WriteWord(num4);
							this.WriteWord(num5);
							this.WriteBinary(this.fabricatedBuffer, this.nameOffset, num);
							this.WriteByte(0);
							if ((num + 1 & 1) != 0)
							{
								this.WriteByte(0);
							}
							this.WriteBinary(this.fabricatedBuffer, this.addrTypeOffset, num2);
							this.WriteByte(58);
							this.WriteBinary(this.fabricatedBuffer, this.addressOffset, num3);
							this.WriteByte(0);
							this.WriteQword(0L);
							return;
						}
						if (!this.disposing)
						{
							throw new InvalidCalendarDataException(TnefStrings.WriterNotSupportedInvalidRecipientInformation);
						}
						return;
					}
				}
				else
				{
					switch (tnefAttributeTag)
					{
					case TnefAttributeTag.DateSent:
					case TnefAttributeTag.DateReceived:
						break;
					default:
						switch (tnefAttributeTag)
						{
						case TnefAttributeTag.AttachCreateDate:
						case TnefAttributeTag.AttachModifyDate:
							break;
						default:
							if (tnefAttributeTag != TnefAttributeTag.DateModified)
							{
								return;
							}
							break;
						}
						break;
					}
				}
				if (this.fabricatedOffset != 8)
				{
					if (!this.disposing)
					{
						throw new NotSupportedException(TnefStrings.WriterInvalidOperationValueSizeInvalidForType);
					}
					while (this.fabricatedOffset < 8)
					{
						this.EnsureFabricatedSpace(1);
						this.fabricatedBuffer[this.fabricatedOffset++] = 0;
					}
				}
				DateTime dateTime = DateTime.FromFileTimeUtc(BitConverter.ToInt64(this.fabricatedBuffer, 0));
				this.WriteWord((short)dateTime.Year);
				this.WriteWord((short)dateTime.Month);
				this.WriteWord((short)dateTime.Day);
				this.WriteWord((short)dateTime.Hour);
				this.WriteWord((short)dateTime.Minute);
				this.WriteWord((short)dateTime.Second);
				this.WriteWord((short)dateTime.DayOfWeek);
				return;
			}
			if (tnefAttributeTag <= TnefAttributeTag.MessageStatus)
			{
				if (tnefAttributeTag == TnefAttributeTag.Priority)
				{
					if (this.fabricatedOffset != 4)
					{
						if (!this.disposing)
						{
							throw new NotSupportedException(TnefStrings.WriterInvalidOperationValueSizeInvalidForType);
						}
						while (this.fabricatedOffset < 4)
						{
							this.EnsureFabricatedSpace(1);
							this.fabricatedBuffer[this.fabricatedOffset++] = 0;
						}
					}
					int num6 = BitConverter.ToInt32(this.fabricatedBuffer, 0);
					short value2 = 2;
					if (num6 == 0)
					{
						value2 = 3;
					}
					else if (num6 == 2)
					{
						value2 = 1;
					}
					this.WriteWord(value2);
					return;
				}
				switch (tnefAttributeTag)
				{
				case TnefAttributeTag.Owner:
				case TnefAttributeTag.SentFor:
				{
					if (this.nameOffset == -1 || this.addrTypeOffset == -1 || this.addressOffset == -1)
					{
						if (this.entryIdOffset != -1)
						{
							this.CrackEntryId();
						}
						if (this.nameOffset == -1 || this.addrTypeOffset == -1 || this.addressOffset == -1)
						{
							if (!this.disposing)
							{
								throw new NotSupportedException(TnefStrings.WriterNotSupportedNotEnoughInformationForAttribute);
							}
							break;
						}
					}
					int num7 = TnefCommon.StrZLength(this.fabricatedBuffer, this.nameOffset, this.fabricatedOffset);
					int num8 = TnefCommon.StrZLength(this.fabricatedBuffer, this.addrTypeOffset, this.fabricatedOffset);
					int num9 = TnefCommon.StrZLength(this.fabricatedBuffer, this.addressOffset, this.fabricatedOffset);
					if (num7 == 0 || num8 == 0 || num9 == 0)
					{
						if (!this.disposing)
						{
							throw new InvalidCalendarDataException(TnefStrings.WriterNotSupportedInvalidRecipientInformation);
						}
					}
					else
					{
						this.WriteWord((short)(num7 + 1));
						this.WriteBinary(this.fabricatedBuffer, this.nameOffset, num7);
						this.WriteByte(0);
						this.WriteWord((short)(num8 + 1 + num9 + 1));
						this.WriteBinary(this.fabricatedBuffer, this.addrTypeOffset, num8);
						this.WriteByte(58);
						this.WriteBinary(this.fabricatedBuffer, this.addressOffset, num9);
						this.WriteByte(0);
					}
					break;
				}
				default:
				{
					if (tnefAttributeTag != TnefAttributeTag.MessageStatus)
					{
						return;
					}
					if (this.fabricatedOffset != 4)
					{
						if (!this.disposing)
						{
							throw new NotSupportedException(TnefStrings.WriterInvalidOperationValueSizeInvalidForType);
						}
						while (this.fabricatedOffset < 4)
						{
							this.EnsureFabricatedSpace(1);
							this.fabricatedBuffer[this.fabricatedOffset++] = 0;
						}
					}
					int num10 = BitConverter.ToInt32(this.fabricatedBuffer, 0);
					int num11 = 0;
					num11 |= (((num10 & 1) != 0) ? 32 : 0);
					num11 |= (((num10 & 2) != 0) ? 0 : 1);
					num11 |= (((num10 & 4) != 0) ? 4 : 0);
					num11 |= (((num10 & 16) != 0) ? 128 : 0);
					num11 |= (((num10 & 8) != 0) ? 2 : 0);
					this.WriteByte((byte)num11);
					return;
				}
				}
			}
			else
			{
				if (tnefAttributeTag == TnefAttributeTag.AttachRenderData)
				{
					short value3 = 1;
					int value4 = 0;
					int value5 = -1;
					short value6 = -1;
					short value7 = -1;
					if (this.attachMethodOffset != -1)
					{
						int num12 = BitConverter.ToInt32(this.fabricatedBuffer, this.attachMethodOffset);
						if (num12 != 6)
						{
							if (this.attachEncodingOffset != -1 && TnefCommon.BytesEqualToPattern(this.fabricatedBuffer, this.attachEncodingOffset, TnefCommon.OidMacBinary))
							{
								value4 = 1;
							}
							value6 = 32;
							value7 = 32;
						}
						else
						{
							value3 = 2;
						}
					}
					if (this.renderingPositionOffset != -1)
					{
						value5 = BitConverter.ToInt32(this.fabricatedBuffer, this.renderingPositionOffset);
					}
					this.WriteWord(value3);
					this.WriteDword(value5);
					this.WriteWord(value6);
					this.WriteWord(value7);
					this.WriteDword(value4);
					return;
				}
				if (tnefAttributeTag != TnefAttributeTag.OriginalMessageClass && tnefAttributeTag != TnefAttributeTag.MessageClass)
				{
					return;
				}
				if (this.fabricatedOffset == 0 || this.fabricatedBuffer[0] == 0)
				{
					throw new ExchangeDataException(TnefStrings.WriterNotSupportedInvalidMessageClass);
				}
				int i;
				for (i = 0; i < TnefCommon.MessageClassMappingTable.Length; i++)
				{
					if (this.fabricatedOffset == TnefCommon.MessageClassMappingTable[i].MapiName.Length + 1 && TnefCommon.BytesEqualToPattern(this.fabricatedBuffer, 0, TnefCommon.MessageClassMappingTable[i].MapiName) && this.fabricatedBuffer[this.fabricatedOffset - 1] == 0)
					{
						TnefAttributeTag tnefAttributeTag2 = this.attributeTag;
						byte[] bytes = CTSGlobals.AsciiEncoding.GetBytes(TnefCommon.MessageClassMappingTable[i].LegacyName);
						this.WriteBinary(bytes, 0, bytes.Length);
						this.WriteByte(0);
						break;
					}
				}
				if (i == TnefCommon.MessageClassMappingTable.Length)
				{
					this.WriteBinary(this.fabricatedBuffer, 0, this.fabricatedOffset);
					if (this.fabricatedBuffer[this.fabricatedOffset - 1] != 0)
					{
						this.WriteByte(0);
						return;
					}
				}
			}
		}

		private void CrackEntryId()
		{
			if (this.fabricatedOffset - this.entryIdOffset < 27)
			{
				throw new ArgumentException(TnefStrings.WriterNotSupportedMallformedEntryId);
			}
			if (!TnefCommon.BytesEqualToPattern(this.fabricatedBuffer, this.entryIdOffset + 4, TnefCommon.MuidOOP))
			{
				throw new NotSupportedException(TnefStrings.WriterNotSupportedNotOneOffEntryId);
			}
			int num = BitConverter.ToInt32(this.fabricatedBuffer, this.entryIdOffset + 20);
			if (((long)num & (long)((ulong)-2147483648)) != 0L)
			{
				throw new NotSupportedException(TnefStrings.WriterNotSupportedUnicodeOneOffEntryId);
			}
			int num2 = this.entryIdOffset + 24;
			int num3 = TnefCommon.StrZLength(this.fabricatedBuffer, num2, this.fabricatedOffset);
			int num4 = num2 + num3 + 1;
			if (num4 >= this.fabricatedOffset)
			{
				throw new ArgumentException(TnefStrings.WriterNotSupportedMallformedEntryId);
			}
			int num5 = TnefCommon.StrZLength(this.fabricatedBuffer, num4, this.fabricatedOffset);
			int num6 = num4 + num5 + 1;
			if (num6 >= this.fabricatedOffset)
			{
				throw new ArgumentException(TnefStrings.WriterNotSupportedMallformedEntryId);
			}
			int num7 = TnefCommon.StrZLength(this.fabricatedBuffer, num6, this.fabricatedOffset);
			if (num7 + 1 > this.fabricatedOffset)
			{
				throw new ArgumentException(TnefStrings.WriterNotSupportedMallformedEntryId);
			}
			if (this.nameOffset == -1)
			{
				this.nameOffset = num2;
			}
			if (this.addrTypeOffset == -1)
			{
				this.addrTypeOffset = num4;
			}
			if (this.addressOffset == -1)
			{
				this.addressOffset = num6;
			}
		}

		internal void AssertGoodToUse(bool affectsChild)
		{
			if (this.outputStream == null)
			{
				throw new ObjectDisposedException("TnefWriter");
			}
			if (affectsChild && this.Child != null)
			{
				throw new InvalidOperationException(TnefStrings.WriterInvalidOperationChildActive);
			}
		}

		private static double ToOADate(DateTime value)
		{
			return value.ToOADate();
		}

		private static TypeCode GetTypeCode(object value)
		{
			Type type = value.GetType();
			return Type.GetTypeCode(type);
		}

		private const int WriteBufferSize = 4096;

		private TnefWriterFlags flags;

		private bool disposing;

		private TnefWriter parent;

		internal object Child;

		private int messageCodePage;

		private Encoder unicodeEncoder;

		private Encoder string8Encoder;

		private bool canFlush;

		private Stream outputStream;

		private int streamOffset;

		private Stream writeStream;

		private int writeStreamOffset;

		private byte[] writeBuffer;

		private int writeOffset;

		private TnefWriter.WriteState writeState;

		private ushort checksum;

		private ushort attributeStartChecksum;

		private short attachmentKey;

		private TnefAttributeLevel attributeLevel;

		private TnefAttributeTag attributeTag;

		private bool legacyAttribute;

		private int totalLength;

		private int attributeLengthOffset;

		private int attributeLength;

		private int rowCountOffset;

		private int rowCount;

		private int propertyCountOffset;

		private int propertyCount;

		private int valueCountOffset;

		private int valueCount;

		private int valueLengthOffset;

		private int valueLength;

		private TnefPropertyTag propertyTag;

		private int valueFixedLength;

		private char[] charBuffer;

		private byte[] byteBuffer;

		private bool valueAsText;

		private bool directWrite;

		private bool string8AsUnicode;

		private bool hexEncodeBinary;

		private byte[] fabricatedBuffer;

		private int fabricatedOffset;

		private int renderingPositionOffset;

		private int attachMethodOffset;

		private int attachEncodingOffset;

		private int entryIdOffset;

		private int nameOffset;

		private int addrTypeOffset;

		private int addressOffset;

		private enum WriteState
		{
			BeforeAttribute,
			BeginAttribute,
			WriteAttributeValue,
			BeforeRow,
			BeforeProperty,
			BeforePropertyValue,
			BeginPropertyValue,
			WritePropertyValue
		}
	}
}
