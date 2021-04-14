using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class FlagProperty : ComplexPropertyBase, IToXmlCommand, IToXmlForPropertyBagCommand, IToServiceObjectCommand, IToServiceObjectForPropertyBagCommand, ISetCommand, ISetUpdateCommand, IUpdateCommand, IPropertyCommand
	{
		public FlagProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static FlagProperty CreateCommand(CommandContext commandContext)
		{
			return new FlagProperty(commandContext);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("FlagProperty.ToXml should not be called.");
		}

		public void ToXmlForPropertyBag()
		{
			throw new InvalidOperationException("FlagProperty.ToXmlForPropertyBag should not be called.");
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			ServiceObject serviceObject = commandSettings.ServiceObject;
			Item storeItem = (Item)commandSettings.StoreObject;
			serviceObject[this.commandContext.PropertyInformation] = FlagProperty.GetFlag(storeItem);
		}

		public void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			serviceObject[this.commandContext.PropertyInformation] = FlagProperty.GetFlag(propertyBag);
		}

		public void Set()
		{
			SetCommandSettings commandSettings = base.GetCommandSettings<SetCommandSettings>();
			Item item = (Item)commandSettings.StoreObject;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			this.SetFlag(serviceObject, item);
		}

		public override void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			Item item = (Item)updateCommandSettings.StoreObject;
			this.SetFlag(setPropertyUpdate.ServiceObject, item);
		}

		internal static FlagType GetFlagForItemPart(ItemPart itemPart)
		{
			FlagStatus valueOrDefault = itemPart.StorePropertyBag.GetValueOrDefault<FlagStatus>(ItemSchema.FlagStatus, FlagStatus.NotFlagged);
			FlagType flagType = new FlagType
			{
				FlagStatus = valueOrDefault
			};
			ExDateTime systemDateTime;
			if (flagType.FlagStatus == FlagStatus.Flagged)
			{
				if (FlagProperty.TryGetDateTimeProperty(itemPart, TaskSchema.StartDate, out systemDateTime))
				{
					flagType.StartDate = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(systemDateTime);
				}
				if (FlagProperty.TryGetDateTimeProperty(itemPart, TaskSchema.DueDate, out systemDateTime))
				{
					flagType.DueDate = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(systemDateTime);
				}
			}
			else if (flagType.FlagStatus == FlagStatus.Complete && FlagProperty.TryGetDateTimeProperty(itemPart, ItemSchema.CompleteDate, out systemDateTime))
			{
				flagType.CompleteDate = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(systemDateTime);
			}
			return flagType;
		}

		private static FlagType GetFlag(IDictionary<PropertyDefinition, object> propertyBag)
		{
			FlagType flagType = new FlagType();
			FlagStatus flagStatus;
			if (PropertyCommand.TryGetValueFromPropertyBag<FlagStatus>(propertyBag, ItemSchema.FlagStatus, out flagStatus))
			{
				flagType.FlagStatus = flagStatus;
				ExDateTime systemDateTime;
				if (flagType.FlagStatus == FlagStatus.Flagged)
				{
					if (PropertyCommand.TryGetValueFromPropertyBag<ExDateTime>(propertyBag, TaskSchema.StartDate, out systemDateTime))
					{
						flagType.StartDate = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(systemDateTime);
					}
					if (PropertyCommand.TryGetValueFromPropertyBag<ExDateTime>(propertyBag, TaskSchema.DueDate, out systemDateTime))
					{
						flagType.DueDate = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(systemDateTime);
					}
				}
				if (flagType.FlagStatus == FlagStatus.Complete && PropertyCommand.TryGetValueFromPropertyBag<ExDateTime>(propertyBag, ItemSchema.CompleteDate, out systemDateTime))
				{
					flagType.CompleteDate = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(systemDateTime);
				}
			}
			return flagType;
		}

		private static bool TryGetDateTimeProperty(ItemPart itemPart, PropertyDefinition propDef, out ExDateTime exDate)
		{
			exDate = itemPart.StorePropertyBag.GetValueOrDefault<ExDateTime>(propDef, ExDateTime.MaxValue);
			return exDate != ExDateTime.MaxValue;
		}

		private static FlagType GetFlag(Item storeItem)
		{
			FlagType flagType = new FlagType();
			object obj = null;
			if (FlagProperty.TryGetProperty(storeItem, ItemSchema.FlagStatus, out obj))
			{
				flagType.FlagStatus = (FlagStatus)obj;
				if (flagType.FlagStatus == FlagStatus.Flagged)
				{
					if (FlagProperty.TryGetProperty(storeItem, TaskSchema.StartDate, out obj))
					{
						flagType.StartDate = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime((ExDateTime)obj);
					}
					if (FlagProperty.TryGetProperty(storeItem, TaskSchema.DueDate, out obj))
					{
						flagType.DueDate = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime((ExDateTime)obj);
					}
				}
				else if (flagType.FlagStatus == FlagStatus.Complete && FlagProperty.TryGetProperty(storeItem, ItemSchema.CompleteDate, out obj))
				{
					flagType.CompleteDate = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime((ExDateTime)obj);
				}
			}
			return flagType;
		}

		private static bool TryGetProperty(StoreObject storeObject, PropertyDefinition propDef, out object value)
		{
			object obj = storeObject.TryGetProperty(propDef);
			if (obj is PropertyError)
			{
				value = null;
				return false;
			}
			value = obj;
			return true;
		}

		private void SetFlag(ServiceObject serviceObject, Item item)
		{
			FlagType valueOrDefault = serviceObject.GetValueOrDefault<FlagType>(this.commandContext.PropertyInformation);
			if (valueOrDefault != null)
			{
				if (valueOrDefault.FlagStatus == FlagStatus.Flagged)
				{
					ExDateTime? startDate = null;
					ExDateTime? dueDate = null;
					if ((valueOrDefault.StartDate == null && valueOrDefault.DueDate != null) || (valueOrDefault.StartDate != null && valueOrDefault.DueDate == null) || valueOrDefault.CompleteDate != null)
					{
						throw new ServiceArgumentException((CoreResources.IDs)3784063568U);
					}
					if (valueOrDefault.StartDate != null && valueOrDefault.DueDate != null)
					{
						startDate = new ExDateTime?(ExDateTimeConverter.ParseTimeZoneRelated(valueOrDefault.StartDate, EWSSettings.RequestTimeZone));
						dueDate = new ExDateTime?(ExDateTimeConverter.ParseTimeZoneRelated(valueOrDefault.DueDate, EWSSettings.RequestTimeZone));
					}
					item.SetFlag(CoreResources.FlagForFollowUp, startDate, dueDate);
					return;
				}
				else if (valueOrDefault.FlagStatus == FlagStatus.Complete)
				{
					ExDateTime? completeTime = null;
					if (valueOrDefault.StartDate != null || valueOrDefault.DueDate != null)
					{
						throw new ServiceArgumentException((CoreResources.IDs)3784063568U);
					}
					if (valueOrDefault.CompleteDate != null)
					{
						completeTime = new ExDateTime?(ExDateTimeConverter.ParseTimeZoneRelated(valueOrDefault.CompleteDate, EWSSettings.RequestTimeZone));
					}
					item.CompleteFlag(completeTime);
					return;
				}
				else
				{
					if (valueOrDefault.StartDate != null || valueOrDefault.DueDate != null || valueOrDefault.CompleteDate != null)
					{
						throw new ServiceArgumentException((CoreResources.IDs)3784063568U);
					}
					item.ClearFlag();
				}
			}
		}
	}
}
