using System;

namespace Microsoft.Exchange.Data.Metering.ResourceMonitoring
{
	internal interface IResourceMeter
	{
		ResourceIdentifier Resource { get; }

		long Pressure { get; }

		PressureTransitions PressureTransitions { get; }

		ResourceUse ResourceUse { get; }

		ResourceUse RawResourceUse { get; }

		void Refresh();
	}
}
