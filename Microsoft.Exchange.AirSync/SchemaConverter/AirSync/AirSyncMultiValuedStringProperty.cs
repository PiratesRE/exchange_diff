using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncMultiValuedStringProperty : AirSyncProperty, IMultivaluedProperty<string>, IProperty, IEnumerable<string>, IEnumerable
	{
		public AirSyncMultiValuedStringProperty(string xmlNodeNamespace, string airSyncTagName, string airSyncChildTagName, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, requiresClientSupport)
		{
			this.airSyncChildTagName = airSyncChildTagName;
		}

		public int Count
		{
			get
			{
				return base.XmlNode.ChildNodes.Count;
			}
		}

		public IEnumerator<string> GetEnumerator()
		{
			foreach (object obj in base.XmlNode.ChildNodes)
			{
				XmlNode childNode = (XmlNode)obj;
				yield return childNode.InnerText;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		protected override void InternalCopyFrom(IProperty srcProperty)
		{
			IMultivaluedProperty<string> multivaluedProperty = srcProperty as IMultivaluedProperty<string>;
			if (multivaluedProperty == null)
			{
				throw new UnexpectedTypeException("IMultivaluedProperty<string>", srcProperty);
			}
			base.XmlNode = base.XmlParentNode.OwnerDocument.CreateElement(base.AirSyncTagNames[0], base.Namespace);
			foreach (string text in multivaluedProperty)
			{
				if (!string.IsNullOrEmpty(text))
				{
					XmlNode xmlNode = base.XmlParentNode.OwnerDocument.CreateElement(this.airSyncChildTagName, base.Namespace);
					xmlNode.InnerText = text;
					base.XmlNode.AppendChild(xmlNode);
				}
			}
			if (base.XmlNode.HasChildNodes)
			{
				base.XmlParentNode.AppendChild(base.XmlNode);
			}
		}

		private string airSyncChildTagName;
	}
}
