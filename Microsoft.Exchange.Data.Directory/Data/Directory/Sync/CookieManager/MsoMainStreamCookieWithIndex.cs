using System;

namespace Microsoft.Exchange.Data.Directory.Sync.CookieManager
{
	internal class MsoMainStreamCookieWithIndex
	{
		public MsoMainStreamCookieWithIndex(MsoMainStreamCookie cookie, int index)
		{
			if (cookie == null)
			{
				throw new ArgumentNullException("cookie");
			}
			this.Cookie = cookie;
			this.Index = index;
		}

		public MsoMainStreamCookie Cookie { get; private set; }

		public int Index { get; private set; }
	}
}
