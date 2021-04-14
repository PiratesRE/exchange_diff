using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Metering.ResourceMonitoring;

namespace Microsoft.Exchange.Transport.ResourceThrottling
{
	internal class BootScannerResourceLevelObserver : ResourceLevelObserver
	{
		public BootScannerResourceLevelObserver(IStartableTransportComponent bootScanner) : base("BootScanner", bootScanner, new List<ResourceIdentifier>
		{
			new ResourceIdentifier("PrivateBytes", ""),
			new ResourceIdentifier("QueueLength", "SubmissionQueue")
		})
		{
		}

		internal const string ResourceObserverName = "BootScanner";
	}
}
