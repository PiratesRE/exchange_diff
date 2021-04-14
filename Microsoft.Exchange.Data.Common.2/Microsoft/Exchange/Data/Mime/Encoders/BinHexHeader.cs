using System;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.Mime.Encoders
{
	internal class BinHexHeader
	{
		public BinHexHeader()
		{
		}

		public BinHexHeader(byte[] header)
		{
			if (header.Length < 23)
			{
				throw new ArgumentException(EncodersStrings.BinHexHeaderTooSmall, "header");
			}
			int num = (int)header[0];
			if (header.Length - 22 < num)
			{
				throw new ByteEncoderException(EncodersStrings.BinHexHeaderIncomplete);
			}
			if (63 < num || 1 > num)
			{
				throw new ByteEncoderException(EncodersStrings.BinHexHeaderInvalidNameLength);
			}
			int num2 = 2 + num;
			ushort num3 = BinHexUtils.CalculateHeaderCrc(header, num2 + 18);
			ushort num4 = BinHexUtils.UnmarshalUInt16(header, num2 + 18);
			if (num3 != num4)
			{
				throw new ByteEncoderException(EncodersStrings.BinHexHeaderInvalidCrc);
			}
			if (header[1 + num] != 0)
			{
				throw new ByteEncoderException(EncodersStrings.BinHexHeaderUnsupportedVersion);
			}
			this.fileNameLength = num;
			this.fileName = new byte[num];
			Buffer.BlockCopy(header, 1, this.fileName, 0, this.fileNameLength);
			this.version = (int)header[this.fileNameLength + 1];
			this.fileType = BinHexUtils.UnmarshalInt32(header, num2);
			this.fileCreator = BinHexUtils.UnmarshalInt32(header, num2 + 4);
			this.finderFlags = (int)BinHexUtils.UnmarshalUInt16(header, num2 + 8);
			this.dataForkLength = (long)BinHexUtils.UnmarshalInt32(header, num2 + 10);
			this.resourceForkLength = (long)BinHexUtils.UnmarshalInt32(header, num2 + 14);
			this.headerCRC = num4;
		}

		public BinHexHeader(MacBinaryHeader header)
		{
			if (63 < header.FileNameLength || 1 > header.FileNameLength)
			{
				throw new ByteEncoderException(EncodersStrings.BinHexHeaderBadFileNameLength);
			}
			this.FileName = header.FileName;
			this.version = 0;
			this.fileType = header.FileType;
			this.fileCreator = header.FileCreator;
			this.finderFlags = 0;
			this.dataForkLength = header.DataForkLength;
			this.resourceForkLength = header.ResourceForkLength;
			this.GetBytes();
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
				return CTSGlobals.AsciiEncoding.GetString(this.fileName, 0, this.fileNameLength);
			}
			set
			{
				byte[] bytes = CTSGlobals.AsciiEncoding.GetBytes(value);
				if (63 < bytes.Length || 1 > bytes.Length)
				{
					throw new ByteEncoderException(EncodersStrings.BinHexHeaderBadFileNameLength);
				}
				this.fileName = bytes;
				this.fileNameLength = bytes.Length;
			}
		}

		public long DataForkLength
		{
			get
			{
				return this.dataForkLength;
			}
		}

		public long ResourceForkLength
		{
			get
			{
				return this.resourceForkLength;
			}
		}

		public static implicit operator MacBinaryHeader(BinHexHeader rhs)
		{
			return new MacBinaryHeader
			{
				FileName = rhs.FileName,
				FileType = rhs.fileType,
				FileCreator = rhs.fileCreator,
				FinderFlags = rhs.finderFlags,
				DataForkLength = rhs.dataForkLength,
				ResourceForkLength = rhs.resourceForkLength
			};
		}

		public byte[] GetBytes()
		{
			int num = 0;
			int num2 = 1 + this.FileNameLength + 1 + 4 + 4 + 2 + 4 + 4 + 2;
			byte[] array = new byte[num2];
			array[num++] = (byte)this.fileNameLength;
			Buffer.BlockCopy(this.fileName, 0, array, num, this.fileNameLength);
			num += this.FileNameLength;
			array[num++] = (byte)this.version;
			num += BinHexUtils.MarshalInt32(array, num, (long)this.fileType);
			num += BinHexUtils.MarshalInt32(array, num, (long)this.fileCreator);
			num += BinHexUtils.MarshalUInt16(array, num, (ushort)this.finderFlags);
			num += BinHexUtils.MarshalInt32(array, num, this.dataForkLength);
			num += BinHexUtils.MarshalInt32(array, num, this.resourceForkLength);
			this.headerCRC = BinHexUtils.CalculateHeaderCrc(array, num);
			num += BinHexUtils.MarshalUInt16(array, num, this.headerCRC);
			return array;
		}

		public BinHexHeader Clone()
		{
			BinHexHeader binHexHeader = base.MemberwiseClone() as BinHexHeader;
			binHexHeader.fileName = (this.fileName.Clone() as byte[]);
			return binHexHeader;
		}

		private int fileNameLength;

		private byte[] fileName;

		private int version;

		private int fileType;

		private int fileCreator;

		private int finderFlags;

		private long dataForkLength;

		private long resourceForkLength;

		private ushort headerCRC;
	}
}
