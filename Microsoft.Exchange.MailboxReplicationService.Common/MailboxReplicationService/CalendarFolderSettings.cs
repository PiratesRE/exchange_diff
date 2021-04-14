using System;
using System.Collections;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal class CalendarFolderSettings : ItemPropertiesBase
	{
		public OlcCalendarType OlcCalendarType
		{
			get
			{
				return (OlcCalendarType)this.CalendarType;
			}
		}

		public override void Apply(CoreFolder folder)
		{
			using (UserConfiguration mailboxConfiguration = this.GetMailboxConfiguration(folder))
			{
				IDictionary dictionary = mailboxConfiguration.GetDictionary();
				dictionary["OlcIsVisible"] = this.IsVisible;
				dictionary["OlcIsHidden"] = this.IsHidden;
				dictionary["OlcColorIndex"] = this.ColorIndex;
				dictionary["OlcIsDailySummaryEnabled"] = this.IsDailySummaryEnabled;
				dictionary["ConsumerTaskPermissionLevel"] = this.ConsumerTaskPermissionLevel;
				dictionary["OlcConsecutiveErrorCount"] = this.ConsecutiveErrorCount;
				dictionary["OlcTotalErrorCount"] = this.TotalErrorCount;
				dictionary["OlcPollingInterval"] = this.PollingInterval;
				dictionary["OlcEntityTag"] = (this.EntityTag ?? string.Empty);
				dictionary["OlcImportedEventCount"] = this.ImportedEventCount;
				dictionary["OlcTotalEventCount"] = this.TotalEventCount;
				dictionary["OlcUpdateStatus"] = this.UpdateStatus;
				dictionary["OlcMissingUidCount"] = this.MissingUidCount;
				dictionary["OlcConsecutiveCriticalErrorCount"] = this.ConsecutiveCriticalErrorCount;
				dictionary["OlcPersonIdMigrated"] = this.PersonIdMigrated;
				ConflictResolutionResult conflictResolutionResult = mailboxConfiguration.Save(SaveMode.ResolveConflicts);
				if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
				{
					throw new ConversionFailedException(ConversionFailureReason.ConverterInternalFailure, MrsStrings.ReportCalendarFolderFaiSaveFailed, null);
				}
				MrsTracer.Provider.Debug("Calendar folder user configuration has been updated.", new object[0]);
			}
			folder.PropertyBag.Load(CalendarFolderSchema.Instance.AutoloadProperties);
			if (this.OlcCalendarType != OlcCalendarType.RegularEvents)
			{
				folder.PropertyBag[FolderSchema.DisplayName] = (this.Name ?? string.Empty);
			}
			folder.PropertyBag[CalendarFolderSchema.CharmId] = (this.CharmId ?? string.Empty);
			FolderSaveResult folderSaveResult = folder.Save(SaveMode.NoConflictResolution);
			if (folderSaveResult.OperationResult == OperationResult.Failed)
			{
				throw new ConversionFailedException(ConversionFailureReason.ConverterInternalFailure, MrsStrings.ReportCalendarFolderSaveFailed, null);
			}
			MrsTracer.Provider.Debug("Calendar folder has been updated with settings.", new object[0]);
			folder.PropertyBag.Load(CalendarFolderSchema.Instance.AutoloadProperties);
		}

		protected UserConfiguration GetMailboxConfiguration(CoreFolder folder)
		{
			UserConfigurationManager userConfigurationManager = ((MailboxSession)folder.Session).UserConfigurationManager;
			UserConfiguration result;
			try
			{
				result = userConfigurationManager.GetFolderConfiguration("OlcCalendarFolderSettings", UserConfigurationTypes.Dictionary, folder.Id);
			}
			catch (ObjectNotFoundException)
			{
				result = userConfigurationManager.CreateFolderConfiguration("OlcCalendarFolderSettings", UserConfigurationTypes.Dictionary, folder.Id);
			}
			return result;
		}

		[DataMember(Name = "CalendarType")]
		public int CalendarType { get; set; }

		[DataMember(Name = "HolidayLocale")]
		public string HolidayLocale { get; set; }

		[DataMember(Name = "ExternalCalendarLocation")]
		public string ExternalCalendarLocation { get; set; }

		[DataMember(Name = "IsVisible")]
		public bool IsVisible { get; set; }

		[DataMember(Name = "IsHidden")]
		public bool IsHidden { get; set; }

		[DataMember(Name = "ColorIndex")]
		public int ColorIndex { get; set; }

		[DataMember(Name = "Name")]
		public string Name { get; set; }

		[DataMember(Name = "CharmId")]
		public string CharmId { get; set; }

		[DataMember(Name = "IsDailySummaryEnabled")]
		public bool IsDailySummaryEnabled { get; set; }

		[DataMember(Name = "ConsumerTaskPermissionLevel")]
		public int ConsumerTaskPermissionLevel { get; set; }

		[DataMember(Name = "Ordinal")]
		public int Ordinal { get; set; }

		[DataMember(Name = "EmailRemindersDisabled")]
		public bool EmailRemindersDisabled { get; set; }

		[DataMember(Name = "ConsecutiveErrorCount")]
		public int ConsecutiveErrorCount { get; set; }

		[DataMember(Name = "TotalErrorCount")]
		public int TotalErrorCount { get; set; }

		[DataMember(Name = "PollingInterval")]
		public int PollingInterval { get; set; }

		[DataMember(Name = "EntityTag")]
		public string EntityTag { get; set; }

		[DataMember(Name = "ImportedEventCount")]
		public int ImportedEventCount { get; set; }

		[DataMember(Name = "TotalEventCount")]
		public int TotalEventCount { get; set; }

		[DataMember(Name = "UpdateStatus")]
		public int UpdateStatus { get; set; }

		[DataMember(Name = "ExternalIdOfCalendar")]
		public string ExternalIdOfCalendar { get; set; }

		[DataMember(Name = "MissingUidCount")]
		public int MissingUidCount { get; set; }

		[DataMember(Name = "ConsecutiveCriticalErrorCount")]
		public int ConsecutiveCriticalErrorCount { get; set; }

		[DataMember(Name = "PersonIdMigrated")]
		public bool PersonIdMigrated { get; set; }

		[DataMember(Name = "ConsumerSharingCalendarSubscriptionCount")]
		public int ConsumerSharingCalendarSubscriptionCount { get; set; }

		[DataMember(Name = "ConsumerCalendarGuid")]
		public string ConsumerCalendarGuid { get; set; }

		[DataMember(Name = "ConsumerCalendarOwnerId")]
		public long ConsumerCalendarOwnerId { get; set; }

		[DataMember(Name = "ConsumerCalendarPrivateFreeBusyId")]
		public string ConsumerCalendarPrivateFreeBusyId { get; set; }

		[DataMember(Name = "ConsumerCalendarPrivateDetailId")]
		public string ConsumerCalendarPrivateDetailId { get; set; }

		[DataMember(Name = "ConsumerCalendarPublishVisibility")]
		public int ConsumerCalendarPublishVisibility { get; set; }

		[DataMember(Name = "ConsumerCalendarSharingInvitations")]
		public string ConsumerCalendarSharingInvitations { get; set; }

		[DataMember(Name = "ConsumerCalendarPermissionLevel")]
		public int ConsumerCalendarPermissionLevel { get; set; }

		[DataMember(Name = "ConsumerCalendarSynchronizationState")]
		public string ConsumerCalendarSynchronizationState { get; set; }

		public const string ConfigurationName = "OlcCalendarFolderSettings";

		public const string OlcIsVisible = "OlcIsVisible";

		public const string OlcIsHidden = "OlcIsHidden";

		public const string OlcColorIndex = "OlcColorIndex";

		public const string OlcConsumerTaskPermissionLevel = "ConsumerTaskPermissionLevel";

		public const string OlcIsDailySummaryEnabled = "OlcIsDailySummaryEnabled";

		public const string OlcConsecutiveErrorCount = "OlcConsecutiveErrorCount";

		public const string OlcTotalErrorCount = "OlcTotalErrorCount";

		public const string OlcPollingInterval = "OlcPollingInterval";

		public const string OlcEntityTag = "OlcEntityTag";

		public const string OlcImportedEventCount = "OlcImportedEventCount";

		public const string OlcTotalEventCount = "OlcTotalEventCount";

		public const string OlcUpdateStatus = "OlcUpdateStatus";

		public const string OlcMissingUidCount = "OlcMissingUidCount";

		public const string OlcConsecutiveCriticalErrorCount = "OlcConsecutiveCriticalErrorCount";

		public const string OlcPersonIdMigrated = "OlcPersonIdMigrated";
	}
}
