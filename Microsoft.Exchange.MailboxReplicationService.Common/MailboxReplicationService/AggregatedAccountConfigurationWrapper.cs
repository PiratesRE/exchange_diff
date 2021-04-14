using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	internal class AggregatedAccountConfigurationWrapper : AggregatedAccountConfiguration, IRequestIndexEntry, IConfigurable, IAggregatedAccountConfigurationWrapper
	{
		public ADUser TargetUser { get; set; }

		public Guid RequestGuid
		{
			get
			{
				Guid? syncRequestGuid = base.SyncRequestGuid;
				if (syncRequestGuid == null)
				{
					return Guid.Empty;
				}
				return syncRequestGuid.GetValueOrDefault();
			}
			set
			{
				base.SyncRequestGuid = new Guid?(value);
			}
		}

		public string Name { get; set; }

		public RequestStatus Status { get; set; }

		public RequestFlags Flags { get; set; }

		public string RemoteHostName { get; set; }

		public string BatchName { get; set; }

		public ADObjectId SourceMDB { get; set; }

		public ADObjectId TargetMDB { get; set; }

		public ADObjectId StorageMDB { get; set; }

		public string FilePath { get; set; }

		public MRSRequestType Type
		{
			get
			{
				return MRSRequestType.Sync;
			}
			set
			{
			}
		}

		public ADObjectId TargetUserId { get; set; }

		public Guid TargetExchangeGuid { get; set; }

		public ADObjectId SourceUserId { get; set; }

		public OrganizationId OrganizationId { get; set; }

		public DateTime? WhenChanged { get; set; }

		public DateTime? WhenCreated { get; set; }

		public DateTime? WhenChangedUTC { get; set; }

		public DateTime? WhenCreatedUTC { get; set; }

		public RequestIndexId RequestIndexId
		{
			get
			{
				return AggregatedAccountConfigurationWrapper.indexId;
			}
		}

		public RequestJobObjectId GetRequestJobId()
		{
			throw new NotImplementedException();
		}

		public RequestIndexEntryObjectId GetRequestIndexEntryId(RequestBase owner)
		{
			throw new NotImplementedException();
		}

		public IExchangePrincipal GetExchangePrincipal()
		{
			return base.Principal;
		}

		public void SetExchangePrincipal()
		{
			base.Principal = AggregatedAccountConfigurationWrapper.GetExchangePrincipal(this.TargetUser, this.TargetExchangeGuid, this.Flags.HasFlag(RequestFlags.TargetIsAggregatedMailbox));
		}

		public void UpdateData(RequestJobBase requestJob)
		{
			base.Principal = AggregatedAccountConfigurationWrapper.GetExchangePrincipal(requestJob.TargetUser, requestJob.TargetExchangeGuid, requestJob.Flags.HasFlag(RequestFlags.TargetIsAggregatedMailbox));
			this.TargetUser = requestJob.TargetUser;
			this.TargetExchangeGuid = requestJob.TargetExchangeGuid;
			base.EmailAddress = new SmtpAddress?(requestJob.EmailAddress);
			base.SyncFailureCode = requestJob.FailureCode;
			base.SyncFailureTimestamp = (ExDateTime?)requestJob.TimeTracker.GetTimestamp(RequestJobTimestamp.Failure);
			base.SyncFailureType = requestJob.FailureType;
			base.SyncLastUpdateTimestamp = (ExDateTime?)requestJob.TimeTracker.GetTimestamp(RequestJobTimestamp.LastUpdate);
			base.SyncQueuedTimestamp = (ExDateTime?)requestJob.TimeTracker.GetTimestamp(RequestJobTimestamp.Creation);
			base.SyncRequestGuid = new Guid?(requestJob.RequestGuid);
			base.SyncStartTimestamp = (ExDateTime?)requestJob.TimeTracker.GetTimestamp(RequestJobTimestamp.Start);
			base.SyncStatus = new RequestStatus?(requestJob.Status);
			base.SyncSuspendedTimestamp = (ExDateTime?)requestJob.TimeTracker.GetTimestamp(RequestJobTimestamp.Suspended);
		}

		private static ExchangePrincipal GetExchangePrincipal(ADUser targetUser, Guid targetExchangeGuid, bool isAggregated)
		{
			if (!isAggregated)
			{
				return ExchangePrincipal.FromADUser(targetUser, RemotingOptions.AllowCrossSite);
			}
			return ExchangePrincipal.FromADUser(targetUser, RemotingOptions.AllowCrossSite).GetAggregatedExchangePrincipal(targetExchangeGuid);
		}

		private static RequestIndexId indexId = new RequestIndexId(RequestIndexLocation.UserMailbox);
	}
}
