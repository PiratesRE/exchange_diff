using System;
using System.Globalization;
using System.Web.Security;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ProvisioningStepHandlerBase : IStepHandler
	{
		protected ProvisioningStepHandlerBase(IMigrationDataProvider dataProvider)
		{
			this.DataProvider = dataProvider;
		}

		public bool ExpectMailboxData
		{
			get
			{
				return false;
			}
		}

		private protected IMigrationDataProvider DataProvider { protected get; private set; }

		protected IProvisioningHandler ProvisioningHandler
		{
			get
			{
				return MigrationApplication.ProvisioningHandler;
			}
		}

		public IStepSettings Discover(MigrationJobItem jobItem, MailboxData localMailbox)
		{
			MigrationUtil.ThrowOnNullArgument(jobItem.MigrationJob, "MigrationJob");
			if (jobItem.MigrationType == MigrationType.ExchangeOutlookAnywhere)
			{
				ExchangeProvisioningDataStorage exchangeProvisioningDataStorage = jobItem.ProvisioningData as ExchangeProvisioningDataStorage;
				NspiMigrationDataReader nspiDataReader = jobItem.MigrationJob.SourceEndpoint.GetNspiDataReader(jobItem.MigrationJob);
				ExchangeMigrationRecipient recipientData = nspiDataReader.GetRecipientData(jobItem.RemoteIdentifier ?? jobItem.Identifier, this.GetProvisioningType(jobItem));
				string encryptedPassword = null;
				if ((recipientData.RecipientType == MigrationUserRecipientType.Mailbox || recipientData.RecipientType == MigrationUserRecipientType.Mailuser) && !jobItem.MigrationJob.IsStaged && (exchangeProvisioningDataStorage == null || exchangeProvisioningDataStorage.ExchangeRecipient == null))
				{
					MigrationLogger.Log(MigrationEventType.Verbose, "provisioning a password for job-item {0}", new object[]
					{
						jobItem.Identifier
					});
					string clearString = Membership.GeneratePassword(16, 3);
					encryptedPassword = MigrationServiceFactory.Instance.GetCryptoAdapter().ClearStringToEncryptedString(clearString);
				}
				exchangeProvisioningDataStorage.ExchangeRecipient = recipientData;
				exchangeProvisioningDataStorage.EncryptedPassword = encryptedPassword;
				if (localMailbox != null)
				{
					exchangeProvisioningDataStorage.ExchangeRecipient.DoesADObjectExist = true;
				}
				return exchangeProvisioningDataStorage;
			}
			return null;
		}

		protected abstract ProvisioningType GetProvisioningType(MigrationJobItem jobItem);

		public void Validate(MigrationJobItem jobItem)
		{
		}

		public IStepSnapshot Inject(MigrationJobItem jobItem)
		{
			return null;
		}

		public IStepSnapshot Process(ISnapshotId id, MigrationJobItem jobItem, out bool updated)
		{
			updated = false;
			ProvisioningId provisioningId = (ProvisioningId)id;
			this.EnsureJobRegistered(jobItem.MigrationJob);
			if (!this.ProvisioningHandler.IsItemQueued(provisioningId))
			{
				if (!this.ProvisioningHandler.HasCapacity(jobItem.MigrationJobId))
				{
					throw new ProvisioningThrottledTransientException();
				}
				IProvisioningData provisioningData = this.GetProvisioningData(jobItem);
				if (provisioningData == null)
				{
					return ProvisioningSnapshot.CreateCompleted(provisioningId);
				}
				this.ProvisioningHandler.QueueItem(jobItem.MigrationJobId, jobItem.ProvisioningId, provisioningData);
			}
			IStepSnapshot stepSnapshot = this.RetrieveCurrentSnapshot(provisioningId, jobItem);
			if (stepSnapshot.Status == SnapshotStatus.Finalized)
			{
				this.TryUnregisterJob(provisioningId.JobGuid);
			}
			return stepSnapshot;
		}

		public void Start(ISnapshotId id)
		{
		}

		public IStepSnapshot Stop(ISnapshotId id)
		{
			ProvisioningId provisioningId = (ProvisioningId)id;
			if (!this.ProvisioningHandler.IsJobRegistered(provisioningId.JobGuid))
			{
				return null;
			}
			if (!this.ProvisioningHandler.IsItemQueued(provisioningId))
			{
				return null;
			}
			IStepSnapshot stepSnapshot = this.RetrieveCurrentSnapshot(provisioningId, null);
			if (stepSnapshot.Status == SnapshotStatus.Finalized)
			{
				this.TryUnregisterJob(provisioningId.JobGuid);
			}
			return stepSnapshot;
		}

		public void Delete(ISnapshotId id)
		{
			ProvisioningId provisioningId = (ProvisioningId)id;
			if (!this.ProvisioningHandler.IsJobRegistered(provisioningId.JobGuid))
			{
				return;
			}
			if (!this.ProvisioningHandler.IsItemQueued(provisioningId))
			{
				return;
			}
			if (this.ProvisioningHandler.IsItemCompleted(provisioningId))
			{
				this.ProvisioningHandler.DequeueItem(provisioningId);
				this.TryUnregisterJob(provisioningId.JobGuid);
			}
			this.ProvisioningHandler.CancelItem(provisioningId);
		}

		public bool CanProcess(MigrationJobItem jobItem)
		{
			this.EnsureJobRegistered(jobItem.MigrationJob);
			return this.ProvisioningHandler.HasCapacity(jobItem.MigrationJobId) || this.ProvisioningHandler.IsItemQueued(jobItem.ProvisioningId);
		}

		public abstract MigrationUserStatus ResolvePresentationStatus(MigrationFlags flags, IStepSnapshot stepSnapshot = null);

		protected abstract IProvisioningData GetProvisioningData(MigrationJobItem jobItem);

		private void EnsureJobRegistered(MigrationJob job)
		{
			if (this.ProvisioningHandler.IsJobRegistered(job.JobId))
			{
				return;
			}
			Guid jobId = job.JobId;
			CultureInfo adminCulture = job.AdminCulture;
			Guid ownerExchangeObjectId = job.OwnerExchangeObjectId;
			ADObjectId ownerId = job.OwnerId;
			DelegatedPrincipal delegatedAdminOwner = job.DelegatedAdminOwner;
			if (ownerId == null && delegatedAdminOwner == null)
			{
				throw MigrationHelperBase.CreatePermanentExceptionWithInternalData<MigrationUnknownException>("Cannot do provisioning since both owner id and delegated admin are null");
			}
			this.ProvisioningHandler.RegisterJob(jobId, adminCulture, ownerExchangeObjectId, ownerId, delegatedAdminOwner, job.SubmittedByUserAdminType, this.DataProvider.ADProvider.TenantOrganizationName, this.DataProvider.OrganizationId);
		}

		private void TryUnregisterJob(Guid jobId)
		{
			if (!this.ProvisioningHandler.IsJobRegistered(jobId))
			{
				return;
			}
			if (this.ProvisioningHandler.CanUnregisterJob(jobId))
			{
				this.ProvisioningHandler.UnregisterJob(jobId);
			}
		}

		private IStepSnapshot RetrieveCurrentSnapshot(ProvisioningId provisioningId, MigrationJobItem jobItem)
		{
			if (!this.ProvisioningHandler.IsItemCompleted(provisioningId))
			{
				return ProvisioningSnapshot.CreateInProgress(provisioningId);
			}
			ProvisionedObject provisionedObject = this.ProvisioningHandler.DequeueItem(provisioningId);
			if (provisionedObject.Type == ProvisioningType.GroupMember)
			{
				GroupProvisioningSnapshot groupProvisioningSnapshot = new GroupProvisioningSnapshot(provisionedObject);
				if (jobItem != null && jobItem.MigrationType == MigrationType.ExchangeOutlookAnywhere)
				{
					GroupProvisioningSnapshot groupProvisioningSnapshot2 = jobItem.ProvisioningStatistics as GroupProvisioningSnapshot;
					if (groupProvisioningSnapshot2 != null)
					{
						groupProvisioningSnapshot.CountOfProvisionedMembers += groupProvisioningSnapshot2.CountOfProvisionedMembers;
						groupProvisioningSnapshot.CountOfSkippedMembers += groupProvisioningSnapshot2.CountOfSkippedMembers;
					}
					ExchangeProvisioningDataStorage exchangeProvisioningDataStorage = jobItem.ProvisioningData as ExchangeProvisioningDataStorage;
					MigrationUtil.ThrowOnNullArgument(exchangeProvisioningDataStorage, "we only currently provision groups for Exchange provisioning");
					ExchangeMigrationGroupRecipient exchangeMigrationGroupRecipient = exchangeProvisioningDataStorage.ExchangeRecipient as ExchangeMigrationGroupRecipient;
					MigrationUtil.ThrowOnNullArgument(exchangeMigrationGroupRecipient, "we should only be provisioning a group if we have already discovered its information");
					int num = groupProvisioningSnapshot.CountOfProvisionedMembers + groupProvisioningSnapshot.CountOfSkippedMembers;
					if (exchangeMigrationGroupRecipient.Members == null || num >= exchangeMigrationGroupRecipient.Members.Length)
					{
						groupProvisioningSnapshot.ProvisioningState = GroupMembershipProvisioningState.MemberRetrievedAndProvisioned;
					}
					else
					{
						groupProvisioningSnapshot.Status = SnapshotStatus.InProgress;
					}
				}
				MigrationLogger.Log(MigrationEventType.Verbose, "job-item {0} now totals {1} members provisioned ({2} skipped)", new object[]
				{
					provisioningId.JobItemGuid.ToString(),
					groupProvisioningSnapshot.CountOfProvisionedMembers,
					groupProvisioningSnapshot.CountOfSkippedMembers
				});
				return groupProvisioningSnapshot;
			}
			return new ProvisioningSnapshot(provisionedObject);
		}

		private const int MaxPasswordLength = 16;

		private const int NumberAlphaNumericChars = 3;

		public static readonly MigrationStage[] AllowedStages = new MigrationStage[]
		{
			MigrationStage.Discovery,
			MigrationStage.Processing
		};
	}
}
