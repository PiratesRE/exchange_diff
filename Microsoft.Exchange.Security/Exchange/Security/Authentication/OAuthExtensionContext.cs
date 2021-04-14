using System;
using System.Web;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.Security.Authentication
{
	internal class OAuthExtensionContext
	{
		public HttpContext HttpContext { get; set; }

		public OAuthTokenHandler TokenHandler { get; set; }
	}
}
