using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal abstract class ItemIdPropertyBase : ComplexPropertyBase, IToXmlCommand, IToXmlForPropertyBagCommand, IToServiceObjectCommand, IToServiceObjectForPropertyBagCommand, IPropertyCommand
	{
		public ItemIdPropertyBase(CommandContext commandContext) : base(commandContext)
		{
		}

		protected virtual StoreId GetIdFromObject(StoreObject storeObject)
		{
			return storeObject.Id;
		}

		internal abstract ServiceObjectId CreateServiceObjectId(string id, string changeKey);

		internal abstract Array CreateServiceObjectidArray(List<ConcatenatedIdAndChangeKey> ids);

		public virtual void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			StoreId idFromObject = this.GetIdFromObject(storeObject);
			if (idFromObject != null && IdConverter.GetAsStoreObjectId(idFromObject).ProviderLevelItemId.Length > 0)
			{
				ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(idFromObject, commandSettings.IdAndSession, null);
				serviceObject[propertyInformation] = this.CreateServiceObjectId(concatenatedId.Id, concatenatedId.ChangeKey);
			}
		}

		public virtual void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			IdAndSession idAndSession = commandSettings.IdAndSession;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			ArrayPropertyInformation arrayPropertyInformation = propertyInformation as ArrayPropertyInformation;
			if (arrayPropertyInformation != null)
			{
				object obj = null;
				if (PropertyCommand.TryGetValueFromPropertyBag<object>(propertyBag, this.propertyDefinitions[0], out obj))
				{
					List<ConcatenatedIdAndChangeKey> list = new List<ConcatenatedIdAndChangeKey>();
					Array array = obj as Array;
					if (array != null)
					{
						using (IEnumerator enumerator = array.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								object obj2 = enumerator.Current;
								StoreId storeId = obj2 as StoreId;
								ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(storeId, idAndSession, null);
								list.Add(concatenatedId);
							}
							goto IL_FC;
						}
					}
					ExTraceGlobals.ItemDataTracer.TraceDebug<string>((long)this.GetHashCode(), "[ItemIdPropertyBase::ToServiceObjectForPropertyBag] Unexpectedly got non-array property {0}", obj.ToString());
					StoreId storeId2 = null;
					if (PropertyCommand.TryGetValueFromPropertyBag<StoreId>(propertyBag, this.propertyDefinitions[0], out storeId2))
					{
						ConcatenatedIdAndChangeKey concatenatedId2 = IdConverter.GetConcatenatedId(storeId2, idAndSession, null);
						list.Add(concatenatedId2);
					}
					IL_FC:
					serviceObject[propertyInformation] = this.CreateServiceObjectidArray(list);
					return;
				}
			}
			else
			{
				StoreId storeId3 = null;
				if (PropertyCommand.TryGetValueFromPropertyBag<StoreId>(propertyBag, this.propertyDefinitions[0], out storeId3))
				{
					ConcatenatedIdAndChangeKey concatenatedId3 = IdConverter.GetConcatenatedId(storeId3, idAndSession, null);
					serviceObject[propertyInformation] = this.CreateServiceObjectId(concatenatedId3.Id, concatenatedId3.ChangeKey);
				}
			}
		}

		public void ToXml()
		{
			ToXmlCommandSettings commandSettings = base.GetCommandSettings<ToXmlCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			XmlElement serviceItem = commandSettings.ServiceItem;
			StoreSession session = storeObject.Session;
			IdAndSession idAndSession = commandSettings.IdAndSession;
			StoreId idFromObject = this.GetIdFromObject(storeObject);
			if (idFromObject != null && IdConverter.GetAsStoreObjectId(idFromObject).ProviderLevelItemId.Length > 0)
			{
				this.ToXmlFromStoreId(serviceItem, idFromObject, idAndSession);
			}
		}

		public void ToXmlForPropertyBag()
		{
			ToXmlForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToXmlForPropertyBagCommandSettings>();
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			XmlElement serviceItem = commandSettings.ServiceItem;
			IdAndSession idAndSession = commandSettings.IdAndSession;
			ArrayPropertyInformation arrayPropertyInformation = this.commandContext.PropertyInformation as ArrayPropertyInformation;
			if (arrayPropertyInformation != null)
			{
				object propertyValue = null;
				if (PropertyCommand.TryGetValueFromPropertyBag<object>(propertyBag, this.propertyDefinitions[0], out propertyValue))
				{
					this.ToXmlFromArrayOfStoreId(serviceItem, propertyValue, idAndSession, arrayPropertyInformation);
					return;
				}
			}
			else
			{
				StoreId storeId = null;
				if (PropertyCommand.TryGetValueFromPropertyBag<StoreId>(propertyBag, this.propertyDefinitions[0], out storeId))
				{
					this.ToXmlFromStoreId(serviceItem, storeId, idAndSession);
				}
			}
		}

		protected void ToXmlFromStoreId(XmlElement serviceItem, StoreId storeId, IdAndSession idAndSession)
		{
			IdConverter.CreateStoreIdXml(serviceItem, storeId, idAndSession, this.xmlLocalName);
		}

		protected void ToXmlFromArrayOfStoreId(XmlElement serviceItem, object propertyValue, IdAndSession idAndSession, ArrayPropertyInformation arrayPropertyInformation)
		{
			XmlElement idParentElement = base.CreateXmlElement(serviceItem, this.xmlLocalName);
			Array array = propertyValue as Array;
			if (array != null)
			{
				using (IEnumerator enumerator = array.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						StoreId storeId = (StoreId)obj;
						IdConverter.CreateStoreIdXml(idParentElement, storeId, idAndSession, arrayPropertyInformation.ArrayItemLocalName);
					}
					return;
				}
			}
			ExTraceGlobals.ItemDataTracer.TraceDebug<string>((long)this.GetHashCode(), "[ItemIdPropertyBase::ToXmlFromStoreId] Unexpectedly got non-array property {0}", propertyValue.ToString());
			StoreId storeId2 = propertyValue as StoreId;
			IdConverter.CreateStoreIdXml(idParentElement, storeId2, idAndSession, arrayPropertyInformation.ArrayItemLocalName);
		}
	}
}
