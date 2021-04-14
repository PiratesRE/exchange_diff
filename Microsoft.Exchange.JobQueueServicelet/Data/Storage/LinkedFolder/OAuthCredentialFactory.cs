using System;
using System.Net;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OAuthCredentialFactory : IOAuthCredentialFactory
	{
		public ICredentials Get(OrganizationId organizationId)
		{
			return OAuthCredentials.GetOAuthCredentialsForAppToken(organizationId, "PlaceHolder");
		}
	}
}
