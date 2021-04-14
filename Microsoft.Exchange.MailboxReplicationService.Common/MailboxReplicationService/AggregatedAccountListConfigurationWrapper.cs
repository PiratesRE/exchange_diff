using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.Principal;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class AggregatedAccountListConfigurationWrapper : AggregatedAccountListConfiguration, IRequestIndexEntry, IConfigurable, IAggregatedAccountConfigurationWrapper
	{
		public ADUser TargetUser { get; set; }

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
				return AggregatedAccountListConfigurationWrapper.indexId;
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
			if (base.Principal == null)
			{
				base.Principal = AggregatedAccountListConfigurationWrapper.GetExchangePrincipalFromADUser(this.TargetUser);
			}
		}

		public void UpdateData(RequestJobBase requestJob)
		{
			base.Principal = AggregatedAccountListConfigurationWrapper.GetExchangePrincipalFromADUser(requestJob.TargetUser);
			this.TargetUser = requestJob.TargetUser;
			this.TargetExchangeGuid = requestJob.TargetExchangeGuid;
			base.SmtpAddress = requestJob.EmailAddress;
			base.RequestGuid = requestJob.RequestGuid;
			base.AggregatedMailboxGuid = (requestJob.Flags.HasFlag(RequestFlags.TargetIsAggregatedMailbox) ? requestJob.TargetExchangeGuid : Guid.Empty);
		}

		private static ExchangePrincipal GetExchangePrincipalFromADUser(ADUser targetUser)
		{
			return ExchangePrincipal.FromADUser(targetUser, RemotingOptions.AllowCrossSite);
		}

		Guid IRequestIndexEntry.get_RequestGuid()
		{
			return base.RequestGuid;
		}

		void IRequestIndexEntry.set_RequestGuid(Guid A_1)
		{
			base.RequestGuid = A_1;
		}

		private static RequestIndexId indexId = new RequestIndexId(RequestIndexLocation.UserMailboxList);
	}
}
