using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.CalendarDiagnostics;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CalendarLocalItem : CalendarInstance
	{
		internal CalendarLocalItem(ExchangePrincipal principal, MailboxSession session) : base(principal)
		{
			this.session = session;
		}

		internal byte[] CalendarFolderId { get; set; }

		internal override CalendarProcessingFlags? GetCalendarConfig()
		{
			CalendarConfiguration calendarConfiguration = null;
			using (CalendarConfigurationDataProvider calendarConfigurationDataProvider = new CalendarConfigurationDataProvider(this.session))
			{
				calendarConfiguration = (CalendarConfiguration)calendarConfigurationDataProvider.Read<CalendarConfiguration>(null);
			}
			if (calendarConfiguration == null)
			{
				Globals.ConsistencyChecksTracer.TraceWarning(0L, "Could not load the attendee's calendar configuration data. Skipping mailbox.");
				return null;
			}
			return new CalendarProcessingFlags?(calendarConfiguration.AutomateProcessing);
		}

		internal override Inconsistency GetInconsistency(CalendarValidationContext context, string fullDescription)
		{
			if (base.LoadInconsistency != null)
			{
				return base.LoadInconsistency;
			}
			if (this.CalendarFolderId == null)
			{
				return null;
			}
			bool isCleanGlobalObjectId = context.BaseItem.GlobalObjectId.IsCleanGlobalObjectId;
			Inconsistency inconsistency;
			if (context.OppositeRole == RoleType.Attendee)
			{
				ClientIntentQuery.QueryResult missingItemIntent = this.GetMissingItemIntent(context, isCleanGlobalObjectId);
				inconsistency = (isCleanGlobalObjectId ? this.GetAttendeeMissingItemInconsistency(context, fullDescription, missingItemIntent, ClientIntentFlags.RespondedDecline, ClientIntentFlags.DeletedWithNoResponse) : this.GetAttendeeMissingItemInconsistency(context, fullDescription, missingItemIntent, ClientIntentFlags.RespondedExceptionDecline, ClientIntentFlags.DeletedExceptionWithNoResponse));
			}
			else
			{
				inconsistency = MissingItemInconsistency.CreateOrganizerMissingItemInstance(fullDescription, context);
				if (RumFactory.Instance.Policy.RepairMode == CalendarRepairType.ValidateOnly)
				{
					inconsistency.Intent = this.GetMissingItemIntent(context, isCleanGlobalObjectId).Intent;
				}
			}
			return inconsistency;
		}

		internal override bool WouldTryToRepairIfMissing(CalendarValidationContext context, out MeetingInquiryAction predictedAction)
		{
			return MeetingInquiryMessage.WouldTryToRepairIfMissing(context.CvsGateway, context.AttendeeItem.GlobalObjectId, this.session, this.CalendarFolderId, out predictedAction);
		}

		internal override ClientIntentFlags? GetLocationIntent(CalendarValidationContext context, GlobalObjectId globalObjectId, string organizerLocation, string attendeeLocation)
		{
			ICalendarItemStateDefinition initialState = new LocationBasedCalendarItemStateDefinition(organizerLocation);
			ICalendarItemStateDefinition targetState = new LocationBasedCalendarItemStateDefinition(attendeeLocation);
			ClientIntentQuery clientIntentQuery = new TransitionalClientIntentQuery(globalObjectId, initialState, targetState);
			return clientIntentQuery.Execute(this.session, context.CvsGateway).Intent;
		}

		private ClientIntentQuery.QueryResult GetMissingItemIntent(CalendarValidationContext context, bool isNotOccurrence)
		{
			ClientIntentQuery clientIntentQuery;
			if (isNotOccurrence)
			{
				ICalendarItemStateDefinition deletedFromFolderStateDefinition = CompositeCalendarItemStateDefinition.GetDeletedFromFolderStateDefinition(this.CalendarFolderId);
				clientIntentQuery = new ItemDeletionClientIntentQuery(context.BaseItem.GlobalObjectId, deletedFromFolderStateDefinition);
			}
			else
			{
				ICalendarItemStateDefinition targetState = new DeletedOccurrenceCalendarItemStateDefinition(context.BaseItem.GlobalObjectId.Date, false);
				clientIntentQuery = new NonTransitionalClientIntentQuery(context.BaseItem.GlobalObjectId, targetState);
			}
			return clientIntentQuery.Execute(this.session, context.CvsGateway);
		}

		private Inconsistency GetAttendeeMissingItemInconsistency(CalendarValidationContext context, string fullDescription, ClientIntentQuery.QueryResult inconsistencyIntent, ClientIntentFlags declineIntent, ClientIntentFlags deleteIntent)
		{
			Inconsistency inconsistency = null;
			if (ClientIntentQuery.CheckDesiredClientIntent(inconsistencyIntent.Intent, new ClientIntentFlags[]
			{
				declineIntent
			}))
			{
				inconsistency = ResponseInconsistency.CreateInstance(fullDescription, ResponseType.Decline, context.Attendee.ResponseType, ExDateTime.MinValue, context.Attendee.ReplyTime, context);
				inconsistency.Intent = inconsistencyIntent.Intent;
			}
			else if (ClientIntentQuery.CheckDesiredClientIntent(inconsistencyIntent.Intent, new ClientIntentFlags[]
			{
				deleteIntent
			}))
			{
				inconsistency = null;
			}
			else if (inconsistencyIntent.SourceVersionId != null)
			{
				int? deletedItemVersion = null;
				using (CalendarItemBase calendarItemBase = CalendarItemBase.Bind(this.session, inconsistencyIntent.SourceVersionId))
				{
					deletedItemVersion = calendarItemBase.GetValueAsNullable<int>(CalendarItemBaseSchema.ItemVersion);
				}
				if (deletedItemVersion != null)
				{
					inconsistency = MissingItemInconsistency.CreateAttendeeMissingItemInstance(fullDescription, inconsistencyIntent.Intent, deletedItemVersion, context);
					inconsistency.Intent = inconsistencyIntent.Intent;
				}
			}
			return inconsistency;
		}

		private MailboxSession session;
	}
}
