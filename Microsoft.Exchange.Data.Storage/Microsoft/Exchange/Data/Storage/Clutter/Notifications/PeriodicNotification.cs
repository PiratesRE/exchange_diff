using System;
using System.IO;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage.Clutter.Notifications
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PeriodicNotification : ClutterNotification
	{
		public PeriodicNotification(MailboxSession session, VariantConfigurationSnapshot snapshot, IFrontEndLocator frontEndLocator) : base(session, snapshot, frontEndLocator)
		{
		}

		protected override LocalizedString GetSubject()
		{
			return ClientStrings.ClutterNotificationPeriodicSubject;
		}

		protected override void WriteMessageBody(StreamWriter streamWriter)
		{
			string clutterFolderName = ClientStrings.ClutterFolderName.ToString(base.Culture);
			base.WriteHeader(streamWriter, ClientStrings.ClutterNotificationPeriodicHeader);
			base.WriteParagraph(streamWriter, ClientStrings.ClutterNotificationPeriodicIntro(clutterFolderName));
			base.WriteSurveyInstructions(streamWriter);
			base.WriteParagraph(streamWriter, ClientStrings.ClutterNotificationPeriodicCheckBack, 20U);
			base.WriteParagraph(streamWriter, ClientStrings.ClutterNotificationO365DisplayName, 0U);
			base.WriteParagraph(streamWriter, ClientStrings.ClutterNotificationPeriodicLearnMore(ClutterNotification.LearnMoreUrl));
		}
	}
}
