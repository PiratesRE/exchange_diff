using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal static class SendMeetingInvitations
	{
		public static CalendarItemOperationType.CreateOrDelete ConvertToEnum(string sendMeetingInvitationsValue)
		{
			CalendarItemOperationType.CreateOrDelete result = CalendarItemOperationType.CreateOrDelete.SendToNone;
			if (sendMeetingInvitationsValue != null)
			{
				if (!(sendMeetingInvitationsValue == "SendToNone"))
				{
					if (!(sendMeetingInvitationsValue == "SendOnlyToAll"))
					{
						if (sendMeetingInvitationsValue == "SendToAllAndSaveCopy")
						{
							result = CalendarItemOperationType.CreateOrDelete.SendToAllAndSaveCopy;
						}
					}
					else
					{
						result = CalendarItemOperationType.CreateOrDelete.SendOnlyToAll;
					}
				}
				else
				{
					result = CalendarItemOperationType.CreateOrDelete.SendToNone;
				}
			}
			return result;
		}

		public const string SendToNone = "SendToNone";

		public const string SendOnlyToAll = "SendOnlyToAll";

		public const string SendToAllAndSaveCopy = "SendToAllAndSaveCopy";
	}
}
