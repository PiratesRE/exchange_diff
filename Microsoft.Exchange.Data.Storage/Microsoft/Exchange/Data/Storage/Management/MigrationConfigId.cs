using System;
using System.Text;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class MigrationConfigId : ObjectId
	{
		internal MigrationConfigId(OrganizationId orgId)
		{
			this.OrganizationId = orgId;
		}

		public OrganizationId OrganizationId { get; private set; }

		public override byte[] GetBytes()
		{
			return Encoding.UTF8.GetBytes(this.OrganizationId.OrganizationalUnit.DistinguishedName);
		}

		public override string ToString()
		{
			if (OrganizationId.ForestWideOrgId == this.OrganizationId)
			{
				return null;
			}
			return this.OrganizationId.OrganizationalUnit.Name;
		}

		public override bool Equals(object obj)
		{
			MigrationConfigId migrationConfigId = obj as MigrationConfigId;
			return migrationConfigId != null && object.Equals(this.OrganizationId, migrationConfigId.OrganizationId);
		}

		public override int GetHashCode()
		{
			return this.OrganizationId.GetHashCode();
		}
	}
}
