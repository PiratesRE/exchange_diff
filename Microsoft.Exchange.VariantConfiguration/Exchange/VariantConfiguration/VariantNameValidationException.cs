using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.VariantConfiguration
{
	public sealed class VariantNameValidationException : OverrideValidationException
	{
		public VariantNameValidationException(VariantConfigurationOverride o, string variantName, IEnumerable<string> allowedValues, Exception innerException = null) : base(string.Format("The variant name '{0}' is not recognized. Available variant names are {1}.", variantName, string.Join(", ", allowedValues)), o, innerException)
		{
			this.VariantName = variantName;
			this.AllowedValues = allowedValues;
		}

		public string VariantName { get; private set; }

		public IEnumerable<string> AllowedValues { get; private set; }
	}
}
