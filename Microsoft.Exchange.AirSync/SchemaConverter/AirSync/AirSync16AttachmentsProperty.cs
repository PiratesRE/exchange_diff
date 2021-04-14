using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.AirSync.Wbxml;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	internal class AirSync16AttachmentsProperty : AirSync14AttachmentsProperty
	{
		public AirSync16AttachmentsProperty(string xmlNodeNamespace, string airSyncTagName, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, requiresClientSupport)
		{
		}

		public override IEnumerator<Attachment12Data> GetEnumerator()
		{
			foreach (object obj in base.XmlNode.ChildNodes)
			{
				XmlNode node = (XmlNode)obj;
				string name;
				if ((name = node.Name) != null)
				{
					Attachment16Data attachment16;
					if (!(name == "Add"))
					{
						if (!(name == "Delete"))
						{
							goto IL_C1;
						}
						attachment16 = this.ParseAttachmentsData(node, AttachmentAction.Delete);
					}
					else
					{
						attachment16 = this.ParseAttachmentsData(node, AttachmentAction.Add);
					}
					yield return attachment16;
					continue;
				}
				IL_C1:
				throw new ConversionException(string.Format("Invalid AttachmentAction {0} found in AttachmentsNode", node.InnerText));
			}
			yield break;
		}

		private Attachment16Data ParseAttachmentsData(XmlNode parentNode, AttachmentAction action)
		{
			Attachment16Data attachment16Data = new Attachment16Data(action);
			foreach (object obj in parentNode)
			{
				XmlNode xmlNode = (XmlNode)obj;
				string name;
				switch (name = xmlNode.Name)
				{
				case "FileReference":
					attachment16Data.FileReference = HttpUtility.UrlDecode(xmlNode.InnerText);
					continue;
				case "ContentId":
					attachment16Data.ContentId = xmlNode.InnerText;
					continue;
				case "ContentLocation":
					attachment16Data.ContentLocation = xmlNode.InnerText;
					continue;
				case "Method":
					attachment16Data.Method = Convert.ToByte(xmlNode.InnerText);
					continue;
				case "DisplayName":
					attachment16Data.DisplayName = xmlNode.InnerText;
					continue;
				case "IsInline":
					if (string.IsNullOrEmpty(xmlNode.InnerText))
					{
						attachment16Data.IsInline = true;
						continue;
					}
					if (xmlNode.InnerText.Equals("1"))
					{
						attachment16Data.IsInline = true;
						continue;
					}
					if (xmlNode.InnerText.Equals("0"))
					{
						attachment16Data.IsInline = false;
						continue;
					}
					throw new ConversionException("Airsync16AttachmentProperty:ParseAttachmentsData. Invalid value passed in 'IsInline' Node.");
				case "Content":
				{
					AirSyncBlobXmlNode airSyncBlobXmlNode = xmlNode as AirSyncBlobXmlNode;
					if (airSyncBlobXmlNode == null)
					{
						continue;
					}
					if (airSyncBlobXmlNode.ByteArray != null)
					{
						attachment16Data.Content = airSyncBlobXmlNode.ByteArray;
						continue;
					}
					if (airSyncBlobXmlNode.Stream != null && airSyncBlobXmlNode.Stream.CanSeek && airSyncBlobXmlNode.Stream.CanRead)
					{
						attachment16Data.Content = new byte[airSyncBlobXmlNode.Stream.Length];
						airSyncBlobXmlNode.Stream.Seek(0L, SeekOrigin.Begin);
						airSyncBlobXmlNode.Stream.Read(attachment16Data.Content, 0, (int)airSyncBlobXmlNode.Stream.Length);
						continue;
					}
					throw new ConversionException("Airsync16AttachmentProperty:ParseAttachmentsData. Invalid value passed in 'Data' Node.");
				}
				case "ContentType":
					attachment16Data.ContentType = xmlNode.InnerText;
					continue;
				case "ClientId":
					attachment16Data.ClientId = xmlNode.InnerText;
					continue;
				}
				throw new ConversionException(string.Format("Airsync16AttachmentProperty:ParseAttachmentsData. invalid node {0}.", xmlNode.Name));
			}
			return attachment16Data;
		}
	}
}
