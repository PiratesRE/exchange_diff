using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class IntraOrganizationConnectorIdParameter : ADIdParameter
	{
		public IntraOrganizationConnectorIdParameter()
		{
		}

		public IntraOrganizationConnectorIdParameter(ADObjectId adobjectid) : base(adobjectid)
		{
		}

		public IntraOrganizationConnectorIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected IntraOrganizationConnectorIdParameter(string identity) : base(identity)
		{
		}

		public static IntraOrganizationConnectorIdParameter Parse(string identity)
		{
			return new IntraOrganizationConnectorIdParameter(identity);
		}
	}
}
