using System;
using System.IO;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage.Clutter.Notifications
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OptInNotification : ClutterNotification
	{
		public OptInNotification(MailboxSession session, VariantConfigurationSnapshot snapshot, IFrontEndLocator frontEndLocator) : base(session, snapshot, frontEndLocator)
		{
		}

		protected override LocalizedString GetSubject()
		{
			return ClientStrings.ClutterNotificationOptInSubject;
		}

		protected override void WriteMessageBody(StreamWriter streamWriter)
		{
			string clutterFolderName = ClientStrings.ClutterFolderName.ToString(base.Culture);
			base.WriteHeader(streamWriter, ClientStrings.ClutterNotificationOptInHeader);
			base.WriteParagraph(streamWriter, ClientStrings.ClutterNotificationOptInIntro(clutterFolderName));
			base.WriteParagraph(streamWriter, ClientStrings.ClutterNotificationOptInHowToTrain(clutterFolderName));
			base.WriteParagraph(streamWriter, ClientStrings.ClutterNotificationOptInFeedback(ClutterNotification.FeedbackMailtoUrl));
			base.WriteTurnOffInstructions(streamWriter);
			base.WriteParagraph(streamWriter, ClientStrings.ClutterNotificationOptInPrivacy, 0U);
			base.WriteParagraph(streamWriter, ClientStrings.ClutterNotificationOptInLearnMore(ClutterNotification.LearnMoreUrl), 20U);
			base.WriteParagraph(streamWriter, ClientStrings.ClutterNotificationO365Closing, 0U);
			base.WriteParagraph(streamWriter, ClientStrings.ClutterNotificationO365DisplayName);
		}
	}
}
