using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Metering.ResourceMonitoring;

namespace Microsoft.Exchange.Transport.ResourceThrottling
{
	internal interface IResourceLevelObserver
	{
		string Name { get; }

		bool Paused { get; }

		string SubStatus { get; }

		void HandleResourceChange(IEnumerable<ResourceUse> allResourceUses, IEnumerable<ResourceUse> changedResourceUses, IEnumerable<ResourceUse> rawResourceUses);
	}
}
