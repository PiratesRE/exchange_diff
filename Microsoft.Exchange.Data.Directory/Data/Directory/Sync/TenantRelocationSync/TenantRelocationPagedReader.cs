using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync
{
	internal class TenantRelocationPagedReader : ADPagedReader<TenantRelocationSyncObject>
	{
		internal TenantRelocationPagedReader(IDirectorySession session, ADObjectId OrganizationUnitRoot, int pageSize, IEnumerable<PropertyDefinition> properties, byte[] cookie) : base(session, session.GetDomainNamingContext(), QueryScope.SubTree, null, null, pageSize, properties, false)
		{
			base.LdapFilter = string.Format("(msexchouroot={0})", OrganizationUnitRoot.ToGuidOrDNString());
			base.Cookie = cookie;
			base.IncludeDeletedObjects = true;
			base.SearchAllNcs = true;
		}

		internal TenantRelocationSyncObject[] GetNextResultPage()
		{
			return this.GetNextPage();
		}

		private const string TenantObjectsLdapFilter = "(msexchouroot={0})";
	}
}
