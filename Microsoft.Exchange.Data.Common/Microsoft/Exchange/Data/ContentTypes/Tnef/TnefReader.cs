using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Globalization;

namespace Microsoft.Exchange.Data.ContentTypes.Tnef
{
	public class TnefReader : IDisposable
	{
		public TnefReader(Stream inputStream) : this(inputStream, 0, TnefComplianceMode.Strict)
		{
		}

		public TnefReader(Stream inputStream, int defaultMessageCodepage, TnefComplianceMode complianceMode)
		{
			if (inputStream == null)
			{
				throw new ArgumentNullException("inputStream");
			}
			if (!inputStream.CanRead)
			{
				throw new NotSupportedException(TnefStrings.StreamDoesNotSupportRead);
			}
			this.InputStream = inputStream;
			this.streamMaxLength = int.MaxValue;
			this.complianceMode = complianceMode;
			this.complianceStatus = TnefComplianceStatus.Compliant;
			if (TnefCommon.IsUnicodeCodepage(defaultMessageCodepage))
			{
				defaultMessageCodepage = 0;
			}
			this.messageCodepage = defaultMessageCodepage;
			this.readBuffer = new byte[4096];
			this.fabricatedBuffer = new byte[512];
			this.propertyReader = new TnefPropertyReader(this);
			this.unicodeDecoder = Encoding.Unicode.GetDecoder();
			this.ReadTnefHeader();
		}

		internal TnefReader(TnefReader parent)
		{
			this.embeddingDepth = parent.embeddingDepth + 1;
			this.parent = parent;
			this.parent.ReadStateValue = TnefReader.ReadState.ReadPropertyValue;
			this.InputStream = parent.InputStream;
			this.streamOffset = -parent.readOffset;
			this.streamMaxLength = Math.Min(parent.propertyValueLength, parent.streamMaxLength - parent.propertyValueStreamOffset);
			this.complianceMode = parent.complianceMode;
			this.complianceStatus = TnefComplianceStatus.Compliant;
			this.messageCodepage = parent.MessageCodepage;
			this.checksumDisabled = parent.checksumDisabled;
			this.readBuffer = parent.readBuffer;
			this.readOffset = parent.readOffset;
			this.readEnd = parent.readEnd;
			this.readEndReal = parent.readEndReal;
			this.endOfFile = parent.endOfFile;
			if (this.streamOffset + this.readEnd > this.streamMaxLength)
			{
				this.readEnd = this.streamMaxLength - this.streamOffset;
				this.endOfFile = true;
			}
			this.propertyReader = new TnefPropertyReader(this);
			this.unicodeDecoder = parent.unicodeDecoder;
			this.string8Decoder = parent.string8Decoder;
			this.fabricatedBuffer = parent.fabricatedBuffer;
			this.decodeBuffer = parent.decodeBuffer;
			this.ReadTnefHeader();
			this.parent.Child = this;
		}

		public TnefComplianceMode ComplianceMode
		{
			get
			{
				this.AssertGoodToUse(false);
				return this.complianceMode;
			}
		}

		public TnefComplianceStatus ComplianceStatus
		{
			get
			{
				this.AssertGoodToUse(false);
				return this.complianceStatus;
			}
		}

		public int StreamOffset
		{
			get
			{
				this.AssertGoodToUse(true);
				return this.streamOffset + this.readOffset;
			}
		}

		public int TnefVersion
		{
			get
			{
				this.AssertGoodToUse(false);
				return this.tnefVersion;
			}
		}

		public short AttachmentKey
		{
			get
			{
				this.AssertGoodToUse(false);
				return this.attachmentKey;
			}
		}

		public int MessageCodepage
		{
			get
			{
				this.AssertGoodToUse(false);
				return this.messageCodepage;
			}
			set
			{
				this.AssertGoodToUse(false);
				this.messageCodepage = value;
				this.string8Decoder = null;
			}
		}

		public TnefAttributeLevel AttributeLevel
		{
			get
			{
				this.AssertGoodToUse(false);
				this.AssertInAttribute();
				return this.attributeLevel;
			}
		}

		public TnefAttributeTag AttributeTag
		{
			get
			{
				this.AssertGoodToUse(false);
				this.AssertInAttribute();
				return this.attributeTag;
			}
		}

		public TnefPropertyReader PropertyReader
		{
			get
			{
				this.AssertGoodToUse(true);
				this.AssertInAttribute();
				if (this.ReadStateValue == TnefReader.ReadState.ReadAttributeValue)
				{
					throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationPropAfterRaw);
				}
				return this.propertyReader;
			}
		}

		public int AttributeRawValueStreamOffset
		{
			get
			{
				this.AssertGoodToUse(false);
				this.AssertInAttribute();
				return this.attributeValueStreamOffset;
			}
		}

		public int AttributeRawValueLength
		{
			get
			{
				this.AssertGoodToUse(false);
				this.AssertInAttribute();
				return this.attributeValueLength;
			}
		}

		internal int RowCount
		{
			get
			{
				this.AssertGoodToUse(false);
				this.AssertInAttribute();
				if (this.attributeTag != TnefAttributeTag.RecipientTable)
				{
					throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationRowsOnlyInRecipientTable);
				}
				return this.rowCount;
			}
		}

		internal int PropertyCount
		{
			get
			{
				this.AssertGoodToUse(false);
				this.AssertInAttribute();
				return this.propertyCount;
			}
		}

		internal TnefPropertyTag PropertyTag
		{
			get
			{
				this.AssertGoodToUse(false);
				this.AssertInProperty();
				return this.propertyTag;
			}
		}

		internal int PropertyValueCount
		{
			get
			{
				this.AssertGoodToUse(false);
				this.AssertInProperty();
				return this.propertyValueCount;
			}
		}

		internal Guid PropertyValueOleIID
		{
			get
			{
				this.AssertGoodToUse(false);
				this.AssertInPropertyValue();
				if (this.propertyTag.ValueTnefType != TnefPropertyType.Object)
				{
					throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationNotObjectProperty);
				}
				return this.propertyValueIId;
			}
		}

		internal bool IsPropertyEmbeddedMessage
		{
			get
			{
				this.AssertGoodToUse(false);
				this.AssertInProperty();
				if (this.propertyTag.ValueTnefType != TnefPropertyType.Object)
				{
					return false;
				}
				this.AssertInPropertyValue();
				return this.propertyValueIId == TnefCommon.MessageIID;
			}
		}

		internal TnefNameId PropertyNameId
		{
			get
			{
				this.AssertGoodToUse(false);
				this.AssertInProperty();
				if (!this.propertyTag.IsNamed)
				{
					throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationNotNamedProperty);
				}
				return this.propertyNameId;
			}
		}

		internal bool IsComputedProperty
		{
			get
			{
				this.AssertGoodToUse(false);
				this.AssertInProperty();
				return !this.directRead;
			}
		}

		internal int PropertyRawValueStreamOffset
		{
			get
			{
				this.AssertGoodToUse(false);
				this.AssertInPropertyValue();
				if (this.IsComputedProperty)
				{
					return -1;
				}
				return this.propertyValueStreamOffset;
			}
		}

		internal int PropertyRawValueLength
		{
			get
			{
				this.AssertGoodToUse(false);
				this.AssertInPropertyValue();
				return this.propertyValueLength;
			}
		}

		public int ReadAttributeRawValue(byte[] buffer, int offset, int count)
		{
			this.AssertGoodToUse(true);
			this.AssertInAttribute();
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
			if (this.ReadStateValue == TnefReader.ReadState.BeginAttribute)
			{
				this.ReadStateValue = TnefReader.ReadState.ReadAttributeValue;
			}
			else if (this.ReadStateValue != TnefReader.ReadState.ReadAttributeValue)
			{
				if (this.ReadStateValue == TnefReader.ReadState.EndAttribute)
				{
					return 0;
				}
				throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationRawAfterProp);
			}
			int num = 0;
			while (this.attributeValueOffset < this.attributeValueLength && count != 0)
			{
				if (!this.EnsureMoreDataLoaded(1))
				{
					this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, TnefStrings.ReaderComplianceTnefTruncated);
					this.ReadStateValue = TnefReader.ReadState.EndAttribute;
					this.error = true;
					break;
				}
				int num2 = Math.Min(count, Math.Min(this.AttributeRemainingCount(), this.AvailableCount()));
				this.ReadBytes(buffer, offset, num2);
				offset += num2;
				count -= num2;
				num += num2;
			}
			if (this.attributeValueOffset == this.attributeValueLength && !this.error)
			{
				this.VerifyAttributeChecksum();
			}
			return num;
		}

		public bool ReadNextAttribute()
		{
			this.AssertGoodToUse(true);
			if (this.ReadStateValue == TnefReader.ReadState.EndOfFile)
			{
				return false;
			}
			if (this.ReadStateValue > TnefReader.ReadState.EndAttribute && this.attributeValueOffset <= this.attributeValueLength)
			{
				this.SkipRemainderOfCurrentAttribute();
			}
			if (this.error)
			{
				this.ReadStateValue = TnefReader.ReadState.EndOfFile;
				return false;
			}
			if (!this.ReadAttributeHeader())
			{
				this.ReadStateValue = TnefReader.ReadState.EndOfFile;
				return false;
			}
			return true;
		}

		public void ResetComplianceStatus()
		{
			this.complianceStatus = TnefComplianceStatus.Compliant;
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
				if (this.InputStream == null)
				{
					return;
				}
				if (this.Child != null)
				{
					if (this.Child is TnefReaderStreamWrapper)
					{
						(this.Child as TnefReaderStreamWrapper).Dispose();
					}
					else if (this.Child is TnefReader)
					{
						(this.Child as TnefReader).Dispose();
					}
				}
				if (this.parent == null)
				{
					this.InputStream.Dispose();
				}
				else
				{
					this.parent.propertyValueOffset += this.StreamOffset;
					this.parent.attributeValueOffset += this.StreamOffset;
					TnefReader tnefReader = this.parent;
					tnefReader.checksum += this.checksum;
					this.parent.streamOffset += this.parent.readOffset + this.streamOffset;
					this.parent.readBuffer = this.readBuffer;
					this.parent.readOffset = this.readOffset;
					this.parent.readEnd = this.readEndReal;
					this.parent.readEndReal = this.readEndReal;
					if (this.parent.streamOffset + this.parent.readEnd > this.parent.streamMaxLength)
					{
						this.parent.readEnd = this.parent.streamMaxLength - this.parent.streamOffset;
						this.parent.endOfFile = true;
					}
					this.parent.Child = null;
				}
			}
			this.InputStream = null;
			this.parent = null;
			this.readBuffer = null;
			this.fabricatedBuffer = null;
			this.decoder = null;
			this.unicodeDecoder = null;
			this.string8Decoder = null;
		}

		private void ReadTnefHeader()
		{
			if (!this.EnsureMoreDataLoaded(6))
			{
				this.SetComplianceStatus(TnefComplianceStatus.InvalidTnefSignature, TnefStrings.ReaderComplianceInvalidTnefSignature);
				this.ReadStateValue = TnefReader.ReadState.EndOfFile;
				this.error = true;
				return;
			}
			int num = this.ReadDword();
			if (num != 574529400)
			{
				this.SetComplianceStatus(TnefComplianceStatus.InvalidTnefSignature, TnefStrings.ReaderComplianceInvalidTnefSignature);
				this.ReadStateValue = TnefReader.ReadState.EndOfFile;
				this.error = true;
				return;
			}
			this.attachmentKey = this.ReadWord();
			if (this.EnsureMoreDataLoaded(15) && this.PeekByte(0) == 1 && this.PeekDword(1) == 561158 && this.PeekDword(5) == 4)
			{
				this.tnefVersion = this.PeekDword(9);
				if (this.tnefVersion > 65536)
				{
					this.SetComplianceStatus(TnefComplianceStatus.InvalidTnefVersion, TnefStrings.ReaderComplianceInvalidTnefVersion);
				}
			}
			if (this.messageCodepage == 0 && this.EnsureMoreDataLoaded(34) && this.PeekByte(15) == 1 && this.PeekDword(16) == 430087 && this.PeekDword(20) == 8)
			{
				int messageCodePage = this.PeekDword(24);
				if (!TnefCommon.IsUnicodeCodepage(messageCodePage))
				{
					this.messageCodepage = messageCodePage;
					this.string8Decoder = null;
				}
			}
			this.ReadStateValue = TnefReader.ReadState.Begin;
		}

		private bool ReadAttributeHeader()
		{
			if (!this.EnsureMoreDataLoaded(9))
			{
				return false;
			}
			this.attributeLevel = (TnefAttributeLevel)this.ReadByte();
			this.attributeTag = (TnefAttributeTag)this.ReadDword();
			this.attributeValueLength = this.ReadDword();
			if (this.attributeValueLength < 0)
			{
				this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeLength, TnefStrings.ReaderComplianceInvalidAttributeLength);
				this.error = true;
				return false;
			}
			if (this.attributeLevel != TnefAttributeLevel.Message && this.attributeLevel != TnefAttributeLevel.Attachment)
			{
				this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeLevel, TnefStrings.ReaderComplianceInvalidAttributeLevel);
			}
			this.legacyAttribute = (this.attributeTag != TnefAttributeTag.MapiProperties && this.attributeTag != TnefAttributeTag.Attachment && this.attributeTag != TnefAttributeTag.RecipientTable);
			this.attributeValueStreamOffset = this.StreamOffset;
			this.attributeValueOffset = 0;
			this.attributeStartChecksum = this.checksum;
			this.ReadStateValue = TnefReader.ReadState.BeginAttribute;
			return this.PreviewAttributeContent();
		}

		private void SkipRemainderOfCurrentAttribute()
		{
			if (this.attributeValueOffset <= this.attributeValueLength)
			{
				if (this.attributeValueOffset < this.attributeValueLength)
				{
					this.EatAttributeBytes(this.AttributeRemainingCount());
				}
				if (!this.error)
				{
					this.VerifyAttributeChecksum();
				}
			}
		}

		private void VerifyAttributeChecksum()
		{
			if (!this.EnsureMoreDataLoaded(2))
			{
				this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, TnefStrings.ReaderComplianceTnefTruncated);
				this.error = true;
				return;
			}
			ushort num = this.checksum - this.attributeStartChecksum;
			ushort num2 = (ushort)this.ReadWord();
			if (!this.checksumDisabled && num != num2 && this.attributeTag != TnefAttributeTag.MessageClass && this.attributeTag != TnefAttributeTag.OriginalMessageClass)
			{
				this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeChecksum, TnefStrings.ReaderComplianceInvalidAttributeChecksum);
			}
		}

		internal bool ReadNextRow()
		{
			this.AssertGoodToUse(true);
			this.AssertInAttribute();
			if (this.attributeTag != TnefAttributeTag.RecipientTable)
			{
				throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationReadNextRowOnlyInRecipientTable);
			}
			if (this.ReadStateValue != TnefReader.ReadState.EndRow)
			{
				if (this.ReadStateValue == TnefReader.ReadState.EndAttribute)
				{
					return false;
				}
				if (this.ReadStateValue == TnefReader.ReadState.BeginAttribute)
				{
					this.ReadDword();
					this.rowIndex = -1;
				}
				else
				{
					if (this.ReadStateValue == TnefReader.ReadState.ReadAttributeValue)
					{
						throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationPropAfterRaw);
					}
					while (this.ReadNextProperty())
					{
					}
					if (this.error)
					{
						this.ReadStateValue = TnefReader.ReadState.EndAttribute;
						return false;
					}
				}
			}
			if (++this.rowIndex == this.rowCount)
			{
				this.SkipRemainderOfCurrentAttribute();
				this.ReadStateValue = TnefReader.ReadState.EndAttribute;
				return false;
			}
			if (!this.CheckAndEnsureMoreAttributeDataLoaded(4))
			{
				this.ReadStateValue = TnefReader.ReadState.EndAttribute;
				return false;
			}
			this.propertyCount = this.PeekDword(0);
			this.propertyIndex = -1;
			this.ReadStateValue = TnefReader.ReadState.BeginRow;
			return true;
		}

		internal bool ReadNextProperty()
		{
			this.AssertGoodToUse(true);
			this.AssertInAttribute();
			if (this.ReadStateValue != TnefReader.ReadState.EndProperty)
			{
				if (this.ReadStateValue == TnefReader.ReadState.EndRow || this.ReadStateValue == TnefReader.ReadState.EndAttribute)
				{
					return false;
				}
				if (this.ReadStateValue == TnefReader.ReadState.BeginAttribute)
				{
					if (!this.PrepareFirstProperty())
					{
						this.ReadStateValue = ((this.attributeTag != TnefAttributeTag.RecipientTable) ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow);
						this.error = true;
						return false;
					}
					this.propertyIndex = -1;
				}
				else if (this.ReadStateValue == TnefReader.ReadState.BeginRow)
				{
					this.ReadDword();
					this.propertyIndex = -1;
				}
				else
				{
					if (this.ReadStateValue == TnefReader.ReadState.ReadAttributeValue)
					{
						throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationPropAfterRaw);
					}
					if (!this.legacyAttribute)
					{
						if (this.propertyTag.IsMultiValued)
						{
							while (this.ReadNextPropertyValue())
							{
							}
						}
						else if (this.propertyTag.ValueTnefType == TnefPropertyType.Null)
						{
							this.ReadStateValue = TnefReader.ReadState.EndProperty;
						}
						else
						{
							if (this.ReadStateValue == TnefReader.ReadState.BeginProperty && !this.ReadNextPropertyValue())
							{
								this.ReadStateValue = ((this.attributeTag != TnefAttributeTag.RecipientTable) ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow);
								return false;
							}
							if (this.propertyValueOffset != this.propertyValueLength)
							{
								this.EatPropertyBytes(this.PropertyRemainingCount());
							}
							if (this.propertyValuePaddingLength != 0)
							{
								this.EatAttributeBytes(this.propertyValuePaddingLength);
								this.propertyValuePaddingLength = 0;
							}
							if (this.propertyPaddingLength != 0)
							{
								this.EatAttributeBytes(this.propertyPaddingLength);
								this.propertyPaddingLength = 0;
							}
						}
						if (this.error)
						{
							this.ReadStateValue = ((this.attributeTag != TnefAttributeTag.RecipientTable) ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow);
							return false;
						}
					}
				}
			}
			if (++this.propertyIndex == this.propertyCount)
			{
				if (this.attributeTag != TnefAttributeTag.RecipientTable)
				{
					this.SkipRemainderOfCurrentAttribute();
					this.ReadStateValue = TnefReader.ReadState.EndAttribute;
				}
				else
				{
					this.ReadStateValue = TnefReader.ReadState.EndRow;
				}
				return false;
			}
			if (this.legacyAttribute)
			{
				if (!this.PrepareLegacyProperty())
				{
					this.ReadStateValue = ((this.attributeTag != TnefAttributeTag.RecipientTable) ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow);
					this.error = true;
					return false;
				}
				if (!this.CheckPropertyType())
				{
					this.ReadStateValue = ((this.attributeTag != TnefAttributeTag.RecipientTable) ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow);
					return false;
				}
			}
			else
			{
				this.directRead = true;
				if (!this.CheckAndEnsureMoreAttributeDataLoaded(4))
				{
					this.ReadStateValue = ((this.attributeTag != TnefAttributeTag.RecipientTable) ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow);
					return false;
				}
				this.propertyTag = this.ReadDword();
				if (!this.CheckPropertyType())
				{
					this.ReadStateValue = ((this.attributeTag != TnefAttributeTag.RecipientTable) ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow);
					return false;
				}
				if (this.propertyTag.IsNamed)
				{
					if (!this.CheckAndEnsureMoreAttributeDataLoaded(24))
					{
						this.ReadStateValue = ((this.attributeTag != TnefAttributeTag.RecipientTable) ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow);
						return false;
					}
					Guid propertySetGuid = this.ReadGuid();
					int num = this.ReadDword();
					if (num == 1)
					{
						int num2 = this.ReadDword();
						int num3 = (4 - num2 % 4) % 4;
						if (num2 <= 0 || num2 > 10240)
						{
							this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeValue, TnefStrings.ReaderComplianceInvalidNamedPropertyNameLength);
							this.error = true;
							this.ReadStateValue = ((this.attributeTag != TnefAttributeTag.RecipientTable) ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow);
							return false;
						}
						if (!this.CheckAndEnsureMoreAttributeDataLoaded(num2 + num3))
						{
							this.error = true;
							this.ReadStateValue = ((this.attributeTag != TnefAttributeTag.RecipientTable) ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow);
							return false;
						}
						if (this.PeekWord(num2 + num3 - 2) != 0)
						{
							this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeValue, TnefStrings.ReaderComplianceInvalidNamedPropertyNameNotZeroTerminated);
							this.error = true;
							this.ReadStateValue = ((this.attributeTag != TnefAttributeTag.RecipientTable) ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow);
							return false;
						}
						string name = this.ReadAttributeUnicodeString(num2);
						if (num3 != 0)
						{
							this.SkipBytes(num3);
						}
						this.propertyNameId.Set(propertySetGuid, name);
					}
					else
					{
						this.propertyNameId.Set(propertySetGuid, this.ReadDword());
					}
				}
				this.propertyValueIndex = -1;
				if (this.propertyTag.IsMultiValued)
				{
					if (!this.CheckAndEnsureMoreAttributeDataLoaded(4))
					{
						this.ReadStateValue = ((this.attributeTag != TnefAttributeTag.RecipientTable) ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow);
						return false;
					}
					this.propertyValueCount = this.ReadDword();
					if (this.propertyValueCount < 0)
					{
						this.SetComplianceStatus(TnefComplianceStatus.InvalidPropertyLength, TnefStrings.ReaderComplianceInvalidPropertyValueCount);
						this.error = true;
						this.ReadStateValue = ((this.attributeTag != TnefAttributeTag.RecipientTable) ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow);
						return false;
					}
				}
				else if (this.propertyTag.ValueTnefType == TnefPropertyType.Null)
				{
					this.propertyValueCount = 0;
				}
				else
				{
					this.propertyValueCount = 1;
					if (this.propertyTag.ValueTnefType == TnefPropertyType.Binary || this.propertyTag.ValueTnefType == TnefPropertyType.String8 || this.propertyTag.ValueTnefType == TnefPropertyType.Unicode || this.propertyTag.ValueTnefType == TnefPropertyType.Object)
					{
						if (!this.CheckAndEnsureMoreAttributeDataLoaded(4))
						{
							this.ReadStateValue = ((this.attributeTag != TnefAttributeTag.RecipientTable) ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow);
							return false;
						}
						int num4 = this.ReadDword();
						if (num4 != 1)
						{
							this.SetComplianceStatus(TnefComplianceStatus.InvalidPropertyLength, TnefStrings.ReaderComplianceInvalidPropertyValueCount);
							this.error = true;
							this.ReadStateValue = ((this.attributeTag != TnefAttributeTag.RecipientTable) ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow);
							return false;
						}
					}
					else if (this.propertyValueFixedLength != 0 && !this.CheckAndEnsureMoreAttributeDataLoaded(Math.Max(this.propertyValueFixedLength, 4)))
					{
						this.ReadStateValue = ((this.attributeTag != TnefAttributeTag.RecipientTable) ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow);
						return false;
					}
				}
				this.propertyPaddingLength = (4 - this.propertyValueCount * this.propertyValueFixedLength % 4) % 4;
				if (this.propertyValueFixedLength != 0 && this.propertyValueCount * this.propertyValueFixedLength + this.propertyPaddingLength > this.AttributeRemainingCount())
				{
					this.SetComplianceStatus(TnefComplianceStatus.AttributeOverflow, TnefStrings.ReaderComplianceAttributeValueOverflow);
					this.error = true;
					this.ReadStateValue = ((this.attributeTag != TnefAttributeTag.RecipientTable) ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow);
					return false;
				}
			}
			this.ReadStateValue = TnefReader.ReadState.BeginProperty;
			return true;
		}

		internal bool ReadNextPropertyValue()
		{
			this.AssertGoodToUse(true);
			this.AssertInProperty();
			if (this.ReadStateValue != TnefReader.ReadState.EndPropertyValue)
			{
				if (this.ReadStateValue == TnefReader.ReadState.EndProperty)
				{
					return false;
				}
				if (this.ReadStateValue == TnefReader.ReadState.BeginProperty)
				{
					this.propertyValueIndex = -1;
				}
				else
				{
					if (this.ReadStateValue == TnefReader.ReadState.BeginPropertyValue && !this.propertyTag.IsMultiValued)
					{
						return true;
					}
					if (!this.legacyAttribute)
					{
						if (this.propertyValueOffset != this.propertyValueLength)
						{
							this.EatPropertyBytes(this.PropertyRemainingCount());
						}
						if (this.propertyValuePaddingLength != 0)
						{
							this.EatAttributeBytes(this.propertyValuePaddingLength);
							this.propertyValuePaddingLength = 0;
						}
						if (this.error)
						{
							this.ReadStateValue = TnefReader.ReadState.EndProperty;
							return false;
						}
					}
				}
			}
			if (++this.propertyValueIndex == this.propertyValueCount)
			{
				this.ReadStateValue = TnefReader.ReadState.EndProperty;
				return false;
			}
			this.ReadStateValue = TnefReader.ReadState.BeginPropertyValue;
			this.propertyValueOffset = 0;
			this.propertyValueLength = 0;
			this.propertyValuePaddingLength = 0;
			this.propertyValueStreamOffset = this.StreamOffset;
			if (this.legacyAttribute)
			{
				if (!this.PrepareLegacyPropertyValue())
				{
					this.ReadStateValue = (this.propertyTag.IsMultiValued ? TnefReader.ReadState.EndProperty : TnefReader.ReadState.BeginPropertyValue);
					return false;
				}
			}
			else
			{
				this.directRead = true;
				if (!this.CheckAndEnsureMoreAttributeDataLoaded(Math.Max(this.propertyValueFixedLength + this.propertyPaddingLength, 4)))
				{
					this.ReadStateValue = (this.propertyTag.IsMultiValued ? TnefReader.ReadState.EndProperty : TnefReader.ReadState.BeginPropertyValue);
					return false;
				}
				this.propertyValueLength = this.GetPropertyValueLength();
				this.propertyValueStreamOffset = this.StreamOffset;
				if (this.error)
				{
					this.ReadStateValue = (this.propertyTag.IsMultiValued ? TnefReader.ReadState.EndProperty : TnefReader.ReadState.BeginPropertyValue);
					return false;
				}
			}
			return true;
		}

		internal bool IsLargePropertyValue
		{
			get
			{
				this.AssertGoodToUse(false);
				this.AssertInProperty();
				if (this.propertyTag.TnefType == TnefPropertyType.Null)
				{
					return false;
				}
				this.AssertInPropertyValue();
				return this.propertyValueLength > 32768 && this.propertyValueFixedLength == 0;
			}
		}

		internal Type PropertyValueClrType
		{
			get
			{
				this.AssertGoodToUse(false);
				this.AssertInProperty();
				TnefPropertyType valueTnefType = this.propertyTag.ValueTnefType;
				if (valueTnefType <= TnefPropertyType.Unicode)
				{
					switch (valueTnefType)
					{
					case TnefPropertyType.I2:
						return typeof(short);
					case TnefPropertyType.Long:
						return typeof(int);
					case TnefPropertyType.R4:
						return typeof(float);
					case TnefPropertyType.Double:
						return typeof(double);
					case TnefPropertyType.Currency:
						return typeof(long);
					case TnefPropertyType.AppTime:
						return typeof(DateTime);
					case (TnefPropertyType)8:
					case (TnefPropertyType)9:
					case (TnefPropertyType)12:
					case (TnefPropertyType)14:
					case (TnefPropertyType)15:
					case (TnefPropertyType)16:
					case (TnefPropertyType)17:
					case (TnefPropertyType)18:
					case (TnefPropertyType)19:
						break;
					case TnefPropertyType.Error:
						return typeof(int);
					case TnefPropertyType.Boolean:
						return typeof(bool);
					case TnefPropertyType.Object:
						return typeof(byte[]);
					case TnefPropertyType.I8:
						return typeof(long);
					default:
						switch (valueTnefType)
						{
						case TnefPropertyType.String8:
							return typeof(string);
						case TnefPropertyType.Unicode:
							return typeof(string);
						}
						break;
					}
				}
				else
				{
					if (valueTnefType == TnefPropertyType.SysTime)
					{
						return typeof(DateTime);
					}
					if (valueTnefType == TnefPropertyType.ClassId)
					{
						return typeof(Guid);
					}
					if (valueTnefType == TnefPropertyType.Binary)
					{
						return typeof(byte[]);
					}
				}
				return null;
			}
		}

		internal object ReadPropertyValue()
		{
			this.AssertGoodToUse(true);
			this.AssertInProperty();
			TnefPropertyType valueTnefType = this.propertyTag.ValueTnefType;
			if (valueTnefType <= TnefPropertyType.Unicode)
			{
				switch (valueTnefType)
				{
				case TnefPropertyType.I2:
					return this.ReadPropertyValueAsShort();
				case TnefPropertyType.Long:
					return this.ReadPropertyValueAsInt();
				case TnefPropertyType.R4:
					return this.ReadPropertyValueAsFloat();
				case TnefPropertyType.Double:
					return this.ReadPropertyValueAsDouble();
				case TnefPropertyType.Currency:
					return this.ReadPropertyValueAsLong();
				case TnefPropertyType.AppTime:
					return this.ReadPropertyValueAsDateTime();
				case (TnefPropertyType)8:
				case (TnefPropertyType)9:
				case (TnefPropertyType)12:
				case (TnefPropertyType)14:
				case (TnefPropertyType)15:
				case (TnefPropertyType)16:
				case (TnefPropertyType)17:
				case (TnefPropertyType)18:
				case (TnefPropertyType)19:
					break;
				case TnefPropertyType.Error:
					return this.ReadPropertyValueAsInt();
				case TnefPropertyType.Boolean:
					return this.ReadPropertyValueAsBool();
				case TnefPropertyType.Object:
					return this.ReadPropertyValueAsByteArray();
				case TnefPropertyType.I8:
					return this.ReadPropertyValueAsLong();
				default:
					switch (valueTnefType)
					{
					case TnefPropertyType.String8:
						return this.ReadPropertyValueAsString();
					case TnefPropertyType.Unicode:
						return this.ReadPropertyValueAsString();
					}
					break;
				}
			}
			else
			{
				if (valueTnefType == TnefPropertyType.SysTime)
				{
					return this.ReadPropertyValueAsDateTime();
				}
				if (valueTnefType == TnefPropertyType.ClassId)
				{
					return this.ReadPropertyValueAsGuid();
				}
				if (valueTnefType == TnefPropertyType.Binary)
				{
					return this.ReadPropertyValueAsByteArray();
				}
			}
			return null;
		}

		internal bool ReadPropertyValueAsBool()
		{
			this.AssertAtTheBeginningOfPropertyValue();
			if (this.propertyTag.ValueTnefType != TnefPropertyType.Boolean)
			{
				throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationCannotConvertValue);
			}
			if (this.error || !this.EnsureMorePropertyDataLoaded(this.propertyValueFixedLength))
			{
				if (!this.error)
				{
					this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, TnefStrings.ReaderComplianceTnefTruncated);
				}
				this.ReadStateValue = TnefReader.ReadState.EndPropertyValue;
				this.error = true;
				return false;
			}
			this.ReadStateValue = TnefReader.ReadState.ReadPropertyValue;
			bool result = 0 != this.ReadPropertyWord();
			if (this.propertyValueOffset == this.propertyValueLength)
			{
				this.ProcessEndOfValue();
			}
			return result;
		}

		internal short ReadPropertyValueAsShort()
		{
			this.AssertAtTheBeginningOfPropertyValue();
			if (this.propertyTag.ValueTnefType != TnefPropertyType.I2 && this.propertyTag.ValueTnefType != TnefPropertyType.Boolean)
			{
				throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationCannotConvertValue);
			}
			if (this.error || !this.EnsureMorePropertyDataLoaded(this.propertyValueFixedLength))
			{
				if (!this.error)
				{
					this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, TnefStrings.ReaderComplianceTnefTruncated);
				}
				this.ReadStateValue = TnefReader.ReadState.EndPropertyValue;
				this.error = true;
				return 0;
			}
			this.ReadStateValue = TnefReader.ReadState.ReadPropertyValue;
			short result = this.ReadPropertyWord();
			if (this.propertyValueOffset == this.propertyValueLength)
			{
				this.ProcessEndOfValue();
			}
			return result;
		}

		internal int ReadPropertyValueAsInt()
		{
			this.AssertAtTheBeginningOfPropertyValue();
			if (this.propertyTag.ValueTnefType != TnefPropertyType.I2 && this.propertyTag.ValueTnefType != TnefPropertyType.Boolean && this.propertyTag.ValueTnefType != TnefPropertyType.Long && this.propertyTag.ValueTnefType != TnefPropertyType.Error)
			{
				throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationCannotConvertValue);
			}
			if (this.error || !this.EnsureMorePropertyDataLoaded(this.propertyValueFixedLength))
			{
				if (!this.error)
				{
					this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, TnefStrings.ReaderComplianceTnefTruncated);
				}
				this.ReadStateValue = TnefReader.ReadState.EndPropertyValue;
				this.error = true;
				return 0;
			}
			this.ReadStateValue = TnefReader.ReadState.ReadPropertyValue;
			int result;
			if (this.propertyTag.ValueTnefType == TnefPropertyType.I2 || this.propertyTag.ValueTnefType == TnefPropertyType.Boolean)
			{
				if (this.propertyTag.IsMultiValued || this.legacyAttribute)
				{
					result = (int)this.ReadPropertyWord();
				}
				else
				{
					this.propertyValueFixedLength = 4;
					this.propertyPaddingLength = 0;
					this.propertyValueLength = 4;
					result = this.ReadPropertyDword();
				}
			}
			else
			{
				result = this.ReadPropertyDword();
			}
			if (this.propertyValueOffset == this.propertyValueLength)
			{
				this.ProcessEndOfValue();
			}
			return result;
		}

		internal long ReadPropertyValueAsLong()
		{
			this.AssertAtTheBeginningOfPropertyValue();
			if (this.propertyTag.ValueTnefType != TnefPropertyType.I2 && this.propertyTag.ValueTnefType != TnefPropertyType.Boolean && this.propertyTag.ValueTnefType != TnefPropertyType.Long && this.propertyTag.ValueTnefType != TnefPropertyType.Error && this.propertyTag.ValueTnefType != TnefPropertyType.Currency && this.propertyTag.ValueTnefType != TnefPropertyType.I8 && this.propertyTag.ValueTnefType != TnefPropertyType.AppTime && this.propertyTag.ValueTnefType != TnefPropertyType.SysTime)
			{
				throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationCannotConvertValue);
			}
			if (this.error || !this.EnsureMorePropertyDataLoaded(this.propertyValueFixedLength))
			{
				if (!this.error)
				{
					this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, TnefStrings.ReaderComplianceTnefTruncated);
				}
				this.ReadStateValue = TnefReader.ReadState.EndPropertyValue;
				this.error = true;
				return 0L;
			}
			this.ReadStateValue = TnefReader.ReadState.ReadPropertyValue;
			long result;
			if (this.propertyTag.ValueTnefType == TnefPropertyType.I2 || this.propertyTag.ValueTnefType == TnefPropertyType.Boolean)
			{
				result = (long)this.ReadPropertyWord();
			}
			else if (this.propertyTag.ValueTnefType == TnefPropertyType.Long || this.propertyTag.ValueTnefType == TnefPropertyType.Error)
			{
				result = (long)this.ReadPropertyDword();
			}
			else
			{
				result = this.ReadPropertyQword();
			}
			if (this.propertyValueOffset == this.propertyValueLength)
			{
				this.ProcessEndOfValue();
			}
			return result;
		}

		internal Guid ReadPropertyValueAsGuid()
		{
			this.AssertAtTheBeginningOfPropertyValue();
			if (this.propertyTag.ValueTnefType != TnefPropertyType.ClassId)
			{
				throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationCannotConvertValue);
			}
			if (this.error || !this.EnsureMorePropertyDataLoaded(this.propertyValueFixedLength))
			{
				if (!this.error)
				{
					this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, TnefStrings.ReaderComplianceTnefTruncated);
				}
				this.ReadStateValue = TnefReader.ReadState.EndPropertyValue;
				this.error = true;
				return default(Guid);
			}
			this.ReadStateValue = TnefReader.ReadState.ReadPropertyValue;
			Guid result = this.ReadPropertyGuid();
			if (this.propertyValueOffset == this.propertyValueLength)
			{
				this.ProcessEndOfValue();
			}
			return result;
		}

		internal float ReadPropertyValueAsFloat()
		{
			this.AssertAtTheBeginningOfPropertyValue();
			if (this.propertyTag.ValueTnefType != TnefPropertyType.R4)
			{
				throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationCannotConvertValue);
			}
			if (this.error || !this.EnsureMorePropertyDataLoaded(this.propertyValueFixedLength))
			{
				if (!this.error)
				{
					this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, TnefStrings.ReaderComplianceTnefTruncated);
				}
				this.ReadStateValue = TnefReader.ReadState.EndPropertyValue;
				this.error = true;
				return 0f;
			}
			this.ReadStateValue = TnefReader.ReadState.ReadPropertyValue;
			float result = this.ReadPropertyFloat();
			if (this.propertyValueOffset == this.propertyValueLength)
			{
				this.ProcessEndOfValue();
			}
			return result;
		}

		internal double ReadPropertyValueAsDouble()
		{
			this.AssertAtTheBeginningOfPropertyValue();
			if (this.propertyTag.ValueTnefType != TnefPropertyType.R4 && this.propertyTag.ValueTnefType != TnefPropertyType.Double)
			{
				throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationCannotConvertValue);
			}
			if (this.error || !this.EnsureMorePropertyDataLoaded(this.propertyValueFixedLength))
			{
				if (!this.error)
				{
					this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, TnefStrings.ReaderComplianceTnefTruncated);
				}
				this.ReadStateValue = TnefReader.ReadState.EndPropertyValue;
				this.error = true;
				return 0.0;
			}
			this.ReadStateValue = TnefReader.ReadState.ReadPropertyValue;
			double result;
			if (this.propertyTag.ValueTnefType == TnefPropertyType.R4)
			{
				result = (double)this.ReadPropertyFloat();
			}
			else
			{
				result = this.ReadPropertyDouble();
			}
			if (this.propertyValueOffset == this.propertyValueLength)
			{
				this.ProcessEndOfValue();
			}
			return result;
		}

		internal DateTime ReadPropertyValueAsDateTime()
		{
			this.AssertAtTheBeginningOfPropertyValue();
			if (this.propertyTag.ValueTnefType != TnefPropertyType.AppTime && this.propertyTag.ValueTnefType != TnefPropertyType.SysTime)
			{
				throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationCannotConvertValue);
			}
			if (this.error || !this.EnsureMorePropertyDataLoaded(this.propertyValueFixedLength))
			{
				if (!this.error)
				{
					this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, TnefStrings.ReaderComplianceTnefTruncated);
				}
				this.ReadStateValue = TnefReader.ReadState.EndPropertyValue;
				this.error = true;
				return TnefReader.MinDateTime;
			}
			this.ReadStateValue = TnefReader.ReadState.ReadPropertyValue;
			DateTime result;
			if (this.propertyTag.ValueTnefType == TnefPropertyType.AppTime)
			{
				result = this.ReadPropertyAppTime();
			}
			else
			{
				result = this.ReadPropertySysTime();
			}
			if (this.propertyValueOffset == this.propertyValueLength)
			{
				this.ProcessEndOfValue();
			}
			return result;
		}

		internal string ReadPropertyValueAsString()
		{
			this.AssertAtTheBeginningOfPropertyValue();
			if (this.propertyTag.ValueTnefType != TnefPropertyType.String8 && this.propertyTag.ValueTnefType != TnefPropertyType.Unicode)
			{
				throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationCannotConvertValue);
			}
			if (this.IsLargePropertyValue)
			{
				throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationTextPropertyTooLong);
			}
			if (this.decodeBuffer == null)
			{
				this.decodeBuffer = new char[512];
			}
			int num = this.ReadPropertyTextValue(this.decodeBuffer, 0, this.decodeBuffer.Length);
			if (this.ReadStateValue != TnefReader.ReadState.EndPropertyValue)
			{
				StringBuilder stringBuilder = new StringBuilder();
				do
				{
					stringBuilder.Append(this.decodeBuffer, 0, num);
					num = this.ReadPropertyTextValue(this.decodeBuffer, 0, this.decodeBuffer.Length);
				}
				while (num != 0);
				return stringBuilder.ToString();
			}
			if (num == 0)
			{
				return string.Empty;
			}
			return new string(this.decodeBuffer, 0, num);
		}

		internal byte[] ReadPropertyValueAsByteArray()
		{
			this.AssertAtTheBeginningOfPropertyValue();
			if (this.IsLargePropertyValue)
			{
				throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationPropertyRawValueTooLong);
			}
			byte[] array = new byte[this.PropertyRawValueLength];
			this.ReadPropertyRawValue(array, 0, array.Length, false);
			return array;
		}

		internal int ReadPropertyTextValue(char[] buffer, int offset, int count)
		{
			this.AssertGoodToUse(true);
			if (this.ReadStateValue < TnefReader.ReadState.BeginPropertyValue)
			{
				if (this.ReadStateValue == TnefReader.ReadState.EndPropertyValue)
				{
					return 0;
				}
				if (this.ReadStateValue != TnefReader.ReadState.BeginProperty || this.propertyTag.IsMultiValued || this.propertyTag.ValueTnefType == TnefPropertyType.Null)
				{
					throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationMustBeInProperty);
				}
				this.ReadNextPropertyValue();
			}
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
			if (this.propertyTag.ValueTnefType != TnefPropertyType.String8 && this.propertyTag.ValueTnefType != TnefPropertyType.Unicode)
			{
				throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationCannotConvertValue);
			}
			if (this.ReadStateValue == TnefReader.ReadState.BeginPropertyValue)
			{
				if (this.propertyTag.ValueTnefType == TnefPropertyType.String8)
				{
					if (this.string8Decoder == null)
					{
						Encoding encoding = null;
						if (this.messageCodepage == 0)
						{
							encoding = Charset.DefaultWindowsCharset.GetEncoding();
							this.messageCodepage = CodePageMap.GetCodePage(encoding);
						}
						else if (!Charset.TryGetEncoding(this.messageCodepage, out encoding))
						{
							this.SetComplianceStatus(TnefComplianceStatus.InvalidMessageCodepage, TnefStrings.ReaderComplianceInvalidMessageCodepage);
							encoding = Charset.DefaultWindowsCharset.GetEncoding();
						}
						int codePage = CodePageMap.GetCodePage(encoding);
						if (TnefCommon.IsUnicodeCodepage(codePage))
						{
							this.messageCodepage = 1252;
							encoding = Charset.GetEncoding(this.messageCodepage);
						}
						this.string8Decoder = encoding.GetDecoder();
					}
					this.decoder = this.string8Decoder;
				}
				else
				{
					this.decoder = this.unicodeDecoder;
				}
				this.decoder.Reset();
				this.decoderFlushed = false;
				this.ReadStateValue = TnefReader.ReadState.ReadPropertyValue;
			}
			else if (this.decoder == null)
			{
				throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationPropTextAfterRaw);
			}
			int num = 0;
			while ((this.PropertyRemainingCount() != 0 || !this.decoderFlushed) && count > 12)
			{
				if (this.MorePropertyData(1) && !this.EnsureMorePropertyDataLoaded(1))
				{
					this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, TnefStrings.ReaderComplianceTnefTruncated);
					this.error = true;
					break;
				}
				int num2 = this.PropertyAvailableCount();
				int count2;
				int num3;
				if (this.directRead)
				{
					this.decoder.Convert(this.readBuffer, this.readOffset, num2, buffer, offset, count, num2 == this.PropertyRemainingCount(), out count2, out num3, out this.decoderFlushed);
				}
				else
				{
					this.decoder.Convert(this.fabricatedBuffer, this.fabricatedOffset, num2, buffer, offset, count, num2 == this.PropertyRemainingCount(), out count2, out num3, out this.decoderFlushed);
				}
				this.SkipPropertyBytes(count2);
				offset += num3;
				count -= num3;
				num += num3;
				for (int i = offset - num3; i < offset; i++)
				{
					if (buffer[i] == '\0')
					{
						num -= offset - i;
						this.EatPropertyBytes(this.PropertyRemainingCount());
						this.decoderFlushed = true;
						break;
					}
				}
			}
			if (this.error)
			{
				this.ReadStateValue = TnefReader.ReadState.EndPropertyValue;
			}
			else if (this.propertyValueOffset == this.propertyValueLength && this.decoderFlushed)
			{
				this.ProcessEndOfValue();
			}
			return num;
		}

		internal int ReadPropertyRawValue(byte[] buffer, int offset, int count, bool fromWrapper)
		{
			this.AssertGoodToUse(!fromWrapper);
			if (this.ReadStateValue < TnefReader.ReadState.BeginPropertyValue)
			{
				if (this.ReadStateValue == TnefReader.ReadState.EndPropertyValue)
				{
					return 0;
				}
				if (this.ReadStateValue != TnefReader.ReadState.BeginProperty || this.propertyTag.IsMultiValued || this.propertyTag.ValueTnefType == TnefPropertyType.Null)
				{
					throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationMustBeInProperty);
				}
				this.ReadNextPropertyValue();
			}
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
			if (this.ReadStateValue == TnefReader.ReadState.BeginPropertyValue)
			{
				this.decoder = null;
				this.ReadStateValue = TnefReader.ReadState.ReadPropertyValue;
			}
			else if (this.decoder != null)
			{
				throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationPropRawAfterText);
			}
			int num = 0;
			while (this.MorePropertyData(1) && count != 0)
			{
				if (!this.EnsureMorePropertyDataLoaded(1))
				{
					this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, TnefStrings.ReaderComplianceTnefTruncated);
					this.error = true;
					break;
				}
				int num2 = Math.Min(count, this.PropertyAvailableCount());
				this.ReadPropertyBytes(buffer, offset, num2);
				offset += num2;
				count -= num2;
				num += num2;
			}
			if (this.error)
			{
				this.ReadStateValue = TnefReader.ReadState.EndPropertyValue;
			}
			else if (this.propertyValueOffset == this.propertyValueLength)
			{
				this.ProcessEndOfValue();
			}
			return num;
		}

		internal TnefReader GetEmbeddedMessageReader()
		{
			this.AssertGoodToUse(true);
			this.AssertAtTheBeginningOfPropertyValue();
			if (!this.IsPropertyEmbeddedMessage)
			{
				throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationNotEmbeddedMessage);
			}
			if (this.embeddingDepth + 1 == 100)
			{
				this.SetComplianceStatus(TnefComplianceStatus.NestingTooDeep, TnefStrings.ReaderComplianceTooDeepEmbedding);
			}
			return new TnefReader(this);
		}

		internal Stream GetRawPropertyValueReadStream()
		{
			this.AssertGoodToUse(true);
			this.AssertAtTheBeginningOfPropertyValue();
			return new TnefReaderStreamWrapper(this);
		}

		private void ProcessEndOfValue()
		{
			if (this.propertyValuePaddingLength != 0)
			{
				if (!this.EnsureMoreDataLoaded(this.propertyValuePaddingLength))
				{
					this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, TnefStrings.ReaderComplianceTnefTruncated);
					this.error = true;
					this.ReadStateValue = TnefReader.ReadState.EndPropertyValue;
					return;
				}
				this.SkipBytes(this.propertyValuePaddingLength);
				this.propertyValuePaddingLength = 0;
			}
			if (!this.propertyTag.IsMultiValued && this.propertyPaddingLength != 0)
			{
				if (!this.EnsureMoreDataLoaded(this.propertyPaddingLength))
				{
					this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, TnefStrings.ReaderComplianceTnefTruncated);
					this.error = true;
					this.ReadStateValue = TnefReader.ReadState.EndPropertyValue;
					return;
				}
				this.SkipBytes(this.propertyPaddingLength);
				this.propertyPaddingLength = 0;
			}
			this.ReadStateValue = TnefReader.ReadState.EndPropertyValue;
		}

		private bool CheckPropertyType()
		{
			TnefPropertyType valueTnefType = this.propertyTag.ValueTnefType;
			if (valueTnefType <= TnefPropertyType.Unicode)
			{
				switch (valueTnefType)
				{
				case TnefPropertyType.Null:
					this.SetComplianceStatus(TnefComplianceStatus.UnsupportedPropertyType, TnefStrings.ReaderComplianceInvalidPropertyTypeError);
					if (!this.propertyTag.IsMultiValued)
					{
						this.propertyValueFixedLength = 0;
						return true;
					}
					goto IL_186;
				case TnefPropertyType.I2:
					this.propertyValueFixedLength = 2;
					return true;
				case TnefPropertyType.Long:
				case TnefPropertyType.R4:
					this.propertyValueFixedLength = 4;
					return true;
				case TnefPropertyType.Double:
				case TnefPropertyType.Currency:
				case TnefPropertyType.AppTime:
				case TnefPropertyType.I8:
					break;
				case (TnefPropertyType)8:
				case (TnefPropertyType)9:
				case (TnefPropertyType)12:
				case (TnefPropertyType)14:
				case (TnefPropertyType)15:
				case (TnefPropertyType)16:
				case (TnefPropertyType)17:
				case (TnefPropertyType)18:
				case (TnefPropertyType)19:
					goto IL_186;
				case TnefPropertyType.Error:
					this.propertyValueFixedLength = 4;
					this.SetComplianceStatus(TnefComplianceStatus.UnsupportedPropertyType, TnefStrings.ReaderComplianceInvalidPropertyTypeError);
					return true;
				case TnefPropertyType.Boolean:
					this.propertyValueFixedLength = 2;
					if (this.propertyTag.IsMultiValued)
					{
						this.SetComplianceStatus(TnefComplianceStatus.UnsupportedPropertyType, TnefStrings.ReaderComplianceInvalidPropertyTypeMvBoolean);
						return true;
					}
					return true;
				case TnefPropertyType.Object:
					this.propertyValueFixedLength = 0;
					if (this.propertyTag.IsMultiValued)
					{
						this.SetComplianceStatus(TnefComplianceStatus.UnsupportedPropertyType, TnefStrings.ReaderComplianceInvalidPropertyTypeMvObject);
						goto IL_186;
					}
					if (this.attributeTag == TnefAttributeTag.RecipientTable)
					{
						this.SetComplianceStatus(TnefComplianceStatus.UnsupportedPropertyType, TnefStrings.ReaderComplianceInvalidPropertyTypeObjectInRecipientTable);
						return true;
					}
					return true;
				default:
					switch (valueTnefType)
					{
					case TnefPropertyType.String8:
					case TnefPropertyType.Unicode:
						goto IL_17D;
					default:
						goto IL_186;
					}
					break;
				}
			}
			else if (valueTnefType != TnefPropertyType.SysTime)
			{
				if (valueTnefType == TnefPropertyType.ClassId)
				{
					this.propertyValueFixedLength = 16;
					return true;
				}
				if (valueTnefType != TnefPropertyType.Binary)
				{
					goto IL_186;
				}
				goto IL_17D;
			}
			this.propertyValueFixedLength = 8;
			return true;
			IL_17D:
			this.propertyValueFixedLength = 0;
			return true;
			IL_186:
			this.propertyValueFixedLength = 0;
			this.SetComplianceStatus(TnefComplianceStatus.UnsupportedPropertyType, TnefStrings.ReaderComplianceInvalidPropertyType);
			this.error = true;
			return false;
		}

		private int GetPropertyValueLength()
		{
			TnefPropertyType valueTnefType = this.propertyTag.ValueTnefType;
			if (valueTnefType != TnefPropertyType.Object)
			{
				switch (valueTnefType)
				{
				case TnefPropertyType.String8:
				case TnefPropertyType.Unicode:
					break;
				default:
					if (valueTnefType != TnefPropertyType.Binary)
					{
						this.propertyValuePaddingLength = 0;
						return this.propertyValueFixedLength;
					}
					break;
				}
				int num = this.ReadDword();
				this.propertyValuePaddingLength = (4 - num % 4) % 4;
				if (num < 0 || (long)this.attributeValueOffset + (long)num + (long)this.propertyValuePaddingLength > (long)this.attributeValueLength)
				{
					this.SetComplianceStatus(TnefComplianceStatus.InvalidPropertyLength, TnefStrings.ReaderComplianceInvalidPropertyLength);
					this.error = true;
					return 0;
				}
				return num;
			}
			else
			{
				int num = this.ReadDword();
				this.propertyValuePaddingLength = (4 - num % 4) % 4;
				if (num < 16 || (long)this.attributeValueOffset + (long)num + (long)this.propertyValuePaddingLength > (long)this.attributeValueLength)
				{
					if (num < 0 || (long)this.attributeValueOffset + (long)num + (long)this.propertyValuePaddingLength > (long)this.attributeValueLength)
					{
						this.SetComplianceStatus(TnefComplianceStatus.InvalidPropertyLength, TnefStrings.ReaderComplianceInvalidPropertyLength);
						this.error = true;
						return 0;
					}
					this.SetComplianceStatus(TnefComplianceStatus.InvalidPropertyLength, TnefStrings.ReaderComplianceInvalidPropertyLengthObject);
					return num;
				}
				else
				{
					if (!this.CheckAndEnsureMoreAttributeDataLoaded(16))
					{
						this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, TnefStrings.ReaderComplianceTnefTruncated);
						return num;
					}
					this.propertyValueIId = this.ReadGuid();
					return num - 16;
				}
			}
		}

		private short ReadPropertyWord()
		{
			this.propertyValueOffset += 2;
			if (!this.directRead)
			{
				short result = BitConverter.ToInt16(this.fabricatedBuffer, this.fabricatedOffset);
				this.fabricatedOffset += 2;
				return result;
			}
			return this.ReadWord();
		}

		private int ReadPropertyDword()
		{
			this.propertyValueOffset += 4;
			if (!this.directRead)
			{
				int result = BitConverter.ToInt32(this.fabricatedBuffer, this.fabricatedOffset);
				this.fabricatedOffset += 4;
				return result;
			}
			return this.ReadDword();
		}

		private long ReadPropertyQword()
		{
			this.propertyValueOffset += 8;
			if (!this.directRead)
			{
				long result = BitConverter.ToInt64(this.fabricatedBuffer, this.fabricatedOffset);
				this.fabricatedOffset += 8;
				return result;
			}
			return this.ReadQword();
		}

		private Guid ReadPropertyGuid()
		{
			this.propertyValueOffset += 16;
			if (!this.directRead)
			{
				Guid result = new Guid(BitConverter.ToInt32(this.fabricatedBuffer, this.fabricatedOffset), BitConverter.ToInt16(this.fabricatedBuffer, this.fabricatedOffset + 4), BitConverter.ToInt16(this.fabricatedBuffer, this.fabricatedOffset + 6), this.fabricatedBuffer[this.fabricatedOffset + 8], this.fabricatedBuffer[this.fabricatedOffset + 9], this.fabricatedBuffer[this.fabricatedOffset + 10], this.fabricatedBuffer[this.fabricatedOffset + 11], this.fabricatedBuffer[this.fabricatedOffset + 12], this.fabricatedBuffer[this.fabricatedOffset + 13], this.fabricatedBuffer[this.fabricatedOffset + 14], this.fabricatedBuffer[this.fabricatedOffset + 15]);
				this.fabricatedOffset += 16;
				return result;
			}
			return this.ReadGuid();
		}

		private float ReadPropertyFloat()
		{
			this.propertyValueOffset += 4;
			if (!this.directRead)
			{
				float result = BitConverter.ToSingle(this.fabricatedBuffer, this.fabricatedOffset);
				this.fabricatedOffset += 4;
				return result;
			}
			return this.ReadFloat();
		}

		private double ReadPropertyDouble()
		{
			this.propertyValueOffset += 8;
			if (!this.directRead)
			{
				double result = BitConverter.ToDouble(this.fabricatedBuffer, this.fabricatedOffset);
				this.fabricatedOffset += 8;
				return result;
			}
			return this.ReadDouble();
		}

		private DateTime ReadPropertyAppTime()
		{
			double num = this.ReadPropertyDouble();
			DateTime result;
			try
			{
				if (num != 0.0)
				{
					result = DateTime.SpecifyKind(TnefReader.FromOADate(num), DateTimeKind.Utc);
				}
				else
				{
					result = TnefReader.MinDateTime;
				}
			}
			catch (ArgumentException)
			{
				this.SetComplianceStatus(TnefComplianceStatus.InvalidDate, TnefStrings.ReaderComplianceInvalidPropertyValueDate);
				result = DateTime.UtcNow;
			}
			return result;
		}

		private DateTime ReadPropertySysTime()
		{
			long num = this.ReadPropertyQword();
			DateTime result;
			try
			{
				if (num != 0L)
				{
					result = DateTime.FromFileTimeUtc(num);
				}
				else
				{
					result = TnefReader.MinDateTime;
				}
			}
			catch (ArgumentOutOfRangeException)
			{
				this.SetComplianceStatus(TnefComplianceStatus.InvalidDate, TnefStrings.ReaderComplianceInvalidPropertyValueDate);
				result = DateTime.UtcNow;
			}
			return result;
		}

		private void ReadPropertyBytes(byte[] buffer, int offset, int count)
		{
			this.propertyValueOffset += count;
			if (!this.directRead)
			{
				Buffer.BlockCopy(this.fabricatedBuffer, this.fabricatedOffset, buffer, offset, count);
				this.fabricatedOffset += count;
				return;
			}
			this.ReadBytes(buffer, offset, count);
		}

		private void SkipPropertyBytes(int count)
		{
			this.propertyValueOffset += count;
			if (!this.directRead)
			{
				this.fabricatedOffset += count;
				return;
			}
			this.SkipBytes(count);
		}

		private void EatPropertyBytes(int count)
		{
			while (this.MorePropertyData(1) && count != 0)
			{
				if (!this.EnsureMorePropertyDataLoaded(1))
				{
					this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, TnefStrings.ReaderComplianceTnefTruncated);
					this.error = true;
					return;
				}
				int num = Math.Min(count, this.PropertyAvailableCount());
				this.SkipPropertyBytes(num);
				count -= num;
			}
		}

		private int PropertyRemainingCount()
		{
			return this.propertyValueLength - this.propertyValueOffset;
		}

		private int PropertyAvailableCount()
		{
			if (!this.directRead)
			{
				return this.fabricatedEnd - this.fabricatedOffset;
			}
			return Math.Min(this.AvailableCount(), this.PropertyRemainingCount());
		}

		private bool MorePropertyData(int count)
		{
			return this.propertyValueOffset + count <= this.propertyValueLength;
		}

		private bool EnsureMorePropertyDataLoaded(int count)
		{
			if (!this.directRead)
			{
				return this.EnsureMoreFabricatedDataAvailable(count);
			}
			return this.EnsureMoreDataLoaded(count);
		}

		private bool EnsureMoreFabricatedDataAvailable(int count)
		{
			return this.fabricatedOffset + count <= this.fabricatedEnd || this.FabricateMorePropertyData(count);
		}

		private bool PreviewAttributeContent()
		{
			this.rowCount = -1;
			this.propertyCount = 0;
			TnefAttributeTag tnefAttributeTag = this.attributeTag;
			if (tnefAttributeTag <= TnefAttributeTag.DateModified)
			{
				if (tnefAttributeTag <= TnefAttributeTag.AttachTitle)
				{
					if (tnefAttributeTag <= TnefAttributeTag.From)
					{
						if (tnefAttributeTag == TnefAttributeTag.Null)
						{
							return true;
						}
						if (tnefAttributeTag != TnefAttributeTag.From)
						{
							return true;
						}
						this.propertyCount = 2;
						return true;
					}
					else if (tnefAttributeTag != TnefAttributeTag.Subject)
					{
						switch (tnefAttributeTag)
						{
						case TnefAttributeTag.MessageId:
						case TnefAttributeTag.ParentId:
						case TnefAttributeTag.ConversationId:
							goto IL_2CF;
						default:
							if (tnefAttributeTag != TnefAttributeTag.AttachTitle)
							{
								return true;
							}
							goto IL_324;
						}
					}
				}
				else if (tnefAttributeTag <= TnefAttributeTag.DateEnd)
				{
					if (tnefAttributeTag == TnefAttributeTag.Body)
					{
						goto IL_2CF;
					}
					switch (tnefAttributeTag)
					{
					case TnefAttributeTag.DateStart:
					case TnefAttributeTag.DateEnd:
						goto IL_381;
					default:
						return true;
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
							goto IL_324;
						default:
							if (tnefAttributeTag != TnefAttributeTag.DateModified)
							{
								return true;
							}
							break;
						}
						break;
					}
				}
				this.propertyCount = 1;
				return true;
			}
			if (tnefAttributeTag <= TnefAttributeTag.MessageStatus)
			{
				if (tnefAttributeTag <= TnefAttributeTag.Priority)
				{
					if (tnefAttributeTag == TnefAttributeTag.RequestResponse)
					{
						goto IL_381;
					}
					if (tnefAttributeTag != TnefAttributeTag.Priority)
					{
						return true;
					}
				}
				else
				{
					if (tnefAttributeTag == TnefAttributeTag.AidOwner)
					{
						goto IL_381;
					}
					switch (tnefAttributeTag)
					{
					case TnefAttributeTag.Owner:
					case TnefAttributeTag.SentFor:
						if (this.messageClassSPlus)
						{
							this.propertyCount = 2;
							return true;
						}
						this.propertyCount = 0;
						return true;
					case TnefAttributeTag.Delegate:
						goto IL_381;
					default:
						if (tnefAttributeTag != TnefAttributeTag.MessageStatus)
						{
							return true;
						}
						break;
					}
				}
			}
			else if (tnefAttributeTag <= TnefAttributeTag.OemCodepage)
			{
				switch (tnefAttributeTag)
				{
				case TnefAttributeTag.AttachData:
					this.propertyCount = 1;
					if (this.currentAttachmentIsOle)
					{
						this.propertyCount++;
						return true;
					}
					return true;
				case (TnefAttributeTag)426000:
					return true;
				case TnefAttributeTag.AttachMetaFile:
					goto IL_324;
				default:
					switch (tnefAttributeTag)
					{
					case TnefAttributeTag.AttachTransportFilename:
						goto IL_324;
					case TnefAttributeTag.AttachRenderData:
						if (this.CheckAndEnsureMoreAttributeDataLoaded(14))
						{
							short num = this.PeekWord(0);
							int num2 = this.PeekDword(10);
							this.propertyCount = 2;
							if (num2 == 1)
							{
								this.propertyCount++;
							}
							this.currentAttachmentIsOle = (num != 1);
							return true;
						}
						return true;
					case TnefAttributeTag.MapiProperties:
					case TnefAttributeTag.Attachment:
						if (!this.CheckAndEnsureMoreAttributeDataLoaded(4))
						{
							return false;
						}
						this.propertyCount = this.PeekDword(0);
						if (this.propertyCount < 0)
						{
							this.SetComplianceStatus(TnefComplianceStatus.InvalidRowCount, TnefStrings.ReaderComplianceInvalidPropertyCount);
							this.error = true;
							return false;
						}
						return true;
					case TnefAttributeTag.RecipientTable:
						this.propertyCount = -1;
						if (!this.CheckAndEnsureMoreAttributeDataLoaded(4))
						{
							return false;
						}
						this.rowCount = this.PeekDword(0);
						if (this.rowCount < 0)
						{
							this.SetComplianceStatus(TnefComplianceStatus.InvalidRowCount, TnefStrings.ReaderComplianceInvalidRowCount);
							this.error = true;
							this.rowCount = 0;
							return false;
						}
						return true;
					case (TnefAttributeTag)430086:
						return true;
					case TnefAttributeTag.OemCodepage:
					{
						if (!this.CheckAndEnsureMoreAttributeDataLoaded(8))
						{
							this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeLength, TnefStrings.ReaderComplianceInvalidOemCodepageAttributeLength);
							return true;
						}
						int messageCodePage = this.PeekDword(0);
						if (this.messageCodepage == 0 && !TnefCommon.IsUnicodeCodepage(messageCodePage))
						{
							this.messageCodepage = messageCodePage;
							this.string8Decoder = null;
							return true;
						}
						return true;
					}
					default:
						return true;
					}
					break;
				}
			}
			else if (tnefAttributeTag != TnefAttributeTag.OriginalMessageClass && tnefAttributeTag != TnefAttributeTag.MessageClass)
			{
				if (tnefAttributeTag != TnefAttributeTag.TnefVersion)
				{
					return true;
				}
				if (!this.CheckAndEnsureMoreAttributeDataLoaded(4))
				{
					this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeLength, TnefStrings.ReaderComplianceInvalidTnefVersionAttributeLength);
					return true;
				}
				this.tnefVersion = this.PeekDword(0);
				if (this.tnefVersion > 65536)
				{
					this.SetComplianceStatus(TnefComplianceStatus.InvalidTnefVersion, TnefStrings.ReaderComplianceInvalidTnefVersion);
					return true;
				}
				return true;
			}
			else
			{
				if (this.attributeValueLength == 0 || this.attributeValueLength > 255)
				{
					this.SetComplianceStatus(TnefComplianceStatus.InvalidMessageClass, TnefStrings.ReaderComplianceInvalidMessageClassLength);
					this.directReadForMessageClass = true;
					return true;
				}
				if (!this.CheckAndEnsureMoreAttributeDataLoaded(this.attributeValueLength))
				{
					return false;
				}
				if (this.PeekByte(this.attributeValueLength - 1) != 0)
				{
					this.SetComplianceStatus(TnefComplianceStatus.InvalidMessageClass, TnefStrings.ReaderComplianceInvalidMessageClassNotZeroTerminated);
				}
				this.propertyCount = 1;
				this.FabricateMessageClass(this.attributeTag == TnefAttributeTag.MessageClass);
				return true;
			}
			IL_2CF:
			this.propertyCount = 1;
			return true;
			IL_324:
			this.propertyCount = 1;
			return true;
			IL_381:
			this.propertyCount = 1;
			return true;
		}

		private bool PrepareFirstProperty()
		{
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
								if (this.attributeValueLength <= 8 || this.attributeValueLength > 32768)
								{
									this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeValue, TnefStrings.ReaderComplianceInvalidFrom);
									return false;
								}
								if (!this.CheckAndEnsureMoreAttributeDataLoaded(this.attributeValueLength))
								{
									return false;
								}
								if (!this.CrackTriple())
								{
									return false;
								}
							}
						}
					}
					else if (tnefAttributeTag != TnefAttributeTag.Subject)
					{
						switch (tnefAttributeTag)
						{
						case TnefAttributeTag.MessageId:
						case TnefAttributeTag.ParentId:
						case TnefAttributeTag.ConversationId:
							if (this.attributeValueLength == 0)
							{
								this.SetComplianceStatus(TnefComplianceStatus.InvalidAttribute, TnefStrings.ReaderComplianceInvalidConversationId);
							}
							break;
						default:
							if (tnefAttributeTag != TnefAttributeTag.AttachTitle)
							{
							}
							break;
						}
					}
				}
				else if (tnefAttributeTag <= TnefAttributeTag.DateEnd)
				{
					if (tnefAttributeTag != TnefAttributeTag.Body)
					{
						switch (tnefAttributeTag)
						{
						case TnefAttributeTag.DateStart:
						case TnefAttributeTag.DateEnd:
							if (!this.CheckAndEnsureMoreAttributeDataLoaded(14))
							{
								return false;
							}
							break;
						}
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
							if (!this.CheckAndEnsureMoreAttributeDataLoaded(14))
							{
								return false;
							}
							return true;
						default:
							if (tnefAttributeTag != TnefAttributeTag.DateModified)
							{
								return true;
							}
							break;
						}
						break;
					}
					if (!this.CheckAndEnsureMoreAttributeDataLoaded(14))
					{
						this.error = true;
						return false;
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
							if (!this.CheckAndEnsureMoreAttributeDataLoaded(2))
							{
								return false;
							}
						}
					}
					else if (!this.CheckAndEnsureMoreAttributeDataLoaded(2))
					{
						return false;
					}
				}
				else if (tnefAttributeTag != TnefAttributeTag.AidOwner)
				{
					switch (tnefAttributeTag)
					{
					case TnefAttributeTag.Owner:
					case TnefAttributeTag.SentFor:
						if (this.messageClassSPlus)
						{
							if (this.attributeValueLength <= 4 || this.attributeValueLength > 32768)
							{
								this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeValue, TnefStrings.ReaderComplianceInvalidSchedulePlus);
								return false;
							}
							if (!this.CheckAndEnsureMoreAttributeDataLoaded(this.attributeValueLength))
							{
								return false;
							}
							if (!this.CrackSchedulePlusId())
							{
								return false;
							}
						}
						break;
					case TnefAttributeTag.Delegate:
						break;
					default:
						if (tnefAttributeTag == TnefAttributeTag.MessageStatus)
						{
							if (!this.CheckAndEnsureMoreAttributeDataLoaded(1))
							{
								return false;
							}
						}
						break;
					}
				}
				else if (!this.CheckAndEnsureMoreAttributeDataLoaded(4))
				{
					return false;
				}
			}
			else if (tnefAttributeTag <= TnefAttributeTag.OemCodepage)
			{
				switch (tnefAttributeTag)
				{
				case TnefAttributeTag.AttachData:
				case (TnefAttributeTag)426000:
				case TnefAttributeTag.AttachMetaFile:
					break;
				default:
					switch (tnefAttributeTag)
					{
					case TnefAttributeTag.MapiProperties:
					case TnefAttributeTag.Attachment:
						this.ReadDword();
						break;
					case TnefAttributeTag.RecipientTable:
						throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationMustBeInARow);
					}
					break;
				}
			}
			else if (tnefAttributeTag != TnefAttributeTag.OriginalMessageClass && tnefAttributeTag != TnefAttributeTag.MessageClass && tnefAttributeTag != TnefAttributeTag.TnefVersion)
			{
			}
			return true;
		}

		private bool PrepareLegacyProperty()
		{
			this.propertyPaddingLength = 0;
			this.propertyValueCount = 1;
			this.directRead = false;
			TnefAttributeTag tnefAttributeTag = this.attributeTag;
			if (tnefAttributeTag <= TnefAttributeTag.DateModified)
			{
				if (tnefAttributeTag <= TnefAttributeTag.AttachTitle)
				{
					if (tnefAttributeTag <= TnefAttributeTag.Subject)
					{
						if (tnefAttributeTag != TnefAttributeTag.From)
						{
							if (tnefAttributeTag == TnefAttributeTag.Subject)
							{
								this.propertyTag = TnefPropertyTag.SubjectA;
								this.directRead = true;
								return true;
							}
						}
						else
						{
							if (this.propertyIndex == 0)
							{
								this.propertyTag = TnefPropertyTag.SenderEntryId;
								return true;
							}
							this.propertyTag = TnefPropertyTag.SenderNameA;
							return true;
						}
					}
					else
					{
						switch (tnefAttributeTag)
						{
						case TnefAttributeTag.MessageId:
							this.propertyTag = TnefPropertyTag.SearchKey;
							return true;
						case TnefAttributeTag.ParentId:
							this.propertyTag = TnefPropertyTag.ParentKey;
							return true;
						case TnefAttributeTag.ConversationId:
							this.propertyTag = TnefPropertyTag.ConversationKey;
							return true;
						default:
							if (tnefAttributeTag == TnefAttributeTag.AttachTitle)
							{
								this.propertyTag = TnefPropertyTag.AttachFilenameA;
								this.directRead = true;
								return true;
							}
							break;
						}
					}
				}
				else if (tnefAttributeTag <= TnefAttributeTag.DateEnd)
				{
					if (tnefAttributeTag == TnefAttributeTag.Body)
					{
						this.propertyTag = TnefPropertyTag.BodyA;
						this.directRead = true;
						return true;
					}
					switch (tnefAttributeTag)
					{
					case TnefAttributeTag.DateStart:
						this.propertyTag = TnefPropertyTag.StartDate;
						return true;
					case TnefAttributeTag.DateEnd:
						this.propertyTag = TnefPropertyTag.EndDate;
						return true;
					}
				}
				else
				{
					switch (tnefAttributeTag)
					{
					case TnefAttributeTag.DateSent:
						this.propertyTag = TnefPropertyTag.ClientSubmitTime;
						return true;
					case TnefAttributeTag.DateReceived:
						this.propertyTag = TnefPropertyTag.MessageDeliveryTime;
						return true;
					default:
						switch (tnefAttributeTag)
						{
						case TnefAttributeTag.AttachCreateDate:
							this.propertyTag = TnefPropertyTag.CreationTime;
							return true;
						case TnefAttributeTag.AttachModifyDate:
							this.propertyTag = TnefPropertyTag.LastModificationTime;
							return true;
						default:
							if (tnefAttributeTag == TnefAttributeTag.DateModified)
							{
								this.propertyTag = TnefPropertyTag.LastModificationTime;
								return true;
							}
							break;
						}
						break;
					}
				}
			}
			else if (tnefAttributeTag <= TnefAttributeTag.Delegate)
			{
				if (tnefAttributeTag <= TnefAttributeTag.Priority)
				{
					if (tnefAttributeTag == TnefAttributeTag.RequestResponse)
					{
						this.propertyTag = TnefPropertyTag.ResponseRequested;
						this.directRead = true;
						return true;
					}
					if (tnefAttributeTag == TnefAttributeTag.Priority)
					{
						this.propertyTag = TnefPropertyTag.Importance;
						return true;
					}
				}
				else
				{
					if (tnefAttributeTag == TnefAttributeTag.AidOwner)
					{
						this.propertyTag = TnefPropertyTag.OwnerApptId;
						this.directRead = true;
						return true;
					}
					switch (tnefAttributeTag)
					{
					case TnefAttributeTag.Owner:
						if (this.propertyIndex == 0)
						{
							this.propertyTag = (this.messageClassSPlusResponse ? TnefPropertyTag.RcvdRepresentingEntryId : TnefPropertyTag.SentRepresentingEntryId);
							return true;
						}
						this.propertyTag = (this.messageClassSPlusResponse ? TnefPropertyTag.RcvdRepresentingNameA : TnefPropertyTag.SentRepresentingNameA);
						return true;
					case TnefAttributeTag.SentFor:
						if (this.propertyIndex == 0)
						{
							this.propertyTag = TnefPropertyTag.SentRepresentingEntryId;
							return true;
						}
						this.propertyTag = TnefPropertyTag.SentRepresentingNameA;
						return true;
					case TnefAttributeTag.Delegate:
						this.propertyTag = TnefPropertyTag.Delegation;
						this.directRead = true;
						return true;
					}
				}
			}
			else if (tnefAttributeTag <= TnefAttributeTag.AttachMetaFile)
			{
				if (tnefAttributeTag == TnefAttributeTag.MessageStatus)
				{
					this.propertyTag = TnefPropertyTag.MessageFlags;
					return true;
				}
				switch (tnefAttributeTag)
				{
				case TnefAttributeTag.AttachData:
					if (this.propertyIndex == 0)
					{
						this.propertyTag = TnefPropertyTag.AttachDataBin;
						this.directRead = true;
						return true;
					}
					this.propertyTag = TnefPropertyTag.AttachTag;
					return true;
				case TnefAttributeTag.AttachMetaFile:
					this.propertyTag = TnefPropertyTag.AttachRendering;
					this.directRead = true;
					return true;
				}
			}
			else
			{
				switch (tnefAttributeTag)
				{
				case TnefAttributeTag.AttachTransportFilename:
					this.propertyTag = TnefPropertyTag.AttachTransportNameA;
					this.directRead = true;
					return true;
				case TnefAttributeTag.AttachRenderData:
					if (this.propertyIndex == 0)
					{
						this.propertyTag = TnefPropertyTag.RenderingPosition;
						return true;
					}
					if (this.propertyIndex == 1)
					{
						this.propertyTag = TnefPropertyTag.AttachMethod;
						return true;
					}
					this.propertyTag = TnefPropertyTag.AttachEncoding;
					return true;
				case TnefAttributeTag.MapiProperties:
				case TnefAttributeTag.RecipientTable:
				case TnefAttributeTag.Attachment:
					break;
				default:
					if (tnefAttributeTag == TnefAttributeTag.OriginalMessageClass)
					{
						this.propertyTag = TnefPropertyTag.OrigMessageClassA;
						this.directRead = this.directReadForMessageClass;
						return true;
					}
					if (tnefAttributeTag == TnefAttributeTag.MessageClass)
					{
						this.propertyTag = TnefPropertyTag.MessageClassA;
						this.directRead = this.directReadForMessageClass;
						return true;
					}
					break;
				}
			}
			return false;
		}

		private bool PrepareLegacyPropertyValue()
		{
			this.propertyValueOffset = 0;
			this.propertyValueLength = this.propertyValueFixedLength;
			this.propertyValueStreamOffset = this.attributeValueStreamOffset;
			this.propertyValuePaddingLength = 0;
			TnefAttributeTag tnefAttributeTag = this.attributeTag;
			if (tnefAttributeTag <= TnefAttributeTag.DateModified)
			{
				if (tnefAttributeTag <= TnefAttributeTag.AttachTitle)
				{
					if (tnefAttributeTag <= TnefAttributeTag.Subject)
					{
						if (tnefAttributeTag != TnefAttributeTag.From)
						{
							if (tnefAttributeTag == TnefAttributeTag.Subject)
							{
								this.propertyValueLength = this.attributeValueLength;
								return true;
							}
						}
						else
						{
							if (this.propertyIndex == 0)
							{
								this.FabricateEntryIdFromTriple();
								return true;
							}
							this.FabricateNameFromTriple();
							return true;
						}
					}
					else
					{
						switch (tnefAttributeTag)
						{
						case TnefAttributeTag.MessageId:
							if (!this.FabricateTextizedBinary())
							{
								return false;
							}
							return true;
						case TnefAttributeTag.ParentId:
							if (!this.FabricateTextizedBinary())
							{
								return false;
							}
							return true;
						case TnefAttributeTag.ConversationId:
							if (!this.FabricateTextizedBinary())
							{
								return false;
							}
							return true;
						default:
							if (tnefAttributeTag == TnefAttributeTag.AttachTitle)
							{
								this.propertyValueLength = this.attributeValueLength;
								return true;
							}
							break;
						}
					}
				}
				else if (tnefAttributeTag <= TnefAttributeTag.DateEnd)
				{
					if (tnefAttributeTag == TnefAttributeTag.Body)
					{
						this.propertyValueLength = this.attributeValueLength;
						return true;
					}
					switch (tnefAttributeTag)
					{
					case TnefAttributeTag.DateStart:
						if (!this.FabricateSysTimeFromDTR())
						{
							return false;
						}
						return true;
					case TnefAttributeTag.DateEnd:
						if (!this.FabricateSysTimeFromDTR())
						{
							return false;
						}
						return true;
					}
				}
				else
				{
					switch (tnefAttributeTag)
					{
					case TnefAttributeTag.DateSent:
						if (!this.FabricateSysTimeFromDTR())
						{
							return false;
						}
						return true;
					case TnefAttributeTag.DateReceived:
						if (!this.FabricateSysTimeFromDTR())
						{
							return false;
						}
						return true;
					default:
						switch (tnefAttributeTag)
						{
						case TnefAttributeTag.AttachCreateDate:
							if (!this.FabricateSysTimeFromDTR())
							{
								return false;
							}
							return true;
						case TnefAttributeTag.AttachModifyDate:
							if (!this.FabricateSysTimeFromDTR())
							{
								return false;
							}
							return true;
						default:
							if (tnefAttributeTag == TnefAttributeTag.DateModified)
							{
								if (!this.FabricateSysTimeFromDTR())
								{
									return false;
								}
								return true;
							}
							break;
						}
						break;
					}
				}
			}
			else if (tnefAttributeTag <= TnefAttributeTag.Delegate)
			{
				if (tnefAttributeTag <= TnefAttributeTag.Priority)
				{
					if (tnefAttributeTag == TnefAttributeTag.RequestResponse)
					{
						this.propertyValueLength = 2;
						return true;
					}
					if (tnefAttributeTag == TnefAttributeTag.Priority)
					{
						this.FabricateImportanceFromPriority();
						return true;
					}
				}
				else
				{
					if (tnefAttributeTag == TnefAttributeTag.AidOwner)
					{
						this.propertyValueLength = 4;
						return true;
					}
					switch (tnefAttributeTag)
					{
					case TnefAttributeTag.Owner:
						if (this.propertyIndex == 0)
						{
							this.FabricateEntryIdFromTriple();
							return true;
						}
						this.FabricateNameFromTriple();
						return true;
					case TnefAttributeTag.SentFor:
						if (this.propertyIndex == 0)
						{
							this.FabricateEntryIdFromTriple();
							return true;
						}
						this.FabricateNameFromTriple();
						return true;
					case TnefAttributeTag.Delegate:
						this.propertyValueLength = this.attributeValueLength;
						return true;
					}
				}
			}
			else if (tnefAttributeTag <= TnefAttributeTag.AttachMetaFile)
			{
				if (tnefAttributeTag == TnefAttributeTag.MessageStatus)
				{
					this.FabricateMessageFlagsFromMessageStatus();
					return true;
				}
				switch (tnefAttributeTag)
				{
				case TnefAttributeTag.AttachData:
					if (this.propertyIndex == 0)
					{
						this.propertyValueLength = this.attributeValueLength;
						return true;
					}
					this.FabricateAttachTagOle1Storage();
					return true;
				case TnefAttributeTag.AttachMetaFile:
					this.propertyValueLength = this.attributeValueLength;
					return true;
				}
			}
			else
			{
				switch (tnefAttributeTag)
				{
				case TnefAttributeTag.AttachTransportFilename:
					this.propertyValueLength = this.attributeValueLength;
					return true;
				case TnefAttributeTag.AttachRenderData:
					if (this.propertyIndex == 0)
					{
						this.FabricateRenderingPositionFromRendData();
						return true;
					}
					if (this.propertyIndex == 1)
					{
						this.FabricateAttachMethodFromRendData();
						return true;
					}
					this.FabricateAttachEncodingFromRendData();
					return true;
				case TnefAttributeTag.MapiProperties:
				case TnefAttributeTag.RecipientTable:
				case TnefAttributeTag.Attachment:
					break;
				default:
					if (tnefAttributeTag == TnefAttributeTag.OriginalMessageClass || tnefAttributeTag == TnefAttributeTag.MessageClass)
					{
						if (!this.directRead)
						{
							this.propertyValueLength = this.fabricatedEnd - this.fabricatedOffset;
							return true;
						}
						this.propertyValueLength = this.AttributeRemainingCount();
						return true;
					}
					break;
				}
			}
			return false;
		}

		private bool FabricateMorePropertyData(int count)
		{
			TnefAttributeTag tnefAttributeTag = this.attributeTag;
			if (tnefAttributeTag <= TnefAttributeTag.DateModified)
			{
				if (tnefAttributeTag <= TnefAttributeTag.AttachTitle)
				{
					if (tnefAttributeTag <= TnefAttributeTag.Subject)
					{
						if (tnefAttributeTag == TnefAttributeTag.From)
						{
							goto IL_18B;
						}
						if (tnefAttributeTag != TnefAttributeTag.Subject)
						{
						}
					}
					else
					{
						switch (tnefAttributeTag)
						{
						case TnefAttributeTag.MessageId:
						case TnefAttributeTag.ParentId:
						case TnefAttributeTag.ConversationId:
							if (!this.FabricateTextizedBinary())
							{
								return false;
							}
							goto IL_1A1;
						default:
							if (tnefAttributeTag != TnefAttributeTag.AttachTitle)
							{
							}
							break;
						}
					}
				}
				else if (tnefAttributeTag <= TnefAttributeTag.DateEnd)
				{
					if (tnefAttributeTag != TnefAttributeTag.Body)
					{
						switch (tnefAttributeTag)
						{
						case TnefAttributeTag.DateStart:
						case TnefAttributeTag.DateEnd:
							goto IL_16F;
						}
					}
				}
				else
				{
					switch (tnefAttributeTag)
					{
					case TnefAttributeTag.DateSent:
					case TnefAttributeTag.DateReceived:
						goto IL_16F;
					default:
						switch (tnefAttributeTag)
						{
						case TnefAttributeTag.AttachCreateDate:
						case TnefAttributeTag.AttachModifyDate:
							goto IL_16F;
						default:
							if (tnefAttributeTag == TnefAttributeTag.DateModified)
							{
								goto IL_16F;
							}
							break;
						}
						break;
					}
				}
			}
			else if (tnefAttributeTag <= TnefAttributeTag.Delegate)
			{
				if (tnefAttributeTag <= TnefAttributeTag.Priority)
				{
					if (tnefAttributeTag != TnefAttributeTag.RequestResponse)
					{
						if (tnefAttributeTag == TnefAttributeTag.Priority)
						{
							goto IL_16F;
						}
					}
				}
				else if (tnefAttributeTag != TnefAttributeTag.AidOwner)
				{
					switch (tnefAttributeTag)
					{
					case TnefAttributeTag.Owner:
					case TnefAttributeTag.SentFor:
						goto IL_18B;
					}
				}
			}
			else if (tnefAttributeTag <= TnefAttributeTag.AttachMetaFile)
			{
				if (tnefAttributeTag == TnefAttributeTag.MessageStatus)
				{
					goto IL_16F;
				}
				switch (tnefAttributeTag)
				{
				case TnefAttributeTag.AttachData:
					goto IL_16F;
				}
			}
			else
			{
				switch (tnefAttributeTag)
				{
				case TnefAttributeTag.AttachTransportFilename:
				case TnefAttributeTag.MapiProperties:
				case TnefAttributeTag.RecipientTable:
				case TnefAttributeTag.Attachment:
					break;
				case TnefAttributeTag.AttachRenderData:
					goto IL_16F;
				default:
					if (tnefAttributeTag == TnefAttributeTag.OriginalMessageClass || tnefAttributeTag == TnefAttributeTag.MessageClass)
					{
						goto IL_16F;
					}
					break;
				}
			}
			return false;
			IL_16F:
			this.fabricatedEnd = (this.fabricatedOffset = 0);
			return false;
			IL_18B:
			if (this.propertyIndex == 0)
			{
				this.FabricateEntryIdFromTriple();
			}
			else
			{
				this.FabricateNameFromTriple();
			}
			IL_1A1:
			if (this.fabricatedEnd - this.fabricatedOffset < count)
			{
				this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeLength, TnefStrings.ReaderComplianceInvalidComputedPropertyLength);
				this.error = true;
				return false;
			}
			return true;
		}

		private bool FabricateSysTimeFromDTR()
		{
			if (this.ReadStateValue == TnefReader.ReadState.BeginPropertyValue)
			{
				try
				{
					DateTime dateTime = new DateTime((int)this.PeekWord(0), (int)this.PeekWord(2), (int)this.PeekWord(4), (int)this.PeekWord(6), (int)this.PeekWord(8), (int)this.PeekWord(10), DateTimeKind.Utc);
					TnefBitConverter.GetBytes(this.fabricatedBuffer, 0, dateTime.ToFileTimeUtc());
				}
				catch (ArgumentException)
				{
					this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeValue, TnefStrings.ReaderComplianceInvalidDateOrTimeValue);
					DateTime dateTime = DateTime.UtcNow;
					TnefBitConverter.GetBytes(this.fabricatedBuffer, 0, dateTime.ToFileTimeUtc());
				}
				this.fabricatedEnd = 8;
				this.propertyValueLength = this.fabricatedEnd;
			}
			else
			{
				this.fabricatedEnd = 0;
			}
			this.fabricatedOffset = 0;
			return true;
		}

		private void FabricateEntryIdFromTriple()
		{
			if (this.ReadStateValue == TnefReader.ReadState.BeginPropertyValue)
			{
				this.fabricatedOffset = 0;
				this.fabricatedEnd = 0;
				this.propertyValueLength = 24 + this.tripleNameLength + 1 + this.tripleAddressTypeLength + 1 + this.tripleAddressLength + 1;
				TnefBitConverter.GetBytes(this.fabricatedBuffer, 0, 0);
				Buffer.BlockCopy(TnefCommon.MuidOOP, 0, this.fabricatedBuffer, 4, 16);
				TnefBitConverter.GetBytes(this.fabricatedBuffer, 20, 0);
				this.fabricatedEnd = 24;
			}
			else
			{
				this.fabricatedOffset = 0;
				this.fabricatedEnd = 0;
			}
			int num = this.propertyValueOffset + (this.fabricatedEnd - this.fabricatedOffset);
			int num2 = this.fabricatedBuffer.Length - this.fabricatedEnd;
			if (num < 24 + this.tripleNameLength && num2 > 0)
			{
				int num3 = Math.Min(num2, 24 + this.tripleNameLength - num);
				int num4 = num - 24;
				Buffer.BlockCopy(this.readBuffer, this.readOffset + this.tripleNameOffset + num4, this.fabricatedBuffer, this.fabricatedEnd, num3);
				num += num3;
				num2 -= num3;
				this.fabricatedEnd += num3;
			}
			if (num == 24 + this.tripleNameLength && num2 > 0)
			{
				this.fabricatedBuffer[this.fabricatedEnd++] = 0;
				num++;
				num2--;
			}
			int num5 = 24 + this.tripleNameLength + 1;
			if (num < num5 + this.tripleAddressTypeLength && num2 > 0)
			{
				int num6 = Math.Min(num2, num5 + this.tripleAddressTypeLength - num);
				int num7 = num - num5;
				for (int i = 0; i < num6; i++)
				{
					this.fabricatedBuffer[this.fabricatedEnd + i] = (byte)char.ToUpperInvariant((char)this.readBuffer[this.readOffset + this.tripleAddressTypeOffset + num7]);
					num7++;
				}
				num += num6;
				num2 -= num6;
				this.fabricatedEnd += num6;
			}
			if (num == num5 + this.tripleAddressTypeLength && num2 > 0)
			{
				this.fabricatedBuffer[this.fabricatedEnd++] = 0;
				num++;
				num2--;
			}
			int num8 = num5 + this.tripleAddressTypeLength + 1;
			if (num < num8 + this.tripleAddressLength && num2 > 0)
			{
				int num9 = Math.Min(num2, num8 + this.tripleAddressLength - num);
				int num10 = num - num8;
				Buffer.BlockCopy(this.readBuffer, this.readOffset + this.tripleAddressOffset + num10, this.fabricatedBuffer, this.fabricatedEnd, num9);
				num += num9;
				num2 -= num9;
				this.fabricatedEnd += num9;
			}
			if (num == num8 + this.tripleAddressLength && num2 > 0)
			{
				this.fabricatedBuffer[this.fabricatedEnd++] = 0;
				num++;
				num2--;
			}
		}

		private void FabricateNameFromTriple()
		{
			if (this.ReadStateValue == TnefReader.ReadState.BeginPropertyValue)
			{
				this.fabricatedOffset = 0;
				this.fabricatedEnd = 0;
				this.propertyValueLength = this.tripleNameLength + 1;
			}
			else
			{
				this.fabricatedOffset = 0;
				this.fabricatedEnd = 0;
			}
			int num = Math.Min(this.PropertyRemainingCount() - (this.fabricatedEnd - this.fabricatedOffset), this.fabricatedBuffer.Length - this.fabricatedEnd);
			Buffer.BlockCopy(this.readBuffer, this.readOffset + this.tripleNameOffset + this.propertyValueOffset, this.fabricatedBuffer, this.fabricatedEnd, num);
			this.fabricatedEnd += num;
		}

		private void FabricateAttachTagOle1Storage()
		{
			if (this.ReadStateValue == TnefReader.ReadState.BeginPropertyValue)
			{
				Buffer.BlockCopy(TnefCommon.OidOle1Storage, 0, this.fabricatedBuffer, 0, TnefCommon.OidOle1Storage.Length);
				this.fabricatedEnd = TnefCommon.OidOle1Storage.Length;
				this.propertyValueLength = this.fabricatedEnd;
			}
			else
			{
				this.fabricatedEnd = 0;
			}
			this.fabricatedOffset = 0;
		}

		private void FabricateRenderingPositionFromRendData()
		{
			if (this.ReadStateValue == TnefReader.ReadState.BeginPropertyValue)
			{
				int value = this.PeekDword(2);
				TnefBitConverter.GetBytes(this.fabricatedBuffer, 0, value);
				this.fabricatedEnd = 4;
				this.propertyValueLength = this.fabricatedEnd;
			}
			else
			{
				this.fabricatedEnd = 0;
			}
			this.fabricatedOffset = 0;
		}

		private void FabricateAttachMethodFromRendData()
		{
			if (this.ReadStateValue == TnefReader.ReadState.BeginPropertyValue)
			{
				short num = this.PeekWord(0);
				int value = (num == 1) ? 1 : 6;
				TnefBitConverter.GetBytes(this.fabricatedBuffer, 0, value);
				this.fabricatedEnd = 4;
				this.propertyValueLength = this.fabricatedEnd;
			}
			else
			{
				this.fabricatedEnd = 0;
			}
			this.fabricatedOffset = 0;
		}

		private void FabricateAttachEncodingFromRendData()
		{
			if (this.ReadStateValue == TnefReader.ReadState.BeginPropertyValue)
			{
				Buffer.BlockCopy(TnefCommon.OidMacBinary, 0, this.fabricatedBuffer, 0, TnefCommon.OidMacBinary.Length);
				this.fabricatedEnd = TnefCommon.OidMacBinary.Length;
				this.propertyValueLength = this.fabricatedEnd;
			}
			else
			{
				this.fabricatedEnd = 0;
			}
			this.fabricatedOffset = 0;
		}

		private bool FabricateTextizedBinary()
		{
			if (this.ReadStateValue == TnefReader.ReadState.BeginPropertyValue)
			{
				this.fabricatedOffset = 0;
				this.fabricatedEnd = 0;
				this.propertyValueLength = (this.attributeValueLength & -2) / 2;
			}
			else
			{
				this.fabricatedOffset = 0;
				this.fabricatedEnd = 0;
			}
			while (this.attributeValueOffset != this.propertyValueLength * 2 && this.fabricatedEnd < this.fabricatedBuffer.Length)
			{
				if (!this.CheckAndEnsureMoreAttributeDataLoaded(2))
				{
					this.error = true;
					return false;
				}
				int val = 2 * Math.Min(this.PropertyRemainingCount() - (this.fabricatedEnd - this.fabricatedOffset), this.fabricatedBuffer.Length - this.fabricatedEnd);
				int num = Math.Min(val, this.AvailableCount()) & -2;
				for (int i = 0; i < num; i += 2)
				{
					this.fabricatedBuffer[this.fabricatedEnd++] = (byte)(((int)TnefReader.NumFromHex[(int)this.PeekByte(i)] << 4) + (int)TnefReader.NumFromHex[(int)this.PeekByte(i + 1)]);
				}
				this.SkipBytes(num);
			}
			return true;
		}

		private void FabricateImportanceFromPriority()
		{
			if (this.ReadStateValue == TnefReader.ReadState.BeginPropertyValue)
			{
				int value;
				switch (this.PeekWord(0))
				{
				case 1:
					value = 2;
					goto IL_34;
				case 3:
					value = 0;
					goto IL_34;
				}
				value = 1;
				IL_34:
				TnefBitConverter.GetBytes(this.fabricatedBuffer, 0, value);
				this.fabricatedEnd = 4;
				this.propertyValueLength = this.fabricatedEnd;
			}
			else
			{
				this.fabricatedEnd = 0;
			}
			this.fabricatedOffset = 0;
		}

		private void FabricateMessageFlagsFromMessageStatus()
		{
			if (this.ReadStateValue == TnefReader.ReadState.BeginPropertyValue)
			{
				short num = (short)this.PeekByte(0);
				int num2 = 0;
				num2 |= (((num & 32) != 0) ? 1 : 0);
				num2 |= (((num & 1) != 0) ? 0 : 2);
				num2 |= (((num & 4) != 0) ? 4 : 0);
				num2 |= (((num & 128) != 0) ? 16 : 0);
				num2 |= (((num & 2) != 0) ? 8 : 0);
				TnefBitConverter.GetBytes(this.fabricatedBuffer, 0, num2);
				this.fabricatedEnd = 4;
				this.propertyValueLength = this.fabricatedEnd;
			}
			else
			{
				this.fabricatedEnd = 0;
			}
			this.fabricatedOffset = 0;
		}

		private void FabricateMessageClass(bool rememberMessageClass)
		{
			if (TnefCommon.BytesEqualToPattern(this.readBuffer, this.readOffset, TnefCommon.MessageClassLegacyPrefix))
			{
				this.SkipBytes(TnefCommon.MessageClassLegacyPrefix.Length);
			}
			for (int i = 0; i < TnefCommon.MessageClassMappingTable.Length; i++)
			{
				if (this.AttributeRemainingCount() == TnefCommon.MessageClassMappingTable[i].LegacyName.Length + 1 && TnefCommon.BytesEqualToPattern(this.readBuffer, this.readOffset, TnefCommon.MessageClassMappingTable[i].LegacyName) && this.readBuffer[this.readOffset + this.AttributeRemainingCount() - 1] == 0)
				{
					if (rememberMessageClass)
					{
						this.messageClassSPlus = TnefCommon.MessageClassMappingTable[i].Splus;
						this.messageClassSPlusResponse = TnefCommon.MessageClassMappingTable[i].SplusResponse;
					}
					this.fabricatedOffset = 0;
					this.fabricatedEnd = CTSGlobals.AsciiEncoding.GetBytes(TnefCommon.MessageClassMappingTable[i].MapiName, 0, TnefCommon.MessageClassMappingTable[i].MapiName.Length, this.fabricatedBuffer, 0);
					this.fabricatedBuffer[this.fabricatedEnd++] = 0;
					this.directReadForMessageClass = false;
					this.propertyValueLength = this.fabricatedEnd;
					return;
				}
			}
			this.directReadForMessageClass = true;
			this.propertyValueLength = this.AttributeRemainingCount();
			this.fabricatedOffset = 0;
		}

		private string ReadAttributeUnicodeString(int bytes)
		{
			int num = 0;
			while (num < bytes - 1 && this.PeekWord(num) != 0)
			{
				num += 2;
			}
			if (this.decodeBuffer == null)
			{
				this.decodeBuffer = new char[512];
			}
			int num2;
			int num3;
			bool flag;
			this.unicodeDecoder.Convert(this.readBuffer, this.readOffset, num, this.decodeBuffer, 0, this.decodeBuffer.Length, true, out num2, out num3, out flag);
			if (flag)
			{
				this.SkipBytes(bytes);
				return new string(this.decodeBuffer, 0, num3);
			}
			StringBuilder stringBuilder = new StringBuilder(num / 2 + 1);
			stringBuilder.Append(this.decodeBuffer, 0, num3);
			this.SkipBytes(num2);
			num -= num2;
			bytes -= num2;
			do
			{
				this.unicodeDecoder.Convert(this.readBuffer, this.readOffset, num, this.decodeBuffer, 0, this.decodeBuffer.Length, true, out num2, out num3, out flag);
				stringBuilder.Append(this.decodeBuffer, 0, num3);
				this.SkipBytes(num2);
				num -= num2;
				bytes -= num2;
			}
			while (!flag);
			if (bytes != 0)
			{
				this.SkipBytes(bytes);
			}
			return stringBuilder.ToString();
		}

		private int AttributeRemainingCount()
		{
			return this.attributeValueLength - this.attributeValueOffset;
		}

		private bool MoreAttributeData(int bytes)
		{
			return this.AttributeRemainingCount() >= bytes;
		}

		private bool CheckAndEnsureMoreAttributeDataLoaded(int bytes)
		{
			if (this.AttributeRemainingCount() < bytes)
			{
				this.SetComplianceStatus(TnefComplianceStatus.AttributeOverflow, TnefStrings.ReaderComplianceAttributeValueOverflow);
				this.error = true;
				return false;
			}
			if (!this.EnsureMoreDataLoaded(bytes))
			{
				this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, TnefStrings.ReaderComplianceTnefTruncated);
				this.error = true;
				return false;
			}
			return true;
		}

		private void EatAttributeBytes(int count)
		{
			while (this.attributeValueOffset < this.attributeValueLength && count != 0)
			{
				if (!this.CheckAndEnsureMoreAttributeDataLoaded(1))
				{
					return;
				}
				int num = Math.Min(count, Math.Min(this.AttributeRemainingCount(), this.AvailableCount()));
				this.SkipBytes(num);
				count -= num;
			}
		}

		private byte PeekByte(int offsetFromCurrentPosition)
		{
			return this.readBuffer[this.readOffset + offsetFromCurrentPosition];
		}

		private short PeekWord(int offsetFromCurrentPosition)
		{
			return BitConverter.ToInt16(this.readBuffer, this.readOffset + offsetFromCurrentPosition);
		}

		private int PeekDword(int offsetFromCurrentPosition)
		{
			return BitConverter.ToInt32(this.readBuffer, this.readOffset + offsetFromCurrentPosition);
		}

		private short ReadByte()
		{
			byte b = this.readBuffer[this.readOffset];
			this.Checksum1(b);
			this.readOffset++;
			return (short)b;
		}

		private short ReadWord()
		{
			short num = BitConverter.ToInt16(this.readBuffer, this.readOffset);
			this.Checksum2(num);
			this.readOffset += 2;
			this.attributeValueOffset += 2;
			return num;
		}

		private int ReadDword()
		{
			int result = BitConverter.ToInt32(this.readBuffer, this.readOffset);
			this.Checksum4();
			this.readOffset += 4;
			this.attributeValueOffset += 4;
			return result;
		}

		private long ReadQword()
		{
			long result = BitConverter.ToInt64(this.readBuffer, this.readOffset);
			this.Checksum(8);
			this.readOffset += 8;
			this.attributeValueOffset += 8;
			return result;
		}

		private float ReadFloat()
		{
			float result = BitConverter.ToSingle(this.readBuffer, this.readOffset);
			this.Checksum4();
			this.readOffset += 4;
			this.attributeValueOffset += 4;
			return result;
		}

		private double ReadDouble()
		{
			double result = BitConverter.ToDouble(this.readBuffer, this.readOffset);
			this.Checksum(8);
			this.readOffset += 8;
			this.attributeValueOffset += 8;
			return result;
		}

		private Guid ReadGuid()
		{
			Guid result = new Guid(BitConverter.ToInt32(this.readBuffer, this.readOffset), BitConverter.ToInt16(this.readBuffer, this.readOffset + 4), BitConverter.ToInt16(this.readBuffer, this.readOffset + 6), this.readBuffer[this.readOffset + 8], this.readBuffer[this.readOffset + 9], this.readBuffer[this.readOffset + 10], this.readBuffer[this.readOffset + 11], this.readBuffer[this.readOffset + 12], this.readBuffer[this.readOffset + 13], this.readBuffer[this.readOffset + 14], this.readBuffer[this.readOffset + 15]);
			this.Checksum(16);
			this.readOffset += 16;
			this.attributeValueOffset += 16;
			return result;
		}

		private void ReadBytes(byte[] buffer, int offset, int count)
		{
			Buffer.BlockCopy(this.readBuffer, this.readOffset, buffer, offset, count);
			this.Checksum(count);
			this.readOffset += count;
			this.attributeValueOffset += count;
		}

		private void SkipBytes(int count)
		{
			this.Checksum(count);
			this.readOffset += count;
			this.attributeValueOffset += count;
		}

		private int AvailableCount()
		{
			return this.readEnd - this.readOffset;
		}

		private bool EnsureMoreDataLoaded(int bytes)
		{
			return !this.NeedToLoadMoreData(bytes) || this.LoadMoreData(bytes);
		}

		private bool NeedToLoadMoreData(int bytes)
		{
			return this.AvailableCount() < bytes;
		}

		private bool LoadMoreData(int bytes)
		{
			if (this.endOfFile)
			{
				return false;
			}
			if (this.readBuffer.Length < bytes)
			{
				byte[] dst = new byte[Math.Max(this.readBuffer.Length * 2, bytes)];
				if (this.readEndReal - this.readOffset != 0)
				{
					Buffer.BlockCopy(this.readBuffer, this.readOffset, dst, 0, this.readEndReal - this.readOffset);
				}
				this.readBuffer = dst;
			}
			else if (this.readEndReal - this.readOffset != 0 && this.readOffset != 0)
			{
				Buffer.BlockCopy(this.readBuffer, this.readOffset, this.readBuffer, 0, this.readEndReal - this.readOffset);
			}
			int num = this.readOffset;
			this.readEndReal -= this.readOffset;
			this.readOffset = 0;
			this.streamOffset += num;
			int num2 = this.InputStream.Read(this.readBuffer, this.readEndReal, this.readBuffer.Length - this.readEndReal);
			this.readEndReal += num2;
			this.endOfFile = (num2 == 0);
			this.readEnd = this.readEndReal;
			if (this.streamOffset + this.readEnd > this.streamMaxLength)
			{
				this.readEnd = this.streamMaxLength - this.streamOffset;
				this.endOfFile = true;
			}
			return this.readEnd >= bytes;
		}

		private void Checksum1(byte value)
		{
			if (!this.checksumDisabled)
			{
				this.checksum += (ushort)value;
			}
		}

		private void Checksum2(short value)
		{
			if (!this.checksumDisabled)
			{
				this.checksum += (ushort)(value & 255);
				this.checksum += (ushort)(value >> 8 & 255);
			}
		}

		private void Checksum4()
		{
			if (!this.checksumDisabled)
			{
				this.checksum += (ushort)this.readBuffer[this.readOffset];
				this.checksum += (ushort)this.readBuffer[this.readOffset + 1];
				this.checksum += (ushort)this.readBuffer[this.readOffset + 2];
				this.checksum += (ushort)this.readBuffer[this.readOffset + 3];
			}
		}

		private void Checksum(int count)
		{
			if (!this.checksumDisabled)
			{
				int num = this.readOffset;
				while (count-- > 0)
				{
					this.checksum += (ushort)this.readBuffer[num++];
				}
			}
		}

		private void SetComplianceStatus(TnefComplianceStatus status, string explanation)
		{
			this.complianceStatus |= status;
			if (this.complianceMode == TnefComplianceMode.Strict)
			{
				throw new TnefException(explanation);
			}
		}

		private bool CrackTriple()
		{
			short num = this.PeekWord(0);
			short num2 = this.PeekWord(4);
			short num3 = this.PeekWord(6);
			if (this.attributeValueLength < (int)(8 + num2 + num3) || num2 <= 0 || num3 <= 0)
			{
				this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeValue, TnefStrings.ReaderComplianceInvalidFrom);
				return false;
			}
			if (this.PeekByte((int)(8 + num2 + num3 - 1)) != 0)
			{
				this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeValue, TnefStrings.ReaderComplianceInvalidFrom);
				return false;
			}
			this.tripleNameOffset = 8;
			byte b;
			while ((b = this.PeekByte(this.tripleNameOffset)) == 32 || b == 9)
			{
				this.tripleNameOffset++;
			}
			switch (num)
			{
			case 3:
			case 4:
			case 9:
			{
				int num4 = this.tripleNameOffset;
				while (this.PeekByte(num4) != 0)
				{
					num4++;
				}
				this.tripleNameLength = num4 - this.tripleNameOffset;
				this.tripleAddressTypeOffset = (int)(8 + num2);
				while ((b = this.PeekByte(this.tripleAddressTypeOffset)) == 32 || b == 9)
				{
					this.tripleAddressTypeOffset++;
				}
				this.tripleAddressTypeLength = 0;
				this.tripleAddressOffset = 0;
				this.tripleAddressLength = 0;
				num4 = this.tripleAddressTypeOffset;
				while ((b = this.PeekByte(num4)) != 0 && b != 58)
				{
					num4++;
				}
				if (b == 58)
				{
					this.tripleAddressTypeLength = num4 - this.tripleAddressTypeOffset;
					while ((this.tripleAddressTypeLength > 0 && (b = this.PeekByte(this.tripleAddressTypeOffset + this.tripleAddressTypeLength - 1)) == 32) || b == 9)
					{
						this.tripleAddressTypeLength--;
					}
					this.tripleAddressOffset = num4 + 1;
					while ((b = this.PeekByte(this.tripleAddressOffset)) == 32 || b == 9)
					{
						this.tripleAddressOffset++;
					}
					num4 = this.tripleAddressOffset;
					while (this.PeekByte(num4) != 0)
					{
						num4++;
					}
					this.tripleAddressLength = num4 - this.tripleAddressOffset;
				}
				if (this.tripleAddressTypeLength == 0 || this.tripleAddressLength == 0)
				{
					this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeValue, TnefStrings.ReaderComplianceInvalidFrom);
					return false;
				}
				return true;
			}
			}
			this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeValue, TnefStrings.ReaderComplianceInvalidFrom);
			return false;
		}

		private bool CrackSchedulePlusId()
		{
			short num = this.PeekWord(0);
			if (num <= 0 || (int)(2 + num + 2) > this.attributeValueLength || this.PeekByte((int)(2 + num - 1)) != 0)
			{
				this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeValue, TnefStrings.ReaderComplianceInvalidSchedulePlus);
				return false;
			}
			this.tripleNameOffset = 2;
			byte b;
			while ((b = this.PeekByte(this.tripleNameOffset)) == 32 || b == 9)
			{
				this.tripleNameOffset++;
			}
			int num2 = this.tripleNameOffset;
			while (this.PeekByte(num2) != 0)
			{
				num2++;
			}
			this.tripleNameLength = num2 - this.tripleNameOffset;
			short num3 = this.PeekWord((int)(2 + num));
			if (num3 <= 0 || (int)(2 + num + 2 + num3) > this.attributeValueLength || this.PeekByte((int)(2 + num + 2 + num3 - 1)) != 0)
			{
				this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeValue, TnefStrings.ReaderComplianceInvalidSchedulePlus);
				return false;
			}
			this.tripleAddressTypeOffset = (int)(2 + num + 2);
			while ((b = this.PeekByte(this.tripleAddressTypeOffset)) == 32 || b == 9)
			{
				this.tripleAddressTypeOffset++;
			}
			this.tripleAddressTypeLength = 0;
			this.tripleAddressOffset = 0;
			this.tripleAddressLength = 0;
			num2 = this.tripleAddressTypeOffset;
			while ((b = this.PeekByte(num2)) != 0 && b != 58)
			{
				num2++;
			}
			if (b == 58)
			{
				this.tripleAddressTypeLength = num2 - this.tripleAddressTypeOffset;
				while ((this.tripleAddressTypeLength > 0 && (b = this.PeekByte(this.tripleAddressTypeOffset + this.tripleAddressTypeLength - 1)) == 32) || b == 9)
				{
					this.tripleAddressTypeLength--;
				}
				this.tripleAddressOffset = num2 + 1;
				while ((b = this.PeekByte(this.tripleAddressOffset)) == 32 || b == 9)
				{
					this.tripleAddressOffset++;
				}
				num2 = this.tripleAddressOffset;
				while (this.PeekByte(num2) != 0)
				{
					num2++;
				}
				this.tripleAddressLength = num2 - this.tripleAddressOffset;
			}
			if (this.tripleNameLength == 0 || this.tripleAddressTypeLength == 0 || this.tripleAddressLength == 0)
			{
				this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeValue, TnefStrings.ReaderComplianceInvalidSchedulePlus);
				return false;
			}
			return true;
		}

		internal void AssertGoodToUse(bool affectsChild)
		{
			if (this.InputStream == null)
			{
				throw new ObjectDisposedException("TnefReader");
			}
			if (affectsChild && this.Child != null)
			{
				throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationChildActive);
			}
		}

		internal void AssertInAttribute()
		{
			if (this.ReadStateValue < TnefReader.ReadState.EndAttribute)
			{
				throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationMustBeInAttribute);
			}
		}

		internal void AssertInProperty()
		{
			if (this.ReadStateValue < TnefReader.ReadState.EndProperty)
			{
				throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationMustBeInProperty);
			}
		}

		internal void AssertInPropertyValue()
		{
			if (this.ReadStateValue >= TnefReader.ReadState.EndPropertyValue)
			{
				return;
			}
			if (this.ReadStateValue == TnefReader.ReadState.BeginProperty && !this.propertyTag.IsMultiValued && this.propertyTag.ValueTnefType != TnefPropertyType.Null)
			{
				this.ReadNextPropertyValue();
				return;
			}
			throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationMustBeInPropertyValue);
		}

		private void AssertAtTheBeginningOfPropertyValue()
		{
			this.AssertGoodToUse(true);
			if (this.ReadStateValue == TnefReader.ReadState.BeginPropertyValue)
			{
				return;
			}
			if (this.ReadStateValue == TnefReader.ReadState.BeginProperty && !this.propertyTag.IsMultiValued && this.propertyTag.ValueTnefType != TnefPropertyType.Null)
			{
				this.ReadNextPropertyValue();
				return;
			}
			throw new InvalidOperationException(TnefStrings.ReaderInvalidOperationMustBeInPropertyValue);
		}

		private static DateTime FromOADate(double value)
		{
			return DateTime.FromOADate(value);
		}

		private const int ReadBufferSize = 4096;

		private static readonly DateTime MinDateTime = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);

		private static readonly byte[] NumFromHex = new byte[]
		{
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			0,
			1,
			2,
			3,
			4,
			5,
			6,
			7,
			8,
			9,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			10,
			11,
			12,
			13,
			14,
			15,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			10,
			11,
			12,
			13,
			14,
			15,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16
		};

		private TnefComplianceMode complianceMode;

		private bool checksumDisabled;

		private TnefComplianceStatus complianceStatus;

		internal Stream InputStream;

		private TnefReader parent;

		internal object Child;

		private int embeddingDepth;

		private int streamMaxLength;

		private bool endOfFile;

		private int streamOffset;

		private byte[] readBuffer;

		private int readOffset;

		private int readEnd;

		private int readEndReal;

		internal TnefReader.ReadState ReadStateValue;

		private bool error;

		private int tnefVersion;

		private short attachmentKey;

		private int messageCodepage;

		private TnefAttributeLevel attributeLevel;

		private TnefAttributeTag attributeTag;

		private bool legacyAttribute;

		private int attributeValueStreamOffset;

		private int attributeValueLength;

		private int attributeValueOffset;

		private ushort checksum;

		private ushort attributeStartChecksum;

		private int rowCount;

		private int rowIndex;

		private int propertyCount;

		private int propertyIndex;

		private TnefPropertyTag propertyTag;

		private TnefNameId propertyNameId;

		private int propertyValueCount;

		private int propertyValueIndex;

		private Guid propertyValueIId;

		private int propertyValueStreamOffset;

		private int propertyValueLength;

		private int propertyValueOffset;

		private int propertyValueFixedLength;

		private int propertyValuePaddingLength;

		private int propertyPaddingLength;

		private bool decoderFlushed;

		private Decoder decoder;

		private char[] decodeBuffer;

		private Decoder unicodeDecoder;

		private Decoder string8Decoder;

		private TnefPropertyReader propertyReader;

		private bool directRead;

		private byte[] fabricatedBuffer;

		private int fabricatedOffset;

		private int fabricatedEnd;

		private bool messageClassSPlus;

		private bool messageClassSPlusResponse;

		private bool currentAttachmentIsOle;

		private bool directReadForMessageClass;

		private int tripleNameOffset;

		private int tripleNameLength;

		private int tripleAddressTypeOffset;

		private int tripleAddressTypeLength;

		private int tripleAddressOffset;

		private int tripleAddressLength;

		internal enum ReadState
		{
			EndOfFile,
			Begin,
			EndAttribute,
			BeginAttribute,
			ReadAttributeValue,
			EndRow,
			BeginRow,
			EndProperty,
			BeginProperty,
			EndPropertyValue,
			BeginPropertyValue,
			ReadPropertyValue
		}
	}
}
