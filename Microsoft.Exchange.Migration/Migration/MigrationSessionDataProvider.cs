using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationSessionDataProvider : XsoMailboxDataProviderBase
	{
		private MigrationSessionDataProvider(MigrationDataProvider dataProvider) : base(dataProvider.MailboxSession)
		{
			this.dataProvider = dataProvider;
			this.MigrationSession = MigrationSession.Get(this.dataProvider);
			this.diagnosticEnabled = false;
		}

		public IMigrationDataProvider MailboxProvider
		{
			get
			{
				return this.dataProvider;
			}
		}

		public MigrationSession MigrationSession { get; private set; }

		public static MigrationSessionDataProvider CreateDataProvider(string action, IRecipientSession recipientSession, ADUser partitionMailbox)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(action, "action");
			MigrationUtil.ThrowOnNullArgument(recipientSession, "recipientSession");
			MigrationSessionDataProvider result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				MigrationDataProvider disposable = MigrationDataProvider.CreateProviderForMigrationMailbox(action, recipientSession, partitionMailbox);
				disposeGuard.Add<MigrationDataProvider>(disposable);
				MigrationSessionDataProvider migrationSessionDataProvider = new MigrationSessionDataProvider(disposable);
				disposeGuard.Success();
				result = migrationSessionDataProvider;
			}
			return result;
		}

		public static bool IsKnownException(Exception exception)
		{
			return exception is StorageTransientException || exception is StoragePermanentException || exception is MigrationTransientException || exception is MigrationPermanentException || exception is MigrationDataCorruptionException || exception is DiagnosticArgumentException;
		}

		public void EnableDiagnostics(string argument)
		{
			this.diagnosticEnabled = true;
			this.diagnosticArgument = new MigrationDiagnosticArgument(argument);
		}

		protected override IEnumerable<T> InternalFindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			Type pagedType = typeof(T);
			if (pagedType == typeof(MigrationStatistics))
			{
				MigrationStatistics migrationStats = this.MigrationSession.GetMigrationStatistics(this.dataProvider);
				if (this.diagnosticEnabled)
				{
					XElement diagnosticInfo = this.MigrationSession.GetDiagnosticInfo(this.dataProvider, this.diagnosticArgument);
					if (diagnosticInfo != null)
					{
						migrationStats.DiagnosticInfo = diagnosticInfo.ToString();
					}
				}
				yield return (T)((object)migrationStats);
			}
			else
			{
				if (!(pagedType == typeof(MigrationConfig)))
				{
					throw new ArgumentException("Unknown type: " + pagedType, "pagedType");
				}
				yield return (T)((object)this.MigrationSession.GetMigrationConfig(this.dataProvider));
			}
			yield break;
		}

		protected override void InternalSave(ConfigurableObject instance)
		{
			switch (instance.ObjectState)
			{
			default:
				return;
			}
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
			return DisposeTracker.Get<MigrationSessionDataProvider>(this);
		}

		private MigrationDataProvider dataProvider;

		private bool diagnosticEnabled;

		private MigrationDiagnosticArgument diagnosticArgument;
	}
}
