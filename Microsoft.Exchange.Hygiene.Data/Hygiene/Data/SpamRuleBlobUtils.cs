using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal static class SpamRuleBlobUtils
	{
		public static string CompressData(string data)
		{
			string @string;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					binaryFormatter.Serialize(gzipStream, data);
				}
				@string = Encoding.Default.GetString(memoryStream.ToArray());
			}
			return @string;
		}

		public static string DecompressData(string data)
		{
			byte[] bytes = Encoding.Default.GetBytes(data);
			string result;
			using (MemoryStream memoryStream = new MemoryStream(bytes))
			{
				int num = bytes.Length;
				int i = num;
				int num2 = 0;
				while (i > 0)
				{
					int num3 = memoryStream.Read(bytes, num2, i);
					if (num3 == 0)
					{
						break;
					}
					num2 += num3;
					i -= num3;
				}
				memoryStream.Position = 0L;
				using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					result = (string)binaryFormatter.Deserialize(gzipStream);
				}
			}
			return result;
		}

		public static string[] GetProcessorIds(string compressedProcessorIds)
		{
			return SpamRuleBlobUtils.DecompressData(compressedProcessorIds).Split(new char[]
			{
				','
			});
		}
	}
}
