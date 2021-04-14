using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class CalendarNavigator
	{
		internal CalendarNavigator(UMSubscriber user)
		{
			this.user = user;
			this.Today();
		}

		internal ExDateTime CurrentDay
		{
			get
			{
				return this.currentDay;
			}
		}

		internal UMSubscriber Owner
		{
			get
			{
				return this.user;
			}
		}

		internal ArrayList CurrentAgenda
		{
			get
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "Getting view between {0} and {1}.", new object[]
				{
					this.currentDay,
					this.currentDay.AddDays(1.0).AddMinutes(-1.0)
				});
				ArrayList arrayList = new ArrayList();
				ArrayList result;
				using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.user.CreateSessionLock())
				{
					using (CalendarFolder calendarFolder = CalendarFolder.Bind(mailboxSessionLock.Session, mailboxSessionLock.Session.GetDefaultFolderId(DefaultFolderType.Calendar)))
					{
						object[][] calendarView = calendarFolder.GetCalendarView(this.currentDay, this.currentDay.AddDays(1.0).AddMinutes(-1.0), new PropertyDefinition[]
						{
							ItemSchema.Id,
							CalendarItemInstanceSchema.StartTime,
							CalendarItemInstanceSchema.EndTime,
							CalendarItemBaseSchema.IsAllDayEvent,
							CalendarItemBaseSchema.AppointmentState
						});
						CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "Found {0} calendar items in this view.", new object[]
						{
							calendarView.Length
						});
						for (int i = 0; i < calendarView.Length; i++)
						{
							if (this.IsValidMeeting(calendarView[i][0], calendarView[i][1], calendarView[i][2], calendarView[i][4], this.currentDay))
							{
								arrayList.Add(new CalendarNavigator.MeetingInfo(calendarView[i], this.user));
							}
						}
						arrayList.Sort();
						result = arrayList;
					}
				}
				return result;
			}
		}

		internal void Next()
		{
			this.currentDay = this.currentDay.AddDays(1.0);
		}

		internal void Previous()
		{
			this.currentDay = this.currentDay.AddDays(-1.0);
		}

		internal void Goto(ExDateTime target)
		{
			this.currentDay = target;
		}

		internal bool SeekNext()
		{
			ExDateTime date = this.currentDay.AddDays(1.0).Date;
			ExDateTime date2 = date.AddDays(8.0).AddMinutes(-1.0).Date;
			CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "Calendar navigator seeking next meeting between {0} and {1}.", new object[]
			{
				date,
				date2
			});
			bool result;
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.user.CreateSessionLock())
			{
				using (CalendarFolder calendarFolder = CalendarFolder.Bind(mailboxSessionLock.Session, mailboxSessionLock.Session.GetDefaultFolderId(DefaultFolderType.Calendar)))
				{
					object[][] calendarView = calendarFolder.GetCalendarView(date, date2, new PropertyDefinition[]
					{
						ItemSchema.Id,
						CalendarItemInstanceSchema.StartTime,
						CalendarItemInstanceSchema.EndTime,
						CalendarItemBaseSchema.AppointmentState
					});
					ExDateTime exDateTime = ExDateTime.MaxValue;
					for (int i = 0; i < calendarView.Length; i++)
					{
						ExDateTime t = (ExDateTime)calendarView[i][1];
						if (t.Date > this.currentDay && t < exDateTime && this.IsValidMeeting(calendarView[i][0], calendarView[i][1], calendarView[i][2], calendarView[i][3], t.Date))
						{
							exDateTime = t.Date;
						}
					}
					if (exDateTime < ExDateTime.MaxValue)
					{
						this.currentDay = exDateTime;
						CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "Next meeting found is {0}.", new object[]
						{
							exDateTime
						});
						result = true;
					}
					else
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "No meetings found for {0} days, starting, {1}.", new object[]
						{
							7,
							this.currentDay
						});
						this.currentDay = this.currentDay.AddDays(7.0).Date;
						result = false;
					}
				}
			}
			return result;
		}

		internal void Today()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "Resetting calendar view to Today.", new object[0]);
			this.currentDay = this.user.Now.Date;
		}

		internal void SkipMeeting(StoreObjectId objectId)
		{
			if (this.skipList == null)
			{
				this.skipList = new Dictionary<StoreObjectId, bool>();
			}
			this.skipList[objectId] = true;
		}

		private bool IsInSkipList(StoreObjectId objectId)
		{
			return this.skipList != null && this.skipList.ContainsKey(objectId);
		}

		private bool IsValidMeeting(object itemStoreId, object itemStartTime, object itemEndTime, object itemAppointmentState, ExDateTime agendaDate)
		{
			if (!(itemStoreId is StoreId) || !(itemStartTime is ExDateTime) || !(itemEndTime is ExDateTime))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "Calendar Item doesn't have a valid ID or start or end time... ignoring it.", new object[0]);
				return false;
			}
			if (this.IsInSkipList(((VersionedId)itemStoreId).ObjectId))
			{
				return false;
			}
			int num = (itemAppointmentState is int) ? ((int)itemAppointmentState) : 0;
			return (num & 4) == 0 && (ExDateTime)itemEndTime > agendaDate;
		}

		internal const int MaxSearchWindowDays = 7;

		private ExDateTime currentDay;

		private UMSubscriber user;

		private Dictionary<StoreObjectId, bool> skipList;

		internal class AttendeeInfo
		{
			internal AttendeeInfo(Attendee attendee)
			{
				this.attendeeType = attendee.AttendeeType;
				this.responseType = attendee.ResponseType;
				this.participant = attendee.Participant;
			}

			internal AttendeeType AttendeeType
			{
				get
				{
					return this.attendeeType;
				}
			}

			internal ResponseType ResponseType
			{
				get
				{
					return this.responseType;
				}
			}

			internal Participant Participant
			{
				get
				{
					return this.participant;
				}
			}

			private AttendeeType attendeeType;

			private ResponseType responseType;

			private Participant participant;
		}

		internal class MeetingInfo : IComparable
		{
			internal MeetingInfo(object[] meetingProps, UMSubscriber user)
			{
				this.id = ((VersionedId)meetingProps[0]).ObjectId;
				this.startTime = (ExDateTime)meetingProps[1];
				this.endTime = (ExDateTime)meetingProps[2];
				this.isAllDayEvent = (meetingProps[3] is bool && (bool)meetingProps[3]);
				this.user = user;
			}

			internal StoreObjectId UniqueId
			{
				get
				{
					return this.id;
				}
			}

			internal bool IsOrganizer
			{
				get
				{
					return this.Cache.IsOrganizer;
				}
			}

			internal string Subject
			{
				get
				{
					return this.Cache.Subject;
				}
			}

			internal string Location
			{
				get
				{
					return this.Cache.Location;
				}
			}

			internal string OrganizerEmail
			{
				get
				{
					return this.Cache.OrganizerEmail;
				}
			}

			internal string OrganizerName
			{
				get
				{
					return this.Cache.OrganizerName;
				}
			}

			internal ExDateTime StartTime
			{
				get
				{
					return this.startTime;
				}
			}

			internal ExDateTime EndTime
			{
				get
				{
					return this.endTime;
				}
			}

			internal BusyType FreeBusyStatus
			{
				get
				{
					return this.Cache.ClassicFreeBusyStatus;
				}
			}

			internal bool IsCancelled
			{
				get
				{
					return this.Cache.IsCancelled;
				}
			}

			internal bool IsMeeting
			{
				get
				{
					return this.Cache.IsMeeting;
				}
			}

			internal bool IsAllDayEvent
			{
				get
				{
					return this.isAllDayEvent;
				}
			}

			internal List<CalendarNavigator.AttendeeInfo> Attendees
			{
				get
				{
					return this.Cache.Attendees;
				}
			}

			internal PhoneNumber OrganizerPhone
			{
				get
				{
					if (this.Cache.OrganizerPhone == null)
					{
						this.Cache.OrganizerPhone = this.BuildOrganizerPhone();
					}
					return this.Cache.OrganizerPhone;
				}
			}

			internal PhoneNumber LocationPhone
			{
				get
				{
					if (this.Cache.LocationPhone == null)
					{
						this.Cache.LocationPhone = this.BuildLocationPhone();
					}
					return this.Cache.LocationPhone;
				}
			}

			private CalendarNavigator.MeetingInfo.CachedData Cache
			{
				get
				{
					if (this.cache == null)
					{
						PIIMessage data = PIIMessage.Create(PIIType._User, this.user);
						CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, data, "Getting store item for User=_User, MeetingInfo={0}.", new object[]
						{
							this.user,
							this.id.ToBase64String()
						});
						using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.user.CreateSessionLock())
						{
							using (CalendarItemBase calendarItemBase = CalendarItemBase.Bind(mailboxSessionLock.Session, this.id))
							{
								this.cache = new CalendarNavigator.MeetingInfo.CachedData(calendarItemBase);
							}
						}
					}
					return this.cache;
				}
			}

			public override string ToString()
			{
				return string.Format(CultureInfo.InvariantCulture, "Id={0}, Start={1}, End={2}, User={3}", new object[]
				{
					this.id.ToBase64String(),
					this.StartTime,
					this.EndTime,
					this.user.ExchangeLegacyDN
				});
			}

			public int CompareTo(object obj)
			{
				CalendarNavigator.MeetingInfo meetingInfo = (CalendarNavigator.MeetingInfo)obj;
				if (this.startTime < meetingInfo.startTime)
				{
					return -1;
				}
				if (!(this.startTime == meetingInfo.startTime))
				{
					return 1;
				}
				if (this.endTime < meetingInfo.endTime)
				{
					return -1;
				}
				if (this.endTime == meetingInfo.endTime)
				{
					return 0;
				}
				return 1;
			}

			internal void AcceptMeeting()
			{
				this.cache = null;
				using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.user.CreateSessionLock())
				{
					using (CalendarItemBase calendarItemBase = CalendarItemBase.Bind(mailboxSessionLock.Session, this.UniqueId))
					{
						calendarItemBase.OpenAsReadWrite();
						using (MessageItem messageItem = XsoUtil.RespondToMeetingRequest(calendarItemBase, ResponseType.Accept))
						{
							messageItem[MessageItemSchema.VoiceMessageDuration] = 0;
							XsoUtil.SetSubscriberAccessSenderProperties(messageItem, this.user);
							messageItem.Send();
						}
						calendarItemBase.Load();
						this.id = calendarItemBase.Id.ObjectId;
						this.cache = new CalendarNavigator.MeetingInfo.CachedData(calendarItemBase);
					}
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "MeetingInfo::AcceptMeeting successfully built response.", new object[0]);
			}

			internal void MarkAsTentative()
			{
				this.cache = null;
				using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.user.CreateSessionLock())
				{
					using (CalendarItemBase calendarItemBase = CalendarItemBase.Bind(mailboxSessionLock.Session, this.UniqueId))
					{
						calendarItemBase.OpenAsReadWrite();
						using (MessageItem messageItem = XsoUtil.RespondToMeetingRequest(calendarItemBase, ResponseType.Tentative))
						{
							messageItem[MessageItemSchema.VoiceMessageDuration] = 0;
							XsoUtil.SetSubscriberAccessSenderProperties(messageItem, this.user);
							messageItem.Send();
							calendarItemBase.Load();
							this.id = calendarItemBase.Id.ObjectId;
							this.cache = new CalendarNavigator.MeetingInfo.CachedData(calendarItemBase);
						}
					}
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "MeetingInfo::MarkAsTentative successfully built response.", new object[0]);
			}

			private PhoneNumber BuildLocationPhone()
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "Looking for location phone for location={0}.", new object[]
				{
					this.Location
				});
				Participant participant = null;
				foreach (CalendarNavigator.AttendeeInfo attendeeInfo in this.Cache.Attendees)
				{
					if (!string.IsNullOrEmpty(attendeeInfo.Participant.DisplayName) && string.Compare(attendeeInfo.Participant.DisplayName, this.Location, StringComparison.OrdinalIgnoreCase) == 0)
					{
						participant = attendeeInfo.Participant;
						break;
					}
				}
				PhoneNumber phoneNumber = null;
				if (null != participant)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "Found location participant={0}.", new object[]
					{
						participant
					});
					ContactInfo contactInfo = ContactInfo.FindByParticipant(this.user, participant);
					if (contactInfo != null)
					{
						phoneNumber = Util.GetNumberToDial(this.user, contactInfo);
					}
				}
				PIIMessage data = PIIMessage.Create(PIIType._PhoneNumber, phoneNumber);
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, data, "Returning location phone=_PhoneNumber.", new object[0]);
				return phoneNumber;
			}

			private PhoneNumber BuildOrganizerPhone()
			{
				PIIMessage data = PIIMessage.Create(PIIType._EmailAddress, this.OrganizerEmail);
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, data, "Looking for organizerPhone for organizer=_EmailAddress.", new object[0]);
				PhoneNumber phoneNumber = null;
				if (null != this.Cache.Organizer)
				{
					ContactInfo contactInfo = ContactInfo.FindByParticipant(this.user, this.Cache.Organizer);
					if (contactInfo != null)
					{
						phoneNumber = Util.GetNumberToDial(this.user, contactInfo);
					}
				}
				PIIMessage data2 = PIIMessage.Create(PIIType._PhoneNumber, phoneNumber);
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, data2, "Returning organizer phone=_PhoneNumber.", new object[0]);
				return phoneNumber;
			}

			private StoreObjectId id;

			private ExDateTime startTime;

			private ExDateTime endTime;

			private bool isAllDayEvent;

			private UMSubscriber user;

			private CalendarNavigator.MeetingInfo.CachedData cache;

			private class CachedData
			{
				internal CachedData(CalendarItemBase calendarItem)
				{
					calendarItem.Load(new PropertyDefinition[]
					{
						ItemSchema.Subject,
						CalendarItemBaseSchema.Location,
						CalendarItemBaseSchema.FreeBusyStatus,
						CalendarItemBaseSchema.IsMeeting,
						CalendarItemBaseSchema.OrganizerEmailAddress,
						CalendarItemBaseSchema.IsAllDayEvent,
						ItemSchema.SentRepresentingDisplayName
					});
					this.subject = calendarItem.Subject;
					this.location = calendarItem.Location;
					this.classicFreeBusyStatus = calendarItem.FreeBusyStatus;
					this.isCancelled = calendarItem.IsCancelled;
					this.isMeeting = calendarItem.IsMeeting;
					this.organizer = calendarItem.Organizer;
					this.organizerEmail = (string)XsoUtil.SafeGetProperty(calendarItem, CalendarItemBaseSchema.OrganizerEmailAddress, string.Empty);
					this.organizerName = (string)XsoUtil.SafeGetProperty(calendarItem, ItemSchema.SentRepresentingDisplayName, this.organizerEmail);
					this.isOrganizer = (calendarItem.IsOrganizer() || null == calendarItem.Organizer);
					this.attendees = new List<CalendarNavigator.AttendeeInfo>();
					foreach (Attendee attendee in calendarItem.AttendeeCollection)
					{
						if (AttendeeType.Resource != attendee.AttendeeType && string.Compare(attendee.Participant.EmailAddress, this.organizerEmail, true, CultureInfo.InvariantCulture) != 0)
						{
							this.attendees.Add(new CalendarNavigator.AttendeeInfo(attendee));
						}
					}
				}

				internal string Subject
				{
					get
					{
						return this.subject;
					}
				}

				internal bool IsOrganizer
				{
					get
					{
						return this.isOrganizer;
					}
				}

				internal string Location
				{
					get
					{
						return this.location;
					}
				}

				internal BusyType ClassicFreeBusyStatus
				{
					get
					{
						return this.classicFreeBusyStatus;
					}
				}

				internal bool IsCancelled
				{
					get
					{
						return this.isCancelled;
					}
				}

				internal bool IsMeeting
				{
					get
					{
						return this.isMeeting;
					}
				}

				internal string OrganizerEmail
				{
					get
					{
						return this.organizerEmail;
					}
				}

				internal string OrganizerName
				{
					get
					{
						return this.organizerName;
					}
				}

				internal List<CalendarNavigator.AttendeeInfo> Attendees
				{
					get
					{
						return this.attendees;
					}
				}

				internal PhoneNumber OrganizerPhone
				{
					get
					{
						return this.organizerPhone;
					}
					set
					{
						this.organizerPhone = value;
					}
				}

				internal Participant Organizer
				{
					get
					{
						return this.organizer;
					}
				}

				internal PhoneNumber LocationPhone
				{
					get
					{
						return this.locationPhone;
					}
					set
					{
						this.locationPhone = value;
					}
				}

				private string subject;

				private bool isOrganizer;

				private string location;

				private BusyType classicFreeBusyStatus;

				private bool isCancelled;

				private bool isMeeting;

				private string organizerEmail;

				private string organizerName;

				private List<CalendarNavigator.AttendeeInfo> attendees;

				private PhoneNumber organizerPhone;

				private Participant organizer;

				private PhoneNumber locationPhone;
			}
		}

		internal class AgendaContext
		{
			internal AgendaContext(ArrayList agenda, UMSubscriber user, bool isInitialPosition, bool isReadConflicts)
			{
				this.agenda = agenda;
				this.conflicts = new ArrayList();
				this.isInitialPosition = isInitialPosition;
				this.isReadConflicts = isReadConflicts;
				this.user = user;
				this.isOnlyReadRemaining = isInitialPosition;
				this.SeekBest();
			}

			internal int ConflictCount
			{
				get
				{
					return this.conflicts.Count;
				}
			}

			internal int Remaining
			{
				get
				{
					int num = 0;
					for (int i = 0; i < this.agenda.Count; i++)
					{
						if (!this.IsOver((CalendarNavigator.MeetingInfo)this.agenda[i]))
						{
							num++;
						}
					}
					return num;
				}
			}

			internal int TotalCount
			{
				get
				{
					return this.agenda.Count;
				}
			}

			internal CalendarNavigator.MeetingInfo Current
			{
				get
				{
					return (CalendarNavigator.MeetingInfo)this.agenda[this.idx];
				}
			}

			internal CalendarNavigator.MeetingInfo CurrentConflict
			{
				get
				{
					return (CalendarNavigator.MeetingInfo)this.conflicts[this.conflictIdx];
				}
			}

			internal ExDateTime ConflictTime
			{
				get
				{
					return this.conflictTime;
				}
			}

			internal bool IsFirst
			{
				get
				{
					return 0 == this.idx;
				}
			}

			internal bool IsFirstConflict
			{
				get
				{
					return this.ConflictCount > 0 && 0 == this.conflictIdx;
				}
			}

			internal bool IsLast
			{
				get
				{
					return this.agenda.Count == this.idx + 1;
				}
			}

			internal bool IsInitialPosition
			{
				get
				{
					return this.isInitialPosition;
				}
			}

			internal bool IsValid
			{
				get
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "AgendaContext::IsValid. agenda={0}, idx={1}.", new object[]
					{
						this.agenda,
						this.idx
					});
					return this.agenda != null && this.idx >= 0 && this.idx < this.agenda.Count;
				}
			}

			internal bool ConflictsWithLastHeard
			{
				get
				{
					return this.lastMeetingHeard != null && this.IsValid && this.lastMeetingHeard != this.Current && ((this.lastMeetingHeard.StartTime >= this.Current.StartTime && this.lastMeetingHeard.StartTime < this.Current.EndTime) || (this.Current.StartTime >= this.lastMeetingHeard.StartTime && this.Current.StartTime < this.lastMeetingHeard.EndTime));
				}
			}

			internal bool Next()
			{
				return this.Next(false);
			}

			internal bool Next(bool keepLastInContext)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "AgendaContext.Next().", new object[0]);
				this.lastMeetingHeard = (this.IsValid ? this.Current : null);
				if (this.IsValid)
				{
					this.isInitialPosition = false;
				}
				if (this.isReadConflicts && 0 < this.ConflictCount && ++this.conflictIdx < this.ConflictCount)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "Still in conflict.", new object[0]);
					return true;
				}
				if (keepLastInContext && !this.HasNext())
				{
					return false;
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "Seeking next meeting of the day.", new object[0]);
				this.SeekNext();
				this.SetConflictContext();
				return this.idx < this.agenda.Count;
			}

			internal bool Previous()
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "AgendaContext.Previous().", new object[0]);
				this.lastMeetingHeard = (this.IsValid ? this.Current : null);
				if (this.IsValid && !this.Current.IsAllDayEvent)
				{
					this.isInitialPosition = false;
				}
				this.isOnlyReadRemaining = false;
				if (!this.HasPrevious())
				{
					return false;
				}
				if (this.isReadConflicts && 0 < this.ConflictCount && --this.conflictIdx >= 0)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "Still in conflict.", new object[0]);
					return true;
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "Seeking previous.", new object[0]);
				this.SeekPrevious();
				this.SetConflictContext();
				return 0 <= this.idx;
			}

			internal bool SeekFirst()
			{
				this.lastMeetingHeard = (this.IsValid ? this.Current : null);
				this.isInitialPosition = false;
				this.isOnlyReadRemaining = false;
				this.idx = 0;
				this.SetConflictContext();
				return this.idx < this.agenda.Count;
			}

			internal bool SeekLast()
			{
				this.lastMeetingHeard = (this.IsValid ? this.Current : null);
				this.isInitialPosition = false;
				this.isOnlyReadRemaining = false;
				this.idx = this.agenda.Count;
				this.SeekPrevious();
				this.SetConflictContext();
				return this.IsValid;
			}

			internal void Remove(StoreObjectId theIdToRemove)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "AgendaContext::Remove.", new object[0]);
				for (int i = 0; i < this.conflicts.Count; i++)
				{
					CalendarNavigator.MeetingInfo meetingInfo = (CalendarNavigator.MeetingInfo)this.conflicts[i];
					if (meetingInfo.UniqueId == theIdToRemove)
					{
						this.conflicts.RemoveAt(i);
					}
				}
				int num = -1;
				for (int j = 0; j < this.agenda.Count; j++)
				{
					CalendarNavigator.MeetingInfo meetingInfo2 = (CalendarNavigator.MeetingInfo)this.agenda[j];
					if (meetingInfo2.UniqueId == theIdToRemove)
					{
						num = j;
					}
				}
				if (-1 == num)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "AgendaContext::Remove. Did not find meeting to remove in agenda!", new object[0]);
					return;
				}
				this.agenda.RemoveAt(num);
				if (num < this.idx)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "Removed a meeting that's before the current one in the agenda.", new object[0]);
					this.idx--;
				}
				this.PostRemovalCleanup();
			}

			internal void RemoveMeetings(IList<StoreObjectId> idsToRemove)
			{
				int i = 0;
				while (i < this.agenda.Count)
				{
					CalendarNavigator.MeetingInfo meetingInfo = (CalendarNavigator.MeetingInfo)this.agenda[i];
					if (-1 != idsToRemove.IndexOf(meetingInfo.UniqueId))
					{
						this.agenda.RemoveAt(i);
						if (i < this.idx)
						{
							this.idx--;
						}
					}
					else
					{
						i++;
					}
				}
				i = 0;
				while (i < this.conflicts.Count)
				{
					CalendarNavigator.MeetingInfo meetingInfo2 = (CalendarNavigator.MeetingInfo)this.conflicts[i];
					if (-1 != idsToRemove.IndexOf(meetingInfo2.UniqueId))
					{
						this.conflicts.RemoveAt(i);
					}
					else
					{
						i++;
					}
				}
				this.PostRemovalCleanup();
			}

			private bool SeekBest()
			{
				int num = 0;
				CalendarNavigator.MeetingInfo bestSoFar = null;
				if (this.isInitialPosition)
				{
					if (this.isReadConflicts)
					{
						num = this.agenda.Count;
						for (int i = 0; i < this.agenda.Count; i++)
						{
							CalendarNavigator.MeetingInfo meetingInfo = (CalendarNavigator.MeetingInfo)this.agenda[i];
							if (this.IsBetter(meetingInfo, bestSoFar))
							{
								bestSoFar = meetingInfo;
								num = i;
							}
						}
					}
					else
					{
						num = this.agenda.Count;
						for (int j = 0; j < this.agenda.Count; j++)
						{
							CalendarNavigator.MeetingInfo mi = (CalendarNavigator.MeetingInfo)this.agenda[j];
							if (!this.IsOver(mi))
							{
								num = j;
								break;
							}
						}
					}
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "SeekBest stops at meeting= {0}.", new object[]
				{
					num
				});
				this.idx = num;
				this.SetConflictContext();
				return this.idx < this.agenda.Count;
			}

			private void PostRemovalCleanup()
			{
				if (1 < this.ConflictCount && this.conflictIdx < this.ConflictCount)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "Still in conflict after removal of meeting.", new object[0]);
					return;
				}
				if (!this.IsValid)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "Not on a valid meeting after removal of meeting(s).", new object[0]);
					return;
				}
				if (this.isOnlyReadRemaining && this.IsOver(this.Current))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "Removed a meeting with fOnlyReadRemaining and next meeting is over.", new object[0]);
					this.Next();
					return;
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "setting conflict context after meeting removal.", new object[0]);
				this.SetConflictContext();
			}

			private bool HasPrevious()
			{
				if (this.ConflictCount > 0)
				{
					return this.conflictIdx > 0 || this.idx > 0;
				}
				return this.idx > 0;
			}

			private bool HasNext()
			{
				int num = this.idx;
				this.SeekNext();
				bool isValid = this.IsValid;
				this.idx = num;
				return isValid;
			}

			private void SetConflictContext()
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "Setting conflict context.", new object[0]);
				this.conflicts.Clear();
				this.conflictIdx = 0;
				if (!this.IsValid)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "idx not valid in setconflictcontext: idx={0}. returning empty arraylist.", new object[]
					{
						this.idx
					});
					return;
				}
				if (!this.isReadConflicts)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "not reading conflicts in setconflictcontext.", new object[0]);
					return;
				}
				if (this.Current.IsAllDayEvent)
				{
					return;
				}
				this.SetConflictTime();
				for (int i = 0; i < this.agenda.Count; i++)
				{
					CalendarNavigator.MeetingInfo meetingInfo = (CalendarNavigator.MeetingInfo)this.agenda[i];
					if (!meetingInfo.IsAllDayEvent)
					{
						if (meetingInfo.StartTime > this.Current.StartTime)
						{
							break;
						}
						if (meetingInfo != this.Current && !(meetingInfo.EndTime <= this.Current.StartTime))
						{
							if (!this.isInitialPosition)
							{
								if (meetingInfo.StartTime == this.Current.StartTime)
								{
									this.conflicts.Add(meetingInfo);
								}
							}
							else if (this.InProgress(this.Current))
							{
								if (this.InProgress(meetingInfo))
								{
									this.conflicts.Add(meetingInfo);
								}
							}
							else if (meetingInfo.StartTime == this.Current.StartTime)
							{
								this.conflicts.Add(meetingInfo);
							}
						}
					}
				}
				if (this.conflicts.Count > 0)
				{
					this.conflicts.Add(this.Current);
					this.conflicts.Sort();
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "Found {0} conflicts at {1}.", new object[]
				{
					this.conflicts.Count,
					this.ConflictTime
				});
			}

			private void SetConflictTime()
			{
				if (this.isInitialPosition && this.InProgress(this.Current))
				{
					this.conflictTime = this.user.Now;
					return;
				}
				this.conflictTime = this.Current.StartTime;
			}

			private void SeekNext()
			{
				if (this.idx >= this.agenda.Count)
				{
					return;
				}
				ExDateTime startTime = ((CalendarNavigator.MeetingInfo)this.agenda[this.idx]).StartTime;
				while (++this.idx < this.agenda.Count)
				{
					CalendarNavigator.MeetingInfo meetingInfo = (CalendarNavigator.MeetingInfo)this.agenda[this.idx];
					if (this.isOnlyReadRemaining)
					{
						if (!this.IsOver(meetingInfo))
						{
							if (!this.isReadConflicts || meetingInfo.StartTime > startTime)
							{
								break;
							}
							if (meetingInfo.IsAllDayEvent)
							{
								break;
							}
						}
					}
					else if (!this.isReadConflicts || meetingInfo.StartTime > startTime || meetingInfo.IsAllDayEvent)
					{
						break;
					}
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "Next meeting of the day found is at index: {0}.", new object[]
				{
					this.idx
				});
			}

			private void SeekPrevious()
			{
				if (this.idx < 1)
				{
					return;
				}
				while (--this.idx > 0)
				{
					CalendarNavigator.MeetingInfo meetingInfo = (CalendarNavigator.MeetingInfo)this.agenda[this.idx];
					CalendarNavigator.MeetingInfo meetingInfo2 = (CalendarNavigator.MeetingInfo)this.agenda[this.idx - 1];
					if (!this.isReadConflicts || meetingInfo.StartTime > meetingInfo2.StartTime)
					{
						break;
					}
					if (meetingInfo.IsAllDayEvent)
					{
						return;
					}
				}
			}

			private bool IsOver(CalendarNavigator.MeetingInfo mi)
			{
				return mi.EndTime < this.user.Now;
			}

			private bool IsBetter(CalendarNavigator.MeetingInfo challenger, CalendarNavigator.MeetingInfo bestSoFar)
			{
				if (!this.IsOver(challenger))
				{
					if (bestSoFar == null)
					{
						return true;
					}
					if (challenger.StartTime < bestSoFar.StartTime || (challenger.StartTime > bestSoFar.StartTime && this.InProgress(challenger) && this.InProgress(bestSoFar)))
					{
						return !bestSoFar.IsAllDayEvent;
					}
				}
				return false;
			}

			private bool InProgress(CalendarNavigator.MeetingInfo mi)
			{
				return mi.StartTime <= this.user.Now && mi.EndTime > this.user.Now;
			}

			private ArrayList agenda;

			private int idx;

			private ArrayList conflicts;

			private int conflictIdx;

			private bool isInitialPosition;

			private bool isOnlyReadRemaining;

			private bool isReadConflicts;

			private ExDateTime conflictTime;

			private UMSubscriber user;

			private CalendarNavigator.MeetingInfo lastMeetingHeard;
		}
	}
}
