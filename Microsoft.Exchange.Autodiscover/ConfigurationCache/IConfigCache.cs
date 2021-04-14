using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Autodiscover.ConfigurationCache
{
	internal interface IConfigCache
	{
		void Refresh(IConfigurationSession session);
	}
}
