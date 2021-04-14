using System;

namespace Microsoft.Exchange.VariantConfiguration.Parser
{
	public class VariantConfigurationConventionException : Exception
	{
		public VariantConfigurationConventionException(string errorMessage) : this(errorMessage, null)
		{
		}

		public VariantConfigurationConventionException(string errorMessage, Exception innerException) : base(errorMessage, innerException)
		{
		}
	}
}
