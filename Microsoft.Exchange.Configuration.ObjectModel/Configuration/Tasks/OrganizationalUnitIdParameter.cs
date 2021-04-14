using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class OrganizationalUnitIdParameter : OrganizationalUnitIdParameterBase
	{
		public OrganizationalUnitIdParameter(string identity) : base(identity)
		{
		}

		public OrganizationalUnitIdParameter()
		{
		}

		public OrganizationalUnitIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public OrganizationalUnitIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public OrganizationalUnitIdParameter(ExtendedOrganizationalUnit organizationalUnit) : base(organizationalUnit.Id)
		{
		}

		public static OrganizationalUnitIdParameter Parse(string identity)
		{
			return new OrganizationalUnitIdParameter(identity);
		}
	}
}
