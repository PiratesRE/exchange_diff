using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Metering.ResourceMonitoring;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Protocols.Smtp;

namespace Microsoft.Exchange.Transport.ResourceThrottling
{
	internal class SmtpInResourceLevelObserver : IResourceLevelObserver
	{
		public SmtpInResourceLevelObserver(ISmtpInComponent smtpInComponent, ThrottlingController throttlingController, IComponentsWrapper componentsWrapper)
		{
			ArgumentValidator.ThrowIfNull("smtpInComponent", smtpInComponent);
			ArgumentValidator.ThrowIfNull("throttlingController", throttlingController);
			ArgumentValidator.ThrowIfNull("componentsWrapper", componentsWrapper);
			this.smtpInComponent = smtpInComponent;
			this.throttlingController = throttlingController;
			this.componentsWrapper = componentsWrapper;
		}

		public ISmtpInComponent SmtpInComponent
		{
			get
			{
				return this.smtpInComponent;
			}
		}

		public virtual void HandleResourceChange(IEnumerable<ResourceUse> allResourceUses, IEnumerable<ResourceUse> changedResourceUses, IEnumerable<ResourceUse> rawResourceUses)
		{
			ArgumentValidator.ThrowIfNull("allResourceUses", allResourceUses);
			ArgumentValidator.ThrowIfNull("changedResourceUses", changedResourceUses);
			ArgumentValidator.ThrowIfNull("rawResourceUses", rawResourceUses);
			this.SetThrottleDelay(rawResourceUses);
			if (this.componentsWrapper.IsPaused)
			{
				return;
			}
			UseLevel useLevel = ResourceHelper.TryGetCurrentUseLevel(allResourceUses, this.aggregateResourceIdentifier, UseLevel.Low);
			if (useLevel == UseLevel.Low)
			{
				this.smtpInComponent.Continue();
				this.componentPaused = false;
				this.rejectSubmits = false;
				return;
			}
			this.rejectSubmits = (useLevel == UseLevel.High);
			this.smtpInComponent.Pause(this.rejectSubmits, SmtpResponse.InsufficientResource);
			this.componentPaused = true;
		}

		public string Name
		{
			get
			{
				return "SmtpIn";
			}
		}

		private void SetThrottleDelay(IEnumerable<ResourceUse> rawResourceUses)
		{
			UseLevel useLevel = ResourceHelper.TryGetCurrentUseLevel(rawResourceUses, this.submissionQueueResource, UseLevel.Low);
			UseLevel useLevel2 = ResourceHelper.TryGetCurrentUseLevel(rawResourceUses, this.versionBucketsResource, UseLevel.Low);
			if (useLevel != UseLevel.Low || useLevel2 != UseLevel.Low)
			{
				this.throttlingController.Increase();
			}
			else
			{
				this.throttlingController.Decrease();
			}
			TimeSpan current = this.throttlingController.GetCurrent();
			if (current == TimeSpan.Zero)
			{
				this.smtpInComponent.SetThrottleDelay(current, null);
				return;
			}
			string throttleDelayContext = string.Format(CultureInfo.InvariantCulture, "VB={0};QS={1}", new object[]
			{
				useLevel2,
				useLevel
			});
			this.smtpInComponent.SetThrottleDelay(current, throttleDelayContext);
		}

		public bool Paused
		{
			get
			{
				return this.componentPaused;
			}
		}

		public string SubStatus
		{
			get
			{
				if (this.rejectSubmits)
				{
					return "Rejecting Submissions";
				}
				return string.Empty;
			}
		}

		internal const string RejectSubmitStatus = "Rejecting Submissions";

		internal const string ResourceObserverName = "SmtpIn";

		private readonly ISmtpInComponent smtpInComponent;

		private readonly IComponentsWrapper componentsWrapper;

		private readonly ThrottlingController throttlingController;

		private readonly ResourceIdentifier submissionQueueResource = new ResourceIdentifier("QueueLength", "SubmissionQueue");

		private readonly ResourceIdentifier versionBucketsResource = new ResourceIdentifier("UsedVersionBuckets", "");

		private readonly ResourceIdentifier aggregateResourceIdentifier = new ResourceIdentifier("Aggregate", "");

		private bool componentPaused;

		private bool rejectSubmits;
	}
}
