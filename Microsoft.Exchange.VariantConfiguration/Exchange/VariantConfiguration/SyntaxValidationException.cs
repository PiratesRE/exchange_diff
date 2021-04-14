using System;

namespace Microsoft.Exchange.VariantConfiguration
{
	public sealed class SyntaxValidationException : OverrideValidationException
	{
		public SyntaxValidationException(VariantConfigurationOverride o, Exception innerException = null) : base(string.Format("The override parameters @(\"{0}\") contain syntax error: {1}", string.Join("\", \"", Array.ConvertAll<string, string>(o.Parameters, (string parameter) => parameter.Replace("\"", "^\""))), (innerException != null) ? innerException.Message : string.Empty), o, innerException)
		{
		}
	}
}
