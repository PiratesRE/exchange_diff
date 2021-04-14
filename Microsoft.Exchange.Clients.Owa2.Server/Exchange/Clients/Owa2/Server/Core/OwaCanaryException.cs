using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Serializable]
	public class OwaCanaryException : OwaPermanentException
	{
		public OwaCanaryException(string cookieName, string cookieValue) : base(LocalizedString.Empty)
		{
			this.CanaryCookieName = cookieName;
			this.CanaryCookieValue = cookieValue;
		}

		public string CanaryCookieValue { get; set; }

		public string CanaryCookieName { get; set; }
	}
}
