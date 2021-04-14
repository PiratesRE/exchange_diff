using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class RecipientCountsProperty : ComplexPropertyBase, IToServiceObjectCommand, IPropertyCommand
	{
		public RecipientCountsProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static RecipientCountsProperty CreateCommand(CommandContext commandContext)
		{
			return new RecipientCountsProperty(commandContext);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("RecipientCountsProperty.ToXml should not be called.");
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			ServiceObject serviceObject = commandSettings.ServiceObject;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			MessageItem messageItem = commandSettings.StoreObject as MessageItem;
			if (messageItem == null)
			{
				return;
			}
			RecipientCountsType recipientCountsType = new RecipientCountsType();
			recipientCountsType.ToRecipientsCount = 0;
			recipientCountsType.CcRecipientsCount = 0;
			recipientCountsType.BccRecipientsCount = 0;
			foreach (Recipient recipient in messageItem.Recipients)
			{
				if (recipient.Participant != null)
				{
					switch (recipient.RecipientItemType)
					{
					case RecipientItemType.To:
						recipientCountsType.ToRecipientsCount++;
						break;
					case RecipientItemType.Cc:
						recipientCountsType.CcRecipientsCount++;
						break;
					case RecipientItemType.Bcc:
						recipientCountsType.BccRecipientsCount++;
						break;
					}
				}
			}
			serviceObject.PropertyBag[propertyInformation] = recipientCountsType;
		}
	}
}
