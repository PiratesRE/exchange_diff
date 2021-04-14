using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.ContentTypes.Tnef;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.TextConverters;

namespace Microsoft.Exchange.Data.MsgStorage.Internal
{
	internal class MsgSubStorageReader
	{
		internal MsgSubStorageReader(MsgStorageReader owner, ComStorage propertiesStorage, Encoding messageEncoding, MsgSubStorageType subStorageType)
		{
			this.owner = owner;
			this.propertiesStorage = propertiesStorage;
			this.messageEncoding = messageEncoding;
			this.subStorageType = subStorageType;
			this.ReadPropertiesStream();
		}

		internal Encoding MessageEncoding
		{
			get
			{
				if (this.messageEncoding == null)
				{
					Charset.DefaultWindowsCharset.TryGetEncoding(out this.messageEncoding);
				}
				return this.messageEncoding;
			}
		}

		internal int AttachmentCount
		{
			get
			{
				return this.prefix.AttachmentCount;
			}
		}

		internal int RecipientCount
		{
			get
			{
				return this.prefix.RecipientCount;
			}
		}

		internal TnefPropertyTag PropertyTag
		{
			get
			{
				this.CheckPropertyOffset();
				return this.currentProperty.Tag;
			}
		}

		internal int AttachMethod
		{
			get
			{
				if (this.subStorageType != MsgSubStorageType.Attachment)
				{
					throw new InvalidOperationException();
				}
				return this.attachMethod;
			}
		}

		private MsgSubStorageReader.ReaderBuffers Buffers
		{
			get
			{
				return this.owner.Buffers;
			}
		}

		internal bool ReadNextProperty()
		{
			if (!this.NextProperty())
			{
				return false;
			}
			this.ReadCurrentProperty();
			return true;
		}

		internal bool IsLargeValue()
		{
			this.CheckPropertyOffset();
			if (this.currentProperty.Tag.TnefType == TnefPropertyType.Object)
			{
				return true;
			}
			if (this.currentProperty.PropertyLength <= 32768)
			{
				return false;
			}
			if (!this.currentProperty.Rule.CanOpenStream)
			{
				throw new MsgStorageException(MsgStorageErrorCode.NonStreamablePropertyTooLong, MsgStorageStrings.CorruptData);
			}
			if (this.subStorageType == MsgSubStorageType.Recipient)
			{
				throw new MsgStorageException(MsgStorageErrorCode.RecipientPropertyTooLong, MsgStorageStrings.RecipientPropertyTooLong);
			}
			return true;
		}

		internal object ReadPropertyValue()
		{
			this.CheckPropertyOffset();
			if (this.IsLargeValue())
			{
				throw new InvalidOperationException(MsgStorageStrings.PropertyLongValue);
			}
			return this.InternalReadValue();
		}

		internal Stream OpenPropertyStream()
		{
			this.CheckPropertyOffset();
			if (this.subStorageType == MsgSubStorageType.Recipient)
			{
				throw new InvalidOperationException(MsgStorageStrings.RecipientPropertiesNotStreamable);
			}
			return this.InternalOpenPropertyStream(this.currentProperty.Tag);
		}

		internal Stream OpenPropertyStream(TnefPropertyTag propertyTag)
		{
			int num = this.prefix.Size;
			while (num + 16 <= this.propertiesContent.Length)
			{
				TnefPropertyTag propertyTag2 = MsgStoragePropertyData.ReadPropertyTag(this.propertiesContent, num);
				if (propertyTag2.ToUnicode() == propertyTag)
				{
					return this.InternalOpenPropertyStream(propertyTag2);
				}
				num += 16;
			}
			throw new InvalidOperationException(MsgStorageStrings.PropertyNotFound(propertyTag));
		}

		internal MsgStorageReader OpenAttachedMessage()
		{
			if (this.attachMethod != 5)
			{
				throw new InvalidOperationException(MsgStorageStrings.NotAMessageAttachment);
			}
			ComStorage comStorage = null;
			MsgStorageReader msgStorageReader = null;
			try
			{
				string storageName = Util.PropertyStreamName(TnefPropertyTag.AttachDataObj);
				comStorage = this.propertiesStorage.OpenStorage(storageName, ComStorage.OpenMode.Read);
				msgStorageReader = new MsgStorageReader(comStorage, this.owner.NamedPropertyList, this.MessageEncoding);
			}
			finally
			{
				if (comStorage != null && msgStorageReader == null)
				{
					comStorage.Dispose();
				}
			}
			return msgStorageReader;
		}

		internal byte[] ReadPropertyLengthsStream(TnefPropertyTag propertyTag, int bytesToRead)
		{
			byte[] lengthsBuffer = this.Buffers.GetLengthsBuffer(bytesToRead);
			string streamName = Util.PropertyStreamName(propertyTag);
			this.InternalReadStream(streamName, lengthsBuffer, bytesToRead, 0);
			return lengthsBuffer;
		}

		internal byte[] ReadPropertyStream(TnefPropertyTag propertyTag, int bytesToRead, int maxBytesSkipped)
		{
			byte[] valueBuffer = this.Buffers.GetValueBuffer(bytesToRead);
			this.ReadPropertyStream(propertyTag, valueBuffer, bytesToRead, maxBytesSkipped);
			return valueBuffer;
		}

		internal void ReadPropertyStream(TnefPropertyTag propertyTag, byte[] buffer, int bytesToRead, int maxBytesSkipped)
		{
			string streamName = Util.PropertyStreamName(propertyTag);
			this.InternalReadStream(streamName, buffer, bytesToRead, maxBytesSkipped);
		}

		internal byte[] ReadPropertyIndexStream(TnefPropertyTag propertyTag, int index, int bytesToRead, int maxBytesSkipped)
		{
			byte[] valueBuffer = this.Buffers.GetValueBuffer(bytesToRead);
			this.ReadPropertyIndexStream(propertyTag, index, valueBuffer, bytesToRead, maxBytesSkipped);
			return valueBuffer;
		}

		internal void ReadPropertyIndexStream(TnefPropertyTag propertyTag, int index, byte[] buffer, int bytesToRead, int maxBytesSkipped)
		{
			string streamName = Util.PropertyStreamName(propertyTag, index);
			this.InternalReadStream(streamName, buffer, bytesToRead, maxBytesSkipped);
		}

		private void CheckPropertyOffset()
		{
			if (this.currentOffset < this.prefix.Size)
			{
				throw new InvalidOperationException(MsgStorageStrings.CallReadNextProperty);
			}
			if (!this.IsCurrentPropertyValid())
			{
				throw new InvalidOperationException(MsgStorageStrings.AllPropertiesRead);
			}
		}

		private bool NextProperty()
		{
			if (this.currentOffset == 0)
			{
				this.currentOffset = this.prefix.Size;
			}
			else
			{
				if (!this.IsCurrentPropertyValid())
				{
					return false;
				}
				this.currentOffset += 16;
			}
			return this.IsCurrentPropertyValid();
		}

		private bool IsCurrentPropertyValid()
		{
			return this.currentOffset >= this.prefix.Size && this.currentOffset + 16 <= this.propertiesContent.Length;
		}

		private void ReadCurrentProperty()
		{
			TnefPropertyTag tnefPropertyTag = MsgStoragePropertyData.ReadPropertyTag(this.propertiesContent, this.currentOffset);
			int propertyLength = 0;
			MsgStoragePropertyTypeRule msgStoragePropertyTypeRule;
			if (!MsgStorageRulesTable.TryFindRule(tnefPropertyTag, out msgStoragePropertyTypeRule))
			{
				throw new MsgStorageException(MsgStorageErrorCode.InvalidPropertyType, MsgStorageStrings.CorruptData);
			}
			if (!msgStoragePropertyTypeRule.IsFixedValue)
			{
				propertyLength = MsgStoragePropertyData.ReadPropertyByteCount(this.propertiesContent, this.currentOffset);
			}
			this.currentProperty = default(MsgSubStorageReader.PropertyInfo);
			this.currentProperty.Tag = tnefPropertyTag;
			this.currentProperty.Rule = msgStoragePropertyTypeRule;
			this.currentProperty.PropertyLength = propertyLength;
		}

		private void ReadPropertiesStream()
		{
			this.propertiesContent = this.propertiesStorage.ReadFromStreamMaxLength("__properties_version1.0", int.MaxValue);
			this.prefix = new MsgStoragePropertyPrefix(this.subStorageType);
			if (this.propertiesContent.Length == 0 && (this.subStorageType == MsgSubStorageType.Attachment || this.subStorageType == MsgSubStorageType.Recipient))
			{
				throw new MsgStorageNotFoundException(MsgStorageErrorCode.EmptyPropertiesStream, MsgStorageStrings.NotFound, null);
			}
			if (this.propertiesContent.Length < this.prefix.Size)
			{
				throw new MsgStorageException(MsgStorageErrorCode.PropertyListTruncated, MsgStorageStrings.CorruptData);
			}
			this.prefix.Read(this.propertiesContent);
			this.currentOffset = 0;
			while (this.ReadNextProperty())
			{
				this.InternalAddProperty();
			}
			this.currentOffset = 0;
		}

		private void InternalAddProperty()
		{
			if (this.currentProperty.Tag == TnefPropertyTag.InternetCPID && this.messageEncoding == null && (this.subStorageType == MsgSubStorageType.AttachedMessage || this.subStorageType == MsgSubStorageType.TopLevelMessage))
			{
				int codePage = (int)this.ReadPropertyValue();
				Charset charset;
				if (Charset.TryGetCharset(codePage, out charset))
				{
					charset.Culture.WindowsCharset.TryGetEncoding(out this.messageEncoding);
				}
				if (this.messageEncoding == null)
				{
					this.messageEncoding = Charset.DefaultWindowsCharset.GetEncoding();
					return;
				}
			}
			else if (this.currentProperty.Tag == TnefPropertyTag.AttachMethod && this.subStorageType == MsgSubStorageType.Attachment)
			{
				this.attachMethod = (int)this.ReadPropertyValue();
			}
		}

		private Stream InternalOpenPropertyStream(TnefPropertyTag propertyTag)
		{
			if (propertyTag == TnefPropertyTag.AttachDataObj)
			{
				return this.InternalOpenOleAttachmentStream();
			}
			MsgStoragePropertyTypeRule msgStoragePropertyTypeRule;
			MsgStorageRulesTable.TryFindRule(propertyTag, out msgStoragePropertyTypeRule);
			if (!msgStoragePropertyTypeRule.CanOpenStream)
			{
				throw new InvalidOperationException(MsgStorageStrings.NonStreamableProperty);
			}
			Stream stream = this.propertiesStorage.OpenStream(Util.PropertyStreamName(propertyTag), ComStorage.OpenMode.Read);
			if (propertyTag.TnefType == TnefPropertyType.String8)
			{
				stream = new ConverterStream(stream, new TextToText(TextToTextConversionMode.ConvertCodePageOnly)
				{
					InputEncoding = this.MessageEncoding,
					OutputEncoding = Util.UnicodeEncoding
				}, ConverterStreamAccess.Read);
			}
			return stream;
		}

		private Stream InternalOpenOleAttachmentStream()
		{
			if (this.subStorageType != MsgSubStorageType.Attachment || this.attachMethod != 6)
			{
				throw new InvalidOperationException(MsgStorageStrings.NotAnOleAttachment);
			}
			ComStorage comStorage = null;
			ComStorage comStorage2 = null;
			Stream stream = Streams.CreateTemporaryStorageStream();
			try
			{
				string storageName = Util.PropertyStreamName(TnefPropertyTag.AttachDataObj);
				comStorage = this.propertiesStorage.OpenStorage(storageName, ComStorage.OpenMode.Read);
				comStorage2 = ComStorage.CreateStorageOnStream(stream, ComStorage.OpenMode.CreateWrite);
				ComStorage.CopyStorageContent(comStorage, comStorage2);
				comStorage2.Flush();
			}
			finally
			{
				if (comStorage != null)
				{
					comStorage.Dispose();
				}
				if (comStorage2 != null)
				{
					comStorage2.Dispose();
				}
			}
			stream.Position = 0L;
			return stream;
		}

		private void InternalReadStream(string streamName, byte[] buffer, int bytesToRead, int maxBytesSkipped)
		{
			if (bytesToRead != 0)
			{
				int num = this.propertiesStorage.ReadFromStream(streamName, buffer, bytesToRead);
				if (num < bytesToRead)
				{
					if (num < bytesToRead - maxBytesSkipped)
					{
						throw new MsgStorageException(MsgStorageErrorCode.PropertyValueTruncated, MsgStorageStrings.PropertyValueTruncated);
					}
					for (int i = num; i < bytesToRead; i++)
					{
						buffer[i] = 0;
					}
				}
			}
		}

		private object InternalReadValue()
		{
			MsgStoragePropertyTypeRule rule = this.currentProperty.Rule;
			if (rule.IsFixedValue)
			{
				return rule.ReadFixedValue(this.propertiesContent, this.currentOffset);
			}
			return rule.ReadStreamedValue(this, this.currentProperty);
		}

		private readonly MsgStorageReader owner;

		private readonly ComStorage propertiesStorage;

		private MsgSubStorageType subStorageType;

		private MsgStoragePropertyPrefix prefix;

		private byte[] propertiesContent;

		private int currentOffset;

		private MsgSubStorageReader.PropertyInfo currentProperty;

		private Encoding messageEncoding;

		private int attachMethod;

		internal struct ReaderBuffers
		{
			public byte[] GetValueBuffer(int size)
			{
				if (this.valueBuffer == null || this.valueBuffer.Length < size)
				{
					size = (size + 2048 & -2048);
					this.valueBuffer = new byte[size];
				}
				return this.valueBuffer;
			}

			public byte[] GetLengthsBuffer(int size)
			{
				if (this.lengthsBuffer == null || this.lengthsBuffer.Length < size)
				{
					size = (size + 512 & -512);
					this.lengthsBuffer = new byte[size];
				}
				return this.lengthsBuffer;
			}

			private byte[] valueBuffer;

			private byte[] lengthsBuffer;
		}

		internal struct PropertyInfo
		{
			public TnefPropertyTag Tag;

			public int PropertyLength;

			public MsgStoragePropertyTypeRule Rule;
		}
	}
}
