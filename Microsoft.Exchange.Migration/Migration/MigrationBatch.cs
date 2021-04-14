using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Migration
{
	[Serializable]
	public class MigrationBatch : ConfigurableObject
	{
		public MigrationBatch() : base(new SimplePropertyBag(MigrationBatchSchema.Identity, MigrationBatchSchema.ObjectState, MigrationBatchSchema.ExchangeVersion))
		{
			base.ResetChangeTracking();
		}

		public new MigrationBatchId Identity
		{
			get
			{
				return (MigrationBatchId)base.Identity;
			}
			internal set
			{
				this[MigrationBatchSchema.Identity] = value;
			}
		}

		public MigrationBatchStatus Status
		{
			get
			{
				return (MigrationBatchStatus)this[MigrationBatchSchema.BatchStatus];
			}
			internal set
			{
				this[MigrationBatchSchema.BatchStatus] = value;
			}
		}

		public Guid BatchGuid
		{
			get
			{
				if (this.Identity == null)
				{
					return Guid.Empty;
				}
				return this.Identity.JobId;
			}
		}

		public int TotalCount
		{
			get
			{
				return (int)this[MigrationBatchSchema.TotalCount];
			}
			internal set
			{
				this[MigrationBatchSchema.TotalCount] = value;
			}
		}

		public int ActiveCount
		{
			get
			{
				return (int)this[MigrationBatchSchema.ActiveCount];
			}
			internal set
			{
				this[MigrationBatchSchema.ActiveCount] = value;
			}
		}

		public int StoppedCount
		{
			get
			{
				return (int)this[MigrationBatchSchema.StoppedCount];
			}
			internal set
			{
				this[MigrationBatchSchema.StoppedCount] = value;
			}
		}

		public int SyncedCount
		{
			get
			{
				return (int)this[MigrationBatchSchema.SyncedCount];
			}
			internal set
			{
				this[MigrationBatchSchema.SyncedCount] = value;
			}
		}

		public int FinalizedCount
		{
			get
			{
				return (int)this[MigrationBatchSchema.FinalizedCount];
			}
			internal set
			{
				this[MigrationBatchSchema.FinalizedCount] = value;
			}
		}

		public int FailedCount
		{
			get
			{
				return (int)this[MigrationBatchSchema.FailedCount];
			}
			internal set
			{
				this[MigrationBatchSchema.FailedCount] = value;
			}
		}

		public int FailedInitialSyncCount
		{
			get
			{
				return (int)this[MigrationBatchSchema.FailedInitialSyncCount];
			}
			internal set
			{
				this[MigrationBatchSchema.FailedInitialSyncCount] = value;
			}
		}

		public int FailedIncrementalSyncCount
		{
			get
			{
				return (int)this[MigrationBatchSchema.FailedIncrementalSyncCount];
			}
			internal set
			{
				this[MigrationBatchSchema.FailedIncrementalSyncCount] = value;
			}
		}

		public int PendingCount
		{
			get
			{
				return (int)this[MigrationBatchSchema.PendingCount];
			}
			internal set
			{
				this[MigrationBatchSchema.PendingCount] = value;
			}
		}

		public int ProvisionedCount
		{
			get
			{
				return (int)this[MigrationBatchSchema.ProvisionedCount];
			}
			internal set
			{
				this[MigrationBatchSchema.ProvisionedCount] = value;
			}
		}

		public int ValidationWarningCount
		{
			get
			{
				return (int)this[MigrationBatchSchema.NumValidationErrors];
			}
			internal set
			{
				this[MigrationBatchSchema.NumValidationErrors] = value;
			}
		}

		public MultiValuedProperty<MigrationBatchError> ValidationWarnings
		{
			get
			{
				return (MultiValuedProperty<MigrationBatchError>)this[MigrationBatchSchema.ValidationErrors];
			}
			internal set
			{
				this[MigrationBatchSchema.ValidationErrors] = value;
			}
		}

		public LocalizedString Message
		{
			get
			{
				return (LocalizedString)this[MigrationBatchSchema.Message];
			}
			internal set
			{
				this[MigrationBatchSchema.Message] = value;
			}
		}

		public DateTime CreationDateTime
		{
			get
			{
				return (DateTime)this[MigrationBatchSchema.CreationDateTime];
			}
			internal set
			{
				this[MigrationBatchSchema.CreationDateTime] = value;
			}
		}

		public DateTime CreationDateTimeUTC
		{
			get
			{
				return (DateTime)this[MigrationBatchSchema.CreationDateTimeUTC];
			}
			internal set
			{
				this[MigrationBatchSchema.CreationDateTimeUTC] = value;
			}
		}

		public DateTime? StartDateTime
		{
			get
			{
				return (DateTime?)this[MigrationBatchSchema.StartDateTime];
			}
			internal set
			{
				this[MigrationBatchSchema.StartDateTime] = value;
			}
		}

		public DateTime? StartDateTimeUTC
		{
			get
			{
				return (DateTime?)this[MigrationBatchSchema.StartDateTimeUTC];
			}
			internal set
			{
				this[MigrationBatchSchema.StartDateTimeUTC] = value;
			}
		}

		public DateTime? InitialSyncDateTime
		{
			get
			{
				return (DateTime?)this[MigrationBatchSchema.InitialSyncDateTime];
			}
			internal set
			{
				this[MigrationBatchSchema.InitialSyncDateTime] = value;
			}
		}

		public DateTime? InitialSyncDateTimeUTC
		{
			get
			{
				return (DateTime?)this[MigrationBatchSchema.InitialSyncDateTimeUTC];
			}
			internal set
			{
				this[MigrationBatchSchema.InitialSyncDateTimeUTC] = value;
			}
		}

		public TimeSpan? InitialSyncDuration
		{
			get
			{
				return (TimeSpan?)this[MigrationBatchSchema.InitialSyncDuration];
			}
			internal set
			{
				this[MigrationBatchSchema.InitialSyncDuration] = ((value != null) ? new TimeSpan?(TimeSpan.FromTicks(value.Value.Ticks - value.Value.Ticks % 10000000L)) : null);
			}
		}

		public DateTime? LastSyncedDateTime
		{
			get
			{
				return (DateTime?)this[MigrationBatchSchema.LastSyncedDateTime];
			}
			internal set
			{
				this[MigrationBatchSchema.LastSyncedDateTime] = value;
			}
		}

		public DateTime? LastSyncedDateTimeUTC
		{
			get
			{
				return (DateTime?)this[MigrationBatchSchema.LastSyncedDateTimeUTC];
			}
			internal set
			{
				this[MigrationBatchSchema.LastSyncedDateTimeUTC] = value;
			}
		}

		public DateTime? FinalizedDateTime
		{
			get
			{
				return (DateTime?)this[MigrationBatchSchema.FinalizedDateTime];
			}
			internal set
			{
				this[MigrationBatchSchema.FinalizedDateTime] = value;
			}
		}

		public DateTime? FinalizedDateTimeUTC
		{
			get
			{
				return (DateTime?)this[MigrationBatchSchema.FinalizedDateTimeUTC];
			}
			internal set
			{
				this[MigrationBatchSchema.FinalizedDateTimeUTC] = value;
			}
		}

		public string SubmittedByUser
		{
			get
			{
				return (string)this[MigrationBatchSchema.SubmittedByUser];
			}
			internal set
			{
				this[MigrationBatchSchema.SubmittedByUser] = value;
			}
		}

		public ADObjectId OwnerId
		{
			get
			{
				return (ADObjectId)this[MigrationBatchSchema.OwnerId];
			}
			internal set
			{
				this[MigrationBatchSchema.OwnerId] = value;
			}
		}

		public Guid OwnerExchangeObjectId
		{
			get
			{
				return (Guid)this[MigrationBatchSchema.OwnerExchangeObjectId];
			}
			internal set
			{
				this[MigrationBatchSchema.OwnerExchangeObjectId] = value;
			}
		}

		public MultiValuedProperty<SmtpAddress> NotificationEmails
		{
			get
			{
				return (MultiValuedProperty<SmtpAddress>)this[MigrationBatchSchema.NotificationEmails];
			}
			internal set
			{
				this[MigrationBatchSchema.NotificationEmails] = value;
			}
		}

		public MultiValuedProperty<string> ExcludedFolders
		{
			get
			{
				return (MultiValuedProperty<string>)this[MigrationBatchSchema.ExcludedFolders];
			}
			internal set
			{
				this[MigrationBatchSchema.ExcludedFolders] = value;
			}
		}

		public MigrationType MigrationType
		{
			get
			{
				return (MigrationType)this[MigrationBatchSchema.MigrationType];
			}
			internal set
			{
				this[MigrationBatchSchema.MigrationType] = (int)value;
			}
		}

		public MigrationBatchDirection BatchDirection
		{
			get
			{
				return (MigrationBatchDirection)this[MigrationBatchSchema.BatchDirection];
			}
			internal set
			{
				this[MigrationBatchSchema.BatchDirection] = (int)value;
			}
		}

		public CultureInfo Locale
		{
			get
			{
				return (CultureInfo)this[MigrationBatchSchema.Locale];
			}
			internal set
			{
				this[MigrationBatchSchema.Locale] = value;
			}
		}

		public MultiValuedProperty<MigrationReportSet> Reports
		{
			get
			{
				return (MultiValuedProperty<MigrationReportSet>)this[MigrationBatchSchema.Reports];
			}
			internal set
			{
				this[MigrationBatchSchema.Reports] = value;
			}
		}

		public bool IsProvisioning
		{
			get
			{
				return (bool)this[MigrationBatchSchema.IsProvisioning];
			}
			set
			{
				this[MigrationBatchSchema.IsProvisioning] = value;
			}
		}

		public MigrationBatchFlags BatchFlags
		{
			get
			{
				return (MigrationBatchFlags)this[MigrationBatchSchema.MigrationBatchFlags];
			}
			set
			{
				this[MigrationBatchSchema.MigrationBatchFlags] = (int)value;
			}
		}

		public int? AutoRetryCount
		{
			get
			{
				return (int?)this[MigrationBatchSchema.AutoRetryCount];
			}
			internal set
			{
				this[MigrationBatchSchema.AutoRetryCount] = value;
			}
		}

		public int CurrentRetryCount
		{
			get
			{
				return (int)this[MigrationBatchSchema.CurrentRetryCount];
			}
			internal set
			{
				this[MigrationBatchSchema.CurrentRetryCount] = value;
			}
		}

		public bool AllowUnknownColumnsInCsv
		{
			get
			{
				return (bool)this[MigrationBatchSchema.AllowUnknownColumnsInCsv];
			}
			internal set
			{
				this[MigrationBatchSchema.AllowUnknownColumnsInCsv] = value;
			}
		}

		public string DiagnosticInfo
		{
			get
			{
				return (string)this[MigrationBatchSchema.DiagnosticInfo];
			}
			internal set
			{
				this[MigrationBatchSchema.DiagnosticInfo] = value;
			}
		}

		public MigrationBatchSupportedActions SupportedActions
		{
			get
			{
				return (MigrationBatchSupportedActions)this[MigrationBatchSchema.SupportedActions];
			}
			internal set
			{
				this[MigrationBatchSchema.SupportedActions] = value;
			}
		}

		public MigrationEndpoint SourceEndpoint
		{
			get
			{
				return (MigrationEndpoint)this[MigrationBatchSchema.SourceEndpoint];
			}
			set
			{
				this[MigrationBatchSchema.SourceEndpoint] = value;
			}
		}

		public MigrationEndpoint TargetEndpoint
		{
			get
			{
				return (MigrationEndpoint)this[MigrationBatchSchema.TargetEndpoint];
			}
			set
			{
				this[MigrationBatchSchema.TargetEndpoint] = value;
			}
		}

		public string SourcePublicFolderDatabase
		{
			get
			{
				return (string)this[MigrationBatchSchema.SourcePublicFolderDatabase];
			}
			set
			{
				this[MigrationBatchSchema.SourcePublicFolderDatabase] = value;
			}
		}

		public MultiValuedProperty<string> TargetDatabases
		{
			get
			{
				return (MultiValuedProperty<string>)this[MigrationBatchSchema.TargetDatabases];
			}
			set
			{
				this[MigrationBatchSchema.TargetDatabases] = value;
			}
		}

		public MultiValuedProperty<string> TargetArchiveDatabases
		{
			get
			{
				return (MultiValuedProperty<string>)this[MigrationBatchSchema.TargetArchiveDatabases];
			}
			set
			{
				this[MigrationBatchSchema.TargetArchiveDatabases] = value;
			}
		}

		public Unlimited<int> BadItemLimit
		{
			get
			{
				return (Unlimited<int>)(this[MigrationBatchSchema.BadItemLimit] ?? Unlimited<int>.UnlimitedValue);
			}
			set
			{
				this[MigrationBatchSchema.BadItemLimit] = value;
			}
		}

		public Unlimited<int> LargeItemLimit
		{
			get
			{
				return (Unlimited<int>)(this[MigrationBatchSchema.LargeItemLimit] ?? Unlimited<int>.UnlimitedValue);
			}
			set
			{
				this[MigrationBatchSchema.LargeItemLimit] = value;
			}
		}

		public bool? PrimaryOnly
		{
			get
			{
				return new bool?((bool)(this[MigrationBatchSchema.PrimaryOnly] ?? false));
			}
			set
			{
				this[MigrationBatchSchema.PrimaryOnly] = value;
			}
		}

		public bool? ArchiveOnly
		{
			get
			{
				return new bool?((bool)(this[MigrationBatchSchema.ArchiveOnly] ?? false));
			}
			set
			{
				this[MigrationBatchSchema.ArchiveOnly] = value;
			}
		}

		public string TargetDeliveryDomain
		{
			get
			{
				return (string)this[MigrationBatchSchema.TargetDeliveryDomain];
			}
			set
			{
				this[MigrationBatchSchema.TargetDeliveryDomain] = value;
			}
		}

		public SkippableMigrationSteps SkipSteps
		{
			get
			{
				return (SkippableMigrationSteps)this[MigrationBatchSchema.SkipSteps];
			}
			set
			{
				this[MigrationBatchSchema.SkipSteps] = value;
			}
		}

		public Report Report { get; set; }

		public DateTime? StartAfter
		{
			get
			{
				return (DateTime?)this[MigrationBatchSchema.StartAfter];
			}
			set
			{
				this[MigrationBatchSchema.StartAfter] = value;
			}
		}

		public DateTime? StartAfterUTC
		{
			get
			{
				return (DateTime?)this[MigrationBatchSchema.StartAfterUTC];
			}
			set
			{
				this[MigrationBatchSchema.StartAfterUTC] = value;
			}
		}

		public DateTime? CompleteAfter
		{
			get
			{
				return (DateTime?)this[MigrationBatchSchema.CompleteAfter];
			}
			set
			{
				this[MigrationBatchSchema.CompleteAfter] = value;
			}
		}

		public DateTime? CompleteAfterUTC
		{
			get
			{
				return (DateTime?)this[MigrationBatchSchema.CompleteAfterUTC];
			}
			set
			{
				this[MigrationBatchSchema.CompleteAfterUTC] = value;
			}
		}

		internal MigrationFlags Flags { get; set; }

		internal Stream CsvStream
		{
			get
			{
				return this.csvStream;
			}
			set
			{
				this.csvStream = value;
			}
		}

		internal DateTime SubscriptionSettingsModified
		{
			get
			{
				return this.subscriptionSettingsModified;
			}
			set
			{
				this.subscriptionSettingsModified = value;
			}
		}

		internal ExTimeZoneValue UserTimeZone
		{
			get
			{
				return this.userTimeZone;
			}
			set
			{
				this.userTimeZone = value;
			}
		}

		internal Guid? OriginalBatchId
		{
			get
			{
				return (Guid?)this[MigrationBatchSchema.OriginalBatchId];
			}
			set
			{
				this[MigrationBatchSchema.OriginalBatchId] = value;
			}
		}

		internal DateTime OriginalCreationTime
		{
			get
			{
				return this.originalCreationTime;
			}
			set
			{
				this.originalCreationTime = value;
			}
		}

		internal bool OriginalStatisticsEnabled
		{
			get
			{
				return this.originalStatisticsEnabled;
			}
			set
			{
				this.originalStatisticsEnabled = value;
			}
		}

		internal string TargetDomainName
		{
			get
			{
				return this.targetDomainName;
			}
			set
			{
				this.targetDomainName = value;
			}
		}

		internal SubmittedByUserAdminType SubmittedByUserAdminType
		{
			get
			{
				return this.submittedByUserAdminType;
			}
			set
			{
				this.submittedByUserAdminType = value;
			}
		}

		internal string DelegatedAdminOwner
		{
			get
			{
				return (string)this[MigrationBatchSchema.DelegatedAdminOwner];
			}
			set
			{
				this[MigrationBatchSchema.DelegatedAdminOwner] = value;
			}
		}

		internal TimeSpan? ReportInterval
		{
			get
			{
				return (TimeSpan?)this[MigrationBatchSchema.ReportInterval];
			}
			set
			{
				this[MigrationBatchSchema.ReportInterval] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<MigrationBatchSchema>();
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		internal static SkippableMigrationSteps ConvertStepsArrayToFlags(SkippableMigrationSteps[] steps)
		{
			SkippableMigrationSteps skippableMigrationSteps = SkippableMigrationSteps.None;
			if (steps != null)
			{
				foreach (SkippableMigrationSteps skippableMigrationSteps2 in steps)
				{
					skippableMigrationSteps |= skippableMigrationSteps2;
				}
			}
			return skippableMigrationSteps;
		}

		[NonSerialized]
		private Stream csvStream;

		[NonSerialized]
		private DateTime subscriptionSettingsModified;

		[NonSerialized]
		private ExTimeZoneValue userTimeZone;

		[NonSerialized]
		private DateTime originalCreationTime;

		[NonSerialized]
		private SubmittedByUserAdminType submittedByUserAdminType;

		[NonSerialized]
		private bool originalStatisticsEnabled;

		[NonSerialized]
		private string targetDomainName;
	}
}
