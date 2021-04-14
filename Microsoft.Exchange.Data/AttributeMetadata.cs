using System;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.Data
{
	public class AttributeMetadata
	{
		internal AttributeMetadata()
		{
		}

		public string AttributeName { get; private set; }

		public int Version { get; private set; }

		public DateTime LastWriteTime { get; private set; }

		public Guid OriginatingInvocationId { get; private set; }

		public long OriginatingUpdateSequenceNumber { get; private set; }

		public long LocalUpdateSequenceNumber { get; private set; }

		public static AttributeMetadata Parse(byte[] binary)
		{
			if (binary == null)
			{
				throw new ArgumentNullException("binary");
			}
			Exception innerException = null;
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(binary))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.Unicode))
					{
						AttributeMetadata attributeMetadata = new AttributeMetadata();
						int num = binaryReader.ReadInt32();
						attributeMetadata.Version = binaryReader.ReadInt32();
						attributeMetadata.LastWriteTime = AttributeMetadata.ReadFileTimeUtc(binaryReader);
						attributeMetadata.OriginatingInvocationId = new Guid(binaryReader.ReadBytes(16));
						attributeMetadata.OriginatingUpdateSequenceNumber = binaryReader.ReadInt64();
						attributeMetadata.LocalUpdateSequenceNumber = binaryReader.ReadInt64();
						memoryStream.Seek((long)num, SeekOrigin.Begin);
						attributeMetadata.AttributeName = AttributeMetadata.ReadNullTerminatedString(binaryReader);
						return attributeMetadata;
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

		internal static DateTime ReadFileTimeUtc(BinaryReader reader)
		{
			long num = reader.ReadInt64();
			long num2 = num;
			if (num2 <= 0L && num2 >= -1L)
			{
				switch ((int)(num2 - -1L))
				{
				case 0:
					return DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc);
				case 1:
					return DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
				}
			}
			return DateTime.FromFileTimeUtc(num);
		}

		internal static string ReadNullTerminatedString(BinaryReader reader)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (;;)
			{
				char c = reader.ReadChar();
				if (c == '\0')
				{
					break;
				}
				stringBuilder.Append(c);
			}
			return stringBuilder.ToString();
		}
	}
}
