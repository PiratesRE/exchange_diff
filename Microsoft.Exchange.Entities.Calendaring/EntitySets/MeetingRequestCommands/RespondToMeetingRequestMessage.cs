using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.Calendaring.DataProviders;
using Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions;
using Microsoft.Exchange.Entities.DataProviders;
using Microsoft.Exchange.Entities.EntitySets.Commands;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.MeetingRequestCommands
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RespondToMeetingRequestMessage : EntityCommand<MeetingRequestMessages, VoidResult>
	{
		[DataMember]
		public RespondToEventParameters Parameters { get; set; }

		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.RespondToMeetingRequestTracer;
			}
		}

		protected void Validate()
		{
			if (this.Parameters == null)
			{
				throw new InvalidRequestException(Strings.ErrorMissingRequiredParameter("RespondToEventParameters"));
			}
			if (this.Parameters.MeetingRequestIdToBeDeleted == null)
			{
				throw new InvalidRequestException(Strings.ErrorMissingRequiredParameter("RespondToEventParameters.MeetingRequestIdToBeDeleted"));
			}
		}

		protected override VoidResult OnExecute()
		{
			this.Validate();
			MeetingRequestMessageDataProvider meetingRequestMessageDataProvider = this.Scope.MeetingRequestMessageDataProvider;
			string changeKey = (this.Context == null) ? null : this.Context.IfMatchETag;
			StoreObjectId id = this.Scope.IdConverter.ToStoreObjectId(this.Parameters.MeetingRequestIdToBeDeleted);
			string key;
			using (IMeetingRequest meetingRequest = meetingRequestMessageDataProvider.BindToWrite(id, changeKey))
			{
				StoreId storeId = meetingRequestMessageDataProvider.GetCorrelatedItemId(meetingRequest);
				if (storeId == null)
				{
					storeId = this.CreateCorrelatedEvent(meetingRequest);
				}
				key = this.Scope.IdConverter.ToStringId(storeId, this.Scope.Session);
			}
			this.Scope.Events.Respond(key, this.Parameters, this.Context);
			return VoidResult.Value;
		}

		protected virtual StoreId CreateCorrelatedEvent(IMeetingRequest meetingRequest)
		{
			CalendarItemBase calendarItemBase = null;
			StoreId result = null;
			try
			{
				if (meetingRequest.TryUpdateCalendarItem(ref calendarItemBase, meetingRequest.IsDelegated()))
				{
					calendarItemBase.Save(SaveMode.NoConflictResolution);
					this.Scope.MeetingRequestMessageDataProvider.SaveMeetingRequest(meetingRequest, this.Context);
					result = calendarItemBase.Id;
				}
			}
			finally
			{
				if (calendarItemBase != null)
				{
					calendarItemBase.Dispose();
				}
			}
			return result;
		}
	}
}
