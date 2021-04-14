using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.CalendarDiagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class CalendarLocalParticipant : CalendarParticipant
	{
		internal CalendarLocalParticipant(UserObject userObject, ExDateTime validateFrom, ExDateTime validateUntil, SessionManager sessionManager) : base(userObject, validateFrom, validateUntil)
		{
			if (userObject.ExchangePrincipal == null)
			{
				throw new ArgumentNullException("userObject.ExchangePrincipal");
			}
			this.sessionManager = sessionManager;
			this.ExchangePrincipal = userObject.ExchangePrincipal;
		}

		internal ExchangePrincipal ExchangePrincipal { get; private set; }

		private bool IsOrganizerValid(MailboxSession organizerSession, CalendarInstanceContext instanceContext)
		{
			bool isOrganizer = false;
			CalendarVersionStoreGateway cvsGateway = instanceContext.ValidationContext.CvsGateway;
			cvsGateway.QueryByGlobalObjectId(organizerSession, instanceContext.ValidationContext.BaseItem.GlobalObjectId, string.Empty, new StorePropertyDefinition[]
			{
				StoreObjectSchema.ItemClass,
				CalendarItemBaseSchema.AppointmentState
			}, delegate(PropertyBag propertyBag)
			{
				string valueOrDefault = propertyBag.GetValueOrDefault<string>(StoreObjectSchema.ItemClass);
				if (ObjectClass.IsCalendarItemCalendarItemOccurrenceOrRecurrenceException(valueOrDefault))
				{
					AppointmentStateFlags valueOrDefault2 = propertyBag.GetValueOrDefault<AppointmentStateFlags>(CalendarItemBaseSchema.AppointmentState);
					isOrganizer = ((valueOrDefault2 & AppointmentStateFlags.Received) == AppointmentStateFlags.None);
					if (!isOrganizer)
					{
						return false;
					}
				}
				return true;
			}, false, null, null, null);
			return isOrganizer;
		}

		internal override void ValidateMeetings(ref Dictionary<GlobalObjectId, List<Attendee>> organizerRumsSent, Action<long> onItemRepaired)
		{
			bool shouldProcessMailbox = CalendarParticipant.InternalShouldProcessMailbox(this.ExchangePrincipal);
			SessionManager.SessionData sessionData = this.sessionManager[this.ExchangePrincipal];
			foreach (CalendarInstanceContext calendarInstanceContext in base.ItemList.Values)
			{
				string empty = string.Empty;
				CalendarLocalItem calendarLocalItem = new CalendarLocalItem(this.ExchangePrincipal, sessionData.Session);
				calendarInstanceContext.ValidationContext.CalendarInstance = calendarLocalItem;
				calendarInstanceContext.ValidationContext.CalendarInstance.ShouldProcessMailbox = shouldProcessMailbox;
				try
				{
					using (CalendarFolder calendarFolder = CalendarFolder.Bind(sessionData.Session, DefaultFolderType.Calendar, null))
					{
						calendarLocalItem.CalendarFolderId = calendarFolder.Id.ObjectId.ProviderLevelItemId;
						CalendarItemBase calendarItemBase = CalendarQuery.FindMatchingItem(sessionData.Session, calendarFolder, calendarInstanceContext.ValidationContext.BaseItem.CalendarItemType, calendarInstanceContext.ValidationContext.BaseItem.GlobalObjectId.Bytes, ref empty);
						if (calendarInstanceContext.ValidationContext.OppositeRole == RoleType.Organizer)
						{
							if (calendarItemBase != null)
							{
								calendarInstanceContext.ValidationContext.OppositeRoleOrganizerIsValid = calendarItemBase.IsOrganizer();
							}
							else
							{
								calendarInstanceContext.ValidationContext.OppositeRoleOrganizerIsValid = this.IsOrganizerValid(sessionData.Session, calendarInstanceContext);
							}
							calendarInstanceContext.ValidationContext.OppositeItem = (calendarInstanceContext.ValidationContext.OppositeRoleOrganizerIsValid ? calendarItemBase : null);
						}
						else
						{
							calendarInstanceContext.ValidationContext.OppositeItem = calendarItemBase;
						}
						calendarInstanceContext.ValidationContext.ErrorString = empty;
					}
				}
				catch (StorageTransientException ex)
				{
					string text = string.Format("Could not open item store session or calendar, exception = {0}", ex.GetType());
					Globals.ConsistencyChecksTracer.TraceError((long)this.GetHashCode(), text);
					calendarInstanceContext.ValidationContext.CalendarInstance.LoadInconsistency = Inconsistency.CreateInstance(calendarInstanceContext.ValidationContext.OppositeRole, text, CalendarInconsistencyFlag.StorageException, calendarInstanceContext.ValidationContext);
				}
				catch (RecurrenceFormatException ex2)
				{
					string text2 = string.Format("Could not open item store session or calendar due recurrence format error, exception = {0}", ex2.GetType());
					Globals.ConsistencyChecksTracer.TraceError((long)this.GetHashCode(), text2);
					calendarInstanceContext.ValidationContext.CalendarInstance.LoadInconsistency = ((calendarInstanceContext.ValidationContext.OppositeRole == RoleType.Attendee) ? Inconsistency.CreateInstance(calendarInstanceContext.ValidationContext.OppositeRole, text2, CalendarInconsistencyFlag.RecurrenceBlob, calendarInstanceContext.ValidationContext) : null);
				}
				catch (StoragePermanentException ex3)
				{
					string text3 = string.Format("Could not open item store session or calendar, exception = {0}", ex3.GetType());
					Globals.ConsistencyChecksTracer.TraceError((long)this.GetHashCode(), text3);
					calendarInstanceContext.ValidationContext.CalendarInstance.LoadInconsistency = Inconsistency.CreateInstance(calendarInstanceContext.ValidationContext.OppositeRole, text3, CalendarInconsistencyFlag.StorageException, calendarInstanceContext.ValidationContext);
				}
				catch (ArgumentException arg)
				{
					Globals.ConsistencyChecksTracer.TraceError<RoleType, ArgumentException>((long)this.GetHashCode(), "Could not open item store session or calendar for {0}, exception = {1}", calendarInstanceContext.ValidationContext.OppositeRole, arg);
				}
				finally
				{
					base.ValidateInstance(calendarInstanceContext, organizerRumsSent, onItemRepaired);
					if (calendarInstanceContext.ValidationContext.OppositeItem != null)
					{
						calendarInstanceContext.ValidationContext.OppositeItem.Dispose();
						calendarInstanceContext.ValidationContext.OppositeItem = null;
					}
				}
			}
		}

		private SessionManager sessionManager;
	}
}
