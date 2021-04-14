using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MoveJobSyncInitializingProcessor : MigrationJobSyncInitializingProcessor
	{
		protected override bool IgnorePostCompleteSubmits
		{
			get
			{
				return false;
			}
		}

		protected override IMigrationDataRowProvider GetMigrationDataRowProvider()
		{
			return new MoveCsvDataRowProvider(base.Job, base.DataProvider);
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
			MoveMigrationDataRow moveMigrationDataRow = dataRow as MoveMigrationDataRow;
			if (moveMigrationDataRow == null)
			{
				MigrationJobItem.CreateFailed(base.DataProvider, base.Job, dataRow, new MigrationUnknownException(), null, null);
				return;
			}
			MigrationUserRecipientType recipientType = moveMigrationDataRow.RecipientType;
			if (recipientType != MigrationUserRecipientType.Mailbox)
			{
				switch (recipientType)
				{
				case MigrationUserRecipientType.Mailuser:
					if (mailboxData.RecipientType != MigrationUserRecipientType.Mailuser)
					{
						MigrationJobItem.CreateFailed(base.DataProvider, base.Job, dataRow, new InvalidRecipientTypeException(MigrationUserRecipientType.Mailbox.ToString(), MigrationUserRecipientType.Mailuser.ToString()), null, null);
						return;
					}
					break;
				case MigrationUserRecipientType.MailboxOrMailuser:
					break;
				default:
					MigrationJobItem.CreateFailed(base.DataProvider, base.Job, dataRow, new UnsupportedRecipientTypeForProtocolException(moveMigrationDataRow.RecipientType.ToString(), base.Job.MigrationType.ToString()), null, null);
					return;
				}
			}
			else if (mailboxData.RecipientType != MigrationUserRecipientType.Mailbox)
			{
				MigrationJobItem.CreateFailed(base.DataProvider, base.Job, dataRow, new InvalidRecipientTypeException(MigrationUserRecipientType.Mailuser.ToString(), MigrationUserRecipientType.Mailbox.ToString()), null, null);
				return;
			}
			base.CreateNewJobItem(dataRow, mailboxData, this.DetermineInitialStatus());
		}

		protected override MigrationBatchError GetValidationError(IMigrationDataRow dataRow, LocalizedString locErrorString)
		{
			MoveMigrationDataRow moveMigrationDataRow = (MoveMigrationDataRow)dataRow;
			return new MigrationBatchError
			{
				RowIndex = moveMigrationDataRow.CursorPosition,
				EmailAddress = dataRow.Identifier,
				LocalizedErrorMessage = locErrorString
			};
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MoveJobSyncInitializingProcessor>(this);
		}
	}
}
