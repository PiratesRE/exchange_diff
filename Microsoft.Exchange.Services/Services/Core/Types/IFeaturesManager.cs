using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	public interface IFeaturesManager
	{
		bool IsFeatureSupported(string featureName);
	}
}
