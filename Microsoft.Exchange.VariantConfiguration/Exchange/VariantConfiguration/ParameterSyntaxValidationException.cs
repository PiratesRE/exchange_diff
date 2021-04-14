using System;

namespace Microsoft.Exchange.VariantConfiguration
{
	public sealed class ParameterSyntaxValidationException : OverrideValidationException
	{
		public ParameterSyntaxValidationException(VariantConfigurationOverride o, string parameterLine, Exception innerException = null) : base(string.Format("The syntax in parameter override '{0}' is incorrect.", parameterLine), o, innerException)
		{
			this.ParameterLine = parameterLine;
		}

		public string ParameterLine { get; private set; }
	}
}
