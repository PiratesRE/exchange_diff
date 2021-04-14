using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class HasBlockedImagesProperty : ComplexPropertyBase, IToServiceObjectCommand, IPropertyCommand
	{
		public HasBlockedImagesProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static HasBlockedImagesProperty CreateCommand(CommandContext commandContext)
		{
			return new HasBlockedImagesProperty(commandContext);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("HasBlockedImagesProperty.ToXml should not be called.");
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			if (EWSSettings.ItemHasBlockedImages != null)
			{
				serviceObject.PropertyBag[propertyInformation] = EWSSettings.ItemHasBlockedImages;
				return;
			}
			serviceObject.PropertyBag[propertyInformation] = false;
		}
	}
}
