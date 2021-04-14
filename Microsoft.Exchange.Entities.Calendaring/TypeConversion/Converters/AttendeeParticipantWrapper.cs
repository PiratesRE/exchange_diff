using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters
{
	internal class AttendeeParticipantWrapper : ParticipantWrapper<Attendee>
	{
		public AttendeeParticipantWrapper(Attendee attendee) : base(attendee)
		{
		}

		public override Participant Participant
		{
			get
			{
				if (base.Original != null)
				{
					return base.Original.Participant;
				}
				return null;
			}
		}
	}
}
