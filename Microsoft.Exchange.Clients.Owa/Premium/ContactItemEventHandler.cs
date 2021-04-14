using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("ABCA")]
	[OwaEventObjectId(typeof(OwaStoreObjectId))]
	[OwaEventSegmentation(Feature.Contacts)]
	internal sealed class ContactItemEventHandler : AddressBookItemEventHandler
	{
		protected override void BindToData()
		{
			this.itemIds = (OwaStoreObjectId[])base.GetParameter("id");
		}

		protected override void LoadRecipientsToItem(Item item, AddressBookItemEventHandler.ItemTypeToPeople itemType)
		{
			MessageItem messageItem = null;
			CalendarItemBase calendarItemBase = null;
			if (itemType == AddressBookItemEventHandler.ItemTypeToPeople.Meeting)
			{
				calendarItemBase = (item as CalendarItemBase);
				calendarItemBase.IsMeeting = true;
			}
			else
			{
				messageItem = (item as MessageItem);
			}
			for (int i = 0; i < this.itemIds.Length; i++)
			{
				Item item2 = null;
				try
				{
					if (itemType == AddressBookItemEventHandler.ItemTypeToPeople.TextMessage)
					{
						item2 = Utilities.GetItem<Item>(base.UserContext, (OwaStoreObjectId)this.itemIds[i], new PropertyDefinition[]
						{
							ContactSchema.MobilePhone
						});
					}
					else
					{
						item2 = Utilities.GetItem<Item>(base.UserContext, (OwaStoreObjectId)this.itemIds[i], new PropertyDefinition[0]);
					}
					if (item2 is DistributionList)
					{
						if (itemType != AddressBookItemEventHandler.ItemTypeToPeople.TextMessage)
						{
							Participant asParticipant = ((DistributionList)item2).GetAsParticipant();
							if (itemType == AddressBookItemEventHandler.ItemTypeToPeople.Message)
							{
								messageItem.Recipients.Add(asParticipant, RecipientItemType.To);
							}
							else
							{
								calendarItemBase.AttendeeCollection.Add(asParticipant, AttendeeType.Required, null, null, false);
							}
						}
					}
					else if (item2 is Contact)
					{
						Contact contact = (Contact)item2;
						IDictionary<EmailAddressIndex, Participant> emailAddresses = contact.EmailAddresses;
						if (itemType == AddressBookItemEventHandler.ItemTypeToPeople.TextMessage)
						{
							string emailAddress;
							if (!string.IsNullOrEmpty(emailAddress = Utilities.NormalizePhoneNumber(contact.GetValueOrDefault<string>(ContactSchema.MobilePhone))))
							{
								Participant participant = new Participant(contact.DisplayName, emailAddress, "MOBILE");
								messageItem.Recipients.Add(participant, RecipientItemType.To);
							}
						}
						else if (itemType == AddressBookItemEventHandler.ItemTypeToPeople.Message)
						{
							if (emailAddresses[EmailAddressIndex.Email1] != null && !string.IsNullOrEmpty(emailAddresses[EmailAddressIndex.Email1].EmailAddress))
							{
								Participant participant2 = emailAddresses[EmailAddressIndex.Email1];
								messageItem.Recipients.Add(participant2, RecipientItemType.To);
							}
							if (emailAddresses[EmailAddressIndex.Email2] != null && !string.IsNullOrEmpty(emailAddresses[EmailAddressIndex.Email2].EmailAddress))
							{
								Participant participant3 = emailAddresses[EmailAddressIndex.Email2];
								messageItem.Recipients.Add(participant3, RecipientItemType.To);
							}
							if (emailAddresses[EmailAddressIndex.Email3] != null && !string.IsNullOrEmpty(emailAddresses[EmailAddressIndex.Email3].EmailAddress))
							{
								Participant participant4 = emailAddresses[EmailAddressIndex.Email3];
								messageItem.Recipients.Add(participant4, RecipientItemType.To);
							}
						}
						else
						{
							if (emailAddresses[EmailAddressIndex.Email1] != null && !string.IsNullOrEmpty(emailAddresses[EmailAddressIndex.Email1].EmailAddress))
							{
								Participant participant5 = emailAddresses[EmailAddressIndex.Email1];
								calendarItemBase.AttendeeCollection.Add(participant5, AttendeeType.Required, null, null, false);
							}
							if (emailAddresses[EmailAddressIndex.Email2] != null && !string.IsNullOrEmpty(emailAddresses[EmailAddressIndex.Email2].EmailAddress))
							{
								Participant participant6 = emailAddresses[EmailAddressIndex.Email2];
								calendarItemBase.AttendeeCollection.Add(participant6, AttendeeType.Required, null, null, false);
							}
							if (emailAddresses[EmailAddressIndex.Email3] != null && !string.IsNullOrEmpty(emailAddresses[EmailAddressIndex.Email3].EmailAddress))
							{
								Participant participant7 = emailAddresses[EmailAddressIndex.Email3];
								calendarItemBase.AttendeeCollection.Add(participant7, AttendeeType.Required, null, null, false);
							}
						}
					}
				}
				catch (ObjectNotFoundException)
				{
					throw new OwaEventHandlerException("The message could not be created because one of the contacts could not be found.", LocalizedStrings.GetNonEncoded(1863833891));
				}
				finally
				{
					if (item2 != null)
					{
						item2.Dispose();
						item2 = null;
					}
				}
			}
		}

		public const string EventNamespace = "ABCA";
	}
}
