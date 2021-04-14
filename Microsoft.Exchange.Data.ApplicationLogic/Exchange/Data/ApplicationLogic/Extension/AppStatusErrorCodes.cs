using System;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal sealed class AppStatusErrorCodes
	{
		public const string AppUpdateFailed = "1.0";

		public const string AppUpdateFailedPermissionChange = "1.1";

		public const string AppUpdateFailedInvalidEtoken = "1.2";

		public const string LicenseUpdateFailed = "2.0";

		public const string LicenseUpdateFailedAndExpired = "2.1";

		public const string AppStateUnknown = "3.0";

		public const string AppStateWithdrawn = "3.1";

		public const string AppStateFlagged = "3.2";

		public const string AppStateWithdrawnSoon = "3.3";

		public const string AppDisabledByClient = "4.0";

		public const string AppDisabledOutlookPerformance = "4.1";

		public const string AppInTrialMode = "5.0";
	}
}
