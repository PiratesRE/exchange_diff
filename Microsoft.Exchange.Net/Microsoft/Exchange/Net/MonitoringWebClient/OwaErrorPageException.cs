using System;
using System.Text;
using Microsoft.Exchange.Net.MonitoringWebClient.Owa.Parsers;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class OwaErrorPageException : HttpWebResponseWrapperException
	{
		public OwaErrorPage OwaErrorPage { get; private set; }

		public OwaErrorPageException(string message, HttpWebRequestWrapper request, HttpWebResponseWrapper response, OwaErrorPage owaErrorPage) : base(message, request, response)
		{
			this.OwaErrorPage = owaErrorPage;
		}

		public override string Message
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("{0}{1}", base.GetType().FullName, Environment.NewLine);
				stringBuilder.AppendFormat("{0}{1}", this.OwaErrorPage, Environment.NewLine);
				stringBuilder.Append(base.Message);
				return stringBuilder.ToString();
			}
		}

		public override string ExceptionHint
		{
			get
			{
				return "OwaErrorPage: " + this.OwaErrorPage.ExceptionType;
			}
		}
	}
}
