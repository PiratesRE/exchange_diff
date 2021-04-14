using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RightsNotAllowedRecipient
	{
		internal RightsNotAllowedRecipient(PermissionSecurityPrincipal principal, MemberRights notAllowedRights)
		{
			Util.ThrowOnNullArgument(principal, "principal");
			this.principal = principal;
			this.notAllowedRights = notAllowedRights;
		}

		public PermissionSecurityPrincipal Principal
		{
			get
			{
				return this.principal;
			}
		}

		public MemberRights NotAllowedRights
		{
			get
			{
				return this.notAllowedRights;
			}
		}

		public override string ToString()
		{
			return "Principal=" + this.Principal.ToString() + ", NotAllowedRights=" + this.NotAllowedRights.ToString();
		}

		private PermissionSecurityPrincipal principal;

		private MemberRights notAllowedRights;
	}
}
