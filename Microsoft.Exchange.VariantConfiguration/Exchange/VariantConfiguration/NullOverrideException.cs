using System;

namespace Microsoft.Exchange.VariantConfiguration
{
	public sealed class NullOverrideException : OverrideValidationException
	{
		public NullOverrideException(Exception innerException = null) : base("A null override is passed in for validation.", null, innerException)
		{
		}
	}
}
