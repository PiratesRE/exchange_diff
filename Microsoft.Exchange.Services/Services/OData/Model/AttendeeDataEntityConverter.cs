using System;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal static class AttendeeDataEntityConverter
	{
		internal static Attendee ToAttendee(this Attendee dataEntityAttendee)
		{
			if (dataEntityAttendee == null)
			{
				return null;
			}
			return new Attendee
			{
				Name = dataEntityAttendee.Name,
				Address = dataEntityAttendee.EmailAddress,
				Status = dataEntityAttendee.Status.ToResponseStatus(),
				Type = EnumConverter.CastEnumType<AttendeeType>(dataEntityAttendee.Type)
			};
		}

		internal static Attendee ToDataEntityAttendee(this Attendee attendee)
		{
			if (attendee == null)
			{
				return null;
			}
			return new Attendee
			{
				Name = attendee.Name,
				EmailAddress = attendee.Address,
				Status = attendee.Status.ToDataEntityResponseStatus(),
				Type = EnumConverter.CastEnumType<AttendeeType>(attendee.Type)
			};
		}
	}
}
