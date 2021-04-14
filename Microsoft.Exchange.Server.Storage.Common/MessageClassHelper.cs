using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public static class MessageClassHelper
	{
		public static bool IsValidMessageClass(string msgClass)
		{
			if (msgClass == null)
			{
				return false;
			}
			if (msgClass == string.Empty)
			{
				return true;
			}
			if (msgClass.StartsWith(".") || msgClass.EndsWith(".") || msgClass.Contains(".."))
			{
				return false;
			}
			foreach (char c in msgClass)
			{
				if (c < ' ' || c > '~')
				{
					return false;
				}
			}
			return true;
		}

		public static bool MatchingMessageClass(string messageClassToCheck, string desiredMessageClass)
		{
			return !string.IsNullOrEmpty(messageClassToCheck) && messageClassToCheck.Length >= desiredMessageClass.Length && messageClassToCheck.StartsWith(desiredMessageClass, StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsSchedulePlusMessage(string messageClass)
		{
			return MessageClassHelper.MatchingMessageClass(messageClass, "IPM.Schedule.") || MessageClassHelper.MatchingMessageClass(messageClass, "IPM.MeetingMessageSeries.") || MessageClassHelper.MatchingMessageClass(messageClass, "IPM.SchedulePlus.FreeBusy.BinaryData");
		}

		public static bool IsCalendarFamilyMessage(string messageClass)
		{
			return MessageClassHelper.MatchingMessageClass(messageClass, "IPM.Appointment") || MessageClassHelper.MatchingMessageClass(messageClass, "IPM.Schedule.Meeting") || MessageClassHelper.MatchingMessageClass(messageClass, "IPM.OLE.CLASS.{00061055-0000-0000-C000-000000000046}");
		}

		public static bool IsFreeDocumentMessage(string messageClass)
		{
			return MessageClassHelper.MatchingMessageClass(messageClass, "IPM.Document");
		}
	}
}
