using System;

namespace Microsoft.Exchange.VariantConfiguration
{
	public abstract class OverrideValidationException : Exception
	{
		public OverrideValidationException(string message, VariantConfigurationOverride o, Exception innerException = null) : base(message, innerException)
		{
			this.Override = o;
		}

		public VariantConfigurationOverride Override { get; private set; }
	}
}
