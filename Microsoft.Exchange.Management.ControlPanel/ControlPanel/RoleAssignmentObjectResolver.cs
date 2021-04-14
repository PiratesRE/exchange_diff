using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class RoleAssignmentObjectResolver : AdObjectResolver
	{
		public IEnumerable<RoleAssignmentObjectResolverRow> ResolveObjects(IEnumerable<ADObjectId> identities)
		{
			return base.ResolveObjects<RoleAssignmentObjectResolverRow>(identities, RoleAssignmentObjectResolverRow.Properties, (ADRawEntry e) => new RoleAssignmentObjectResolverRow(e));
		}

		internal override IDirectorySession CreateAdSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, base.TenantSharedConfigurationSessionSetting ?? base.TenantSessionSetting, 332, "CreateAdSession", "f:\\15.00.1497\\sources\\dev\\admin\\src\\ecp\\UsersGroups\\RoleAssignmentResolver.cs");
		}

		internal static readonly RoleAssignmentObjectResolver Instance = new RoleAssignmentObjectResolver();
	}
}
