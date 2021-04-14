using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal enum WatsonIssueType
	{
		GenericReport,
		NativeCodeCrash,
		ScriptError,
		ManagedCodeIISException,
		ManagedCodeException,
		ManagedCodeDisposableLeak,
		ManagedCodeLatencyIssue,
		ManagedCodeTroubleshootingIssue
	}
}
