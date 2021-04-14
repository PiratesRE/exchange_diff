using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class DsnRecipientHeaderType
	{
		internal const string OriginalRecipient = "Original-recipient";

		internal const string FinalRecipient = "Final-recipient";

		internal const string Action = "Action";

		internal const string Status = "Status";

		internal const string RemoteMta = "Remote-MTA";

		internal const string DiagnosticCode = "Diagnostic-code";

		internal const string DisplayName = "X-Display-Name";

		internal const string SupplementaryInfo = "X-Supplementary-Info";
	}
}
