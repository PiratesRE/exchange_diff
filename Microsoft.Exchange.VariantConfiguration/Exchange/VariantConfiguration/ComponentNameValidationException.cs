using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.VariantConfiguration
{
	public sealed class ComponentNameValidationException : OverrideValidationException
	{
		public ComponentNameValidationException(VariantConfigurationOverride o, IEnumerable<string> allowedValues, Exception innerException = null) : base(string.Format("A component with the name '{0}' does not exist. Available component names are {1}.", o.ComponentName, string.Join(", ", allowedValues)), o, innerException)
		{
			this.AllowedValues = allowedValues;
		}

		public IEnumerable<string> AllowedValues { get; private set; }
	}
}
