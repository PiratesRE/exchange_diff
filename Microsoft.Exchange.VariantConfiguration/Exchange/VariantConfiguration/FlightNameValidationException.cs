using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.VariantConfiguration
{
	public sealed class FlightNameValidationException : OverrideValidationException
	{
		public FlightNameValidationException(VariantConfigurationOverride o, IEnumerable<string> allowedValues, Exception innerException = null) : base(string.Format("A flight with the name '{0}' does not exist. Available flight names are {1}.", o.SectionName, string.Join(", ", allowedValues)), o, innerException)
		{
			this.AllowedValues = allowedValues;
		}

		public IEnumerable<string> AllowedValues { get; private set; }
	}
}
