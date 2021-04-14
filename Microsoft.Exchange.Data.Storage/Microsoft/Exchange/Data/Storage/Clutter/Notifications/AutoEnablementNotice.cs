using System;
using System.IO;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage.Clutter.Notifications
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AutoEnablementNotice : ClutterNotification
	{
		static AutoEnablementNotice()
		{
			ExTimeZone pacificStandardTimeZone = null;
			if (!ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName("Pacific Standard Time", out pacificStandardTimeZone))
			{
				pacificStandardTimeZone = ExTimeZone.UtcTimeZone;
			}
			AutoEnablementNotice.PacificStandardTimeZone = pacificStandardTimeZone;
		}

		public AutoEnablementNotice(MailboxSession session, VariantConfigurationSnapshot snapshot, IFrontEndLocator frontEndLocator) : base(session, snapshot, frontEndLocator)
		{
		}

		protected override LocalizedString GetSubject()
		{
			return ClientStrings.ClutterNotificationAutoEnablementNoticeSubject;
		}

		protected override Importance GetImportance()
		{
			return Importance.High;
		}

		protected override void WriteMessageProperties(MessageItem message)
		{
			ExTimeZone userTimeZoneOrDefault = DateTimeHelper.GetUserTimeZoneOrDefault(base.Session, AutoEnablementNotice.PacificStandardTimeZone);
			ExDateTime futureTimestamp = DateTimeHelper.GetFutureTimestamp(ExDateTime.UtcNow, 0, DayOfWeek.Tuesday, TimeSpan.FromHours(12.0), userTimeZoneOrDefault);
			message.SetFlag(ClientStrings.RequestedActionFollowUp.ToString(base.Culture), new ExDateTime?(ExDateTime.UtcNow), new ExDateTime?(futureTimestamp));
			message.Reminder.DueBy = new ExDateTime?(futureTimestamp);
			message.Reminder.IsSet = true;
		}

		protected override void WriteMessageBody(StreamWriter streamWriter)
		{
			base.WriteHeader(streamWriter, ClientStrings.ClutterNotificationAutoEnablementNoticeHeader);
			base.WriteParagraph(streamWriter, ClientStrings.ClutterNotificationAutoEnablementNoticeIntro, 30U);
			base.WriteSubHeader(streamWriter, ClientStrings.ClutterNotificationAutoEnablementNoticeWeCallIt);
			base.WriteParagraph(streamWriter, ClientStrings.ClutterNotificationAutoEnablementNoticeHowItWorks);
			base.WriteParagraph(streamWriter, ClientStrings.ClutterNotificationAutoEnablementNoticeYoullBeEnabed(AutoEnablementNotice.OptOutUrl));
			base.WriteParagraph(streamWriter, ClientStrings.ClutterNotificationAutoEnablementNoticeItsAutomatic, 20U);
			base.WriteParagraph(streamWriter, ClientStrings.ClutterNotificationO365Closing, 0U);
			base.WriteParagraph(streamWriter, ClientStrings.ClutterNotificationO365DisplayName);
			base.WriteParagraph(streamWriter, ClientStrings.ClutterNotificationAutoEnablementNoticeLearnMore(ClutterNotification.AnnouncementUrl, ClutterNotification.LearnMoreUrl));
		}

		public static readonly string OptOutUrl = "http://aka.ms/ItsCrazyButDontEnableMeForClutter";

		private static readonly ExTimeZone PacificStandardTimeZone;
	}
}
