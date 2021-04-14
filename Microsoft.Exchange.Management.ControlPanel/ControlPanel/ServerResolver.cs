using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.ControlPanel.DBMgmt;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class ServerResolver : AdObjectResolver, IServerResolver
	{
		private ServerResolver()
		{
		}

		internal static IServerResolver Instance { get; set; } = new ServerResolver();

		public IEnumerable<ServerResolverRow> ResolveObjects(IEnumerable<ADObjectId> identities)
		{
			return from row in base.ResolveObjects<ServerResolverRow>(identities, ServerResolverRow.Properties, (ADRawEntry e) => new ServerResolverRow(e))
			orderby row.DisplayName
			select row;
		}

		internal override IDirectorySession CreateAdSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.PartiallyConsistent, base.TenantSessionSetting, 199, "CreateAdSession", "f:\\15.00.1497\\sources\\dev\\admin\\src\\ecp\\DBMgmt\\ServerResolver.cs");
		}
	}
}
