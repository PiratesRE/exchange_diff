using System;
using Microsoft.Exchange.Collections.TimeoutCache;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class BadTokenHashCache : TimeoutCache<string, bool>
	{
		private BadTokenHashCache() : base(20, 5000, false)
		{
		}

		internal static BadTokenHashCache Singleton
		{
			get
			{
				return BadTokenHashCache.singleton;
			}
		}

		internal static string BuildKey(string wsSecurityToken)
		{
			int length = 33 * wsSecurityToken.Length / 100;
			int startIndex = BadTokenHashCache.RandomStartPointPercentage * wsSecurityToken.Length / 100;
			string text = wsSecurityToken.Substring(startIndex, length);
			return string.Format("F:{0:X8};P:{1:X8}", wsSecurityToken.GetHashCode(), text.GetHashCode());
		}

		private const int PartialTokenPercentageToHash = 33;

		private static readonly BadTokenHashCache singleton = new BadTokenHashCache();

		private static readonly int RandomStartPointPercentage = new Random().Next(5, 66);

		internal static readonly TimeSpan CacheTimeout = new TimeSpan(0, 5, 0);
	}
}
