using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RecipientCollection : RecipientBaseCollection<Recipient>, IRecipientBaseCollection<Recipient>, IList<Recipient>, ICollection<Recipient>, IEnumerable<Recipient>, IEnumerable
	{
		internal RecipientCollection(CoreRecipientCollection coreRecipientCollection) : base(coreRecipientCollection)
		{
		}

		public Recipient Add(Participant participant, RecipientItemType recipItemType)
		{
			CoreRecipient coreRecipient = base.CreateCoreRecipient(new CoreRecipient.SetDefaultPropertiesDelegate(Recipient.SetDefaultRecipientProperties), participant);
			Recipient recipient = this.ConstructStronglyTypedRecipient(coreRecipient);
			recipient.RecipientItemType = recipItemType;
			return recipient;
		}

		public override Recipient Add(Participant participant)
		{
			if (participant == null)
			{
				throw new ArgumentNullException("participant");
			}
			return this.Add(participant, RecipientItemType.To);
		}

		public override void Add(Recipient recipient)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("item");
			}
			this.Add(recipient.Participant, recipient.RecipientItemType);
		}

		internal void CopyRecipientsFrom(RecipientCollection recipientCollection)
		{
			base.CoreItem.Recipients.CopyRecipientsFrom(recipientCollection.CoreItem.Recipients);
		}

		protected override Recipient ConstructStronglyTypedRecipient(CoreRecipient coreRecipient)
		{
			return new Recipient(coreRecipient);
		}

		public bool Contains(Participant participant)
		{
			return this.Contains(participant, false);
		}

		public bool Contains(Participant participant, bool canLookup)
		{
			if (participant == null)
			{
				throw new ArgumentNullException("participant");
			}
			foreach (Recipient recipient in this)
			{
				if (Participant.HasSameEmail(recipient.Participant, participant, canLookup))
				{
					return true;
				}
			}
			return false;
		}
	}
}
