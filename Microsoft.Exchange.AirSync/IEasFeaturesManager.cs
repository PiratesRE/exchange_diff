using System;

namespace Microsoft.Exchange.AirSync
{
	internal interface IEasFeaturesManager
	{
		bool IsEnabled(EasFeature featureId);

		bool IsOverridden(EasFeature featureId);
	}
}
