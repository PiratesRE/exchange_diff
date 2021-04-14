using System;
using System.Net;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public class Pop3Connection : TcpConnection
	{
		public Pop3Connection(IPEndPoint endpoint) : base(endpoint)
		{
		}

		public TcpResponse SendRequest(string request, bool multiLine)
		{
			base.SendData(request);
			return base.GetResponse(120, null, multiLine);
		}

		public override bool LastLineReceived(string responseString, string expectedTag, bool multiLine)
		{
			int num = responseString.LastIndexOf("\r\n");
			return num > 0 && (!multiLine || !responseString.StartsWith("+OK", StringComparison.InvariantCultureIgnoreCase) || responseString.EndsWith("\r\n.\r\n", StringComparison.InvariantCultureIgnoreCase));
		}

		public override TcpResponse CreateResponse(string responseString)
		{
			return new Pop3Response(responseString);
		}

		private const string MultiLineStart = "+OK";

		private const string MultiLineEnd = "\r\n.\r\n";
	}
}
