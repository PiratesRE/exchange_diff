using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class RoleTypeSegment
	{
		public RoleTypeSegment(RbacSettings rbacSettings)
		{
			this.rbacSettings = rbacSettings;
		}

		public IList<RoleType> GetAllowedFeatures()
		{
			IList<RoleType> result = null;
			IList<RoleTypeConstraint> constraints = this.GetEnabledConstraints();
			if (constraints != null)
			{
				IEnumerable<RoleType> allFeatures = this.GetAllFeatures();
				result = (from roleType in allFeatures
				where constraints.Any((RoleTypeConstraint constraint) => constraint.Validate(roleType))
				select roleType).ToList<RoleType>().AsReadOnly();
			}
			return result;
		}

		private IList<RoleTypeConstraint> GetEnabledConstraints()
		{
			IList<RoleTypeConstraint> result = null;
			if (!this.IsAllFeaturesEnabled())
			{
				List<RoleTypeConstraint> list = new List<RoleTypeConstraint>();
				if (this.rbacSettings.AdminEnabled)
				{
					list.Add(RoleTypeConstraint.AdminRoleTypeConstraint);
				}
				if (this.rbacSettings.OwaOptionsEnabled)
				{
					list.Add(RoleTypeConstraint.EndUserRoleTypeConstraint);
				}
				result = list.AsReadOnly();
			}
			return result;
		}

		private bool IsAllFeaturesEnabled()
		{
			return this.rbacSettings.AdminEnabled && this.rbacSettings.OwaOptionsEnabled;
		}

		private IEnumerable<RoleType> GetAllFeatures()
		{
			return Enum.GetValues(typeof(RoleType)).OfType<RoleType>();
		}

		private RbacSettings rbacSettings;
	}
}
