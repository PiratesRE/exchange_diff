using System;
using System.Net;
using Microsoft.Exchange.SoapWebClient;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class AutoDiscoverAuthenticator
	{
		public NetworkCredential Credentials { get; private set; }

		public CredentialCache CredentialCache { get; private set; }

		public ProxyAuthenticator ProxyAuthenticator { get; private set; }

		public AutoDiscoverAuthenticator(NetworkCredential credentials)
		{
			if (Testability.WebServiceCredentials == null)
			{
				this.Credentials = credentials;
			}
			else
			{
				this.Credentials = Testability.WebServiceCredentials;
			}
			this.CredentialCache = null;
		}

		public AutoDiscoverAuthenticator(CredentialCache cache, NetworkCredential credentials)
		{
			if (Testability.WebServiceCredentials == null)
			{
				this.CredentialCache = cache;
				this.Credentials = credentials;
				return;
			}
			this.Credentials = Testability.WebServiceCredentials;
		}

		public AutoDiscoverAuthenticator(ProxyAuthenticator proxyAuthenticator)
		{
			this.ProxyAuthenticator = proxyAuthenticator;
		}

		public void Authenticate(CustomSoapHttpClientProtocol client)
		{
			if (this.ProxyAuthenticator == null)
			{
				if (this.CredentialCache != null)
				{
					this.ProxyAuthenticator = ProxyAuthenticator.Create(this.CredentialCache, null, null);
				}
				else
				{
					this.ProxyAuthenticator = ProxyAuthenticator.Create(this.Credentials, null, null);
				}
			}
			this.ProxyAuthenticator.Authenticate(client);
		}
	}
}
