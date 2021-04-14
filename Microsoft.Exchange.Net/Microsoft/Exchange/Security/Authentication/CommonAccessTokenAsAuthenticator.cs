using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.PswsClient;

namespace Microsoft.Exchange.Security.Authentication
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CommonAccessTokenAsAuthenticator : IAuthenticator
	{
		public CommonAccessTokenAsAuthenticator(string token, IAuthenticator serviceAuthenticator)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("token", token);
			ArgumentValidator.ThrowIfNull("serviceAuthenticator", serviceAuthenticator);
			this.token = token;
			this.serviceAuthenticator = serviceAuthenticator;
		}

		public IDisposable Authenticate(HttpWebRequest request)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			request.Headers["X-CommonAccessToken"] = this.token;
			return this.serviceAuthenticator.Authenticate(request);
		}

		private readonly string token;

		private readonly IAuthenticator serviceAuthenticator;
	}
}
