using System;
using Microsoft.Exchange.Entities.DataModel.Items;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring
{
	public interface IAttendee : IRecipient
	{
		ResponseStatus Status { get; set; }

		AttendeeType Type { get; set; }
	}
}
