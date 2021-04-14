using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ReminderMessageDataProperty : ComplexPropertyBase, IToServiceObjectCommand, IPropertyCommand
	{
		public ReminderMessageDataProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static ReminderMessageDataProperty CreateCommand(CommandContext commandContext)
		{
			return new ReminderMessageDataProperty(commandContext);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("ApprovalRequestDataProperty.ToXml should not be called.");
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			MessageItem messageItem = commandSettings.StoreObject as MessageItem;
			if (messageItem == null || !messageItem.IsEventReminderMessage() || !(messageItem is ReminderMessage))
			{
				return;
			}
			ExDateTime valueOrDefault = messageItem.GetValueOrDefault<ExDateTime>(ReminderMessageSchema.ReminderStartTime, ExDateTime.MinValue);
			if (valueOrDefault == ExDateTime.MinValue)
			{
				return;
			}
			ReminderMessageDataType value = new ReminderMessageDataType((ReminderMessage)messageItem);
			ServiceObject serviceObject = commandSettings.ServiceObject;
			serviceObject.PropertyBag[propertyInformation] = value;
		}
	}
}
