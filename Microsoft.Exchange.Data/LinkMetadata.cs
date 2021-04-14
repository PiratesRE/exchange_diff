using System;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.Data
{
	internal sealed class LinkMetadata
	{
		private LinkMetadata()
		{
		}

		public string AttributeName { get; private set; }

		public string TargetDistinguishedName { get; private set; }

		public DateTime CreationTime { get; private set; }

		public DateTime DeletionTime { get; private set; }

		public int Version { get; private set; }

		public DateTime LastWriteTime { get; private set; }

		public Guid OriginatingInvocationId { get; private set; }

		public long OriginatingUpdateSequenceNumber { get; private set; }

		public long LocalUpdateSequenceNumber { get; private set; }

		public byte[] Data { get; private set; }

		public bool IsDeleted
		{
			get
			{
				return this.DeletionTime != DateTime.MinValue;
			}
		}

		public static LinkMetadata Parse(byte[] binary)
		{
			if (binary == null)
			{
				throw new ArgumentNullException("binary");
			}
			Exception innerException;
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(binary))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.Unicode))
					{
						LinkMetadata linkMetadata = new LinkMetadata();
						int num = binaryReader.ReadInt32();
						int num2 = binaryReader.ReadInt32();
						int count = binaryReader.ReadInt32();
						int num3 = binaryReader.ReadInt32();
						linkMetadata.DeletionTime = AttributeMetadata.ReadFileTimeUtc(binaryReader);
						linkMetadata.CreationTime = AttributeMetadata.ReadFileTimeUtc(binaryReader);
						linkMetadata.Version = binaryReader.ReadInt32();
						linkMetadata.LastWriteTime = AttributeMetadata.ReadFileTimeUtc(binaryReader);
						linkMetadata.OriginatingInvocationId = new Guid(binaryReader.ReadBytes(16));
						binaryReader.ReadInt32();
						linkMetadata.OriginatingUpdateSequenceNumber = binaryReader.ReadInt64();
						linkMetadata.LocalUpdateSequenceNumber = binaryReader.ReadInt64();
						memoryStream.Seek((long)num, SeekOrigin.Begin);
						linkMetadata.AttributeName = AttributeMetadata.ReadNullTerminatedString(binaryReader);
						memoryStream.Seek((long)num2, SeekOrigin.Begin);
						linkMetadata.TargetDistinguishedName = AttributeMetadata.ReadNullTerminatedString(binaryReader);
						memoryStream.Seek((long)num3, SeekOrigin.Begin);
						linkMetadata.Data = binaryReader.ReadBytes(count);
						return linkMetadata;
					}
				}
			}
			catch (ArgumentException ex)
			{
				innerException = ex;
			}
			catch (IOException ex2)
			{
				innerException = ex2;
			}
			throw new FormatException(DataStrings.InvalidFormat, innerException);
		}
	}
}
