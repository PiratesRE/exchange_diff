using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public interface IConfigurationContext
	{
		bool IsFeatureEnabled(Feature feature);

		Feature GetEnabledFeatures();
	}
}
