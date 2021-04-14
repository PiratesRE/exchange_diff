using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal class OrgAndObjectId : IEquatable<OrgAndObjectId>
	{
		private OrgAndObjectId()
		{
		}

		public OrgAndObjectId(OrganizationId orgId, ADObjectId objectId)
		{
			if (orgId == null)
			{
				throw new ArgumentNullException("orgId");
			}
			this.OrganizationId = orgId;
			this.Id = objectId;
			this.idString = string.Format("Org: {0}, Id: {1}", orgId, (objectId == null) ? "<NULL>" : objectId.DistinguishedName);
		}

		public OrganizationId OrganizationId { get; private set; }

		public ADObjectId Id { get; private set; }

		public override string ToString()
		{
			return this.idString;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as OrgAndObjectId);
		}

		public bool Equals(OrgAndObjectId other)
		{
			if (other == null || this.OrganizationId == null || other.OrganizationId == null)
			{
				return false;
			}
			if (this.OrganizationId.Equals(other.OrganizationId))
			{
				if (this.Id == null)
				{
					if (other.Id == null)
					{
						return true;
					}
				}
				else if (this.Id.Equals(other.Id))
				{
					return true;
				}
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		private readonly string idString;
	}
}
