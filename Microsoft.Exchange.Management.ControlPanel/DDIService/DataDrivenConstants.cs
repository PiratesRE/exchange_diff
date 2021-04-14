using System;

namespace Microsoft.Exchange.Management.DDIService
{
	internal static class DataDrivenConstants
	{
		public const string Identity = "Identity";

		public const string Force = "Force";

		public const string ShouldContinueContextKey = "ShouldContinueContext";

		public const string LambdaExpression = "LambdaExpression";

		public const string LambdaSeparator = "=>";

		public const string UnlimitedValueString = "unlimited";

		public const string ShouldContinue = "ShouldContinue";

		public const string LastErrorContext = "LastErrorContext";

		public static readonly string[] ReservedVariableNames = new string[]
		{
			"ShouldContinue",
			"LastErrorContext"
		};
	}
}
