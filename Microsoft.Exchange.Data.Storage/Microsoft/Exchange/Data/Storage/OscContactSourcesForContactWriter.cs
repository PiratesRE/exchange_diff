using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OscContactSourcesForContactWriter
	{
		private OscContactSourcesForContactWriter()
		{
		}

		public byte[] Write(Guid provider, string networkId, string userId)
		{
			Util.ThrowOnNullOrEmptyArgument(userId, "userId");
			return this.WriteNormalized(provider, networkId ?? string.Empty, userId);
		}

		private byte[] WriteNormalized(Guid provider, string networkId, string userId)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(2);
					binaryWriter.Write(0);
					binaryWriter.Write(1);
					int num = this.WriteEntry(binaryWriter, provider, networkId, userId);
					binaryWriter.Seek(8, SeekOrigin.Begin);
					binaryWriter.Write((ushort)num);
					binaryWriter.Seek(2, SeekOrigin.Begin);
					binaryWriter.Write((ushort)memoryStream.Length);
					result = memoryStream.ToArray();
				}
			}
			return result;
		}

		private int WriteEntry(BinaryWriter writer, Guid provider, string networkId, string userId)
		{
			int num = 0;
			writer.Write(2);
			num += 2;
			writer.Write(0);
			num += 2;
			writer.Write(provider.ToByteArray());
			num += 16;
			int byteCount = Encoding.Unicode.GetByteCount(networkId);
			writer.Write((ushort)byteCount);
			num += 2;
			writer.Write(Encoding.Unicode.GetBytes(networkId));
			num += byteCount;
			int byteCount2 = Encoding.Unicode.GetByteCount(userId);
			writer.Write((ushort)byteCount2);
			num += 2;
			writer.Write(Encoding.Unicode.GetBytes(userId));
			return num + byteCount2;
		}

		private const ushort HeaderVersion = 2;

		private const int HeaderVersionLength = 2;

		private const ushort EntryVersion = 2;

		private const int EntryVersionLength = 2;

		private const int PropertySizeOffset = 2;

		private const int PropertySizeLength = 2;

		private const int EntryCountLength = 2;

		private const int EntrySizeOffset = 8;

		private const int EntrySizeLength = 2;

		private const int ProviderGuidLength = 16;

		private const int NetworkIdLength = 2;

		private const int UserIdLength = 2;

		public static readonly OscContactSourcesForContactWriter Instance = new OscContactSourcesForContactWriter();
	}
}
