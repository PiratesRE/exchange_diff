using System;
using System.Net;

namespace Microsoft.Exchange.Net.WebApplicationClient
{
	internal class AuthenticateWebSession : WebSession
	{
		public AuthenticateWebSession(Uri loginUrl, NetworkCredential credentials) : base(loginUrl, credentials)
		{
		}

		public override void Initialize()
		{
		}

		protected override void Authenticate(HttpWebRequest request)
		{
			request.Credentials = base.Credentials;
		}
	}
}
