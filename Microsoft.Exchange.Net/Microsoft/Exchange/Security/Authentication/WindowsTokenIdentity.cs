using System;
using System.Security.Principal;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Security.Authentication
{
	public class WindowsTokenIdentity : GenericSidIdentity
	{
		public WindowsTokenIdentity(WindowsAccessToken accessToken) : base(accessToken.LogonName, accessToken.AuthenticationType, new SecurityIdentifier(accessToken.UserSid))
		{
			this.accessToken = accessToken;
		}

		public WindowsAccessToken AccessToken
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

		internal SerializedIdentity ToSerializedIdentity()
		{
			SerializedIdentity result;
			using (ClientSecurityContext clientSecurityContext = this.CreateClientSecurityContext())
			{
				SerializedAccessToken serializedAccessToken = new SerializedAccessToken(this.AccessToken.LogonName, this.AccessToken.AuthenticationType, clientSecurityContext);
				result = new SerializedIdentity(serializedAccessToken);
			}
			return result;
		}

		private WindowsAccessToken accessToken;
	}
}
