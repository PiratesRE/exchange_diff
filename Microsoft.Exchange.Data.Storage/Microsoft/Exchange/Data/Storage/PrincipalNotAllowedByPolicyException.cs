using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PrincipalNotAllowedByPolicyException : StoragePermanentException
	{
		public PrincipalNotAllowedByPolicyException(PermissionSecurityPrincipal principal) : base(ServerStrings.PrincipalNotAllowedByPolicy((principal == null) ? string.Empty : principal.ToString()))
		{
			if (principal == null)
			{
				throw new ArgumentNullException("principal");
			}
			this.principal = principal;
		}

		public PermissionSecurityPrincipal Principal
		{
			get
			{
				return this.principal;
			}
		}

		private PermissionSecurityPrincipal principal;
	}
}
