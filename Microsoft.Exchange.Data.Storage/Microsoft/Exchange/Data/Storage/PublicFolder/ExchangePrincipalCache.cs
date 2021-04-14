using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.PublicFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ExchangePrincipalCache : LazyLookupTimeoutCache<MultiValueKey, ExchangePrincipal>
	{
		public static ExchangePrincipalCache Instance
		{
			get
			{
				return ExchangePrincipalCache.instance;
			}
		}

		private ExchangePrincipalCache() : base(5, 1000, false, TimeSpan.FromMinutes(15.0), TimeSpan.FromMinutes(15.0))
		{
		}

		protected override ExchangePrincipal CreateOnCacheMiss(MultiValueKey cacheKey, ref bool shouldAdd)
		{
			if (cacheKey.KeyLength != 2)
			{
				throw new ArgumentException(string.Format("Unexpected cacheKey length: {0}", cacheKey.KeyLength), "cacheKey");
			}
			OrganizationId organizationId = (OrganizationId)cacheKey.GetKey(0);
			Guid mailboxGuid = (Guid)cacheKey.GetKey(1);
			ExchangePrincipal result = ExchangePrincipal.FromMailboxGuid(organizationId.ToADSessionSettings(), mailboxGuid, RemotingOptions.AllowCrossSite, null);
			shouldAdd = true;
			return result;
		}

		private static ExchangePrincipalCache instance = new ExchangePrincipalCache();
	}
}
