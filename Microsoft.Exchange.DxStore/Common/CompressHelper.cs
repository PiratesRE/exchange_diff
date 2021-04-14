using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Microsoft.Exchange.DxStore.Common
{
	public static class CompressHelper
	{
		public static byte[] Zip(string str)
		{
			if (str == null)
			{
				str = string.Empty;
			}
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream(bytes))
			{
				using (MemoryStream memoryStream2 = new MemoryStream())
				{
					using (GZipStream gzipStream = new GZipStream(memoryStream2, CompressionMode.Compress))
					{
						memoryStream.CopyTo(gzipStream);
					}
					result = memoryStream2.ToArray();
				}
			}
			return result;
		}

		public static string Unzip(byte[] bytes)
		{
			string result = string.Empty;
			if (bytes != null)
			{
				using (MemoryStream memoryStream = new MemoryStream(bytes))
				{
					using (MemoryStream memoryStream2 = new MemoryStream())
					{
						using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
						{
							gzipStream.CopyTo(memoryStream2);
						}
						result = Encoding.UTF8.GetString(memoryStream2.ToArray());
					}
				}
			}
			return result;
		}

		public static string ZipToBase64String(string str)
		{
			return Convert.ToBase64String(CompressHelper.Zip(str));
		}

		public static string UnzipFromBase64String(string base64Str)
		{
			return CompressHelper.Unzip(Convert.FromBase64String(base64Str));
		}
	}
}
