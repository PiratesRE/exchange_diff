using System;
using Microsoft.Exchange.Net.MonitoringWebClient.Ecp.Parsers;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class EcpErrorResponseException : HttpWebResponseWrapperException
	{
		public EcpErrorResponse EcpErrorResponse { get; private set; }

		public EcpErrorResponseException(string message, HttpWebRequestWrapper request, HttpWebResponseWrapper response, EcpErrorResponse ecpErrorResponse) : base(message, request, response)
		{
			this.EcpErrorResponse = ecpErrorResponse;
		}

		public override string ToString()
		{
			string text = base.ToString();
			text = text + Environment.NewLine + Environment.NewLine;
			if (this.EcpErrorResponse != null)
			{
				text = text + this.EcpErrorResponse.ToString() + Environment.NewLine;
			}
			return text;
		}

		public override string ExceptionHint
		{
			get
			{
				return "EcpErrorResponse: " + this.EcpErrorResponse.ExceptionType;
			}
		}
	}
}
