using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class AllowNewTimeProposalProperty : ComplexPropertyBase, IToXmlCommand, IToXmlForPropertyBagCommand, IToServiceObjectCommand, IToServiceObjectForPropertyBagCommand, ISetCommand, ISetUpdateCommand, IUpdateCommand, IPropertyCommand
	{
		private AllowNewTimeProposalProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static AllowNewTimeProposalProperty CreateCommand(CommandContext commandContext)
		{
			return new AllowNewTimeProposalProperty(commandContext);
		}

		public void Set()
		{
			SetCommandSettings commandSettings = base.GetCommandSettings<SetCommandSettings>();
			ServiceObject serviceObject = commandSettings.ServiceObject;
			bool allowNewTimeProposal = (bool)serviceObject[this.commandContext.PropertyInformation];
			CalendarItemBase calendarItemBase = commandSettings.StoreObject as CalendarItemBase;
			if (calendarItemBase != null)
			{
				AllowNewTimeProposalProperty.InvertAndSetProperty(calendarItemBase, allowNewTimeProposal);
				return;
			}
			throw new InvalidPropertySetException(this.commandContext.PropertyInformation.PropertyPath);
		}

		public override void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			ServiceObject serviceObject = setPropertyUpdate.ServiceObject;
			CalendarItemBase calendarItemBase = updateCommandSettings.StoreObject as CalendarItemBase;
			if (calendarItemBase != null)
			{
				bool allowNewTimeProposal = (bool)serviceObject[this.commandContext.PropertyInformation];
				AllowNewTimeProposalProperty.InvertAndSetProperty(calendarItemBase, allowNewTimeProposal);
				return;
			}
			throw new InvalidPropertySetException(setPropertyUpdate.PropertyPath);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("AllowNewTimeZoneProposalProperty.ToXml should not be called.");
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			PropertyDefinition[] propertyDefinitions = this.commandContext.GetPropertyDefinitions();
			if (PropertyCommand.StorePropertyExists(storeObject, propertyDefinitions[0]))
			{
				bool flag = (bool)storeObject[CalendarItemBaseSchema.DisallowNewTimeProposal];
				commandSettings.ServiceObject[CalendarItemSchema.OrganizerSpecific.AllowNewTimeProposal] = !flag;
			}
		}

		public void ToXmlForPropertyBag()
		{
			throw new InvalidOperationException("AllowNewTimeZoneProposalProperty.ToXmlForPropertyBag should not be called.");
		}

		public void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			ServiceObject serviceObject = commandSettings.ServiceObject;
			bool flag;
			if (PropertyCommand.TryGetValueFromPropertyBag<bool>(commandSettings.PropertyBag, CalendarItemBaseSchema.DisallowNewTimeProposal, out flag))
			{
				serviceObject.PropertyBag[CalendarItemSchema.OrganizerSpecific.AllowNewTimeProposal] = !flag;
			}
		}

		private static void InvertAndSetProperty(CalendarItemBase calendarItemBase, bool allowNewTimeProposal)
		{
			calendarItemBase[CalendarItemBaseSchema.DisallowNewTimeProposal] = !allowNewTimeProposal;
		}
	}
}
