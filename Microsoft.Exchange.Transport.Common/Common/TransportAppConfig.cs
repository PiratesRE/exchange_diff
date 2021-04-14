using System;
using System.Collections.Specialized;

namespace Microsoft.Exchange.Transport.Common
{
	internal abstract class TransportAppConfig : AppConfig
	{
		protected TransportAppConfig(NameValueCollection appSettings = null) : base(appSettings)
		{
		}
	}
}
