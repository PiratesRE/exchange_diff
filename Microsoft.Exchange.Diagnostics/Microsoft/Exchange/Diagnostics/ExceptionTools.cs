using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Microsoft.Exchange.Diagnostics
{
	internal class ExceptionTools
	{
		public static string GetCompressedStackTrace(Exception e)
		{
			string result = null;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
				{
					byte[] bytes = Encoding.UTF8.GetBytes(e.ToString());
					gzipStream.Write(bytes, 0, bytes.Length);
					gzipStream.Flush();
				}
				memoryStream.Flush();
				byte[] buffer = memoryStream.GetBuffer();
				result = Convert.ToBase64String(buffer, 0, (int)memoryStream.Position, Base64FormattingOptions.None);
			}
			return result;
		}
	}
}
