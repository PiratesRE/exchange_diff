using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class RoleTypeConstraint
	{
		public RoleTypeConstraint(Predicate<RoleType> predicate)
		{
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			this.predicate = predicate;
		}

		public bool Validate(RoleType roleType)
		{
			return this.predicate(roleType);
		}

		private Predicate<RoleType> predicate;

		public static RoleTypeConstraint AdminRoleTypeConstraint = new RoleTypeConstraint((RoleType x) => ExchangeRole.IsAdminRole(x));

		public static RoleTypeConstraint EndUserRoleTypeConstraint = new RoleTypeConstraint((RoleType x) => !ExchangeRole.IsAdminRole(x));
	}
}
