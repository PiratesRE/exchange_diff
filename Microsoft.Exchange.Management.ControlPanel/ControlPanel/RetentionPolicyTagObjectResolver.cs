using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class RetentionPolicyTagObjectResolver : AdObjectResolver
	{
		public IEnumerable<RetentionPolicyTagResolverRow> ResolveObjects(IEnumerable<ADObjectId> identities)
		{
			IConfigurationSession session = (IConfigurationSession)this.CreateAdSession();
			return from row in base.ResolveObjects<RetentionPolicyTagResolverRow>(identities, RetentionPolicyTagResolverRow.Properties, (ADRawEntry e) => new RetentionPolicyTagResolverRow(e)
			{
				ContentSettings = this.GetELCContentSettings(session, e.Id).FirstOrDefault<ElcContentSettings>()
			})
			orderby row.Name
			select row;
		}

		internal override IDirectorySession CreateAdSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, base.TenantSharedConfigurationSessionSetting ?? base.TenantSessionSetting, 186, "CreateAdSession", "f:\\15.00.1497\\sources\\dev\\admin\\src\\ecp\\Organize\\RetentionPolicyTagResolver.cs");
		}

		internal ADPagedReader<ElcContentSettings> GetELCContentSettings(IConfigurationSession session, ADObjectId identity)
		{
			return session.FindPaged<ElcContentSettings>(identity, QueryScope.SubTree, null, null, 0);
		}

		internal static readonly RetentionPolicyTagObjectResolver Instance = new RetentionPolicyTagObjectResolver();
	}
}
