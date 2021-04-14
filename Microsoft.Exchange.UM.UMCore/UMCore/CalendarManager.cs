using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.MessageContent;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class CalendarManager : SendMessageManager
	{
		internal CalendarManager(ActivityManager manager, CalendarManager.ConfigClass config) : base(manager, config)
		{
		}

		internal CalendarNavigator Navigator
		{
			get
			{
				if (this.nav == null)
				{
					this.nav = new CalendarNavigator(this.user);
				}
				return this.nav;
			}
		}

		internal override bool LargeGrammarsNeeded
		{
			get
			{
				return true;
			}
		}

		internal CalendarNavigator.AgendaContext Context
		{
			get
			{
				return this.agendaCtx;
			}
			set
			{
				this.agendaCtx = value;
			}
		}

		internal override void Start(BaseUMCallSession vo, string refInfo)
		{
			vo.IncrementCounter(SubscriberAccessCounters.CalendarAccessed);
			base.Start(vo, refInfo);
		}

		internal override TransitionBase ExecuteAction(string action, BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "Calendar Manager asked to do action: {0}.", new object[]
			{
				action
			});
			this.user = vo.CurrentCallContext.CallerInfo;
			string input = null;
			if (string.Equals(action, "getTodaysMeetings", StringComparison.OrdinalIgnoreCase))
			{
				this.targetDate = this.user.Now.Date;
				input = this.OpenCalendarDate(vo);
			}
			else if (string.Equals(action, "nextMeeting", StringComparison.OrdinalIgnoreCase))
			{
				input = this.NextMeeting(vo);
			}
			else if (string.Equals(action, "nextDay", StringComparison.OrdinalIgnoreCase))
			{
				input = this.NextDay(vo);
			}
			else if (string.Equals(action, "previousMeeting", StringComparison.OrdinalIgnoreCase))
			{
				input = this.PreviousMeeting(vo);
			}
			else if (string.Equals(action, "getDetails", StringComparison.OrdinalIgnoreCase))
			{
				input = this.GetDetails();
			}
			else if (string.Equals(action, "getParticipants", StringComparison.OrdinalIgnoreCase))
			{
				input = this.GetParticipants();
			}
			else if (string.Equals(action, "lateForMeeting", StringComparison.OrdinalIgnoreCase))
			{
				input = this.LateForMeeting();
			}
			else if (string.Equals(action, "cancelOrDecline", StringComparison.OrdinalIgnoreCase))
			{
				CalendarNavigator.MeetingInfo meetingInfo = (this.Context.ConflictCount > 0) ? this.Context.CurrentConflict : this.Context.Current;
				if (!meetingInfo.IsOrganizer)
				{
					vo.IncrementCounter(SubscriberAccessCounters.MeetingsDeclined);
				}
				base.SendMsg = new CalendarManager.CancelOrDecline(vo, this.user, meetingInfo, this);
				base.WriteReplyIntroType(IntroType.Cancel);
			}
			else if (string.Equals(action, "cancelSeveral", StringComparison.OrdinalIgnoreCase))
			{
				CalendarNavigator.MeetingInfo mi = (this.Context.ConflictCount > 0) ? this.Context.CurrentConflict : this.Context.Current;
				ExDateTime startTimeToClearFrom = this.GetStartTimeToClearFrom(mi);
				base.SendMsg = new CalendarManager.CancelSeveralMeetings(vo, this.user, startTimeToClearFrom, this, true);
				base.WriteReplyIntroType(IntroType.ClearCalendar);
			}
			else if (string.Equals(action, "replyToOrganizer", StringComparison.OrdinalIgnoreCase))
			{
				CalendarNavigator.MeetingInfo mi2 = (this.Context.ConflictCount > 0) ? this.Context.CurrentConflict : this.Context.Current;
				base.SendMsg = new CalendarManager.ReplyToOrganizer(vo, this.user, this, mi2);
				base.WriteReplyIntroType(IntroType.Reply);
			}
			else if (string.Equals(action, "replyToAll", StringComparison.OrdinalIgnoreCase))
			{
				CalendarNavigator.MeetingInfo mi3 = (this.Context.ConflictCount > 0) ? this.Context.CurrentConflict : this.Context.Current;
				base.SendMsg = new CalendarManager.ReplyToAll(vo, this.user, this, mi3);
				base.WriteReplyIntroType(IntroType.ReplyAll);
			}
			else if (string.Equals(action, "forward", StringComparison.OrdinalIgnoreCase))
			{
				CalendarNavigator.MeetingInfo mi4 = (this.Context.ConflictCount > 0) ? this.Context.CurrentConflict : this.Context.Current;
				base.SendMsg = new CalendarManager.CalendarForward(vo, this.user, this, mi4);
				base.WriteReplyIntroType(IntroType.Forward);
			}
			else if (string.Equals(action, "giveShortcutHint", StringComparison.OrdinalIgnoreCase))
			{
				base.WriteVariable("giveShortcutHint", true);
			}
			else if (string.Equals(action, "parseDateSpeech", StringComparison.OrdinalIgnoreCase))
			{
				this.ParseDateSpeech();
			}
			else if (string.Equals(action, "openCalendarDate", StringComparison.OrdinalIgnoreCase))
			{
				input = this.OpenCalendarDate(vo);
			}
			else if (string.Equals(action, "nextMeetingSameDay", StringComparison.OrdinalIgnoreCase))
			{
				input = this.NextMeetingSameDay(vo);
			}
			else if (string.Equals(action, "previousMeetingSameDay", StringComparison.OrdinalIgnoreCase))
			{
				input = this.PreviousMeetingSameDay(vo);
			}
			else if (string.Equals(action, "firstMeetingSameDay", StringComparison.OrdinalIgnoreCase))
			{
				input = this.FirstMeetingSameDay(vo);
			}
			else if (string.Equals(action, "lastMeetingSameDay", StringComparison.OrdinalIgnoreCase))
			{
				input = this.LastMeetingSameDay(vo);
			}
			else if (string.Equals(action, "acceptMeeting", StringComparison.OrdinalIgnoreCase))
			{
				this.AcceptMeeting();
			}
			else if (string.Equals(action, "markAsTentative", StringComparison.OrdinalIgnoreCase))
			{
				this.MarkAsTentative();
			}
			else if (string.Equals(action, "seekValidMeeting", StringComparison.OrdinalIgnoreCase))
			{
				input = this.SeekValidMeeting(vo);
			}
			else if (string.Equals(action, "isValidMeeting", StringComparison.OrdinalIgnoreCase))
			{
				if (!this.Context.IsValid)
				{
					input = "noMeetings";
				}
				this.WriteMeetingVariables(vo);
			}
			else if (string.Equals(action, "skipHeader", StringComparison.OrdinalIgnoreCase))
			{
				base.WriteVariable("skipHeader", true);
			}
			else if (string.Equals(action, "more", StringComparison.OrdinalIgnoreCase))
			{
				base.WriteVariable("skipHeader", true);
				base.ExecuteAction(action, vo);
			}
			else if (string.Equals(action, "readTheHeader", StringComparison.OrdinalIgnoreCase))
			{
				base.WriteVariable("skipHeader", false);
				base.WriteVariable("repeat", true);
				base.WriteVariable("more", false);
			}
			else if (string.Equals(action, "clearMinutesLate", StringComparison.OrdinalIgnoreCase))
			{
				base.WriteVariable("minutesLateMax", 0);
				base.WriteVariable("minutesLateMin", 0);
			}
			else if (string.Equals(action, "parseLateMinutes", StringComparison.OrdinalIgnoreCase))
			{
				this.ParseLateMinutes();
			}
			else if (string.Equals(action, "parseClearTimeDays", StringComparison.OrdinalIgnoreCase))
			{
				input = this.ParseClearTimeDays();
			}
			else if (string.Equals(action, "parseClearHours", StringComparison.OrdinalIgnoreCase))
			{
				this.ParseClearHours();
			}
			else if (string.Equals(action, "giveLateMinutesHint", StringComparison.OrdinalIgnoreCase))
			{
				base.WriteVariable("giveMinutesLateHint", true);
			}
			else if (string.Equals(action, "callOrganizer", StringComparison.OrdinalIgnoreCase))
			{
				vo.IncrementCounter(SubscriberAccessCounters.CalledMeetingOrganizer);
			}
			else if (string.Equals(action, "selectLanguage", StringComparison.OrdinalIgnoreCase))
			{
				input = base.SelectLanguage();
			}
			else
			{
				if (!string.Equals(action, "nextLanguage", StringComparison.OrdinalIgnoreCase))
				{
					return base.ExecuteAction(action, vo);
				}
				input = base.NextLanguage(vo);
			}
			return base.CurrentActivity.GetTransition(input);
		}

		internal string EndOfAgenda()
		{
			string result = null;
			CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "No more meetings today.", new object[0]);
			if (!this.Navigator.SeekNext())
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "No more meetings within seek threshold.", new object[0]);
				result = "emptyCalendar";
			}
			else
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "creating new agenda context.", new object[0]);
				this.Context = new CalendarNavigator.AgendaContext(this.Navigator.CurrentAgenda, this.user, false, false);
			}
			return result;
		}

		protected override string SendMessage(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "CalendarManager::SendMessage.", new object[0]);
			string result = base.SendMessage(vo);
			IntroType messageIntroType = base.MessageIntroType;
			this.WriteMeetingVariables(vo);
			base.WriteReplyIntroType(messageIntroType);
			return result;
		}

		private string SeekValidMeeting(BaseUMCallSession vo)
		{
			string result = null;
			ExDateTime currentDay = this.Navigator.CurrentDay;
			if (!this.Context.IsValid)
			{
				result = this.EndOfAgenda();
			}
			this.WriteMeetingVariables(vo);
			base.WriteVariable("dateChanged", currentDay.Date != this.Navigator.CurrentDay.Date);
			return result;
		}

		private string PreviousMeeting(BaseUMCallSession vo)
		{
			string result = null;
			ExDateTime currentDay = this.Navigator.CurrentDay;
			if (!this.Context.Previous())
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "CalendarManager.Previous called with no previous meeting.", new object[0]);
				this.Navigator.Previous();
				this.Context = new CalendarNavigator.AgendaContext(this.Navigator.CurrentAgenda, this.user, false, false);
				if (!this.Context.SeekLast())
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "Context.SeekLast returns false.  Firing autoevent NoMeetings.", new object[0]);
					result = "noMeetings";
				}
			}
			else if (this.Context.Current.StartTime.Date != this.Navigator.CurrentDay.Date)
			{
				this.Navigator.Goto(this.Context.Current.StartTime.Date);
			}
			base.WriteVariable("dateChanged", currentDay.Date != this.Navigator.CurrentDay.Date);
			this.WriteMeetingVariables(vo);
			return result;
		}

		private string NextDay(BaseUMCallSession vo)
		{
			string result = null;
			this.Navigator.Next();
			this.Context = new CalendarNavigator.AgendaContext(this.Navigator.CurrentAgenda, this.user, false, false);
			if (0 < this.Context.TotalCount)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "CalendarManager.NextDay found remaining={0} meetings on day={1}.", new object[]
				{
					this.Context.Remaining,
					this.Navigator.CurrentDay
				});
			}
			else
			{
				result = "noMeetings";
			}
			base.WriteVariable("dateChanged", true);
			this.WriteMeetingVariables(vo);
			return result;
		}

		private string NextMeeting(BaseUMCallSession vo)
		{
			string result = null;
			ExDateTime currentDay = this.Navigator.CurrentDay;
			if (!this.Context.Next())
			{
				result = this.EndOfAgenda();
			}
			else
			{
				vo.IncrementCounter(SubscriberAccessCounters.CalendarItemsHeard);
			}
			base.WriteVariable("dateChanged", currentDay.Date != this.Navigator.CurrentDay.Date);
			this.WriteMeetingVariables(vo);
			return result;
		}

		private string PreviousMeetingSameDay(BaseUMCallSession vo)
		{
			string result = null;
			StoreObjectId uniqueId = this.Context.Current.UniqueId;
			this.Context.Previous();
			if (!uniqueId.Equals(this.Context.Current.UniqueId))
			{
				vo.IncrementCounter(SubscriberAccessCounters.CalendarItemsHeard);
				this.WriteMeetingVariables(vo);
			}
			else
			{
				result = "noMeetings";
			}
			return result;
		}

		private string FirstMeetingSameDay(BaseUMCallSession vo)
		{
			string result = null;
			StoreObjectId uniqueId = this.Context.Current.UniqueId;
			this.Context.SeekFirst();
			if (!uniqueId.Equals(this.Context.Current.UniqueId))
			{
				vo.IncrementCounter(SubscriberAccessCounters.CalendarItemsHeard);
				this.WriteMeetingVariables(vo);
			}
			else
			{
				result = "noMeetings";
			}
			return result;
		}

		private string LastMeetingSameDay(BaseUMCallSession vo)
		{
			string result = null;
			StoreObjectId uniqueId = this.Context.Current.UniqueId;
			this.Context.SeekLast();
			if (!uniqueId.Equals(this.Context.Current.UniqueId))
			{
				vo.IncrementCounter(SubscriberAccessCounters.CalendarItemsHeard);
				this.WriteMeetingVariables(vo);
			}
			else
			{
				result = "noMeetings";
			}
			return result;
		}

		private string NextMeetingSameDay(BaseUMCallSession vo)
		{
			string result = null;
			if (!this.Context.Next(true))
			{
				result = "noMeetings";
			}
			else
			{
				vo.IncrementCounter(SubscriberAccessCounters.CalendarItemsHeard);
			}
			this.WriteMeetingVariables(vo);
			return result;
		}

		private void WriteMeetingVariables(BaseUMCallSession vo)
		{
			ExDateTime now = this.user.Now;
			ExDateTime date = now.Date;
			base.WriteVariable("skipHeader", false);
			base.WriteReplyIntroType(IntroType.None);
			base.WriteVariable("remaining", this.Context.Remaining);
			base.WriteVariable("calendarDate", this.Navigator.CurrentDay);
			base.WriteVariable("numConflicts", this.Context.ConflictCount);
			base.WriteVariable("current", null);
			base.WriteVariable("calendarDate", this.Navigator.CurrentDay);
			base.WriteVariable("dayOfWeek", (int)this.Navigator.CurrentDay.DayOfWeek);
			base.WriteVariable("dayOffset", (this.Navigator.CurrentDay - date).Days);
			base.WriteVariable("messageLanguage", null);
			base.WriteVariable("languageDetected", null);
			if (this.Context.IsValid)
			{
				CalendarNavigator.MeetingInfo meetingInfo = (this.Context.ConflictCount > 0) ? this.Context.CurrentConflict : this.Context.Current;
				base.WriteVariable("current", meetingInfo);
				base.MessagePlayerContext.Reset(meetingInfo.UniqueId);
				base.WriteVariable("startTime", meetingInfo.StartTime);
				base.WriteVariable("endTime", meetingInfo.EndTime);
				base.WriteVariable("meetingTimeRange", new TimeRange(meetingInfo.StartTime, meetingInfo.EndTime));
				base.WriteVariable("time", meetingInfo.StartTime);
				base.WriteVariable("subject", meetingInfo.Subject);
				base.WriteVariable("location", meetingInfo.Location);
				bool flag = meetingInfo.StartTime.Date == date;
				bool flag2 = meetingInfo.EndTime < now;
				bool flag3 = meetingInfo.StartTime > now;
				base.WriteVariable("today", flag);
				base.WriteVariable("past", flag2);
				base.WriteVariable("future", flag3);
				base.WriteVariable("present", !flag2 && !flag3);
				base.WriteVariable("first", this.Context.IsFirst);
				base.WriteVariable("firstConflict", this.Context.IsFirstConflict);
				base.WriteVariable("middle", !this.Context.IsFirst && !this.Context.IsLast);
				base.WriteVariable("last", this.Context.IsLast);
				base.WriteVariable("initial", this.Context.IsInitialPosition);
				base.WriteVariable("tentative", meetingInfo.FreeBusyStatus == BusyType.Tentative);
				base.WriteVariable("owner", meetingInfo.IsOrganizer);
				base.WriteVariable("conflictTime", this.Context.ConflictTime);
				base.WriteVariable("isAllDayEvent", meetingInfo.IsAllDayEvent);
				base.WriteVariable("organizerPhone", meetingInfo.OrganizerPhone);
				base.WriteVariable("locationPhone", meetingInfo.LocationPhone);
				base.WriteVariable("isMeeting", meetingInfo.IsMeeting);
				base.WriteVariable("conflictWithLastHeard", this.Context.ConflictsWithLastHeard);
			}
		}

		private string GetDetails()
		{
			string result = null;
			CalendarNavigator.MeetingInfo meetingInfo = (this.Context.ConflictCount > 0) ? this.Context.CurrentConflict : this.Context.Current;
			base.CallSession.IncrementCounter(SubscriberAccessCounters.CalendarItemsDetailsRequested);
			if (meetingInfo.IsOrganizer)
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				foreach (CalendarNavigator.AttendeeInfo attendeeInfo in meetingInfo.Attendees)
				{
					switch (attendeeInfo.ResponseType)
					{
					case ResponseType.None:
					case ResponseType.Tentative:
					case ResponseType.NotResponded:
						num3++;
						break;
					case ResponseType.Accept:
						num++;
						break;
					case ResponseType.Decline:
						num2++;
						break;
					}
				}
				base.WriteVariable("numAccepted", num);
				base.WriteVariable("numDeclined", num2);
				base.WriteVariable("numUndecided", num3);
			}
			else
			{
				base.WriteVariable("ownerName", meetingInfo.OrganizerName);
				base.WriteVariable("numAttendees", meetingInfo.Attendees.Count);
			}
			return result;
		}

		private string GetParticipants()
		{
			string result = null;
			CalendarNavigator.MeetingInfo meetingInfo = (this.Context.ConflictCount > 0) ? this.Context.CurrentConflict : this.Context.Current;
			if (meetingInfo.IsOrganizer)
			{
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = new StringBuilder();
				StringBuilder stringBuilder3 = new StringBuilder();
				foreach (CalendarNavigator.AttendeeInfo attendeeInfo in meetingInfo.Attendees)
				{
					switch (attendeeInfo.ResponseType)
					{
					case ResponseType.None:
					case ResponseType.Tentative:
					case ResponseType.NotResponded:
						stringBuilder3.Append(attendeeInfo.Participant.DisplayName);
						stringBuilder3.Append(", ");
						break;
					case ResponseType.Accept:
						stringBuilder.Append(attendeeInfo.Participant.DisplayName);
						stringBuilder.Append(", ");
						break;
					case ResponseType.Decline:
						stringBuilder2.Append(attendeeInfo.Participant.DisplayName);
						stringBuilder2.Append(", ");
						break;
					}
				}
				base.WriteVariable("acceptedList", (stringBuilder.Length == 0) ? null : stringBuilder.ToString());
				base.WriteVariable("declinedList", (stringBuilder2.Length == 0) ? null : stringBuilder2.ToString());
				base.WriteVariable("undecidedList", (stringBuilder3.Length == 0) ? null : stringBuilder3.ToString());
			}
			else
			{
				base.WriteVariable("ownerName", meetingInfo.OrganizerName);
				StringBuilder stringBuilder4 = new StringBuilder();
				foreach (CalendarNavigator.AttendeeInfo attendeeInfo2 in meetingInfo.Attendees)
				{
					stringBuilder4.Append(attendeeInfo2.Participant.DisplayName);
					stringBuilder4.Append(", ");
				}
				base.WriteVariable("attendeeList", (stringBuilder4.Length == 0) ? null : stringBuilder4.ToString());
			}
			return result;
		}

		private string LateForMeeting()
		{
			string result = null;
			CalendarNavigator.MeetingInfo meetingInfo = (this.Context.ConflictCount > 0) ? this.Context.CurrentConflict : this.Context.Current;
			int num = (int)(this.ReadVariable("minutesLateMin") ?? 0);
			int num2 = (int)(this.ReadVariable("minutesLateMax") ?? 0);
			if (base.NumericInput != 0)
			{
				num = 0;
				num2 = base.NumericInput;
			}
			base.WriteVariable("minutesLateMin", num);
			base.WriteVariable("minutesLateMax", num2);
			CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "Sending I'll Be Late message with minutesLateMax={0}, minutesLateMin={1}.", new object[]
			{
				num2,
				num
			});
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.user.CreateSessionLock())
			{
				using (CalendarItemBase calendarItemBase = CalendarItemBase.Bind(mailboxSessionLock.Session, meetingInfo.UniqueId))
				{
					using (MessageItem messageItem = calendarItemBase.CreateReplyAll(XsoUtil.GetDraftsFolderId(mailboxSessionLock.Session), new ReplyForwardConfiguration(BodyFormat.TextHtml)))
					{
						using (TextWriter textWriter = messageItem.Body.OpenTextWriter(BodyFormat.TextHtml))
						{
							textWriter.Write(this.BuildLateMessageBody(num, num2, calendarItemBase));
						}
						messageItem.Send();
					}
				}
			}
			base.CallSession.IncrementCounter(SubscriberAccessCounters.CalendarLateAttendance);
			PIIMessage data = PIIMessage.Create(PIIType._User, this.user);
			CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, data, "Succesfully sent I'll be late message for user=_User.", new object[0]);
			return result;
		}

		private string BuildLateMessageBody(int minutesLateMin, int minutesLateMax, CalendarItemBase cal)
		{
			MessageContentBuilder messageContentBuilder = MessageContentBuilder.Create(this.user.TelephonyCulture);
			LocalizedString lateInfo;
			if (Regex.Match(minutesLateMax.ToString(this.user.TelephonyCulture), PromptConfigBase.PromptResourceManager.GetString("Singular", this.user.TelephonyCulture)).Success)
			{
				lateInfo = ((minutesLateMin > 0) ? Strings.LateForMeetingRange_Singular(minutesLateMin, minutesLateMax) : Strings.LateForMeeting_Singular(minutesLateMax));
			}
			else if (Regex.Match(minutesLateMax.ToString(this.user.TelephonyCulture), PromptConfigBase.PromptResourceManager.GetString("Plural", this.user.TelephonyCulture)).Success)
			{
				lateInfo = ((minutesLateMin > 0) ? Strings.LateForMeetingRange_Plural(minutesLateMin, minutesLateMax) : Strings.LateForMeeting_Plural(minutesLateMax));
			}
			else if (Regex.Match(minutesLateMax.ToString(this.user.TelephonyCulture), PromptConfigBase.PromptResourceManager.GetString("Plural2", this.user.TelephonyCulture)).Success)
			{
				lateInfo = ((minutesLateMin > 0) ? Strings.LateForMeetingRange_Plural2(minutesLateMin, minutesLateMax) : Strings.LateForMeeting_Plural2(minutesLateMax));
			}
			else
			{
				lateInfo = ((minutesLateMin > 0) ? Strings.LateForMeetingRange_Zero(minutesLateMin, minutesLateMax) : Strings.LateForMeeting_Zero(minutesLateMax));
			}
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.user.CreateSessionLock())
			{
				messageContentBuilder.AddLateForMeetingBody(cal, mailboxSessionLock.Session.ExTimeZone, lateInfo);
			}
			return messageContentBuilder.ToString();
		}

		private void ParseDateSpeech()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "CalendarManager::ParseDateSpeech.", new object[0]);
			if (!(base.RecoResult["RelativeDayOffset"] is string))
			{
				this.ParseAbsoluteDateSpeech();
			}
			else
			{
				this.ParseRelativeDateSpeech();
			}
			base.WriteVariable("calendarDate", this.targetDate);
			base.WriteVariable("dayOfWeek", (int)this.targetDate.DayOfWeek);
			base.WriteVariable("dayOffset", (this.targetDate - this.user.Now.Date).Days);
		}

		private void ParseAbsoluteDateSpeech()
		{
			int num = int.Parse((string)base.RecoResult["Year"], CultureInfo.InvariantCulture);
			int num2 = int.Parse((string)base.RecoResult["Month"], CultureInfo.InvariantCulture);
			int num3 = int.Parse((string)base.RecoResult["Day"], CultureInfo.InvariantCulture);
			CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "Setting absolute date with year={0}, month={1}, day={2}.", new object[]
			{
				num,
				num2,
				num3
			});
			this.targetDate = new ExDateTime(this.user.TimeZone, num, num2, num3);
		}

		private void ParseRelativeDateSpeech()
		{
			string text = (string)base.RecoResult["RelativeDayOffset"];
			ExDateTime date = this.user.Now.Date;
			int num;
			if (!string.IsNullOrEmpty(text))
			{
				num = int.Parse(text, CultureInfo.InvariantCulture);
			}
			else
			{
				int num2 = int.Parse((string)base.RecoResult["SpokenDay"], CultureInfo.InvariantCulture);
				int dayOfWeek = (int)date.DayOfWeek;
				if (dayOfWeek < num2)
				{
					num = num2 - dayOfWeek;
				}
				else
				{
					num = 7 + (num2 - dayOfWeek);
				}
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "Setting relative day with dateOffset={0}.", new object[]
			{
				num
			});
			this.targetDate = date.AddDays((double)num);
		}

		private string OpenCalendarDate(BaseUMCallSession vo)
		{
			string result = null;
			ExDateTime date = this.user.Now.Date;
			this.Navigator.Goto(this.targetDate);
			this.Context = new CalendarNavigator.AgendaContext(this.Navigator.CurrentAgenda, this.user, this.targetDate.Date == date, false);
			if ((date == this.targetDate && 0 < this.Context.Remaining) || (date != this.targetDate && 0 < this.Context.TotalCount))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "CalendarManager.OpenCalendarDate found remaining={0} meetings on day={1}.", new object[]
				{
					this.Context.Remaining,
					this.Navigator.CurrentDay
				});
			}
			else
			{
				result = "noMeetings";
			}
			this.WriteMeetingVariables(vo);
			return result;
		}

		private void AcceptMeeting()
		{
			CalendarNavigator.MeetingInfo meetingInfo = (this.Context.ConflictCount > 0) ? this.Context.CurrentConflict : this.Context.Current;
			meetingInfo.AcceptMeeting();
			base.CallSession.IncrementCounter(SubscriberAccessCounters.MeetingsAccepted);
		}

		private void MarkAsTentative()
		{
			CalendarNavigator.MeetingInfo meetingInfo = (this.Context.ConflictCount > 0) ? this.Context.CurrentConflict : this.Context.Current;
			meetingInfo.MarkAsTentative();
		}

		private void ParseLateMinutes()
		{
			base.WriteVariable("minutesLateMax", 0);
			base.WriteVariable("minutesLateMin", 0);
			string text = base.RecoResult["RangeStart"] as string;
			string text2 = base.RecoResult["RangeEnd"] as string;
			string text3 = base.RecoResult["Minutes"] as string;
			int num = 0;
			if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
			{
				int val = int.Parse(text, CultureInfo.InvariantCulture);
				int val2 = int.Parse(text2, CultureInfo.InvariantCulture);
				num = Math.Max(val, val2);
				int num2 = Math.Min(val, val2);
				base.WriteVariable("minutesLateMin", num2);
			}
			else if (!string.IsNullOrEmpty(text3))
			{
				num = int.Parse(text3, CultureInfo.InvariantCulture);
			}
			base.WriteVariable("minutesLateMax", num);
			if (string.Compare(base.LastRecoEvent, "recoSendLateMessageMinutes", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(base.LastRecoEvent, "recoNotSure", StringComparison.OrdinalIgnoreCase) == 0)
			{
				base.WriteVariable("giveMinutesLateHint", false);
			}
		}

		private ExDateTime ParseClearTime(IUMRecognitionResult result)
		{
			int num = int.Parse((string)result["Hour"], CultureInfo.InvariantCulture);
			int num2 = int.Parse((string)result["Minute"], CultureInfo.InvariantCulture);
			int num3 = num;
			if (!int.TryParse((string)result["AlternateHour"], out num3))
			{
				num3 = num;
			}
			CalendarNavigator.MeetingInfo mi = (this.Context.ConflictCount > 0) ? this.Context.CurrentConflict : this.Context.Current;
			ExDateTime startTimeToClearFrom = this.GetStartTimeToClearFrom(mi);
			if (num < startTimeToClearFrom.Hour)
			{
				num = num3;
			}
			return startTimeToClearFrom.Date.AddHours((double)num).AddMinutes((double)num2);
		}

		private ExDateTime ParseClearDays(IUMRecognitionResult result)
		{
			int num = int.Parse((string)result["Days"], CultureInfo.InvariantCulture);
			CalendarNavigator.MeetingInfo mi = (this.Context.ConflictCount > 0) ? this.Context.CurrentConflict : this.Context.Current;
			ExDateTime result2 = this.GetStartTimeToClearFrom(mi).Date.AddDays((double)num).AddSeconds(-1.0);
			base.WriteVariable("clearDays", num);
			return result2;
		}

		private string ParseClearTimeDays()
		{
			ExDateTime exDateTime = ExDateTime.MaxValue;
			string result = null;
			string lastRecoEvent;
			if ((lastRecoEvent = base.LastRecoEvent) != null)
			{
				if (!(lastRecoEvent == "recoTimePhrase"))
				{
					if (!(lastRecoEvent == "recoDayPhrase"))
					{
						if (!(lastRecoEvent == "recoAmbiguousPhrase"))
						{
							if (!(lastRecoEvent == "recoTimeOfDay"))
							{
								if (lastRecoEvent == "recoNumberOfDays")
								{
									exDateTime = this.ambiguousDay;
								}
							}
							else
							{
								exDateTime = this.ambiguousTime;
							}
						}
						else
						{
							this.ambiguousTime = this.ParseClearTime(base.RecoResult);
							this.ambiguousDay = this.ParseClearDays(base.RecoResult);
						}
					}
					else
					{
						exDateTime = this.ParseClearDays(base.RecoResult);
					}
				}
				else
				{
					exDateTime = this.ParseClearTime(base.RecoResult);
				}
			}
			if (ExDateTime.MaxValue != exDateTime)
			{
				base.WriteVariable("clearTime", exDateTime);
				CalendarNavigator.MeetingInfo meetingInfo = (this.Context.ConflictCount > 0) ? this.Context.CurrentConflict : this.Context.Current;
				if (exDateTime < meetingInfo.StartTime)
				{
					result = "invalidTime";
				}
			}
			return result;
		}

		private void ParseClearHours()
		{
			int numericInput = base.NumericInput;
			CalendarNavigator.MeetingInfo meetingInfo = (this.Context.ConflictCount > 0) ? this.Context.CurrentConflict : this.Context.Current;
			ExDateTime exDateTime = meetingInfo.StartTime.AddHours((double)numericInput);
			base.WriteVariable("clearTime", exDateTime);
		}

		private ExDateTime GetStartTimeToClearFrom(CalendarNavigator.MeetingInfo mi)
		{
			ExDateTime exDateTime = (ExDateTime)this.ReadVariable("calendarDate");
			if (!(mi.StartTime > exDateTime))
			{
				return exDateTime;
			}
			return mi.StartTime;
		}

		private const int MaxDigitsInHours = 3;

		private CalendarNavigator nav;

		private CalendarNavigator.AgendaContext agendaCtx;

		private UMSubscriber user;

		private ExDateTime targetDate;

		private ExDateTime ambiguousTime;

		private ExDateTime ambiguousDay;

		internal class ConfigClass : ActivityManagerConfig
		{
			internal ConfigClass(ActivityManagerConfig manager) : base(manager)
			{
			}

			internal override ActivityManager CreateActivityManager(ActivityManager manager)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Constructing CalendarManager", new object[0]);
				return new CalendarManager(manager, this);
			}
		}

		internal abstract class ReplyToBase : XsoRecordedMessage
		{
			internal ReplyToBase(BaseUMCallSession vo, UMSubscriber user, CalendarManager manager, CalendarNavigator.MeetingInfo mi) : base(vo, user, manager)
			{
				this.mi = mi;
			}

			public override void DoPostSubmit()
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "ReplyToBase::DoPostSubmit.", new object[0]);
				base.Session.IncrementCounter(SubscriberAccessCounters.ReplyMessagesSent);
				base.DoPostSubmit();
			}

			protected override MessageItem GenerateMessage(MailboxSession session)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "ReplyToBase::GenerateMessage.", new object[0]);
				MessageItem result;
				using (CalendarItemBase calendarItemBase = CalendarItemBase.Bind(session, this.mi.UniqueId))
				{
					base.SetAttachmentName(calendarItemBase.AttachmentCollection);
					MessageItem messageItem = this.CreateReplyMessage(calendarItemBase, base.PrepareMessageBodyPrefix(calendarItemBase), BodyFormat.TextHtml, XsoUtil.GetDraftsFolderId(session));
					result = messageItem;
				}
				return result;
			}

			protected abstract MessageItem CreateReplyMessage(CalendarItemBase cal, string bodyPrefix, BodyFormat bodyFormat, StoreObjectId parentFolderId);

			protected override void AddRecordedMessageText(MessageContentBuilder content)
			{
				content.AddRecordedReplyText(base.User.DisplayName);
			}

			protected override void AddMessageHeader(Item originalMessage, MessageContentBuilder content)
			{
				using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = base.User.CreateSessionLock())
				{
					content.AddCalendarHeader((CalendarItemBase)originalMessage, mailboxSessionLock.Session.ExTimeZone, true);
				}
			}

			private CalendarNavigator.MeetingInfo mi;
		}

		internal class ReplyToOrganizer : CalendarManager.ReplyToBase
		{
			internal ReplyToOrganizer(BaseUMCallSession vo, UMSubscriber user, CalendarManager manager, CalendarNavigator.MeetingInfo mi) : base(vo, user, manager, mi)
			{
			}

			public override void DoPostSubmit()
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "ReplyToOrganizer::DoPostSubmit.", new object[0]);
				base.Session.IncrementCounter(SubscriberAccessCounters.RepliedToOrganizer);
				base.DoPostSubmit();
			}

			protected override MessageItem CreateReplyMessage(CalendarItemBase cal, string bodyPrefix, BodyFormat bodyFormat, StoreObjectId parentFolderId)
			{
				ReplyForwardConfiguration replyForwardConfiguration = new ReplyForwardConfiguration(bodyFormat);
				replyForwardConfiguration.AddBodyPrefix(bodyPrefix);
				return cal.CreateReply(parentFolderId, replyForwardConfiguration);
			}
		}

		internal class ReplyToAll : CalendarManager.ReplyToBase
		{
			internal ReplyToAll(BaseUMCallSession vo, UMSubscriber user, CalendarManager manager, CalendarNavigator.MeetingInfo mi) : base(vo, user, manager, mi)
			{
			}

			protected override MessageItem CreateReplyMessage(CalendarItemBase cal, string bodyPrefix, BodyFormat bodyFormat, StoreObjectId parentFolderId)
			{
				ReplyForwardConfiguration replyForwardConfiguration = new ReplyForwardConfiguration(bodyFormat);
				replyForwardConfiguration.AddBodyPrefix(bodyPrefix);
				return cal.CreateReplyAll(parentFolderId, replyForwardConfiguration);
			}
		}

		internal class CalendarForward : XsoRecordedMessage
		{
			internal CalendarForward(BaseUMCallSession vo, UMSubscriber user, CalendarManager manager, CalendarNavigator.MeetingInfo mi) : base(vo, user, manager)
			{
				this.mi = mi;
			}

			protected override MessageItem GenerateMessage(MailboxSession session)
			{
				MessageItem result;
				using (CalendarItemBase calendarItemBase = CalendarItemBase.Bind(session, this.mi.UniqueId))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "CalendarForward::GenerateResponse.", new object[0]);
					base.SetAttachmentName(calendarItemBase.AttachmentCollection);
					ReplyForwardConfiguration replyForwardParameters = new ReplyForwardConfiguration(BodyFormat.TextHtml);
					result = calendarItemBase.CreateForward(XsoUtil.GetDraftsFolderId(session), replyForwardParameters);
				}
				return result;
			}

			private CalendarNavigator.MeetingInfo mi;
		}

		internal class CancelOrDecline : XsoRecordedMessage
		{
			internal CancelOrDecline(BaseUMCallSession vo, UMSubscriber user, CalendarNavigator.MeetingInfo mi, CalendarManager cm, bool autoPostSubmit) : base(vo, user, cm, autoPostSubmit)
			{
				this.mi = mi;
			}

			internal CancelOrDecline(BaseUMCallSession vo, UMSubscriber user, CalendarNavigator.MeetingInfo mi, CalendarManager cm) : this(vo, user, mi, cm, true)
			{
			}

			public override void DoPostSubmit()
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "CancelOrDecline::DoPostSubmit.", new object[0]);
				CalendarManager calendarManager = (CalendarManager)base.Manager;
				calendarManager.Context.Remove(this.mi.UniqueId);
				UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = null;
				try
				{
					mailboxSessionLock = base.User.CreateSessionLock();
					mailboxSessionLock.Session.Delete(DeleteItemFlags.MoveToDeletedItems, new StoreId[]
					{
						this.mi.UniqueId
					});
					CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "CancelOrDecline::DoPostSubmit successfully deleted original calendar item.", new object[0]);
				}
				catch (StorageTransientException ex)
				{
					CallIdTracer.TraceError(ExTraceGlobals.CalendarTracer, this, "CancelOrDecline::DoPostSubmit failed to delete the calendar item! e={0}", new object[]
					{
						ex
					});
				}
				catch (StoragePermanentException ex2)
				{
					CallIdTracer.TraceError(ExTraceGlobals.CalendarTracer, this, "CancelOrDecline::DoPostSubmit failed to delete the calendar item! e={0}", new object[]
					{
						ex2
					});
				}
				finally
				{
					if (mailboxSessionLock != null)
					{
						mailboxSessionLock.Dispose();
					}
				}
				base.DoPostSubmit();
			}

			protected override MessageItem GenerateMessage(MailboxSession session)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "CancelOrDecline::GenerateMessage.", new object[0]);
				CalendarManager calendarManager = (CalendarManager)base.Manager;
				MessageItem messageItem = null;
				using (CalendarItemBase calendarItemBase = CalendarItemBase.Bind(session, this.mi.UniqueId))
				{
					calendarItemBase.OpenAsReadWrite();
					if (this.mi.IsOrganizer && this.mi.IsMeeting)
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "User is organizer...creating meeting cancellation.", new object[0]);
						messageItem = calendarItemBase.CancelMeeting(null, null);
					}
					else
					{
						if (this.mi.IsOrganizer)
						{
							CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "CancelMeeeting::GenerateMessage creating message for appointment.", new object[0]);
							using (calendarItemBase.CancelMeeting(null, null))
							{
								StoreObjectId draftsFolderId = XsoUtil.GetDraftsFolderId(session);
								messageItem = MessageItem.Create(session, draftsFolderId);
								messageItem.Subject = Strings.CancelledMeetingSubject((string)XsoUtil.SafeGetProperty(calendarItemBase, ItemSchema.NormalizedSubject, string.Empty)).ToString(base.User.TelephonyCulture);
								Participant participant = new Participant(base.User.DisplayName, base.User.ExchangeLegacyDN, "EX");
								messageItem.Recipients.Add(participant, RecipientItemType.To);
								goto IL_159;
							}
						}
						CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "User is NOT organizer...creating meeting decline.", new object[0]);
						messageItem = XsoUtil.RespondToMeetingRequest(calendarItemBase, ResponseType.Decline);
					}
					IL_159:
					calendarItemBase.Load(new PropertyDefinition[]
					{
						ItemSchema.HasAttachment
					});
					base.SetAttachmentName(calendarItemBase.AttachmentCollection);
				}
				return messageItem;
			}

			private CalendarNavigator.MeetingInfo mi;
		}

		internal class CancelSeveralMeetings : IRecordedMessage
		{
			internal CancelSeveralMeetings(BaseUMCallSession vo, UMSubscriber user, ExDateTime initialStartTime, CalendarManager cm, bool reserveTime)
			{
				this.vo = vo;
				this.meetingsToCancel = new List<CalendarNavigator.MeetingInfo>(16);
				this.cm = cm;
				this.startTime = initialStartTime;
				this.endTime = this.startTime;
				this.user = user;
				this.reserveTime = reserveTime;
			}

			public void DoSubmit(Importance imp, bool markPrivate, Stack<Participant> recips)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "CancelSeveralMeetings::DoSubmit.", new object[0]);
				ExAssert.RetailAssert(!markPrivate, "Calendar meetings cannot be marked private");
				this.BuildMeetingList();
				foreach (CalendarNavigator.MeetingInfo meetingInfo in this.meetingsToCancel)
				{
					if (!meetingInfo.IsCancelled)
					{
						CalendarManager.CancelOrDecline cancelOrDecline = new CalendarManager.CancelOrDecline(this.vo, this.user, meetingInfo, this.cm, false);
						cancelOrDecline.DoSubmit(imp, false, recips);
					}
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "CancelSeveralMeetings::DoPostSubmit.", new object[0]);
				this.DoPostSubmit();
			}

			public void DoSubmit(Importance imp)
			{
				this.DoSubmit(imp, false, null);
			}

			public void DoPostSubmit()
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "CancelSeveralMeetings::DoPostSubmit.", new object[0]);
				this.cm.Context.RemoveMeetings(this.meetingIdsToDelete);
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "Removed items between start={0} and end={1} from the agenda.", new object[]
				{
					this.startTime,
					this.endTime
				});
				using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.user.CreateSessionLock())
				{
					mailboxSessionLock.Session.Delete(DeleteItemFlags.MoveToDeletedItems, this.meetingIdsToDelete.ToArray());
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "Removed items between start={0} and end={1} from the store.", new object[]
				{
					this.startTime,
					this.endTime
				});
				if (this.reserveTime)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "CancelSeveralMeetings::DoPostSubmit::Creating a reserved time entry.", new object[0]);
					this.CreateReservedTimeEntry();
				}
				this.vo.IncrementCounter(SubscriberAccessCounters.VoiceMessagesSent, (long)this.meetingsToCancel.Count);
				this.cm.RecordContext.Reset();
			}

			private void BuildMeetingList()
			{
				this.meetingsToCancel.Clear();
				this.meetingIdsToDelete = null;
				this.endTime = (ExDateTime)this.cm.ReadVariable("clearTime");
				CallIdTracer.TraceDebug(ExTraceGlobals.CalendarTracer, this, "CancelSeveralMeetings::DoSubmit cancelling all meetings between {0} and {1}.", new object[]
				{
					this.startTime,
					this.endTime
				});
				using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.user.CreateSessionLock())
				{
					using (CalendarFolder calendarFolder = CalendarFolder.Bind(mailboxSessionLock.Session, mailboxSessionLock.Session.GetDefaultFolderId(DefaultFolderType.Calendar)))
					{
						object[][] calendarView = calendarFolder.GetCalendarView(this.startTime, this.endTime, new PropertyDefinition[]
						{
							ItemSchema.Id,
							CalendarItemInstanceSchema.StartTime,
							CalendarItemInstanceSchema.EndTime,
							CalendarItemBaseSchema.IsAllDayEvent,
							CalendarItemBaseSchema.AppointmentState
						});
						this.meetingIdsToDelete = new List<StoreObjectId>(calendarView.Length);
						for (int i = 0; i < calendarView.Length; i++)
						{
							ExDateTime t = (ExDateTime)calendarView[i][1];
							ExDateTime t2 = (ExDateTime)calendarView[i][2];
							if (t >= this.startTime && t2 <= this.endTime)
							{
								this.meetingsToCancel.Add(new CalendarNavigator.MeetingInfo(calendarView[i], this.user));
								this.meetingIdsToDelete.Add(((VersionedId)calendarView[i][0]).ObjectId);
							}
						}
					}
				}
			}

			private void CreateReservedTimeEntry()
			{
				using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.user.CreateSessionLock())
				{
					using (CalendarItem calendarItem = CalendarItem.Create(mailboxSessionLock.Session, mailboxSessionLock.Session.GetDefaultFolderId(DefaultFolderType.Calendar)))
					{
						LocalizedString localizedString = Strings.ReservedTimeBody(this.user.DisplayName, calendarItem.CreationTime.ToString(this.user.TelephonyCulture));
						calendarItem.StartTime = this.startTime;
						calendarItem.EndTime = this.endTime;
						calendarItem.FreeBusyStatus = BusyType.OOF;
						calendarItem.Reminder.IsSet = false;
						calendarItem.Subject = Strings.ReservedTimeTitle.ToString(this.user.TelephonyCulture);
						using (TextWriter textWriter = calendarItem.Body.OpenTextWriter(BodyFormat.TextPlain))
						{
							textWriter.Write(localizedString.ToString(this.user.TelephonyCulture));
						}
						calendarItem.Save(SaveMode.NoConflictResolution);
						calendarItem.Load(new PropertyDefinition[]
						{
							ItemSchema.Id
						});
						this.cm.Navigator.SkipMeeting(calendarItem.Id.ObjectId);
					}
				}
			}

			private List<CalendarNavigator.MeetingInfo> meetingsToCancel;

			private List<StoreObjectId> meetingIdsToDelete;

			private ExDateTime startTime;

			private ExDateTime endTime;

			private CalendarManager cm;

			private UMSubscriber user;

			private bool reserveTime;

			private BaseUMCallSession vo;
		}
	}
}
