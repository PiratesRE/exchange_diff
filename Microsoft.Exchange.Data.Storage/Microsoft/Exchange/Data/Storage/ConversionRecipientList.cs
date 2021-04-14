using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ConversionRecipientList : List<ConversionRecipientEntry>, IConversionParticipantList
	{
		internal ConversionRecipientList()
		{
		}

		public new int Count
		{
			get
			{
				return base.Count;
			}
		}

		public Participant this[int index]
		{
			get
			{
				return base[index].Participant;
			}
			set
			{
				base[index].Participant = value;
			}
		}

		public bool IsConversionParticipantAlwaysResolvable(int index)
		{
			return true;
		}

		internal List<ConversionRecipientEntry> Recipients
		{
			get
			{
				return this;
			}
		}
	}
}
