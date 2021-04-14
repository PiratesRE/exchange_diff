using System;
using System.Security.Principal;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	internal class ExternalIdentity : IIdentity
	{
		public string AuthenticationType
		{
			get
			{
				return "External";
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				return true;
			}
		}

		public string Name
		{
			get
			{
				return this.emailAddress.ToString();
			}
		}

		public SmtpAddress EmailAddress
		{
			get
			{
				return this.emailAddress;
			}
		}

		public SmtpAddress ExternalId
		{
			get
			{
				return this.externalId;
			}
		}

		internal ExternalIdentity(SmtpAddress emailAddress, SmtpAddress externalId)
		{
			this.emailAddress = emailAddress;
			this.externalId = externalId;
		}

		private const string ExternalAuthenticationType = "External";

		private SmtpAddress emailAddress;

		private SmtpAddress externalId;
	}
}
