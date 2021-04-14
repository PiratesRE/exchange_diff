using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.VariantConfiguration
{
	public sealed class SectionNameValidationException : OverrideValidationException
	{
		public SectionNameValidationException(VariantConfigurationOverride o, IEnumerable<string> allowedValues, Exception innerException = null) : base(string.Format("A section with the name '{1}' does not exist in component '{0}'. Available section names in this component: {2}.", o.ComponentName, o.SectionName, string.Join(", ", allowedValues)), o, innerException)
		{
			this.AllowedValues = allowedValues;
		}

		public IEnumerable<string> AllowedValues { get; private set; }
	}
}
