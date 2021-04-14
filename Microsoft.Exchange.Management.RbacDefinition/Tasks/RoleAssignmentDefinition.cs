using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class RoleAssignmentDefinition
	{
		public RoleType RoleType { get; private set; }

		public RoleAssignmentDelegationType DelegationType { get; private set; }

		public bool UseSafeRole { get; private set; }

		public ExchangeBuild IntroducedInBuild
		{
			get
			{
				if (this.introducedInBuild == null)
				{
					return new ExchangeBuild(0, 0, 0, 0);
				}
				return ExchangeBuild.Parse(this.introducedInBuild);
			}
		}

		public RoleAssignmentDefinition(RoleType roleType, RoleAssignmentDelegationType delegationType, string[] neededFeatures, string introducedInBuild, bool useSafeVersionOfRole)
		{
			this.RoleType = roleType;
			this.DelegationType = delegationType;
			this.neededFeatures = neededFeatures;
			this.introducedInBuild = introducedInBuild;
			this.UseSafeRole = useSafeVersionOfRole;
		}

		public bool SatisfyCondition(List<string> enabledFeatures)
		{
			bool result = false;
			if (enabledFeatures == null || this.neededFeatures == null)
			{
				result = true;
			}
			else
			{
				foreach (string item in this.neededFeatures)
				{
					if (enabledFeatures.Contains(item))
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		public bool SatisfyCondition(List<string> enabledFeatures, RoleGroupRoleMapping[] assignments)
		{
			bool flag = this.SatisfyCondition(enabledFeatures);
			if (!flag && this.DelegationType == RoleAssignmentDelegationType.Regular)
			{
				foreach (RoleGroupRoleMapping roleGroupRoleMapping in assignments)
				{
					foreach (RoleAssignmentDefinition roleAssignmentDefinition in roleGroupRoleMapping.Assignments)
					{
						if (roleAssignmentDefinition.RoleType == this.RoleType && roleAssignmentDefinition.DelegationType != RoleAssignmentDelegationType.Regular && roleAssignmentDefinition.SatisfyCondition(enabledFeatures))
						{
							return true;
						}
					}
				}
			}
			return flag;
		}

		private string[] neededFeatures;

		private readonly string introducedInBuild;
	}
}
