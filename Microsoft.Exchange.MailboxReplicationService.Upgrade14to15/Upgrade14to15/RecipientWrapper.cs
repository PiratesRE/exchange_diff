using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	public class RecipientWrapper
	{
		public RecipientWrapper(string identity)
		{
			this.Identity = identity;
		}

		public RecipientWrapper(Mailbox mailbox)
		{
			this.Identity = mailbox.Identity.ToString();
			this.PersistedCapabilities = mailbox.PersistedCapabilities;
		}

		public RecipientWrapper(ADObjectId id, RequestStatus moveStatus = RequestStatus.None, string moveBatchName = null, RequestFlags requestFlags = RequestFlags.None, RecipientType recipientType = RecipientType.UserMailbox, RecipientTypeDetails recipientTypeDetails = RecipientTypeDetails.None)
		{
			this.Id = id;
			this.MoveStatus = moveStatus;
			this.MailboxMoveBatchName = moveBatchName;
			this.RequestFlags = requestFlags;
			this.RecipientType = recipientType;
			this.RecipientTypeDetails = recipientTypeDetails;
		}

		public RecipientWrapper(User user)
		{
			this.Id = user.Id;
			this.RecipientType = user.RecipientType;
			this.UpgradeStatus = user.UpgradeStatus;
			this.UpgradeRequest = user.UpgradeRequest;
			this.UpgradeMessage = user.UpgradeMessage;
			this.UpgradeDetails = user.UpgradeDetails;
			this.UpgradeStage = user.UpgradeStage;
			this.UpgradeStageTimeStamp = user.UpgradeStageTimeStamp;
		}

		public ADObjectId Id { get; private set; }

		public RequestStatus MoveStatus { get; private set; }

		public RecipientType RecipientType { get; private set; }

		public RequestFlags RequestFlags { get; private set; }

		public string MailboxMoveBatchName { get; private set; }

		public RecipientTypeDetails RecipientTypeDetails { get; private set; }

		public MultiValuedProperty<Capability> PersistedCapabilities { get; set; }

		public string Identity { get; set; }

		public UpgradeStatusTypes UpgradeStatus { get; set; }

		public UpgradeRequestTypes UpgradeRequest { get; set; }

		public string UpgradeMessage { get; set; }

		public string UpgradeDetails { get; set; }

		public UpgradeStage? UpgradeStage { get; set; }

		public DateTime? UpgradeStageTimeStamp { get; set; }
	}
}
