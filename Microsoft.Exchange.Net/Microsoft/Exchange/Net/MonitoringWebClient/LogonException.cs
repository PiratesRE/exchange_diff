using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class LogonException : HttpWebResponseWrapperException
	{
		public LogonException(string message, HttpWebRequestWrapper request, HttpWebResponseWrapper response, LogonErrorPage logonErrorPage) : base(message, request, response)
		{
			this.logonErrorPage = logonErrorPage;
		}

		public LogonErrorType LogonErrorType
		{
			get
			{
				return this.logonErrorPage.LogonErrorType;
			}
		}

		public override string ExceptionHint
		{
			get
			{
				if (this.LogonErrorType != LogonErrorType.Unknown)
				{
					return "LogonError: " + this.LogonErrorType.ToString();
				}
				return "LogonError: " + this.logonErrorPage.ErrorString;
			}
		}

		private readonly LogonErrorPage logonErrorPage;
	}
}
