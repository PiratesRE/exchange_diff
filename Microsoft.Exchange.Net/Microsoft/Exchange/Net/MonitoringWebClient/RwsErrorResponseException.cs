using System;
using Microsoft.Exchange.Net.MonitoringWebClient.Rws.Parsers;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class RwsErrorResponseException : HttpWebResponseWrapperException
	{
		public RwsErrorResponse RwsErrorResponse { get; private set; }

		public RwsErrorResponseException(string message, HttpWebRequestWrapper request, HttpWebResponseWrapper response, RwsErrorResponse rwsErrorResponse) : base(message, request, response)
		{
			this.RwsErrorResponse = rwsErrorResponse;
		}

		public override string ToString()
		{
			string text = base.ToString();
			text = text + Environment.NewLine + Environment.NewLine;
			if (this.RwsErrorResponse != null)
			{
				text = text + this.RwsErrorResponse.ToString() + Environment.NewLine;
			}
			return text;
		}

		public override string ExceptionHint
		{
			get
			{
				return "RwsErrorResponse: " + this.RwsErrorResponse.ExceptionType;
			}
		}
	}
}
