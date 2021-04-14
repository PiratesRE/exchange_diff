using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.Exchange.Transport.Common;

namespace Microsoft.Exchange.Transport.ResourceMonitoring
{
	internal class ResourceThrottlingConfig : TransportAppConfig
	{
		public ResourceThrottlingConfig(NameValueCollection appSettings = null) : base(appSettings)
		{
			this.isResourceThrottlingEnabled = base.GetConfigBool("ResourceThrottlingEnabled", false);
			if (this.isResourceThrottlingEnabled)
			{
				this.disabledObserverNames = this.GetDisabledResourceLevelObservers();
				this.maxTransientExceptionsAllowed = base.GetConfigInt("MaxTransientExceptionsAllowed", 1, 100, 5);
				this.resourceObserverTimeout = base.GetConfigTimeSpan("ResourceLevelObserverTimeout", TimeSpan.FromMilliseconds(500.0), TimeSpan.FromMinutes(30.0), TimeSpan.FromMinutes(5.0));
				this.maxThrottlingDelayInterval = base.GetConfigTimeSpan("SmtpMaxThrottlingDelayInterval", TimeSpan.Zero, TimeSpan.FromMinutes(10.0), TimeSpan.FromSeconds(55.0));
				this.baseThrottlingDelayInterval = base.GetConfigTimeSpan("SmtpBaseThrottlingDelayInterval", TimeSpan.Zero, this.maxThrottlingDelayInterval, TimeSpan.Zero);
				this.stepThrottlingDelayInterval = base.GetConfigTimeSpan("SmtpStepThrottlingDelayInterval", TimeSpan.Zero, this.maxThrottlingDelayInterval, TimeSpan.FromSeconds(1.0));
				this.startThrottlingDelayInterval = base.GetConfigTimeSpan("SmtpStartThrottlingDelayInterval", TimeSpan.Zero, this.maxThrottlingDelayInterval, TimeSpan.FromSeconds(1.0));
				this.dehydrateMessagesUnderMemoryPressure = base.GetConfigBool("DehydrateMessagesUnderMemoryPressure", true);
			}
		}

		public IEnumerable<string> DisabledResourceLevelObservers
		{
			get
			{
				return this.disabledObserverNames;
			}
		}

		public TimeSpan MaxThrottlingDelayInterval
		{
			get
			{
				return this.maxThrottlingDelayInterval;
			}
		}

		public TimeSpan BaseThrottlingDelayInterval
		{
			get
			{
				return this.baseThrottlingDelayInterval;
			}
		}

		public TimeSpan StepThrottlingDelayInterval
		{
			get
			{
				return this.stepThrottlingDelayInterval;
			}
		}

		public TimeSpan StartThrottlingDelayInterval
		{
			get
			{
				return this.startThrottlingDelayInterval;
			}
		}

		public TimeSpan ResourceObserverTimeout
		{
			get
			{
				return this.resourceObserverTimeout;
			}
		}

		public int MaxTransientExceptionsAllowed
		{
			get
			{
				return this.maxTransientExceptionsAllowed;
			}
		}

		public bool DehydrateMessagesUnderMemoryPressure
		{
			get
			{
				return this.dehydrateMessagesUnderMemoryPressure;
			}
		}

		public bool IsResourceThrottlingEnabled
		{
			get
			{
				return this.isResourceThrottlingEnabled;
			}
		}

		private IEnumerable<string> GetDisabledResourceLevelObservers()
		{
			return base.GetConfigString("DisabledResourceLevelObservers", string.Empty).Split(new char[]
			{
				';'
			});
		}

		private readonly bool isResourceThrottlingEnabled;

		private readonly IEnumerable<string> disabledObserverNames;

		private readonly int maxTransientExceptionsAllowed;

		private readonly TimeSpan resourceObserverTimeout;

		private readonly TimeSpan maxThrottlingDelayInterval;

		private readonly TimeSpan baseThrottlingDelayInterval;

		private readonly TimeSpan stepThrottlingDelayInterval;

		private readonly TimeSpan startThrottlingDelayInterval;

		private readonly bool dehydrateMessagesUnderMemoryPressure;
	}
}
