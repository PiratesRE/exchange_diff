using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class MailboxPlanResolver : AdObjectResolver
	{
		public IEnumerable<MailboxPlanResolverRow> ResolveObjects(IEnumerable<ADObjectId> identities)
		{
			return from row in base.ResolveObjects<MailboxPlanResolverRow>(identities, MailboxPlanResolverRow.Properties, (ADRawEntry e) => new MailboxPlanResolverRow(e))
			orderby row.DisplayName
			select row;
		}

		internal override IDirectorySession CreateAdSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.PartiallyConsistent, base.TenantSessionSetting, 106, "CreateAdSession", "f:\\15.00.1497\\sources\\dev\\admin\\src\\ecp\\UsersGroups\\MailboxPlanResolver.cs");
		}

		internal static readonly MailboxPlanResolver Instance = new MailboxPlanResolver();
	}
}
