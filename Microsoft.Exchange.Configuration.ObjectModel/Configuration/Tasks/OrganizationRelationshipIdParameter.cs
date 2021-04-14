using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class OrganizationRelationshipIdParameter : ADIdParameter
	{
		public OrganizationRelationshipIdParameter()
		{
		}

		public OrganizationRelationshipIdParameter(ADObjectId adobjectid) : base(adobjectid)
		{
		}

		public OrganizationRelationshipIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected OrganizationRelationshipIdParameter(string identity) : base(identity)
		{
		}

		public static OrganizationRelationshipIdParameter Parse(string identity)
		{
			return new OrganizationRelationshipIdParameter(identity);
		}
	}
}
