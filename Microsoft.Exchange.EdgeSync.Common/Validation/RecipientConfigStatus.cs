using System;
using System.Text;

namespace Microsoft.Exchange.EdgeSync.Validation
{
	[Serializable]
	public class RecipientConfigStatus : EdgeConfigStatus
	{
		public RecipientConfigStatus(SyncStatus status, string detail)
		{
			base.SyncStatus = status;
			this.detail = detail;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(base.SyncStatus.ToString());
			if (!string.IsNullOrEmpty(this.detail))
			{
				stringBuilder.Append(" - ");
				stringBuilder.Append(this.detail);
			}
			return stringBuilder.ToString();
		}

		public const string RecipientDoesNotExistInAD = "Recipient doesn't exist in source Active Directory";

		public const string MoreThanOneRecipientsInAD = "More than one recipient found in source Active Directory and may cause NDR on Edge server. RecipientStatus.ConflictObjects contains relevant entries.";

		public const string MoreThanOneRecipientsInADAM = "More than one recipient found in target Edge Server and may cause NDR on Edge server. RecipientStatus.ConflictObjects contains relevant entries.";

		public const string RecipientDoesNotExistInEdge = "Recipient doesn't exist in target Edge Server and may cause NDR on Edge server";

		public const string RecipientAttributesNotSynced = "Recipient exists in target Edge Server but attributes are not synchronized";

		public const string RecipientRequiresAuthToSendTo = "Recipient requires sender authentication and this may cause NDR on Edge server. RecipientStatus.ConflictObjects contains the recipient object in source Active Directory";

		private readonly string detail;
	}
}
