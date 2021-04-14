using System;
using System.IO;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage.Clutter.Notifications
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InvitationNotification : ClutterNotification
	{
		public InvitationNotification(MailboxSession session, VariantConfigurationSnapshot snapshot, IFrontEndLocator frontEndLocator) : base(session, snapshot, frontEndLocator)
		{
		}

		protected override LocalizedString GetSubject()
		{
			return ClientStrings.ClutterNotificationInvitationSubject;
		}

		protected override void WriteMessageBody(StreamWriter streamWriter)
		{
			base.WriteHeader(streamWriter, ClientStrings.ClutterNotificationInvitationHeader);
			base.WriteParagraph(streamWriter, ClientStrings.ClutterNotificationInvitationIntro, 30U);
			base.WriteSubHeader(streamWriter, ClientStrings.ClutterNotificationInvitationWeCallIt);
			base.WriteParagraph(streamWriter, ClientStrings.ClutterNotificationInvitationHowItWorks);
			base.WriteParagraph(streamWriter, ClientStrings.ClutterNotificationInvitationItsAutomatic);
			base.WriteTurnOnInstructions(streamWriter);
			base.WriteParagraph(streamWriter, ClientStrings.ClutterNotificationInvitationIfYouDontLikeIt, 0U);
			base.WriteParagraph(streamWriter, ClientStrings.ClutterNotificationInvitationLearnMore(ClutterNotification.LearnMoreUrl));
			base.WriteParagraph(streamWriter, ClientStrings.ClutterNotificationInvitationO365Helps, 20U);
			base.WriteParagraph(streamWriter, ClientStrings.ClutterNotificationO365Closing, 0U);
			base.WriteParagraph(streamWriter, ClientStrings.ClutterNotificationO365DisplayName);
		}
	}
}
