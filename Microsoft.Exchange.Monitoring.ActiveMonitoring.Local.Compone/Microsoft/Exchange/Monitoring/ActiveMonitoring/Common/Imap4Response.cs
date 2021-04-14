using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public class Imap4Response : TcpResponse
	{
		public Imap4Response(string responseString) : base(responseString)
		{
		}

		public string[] ResponseLines
		{
			get
			{
				return this.responseLines;
			}
		}

		public override ResponseType ParseResponseType(string responseString)
		{
			if (responseString.StartsWith("+", StringComparison.InvariantCultureIgnoreCase))
			{
				return ResponseType.sendMore;
			}
			this.responseLines = base.ResponseString.Trim().Replace("\r", string.Empty).Split(TcpResponse.LineDelimiters);
			string[] array = this.responseLines[this.responseLines.Length - 1].Trim().Split(TcpResponse.WordDelimiters, 3);
			if (array.Length < 1)
			{
				return ResponseType.unknown;
			}
			if (array[0] == "+")
			{
				if (array.Length == 2)
				{
					base.ResponseMessage = array[1];
				}
				return ResponseType.sendMore;
			}
			if (array.Length < 2)
			{
				return ResponseType.unknown;
			}
			string strA = array[1].ToLower();
			if (array.Length == 3)
			{
				base.ResponseMessage = array[2];
				int num = base.ResponseMessage.IndexOf(']');
				if (base.ResponseMessage[0] == '[' && num > 0)
				{
					base.ResponseMessage = base.ResponseMessage.Substring(num + 2);
				}
			}
			if (string.Compare(strA, "ok", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				return ResponseType.success;
			}
			if (string.Compare(strA, "no", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				return ResponseType.failure;
			}
			if (string.Compare(strA, "bad", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				return ResponseType.error;
			}
			if (string.Compare(strA, "bye", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				return ResponseType.bye;
			}
			return ResponseType.unknown;
		}

		public const string SyncResponse = "+";

		private string[] responseLines;
	}
}
