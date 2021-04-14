using System;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal static class OrganizerDataEntityConverter
	{
		internal static Recipient ToRecipient(this Organizer dataEntityOrganizer)
		{
			if (dataEntityOrganizer == null)
			{
				return null;
			}
			return new Recipient
			{
				Name = dataEntityOrganizer.Name,
				Address = dataEntityOrganizer.EmailAddress
			};
		}
	}
}
