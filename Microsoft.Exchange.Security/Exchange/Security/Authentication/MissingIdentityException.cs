using System;
using System.Net;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.Security.Authentication
{
	public class MissingIdentityException : Exception
	{
		public MissingIdentityException(Guid mailboxId, string statusDescription)
		{
			this.StatusCode = HttpStatusCode.Unauthorized;
			this.StatusDescription = statusDescription;
			this.ChallengeString = ConfigProvider.Instance.Configuration.ChallengeResponseStringWithClientProfileEnabled;
			this.DiagnosticText = mailboxId.ToString();
		}

		public HttpStatusCode StatusCode { get; private set; }

		public string StatusDescription { get; private set; }

		public string ChallengeString { get; private set; }

		public string DiagnosticText { get; private set; }
	}
}
