using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.Calendaring.EntitySets;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.EntitySets.Commands;

namespace Microsoft.Exchange.Entities.Calendaring.MessageProcessing
{
	internal abstract class ProcessMeetingMessageCommand : EntityCommand<Events, VoidResult>
	{
		public MeetingMessage MeetingMessage { get; set; }

		public Event Event { get; set; }

		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.MeetingMessageProcessingTracer;
			}
		}

		protected sealed override VoidResult OnExecute()
		{
			this.ProcessMessage();
			return VoidResult.Value;
		}

		protected abstract void ProcessMessage();
	}
}
