using System;
using System.IO;
using System.Security.Principal;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Server.Storage.MapiDisp
{
	internal static class AuthenticationContextFactory
	{
		public static AuthenticationContext Create(string connectAs, ClientSecurityContext securityContext)
		{
			if (securityContext == null || securityContext.UserSid.IsWellKnown(WellKnownSidType.LocalSystemSid) || securityContext.UserSid.IsWellKnown(WellKnownSidType.NetworkServiceSid))
			{
				return null;
			}
			SecurityAccessTokenEx securityAccessTokenEx = new SecurityAccessTokenEx();
			securityContext.SetSecurityAccessToken(securityAccessTokenEx);
			int primaryGroupIndex = (securityAccessTokenEx.GroupSids == null || securityAccessTokenEx.GroupSids.Length == 0) ? -1 : 0;
			return new AuthenticationContext(connectAs, securityAccessTokenEx.UserSid, primaryGroupIndex, securityAccessTokenEx.GroupSids, securityAccessTokenEx.RestrictedGroupSids);
		}

		public static byte[] CreateSerialization(string connectAs, ClientSecurityContext securityContext)
		{
			AuthenticationContext authenticationContext = AuthenticationContextFactory.Create(connectAs, securityContext);
			if (authenticationContext == null)
			{
				return null;
			}
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream(1024))
			{
				using (StreamWriter streamWriter = new StreamWriter(memoryStream))
				{
					authenticationContext.Serialize(streamWriter);
					result = memoryStream.ToArray();
				}
			}
			return result;
		}
	}
}
