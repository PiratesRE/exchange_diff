using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.VariantConfiguration
{
	public sealed class ParameterNameValidationException : OverrideValidationException
	{
		public ParameterNameValidationException(VariantConfigurationOverride o, string parameterName, IEnumerable<string> allowedValues, Exception innerException = null) : base(string.Format("A parameter with the name '{2}' does not exist in component '{0}', section '{1}'. Available parameter names are {3}.", new object[]
		{
			o.ComponentName,
			o.SectionName,
			parameterName,
			string.Join(", ", allowedValues)
		}), o, innerException)
		{
			this.ParameterName = parameterName;
			this.AllowedValues = allowedValues;
		}

		public string ParameterName { get; private set; }

		public IEnumerable<string> AllowedValues { get; private set; }
	}
}
