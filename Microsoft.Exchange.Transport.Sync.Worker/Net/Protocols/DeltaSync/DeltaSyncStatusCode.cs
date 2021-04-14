using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class DeltaSyncStatusCode
	{
		internal const int SuccessfulOperation = 1;

		internal const int InvalidSyncKey = 4104;

		internal const int MessageSizeLimitExceeded = 4309;

		internal const int FolderDoesnotExist = 4402;

		internal const int SyncConflict = 4404;

		internal const int PartnerAuthFailureLowerLimit = 3100;

		internal const int PartnerAuthFailureUpperLimit = 3199;

		internal const int UserAccessFailureLowerLimit = 3200;

		internal const int UserDoesNotExist = 3201;

		internal const int NoSystemAccessForUser = 3202;

		internal const int AuthFailure = 3204;

		internal const int OutOfMailboxQuotaDiskSpace = 3205;

		internal const int MaxedOutSyncRelationships = 3206;

		internal const int UserAccessFailureUpperLimit = 3299;

		internal const int RequestFormatErrorLowerLimit = 4100;

		internal const int RequestFormatErrorUpperLimit = 4199;

		internal const int RequestContentErrorLowerLimit = 4200;

		internal const int RequestContentErrorUpperLimit = 4299;

		internal const int SettingsErrorLowerLimit = 4300;

		internal const int SettingsErrorUpperLimit = 4399;

		internal const int DataOutOfSyncErrorLowerLimit = 4400;

		internal const int DataOutOfSyncErrorUpperLimit = 4499;

		internal const int ServerErrorLowerLimit = 5000;

		internal const int ServerErrorUpperLimit = 5999;
	}
}
