using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal static class SendMeetingInvitationsOrCancellations
	{
		public static CalendarItemOperationType.Update ConvertToEnum(string sendMeetingInvitationsOrCancellationsValue)
		{
			CalendarItemOperationType.Update result = CalendarItemOperationType.Update.SendToNone;
			if (sendMeetingInvitationsOrCancellationsValue != null)
			{
				if (!(sendMeetingInvitationsOrCancellationsValue == "SendToNone"))
				{
					if (!(sendMeetingInvitationsOrCancellationsValue == "SendOnlyToAll"))
					{
						if (!(sendMeetingInvitationsOrCancellationsValue == "SendOnlyToChanged"))
						{
							if (!(sendMeetingInvitationsOrCancellationsValue == "SendToAllAndSaveCopy"))
							{
								if (sendMeetingInvitationsOrCancellationsValue == "SendToChangedAndSaveCopy")
								{
									result = CalendarItemOperationType.Update.SendToChangedAndSaveCopy;
								}
							}
							else
							{
								result = CalendarItemOperationType.Update.SendToAllAndSaveCopy;
							}
						}
						else
						{
							result = CalendarItemOperationType.Update.SendOnlyToChanged;
						}
					}
					else
					{
						result = CalendarItemOperationType.Update.SendOnlyToAll;
					}
				}
				else
				{
					result = CalendarItemOperationType.Update.SendToNone;
				}
			}
			return result;
		}

		public const string SendToNone = "SendToNone";

		public const string SendOnlyToAll = "SendOnlyToAll";

		public const string SendOnlyToChanged = "SendOnlyToChanged";

		public const string SendToAllAndSaveCopy = "SendToAllAndSaveCopy";

		public const string SendToChangedAndSaveCopy = "SendToChangedAndSaveCopy";
	}
}
