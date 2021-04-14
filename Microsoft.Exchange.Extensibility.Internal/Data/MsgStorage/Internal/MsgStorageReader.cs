using System;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.Data.MsgStorage.Internal
{
	public class MsgStorageReader : IDisposable
	{
		internal MsgStorageReader(ComStorage storage, NamedPropertyList namedPropertyList, Encoding parentEncoding)
		{
			this.isTopLevelMessage = false;
			this.messageStorage = storage;
			this.namedPropertyList = namedPropertyList;
			this.Initialize(parentEncoding);
		}

		public MsgStorageReader(string filename)
		{
			this.isTopLevelMessage = true;
			this.messageStorage = ComStorage.OpenFileStorage(filename, ComStorage.OpenMode.Read);
			this.Initialize(null);
		}

		public MsgStorageReader(Stream contentStream)
		{
			this.isTopLevelMessage = true;
			this.messageStorage = ComStorage.OpenStorageOnStream(contentStream, ComStorage.OpenMode.Read);
			this.Initialize(null);
		}

		private void Initialize(Encoding messageEncoding)
		{
			this.readBuffers = default(MsgSubStorageReader.ReaderBuffers);
			this.reader = new MsgStoragePropertyReader(this);
			MsgSubStorageType subStorageType;
			if (this.isTopLevelMessage)
			{
				subStorageType = MsgSubStorageType.TopLevelMessage;
				this.namedPropertyList = NamedPropertyList.ReadNamedPropertyList(this.messageStorage);
			}
			else
			{
				subStorageType = MsgSubStorageType.AttachedMessage;
			}
			this.subStorageParser = new MsgSubStorageReader(this, this.messageStorage, messageEncoding, subStorageType);
			this.attachmentCount = this.subStorageParser.AttachmentCount;
			this.recipientCount = this.subStorageParser.RecipientCount;
			this.encoding = this.subStorageParser.MessageEncoding;
		}

		public int AttachmentCount
		{
			get
			{
				this.CheckDisposed("MsgStorageReader::get_AttachmentCount");
				return this.attachmentCount;
			}
		}

		public int RecipientCount
		{
			get
			{
				this.CheckDisposed("MsgStorageReader::get_RecipientCount");
				return this.recipientCount;
			}
		}

		public MsgStoragePropertyReader PropertyReader
		{
			get
			{
				this.CheckDisposed("MsgStorageReader::get_PropertyReader");
				return this.reader;
			}
		}

		internal MsgSubStorageReader SubStorageReader
		{
			get
			{
				return this.subStorageParser;
			}
		}

		internal NamedPropertyList NamedPropertyList
		{
			get
			{
				return this.namedPropertyList;
			}
		}

		internal MsgSubStorageReader.ReaderBuffers Buffers
		{
			get
			{
				return this.readBuffers;
			}
		}

		public void OpenAttachment(int attachmentIndex)
		{
			this.CheckDisposed("MsgStorageReader::OpenAttachment");
			if (attachmentIndex < 0 || attachmentIndex >= this.AttachmentCount)
			{
				throw new ArgumentOutOfRangeException("attachmentIndex");
			}
			this.OpenSubStorage(Util.AttachmentStorageName(attachmentIndex), MsgSubStorageType.Attachment);
		}

		public void OpenRecipient(int recipientIndex)
		{
			this.CheckDisposed("MsgStorageReader::OpenRecipient");
			if (recipientIndex < 0 || recipientIndex >= this.RecipientCount)
			{
				throw new ArgumentOutOfRangeException("recipientIndex");
			}
			this.OpenSubStorage(Util.RecipientStorageName(recipientIndex), MsgSubStorageType.Recipient);
		}

		private void OpenSubStorage(string subStorageName, MsgSubStorageType type)
		{
			ComStorage comStorage = null;
			try
			{
				comStorage = this.messageStorage.OpenStorage(subStorageName, ComStorage.OpenMode.Read);
				MsgSubStorageReader msgSubStorageReader = new MsgSubStorageReader(this, comStorage, this.encoding, type);
				if (this.subStorage != null)
				{
					this.subStorage.Dispose();
				}
				this.subStorage = comStorage;
				this.subStorageParser = msgSubStorageReader;
				comStorage = null;
			}
			finally
			{
				if (comStorage != null)
				{
					comStorage.Dispose();
				}
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.subStorage != null)
				{
					this.subStorage.Dispose();
					this.subStorage = null;
				}
				if (this.messageStorage != null)
				{
					this.messageStorage.Dispose();
					this.messageStorage = null;
				}
				GC.SuppressFinalize(this);
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

		private ComStorage messageStorage;

		private ComStorage subStorage;

		private MsgStoragePropertyReader reader;

		private MsgSubStorageReader subStorageParser;

		private MsgSubStorageReader.ReaderBuffers readBuffers;

		private NamedPropertyList namedPropertyList;

		private Encoding encoding;

		private int recipientCount;

		private int attachmentCount;

		private bool isDisposed;

		private bool isTopLevelMessage;
	}
}
