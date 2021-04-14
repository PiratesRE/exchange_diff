using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class DatabaseAvailabilityGroupIdParameter : ADIdParameter
	{
		public DatabaseAvailabilityGroupIdParameter(DatabaseAvailabilityGroup dag) : base(dag.Id)
		{
		}

		public DatabaseAvailabilityGroupIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public DatabaseAvailabilityGroupIdParameter()
		{
		}

		public DatabaseAvailabilityGroupIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		protected DatabaseAvailabilityGroupIdParameter(string identity) : base(identity)
		{
		}

		public static DatabaseAvailabilityGroupIdParameter Parse(string identity)
		{
			return new DatabaseAvailabilityGroupIdParameter(identity);
		}
	}
}
