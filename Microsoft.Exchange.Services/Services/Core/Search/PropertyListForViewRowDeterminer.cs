using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.Search
{
	internal class PropertyListForViewRowDeterminer
	{
		private PropertyListForViewRowDeterminer(StorePropertyDefinition idPropertyDefinition, Dictionary<ObjectInformation, ToXmlForPropertyBagPropertyList> xmlMapping, Dictionary<ObjectInformation, ToServiceObjectForPropertyBagPropertyList> serviceObjectMapping) : this(idPropertyDefinition, xmlMapping, serviceObjectMapping, xmlMapping.Values)
		{
		}

		private PropertyListForViewRowDeterminer(StorePropertyDefinition idPropertyDefinition, Dictionary<ObjectInformation, ToXmlForPropertyBagPropertyList> xmlMapping, Dictionary<ObjectInformation, ToServiceObjectForPropertyBagPropertyList> serviceObjectMapping, IEnumerable<ToXmlForPropertyBagPropertyList> propertyLists)
		{
			this.idPropertyDefinition = idPropertyDefinition;
			this.objectTypeToPropertyListMap = xmlMapping;
			this.objectTypeToServiceObjectPropertyListMap = serviceObjectMapping;
			this.propsToFetch = new List<PropertyDefinition>();
			foreach (ToXmlForPropertyBagPropertyList toXmlForPropertyBagPropertyList in propertyLists)
			{
				PropertyDefinition[] propertyDefinitions = toXmlForPropertyBagPropertyList.GetPropertyDefinitions();
				PropertyListForViewRowDeterminer.AddUniquePropertyDefinitions(propertyDefinitions, this.propsToFetch);
			}
		}

		private static void AddUniquePropertyDefinitions(IList<PropertyDefinition> source, IList<PropertyDefinition> dest)
		{
			foreach (PropertyDefinition item in source)
			{
				if (dest.IndexOf(item) == -1)
				{
					dest.Add(item);
				}
			}
		}

		internal PropertyDefinition[] GetPropertiesToFetch()
		{
			return this.propsToFetch.ToArray();
		}

		internal ToXmlForPropertyBagPropertyList GetPropertyList(IDictionary<PropertyDefinition, object> view)
		{
			StoreId storeId = null;
			PropertyCommand.TryGetValueFromPropertyBag<StoreId>(view, this.idPropertyDefinition, out storeId);
			if (storeId is ConversationId)
			{
				return this.objectTypeToPropertyListMap[Schema.Conversation];
			}
			if (storeId is PersonId)
			{
				return this.objectTypeToPropertyListMap[Schema.Persona];
			}
			StoreObjectId asStoreObjectId = IdConverter.GetAsStoreObjectId(storeId);
			StoreObjectType objectType = asStoreObjectId.ObjectType;
			ObjectInformation objectInformation = Schema.GetObjectInformation(objectType);
			ToXmlForPropertyBagPropertyList result = null;
			this.objectTypeToPropertyListMap.TryGetValue(objectInformation, out result);
			return result;
		}

		internal ToServiceObjectForPropertyBagPropertyList GetToServiceObjectPropertyList(IDictionary<PropertyDefinition, object> view, out StoreObjectType storeObjectType)
		{
			StoreId storeId = null;
			PropertyCommand.TryGetValueFromPropertyBag<StoreId>(view, this.idPropertyDefinition, out storeId);
			storeObjectType = StoreObjectType.Unknown;
			if (storeId is ConversationId)
			{
				return this.objectTypeToServiceObjectPropertyListMap[Schema.Conversation];
			}
			if (storeId is PersonId)
			{
				return this.objectTypeToServiceObjectPropertyListMap[Schema.Persona];
			}
			StoreObjectId asStoreObjectId = IdConverter.GetAsStoreObjectId(storeId);
			storeObjectType = asStoreObjectId.ObjectType;
			ObjectInformation objectInformation = Schema.GetObjectInformation(storeObjectType);
			ToServiceObjectForPropertyBagPropertyList result = null;
			this.objectTypeToServiceObjectPropertyListMap.TryGetValue(objectInformation, out result);
			return result;
		}

		internal ToServiceObjectForPropertyBagPropertyList GetToServiceObjectPropertyListForConversation()
		{
			return this.objectTypeToServiceObjectPropertyListMap[Schema.Conversation];
		}

		internal static PropertyListForViewRowDeterminer BuildForFolders(ResponseShape responseShape)
		{
			Dictionary<ObjectInformation, ToXmlForPropertyBagPropertyList> xmlMapping = new Dictionary<ObjectInformation, ToXmlForPropertyBagPropertyList>();
			Dictionary<ObjectInformation, ToServiceObjectForPropertyBagPropertyList> serviceObjectMapping = new Dictionary<ObjectInformation, ToServiceObjectForPropertyBagPropertyList>();
			ObjectInformation[] folderInformation = Schema.GetFolderInformation();
			foreach (ObjectInformation objectInformation in folderInformation)
			{
				PropertyListForViewRowDeterminer.AddMapping(xmlMapping, serviceObjectMapping, responseShape, objectInformation);
			}
			return new PropertyListForViewRowDeterminer(FolderSchema.Id, xmlMapping, serviceObjectMapping);
		}

		internal static PropertyListForViewRowDeterminer BuildForItems(ResponseShape responseShape, Folder folder)
		{
			Dictionary<ObjectInformation, ToXmlForPropertyBagPropertyList> xmlMapping = new Dictionary<ObjectInformation, ToXmlForPropertyBagPropertyList>();
			Dictionary<ObjectInformation, ToServiceObjectForPropertyBagPropertyList> serviceObjectMapping = new Dictionary<ObjectInformation, ToServiceObjectForPropertyBagPropertyList>();
			ObjectInformation[] itemInformation = Schema.GetItemInformation();
			foreach (ObjectInformation objectInformation in itemInformation)
			{
				PropertyListForViewRowDeterminer.AddMapping(xmlMapping, serviceObjectMapping, responseShape, objectInformation);
			}
			List<ToXmlForPropertyBagPropertyList> list = new List<ToXmlForPropertyBagPropertyList>();
			ObjectInformation objectInformation2 = Schema.GetObjectInformation(folder);
			ObjectInformation[] itemInformationForFolder = Schema.GetItemInformationForFolder(objectInformation2);
			foreach (ObjectInformation objectInformation3 in itemInformationForFolder)
			{
				list.Add(XsoDataConverter.GetPropertyListForPropertyBag(responseShape, objectInformation3));
			}
			if (responseShape.AdditionalProperties != null && responseShape.AdditionalProperties.Length > 0)
			{
				ResponseShape responseShape2 = new ItemResponseShape
				{
					BaseShape = ShapeEnum.IdOnly,
					AdditionalProperties = responseShape.AdditionalProperties
				};
				foreach (ObjectInformation objectInformation4 in itemInformation.Except(itemInformationForFolder))
				{
					list.Add(XsoDataConverter.GetPropertyListForPropertyBag(responseShape2, objectInformation4));
				}
			}
			return new PropertyListForViewRowDeterminer(ItemSchema.Id, xmlMapping, serviceObjectMapping, list);
		}

		internal static PropertyListForViewRowDeterminer BuildForConversation(ResponseShape responseShape)
		{
			ObjectInformation conversation = Schema.Conversation;
			Dictionary<ObjectInformation, ToXmlForPropertyBagPropertyList> dictionary = new Dictionary<ObjectInformation, ToXmlForPropertyBagPropertyList>();
			ToXmlForPropertyBagPropertyList value = PropertyList.CreateToXmlForPropertyBagPropertyList(responseShape, conversation);
			dictionary.Add(conversation, value);
			Dictionary<ObjectInformation, ToServiceObjectForPropertyBagPropertyList> dictionary2 = new Dictionary<ObjectInformation, ToServiceObjectForPropertyBagPropertyList>();
			ToServiceObjectForPropertyBagPropertyList value2 = PropertyList.CreateToServiceObjectForPropertyBagPropertyList(responseShape, conversation);
			dictionary2.Add(conversation, value2);
			return new PropertyListForViewRowDeterminer((StorePropertyDefinition)ConversationItemSchema.ConversationId, dictionary, dictionary2);
		}

		internal static PropertyListForViewRowDeterminer BuildForPersonObjects(PersonaResponseShape responseShape)
		{
			ObjectInformation persona = Schema.Persona;
			Dictionary<ObjectInformation, ToXmlForPropertyBagPropertyList> dictionary = new Dictionary<ObjectInformation, ToXmlForPropertyBagPropertyList>();
			ToXmlForPropertyBagPropertyList value = PropertyList.CreateToXmlForPropertyBagPropertyList(responseShape, persona);
			dictionary.Add(persona, value);
			Dictionary<ObjectInformation, ToServiceObjectForPropertyBagPropertyList> dictionary2 = new Dictionary<ObjectInformation, ToServiceObjectForPropertyBagPropertyList>();
			ToServiceObjectForPropertyBagPropertyList propertyListForPersonaResponseShape = Persona.GetPropertyListForPersonaResponseShape(responseShape);
			dictionary2.Add(persona, propertyListForPersonaResponseShape);
			return new PropertyListForViewRowDeterminer(PersonSchema.Id, dictionary, dictionary2);
		}

		private static void AddMapping(Dictionary<ObjectInformation, ToXmlForPropertyBagPropertyList> xmlMapping, Dictionary<ObjectInformation, ToServiceObjectForPropertyBagPropertyList> serviceObjectMapping, ResponseShape responseShape, ObjectInformation objectInformation)
		{
			ToXmlForPropertyBagPropertyList propertyListForPropertyBag = XsoDataConverter.GetPropertyListForPropertyBag(responseShape, objectInformation);
			xmlMapping.Add(objectInformation, propertyListForPropertyBag);
			ToServiceObjectForPropertyBagPropertyList serviceObjectPropertyListForPropertyBag = XsoDataConverter.GetServiceObjectPropertyListForPropertyBag(responseShape, objectInformation);
			serviceObjectMapping.Add(objectInformation, serviceObjectPropertyListForPropertyBag);
		}

		private Dictionary<ObjectInformation, ToXmlForPropertyBagPropertyList> objectTypeToPropertyListMap;

		private Dictionary<ObjectInformation, ToServiceObjectForPropertyBagPropertyList> objectTypeToServiceObjectPropertyListMap;

		private List<PropertyDefinition> propsToFetch;

		private StorePropertyDefinition idPropertyDefinition;
	}
}
