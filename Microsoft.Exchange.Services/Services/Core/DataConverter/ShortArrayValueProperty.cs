using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class ShortArrayValueProperty : SimpleProperty, IToServiceObjectForPropertyBagCommand, IPropertyCommand
	{
		private ShortArrayValueProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public new static ShortArrayValueProperty CreateCommand(CommandContext commandContext)
		{
			return new ShortArrayValueProperty(commandContext);
		}

		public new void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			object value = null;
			serviceObject[propertyInformation] = value;
			if (PropertyCommand.TryGetValueFromPropertyBag<object>(propertyBag, this.propertyDefinition, out value))
			{
				ArrayPropertyInformation arrayPropertyInformation = propertyInformation as ArrayPropertyInformation;
				if (arrayPropertyInformation != null)
				{
					serviceObject[propertyInformation] = value;
				}
			}
		}
	}
}
