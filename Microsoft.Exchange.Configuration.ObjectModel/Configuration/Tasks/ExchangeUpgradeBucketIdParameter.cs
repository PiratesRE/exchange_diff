using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public sealed class ExchangeUpgradeBucketIdParameter : ADIdParameter
	{
		public ExchangeUpgradeBucketIdParameter()
		{
		}

		public ExchangeUpgradeBucketIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public ExchangeUpgradeBucketIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public ExchangeUpgradeBucketIdParameter(string identity) : base(identity)
		{
		}

		public ExchangeUpgradeBucketIdParameter(ExchangeUpgradeBucket upgradeBucket) : base(upgradeBucket.Id)
		{
		}

		public static ExchangeUpgradeBucketIdParameter Parse(string identity)
		{
			return new ExchangeUpgradeBucketIdParameter(identity);
		}
	}
}
