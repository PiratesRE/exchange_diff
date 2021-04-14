using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(2999405978U, "QuarantineLoadRegistryAccessFailed");
			Strings.stringIDs.Add(1182810610U, "descExplination");
			Strings.stringIDs.Add(204157539U, "MeetingForwardNotificationSubject");
			Strings.stringIDs.Add(2422042322U, "UnexpectedException");
			Strings.stringIDs.Add(3518739609U, "NoSubject");
			Strings.stringIDs.Add(1075460866U, "descCredit");
			Strings.stringIDs.Add(1630794581U, "descArialGreySmallFontTag");
			Strings.stringIDs.Add(869936810U, "NoStartTime");
			Strings.stringIDs.Add(3779075844U, "descTahomaBlackMediumFontTag");
			Strings.stringIDs.Add(445569887U, "descMeetingTimeLabel");
			Strings.stringIDs.Add(3244013810U, "descTahomaGreyMediumFontTag");
			Strings.stringIDs.Add(3488657717U, "descTitle");
			Strings.stringIDs.Add(3493819938U, "descMeetingSubjectLabel");
			Strings.stringIDs.Add(3885561947U, "descRecipientsLabel");
			Strings.stringIDs.Add(4274878084U, "descTimeZoneInfo");
			Strings.stringIDs.Add(1004476481U, "PoisonMessageRegistryAccessFailed");
		}

		public static LocalizedString QuarantineLoadRegistryAccessFailed
		{
			get
			{
				return new LocalizedString("QuarantineLoadRegistryAccessFailed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descExplination
		{
			get
			{
				return new LocalizedString("descExplination", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MeetingForwardNotificationSubject
		{
			get
			{
				return new LocalizedString("MeetingForwardNotificationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnexpectedException
		{
			get
			{
				return new LocalizedString("UnexpectedException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoSubject
		{
			get
			{
				return new LocalizedString("NoSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descCredit
		{
			get
			{
				return new LocalizedString("descCredit", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descArialGreySmallFontTag
		{
			get
			{
				return new LocalizedString("descArialGreySmallFontTag", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoStartTime
		{
			get
			{
				return new LocalizedString("NoStartTime", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descTahomaBlackMediumFontTag
		{
			get
			{
				return new LocalizedString("descTahomaBlackMediumFontTag", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descMeetingTimeLabel
		{
			get
			{
				return new LocalizedString("descMeetingTimeLabel", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descTahomaGreyMediumFontTag
		{
			get
			{
				return new LocalizedString("descTahomaGreyMediumFontTag", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descTitle
		{
			get
			{
				return new LocalizedString("descTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descMeetingSubjectLabel
		{
			get
			{
				return new LocalizedString("descMeetingSubjectLabel", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descRecipientsLabel
		{
			get
			{
				return new LocalizedString("descRecipientsLabel", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descTimeZoneInfo
		{
			get
			{
				return new LocalizedString("descTimeZoneInfo", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PoisonMessageRegistryAccessFailed
		{
			get
			{
				return new LocalizedString("PoisonMessageRegistryAccessFailed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(16);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			QuarantineLoadRegistryAccessFailed = 2999405978U,
			descExplination = 1182810610U,
			MeetingForwardNotificationSubject = 204157539U,
			UnexpectedException = 2422042322U,
			NoSubject = 3518739609U,
			descCredit = 1075460866U,
			descArialGreySmallFontTag = 1630794581U,
			NoStartTime = 869936810U,
			descTahomaBlackMediumFontTag = 3779075844U,
			descMeetingTimeLabel = 445569887U,
			descTahomaGreyMediumFontTag = 3244013810U,
			descTitle = 3488657717U,
			descMeetingSubjectLabel = 3493819938U,
			descRecipientsLabel = 3885561947U,
			descTimeZoneInfo = 4274878084U,
			PoisonMessageRegistryAccessFailed = 1004476481U
		}
	}
}
