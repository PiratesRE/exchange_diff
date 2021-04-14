using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal abstract class PropertyList
	{
		protected PropertyList()
		{
		}

		public PropertyList(Shape shape)
		{
			this.shape = shape;
		}

		public static ToXmlPropertyList CreateToXmlPropertyList(ResponseShape responseShape, StoreObject storeObject)
		{
			Shape shape = ObjectInformation.CreateShape(storeObject);
			return new ToXmlPropertyList(shape, responseShape);
		}

		public static ToServiceObjectPropertyList CreateToServiceObjectPropertyList(ResponseShape responseShape, StoreObject storeObject)
		{
			Shape shape = ObjectInformation.CreateShape(storeObject);
			return new ToServiceObjectPropertyList(shape, responseShape, StaticParticipantResolver.DefaultInstance);
		}

		public static ToServiceObjectPropertyListInMemory CreateToServiceObjectPropertyListInMemory(ResponseShape responseShape, StoreObject storeObject)
		{
			Shape shape = ObjectInformation.CreateShape(storeObject);
			return new ToServiceObjectPropertyListInMemory(shape, responseShape);
		}

		public static ToXmlPropertyList CreateToXmlPropertyList(ResponseShape responseShape, ObjectInformation objectInformation)
		{
			Shape shape = objectInformation.CreateShape(false);
			return new ToXmlPropertyList(shape, responseShape);
		}

		public static ToServiceObjectPropertyList CreateToServiceObjectPropertyList(ResponseShape responseShape, ObjectInformation objectInformation, IParticipantResolver participantResolver)
		{
			Shape shape = objectInformation.CreateShape(false);
			return new ToServiceObjectPropertyList(shape, responseShape, participantResolver);
		}

		public static ToXmlForPropertyBagPropertyList CreateToXmlForPropertyBagPropertyList(ResponseShape responseShape, ObjectInformation objectInformation)
		{
			Shape shape = objectInformation.CreateShape(true);
			return new ToXmlForPropertyBagPropertyList(shape, responseShape);
		}

		public static ToServiceObjectForPropertyBagPropertyList CreateToServiceObjectForPropertyBagPropertyList(ResponseShape responseShape, ObjectInformation objectInformation)
		{
			Shape shape = objectInformation.CreateShape(true);
			return new ToServiceObjectForPropertyBagPropertyList(shape, responseShape);
		}

		public static ToXmlForPropertyBagUsingStoreObject CreateToXmlForPropertyBagUsingStoreObject(ResponseShape responseShape, ObjectInformation objectInformation)
		{
			Shape shape = objectInformation.CreateShape(true);
			return new ToXmlForPropertyBagUsingStoreObject(shape, responseShape);
		}

		public static ToServiceObjectForPropertyBagUsingStoreObject CreateToServiceObjectForPropertyBagUsingStoreObject(ResponseShape responseShape, ObjectInformation objectInformation, IParticipantResolver participantResolver)
		{
			Shape shape = objectInformation.CreateShape(true);
			return new ToServiceObjectForPropertyBagUsingStoreObject(shape, responseShape, participantResolver);
		}

		public static IList<ISetCommand> CreateSetPropertyCommands(ServiceObject serviceObject, StoreObject storeObject, IdConverter idConverter)
		{
			Shape shape = ObjectInformation.CreateShape(storeObject);
			SetPropertyList setPropertyList = new SetPropertyList(shape, serviceObject, storeObject, idConverter);
			return setPropertyList.CreatePropertyCommands();
		}

		public static IList<IUpdateCommand> CreateUpdatePropertyCommands(PropertyUpdate[] propertyUpdates, StoreObject storeObject, IdConverter idConverter, bool suppressReadReceipts, IFeaturesManager featuresManager)
		{
			Shape shape = ObjectInformation.CreateShape(storeObject);
			UpdatePropertyList updatePropertyList = new UpdatePropertyList(shape, propertyUpdates, storeObject, idConverter, suppressReadReceipts, featuresManager);
			return updatePropertyList.CreatePropertyCommands();
		}

		public static IList<ISetCommand> CreateSetPropertyCommands(XmlElement serviceItem, StoreObject storeObject, IdConverter idConverter)
		{
			Shape shape = ObjectInformation.CreateShape(storeObject);
			SetPropertyList setPropertyList = new SetPropertyList(shape, serviceItem, storeObject, idConverter);
			return setPropertyList.CreatePropertyCommands();
		}

		protected Shape shape;
	}
}
