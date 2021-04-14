using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.TextMessaging.MobileDriver.Resources;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class Eas : IMobileService
	{
		public Eas(EasSelector selector)
		{
			this.Manager = new EasManager(selector);
		}

		public EasManager Manager { get; private set; }

		IMobileServiceManager IMobileService.Manager
		{
			get
			{
				return this.Manager;
			}
		}

		public void Send(IList<TextSendingPackage> textPackages, Message message, MobileRecipient sender)
		{
			this.Send(textPackages, message, sender, null);
		}

		public void Send(IList<TextSendingPackage> textPackages, Message message, MobileRecipient sender, string internetMessageId)
		{
			ExSmsCounters.NumberOfTextMessagesSentViaEas.Increment();
			using (MailboxSession mailboxSession = MailboxSession.OpenAsTransport(this.Manager.Selector.Principal, OpenTransportSessionFlags.OpenForSpecialMessageDelivery))
			{
				StoreObjectId storeObjectId = null;
				try
				{
					storeObjectId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Outbox);
					if (storeObjectId == null)
					{
						throw new MobileServicePermanentException(Strings.ErrorObjectNotFound(DefaultFolderType.Outbox.ToString()));
					}
				}
				catch (ObjectNotFoundException ex)
				{
					throw new MobileServicePermanentException(ex.LocalizedString, ex);
				}
				foreach (TextSendingPackage textSendingPackage in textPackages)
				{
					foreach (Bookmark bookmark in textSendingPackage.BookmarkRetriever.Parts)
					{
						using (MessageItem messageItem = MessageItem.Create(mailboxSession, storeObjectId))
						{
							messageItem.ClassName = "IPM.Note.Mobile.SMS";
							messageItem.Sender = new Participant(this.Manager.Selector.Principal);
							messageItem.From = new Participant(this.Manager.Selector.Principal.MailboxInfo.DisplayName, this.Manager.Selector.Number.Number, "MOBILE");
							foreach (MobileRecipient recipient in textSendingPackage.Recipients)
							{
								Participant participant = new Participant(null, MobileRecipient.GetNumberString(recipient), "MOBILE");
								Recipient recipient2 = messageItem.Recipients.Add(participant, RecipientItemType.To);
								recipient2[ItemSchema.Responsibility] = true;
							}
							using (TextWriter textWriter = messageItem.Body.OpenTextWriter(new BodyWriteConfiguration(BodyFormat.TextPlain, Charset.Unicode.Name)))
							{
								textWriter.Write(bookmark.ToString());
							}
							if (!string.IsNullOrEmpty(internetMessageId))
							{
								messageItem.SetProperties(Eas.propertyInternetMessageId, new object[]
								{
									internetMessageId
								});
							}
							messageItem.SetProperties(Eas.propertyTextMessageDeliveryStatus, Eas.propertyValueTextMessageDeliveryStatus);
							mailboxSession.Deliver(messageItem, null, RecipientItemType.Unknown);
						}
					}
				}
			}
		}

		private static readonly PropertyDefinition[] propertyTextMessageDeliveryStatus = new PropertyDefinition[]
		{
			MessageItemSchema.TextMessageDeliveryStatus
		};

		private static readonly object[] propertyValueTextMessageDeliveryStatus = new object[]
		{
			25
		};

		private static readonly PropertyDefinition[] propertyInternetMessageId = new PropertyDefinition[]
		{
			ItemSchema.InternetMessageId
		};
	}
}
