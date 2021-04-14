using System;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.Mime.Encoders
{
	public class MacBinaryHeader
	{
		public MacBinaryHeader()
		{
			this.version = 130;
			this.minimumVersion = 129;
		}

		public MacBinaryHeader(byte[] bytes)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (bytes.Length != 128)
			{
				throw new ArgumentException(EncodersStrings.MacBinHeaderMustBe128Long, "bytes");
			}
			if (bytes[0] != 0 || bytes[74] != 0 || bytes[82] != 0)
			{
				throw new ByteEncoderException(EncodersStrings.MacBinInvalidData);
			}
			if (bytes[1] > 63)
			{
				throw new ByteEncoderException(EncodersStrings.MacBinInvalidData);
			}
			this.fileNameLength = (int)bytes[1];
			this.fileName = CTSGlobals.AsciiEncoding.GetString(bytes, 2, this.fileNameLength);
			this.fileType = BinHexUtils.UnmarshalInt32(bytes, 65);
			this.fileCreator = BinHexUtils.UnmarshalInt32(bytes, 69);
			this.finderFlags = (int)bytes[73];
			this.iconXOffset = (int)BinHexUtils.UnmarshalUInt16(bytes, 75);
			this.iconYOffset = (int)BinHexUtils.UnmarshalUInt16(bytes, 77);
			this.fileProtected = (1 == bytes[81]);
			this.dataForkLength = (long)BinHexUtils.UnmarshalInt32(bytes, 83);
			this.resourceForkLength = (long)BinHexUtils.UnmarshalInt32(bytes, 87);
			this.version = (int)bytes[122];
			this.minimumVersion = (int)bytes[123];
		}

		public int OldVersion
		{
			get
			{
				return 0;
			}
		}

		public int FileNameLength
		{
			get
			{
				return this.fileNameLength;
			}
		}

		public string FileName
		{
			get
			{
				return this.fileName;
			}
			set
			{
				byte[] bytes = CTSGlobals.AsciiEncoding.GetBytes(value);
				if (bytes.Length > 63)
				{
					throw new ArgumentException(EncodersStrings.MacBinFileNameTooLong, "value");
				}
				this.fileName = value;
				this.fileNameLength = bytes.Length;
			}
		}

		public int FileType
		{
			get
			{
				return this.fileType;
			}
			set
			{
				this.fileType = value;
			}
		}

		public int FileCreator
		{
			get
			{
				return this.fileCreator;
			}
			set
			{
				this.fileCreator = value;
			}
		}

		public int FinderFlags
		{
			get
			{
				return this.finderFlags;
			}
			set
			{
				this.finderFlags = value;
			}
		}

		public int XIcon
		{
			get
			{
				return this.iconXOffset;
			}
			set
			{
				if (65535 < value)
				{
					throw new ArgumentOutOfRangeException("value", EncodersStrings.MacBinIconOffsetTooLarge(65535));
				}
				this.iconXOffset = value;
			}
		}

		public int YIcon
		{
			get
			{
				return this.iconYOffset;
			}
			set
			{
				if (65535 < value)
				{
					throw new ArgumentOutOfRangeException("value", EncodersStrings.MacBinIconOffsetTooLarge(65535));
				}
				this.iconYOffset = value;
			}
		}

		public int FileId
		{
			get
			{
				return this.fileID;
			}
			set
			{
				this.fileID = value;
			}
		}

		public bool Protected
		{
			get
			{
				return this.fileProtected;
			}
			set
			{
				this.fileProtected = value;
			}
		}

		public long DataForkLength
		{
			get
			{
				return this.dataForkLength;
			}
			set
			{
				this.dataForkLength = value;
			}
		}

		public long ResourceForkLength
		{
			get
			{
				return this.resourceForkLength;
			}
			set
			{
				this.resourceForkLength = value;
			}
		}

		public DateTime CreationDate
		{
			get
			{
				return this.creationDate;
			}
			set
			{
				this.creationDate = value;
			}
		}

		public DateTime ModificationDate
		{
			get
			{
				return this.modificationDate;
			}
			set
			{
				this.modificationDate = value;
			}
		}

		public int GetInfoLength
		{
			get
			{
				return this.commentLength;
			}
			set
			{
				this.commentLength = value;
			}
		}

		public int UnpackedSize
		{
			get
			{
				return this.unpackedSize;
			}
			set
			{
				this.unpackedSize = value;
			}
		}

		public int SecondaryHeaderLength
		{
			get
			{
				return this.secondaryHeaderLength;
			}
			set
			{
				this.secondaryHeaderLength = value;
			}
		}

		public int Version
		{
			get
			{
				return this.version;
			}
			set
			{
				if (value != 0 && 129 != value && 130 != value)
				{
					throw new ArgumentOutOfRangeException("value", EncodersStrings.MacBinBadVersion);
				}
				this.version = value;
			}
		}

		public int MinimumVersion
		{
			get
			{
				return this.minimumVersion;
			}
			set
			{
				if (value != 0 && 129 != value && 130 != value)
				{
					throw new ArgumentOutOfRangeException("value", EncodersStrings.MacBinBadVersion);
				}
				this.minimumVersion = value;
			}
		}

		public short CheckSum
		{
			get
			{
				this.GetBytes();
				return (short)this.headerCRC;
			}
		}

		internal byte[] GetBytes()
		{
			int num = 0;
			byte[] array = new byte[128];
			array[num++] = 0;
			array[num++] = (byte)this.fileNameLength;
			byte[] src = this.FileNameAsByteArray();
			Buffer.BlockCopy(src, 0, array, num, this.fileNameLength);
			num = 65;
			num += BinHexUtils.MarshalInt32(array, num, (long)this.fileType);
			num += BinHexUtils.MarshalInt32(array, num, (long)this.fileCreator);
			array[num++] = (byte)((65280 & this.finderFlags) >> 8);
			array[num++] = 0;
			num += BinHexUtils.MarshalUInt16(array, num, (ushort)this.iconXOffset);
			num += BinHexUtils.MarshalUInt16(array, num, (ushort)this.iconYOffset);
			num += BinHexUtils.MarshalUInt16(array, num, 0);
			array[num++] = (this.fileProtected ? 1 : 0);
			array[num++] = 0;
			num += BinHexUtils.MarshalInt32(array, num, this.dataForkLength);
			num += BinHexUtils.MarshalInt32(array, num, this.resourceForkLength);
			num += BinHexUtils.MarshalInt32(array, num, 0L);
			num += BinHexUtils.MarshalInt32(array, num, 0L);
			num += BinHexUtils.MarshalUInt16(array, num, (ushort)this.commentLength);
			array[num++] = (byte)(255 & this.finderFlags);
			num += 18;
			num += BinHexUtils.MarshalUInt16(array, num, (ushort)this.secondaryHeaderLength);
			array[num++] = (byte)this.version;
			array[num++] = (byte)this.minimumVersion;
			this.headerCRC = BinHexUtils.CalculateHeaderCrc(array, 124);
			num += BinHexUtils.MarshalUInt16(array, num, this.headerCRC);
			array[num++] = 0;
			array[num++] = 0;
			return array;
		}

		private byte[] FileNameAsByteArray()
		{
			return CTSGlobals.AsciiEncoding.GetBytes(this.fileName);
		}

		private int fileNameLength;

		private string fileName;

		private int version;

		private int fileType;

		private int fileCreator;

		private int finderFlags;

		private long dataForkLength;

		private long resourceForkLength;

		private int minimumVersion;

		private int secondaryHeaderLength;

		private int unpackedSize;

		private int commentLength;

		private DateTime creationDate;

		private DateTime modificationDate;

		private bool fileProtected;

		private int iconXOffset;

		private int iconYOffset;

		private int fileID;

		private ushort headerCRC;

		[Flags]
		internal enum FinderFlagsFields
		{
			OnDesk = 1,
			Color = 14,
			ColorReserved = 16,
			SwitchLaunch = 32,
			Shared = 64,
			NoInits = 128,
			Initialized = 256,
			Reserved = 512,
			CustomIcon = 1024,
			Stationary = 2048,
			NameLocked = 4096,
			Bundle = 8192,
			Invisible = 16384,
			Alias = 32768
		}
	}
}
