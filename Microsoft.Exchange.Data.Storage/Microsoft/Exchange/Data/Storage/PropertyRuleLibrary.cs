using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class PropertyRuleLibrary
	{
		private static bool GetCleanGoidFromGoid(byte[] globalObjectId, out byte[] cleanGlobalObjectId)
		{
			if (!GlobalObjectId.HasInstanceDate(globalObjectId))
			{
				cleanGlobalObjectId = null;
				return false;
			}
			cleanGlobalObjectId = (byte[])globalObjectId.Clone();
			GlobalObjectId.MakeGlobalObjectIdBytesToClean(cleanGlobalObjectId);
			return true;
		}

		private static bool GetIsExceptionFromItemClass(string itemClass, out bool isException)
		{
			isException = false;
			if (string.IsNullOrEmpty(itemClass))
			{
				return false;
			}
			isException = ObjectClass.IsRecurrenceException(itemClass);
			return true;
		}

		private static bool GetCleanGoidFromGoidIfRecurringMaster(bool appointmentRecurring, byte[] globalObjectId, out byte[] newGlobalObjectId)
		{
			if (!appointmentRecurring)
			{
				newGlobalObjectId = null;
				return false;
			}
			return PropertyRuleLibrary.GetCleanGoidFromGoid(globalObjectId, out newGlobalObjectId);
		}

		private static Func<ICorePropertyBag, bool> GetDeleteEmptyRecurrenceBlobFunc(PropertyDefinition recurrenceBlob)
		{
			return delegate(ICorePropertyBag propertyBag)
			{
				if (propertyBag.IsPropertyDirty(recurrenceBlob))
				{
					byte[] array = propertyBag.TryGetProperty(recurrenceBlob) as byte[];
					if (array != null && array.Length == 0)
					{
						propertyBag.Delete(recurrenceBlob);
						return true;
					}
				}
				return false;
			};
		}

		internal static bool StampDoItTime(ICorePropertyBag propertyBag)
		{
			ExDateTime? exDateTime = null;
			bool valueOrDefault = propertyBag.GetValueOrDefault<bool>(InternalSchema.ReminderIsSetInternal);
			if (valueOrDefault)
			{
				exDateTime = new ExDateTime?(propertyBag.GetValueOrDefault<ExDateTime>(InternalSchema.ReminderNextTime));
			}
			else
			{
				bool flag = ((IDirectPropertyBag)propertyBag).IsLoaded(InternalSchema.ModernRemindersState);
				ExDateTime valueOrDefault2 = propertyBag.GetValueOrDefault<ExDateTime>(InternalSchema.DueDate);
				if (flag)
				{
					RemindersState<ModernReminderState> remindersState = RemindersState<ModernReminderState>.Get(propertyBag, InternalSchema.ModernRemindersState);
					if (remindersState.StateList.Count >= 1)
					{
						ExDateTime? exDateTime2 = new ExDateTime?(remindersState.StateList[0].ScheduledReminderTime);
						if (exDateTime2 > ExDateTime.MinValue)
						{
							exDateTime = new ExDateTime?(exDateTime2.Value);
						}
					}
				}
				if (exDateTime == null)
				{
					if (valueOrDefault2 > ExDateTime.MinValue)
					{
						exDateTime = new ExDateTime?(valueOrDefault2);
					}
					else
					{
						exDateTime = new ExDateTime?(propertyBag.GetValueOrDefault<ExDateTime>(InternalSchema.CreationTime));
					}
				}
			}
			propertyBag.SetProperty(InternalSchema.DoItTime, exDateTime);
			return true;
		}

		private static bool SetExceptionalBodyWhenBodyChanged(ICorePropertyBag propertyBag)
		{
			bool result = false;
			foreach (StorePropertyDefinition propertyDefinition in Body.BodyProps)
			{
				if (propertyBag.IsPropertyDirty(propertyDefinition))
				{
					propertyBag.SetProperty(InternalSchema.ExceptionalBody, true);
					result = true;
				}
			}
			return result;
		}

		private static PropertyReference[] GetExceptionalBodyPropertyReferences()
		{
			PropertyReference[] array = new PropertyReference[Body.BodyProps.Length + 1];
			for (int i = 0; i < Body.BodyProps.Length; i++)
			{
				array[i] = new PropertyReference((NativeStorePropertyDefinition)Body.BodyProps[i], PropertyAccess.Read);
			}
			array[Body.BodyProps.Length] = new PropertyReference(InternalSchema.ExceptionalBody, PropertyAccess.Write);
			return array;
		}

		private static bool SyncResponseAndReplyRequested(ICorePropertyBag propertyBag)
		{
			bool result = false;
			bool? valueAsNullable = propertyBag.GetValueAsNullable<bool>(InternalSchema.IsResponseRequested);
			bool? valueAsNullable2 = propertyBag.GetValueAsNullable<bool>(InternalSchema.IsReplyRequested);
			if (valueAsNullable == null)
			{
				if (valueAsNullable2 == null)
				{
					string valueOrDefault = propertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass);
					if (!string.IsNullOrEmpty(valueOrDefault) && ObjectClass.IsRecurrenceException(valueOrDefault))
					{
						return false;
					}
				}
				valueAsNullable = new bool?(false);
				if (!propertyBag.GetValueOrDefault<bool>(InternalSchema.DisallowNewTimeProposal))
				{
					valueAsNullable = new bool?(true);
				}
				else if (valueAsNullable2 != null)
				{
					valueAsNullable = new bool?(valueAsNullable2.Value);
				}
				propertyBag.SetProperty(InternalSchema.IsResponseRequested, valueAsNullable);
				result = true;
			}
			if (valueAsNullable2 != valueAsNullable)
			{
				propertyBag.SetProperty(InternalSchema.IsReplyRequested, valueAsNullable);
				result = true;
			}
			return result;
		}

		private static bool SetCalendarOriginatorId(ICorePropertyBag propertyBag)
		{
			MailboxSession mailboxSession = ((IDirectPropertyBag)propertyBag).Context.Session as MailboxSession;
			if (mailboxSession == null)
			{
				return false;
			}
			string valueOrDefault = propertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass);
			if (string.IsNullOrEmpty(valueOrDefault))
			{
				return false;
			}
			if (!ObjectClass.IsCalendarItemCalendarItemOccurrenceOrRecurrenceException(valueOrDefault))
			{
				return false;
			}
			bool flag = false;
			string valueOrDefault2 = propertyBag.GetValueOrDefault<string>(InternalSchema.CalendarOriginatorId);
			if (!string.IsNullOrEmpty(valueOrDefault2))
			{
				string[] array;
				if (CalendarOriginatorIdProperty.TryParse(valueOrDefault2, out array) && !string.IsNullOrEmpty(array[0]) && !string.IsNullOrEmpty(array[1]))
				{
					return false;
				}
				propertyBag.Delete(InternalSchema.CalendarOriginatorId);
				flag = true;
			}
			PersistablePropertyBag persistablePropertyBag = PersistablePropertyBag.GetPersistablePropertyBag(propertyBag);
			if (!persistablePropertyBag.IsNew && !flag)
			{
				return false;
			}
			if (ObjectClass.IsRecurrenceException(valueOrDefault) || ObjectClass.IsCalendarItemOccurrence(valueOrDefault))
			{
				return flag;
			}
			object obj = propertyBag.TryGetProperty(InternalSchema.AppointmentStateInternal);
			if (PropertyError.IsPropertyNotFound(obj))
			{
				return false;
			}
			if (((AppointmentStateFlags)obj & AppointmentStateFlags.Received) == AppointmentStateFlags.Received)
			{
				return false;
			}
			string value;
			if (CalendarOriginatorIdProperty.TryCreate(mailboxSession, out value))
			{
				propertyBag.SetProperty(InternalSchema.CalendarOriginatorId, value);
				return true;
			}
			return true;
		}

		private static bool SetHasAttendees(ICorePropertyBag propertyBag)
		{
			PersistablePropertyBag persistablePropertyBag = PersistablePropertyBag.GetPersistablePropertyBag(propertyBag);
			ICoreItem coreItem = persistablePropertyBag.Context.CoreObject as ICoreItem;
			if (coreItem == null)
			{
				return false;
			}
			PropertyDefinition hasAttendees = InternalSchema.HasAttendees;
			bool flag;
			if (persistablePropertyBag.IsNew)
			{
				flag = true;
				propertyBag.SetLocationIdentifier(53708U);
			}
			else if (propertyBag.IsPropertyDirty(hasAttendees))
			{
				flag = true;
				propertyBag.SetLocationIdentifier(41420U);
			}
			else if (coreItem.Recipients != null && coreItem.Recipients.IsDirty)
			{
				flag = true;
				propertyBag.SetLocationIdentifier(57804U);
			}
			else
			{
				object obj = propertyBag.TryGetProperty(hasAttendees);
				if (obj is bool)
				{
					flag = false;
				}
				else
				{
					flag = true;
					propertyBag.SetLocationIdentifier(33228U);
				}
			}
			if (flag)
			{
				bool flag2 = coreItem.Recipients != null && !coreItem.Recipients.All(new Func<CoreRecipient, bool>(PropertyRuleLibrary.IsRecipientOrganizer));
				propertyBag.SetProperty(hasAttendees, flag2);
				return true;
			}
			return false;
		}

		private static bool IsRecipientOrganizer(CoreRecipient recipient)
		{
			Attendee attendee = new Attendee(recipient);
			return attendee.IsOrganizer;
		}

		private static bool GetAppointmentStateFromItemClass(ICorePropertyBag propertyBag)
		{
			object obj = propertyBag.TryGetProperty(InternalSchema.AppointmentStateInternal);
			if (!PropertyError.IsPropertyNotFound(obj) && obj is int)
			{
				return false;
			}
			string valueOrDefault = propertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass);
			if (string.IsNullOrEmpty(valueOrDefault))
			{
				return false;
			}
			if (ObjectClass.IsRecurrenceException(valueOrDefault))
			{
				return false;
			}
			AppointmentStateFlags appointmentStateFlags = AppointmentStateFlags.Meeting;
			if (ObjectClass.IsMeetingCancellation(valueOrDefault))
			{
				appointmentStateFlags = (AppointmentStateFlags.Meeting | AppointmentStateFlags.Received | AppointmentStateFlags.Cancelled);
			}
			else if (ObjectClass.IsMeetingMessage(valueOrDefault))
			{
				appointmentStateFlags = (AppointmentStateFlags.Meeting | AppointmentStateFlags.Received);
			}
			else if (ObjectClass.IsCalendarItem(valueOrDefault))
			{
				bool valueOrDefault2 = propertyBag.GetValueOrDefault<bool>(InternalSchema.MeetingRequestWasSent);
				if (valueOrDefault2)
				{
					appointmentStateFlags = AppointmentStateFlags.Meeting;
				}
				else
				{
					PersistablePropertyBag persistablePropertyBag = PersistablePropertyBag.GetPersistablePropertyBag(propertyBag);
					ICoreItem coreItem = persistablePropertyBag.Context.CoreObject as ICoreItem;
					if (coreItem == null)
					{
						return false;
					}
					appointmentStateFlags = ((coreItem.Recipients.Count != 0) ? AppointmentStateFlags.Meeting : AppointmentStateFlags.None);
				}
			}
			propertyBag.SetProperty(InternalSchema.AppointmentStateInternal, appointmentStateFlags);
			return true;
		}

		private static bool SetDefaultOrganizerForAppointments(ICorePropertyBag propertyBag)
		{
			string valueOrDefault = propertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass);
			if (string.IsNullOrEmpty(valueOrDefault))
			{
				return false;
			}
			if (ObjectClass.IsRecurrenceException(valueOrDefault))
			{
				return false;
			}
			int valueOrDefault2 = propertyBag.GetValueOrDefault<int>(InternalSchema.AppointmentStateInternal, -1);
			if (valueOrDefault2 == -1)
			{
				return false;
			}
			if ((valueOrDefault2 & 1) != 0)
			{
				return false;
			}
			string valueOrDefault3 = propertyBag.GetValueOrDefault<string>(InternalSchema.SentRepresentingEmailAddress);
			if (!string.IsNullOrEmpty(valueOrDefault3))
			{
				return false;
			}
			byte[] valueOrDefault4 = propertyBag.GetValueOrDefault<byte[]>(InternalSchema.SentRepresentingEntryId, null);
			if (valueOrDefault4 != null && valueOrDefault4.Length != 0)
			{
				return false;
			}
			MailboxSession mailboxSession = ((IDirectPropertyBag)propertyBag).Context.Session as MailboxSession;
			if (mailboxSession == null)
			{
				return false;
			}
			if (mailboxSession.MailboxOwner != null)
			{
				Participant value = new Participant(mailboxSession.MailboxOwner);
				propertyBag.SetProperty(InternalSchema.From, value);
			}
			return true;
		}

		private static bool FixRecurringTimeZone(ICorePropertyBag propertyBag)
		{
			bool flag = false;
			if (PropertyError.IsPropertyNotFound(propertyBag.TryGetProperty(InternalSchema.TimeZone)))
			{
				ExTimeZone exTimeZone = PersistablePropertyBag.GetPersistablePropertyBag(propertyBag).ExTimeZone;
				if (exTimeZone != null)
				{
					propertyBag.SetProperty(InternalSchema.TimeZone, exTimeZone.LocalizableDisplayName.ToString());
					flag = true;
				}
			}
			bool valueOrDefault = propertyBag.GetValueOrDefault<bool>(InternalSchema.AppointmentRecurring);
			if (valueOrDefault)
			{
				ExTimeZone exTimeZone2 = null;
				flag = PropertyRuleLibrary.UpdateRecurringTimeZoneBlobForLegacy(propertyBag, out exTimeZone2);
				if (!flag)
				{
					if (exTimeZone2 == null)
					{
						byte[] valueOrDefault2 = propertyBag.GetValueOrDefault<byte[]>(InternalSchema.TimeZoneDefinitionRecurring);
						O12TimeZoneFormatter.TryParseTimeZoneBlob(valueOrDefault2, string.Empty, out exTimeZone2);
					}
					if (exTimeZone2 != null && exTimeZone2.AlternativeId == "tzone://Microsoft/Custom")
					{
						ExTimeZone exTimeZone3 = TimeZoneHelper.PromoteCustomizedTimeZone(exTimeZone2);
						if (exTimeZone3 != null)
						{
							PropertyRuleLibrary.SetRecurringTimeZoneProperties(propertyBag, exTimeZone3);
							flag = true;
						}
					}
				}
			}
			return flag;
		}

		private static bool UpdateRecurringTimeZoneBlobForLegacy(ICorePropertyBag propertyBag, out ExTimeZone timeZoneFromO12Blob)
		{
			bool result = false;
			timeZoneFromO12Blob = null;
			if (propertyBag.IsPropertyDirty(InternalSchema.TimeZoneBlob))
			{
				byte[] valueOrDefault = propertyBag.GetValueOrDefault<byte[]>(InternalSchema.TimeZoneBlob);
				byte[] valueOrDefault2 = propertyBag.GetValueOrDefault<byte[]>(InternalSchema.TimeZoneDefinitionRecurring);
				ExTimeZone timeZone;
				if (O11TimeZoneFormatter.TryParseTimeZoneBlob(valueOrDefault, string.Empty, out timeZone) && O12TimeZoneFormatter.TryParseTimeZoneBlob(valueOrDefault2, string.Empty, out timeZoneFromO12Blob))
				{
					REG_TIMEZONE_INFO v = TimeZoneHelper.RegTimeZoneInfoFromExTimeZone(timeZone);
					REG_TIMEZONE_INFO v2 = TimeZoneHelper.RegTimeZoneInfoFromExTimeZone(timeZoneFromO12Blob);
					if (v != v2)
					{
						propertyBag.Delete(InternalSchema.TimeZoneDefinitionRecurring);
						propertyBag.Delete(InternalSchema.TimeZoneDefinitionStart);
						result = true;
					}
				}
			}
			return result;
		}

		private static bool SyncLocationAndLidWhere(ICorePropertyBag propertyBag)
		{
			bool result = false;
			if (propertyBag.IsPropertyDirty(InternalSchema.Location))
			{
				propertyBag.SetOrDeleteProperty(InternalSchema.LidWhere, propertyBag.TryGetProperty(InternalSchema.Location));
				result = true;
			}
			else if (propertyBag.IsPropertyDirty(InternalSchema.LidWhere))
			{
				string valueOrDefault = propertyBag.GetValueOrDefault<string>(InternalSchema.LidWhere, null);
				if (valueOrDefault != string.Empty)
				{
					propertyBag.SetOrDeleteProperty(InternalSchema.Location, valueOrDefault);
					result = true;
				}
			}
			return result;
		}

		private static bool StampDefaultClientIntent(ICorePropertyBag propertyBag)
		{
			bool result = false;
			if (propertyBag.GetValueAsNullable<ClientIntentFlags>(InternalSchema.ClientIntent) == null || !propertyBag.IsPropertyDirty(InternalSchema.ClientIntent))
			{
				propertyBag.SetProperty(InternalSchema.ClientIntent, ClientIntentFlags.None);
				result = true;
			}
			return result;
		}

		internal static bool InternalTruncateSubject(ICorePropertyBag propertyBag)
		{
			bool flag = false;
			if (propertyBag.IsPropertyDirty(InternalSchema.MapiSubject) || propertyBag.IsPropertyDirty(InternalSchema.SubjectPrefixInternal) || propertyBag.IsPropertyDirty(InternalSchema.NormalizedSubjectInternal))
			{
				flag |= SubjectProperty.TruncateSubject(PersistablePropertyBag.GetPersistablePropertyBag(propertyBag), 255);
			}
			return flag;
		}

		private static bool UpdateDurationFromNativeStartEndTime(ICorePropertyBag propertyBag)
		{
			bool result = false;
			if (propertyBag.IsPropertyDirty(InternalSchema.MapiStartTime) || propertyBag.IsPropertyDirty(InternalSchema.MapiEndTime) || propertyBag.IsPropertyDirty(InternalSchema.Duration))
			{
				ExDateTime valueOrDefault = propertyBag.GetValueOrDefault<ExDateTime>(InternalSchema.MapiStartTime);
				ExDateTime valueOrDefault2 = propertyBag.GetValueOrDefault<ExDateTime>(InternalSchema.MapiEndTime);
				TimeSpan timeSpan = valueOrDefault2 - valueOrDefault;
				propertyBag.SetProperty(InternalSchema.Duration, (int)timeSpan.TotalMinutes);
				result = true;
			}
			return result;
		}

		private static bool GetReminderNextTimeFromNativeStartTimeAndReminderDelta(ICorePropertyBag propertyBag)
		{
			bool result = false;
			if (PropertyError.IsPropertyNotFound(propertyBag.TryGetProperty(InternalSchema.ReminderNextTime)))
			{
				ExDateTime? valueAsNullable = propertyBag.GetValueAsNullable<ExDateTime>(InternalSchema.MapiStartTime);
				if (valueAsNullable != null)
				{
					int num = propertyBag.GetValueOrDefault<int>(InternalSchema.ReminderMinutesBeforeStartInternal, 15);
					if (num < 0)
					{
						num = 15;
					}
					TimeSpan t = TimeSpan.FromMinutes((double)num);
					if (valueAsNullable.Value >= ExDateTime.MinValue + t)
					{
						propertyBag.SetProperty(InternalSchema.ReminderNextTime, valueAsNullable.Value - t);
						result = true;
					}
				}
			}
			return result;
		}

		private static bool UpdateRecurringFlags(ICorePropertyBag propertyBag)
		{
			bool flag = InternalRecurrence.HasRecurrenceBlob(propertyBag);
			propertyBag.SetProperty(InternalSchema.AppointmentRecurring, flag);
			bool valueOrDefault = propertyBag.GetValueOrDefault<bool>(InternalSchema.IsException);
			string valueOrDefault2 = propertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass);
			if (string.IsNullOrEmpty(valueOrDefault2))
			{
				return false;
			}
			bool flag2 = false;
			if (ObjectClass.IsCalendarItem(valueOrDefault2))
			{
				flag2 = propertyBag.GetValueOrDefault<bool>(InternalSchema.IsOrganizer);
			}
			bool flag3 = valueOrDefault || (flag && !flag2);
			propertyBag.SetProperty(InternalSchema.IsRecurring, flag3);
			return true;
		}

		private static bool StampClipStartTimeForSingleMeeting(ICorePropertyBag propertyBag)
		{
			return PropertyRuleLibrary.StampClipTimeForSingleMeeting(propertyBag, InternalSchema.ClipStartTime, InternalSchema.MapiStartTime);
		}

		private static bool StampClipEndTimeForSingleMeeting(ICorePropertyBag propertyBag)
		{
			return PropertyRuleLibrary.StampClipTimeForSingleMeeting(propertyBag, InternalSchema.ClipEndTime, InternalSchema.MapiEndTime);
		}

		private static bool StampClipTimeForSingleMeeting(ICorePropertyBag propertyBag, StorePropertyDefinition clipProperty, StorePropertyDefinition copyFromProperty)
		{
			bool result = false;
			if (propertyBag.GetValueAsNullable<ExDateTime>(clipProperty) == null && !InternalRecurrence.HasRecurrenceBlob(propertyBag))
			{
				propertyBag.SetProperty(clipProperty, propertyBag.GetValueOrDefault<ExDateTime>(copyFromProperty));
				result = true;
			}
			return result;
		}

		private static bool StampCalendarViewTime(ICorePropertyBag propertyBag)
		{
			if (!(((IDirectPropertyBag)propertyBag).Context.Session is MailboxSession))
			{
				return false;
			}
			object propertyValue = propertyBag.TryGetProperty(InternalSchema.ViewStartTime);
			object propertyValue2 = propertyBag.TryGetProperty(InternalSchema.ViewEndTime);
			bool flag = PropertyError.IsPropertyNotFound(propertyValue) || PropertyError.IsPropertyNotFound(propertyValue2);
			object obj = propertyBag.TryGetProperty(InternalSchema.AppointmentRecurrenceBlob);
			if (PropertyError.IsPropertyNotFound(obj))
			{
				return PropertyRuleLibrary.StampCalendarViewTimeOnSingleMeetings(propertyBag, flag);
			}
			if (!propertyBag.IsPropertyDirty(InternalSchema.AppointmentRecurrenceBlob) && !flag)
			{
				return false;
			}
			byte[] array = obj as byte[];
			return array != null && PropertyRuleLibrary.StampCalendarViewTimeOnRecurringItems(propertyBag, array);
		}

		private static bool StampCalendarViewTimeOnSingleMeetings(ICorePropertyBag propertyBag, bool setViewProperties)
		{
			bool result = false;
			if (propertyBag.IsPropertyDirty(InternalSchema.MapiStartTime) || setViewProperties)
			{
				propertyBag.SetProperty(InternalSchema.ViewStartTime, propertyBag.GetValueOrDefault<ExDateTime>(InternalSchema.MapiStartTime));
				result = true;
			}
			if (propertyBag.IsPropertyDirty(InternalSchema.MapiEndTime) || setViewProperties)
			{
				object obj = propertyBag.TryGetProperty(InternalSchema.MapiEndTime);
				ExDateTime exDateTime;
				if (obj == null || PropertyError.IsPropertyError(obj))
				{
					exDateTime = Recurrence.MaximumDateForRecurrenceEnd;
				}
				else
				{
					exDateTime = (ExDateTime)obj;
				}
				propertyBag.SetProperty(InternalSchema.ViewEndTime, exDateTime);
				result = true;
			}
			return result;
		}

		private static bool StampCalendarViewTimeOnRecurringItems(ICorePropertyBag propertyBag, byte[] recurrenceBlob)
		{
			ExDateTime exDateTime = ExDateTime.MinValue;
			ExDateTime exDateTime2 = Recurrence.MaximumDateForRecurrenceEnd;
			MailboxSession session = ((IDirectPropertyBag)propertyBag).Context.Session as MailboxSession;
			try
			{
				string valueOrDefault = propertyBag.GetValueOrDefault<string>(InternalSchema.TimeZone, null);
				byte[] valueOrDefault2 = propertyBag.GetValueOrDefault<byte[]>(InternalSchema.TimeZoneBlob, null);
				byte[] valueOrDefault3 = propertyBag.GetValueOrDefault<byte[]>(InternalSchema.TimeZoneDefinitionRecurring, null);
				ExTimeZone timeZoneFromProperties = TimeZoneHelper.GetTimeZoneFromProperties(valueOrDefault, valueOrDefault2, valueOrDefault3);
				int valueOrDefault4 = propertyBag.GetValueOrDefault<int>(InternalSchema.Codepage, CalendarItem.DefaultCodePage);
				VersionedId versionedId = propertyBag.TryGetProperty(InternalSchema.ItemId) as VersionedId;
				InternalRecurrence recurrence = InternalRecurrence.GetRecurrence(versionedId, session, recurrenceBlob, timeZoneFromProperties, valueOrDefault4);
				OccurrenceInfo firstOccurrence = recurrence.GetFirstOccurrence();
				exDateTime = firstOccurrence.StartTime.ToUtc();
				if (recurrence.Range is NoEndRecurrenceRange)
				{
					exDateTime2 = Recurrence.MaximumDateForRecurrenceEnd;
				}
				else
				{
					OccurrenceInfo lastOccurrence = recurrence.GetLastOccurrence();
					exDateTime2 = lastOccurrence.EndTime.ToUtc();
				}
			}
			catch (RecurrenceException arg)
			{
				ExTraceGlobals.StorageTracer.TraceError<RecurrenceException>(0L, "PropertyRuleLibrary::StampCalendarViewTime. Error parsing the recurrence blob. {0}.", arg);
			}
			catch (OccurrenceNotFoundException arg2)
			{
				ExTraceGlobals.StorageTracer.TraceError<OccurrenceNotFoundException>(0L, "PropertyRuleLibrary::StampCalendarViewTime. Occurrence not found. {0}.", arg2);
			}
			catch (CorruptDataException arg3)
			{
				ExTraceGlobals.StorageTracer.TraceError<CorruptDataException>(0L, "PropertyRuleLibrary::StampCalendarViewTime. Recurrence blob is corrupt. {0}.", arg3);
			}
			propertyBag.SetProperty(InternalSchema.ViewStartTime, exDateTime);
			propertyBag.SetProperty(InternalSchema.ViewEndTime, exDateTime2);
			return true;
		}

		private static bool GenerateRecurrenceBlobFromSchedulePlusProperties(ICorePropertyBag propertyBag)
		{
			if (propertyBag.IsPropertyDirty(InternalSchema.AppointmentRecurrenceBlob) || !PropertyError.IsPropertyNotFound(propertyBag.TryGetProperty(InternalSchema.AppointmentRecurrenceBlob)))
			{
				return false;
			}
			if (propertyBag.GetValueOrDefault<bool>(InternalSchema.LidSingleInvite) || propertyBag.GetValueOrDefault<bool>(InternalSchema.IsException))
			{
				return false;
			}
			int? valueAsNullable = propertyBag.GetValueAsNullable<int>(InternalSchema.LidTimeZone);
			if (valueAsNullable == null)
			{
				return false;
			}
			int num = valueAsNullable.Value & 65535;
			if (num <= 0 || num >= PropertyRuleLibrary.SchedulePlusTimeZoneId.Length)
			{
				return false;
			}
			ExTimeZone exTimeZone = null;
			if (!ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(PropertyRuleLibrary.SchedulePlusTimeZoneId[num], out exTimeZone))
			{
				return false;
			}
			int? valueAsNullable2 = propertyBag.GetValueAsNullable<int>(InternalSchema.StartRecurDate);
			int? valueAsNullable3 = propertyBag.GetValueAsNullable<int>(InternalSchema.StartRecurTime);
			int? valueAsNullable4 = propertyBag.GetValueAsNullable<int>(InternalSchema.EndRecurDate);
			int? valueAsNullable5 = propertyBag.GetValueAsNullable<int>(InternalSchema.EndRecurTime);
			if (valueAsNullable2 == null || valueAsNullable2.Value == 0 || valueAsNullable3 == null || valueAsNullable5 == null)
			{
				return false;
			}
			TimeSpan startOffset = Util.ConvertSCDTimeToTimeSpan(valueAsNullable3.Value);
			TimeSpan endOffset = Util.ConvertSCDTimeToTimeSpan(valueAsNullable5.Value);
			ExDateTime exDateTime;
			ExDateTime? exDateTime2;
			try
			{
				exDateTime = exTimeZone.ConvertDateTime(Util.ConvertSCDDateToDateTime(valueAsNullable2.Value));
				exDateTime2 = ((valueAsNullable4 != null && valueAsNullable4.Value != 0) ? new ExDateTime?(exTimeZone.ConvertDateTime(Util.ConvertSCDDateToDateTime(valueAsNullable4.Value))) : null);
			}
			catch (ArgumentOutOfRangeException)
			{
				return false;
			}
			RecurrenceRange range;
			if (exDateTime2 != null && exDateTime2.Value > exDateTime)
			{
				range = new EndDateRecurrenceRange(exDateTime, exDateTime2.Value);
			}
			else
			{
				range = new NoEndRecurrenceRange(exDateTime);
			}
			short? valueAsNullable6 = propertyBag.GetValueAsNullable<short>(InternalSchema.LidRecurType);
			int? recurrenceType = (valueAsNullable6 != null) ? new int?((int)valueAsNullable6.GetValueOrDefault()) : null;
			short? valueAsNullable7 = propertyBag.GetValueAsNullable<short>(InternalSchema.LidDayInterval);
			int? dayInterval = (valueAsNullable7 != null) ? new int?((int)valueAsNullable7.GetValueOrDefault()) : null;
			short? valueAsNullable8 = propertyBag.GetValueAsNullable<short>(InternalSchema.LidWeekInterval);
			int? weekInterval = (valueAsNullable8 != null) ? new int?((int)valueAsNullable8.GetValueOrDefault()) : null;
			short? valueAsNullable9 = propertyBag.GetValueAsNullable<short>(InternalSchema.LidMonthInterval);
			int? monthInterval = (valueAsNullable9 != null) ? new int?((int)valueAsNullable9.GetValueOrDefault()) : null;
			short? valueAsNullable10 = propertyBag.GetValueAsNullable<short>(InternalSchema.LidYearInterval);
			int? yearInterval = (valueAsNullable10 != null) ? new int?((int)valueAsNullable10.GetValueOrDefault()) : null;
			int? valueAsNullable11 = propertyBag.GetValueAsNullable<int>(InternalSchema.LidDayOfWeekMask);
			int? valueAsNullable12 = propertyBag.GetValueAsNullable<int>(InternalSchema.LidDayOfMonthMask);
			int? valueAsNullable13 = propertyBag.GetValueAsNullable<int>(InternalSchema.LidMonthOfYearMask);
			short? valueAsNullable14 = propertyBag.GetValueAsNullable<short>(InternalSchema.LidFirstDayOfWeek);
			RecurrencePattern recurrencePatternFromSchedulePlusProperties = PropertyRuleLibrary.GetRecurrencePatternFromSchedulePlusProperties(recurrenceType, dayInterval, weekInterval, monthInterval, yearInterval, valueAsNullable11, valueAsNullable12, valueAsNullable13, (valueAsNullable14 != null) ? new int?((int)valueAsNullable14.GetValueOrDefault()) : null);
			if (recurrencePatternFromSchedulePlusProperties == null)
			{
				return false;
			}
			InternalRecurrence internalRecurrence;
			try
			{
				internalRecurrence = new InternalRecurrence(recurrencePatternFromSchedulePlusProperties, range, null, exTimeZone, exTimeZone, startOffset, endOffset);
			}
			catch (RecurrenceException)
			{
				return false;
			}
			propertyBag.SetProperty(InternalSchema.AppointmentRecurrenceBlob, internalRecurrence.ToByteArray());
			PropertyRuleLibrary.SetRecurringTimeZoneProperties(propertyBag, exTimeZone);
			return true;
		}

		private static void SetRecurringTimeZoneProperties(ICorePropertyBag propertyBag, ExTimeZone recurringTimeZone)
		{
			propertyBag.SetProperty(InternalSchema.TimeZone, recurringTimeZone.LocalizableDisplayName.ToString());
			propertyBag.SetProperty(InternalSchema.TimeZoneBlob, O11TimeZoneFormatter.GetTimeZoneBlob(recurringTimeZone));
			byte[] timeZoneBlob = O12TimeZoneFormatter.GetTimeZoneBlob(recurringTimeZone);
			propertyBag.SetProperty(InternalSchema.TimeZoneDefinitionRecurring, timeZoneBlob);
			propertyBag.SetProperty(InternalSchema.TimeZoneDefinitionStart, timeZoneBlob);
		}

		private static bool RemoveAppointmentMadeRecurrentFromSeries(ICorePropertyBag propertyBag)
		{
			bool result = false;
			string valueOrDefault = propertyBag.GetValueOrDefault<string>(InternalSchema.SeriesId);
			if (!string.IsNullOrEmpty(valueOrDefault) && InternalRecurrence.HasRecurrenceBlob(propertyBag))
			{
				ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "PropertyRuleLibrary::RemoveAppointmentMadeRecurrentFromSeries. Series instance was turned into recurrence master - removing it from series '{0}'.", valueOrDefault);
				propertyBag.Delete(InternalSchema.SeriesId);
				propertyBag.Delete(InternalSchema.PropertyChangeMetadataRaw);
				string valueOrDefault2 = propertyBag.GetValueOrDefault<string>(InternalSchema.EventClientId);
				if (!string.IsNullOrEmpty(valueOrDefault2))
				{
					propertyBag.Delete(InternalSchema.EventClientId);
				}
				result = true;
			}
			return result;
		}

		private static bool ProtectMasterPropertyOverridesOnSeriesDataPropagation(ICorePropertyBag propertyBag)
		{
			bool result = false;
			if (PropertyRuleLibrary.IsSeriesMasterDataPropagationOperation(propertyBag))
			{
				PropertyChangeMetadata propertyChangeMetadata = propertyBag.GetValueOrDefault<PropertyChangeMetadata>(CalendarItemInstanceSchema.PropertyChangeMetadata);
				bool flag = PropertyRuleLibrary.ShouldMarkAllPropertiesAsException(propertyBag);
				if (flag)
				{
					if (propertyChangeMetadata == null)
					{
						propertyChangeMetadata = new PropertyChangeMetadata();
					}
					ExTraceGlobals.StorageTracer.TraceDebug(0L, "PropertyRuleLibrary::ProtectMasterPropertyOverridesOnSeriesDataPropagation. All properties are marked as exceptoins.");
					propertyChangeMetadata.MarkAllAsException();
				}
				if (propertyChangeMetadata != null)
				{
					List<AtomicStorePropertyDefinition> list = new List<AtomicStorePropertyDefinition>();
					List<PropertyChangeMetadata.PropertyGroup> list2 = new List<PropertyChangeMetadata.PropertyGroup>();
					foreach (PropertyChangeMetadata.PropertyGroup propertyGroup in propertyChangeMetadata.GetOverriddenGroups())
					{
						if (!propertyGroup.IsBitField)
						{
							using (IEnumerator<NativeStorePropertyDefinition> enumerator2 = propertyGroup.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									NativeStorePropertyDefinition nativeStorePropertyDefinition = enumerator2.Current;
									ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "PropertyRuleLibrary::ProtectMasterPropertyOverridesOnSeriesDataPropagation. Property {0} is marked as master override in metadata.", nativeStorePropertyDefinition.Name);
									if (propertyBag.IsPropertyDirty(nativeStorePropertyDefinition))
									{
										ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "PropertyRuleLibrary::ProtectMasterPropertyOverridesOnSeriesDataPropagation. Modification to master property override for {0} will be reverted.", nativeStorePropertyDefinition.Name);
										list.Add(nativeStorePropertyDefinition);
									}
								}
								continue;
							}
						}
						ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "PropertyRuleLibrary::ProtectMasterPropertyOverridesOnSeriesDataPropagation. Bit field property group {0} is marked as master override in metadata.", propertyGroup.Name);
						if (propertyBag.IsPropertyDirty(propertyGroup.ContainerStoreProperty))
						{
							ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "PropertyRuleLibrary::ProtectMasterPropertyOverridesOnSeriesDataPropagation. Modification to master property override for bit field: {0} will be reverted.", propertyGroup.Name);
							list2.Add(propertyGroup);
						}
					}
					if (list.Count > 0)
					{
						PropertyRuleLibrary.UnsetProperties(propertyBag, list);
						result = true;
					}
					if (list2.Count > 0)
					{
						PropertyRuleLibrary.UnsetBitFieldProperties(propertyBag, list2);
						result = true;
					}
				}
				if (flag)
				{
					propertyBag.SetProperty(CalendarItemInstanceSchema.PropertyChangeMetadata, propertyChangeMetadata);
					result = true;
				}
			}
			return result;
		}

		private static bool IsSeriesMasterDataPropagationOperation(ICorePropertyBag propertyBag)
		{
			return PropertyRuleLibrary.IsPropertyChangeMetadataPropagationFlagSet(propertyBag, PropertyChangeMetadataProcessingFlags.SeriesMasterDataPropagationOperation);
		}

		private static bool IsOverridingMetadata(ICorePropertyBag propertyBag)
		{
			return PropertyRuleLibrary.IsPropertyChangeMetadataPropagationFlagSet(propertyBag, PropertyChangeMetadataProcessingFlags.OverrideMetadata);
		}

		private static bool ShouldMarkAllPropertiesAsException(ICorePropertyBag propertyBag)
		{
			return PropertyRuleLibrary.IsPropertyChangeMetadataPropagationFlagSet(propertyBag, PropertyChangeMetadataProcessingFlags.MarkAllPropagatedPropertiesAsException);
		}

		private static bool IsPropertyChangeMetadataPropagationFlagSet(ICorePropertyBag propertyBag, PropertyChangeMetadataProcessingFlags processingFlag)
		{
			if (propertyBag.IsPropertyDirty(InternalSchema.PropertyChangeMetadataProcessingFlags))
			{
				PropertyChangeMetadataProcessingFlags valueOrDefault = propertyBag.GetValueOrDefault<PropertyChangeMetadataProcessingFlags>(InternalSchema.PropertyChangeMetadataProcessingFlags);
				return (valueOrDefault & processingFlag) == processingFlag;
			}
			return false;
		}

		private static bool IsSeriesInstance(ICorePropertyBag propertyBag)
		{
			string valueOrDefault = propertyBag.GetValueOrDefault<string>(InternalSchema.SeriesId);
			return !string.IsNullOrEmpty(valueOrDefault);
		}

		private static bool TrackPropertyChanges(ICorePropertyBag propertyBag)
		{
			bool flag = false;
			PropertyChangeMetadata propertyChangeMetadata;
			if (PropertyRuleLibrary.ShouldTrackPropertyChange(propertyBag, out propertyChangeMetadata))
			{
				if (propertyChangeMetadata == null)
				{
					propertyChangeMetadata = new PropertyChangeMetadata();
				}
				foreach (string text in propertyChangeMetadata.GetTrackedNonOverrideStorePropertyNames())
				{
					StorePropertyDefinition storePropertyDefinition;
					PropertyChangeMetadata.TryGetTrackedPropertyForName(text, out storePropertyDefinition);
					if (storePropertyDefinition is NativeStorePropertyDefinition)
					{
						if (propertyBag.IsPropertyDirty(storePropertyDefinition))
						{
							ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "PropertyRuleLibrary::TrackPropertyChanges. Native property {0} was marked as master override in metadata.", storePropertyDefinition.Name);
							propertyChangeMetadata.MarkAsMasterPropertyOverride(storePropertyDefinition.Name);
							flag = true;
						}
					}
					else
					{
						PropertyChangeMetadata.PropertyGroup groupForPropertyName = PropertyChangeMetadata.GetGroupForPropertyName(text);
						if (groupForPropertyName.IsBitField && PropertyRuleLibrary.IsBitFieldDirty(propertyBag, groupForPropertyName))
						{
							ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "PropertyRuleLibrary::TrackPropertyChanges.Bit field property {0} was marked as master override in metadata.", text);
							propertyChangeMetadata.MarkAsMasterPropertyOverride(text);
							flag = true;
						}
					}
				}
				if (flag)
				{
					propertyBag.SetProperty(CalendarItemInstanceSchema.PropertyChangeMetadata, propertyChangeMetadata);
				}
			}
			return flag;
		}

		private static bool IsBitFieldDirty(ICorePropertyBag propertyBag, PropertyChangeMetadata.PropertyGroup propertyGroup)
		{
			if (propertyBag.IsPropertyDirty(propertyGroup.ContainerStoreProperty))
			{
				IValidatablePropertyBag validatablePropertyBag = propertyBag as IValidatablePropertyBag;
				if (validatablePropertyBag != null)
				{
					object obj = propertyBag.TryGetProperty(propertyGroup.ContainerStoreProperty);
					if (obj is int)
					{
						PropertyValueTrackingData originalPropertyInformation = validatablePropertyBag.GetOriginalPropertyInformation(propertyGroup.ContainerStoreProperty);
						if (originalPropertyInformation != null && originalPropertyInformation.OriginalPropertyValue is int)
						{
							bool flag = ((int)obj & propertyGroup.ContainerFlag) != 0;
							bool flag2 = ((int)originalPropertyInformation.OriginalPropertyValue & propertyGroup.ContainerFlag) != 0;
							return flag2 != flag;
						}
					}
				}
			}
			return false;
		}

		private static bool ShouldTrackPropertyChange(ICorePropertyBag corePropertyBag, out PropertyChangeMetadata existingMetadata)
		{
			IDirectPropertyBag directPropertyBag = corePropertyBag as IDirectPropertyBag;
			if (directPropertyBag != null && corePropertyBag.IsDirty && directPropertyBag.Context.Session != null && !directPropertyBag.Context.Session.OperationContext.IsMoveUser)
			{
				string valueOrDefault = corePropertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass);
				if (ObjectClass.IsRecurrenceException(valueOrDefault))
				{
					existingMetadata = corePropertyBag.GetValueOrDefault<PropertyChangeMetadata>(CalendarItemInstanceSchema.PropertyChangeMetadata);
					return existingMetadata != null || directPropertyBag.IsNew;
				}
				if (PropertyRuleLibrary.IsSeriesInstance(corePropertyBag) && !directPropertyBag.IsNew && !PropertyRuleLibrary.IsSeriesMasterDataPropagationOperation(corePropertyBag) && !PropertyRuleLibrary.IsOverridingMetadata(corePropertyBag))
				{
					existingMetadata = corePropertyBag.GetValueOrDefault<PropertyChangeMetadata>(CalendarItemInstanceSchema.PropertyChangeMetadata);
					return true;
				}
			}
			existingMetadata = null;
			return false;
		}

		private static bool CleanupSeriesOperationFlagsPropertyValue(ICorePropertyBag propertyBag)
		{
			bool result = false;
			if (propertyBag.IsPropertyDirty(InternalSchema.PropertyChangeMetadataProcessingFlags))
			{
				propertyBag.Delete(InternalSchema.PropertyChangeMetadataProcessingFlags);
				result = true;
			}
			return result;
		}

		private static void UnsetProperties(ICorePropertyBag propertyBag, ICollection<AtomicStorePropertyDefinition> propertiesToUnload)
		{
			Dictionary<AtomicStorePropertyDefinition, object> dictionary = new Dictionary<AtomicStorePropertyDefinition, object>();
			int num = 0;
			foreach (PropertyDefinition propertyDefinition in propertyBag.AllFoundProperties)
			{
				AtomicStorePropertyDefinition atomicStorePropertyDefinition = propertyDefinition as AtomicStorePropertyDefinition;
				if (atomicStorePropertyDefinition != null && propertyBag.IsPropertyDirty(atomicStorePropertyDefinition))
				{
					if (propertiesToUnload.Contains(atomicStorePropertyDefinition))
					{
						num++;
					}
					else
					{
						object obj = propertyBag.TryGetProperty(atomicStorePropertyDefinition);
						PropertyError propertyError = obj as PropertyError;
						if (propertyError != null && !PropertyError.IsPropertyNotFound(obj))
						{
							throw PropertyError.ToException(new PropertyError[]
							{
								propertyError
							});
						}
						dictionary.Add(atomicStorePropertyDefinition, obj);
					}
				}
			}
			if (num > 0)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<int>(0L, "PropertyRuleLibrary::UnsetProperties. There are {0} properties to be unset - reloading property bag from store.", dictionary.Count);
				propertyBag.Reload();
				if (dictionary.Count > 0)
				{
					foreach (KeyValuePair<AtomicStorePropertyDefinition, object> keyValuePair in dictionary)
					{
						propertyBag.SetOrDeleteProperty(keyValuePair.Key, keyValuePair.Value);
					}
				}
			}
		}

		private static void UnsetBitFieldProperties(ICorePropertyBag propertyBag, IEnumerable<PropertyChangeMetadata.PropertyGroup> bitFieldPropertyGroupsToUnset)
		{
			IValidatablePropertyBag validatablePropertyBag = propertyBag as IValidatablePropertyBag;
			if (validatablePropertyBag != null)
			{
				foreach (PropertyChangeMetadata.PropertyGroup propertyGroup in bitFieldPropertyGroupsToUnset)
				{
					bool valueToSet;
					if (PropertyRuleLibrary.TryGetOriginalValueForBitFieldProperty(validatablePropertyBag, propertyGroup.ContainerStoreProperty, propertyGroup.ContainerFlag, out valueToSet))
					{
						PropertyRuleLibrary.SetBitFieldProperty(propertyBag, propertyGroup.ContainerStoreProperty, propertyGroup.ContainerFlag, valueToSet);
					}
				}
			}
		}

		private static bool TryGetOriginalValueForBitFieldProperty(IValidatablePropertyBag propertyBag, StorePropertyDefinition containerPropertyDefinition, int containerFlag, out bool originalValue)
		{
			originalValue = false;
			PropertyValueTrackingData originalPropertyInformation = propertyBag.GetOriginalPropertyInformation(containerPropertyDefinition);
			object originalPropertyValue = originalPropertyInformation.OriginalPropertyValue;
			if (originalPropertyValue is int)
			{
				originalValue = (((int)originalPropertyValue & containerFlag) != 0);
				return true;
			}
			return false;
		}

		private static void SetBitFieldProperty(ICorePropertyBag propertyBag, StorePropertyDefinition containerStoreProperty, int containerFlag, bool valueToSet)
		{
			object obj = propertyBag.TryGetProperty(containerStoreProperty);
			int num = (int)obj;
			if (valueToSet)
			{
				num |= containerFlag;
			}
			else
			{
				num &= ~containerFlag;
			}
			propertyBag.SetProperty(containerStoreProperty, num);
		}

		private static bool CleanEnhancedLocation(ICorePropertyBag propertyBag)
		{
			bool result = false;
			if ((propertyBag.IsPropertyDirty(InternalSchema.HomeStreet) || propertyBag.IsPropertyDirty(InternalSchema.HomeCity) || propertyBag.IsPropertyDirty(InternalSchema.HomeState) || propertyBag.IsPropertyDirty(InternalSchema.HomeCountry) || propertyBag.IsPropertyDirty(InternalSchema.HomePostalCode)) && !propertyBag.IsPropertyDirty(InternalSchema.HomeLatitude) && !propertyBag.IsPropertyDirty(InternalSchema.HomeLongitude))
			{
				propertyBag.Delete(InternalSchema.HomeLatitude);
				propertyBag.Delete(InternalSchema.HomeLongitude);
				propertyBag.Delete(InternalSchema.HomeAccuracy);
				propertyBag.Delete(InternalSchema.HomeAltitude);
				propertyBag.Delete(InternalSchema.HomeAltitudeAccuracy);
				propertyBag.Delete(InternalSchema.HomeLocationUri);
				propertyBag.Delete(InternalSchema.HomeLocationSource);
				result = true;
			}
			if ((propertyBag.IsPropertyDirty(InternalSchema.WorkAddressStreet) || propertyBag.IsPropertyDirty(InternalSchema.WorkAddressCity) || propertyBag.IsPropertyDirty(InternalSchema.WorkAddressState) || propertyBag.IsPropertyDirty(InternalSchema.WorkAddressCountry) || propertyBag.IsPropertyDirty(InternalSchema.WorkAddressPostalCode)) && !propertyBag.IsPropertyDirty(InternalSchema.WorkLatitude) && !propertyBag.IsPropertyDirty(InternalSchema.WorkLongitude))
			{
				propertyBag.Delete(InternalSchema.WorkLatitude);
				propertyBag.Delete(InternalSchema.WorkLongitude);
				propertyBag.Delete(InternalSchema.WorkAccuracy);
				propertyBag.Delete(InternalSchema.WorkAltitude);
				propertyBag.Delete(InternalSchema.WorkAltitudeAccuracy);
				propertyBag.Delete(InternalSchema.WorkLocationUri);
				propertyBag.Delete(InternalSchema.WorkLocationSource);
				result = true;
			}
			if ((propertyBag.IsPropertyDirty(InternalSchema.OtherStreet) || propertyBag.IsPropertyDirty(InternalSchema.OtherCity) || propertyBag.IsPropertyDirty(InternalSchema.OtherState) || propertyBag.IsPropertyDirty(InternalSchema.OtherCountry) || propertyBag.IsPropertyDirty(InternalSchema.OtherPostalCode)) && !propertyBag.IsPropertyDirty(InternalSchema.OtherLatitude) && !propertyBag.IsPropertyDirty(InternalSchema.OtherLongitude))
			{
				propertyBag.Delete(InternalSchema.OtherLatitude);
				propertyBag.Delete(InternalSchema.OtherLongitude);
				propertyBag.Delete(InternalSchema.OtherAccuracy);
				propertyBag.Delete(InternalSchema.OtherAltitude);
				propertyBag.Delete(InternalSchema.OtherAltitudeAccuracy);
				propertyBag.Delete(InternalSchema.OtherLocationUri);
				propertyBag.Delete(InternalSchema.OtherLocationSource);
				result = true;
			}
			return result;
		}

		private static bool IsLegacyLocationModified(ICorePropertyBag propertyBag)
		{
			return propertyBag.IsPropertyDirty(InternalSchema.Location) || propertyBag.IsPropertyDirty(InternalSchema.LidWhere);
		}

		private static bool IsEnhancedLocationModified(ICorePropertyBag propertyBag)
		{
			for (int i = 0; i < CalendarItemProperties.EnhancedLocationPropertyDefinitions.Length; i++)
			{
				if (propertyBag.IsPropertyDirty(CalendarItemProperties.EnhancedLocationPropertyDefinitions[i]))
				{
					ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "PropertyRuleLibrary::CleanConcatEventLocation. Found at least one dirty property = {0}", CalendarItemProperties.EnhancedLocationPropertyDefinitions[i].Name);
					return true;
				}
			}
			return false;
		}

		private static bool CleanConcatEventLocation(ICorePropertyBag propertyBag)
		{
			bool flag = PropertyRuleLibrary.IsLegacyLocationModified(propertyBag);
			bool flag2 = PropertyRuleLibrary.IsEnhancedLocationModified(propertyBag);
			if (flag && flag2)
			{
				ExTraceGlobals.StorageTracer.TraceDebug(0L, "PropertyRuleLibrary::CleanConcatEventLocation. Both legacy and new location properties modified. Ignoring this call.");
				return false;
			}
			if (flag)
			{
				ExTraceGlobals.StorageTracer.TraceDebug(0L, "PropertyRuleLibrary::CleanConcatEventLocation. Reverting all Enhanced Location properties to default values.");
				for (int i = 0; i < CalendarItemProperties.EnhancedLocationPropertiesWithDefaultValues.Length; i++)
				{
					PropertyDefinition item = CalendarItemProperties.EnhancedLocationPropertiesWithDefaultValues[i].Item1;
					object item2 = CalendarItemProperties.EnhancedLocationPropertiesWithDefaultValues[i].Item2;
					propertyBag.SetOrDeleteProperty(item, item2);
				}
				return true;
			}
			if (flag2)
			{
				string text = propertyBag.GetValueOrDefault<string>(InternalSchema.LocationDisplayName, string.Empty);
				string valueOrDefault = propertyBag.GetValueOrDefault<string>(InternalSchema.LocationAnnotation, string.Empty);
				string text2 = propertyBag.GetValueOrDefault<string>(InternalSchema.LocationAddressInternal, string.Empty).Replace("\r\n", ", ");
				LocationSource valueOrDefault2 = propertyBag.GetValueOrDefault<LocationSource>(InternalSchema.LocationSource, LocationSource.None);
				if (valueOrDefault2 == LocationSource.LocationServices && text2.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0)
				{
					text = string.Empty;
				}
				string text3 = text;
				if (text2.Length > 0)
				{
					text3 = ((text3.Length > 0) ? (text3 + " (" + text2 + ")") : text2);
				}
				if (valueOrDefault.Length > 0)
				{
					text3 = ((text3.Length > 0) ? (text3 + " - " + valueOrDefault) : valueOrDefault);
				}
				if (text3.Length > 255)
				{
					text3 = text3.Substring(0, 255);
				}
				ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "PropertyRuleLibrary::CleanConcatEventLocation. Calculated location = {0}", text3);
				if (text3.Length > 0)
				{
					ExTraceGlobals.StorageTracer.TraceDebug(0L, "PropertyRuleLibrary::CleanConcatEventLocation. Updating with new location.");
					propertyBag.SetOrDeleteProperty(InternalSchema.Location, text3);
					propertyBag.SetOrDeleteProperty(InternalSchema.LidWhere, text3);
					return true;
				}
				if (!propertyBag.IsPropertyDirty(InternalSchema.Location) || !propertyBag.IsPropertyDirty(InternalSchema.LidWhere))
				{
					propertyBag.SetOrDeleteProperty(InternalSchema.Location, string.Empty);
					propertyBag.SetOrDeleteProperty(InternalSchema.LidWhere, string.Empty);
					ExTraceGlobals.StorageTracer.TraceDebug(0L, "PropertyRuleLibrary::CleanConcatEventLocation. Updating legacy location with empty string.");
					return true;
				}
			}
			ExTraceGlobals.StorageTracer.TraceDebug(0L, "PropertyRuleLibrary::CleanConcatEventLocation. Nothing to do.");
			return false;
		}

		private static RecurrencePattern GetRecurrencePatternFromSchedulePlusProperties(int? recurrenceType, int? dayInterval, int? weekInterval, int? monthInterval, int? yearInterval, int? dayOfWeekMask, int? dayOfMonthMask, int? monthOfYearMask, int? firstDayOfWeek)
		{
			if (recurrenceType == null)
			{
				return null;
			}
			RecurrenceOrderType order = (RecurrenceOrderType)((weekInterval != null && weekInterval.Value != 5) ? weekInterval.Value : -1);
			RecurrencePattern result = null;
			PropertyRuleLibrary.SchedulePlusRecurrenceType value = (PropertyRuleLibrary.SchedulePlusRecurrenceType)recurrenceType.Value;
			if (value <= PropertyRuleLibrary.SchedulePlusRecurrenceType.Weekly)
			{
				if (value != PropertyRuleLibrary.SchedulePlusRecurrenceType.Yearly)
				{
					if (value != PropertyRuleLibrary.SchedulePlusRecurrenceType.Monthly)
					{
						if (value == PropertyRuleLibrary.SchedulePlusRecurrenceType.Weekly)
						{
							if (dayOfWeekMask != null && PropertyRuleLibrary.IsValidRecurrenceInterval(weekInterval) && firstDayOfWeek != null)
							{
								result = new WeeklyRecurrencePattern((DaysOfWeek)dayOfWeekMask.Value, weekInterval.Value, (DayOfWeek)firstDayOfWeek.Value);
							}
							else if (dayOfWeekMask != null && PropertyRuleLibrary.IsValidRecurrenceInterval(weekInterval))
							{
								result = new WeeklyRecurrencePattern((DaysOfWeek)dayOfWeekMask.Value, weekInterval.Value);
							}
							else if (dayOfWeekMask != null)
							{
								result = new WeeklyRecurrencePattern((DaysOfWeek)dayOfWeekMask.Value);
							}
						}
					}
					else if (dayOfMonthMask != null && PropertyRuleLibrary.IsValidRecurrenceInterval(monthInterval))
					{
						result = new MonthlyRecurrencePattern(PropertyRuleLibrary.ConvertMonthMaskToInt((long)dayOfMonthMask.Value), monthInterval.Value);
					}
					else if (dayOfMonthMask != null)
					{
						result = new MonthlyRecurrencePattern(PropertyRuleLibrary.ConvertMonthMaskToInt((long)dayOfMonthMask.Value));
					}
				}
				else if (dayOfMonthMask != null && monthOfYearMask != null && PropertyRuleLibrary.IsValidRecurrenceInterval(yearInterval))
				{
					result = new YearlyRecurrencePattern(PropertyRuleLibrary.ConvertMonthMaskToInt((long)dayOfMonthMask.Value), PropertyRuleLibrary.ConvertMonthMaskToInt((long)monthOfYearMask.Value), false, yearInterval.Value, CalendarType.Default);
				}
				else if (dayOfMonthMask != null && monthOfYearMask != null)
				{
					result = new YearlyRecurrencePattern(PropertyRuleLibrary.ConvertMonthMaskToInt((long)dayOfMonthMask.Value), PropertyRuleLibrary.ConvertMonthMaskToInt((long)monthOfYearMask.Value));
				}
			}
			else if (value != PropertyRuleLibrary.SchedulePlusRecurrenceType.YearNth)
			{
				if (value != PropertyRuleLibrary.SchedulePlusRecurrenceType.MonthNth)
				{
					if (value == PropertyRuleLibrary.SchedulePlusRecurrenceType.Daily && PropertyRuleLibrary.IsValidRecurrenceInterval(dayInterval))
					{
						result = new DailyRecurrencePattern(dayInterval.Value);
					}
				}
				else if (dayOfWeekMask != null && PropertyRuleLibrary.IsValidRecurrenceInterval(weekInterval) && PropertyRuleLibrary.IsValidRecurrenceInterval(monthInterval))
				{
					result = new MonthlyThRecurrencePattern((DaysOfWeek)dayOfWeekMask.Value, order, monthInterval.Value);
				}
				else if (dayOfWeekMask != null && PropertyRuleLibrary.IsValidRecurrenceInterval(weekInterval))
				{
					result = new MonthlyThRecurrencePattern((DaysOfWeek)dayOfWeekMask.Value, order);
				}
			}
			else if (dayOfWeekMask != null && PropertyRuleLibrary.IsValidRecurrenceInterval(weekInterval) && monthOfYearMask != null && PropertyRuleLibrary.IsValidRecurrenceInterval(yearInterval))
			{
				result = new YearlyThRecurrencePattern((DaysOfWeek)dayOfWeekMask.Value, order, PropertyRuleLibrary.ConvertMonthMaskToInt((long)monthOfYearMask.Value), false, yearInterval.Value, CalendarType.Default);
			}
			else if (dayOfWeekMask != null && PropertyRuleLibrary.IsValidRecurrenceInterval(weekInterval) && monthOfYearMask != null)
			{
				result = new YearlyThRecurrencePattern((DaysOfWeek)dayOfWeekMask.Value, order, PropertyRuleLibrary.ConvertMonthMaskToInt((long)monthOfYearMask.Value));
			}
			return result;
		}

		private static bool IsValidRecurrenceInterval(int? recurrenceInterval)
		{
			return recurrenceInterval != null && recurrenceInterval.Value >= 1 && recurrenceInterval.Value <= StorageLimits.Instance.RecurrenceMaximumInterval;
		}

		private static int ConvertMonthMaskToInt(long mask)
		{
			for (int i = 0; i < PropertyRuleLibrary.MonthMaskArray.Length; i++)
			{
				if ((mask & PropertyRuleLibrary.MonthMaskArray[i]) > 0L)
				{
					return i + 1;
				}
			}
			return 1;
		}

		private static bool StampServerCalendarViewTime(ICorePropertyBag propertyBag)
		{
			MailboxSession mailboxSession = ((IDirectPropertyBag)propertyBag).Context.Session as MailboxSession;
			if (mailboxSession == null)
			{
				return false;
			}
			if (mailboxSession.IsMoveUser)
			{
				if (!VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).DataStorage.CalendarViewPropertyRule.Enabled)
				{
					return false;
				}
			}
			else
			{
				if (mailboxSession.MailboxOwner == null)
				{
					return false;
				}
				VariantConfigurationSnapshot configuration = mailboxSession.MailboxOwner.GetConfiguration();
				if (!configuration.DataStorage.CalendarViewPropertyRule.Enabled)
				{
					return false;
				}
			}
			return PropertyRuleLibrary.StampCalendarViewTime(propertyBag);
		}

		internal static PropertyRule DefaultIsAllDayEvent = new DefaultValuePropertyRule("DefaultIsAllDayEvent", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(44063U, LastChangeAction.DefaultIsAllDayEventRuleApplied);
		}, InternalSchema.MapiIsAllDayEvent, false);

		internal static PropertyRule DefaultRecurrencePattern = new DefaultValuePropertyRule("DefaultRecurrencePattern", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(52255U, LastChangeAction.DefaultRecurrencePatternRuleApplied);
		}, InternalSchema.RecurrencePattern, string.Empty);

		internal static PropertyRule DefaultOwnerAppointmentId = new DefaultValuePropertyRule("DefaultOwnerAppointmentId", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(46111U, LastChangeAction.DefaultOwnerAppointmentIdRuleApplied);
		}, InternalSchema.OwnerAppointmentID, 0);

		internal static PropertyRule DefaultInvitedForCalendarItem = new DefaultValuePropertyRule("DefaultInvitedForCalendarItem", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(62495U, LastChangeAction.DefaultInvitedForCalendarItemRuleApplied);
		}, InternalSchema.MeetingRequestWasSent, false);

		internal static PropertyRule DefaultInvitedForMeetingMessage = new DefaultValuePropertyRule("DefaultInvitedForMeetingMessage", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(37919U, LastChangeAction.DefaultInvitedForMeetingMessageRuleApplied);
		}, InternalSchema.MeetingRequestWasSent, true);

		internal static PropertyRule NativeStartTimeToReminderTime = new PrimaryPropertyRule("NativeStartTimeToReminderTime", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(54303U, LastChangeAction.NativeStartTimeToReminderTimeRuleApplied);
		}, InternalSchema.MapiStartTime, InternalSchema.ReminderDueByInternal);

		internal static PropertyRule NativeStartTimeForCalendar = new EquivalentPropertyRule("NativeStartTimeForCalendar", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(42015U, LastChangeAction.NativeStartTimeForCalendarRuleApplied);
		}, new NativeStorePropertyDefinition[]
		{
			InternalSchema.MapiStartTime,
			InternalSchema.MapiPRStartDate,
			InternalSchema.UtcStartDate
		});

		internal static PropertyRule NativeEndTimeForCalendar = new EquivalentPropertyRule("NativeEndTimeForCalendar", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(58399U, LastChangeAction.NativeEndTimeForCalendarRuleApplied);
		}, new NativeStorePropertyDefinition[]
		{
			InternalSchema.MapiEndTime,
			InternalSchema.MapiPREndDate,
			InternalSchema.UtcDueDate
		});

		internal static PropertyRule NativeStartTimeForMessage = new EquivalentPropertyRule("NativeStartTimeForMessage", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(33823U, LastChangeAction.NativeStartTimeForMessageRuleApplied);
		}, new NativeStorePropertyDefinition[]
		{
			InternalSchema.MapiStartTime,
			InternalSchema.MapiPRStartDate
		});

		internal static PropertyRule NativeEndTimeForMessage = new EquivalentPropertyRule("NativeEndTimeForMessage", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(50207U, LastChangeAction.NativeEndTimeForMessageRuleApplied);
		}, new NativeStorePropertyDefinition[]
		{
			InternalSchema.MapiEndTime,
			InternalSchema.MapiPREndDate
		});

		internal static PropertyRule DefaultCleanGlobalObjectIdFromGlobalObjectId = new CalculatedDefaultValuePropertyRule<byte[], byte[]>("DefaultCleanGlobalObjectIdFromGlobalObjectId", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(47135U, LastChangeAction.DefaultCleanGlobalObjectIdFromGlobalObjectIdRuleApplied);
		}, InternalSchema.GlobalObjectId, InternalSchema.CleanGlobalObjectId, new CalculatedDefaultValuePropertyRule<byte[], byte[]>.CalculateDefaultValueDelegate(PropertyRuleLibrary.GetCleanGoidFromGoid));

		internal static PropertyRule DefaultIsExceptionFromItemClass = new CalculatedDefaultValuePropertyRule<string, bool>("DefaultIsExceptionFromItemClass", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(63519U, LastChangeAction.DefaultIsExceptionFromItemClassRuleApplied);
		}, InternalSchema.ItemClass, InternalSchema.IsException, new CalculatedDefaultValuePropertyRule<string, bool>.CalculateDefaultValueDelegate(PropertyRuleLibrary.GetIsExceptionFromItemClass));

		internal static PropertyRule GlobalObjectIdOnRecurringMaster = new CalculatedValueUpdatePropertyRule<bool, byte[]>("GlobalObjectIdOnRecurringMaster", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(38943U, LastChangeAction.GlobalObjectIdOnRecurringMasterRuleApplied);
		}, InternalSchema.AppointmentRecurring, InternalSchema.GlobalObjectId, new CalculatedValueUpdatePropertyRule<bool, byte[]>.CalculateNewValueDelegate(PropertyRuleLibrary.GetCleanGoidFromGoidIfRecurringMaster));

		internal static PropertyRule NoEmptyTaskRecurrenceBlob = new CustomPropertyRule("NoEmptyTaskRecurrenceBlob", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(38463U, LastChangeAction.NoEmptyTaskRecurrenceBlobRuleApplied);
		}, PropertyRuleLibrary.GetDeleteEmptyRecurrenceBlobFunc(InternalSchema.TaskRecurrence), new PropertyReference[]
		{
			new PropertyReference(InternalSchema.TaskRecurrence, PropertyAccess.ReadWrite)
		});

		internal static PropertyRule DoItTimeProperty = new CustomPropertyRule("DoItTimeProperty", new Func<ICorePropertyBag, bool>(PropertyRuleLibrary.StampDoItTime), new PropertyReference[]
		{
			new PropertyReference(InternalSchema.CreationTime, PropertyAccess.Read),
			new PropertyReference(InternalSchema.ReminderIsSetInternal, PropertyAccess.Read),
			new PropertyReference(InternalSchema.ReminderNextTime, PropertyAccess.Read),
			new PropertyReference(InternalSchema.ModernReminders, PropertyAccess.Read),
			new PropertyReference(InternalSchema.LocalDueDate, PropertyAccess.Read),
			new PropertyReference(InternalSchema.DoItTime, PropertyAccess.ReadWrite)
		});

		internal static PropertyRule ExceptionalBodyFromBody = new CustomPropertyRule("ExceptionalBodyFromBody", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(58047U, LastChangeAction.ExceptionalBodyFromBodyRuleApplied);
		}, new Func<ICorePropertyBag, bool>(PropertyRuleLibrary.SetExceptionalBodyWhenBodyChanged), PropertyRuleLibrary.GetExceptionalBodyPropertyReferences());

		internal static PropertyRule ResponseAndReplyRequested = new CustomPropertyRule("ResponseAndReplyRequested", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(55327U, LastChangeAction.ResponseAndReplyRequestedRuleApplied);
		}, new Func<ICorePropertyBag, bool>(PropertyRuleLibrary.SyncResponseAndReplyRequested), new PropertyReference[]
		{
			new PropertyReference(InternalSchema.DisallowNewTimeProposal, PropertyAccess.Read),
			new PropertyReference(InternalSchema.IsResponseRequested, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.IsReplyRequested, PropertyAccess.ReadWrite)
		});

		internal static PropertyRule CalendarOriginatorId = new CustomPropertyRule("CalendarOriginatorId", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(63260U, LastChangeAction.CalendarOriginatorIdRuleApplied);
		}, new Func<ICorePropertyBag, bool>(PropertyRuleLibrary.SetCalendarOriginatorId), new PropertyReference[]
		{
			new PropertyReference(InternalSchema.ItemClass, PropertyAccess.Read),
			new PropertyReference(InternalSchema.AppointmentStateInternal, PropertyAccess.Read),
			new PropertyReference(InternalSchema.CalendarOriginatorId, PropertyAccess.ReadWrite)
		});

		internal static PropertyRule HasAttendees = new CustomPropertyRule("HasAttendees", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(37324U, LastChangeAction.HasAttendeesRuleApplied);
		}, new Func<ICorePropertyBag, bool>(PropertyRuleLibrary.SetHasAttendees), new PropertyReference[0]);

		internal static PropertyRule DefaultAppointmentStateFromItemClass = new CustomPropertyRule("DefaultAppointmentStateFromItemClass", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(43039U, LastChangeAction.DefaultAppointmentStateFromItemClassRuleApplied);
		}, new Func<ICorePropertyBag, bool>(PropertyRuleLibrary.GetAppointmentStateFromItemClass), new PropertyReference[]
		{
			new PropertyReference(InternalSchema.ItemClass, PropertyAccess.Read),
			new PropertyReference(InternalSchema.MeetingRequestWasSent, PropertyAccess.Read),
			new PropertyReference(InternalSchema.AppointmentStateInternal, PropertyAccess.ReadWrite)
		});

		internal static PropertyRule DefaultOrganizerForAppointments = new CustomPropertyRule("DefaultOrganizer", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(48124U, LastChangeAction.DefaultOrganizerRuleApplied);
		}, new Func<ICorePropertyBag, bool>(PropertyRuleLibrary.SetDefaultOrganizerForAppointments), new PropertyReference[]
		{
			new PropertyReference(InternalSchema.ItemClass, PropertyAccess.Read),
			new PropertyReference(InternalSchema.AppointmentStateInternal, PropertyAccess.Read),
			new PropertyReference(InternalSchema.SentRepresentingDisplayName, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.SentRepresentingEmailAddress, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.SentRepresentingType, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.SentRepresentingEntryId, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.SentRepresentingSmtpAddress, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.SipUri, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.SentRepresentingSID, PropertyAccess.ReadWrite)
		});

		internal static PropertyRule RecurringTimeZone = new CustomPropertyRule("RecurringTimeZone", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(59423U, LastChangeAction.RecurringTimeZoneRuleApplied);
		}, new Func<ICorePropertyBag, bool>(PropertyRuleLibrary.FixRecurringTimeZone), new PropertyReference[]
		{
			new PropertyReference(InternalSchema.AppointmentRecurring, PropertyAccess.Read),
			new PropertyReference(InternalSchema.TimeZone, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.TimeZoneBlob, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.TimeZoneDefinitionRecurring, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.TimeZoneDefinitionStart, PropertyAccess.Write)
		});

		internal static PropertyRule LocationLidWhere = new CustomPropertyRule("LocationLidWhere", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(34847U, LastChangeAction.LocationLidWhereRuleApplied);
		}, new Func<ICorePropertyBag, bool>(PropertyRuleLibrary.SyncLocationAndLidWhere), new PropertyReference[]
		{
			new PropertyReference(InternalSchema.LidWhere, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.Location, PropertyAccess.ReadWrite)
		});

		internal static PropertyRule DefaultClientIntent = new CustomPropertyRule("DefaultClientIntent", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(51231U, LastChangeAction.DefaultClientIntentRuleApplied);
		}, new Func<ICorePropertyBag, bool>(PropertyRuleLibrary.StampDefaultClientIntent), new PropertyReference[]
		{
			new PropertyReference(InternalSchema.ClientIntent, PropertyAccess.ReadWrite)
		});

		internal static List<PropertyReference> TruncateSubjectPropertyReferenceList = new List<PropertyReference>
		{
			new PropertyReference(InternalSchema.MapiSubject, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.SubjectPrefixInternal, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.NormalizedSubjectInternal, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.ConversationTopic, PropertyAccess.Write)
		};

		internal static PropertyRule TruncateSubject = new CustomPropertyRule("TruncateSubject", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(45087U, LastChangeAction.TruncateSubjectRuleApplied);
		}, new Func<ICorePropertyBag, bool>(PropertyRuleLibrary.InternalTruncateSubject), PropertyRuleLibrary.TruncateSubjectPropertyReferenceList.ToArray());

		internal static PropertyRule StartTimeEndTimeToDuration = new CustomPropertyRule("StartTimeEndTimeToDuration", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(61471U, LastChangeAction.StartTimeEndTimeToDurationRuleApplied);
		}, new Func<ICorePropertyBag, bool>(PropertyRuleLibrary.UpdateDurationFromNativeStartEndTime), new PropertyReference[]
		{
			new PropertyReference(InternalSchema.MapiStartTime, PropertyAccess.Read),
			new PropertyReference(InternalSchema.MapiEndTime, PropertyAccess.Read),
			new PropertyReference(InternalSchema.Duration, PropertyAccess.ReadWrite)
		});

		internal static PropertyRule DefaultReminderNextTimeFromStartTimeAndOffset = new CustomPropertyRule("DefaultReminderNextTimeFromStartTimeAndOffset", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(36895U, LastChangeAction.DefaultReminderNextTimeFromStartTimeAndOffsetRuleApplied);
		}, new Func<ICorePropertyBag, bool>(PropertyRuleLibrary.GetReminderNextTimeFromNativeStartTimeAndReminderDelta), new PropertyReference[]
		{
			new PropertyReference(InternalSchema.MapiStartTime, PropertyAccess.Read),
			new PropertyReference(InternalSchema.ReminderMinutesBeforeStartInternal, PropertyAccess.Read),
			new PropertyReference(InternalSchema.ReminderNextTime, PropertyAccess.ReadWrite)
		});

		internal static PropertyRule RecurrenceBlobToFlags = new CustomPropertyRule("RecurrenceBlobToFlags", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(53279U, LastChangeAction.RecurrenceBlobToFlagsRuleApplied);
		}, new Func<ICorePropertyBag, bool>(PropertyRuleLibrary.UpdateRecurringFlags), new PropertyReference[]
		{
			new PropertyReference(InternalSchema.AppointmentRecurrenceBlob, PropertyAccess.Read),
			new PropertyReference(InternalSchema.IsException, PropertyAccess.Read),
			new PropertyReference(InternalSchema.AppointmentStateInternal, PropertyAccess.Read),
			new PropertyReference(InternalSchema.AppointmentRecurring, PropertyAccess.Write),
			new PropertyReference(InternalSchema.IsRecurring, PropertyAccess.Write)
		});

		internal static PropertyRule ClipStartTimeForSingleMeeting = new CustomPropertyRule("ClipStartTimeForSingleMeeting", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(34472U, LastChangeAction.ClipStartTimeForSingleMeetingRuleApplied);
		}, new Func<ICorePropertyBag, bool>(PropertyRuleLibrary.StampClipStartTimeForSingleMeeting), new PropertyReference[]
		{
			new PropertyReference(InternalSchema.MapiStartTime, PropertyAccess.Read),
			new PropertyReference(InternalSchema.ClipStartTime, PropertyAccess.ReadWrite)
		});

		internal static PropertyRule ClipEndTimeForSingleMeeting = new CustomPropertyRule("ClipEndTimeForSingleMeeting", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(50856U, LastChangeAction.ClipEndTimeForSingleMeetingRuleApplied);
		}, new Func<ICorePropertyBag, bool>(PropertyRuleLibrary.StampClipEndTimeForSingleMeeting), new PropertyReference[]
		{
			new PropertyReference(InternalSchema.MapiEndTime, PropertyAccess.Read),
			new PropertyReference(InternalSchema.ClipEndTime, PropertyAccess.ReadWrite)
		});

		internal static PropertyRule SchedulePlusPropertiesToRecurrenceBlob = new CustomPropertyRule("SchedulePlusPropertiesToRecurrenceBlob", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(48287U, LastChangeAction.SchedulePlusPropertiesToRecurrenceBlobRuleApplied);
		}, new Func<ICorePropertyBag, bool>(PropertyRuleLibrary.GenerateRecurrenceBlobFromSchedulePlusProperties), new PropertyReference[]
		{
			new PropertyReference(InternalSchema.LidSingleInvite, PropertyAccess.Read),
			new PropertyReference(InternalSchema.StartRecurDate, PropertyAccess.Read),
			new PropertyReference(InternalSchema.StartRecurTime, PropertyAccess.Read),
			new PropertyReference(InternalSchema.EndRecurDate, PropertyAccess.Read),
			new PropertyReference(InternalSchema.EndRecurTime, PropertyAccess.Read),
			new PropertyReference(InternalSchema.LidDayInterval, PropertyAccess.Read),
			new PropertyReference(InternalSchema.LidWeekInterval, PropertyAccess.Read),
			new PropertyReference(InternalSchema.LidMonthInterval, PropertyAccess.Read),
			new PropertyReference(InternalSchema.LidYearInterval, PropertyAccess.Read),
			new PropertyReference(InternalSchema.LidDayOfWeekMask, PropertyAccess.Read),
			new PropertyReference(InternalSchema.LidDayOfMonthMask, PropertyAccess.Read),
			new PropertyReference(InternalSchema.LidMonthOfYearMask, PropertyAccess.Read),
			new PropertyReference(InternalSchema.LidFirstDayOfWeek, PropertyAccess.Read),
			new PropertyReference(InternalSchema.LidRecurType, PropertyAccess.Read),
			new PropertyReference(InternalSchema.AppointmentRecurrenceBlob, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.TimeZone, PropertyAccess.Write),
			new PropertyReference(InternalSchema.TimeZoneBlob, PropertyAccess.Write),
			new PropertyReference(InternalSchema.TimeZoneDefinitionStart, PropertyAccess.Write),
			new PropertyReference(InternalSchema.TimeZoneDefinitionRecurring, PropertyAccess.Write)
		});

		internal static PropertyRule RemoveAppointmentMadeRecurrentFromSeriesRule = new CustomPropertyRule("RemoveAppointmentMadeRecurrentFromSeriesRule", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(64412U, LastChangeAction.AppointmentsMadeRecurrentRemovedFromSeries);
		}, new Func<ICorePropertyBag, bool>(PropertyRuleLibrary.RemoveAppointmentMadeRecurrentFromSeries), new PropertyReference[]
		{
			new PropertyReference(InternalSchema.AppointmentRecurrenceBlob, PropertyAccess.Read),
			new PropertyReference(InternalSchema.EventClientId, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.SeriesId, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.PropertyChangeMetadataRaw, PropertyAccess.ReadWrite)
		});

		internal static PropertyRule MasterPropertyOverrideProtection = new CustomPropertyRule("MasterPropertyOverrideProtection", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(60700U, LastChangeAction.MasterPropertyOverrideProtectionInvoked);
		}, new Func<ICorePropertyBag, bool>(PropertyRuleLibrary.ProtectMasterPropertyOverridesOnSeriesDataPropagation), new PropertyReference[0]);

		internal static PropertyRule PropertyChangeMetadataTracking = new CustomPropertyRule("PropertyChangeMetadataTracking", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(41692U, LastChangeAction.PropertyChangeMetadataTrackingUpdated);
		}, new Func<ICorePropertyBag, bool>(PropertyRuleLibrary.TrackPropertyChanges), new PropertyReference[0]);

		internal static PropertyRule CleanupSeriesOperationFlagsProperty = new CustomPropertyRule("CleanupSeriesOperationFlagsProperty", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(36124U, LastChangeAction.SeriesOperationVirtualPropertyCleanedUp);
		}, new Func<ICorePropertyBag, bool>(PropertyRuleLibrary.CleanupSeriesOperationFlagsPropertyValue), new PropertyReference[0]);

		internal static PropertyRule EnhancedLocation = new CustomPropertyRule("EnhancedLocation", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(60712U, LastChangeAction.EnhancedLocationRuleApplied);
		}, new Func<ICorePropertyBag, bool>(PropertyRuleLibrary.CleanEnhancedLocation), new PropertyReference[]
		{
			new PropertyReference(InternalSchema.HomeStreet, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.HomeCity, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.HomeState, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.HomeCountry, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.HomePostalCode, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.HomeLatitude, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.HomeLongitude, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.HomeAccuracy, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.HomeAltitude, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.HomeAltitudeAccuracy, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.HomeLocationUri, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.HomeLocationSource, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.WorkAddressStreet, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.WorkAddressCity, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.WorkAddressState, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.WorkAddressCountry, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.WorkAddressPostalCode, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.WorkLatitude, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.WorkLongitude, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.WorkAccuracy, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.WorkAltitude, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.WorkAltitudeAccuracy, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.WorkLocationUri, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.WorkLocationSource, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.OtherStreet, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.OtherCity, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.OtherState, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.OtherCountry, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.OtherPostalCode, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.OtherLatitude, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.OtherLongitude, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.OtherAccuracy, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.OtherAltitude, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.OtherAltitudeAccuracy, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.OtherLocationUri, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.OtherLocationSource, PropertyAccess.ReadWrite)
		});

		internal static PropertyRule EventLocationRule = new CustomPropertyRule("EventLocationRule", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(41256U, LastChangeAction.EventLocationRuleApplied);
		}, new Func<ICorePropertyBag, bool>(PropertyRuleLibrary.CleanConcatEventLocation), new PropertyReference[]
		{
			new PropertyReference(InternalSchema.Location, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.LidWhere, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.LocationDisplayName, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.LocationAnnotation, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.LocationSource, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.LocationUri, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.Latitude, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.Longitude, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.Accuracy, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.Altitude, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.AltitudeAccuracy, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.LocationAddressInternal, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.LocationStreet, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.LocationCity, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.LocationState, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.LocationCountry, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.LocationPostalCode, PropertyAccess.ReadWrite)
		});

		private static ContactDisplayNamePropertyRule contactDisplayNamePropertyRule = new ContactDisplayNamePropertyRule();

		private static PDLDisplayNamePropertyRule pdlDisplayNamePropertyRule = new PDLDisplayNamePropertyRule();

		internal static PropertyRule ContactDisplayNameRule = new CustomPropertyRule("ContactDisplayNameRuleApplied", new Func<ICorePropertyBag, bool>(PropertyRuleLibrary.contactDisplayNamePropertyRule.UpdateDisplayNameProperties), PropertyRuleLibrary.contactDisplayNamePropertyRule.GetPropertyReferenceList().ToArray());

		internal static PropertyRule PDLDisplayNameRule = new CustomPropertyRule("PDLDisplayNameRuleApplied", new Func<ICorePropertyBag, bool>(PropertyRuleLibrary.pdlDisplayNamePropertyRule.UpdateDisplayNameProperties), PropertyRuleLibrary.pdlDisplayNamePropertyRule.GetPropertyReferenceList().ToArray());

		internal static PropertyRule PDLMembershipRule = new CustomPropertyRule("PDLMembershipRuleApplied", new Func<ICorePropertyBag, bool>(PDLMembershipPropertyRule.UpdateProperties), PDLMembershipPropertyRule.PropertyReferences);

		internal static PropertyRule PersonIdRule = new CustomPropertyRule("PersonIdRuleApplied", new Func<ICorePropertyBag, bool>(new PersonIdPropertyRule().UpdateProperties), PersonIdPropertyRule.PropertyReferences);

		internal static PropertyRule EmailAddressUpdateRule = new CustomPropertyRule("EmailAddressPropertyRuleApplied", new Func<ICorePropertyBag, bool>(EmailAddressPropertyRule.UpdateEmailProperties), EmailAddressPropertyRule.UpdateProperties);

		private static readonly long[] MonthMaskArray = new long[]
		{
			1L,
			2L,
			4L,
			8L,
			16L,
			32L,
			64L,
			128L,
			256L,
			512L,
			1024L,
			2048L,
			4096L,
			8192L,
			16384L,
			32768L,
			65536L,
			131072L,
			262144L,
			524288L,
			1048576L,
			2097152L,
			4194304L,
			8388608L,
			16777216L,
			33554432L,
			67108864L,
			134217728L,
			268435456L,
			536870912L,
			1073741824L,
			2147483648L
		};

		private static readonly string[] SchedulePlusTimeZoneId = new string[]
		{
			"GMT Standard Time",
			"GMT Standard Time",
			"Central European Standard Time",
			"Romance Standard Time",
			"W. Europe Standard Time",
			"E. Europe Standard Time",
			"Central Europe Standard Time",
			"GTB Standard Time",
			"E. South America Standard Time",
			"Atlantic Standard Time",
			"Eastern Standard Time",
			"Central Standard Time",
			"Mountain Standard Time",
			"Pacific Standard Time",
			"Alaskan Standard Time",
			"Hawaiian Standard Time",
			"Samoa Standard Time",
			"New Zealand Standard Time",
			"AUS Eastern Standard Time",
			"Cen. Australia Standard Time",
			"Tokyo Standard Time",
			"Taipei Standard Time",
			"SE Asia Standard Time",
			"India Standard Time",
			"Arabian Standard Time",
			"Iran Standard Time",
			"Arab Standard Time",
			"Israel Standard Time",
			"Newfoundland Standard Time",
			"Azores Standard Time",
			"Mid-Atlantic Standard Time",
			"Greenwich Standard Time",
			"SA Eastern Standard Time",
			"SA Western Standard Time",
			"US Eastern Standard Time",
			"SA Pacific Standard Time",
			"Canada Central Standard Time",
			"Central Standard Time (Mexico)",
			"US Mountain Standard Time",
			"Dateline Standard Time",
			"Fiji Standard Time",
			"Central Pacific Standard Time",
			"Tasmania Standard Time",
			"West Pacific Standard Time",
			"AUS Central Standard Time",
			"China Standard Time",
			"Central Asia Standard Time",
			"West Asia Standard Time",
			"Afghanistan Standard Time",
			"Egypt Standard Time",
			"South Africa Standard Time",
			"Russian Standard Time",
			"AUS Eastern Standard Time",
			"AUS Eastern Standard Time",
			"Cen. Australia Standard Time",
			"Tasmania Standard Time",
			"Pacific SA Standard Time",
			"W. Australia Standard Time",
			"Mountain Standard Time (Mexico)",
			"Pacific Standard Time (Mexico)",
			"Caucasus Standard Time",
			"AUS Eastern Standard Time",
			"Venezuela Standard Time",
			"Jordan Standard Time",
			"Azerbaijan Standard Time",
			"Armenian Standard Time",
			"Georgian Standard Time",
			"Argentina Standard Time",
			"Morocco Standard Time",
			"Pakistan Standard Time"
		};

		internal static PropertyRule OscContactSourcesForContactRule = new CustomPropertyRule("OscContactSourcesForContactUpdateRuleApplied", new Func<ICorePropertyBag, bool>(new OscContactSourcesForContactUpdateRule().UpdatePartnerNetworkProperties), OscContactSourcesForContactUpdateRule.UpdateProperties);

		internal static PropertyRule CalendarViewProperties = new CustomPropertyRule("CalendarViewProperties", delegate(ILocationIdentifierSetter lidSetter)
		{
			lidSetter.SetLocationIdentifier(57404U, LastChangeAction.CalendarViewPropertiesApplied);
		}, new Func<ICorePropertyBag, bool>(PropertyRuleLibrary.StampServerCalendarViewTime), new PropertyReference[]
		{
			new PropertyReference(InternalSchema.MapiStartTime, PropertyAccess.Read),
			new PropertyReference(InternalSchema.IsRecurring, PropertyAccess.Read),
			new PropertyReference(InternalSchema.AppointmentRecurrenceBlob, PropertyAccess.Read),
			new PropertyReference(InternalSchema.TimeZone, PropertyAccess.Read),
			new PropertyReference(InternalSchema.TimeZoneBlob, PropertyAccess.Read),
			new PropertyReference(InternalSchema.TimeZoneDefinitionRecurring, PropertyAccess.Read),
			new PropertyReference(InternalSchema.Codepage, PropertyAccess.Read),
			new PropertyReference(InternalSchema.ViewStartTime, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.ViewEndTime, PropertyAccess.ReadWrite)
		});

		private enum SchedulePlusRecurrenceType
		{
			Unknown,
			Daily = 64,
			Weekly = 48,
			Monthly = 12,
			MonthNth = 56,
			Yearly = 7,
			YearNth = 51
		}
	}
}
