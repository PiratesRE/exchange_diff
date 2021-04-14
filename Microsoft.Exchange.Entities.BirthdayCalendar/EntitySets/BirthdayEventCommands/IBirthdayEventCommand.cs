using System;

namespace Microsoft.Exchange.Entities.BirthdayCalendar.EntitySets.BirthdayEventCommands
{
	internal interface IBirthdayEventCommand
	{
		BirthdayEventCommandResult ExecuteAndGetResult();
	}
}
