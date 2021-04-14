using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class ExchangeRoleObjectResolver : AdObjectResolver
	{
		internal override IDirectorySession CreateAdSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, base.TenantSharedConfigurationSessionSetting ?? base.TenantSessionSetting, 89, "CreateAdSession", "f:\\15.00.1497\\sources\\dev\\admin\\src\\ecp\\UsersGroups\\ExchangeRoleResolver.cs");
		}

		public IEnumerable<ExchangeRoleObject> ResolveObjects(IEnumerable<ADObjectId> identities)
		{
			return from row in base.ResolveObjects<ExchangeRoleObject>(identities, ExchangeRoleObject.Properties, (ADRawEntry e) => new ExchangeRoleObject(e))
			orderby row.DisplayName
			select row;
		}

		internal static readonly ExchangeRoleObjectResolver Instance = new ExchangeRoleObjectResolver();
	}
}
