using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class PermissionEntry
	{
		public PermissionEntry(PermissionSecurityPrincipal userSecurityPrincipal, MemberRights userRights)
		{
			this.userSecurityPrincipal = userSecurityPrincipal;
			this.userRights = userRights;
		}

		public PermissionSecurityPrincipal UserSecurityPrincipal
		{
			get
			{
				return this.userSecurityPrincipal;
			}
		}

		public MemberRights UserRights
		{
			get
			{
				return this.userRights;
			}
		}

		private PermissionSecurityPrincipal userSecurityPrincipal;

		private MemberRights userRights;
	}
}
