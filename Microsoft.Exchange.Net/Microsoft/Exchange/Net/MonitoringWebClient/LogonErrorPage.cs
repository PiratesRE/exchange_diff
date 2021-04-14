using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal abstract class LogonErrorPage
	{
		protected LogonErrorPage(string errorString)
		{
			this.errorString = errorString;
			this.logonErrorType = LogonErrorType.Unknown;
		}

		public string ErrorString
		{
			get
			{
				return this.errorString;
			}
		}

		public LogonErrorType LogonErrorType
		{
			get
			{
				return this.logonErrorType;
			}
		}

		private readonly string errorString;

		protected LogonErrorType logonErrorType;
	}
}
