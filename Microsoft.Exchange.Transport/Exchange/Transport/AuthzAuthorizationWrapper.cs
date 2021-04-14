using System;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Transport
{
	internal class AuthzAuthorizationWrapper : IAuthzAuthorization
	{
		public int CheckGenericPermission(SecurityIdentifier sid, RawSecurityDescriptor rawSecurityDescriptor, AccessMask requestedAccess)
		{
			return AuthzAuthorization.CheckGenericPermission(sid, rawSecurityDescriptor, requestedAccess);
		}

		public Permission CheckPermissions(IntPtr token, RawSecurityDescriptor rawSecurityDescriptor, SecurityIdentifier principalSelfSid)
		{
			return AuthzAuthorization.CheckPermissions(token, rawSecurityDescriptor, principalSelfSid);
		}

		public Permission CheckPermissions(SecurityIdentifier sid, RawSecurityDescriptor rawSecurityDescriptor, SecurityIdentifier principalSelfSid)
		{
			return AuthzAuthorization.CheckPermissions(sid, rawSecurityDescriptor, principalSelfSid);
		}

		public bool CheckSingleExtendedRight(SecurityIdentifier sid, RawSecurityDescriptor rawSecurityDescriptor, Guid extendedRightGuid)
		{
			return AuthzAuthorization.CheckSingleExtendedRight(sid, rawSecurityDescriptor, extendedRightGuid);
		}

		public bool CheckSinglePermission(IntPtr token, RawSecurityDescriptor rawSecurityDescriptor, Permission permission)
		{
			return AuthzAuthorization.CheckSinglePermission(token, rawSecurityDescriptor, permission);
		}

		public bool CheckSinglePermission(IntPtr token, SecurityDescriptor securityDescriptor, Permission permission)
		{
			return AuthzAuthorization.CheckSinglePermission(token, securityDescriptor, permission);
		}

		public bool CheckSinglePermission(IntPtr token, RawSecurityDescriptor rawSecurityDescriptor, Permission permission, SecurityIdentifier principalSelfSid)
		{
			return AuthzAuthorization.CheckSinglePermission(token, rawSecurityDescriptor, permission, principalSelfSid);
		}

		public bool CheckSinglePermission(IntPtr token, SecurityDescriptor securityDescriptor, Permission permission, SecurityIdentifier principalSelfSid)
		{
			return AuthzAuthorization.CheckSinglePermission(token, securityDescriptor, permission, principalSelfSid);
		}

		public bool CheckSinglePermission(SecurityIdentifier sid, bool isExchangeWellKnownSid, RawSecurityDescriptor rawSecurityDescriptor, Permission permission)
		{
			return AuthzAuthorization.CheckSinglePermission(sid, isExchangeWellKnownSid, rawSecurityDescriptor, permission);
		}

		public bool CheckSinglePermission(SecurityIdentifier sid, bool isExchangeWellKnownSid, RawSecurityDescriptor rawSecurityDescriptor, Permission permission, SecurityIdentifier principalSelfSid)
		{
			return AuthzAuthorization.CheckSinglePermission(sid, isExchangeWellKnownSid, rawSecurityDescriptor, permission, principalSelfSid);
		}

		public bool CheckSinglePermission(SecurityIdentifier sid, bool isExchangeWellKnownSid, SecurityDescriptor securityDescriptor, Permission permission, SecurityIdentifier principalSelfSid)
		{
			return AuthzAuthorization.CheckSinglePermission(sid, isExchangeWellKnownSid, securityDescriptor, permission, principalSelfSid);
		}
	}
}
