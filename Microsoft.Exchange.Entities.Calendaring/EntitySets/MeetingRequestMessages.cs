using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.Calendaring.DataProviders;
using Microsoft.Exchange.Entities.Calendaring.EntitySets.MeetingRequestCommands;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions;
using Microsoft.Exchange.Entities.EntitySets;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MeetingRequestMessages : StorageEntitySet<MeetingRequestMessages, MeetingRequestMessage, IMeetingRequestMessageCommandFactory, IMailboxSession>, IMeetingRequestMessages, IEntitySet<MeetingRequestMessage>
	{
		protected internal MeetingRequestMessages(IStorageEntitySetScope<IMailboxSession> parentScope, IEvents events) : base(parentScope, "MeetingRequestMessages", MeetingRequestMessageCommandFactory.Instance)
		{
			this.Events = events;
		}

		public IEvents Events { get; private set; }

		internal virtual MeetingRequestMessageDataProvider MeetingRequestMessageDataProvider
		{
			get
			{
				MeetingRequestMessageDataProvider result;
				if ((result = this.meetingRequestMessageDataProvider) == null)
				{
					result = (this.meetingRequestMessageDataProvider = new MeetingRequestMessageDataProvider(this));
				}
				return result;
			}
		}

		public void Respond(RespondToEventParameters parameters, CommandContext context = null)
		{
			RespondToMeetingRequestMessage respondToMeetingRequestMessage = base.CommandFactory.CreateRespondCommand(this, parameters);
			respondToMeetingRequestMessage.Execute(context);
		}

		private MeetingRequestMessageDataProvider meetingRequestMessageDataProvider;
	}
}
