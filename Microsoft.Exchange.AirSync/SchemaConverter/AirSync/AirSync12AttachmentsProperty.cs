using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.SchemaConverter;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSync12AttachmentsProperty : AirSyncProperty, IAttachments12Property, IMultivaluedProperty<Attachment12Data>, IProperty, IEnumerable<Attachment12Data>, IEnumerable
	{
		public AirSync12AttachmentsProperty(string xmlNodeNamespace, string airSyncTagName, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, requiresClientSupport)
		{
			this.nodeMap = new Dictionary<AttachmentId, XmlNode>();
		}

		public int Count
		{
			get
			{
				return base.XmlNode.ChildNodes.Count;
			}
		}

		public override void Unbind()
		{
			this.nodeMap.Clear();
			base.Unbind();
		}

		public virtual IEnumerator<Attachment12Data> GetEnumerator()
		{
			foreach (object obj in base.XmlNode.ChildNodes)
			{
				XmlNode childNode = (XmlNode)obj;
				Attachment12Data attachment = new Attachment12Data();
				foreach (object obj2 in childNode)
				{
					XmlNode xmlNode = (XmlNode)obj2;
					string name;
					switch (name = xmlNode.Name)
					{
					case "FileReference":
						attachment.FileReference = xmlNode.InnerText;
						break;
					case "EstimatedDataSize":
						attachment.EstimatedDataSize = (long)Convert.ToInt32(xmlNode.InnerText);
						break;
					case "ContentId":
						attachment.ContentId = xmlNode.InnerText;
						break;
					case "ContentLocation":
						attachment.ContentLocation = xmlNode.InnerText;
						break;
					case "Method":
						attachment.Method = Convert.ToByte(xmlNode.InnerText);
						break;
					case "DisplayName":
						attachment.DisplayName = xmlNode.InnerText;
						break;
					case "IsInline":
						attachment.IsInline = true;
						break;
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
			IAttachments12Property attachments12Property = srcProperty as IAttachments12Property;
			if (attachments12Property == null)
			{
				throw new UnexpectedTypeException("IAttachments12Property", srcProperty);
			}
			base.XmlNode = base.XmlParentNode.OwnerDocument.CreateElement(base.AirSyncTagNames[0], base.Namespace);
			foreach (Attachment12Data attachment12Data in attachments12Property)
			{
				XmlNode xmlNode = base.XmlParentNode.OwnerDocument.CreateElement("Attachment", base.Namespace);
				this.CopyAttachment(xmlNode, attachment12Data);
				if (attachment12Data.Id != null)
				{
					this.nodeMap[attachment12Data.Id] = base.XmlNode.AppendChild(xmlNode);
				}
				else
				{
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.XsoTracer, this, "Skip attachment {0}, its ID is null", attachment12Data.DisplayName);
				}
			}
			if (base.XmlNode.HasChildNodes)
			{
				base.XmlParentNode.AppendChild(base.XmlNode);
				base.PostProcessingDelegate = new PropertyBase.PostProcessProperties(this.PostProcessAttachments);
			}
		}

		protected virtual void CopyAttachment(XmlNode attachmentNode, Attachment12Data attachment)
		{
			XmlNode xmlNode = base.XmlParentNode.OwnerDocument.CreateElement("DisplayName", base.Namespace);
			xmlNode.InnerText = attachment.DisplayName;
			XmlNode xmlNode2 = base.XmlParentNode.OwnerDocument.CreateElement("FileReference", base.Namespace);
			xmlNode2.InnerText = attachment.FileReference;
			XmlNode xmlNode3 = base.XmlParentNode.OwnerDocument.CreateElement("Method", base.Namespace);
			xmlNode3.InnerText = attachment.Method.ToString(CultureInfo.InvariantCulture);
			if (attachment.Method == 5)
			{
				XmlNode xmlNode4 = xmlNode;
				xmlNode4.InnerText += ".eml";
			}
			XmlNode xmlNode5 = base.XmlParentNode.OwnerDocument.CreateElement("EstimatedDataSize", base.Namespace);
			xmlNode5.InnerText = attachment.EstimatedDataSize.ToString(CultureInfo.InvariantCulture);
			attachmentNode.AppendChild(xmlNode);
			attachmentNode.AppendChild(xmlNode2);
			attachmentNode.AppendChild(xmlNode3);
			attachmentNode.AppendChild(xmlNode5);
			if (!string.IsNullOrEmpty(attachment.ContentId))
			{
				base.AppendChildNode(attachmentNode, "ContentId", attachment.ContentId);
			}
			if (!string.IsNullOrEmpty(attachment.ContentLocation))
			{
				base.AppendChildNode(attachmentNode, "ContentLocation", attachment.ContentLocation);
			}
			if (attachment.IsInline)
			{
				base.AppendChildNode(attachmentNode, "IsInline", "1");
			}
		}

		protected virtual void FixupXml(IEnumerable<IGrouping<AttachmentId, AirSyncAttachmentInfo>> allAttachments)
		{
			foreach (IGrouping<AttachmentId, AirSyncAttachmentInfo> grouping in allAttachments)
			{
				AirSyncAttachmentInfo airSyncAttachmentInfo = grouping.First<AirSyncAttachmentInfo>();
				if (grouping.Count<AirSyncAttachmentInfo>() != 1)
				{
					foreach (AirSyncAttachmentInfo airSyncAttachmentInfo2 in grouping)
					{
						AirSyncDiagnostics.TraceInfo<AttachmentId, bool, string>(ExTraceGlobals.AirSyncTracer, this, "AttachmentId = {0}, IsInline = {1}, ContentId = {2}", airSyncAttachmentInfo2.AttachmentId, airSyncAttachmentInfo2.IsInline, airSyncAttachmentInfo2.ContentId);
						if (airSyncAttachmentInfo2.IsInline && !string.IsNullOrEmpty(airSyncAttachmentInfo2.ContentId))
						{
							if (airSyncAttachmentInfo.IsInline && !string.IsNullOrEmpty(airSyncAttachmentInfo.ContentId) && !string.Equals(airSyncAttachmentInfo.ContentId, airSyncAttachmentInfo2.ContentId))
							{
								throw new InvalidOperationException(string.Format("Different ContentId generated for the same attachment! contentId1: {0}, contentId2: {1}", airSyncAttachmentInfo.ContentId, airSyncAttachmentInfo2.ContentId));
							}
							airSyncAttachmentInfo = airSyncAttachmentInfo2;
						}
					}
				}
				XmlNode parentNode;
				if (this.nodeMap.TryGetValue(grouping.Key, out parentNode))
				{
					if (airSyncAttachmentInfo.IsInline)
					{
						base.ReplaceValueOrCreateNode(parentNode, "IsInline", "1");
						if (!string.IsNullOrEmpty(airSyncAttachmentInfo.ContentId))
						{
							base.ReplaceValueOrCreateNode(parentNode, "ContentId", airSyncAttachmentInfo.ContentId);
						}
					}
					else
					{
						base.RemoveNode(parentNode, "IsInline");
						base.RemoveNode(parentNode, "ContentId");
					}
				}
			}
		}

		private void PostProcessAttachments(IProperty srcProperty, IList<IProperty> airSyncProps)
		{
			if (!(srcProperty is IAttachments12Property))
			{
				throw new UnexpectedTypeException("IAttachments12Property", srcProperty);
			}
			IEnumerable<AirSyncAttachmentInfo> enumerable = null;
			foreach (IProperty property in airSyncProps)
			{
				AirSyncProperty airSyncProperty = (AirSyncProperty)property;
				if (airSyncProperty is IAirSyncAttachments)
				{
					IEnumerable<AirSyncAttachmentInfo> attachments = ((IAirSyncAttachments)airSyncProperty).Attachments;
					if (attachments != null && attachments.Count<AirSyncAttachmentInfo>() > 0)
					{
						enumerable = ((enumerable == null) ? attachments : enumerable.Union(attachments));
					}
				}
			}
			if (enumerable != null)
			{
				IEnumerable<IGrouping<AttachmentId, AirSyncAttachmentInfo>> allAttachments = from attachment in enumerable
				group attachment by attachment.AttachmentId;
				this.FixupXml(allAttachments);
			}
		}

		private Dictionary<AttachmentId, XmlNode> nodeMap;
	}
}
