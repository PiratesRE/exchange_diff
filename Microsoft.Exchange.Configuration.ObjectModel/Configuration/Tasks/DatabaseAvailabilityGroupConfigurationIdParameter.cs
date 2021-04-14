using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class DatabaseAvailabilityGroupConfigurationIdParameter : ADIdParameter
	{
		public DatabaseAvailabilityGroupConfigurationIdParameter(DatabaseAvailabilityGroupConfiguration dagConfig) : base(dagConfig.Id)
		{
		}

		public DatabaseAvailabilityGroupConfigurationIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public DatabaseAvailabilityGroupConfigurationIdParameter()
		{
		}

		public DatabaseAvailabilityGroupConfigurationIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		protected DatabaseAvailabilityGroupConfigurationIdParameter(string identity) : base(identity)
		{
		}

		public static DatabaseAvailabilityGroupConfigurationIdParameter Parse(string identity)
		{
			return new DatabaseAvailabilityGroupConfigurationIdParameter(identity);
		}
	}
}
