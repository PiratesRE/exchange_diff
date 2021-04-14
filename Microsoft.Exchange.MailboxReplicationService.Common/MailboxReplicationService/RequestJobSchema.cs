using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class RequestJobSchema : SimpleProviderObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition UserId = new SimpleProviderPropertyDefinition("UserId", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition DisplayName = new SimpleProviderPropertyDefinition("DisplayName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Alias = new SimpleProviderPropertyDefinition("Alias", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SourceAlias = new SimpleProviderPropertyDefinition("SourceAlias", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TargetAlias = new SimpleProviderPropertyDefinition("TargetAlias", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SourceDatabase = new SimpleProviderPropertyDefinition("SourceDatabase", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SourceVersion = new SimpleProviderPropertyDefinition("SourceVersion", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SourceServer = new SimpleProviderPropertyDefinition("SourceServer", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TargetDatabase = new SimpleProviderPropertyDefinition("TargetDatabase", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TargetVersion = new SimpleProviderPropertyDefinition("TargetVersion", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TargetServer = new SimpleProviderPropertyDefinition("TargetServer", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TargetContainerGuid = new SimpleProviderPropertyDefinition("TargetContainerGuid", ExchangeObjectVersion.Exchange2012, typeof(Guid?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TargetUnifiedMailboxId = new SimpleProviderPropertyDefinition("TargetUnifiedMailboxId", ExchangeObjectVersion.Exchange2012, typeof(CrossTenantObjectId), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RequestQueue = new SimpleProviderPropertyDefinition("RequestQueue", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SourceArchiveDatabase = new SimpleProviderPropertyDefinition("SourceArchiveDatabase", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SourceArchiveVersion = new SimpleProviderPropertyDefinition("SourceArchiveVersion", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SourceArchiveServer = new SimpleProviderPropertyDefinition("SourceArchiveServer", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TargetArchiveDatabase = new SimpleProviderPropertyDefinition("TargetArchiveDatabase", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TargetArchiveVersion = new SimpleProviderPropertyDefinition("TargetArchiveVersion", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TargetArchiveServer = new SimpleProviderPropertyDefinition("TargetArchiveServer", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ArchiveDomain = new SimpleProviderPropertyDefinition("ArchiveDomain", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ExchangeGuid = new SimpleProviderPropertyDefinition("ExchangeGuid", ExchangeObjectVersion.Exchange2010, typeof(Guid), PropertyDefinitionFlags.None, Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SourceExchangeGuid = new SimpleProviderPropertyDefinition("SourceExchangeGuid", ExchangeObjectVersion.Exchange2010, typeof(Guid), PropertyDefinitionFlags.None, Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TargetExchangeGuid = new SimpleProviderPropertyDefinition("TargetExchangeGuid", ExchangeObjectVersion.Exchange2010, typeof(Guid), PropertyDefinitionFlags.None, Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ArchiveGuid = new SimpleProviderPropertyDefinition("ArchiveGuid", ExchangeObjectVersion.Exchange2010, typeof(Guid?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SourceIsArchive = new SimpleProviderPropertyDefinition("SourceIsArchive", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TargetIsArchive = new SimpleProviderPropertyDefinition("TargetIsArchive", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SourceRootFolder = new SimpleProviderPropertyDefinition("SourceRootFolder", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 1024)
		}, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TargetRootFolder = new SimpleProviderPropertyDefinition("TargetRootFolder", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 1024)
		}, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition IncludeFolders = new SimpleProviderPropertyDefinition("IncludeFolders", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.MultiValued, null, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 1024)
		}, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ExcludeFolders = new SimpleProviderPropertyDefinition("ExcludeFolders", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.MultiValued, null, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 1024)
		}, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ExcludeDumpster = new SimpleProviderPropertyDefinition("ExcludeDumpster", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RequestGuid = new SimpleProviderPropertyDefinition("RequestGuid", ExchangeObjectVersion.Exchange2010, typeof(Guid), PropertyDefinitionFlags.None, Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Status = new SimpleProviderPropertyDefinition("Status", ExchangeObjectVersion.Exchange2010, typeof(RequestStatus), PropertyDefinitionFlags.None, RequestStatus.None, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(RequestStatus))
		}, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Flags = new SimpleProviderPropertyDefinition("Flags", ExchangeObjectVersion.Exchange2010, typeof(RequestFlags), PropertyDefinitionFlags.None, RequestFlags.None, new PropertyDefinitionConstraint[]
		{
			new DelegateConstraint(new ValidationDelegate(ConstraintDelegates.ValidateMailboxMoveFlags))
		}, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RemoteHostName = new SimpleProviderPropertyDefinition("RemoteHostName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 1024)
		}, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RemoteHostPort = new SimpleProviderPropertyDefinition("RemoteHostPort", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SmtpServerName = new SimpleProviderPropertyDefinition("SmtpServerName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 1024)
		}, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SmtpServerPort = new SimpleProviderPropertyDefinition("SmtpServerPort", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SecurityMechanism = new SimpleProviderPropertyDefinition("SecurityMechanism", ExchangeObjectVersion.Exchange2010, typeof(IMAPSecurityMechanism), PropertyDefinitionFlags.None, IMAPSecurityMechanism.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SyncProtocol = new SimpleProviderPropertyDefinition("SyncProtocol", ExchangeObjectVersion.Exchange2010, typeof(SyncProtocol), PropertyDefinitionFlags.None, Microsoft.Exchange.MailboxReplicationService.SyncProtocol.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition EmailAddress = new SimpleProviderPropertyDefinition("EmailAddress", ExchangeObjectVersion.Exchange2010, typeof(SmtpAddress), PropertyDefinitionFlags.None, SmtpAddress.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition IncrementalSyncInterval = new SimpleProviderPropertyDefinition("IncrementalSyncInterval", ExchangeObjectVersion.Exchange2010, typeof(TimeSpan), PropertyDefinitionFlags.None, TimeSpan.Zero, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition BatchName = new SimpleProviderPropertyDefinition("BatchName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 255)
		}, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RequestJobState = new SimpleProviderPropertyDefinition("RequestJobState", ExchangeObjectVersion.Exchange2010, typeof(JobProcessingState), PropertyDefinitionFlags.None, JobProcessingState.NotReady, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RemoteDatabaseName = new SimpleProviderPropertyDefinition("RemoteDatabaseName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RemoteDatabaseGuid = new SimpleProviderPropertyDefinition("RemoteDatabaseGuid", ExchangeObjectVersion.Exchange2010, typeof(Guid?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RemoteArchiveDatabaseName = new SimpleProviderPropertyDefinition("RemoteArchiveDatabaseName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RemoteArchiveDatabaseGuid = new SimpleProviderPropertyDefinition("RemoteArchiveDatabaseGuid", ExchangeObjectVersion.Exchange2010, typeof(Guid?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition BadItemLimit = new SimpleProviderPropertyDefinition("BadItemLimit", ExchangeObjectVersion.Exchange2010, typeof(Unlimited<int>), PropertyDefinitionFlags.None, Unlimited<int>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition BadItemsEncountered = new SimpleProviderPropertyDefinition("BadItemsEncountered", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition LargeItemLimit = new SimpleProviderPropertyDefinition("LargeItemLimit", ExchangeObjectVersion.Exchange2010, typeof(Unlimited<int>), PropertyDefinitionFlags.None, Unlimited<int>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition LargeItemsEncountered = new SimpleProviderPropertyDefinition("LargeItemsEncountered", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition MissingItemsEncountered = new SimpleProviderPropertyDefinition("MissingItemsEncountered", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition AllowLargeItems = new SimpleProviderPropertyDefinition("AllowLargeItems", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition MRSServerName = new SimpleProviderPropertyDefinition("MRSServerName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TotalMailboxItemCount = new SimpleProviderPropertyDefinition("TotalMailboxItemCount", ExchangeObjectVersion.Exchange2010, typeof(ulong), PropertyDefinitionFlags.PersistDefaultValue, 0UL, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TotalArchiveItemCount = new SimpleProviderPropertyDefinition("TotalArchiveItemCount", ExchangeObjectVersion.Exchange2010, typeof(ulong?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ItemsTransferred = new SimpleProviderPropertyDefinition("ItemsTransferred", ExchangeObjectVersion.Exchange2010, typeof(ulong?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition PercentComplete = new SimpleProviderPropertyDefinition("PercentComplete", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition FailureCode = new SimpleProviderPropertyDefinition("FailureCode", ExchangeObjectVersion.Exchange2010, typeof(int?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition FailureType = new SimpleProviderPropertyDefinition("FailureType", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition FailureSide = new SimpleProviderPropertyDefinition("FailureSide", ExchangeObjectVersion.Exchange2010, typeof(ExceptionSide?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Message = new SimpleProviderPropertyDefinition("Message", ExchangeObjectVersion.Exchange2010, typeof(LocalizedString), PropertyDefinitionFlags.None, LocalizedString.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RemoteCredentialUsername = new SimpleProviderPropertyDefinition("RemoteCredentialUsername", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RemoteOrgName = new SimpleProviderPropertyDefinition("RemoteOrgName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition AllowedToFinishMove = new SimpleProviderPropertyDefinition("AllowedToFinishJob", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition PreserveMailboxSignature = new SimpleProviderPropertyDefinition("PreserveMailboxSignature", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.PersistDefaultValue, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition CancelRequest = new SimpleProviderPropertyDefinition("CancelRequest", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition DomainControllerToUpdate = new SimpleProviderPropertyDefinition("DomainControllerToUpdate", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.PersistDefaultValue, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RemoteDomainControllerToUpdate = new SimpleProviderPropertyDefinition("RemoteDomainControllerToUpdate", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.PersistDefaultValue, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SyncStage = new SimpleProviderPropertyDefinition("SyncStage", ExchangeObjectVersion.Exchange2010, typeof(SyncStage), PropertyDefinitionFlags.None, Microsoft.Exchange.MailboxReplicationService.SyncStage.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SourceDCName = new SimpleProviderPropertyDefinition("SourceDCName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TargetDCName = new SimpleProviderPropertyDefinition("TargetDCName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TotalMailboxSize = new SimpleProviderPropertyDefinition("TotalMailboxSize", ExchangeObjectVersion.Exchange2010, typeof(ulong), PropertyDefinitionFlags.PersistDefaultValue, 0UL, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TotalArchiveSize = new SimpleProviderPropertyDefinition("TotalArchiveSize", ExchangeObjectVersion.Exchange2010, typeof(ulong?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition BytesTransferred = new SimpleProviderPropertyDefinition("BytesTransferred", ExchangeObjectVersion.Exchange2010, typeof(ulong?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RetryCount = new SimpleProviderPropertyDefinition("RetryCount", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TotalRetryCount = new SimpleProviderPropertyDefinition("TotalRetryCount", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition UserOrgName = new SimpleProviderPropertyDefinition("UserOrgName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TargetDeliveryDomain = new SimpleProviderPropertyDefinition("TargetDeliveryDomain", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition IgnoreRuleLimitErrors = new SimpleProviderPropertyDefinition("IgnoreRuleLimitErrors", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition JobType = new SimpleProviderPropertyDefinition("JobType", ExchangeObjectVersion.Exchange2010, typeof(MRSJobType), PropertyDefinitionFlags.None, MRSJobType.RequestJobE14R3, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RequestType = new SimpleProviderPropertyDefinition("RequestType", ExchangeObjectVersion.Exchange2010, typeof(MRSRequestType), PropertyDefinitionFlags.PersistDefaultValue, MRSRequestType.Move, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Name = new SimpleProviderPropertyDefinition("RequestName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition FilePath = new SimpleProviderPropertyDefinition("FilePath", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition MailboxRestoreFlags = new SimpleProviderPropertyDefinition("MailboxRestoreFlags", ExchangeObjectVersion.Exchange2010, typeof(MailboxRestoreType?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SourceUserId = new SimpleProviderPropertyDefinition("SourceUser", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TargetUserId = new SimpleProviderPropertyDefinition("TargetUser", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RemoteUserName = new SimpleProviderPropertyDefinition("RemoteUserName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RemoteMailboxLegacyDN = new SimpleProviderPropertyDefinition("RemoteMailboxLegacyDN", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RemoteMailboxServerLegacyDN = new SimpleProviderPropertyDefinition("RemoteMailboxServerLegacyDN", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RemoteUserLegacyDN = new SimpleProviderPropertyDefinition("RemoteUserLegacyDN", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition OutlookAnywhereHostName = new SimpleProviderPropertyDefinition("OutlookAnywhereHostName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition AuthMethod = new SimpleProviderPropertyDefinition("AuthMethod", ExchangeObjectVersion.Exchange2010, typeof(AuthenticationMethod?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition IsAdministrativeCredential = new SimpleProviderPropertyDefinition("IsAdministrativeCredential", ExchangeObjectVersion.Exchange2010, typeof(bool?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ConflictResolutionOption = new SimpleProviderPropertyDefinition("ConflictResolutionOption", ExchangeObjectVersion.Exchange2010, typeof(ConflictResolutionOption?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition AssociatedMessagesCopyOption = new SimpleProviderPropertyDefinition("AssociatedMessagesCopyOption", ExchangeObjectVersion.Exchange2010, typeof(FAICopyOption?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition OrganizationalUnitRoot = new SimpleProviderPropertyDefinition("OrganizationalUnitRoot", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ConfigurationUnit = new SimpleProviderPropertyDefinition("ConfigurationUnit", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition OrganizationId = new SimpleProviderPropertyDefinition("OrganizationId", ExchangeObjectVersion.Exchange2010, typeof(OrganizationId), PropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new SimpleProviderPropertyDefinition[]
		{
			RequestJobSchema.OrganizationalUnitRoot,
			RequestJobSchema.ConfigurationUnit
		}, null, new GetterDelegate(RequestJobBase.OrganizationIdGetter), new SetterDelegate(RequestJobBase.OrganizationIdSetter));

		public static readonly SimpleProviderPropertyDefinition ContentFilter = new SimpleProviderPropertyDefinition("ContentFilter", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ContentFilterLCID = new SimpleProviderPropertyDefinition("ContentFilterLCID", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, CultureInfo.InvariantCulture.LCID, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Priority = new SimpleProviderPropertyDefinition("Priority", ExchangeObjectVersion.Exchange2010, typeof(RequestPriority), PropertyDefinitionFlags.PersistDefaultValue, RequestPriority.Normal, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition WorkloadType = new SimpleProviderPropertyDefinition("WorkloadType", ExchangeObjectVersion.Exchange2010, typeof(RequestWorkloadType), PropertyDefinitionFlags.PersistDefaultValue, RequestWorkloadType.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition JobInternalFlags = new SimpleProviderPropertyDefinition("JobInternalFlags", ExchangeObjectVersion.Exchange2010, typeof(RequestJobInternalFlags), PropertyDefinitionFlags.PersistDefaultValue, RequestJobInternalFlags.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition CompletedRequestAgeLimit = new SimpleProviderPropertyDefinition("CompletedRequestAgeLimit", ExchangeObjectVersion.Exchange2010, typeof(Unlimited<EnhancedTimeSpan>), PropertyDefinitionFlags.None, Unlimited<EnhancedTimeSpan>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.Zero, EnhancedTimeSpan.FromSeconds(2147483647.0)),
			new UnlimitedEnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RequestCreator = new SimpleProviderPropertyDefinition("RequestCreator", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RehomeRequest = new SimpleProviderPropertyDefinition("RehomeRequest", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition PoisonCount = new SimpleProviderPropertyDefinition("PoisonCount", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ContentCodePage = new SimpleProviderPropertyDefinition("ContentCodePage", ExchangeObjectVersion.Exchange2010, typeof(int?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RecipientTypeDetails = new SimpleProviderPropertyDefinition("RecipientTypeDetails", ExchangeObjectVersion.Exchange2010, typeof(long), PropertyDefinitionFlags.PersistDefaultValue, 0L, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition LastPickupTime = new SimpleProviderPropertyDefinition("LastPickupTime", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RestartingAfterSignatureChange = new SimpleProviderPropertyDefinition("RestartingAfterSignatureChange", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition IsIntegData = new SimpleProviderPropertyDefinition("IsIntegData", ExchangeObjectVersion.Exchange2010, typeof(int?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition UserPuid = new SimpleProviderPropertyDefinition("UserPuid", ExchangeObjectVersion.Exchange2010, typeof(long?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition OlcDGroup = new SimpleProviderPropertyDefinition("OlcDGroup", ExchangeObjectVersion.Exchange2010, typeof(int?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
