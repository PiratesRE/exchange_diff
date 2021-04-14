using System;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	internal sealed class RefreshCompletedEventArgs : EventArgs
	{
		public RefreshCompletedEventArgs(bool changed, VariantConfigurationOverride[] overrides)
		{
			this.IsChanged = changed;
			this.Overrides = (overrides ?? new VariantConfigurationOverride[0]);
		}

		public bool IsChanged { get; private set; }

		public VariantConfigurationOverride[] Overrides { get; private set; }
	}
}
