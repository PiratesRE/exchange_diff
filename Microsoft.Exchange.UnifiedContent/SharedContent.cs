using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Security.Cryptography;

namespace Microsoft.Exchange.UnifiedContent
{
	internal class SharedContent : IExtractedContent, IRawContent
	{
		private SharedContent(Stream sharedStream)
		{
			this.NeedsUpdate = true;
			this.sharedStream = sharedStream;
		}

		private SharedContent(Stream sharedStream, Stream dataStream)
		{
			this.NeedsUpdate = true;
			this.sharedStream = sharedStream;
			long value = 12L + SharedContentWriter.ComputeLength(dataStream);
			this.rawDataEntryPosition = sharedStream.Length;
			sharedStream.Position = this.rawDataEntryPosition;
			using (SharedContentWriter sharedContentWriter = new SharedContentWriter(sharedStream))
			{
				sharedContentWriter.Write(value);
				sharedContentWriter.Write(286331153U);
				sharedContentWriter.Write(0UL);
				this.rawDataPosition = sharedStream.Position + 8L;
				sharedContentWriter.Write(dataStream);
				this.rawSize = sharedStream.Position - this.rawDataPosition;
				this.ComputeHashes(this.rawDataPosition);
				sharedContentWriter.ValidateAtEndOfEntry();
			}
		}

		public string FileName
		{
			get
			{
				this.Update();
				return this.fileName;
			}
			set
			{
				this.fileName = value;
			}
		}

		public long Rawsize
		{
			get
			{
				return this.rawSize;
			}
		}

		public string RawFileName
		{
			get
			{
				return this.rawFileName;
			}
		}

		public bool NeedsUpdate { get; set; }

		public TextExtractionStatus TextExtractionStatus
		{
			get
			{
				this.Update();
				return this.textExtractionStatus;
			}
		}

		public int RefId
		{
			get
			{
				this.Update();
				return this.refId;
			}
		}

		public Dictionary<string, object> Properties
		{
			get
			{
				return this.properties;
			}
		}

		public byte[] LowFidelityHash
		{
			get
			{
				return this.lowFidelitySHA256Hash;
			}
		}

		public byte[] HighFidelityHash
		{
			get
			{
				if (this.highFidelitySHA256Hash == null)
				{
					this.highFidelitySHA256Hash = this.CalculateHighFidelityHash();
				}
				return this.highFidelitySHA256Hash;
			}
		}

		public long StreamOffset
		{
			get
			{
				return this.rawDataPosition + 24L;
			}
		}

		internal long EntryPosition
		{
			get
			{
				return this.entryPosition;
			}
		}

		internal long RawDataPosition
		{
			get
			{
				return this.rawDataPosition;
			}
		}

		internal long RawDataEntryPosition
		{
			get
			{
				return this.rawDataEntryPosition;
			}
		}

		internal uint Crc32Hash
		{
			get
			{
				return this.crc32Hash;
			}
			set
			{
				this.crc32Hash = value;
			}
		}

		public Stream GetContentReadStream()
		{
			this.Update();
			if (this.textStream != null)
			{
				this.textStream.Position = 0L;
			}
			return this.textStream;
		}

		public bool IsModified(Stream rawStream)
		{
			uint hash = Crc32.ComputeHash(rawStream, 0L);
			return this.IsModified(hash);
		}

		public bool IsModified(uint hash)
		{
			return hash != this.crc32Hash;
		}

		public IList<IExtractedContent> GetChildren()
		{
			this.Update();
			if (this.children == null && !this.NeedsUpdate)
			{
				this.children = new List<IExtractedContent>();
				if (this.firstChildEntryPos != 0L)
				{
					SharedContent sharedContent = SharedContent.Open(this.sharedStream, this.firstChildEntryPos, this);
					this.children.Add(sharedContent);
					while (sharedContent.nextSiblingEntryPos != 0L)
					{
						sharedContent = SharedContent.Open(this.sharedStream, sharedContent.nextSiblingEntryPos, this);
						this.children.Add(sharedContent);
					}
				}
			}
			return this.children;
		}

		internal static SharedContent Create(SharedStream sharedStream, Stream contentStream, string contentName = null)
		{
			return new SharedContent(sharedStream, contentStream)
			{
				rawFileName = contentName
			};
		}

		internal static SharedContent Open(Stream sharedStream, long entryPosition, SharedContent parent)
		{
			SharedContent sharedContent = new SharedContent(sharedStream);
			sharedContent.entryPosition = entryPosition;
			sharedContent.parent = parent;
			sharedContent.Update();
			return sharedContent;
		}

		internal static Stream OpenEntryStream(Stream sharedStream, long entryPosition)
		{
			long length = new BinaryReader(sharedStream)
			{
				BaseStream = 
				{
					Position = entryPosition
				}
			}.ReadInt64();
			return new StreamOnStream(sharedStream, entryPosition + 8L, length);
		}

		private void Update()
		{
			if (this.NeedsUpdate)
			{
				if (this.entryPosition == 0L)
				{
					RawDataEntry rawDataEntry = new RawDataEntry(SharedContent.OpenEntryStream(this.sharedStream, this.rawDataEntryPosition), this.rawDataEntryPosition);
					this.entryPosition = rawDataEntry.ExtractedContentEntryPosition;
				}
				if (this.entryPosition != 0L)
				{
					this.NeedsUpdate = false;
					ExtractedContentEntry extractedContentEntry = new ExtractedContentEntry(SharedContent.OpenEntryStream(this.sharedStream, this.entryPosition), this.entryPosition);
					this.parentEntryPos = extractedContentEntry.ParentPos;
					if (this.parent == null)
					{
						if (extractedContentEntry.ParentPos != 0L)
						{
							throw new InvalidDataException();
						}
					}
					else if (this.parent.entryPosition != this.parentEntryPos)
					{
						throw new InvalidDataException();
					}
					this.firstChildEntryPos = extractedContentEntry.FirstChildPos;
					this.nextSiblingEntryPos = extractedContentEntry.NextSiblingPos;
					this.fileName = extractedContentEntry.FileName;
					this.textExtractionStatus = (TextExtractionStatus)extractedContentEntry.TextExtractionStatus;
					this.textStream = extractedContentEntry.TextStream;
					this.refId = (int)extractedContentEntry.RefId;
					foreach (KeyValuePair<string, object> keyValuePair in extractedContentEntry.Properties)
					{
						this.properties[keyValuePair.Key] = keyValuePair.Value;
					}
				}
			}
		}

		private void ComputeHashes(long start = 0L)
		{
			byte[] array = new byte[16384];
			long position = -1L;
			if (this.sharedStream.CanSeek)
			{
				position = this.sharedStream.Position;
				this.sharedStream.Position = start;
			}
			int num;
			if ((num = this.sharedStream.Read(array, 0, 16384)) > 0)
			{
				this.crc32Hash = Crc32.ComputeHash(array, num);
				using (SHA256CryptoServiceProvider sha256CryptoServiceProvider = new SHA256CryptoServiceProvider())
				{
					this.lowFidelitySHA256Hash = sha256CryptoServiceProvider.ComputeHash(array, 0, Math.Min(num, 4096));
				}
			}
			if (this.sharedStream.CanSeek)
			{
				this.sharedStream.Position = position;
			}
		}

		private byte[] CalculateHighFidelityHash()
		{
			if (this.rawSize <= 4096L)
			{
				return this.lowFidelitySHA256Hash;
			}
			RawDataEntry rawDataEntry = new RawDataEntry(SharedContent.OpenEntryStream(this.sharedStream, this.rawDataEntryPosition), this.rawDataEntryPosition);
			byte[] result;
			using (SHA256CryptoServiceProvider sha256CryptoServiceProvider = new SHA256CryptoServiceProvider())
			{
				result = sha256CryptoServiceProvider.ComputeHash(rawDataEntry.DataStream);
			}
			return result;
		}

		private const uint EntryId = 286331153U;

		private const int MaxBytesToCalculateCrc32Hash = 16384;

		private const int MaxBytesToCalculateLowFidelitySHA256Hash = 4096;

		private const int MaxBufferLengthToCalculateHash = 16384;

		private readonly long rawDataEntryPosition;

		private readonly long rawDataPosition;

		private readonly long rawSize;

		private readonly Stream sharedStream;

		private readonly Dictionary<string, object> properties = new Dictionary<string, object>();

		private long entryPosition;

		private Stream textStream;

		private string fileName;

		private IList<IExtractedContent> children;

		private SharedContent parent;

		private long parentEntryPos;

		private long nextSiblingEntryPos;

		private long firstChildEntryPos;

		private TextExtractionStatus textExtractionStatus;

		private string rawFileName;

		private uint crc32Hash;

		private byte[] lowFidelitySHA256Hash;

		private byte[] highFidelitySHA256Hash;

		private int refId = -1;
	}
}
