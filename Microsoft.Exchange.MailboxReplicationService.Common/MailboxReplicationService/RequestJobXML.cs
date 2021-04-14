using System;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[XmlType(TypeName = "MoveRequestXML")]
	[Serializable]
	public sealed class RequestJobXML : RequestJobBase
	{
		public RequestJobXML()
		{
		}

		internal RequestJobXML(SimpleProviderPropertyBag propertyBag) : base(propertyBag)
		{
		}

		internal RequestJobXML(TransactionalRequestJob requestJob) : this((SimpleProviderPropertyBag)requestJob.propertyBag)
		{
			base.CopyNonSchematizedPropertiesFrom(requestJob);
			this.UnknownElements = requestJob.UnknownElements;
		}

		[XmlElement(ElementName = "Identity")]
		public ADObjectIdXML UserIdentityXML
		{
			get
			{
				return ADObjectIdXML.Serialize(base.UserId);
			}
			set
			{
				base.UserId = ADObjectIdXML.Deserialize(value);
			}
		}

		[XmlElement(ElementName = "ExternalDirectoryOrganizationId")]
		public new Guid ExternalDirectoryOrganizationId
		{
			get
			{
				return base.ExternalDirectoryOrganizationId;
			}
			set
			{
				base.ExternalDirectoryOrganizationId = value;
			}
		}

		[XmlElement(ElementName = "PartitionHint")]
		public byte[] PersistablePartitionHint
		{
			get
			{
				if (base.PartitionHint != null)
				{
					return base.PartitionHint.GetPersistablePartitionHint();
				}
				return null;
			}
			set
			{
				base.PartitionHint = ((value == null) ? null : TenantPartitionHint.FromPersistablePartitionHint(value));
			}
		}

		[XmlElement(ElementName = "ExchangeGuid")]
		public new Guid ExchangeGuid
		{
			get
			{
				return base.ExchangeGuid;
			}
			set
			{
				base.ExchangeGuid = value;
			}
		}

		[XmlElement(ElementName = "ArchiveGuid")]
		public new Guid? ArchiveGuid
		{
			get
			{
				return base.ArchiveGuid;
			}
			set
			{
				base.ArchiveGuid = value;
			}
		}

		[XmlElement(ElementName = "MoveRequestGuid")]
		public new Guid RequestGuid
		{
			get
			{
				return base.RequestGuid;
			}
			set
			{
				base.RequestGuid = value;
			}
		}

		[XmlElement(ElementName = "Status")]
		public int StatusInt
		{
			get
			{
				return (int)base.Status;
			}
			set
			{
				base.Status = (RequestStatus)value;
			}
		}

		[XmlElement(ElementName = "MoveState")]
		public int RequestJobStateInt
		{
			get
			{
				return (int)base.RequestJobState;
			}
			set
			{
				base.RequestJobState = (JobProcessingState)value;
			}
		}

		[XmlElement(ElementName = "SyncStage")]
		public int SyncStageInt
		{
			get
			{
				return (int)base.SyncStage;
			}
			set
			{
				base.SyncStage = (SyncStage)value;
			}
		}

		[XmlElement(ElementName = "Flags")]
		public int FlagsInt
		{
			get
			{
				return (int)base.Flags;
			}
			set
			{
				base.Flags = (RequestFlags)value;
			}
		}

		[XmlElement(ElementName = "SourceExchangeGuid")]
		public new Guid SourceExchangeGuid
		{
			get
			{
				return base.SourceExchangeGuid;
			}
			set
			{
				base.SourceExchangeGuid = value;
			}
		}

		[XmlElement(ElementName = "SourceIsArchive")]
		public new bool SourceIsArchive
		{
			get
			{
				return base.SourceIsArchive;
			}
			set
			{
				base.SourceIsArchive = value;
			}
		}

		[XmlElement(ElementName = "SourceRootFolder")]
		public new string SourceRootFolder
		{
			get
			{
				return base.SourceRootFolder;
			}
			set
			{
				base.SourceRootFolder = value;
			}
		}

		[XmlElement(ElementName = "TargetExchangeGuid")]
		public new Guid TargetExchangeGuid
		{
			get
			{
				return base.TargetExchangeGuid;
			}
			set
			{
				base.TargetExchangeGuid = value;
			}
		}

		[XmlElement(ElementName = "TargetIsArchive")]
		public new bool TargetIsArchive
		{
			get
			{
				return base.TargetIsArchive;
			}
			set
			{
				base.TargetIsArchive = value;
			}
		}

		[XmlElement(ElementName = "TargetRootFolder")]
		public new string TargetRootFolder
		{
			get
			{
				return base.TargetRootFolder;
			}
			set
			{
				base.TargetRootFolder = value;
			}
		}

		[XmlElement(ElementName = "ArchiveDomain")]
		public new string ArchiveDomain
		{
			get
			{
				return base.ArchiveDomain;
			}
			set
			{
				base.ArchiveDomain = value;
			}
		}

		[XmlElement(ElementName = "SourceDatabase")]
		public ADObjectIdXML SourceDatabaseXML
		{
			get
			{
				return ADObjectIdXML.Serialize(base.SourceDatabase);
			}
			set
			{
				base.SourceDatabase = ADObjectIdXML.Deserialize(value);
			}
		}

		[XmlElement(ElementName = "SourceArchiveDatabase")]
		public ADObjectIdXML SourceArchiveDatabaseXML
		{
			get
			{
				return ADObjectIdXML.Serialize(base.SourceArchiveDatabase);
			}
			set
			{
				base.SourceArchiveDatabase = ADObjectIdXML.Deserialize(value);
			}
		}

		[XmlElement(ElementName = "SourceVersion")]
		public new int SourceVersion
		{
			get
			{
				return base.SourceVersion;
			}
			set
			{
				base.SourceVersion = value;
			}
		}

		[XmlElement(ElementName = "SourceArchiveVersion")]
		public new int SourceArchiveVersion
		{
			get
			{
				return base.SourceArchiveVersion;
			}
			set
			{
				base.SourceArchiveVersion = value;
			}
		}

		[XmlElement(ElementName = "SourceServer")]
		public new string SourceServer
		{
			get
			{
				return base.SourceServer;
			}
			set
			{
				base.SourceServer = value;
			}
		}

		[XmlElement(ElementName = "SourceArchiveServer")]
		public new string SourceArchiveServer
		{
			get
			{
				return base.SourceArchiveServer;
			}
			set
			{
				base.SourceArchiveServer = value;
			}
		}

		[XmlElement(ElementName = "DestinationDatabase")]
		public ADObjectIdXML DestinationDatabaseXML
		{
			get
			{
				return ADObjectIdXML.Serialize(base.TargetDatabase);
			}
			set
			{
				base.TargetDatabase = ADObjectIdXML.Deserialize(value);
			}
		}

		[XmlElement(ElementName = "ArchiveDestinationDatabase")]
		public ADObjectIdXML ArchiveDestinationDatabaseXML
		{
			get
			{
				return ADObjectIdXML.Serialize(base.TargetArchiveDatabase);
			}
			set
			{
				base.TargetArchiveDatabase = ADObjectIdXML.Deserialize(value);
			}
		}

		[XmlElement(ElementName = "DestinationVersion")]
		public int DestinationVersion
		{
			get
			{
				return base.TargetVersion;
			}
			set
			{
				base.TargetVersion = value;
			}
		}

		[XmlElement(ElementName = "DestinationArchiveVersion")]
		public int DestinationArchiveVersion
		{
			get
			{
				return base.TargetArchiveVersion;
			}
			set
			{
				base.TargetArchiveVersion = value;
			}
		}

		[XmlElement(ElementName = "DestinationServer")]
		public string DestinationServer
		{
			get
			{
				return base.TargetServer;
			}
			set
			{
				base.TargetServer = value;
			}
		}

		[XmlElement(ElementName = "DestinationContainerGuid")]
		public Guid? DestinationContainerGuid
		{
			get
			{
				return base.TargetContainerGuid;
			}
			set
			{
				base.TargetContainerGuid = value;
			}
		}

		[XmlElement(ElementName = "DestinationUnifiedMailboxId")]
		public byte[] DestinationUnifiedMailboxId
		{
			get
			{
				if (base.TargetUnifiedMailboxId != null)
				{
					return base.TargetUnifiedMailboxId.GetBytes();
				}
				return null;
			}
			set
			{
				base.TargetUnifiedMailboxId = ((value == null) ? null : CrossTenantObjectId.Parse(value, true));
			}
		}

		[XmlElement(ElementName = "DestinationArchiveServer")]
		public string DestinationArchiveServer
		{
			get
			{
				return base.TargetArchiveServer;
			}
			set
			{
				base.TargetArchiveServer = value;
			}
		}

		[XmlElement(ElementName = "RequestQueue")]
		public ADObjectIdXML RequestQueueXML
		{
			get
			{
				return ADObjectIdXML.Serialize(base.RequestQueue);
			}
			set
			{
				base.RequestQueue = ADObjectIdXML.Deserialize(value);
			}
		}

		[XmlElement(ElementName = "RehomeRequest")]
		public new bool RehomeRequest
		{
			get
			{
				return base.RehomeRequest;
			}
			set
			{
				base.RehomeRequest = value;
			}
		}

		[XmlArrayItem(ElementName = "F")]
		[XmlArray(ElementName = "IncludeFolders")]
		public new string[] IncludeFolders
		{
			get
			{
				return base.IncludeFolders;
			}
			set
			{
				base.IncludeFolders = value;
			}
		}

		[XmlArray(ElementName = "ExcludeFolders")]
		[XmlArrayItem(ElementName = "F")]
		public new string[] ExcludeFolders
		{
			get
			{
				return base.ExcludeFolders;
			}
			set
			{
				base.ExcludeFolders = value;
			}
		}

		[XmlElement(ElementName = "ExcludeDumpster")]
		public new bool ExcludeDumpster
		{
			get
			{
				return base.ExcludeDumpster;
			}
			set
			{
				base.ExcludeDumpster = value;
			}
		}

		[XmlElement(ElementName = "SourceDCName")]
		public new string SourceDCName
		{
			get
			{
				return base.SourceDCName;
			}
			set
			{
				base.SourceDCName = value;
			}
		}

		[XmlElement(ElementName = "SourceCredential")]
		public NetworkCredentialXML SourceCredentialXML
		{
			get
			{
				return NetworkCredentialXML.Get(base.SourceCredential);
			}
			set
			{
				base.SourceCredential = NetworkCredentialXML.Get(value);
			}
		}

		[XmlElement(ElementName = "DestinationDCName")]
		public string DestinationDCName
		{
			get
			{
				return base.TargetDCName;
			}
			set
			{
				base.TargetDCName = value;
			}
		}

		[XmlElement(ElementName = "DestinationCredential")]
		public NetworkCredentialXML DestinationCredentialXML
		{
			get
			{
				return NetworkCredentialXML.Get(base.TargetCredential);
			}
			set
			{
				base.TargetCredential = NetworkCredentialXML.Get(value);
			}
		}

		[XmlElement(ElementName = "RemoteHostName")]
		public new string RemoteHostName
		{
			get
			{
				return base.RemoteHostName;
			}
			set
			{
				base.RemoteHostName = value;
			}
		}

		[XmlElement(ElementName = "BatchName")]
		public new string BatchName
		{
			get
			{
				return base.BatchName;
			}
			set
			{
				base.BatchName = value;
			}
		}

		[XmlElement(ElementName = "RemoteCredential")]
		public NetworkCredentialXML RemoteCredentialXML
		{
			get
			{
				return NetworkCredentialXML.Get(base.RemoteCredential);
			}
			set
			{
				base.RemoteCredential = NetworkCredentialXML.Get(value);
			}
		}

		[XmlElement(ElementName = "RemoteDatabaseName")]
		public new string RemoteDatabaseName
		{
			get
			{
				return base.RemoteDatabaseName;
			}
			set
			{
				base.RemoteDatabaseName = value;
			}
		}

		[XmlElement(ElementName = "RemoteDatabaseGuid")]
		public new Guid? RemoteDatabaseGuid
		{
			get
			{
				return base.RemoteDatabaseGuid;
			}
			set
			{
				base.RemoteDatabaseGuid = value;
			}
		}

		[XmlElement(ElementName = "RemoteArchiveDatabaseName")]
		public new string RemoteArchiveDatabaseName
		{
			get
			{
				return base.RemoteArchiveDatabaseName;
			}
			set
			{
				base.RemoteArchiveDatabaseName = value;
			}
		}

		[XmlElement(ElementName = "RemoteArchiveDatabaseGuid")]
		public new Guid? RemoteArchiveDatabaseGuid
		{
			get
			{
				return base.RemoteArchiveDatabaseGuid;
			}
			set
			{
				base.RemoteArchiveDatabaseGuid = value;
			}
		}

		[XmlElement(ElementName = "BadItemLimit")]
		public int BadItemLimitInt
		{
			get
			{
				if (!base.BadItemLimit.IsUnlimited)
				{
					return base.BadItemLimit.Value;
				}
				return -1;
			}
			set
			{
				base.BadItemLimit = ((value < 0) ? Unlimited<int>.UnlimitedValue : new Unlimited<int>(value));
			}
		}

		[XmlElement(ElementName = "BadItemsEncountered")]
		public new int BadItemsEncountered
		{
			get
			{
				return base.BadItemsEncountered;
			}
			set
			{
				base.BadItemsEncountered = value;
			}
		}

		[XmlElement(ElementName = "LargeItemLimit")]
		public int LargeItemLimitInt
		{
			get
			{
				if (!base.LargeItemLimit.IsUnlimited)
				{
					return base.LargeItemLimit.Value;
				}
				return -1;
			}
			set
			{
				base.LargeItemLimit = ((value < 0) ? Unlimited<int>.UnlimitedValue : new Unlimited<int>(value));
			}
		}

		[XmlElement(ElementName = "LargeItemsEncountered")]
		public new int LargeItemsEncountered
		{
			get
			{
				return base.LargeItemsEncountered;
			}
			set
			{
				base.LargeItemsEncountered = value;
			}
		}

		[XmlElement(ElementName = "AllowLargeItems")]
		public new bool AllowLargeItems
		{
			get
			{
				return base.AllowLargeItems;
			}
			set
			{
				base.AllowLargeItems = value;
			}
		}

		[XmlElement(ElementName = "MissingItemsEncountered")]
		public new int MissingItemsEncountered
		{
			get
			{
				return base.MissingItemsEncountered;
			}
			set
			{
				base.MissingItemsEncountered = value;
			}
		}

		[XmlElement(ElementName = "TimeTracker")]
		public new RequestJobTimeTracker TimeTracker
		{
			get
			{
				return base.TimeTracker;
			}
			set
			{
				base.TimeTracker = value;
			}
		}

		[XmlElement(ElementName = "ProgressTracker")]
		public new TransferProgressTracker ProgressTracker
		{
			get
			{
				return base.ProgressTracker;
			}
			set
			{
				base.ProgressTracker += value;
			}
		}

		[XmlArrayItem(ElementName = "Folder")]
		[XmlArray(ElementName = "FolderList")]
		public MoveFolderInfo[] Folders
		{
			get
			{
				return base.FolderList.ToArray();
			}
			set
			{
				base.FolderList.Clear();
				if (value != null)
				{
					base.FolderList.AddRange(value);
				}
			}
		}

		[XmlElement(ElementName = "MoveServerName")]
		public new string MRSServerName
		{
			get
			{
				return base.MRSServerName;
			}
			set
			{
				base.MRSServerName = value;
			}
		}

		[XmlElement(ElementName = "TotalMailboxSize")]
		public new ulong TotalMailboxSize
		{
			get
			{
				return base.TotalMailboxSize;
			}
			set
			{
				base.TotalMailboxSize = value;
			}
		}

		[XmlElement(ElementName = "TotalMailboxItemCount")]
		public new ulong TotalMailboxItemCount
		{
			get
			{
				return base.TotalMailboxItemCount;
			}
			set
			{
				base.TotalMailboxItemCount = value;
			}
		}

		[XmlElement(ElementName = "TotalArchiveSize")]
		public new ulong? TotalArchiveSize
		{
			get
			{
				return base.TotalArchiveSize;
			}
			set
			{
				base.TotalArchiveSize = value;
			}
		}

		[XmlElement(ElementName = "TotalArchiveItemCount")]
		public new ulong? TotalArchiveItemCount
		{
			get
			{
				return base.TotalArchiveItemCount;
			}
			set
			{
				base.TotalArchiveItemCount = value;
			}
		}

		[XmlElement(ElementName = "BytesTransferred")]
		public string BytesTransferred
		{
			get
			{
				return null;
			}
			set
			{
				ulong num;
				if (value != null && ulong.TryParse(value, out num))
				{
					this.ProgressTracker.BytesTransferred += num;
				}
			}
		}

		[XmlElement(ElementName = "ItemsTransferred")]
		public string ItemsTransferred
		{
			get
			{
				return null;
			}
			set
			{
				ulong num;
				if (value != null && ulong.TryParse(value, out num))
				{
					this.ProgressTracker.ItemsTransferred += num;
				}
			}
		}

		[XmlElement(ElementName = "PercentComplete")]
		public new int PercentComplete
		{
			get
			{
				return base.PercentComplete;
			}
			set
			{
				base.PercentComplete = value;
			}
		}

		[XmlElement(ElementName = "FailureCode")]
		public new int? FailureCode
		{
			get
			{
				return base.FailureCode;
			}
			set
			{
				base.FailureCode = value;
			}
		}

		[XmlElement(ElementName = "FailureType")]
		public new string FailureType
		{
			get
			{
				return base.FailureType;
			}
			set
			{
				base.FailureType = value;
			}
		}

		[XmlElement(ElementName = "FailureSide")]
		public int? FailureSideInt
		{
			get
			{
				if (base.FailureSide == null)
				{
					return null;
				}
				return new int?((int)base.FailureSide.Value);
			}
			set
			{
				base.FailureSide = ((value != null) ? new ExceptionSide?((ExceptionSide)value.Value) : null);
			}
		}

		[XmlElement(ElementName = "MessageData")]
		public byte[] MessageData
		{
			get
			{
				return CommonUtils.ByteSerialize(base.Message);
			}
			set
			{
				base.Message = CommonUtils.ByteDeserialize(value);
			}
		}

		[XmlElement(ElementName = "RetryCount")]
		public new int RetryCount
		{
			get
			{
				return base.RetryCount;
			}
			set
			{
				base.RetryCount = value;
			}
		}

		[XmlElement(ElementName = "TotalRetryCount")]
		public new int TotalRetryCount
		{
			get
			{
				return base.TotalRetryCount;
			}
			set
			{
				base.TotalRetryCount = value;
			}
		}

		[XmlElement(ElementName = "AllowedToFinishMove")]
		public new bool AllowedToFinishMove
		{
			get
			{
				return base.AllowedToFinishMove;
			}
			set
			{
				base.AllowedToFinishMove = value;
			}
		}

		[XmlElement(ElementName = "PreserveMailboxSignature")]
		public new bool PreserveMailboxSignature
		{
			get
			{
				return base.PreserveMailboxSignature;
			}
			set
			{
				base.PreserveMailboxSignature = value;
			}
		}

		[XmlElement(ElementName = "RestartingAfterSignatureChange")]
		public new bool RestartingAfterSignatureChange
		{
			get
			{
				return base.RestartingAfterSignatureChange;
			}
			set
			{
				base.RestartingAfterSignatureChange = value;
			}
		}

		[XmlElement(ElementName = "IsIntegData")]
		public new int? IsIntegData
		{
			get
			{
				return base.IsIntegData;
			}
			set
			{
				base.IsIntegData = value;
			}
		}

		[XmlElement(ElementName = "UserPuid")]
		public new long? UserPuid
		{
			get
			{
				return base.UserPuid;
			}
			set
			{
				base.UserPuid = value;
			}
		}

		[XmlElement(ElementName = "OlcDGroup")]
		public new int? OlcDGroup
		{
			get
			{
				return base.OlcDGroup;
			}
			set
			{
				base.OlcDGroup = value;
			}
		}

		[XmlElement(ElementName = "CancelMove")]
		public new bool CancelRequest
		{
			get
			{
				return base.CancelRequest;
			}
			set
			{
				base.CancelRequest = value;
			}
		}

		[XmlElement(ElementName = "DomainControllerToUpdate")]
		public new string DomainControllerToUpdate
		{
			get
			{
				return base.DomainControllerToUpdate;
			}
			set
			{
				base.DomainControllerToUpdate = value;
			}
		}

		[XmlElement(ElementName = "RemoteDomainControllerToUpdate")]
		public new string RemoteDomainControllerToUpdate
		{
			get
			{
				return base.RemoteDomainControllerToUpdate;
			}
			set
			{
				base.RemoteDomainControllerToUpdate = value;
			}
		}

		[XmlElement(ElementName = "RemoteOrgName")]
		public new string RemoteOrgName
		{
			get
			{
				return base.RemoteOrgName;
			}
			set
			{
				base.RemoteOrgName = value;
			}
		}

		[XmlElement(ElementName = "IgnoreRuleLimitErrors")]
		public new bool IgnoreRuleLimitErrors
		{
			get
			{
				return base.IgnoreRuleLimitErrors;
			}
			set
			{
				base.IgnoreRuleLimitErrors = value;
			}
		}

		[XmlElement(ElementName = "TargetDeliveryDomain")]
		public new string TargetDeliveryDomain
		{
			get
			{
				return base.TargetDeliveryDomain;
			}
			set
			{
				base.TargetDeliveryDomain = value;
			}
		}

		[XmlElement(ElementName = "JobType")]
		public int JobTypeInt
		{
			get
			{
				return (int)base.JobType;
			}
			set
			{
				base.JobType = (MRSJobType)value;
			}
		}

		[XmlArray(ElementName = "IndexIds")]
		[XmlArrayItem(ElementName = "IndexId")]
		public RequestIndexId[] IndexIdsArray
		{
			get
			{
				return base.IndexIds.ToArray();
			}
			set
			{
				base.IndexIds.Clear();
				if (value != null)
				{
					base.IndexIds.AddRange(value);
				}
			}
		}

		[XmlArray(ElementName = "FolderToMailboxes")]
		[XmlArrayItem(ElementName = "FolderToMailbox")]
		public FolderToMailboxMapping[] FolderToMailboxArray
		{
			get
			{
				return base.FolderToMailboxMap.ToArray();
			}
			set
			{
				base.FolderToMailboxMap.Clear();
				if (value != null)
				{
					base.FolderToMailboxMap.AddRange(value);
				}
			}
		}

		[XmlElement(ElementName = "Name")]
		public new string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		[XmlElement(ElementName = "RequestType")]
		public int RequestTypeInt
		{
			get
			{
				return (int)base.RequestType;
			}
			set
			{
				base.RequestType = (MRSRequestType)value;
			}
		}

		[XmlElement(ElementName = "FileName")]
		public new string FilePath
		{
			get
			{
				return base.FilePath;
			}
			set
			{
				base.FilePath = value;
			}
		}

		[XmlElement(ElementName = "MailboxRestoreFlags")]
		public int? MailboxRestoreFlagsInt
		{
			get
			{
				if (base.MailboxRestoreFlags == null)
				{
					return null;
				}
				return new int?((int)base.MailboxRestoreFlags.Value);
			}
			set
			{
				base.MailboxRestoreFlags = ((value != null) ? new MailboxRestoreType?((MailboxRestoreType)value.Value) : null);
			}
		}

		[XmlElement(ElementName = "TargetUserId")]
		public ADObjectIdXML TargetUserIdXML
		{
			get
			{
				return ADObjectIdXML.Serialize(base.TargetUserId);
			}
			set
			{
				base.TargetUserId = ADObjectIdXML.Deserialize(value);
			}
		}

		[XmlElement(ElementName = "SourceUserId")]
		public ADObjectIdXML SourceUserIdXML
		{
			get
			{
				return ADObjectIdXML.Serialize(base.SourceUserId);
			}
			set
			{
				base.SourceUserId = ADObjectIdXML.Deserialize(value);
			}
		}

		[XmlElement(ElementName = "RemoteMailboxLegacyDN")]
		public new string RemoteMailboxLegacyDN
		{
			get
			{
				return base.RemoteMailboxLegacyDN;
			}
			set
			{
				base.RemoteMailboxLegacyDN = value;
			}
		}

		[XmlElement(ElementName = "RemoteUserLegacyDN")]
		public new string RemoteUserLegacyDN
		{
			get
			{
				return base.RemoteUserLegacyDN;
			}
			set
			{
				base.RemoteUserLegacyDN = value;
			}
		}

		[XmlElement(ElementName = "RemoteMailboxServerLegacyDN")]
		public new string RemoteMailboxServerLegacyDN
		{
			get
			{
				return base.RemoteMailboxServerLegacyDN;
			}
			set
			{
				base.RemoteMailboxServerLegacyDN = value;
			}
		}

		[XmlElement(ElementName = "OutlookAnywhereHostName")]
		public new string OutlookAnywhereHostName
		{
			get
			{
				return base.OutlookAnywhereHostName;
			}
			set
			{
				base.OutlookAnywhereHostName = value;
			}
		}

		[XmlElement(ElementName = "AuthenticationMethod")]
		public new AuthenticationMethod? AuthenticationMethod
		{
			get
			{
				return base.AuthenticationMethod;
			}
			set
			{
				base.AuthenticationMethod = value;
			}
		}

		[XmlElement(ElementName = "IsAdministrativeCredential")]
		public new bool? IsAdministrativeCredential
		{
			get
			{
				return base.IsAdministrativeCredential;
			}
			set
			{
				base.IsAdministrativeCredential = value;
			}
		}

		[XmlElement(ElementName = "ConflictResolutionOption")]
		public new ConflictResolutionOption? ConflictResolutionOption
		{
			get
			{
				return base.ConflictResolutionOption;
			}
			set
			{
				base.ConflictResolutionOption = value;
			}
		}

		[XmlElement(ElementName = "AssociatedMessagesCopyOption")]
		public new FAICopyOption? AssociatedMessagesCopyOption
		{
			get
			{
				return base.AssociatedMessagesCopyOption;
			}
			set
			{
				base.AssociatedMessagesCopyOption = value;
			}
		}

		[XmlElement(ElementName = "OrganizationalUnitRoot")]
		public ADObjectIdXML OrganizationalUnitRootXML
		{
			get
			{
				return ADObjectIdXML.Serialize(base.OrganizationalUnitRoot);
			}
			set
			{
				base.OrganizationalUnitRoot = ADObjectIdXML.Deserialize(value);
			}
		}

		[XmlElement(ElementName = "ConfigurationUnit")]
		public ADObjectIdXML ConfigurationUnitXML
		{
			get
			{
				return ADObjectIdXML.Serialize(base.ConfigurationUnit);
			}
			set
			{
				base.ConfigurationUnit = ADObjectIdXML.Deserialize(value);
			}
		}

		[XmlElement(ElementName = "ContentFilter")]
		public new string ContentFilter
		{
			get
			{
				return base.ContentFilter;
			}
			set
			{
				base.ContentFilter = value;
			}
		}

		[XmlElement(ElementName = "ContentFilterLCID")]
		public new int ContentFilterLCID
		{
			get
			{
				return base.ContentFilterLCID;
			}
			set
			{
				base.ContentFilterLCID = value;
			}
		}

		[XmlElement(ElementName = "Priority")]
		public new int Priority
		{
			get
			{
				return (int)base.Priority;
			}
			set
			{
				base.Priority = (RequestPriority)value;
			}
		}

		[XmlElement(ElementName = "WorkloadType")]
		public new int WorkloadType
		{
			get
			{
				return (int)base.WorkloadType;
			}
			set
			{
				base.WorkloadType = (RequestWorkloadType)value;
			}
		}

		[XmlElement(ElementName = "RequestFlags")]
		public int RequestJobInternalFlagsInt
		{
			get
			{
				return (int)base.RequestJobInternalFlags;
			}
			set
			{
				base.RequestJobInternalFlags = (RequestJobInternalFlags)value;
			}
		}

		[XmlElement(ElementName = "CompletedRequestAgeLimitTicks")]
		public long? CompletedRequestAgeLimitTicks
		{
			get
			{
				if (!base.CompletedRequestAgeLimit.IsUnlimited)
				{
					return new long?(base.CompletedRequestAgeLimit.Value.Ticks);
				}
				return null;
			}
			set
			{
				base.CompletedRequestAgeLimit = ((value != null) ? new Unlimited<EnhancedTimeSpan>(new TimeSpan(value.Value)) : Unlimited<EnhancedTimeSpan>.UnlimitedValue);
			}
		}

		[XmlElement(ElementName = "LastPickupTime")]
		public long? LastPickupTimeTicks
		{
			get
			{
				if (base.LastPickupTime == null)
				{
					return null;
				}
				return new long?(base.LastPickupTime.Value.Ticks);
			}
			set
			{
				base.LastPickupTime = ((value != null) ? new DateTime?(new DateTime(value.Value)) : null);
			}
		}

		[XmlElement(ElementName = "RequestCreator")]
		public new string RequestCreator
		{
			get
			{
				return base.RequestCreator;
			}
			set
			{
				base.RequestCreator = value;
			}
		}

		[XmlElement(ElementName = "PoisonCount")]
		public new int PoisonCount
		{
			get
			{
				return base.PoisonCount;
			}
			set
			{
				base.PoisonCount = value;
			}
		}

		[XmlElement(ElementName = "ContentCodePage")]
		public new int? ContentCodePage
		{
			get
			{
				return base.ContentCodePage;
			}
			set
			{
				base.ContentCodePage = value;
			}
		}

		[XmlElement(ElementName = "RemoteHostPort")]
		public new int RemoteHostPort
		{
			get
			{
				return base.RemoteHostPort;
			}
			set
			{
				base.RemoteHostPort = value;
			}
		}

		[XmlElement(ElementName = "SmtpServerName")]
		public new string SmtpServerName
		{
			get
			{
				return base.SmtpServerName;
			}
			set
			{
				base.SmtpServerName = value;
			}
		}

		[XmlElement(ElementName = "SmtpServerPort")]
		public new int SmtpServerPort
		{
			get
			{
				return base.SmtpServerPort;
			}
			set
			{
				base.SmtpServerPort = value;
			}
		}

		[XmlElement(ElementName = "SecurityMechanism")]
		public int SecurityMechanismInt
		{
			get
			{
				return (int)base.SecurityMechanism;
			}
			set
			{
				base.SecurityMechanism = (IMAPSecurityMechanism)value;
			}
		}

		[XmlElement(ElementName = "SyncProtocol")]
		public int SyncProtocolInt
		{
			get
			{
				return (int)base.SyncProtocol;
			}
			set
			{
				base.SyncProtocol = (SyncProtocol)value;
			}
		}

		[XmlElement(ElementName = "EmailAddress")]
		public string EmailAddressString
		{
			get
			{
				return base.EmailAddress.ToString();
			}
			set
			{
				base.EmailAddress = new SmtpAddress(value);
			}
		}

		[XmlElement(ElementName = "IncrementalSyncInterval")]
		public long IncrementalSyncIntervalTicks
		{
			get
			{
				return base.IncrementalSyncInterval.Ticks;
			}
			set
			{
				base.IncrementalSyncInterval = new TimeSpan(value);
			}
		}

		[XmlElement(ElementName = "SkippedItemCounts")]
		public new SkippedItemCounts SkippedItemCounts
		{
			get
			{
				return base.SkippedItemCounts;
			}
			set
			{
				base.SkippedItemCounts = value;
			}
		}

		[XmlElement(ElementName = "FailureHistory")]
		public new FailureHistory FailureHistory
		{
			get
			{
				return base.FailureHistory;
			}
			set
			{
				base.FailureHistory = value;
			}
		}

		[XmlAnyElement]
		public XmlElement[] UnknownElements
		{
			get
			{
				return this.unknownElements;
			}
			set
			{
				this.unknownElements = value;
			}
		}

		internal static MapiFolder GetRequestJobsFolder(MapiStore systemMbx)
		{
			return MapiUtils.OpenFolderUnderRoot(systemMbx, RequestJobXML.RequestJobsFolderName, true);
		}

		internal static string CreateMessageSubject(Guid guid)
		{
			ExDateTime utcNow = ExDateTime.UtcNow;
			return string.Format(CultureInfo.InvariantCulture, "Move Mailbox Work Item : {0} :: {1} :: {2} ", new object[]
			{
				guid.ToString(),
				utcNow.ToLongDateString(),
				utcNow.ToLongTimeString()
			});
		}

		internal static byte[] CreateMessageSearchKey(Guid guid)
		{
			return guid.ToByteArray();
		}

		internal static bool IsMessageTypeSupported(MapiMessage message, MapiStore store)
		{
			RequestJobNamedPropertySet requestJobNamedPropertySet = RequestJobNamedPropertySet.Get(store);
			PropValue prop = message.GetProp(requestJobNamedPropertySet.PropTags[9]);
			MRSJobType value = MapiUtils.GetValue<MRSJobType>(prop, MRSJobType.RequestJobE14R3);
			return RequestJobXML.IsKnownJobType(value);
		}

		internal static bool IsKnownJobType(MRSJobType jobType)
		{
			switch (jobType)
			{
			case MRSJobType.RequestJobE14R4_WithDurations:
			case MRSJobType.RequestJobE14R5_WithImportExportMerge:
			case MRSJobType.RequestJobE14R5_PrimaryOrArchiveExclusiveMoves:
			case MRSJobType.RequestJobE14R6_CompressedReports:
			case MRSJobType.RequestJobE15_TenantHint:
			case MRSJobType.RequestJobE15_AutoResume:
			case MRSJobType.RequestJobE15_SubType:
			case MRSJobType.RequestJobE15_AutoResumeMerges:
			case MRSJobType.RequestJobE15_CreatePublicFoldersUnderParentInSecondary:
				return true;
			default:
				return false;
			}
		}

		internal static MRSCapabilities MapJobTypeToCapability(MRSJobType jobType)
		{
			switch (jobType)
			{
			case MRSJobType.Unknown:
				return MRSCapabilities.E14_RTM;
			case MRSJobType.RequestJobE14R4_WithDurations:
				return MRSCapabilities.E14_RTM;
			case MRSJobType.RequestJobE14R5_WithImportExportMerge:
			case MRSJobType.RequestJobE14R5_PrimaryOrArchiveExclusiveMoves:
			case MRSJobType.RequestJobE14R6_CompressedReports:
				return MRSCapabilities.Merges;
			case MRSJobType.RequestJobE15_TenantHint:
				return MRSCapabilities.TenantHint;
			case MRSJobType.RequestJobE15_AutoResume:
				return MRSCapabilities.AutoResume;
			case MRSJobType.RequestJobE15_SubType:
				return MRSCapabilities.SubType;
			case MRSJobType.RequestJobE15_AutoResumeMerges:
				return MRSCapabilities.AutoResumeMerges;
			case MRSJobType.RequestJobE15_CreatePublicFoldersUnderParentInSecondary:
				return MRSCapabilities.CreatePublicFoldersUnderParentInSecondary;
			}
			return MRSCapabilities.E14_RTM;
		}

		internal static bool IsKnownRequestType(MRSRequestType requestType)
		{
			switch (requestType)
			{
			case MRSRequestType.Move:
			case MRSRequestType.Merge:
			case MRSRequestType.MailboxImport:
			case MRSRequestType.MailboxExport:
			case MRSRequestType.MailboxRestore:
			case MRSRequestType.PublicFolderMove:
			case MRSRequestType.PublicFolderMigration:
			case MRSRequestType.Sync:
			case MRSRequestType.MailboxRelocation:
			case MRSRequestType.FolderMove:
			case MRSRequestType.PublicFolderMailboxMigration:
				return true;
			}
			return false;
		}

		internal PropValue[] GetPropertiesWrittenOnRequestJob(MapiStore store)
		{
			RequestJobNamedPropertySet requestJobNamedPropertySet = RequestJobNamedPropertySet.Get(store);
			return requestJobNamedPropertySet.GetValuesFromRequestJob(this);
		}

		internal static readonly string RequestJobsFolderName = "MailboxReplicationService Move Jobs";

		internal static readonly string RequestJobsMessageClass = "IPM.MS-Exchange.MailboxMove";

		[NonSerialized]
		private XmlElement[] unknownElements;
	}
}
