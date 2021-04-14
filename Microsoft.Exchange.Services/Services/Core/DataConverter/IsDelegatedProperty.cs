using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class IsDelegatedProperty : ComplexPropertyBase, IToXmlCommand, IToServiceObjectCommand, IPropertyCommand
	{
		private IsDelegatedProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static IsDelegatedProperty CreateCommand(CommandContext commandContext)
		{
			return new IsDelegatedProperty(commandContext);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("IsDelegatedProperty.ToXml should not be called.");
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			MeetingMessage meetingMessage = (MeetingMessage)commandSettings.StoreObject;
			serviceObject[propertyInformation] = meetingMessage.IsDelegated();
		}
	}
}
