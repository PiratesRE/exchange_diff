using System;

namespace Microsoft.Exchange.Migration
{
	internal class GetMoveRequestStatisticsCommand : MrsAccessorCommand
	{
		public GetMoveRequestStatisticsCommand() : base("Get-MoveRequestStatistics", null, null)
		{
		}

		internal const string CmdletName = "Get-MoveRequestStatistics";
	}
}
