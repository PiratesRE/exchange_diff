using System;
using Microsoft.Exchange.Entities.Calendaring.EntitySets.MeetingRequestCommands;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions;
using Microsoft.Exchange.Entities.EntitySets;
using Microsoft.Exchange.Entities.EntitySets.Commands;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets
{
	internal sealed class MeetingRequestMessageCommandFactory : IMeetingRequestMessageCommandFactory, IEntityCommandFactory<MeetingRequestMessages, MeetingRequestMessage>
	{
		private MeetingRequestMessageCommandFactory()
		{
		}

		public RespondToMeetingRequestMessage CreateRespondCommand(MeetingRequestMessages scope, RespondToEventParameters respondToEventParameters)
		{
			return new RespondToMeetingRequestMessage
			{
				Scope = scope,
				Parameters = respondToEventParameters
			};
		}

		public ICreateEntityCommand<MeetingRequestMessages, MeetingRequestMessage> CreateCreateCommand(MeetingRequestMessage entity, MeetingRequestMessages scope)
		{
			throw new NotImplementedException();
		}

		public IDeleteEntityCommand<MeetingRequestMessages> CreateDeleteCommand(string key, MeetingRequestMessages scope)
		{
			throw new NotImplementedException();
		}

		public IFindEntitiesCommand<MeetingRequestMessages, MeetingRequestMessage> CreateFindCommand(IEntityQueryOptions queryOptions, MeetingRequestMessages scope)
		{
			throw new NotImplementedException();
		}

		public IReadEntityCommand<MeetingRequestMessages, MeetingRequestMessage> CreateReadCommand(string key, MeetingRequestMessages scope)
		{
			throw new NotImplementedException();
		}

		public IUpdateEntityCommand<MeetingRequestMessages, MeetingRequestMessage> CreateUpdateCommand(string key, MeetingRequestMessage entity, MeetingRequestMessages scope)
		{
			throw new NotImplementedException();
		}

		public static readonly IMeetingRequestMessageCommandFactory Instance = new MeetingRequestMessageCommandFactory();
	}
}
