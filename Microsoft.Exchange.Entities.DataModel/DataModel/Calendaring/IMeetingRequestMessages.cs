using System;
using Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring
{
	public interface IMeetingRequestMessages : IEntitySet<MeetingRequestMessage>
	{
		void Respond(RespondToEventParameters parameters, CommandContext context = null);
	}
}
