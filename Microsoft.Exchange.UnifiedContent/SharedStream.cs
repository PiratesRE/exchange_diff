using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;

namespace Microsoft.Exchange.UnifiedContent
{
	public class SharedStream : Stream
	{
		private SharedStream(string sPath, long lCapacity, FileSecurity fileSecurity)
		{
			this.Path = sPath;
			this.lCapacity = lCapacity;
			this.fileSecurity = fileSecurity;
			this.lSize = 0L;
			this.fOverflow = false;
			this.DeleteFileOnDispose = true;
			MemoryMappedFileSecurity memoryMappedFileSecurity = new MemoryMappedFileSecurity();
			memoryMappedFileSecurity.SetSecurityDescriptorBinaryForm(SharedStream.BuildSecurityDescriptor(SharedStream.defaultAllowedList).GetSecurityDescriptorBinaryForm());
			this.Name = Guid.NewGuid().ToString();
			this.mapping = MemoryMappedFile.CreateNew(this.Name, this.lCapacity, MemoryMappedFileAccess.ReadWrite, MemoryMappedFileOptions.None, memoryMappedFileSecurity, HandleInheritability.None);
			this.streamMapping = this.mapping.CreateViewStream();
			this.WriteHeader();
			this.streamCurrent = this.streamMapping;
			this.streamCurrent.Position = 24L;
		}

		private SharedStream(string sPath, string sName, FileSecurity fileSecurity)
		{
			this.Path = sPath;
			this.fileSecurity = fileSecurity;
			this.lSize = 0L;
			this.DeleteFileOnDispose = false;
			this.Name = sName;
			this.mapping = MemoryMappedFile.OpenExisting(sName);
			this.streamMapping = this.mapping.CreateViewStream();
			this.streamCurrent = this.streamMapping;
			this.CheckForOverflow();
			this.streamCurrent.Position = (long)this.iHeaderSize;
		}

		~SharedStream()
		{
			this.Dispose(false);
		}

		public string Name { get; private set; }

		public string Path { get; private set; }

		public string SharedName
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder(this.Path);
				stringBuilder.Append("|");
				stringBuilder.Append(this.Name);
				stringBuilder.Append("|");
				stringBuilder.Append(SharedStream.sessionId);
				return stringBuilder.ToString();
			}
		}

		public string SharedFullName
		{
			get
			{
				this.CheckForOverflow();
				if (this.fOverflow)
				{
					return System.IO.Path.Combine(this.Path, this.Name);
				}
				return string.Format("Session\\{0}\\{1}", SharedStream.sessionId, this.Name);
			}
		}

		public long Capacity
		{
			get
			{
				this.CheckForOverflow();
				return this.lCapacity;
			}
		}

		public bool Overflowed
		{
			get
			{
				long position = this.streamMapping.Position;
				BinaryReader binaryReader = new BinaryReader(this.streamMapping);
				this.streamMapping.Position = 1L;
				byte b = binaryReader.ReadByte();
				this.streamMapping.Position = position;
				return 0 != b;
			}
		}

		public bool DeleteFileOnDispose { get; set; }

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		public override long Length
		{
			get
			{
				return this.ReadSize();
			}
		}

		public override long Position
		{
			get
			{
				this.CheckForOverflow();
				return this.streamCurrent.Position - (long)this.iHeaderSize;
			}
			set
			{
				this.CheckForOverflow();
				if (value < 0L)
				{
					throw new IOException("New position is before beginning of file.");
				}
				this.streamCurrent.Position = value + (long)this.iHeaderSize;
			}
		}

		public static SharedStream Create(string sPath, long lCapacity = 1048576L, FileSecurity fileSecurity = null)
		{
			return new SharedStream(sPath, lCapacity, fileSecurity);
		}

		public static SharedStream Open(string sPath, string sName, FileSecurity fileSecurity = null)
		{
			return new SharedStream(sPath, sName, fileSecurity);
		}

		public static SharedStream Open(string sSharedName, FileSecurity fileSecurity = null)
		{
			string[] array = sSharedName.Split(new char[]
			{
				'|'
			});
			if (array.Length != 3)
			{
				throw new ArgumentException("SharedStream: SharedName is invalid.");
			}
			return new SharedStream(array[0], array[1], fileSecurity);
		}

		public override void Flush()
		{
			if (this.file != null)
			{
				this.file.Flush();
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			this.CheckForOverflow();
			if (this.streamCurrent == null)
			{
				throw new InvalidOperationException("Stream is null.");
			}
			return this.streamCurrent.Read(buffer, offset, (int)Math.Min((long)count, this.lSize - (this.streamCurrent.Position - (long)this.iHeaderSize)));
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			this.CheckForOverflow();
			switch (origin)
			{
			case SeekOrigin.Begin:
				this.Position = offset;
				break;
			case SeekOrigin.Current:
				this.Position += offset;
				break;
			case SeekOrigin.End:
				this.Position = this.lSize - offset;
				break;
			default:
				throw new ArgumentException("Invalid seek origin");
			}
			return this.Position;
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.CheckForOverflow();
			if (this.streamCurrent.Position + (long)count > this.lCapacity)
			{
				if (this.file == null)
				{
					this.fOverflow = true;
					this.WriteHeader();
					if (this.fileSecurity == null)
					{
						this.file = new FileStream(System.IO.Path.Combine(this.Path, this.Name), FileMode.Create, FileAccess.ReadWrite, FileShare.Read | FileShare.Write | FileShare.Delete);
					}
					else
					{
						this.file = new FileStream(System.IO.Path.Combine(this.Path, this.Name), FileMode.Create, FileSystemRights.Modify, FileShare.Read | FileShare.Write | FileShare.Delete, 8192, FileOptions.RandomAccess, this.fileSecurity);
					}
					long position = this.streamMapping.Position;
					this.streamMapping.Position = 0L;
					this.streamMapping.CopyTo(this.file);
					this.file.Position = position;
					this.lCapacity = long.MaxValue;
					this.WriteHeader();
				}
				this.streamCurrent = this.file;
			}
			if (this.Position + (long)count > this.lSize)
			{
				this.UpdateSize(this.Position + (long)count);
			}
			this.streamCurrent.Write(buffer, offset, count);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (this.fDisposed || !disposing)
			{
				return;
			}
			if (this.Overflowed && this.DeleteFileOnDispose)
			{
				FileStream fileStream = new FileStream(System.IO.Path.Combine(this.Path, this.Name), FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read | FileShare.Write | FileShare.Delete, 4096, FileOptions.DeleteOnClose);
				fileStream.Dispose();
			}
			if (this.streamMapping != null)
			{
				this.streamMapping.Dispose();
				this.streamMapping = null;
			}
			if (this.mapping != null)
			{
				this.mapping.Dispose();
				this.mapping = null;
			}
			if (this.file != null)
			{
				this.file.Dispose();
				this.file = null;
			}
			this.fDisposed = true;
		}

		private static FileSecurity BuildSecurityDescriptor(IEnumerable<WellKnownSidType> allowedList)
		{
			FileSecurity fileSecurity = new FileSecurity();
			foreach (WellKnownSidType sidType in allowedList)
			{
				SecurityIdentifier identity = new SecurityIdentifier(sidType, null);
				fileSecurity.AddAccessRule(new FileSystemAccessRule(identity, FileSystemRights.FullControl, AccessControlType.Allow));
			}
			return fileSecurity;
		}

		private void CheckForOverflow()
		{
			if (this.fOverflow)
			{
				return;
			}
			long position = this.streamCurrent.Position;
			BinaryReader binaryReader = new BinaryReader(this.streamMapping);
			long position2 = this.streamMapping.Position;
			this.streamMapping.Position = 0L;
			this.iHeaderSize = (int)binaryReader.ReadByte();
			if (this.iHeaderSize != 24)
			{
				throw new ArgumentException("Invalid header size.");
			}
			if (binaryReader.ReadByte() != 0)
			{
				this.file = File.Open(System.IO.Path.Combine(this.Path, this.Name), FileMode.Open, FileAccess.ReadWrite, FileShare.Read | FileShare.Write | FileShare.Delete);
				this.file.Position = Math.Max(this.file.Length, 24L);
				this.streamCurrent = this.file;
				this.fOverflow = true;
			}
			else
			{
				this.fOverflow = false;
			}
			this.streamMapping.Position = 8L;
			this.lSize = (long)binaryReader.ReadUInt64();
			this.streamMapping.Position = 16L;
			this.lCapacity = (long)binaryReader.ReadUInt64();
			this.streamMapping.Position = position2;
			this.streamCurrent.Position = position;
		}

		private void WriteHeader()
		{
			long position = this.streamMapping.Position;
			this.streamMapping.Position = 0L;
			this.streamMapping.WriteByte((byte)this.iHeaderSize);
			this.streamMapping.WriteByte(this.fOverflow ? 1 : 0);
			BinaryWriter binaryWriter = new BinaryWriter(this.streamMapping);
			this.streamMapping.Position = 8L;
			binaryWriter.Write(this.lSize);
			this.streamMapping.Position = 16L;
			binaryWriter.Write(this.lCapacity);
			this.streamMapping.Position = position;
		}

		private long ReadSize()
		{
			long position = this.streamMapping.Position;
			BinaryReader binaryReader = new BinaryReader(this.streamMapping);
			this.streamMapping.Position = 8L;
			this.lSize = (long)binaryReader.ReadUInt64();
			this.streamMapping.Position = position;
			return this.lSize;
		}

		private void UpdateSize(long size)
		{
			this.lSize = size;
			long position = this.streamMapping.Position;
			this.streamMapping.Position = 8L;
			BinaryWriter binaryWriter = new BinaryWriter(this.streamMapping);
			binaryWriter.Write(this.lSize);
			this.streamMapping.Position = position;
		}

		public const int HeaderSize = 24;

		private const int DefaultCapacity = 1048576;

		private const int HeaderContentOverflowOffset = 1;

		private const int HeaderContentSizeOffset = 8;

		private const int HeaderCapacityOffset = 16;

		private static readonly int sessionId = Process.GetCurrentProcess().SessionId;

		private static readonly WellKnownSidType[] defaultAllowedList = new WellKnownSidType[]
		{
			WellKnownSidType.WorldSid,
			WellKnownSidType.SelfSid,
			WellKnownSidType.NetworkServiceSid,
			WellKnownSidType.BuiltinAdministratorsSid,
			WellKnownSidType.LocalServiceSid
		};

		private readonly FileSecurity fileSecurity;

		private MemoryMappedFile mapping;

		private MemoryMappedViewStream streamMapping;

		private FileStream file;

		private long lCapacity;

		private long lSize;

		private int iHeaderSize = 24;

		private bool fOverflow;

		private bool fDisposed;

		private Stream streamCurrent;
	}
}
