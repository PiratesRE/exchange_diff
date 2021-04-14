using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration.DataAccessLayer;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class IMAPJobSyncInitializingProcessor : MigrationJobSyncInitializingProcessor
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
			return new IMAPCSVDataRowProvider(base.Job, base.DataProvider);
		}

		protected override void CreateNewJobItem(IMigrationDataRow dataRow)
		{
			LocalizedException ex;
			MailboxData mailboxData = base.GetMailboxData(dataRow.Identifier, out ex);
			if (mailboxData == null)
			{
				if (ex == null)
				{
					ex = new MigrationUnknownException();
				}
				MigrationJobItem.CreateFailed(base.DataProvider, base.Job, dataRow, ex, null, null);
				return;
			}
			base.CreateNewJobItem(dataRow, mailboxData, MigrationUserStatus.Queued);
		}

		protected override MigrationBatchError ValidateDataRow(IMigrationDataRow row)
		{
			MigrationBatchError migrationBatchError = base.ValidateDataRow(row);
			if (migrationBatchError != null)
			{
				return migrationBatchError;
			}
			IMAPMigrationDataRow imapmigrationDataRow = (IMAPMigrationDataRow)row;
			ImapEndpoint imapEndpoint = (ImapEndpoint)base.Job.SourceEndpoint;
			if (imapEndpoint.Authentication == IMAPAuthenticationMechanism.Basic)
			{
				string imapUserId = imapmigrationDataRow.ImapUserId;
				string token = MigrationUtil.EncryptedStringToClearText(imapmigrationDataRow.EncryptedPassword);
				if (MigrationUtil.HasUnicodeCharacters(imapUserId) || MigrationUtil.HasUnicodeCharacters(token))
				{
					return this.GetValidationError(row, Strings.CannotSpecifyUnicodeUserIdPasswordWithBasicAuth);
				}
			}
			return null;
		}

		protected override MigrationBatchError GetValidationError(IMigrationDataRow dataRow, LocalizedString locErrorString)
		{
			IMAPMigrationDataRow imapmigrationDataRow = (IMAPMigrationDataRow)dataRow;
			return new MigrationBatchError
			{
				RowIndex = imapmigrationDataRow.CursorPosition,
				EmailAddress = dataRow.Identifier,
				LocalizedErrorMessage = locErrorString
			};
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<IMAPJobSyncInitializingProcessor>(this);
		}
	}
}
