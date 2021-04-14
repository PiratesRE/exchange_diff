using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.ContentTypes.Tnef;
using Microsoft.Exchange.Data.Internal;

namespace Microsoft.Exchange.Data.MsgStorage.Internal
{
	internal class MsgSubStorageWriter : IDisposable
	{
		internal MsgSubStorageWriter(MsgStorageWriter owner, MsgSubStorageType type, ComStorage subStorage)
		{
			this.owner = owner;
			this.subStorage = subStorage;
			this.subStorageType = type;
			this.attachMethod = -1;
			this.propertiesCache = new MemoryStream();
			this.propertiesWriter = new BinaryWriter(this.propertiesCache);
			this.prefix = new MsgStoragePropertyPrefix(type);
			this.prefix.Write(this.propertiesWriter);
		}

		internal void WriteProperty(TnefPropertyTag propertyTag, object propertyValue)
		{
			if (propertyTag == TnefPropertyTag.AttachMethod && this.subStorageType == MsgSubStorageType.Attachment)
			{
				int? num = propertyValue as int?;
				if (num != null)
				{
					this.attachMethod = num.Value;
				}
			}
			MsgStoragePropertyTypeRule msgStoragePropertyTypeRule;
			MsgStorageRulesTable.TryFindRule(propertyTag, out msgStoragePropertyTypeRule);
			msgStoragePropertyTypeRule.WriteValue(this, propertyTag, propertyValue);
		}

		internal Stream OpenPropertyStream(TnefPropertyTag propertyTag)
		{
			if (this.subStorageType == MsgSubStorageType.Recipient)
			{
				throw new InvalidOperationException(MsgStorageStrings.RecipientPropertiesNotStreamable);
			}
			MsgStoragePropertyTypeRule msgStoragePropertyTypeRule;
			MsgStorageRulesTable.TryFindRule(propertyTag, out msgStoragePropertyTypeRule);
			if (!msgStoragePropertyTypeRule.CanOpenStream)
			{
				throw new InvalidOperationException(MsgStorageStrings.NonStreamableProperty);
			}
			Stream stream3 = null;
			Stream stream2 = null;
			try
			{
				int addStringTerminators = 0;
				if (propertyTag.TnefType == TnefPropertyType.Unicode)
				{
					addStringTerminators = 2;
				}
				else if (propertyTag.TnefType == TnefPropertyType.String8)
				{
					addStringTerminators = 1;
				}
				string streamName = Util.PropertyStreamName(propertyTag);
				stream3 = this.subStorage.CreateStream(streamName, ComStorage.OpenMode.CreateWrite);
				MsgStorageWriteStream msgStorageWriteStream = new MsgStorageWriteStream(stream3, addStringTerminators);
				stream3 = msgStorageWriteStream;
				stream3 = new BufferedStream(msgStorageWriteStream, 32768);
				msgStorageWriteStream.AddOnCloseNotifier(delegate(MsgStorageWriteStream stream, Exception onCloseException)
				{
					if (onCloseException != null)
					{
						this.owner.SetFailure(new MsgStorageException(MsgStorageErrorCode.FailedWrite, MsgStorageStrings.ComExceptionThrown, onCloseException));
						return;
					}
					try
					{
						MsgStoragePropertyData.WriteStream(this.propertiesWriter, propertyTag, (int)stream.Length);
					}
					catch (IOException exc)
					{
						this.owner.SetFailure(new MsgStorageException(MsgStorageErrorCode.FailedWriteOle, MsgStorageStrings.CorruptData, exc));
					}
					catch (COMException exc2)
					{
						this.owner.SetFailure(new MsgStorageException(MsgStorageErrorCode.FailedWriteOle, MsgStorageStrings.CorruptData, exc2));
					}
				});
				stream2 = stream3;
			}
			finally
			{
				if (stream2 == null && stream3 != null)
				{
					stream3.Dispose();
				}
			}
			return stream2;
		}

		internal MsgSubStorageWriter OpenRecipientWriter()
		{
			int recipientCount = this.prefix.RecipientCount;
			ComStorage comStorage = null;
			MsgSubStorageWriter msgSubStorageWriter = null;
			try
			{
				string storageName = Util.RecipientStorageName(recipientCount);
				comStorage = this.subStorage.CreateStorage(storageName, ComStorage.OpenMode.CreateWrite);
				msgSubStorageWriter = new MsgSubStorageWriter(this.owner, MsgSubStorageType.Recipient, comStorage);
			}
			finally
			{
				if (msgSubStorageWriter == null && comStorage != null)
				{
					comStorage.Dispose();
				}
			}
			this.prefix.RecipientCount++;
			return msgSubStorageWriter;
		}

		internal MsgSubStorageWriter OpenAttachmentWriter()
		{
			int attachmentCount = this.prefix.AttachmentCount;
			ComStorage comStorage = null;
			MsgSubStorageWriter msgSubStorageWriter = null;
			try
			{
				string storageName = Util.AttachmentStorageName(attachmentCount);
				comStorage = this.subStorage.CreateStorage(storageName, ComStorage.OpenMode.CreateWrite);
				msgSubStorageWriter = new MsgSubStorageWriter(this.owner, MsgSubStorageType.Attachment, comStorage);
			}
			finally
			{
				if (msgSubStorageWriter == null && comStorage != null)
				{
					comStorage.Dispose();
				}
			}
			this.prefix.AttachmentCount++;
			return msgSubStorageWriter;
		}

		internal MsgStorageWriter OpenAttachedMessageWriter()
		{
			if (this.attachMethod != 5)
			{
				throw new InvalidOperationException(MsgStorageStrings.NotAMessageAttachment);
			}
			ComStorage comStorage = null;
			MsgStorageWriter msgStorageWriter = null;
			try
			{
				TnefPropertyTag attachDataObj = TnefPropertyTag.AttachDataObj;
				string storageName = Util.PropertyStreamName(attachDataObj);
				MsgStoragePropertyData.WriteObject(this.propertiesWriter, attachDataObj, MsgStoragePropertyData.ObjectType.Message);
				comStorage = this.subStorage.CreateStorage(storageName, ComStorage.OpenMode.CreateWrite);
				comStorage.StorageClass = Util.ClassIdMessageAttachment;
				msgStorageWriter = new MsgStorageWriter(this.owner, comStorage);
			}
			finally
			{
				if (msgStorageWriter == null && comStorage != null)
				{
					comStorage.Dispose();
				}
			}
			return msgStorageWriter;
		}

		internal Stream OpenOleAttachmentStream()
		{
			if (this.attachMethod != 6)
			{
				throw new InvalidOperationException(MsgStorageStrings.NotAnOleAttachment);
			}
			Stream cacheStream = Streams.CreateTemporaryStorageStream();
			MsgStorageWriteStream msgStorageWriteStream = new MsgStorageWriteStream(cacheStream, 0);
			msgStorageWriteStream.AddOnCloseNotifier(delegate(MsgStorageWriteStream stream, Exception onCloseException)
			{
				if (onCloseException != null)
				{
					this.owner.SetFailure(new MsgStorageException(MsgStorageErrorCode.FailedWrite, MsgStorageStrings.ComExceptionThrown, onCloseException));
					return;
				}
				TnefPropertyTag attachDataObj = TnefPropertyTag.AttachDataObj;
				ComStorage comStorage = null;
				ComStorage comStorage2 = null;
				string storageName = Util.PropertyStreamName(attachDataObj);
				try
				{
					cacheStream.Flush();
					cacheStream.Position = 0L;
					comStorage = ComStorage.OpenStorageOnStream(cacheStream, ComStorage.OpenMode.Read);
					comStorage2 = this.subStorage.CreateStorage(storageName, ComStorage.OpenMode.CreateWrite);
					comStorage2.StorageClass = Util.ClassIdFileAttachment;
					ComStorage.CopyStorageContent(comStorage, comStorage2);
					comStorage2.Flush();
					MsgStoragePropertyData.WriteObject(this.propertiesWriter, attachDataObj, MsgStoragePropertyData.ObjectType.Storage);
				}
				catch (IOException exc)
				{
					this.owner.SetFailure(new MsgStorageException(MsgStorageErrorCode.FailedWriteOle, MsgStorageStrings.CorruptData, exc));
				}
				catch (COMException exc2)
				{
					this.owner.SetFailure(new MsgStorageException(MsgStorageErrorCode.FailedWriteOle, MsgStorageStrings.CorruptData, exc2));
				}
				finally
				{
					if (comStorage2 != null)
					{
						comStorage2.Dispose();
					}
					if (comStorage != null)
					{
						comStorage.Dispose();
					}
					if (cacheStream != null)
					{
						cacheStream.Dispose();
					}
				}
			});
			return msgStorageWriteStream;
		}

		internal MsgStorageWriter Owner
		{
			get
			{
				return this.owner;
			}
		}

		internal BinaryWriter PropertiesWriter
		{
			get
			{
				return this.propertiesWriter;
			}
		}

		internal ComStorage Storage
		{
			get
			{
				return this.subStorage;
			}
		}

		internal NamedPropertyList NamedPropertyList
		{
			get
			{
				return this.owner.NamedPropertyList;
			}
		}

		internal MsgSubStorageWriter.WriterBuffer LengthsBuffer
		{
			get
			{
				return this.owner.LengthsBuffer.Reset();
			}
		}

		internal MsgSubStorageWriter.WriterBuffer ValueBuffer
		{
			get
			{
				return this.owner.ValueBuffer.Reset();
			}
		}

		public void Close()
		{
			try
			{
				if (this.propertiesCache != null)
				{
					if (this.prefix.AttachmentCount != 0 || this.prefix.RecipientCount != 0)
					{
						this.propertiesWriter.Flush();
						this.propertiesWriter.Seek(0, SeekOrigin.Begin);
						this.prefix.Write(this.propertiesWriter);
						this.propertiesWriter.Flush();
					}
					using (Stream stream = this.subStorage.CreateStream("__properties_version1.0", ComStorage.OpenMode.CreateWrite))
					{
						stream.Write(this.propertiesCache.GetBuffer(), 0, (int)this.propertiesCache.Length);
					}
					if (this.subStorage != null)
					{
						this.subStorage.Flush();
					}
				}
			}
			finally
			{
				if (this.propertiesCache != null)
				{
					this.propertiesCache.Flush();
					this.propertiesCache.Dispose();
				}
				if (this.subStorage != null)
				{
					this.subStorage.Dispose();
				}
				this.propertiesCache = null;
				this.subStorage = null;
			}
			this.isDisposed = true;
		}

		protected void CheckDisposed(string methodName)
		{
			if (this.isDisposed)
			{
				throw new ObjectDisposedException(methodName);
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				this.isDisposed = true;
				this.InternalDispose(disposing);
			}
		}

		protected virtual void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				try
				{
					this.Close();
				}
				catch (IOException exc)
				{
					this.owner.SetFailure(new MsgStorageException(MsgStorageErrorCode.FailedWrite, MsgStorageStrings.ComExceptionThrown, exc));
				}
				catch (COMException exc2)
				{
					this.owner.SetFailure(new MsgStorageException(MsgStorageErrorCode.FailedWrite, MsgStorageStrings.ComExceptionThrown, exc2));
				}
			}
		}

		private const int BufferSize = 32768;

		private readonly MsgStorageWriter owner;

		private readonly MsgSubStorageType subStorageType;

		private readonly MsgStoragePropertyPrefix prefix;

		private ComStorage subStorage;

		private MemoryStream propertiesCache;

		private BinaryWriter propertiesWriter;

		private int attachMethod;

		private bool isDisposed;

		internal class WriterBuffer
		{
			internal WriterBuffer(int initialSize)
			{
				this.stream = new MemoryStream(initialSize);
				this.writer = new BinaryWriter(this.stream, Util.UnicodeEncoding);
			}

			internal byte[] PreallocateBuffer(int size)
			{
				if (this.stream.Capacity < size)
				{
					this.stream.Capacity = (size + 2048 & -2048);
				}
				return this.stream.GetBuffer();
			}

			internal BinaryWriter Writer
			{
				get
				{
					return this.writer;
				}
			}

			internal MsgSubStorageWriter.WriterBuffer Reset()
			{
				this.writer.Seek(0, SeekOrigin.Begin);
				this.stream.SetLength(0L);
				return this;
			}

			internal int GetLength()
			{
				this.writer.Flush();
				return (int)this.stream.Length;
			}

			internal byte[] GetBuffer()
			{
				this.writer.Flush();
				return this.stream.GetBuffer();
			}

			private MemoryStream stream;

			private BinaryWriter writer;
		}
	}
}
