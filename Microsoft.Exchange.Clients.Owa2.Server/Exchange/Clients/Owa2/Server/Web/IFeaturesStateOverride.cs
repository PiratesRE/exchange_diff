using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	public interface IFeaturesStateOverride
	{
		bool IsFeatureEnabled(string featureId);
	}
}
