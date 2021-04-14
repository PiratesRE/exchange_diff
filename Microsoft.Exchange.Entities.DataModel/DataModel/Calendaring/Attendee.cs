using System;
using Microsoft.Exchange.Entities.DataModel.Items;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring
{
	public class Attendee : Recipient<AttendeeSchema>, IAttendee, IRecipient
	{
		public ResponseStatus Status
		{
			get
			{
				return base.GetPropertyValueOrDefault<ResponseStatus>(base.Schema.StatusProperty);
			}
			set
			{
				base.SetPropertyValue<ResponseStatus>(base.Schema.StatusProperty, value);
			}
		}

		public AttendeeType Type
		{
			get
			{
				return base.GetPropertyValueOrDefault<AttendeeType>(base.Schema.TypeProperty);
			}
			set
			{
				base.SetPropertyValue<AttendeeType>(base.Schema.TypeProperty, value);
			}
		}
	}
}
