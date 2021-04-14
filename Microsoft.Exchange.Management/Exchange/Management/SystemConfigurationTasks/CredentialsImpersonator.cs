using System;
using System.Net;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal sealed class CredentialsImpersonator
	{
		public CredentialsImpersonator()
		{
		}

		public CredentialsImpersonator(ICredentials credentials)
		{
			this.credentials = credentials;
		}

		public void Impersonate(ImpersonateDelegate impersonateDelegate)
		{
			if (this.credentials != null)
			{
				impersonateDelegate(this.credentials);
				return;
			}
			NetworkServiceImpersonator.Initialize();
			if (NetworkServiceImpersonator.Exception != null)
			{
				impersonateDelegate(CredentialCache.DefaultNetworkCredentials);
				return;
			}
			using (NetworkServiceImpersonator.Impersonate())
			{
				impersonateDelegate(CredentialCache.DefaultNetworkCredentials);
			}
		}

		private ICredentials credentials;
	}
}
