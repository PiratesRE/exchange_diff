using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class UpdatePropertyList : PropertyList
	{
		public UpdatePropertyList(Shape shape, PropertyUpdate[] propertyUpdates, StoreObject storeObject, IdConverter idConverter, bool suppressReadReceipts, IFeaturesManager featuresManager) : base(shape)
		{
			this.propertyUpdates = propertyUpdates;
			this.storeObject = storeObject;
			this.CreatePropertyList(idConverter, suppressReadReceipts, featuresManager);
		}

		private static void ValidateUpdate(PropertyUpdate propertyUpdate, PropertyInformation propertyInformation)
		{
			SetPropertyUpdate setPropertyUpdate = null;
			AppendPropertyUpdate setPropertyUpdate2 = null;
			DeletePropertyUpdate deletePropertyUpdate = null;
			if (UpdatePropertyList.TryGetPropertyUpdate<AppendPropertyUpdate>(propertyUpdate, out setPropertyUpdate2))
			{
				if (propertyInformation.ImplementsAppendUpdateCommand)
				{
					UpdatePropertyList.ValidatePropertyUpdate(setPropertyUpdate2, propertyInformation);
					return;
				}
				throw new InvalidPropertyAppendException(propertyInformation.PropertyPath);
			}
			else if (UpdatePropertyList.TryGetPropertyUpdate<SetPropertyUpdate>(propertyUpdate, out setPropertyUpdate))
			{
				if (propertyInformation.ImplementsSetUpdateCommand)
				{
					UpdatePropertyList.ValidatePropertyUpdate(setPropertyUpdate, propertyInformation);
					return;
				}
				throw new InvalidPropertySetException(propertyInformation.PropertyPath);
			}
			else
			{
				if (UpdatePropertyList.TryGetPropertyUpdate<DeletePropertyUpdate>(propertyUpdate, out deletePropertyUpdate) && !propertyInformation.ImplementsDeleteUpdateCommand)
				{
					throw new InvalidPropertyDeleteException(propertyInformation.PropertyPath);
				}
				return;
			}
		}

		private static void ValidatePropertyUpdate(SetPropertyUpdate setPropertyUpdate, PropertyInformation propertyInformation)
		{
			ServiceObject serviceObject = setPropertyUpdate.ServiceObject;
			List<PropertyInformation> loadedProperties = serviceObject.LoadedProperties;
			if (loadedProperties.Count != 1)
			{
				throw new IncorrectUpdatePropertyCountException();
			}
			if (string.CompareOrdinal(loadedProperties[0].LocalName, propertyInformation.LocalName) != 0)
			{
				throw new UpdatePropertyMismatchException(setPropertyUpdate.PropertyPath);
			}
		}

		private void CreatePropertyList(IdConverter idConverter, bool suppressReadReceipts, IFeaturesManager featuresManager)
		{
			this.commandContexts = new List<CommandContext>();
			foreach (PropertyUpdate propertyUpdate in this.propertyUpdates)
			{
				PropertyInformation propertyInformation = null;
				if (!this.shape.TryGetPropertyInformation(propertyUpdate.PropertyPath, out propertyInformation))
				{
					throw new InvalidPropertyRequestException(propertyUpdate.PropertyPath);
				}
				UpdatePropertyList.ValidateUpdate(propertyUpdate, propertyInformation);
				this.commandContexts.Add(new CommandContext(new UpdateCommandSettings(propertyUpdate, this.storeObject, suppressReadReceipts, featuresManager), propertyInformation, idConverter));
			}
		}

		public static bool TryGetPropertyUpdate<T>(PropertyUpdate propertyUpdate, out T typedPropertyUpdate) where T : PropertyUpdate
		{
			typedPropertyUpdate = (propertyUpdate as T);
			return typedPropertyUpdate != null;
		}

		public IList<IUpdateCommand> CreatePropertyCommands()
		{
			List<IUpdateCommand> list = new List<IUpdateCommand>();
			foreach (CommandContext commandContext in this.commandContexts)
			{
				if (!(this.storeObject is CalendarItemBase) || commandContext.PropertyInformation != ItemSchema.MimeContent)
				{
					list.Add((IUpdateCommand)commandContext.PropertyInformation.CreatePropertyCommand(commandContext));
				}
			}
			return list;
		}

		private List<CommandContext> commandContexts;

		private PropertyUpdate[] propertyUpdates;

		private StoreObject storeObject;
	}
}
