using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public class RequestJobBase : ConfigurableObject, ISettingsContextProvider
	{
		public RequestJobBase() : this(new SimpleProviderPropertyBag())
		{
		}

		internal RequestJobBase(SimpleProviderPropertyBag propertyBag) : base(propertyBag)
		{
			this.isFake = false;
			this.isRetired = false;
			this.timeTracker = new RequestJobTimeTracker();
			this.ProgressTracker = new TransferProgressTracker();
			this.indexEntries = new List<IRequestIndexEntry>();
			this.indexIds = new List<RequestIndexId>();
			this.folderToMailboxMapping = new List<FolderToMailboxMapping>();
			this.folderList = new List<MoveFolderInfo>();
			this.jobConfigContext = new Lazy<SettingsContextBase>(() => this.CreateConfigContext());
		}

		public override bool IsValid
		{
			get
			{
				return base.IsValid && this.ValidationResult == RequestJobBase.ValidationResultEnum.Valid;
			}
		}

		protected internal string DiagnosticInfo { get; protected set; }

		internal ADUser User
		{
			get
			{
				return this.adUser;
			}
			set
			{
				this.adUser = value;
			}
		}

		internal ADUser TargetUser
		{
			get
			{
				return this.targetUser;
			}
			set
			{
				this.targetUser = value;
			}
		}

		internal ADUser SourceUser
		{
			get
			{
				return this.sourceUser;
			}
			set
			{
				this.sourceUser = value;
			}
		}

		internal List<IRequestIndexEntry> IndexEntries
		{
			get
			{
				return this.indexEntries;
			}
			set
			{
				this.indexEntries = value;
			}
		}

		internal RequestJobBase.ValidationResultEnum? ValidationResult
		{
			get
			{
				return this.validationResult;
			}
			set
			{
				this.validationResult = value;
			}
		}

		internal LocalizedString ValidationMessage
		{
			get
			{
				return this.validationMessage;
			}
			set
			{
				this.validationMessage = value;
			}
		}

		internal Guid OriginatingMDBGuid
		{
			get
			{
				return this.originatingMDBGuid;
			}
			set
			{
				this.originatingMDBGuid = value;
			}
		}

		internal bool IsFake
		{
			get
			{
				return this.isFake;
			}
			set
			{
				this.isFake = value;
			}
		}

		internal List<RequestIndexId> IndexIds
		{
			get
			{
				return this.indexIds;
			}
			set
			{
				this.indexIds = value;
			}
		}

		internal RequestJobTimeTracker TimeTracker
		{
			get
			{
				return this.timeTracker;
			}
			set
			{
				this.timeTracker = value;
			}
		}

		internal TransferProgressTracker ProgressTracker { get; set; }

		internal SkippedItemCounts SkippedItemCounts
		{
			get
			{
				return this.skippedItemCounts;
			}
			set
			{
				this.skippedItemCounts = value;
			}
		}

		internal FailureHistory FailureHistory
		{
			get
			{
				return this.failureHistory;
			}
			set
			{
				this.failureHistory = value;
			}
		}

		internal new RequestJobObjectId Identity
		{
			get
			{
				return (RequestJobObjectId)this[SimpleProviderObjectSchema.Identity];
			}
			set
			{
				this[SimpleProviderObjectSchema.Identity] = value;
			}
		}

		internal ADObjectId UserId
		{
			get
			{
				return (ADObjectId)this[RequestJobSchema.UserId];
			}
			set
			{
				this[RequestJobSchema.UserId] = value;
			}
		}

		internal string DistinguishedName
		{
			get
			{
				if (this.UserId == null)
				{
					return null;
				}
				if (!string.IsNullOrEmpty(this.UserId.DistinguishedName))
				{
					return this.UserId.DistinguishedName;
				}
				if (this.UserId.ObjectGuid != Guid.Empty)
				{
					return string.Format("<GUID={0}>", this.UserId.ObjectGuid);
				}
				return null;
			}
			set
			{
				if (this.UserId == null)
				{
					this.UserId = new ADObjectId(value);
					return;
				}
				this.UserId = new ADObjectId(value, this.UserId.ObjectGuid);
			}
		}

		internal string DisplayName
		{
			get
			{
				return (string)this[RequestJobSchema.DisplayName];
			}
			set
			{
				this[RequestJobSchema.DisplayName] = value;
			}
		}

		internal string Alias
		{
			get
			{
				return (string)this[RequestJobSchema.Alias];
			}
			set
			{
				this[RequestJobSchema.Alias] = value;
			}
		}

		internal string SourceAlias
		{
			get
			{
				return (string)this[RequestJobSchema.SourceAlias];
			}
			set
			{
				this[RequestJobSchema.SourceAlias] = value;
			}
		}

		internal string TargetAlias
		{
			get
			{
				return (string)this[RequestJobSchema.TargetAlias];
			}
			set
			{
				this[RequestJobSchema.TargetAlias] = value;
			}
		}

		internal Guid ExchangeGuid
		{
			get
			{
				return (Guid)this[RequestJobSchema.ExchangeGuid];
			}
			set
			{
				this[RequestJobSchema.ExchangeGuid] = value;
			}
		}

		internal Guid SourceExchangeGuid
		{
			get
			{
				return (Guid)this[RequestJobSchema.SourceExchangeGuid];
			}
			set
			{
				this[RequestJobSchema.SourceExchangeGuid] = value;
			}
		}

		internal Guid TargetExchangeGuid
		{
			get
			{
				return (Guid)this[RequestJobSchema.TargetExchangeGuid];
			}
			set
			{
				this[RequestJobSchema.TargetExchangeGuid] = value;
			}
		}

		internal Guid? ArchiveGuid
		{
			get
			{
				return (Guid?)this[RequestJobSchema.ArchiveGuid];
			}
			set
			{
				this[RequestJobSchema.ArchiveGuid] = value;
			}
		}

		internal string SourceRootFolder
		{
			get
			{
				return (string)this[RequestJobSchema.SourceRootFolder];
			}
			set
			{
				this[RequestJobSchema.SourceRootFolder] = value;
			}
		}

		internal string TargetRootFolder
		{
			get
			{
				return (string)this[RequestJobSchema.TargetRootFolder];
			}
			set
			{
				this[RequestJobSchema.TargetRootFolder] = value;
			}
		}

		internal bool SourceIsArchive
		{
			get
			{
				return (bool)this[RequestJobSchema.SourceIsArchive];
			}
			set
			{
				this[RequestJobSchema.SourceIsArchive] = value;
			}
		}

		internal bool TargetIsArchive
		{
			get
			{
				return (bool)this[RequestJobSchema.TargetIsArchive];
			}
			set
			{
				this[RequestJobSchema.TargetIsArchive] = value;
			}
		}

		internal string[] IncludeFolders
		{
			get
			{
				return ((MultiValuedProperty<string>)this[RequestJobSchema.IncludeFolders]).ToArray();
			}
			set
			{
				this[RequestJobSchema.IncludeFolders] = value;
			}
		}

		internal string[] ExcludeFolders
		{
			get
			{
				return ((MultiValuedProperty<string>)this[RequestJobSchema.ExcludeFolders]).ToArray();
			}
			set
			{
				this[RequestJobSchema.ExcludeFolders] = value;
			}
		}

		internal bool ExcludeDumpster
		{
			get
			{
				return (bool)this[RequestJobSchema.ExcludeDumpster];
			}
			set
			{
				this[RequestJobSchema.ExcludeDumpster] = value;
			}
		}

		internal Guid RequestGuid
		{
			get
			{
				return (Guid)this[RequestJobSchema.RequestGuid];
			}
			set
			{
				this[RequestJobSchema.RequestGuid] = value;
			}
		}

		internal RequestStatus Status
		{
			get
			{
				return (RequestStatus)this[RequestJobSchema.Status];
			}
			set
			{
				this[RequestJobSchema.Status] = value;
			}
		}

		internal RequestState StatusDetail
		{
			get
			{
				return this.TimeTracker.CurrentState;
			}
		}

		internal RequestFlags Flags
		{
			get
			{
				RequestFlags requestFlags = (RequestFlags)this[RequestJobSchema.Flags];
				if (this.Priority > RequestPriority.Normal)
				{
					requestFlags |= RequestFlags.HighPriority;
				}
				return requestFlags;
			}
			set
			{
				value &= ~RequestFlags.HighPriority;
				this[RequestJobSchema.Flags] = value;
			}
		}

		internal RecipientTypeDetails RecipientTypeDetails
		{
			get
			{
				return (RecipientTypeDetails)this[RequestJobSchema.RecipientTypeDetails];
			}
			set
			{
				this[RequestJobSchema.RecipientTypeDetails] = (long)value;
			}
		}

		internal int SourceVersion
		{
			get
			{
				return (int)this[RequestJobSchema.SourceVersion];
			}
			set
			{
				this[RequestJobSchema.SourceVersion] = value;
			}
		}

		internal ADObjectId SourceDatabase
		{
			get
			{
				return (ADObjectId)this[RequestJobSchema.SourceDatabase];
			}
			set
			{
				this[RequestJobSchema.SourceDatabase] = value;
			}
		}

		internal string SourceServer
		{
			get
			{
				return (string)this[RequestJobSchema.SourceServer];
			}
			set
			{
				this[RequestJobSchema.SourceServer] = value;
			}
		}

		internal int TargetVersion
		{
			get
			{
				return (int)this[RequestJobSchema.TargetVersion];
			}
			set
			{
				this[RequestJobSchema.TargetVersion] = value;
			}
		}

		internal ADObjectId TargetDatabase
		{
			get
			{
				return (ADObjectId)this[RequestJobSchema.TargetDatabase];
			}
			set
			{
				this[RequestJobSchema.TargetDatabase] = value;
			}
		}

		internal string TargetServer
		{
			get
			{
				return (string)this[RequestJobSchema.TargetServer];
			}
			set
			{
				this[RequestJobSchema.TargetServer] = value;
			}
		}

		internal Guid? TargetContainerGuid
		{
			get
			{
				return (Guid?)this[RequestJobSchema.TargetContainerGuid];
			}
			set
			{
				this[RequestJobSchema.TargetContainerGuid] = value;
			}
		}

		internal CrossTenantObjectId TargetUnifiedMailboxId
		{
			get
			{
				return (CrossTenantObjectId)this[RequestJobSchema.TargetUnifiedMailboxId];
			}
			set
			{
				this[RequestJobSchema.TargetUnifiedMailboxId] = value;
			}
		}

		internal ADObjectId RequestQueue
		{
			get
			{
				return (ADObjectId)this[RequestJobSchema.RequestQueue];
			}
			set
			{
				this[RequestJobSchema.RequestQueue] = value;
			}
		}

		internal bool RehomeRequest
		{
			get
			{
				return (bool)(this[RequestJobSchema.RehomeRequest] ?? false);
			}
			set
			{
				this[RequestJobSchema.RehomeRequest] = value;
			}
		}

		internal ADObjectId OptimalRequestQueue
		{
			get
			{
				MRSRequestType requestType = this.RequestType;
				if (requestType != MRSRequestType.Move)
				{
					switch (requestType)
					{
					case MRSRequestType.PublicFolderMove:
					case MRSRequestType.PublicFolderMigration:
					case MRSRequestType.PublicFolderMailboxMigration:
						return this.RequestQueue;
					case MRSRequestType.FolderMove:
						return this.TargetDatabase ?? this.RequestQueue;
					}
					if (this.Direction == RequestDirection.Push)
					{
						return this.SourceDatabase ?? this.RequestQueue;
					}
					return this.TargetDatabase ?? this.RequestQueue;
				}
				else
				{
					if (this.ArchiveOnly)
					{
						return this.TargetArchiveDatabase ?? this.RequestQueue;
					}
					return this.TargetDatabase ?? this.RequestQueue;
				}
			}
		}

		internal bool ShouldSuspendRequest
		{
			get
			{
				return this.Suspend && this.Status != RequestStatus.Suspended && this.Status != RequestStatus.AutoSuspended && this.Status != RequestStatus.Failed && this.Status != RequestStatus.Completed && this.Status != RequestStatus.CompletedWithWarning;
			}
		}

		internal bool ShouldRehomeRequest
		{
			get
			{
				return this.RehomeRequest && this.RequestQueue != null && !this.RequestQueue.Equals(this.OptimalRequestQueue) && ((this.RequestType != MRSRequestType.Move && this.RequestType != MRSRequestType.MailboxRelocation) || RequestJobStateNode.RequestStateIs(this.StatusDetail, RequestState.Queued));
			}
		}

		internal bool ShouldClearRehomeRequest
		{
			get
			{
				return this.RehomeRequest && (this.RequestQueue == null || this.RequestQueue.Equals(this.OptimalRequestQueue) || ((this.RequestType == MRSRequestType.Move || this.RequestType == MRSRequestType.MailboxRelocation) && !RequestJobStateNode.RequestStateIs(this.StatusDetail, RequestState.Queued)));
			}
		}

		internal DateTime? NextPickupTime
		{
			get
			{
				DateTime utcNow = DateTime.UtcNow;
				DateTime dateTime = DateTime.MaxValue;
				DateTime? timestamp = new DateTime?(this.RequestCanceledTimestamp + ConfigBase<MRSConfigSchema>.GetConfig<TimeSpan>("CanceledRequestAge"));
				if (timestamp < dateTime && timestamp > utcNow)
				{
					dateTime = timestamp.Value;
				}
				timestamp = new DateTime?(this.ServerBusyBackoffUntilTimestamp);
				if (timestamp < dateTime && timestamp > utcNow)
				{
					dateTime = timestamp.Value;
				}
				timestamp = this.TimeTracker.GetTimestamp(RequestJobTimestamp.DoNotPickUntil);
				if (timestamp != null && timestamp.Value < dateTime && timestamp.Value > utcNow)
				{
					dateTime = timestamp.Value;
				}
				if (!(dateTime == DateTime.MaxValue))
				{
					return new DateTime?(dateTime);
				}
				return null;
			}
		}

		internal int SourceArchiveVersion
		{
			get
			{
				return (int)this[RequestJobSchema.SourceArchiveVersion];
			}
			set
			{
				this[RequestJobSchema.SourceArchiveVersion] = value;
			}
		}

		internal ADObjectId SourceArchiveDatabase
		{
			get
			{
				return (ADObjectId)this[RequestJobSchema.SourceArchiveDatabase];
			}
			set
			{
				this[RequestJobSchema.SourceArchiveDatabase] = value;
			}
		}

		internal string SourceArchiveServer
		{
			get
			{
				return (string)this[RequestJobSchema.SourceArchiveServer];
			}
			set
			{
				this[RequestJobSchema.SourceArchiveServer] = value;
			}
		}

		internal int TargetArchiveVersion
		{
			get
			{
				return (int)this[RequestJobSchema.TargetArchiveVersion];
			}
			set
			{
				this[RequestJobSchema.TargetArchiveVersion] = value;
			}
		}

		internal ADObjectId TargetArchiveDatabase
		{
			get
			{
				return (ADObjectId)this[RequestJobSchema.TargetArchiveDatabase];
			}
			set
			{
				this[RequestJobSchema.TargetArchiveDatabase] = value;
			}
		}

		internal string TargetArchiveServer
		{
			get
			{
				return (string)this[RequestJobSchema.TargetArchiveServer];
			}
			set
			{
				this[RequestJobSchema.TargetArchiveServer] = value;
			}
		}

		internal string ArchiveDomain
		{
			get
			{
				return (string)this[RequestJobSchema.ArchiveDomain];
			}
			set
			{
				this[RequestJobSchema.ArchiveDomain] = value;
			}
		}

		internal string RemoteHostName
		{
			get
			{
				return (string)this[RequestJobSchema.RemoteHostName];
			}
			set
			{
				this[RequestJobSchema.RemoteHostName] = value;
			}
		}

		internal int RemoteHostPort
		{
			get
			{
				return (int)this[RequestJobSchema.RemoteHostPort];
			}
			set
			{
				this[RequestJobSchema.RemoteHostPort] = value;
			}
		}

		internal string SmtpServerName
		{
			get
			{
				return (string)this[RequestJobSchema.SmtpServerName];
			}
			set
			{
				this[RequestJobSchema.SmtpServerName] = value;
			}
		}

		internal int SmtpServerPort
		{
			get
			{
				return (int)this[RequestJobSchema.SmtpServerPort];
			}
			set
			{
				this[RequestJobSchema.SmtpServerPort] = value;
			}
		}

		internal IMAPSecurityMechanism SecurityMechanism
		{
			get
			{
				return (IMAPSecurityMechanism)this[RequestJobSchema.SecurityMechanism];
			}
			set
			{
				this[RequestJobSchema.SecurityMechanism] = value;
			}
		}

		internal SyncProtocol SyncProtocol
		{
			get
			{
				return (SyncProtocol)this[RequestJobSchema.SyncProtocol];
			}
			set
			{
				this[RequestJobSchema.SyncProtocol] = value;
			}
		}

		internal SmtpAddress EmailAddress
		{
			get
			{
				return (SmtpAddress)this[RequestJobSchema.EmailAddress];
			}
			set
			{
				this[RequestJobSchema.EmailAddress] = value;
			}
		}

		internal TimeSpan IncrementalSyncInterval
		{
			get
			{
				return (TimeSpan)this[RequestJobSchema.IncrementalSyncInterval];
			}
			set
			{
				this[RequestJobSchema.IncrementalSyncInterval] = value;
			}
		}

		internal string RemoteGlobalCatalog
		{
			get
			{
				if (this.RequestStyle != RequestStyle.CrossOrg)
				{
					return null;
				}
				if (this.Flags.HasFlag(RequestFlags.RemoteLegacy))
				{
					if (this.Direction == RequestDirection.Push)
					{
						return this.TargetDCName;
					}
					return this.SourceDCName;
				}
				else
				{
					if (!string.IsNullOrEmpty(this.RemoteDomainControllerToUpdate))
					{
						return this.RemoteDomainControllerToUpdate;
					}
					return null;
				}
			}
		}

		internal string BatchName
		{
			get
			{
				return (string)this[RequestJobSchema.BatchName];
			}
			set
			{
				this[RequestJobSchema.BatchName] = value;
			}
		}

		internal string RemoteDatabaseName
		{
			get
			{
				return (string)this[RequestJobSchema.RemoteDatabaseName];
			}
			set
			{
				this[RequestJobSchema.RemoteDatabaseName] = value;
			}
		}

		internal Guid? RemoteDatabaseGuid
		{
			get
			{
				return (Guid?)this[RequestJobSchema.RemoteDatabaseGuid];
			}
			set
			{
				this[RequestJobSchema.RemoteDatabaseGuid] = value;
			}
		}

		internal string RemoteArchiveDatabaseName
		{
			get
			{
				return (string)this[RequestJobSchema.RemoteArchiveDatabaseName];
			}
			set
			{
				this[RequestJobSchema.RemoteArchiveDatabaseName] = value;
			}
		}

		internal Guid? RemoteArchiveDatabaseGuid
		{
			get
			{
				return (Guid?)this[RequestJobSchema.RemoteArchiveDatabaseGuid];
			}
			set
			{
				this[RequestJobSchema.RemoteArchiveDatabaseGuid] = value;
			}
		}

		internal Unlimited<int> BadItemLimit
		{
			get
			{
				return (Unlimited<int>)this[RequestJobSchema.BadItemLimit];
			}
			set
			{
				this[RequestJobSchema.BadItemLimit] = value;
			}
		}

		internal int BadItemsEncountered
		{
			get
			{
				return (int)this[RequestJobSchema.BadItemsEncountered];
			}
			set
			{
				this[RequestJobSchema.BadItemsEncountered] = value;
			}
		}

		internal Unlimited<int> LargeItemLimit
		{
			get
			{
				return (Unlimited<int>)this[RequestJobSchema.LargeItemLimit];
			}
			set
			{
				this[RequestJobSchema.LargeItemLimit] = value;
			}
		}

		internal int LargeItemsEncountered
		{
			get
			{
				return (int)this[RequestJobSchema.LargeItemsEncountered];
			}
			set
			{
				this[RequestJobSchema.LargeItemsEncountered] = value;
			}
		}

		internal bool AllowLargeItems
		{
			get
			{
				return (bool)this[RequestJobSchema.AllowLargeItems];
			}
			set
			{
				this[RequestJobSchema.AllowLargeItems] = value;
			}
		}

		internal int MissingItemsEncountered
		{
			get
			{
				return (int)this[RequestJobSchema.MissingItemsEncountered];
			}
			set
			{
				this[RequestJobSchema.MissingItemsEncountered] = value;
			}
		}

		internal string MRSServerName
		{
			get
			{
				return (string)this[RequestJobSchema.MRSServerName];
			}
			set
			{
				this[RequestJobSchema.MRSServerName] = value;
			}
		}

		internal ulong TotalMailboxSize
		{
			get
			{
				return (ulong)this[RequestJobSchema.TotalMailboxSize];
			}
			set
			{
				this[RequestJobSchema.TotalMailboxSize] = value;
			}
		}

		internal ulong TotalMailboxItemCount
		{
			get
			{
				return (ulong)this[RequestJobSchema.TotalMailboxItemCount];
			}
			set
			{
				this[RequestJobSchema.TotalMailboxItemCount] = value;
			}
		}

		internal ulong? TotalArchiveSize
		{
			get
			{
				return (ulong?)this[RequestJobSchema.TotalArchiveSize];
			}
			set
			{
				this[RequestJobSchema.TotalArchiveSize] = value;
			}
		}

		internal ulong? TotalArchiveItemCount
		{
			get
			{
				return (ulong?)this[RequestJobSchema.TotalArchiveItemCount];
			}
			set
			{
				this[RequestJobSchema.TotalArchiveItemCount] = value;
			}
		}

		internal int PercentComplete
		{
			get
			{
				return (int)this[RequestJobSchema.PercentComplete];
			}
			set
			{
				this[RequestJobSchema.PercentComplete] = value;
			}
		}

		internal int? FailureCode
		{
			get
			{
				return (int?)this[RequestJobSchema.FailureCode];
			}
			set
			{
				this[RequestJobSchema.FailureCode] = value;
			}
		}

		internal string FailureType
		{
			get
			{
				return (string)this[RequestJobSchema.FailureType];
			}
			set
			{
				this[RequestJobSchema.FailureType] = value;
			}
		}

		internal ExceptionSide? FailureSide
		{
			get
			{
				return (ExceptionSide?)this[RequestJobSchema.FailureSide];
			}
			set
			{
				this[RequestJobSchema.FailureSide] = value;
			}
		}

		internal LocalizedString Message
		{
			get
			{
				return (LocalizedString)this[RequestJobSchema.Message];
			}
			set
			{
				this[RequestJobSchema.Message] = value;
			}
		}

		internal string RemoteOrgName
		{
			get
			{
				return (string)this[RequestJobSchema.RemoteOrgName];
			}
			set
			{
				this[RequestJobSchema.RemoteOrgName] = value;
			}
		}

		internal NetworkCredential RemoteCredential
		{
			get
			{
				return this.remoteCredential;
			}
			set
			{
				this.remoteCredential = value;
				if (value == null)
				{
					this.RemoteCredentialUsername = null;
					return;
				}
				if (string.IsNullOrEmpty(value.Domain))
				{
					this.RemoteCredentialUsername = value.UserName;
					return;
				}
				this.RemoteCredentialUsername = value.Domain + "\\" + value.UserName;
			}
		}

		internal string RemoteCredentialUsername
		{
			get
			{
				return (string)this[RequestJobSchema.RemoteCredentialUsername];
			}
			set
			{
				this[RequestJobSchema.RemoteCredentialUsername] = value;
			}
		}

		internal string DomainControllerToUpdate
		{
			get
			{
				string text = (string)this[RequestJobSchema.DomainControllerToUpdate];
				if (text == null)
				{
					return text;
				}
				using (this.jobConfigContext.Value.Activate())
				{
					TimeSpan config = ConfigBase<MRSConfigSchema>.GetConfig<TimeSpan>("DCNameValidityInterval");
					if (config != TimeSpan.Zero && DateTime.UtcNow - this.DomainControllerUpdateTimestamp > config)
					{
						this[RequestJobSchema.DomainControllerToUpdate] = null;
						text = null;
					}
				}
				return text;
			}
			set
			{
				this[RequestJobSchema.DomainControllerToUpdate] = value;
			}
		}

		internal string RemoteDomainControllerToUpdate
		{
			get
			{
				return (string)this[RequestJobSchema.RemoteDomainControllerToUpdate];
			}
			set
			{
				this[RequestJobSchema.RemoteDomainControllerToUpdate] = value;
			}
		}

		internal string SourceDomainControllerToUpdate
		{
			get
			{
				if (this.Direction != RequestDirection.Pull)
				{
					return this.DomainControllerToUpdate;
				}
				return this.RemoteDomainControllerToUpdate;
			}
		}

		internal string DestDomainControllerToUpdate
		{
			get
			{
				if (this.Direction != RequestDirection.Pull)
				{
					return this.RemoteDomainControllerToUpdate;
				}
				return this.DomainControllerToUpdate;
			}
		}

		internal bool AllowedToFinishMove
		{
			get
			{
				return (bool)this[RequestJobSchema.AllowedToFinishMove];
			}
			set
			{
				this[RequestJobSchema.AllowedToFinishMove] = value;
			}
		}

		internal bool PreserveMailboxSignature
		{
			get
			{
				return (bool)this[RequestJobSchema.PreserveMailboxSignature];
			}
			set
			{
				this[RequestJobSchema.PreserveMailboxSignature] = value;
			}
		}

		internal bool RestartingAfterSignatureChange
		{
			get
			{
				return (bool)this[RequestJobSchema.RestartingAfterSignatureChange];
			}
			set
			{
				this[RequestJobSchema.RestartingAfterSignatureChange] = value;
			}
		}

		internal int? IsIntegData
		{
			get
			{
				return (int?)this[RequestJobSchema.IsIntegData];
			}
			set
			{
				this[RequestJobSchema.IsIntegData] = value;
			}
		}

		internal long? UserPuid
		{
			get
			{
				return (long?)this[RequestJobSchema.UserPuid];
			}
			set
			{
				this[RequestJobSchema.UserPuid] = value;
			}
		}

		internal int? OlcDGroup
		{
			get
			{
				return (int?)this[RequestJobSchema.OlcDGroup];
			}
			set
			{
				this[RequestJobSchema.OlcDGroup] = value;
			}
		}

		internal bool CancelRequest
		{
			get
			{
				return (bool)this[RequestJobSchema.CancelRequest];
			}
			set
			{
				this[RequestJobSchema.CancelRequest] = value;
				if (value)
				{
					this.AllowedToFinishMove = false;
				}
			}
		}

		internal DateTime RequestCanceledTimestamp
		{
			get
			{
				DateTime? timestamp = this.TimeTracker.GetTimestamp(RequestJobTimestamp.RequestCanceled);
				if (timestamp == null)
				{
					return DateTime.MinValue;
				}
				return timestamp.GetValueOrDefault();
			}
		}

		internal DateTime LastServerBusyBackoffTimestamp
		{
			get
			{
				DateTime? timestamp = this.TimeTracker.GetTimestamp(RequestJobTimestamp.LastServerBusyBackoff);
				if (timestamp == null)
				{
					return DateTime.MinValue;
				}
				return timestamp.GetValueOrDefault();
			}
		}

		internal DateTime ServerBusyBackoffUntilTimestamp
		{
			get
			{
				DateTime? timestamp = this.TimeTracker.GetTimestamp(RequestJobTimestamp.ServerBusyBackoffUntil);
				if (timestamp == null)
				{
					return DateTime.MinValue;
				}
				return timestamp.GetValueOrDefault();
			}
		}

		internal JobProcessingState RequestJobState
		{
			get
			{
				return (JobProcessingState)this[RequestJobSchema.RequestJobState];
			}
			set
			{
				this[RequestJobSchema.RequestJobState] = value;
			}
		}

		internal string UserOrgName
		{
			get
			{
				return (string)this[RequestJobSchema.UserOrgName];
			}
			set
			{
				this[RequestJobSchema.UserOrgName] = value;
			}
		}

		internal TimeSpan IdleTime
		{
			get
			{
				DateTime d = this.TimeTracker.GetTimestamp(RequestJobTimestamp.LastUpdate) ?? DateTime.MinValue;
				return DateTime.UtcNow - d;
			}
		}

		internal SyncStage SyncStage
		{
			get
			{
				return (SyncStage)this[RequestJobSchema.SyncStage];
			}
			set
			{
				this[RequestJobSchema.SyncStage] = value;
			}
		}

		internal string SourceDCName
		{
			get
			{
				return (string)this[RequestJobSchema.SourceDCName];
			}
			set
			{
				this[RequestJobSchema.SourceDCName] = value;
			}
		}

		internal NetworkCredential SourceCredential
		{
			get
			{
				return this.sourceCredential;
			}
			set
			{
				this.sourceCredential = value;
			}
		}

		internal string TargetDCName
		{
			get
			{
				return (string)this[RequestJobSchema.TargetDCName];
			}
			set
			{
				this[RequestJobSchema.TargetDCName] = value;
			}
		}

		internal NetworkCredential TargetCredential
		{
			get
			{
				return this.targetCredential;
			}
			set
			{
				this.targetCredential = value;
			}
		}

		internal int RetryCount
		{
			get
			{
				return (int)this[RequestJobSchema.RetryCount];
			}
			set
			{
				this[RequestJobSchema.RetryCount] = value;
			}
		}

		internal int TotalRetryCount
		{
			get
			{
				return (int)this[RequestJobSchema.TotalRetryCount];
			}
			set
			{
				this[RequestJobSchema.TotalRetryCount] = value;
			}
		}

		internal string TargetDeliveryDomain
		{
			get
			{
				return (string)this[RequestJobSchema.TargetDeliveryDomain];
			}
			set
			{
				this[RequestJobSchema.TargetDeliveryDomain] = value;
			}
		}

		internal bool IgnoreRuleLimitErrors
		{
			get
			{
				return (bool)this[RequestJobSchema.IgnoreRuleLimitErrors];
			}
			set
			{
				this[RequestJobSchema.IgnoreRuleLimitErrors] = value;
			}
		}

		internal MRSJobType JobType
		{
			get
			{
				return (MRSJobType)this[RequestJobSchema.JobType];
			}
			set
			{
				this[RequestJobSchema.JobType] = value;
			}
		}

		internal string Name
		{
			get
			{
				return (string)this[RequestJobSchema.Name];
			}
			set
			{
				this[RequestJobSchema.Name] = value;
			}
		}

		internal MRSRequestType RequestType
		{
			get
			{
				return (MRSRequestType)this[RequestJobSchema.RequestType];
			}
			set
			{
				this[RequestJobSchema.RequestType] = value;
			}
		}

		internal string FilePath
		{
			get
			{
				return (string)this[RequestJobSchema.FilePath];
			}
			set
			{
				this[RequestJobSchema.FilePath] = value;
			}
		}

		internal MailboxRestoreType? MailboxRestoreFlags
		{
			get
			{
				return (MailboxRestoreType?)this[RequestJobSchema.MailboxRestoreFlags];
			}
			set
			{
				this[RequestJobSchema.MailboxRestoreFlags] = value;
			}
		}

		internal ADObjectId TargetUserId
		{
			get
			{
				return (ADObjectId)this[RequestJobSchema.TargetUserId];
			}
			set
			{
				this[RequestJobSchema.TargetUserId] = value;
			}
		}

		internal ADObjectId SourceUserId
		{
			get
			{
				return (ADObjectId)this[RequestJobSchema.SourceUserId];
			}
			set
			{
				this[RequestJobSchema.SourceUserId] = value;
			}
		}

		internal string RemoteMailboxLegacyDN
		{
			get
			{
				return (string)this[RequestJobSchema.RemoteMailboxLegacyDN];
			}
			set
			{
				this[RequestJobSchema.RemoteMailboxLegacyDN] = value;
			}
		}

		internal string RemoteUserLegacyDN
		{
			get
			{
				return (string)this[RequestJobSchema.RemoteUserLegacyDN];
			}
			set
			{
				this[RequestJobSchema.RemoteUserLegacyDN] = value;
			}
		}

		internal string RemoteMailboxServerLegacyDN
		{
			get
			{
				return (string)this[RequestJobSchema.RemoteMailboxServerLegacyDN];
			}
			set
			{
				this[RequestJobSchema.RemoteMailboxServerLegacyDN] = value;
			}
		}

		internal string OutlookAnywhereHostName
		{
			get
			{
				return (string)this[RequestJobSchema.OutlookAnywhereHostName];
			}
			set
			{
				this[RequestJobSchema.OutlookAnywhereHostName] = value;
			}
		}

		internal AuthenticationMethod? AuthenticationMethod
		{
			get
			{
				return (AuthenticationMethod?)this[RequestJobSchema.AuthMethod];
			}
			set
			{
				this[RequestJobSchema.AuthMethod] = value;
			}
		}

		internal bool? IsAdministrativeCredential
		{
			get
			{
				return (bool?)this[RequestJobSchema.IsAdministrativeCredential];
			}
			set
			{
				this[RequestJobSchema.IsAdministrativeCredential] = value;
			}
		}

		internal ConflictResolutionOption? ConflictResolutionOption
		{
			get
			{
				return (ConflictResolutionOption?)this[RequestJobSchema.ConflictResolutionOption];
			}
			set
			{
				this[RequestJobSchema.ConflictResolutionOption] = value;
			}
		}

		internal FAICopyOption? AssociatedMessagesCopyOption
		{
			get
			{
				return (FAICopyOption?)this[RequestJobSchema.AssociatedMessagesCopyOption];
			}
			set
			{
				this[RequestJobSchema.AssociatedMessagesCopyOption] = value;
			}
		}

		internal ADObjectId OrganizationalUnitRoot
		{
			get
			{
				return (ADObjectId)this[RequestJobSchema.OrganizationalUnitRoot];
			}
			set
			{
				this[RequestJobSchema.OrganizationalUnitRoot] = value;
			}
		}

		internal ADObjectId ConfigurationUnit
		{
			get
			{
				return (ADObjectId)this[RequestJobSchema.ConfigurationUnit];
			}
			set
			{
				this[RequestJobSchema.ConfigurationUnit] = value;
			}
		}

		internal Guid ExternalDirectoryOrganizationId { get; set; }

		internal TenantPartitionHint PartitionHint { get; set; }

		internal OrganizationId OrganizationId
		{
			get
			{
				return (OrganizationId)this[RequestJobSchema.OrganizationId];
			}
			set
			{
				this[RequestJobSchema.OrganizationId] = value;
			}
		}

		internal string ContentFilter
		{
			get
			{
				return (string)this[RequestJobSchema.ContentFilter];
			}
			set
			{
				this[RequestJobSchema.ContentFilter] = value;
			}
		}

		internal int ContentFilterLCID
		{
			get
			{
				return (int)this[RequestJobSchema.ContentFilterLCID];
			}
			set
			{
				this[RequestJobSchema.ContentFilterLCID] = value;
			}
		}

		internal RequestPriority Priority
		{
			get
			{
				return (RequestPriority)this[RequestJobSchema.Priority];
			}
			set
			{
				this[RequestJobSchema.Priority] = value;
			}
		}

		internal RequestWorkloadType WorkloadType
		{
			get
			{
				return (RequestWorkloadType)this[RequestJobSchema.WorkloadType];
			}
			set
			{
				this[RequestJobSchema.WorkloadType] = value;
			}
		}

		internal RequestJobInternalFlags RequestJobInternalFlags
		{
			get
			{
				return (RequestJobInternalFlags)this[RequestJobSchema.JobInternalFlags];
			}
			set
			{
				this[RequestJobSchema.JobInternalFlags] = value;
			}
		}

		internal Unlimited<EnhancedTimeSpan> CompletedRequestAgeLimit
		{
			get
			{
				return (Unlimited<EnhancedTimeSpan>)this[RequestJobSchema.CompletedRequestAgeLimit];
			}
			set
			{
				this[RequestJobSchema.CompletedRequestAgeLimit] = value;
			}
		}

		internal string RequestCreator
		{
			get
			{
				return (string)this[RequestJobSchema.RequestCreator];
			}
			set
			{
				this[RequestJobSchema.RequestCreator] = value;
			}
		}

		internal int PoisonCount
		{
			get
			{
				return (int)this[RequestJobSchema.PoisonCount];
			}
			set
			{
				this[RequestJobSchema.PoisonCount] = value;
			}
		}

		internal DateTime? LastPickupTime
		{
			get
			{
				return (DateTime?)this[RequestJobSchema.LastPickupTime];
			}
			set
			{
				this[RequestJobSchema.LastPickupTime] = value;
			}
		}

		internal int? ContentCodePage
		{
			get
			{
				return (int?)this[RequestJobSchema.ContentCodePage];
			}
			set
			{
				this[RequestJobSchema.ContentCodePage] = value;
			}
		}

		internal List<FolderToMailboxMapping> FolderToMailboxMap
		{
			get
			{
				if (SuppressingPiiContext.NeedPiiSuppression && this.folderToMailboxMapping != null && this.folderToMailboxMapping.Count > 0)
				{
					return (from map in this.folderToMailboxMapping
					select new FolderToMailboxMapping(SuppressingPiiData.Redact(map.FolderName), map.MailboxGuid)).ToList<FolderToMailboxMapping>();
				}
				return this.folderToMailboxMapping;
			}
			set
			{
				this.folderToMailboxMapping = value;
			}
		}

		internal List<MoveFolderInfo> FolderList
		{
			get
			{
				return this.folderList;
			}
			set
			{
				this.folderList = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return RequestJobBase.Schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		internal RequestStyle RequestStyle
		{
			get
			{
				if (!this.Flags.HasFlag(RequestFlags.CrossOrg))
				{
					return RequestStyle.IntraOrg;
				}
				return RequestStyle.CrossOrg;
			}
			set
			{
				this.UpdateFlags((value == RequestStyle.CrossOrg) ? RequestFlags.CrossOrg : RequestFlags.IntraOrg, RequestFlags.CrossOrg | RequestFlags.IntraOrg);
			}
		}

		internal RequestDirection Direction
		{
			get
			{
				if (!this.Flags.HasFlag(RequestFlags.Push))
				{
					return RequestDirection.Pull;
				}
				return RequestDirection.Push;
			}
			set
			{
				this.UpdateFlags((value == RequestDirection.Push) ? RequestFlags.Push : RequestFlags.Pull, RequestFlags.Push | RequestFlags.Pull);
			}
		}

		internal bool IsOffline
		{
			get
			{
				return this.Flags.HasFlag(RequestFlags.Offline);
			}
			set
			{
				this.UpdateFlags(value ? RequestFlags.Offline : RequestFlags.None, RequestFlags.Offline);
			}
		}

		internal bool Protect
		{
			get
			{
				return this.Flags.HasFlag(RequestFlags.Protected);
			}
			set
			{
				this.UpdateFlags(value ? RequestFlags.Protected : RequestFlags.None, RequestFlags.Protected);
			}
		}

		internal bool Suspend
		{
			get
			{
				return this.Flags.HasFlag(RequestFlags.Suspend);
			}
			set
			{
				this.UpdateFlags(value ? RequestFlags.Suspend : RequestFlags.None, RequestFlags.Suspend);
			}
		}

		internal bool SuspendWhenReadyToComplete
		{
			get
			{
				return this.Flags.HasFlag(RequestFlags.SuspendWhenReadyToComplete);
			}
			set
			{
				this.UpdateFlags(value ? RequestFlags.SuspendWhenReadyToComplete : RequestFlags.None, RequestFlags.SuspendWhenReadyToComplete);
			}
		}

		internal ADObjectId WorkItemQueueMdb
		{
			get
			{
				if (this.RequestQueue != null)
				{
					return this.RequestQueue;
				}
				if (this.ArchiveOnly)
				{
					if (this.Direction != RequestDirection.Push)
					{
						return this.TargetArchiveDatabase;
					}
					return this.SourceArchiveDatabase;
				}
				else
				{
					if (this.Direction != RequestDirection.Push)
					{
						return this.TargetDatabase;
					}
					return this.SourceDatabase;
				}
			}
		}

		internal Guid IdentifyingGuid
		{
			get
			{
				if (this.RequestType != MRSRequestType.Move)
				{
					return this.RequestGuid;
				}
				return this.ExchangeGuid;
			}
		}

		internal string SourceMDBName
		{
			get
			{
				if (this.SourceIsLocal)
				{
					ADObjectId adobjectId = this.ArchiveOnly ? this.SourceArchiveDatabase : this.SourceDatabase;
					if (adobjectId == null)
					{
						return null;
					}
					return adobjectId.ToString();
				}
				else
				{
					if (!this.ArchiveOnly)
					{
						return this.RemoteDatabaseName;
					}
					return this.RemoteArchiveDatabaseName;
				}
			}
		}

		internal Guid SourceMDBGuid
		{
			get
			{
				if (!this.PrimaryIsMoving)
				{
					return Guid.Empty;
				}
				if (this.SourceIsLocal)
				{
					if (this.SourceDatabase == null)
					{
						return Guid.Empty;
					}
					return this.SourceDatabase.ObjectGuid;
				}
				else
				{
					Guid? remoteDatabaseGuid = this.RemoteDatabaseGuid;
					if (remoteDatabaseGuid == null)
					{
						return Guid.Empty;
					}
					return remoteDatabaseGuid.GetValueOrDefault();
				}
			}
		}

		internal string TargetMDBName
		{
			get
			{
				if (this.TargetIsLocal)
				{
					ADObjectId adobjectId = this.ArchiveOnly ? this.TargetArchiveDatabase : this.TargetDatabase;
					if (adobjectId == null)
					{
						return null;
					}
					return adobjectId.ToString();
				}
				else
				{
					if (!this.ArchiveOnly)
					{
						return this.RemoteDatabaseName;
					}
					return this.RemoteArchiveDatabaseName;
				}
			}
		}

		internal Guid TargetMDBGuid
		{
			get
			{
				if (!this.PrimaryIsMoving)
				{
					return Guid.Empty;
				}
				if (this.TargetIsLocal)
				{
					if (this.TargetDatabase == null)
					{
						return Guid.Empty;
					}
					return this.TargetDatabase.ObjectGuid;
				}
				else
				{
					Guid? remoteDatabaseGuid = this.RemoteDatabaseGuid;
					if (remoteDatabaseGuid == null)
					{
						return Guid.Empty;
					}
					return remoteDatabaseGuid.GetValueOrDefault();
				}
			}
		}

		internal Guid SourceArchiveMDBGuid
		{
			get
			{
				if (!this.ArchiveIsMoving)
				{
					return Guid.Empty;
				}
				if (this.SourceIsLocal)
				{
					ADObjectId adobjectId = this.SourceArchiveDatabase ?? this.SourceDatabase;
					if (adobjectId == null)
					{
						return Guid.Empty;
					}
					return adobjectId.ObjectGuid;
				}
				else
				{
					Guid? remoteArchiveDatabaseGuid = this.RemoteArchiveDatabaseGuid;
					if (remoteArchiveDatabaseGuid == null)
					{
						return Guid.Empty;
					}
					return remoteArchiveDatabaseGuid.GetValueOrDefault();
				}
			}
		}

		internal Guid TargetArchiveMDBGuid
		{
			get
			{
				if (!this.ArchiveIsMoving)
				{
					return Guid.Empty;
				}
				if (this.TargetIsLocal)
				{
					ADObjectId adobjectId = this.TargetArchiveDatabase ?? this.TargetDatabase;
					if (adobjectId == null)
					{
						return Guid.Empty;
					}
					return adobjectId.ObjectGuid;
				}
				else
				{
					Guid? remoteArchiveDatabaseGuid = this.RemoteArchiveDatabaseGuid;
					if (remoteArchiveDatabaseGuid == null)
					{
						return Guid.Empty;
					}
					return remoteArchiveDatabaseGuid.GetValueOrDefault();
				}
			}
		}

		internal string WorkItemQueueMdbName
		{
			get
			{
				if (this.RequestQueue != null)
				{
					return this.RequestQueue.ToString();
				}
				if (this.ArchiveOnly)
				{
					if (this.Direction != RequestDirection.Push)
					{
						return this.TargetArchiveDatabase.Name;
					}
					return this.SourceArchiveDatabase.Name;
				}
				else
				{
					if (this.Direction != RequestDirection.Push)
					{
						return this.TargetMDBName;
					}
					return this.SourceMDBName;
				}
			}
		}

		internal byte[] MessageId
		{
			get
			{
				if (this.Identity != null)
				{
					return this.Identity.MessageId;
				}
				return null;
			}
			set
			{
				if (this.Identity == null)
				{
					this.Identity = new RequestJobObjectId(this.IdentifyingGuid, this.WorkItemQueueMdb.ObjectGuid, value);
					return;
				}
				this.Identity.MessageId = value;
			}
		}

		internal DateTime DomainControllerUpdateTimestamp
		{
			get
			{
				DateTime? timestamp = this.TimeTracker.GetTimestamp(RequestJobTimestamp.DomainControllerUpdate);
				if (timestamp == null)
				{
					timestamp = new DateTime?(this.TimeTracker.GetTimestamp(RequestJobTimestamp.Creation) ?? DateTime.UtcNow);
				}
				return timestamp.Value;
			}
		}

		internal bool PrimaryOnly
		{
			get
			{
				return this.Flags.HasFlag(RequestFlags.MoveOnlyPrimaryMailbox);
			}
		}

		internal bool ArchiveOnly
		{
			get
			{
				return this.Flags.HasFlag(RequestFlags.MoveOnlyArchiveMailbox);
			}
		}

		internal bool PrimaryIsMoving
		{
			get
			{
				return !this.ArchiveOnly;
			}
		}

		internal bool ArchiveIsMoving
		{
			get
			{
				return !this.PrimaryOnly && this.ArchiveGuid != null;
			}
		}

		internal bool SourceIsLocal
		{
			get
			{
				return this.RequestStyle == RequestStyle.IntraOrg || this.Direction == RequestDirection.Push;
			}
		}

		internal bool TargetIsLocal
		{
			get
			{
				return this.RequestStyle == RequestStyle.IntraOrg || this.Direction == RequestDirection.Pull;
			}
		}

		internal bool IsSplitPrimaryAndArchive
		{
			get
			{
				return this.PrimaryOnly || this.ArchiveOnly || (this.ArchiveGuid != null && (this.SourceMDBGuid != this.SourceArchiveMDBGuid || this.TargetMDBGuid != this.TargetArchiveMDBGuid));
			}
		}

		internal ReportVersion ReportVersion
		{
			get
			{
				if (this.JobType < MRSJobType.RequestJobE14R6_CompressedReports)
				{
					return ReportVersion.ReportE14R4;
				}
				return ReportVersion.ReportE14R6Compression;
			}
		}

		internal bool ForceOfflineMove
		{
			get
			{
				return this.RequestJobInternalFlags.HasFlag(RequestJobInternalFlags.ForceOfflineMove);
			}
			set
			{
				this.UpdateInternalFlags(RequestJobInternalFlags.ForceOfflineMove, value);
			}
		}

		internal bool PreventCompletion
		{
			get
			{
				return this.RequestJobInternalFlags.HasFlag(RequestJobInternalFlags.PreventCompletion);
			}
			set
			{
				this.UpdateInternalFlags(RequestJobInternalFlags.PreventCompletion, value);
			}
		}

		internal bool SkipMailboxReleaseCheck
		{
			get
			{
				return this.RequestJobInternalFlags.HasFlag(RequestJobInternalFlags.SkipMailboxReleaseCheck);
			}
			set
			{
				this.UpdateInternalFlags(RequestJobInternalFlags.SkipMailboxReleaseCheck, value);
			}
		}

		internal bool RestartFromScratch
		{
			get
			{
				return this.RequestJobInternalFlags.HasFlag(RequestJobInternalFlags.RestartFromScratch);
			}
			set
			{
				this.UpdateInternalFlags(RequestJobInternalFlags.RestartFromScratch, value);
			}
		}

		internal bool SkipFolderACLs
		{
			get
			{
				return this.RequestJobInternalFlags.HasFlag(RequestJobInternalFlags.SkipFolderACLs);
			}
			set
			{
				this.UpdateInternalFlags(RequestJobInternalFlags.SkipFolderACLs, value);
			}
		}

		internal bool SkipFolderPromotedProperties
		{
			get
			{
				return this.RequestJobInternalFlags.HasFlag(RequestJobInternalFlags.SkipFolderPromotedProperties);
			}
			set
			{
				this.UpdateInternalFlags(RequestJobInternalFlags.SkipFolderPromotedProperties, value);
			}
		}

		internal bool SkipFolderRestrictions
		{
			get
			{
				return this.RequestJobInternalFlags.HasFlag(RequestJobInternalFlags.SkipFolderRestrictions);
			}
			set
			{
				this.UpdateInternalFlags(RequestJobInternalFlags.SkipFolderRestrictions, value);
			}
		}

		internal bool SkipFolderRules
		{
			get
			{
				return this.RequestJobInternalFlags.HasFlag(RequestJobInternalFlags.SkipFolderRules);
			}
			set
			{
				this.UpdateInternalFlags(RequestJobInternalFlags.SkipFolderRules, value);
			}
		}

		internal bool SkipFolderViews
		{
			get
			{
				return this.RequestJobInternalFlags.HasFlag(RequestJobInternalFlags.SkipFolderViews);
			}
			set
			{
				this.UpdateInternalFlags(RequestJobInternalFlags.SkipFolderViews, value);
			}
		}

		internal bool SkipInitialConnectionValidation
		{
			get
			{
				return this.RequestJobInternalFlags.HasFlag(RequestJobInternalFlags.SkipInitialConnectionValidation);
			}
			set
			{
				this.UpdateInternalFlags(RequestJobInternalFlags.SkipInitialConnectionValidation, value);
			}
		}

		internal bool SkipContentVerification
		{
			get
			{
				return (this.RequestJobInternalFlags & RequestJobInternalFlags.SkipContentVerification) != RequestJobInternalFlags.None;
			}
			set
			{
				this.UpdateInternalFlags(RequestJobInternalFlags.SkipContentVerification, value);
			}
		}

		internal bool SkipKnownCorruptions
		{
			get
			{
				return this.RequestJobInternalFlags.HasFlag(RequestJobInternalFlags.SkipKnownCorruptions);
			}
			set
			{
				this.UpdateInternalFlags(RequestJobInternalFlags.SkipKnownCorruptions, value);
			}
		}

		internal bool FailOnCorruptSyncState
		{
			get
			{
				return this.RequestJobInternalFlags.HasFlag(RequestJobInternalFlags.FailOnCorruptSyncState);
			}
			set
			{
				this.UpdateInternalFlags(RequestJobInternalFlags.FailOnCorruptSyncState, value);
			}
		}

		internal bool IncrementallyUpdateGlobalCounterRanges
		{
			get
			{
				return this.RequestJobInternalFlags.HasFlag(RequestJobInternalFlags.IncrementallyUpdateGlobalCounterRanges);
			}
			set
			{
				this.UpdateInternalFlags(RequestJobInternalFlags.IncrementallyUpdateGlobalCounterRanges, value);
			}
		}

		internal bool ExecutedByTransportSync
		{
			get
			{
				return this.RequestJobInternalFlags.HasFlag(RequestJobInternalFlags.ExecutedByTransportSync);
			}
			set
			{
				this.UpdateInternalFlags(RequestJobInternalFlags.ExecutedByTransportSync, value);
			}
		}

		internal bool BlockFinalization
		{
			get
			{
				return (this.RequestJobInternalFlags & RequestJobInternalFlags.BlockFinalization) != RequestJobInternalFlags.None;
			}
			set
			{
				this.UpdateInternalFlags(RequestJobInternalFlags.BlockFinalization, value);
			}
		}

		internal bool SkipStorageProviderForSource
		{
			get
			{
				return this.RequestJobInternalFlags.HasFlag(RequestJobInternalFlags.SkipStorageProviderForSource);
			}
			set
			{
				this.UpdateInternalFlags(RequestJobInternalFlags.SkipStorageProviderForSource, value);
			}
		}

		internal bool FailOnFirstBadItem
		{
			get
			{
				return this.RequestJobInternalFlags.HasFlag(RequestJobInternalFlags.FailOnFirstBadItem);
			}
			set
			{
				this.UpdateInternalFlags(RequestJobInternalFlags.FailOnFirstBadItem, value);
			}
		}

		internal bool IsPublicFolderMailboxRestore
		{
			get
			{
				return this.MailboxRestoreFlags != null && this.MailboxRestoreFlags.Value.HasFlag(MailboxRestoreType.PublicFolderMailbox);
			}
		}

		internal bool IsLivePublicFolderMailboxRestore
		{
			get
			{
				return this.MailboxRestoreFlags != null && this.MailboxRestoreFlags.Value == MailboxRestoreType.PublicFolderMailbox;
			}
		}

		internal bool SkipPreFinalSyncDataProcessing
		{
			get
			{
				return this.RequestJobInternalFlags.HasFlag(RequestJobInternalFlags.SkipPreFinalSyncDataProcessing);
			}
			set
			{
				this.UpdateInternalFlags(RequestJobInternalFlags.SkipPreFinalSyncDataProcessing, value);
			}
		}

		internal bool SkipWordBreaking
		{
			get
			{
				return this.RequestJobInternalFlags.HasFlag(RequestJobInternalFlags.SkipWordBreaking);
			}
			set
			{
				this.UpdateInternalFlags(RequestJobInternalFlags.SkipWordBreaking, value);
			}
		}

		internal bool InvalidateContentIndexAnnotations
		{
			get
			{
				return this.RequestJobInternalFlags.HasFlag(RequestJobInternalFlags.InvalidateContentIndexAnnotations);
			}
			set
			{
				this.UpdateInternalFlags(RequestJobInternalFlags.InvalidateContentIndexAnnotations, value);
			}
		}

		internal bool SkipProvisioningCheck
		{
			get
			{
				return this.RequestJobInternalFlags.HasFlag(RequestJobInternalFlags.SkipProvisioningCheck);
			}
			set
			{
				this.UpdateInternalFlags(RequestJobInternalFlags.SkipProvisioningCheck, value);
			}
		}

		internal bool UseAsyncNotificationAPI
		{
			get
			{
				return this.RequestType == MRSRequestType.MailboxExport || this.RequestType == MRSRequestType.MailboxImport || this.RequestType == MRSRequestType.MailboxRestore;
			}
		}

		internal bool SkipConvertingSourceToMeu
		{
			get
			{
				return this.RequestJobInternalFlags.HasFlag(RequestJobInternalFlags.SkipConvertingSourceToMeu);
			}
			set
			{
				this.UpdateInternalFlags(RequestJobInternalFlags.SkipConvertingSourceToMeu, value);
			}
		}

		internal bool ResolveServer
		{
			get
			{
				return this.RequestJobInternalFlags.HasFlag(RequestJobInternalFlags.ResolveServer);
			}
			set
			{
				this.UpdateInternalFlags(RequestJobInternalFlags.ResolveServer, value);
			}
		}

		internal bool UseTcp
		{
			get
			{
				return this.RequestJobInternalFlags.HasFlag(RequestJobInternalFlags.UseTcp);
			}
			set
			{
				this.UpdateInternalFlags(RequestJobInternalFlags.UseTcp, value);
			}
		}

		internal bool CrossResourceForest
		{
			get
			{
				return this.RequestJobInternalFlags.HasFlag(RequestJobInternalFlags.CrossResourceForest);
			}
			set
			{
				this.UpdateInternalFlags(RequestJobInternalFlags.CrossResourceForest, value);
			}
		}

		internal bool UseCertificateAuthentication
		{
			get
			{
				return this.RequestJobInternalFlags.HasFlag(RequestJobInternalFlags.UseCertificateAuthentication);
			}
			set
			{
				this.UpdateInternalFlags(RequestJobInternalFlags.UseCertificateAuthentication, value);
			}
		}

		internal AsyncOperationType AsyncOperationType
		{
			get
			{
				switch (this.RequestType)
				{
				case MRSRequestType.MailboxImport:
					return AsyncOperationType.ImportPST;
				case MRSRequestType.MailboxExport:
					return AsyncOperationType.ExportPST;
				case MRSRequestType.MailboxRestore:
					return AsyncOperationType.MailboxRestore;
				default:
					return AsyncOperationType.Unknown;
				}
			}
		}

		internal AsyncOperationStatus AsyncOperationStatus
		{
			get
			{
				RequestStatus status = this.Status;
				switch (status)
				{
				case RequestStatus.None:
				case RequestStatus.Queued:
					return AsyncOperationStatus.Queued;
				case RequestStatus.InProgress:
				case RequestStatus.CompletionInProgress:
					return AsyncOperationStatus.InProgress;
				case RequestStatus.AutoSuspended:
					break;
				case RequestStatus.Synced:
				case (RequestStatus)6:
				case (RequestStatus)7:
				case (RequestStatus)8:
				case (RequestStatus)9:
					return AsyncOperationStatus.Failed;
				case RequestStatus.Completed:
				case RequestStatus.CompletedWithWarning:
					return AsyncOperationStatus.Completed;
				default:
					switch (status)
					{
					case RequestStatus.Suspended:
						break;
					case RequestStatus.Failed:
						return AsyncOperationStatus.Failed;
					default:
						return AsyncOperationStatus.Failed;
					}
					break;
				}
				return AsyncOperationStatus.Suspended;
			}
		}

		internal override object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				if (this.isRetired)
				{
					MrsTracer.Common.Warning("Reading data from a retired request...", new object[0]);
				}
				return base[propertyDefinition];
			}
			set
			{
				if (this.isRetired)
				{
					MrsTracer.Common.Warning("Modifying retired request...", new object[0]);
				}
				base[propertyDefinition] = value;
			}
		}

		ISettingsContext ISettingsContextProvider.GetSettingsContext()
		{
			return this.jobConfigContext.Value;
		}

		public override string ToString()
		{
			if (this.UserId != null)
			{
				return this.UserId.ToString();
			}
			if (this.Identity != null)
			{
				return this.Identity.ToString();
			}
			return base.ToString();
		}

		internal static object OrganizationIdGetter(IPropertyBag propertyBag)
		{
			OrganizationId result = OrganizationId.ForestWideOrgId;
			ADObjectId adobjectId = (ADObjectId)propertyBag[RequestJobSchema.OrganizationalUnitRoot];
			ADObjectId adobjectId2 = (ADObjectId)propertyBag[RequestJobSchema.ConfigurationUnit];
			if (adobjectId != null && adobjectId2 != null)
			{
				result = new OrganizationId(adobjectId, adobjectId2);
			}
			return result;
		}

		internal static void OrganizationIdSetter(object value, IPropertyBag propertyBag)
		{
			OrganizationId organizationId = value as OrganizationId;
			if (organizationId != null)
			{
				propertyBag[RequestJobSchema.OrganizationalUnitRoot] = organizationId.OrganizationalUnit;
				propertyBag[RequestJobSchema.ConfigurationUnit] = organizationId.ConfigurationUnit;
			}
		}

		internal static T CreateDummyObject<T>() where T : RequestJobBase, new()
		{
			return RequestJobBase.CreateDummyObject<T>(MRSRequestType.Move);
		}

		internal static T CreateDummyObject<T>(MRSRequestType type) where T : RequestJobBase, new()
		{
			T t = Activator.CreateInstance<T>();
			t.RequestType = type;
			t.isFake = true;
			return t;
		}

		internal static RequestStatus GetVersionAppropriateStatus(RequestStatus status, ExchangeObjectVersion version)
		{
			if (!version.IsOlderThan(ExchangeObjectVersion.Exchange2012))
			{
				return status;
			}
			switch (status)
			{
			case RequestStatus.None:
			case RequestStatus.Queued:
			case RequestStatus.InProgress:
			case RequestStatus.AutoSuspended:
			case RequestStatus.CompletionInProgress:
			case RequestStatus.Completed:
			case RequestStatus.CompletedWithWarning:
				break;
			case RequestStatus.Synced:
				return RequestStatus.InProgress;
			case (RequestStatus)6:
			case (RequestStatus)7:
			case (RequestStatus)8:
			case (RequestStatus)9:
				return status;
			default:
				switch (status)
				{
				case RequestStatus.Suspended:
				case RequestStatus.Failed:
					break;
				default:
					return status;
				}
				break;
			}
			return status;
		}

		internal static RequestFlags GetVersionAppropriateFlags(RequestFlags flags, ExchangeObjectVersion version)
		{
			if (!version.IsOlderThan(ExchangeObjectVersion.Exchange2012))
			{
				return flags;
			}
			return flags;
		}

		internal bool ValidateUser(ADUser user, ADObjectId userId)
		{
			string mrUserId = (userId != null) ? userId.ToString() : string.Empty;
			if (user == null)
			{
				this.ValidationResult = new RequestJobBase.ValidationResultEnum?(RequestJobBase.ValidationResultEnum.Orphaned);
				this.ValidationMessage = MrsStrings.ValidationUserIsNotInAD(mrUserId);
				return false;
			}
			return true;
		}

		internal bool ValidateMailbox(ADUser user, bool archive)
		{
			string jobUser = (user.Id != null) ? user.Id.ToString() : string.Empty;
			if ((user.RecipientType != RecipientType.UserMailbox && (!archive || user.RecipientType != RecipientType.MailUser)) || (archive && (user.ArchiveGuid == Guid.Empty || user.ArchiveDatabase == null)) || (!archive && (user.ExchangeGuid == Guid.Empty || user.Database == null)))
			{
				this.ValidationResult = new RequestJobBase.ValidationResultEnum?(RequestJobBase.ValidationResultEnum.DataMissing);
				this.ValidationMessage = MrsStrings.ValidationUserLacksMailbox(jobUser);
				return false;
			}
			return true;
		}

		internal bool ValidateOutlookAnywhereParams()
		{
			if (string.IsNullOrEmpty(this.RemoteMailboxLegacyDN))
			{
				this.ValidationResult = new RequestJobBase.ValidationResultEnum?(RequestJobBase.ValidationResultEnum.DataMissing);
				this.ValidationMessage = MrsStrings.ValidationValueIsMissing("RemoteMailboxLegacyDN");
				return false;
			}
			if (string.IsNullOrEmpty(this.RemoteMailboxServerLegacyDN))
			{
				this.ValidationResult = new RequestJobBase.ValidationResultEnum?(RequestJobBase.ValidationResultEnum.DataMissing);
				this.ValidationMessage = MrsStrings.ValidationValueIsMissing("RemoteMailboxServerLegacyDN");
				return false;
			}
			if (this.RemoteCredential == null)
			{
				this.ValidationResult = new RequestJobBase.ValidationResultEnum?(RequestJobBase.ValidationResultEnum.DataMissing);
				this.ValidationMessage = MrsStrings.ValidationValueIsMissing("RemoteCredential");
				return false;
			}
			return true;
		}

		internal bool ValidateRequestIndexEntries()
		{
			RequestJobBase.ValidationResultEnum validationResultEnum = RequestJobBase.ValidationResultEnum.Orphaned;
			LocalizedString localizedString = MrsStrings.ValidationNoIndexEntryForRequest(this.ToString());
			if (this.IndexEntries != null)
			{
				foreach (IRequestIndexEntry requestIndexEntry in this.IndexEntries)
				{
					if (requestIndexEntry.RequestGuid.Equals(this.RequestGuid))
					{
						if (this.RequestQueue != null && !this.RequestQueue.Equals(requestIndexEntry.StorageMDB))
						{
							if (validationResultEnum == RequestJobBase.ValidationResultEnum.Orphaned)
							{
								localizedString = MrsStrings.ValidationStorageMDBMismatch((requestIndexEntry.StorageMDB == null) ? "(null)" : requestIndexEntry.StorageMDB.ToString(), (this.RequestQueue == null) ? "(null)" : this.RequestQueue.ToString());
							}
						}
						else if (!this.OrganizationId.Equals(requestIndexEntry.OrganizationId))
						{
							validationResultEnum = RequestJobBase.ValidationResultEnum.DataMismatch;
							localizedString = MrsStrings.ValidationOrganizationMismatch(this.OrganizationId.ToString(), (requestIndexEntry.OrganizationId == null) ? "(null)" : requestIndexEntry.OrganizationId.ToString());
						}
						else
						{
							if (this.Flags != requestIndexEntry.Flags)
							{
								if ((this.Flags & RequestJobBase.StaticFlags) != (requestIndexEntry.Flags & RequestJobBase.StaticFlags))
								{
									MrsTracer.Common.Error("Mismatched RequestJob: flags don't match: Index Entry [{0}], Request Job [{1}]", new object[]
									{
										requestIndexEntry.Flags,
										this.Flags
									});
									validationResultEnum = RequestJobBase.ValidationResultEnum.DataMismatch;
									localizedString = MrsStrings.ValidationFlagsMismatch2(this.Flags.ToString(), requestIndexEntry.Flags.ToString());
									continue;
								}
								MrsTracer.Common.Debug("Possibly mismatched RequestJob: flags don't match: Index Entry [{0}], Request Job [{1}]", new object[]
								{
									requestIndexEntry.Flags,
									this.Flags
								});
							}
							if (this.RequestType != requestIndexEntry.Type)
							{
								validationResultEnum = RequestJobBase.ValidationResultEnum.DataMismatch;
								localizedString = MrsStrings.ValidationRequestTypeMismatch(this.RequestType.ToString(), requestIndexEntry.Type.ToString());
							}
							else if ((string.IsNullOrEmpty(this.Name) && !string.IsNullOrEmpty(requestIndexEntry.Name)) || (!string.IsNullOrEmpty(this.Name) && string.IsNullOrEmpty(requestIndexEntry.Name)) || !this.Name.Equals(requestIndexEntry.Name))
							{
								validationResultEnum = RequestJobBase.ValidationResultEnum.DataMismatch;
								localizedString = MrsStrings.ValidationNameMismatch(this.Name, requestIndexEntry.Name);
							}
							else if (this.SourceUserId != null && !this.SourceUserId.Equals(requestIndexEntry.SourceUserId))
							{
								if (requestIndexEntry.SourceUserId == null)
								{
									validationResultEnum = RequestJobBase.ValidationResultEnum.DataMissing;
									localizedString = MrsStrings.ValidationValueIsMissing("SourceUserId");
								}
								else
								{
									validationResultEnum = RequestJobBase.ValidationResultEnum.DataMismatch;
									localizedString = MrsStrings.ValidationSourceUserMismatch((this.SourceUserId == null) ? "(null)" : this.SourceUserId.ToString(), (requestIndexEntry.SourceUserId == null) ? "(null)" : requestIndexEntry.SourceUserId.ToString());
								}
							}
							else
							{
								if (this.TargetUserId == null || this.TargetUserId.Equals(requestIndexEntry.TargetUserId))
								{
									this.ValidationResult = new RequestJobBase.ValidationResultEnum?(RequestJobBase.ValidationResultEnum.Valid);
									this.ValidationMessage = LocalizedString.Empty;
									return true;
								}
								if (requestIndexEntry.TargetUserId == null)
								{
									validationResultEnum = RequestJobBase.ValidationResultEnum.DataMissing;
									localizedString = MrsStrings.ValidationValueIsMissing("TargetUserId");
								}
								else
								{
									validationResultEnum = RequestJobBase.ValidationResultEnum.DataMismatch;
									localizedString = MrsStrings.ValidationTargetUserMismatch((this.TargetUserId == null) ? "(null)" : this.TargetUserId.ToString(), (requestIndexEntry.TargetUserId == null) ? "(null)" : requestIndexEntry.TargetUserId.ToString());
								}
							}
						}
					}
				}
			}
			this.ValidationResult = new RequestJobBase.ValidationResultEnum?(validationResultEnum);
			this.ValidationMessage = localizedString;
			return false;
		}

		internal void UpdateAsyncNotification(ReportData report)
		{
			if (!this.UseAsyncNotificationAPI)
			{
				return;
			}
			RequestStatus status = this.Status;
			switch (status)
			{
			case RequestStatus.Completed:
			case RequestStatus.CompletedWithWarning:
				break;
			default:
				if (status != RequestStatus.Failed)
				{
					AsyncOperationNotificationDataProvider.UpdateNotification(this.OrganizationId, this.RequestGuid.ToString(), new AsyncOperationStatus?(this.AsyncOperationStatus), new int?(this.PercentComplete), new LocalizedString?(this.Message), false, null);
					return;
				}
				break;
			}
			List<LocalizedString> list = new List<LocalizedString>();
			foreach (ReportEntry reportEntry in report.Entries)
			{
				list.Add(((ILocalizedString)reportEntry).LocalizedString);
			}
			AsyncOperationNotificationDataProvider.CompleteNotification(this.OrganizationId, this.RequestGuid.ToString(), new LocalizedString?(this.Message), list, this.Status != RequestStatus.Failed, new int?(this.PercentComplete), false);
		}

		internal void CreateAsyncNotification(ADRecipientOrAddress requestCreator, params KeyValuePair<string, LocalizedString>[] extendedAttributes)
		{
			if (!this.UseAsyncNotificationAPI)
			{
				return;
			}
			AsyncOperationNotificationDataProvider.CreateNotification(this.OrganizationId, this.RequestGuid.ToString(), this.AsyncOperationType, new LocalizedString(this.Name), requestCreator, extendedAttributes, false);
		}

		internal void RemoveAsyncNotification()
		{
			if (!this.UseAsyncNotificationAPI)
			{
				return;
			}
			AsyncOperationNotificationDataProvider.RemoveNotification(this.OrganizationId, this.RequestGuid.ToString(), false);
		}

		internal void ValidateRequestJob()
		{
			if (this.RequestType == MRSRequestType.Move)
			{
				MoveRequestStatistics.ValidateRequestJob(this);
			}
			else if (this.RequestType == MRSRequestType.Merge)
			{
				MergeRequestStatistics.ValidateRequestJob(this);
			}
			else if (this.RequestType == MRSRequestType.MailboxImport)
			{
				MailboxImportRequestStatistics.ValidateRequestJob(this);
			}
			else if (this.RequestType == MRSRequestType.MailboxExport)
			{
				MailboxExportRequestStatistics.ValidateRequestJob(this);
			}
			else if (this.RequestType == MRSRequestType.MailboxRestore)
			{
				MailboxRestoreRequestStatistics.ValidateRequestJob(this);
			}
			else if (this.RequestType == MRSRequestType.PublicFolderMove)
			{
				PublicFolderMoveRequestStatistics.ValidateRequestJob(this);
			}
			else if (this.RequestType == MRSRequestType.PublicFolderMigration)
			{
				PublicFolderMigrationRequestStatistics.ValidateRequestJob(this);
			}
			else if (this.RequestType == MRSRequestType.PublicFolderMailboxMigration)
			{
				PublicFolderMailboxMigrationRequestStatistics.ValidateRequestJob(this);
			}
			else if (this.RequestType == MRSRequestType.Sync)
			{
				SyncRequestStatistics.ValidateRequestJob(this);
			}
			else if (this.RequestType == MRSRequestType.MailboxRelocation)
			{
				MailboxRelocationRequestStatistics.ValidateRequestJob(this);
			}
			else if (this.RequestType == MRSRequestType.FolderMove)
			{
				FolderMoveRequestStatistics.ValidateRequestJob(this);
			}
			if (string.IsNullOrEmpty(this.UserOrgName))
			{
				if (this.OrganizationId != null && this.OrganizationId.OrganizationalUnit != null)
				{
					this.UserOrgName = this.OrganizationId.OrganizationalUnit.Name;
					return;
				}
				if (this.User != null)
				{
					this.UserOrgName = this.User.Id.DomainId.ToString();
				}
			}
		}

		internal void Retire()
		{
			this.isRetired = true;
			this.propertyBag = new SimpleProviderPropertyBag();
		}

		internal bool ShouldProcessJob()
		{
			bool config;
			using (this.jobConfigContext.Value.Activate())
			{
				config = ConfigBase<MRSConfigSchema>.GetConfig<bool>("IsJobPickupEnabled");
			}
			return config;
		}

		internal bool IsSupported()
		{
			MRSRequestType requestType = this.RequestType;
			return requestType != MRSRequestType.Sync || this.IsSupportedSyncJob();
		}

		internal ProxyControlFlags GetProxyControlFlags()
		{
			ProxyControlFlags proxyControlFlags = ProxyControlFlags.None;
			if (this.ResolveServer)
			{
				proxyControlFlags |= ProxyControlFlags.ResolveServerName;
			}
			if (this.UseTcp)
			{
				proxyControlFlags |= ProxyControlFlags.UseTcp;
			}
			if (this.UseCertificateAuthentication)
			{
				proxyControlFlags |= ProxyControlFlags.UseCertificateToAuthenticate;
			}
			if (this.SyncProtocol == SyncProtocol.Olc)
			{
				proxyControlFlags |= ProxyControlFlags.Olc;
				proxyControlFlags |= ProxyControlFlags.UseCertificateToAuthenticate;
			}
			return proxyControlFlags;
		}

		internal void CopyNonSchematizedPropertiesFrom(RequestJobBase requestJob)
		{
			this.User = requestJob.User;
			this.SourceUser = requestJob.SourceUser;
			this.TargetUser = requestJob.TargetUser;
			this.IndexEntries = requestJob.IndexEntries;
			this.ValidationResult = requestJob.ValidationResult;
			this.ValidationMessage = requestJob.ValidationMessage;
			this.OriginatingMDBGuid = requestJob.OriginatingMDBGuid;
			this.ExternalDirectoryOrganizationId = requestJob.ExternalDirectoryOrganizationId;
			this.PartitionHint = requestJob.PartitionHint;
			this.RemoteCredential = requestJob.RemoteCredential;
			this.SourceCredential = requestJob.SourceCredential;
			this.TargetCredential = requestJob.TargetCredential;
			this.IsFake = requestJob.IsFake;
			this.TimeTracker = requestJob.TimeTracker;
			this.ProgressTracker = requestJob.ProgressTracker;
			this.IndexIds = requestJob.IndexIds;
			this.FolderToMailboxMap = requestJob.FolderToMailboxMap;
			this.FolderList = requestJob.FolderList;
			this.SkippedItemCounts = requestJob.SkippedItemCounts;
			this.FailureHistory = requestJob.FailureHistory;
		}

		internal MailboxReplicationServiceClient CreateMRSClient(IConfigurationSession session, Guid mdbGuid, List<string> unreachableMrsServers)
		{
			string serverNameToConnect = this.GetServerNameToConnect();
			MailboxReplicationServiceClient result;
			if (!string.IsNullOrEmpty(serverNameToConnect))
			{
				result = MailboxReplicationServiceClient.Create(serverNameToConnect);
			}
			else
			{
				result = MailboxReplicationServiceClient.Create(session, this.JobType, mdbGuid, unreachableMrsServers);
			}
			return result;
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			if (this.isFake)
			{
				return;
			}
			base.ValidateRead(errors);
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			if (this.isFake)
			{
				return;
			}
			base.ValidateWrite(errors);
		}

		private bool IsSupportedSyncJob()
		{
			return this.SyncProtocol == SyncProtocol.Imap || this.SyncProtocol == SyncProtocol.Olc || this.SyncProtocol == SyncProtocol.Eas || this.SyncProtocol == SyncProtocol.Pop;
		}

		private void UpdateFlags(RequestFlags setFlag, RequestFlags mask)
		{
			this.Flags = ((this.Flags & ~mask) | setFlag);
		}

		private void UpdateInternalFlags(RequestJobInternalFlags flag, bool value)
		{
			if (value)
			{
				this.RequestJobInternalFlags |= flag;
				return;
			}
			this.RequestJobInternalFlags &= ~flag;
		}

		private string GetServerNameToConnect()
		{
			string result = null;
			if (this.RequestJobState == JobProcessingState.InProgress && this.IdleTime < TimeSpan.FromMinutes(60.0) && !string.IsNullOrEmpty(this.MRSServerName))
			{
				result = this.MRSServerName;
			}
			else if (this.WorkItemQueueMdb != null)
			{
				DatabaseInformation databaseInformation = MapiUtils.FindServerForMdb(this.WorkItemQueueMdb.ObjectGuid, null, null, FindServerFlags.None);
				if (!string.IsNullOrEmpty(databaseInformation.ServerFqdn))
				{
					result = databaseInformation.ServerFqdn;
				}
			}
			return result;
		}

		private SettingsContextBase CreateConfigContext()
		{
			return CommonUtils.CreateConfigContext(this.ExchangeGuid, (this.WorkItemQueueMdb == null) ? Guid.Empty : this.WorkItemQueueMdb.ObjectGuid, this.OrganizationId, this.WorkloadType, this.RequestType, this.SyncProtocol);
		}

		internal static readonly RequestFlags StaticFlags = RequestFlags.CrossOrg | RequestFlags.IntraOrg | RequestFlags.Push | RequestFlags.Pull | RequestFlags.RemoteLegacy;

		internal static readonly ObjectSchema Schema = ObjectSchema.GetInstance<RequestJobSchema>();

		private List<RequestIndexId> indexIds;

		private RequestJobTimeTracker timeTracker;

		private bool isFake;

		private bool isRetired;

		private List<FolderToMailboxMapping> folderToMailboxMapping;

		private List<MoveFolderInfo> folderList;

		[NonSerialized]
		private NetworkCredential remoteCredential;

		[NonSerialized]
		private NetworkCredential sourceCredential;

		[NonSerialized]
		private NetworkCredential targetCredential;

		[NonSerialized]
		private ADUser adUser;

		[NonSerialized]
		private ADUser sourceUser;

		[NonSerialized]
		private ADUser targetUser;

		[NonSerialized]
		private List<IRequestIndexEntry> indexEntries;

		private RequestJobBase.ValidationResultEnum? validationResult;

		private LocalizedString validationMessage;

		private Guid originatingMDBGuid;

		[NonSerialized]
		private SkippedItemCounts skippedItemCounts;

		[NonSerialized]
		private FailureHistory failureHistory;

		[NonSerialized]
		private Lazy<SettingsContextBase> jobConfigContext;

		internal enum ValidationResultEnum
		{
			Valid,
			Orphaned,
			DataMismatch,
			DataMissing
		}
	}
}
