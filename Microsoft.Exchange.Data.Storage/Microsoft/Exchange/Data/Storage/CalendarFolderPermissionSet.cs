using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CalendarFolderPermissionSet : PermissionSet, IEnumerable<CalendarFolderPermission>, IEnumerable<Permission>, IEnumerable
	{
		public CalendarFolderPermissionSet(PermissionTable permissionTable) : base(permissionTable)
		{
		}

		public new CalendarFolderPermission AddEntry(PermissionSecurityPrincipal securityPrincipal, PermissionLevel initialPermissionLevel)
		{
			return (CalendarFolderPermission)base.AddEntry(securityPrincipal, initialPermissionLevel);
		}

		public new CalendarFolderPermission GetEntry(PermissionSecurityPrincipal securityPrincipal)
		{
			return (CalendarFolderPermission)base.GetEntry(securityPrincipal);
		}

		public new CalendarFolderPermission DefaultPermission
		{
			get
			{
				return (CalendarFolderPermission)base.DefaultPermission;
			}
		}

		internal override Permission CreatePermission(PermissionSecurityPrincipal securityPrincipal, MemberRights memberRights)
		{
			return new CalendarFolderPermission(securityPrincipal, memberRights);
		}

		internal override Permission CreatePermission(PermissionSecurityPrincipal securityPrincipal, MemberRights memberRights, long memberId)
		{
			return new CalendarFolderPermission(securityPrincipal, memberRights, memberId);
		}

		internal override ModifyTableOptions ModifyTableOptions
		{
			get
			{
				return base.ModifyTableOptions | ModifyTableOptions.FreeBusyAware;
			}
		}

		public new IEnumerator<CalendarFolderPermission> GetEnumerator()
		{
			return new DownCastEnumerator<Permission, CalendarFolderPermission>(base.PermissionTable.GetEnumerator());
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator<Permission> IEnumerable<Permission>.GetEnumerator()
		{
			return base.PermissionTable.GetEnumerator();
		}
	}
}
