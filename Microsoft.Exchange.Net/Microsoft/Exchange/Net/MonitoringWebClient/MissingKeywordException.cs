using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class MissingKeywordException : HttpWebResponseWrapperException
	{
		public MissingKeywordException(string message, HttpWebRequestWrapper request, HttpWebResponseWrapper response, string expectedKeyword) : base(message, request, response)
		{
			this.expectedKeyword = expectedKeyword;
		}

		public override string ExceptionHint
		{
			get
			{
				return "MissingKeyword: " + this.expectedKeyword;
			}
		}

		private readonly string expectedKeyword;
	}
}
