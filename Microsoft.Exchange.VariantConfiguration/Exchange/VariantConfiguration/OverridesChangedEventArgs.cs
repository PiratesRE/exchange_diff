using System;

namespace Microsoft.Exchange.VariantConfiguration
{
	public class OverridesChangedEventArgs : EventArgs
	{
		public OverridesChangedEventArgs(VariantConfigurationOverride[] newOverrides)
		{
			this.NewOverrides = (newOverrides ?? new VariantConfigurationOverride[0]);
		}

		public VariantConfigurationOverride[] NewOverrides { get; private set; }
	}
}
