using System;
using System.IO;
using System.IO.Compression;

namespace Microsoft.Exchange.Common
{
	public static class StringUtil
	{
		public static string Unwrap(string value)
		{
			if (value.Length < 2)
			{
				return value;
			}
			int index = value.Length - 1;
			char c = value[index];
			char c2 = value[0];
			if (c2 == '\'' && c == '\'')
			{
				return StringUtil.WithoutFirstAndLastCharacters(value);
			}
			if (c2 == '"' && c == '"')
			{
				return StringUtil.WithoutFirstAndLastCharacters(value);
			}
			if (c2 == '<' && c == '>')
			{
				return StringUtil.WithoutFirstAndLastCharacters(value);
			}
			return value;
		}

		public static byte[] PackString(string data)
		{
			if (string.IsNullOrEmpty(data))
			{
				return null;
			}
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (StreamWriter streamWriter = new StreamWriter(memoryStream))
				{
					streamWriter.Write(data);
					streamWriter.Flush();
					array = memoryStream.ToArray();
				}
			}
			if (array.Length == 0)
			{
				return null;
			}
			return array;
		}

		public static string UnpackString(byte[] data)
		{
			if (data == null)
			{
				return null;
			}
			string result;
			using (MemoryStream memoryStream = new MemoryStream(data))
			{
				using (StreamReader streamReader = new StreamReader(memoryStream))
				{
					result = streamReader.ReadToEnd();
				}
			}
			return result;
		}

		public static byte[] DecompressBytes(byte[] data)
		{
			if (data == null || data.Length == 0)
			{
				return null;
			}
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream(data))
			{
				using (MemoryStream memoryStream2 = new MemoryStream())
				{
					using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress, true))
					{
						byte[] array = new byte[4096];
						for (;;)
						{
							int num = gzipStream.Read(array, 0, array.Length);
							if (num == 0)
							{
								break;
							}
							memoryStream2.Write(array, 0, num);
						}
						result = memoryStream2.ToArray();
					}
				}
			}
			return result;
		}

		public static byte[] CompressBytes(byte[] data)
		{
			if (data == null || data.Length == 0)
			{
				return data;
			}
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
				{
					gzipStream.Write(data, 0, data.Length);
				}
				result = memoryStream.ToArray();
			}
			return result;
		}

		public static string DecompressString(byte[] data)
		{
			return StringUtil.UnpackString(StringUtil.DecompressBytes(data));
		}

		public static byte[] CompressString(string data)
		{
			return StringUtil.CompressBytes(StringUtil.PackString(data));
		}

		private static string WithoutFirstAndLastCharacters(string value)
		{
			return value.Substring(1, value.Length - 2);
		}
	}
}
