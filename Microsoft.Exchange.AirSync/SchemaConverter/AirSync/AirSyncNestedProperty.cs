using System;
using System.Collections;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal abstract class AirSyncNestedProperty : AirSyncProperty, INestedProperty
	{
		public AirSyncNestedProperty(string xmlNodeNamespace, string airSyncTagName, INestedData nested, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, requiresClientSupport)
		{
			this.nestedData = nested;
		}

		public INestedData NestedData
		{
			get
			{
				if (this.nestedData != null)
				{
					foreach (object obj in base.XmlNode.ChildNodes)
					{
						XmlNode xmlNode = (XmlNode)obj;
						this.nestedData.SubProperties[xmlNode.Name] = xmlNode.InnerText;
					}
				}
				return this.nestedData;
			}
		}

		public override void Unbind()
		{
			base.Unbind();
			this.nestedData.Clear();
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
			this.AppendToXmlNode(base.XmlNode, nestedData);
			base.XmlParentNode.AppendChild(base.XmlNode);
		}

		protected void AppendToXmlNode(XmlNode parentNode, INestedData nestedData)
		{
			this.AppendToXmlNode(parentNode, nestedData, base.Namespace);
		}

		protected void AppendToXmlNode(XmlNode parentNode, INestedData nestedData, string namespaceString)
		{
			IDictionaryEnumerator enumerator = nestedData.SubProperties.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Value != null && !string.IsNullOrEmpty(enumerator.Value as string))
				{
					XmlNode newChild = base.XmlParentNode.OwnerDocument.CreateTextNode(enumerator.Value.ToString());
					XmlNode xmlNode = base.XmlParentNode.OwnerDocument.CreateElement(enumerator.Key as string, namespaceString);
					xmlNode.AppendChild(newChild);
					parentNode.AppendChild(xmlNode);
				}
			}
		}

		private INestedData nestedData;
	}
}
