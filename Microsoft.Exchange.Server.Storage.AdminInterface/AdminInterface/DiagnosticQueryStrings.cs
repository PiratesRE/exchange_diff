using System;

namespace Microsoft.Exchange.Server.Storage.AdminInterface
{
	internal static class DiagnosticQueryStrings
	{
		public static string InvalidCommitCtxFormat()
		{
			return "CommitCtx length is expected to be greater than 0.";
		}
	}
}
