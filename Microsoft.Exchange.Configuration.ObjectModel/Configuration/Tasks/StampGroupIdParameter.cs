using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class StampGroupIdParameter : ADIdParameter
	{
		public StampGroupIdParameter(StampGroup stampGroup) : base(stampGroup.Id)
		{
		}

		public StampGroupIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public StampGroupIdParameter()
		{
		}

		public StampGroupIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		protected StampGroupIdParameter(string identity) : base(identity)
		{
		}

		public static StampGroupIdParameter Parse(string identity)
		{
			return new StampGroupIdParameter(identity);
		}
	}
}
