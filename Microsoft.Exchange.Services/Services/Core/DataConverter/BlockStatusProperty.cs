using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class BlockStatusProperty : ComplexPropertyBase, IToServiceObjectCommand, ISetUpdateCommand, IUpdateCommand, IPropertyCommand
	{
		public BlockStatusProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static BlockStatusProperty CreateCommand(CommandContext commandContext)
		{
			return new BlockStatusProperty(commandContext);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("BlockStatusProperty.ToXml should not be called.");
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			Item item = commandSettings.StoreObject as Item;
			ItemResponseShape itemResponseShape = commandSettings.ResponseShape as ItemResponseShape;
			serviceObject.PropertyBag[propertyInformation] = Util.GetItemBlockStatus(item, itemResponseShape.BlockExternalImages, itemResponseShape.BlockExternalImagesIfSenderUntrusted);
		}

		public override void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			Item item = (Item)updateCommandSettings.StoreObject;
			bool valueOrDefault = setPropertyUpdate.ServiceObject.GetValueOrDefault<bool>(this.commandContext.PropertyInformation);
			if (item != null)
			{
				item[ItemSchema.BlockStatus] = (valueOrDefault ? BlockStatus.DontKnow : BlockStatus.NoNeverAgain);
			}
		}
	}
}
