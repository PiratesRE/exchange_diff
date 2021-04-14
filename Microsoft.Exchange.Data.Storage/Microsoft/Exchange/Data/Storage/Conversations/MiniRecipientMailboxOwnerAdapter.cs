using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MiniRecipientMailboxOwnerAdapter : MailboxOwnerAdapter
	{
		public MiniRecipientMailboxOwnerAdapter(MiniRecipient miniRecipient, IConstraintProvider constraintProvider, RecipientTypeDetails recipientTypeDetails, LogonType logonType) : base(constraintProvider, recipientTypeDetails, logonType)
		{
			this.miniRecipient = miniRecipient;
		}

		protected override IGenericADUser CalculateGenericADUser()
		{
			return new GenericADUser(this.miniRecipient);
		}

		private readonly MiniRecipient miniRecipient;
	}
}
