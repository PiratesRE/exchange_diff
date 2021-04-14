using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal abstract class Shape
	{
		public Shape(XmlElementInformation itemXmlElementInformation, Schema schema, Shape innerShape, IList<PropertyInformation> defaultProperties)
		{
			this.itemXmlElementInformation = itemXmlElementInformation;
			this.schema = schema;
			this.innerShape = innerShape;
			this.defaultProperties = defaultProperties;
		}

		public static bool IsGenericMessageOnly(MessageItem messageItem)
		{
			object obj = messageItem.TryGetProperty(StoreObjectSchema.ItemClass);
			PropertyError propertyError = obj as PropertyError;
			string text;
			if (propertyError != null)
			{
				if (propertyError.PropertyErrorCode == PropertyErrorCode.NotEnoughMemory)
				{
					using (Stream stream = messageItem.OpenPropertyStream(StoreObjectSchema.ItemClass, PropertyOpenMode.ReadOnly))
					{
						int num = Encoding.Unicode.GetBytes("IPM.Note").Length;
						if (stream.Length >= (long)num)
						{
							byte[] array = new byte[num];
							stream.Read(array, 0, num);
							text = Encoding.Unicode.GetString(array, 0, array.Length);
							return ObjectClass.IsGenericMessage(text) && !ObjectClass.IsMessage(text, false);
						}
						return true;
					}
					return true;
				}
				return true;
			}
			text = (string)obj;
			return string.IsNullOrEmpty(text) || (ObjectClass.IsGenericMessage(text) && !ObjectClass.IsMessage(text, false));
		}

		public bool TryGetPropertyInformation(PropertyPath propertyPath, out PropertyInformation propertyInformation)
		{
			bool flag = Shape.TryGetPropertyInformation(this, propertyPath, out propertyInformation);
			if (!flag)
			{
				DictionaryPropertyUri dictionaryPropertyUri = propertyPath as DictionaryPropertyUri;
				if (dictionaryPropertyUri != null)
				{
					DictionaryPropertyUriBase dictionaryPropertyUriBase = dictionaryPropertyUri.GetDictionaryPropertyUriBase();
					flag = Shape.TryGetPropertyInformation(this, dictionaryPropertyUriBase, out propertyInformation);
				}
				else
				{
					ExtendedPropertyUri extendedPropertyUri = propertyPath as ExtendedPropertyUri;
					if (extendedPropertyUri != null)
					{
						flag = Shape.TryGetPropertyInformation(this, ExtendedPropertyUri.Placeholder, out propertyInformation);
					}
				}
			}
			return flag;
		}

		private static bool TryGetPropertyInformation(Shape shape, PropertyPath propertyPath, out PropertyInformation propertyInformation)
		{
			bool flag = false;
			propertyInformation = null;
			Shape shape2 = shape;
			while (!flag && shape2 != null)
			{
				flag = shape2.Schema.TryGetPropertyInformationByPath(propertyPath, out propertyInformation);
				if (!flag)
				{
					shape2 = shape2.innerShape;
				}
			}
			return flag;
		}

		public IList<PropertyInformation> DefaultProperties
		{
			get
			{
				return this.defaultProperties;
			}
		}

		public Schema Schema
		{
			get
			{
				return this.schema;
			}
		}

		public Shape InnerShape
		{
			get
			{
				return this.innerShape;
			}
		}

		public XmlElement CreateItemXmlElement(XmlDocument ownerDocument)
		{
			return ServiceXml.CreateElement(ownerDocument, this.itemXmlElementInformation.LocalName, this.itemXmlElementInformation.NamespaceUri);
		}

		public XmlElement CreateItemXmlElement(XmlElement parentElement)
		{
			return ServiceXml.CreateElement(parentElement, this.itemXmlElementInformation.LocalName, this.itemXmlElementInformation.NamespaceUri);
		}

		private IList<PropertyInformation> defaultProperties;

		private Schema schema;

		private Shape innerShape;

		private XmlElementInformation itemXmlElementInformation;

		public delegate Shape CreateShapeCallback();

		public delegate Shape CreateShapeForPropertyBagCallback();

		public delegate Shape CreateShapeForStoreObjectCallback(StoreObject storeObject);
	}
}
