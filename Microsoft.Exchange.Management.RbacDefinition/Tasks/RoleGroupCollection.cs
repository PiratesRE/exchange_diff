using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class RoleGroupCollection : List<RoleGroupDefinition>
	{
		internal ADGroup GetADGroupByGuid(Guid wkGuid)
		{
			RoleGroupDefinition roleGroupDefinition = this.FirstOrDefault((RoleGroupDefinition x) => x.RoleGroupGuid.Equals(wkGuid));
			if (roleGroupDefinition == null)
			{
				return null;
			}
			return roleGroupDefinition.ADGroup;
		}
	}
}
