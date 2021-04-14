using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RumFactory
	{
		private RumFactory()
		{
			this.isInitialized = false;
		}

		internal static RumFactory Instance
		{
			get
			{
				RumFactory result;
				lock (RumFactory.threadSafetyLock)
				{
					if (RumFactory.instance == null)
					{
						RumFactory.instance = new RumFactory();
					}
					result = RumFactory.instance;
				}
				return result;
			}
		}

		internal CalendarRepairPolicy Policy { get; private set; }

		internal void Initialize(CalendarRepairPolicy policy)
		{
			this.Policy = policy;
			this.isInitialized = true;
		}

		internal RumInfo CreateRumInfo(CalendarValidationContext context, Inconsistency inconsistency)
		{
			this.CheckInitialized();
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			if (inconsistency == null)
			{
				throw new ArgumentNullException("inconsistency");
			}
			if (this.ShouldSendRum(context, inconsistency))
			{
				IList<Attendee> list = new List<Attendee>(1);
				if (context.BaseRole == RoleType.Organizer && context.Attendee != null)
				{
					if (context.Attendee.Attendee == null)
					{
						context.Attendee.Attendee = context.OrganizerItem.AttendeeCollection.Add(context.Attendee.Participant, AttendeeType.Required, null, null, false);
					}
					list.Add(context.Attendee.Attendee);
				}
				RumInfo rumInfo = inconsistency.CreateRumInfo(context, list);
				if (!rumInfo.IsNullOp && rumInfo is OrganizerRumInfo && context.AttendeeItem != null)
				{
					((OrganizerRumInfo)rumInfo).AttendeeRequiredSequenceNumber = context.AttendeeItem.AppointmentSequenceNumber;
				}
				return rumInfo;
			}
			return NullOpRumInfo.CreateInstance();
		}

		private bool ShouldSendRum(CalendarValidationContext context, Inconsistency inconsistency)
		{
			if (context.BaseRole == RoleType.Attendee && CalendarItemBase.IsTenantToBeFixed(context.BaseItem.Session as MailboxSession))
			{
				return false;
			}
			if (inconsistency.Owner == context.BaseRole)
			{
				return false;
			}
			if (!this.Policy.ContainsRepairFlag(inconsistency.Flag))
			{
				return false;
			}
			if (context.AttendeeItem != null && context.OrganizerItem != null && context.AttendeeItem.CalendarItemType == CalendarItemType.Occurrence && context.OrganizerItem.CalendarItemType == CalendarItemType.Occurrence)
			{
				return false;
			}
			if (context.RemoteUser.ExchangePrincipal == null)
			{
				return false;
			}
			if (this.CheckServerVersion(context, inconsistency))
			{
				CalendarProcessingFlags? calendarConfig = context.CalendarInstance.GetCalendarConfig();
				return calendarConfig != null && (inconsistency.Flag != CalendarInconsistencyFlag.MissingItem || !context.HasSentUpdateForItemOrMaster) && calendarConfig.Value == CalendarProcessingFlags.AutoUpdate;
			}
			return false;
		}

		private bool CheckServerVersion(CalendarValidationContext context, Inconsistency inconsistency)
		{
			ServerVersion serverVersion = new ServerVersion(context.RemoteUser.ExchangePrincipal.MailboxInfo.Location.ServerVersion);
			if (serverVersion.Major <= 8)
			{
				return false;
			}
			if (inconsistency.Flag == CalendarInconsistencyFlag.OrphanedMeeting)
			{
				return ServerVersion.Compare(serverVersion, this.supportsMeetingInquiryAfter) > 0;
			}
			return ServerVersion.Compare(serverVersion, this.supportsRumsAfter) > 0;
		}

		private void CheckInitialized()
		{
			if (!this.isInitialized)
			{
				throw new ObjectNotInitializedException(base.GetType());
			}
		}

		private static RumFactory instance;

		private static object threadSafetyLock = new object();

		private bool isInitialized;

		private ServerVersion supportsMeetingInquiryAfter = new ServerVersion(14, 1, 0, 0);

		private ServerVersion supportsRumsAfter = new ServerVersion(14, 0, 0, 0);
	}
}
