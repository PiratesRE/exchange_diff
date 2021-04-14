using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.TextMessaging.MobileDriver.Resources;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class MessageItemToTextSendingPackages
	{
		public MessageItemToTextSendingPackages(IMobileServiceManager manager)
		{
			this.MobileServiceManager = manager;
		}

		public IMobileServiceManager MobileServiceManager { get; private set; }

		public IList<TextSendingPackage> Convert(MessageItem message, CalendarNotificationType calNotifHint, out int messageCount, out bool hasUnicode)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}
			messageCount = 0;
			hasUnicode = false;
			List<TextSendingPackage> list = new List<TextSendingPackage>();
			Dictionary<MobileServiceCapability, List<MobileRecipient>> dictionary = new Dictionary<MobileServiceCapability, List<MobileRecipient>>(message.Recipients.Count);
			if (!this.MobileServiceManager.CapabilityPerRecipientSupported)
			{
				MobileServiceCapability capabilityForRecipient = this.MobileServiceManager.GetCapabilityForRecipient(null);
				if (capabilityForRecipient == null)
				{
					throw new MobileDriverStateException(Strings.ErrorInvalidState("Capability", Strings.ConstNull));
				}
				dictionary[capabilityForRecipient] = new List<MobileRecipient>(message.Recipients);
			}
			else
			{
				foreach (MobileRecipient mobileRecipient in message.Recipients)
				{
					MobileServiceCapability mobileServiceCapability = this.MobileServiceManager.GetCapabilityForRecipient(mobileRecipient);
					if (mobileServiceCapability == null)
					{
						throw new MobileDriverStateException(Strings.ErrorInvalidState("Capability", Strings.ConstNull));
					}
					bool flag = false;
					foreach (MobileServiceCapability mobileServiceCapability2 in dictionary.Keys)
					{
						if (MobileServiceCapability.DoesCodingAndCapacityMatch(mobileServiceCapability, mobileServiceCapability2))
						{
							mobileServiceCapability = mobileServiceCapability2;
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						dictionary[mobileServiceCapability] = new List<MobileRecipient>();
					}
					dictionary[mobileServiceCapability].Add(mobileRecipient);
				}
			}
			foreach (MobileServiceCapability mobileServiceCapability3 in dictionary.Keys)
			{
				List<MobileRecipient> recipients = dictionary[mobileServiceCapability3];
				CodingSupportability codingSupportability = null;
				CodingSupportability codingSupportability2 = null;
				bool flag2 = false;
				foreach (CodingSupportability codingSupportability3 in mobileServiceCapability3.CodingSupportabilities)
				{
					if (CodingScheme.GsmDefault == codingSupportability3.CodingScheme)
					{
						codingSupportability = codingSupportability3;
					}
					else if (CodingScheme.Unicode == codingSupportability3.CodingScheme)
					{
						codingSupportability2 = codingSupportability3;
					}
					if (flag2 = (codingSupportability != null && null != codingSupportability2))
					{
						break;
					}
				}
				CodingSupportability codingSupportability4 = mobileServiceCapability3.CodingSupportabilities[0];
				if (1 == message.Message.ProportionedTexts.Count)
				{
					ISplitter splitter3;
					if (flag2)
					{
						ISplitter splitter2;
						if (PartType.Short != mobileServiceCapability3.SupportedPartType)
						{
							ISplitter splitter = new GsmConcatenatedPartSplitter(codingSupportability.RadixPerPart, codingSupportability2.RadixPerPart, codingSupportability.RadixPerSegment, codingSupportability2.RadixPerSegment, mobileServiceCapability3.SegmentsPerConcatenatedPart, false, message.MaxSegmentsPerRecipient);
							splitter2 = splitter;
						}
						else
						{
							splitter2 = new GsmShortPartSplitter(codingSupportability.RadixPerPart, codingSupportability2.RadixPerPart, message.MaxSegmentsPerRecipient);
						}
						splitter3 = splitter2;
					}
					else
					{
						splitter3 = new CodedShortPartSplitter(codingSupportability4.CodingScheme, codingSupportability4.RadixPerPart, '?', message.MaxSegmentsPerRecipient);
					}
					TextSendingPackage textSendingPackage = new TextSendingPackage(splitter3.Split(message.Message.ProportionedTexts[0].Text), recipients);
					messageCount += textSendingPackage.BookmarkRetriever.Segments.Count;
					list.Add(textSendingPackage);
				}
				else
				{
					if ((PartType.Short & mobileServiceCapability3.SupportedPartType) == (PartType)0)
					{
						throw new MobileDriverStateException(Strings.ErrorInvalidState("SupportedPartType", mobileServiceCapability3.SupportedPartType.ToString()));
					}
					if (1 != message.MaxSegmentsPerRecipient)
					{
						throw new MobileDriverStateException(Strings.ErrorInvalidState("MaxSegmentsPerRecipient", message.MaxSegmentsPerRecipient.ToString()));
					}
					IComposer composer;
					if (flag2)
					{
						composer = new GsmShortPartComposer(codingSupportability.RadixPerPart, codingSupportability2.RadixPerPart, (CalendarNotificationType.Summary == calNotifHint) ? 5 : 1);
					}
					else
					{
						composer = new CodedShortPartComposer(codingSupportability4.CodingScheme, codingSupportability4.RadixPerPart, (CalendarNotificationType.Summary == calNotifHint) ? 5 : 1, '?');
					}
					TextSendingPackage textSendingPackage2 = new TextSendingPackage(composer.Compose(message.Message.ProportionedTexts), recipients);
					messageCount += textSendingPackage2.BookmarkRetriever.Segments.Count;
					list.Add(textSendingPackage2);
				}
			}
			foreach (Bookmark bookmark in list[0].BookmarkRetriever.Parts)
			{
				if (bookmark.CodingScheme == CodingScheme.Unicode)
				{
					hasUnicode = true;
					break;
				}
			}
			return list.AsReadOnly();
		}
	}
}
