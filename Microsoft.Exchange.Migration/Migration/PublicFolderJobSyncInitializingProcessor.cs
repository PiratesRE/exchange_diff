using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PublicFolderJobSyncInitializingProcessor : MigrationJobSyncInitializingProcessor
	{
		protected override IMigrationDataRowProvider GetMigrationDataRowProvider()
		{
			return new PublicFolderCSVDataRowProvider(base.Job, base.DataProvider);
		}

		protected override void CreateNewJobItem(IMigrationDataRow dataRow)
		{
			LocalizedException ex;
			MailboxData mailboxData = base.GetMailboxData(dataRow.Identifier, out ex);
			if (mailboxData == null)
			{
				if (ex == null)
				{
					ex = new MigrationObjectNotFoundInADException(dataRow.Identifier, base.DataProvider.ADProvider.GetPreferredDomainController());
				}
				MigrationJobItem.CreateFailed(base.DataProvider, base.Job, dataRow, ex, null, null);
				return;
			}
			if (!(dataRow is PublicFolderMigrationDataRow))
			{
				MigrationJobItem.CreateFailed(base.DataProvider, base.Job, dataRow, new MigrationUnknownException(), null, null);
				return;
			}
			base.CreateNewJobItem(dataRow, mailboxData, this.DetermineInitialStatus());
		}

		protected override MailboxData InternalGetMailboxData(string identifier)
		{
			return base.DataProvider.ADProvider.GetPublicFolderMailboxDataFromName(identifier, false, true);
		}

		protected override bool IgnorePostCompleteSubmits
		{
			get
			{
				return false;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PublicFolderJobSyncInitializingProcessor>(this);
		}
	}
}
