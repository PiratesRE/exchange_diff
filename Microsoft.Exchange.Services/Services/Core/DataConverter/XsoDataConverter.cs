using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal static class XsoDataConverter
	{
		public static ToXmlPropertyList GetPropertyList(StoreObject storeObject, ResponseShape responseShape)
		{
			return PropertyList.CreateToXmlPropertyList(responseShape, storeObject);
		}

		public static ToServiceObjectPropertyList GetToServiceObjectPropertyList(StoreObject storeObject, ResponseShape responseShape)
		{
			return PropertyList.CreateToServiceObjectPropertyList(responseShape, storeObject);
		}

		public static ToServiceObjectPropertyListInMemory GetToServiceObjectPropertyListInMemory(StoreObject storeObject, ResponseShape responseShape)
		{
			return PropertyList.CreateToServiceObjectPropertyListInMemory(responseShape, storeObject);
		}

		public static ToServiceObjectPropertyList GetToServiceObjectPropertyListForPropertyBagUsingStoreObject(StoreObjectId storeObjectId, ResponseShape responseShape, IParticipantResolver participantResolver)
		{
			ObjectInformation objectInformation = Schema.GetObjectInformation(storeObjectId.ObjectType);
			return PropertyList.CreateToServiceObjectForPropertyBagUsingStoreObject(responseShape, objectInformation, participantResolver);
		}

		public static ToXmlPropertyList GetPropertyList(ObjectInformation objectInformation, ResponseShape responseShape)
		{
			return PropertyList.CreateToXmlPropertyList(responseShape, objectInformation);
		}

		public static ToServiceObjectPropertyList GetToServiceObjectPropertyList(ObjectInformation objectInformation, ResponseShape responseShape, IParticipantResolver participantResolver)
		{
			return PropertyList.CreateToServiceObjectPropertyList(responseShape, objectInformation, participantResolver);
		}

		public static ToXmlPropertyList GetPropertyList(StoreId storeId, StoreSession session, ResponseShape responseShape)
		{
			StoreObjectId asStoreObjectId = IdConverter.GetAsStoreObjectId(storeId);
			ObjectInformation objectInformation = Schema.GetObjectInformation(asStoreObjectId.ObjectType);
			ToXmlPropertyList propertyList;
			try
			{
				propertyList = XsoDataConverter.GetPropertyList(objectInformation, responseShape);
			}
			catch (InvalidPropertyRequestException)
			{
				propertyList = XsoDataConverter.GetPropertyList(objectInformation, new ItemResponseShape(ShapeEnum.IdOnly, BodyResponseType.Best, false, responseShape.AdditionalProperties));
			}
			return propertyList;
		}

		public static ToServiceObjectPropertyList GetToServiceObjectPropertyList(StoreId storeId, StoreSession session, ResponseShape responseShape, IParticipantResolver participantResolver)
		{
			StoreObjectId asStoreObjectId = IdConverter.GetAsStoreObjectId(storeId);
			ObjectInformation objectInformation = Schema.GetObjectInformation(asStoreObjectId.ObjectType);
			ToServiceObjectPropertyList toServiceObjectPropertyList;
			try
			{
				toServiceObjectPropertyList = XsoDataConverter.GetToServiceObjectPropertyList(objectInformation, responseShape, participantResolver);
			}
			catch (InvalidPropertyRequestException)
			{
				StoreObjectType objectType = IdConverter.GetAsStoreObjectId(storeId).ObjectType;
				using (StoreObject storeObject = (responseShape is FolderResponseShape) ? Folder.Bind(session, storeId, null) : ServiceCommandBase.GetXsoItem(session, storeId, new PropertyDefinition[0]))
				{
					XsoDataConverter.VerifyObjectTypeAssumptions(objectType, storeObject);
				}
				throw;
			}
			return toServiceObjectPropertyList;
		}

		public static void VerifyObjectTypeAssumptions(StoreObjectType expectedType, StoreObject actualObject)
		{
			StoreObjectType objectType = actualObject.Id.ObjectId.ObjectType;
			if (expectedType == StoreObjectType.Folder && IdConverter.IsFolderObjectType(objectType))
			{
				return;
			}
			if (expectedType == StoreObjectType.Unknown)
			{
				return;
			}
			if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2007SP1) && objectType == StoreObjectType.Unknown && expectedType == StoreObjectType.Message)
			{
				return;
			}
			if (objectType != expectedType)
			{
				throw new ObjectNotFoundException(ServerStrings.ConflictingObjectType((int)expectedType, (int)objectType));
			}
		}

		public static ToXmlForPropertyBagPropertyList GetPropertyListForPropertyBag(ResponseShape responseShape, ObjectInformation objectInformation)
		{
			return PropertyList.CreateToXmlForPropertyBagPropertyList(responseShape, objectInformation);
		}

		public static ToServiceObjectForPropertyBagPropertyList GetServiceObjectPropertyListForPropertyBag(ResponseShape responseShape, ObjectInformation objectInformation)
		{
			return PropertyList.CreateToServiceObjectForPropertyBagPropertyList(responseShape, objectInformation);
		}

		public static void SetProperties(StoreObject storeObject, ServiceObject serviceObject, IdConverter idConverter)
		{
			IList<ISetCommand> list = PropertyList.CreateSetPropertyCommands(serviceObject, storeObject, idConverter);
			foreach (ISetCommand setCommand in list)
			{
				setCommand.Set();
			}
			foreach (ISetCommand setCommand2 in list)
			{
				setCommand2.SetPhase2();
			}
			foreach (ISetCommand setCommand3 in list)
			{
				setCommand3.SetPhase3();
			}
		}

		public static void UpdateProperties(StoreObject storeObject, PropertyUpdate[] propertyUpdates, IdConverter idConverter, bool suppressReadReceipts, IFeaturesManager featuresManager)
		{
			IList<IUpdateCommand> list = PropertyList.CreateUpdatePropertyCommands(propertyUpdates, storeObject, idConverter, suppressReadReceipts, featuresManager);
			foreach (IUpdateCommand updateCommand in list)
			{
				updateCommand.Update();
			}
			foreach (IUpdateCommand updateCommand2 in list)
			{
				updateCommand2.PostUpdate();
			}
		}

		public static bool TryGetStoreObject<T>(StoreObject storeObject, out T typedStoreObject) where T : StoreObject
		{
			typedStoreObject = (storeObject as T);
			return typedStoreObject != null;
		}

		public static void SetProperties(StoreObject storeObject, XmlElement serviceItem, IdConverter idConverter)
		{
			IList<ISetCommand> list = PropertyList.CreateSetPropertyCommands(serviceItem, storeObject, idConverter);
			foreach (ISetCommand setCommand in list)
			{
				setCommand.Set();
			}
			foreach (ISetCommand setCommand2 in list)
			{
				setCommand2.SetPhase2();
			}
			foreach (ISetCommand setCommand3 in list)
			{
				setCommand3.SetPhase3();
			}
		}
	}
}
