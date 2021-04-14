using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class BirthdayCalendarFolderCreator : MessageClassBasedDefaultFolderCreator
	{
		internal BirthdayCalendarFolderCreator() : base(DefaultFolderType.Calendar, "IPF.Appointment.Birthday", false)
		{
		}
	}
}
