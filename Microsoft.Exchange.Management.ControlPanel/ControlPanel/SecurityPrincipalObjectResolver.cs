using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class SecurityPrincipalObjectResolver : AdObjectResolver
	{
		public IEnumerable<SecurityPrincipalRow> ResolveObjects(IEnumerable<ADObjectId> identities)
		{
			return from row in base.ResolveObjects<SecurityPrincipalRow>(identities, SecurityPrincipalRow.Properties, (ADRawEntry e) => new SecurityPrincipalRow(e))
			orderby row.Name
			select row;
		}

		internal override IDirectorySession CreateAdSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.PartiallyConsistent, base.TenantSessionSetting, 124, "CreateAdSession", "f:\\15.00.1497\\sources\\dev\\admin\\src\\ecp\\Pickers\\SecurityPrincipalResolver.cs");
		}

		internal static readonly SecurityPrincipalObjectResolver Instance = new SecurityPrincipalObjectResolver();
	}
}
