using System;
using System.Security.Principal;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Security.Authentication
{
	internal class SerializedIdentity : GenericSidIdentity
	{
		public SerializedIdentity(SerializedAccessToken accessToken) : base(accessToken.LogonName, accessToken.AuthenticationType, new SecurityIdentifier(accessToken.UserSid))
		{
			this.accessToken = accessToken;
		}

		public SerializedAccessToken AccessToken
		{
			get
			{
				return this.accessToken;
			}
		}

		internal override ClientSecurityContext CreateClientSecurityContext()
		{
			return new ClientSecurityContext(this.AccessToken, AuthzFlags.AuthzSkipTokenGroups);
		}

		private SerializedAccessToken accessToken;
	}
}
