using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters
{
	internal class OrganizerConverter : ParticipantConverter<Participant, ParticipantWrapper, Organizer>
	{
		public OrganizerConverter(IParticipantRoutingTypeConverter routingTypeConverter) : base(routingTypeConverter)
		{
		}

		protected override ParticipantWrapper WrapStorageType(Participant value)
		{
			return new ParticipantWrapper(value);
		}
	}
}
