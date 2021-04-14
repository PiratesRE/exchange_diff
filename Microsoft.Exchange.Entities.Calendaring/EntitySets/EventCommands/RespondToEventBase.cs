using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions;
using Microsoft.Exchange.Entities.DataProviders;
using Microsoft.Exchange.Entities.EntitySets.Commands;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class RespondToEventBase : KeyedEntityCommand<Events, VoidResult>
	{
		[DataMember]
		public RespondToEventParameters Parameters { get; set; }

		protected override string GetCommandTraceDetails()
		{
			return string.Format("{0}?Response={1}", base.GetCommandTraceDetails(), this.Parameters.Response);
		}

		protected override void UpdateCustomLoggingData()
		{
			base.UpdateCustomLoggingData();
			this.SetCustomLoggingData("RespondToEventParameters", this.Parameters);
		}

		protected virtual void Validate(Event eventObject)
		{
			if (!eventObject.ResponseRequested && this.Parameters.SendResponse)
			{
				throw new InvalidRequestException(CalendaringStrings.ErrorResponseNotRequested);
			}
			if (eventObject.IsOrganizer)
			{
				throw new InvalidRequestException(CalendaringStrings.ErrorOrganizerCantRespond);
			}
			if (eventObject.IsCancelled)
			{
				throw new InvalidRequestException(CalendaringStrings.ErrorRespondToCancelledEvent);
			}
		}

		protected virtual bool CleanUpDeclinedEvent(StoreId id)
		{
			if (this.Parameters.Response == ResponseType.Declined)
			{
				DeleteItemFlags deleteItemFlags = DeleteItemFlags.MoveToDeletedItems;
				deleteItemFlags |= (this.Parameters.SendResponse ? DeleteItemFlags.DeclineCalendarItemWithResponse : DeleteItemFlags.DeclineCalendarItemWithoutResponse);
				this.Scope.EventDataProvider.Delete(id, deleteItemFlags);
				return true;
			}
			return false;
		}

		protected const DeleteItemFlags MoveToDeletedItems = DeleteItemFlags.MoveToDeletedItems;
	}
}
