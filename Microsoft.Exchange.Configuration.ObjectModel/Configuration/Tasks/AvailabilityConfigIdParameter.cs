using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class AvailabilityConfigIdParameter : ADIdParameter
	{
		public AvailabilityConfigIdParameter()
		{
		}

		public AvailabilityConfigIdParameter(string identity) : base(identity)
		{
		}

		public AvailabilityConfigIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public AvailabilityConfigIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static AvailabilityConfigIdParameter Parse(string identity)
		{
			return new AvailabilityConfigIdParameter(identity);
		}
	}
}
