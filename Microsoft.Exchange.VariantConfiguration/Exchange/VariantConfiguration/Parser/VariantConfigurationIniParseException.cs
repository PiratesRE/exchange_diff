using System;

namespace Microsoft.Exchange.VariantConfiguration.Parser
{
	public class VariantConfigurationIniParseException : Exception
	{
		public VariantConfigurationIniParseException(string errorMessage) : this(errorMessage, null)
		{
		}

		public VariantConfigurationIniParseException(string errorMessage, Exception innerException) : base(errorMessage, innerException)
		{
		}
	}
}
