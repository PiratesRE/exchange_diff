using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CalendarFolderPermission : Permission
	{
		internal CalendarFolderPermission(PermissionSecurityPrincipal securityPrincipal, MemberRights memberRights, long memberId) : base(securityPrincipal, memberRights, memberId)
		{
		}

		internal CalendarFolderPermission(PermissionSecurityPrincipal securityPrincipal, MemberRights memberRights) : base(securityPrincipal, memberRights)
		{
		}

		public FreeBusyAccess FreeBusyAccess
		{
			get
			{
				if ((base.MemberRights & MemberRights.FreeBusyDetailed) == MemberRights.FreeBusyDetailed)
				{
					return FreeBusyAccess.Details;
				}
				if ((base.MemberRights & MemberRights.FreeBusySimple) == MemberRights.FreeBusySimple)
				{
					return FreeBusyAccess.Basic;
				}
				return FreeBusyAccess.None;
			}
			set
			{
				EnumValidator.ThrowIfInvalid<FreeBusyAccess>(value, "value");
				switch (value)
				{
				case FreeBusyAccess.None:
					base.MemberRights &= ~(MemberRights.FreeBusySimple | MemberRights.FreeBusyDetailed);
					return;
				case FreeBusyAccess.Basic:
					base.MemberRights |= MemberRights.FreeBusySimple;
					base.MemberRights &= ~MemberRights.FreeBusyDetailed;
					return;
				case FreeBusyAccess.Details:
					base.MemberRights |= (MemberRights.FreeBusySimple | MemberRights.FreeBusyDetailed);
					return;
				default:
					return;
				}
			}
		}

		protected override void ValidateMemberRights(MemberRights memberRights)
		{
			EnumValidator.ThrowIfInvalid<MemberRights>(memberRights, "memberRights");
		}
	}
}
