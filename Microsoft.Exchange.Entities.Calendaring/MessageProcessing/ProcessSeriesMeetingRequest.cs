using System;
using Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands;

namespace Microsoft.Exchange.Entities.Calendaring.MessageProcessing
{
	internal class ProcessSeriesMeetingRequest : ProcessMeetingMessageCommand
	{
		protected override void ProcessMessage()
		{
			if (this.SeriesExists())
			{
				this.ProcessSeriesMeetingUpdate();
				return;
			}
			this.ProcessSeriesMeetingCreation();
		}

		private bool SeriesExists()
		{
			return base.Event.Id != null;
		}

		private void ProcessSeriesMeetingUpdate()
		{
		}

		private void ProcessSeriesMeetingCreation()
		{
			base.Event.Occurrences = base.MeetingMessage.OccurrencesExceptionalViewProperties;
			CreateReceivedSeries createReceivedSeries = new CreateReceivedSeries
			{
				Entity = base.Event,
				Scope = this.Scope
			};
			createReceivedSeries.Execute(null);
		}
	}
}
