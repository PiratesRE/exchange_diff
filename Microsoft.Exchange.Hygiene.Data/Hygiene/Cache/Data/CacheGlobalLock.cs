using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Hygiene.Data;

namespace Microsoft.Exchange.Hygiene.Cache.Data
{
	[Serializable]
	internal class CacheGlobalLock : ConfigurablePropertyBag
	{
		public static string CacheIdentity
		{
			get
			{
				return "CacheGlobalLock";
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(CacheGlobalLock.CacheIdentity);
			}
		}
	}
}
