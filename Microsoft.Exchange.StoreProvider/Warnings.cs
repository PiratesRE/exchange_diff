using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class Warnings
	{
		internal const int MapiNoService = 262659;

		internal const int MapiErrorsReturned = 263040;

		internal const int MapiPositionChanged = 263297;

		internal const int MapiApproxCount = 263298;

		internal const int MapiCancelMessage = 263552;

		internal const int MapiPartialCompletion = 263808;

		internal const int MapiSecurityRequiredLow = 263809;

		internal const int MapiSecuirtyRequiredMedium = 263810;

		internal const int MapiPartialItems = 263815;

		internal const int SyncProgress = 264224;

		internal const int SyncClientChangeNewer = 264225;
	}
}
