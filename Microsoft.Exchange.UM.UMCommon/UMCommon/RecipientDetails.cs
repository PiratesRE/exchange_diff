using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class RecipientDetails
	{
		internal RecipientDetails(RecipientCollection recipients)
		{
			this.count = recipients.Count;
			if (recipients.Count > 0)
			{
				foreach (Recipient recipient in recipients)
				{
					this.participants.Add(recipient.Participant);
				}
				if (recipients.Count == 1)
				{
					if (recipients[0].IsDistributionList() != null && recipients[0].IsDistributionList().Value)
					{
						this.isDistributionList = true;
						return;
					}
					if (string.Equals(recipients[0].Participant.RoutingType, "MAPIPDL", StringComparison.OrdinalIgnoreCase))
					{
						this.isPersonalDistributionList = true;
					}
				}
			}
		}

		internal bool IsDistributionList
		{
			get
			{
				return this.isDistributionList;
			}
		}

		internal bool IsPersonalDistributionList
		{
			get
			{
				return this.isPersonalDistributionList;
			}
		}

		internal List<Participant> Participants
		{
			get
			{
				return this.participants;
			}
		}

		internal int Count
		{
			get
			{
				return this.count;
			}
		}

		private int count;

		private bool isDistributionList;

		private bool isPersonalDistributionList;

		private List<Participant> participants = new List<Participant>();
	}
}
