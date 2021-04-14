using System;
using System.Net;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.PswsClient;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	internal class Authenticator : IAuthenticator
	{
		private Authenticator(ICredentials credentials)
		{
			this.credentials = credentials;
		}

		public static IAuthenticator Create(OrganizationId organizationId, ADObjectId executingUserId)
		{
			ArgumentValidator.ThrowIfNull("organizationId", organizationId);
			ArgumentValidator.ThrowIfNull("executingUserId", executingUserId);
			ADSessionSettings sessionSettings = ADSessionSettings.FromExternalDirectoryOrganizationId(new Guid(organizationId.ToExternalDirectoryOrganizationId()));
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 54, "Create", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\EOPRecipient\\Authenticator.cs");
			ADUser actAsUser = tenantOrRootOrgRecipientSession.FindADUserByObjectId(executingUserId);
			OAuthCredentials oauthCredentialsForAppActAsToken = OAuthCredentials.GetOAuthCredentialsForAppActAsToken(organizationId, actAsUser, null);
			return new Authenticator(oauthCredentialsForAppActAsToken);
		}

		public IDisposable Authenticate(HttpWebRequest request)
		{
			this.authenticateExecuted = true;
			request.Credentials = this.credentials;
			request.PreAuthenticate = true;
			return null;
		}

		public override string ToString()
		{
			return string.Format("Authenticator.Credentials? {0}; Authenticator.AuthenticateExecuted = {1}.", this.credentials != null, this.authenticateExecuted);
		}

		private readonly ICredentials credentials;

		private bool authenticateExecuted;
	}
}
