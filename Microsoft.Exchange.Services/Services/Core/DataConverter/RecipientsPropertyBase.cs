using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal abstract class RecipientsPropertyBase : ComplexPropertyBase, IPregatherParticipants, IToXmlCommand, ISetCommand, IToServiceObjectCommand, IAppendUpdateCommand, ISetUpdateCommand, IDeleteUpdateCommand, IUpdateCommand, IPropertyCommand
	{
		public RecipientsPropertyBase(CommandContext commandContext, RecipientItemType recipientItemType) : base(commandContext)
		{
			this.recipientItemType = recipientItemType;
		}

		void IPregatherParticipants.Pregather(StoreObject storeObject, List<Participant> participants)
		{
			MessageItem messageItem = storeObject as MessageItem;
			int num = 0;
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			ItemResponseShape itemResponseShape = commandSettings.ResponseShape as ItemResponseShape;
			if (messageItem != null)
			{
				foreach (Recipient recipient in messageItem.Recipients)
				{
					if (recipient.RecipientItemType == this.recipientItemType)
					{
						participants.Add(recipient.Participant);
						num++;
						if (itemResponseShape.MaximumRecipientsToReturn > 0 && num >= itemResponseShape.MaximumRecipientsToReturn)
						{
							break;
						}
					}
				}
			}
		}

		public void Set()
		{
			SetCommandSettings commandSettings = base.GetCommandSettings<SetCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			this.SetProperty(serviceObject, storeObject, false);
		}

		private void SetProperty(ServiceObject serviceObject, StoreObject storeObject, bool isAppend)
		{
			MessageItem messageItem = storeObject as MessageItem;
			if (messageItem == null)
			{
				throw new InvalidPropertyRequestException(this.commandContext.PropertyInformation.PropertyPath);
			}
			if (!isAppend)
			{
				this.ClearRecipientTypeFromCollection(messageItem.Recipients, messageItem.IsResend);
			}
			EmailAddressWrapper[] valueOrDefault = serviceObject.GetValueOrDefault<EmailAddressWrapper[]>(this.commandContext.PropertyInformation);
			if (valueOrDefault != null)
			{
				foreach (EmailAddressWrapper emailAddressWrapper in valueOrDefault)
				{
					if (emailAddressWrapper != null)
					{
						messageItem.Recipients.Add(base.GetParticipantOrDLFromAddress(emailAddressWrapper, storeObject), this.recipientItemType);
					}
				}
			}
		}

		public override void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			StoreObject storeObject = updateCommandSettings.StoreObject;
			this.SetProperty(setPropertyUpdate.ServiceObject, storeObject, false);
		}

		public override void DeleteUpdate(DeletePropertyUpdate deletePropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			StoreObject storeObject = updateCommandSettings.StoreObject;
			MessageItem messageItem = storeObject as MessageItem;
			if (messageItem == null)
			{
				throw new InvalidPropertyRequestException(this.commandContext.PropertyInformation.PropertyPath);
			}
			this.ClearRecipientTypeFromCollection(messageItem.Recipients, false);
		}

		public override void AppendUpdate(AppendPropertyUpdate appendPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			StoreObject storeObject = updateCommandSettings.StoreObject;
			this.SetProperty(appendPropertyUpdate.ServiceObject, storeObject, true);
		}

		private void ClearRecipientTypeFromCollection(RecipientCollection recipients, bool keepSubmittedRecipient)
		{
			for (int i = recipients.Count - 1; i >= 0; i--)
			{
				if (recipients[i].RecipientItemType == this.recipientItemType && (!keepSubmittedRecipient || !recipients[i].Submitted))
				{
					recipients.Remove(recipients[i]);
				}
			}
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			MessageItem messageItem = storeObject as MessageItem;
			int num = 0;
			ItemResponseShape itemResponseShape = commandSettings.ResponseShape as ItemResponseShape;
			ParticipantInformationDictionary participantInformation = EWSSettings.ParticipantInformation;
			if (messageItem != null)
			{
				List<EmailAddressWrapper> list = new List<EmailAddressWrapper>();
				foreach (Recipient recipient in messageItem.Recipients)
				{
					ParticipantInformation participantInformation2;
					if (recipient.RecipientItemType == this.recipientItemType && recipient.Participant != null && participantInformation.TryGetParticipant(recipient.Participant, out participantInformation2))
					{
						SingleRecipientType singleRecipientType = PropertyCommand.CreateRecipientFromParticipant(participantInformation2);
						singleRecipientType.Mailbox.ItemId = base.GetParticipantItemId(participantInformation2, storeObject);
						if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2012))
						{
							singleRecipientType.Mailbox.Submitted = new bool?(recipient.Submitted);
						}
						list.Add(singleRecipientType.Mailbox);
						num++;
						if (itemResponseShape.MaximumRecipientsToReturn > 0 && num >= itemResponseShape.MaximumRecipientsToReturn)
						{
							break;
						}
					}
				}
				serviceObject[propertyInformation] = ((list.Count > 0) ? list.ToArray() : null);
			}
		}

		public void ToXml()
		{
			ToXmlCommandSettings commandSettings = base.GetCommandSettings<ToXmlCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			XmlElement serviceItem = commandSettings.ServiceItem;
			MessageItem messageItem = storeObject as MessageItem;
			if (messageItem != null)
			{
				ParticipantInformationDictionary participantInformation = EWSSettings.ParticipantInformation;
				XmlElement xmlElement = null;
				foreach (Recipient recipient in messageItem.Recipients)
				{
					if (recipient.RecipientItemType == this.recipientItemType)
					{
						if (xmlElement == null)
						{
							xmlElement = base.CreateXmlElement(serviceItem, this.xmlLocalName);
						}
						base.CreateParticipantOrDLXml(xmlElement, participantInformation.GetParticipant(recipient.Participant), storeObject);
					}
				}
			}
		}

		private RecipientItemType recipientItemType;
	}
}
