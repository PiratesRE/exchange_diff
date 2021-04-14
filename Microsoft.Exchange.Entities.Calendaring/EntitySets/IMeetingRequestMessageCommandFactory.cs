using System;
using Microsoft.Exchange.Entities.Calendaring.EntitySets.MeetingRequestCommands;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions;
using Microsoft.Exchange.Entities.EntitySets;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets
{
	internal interface IMeetingRequestMessageCommandFactory : IEntityCommandFactory<MeetingRequestMessages, MeetingRequestMessage>
	{
		RespondToMeetingRequestMessage CreateRespondCommand(MeetingRequestMessages scope, RespondToEventParameters parameters);
	}
}
