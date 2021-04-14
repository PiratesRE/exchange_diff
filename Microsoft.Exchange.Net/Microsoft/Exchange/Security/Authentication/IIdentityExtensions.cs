using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Security.Authentication
{
	internal static class IIdentityExtensions
	{
		public static string GetSafeName(this IIdentity identity, bool fallbackToSidForWinId = true)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			string result;
			try
			{
				result = identity.Name;
			}
			catch (SystemException)
			{
				WindowsIdentity windowsIdentity = identity as WindowsIdentity;
				if (windowsIdentity == null || !fallbackToSidForWinId)
				{
					throw;
				}
				result = windowsIdentity.User.ToString();
			}
			return result;
		}

		public static SecurityIdentifier GetSecurityIdentifier(this IIdentity identity)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			if (identity is WindowsIdentity)
			{
				return ((WindowsIdentity)identity).User;
			}
			if (identity is ClientSecurityContextIdentity)
			{
				return ((ClientSecurityContextIdentity)identity).Sid;
			}
			if (identity is GenericIdentity)
			{
				return new SecurityIdentifier(identity.Name);
			}
			throw new NotSupportedException("GetSecurityIdentifier does not support " + identity.GetType().ToString());
		}

		public static ClientSecurityContext CreateClientSecurityContext(this IIdentity identity, bool retainIdentity = true)
		{
			if (identity is WindowsIdentity)
			{
				return new ClientSecurityContext((WindowsIdentity)identity, retainIdentity);
			}
			if (identity is ClientSecurityContextIdentity)
			{
				return ((ClientSecurityContextIdentity)identity).CreateClientSecurityContext();
			}
			GenericSidIdentity genericSidIdentity = new GenericSidIdentity(identity.GetSafeName(true), identity.AuthenticationType, identity.GetSecurityIdentifier());
			return genericSidIdentity.CreateClientSecurityContext();
		}

		public static SerializedAccessToken GetAccessToken(this IIdentity identity)
		{
			if (identity is SerializedIdentity)
			{
				return ((SerializedIdentity)identity).AccessToken;
			}
			SerializedAccessToken result;
			using (ClientSecurityContext clientSecurityContext = identity.CreateClientSecurityContext(true))
			{
				result = new SerializedAccessToken(identity.GetSafeName(true), identity.AuthenticationType, clientSecurityContext);
			}
			return result;
		}

		public static ICollection<SecurityIdentifier> GetGroupAccountsSIDs(this IIdentity identity)
		{
			IEnumerable<SecurityIdentifier> source = from sid in identity.GetGroupSIDs()
			where sid.IsAccountSid()
			select sid;
			return source.ToArray<SecurityIdentifier>();
		}

		public static ICollection<SecurityIdentifier> GetGroupSIDs(this IIdentity identity)
		{
			SerializedAccessToken accessToken = identity.GetAccessToken();
			IEnumerable<SecurityIdentifier> sids = accessToken.GroupSids.GetSids();
			IEnumerable<SecurityIdentifier> sids2 = accessToken.RestrictedGroupSids.GetSids();
			return sids.Concat(sids2).ToArray<SecurityIdentifier>();
		}

		private static IEnumerable<SecurityIdentifier> GetSids(this SidStringAndAttributes[] groups)
		{
			if (groups != null)
			{
				foreach (SidStringAndAttributes group in groups)
				{
					yield return new SecurityIdentifier(group.SecurityIdentifier);
				}
			}
			yield break;
		}
	}
}
