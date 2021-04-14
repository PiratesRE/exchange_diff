using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Metering.ResourceMonitoring;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.ResourceThrottling
{
	internal class ContentAggregatorResourceLevelObserver : IResourceLevelObserver
	{
		public ContentAggregatorResourceLevelObserver(IStartableTransportComponent contentAggregator)
		{
			ArgumentValidator.ThrowIfNull("contentAggregator", contentAggregator);
			this.contentAggregator = contentAggregator;
		}

		public virtual void HandleResourceChange(IEnumerable<ResourceUse> allResourceUses, IEnumerable<ResourceUse> changedResourceUses, IEnumerable<ResourceUse> rawResourceUses)
		{
			ArgumentValidator.ThrowIfNull("allResourceUses", allResourceUses);
			ArgumentValidator.ThrowIfNull("changedResourceUses", changedResourceUses);
			ArgumentValidator.ThrowIfNull("rawResourceUses", rawResourceUses);
			UseLevel useLevel = ResourceHelper.TryGetCurrentUseLevel(rawResourceUses, this.submissionQueueResource, UseLevel.Low);
			UseLevel useLevel2 = ResourceHelper.TryGetCurrentUseLevel(rawResourceUses, this.versionBucketsResource, UseLevel.Low);
			if (useLevel != UseLevel.Low || useLevel2 != UseLevel.Low)
			{
				if (!this.paused)
				{
					this.contentAggregator.Pause();
					this.paused = true;
					return;
				}
			}
			else if (this.paused)
			{
				this.contentAggregator.Continue();
				this.paused = false;
			}
		}

		public string Name
		{
			get
			{
				return "ContentAggregator";
			}
		}

		public bool Paused
		{
			get
			{
				return this.paused;
			}
		}

		public string SubStatus
		{
			get
			{
				return string.Empty;
			}
		}

		internal const string ResourceObserverName = "ContentAggregator";

		private IStartableTransportComponent contentAggregator;

		private readonly ResourceIdentifier submissionQueueResource = new ResourceIdentifier("QueueLength", "SubmissionQueue");

		private readonly ResourceIdentifier versionBucketsResource = new ResourceIdentifier("UsedVersionBuckets", "");

		private bool paused;
	}
}
