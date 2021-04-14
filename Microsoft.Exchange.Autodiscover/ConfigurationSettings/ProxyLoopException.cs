using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Autodiscover.ConfigurationSettings
{
	internal sealed class ProxyLoopException : LocalizedException
	{
		internal ProxyLoopException(string redirectServer) : base(new LocalizedString(redirectServer))
		{
			this.RedirectServer = redirectServer;
		}

		internal string RedirectServer { get; private set; }
	}
}
