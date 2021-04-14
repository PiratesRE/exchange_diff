using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncAttachmentsProperty : AirSyncProperty, IAttachmentsProperty, IMultivaluedProperty<AttachmentData>, IProperty, IEnumerable<AttachmentData>, IEnumerable
	{
		public AirSyncAttachmentsProperty(string xmlNodeNamespace, string airSyncTagName, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, requiresClientSupport)
		{
		}

		public int Count
		{
			get
			{
				return base.XmlNode.ChildNodes.Count;
			}
		}

		public IEnumerator<AttachmentData> GetEnumerator()
		{
			foreach (object obj in base.XmlNode.ChildNodes)
			{
				XmlNode childNode = (XmlNode)obj;
				AttachmentData attachment = default(AttachmentData);
				foreach (object obj2 in childNode)
				{
					XmlNode xmlNode = (XmlNode)obj2;
					string name;
					if ((name = xmlNode.Name) != null)
					{
						if (!(name == "AttName"))
						{
							if (!(name == "AttSize"))
							{
								if (!(name == "AttMethod"))
								{
									if (name == "DisplayName")
									{
										attachment.DisplayName = xmlNode.InnerText;
									}
								}
								else
								{
									attachment.AttMethod = Convert.ToInt32(xmlNode.InnerText);
								}
							}
							else
							{
								attachment.AttSize = (long)Convert.ToInt32(xmlNode.InnerText);
							}
						}
						else
						{
							attachment.AttName = xmlNode.InnerText;
						}
					}
				}
				yield return attachment;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		protected override void InternalCopyFrom(IProperty srcProperty)
		{
			IAttachmentsProperty attachmentsProperty = srcProperty as IAttachmentsProperty;
			if (attachmentsProperty == null)
			{
				throw new UnexpectedTypeException("IAttachmentsProperty", srcProperty);
			}
			base.XmlNode = base.XmlParentNode.OwnerDocument.CreateElement(base.AirSyncTagNames[0], base.Namespace);
			foreach (AttachmentData attachmentData in attachmentsProperty)
			{
				XmlNode xmlNode = base.XmlParentNode.OwnerDocument.CreateElement("Attachment", base.Namespace);
				XmlNode xmlNode2 = base.XmlParentNode.OwnerDocument.CreateElement("AttName", base.Namespace);
				xmlNode2.InnerText = attachmentData.AttName;
				XmlNode xmlNode3 = base.XmlParentNode.OwnerDocument.CreateElement("AttSize", base.Namespace);
				XmlNode xmlNode4 = xmlNode3;
				long attSize = attachmentData.AttSize;
				xmlNode4.InnerText = attSize.ToString(CultureInfo.InvariantCulture);
				XmlNode xmlNode5 = base.XmlParentNode.OwnerDocument.CreateElement("AttMethod", base.Namespace);
				XmlNode xmlNode6 = xmlNode5;
				int attMethod = attachmentData.AttMethod;
				xmlNode6.InnerText = attMethod.ToString(CultureInfo.InvariantCulture);
				XmlNode xmlNode7 = base.XmlParentNode.OwnerDocument.CreateElement("DisplayName", base.Namespace);
				xmlNode7.InnerText = attachmentData.DisplayName;
				if (attachmentData.AttMethod == 5)
				{
					XmlNode xmlNode8 = xmlNode7;
					xmlNode8.InnerText += ".eml";
				}
				xmlNode.AppendChild(xmlNode5);
				xmlNode.AppendChild(xmlNode3);
				xmlNode.AppendChild(xmlNode7);
				xmlNode.AppendChild(xmlNode2);
				base.XmlNode.AppendChild(xmlNode);
			}
			if (base.XmlNode.HasChildNodes)
			{
				base.XmlParentNode.AppendChild(base.XmlNode);
			}
		}
	}
}
