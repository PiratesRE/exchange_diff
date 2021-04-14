using System;

namespace Microsoft.Exchange.VariantConfiguration
{
	public sealed class VariantValueValidationException : OverrideValidationException
	{
		public VariantValueValidationException(VariantConfigurationOverride o, VariantType variant, string value, string format, Exception innerException = null) : base(string.Format("The value '{0}' specified for variant '{1}' does not match its type. Only values of type '{2}' in format '{3}' are allowed for this variant.", new object[]
		{
			value,
			variant.Name,
			variant.Type,
			format
		}), o, innerException)
		{
			this.Variant = variant;
			this.Value = value;
			this.Format = format;
		}

		public VariantType Variant { get; private set; }

		public string Value { get; private set; }

		public string Format { get; private set; }
	}
}
