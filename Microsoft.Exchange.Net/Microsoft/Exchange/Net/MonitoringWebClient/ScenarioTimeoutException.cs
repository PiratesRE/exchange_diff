using System;
using System.Text;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class ScenarioTimeoutException : HttpWebResponseWrapperException
	{
		public ScenarioTimeoutException(string message, HttpWebRequestWrapper request, HttpWebResponseWrapper response) : base(message, request, response)
		{
		}

		public override string Message
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(base.GetType().FullName + Environment.NewLine);
				stringBuilder.Append(base.Message);
				return stringBuilder.ToString();
			}
		}

		public override string ExceptionHint
		{
			get
			{
				string str = (base.Request != null) ? base.Request.RequestUri.Host : string.Empty;
				return "ScenarioTimeout: " + str;
			}
		}
	}
}
