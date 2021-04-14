using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class ManagementRoleObjectResolver : AdObjectResolver
	{
		internal override IDirectorySession CreateAdSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, base.TenantSharedConfigurationSessionSetting ?? base.TenantSessionSetting, 97, "CreateAdSession", "f:\\15.00.1497\\sources\\dev\\admin\\src\\ecp\\UsersGroups\\ManagementRoleResolver.cs");
		}

		public IEnumerable<ManagementRoleResolveRow> ResolveObjects(IEnumerable<ADObjectId> identities)
		{
			return from row in base.ResolveObjects<ManagementRoleResolveRow>(identities, ManagementRoleResolveRow.Properties, (ADRawEntry e) => new ManagementRoleResolveRow(e))
			orderby row.DisplayName
			select row;
		}

		internal static readonly ManagementRoleObjectResolver Instance = new ManagementRoleObjectResolver();
	}
}
