using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RumAgent
	{
		private RumAgent()
		{
		}

		public static RumAgent Instance
		{
			get
			{
				RumAgent result;
				lock (RumAgent.threadSafetyLock)
				{
					if (RumAgent.instance == null)
					{
						RumAgent.instance = new RumAgent();
					}
					result = RumAgent.instance;
				}
				return result;
			}
		}

		public bool SendRums(RumInfo info, bool copyToSentItems, CalendarItemBase item, ref Dictionary<GlobalObjectId, List<Attendee>> organizerRumsSent)
		{
			bool result = false;
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (organizerRumsSent == null)
			{
				throw new ArgumentNullException("organizerRumsSent");
			}
			if (info.IsOccurrenceRum)
			{
				if (item is CalendarItemOccurrence)
				{
					ExDateTime originalStartTime = ((CalendarItemOccurrence)item).OriginalStartTime;
					result = this.SendRumsBase(info, copyToSentItems, item, ref organizerRumsSent);
				}
				else
				{
					if (!(item is CalendarItem))
					{
						throw new ArgumentException("The calendar item's type is unrecognized.", "item");
					}
					CalendarItem calendarItem = (CalendarItem)item;
					using (CalendarItemBase calendarItemBase = calendarItem.OpenOccurrenceByOriginalStartTime(info.OccurrenceOriginalStartTime.Value, new PropertyDefinition[0]))
					{
						calendarItemBase.OpenAsReadWrite();
						result = this.SendRumsBase(info, copyToSentItems, calendarItemBase, ref organizerRumsSent);
					}
					item.Load();
				}
			}
			else
			{
				result = this.SendRumsBase(info, copyToSentItems, item, ref organizerRumsSent);
			}
			return result;
		}

		public bool AttendeeListContainsParticipant(List<Attendee> list, Participant participant)
		{
			return this.AttendeeListContainsParticipant(list, participant, null);
		}

		public bool AttendeeListContainsParticipant(List<Attendee> list, Participant participant, MailboxSession session)
		{
			bool result = false;
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (participant == null)
			{
				throw new ArgumentNullException("participant");
			}
			foreach (Attendee attendee in list)
			{
				if (Participant.HasSameEmail(attendee.Participant, participant, session, true))
				{
					result = true;
					break;
				}
			}
			return result;
		}

		private bool SendRumsBase(RumInfo info, bool copyToSentItems, CalendarItemBase item, ref Dictionary<GlobalObjectId, List<Attendee>> organizerRumsSent)
		{
			bool flag = false;
			if (info.Type != RumType.None)
			{
				if (info is OrganizerRumInfo)
				{
					OrganizerRumInfo info2 = (OrganizerRumInfo)info;
					this.SendOrganizerRums(info2, copyToSentItems, item, ref organizerRumsSent);
					flag = true;
				}
				else if (info is ResponseRumInfo)
				{
					item.SendResponseRum((ResponseRumInfo)info, copyToSentItems);
					flag = true;
				}
				else
				{
					if (!(info is AttendeeInquiryRumInfo))
					{
						throw new NotSupportedException(info.GetType().ToString());
					}
					if (((AttendeeInquiryRumInfo)info).WouldRepair)
					{
						item.SendAttendeeInquiryRum((AttendeeInquiryRumInfo)info, copyToSentItems);
						flag = true;
					}
				}
				if (flag)
				{
					info.SendTime = new ExDateTime?(ExDateTime.Now);
				}
				item.Load();
			}
			return flag;
		}

		private void SendOrganizerRums(OrganizerRumInfo info, bool copyToSentItems, CalendarItemBase item, ref Dictionary<GlobalObjectId, List<Attendee>> sentRumsToCheck)
		{
			bool flag = false;
			List<Attendee> list;
			if (sentRumsToCheck.ContainsKey(item.GlobalObjectId))
			{
				flag = true;
				list = sentRumsToCheck[item.GlobalObjectId];
				for (int i = info.AttendeeList.Count - 1; i > -1; i--)
				{
					Attendee attendee = info.AttendeeList[i];
					if (!attendee.IsSendable() || this.AttendeeListContainsParticipant(list, attendee.Participant, item.Session as MailboxSession))
					{
						info.AttendeeList.RemoveAt(i);
					}
				}
			}
			else
			{
				list = new List<Attendee>(info.AttendeeList.Count);
			}
			if (info.AttendeeList.Count != 0)
			{
				if (info is UpdateRumInfo)
				{
					UpdateRumInfo rumInfo = (UpdateRumInfo)info;
					if (item.CalendarItemType == CalendarItemType.Occurrence)
					{
						item.SendForwardRum(rumInfo, copyToSentItems);
					}
					else
					{
						item.SendUpdateRums(rumInfo, copyToSentItems);
					}
				}
				else
				{
					if (!(info is CancellationRumInfo))
					{
						throw new NotSupportedException(info.GetType().ToString());
					}
					CancellationRumInfo rumInfo2 = (CancellationRumInfo)info;
					item.SendCancellationRums(rumInfo2, copyToSentItems);
				}
				list.AddRange(info.AttendeeList);
				if (!flag)
				{
					sentRumsToCheck.Add(item.GlobalObjectId, list);
				}
			}
			info.AttendeeList.Clear();
		}

		private static RumAgent instance;

		private static object threadSafetyLock = new object();
	}
}
