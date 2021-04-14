using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationReportDataProvider : XsoMailboxDataProviderBase
	{
		private MigrationReportDataProvider(MigrationDataProvider dataProvider) : base(dataProvider.MailboxSession)
		{
			this.dataProvider = dataProvider;
		}

		public IMigrationDataProvider MailboxProvider
		{
			get
			{
				return this.dataProvider;
			}
		}

		public static MigrationReportDataProvider CreateDataProvider(string action, IRecipientSession recipientSession, Stream csvStream, int startingRowIndex, int rowCount, ADUser partitionMailbox, bool isTenantAdmin)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(action, "action");
			MigrationUtil.ThrowOnNullArgument(recipientSession, "recipientSession");
			MigrationReportDataProvider result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				MigrationDataProvider disposable = MigrationDataProvider.CreateProviderForReportMailbox(action, recipientSession, partitionMailbox);
				disposeGuard.Add<MigrationDataProvider>(disposable);
				MigrationReportDataProvider migrationReportDataProvider = new MigrationReportDataProvider(disposable);
				migrationReportDataProvider.csvStream = csvStream;
				migrationReportDataProvider.startingRowIndex = startingRowIndex;
				migrationReportDataProvider.rowCount = rowCount;
				migrationReportDataProvider.isTenantAdmin = isTenantAdmin;
				disposeGuard.Success();
				result = migrationReportDataProvider;
			}
			return result;
		}

		protected override IEnumerable<T> InternalFindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			MigrationReportItem reportItem = MigrationReportItem.Get(this.MailboxProvider, (MigrationReportId)rootId);
			MigrationReport migrationReport = reportItem.GetMigrationReport(this.MailboxProvider, this.csvStream, this.startingRowIndex, this.rowCount, this.isTenantAdmin);
			yield return (T)((object)migrationReport);
			yield break;
		}

		protected override void InternalSave(ConfigurableObject instance)
		{
		}

		protected override void InternalDispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					if (this.dataProvider != null)
					{
						this.dataProvider.Dispose();
					}
					this.dataProvider = null;
				}
			}
			finally
			{
				base.InternalDispose(disposing);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MigrationReportDataProvider>(this);
		}

		private MigrationDataProvider dataProvider;

		private Stream csvStream;

		private int startingRowIndex;

		private int rowCount;

		private bool isTenantAdmin;
	}
}
