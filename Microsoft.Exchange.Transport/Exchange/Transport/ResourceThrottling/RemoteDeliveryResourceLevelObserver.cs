using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Metering.ResourceMonitoring;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.RemoteDelivery;

namespace Microsoft.Exchange.Transport.ResourceThrottling
{
	internal class RemoteDeliveryResourceLevelObserver : ResourceLevelObserver
	{
		public RemoteDeliveryResourceLevelObserver(RemoteDeliveryComponent remoteDelivery, string databasePath, bool dehydrateMessagesUnderMemoryPressure) : base("RemoteDelivery", remoteDelivery, new List<ResourceIdentifier>
		{
			new ResourceIdentifier("UsedVersionBuckets", databasePath)
		})
		{
			ArgumentValidator.ThrowIfNull("remoteDelivery", remoteDelivery);
			ArgumentValidator.ThrowIfNullOrEmpty("databasePath", databasePath);
			this.remoteDelivery = remoteDelivery;
			this.dehydrateMessagesUnderMemoryPressure = dehydrateMessagesUnderMemoryPressure;
			this.versionBucketsResource = new ResourceIdentifier("UsedVersionBuckets", databasePath);
		}

		public override void HandleResourceChange(IEnumerable<ResourceUse> allResourceUses, IEnumerable<ResourceUse> changedResourceUses, IEnumerable<ResourceUse> rawResourceUses)
		{
			ArgumentValidator.ThrowIfNull("allResourceUses", allResourceUses);
			ArgumentValidator.ThrowIfNull("changedResourceUses", changedResourceUses);
			ArgumentValidator.ThrowIfNull("rawResourceUses", rawResourceUses);
			if (this.dehydrateMessagesUnderMemoryPressure)
			{
				UseLevel useLevel = ResourceHelper.TryGetCurrentUseLevel(allResourceUses, this.privateBytesResource, UseLevel.Low);
				UseLevel useLevel2 = ResourceHelper.TryGetCurrentUseLevel(allResourceUses, this.versionBucketsResource, UseLevel.Low);
				UseLevel useLevel3 = ResourceHelper.TryGetCurrentUseLevel(rawResourceUses, this.privateBytesResource, UseLevel.Low);
				UseLevel useLevel4 = ResourceHelper.TryGetCurrentUseLevel(rawResourceUses, this.systemMemoryResource, UseLevel.Low);
				if ((useLevel3 != UseLevel.Low || useLevel != UseLevel.Low || useLevel4 != UseLevel.Low) && useLevel2 == UseLevel.Low)
				{
					this.remoteDelivery.CommitLazyAndDehydrateMessages();
				}
			}
			base.HandleResourceChange(allResourceUses, changedResourceUses, rawResourceUses);
		}

		internal const string ResourceObserverName = "RemoteDelivery";

		private readonly ResourceIdentifier privateBytesResource = new ResourceIdentifier("PrivateBytes", "");

		private readonly bool dehydrateMessagesUnderMemoryPressure;

		private readonly RemoteDeliveryComponent remoteDelivery;

		private readonly ResourceIdentifier systemMemoryResource = new ResourceIdentifier("SystemMemory", "");

		private readonly ResourceIdentifier versionBucketsResource;
	}
}
