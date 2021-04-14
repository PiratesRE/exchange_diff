using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Configuration.DelegatedAuthentication
{
	internal class DelegatedPrincipalCacheData
	{
		internal DelegatedPrincipalCacheData(DelegatedPrincipal principal, DateTime expirationUTC)
		{
			if (principal == null)
			{
				throw new ArgumentNullException("principal");
			}
			this.principal = principal;
			this.expiration = expirationUTC;
		}

		internal DelegatedPrincipal Principal
		{
			get
			{
				return this.principal;
			}
		}

		internal DateTime UTCExpirationTime
		{
			get
			{
				return this.expiration;
			}
		}

		internal DateTime LastReadTime
		{
			get
			{
				return this.lastReadTime;
			}
			set
			{
				this.lastReadTime = value;
			}
		}

		internal bool IsExpired()
		{
			return DateTime.UtcNow > this.expiration;
		}

		private DelegatedPrincipal principal;

		private DateTime expiration;

		private DateTime lastReadTime;
	}
}
