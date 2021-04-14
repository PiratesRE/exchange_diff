using System;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	public class SmtpRecipient
	{
		public string Username
		{
			get
			{
				return this.username;
			}
			internal set
			{
				this.username = value;
			}
		}

		public SmtpExpectedResponse ExpectedResponse
		{
			get
			{
				return this.expectedResponse;
			}
			internal set
			{
				this.expectedResponse = value;
			}
		}

		private string username;

		private SmtpExpectedResponse expectedResponse;
	}
}
