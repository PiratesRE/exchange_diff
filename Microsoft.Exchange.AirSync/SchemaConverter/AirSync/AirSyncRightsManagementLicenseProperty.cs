using System;
using System.Collections;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	internal class AirSyncRightsManagementLicenseProperty : AirSyncNestedProperty
	{
		public AirSyncRightsManagementLicenseProperty(string xmlNodeNamespace, string airSyncTagName, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, new RightsManagementLicenseData(), requiresClientSupport)
		{
		}

		protected override void InternalCopyFrom(IProperty srcProperty)
		{
			INestedProperty nestedProperty = srcProperty as INestedProperty;
			if (nestedProperty == null)
			{
				throw new UnexpectedTypeException("INestedProperty", srcProperty);
			}
			INestedData nestedData = nestedProperty.NestedData;
			if (nestedData == null)
			{
				throw new ConversionException("nestedData is NULL");
			}
			if (PropertyState.SetToDefault == srcProperty.State)
			{
				return;
			}
			if (nestedData.SubProperties.Count == 0)
			{
				return;
			}
			base.XmlNode = base.XmlParentNode.OwnerDocument.CreateElement(base.AirSyncTagNames[0], base.Namespace);
			IDictionaryEnumerator enumerator = nestedData.SubProperties.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string qualifiedName = enumerator.Key as string;
				string text = enumerator.Value as string;
				if (text != null)
				{
					XmlNode newChild = base.XmlParentNode.OwnerDocument.CreateTextNode(text);
					XmlNode xmlNode = base.XmlParentNode.OwnerDocument.CreateElement(qualifiedName, base.Namespace);
					xmlNode.AppendChild(newChild);
					base.XmlNode.AppendChild(xmlNode);
				}
			}
			base.XmlParentNode.AppendChild(base.XmlNode);
		}
	}
}
