using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public abstract class TcpResponse
	{
		public TcpResponse(string responseString)
		{
			this.responseString = responseString;
			this.responseMessage = null;
			this.responseType = this.ParseResponseType(responseString);
		}

		public static char[] AllDelimiters
		{
			get
			{
				return TcpResponse.allDelimiters;
			}
		}

		public static char[] LineDelimiters
		{
			get
			{
				return TcpResponse.lineDelimiters;
			}
		}

		public static char[] WordDelimiters
		{
			get
			{
				return TcpResponse.wordDelimiters;
			}
		}

		public string ResponseString
		{
			get
			{
				return this.responseString;
			}
		}

		public string ResponseMessage
		{
			get
			{
				return this.responseMessage;
			}
			set
			{
				this.responseMessage = value;
			}
		}

		public ResponseType ResponseType
		{
			get
			{
				return this.responseType;
			}
		}

		public abstract ResponseType ParseResponseType(string responseString);

		public override string ToString()
		{
			return this.responseString;
		}

		private readonly string responseString;

		private static char[] allDelimiters = new char[]
		{
			' ',
			'\n'
		};

		private static char[] lineDelimiters = new char[]
		{
			'\n'
		};

		private static char[] wordDelimiters = new char[]
		{
			' '
		};

		private ResponseType responseType;

		private string responseMessage;
	}
}
