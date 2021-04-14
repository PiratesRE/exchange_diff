using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class PushNotificationPublisherSettings : PushNotificationSettingsBase
	{
		public PushNotificationPublisherSettings(string appId, bool enabled, Version minimumVersion, Version maximumVersion, int queueSize, int numberOfChannels, int addTimeout) : base(appId)
		{
			this.Enabled = enabled;
			this.MinimumVersion = minimumVersion;
			this.MaximumVersion = maximumVersion;
			this.QueueSize = queueSize;
			this.NumberOfChannels = numberOfChannels;
			this.AddTimeout = addTimeout;
		}

		public bool Enabled { get; private set; }

		public Version MinimumVersion { get; private set; }

		public Version MaximumVersion { get; private set; }

		public int QueueSize { get; private set; }

		public int NumberOfChannels { get; private set; }

		public int AddTimeout { get; private set; }

		public override string ToString()
		{
			return base.AppId;
		}

		protected override void RunValidationCheck(List<LocalizedString> errors)
		{
			base.RunValidationCheck(errors);
			if (this.MinimumVersion != null && this.MaximumVersion != null && this.MaximumVersion.CompareTo(this.MinimumVersion) < 0)
			{
				errors.Add(Strings.ValidationErrorRangeVersion(this.MinimumVersion, this.MaximumVersion));
			}
			if (this.QueueSize <= 0)
			{
				errors.Add(Strings.ValidationErrorPositiveInteger("QueueSize", this.QueueSize));
			}
			if (this.NumberOfChannels <= 0)
			{
				errors.Add(Strings.ValidationErrorPositiveInteger("NumberOfChannels", this.NumberOfChannels));
			}
			if (this.AddTimeout < 0)
			{
				errors.Add(Strings.ValidationErrorNonNegativeInteger("AddTimeout", this.AddTimeout));
			}
		}

		protected override bool RunSuitabilityCheck()
		{
			bool result = base.RunSuitabilityCheck();
			if (!this.Enabled)
			{
				string name = base.GetType().Name;
				PushNotificationsCrimsonEvents.DisabledApp.Log<string, string>(base.AppId, name);
				ExTraceGlobals.PublisherManagerTracer.TraceWarning<string, string>((long)this.GetHashCode(), "App '{0}' is currently marked as disabled by '{1}'.", base.AppId, name);
				result = false;
			}
			Version executingVersion = ExchangeSetupContext.GetExecutingVersion();
			if (!this.ValidateVersionSupport(executingVersion))
			{
				string text = string.Format("({0} - {1})", this.MinimumVersion, this.MaximumVersion);
				PushNotificationsCrimsonEvents.UnsupportedVersion.Log<string, string>(base.AppId, text);
				ExTraceGlobals.PublisherManagerTracer.TraceWarning<string, string, Version>((long)this.GetHashCode(), "App '{0}' is not enabled because its version range {1} doesn't support current executing version {2}.", base.AppId, text, executingVersion);
				result = false;
			}
			return result;
		}

		private bool ValidateVersionSupport(Version version)
		{
			return (this.MinimumVersion == null || version.CompareTo(this.MinimumVersion) >= 0) && (this.MaximumVersion == null || version.CompareTo(this.MaximumVersion) <= 0);
		}
	}
}
