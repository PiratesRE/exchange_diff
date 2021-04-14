using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	internal class BufferedStreamReader
	{
		public static async Task<StringBuilder> ReadAsync(Stream stream)
		{
			StringBuilder builder = new StringBuilder();
			byte[] buffer = new byte[1024];
			for (int bytesRead = await stream.ReadAsync(buffer, 0, 1024); bytesRead > 0; bytesRead = await stream.ReadAsync(buffer, 0, 1024))
			{
				builder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
				((IList)buffer).Clear();
			}
			return builder;
		}

		private const int bufferSize = 1024;
	}
}
