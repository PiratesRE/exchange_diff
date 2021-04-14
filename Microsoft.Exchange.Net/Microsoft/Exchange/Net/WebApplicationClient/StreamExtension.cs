using System;
using System.IO;

namespace Microsoft.Exchange.Net.WebApplicationClient
{
	internal static class StreamExtension
	{
		public static int CopyTo(this Stream readStream, Stream writeStream)
		{
			int num = 0;
			byte[] array = new byte[4096];
			int num2;
			while ((num2 = readStream.Read(array, 0, array.Length)) > 0)
			{
				writeStream.Write(array, 0, num2);
				num += num2;
			}
			return num;
		}
	}
}
