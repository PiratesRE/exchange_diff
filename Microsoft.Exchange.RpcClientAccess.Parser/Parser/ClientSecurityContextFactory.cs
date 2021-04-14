using System;
using System.Security.Principal;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal static class ClientSecurityContextFactory
	{
		public static ClientSecurityContext Create(byte[] serialization, Action<Exception> onHandledException)
		{
			if (serialization == null)
			{
				throw new ArgumentNullException("serialization");
			}
			if (onHandledException == null)
			{
				throw new ArgumentNullException("onHandledException");
			}
			ClientSecurityContext result;
			using (BufferReader bufferReader = new BufferReader(new ArraySegment<byte>(serialization)))
			{
				AuthenticationContext authenticationContext = null;
				try
				{
					authenticationContext = AuthenticationContext.Parse(bufferReader);
				}
				catch (BufferParseException obj)
				{
					onHandledException(obj);
					return null;
				}
				result = ClientSecurityContextFactory.Create(authenticationContext, onHandledException);
			}
			return result;
		}

		public static ClientSecurityContext Create(AuthenticationContext authenticationContext, Action<Exception> onHandledException)
		{
			if (authenticationContext == null)
			{
				throw new ArgumentNullException("authenticationContext");
			}
			if (onHandledException == null)
			{
				throw new ArgumentNullException("onHandledException");
			}
			if (authenticationContext.RegularGroups.Length == 0)
			{
				return null;
			}
			if (authenticationContext.PrimaryGroupIndex < 0 && !authenticationContext.UserSid.IsWellKnown(WellKnownSidType.LocalSystemSid))
			{
				return null;
			}
			ISecurityAccessTokenEx securityAccessToken = ClientSecurityContextFactory.Normalize(authenticationContext);
			ClientSecurityContext result = null;
			try
			{
				result = new ClientSecurityContext(securityAccessToken, AuthzFlags.AuthzSkipTokenGroups);
			}
			catch (AuthzException obj)
			{
				onHandledException(obj);
			}
			catch (InvalidSidException obj2)
			{
				onHandledException(obj2);
			}
			return result;
		}

		private static ISecurityAccessTokenEx Normalize(AuthenticationContext authenticationContext)
		{
			return new SecurityAccessTokenEx
			{
				UserSid = authenticationContext.UserSid,
				GroupSids = ClientSecurityContextFactory.Normalize(authenticationContext.RegularGroups, authenticationContext.PrimaryGroupIndex),
				RestrictedGroupSids = ClientSecurityContextFactory.Normalize(authenticationContext.RestrictedGroups, 0)
			};
		}

		private static SidBinaryAndAttributes[] Normalize(SidBinaryAndAttributes[] input, int primaryGroupIndex)
		{
			if (input == null || input.Length == 0)
			{
				return null;
			}
			SidBinaryAndAttributes[] array = new SidBinaryAndAttributes[input.Length];
			int num = 0;
			SidBinaryAndAttributes sidBinaryAndAttributes = (primaryGroupIndex >= 0) ? ClientSecurityContextFactory.Normalize(input[primaryGroupIndex]) : null;
			if (sidBinaryAndAttributes != null)
			{
				array[num++] = sidBinaryAndAttributes;
			}
			for (int num2 = 0; num2 != input.Length; num2++)
			{
				if (num2 != primaryGroupIndex)
				{
					sidBinaryAndAttributes = ClientSecurityContextFactory.Normalize(input[num2]);
					if (sidBinaryAndAttributes != null)
					{
						array[num++] = sidBinaryAndAttributes;
					}
				}
			}
			if (num != input.Length)
			{
				Array.Resize<SidBinaryAndAttributes>(ref array, num);
			}
			return array;
		}

		private static SidBinaryAndAttributes Normalize(SidBinaryAndAttributes input)
		{
			SecurityIdentifier securityIdentifier = input.SecurityIdentifier;
			if (securityIdentifier.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid))
			{
				return null;
			}
			GroupAttributes groupAttributes = (GroupAttributes)input.Attributes;
			groupAttributes &= (GroupAttributes.Enabled | GroupAttributes.UseForDenyOnly | GroupAttributes.Integrity | GroupAttributes.IntegrityEnabled | GroupAttributes.IntegrityEnabledDesktop);
			return new SidBinaryAndAttributes(securityIdentifier, (uint)groupAttributes);
		}
	}
}
