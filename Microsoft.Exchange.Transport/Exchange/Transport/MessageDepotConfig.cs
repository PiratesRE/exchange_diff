using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.Exchange.MessageDepot;
using Microsoft.Exchange.Transport.Common;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Transport
{
	internal class MessageDepotConfig : TransportAppConfig
	{
		public MessageDepotConfig(NameValueCollection appSettings = null) : base(appSettings)
		{
			this.isMessageDepotEnabled = base.GetConfigBool("MessageDepotEnabled", false);
			if (!this.IsMessageDepotEnabled)
			{
				IEnumerable<Microsoft.Exchange.MessageDepot.DayOfWeek> enabledOnDaysOfWeek = VariantConfiguration.InvariantNoFlightingSnapshot.Transport.MessageDepot.EnabledOnDaysOfWeek;
				if (VariantConfiguration.InvariantNoFlightingSnapshot.Transport.MessageDepot.Enabled && enabledOnDaysOfWeek != null)
				{
					if (enabledOnDaysOfWeek.Any((Microsoft.Exchange.MessageDepot.DayOfWeek dayOfWeek) => string.Equals(dayOfWeek.ToString("G"), DateTime.UtcNow.DayOfWeek.ToString("G"))))
					{
						this.isMessageDepotEnabledByVariantConfig = true;
						this.isMessageDepotEnabled = true;
					}
				}
			}
			this.delayNotificationTimeout = base.GetConfigTimeSpan("DelayNotificationTimeout", MessageDepotConfig.MinDelayNotificationTimeout, MessageDepotConfig.MaxDelayNotificationTimeout, MessageDepotConfig.DefaultDelayNotificationTimeout);
		}

		public bool IsMessageDepotEnabled
		{
			get
			{
				return this.isMessageDepotEnabled;
			}
		}

		public TimeSpan DelayNotificationTimeout
		{
			get
			{
				return this.delayNotificationTimeout;
			}
		}

		public bool IsMessageDepotEnabledByVariantConfig
		{
			get
			{
				return this.isMessageDepotEnabledByVariantConfig;
			}
		}

		public const string MessageDepotEnabledLabel = "MessageDepotEnabled";

		public const string DelayNotificationTimeoutLabel = "DelayNotificationTimeout";

		public const bool DefaultMessageDepotEnabled = false;

		public static readonly TimeSpan MinDelayNotificationTimeout = TimeSpan.FromHours(1.0);

		public static readonly TimeSpan DefaultDelayNotificationTimeout = TimeSpan.FromHours(4.0);

		public static readonly TimeSpan MaxDelayNotificationTimeout = TimeSpan.FromDays(2.0);

		private readonly bool isMessageDepotEnabled;

		private readonly TimeSpan delayNotificationTimeout;

		private readonly bool isMessageDepotEnabledByVariantConfig;
	}
}
