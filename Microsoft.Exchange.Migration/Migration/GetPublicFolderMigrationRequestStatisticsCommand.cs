using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetPublicFolderMigrationRequestStatisticsCommand : MrsAccessorCommand
	{
		public GetPublicFolderMigrationRequestStatisticsCommand(object identity) : base("Get-PublicFolderMailboxMigrationRequestStatistics", null, null)
		{
			base.Identity = identity;
		}

		public const string CmdletName = "Get-PublicFolderMailboxMigrationRequestStatistics";
	}
}
