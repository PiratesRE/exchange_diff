using System;
using System.Collections;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncFlagProperty : AirSyncNestedProperty
	{
		public AirSyncFlagProperty(string xmlNodeNamespace, string airSyncTagName, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, new FlagData(), requiresClientSupport)
		{
			base.ClientChangeTracked = true;
		}

		protected override void InternalCopyFrom(IProperty srcProperty)
		{
			INestedProperty nestedProperty = srcProperty as INestedProperty;
			if (nestedProperty == null)
			{
				throw new UnexpectedTypeException("INestedProperty", srcProperty);
			}
			if (PropertyState.Modified != srcProperty.State)
			{
				throw new ConversionException("Property only supports conversion from Modified property state");
			}
			INestedData nestedData = nestedProperty.NestedData;
			if (nestedData == null)
			{
				throw new ConversionException("nestedData is NULL");
			}
			base.XmlNode = base.XmlParentNode.OwnerDocument.CreateElement(base.AirSyncTagNames[0], base.Namespace);
			IDictionaryEnumerator enumerator = nestedData.SubProperties.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string text = enumerator.Key as string;
				string text2 = enumerator.Value as string;
				if (text2 != null)
				{
					string namespaceURI = FlagData.IsTaskProperty(text) ? "Tasks:" : base.Namespace;
					XmlNode newChild = base.XmlParentNode.OwnerDocument.CreateTextNode(text2);
					XmlNode xmlNode = base.XmlParentNode.OwnerDocument.CreateElement(text, namespaceURI);
					xmlNode.AppendChild(newChild);
					base.XmlNode.AppendChild(xmlNode);
				}
			}
			base.XmlParentNode.AppendChild(base.XmlNode);
		}
	}
}
