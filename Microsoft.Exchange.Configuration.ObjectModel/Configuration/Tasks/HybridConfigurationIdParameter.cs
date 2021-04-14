using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class HybridConfigurationIdParameter : ADIdParameter
	{
		public HybridConfigurationIdParameter()
		{
		}

		public HybridConfigurationIdParameter(string identity) : base(identity)
		{
		}

		public HybridConfigurationIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public HybridConfigurationIdParameter(HybridConfiguration hybridRelationship) : base(hybridRelationship.Id)
		{
		}

		public HybridConfigurationIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static HybridConfigurationIdParameter Parse(string identity)
		{
			return new HybridConfigurationIdParameter(identity);
		}

		internal const string FixedValue = "Hybrid Configuration";
	}
}
