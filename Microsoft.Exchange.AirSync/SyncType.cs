using System;

namespace Microsoft.Exchange.AirSync
{
	internal struct SyncType
	{
		public const string FirstSync = "F";

		public const string SubsequentSync = "S";

		public const string RecoverySync = "R";

		public const string InvalidSync = "I";
	}
}
