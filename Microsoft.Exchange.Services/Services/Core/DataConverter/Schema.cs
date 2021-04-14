using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal abstract class Schema
	{
		static Schema()
		{
			Schema.itemInformationList = new List<ObjectInformation>
			{
				Schema.CalendarItem,
				Schema.Contact,
				Schema.DistributionList,
				Schema.MeetingCancellation,
				Schema.MeetingMessage,
				Schema.MeetingRequest,
				Schema.MeetingResponse,
				Schema.Message,
				Schema.Item,
				Schema.Task,
				Schema.PostItem
			};
			Schema.folderInformationList = new List<ObjectInformation>
			{
				Schema.CalendarFolder,
				Schema.ContactsFolder,
				Schema.Folder,
				Schema.SearchFolder,
				Schema.OutlookSearchFolder,
				Schema.TasksFolder
			};
			Schema.itemInformationForFolderDictionary = new Dictionary<ObjectInformation, List<ObjectInformation>>
			{
				{
					Schema.CalendarFolder,
					new List<ObjectInformation>
					{
						Schema.CalendarItem,
						Schema.Item
					}
				},
				{
					Schema.ContactsFolder,
					new List<ObjectInformation>
					{
						Schema.Contact,
						Schema.DistributionList,
						Schema.Item
					}
				},
				{
					Schema.Folder,
					new List<ObjectInformation>
					{
						Schema.MeetingCancellation,
						Schema.MeetingMessage,
						Schema.MeetingRequest,
						Schema.MeetingResponse,
						Schema.Message,
						Schema.Item,
						Schema.PostItem
					}
				},
				{
					Schema.SearchFolder,
					Schema.itemInformationList
				},
				{
					Schema.OutlookSearchFolder,
					Schema.itemInformationList
				},
				{
					Schema.TasksFolder,
					new List<ObjectInformation>
					{
						Schema.Item,
						Schema.Task
					}
				}
			};
			Schema.dictionaryObjects = new List<ObjectInformation>();
			Schema.dictionaryObjects.AddRange(Schema.itemInformationList);
			Schema.dictionaryObjects.AddRange(Schema.folderInformationList);
		}

		public Schema(XmlElementInformation[] xmlElements, PropertyInformation itemIdPropertyInformation)
		{
			this.propertyInformationInXmlSchemaSequence = new List<PropertyInformation>();
			this.xmlElementInformationByPath = new Dictionary<string, XmlElementInformation>();
			this.propertyInformationByPath = new Dictionary<PropertyPath, PropertyInformation>();
			this.propertyInformationListByShapeEnum = new Dictionary<ShapeEnum, IList<PropertyInformation>>();
			IList<PropertyInformation> list = new List<PropertyInformation>();
			IList<PropertyInformation> list2 = new List<PropertyInformation>();
			foreach (XmlElementInformation xmlElementInformation in xmlElements)
			{
				PropertyInformation propertyInformation = xmlElementInformation as PropertyInformation;
				if (propertyInformation != null)
				{
					this.propertyInformationInXmlSchemaSequence.Add(propertyInformation);
					if (propertyInformation.PropertyPath != null)
					{
						this.propertyInformationByPath.Add(propertyInformation.PropertyPath, propertyInformation);
					}
					list.Add(propertyInformation);
				}
				if (xmlElementInformation.Path != null)
				{
					this.xmlElementInformationByPath.Add(xmlElementInformation.Path, xmlElementInformation);
				}
			}
			if (itemIdPropertyInformation != null)
			{
				list2.Add(itemIdPropertyInformation);
			}
			this.propertyInformationListByShapeEnum.Add(ShapeEnum.IdOnly, list2);
			this.propertyInformationListByShapeEnum.Add(ShapeEnum.AllProperties, list);
		}

		public Schema(XmlElementInformation[] xmlElements) : this(xmlElements, null)
		{
		}

		public IList<PropertyInformation> PropertyInformationInXmlSchemaSequence
		{
			get
			{
				return this.propertyInformationInXmlSchemaSequence;
			}
		}

		public bool TryGetXmlElementInformationByPath(string path, out XmlElementInformation xmlElementInformation)
		{
			return this.xmlElementInformationByPath.TryGetValue(path, out xmlElementInformation);
		}

		public bool TryGetPropertyInformationByPath(PropertyPath propertyPath, out PropertyInformation propertyInformation)
		{
			return this.propertyInformationByPath.TryGetValue(propertyPath, out propertyInformation);
		}

		public IList<PropertyInformation> GetPropertyInformationListByShapeEnum(ShapeEnum shapeEnum)
		{
			return this.propertyInformationListByShapeEnum[shapeEnum];
		}

		internal static ObjectInformation[] GetFolderInformation()
		{
			return Schema.folderInformationList.ToArray();
		}

		internal static ObjectInformation[] GetItemInformation()
		{
			return Schema.itemInformationList.ToArray();
		}

		internal static ObjectInformation[] GetItemInformationForFolder(ObjectInformation folderInfo)
		{
			ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>(0L, "ObjectInformation.GetItemInformationForFolder: folderInfo.LocalName = {0}", folderInfo.LocalName);
			List<ObjectInformation> list;
			if (!Schema.itemInformationForFolderDictionary.TryGetValue(folderInfo, out list))
			{
				list = Schema.itemInformationList;
			}
			ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<int>(0L, "ObjectInformation.GetItemInformationForFolder: results.Count = {0}", list.Count);
			return list.ToArray();
		}

		internal static ObjectInformation GetObjectInformation(StoreObject storeObject)
		{
			ObjectInformation objectInformation = null;
			Type typeFromHandle = typeof(object);
			Type type = storeObject.GetType();
			ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<Type>(0L, "ObjectInformation.GetObjectInformation: storeObject.GetType() = {0}", type);
			while (type != typeFromHandle && !Schema.objectInformationByType.Member.TryGetValue(type, out objectInformation))
			{
				type = type.GetTypeInfo().BaseType;
			}
			if (objectInformation == Schema.Folder)
			{
				if (storeObject.Id != null && IdConverter.GetAsStoreObjectId(storeObject.Id).ObjectType == StoreObjectType.TasksFolder)
				{
					ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "ObjectInformation.GetObjectInformation: Folder is TaskFolder. Folder replaced with TasksFolder");
					objectInformation = Schema.TasksFolder;
				}
			}
			else if (objectInformation == Schema.Message)
			{
				MessageItem messageItem = storeObject as MessageItem;
				if (!ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2007SP1) && Shape.IsGenericMessageOnly(messageItem))
				{
					ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "ObjectInformation.GetObjectInformation: MessageItem is generic. Message replaced with Item");
					objectInformation = Schema.Item;
				}
			}
			return objectInformation;
		}

		internal static ObjectInformation GetObjectInformation(StoreObjectType storeObjectType)
		{
			ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<StoreObjectType>(0L, "ObjectInformation.GetObjectInformation: storeObjectType = {0}", storeObjectType);
			ObjectInformation result = null;
			if (!Schema.objectInformationByStoreObjectType.Member.TryGetValue(storeObjectType, out result))
			{
				if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2007SP1))
				{
					ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "ObjectInformation.GetObjectInformation: ObjectInformation not found for storeObjectType.  Message used.");
					result = Schema.Message;
				}
				else
				{
					ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "ObjectInformation.GetObjectInformation: ObjectInformation not found for storeObjectType.  Item used.");
					result = Schema.Item;
				}
			}
			return result;
		}

		public static string BuildXmlElementPath(XmlElement xmlElement, string path)
		{
			if (path == Schema.RootXmlElementPath)
			{
				return ServiceXml.GetFullyQualifiedName(xmlElement.LocalName, xmlElement.NamespaceURI);
			}
			StringBuilder stringBuilder = new StringBuilder(path);
			stringBuilder.Append("/");
			stringBuilder.Append(xmlElement.LocalName);
			if (xmlElement.HasAttribute("Key"))
			{
				stringBuilder.Append("[@Key");
				string attribute = xmlElement.GetAttribute("Key");
				if (!string.IsNullOrEmpty(attribute))
				{
					stringBuilder.Append("='");
					stringBuilder.Append(attribute);
					stringBuilder.Append("'");
				}
				stringBuilder.Append("]");
			}
			return stringBuilder.ToString();
		}

		protected const PropertyInformationAttributes AppendUpdateCommand = PropertyInformationAttributes.ImplementsAppendUpdateCommand;

		protected const PropertyInformationAttributes DeleteUpdateCommand = PropertyInformationAttributes.ImplementsDeleteUpdateCommand;

		protected const PropertyInformationAttributes SetCommand = PropertyInformationAttributes.ImplementsSetCommand;

		protected const PropertyInformationAttributes SetUpdateCommand = PropertyInformationAttributes.ImplementsSetUpdateCommand;

		protected const PropertyInformationAttributes ToXmlCommand = PropertyInformationAttributes.ImplementsToXmlCommand;

		protected const PropertyInformationAttributes ToXmlForPropertyBagCommand = PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand;

		protected const PropertyInformationAttributes ToServiceObjectCommand = PropertyInformationAttributes.ImplementsToServiceObjectCommand;

		protected const PropertyInformationAttributes ToServiceObjectForPropertyBagCommand = PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand;

		public static string RootXmlElementPath = string.Empty;

		private static List<ObjectInformation> itemInformationList;

		private static List<ObjectInformation> folderInformationList;

		private static Dictionary<ObjectInformation, List<ObjectInformation>> itemInformationForFolderDictionary;

		private static List<ObjectInformation> dictionaryObjects;

		private static LazyMember<Dictionary<Type, ObjectInformation>> objectInformationByType = new LazyMember<Dictionary<Type, ObjectInformation>>(delegate()
		{
			Dictionary<Type, ObjectInformation> dictionary = new Dictionary<Type, ObjectInformation>();
			foreach (ObjectInformation objectInformation in Schema.dictionaryObjects)
			{
				if (objectInformation.AssociatedType != null)
				{
					dictionary.Add(objectInformation.AssociatedType, objectInformation);
				}
			}
			return dictionary;
		});

		private static LazyMember<Dictionary<StoreObjectType, ObjectInformation>> objectInformationByStoreObjectType = new LazyMember<Dictionary<StoreObjectType, ObjectInformation>>(delegate()
		{
			Dictionary<StoreObjectType, ObjectInformation> dictionary = new Dictionary<StoreObjectType, ObjectInformation>();
			foreach (ObjectInformation objectInformation in Schema.dictionaryObjects)
			{
				if (objectInformation.AssociatedStoreObjectTypes != null)
				{
					foreach (StoreObjectType key in objectInformation.AssociatedStoreObjectTypes)
					{
						dictionary.Add(key, objectInformation);
					}
				}
			}
			return dictionary;
		});

		private IList<PropertyInformation> propertyInformationInXmlSchemaSequence;

		private IDictionary<string, XmlElementInformation> xmlElementInformationByPath;

		private IDictionary<PropertyPath, PropertyInformation> propertyInformationByPath;

		private IDictionary<ShapeEnum, IList<PropertyInformation>> propertyInformationListByShapeEnum;

		internal static readonly ObjectInformation Item = new ObjectInformation("Item", ExchangeVersion.Exchange2007, typeof(Item), null, new Shape.CreateShapeCallback(ItemShape.CreateShape), ObjectInformation.NoPriorVersionObjectInformation);

		internal static readonly ObjectInformation Conversation = new ObjectInformation("Conversation", ExchangeVersion.Exchange2010SP1, null, null, new Shape.CreateShapeCallback(ConversationShape.CreateShape), ObjectInformation.NoPriorVersionObjectInformation);

		internal static readonly ObjectInformation Contact = new ObjectInformation("Contact", ExchangeVersion.Exchange2007, typeof(Contact), new StoreObjectType[]
		{
			StoreObjectType.Contact
		}, new Shape.CreateShapeCallback(ContactShape.CreateShape), ObjectInformation.NoPriorVersionObjectInformation);

		internal static readonly ObjectInformation DistributionList = new ObjectInformation("DistributionList", ExchangeVersion.Exchange2007, typeof(DistributionList), new StoreObjectType[]
		{
			StoreObjectType.DistributionList
		}, new Shape.CreateShapeCallback(DistributionListShape.CreateShape), ObjectInformation.NoPriorVersionObjectInformation);

		internal static readonly ObjectInformation CalendarItem = new ObjectInformation("CalendarItem", ExchangeVersion.Exchange2007, typeof(CalendarItemBase), new StoreObjectType[]
		{
			StoreObjectType.CalendarItem,
			StoreObjectType.CalendarItemOccurrence
		}, new Shape.CreateShapeCallback(CalendarItemShape.CreateShapeForAttendee), null, new Shape.CreateShapeForStoreObjectCallback(CalendarItemShape.CreateShape), ObjectInformation.NoPriorVersionObjectInformation);

		internal static readonly ObjectInformation CalendarItemOccurrenceException = new ObjectInformation("CalendarItemOccurrenceException", ExchangeVersion.Exchange2007, null, null, new Shape.CreateShapeCallback(CalendarItemShape.CreateShapeForAttendee), null, new Shape.CreateShapeForStoreObjectCallback(CalendarItemShape.CreateShape), ObjectInformation.NoPriorVersionObjectInformation);

		internal static readonly ObjectInformation MeetingCancellation = new ObjectInformation("MeetingCancellation", ExchangeVersion.Exchange2007, typeof(MeetingCancellation), new StoreObjectType[]
		{
			StoreObjectType.MeetingCancellation
		}, new Shape.CreateShapeCallback(MeetingCancellationShape.CreateShape), ObjectInformation.NoPriorVersionObjectInformation);

		internal static readonly ObjectInformation MeetingMessage = new ObjectInformation("MeetingMessage", ExchangeVersion.Exchange2007, typeof(MeetingMessage), new StoreObjectType[]
		{
			StoreObjectType.MeetingMessage
		}, new Shape.CreateShapeCallback(MeetingMessageShape.CreateShape), ObjectInformation.NoPriorVersionObjectInformation);

		internal static readonly ObjectInformation MeetingRequest = new ObjectInformation("MeetingRequest", ExchangeVersion.Exchange2007, typeof(MeetingRequest), new StoreObjectType[]
		{
			StoreObjectType.MeetingRequest
		}, new Shape.CreateShapeCallback(MeetingRequestShape.CreateShape), ObjectInformation.NoPriorVersionObjectInformation);

		internal static readonly ObjectInformation MeetingResponse = new ObjectInformation("MeetingResponse", ExchangeVersion.Exchange2007, typeof(MeetingResponse), new StoreObjectType[]
		{
			StoreObjectType.MeetingResponse
		}, new Shape.CreateShapeCallback(MeetingResponseShape.CreateShape), ObjectInformation.NoPriorVersionObjectInformation);

		internal static readonly ObjectInformation Message = new ObjectInformation("Message", ExchangeVersion.Exchange2007, typeof(MessageItem), new StoreObjectType[]
		{
			StoreObjectType.Message,
			StoreObjectType.Report,
			StoreObjectType.MeetingForwardNotification
		}, new Shape.CreateShapeCallback(MessageShape.CreateShape), new Shape.CreateShapeForPropertyBagCallback(MessageShape.CreateShapeForPropertyBag), null, ObjectInformation.NoPriorVersionObjectInformation);

		internal static readonly ObjectInformation Task = new ObjectInformation("Task", ExchangeVersion.Exchange2007, typeof(Task), new StoreObjectType[]
		{
			StoreObjectType.Task
		}, new Shape.CreateShapeCallback(TaskShape.CreateShape), ObjectInformation.NoPriorVersionObjectInformation);

		internal static readonly ObjectInformation PostItem = new ObjectInformation("PostItem", ExchangeVersion.Exchange2007SP1, typeof(PostItem), new StoreObjectType[]
		{
			StoreObjectType.Post
		}, new Shape.CreateShapeCallback(PostItemShape.CreateShape), Schema.Item);

		internal static readonly ObjectInformation DeliveryReport = new ObjectInformation("Message", ExchangeVersion.Exchange2007, null, null, new Shape.CreateShapeCallback(MessageShape.CreateShape), new Shape.CreateShapeForPropertyBagCallback(MessageShape.CreateShapeForPropertyBag), null, ObjectInformation.NoPriorVersionObjectInformation);

		internal static readonly ObjectInformation NonDeliveryReport = new ObjectInformation("Message", ExchangeVersion.Exchange2007, null, null, new Shape.CreateShapeCallback(MessageShape.CreateShape), new Shape.CreateShapeForPropertyBagCallback(MessageShape.CreateShapeForPropertyBag), null, ObjectInformation.NoPriorVersionObjectInformation);

		internal static readonly ObjectInformation Persona = new ObjectInformation("Persona", ExchangeVersion.Exchange2012, null, null, new Shape.CreateShapeCallback(PersonaShape.CreateShape), ObjectInformation.NoPriorVersionObjectInformation);

		internal static readonly ObjectInformation ReadReceipt = new ObjectInformation("Message", ExchangeVersion.Exchange2007, null, null, new Shape.CreateShapeCallback(MessageShape.CreateShape), new Shape.CreateShapeForPropertyBagCallback(MessageShape.CreateShapeForPropertyBag), null, ObjectInformation.NoPriorVersionObjectInformation);

		internal static readonly ObjectInformation NonReadReceipt = new ObjectInformation("Message", ExchangeVersion.Exchange2007, null, null, new Shape.CreateShapeCallback(MessageShape.CreateShape), new Shape.CreateShapeForPropertyBagCallback(MessageShape.CreateShapeForPropertyBag), null, ObjectInformation.NoPriorVersionObjectInformation);

		internal static readonly ObjectInformation Folder = new ObjectInformation("Folder", ExchangeVersion.Exchange2007, typeof(Folder), new StoreObjectType[]
		{
			StoreObjectType.Folder,
			StoreObjectType.JournalFolder,
			StoreObjectType.NotesFolder
		}, new Shape.CreateShapeCallback(FolderShape.CreateShape), ObjectInformation.NoPriorVersionObjectInformation);

		internal static readonly ObjectInformation ContactsFolder = new ObjectInformation("ContactsFolder", ExchangeVersion.Exchange2007, typeof(ContactsFolder), new StoreObjectType[]
		{
			StoreObjectType.ContactsFolder
		}, new Shape.CreateShapeCallback(ContactsFolderShape.CreateShape), ObjectInformation.NoPriorVersionObjectInformation);

		internal static readonly ObjectInformation CalendarFolder = new ObjectInformation("CalendarFolder", ExchangeVersion.Exchange2007, typeof(CalendarFolder), new StoreObjectType[]
		{
			StoreObjectType.CalendarFolder
		}, new Shape.CreateShapeCallback(CalendarFolderShape.CreateShape), ObjectInformation.NoPriorVersionObjectInformation);

		internal static readonly ObjectInformation SearchFolder = new ObjectInformation("SearchFolder", ExchangeVersion.Exchange2007, typeof(SearchFolder), new StoreObjectType[]
		{
			StoreObjectType.SearchFolder
		}, new Shape.CreateShapeCallback(SearchFolderShape.CreateShape), ObjectInformation.NoPriorVersionObjectInformation);

		internal static readonly ObjectInformation OutlookSearchFolder = new ObjectInformation("OutlookSearchFolder", ExchangeVersion.Exchange2010, typeof(OutlookSearchFolder), new StoreObjectType[]
		{
			StoreObjectType.OutlookSearchFolder
		}, new Shape.CreateShapeCallback(SearchFolderShape.CreateShape), Schema.SearchFolder);

		internal static readonly ObjectInformation TasksFolder = new ObjectInformation("TasksFolder", ExchangeVersion.Exchange2007, null, new StoreObjectType[]
		{
			StoreObjectType.TasksFolder
		}, new Shape.CreateShapeCallback(TasksFolderShape.CreateShape), ObjectInformation.NoPriorVersionObjectInformation);
	}
}
