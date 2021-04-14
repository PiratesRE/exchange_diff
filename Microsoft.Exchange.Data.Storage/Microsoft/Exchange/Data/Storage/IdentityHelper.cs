using System;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Mapi.Security;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class IdentityHelper
	{
		public static SecurityIdentifier SidFromAuxiliaryIdentity(GenericIdentity auxiliaryIdentity)
		{
			if (auxiliaryIdentity == null)
			{
				return null;
			}
			if (auxiliaryIdentity is GenericSidIdentity)
			{
				return ((GenericSidIdentity)auxiliaryIdentity).Sid;
			}
			return null;
		}

		public static SecurityIdentifier SidFromLogonIdentity(object identity)
		{
			Util.ThrowOnNullArgument(identity, "identity");
			SecurityIdentifier result = null;
			if (identity is WindowsIdentity)
			{
				result = ((WindowsIdentity)identity).User;
			}
			else if (identity is ClientIdentityInfo)
			{
				result = ((ClientIdentityInfo)identity).sidUser;
			}
			return result;
		}

		public static SecurityIdentifier GetEffectiveLogonSid(MailboxSession session)
		{
			Util.ThrowOnNullArgument(session, "session");
			SecurityIdentifier securityIdentifier = IdentityHelper.SidFromAuxiliaryIdentity(session.AuxiliaryIdentity);
			if (securityIdentifier == null)
			{
				securityIdentifier = IdentityHelper.SidFromLogonIdentity(session.Identity);
			}
			return securityIdentifier;
		}

		public static IdentityPair GetIdentityPair(MailboxSession session)
		{
			Util.ThrowOnNullArgument(session, "session");
			IdentityPair result = default(IdentityPair);
			if (session.AuxiliaryIdentity != null)
			{
				if (session.AuxiliaryIdentity is GenericSidIdentity)
				{
					result.LogonUserSid = ((GenericSidIdentity)session.AuxiliaryIdentity).Sid.Value;
				}
				else
				{
					result.LogonUserDisplayName = session.AuxiliaryIdentity.Name;
				}
			}
			else
			{
				result.LogonUserSid = IdentityHelper.SidFromLogonIdentity(session.Identity).Value;
			}
			return result;
		}

		public static SecurityIdentifier CalculateEffectiveSid(SecurityIdentifier userSid, SecurityIdentifier masterAccountSid)
		{
			SecurityIdentifier result;
			if (masterAccountSid == null)
			{
				result = userSid;
			}
			else if (masterAccountSid.IsWellKnown(WellKnownSidType.SelfSid))
			{
				result = userSid;
			}
			else
			{
				result = masterAccountSid;
			}
			return result;
		}
	}
}
