using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.ContentTypes.Tnef;

namespace Microsoft.Exchange.Data.MsgStorage.Internal
{
	public class MsgStorageWriter : IDisposable
	{
		public MsgStorageWriter(string filename)
		{
			this.namedPropertyList = new NamedPropertyList();
			this.isTopLevelMessage = true;
			this.messageStorage = ComStorage.CreateFileStorage(filename, ComStorage.OpenMode.CreateWrite);
			this.Initialize(MsgSubStorageType.TopLevelMessage);
		}

		public MsgStorageWriter(Stream stream)
		{
			this.namedPropertyList = new NamedPropertyList();
			this.isTopLevelMessage = true;
			this.messageStorage = ComStorage.CreateStorageOnStream(stream, ComStorage.OpenMode.CreateWrite);
			this.Initialize(MsgSubStorageType.TopLevelMessage);
		}

		internal MsgStorageWriter(MsgStorageWriter parent, ComStorage storage)
		{
			this.parent = parent;
			this.messageStorage = storage;
			this.namedPropertyList = parent.NamedPropertyList;
			this.isTopLevelMessage = false;
			this.Initialize(MsgSubStorageType.AttachedMessage);
		}

		private void Initialize(MsgSubStorageType type)
		{
			this.messageStorage.StorageClass = Util.ClassIdMessage;
			this.lengthsBuffer = new MsgSubStorageWriter.WriterBuffer(512);
			this.valueBuffer = new MsgSubStorageWriter.WriterBuffer(2048);
			this.componentWriter = null;
			this.lastFailure = null;
			this.messageWriter = new MsgSubStorageWriter(this, type, this.messageStorage);
		}

		public void WriteProperty(TnefPropertyTag propertyTag, object propertyValue)
		{
			this.CheckDisposed("MsgStorageWriter::WriteProperty(1)");
			if (propertyTag.IsNamed)
			{
				throw new ArgumentException(MsgStorageStrings.InvalidPropertyTag(propertyTag), "propertyTag");
			}
			MsgStorageRulesTable.ThrowOnInvalidPropertyType(propertyTag);
			Util.ThrowOnNullArgument(propertyValue, "propertyValue");
			this.CheckFailure();
			this.CurrentWriter.WriteProperty(propertyTag, propertyValue);
		}

		public void WriteProperty(Guid propertyGuid, string name, TnefPropertyType propertyType, object propertyValue)
		{
			this.CheckDisposed("MsgStorageWriter::WriteProperty(2)");
			Util.ThrowOnNullArgument(name, "name");
			MsgStorageRulesTable.ThrowOnInvalidPropertyType(propertyType);
			Util.ThrowOnNullArgument(propertyValue, "propertyValue");
			this.CheckFailure();
			TnefNameId namedProperty = new TnefNameId(propertyGuid, name);
			TnefPropertyId id = this.NamedPropertyList.Add(namedProperty);
			TnefPropertyTag propertyTag = new TnefPropertyTag(id, propertyType);
			this.CurrentWriter.WriteProperty(propertyTag, propertyValue);
		}

		public void WriteProperty(Guid propertyGuid, int namedId, TnefPropertyType propertyType, object propertyValue)
		{
			this.CheckDisposed("MsgStorageWriter::WriteProperty(3)");
			MsgStorageRulesTable.ThrowOnInvalidPropertyType(propertyType);
			Util.ThrowOnNullArgument(propertyValue, "propertyValue");
			this.CheckFailure();
			TnefNameId namedProperty = new TnefNameId(propertyGuid, namedId);
			TnefPropertyId id = this.NamedPropertyList.Add(namedProperty);
			TnefPropertyTag propertyTag = new TnefPropertyTag(id, propertyType);
			this.CurrentWriter.WriteProperty(propertyTag, propertyValue);
		}

		public Stream OpenPropertyStream(TnefPropertyTag propertyTag)
		{
			this.CheckDisposed("MsgStorageWriter::OpenPropertyStream(1)");
			if (propertyTag.IsNamed)
			{
				throw new ArgumentException(MsgStorageStrings.InvalidPropertyTag(propertyTag), "propertyTag");
			}
			MsgStorageRulesTable.ThrowOnInvalidPropertyType(propertyTag);
			this.CheckFailure();
			if (propertyTag == TnefPropertyTag.AttachDataObj)
			{
				return this.CurrentWriter.OpenOleAttachmentStream();
			}
			return this.CurrentWriter.OpenPropertyStream(propertyTag);
		}

		public Stream OpenPropertyStream(Guid propertyGuid, string name, TnefPropertyType propertyType)
		{
			this.CheckDisposed("MsgStorageWriter::OpenPropertyStream(2)");
			Util.ThrowOnNullArgument(name, "name");
			MsgStorageRulesTable.ThrowOnInvalidPropertyType(propertyType);
			this.CheckFailure();
			TnefNameId namedProperty = new TnefNameId(propertyGuid, name);
			TnefPropertyId id = this.NamedPropertyList.Add(namedProperty);
			TnefPropertyTag propertyTag = new TnefPropertyTag(id, propertyType);
			return this.CurrentWriter.OpenPropertyStream(propertyTag);
		}

		public Stream OpenPropertyStream(Guid propertyGuid, int namedId, TnefPropertyType propertyType)
		{
			this.CheckDisposed("MsgStorageWriter::OpenPropertyStream(3)");
			MsgStorageRulesTable.ThrowOnInvalidPropertyType(propertyType);
			this.CheckFailure();
			TnefNameId namedProperty = new TnefNameId(propertyGuid, namedId);
			TnefPropertyId id = this.NamedPropertyList.Add(namedProperty);
			TnefPropertyTag propertyTag = new TnefPropertyTag(id, propertyType);
			return this.CurrentWriter.OpenPropertyStream(propertyTag);
		}

		public void StartAttachment()
		{
			this.CheckDisposed("MsgStorageWriter::StartAttachment");
			this.CheckFailure();
			MsgSubStorageWriter msgSubStorageWriter = this.messageWriter.OpenAttachmentWriter();
			if (this.componentWriter != null)
			{
				this.componentWriter.Close();
			}
			this.componentWriter = msgSubStorageWriter;
		}

		public void StartRecipient()
		{
			this.CheckDisposed("MsgStorageWriter::StartRecipient");
			this.CheckFailure();
			MsgSubStorageWriter msgSubStorageWriter = this.messageWriter.OpenRecipientWriter();
			if (this.componentWriter != null)
			{
				this.componentWriter.Close();
			}
			this.componentWriter = msgSubStorageWriter;
		}

		public MsgStorageWriter GetEmbeddedMessageWriter()
		{
			this.CheckDisposed("MsgStorageWriter::GetEmbeddedMessageWriter");
			this.CheckFailure();
			return this.componentWriter.OpenAttachedMessageWriter();
		}

		public void Flush()
		{
			if (this.isFlushed)
			{
				return;
			}
			try
			{
				this.CheckFailure();
				if (this.componentWriter != null)
				{
					this.componentWriter.Close();
					this.componentWriter = null;
				}
				if (this.messageWriter != null)
				{
					if (this.isTopLevelMessage)
					{
						this.namedPropertyList.WriteTo(this.messageStorage);
					}
					this.messageWriter.Close();
					this.messageWriter = null;
				}
				this.CheckFailure();
			}
			finally
			{
				if (this.componentWriter != null)
				{
					this.componentWriter.Dispose();
					this.componentWriter = null;
				}
				if (this.messageWriter != null)
				{
					this.messageWriter.Dispose();
					this.messageWriter = null;
				}
			}
			this.isFlushed = true;
		}

		internal void SetFailure(Exception exc)
		{
			this.lastFailure = exc;
		}

		private void CheckFailure()
		{
			if (this.lastFailure != null)
			{
				throw this.lastFailure;
			}
		}

		private MsgSubStorageWriter CurrentWriter
		{
			get
			{
				return this.componentWriter ?? this.messageWriter;
			}
		}

		internal NamedPropertyList NamedPropertyList
		{
			get
			{
				return this.namedPropertyList;
			}
		}

		internal MsgSubStorageWriter.WriterBuffer LengthsBuffer
		{
			get
			{
				return this.lengthsBuffer;
			}
		}

		internal MsgSubStorageWriter.WriterBuffer ValueBuffer
		{
			get
			{
				return this.valueBuffer;
			}
		}

		protected void CheckDisposed(string methodName)
		{
			if (this.isDisposed)
			{
				throw new ObjectDisposedException(methodName);
			}
			if (this.isFlushed)
			{
				throw new InvalidOperationException("Cannot write any data after Flush()");
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
					this.Flush();
				}
				catch (IOException exc)
				{
					if (this.parent != null)
					{
						this.parent.SetFailure(new MsgStorageException(MsgStorageErrorCode.FailedWrite, MsgStorageStrings.ComExceptionThrown, exc));
					}
				}
				catch (COMException exc2)
				{
					if (this.parent != null)
					{
						this.parent.SetFailure(new MsgStorageException(MsgStorageErrorCode.FailedWrite, MsgStorageStrings.ComExceptionThrown, exc2));
					}
				}
			}
		}

		private MsgStorageWriter parent;

		private ComStorage messageStorage;

		private MsgSubStorageWriter messageWriter;

		private MsgSubStorageWriter componentWriter;

		private NamedPropertyList namedPropertyList;

		private MsgSubStorageWriter.WriterBuffer lengthsBuffer;

		private MsgSubStorageWriter.WriterBuffer valueBuffer;

		private Exception lastFailure;

		private bool isTopLevelMessage;

		private bool isDisposed;

		private bool isFlushed;
	}
}
