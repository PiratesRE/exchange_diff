using System;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal class WsuTimestamp
	{
		static WsuTimestamp()
		{
			string s = DateTime.UtcNow.ToString("o");
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			WsuTimestamp.wsuByteCount = WsuTimestamp.wsuBytesP1.Length + WsuTimestamp.wsuBytesP2.Length + WsuTimestamp.wsuBytesP3.Length + bytes.Length * 2;
		}

		internal static int EncodedByteCount
		{
			get
			{
				return WsuTimestamp.wsuByteCount;
			}
		}

		internal static void WriteTimestamp(DateTime time, Stream output)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(time.ToString("o"));
			byte[] bytes2 = Encoding.UTF8.GetBytes(time.AddMinutes(5.0).ToString("o"));
			output.Write(WsuTimestamp.wsuBytesP1, 0, WsuTimestamp.wsuBytesP1.Length);
			output.Write(bytes, 0, bytes.Length);
			output.Write(WsuTimestamp.wsuBytesP2, 0, WsuTimestamp.wsuBytesP2.Length);
			output.Write(bytes2, 0, bytes2.Length);
			output.Write(WsuTimestamp.wsuBytesP3, 0, WsuTimestamp.wsuBytesP3.Length);
		}

		private const string WsuTemplateP1 = "<wsu:Timestamp Id=\"Timestamp\"><wsu:Created>";

		private const string WsuTemplateP2 = "</wsu:Created><wsu:Expires>";

		private const string WsuTemplateP3 = "</wsu:Expires></wsu:Timestamp>";

		private static readonly byte[] wsuBytesP1 = Encoding.UTF8.GetBytes("<wsu:Timestamp Id=\"Timestamp\"><wsu:Created>");

		private static readonly byte[] wsuBytesP2 = Encoding.UTF8.GetBytes("</wsu:Created><wsu:Expires>");

		private static readonly byte[] wsuBytesP3 = Encoding.UTF8.GetBytes("</wsu:Expires></wsu:Timestamp>");

		private static readonly int wsuByteCount;
	}
}
