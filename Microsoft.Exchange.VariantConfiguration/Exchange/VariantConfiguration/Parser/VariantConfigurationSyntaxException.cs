using System;

namespace Microsoft.Exchange.VariantConfiguration.Parser
{
	public class VariantConfigurationSyntaxException : Exception
	{
		public VariantConfigurationSyntaxException(string errorMessage) : this(errorMessage, null)
		{
		}

		public VariantConfigurationSyntaxException(string errorMessage, Exception innerException) : base(errorMessage, innerException)
		{
		}
	}
}
