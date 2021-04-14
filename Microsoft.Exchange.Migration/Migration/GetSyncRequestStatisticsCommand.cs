using System;

namespace Microsoft.Exchange.Migration
{
	internal class GetSyncRequestStatisticsCommand : MrsAccessorCommand
	{
		public GetSyncRequestStatisticsCommand() : base("Get-SyncRequestStatistics", null, null)
		{
		}

		internal const string CmdletName = "Get-SyncRequestStatistics";
	}
}
