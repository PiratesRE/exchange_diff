using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Diagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SICDiagnosticArgument : DiagnosableArgument
	{
		protected override void InitializeSchema(Dictionary<string, Type> schema)
		{
			schema["invokescan"] = typeof(bool);
			schema["issue"] = typeof(bool);
			schema["maxsize"] = typeof(int);
		}

		public const string InvokeScanArgument = "invokescan";

		public const string IssueArgument = "issue";

		public const string MaxSizeArgument = "maxsize";
	}
}
