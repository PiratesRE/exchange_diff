using System;

namespace Microsoft.Exchange.Data.Directory.Sync.CookieManager
{
	internal abstract class CookieManager
	{
		public abstract byte[] ReadCookie();

		public abstract void WriteCookie(byte[] cookie, DateTime timestamp);

		public abstract string DomainController { get; }

		public virtual DateTime? GetMostRecentCookieTimestamp()
		{
			return null;
		}
	}
}
