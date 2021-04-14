using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class WebServiceUserInformation
	{
		internal WebServiceUserInformation(SmtpAddress userSmtpAddress, string organization)
		{
			this.userSmtpAddress = userSmtpAddress;
			this.organization = organization;
		}

		public string EmailAddress
		{
			get
			{
				return this.userSmtpAddress.ToString();
			}
		}

		public string Domain
		{
			get
			{
				return this.userSmtpAddress.Domain;
			}
		}

		public string Organization
		{
			get
			{
				return this.organization;
			}
		}

		private readonly SmtpAddress userSmtpAddress;

		private readonly string organization;
	}
}
