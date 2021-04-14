using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters
{
	internal class ParticipantWrapper : ParticipantWrapper<Participant>
	{
		public ParticipantWrapper(Participant participant) : base(participant)
		{
		}

		public override Participant Participant
		{
			get
			{
				return base.Original;
			}
		}
	}
}
