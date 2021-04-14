using System;
using System.Net;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public class Imap4Connection : TcpConnection
	{
		public Imap4Connection(IPEndPoint endpoint) : base(endpoint)
		{
			this.tag = 0;
		}

		public int NextTag
		{
			get
			{
				return ++this.tag;
			}
		}

		public TcpResponse SendRequest(string request, string expectedTag)
		{
			base.SendData(request);
			return base.GetResponse(120, expectedTag, false);
		}

		public TcpResponse GetResponse(int timeout, string expectedTag)
		{
			return base.GetResponse(timeout, expectedTag, false);
		}

		public override bool LastLineReceived(string responseString, string expectedTag, bool multiLine)
		{
			if (expectedTag == null)
			{
				expectedTag = ((this.tag > 0) ? this.tag.ToString() : "*");
			}
			int num = responseString.LastIndexOf("\r\n");
			if (num <= 0)
			{
				return false;
			}
			if (responseString.StartsWith("+", StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
			num = responseString.LastIndexOf("\r\n", num - 1);
			if (num != -1)
			{
				num += 2;
				responseString = responseString.Substring(num, responseString.Length - num);
			}
			return responseString.StartsWith("+", StringComparison.InvariantCultureIgnoreCase) || responseString.StartsWith(expectedTag, StringComparison.InvariantCultureIgnoreCase) || responseString.StartsWith("* BYE", StringComparison.InvariantCultureIgnoreCase);
		}

		public override TcpResponse CreateResponse(string responseString)
		{
			return new Imap4Response(responseString);
		}

		private const string SyncResponse = "+";

		private const string ByeTag = "* BYE";

		private int tag;
	}
}
