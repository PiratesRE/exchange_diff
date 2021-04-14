using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public sealed class ADOrganizationalUnitIdParameter : OrganizationalUnitIdParameterBase
	{
		public ADOrganizationalUnitIdParameter()
		{
		}

		public ADOrganizationalUnitIdParameter(string identity) : base(identity)
		{
		}

		public ADOrganizationalUnitIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public ADOrganizationalUnitIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static ADOrganizationalUnitIdParameter Parse(string identity)
		{
			return new ADOrganizationalUnitIdParameter(identity);
		}
	}
}
