using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class OrgIdADObjectWrapper
	{
		public OrgIdADObjectWrapper(ADObjectId adObject, OrganizationId orgId)
		{
			if (adObject == null)
			{
				throw new ArgumentNullException("adObject");
			}
			if (orgId == null)
			{
				throw new ArgumentNullException("orgId");
			}
			this.AdObject = adObject;
			this.OrgId = orgId;
		}

		public ADObjectId AdObject { get; private set; }

		public OrganizationId OrgId { get; private set; }

		public override bool Equals(object obj)
		{
			if (!(obj is OrgIdADObjectWrapper))
			{
				return false;
			}
			OrgIdADObjectWrapper orgIdADObjectWrapper = (OrgIdADObjectWrapper)obj;
			return this.AdObject.Equals(orgIdADObjectWrapper.AdObject) && this.OrgId.Equals(orgIdADObjectWrapper.OrgId);
		}

		public override int GetHashCode()
		{
			return this.AdObject.GetHashCode() ^ this.OrgId.GetHashCode();
		}

		public override string ToString()
		{
			return this.AdObject + "-" + this.OrgId;
		}
	}
}
