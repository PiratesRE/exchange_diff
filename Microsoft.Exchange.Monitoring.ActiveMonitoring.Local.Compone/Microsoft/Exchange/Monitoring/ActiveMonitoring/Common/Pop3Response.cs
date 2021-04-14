using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public class Pop3Response : TcpResponse
	{
		public Pop3Response(string responseString) : base(responseString)
		{
		}

		public override ResponseType ParseResponseType(string responseString)
		{
			string[] array = base.ResponseString.Trim().Split(TcpResponse.AllDelimiters, 2);
			if (array.Length == 2)
			{
				base.ResponseMessage = array[1].Trim();
			}
			array[0] = array[0].Trim();
			if (array[0] == "+")
			{
				return ResponseType.sendMore;
			}
			if (array.Length == 0)
			{
				return ResponseType.unknown;
			}
			if (string.Compare(array[0], "+OK", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				return ResponseType.success;
			}
			if (string.Compare(array[0], "-ERR", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				return ResponseType.failure;
			}
			return ResponseType.unknown;
		}

		public bool ParseSTATResponse(out int numMessages, out long mailboxSize)
		{
			return this.ParseTwoNumberResponse(out numMessages, out mailboxSize);
		}

		internal bool ParseTwoNumberResponse(out int numMessages, out long mailboxSize)
		{
			numMessages = 0;
			mailboxSize = 0L;
			string[] array = base.ResponseString.Trim().Split(TcpResponse.WordDelimiters);
			return array.Length == 3 && int.TryParse(array[1], out numMessages) && long.TryParse(array[2], out mailboxSize);
		}

		internal const string SendMore = "+";

		internal enum MultilineParse
		{
			size,
			uid
		}
	}
}
