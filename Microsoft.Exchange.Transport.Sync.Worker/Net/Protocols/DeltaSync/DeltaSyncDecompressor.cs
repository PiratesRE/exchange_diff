using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class DeltaSyncDecompressor
	{
		internal static bool TryDeCompress(Stream compressedData, Stream decompressedData)
		{
			DeltaSyncDecompressor.SclFileHeader sclFileHeader = default(DeltaSyncDecompressor.SclFileHeader);
			DeltaSyncDecompressor.SclBlockHeader sclBlockHeader = default(DeltaSyncDecompressor.SclBlockHeader);
			byte[] array = null;
			byte[] array2 = null;
			int num = 0;
			using (BinaryReader binaryReader = new BinaryReader(compressedData))
			{
				if (!sclFileHeader.TryLoad(binaryReader))
				{
					return false;
				}
				while (sclBlockHeader.TryLoad(binaryReader))
				{
					if (!DeltaSyncDecompressor.TryLoadBlock(binaryReader, (int)sclBlockHeader.CompSize, out array))
					{
						return false;
					}
					if (sclBlockHeader.CompSize >= sclBlockHeader.OrigSize)
					{
						array2 = array;
						num = array2.Length;
					}
					else
					{
						num = (int)sclBlockHeader.OrigSize;
						if (array2 == null || array2.Length < num)
						{
							array2 = new byte[num];
						}
						int num2 = DeltaSyncDecompressor.EcDecompressEx(array, array.Length, array2, ref num);
						if (num2 != 0)
						{
							return false;
						}
						if (sclBlockHeader.CRC32 != 0 && sclBlockHeader.CRC32 != DeltaSyncDecompressor.XpressCrc32(array2, num, 0))
						{
							return false;
						}
					}
					if (num != (int)sclBlockHeader.OrigSize)
					{
						return false;
					}
					decompressedData.Write(array2, 0, num);
					decompressedData.Flush();
				}
				if (decompressedData.Length != (long)((ulong)sclFileHeader.OrigSize))
				{
					return false;
				}
			}
			return true;
		}

		private static bool TryLoadBlock(BinaryReader reader, int size, out byte[] buffer)
		{
			buffer = null;
			try
			{
				buffer = reader.ReadBytes(size);
				if (buffer == null || buffer.Length != size)
				{
					return false;
				}
			}
			catch (EndOfStreamException)
			{
				return false;
			}
			return true;
		}

		[DllImport("huffman_xpress.dll")]
		private static extern int EcDecompressEx([MarshalAs(UnmanagedType.LPArray)] byte[] pbComp, int cbComp, [MarshalAs(UnmanagedType.LPArray)] byte[] pbOrig, ref int cbOrig);

		[DllImport("huffman_xpress.dll")]
		private static extern int XpressCrc32([MarshalAs(UnmanagedType.LPArray)] byte[] pbInput, int cbInput, int crc);

		private const int NoError = 0;

		private const int MinSclFileHeaderSize = 36;

		private const int MinSclBlockHeaderSize = 20;

		private const int GuidByteSize = 16;

		private const int PuidByteSize = 8;

		private static readonly char[] NoCompressionAlgoId = new char[]
		{
			'U',
			'N'
		};

		private static readonly char[] XpressHuffmanCompressionAlgoId = new char[]
		{
			'H',
			'U'
		};

		private static readonly char[] SupportedCompressionVersion = new char[]
		{
			'0',
			'1'
		};

		private static readonly char[] BlockSignature = new char[]
		{
			'S',
			'C',
			'B',
			'H'
		};

		private struct SclFileHeader
		{
			internal char[] Id
			{
				get
				{
					return this.id;
				}
			}

			internal char[] Version
			{
				get
				{
					return this.version;
				}
			}

			internal uint FileHeaderSize
			{
				get
				{
					return this.fileHeaderSize;
				}
			}

			internal Guid GuidMsgId
			{
				get
				{
					return this.guidMsgId;
				}
			}

			internal byte[] PuidUserId
			{
				get
				{
					return this.puidUserId;
				}
			}

			internal uint OrigSize
			{
				get
				{
					return this.origSize;
				}
			}

			internal byte[] RemainingBytes
			{
				get
				{
					return this.remainingBytes;
				}
			}

			private bool IsNotCompressed
			{
				get
				{
					return this.id != null && this.id.Length == 2 && this.id[0] == DeltaSyncDecompressor.NoCompressionAlgoId[0] && this.id[1] == DeltaSyncDecompressor.NoCompressionAlgoId[0];
				}
			}

			private bool IsXpressHuffmanCompressed
			{
				get
				{
					return this.id != null && this.id.Length == 2 && this.id[0] == DeltaSyncDecompressor.XpressHuffmanCompressionAlgoId[0] && this.id[1] == DeltaSyncDecompressor.XpressHuffmanCompressionAlgoId[1];
				}
			}

			private bool IsSupportedVersion
			{
				get
				{
					return this.version != null && this.version.Length == 2 && this.version[0] == DeltaSyncDecompressor.SupportedCompressionVersion[0] && this.version[1] == DeltaSyncDecompressor.SupportedCompressionVersion[1];
				}
			}

			private bool IsSupportedId
			{
				get
				{
					return this.IsNotCompressed || this.IsXpressHuffmanCompressed;
				}
			}

			internal bool TryLoad(BinaryReader reader)
			{
				try
				{
					this.id = reader.ReadChars(2);
					if (!this.IsSupportedId)
					{
						return false;
					}
					this.version = reader.ReadChars(2);
					if (!this.IsSupportedVersion)
					{
						return false;
					}
					this.fileHeaderSize = reader.ReadUInt32();
					if (this.fileHeaderSize < 36U)
					{
						return false;
					}
					byte[] array = reader.ReadBytes(16);
					if (array == null || array.Length != 16)
					{
						return false;
					}
					this.guidMsgId = new Guid(array);
					this.puidUserId = reader.ReadBytes(8);
					if (this.puidUserId == null || this.puidUserId.Length != 8)
					{
						return false;
					}
					this.origSize = reader.ReadUInt32();
					if (this.origSize <= 0U)
					{
						return false;
					}
					this.remainingBytes = reader.ReadBytes((int)(this.fileHeaderSize - 36U));
					if (this.remainingBytes == null || (long)this.remainingBytes.Length != (long)((ulong)(this.fileHeaderSize - 36U)))
					{
						return false;
					}
				}
				catch (EndOfStreamException)
				{
					return false;
				}
				return true;
			}

			private char[] id;

			private char[] version;

			private uint fileHeaderSize;

			private Guid guidMsgId;

			private byte[] puidUserId;

			private uint origSize;

			private byte[] remainingBytes;
		}

		private struct SclBlockHeader
		{
			internal char[] Signature
			{
				get
				{
					return this.signature;
				}
			}

			internal uint BlockHeaderSize
			{
				get
				{
					return this.blockHeaderSize;
				}
			}

			internal uint OrigSize
			{
				get
				{
					return this.origSize;
				}
			}

			internal int CRC32
			{
				get
				{
					return this.crc32;
				}
			}

			internal uint CompSize
			{
				get
				{
					return this.compSize;
				}
			}

			internal byte[] RemainingBytes
			{
				get
				{
					return this.remainingBytes;
				}
			}

			private bool IsValidSignature
			{
				get
				{
					return this.signature != null && this.signature.Length == 4 && (this.signature[0] == DeltaSyncDecompressor.BlockSignature[0] && this.signature[1] == DeltaSyncDecompressor.BlockSignature[1] && this.signature[2] == DeltaSyncDecompressor.BlockSignature[2]) && this.signature[3] == DeltaSyncDecompressor.BlockSignature[3];
				}
			}

			internal bool TryLoad(BinaryReader reader)
			{
				try
				{
					this.signature = reader.ReadChars(4);
					if (!this.IsValidSignature)
					{
						return false;
					}
					this.blockHeaderSize = reader.ReadUInt32();
					if (this.blockHeaderSize < 20U)
					{
						return false;
					}
					this.origSize = reader.ReadUInt32();
					if (this.origSize <= 0U)
					{
						return false;
					}
					this.crc32 = reader.ReadInt32();
					this.compSize = reader.ReadUInt32();
					if (this.compSize <= 0U)
					{
						return false;
					}
					this.remainingBytes = reader.ReadBytes((int)(this.blockHeaderSize - 20U));
					if (this.remainingBytes == null || (long)this.remainingBytes.Length != (long)((ulong)(this.blockHeaderSize - 20U)))
					{
						return false;
					}
				}
				catch (EndOfStreamException)
				{
					return false;
				}
				return true;
			}

			private char[] signature;

			private uint blockHeaderSize;

			private uint origSize;

			private int crc32;

			private uint compSize;

			private byte[] remainingBytes;
		}
	}
}
