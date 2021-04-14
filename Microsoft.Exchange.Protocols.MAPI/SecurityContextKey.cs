using System;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	internal class SecurityContextKey : IEquatable<SecurityContextKey>
	{
		public static bool operator ==(SecurityContextKey left, SecurityContextKey right)
		{
			return object.Equals(left, right);
		}

		public static bool operator !=(SecurityContextKey left, SecurityContextKey right)
		{
			return !object.Equals(left, right);
		}

		public SecurityContextKey(ClientSecurityContext securityContext)
		{
			Globals.AssertRetail(securityContext != null, "SecurityContext can't be null.");
			this.primarySecurityIdentity = securityContext.UserSid;
			foreach (IdentityReference identityReference in securityContext.GetGroups())
			{
				SecurityIdentifier securityIdentifier = identityReference as SecurityIdentifier;
				if (!(securityIdentifier == null) && securityIdentifier.Value.StartsWith("S-1-8"))
				{
					if (!(this.secondarySecurityIdentity == null))
					{
						throw new StoreException((LID)52092U, ErrorCodeValue.NotSupported, "Security context contains more than one group SIDs.");
					}
					this.secondarySecurityIdentity = securityIdentifier;
				}
			}
		}

		public bool Equals(SecurityContextKey other)
		{
			return this.primarySecurityIdentity == other.primarySecurityIdentity && this.secondarySecurityIdentity == other.secondarySecurityIdentity;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as SecurityContextKey);
		}

		public override int GetHashCode()
		{
			return ((this.primarySecurityIdentity != null) ? this.primarySecurityIdentity.GetHashCode() : 0) * 397 ^ ((this.secondarySecurityIdentity != null) ? this.secondarySecurityIdentity.GetHashCode() : 0);
		}

		public override string ToString()
		{
			return string.Format("pSID:{0} sSID:{1}", this.primarySecurityIdentity, this.secondarySecurityIdentity);
		}

		private const string GroupPrefix = "S-1-8";

		private readonly SecurityIdentifier primarySecurityIdentity;

		private readonly SecurityIdentifier secondarySecurityIdentity;
	}
}
