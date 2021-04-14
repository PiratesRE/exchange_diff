using System;

namespace Microsoft.Exchange.Migration
{
	internal class GetMailboxImportRequestStatisticsCommand : MrsAccessorCommand
	{
		public GetMailboxImportRequestStatisticsCommand() : base("Get-MailboxImportRequestStatistics", null, null)
		{
		}

		internal const string CmdletName = "Get-MailboxImportRequestStatistics";
	}
}
