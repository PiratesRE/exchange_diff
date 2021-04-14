using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class RoleNameMappingCollection : List<RoleNameMapping>
	{
		internal List<string> GetDeprecatedRoleNames()
		{
			return (from roleNameMapping in base.FindAll((RoleNameMapping x) => x.IsDeprecatedRole)
			select roleNameMapping.OldName).ToList<string>();
		}

		internal bool IsNewRoleFromOldRole(string roleName, out string oldRoleName)
		{
			List<string> list = (from roleNameMapping in base.FindAll((RoleNameMapping x) => x.NewNames != null && x.NewNames.Contains(roleName))
			select roleNameMapping.OldName).ToList<string>();
			oldRoleName = ((list.Count > 0) ? list[0] : string.Empty);
			return !string.IsNullOrEmpty(oldRoleName);
		}

		internal RoleNameMapping GetMapping(string newName)
		{
			List<RoleNameMapping> list = (from roleNameMapping in base.FindAll((RoleNameMapping x) => !x.IsSplitting && ((x.NewName != null && x.NewName.Equals(newName)) || (x.NewNames != null && x.NewNames.Contains(newName))))
			select roleNameMapping).ToList<RoleNameMapping>();
			if (list.Count <= 0)
			{
				return null;
			}
			return list[0];
		}

		internal RoleNameMapping GetMappingForSplittingRole(string splittingRole)
		{
			List<RoleNameMapping> list = (from roleNameMapping in base.FindAll((RoleNameMapping x) => x.OldName.Equals(splittingRole))
			select roleNameMapping).ToList<RoleNameMapping>();
			if (list.Count <= 0)
			{
				return null;
			}
			return list[0];
		}

		internal List<RoleNameMapping> GetNonRenamingMappings(string name)
		{
			List<RoleNameMapping> list = (from roleNameMapping in base.FindAll((RoleNameMapping x) => (x.IsSplitting || x.IsDeprecatedRole) && (x.OldName.Equals(name) || (x.NewName != null && x.NewName.Equals(name)) || (x.NewNames != null && x.NewNames.Contains(name))))
			select roleNameMapping).ToList<RoleNameMapping>();
			if (list.Count <= 0)
			{
				return null;
			}
			return list;
		}
	}
}
