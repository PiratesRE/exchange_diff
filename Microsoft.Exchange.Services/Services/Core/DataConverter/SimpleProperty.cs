using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class SimpleProperty : PropertyCommand, IToXmlCommand, IToXmlForPropertyBagCommand, ISetCommand, ISetUpdateCommand, IDeleteUpdateCommand, IUpdateCommand, IToServiceObjectCommand, IToServiceObjectForPropertyBagCommand, IPropertyCommand
	{
		public SimpleProperty(CommandContext commandContext, BaseConverter converter) : base(commandContext)
		{
			this.Initialize(commandContext, converter);
		}

		public SimpleProperty(CommandContext commandContext) : base(commandContext)
		{
			this.Initialize(commandContext, BaseConverter.GetConverterForPropertyDefinition(commandContext.GetPropertyDefinitions()[0]));
		}

		public SimpleProperty(CommandContext commandContext, bool returnEmptyXmlElementForNullStringValue) : this(commandContext)
		{
			this.returnEmptyXmlElementForNullStringValue = returnEmptyXmlElementForNullStringValue;
		}

		public static SimpleProperty CreateCommand(CommandContext commandContext)
		{
			return new SimpleProperty(commandContext);
		}

		public static SimpleProperty CreateCommandForDoNonReturnNonRepresentableProperty(CommandContext commandContext)
		{
			return new SimpleProperty(commandContext, false);
		}

		public static SimpleProperty CreateCommandForPropertyWithDefaultValue(CommandContext commandContext)
		{
			BaseConverter converterForPropertyDefinition = BaseConverter.GetConverterForPropertyDefinition(commandContext.GetPropertyDefinitions()[0]);
			return new SimpleProperty(commandContext, converterForPropertyDefinition)
			{
				returnDefaultValue = true
			};
		}

		public static SimpleProperty CreateIsReadReceiptRequestedCommand(CommandContext commandContext)
		{
			PropertyCommand.PreventSentMessageUpdate(commandContext);
			return SimpleProperty.CreateCommand(commandContext);
		}

		public static SimpleProperty CreateIsDeliveryReceiptRequestedCommand(CommandContext commandContext)
		{
			PropertyCommand.PreventSentMessageUpdate(commandContext);
			return SimpleProperty.CreateCommand(commandContext);
		}

		public override void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			StoreObject storeObject = updateCommandSettings.StoreObject;
			this.SetProperty(setPropertyUpdate.ServiceObject, storeObject);
		}

		public override void DeleteUpdate(DeletePropertyUpdate deletePropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			StoreObject storeObject = updateCommandSettings.StoreObject;
			base.DeleteProperties(storeObject, updateCommandSettings.PropertyUpdate.PropertyPath, new PropertyDefinition[]
			{
				this.propertyDefinition
			});
		}

		protected virtual object PreparePropertyBagValue(object propertyValue)
		{
			return propertyValue;
		}

		protected virtual void SetProperty(ServiceObject serviceObject, StoreObject storeObject)
		{
			object obj = serviceObject[this.commandContext.PropertyInformation];
			ArrayPropertyInformation arrayPropertyInformation = this.commandContext.PropertyInformation as ArrayPropertyInformation;
			if (arrayPropertyInformation != null)
			{
				string[] array = obj as string[];
				int num = array.Length;
				Array array2 = Array.CreateInstance(this.GetSimplePropertyType().GetElementType(), num);
				for (int i = 0; i < num; i++)
				{
					object value = this.Parse(array[i]);
					array2.SetValue(value, i);
				}
				base.SetPropertyValueOnStoreObject(storeObject, this.propertyDefinition, array2);
				return;
			}
			if (obj is string)
			{
				obj = this.Parse((string)obj);
			}
			base.SetPropertyValueOnStoreObject(storeObject, this.propertyDefinition, obj);
		}

		protected virtual Type GetSimplePropertyType()
		{
			return this.propertyDefinition.Type;
		}

		protected object GetDefaultValueForPropertyType()
		{
			Type simplePropertyType = this.GetSimplePropertyType();
			object result;
			if (SimpleProperty.defaultValueMap.TryGetValue(simplePropertyType, out result))
			{
				return result;
			}
			if (simplePropertyType.GetTypeInfo().IsValueType)
			{
				return Activator.CreateInstance(simplePropertyType);
			}
			return null;
		}

		protected virtual object Parse(string propertyString)
		{
			return this.propertyConverter.ConvertToObject(propertyString);
		}

		protected virtual object GetPropertyValue(StoreObject storeObject)
		{
			return PropertyCommand.GetPropertyValueFromStoreObject(storeObject, this.propertyDefinition);
		}

		protected virtual bool StorePropertyExists(StoreObject storeObject)
		{
			return PropertyCommand.StorePropertyExists(storeObject, this.propertyDefinition);
		}

		protected virtual string ToString(object propertyValue)
		{
			return this.propertyConverter.ConvertToString(propertyValue);
		}

		protected virtual void WriteServiceProperty(object propertyValue, ServiceObject serviceObject, PropertyInformation propInfo)
		{
			if (propertyValue == null || propertyValue is PropertyError)
			{
				return;
			}
			object servicePropertyValue = this.GetServicePropertyValue(propertyValue);
			serviceObject[propInfo] = servicePropertyValue;
		}

		private void Initialize(CommandContext commandContext, BaseConverter converter)
		{
			PropertyDefinition[] propertyDefinitions = commandContext.GetPropertyDefinitions();
			this.propertyDefinition = propertyDefinitions[0];
			this.propertyConverter = converter;
		}

		private object GetServicePropertyValue(object propertyValue)
		{
			IdConverterWithCommandSettings idConverterWithCommandSettings = new IdConverterWithCommandSettings(base.GetCommandSettings<ToServiceObjectCommandSettingsBase>(), CallContext.Current);
			object obj = this.propertyConverter.ConvertToServiceObjectValue(propertyValue, idConverterWithCommandSettings);
			bool encodeStringProperties;
			if (CallContext.Current == null)
			{
				encodeStringProperties = Global.EncodeStringProperties;
			}
			else
			{
				encodeStringProperties = CallContext.Current.EncodeStringProperties;
			}
			if (encodeStringProperties && ExchangeVersion.Current.Supports(ExchangeVersionType.Exchange2012))
			{
				string text = obj as string;
				string[] array = obj as string[];
				if (text != null)
				{
					obj = Util.EncodeForAntiXSS(text);
				}
				else if (array != null)
				{
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i] != null)
						{
							array[i] = Util.EncodeForAntiXSS(array[i]);
						}
					}
				}
			}
			return obj;
		}

		public virtual void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			if (this.StorePropertyExists(storeObject))
			{
				this.WriteServiceProperty(this.GetPropertyValue(storeObject), serviceObject, propertyInformation);
				return;
			}
			if (this.returnDefaultValue)
			{
				this.WriteServiceProperty(this.GetDefaultValueForPropertyType(), serviceObject, propertyInformation);
			}
		}

		public void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			object obj = null;
			if (!PropertyCommand.TryGetValueFromPropertyBag<object>(propertyBag, this.propertyDefinition, out obj))
			{
				if (this.returnDefaultValue)
				{
					obj = this.GetDefaultValueForPropertyType();
					this.WriteServiceProperty(obj, serviceObject, propertyInformation);
				}
				return;
			}
			if (!(propertyInformation is ArrayPropertyInformation))
			{
				obj = this.PreparePropertyBagValue(obj);
				this.WriteServiceProperty(obj, serviceObject, propertyInformation);
				return;
			}
			object[] array = obj as object[];
			Array array2 = null;
			if (array != null && array.Length > 0)
			{
				object servicePropertyValue = this.GetServicePropertyValue(array[0]);
				array2 = Array.CreateInstance(servicePropertyValue.GetType(), array.Length);
				array2.SetValue(servicePropertyValue, 0);
				for (int i = 1; i < array.Length; i++)
				{
					array2.SetValue(this.GetServicePropertyValue(array[i]), i);
				}
			}
			else if (obj != null && !(obj is PropertyError))
			{
				object servicePropertyValue2 = this.GetServicePropertyValue(obj);
				array2 = Array.CreateInstance(servicePropertyValue2.GetType(), 1);
				array2.SetValue(servicePropertyValue2, 0);
			}
			serviceObject[propertyInformation] = array2;
		}

		public void ToXml()
		{
			ToXmlCommandSettings commandSettings = base.GetCommandSettings<ToXmlCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			XmlElement serviceItem = commandSettings.ServiceItem;
			if (this.StorePropertyExists(storeObject))
			{
				this.WriteServiceProperty(this.GetPropertyValue(storeObject), serviceItem);
			}
		}

		public void ToXmlForPropertyBag()
		{
			ToXmlForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToXmlForPropertyBagCommandSettings>();
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			XmlElement serviceItem = commandSettings.ServiceItem;
			object propertyValue = null;
			if (PropertyCommand.TryGetValueFromPropertyBag<object>(propertyBag, this.propertyDefinition, out propertyValue))
			{
				propertyValue = this.PreparePropertyBagValue(propertyValue);
				this.WriteServiceProperty(propertyValue, serviceItem);
			}
		}

		public void Set()
		{
			SetCommandSettings commandSettings = base.GetCommandSettings<SetCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			XmlElement serviceProperty = commandSettings.ServiceProperty;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			if (serviceObject != null)
			{
				this.SetProperty(serviceObject, storeObject);
				return;
			}
			this.SetProperty(serviceProperty, storeObject);
		}

		protected virtual void SetProperty(XmlElement serviceProperty, StoreObject storeObject)
		{
			ArrayPropertyInformation arrayPropertyInformation = this.commandContext.PropertyInformation as ArrayPropertyInformation;
			if (arrayPropertyInformation != null)
			{
				Array array = Array.CreateInstance(this.GetSimplePropertyType().GetElementType(), serviceProperty.ChildNodes.Count);
				for (int i = 0; i < serviceProperty.ChildNodes.Count; i++)
				{
					XmlElement textNodeParent = (XmlElement)serviceProperty.ChildNodes[i];
					object value = this.Parse(ServiceXml.GetXmlTextNodeValue(textNodeParent));
					array.SetValue(value, i);
				}
				base.SetPropertyValueOnStoreObject(storeObject, this.propertyDefinition, array);
				return;
			}
			base.SetPropertyValueOnStoreObject(storeObject, this.propertyDefinition, this.Parse(ServiceXml.GetXmlTextNodeValue(serviceProperty)));
		}

		private void WriteServiceProperty(object propertyValue, XmlElement serviceItem)
		{
			if (!this.returnEmptyXmlElementForNullStringValue && propertyValue == null)
			{
				return;
			}
			ArrayPropertyInformation arrayPropertyInformation = this.commandContext.PropertyInformation as ArrayPropertyInformation;
			if (arrayPropertyInformation != null)
			{
				XmlElement parentElement = base.CreateXmlElement(serviceItem, this.xmlLocalName);
				Array array = propertyValue as Array;
				if (array != null)
				{
					using (IEnumerator enumerator = array.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							object propertyValue2 = enumerator.Current;
							base.CreateXmlTextElement(parentElement, arrayPropertyInformation.ArrayItemLocalName, this.ToString(propertyValue2));
						}
						return;
					}
				}
				ExTraceGlobals.ItemDataTracer.TraceDebug<string>((long)this.GetHashCode(), "[SimpleProperty::WriteServiceProperty] Unexpectedly got non-array property {0}", this.ToString(propertyValue));
				base.CreateXmlTextElement(parentElement, arrayPropertyInformation.ArrayItemLocalName, this.ToString(propertyValue));
				return;
			}
			string text = this.ToString(propertyValue);
			if (text != null || this.returnEmptyXmlElementForNullStringValue)
			{
				base.CreateXmlTextElement(serviceItem, this.xmlLocalName, text);
			}
		}

		private static Dictionary<Type, object> defaultValueMap = new Dictionary<Type, object>
		{
			{
				typeof(bool),
				false
			},
			{
				typeof(byte),
				0
			},
			{
				typeof(char),
				'\0'
			},
			{
				typeof(double),
				0.0
			},
			{
				typeof(float),
				0f
			},
			{
				typeof(int),
				0
			},
			{
				typeof(long),
				0L
			},
			{
				typeof(short),
				0
			}
		};

		protected PropertyDefinition propertyDefinition;

		protected BaseConverter propertyConverter;

		private bool returnEmptyXmlElementForNullStringValue = true;

		private bool returnDefaultValue;
	}
}
