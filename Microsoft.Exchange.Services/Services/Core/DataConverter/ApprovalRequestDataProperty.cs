using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ApprovalRequestDataProperty : ComplexPropertyBase, IToServiceObjectCommand, IPropertyCommand
	{
		public ApprovalRequestDataProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static ApprovalRequestDataProperty CreateCommand(CommandContext commandContext)
		{
			return new ApprovalRequestDataProperty(commandContext);
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
			if (messageItem == null || !messageItem.IsValidApprovalRequest())
			{
				return;
			}
			ApprovalRequestDataType value = new ApprovalRequestDataType(messageItem);
			ServiceObject serviceObject = commandSettings.ServiceObject;
			serviceObject.PropertyBag[propertyInformation] = value;
		}
	}
}
