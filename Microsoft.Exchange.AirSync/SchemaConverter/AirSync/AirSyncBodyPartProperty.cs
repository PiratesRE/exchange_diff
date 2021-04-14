using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	internal class AirSyncBodyPartProperty : AirSyncProperty, IBodyPartProperty, IProperty, IAirSyncAttachments
	{
		public AirSyncBodyPartProperty(string xmlNodeNamespace, string airSyncTagName, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, requiresClientSupport)
		{
		}

		public string Preview
		{
			get
			{
				throw new NotImplementedException("Preview should not be called");
			}
		}

		public IEnumerable<AirSyncAttachmentInfo> Attachments
		{
			get
			{
				return this.attachments;
			}
		}

		public Stream GetData(BodyType type, long truncationSize, out long totalDataSize, out IEnumerable<AirSyncAttachmentInfo> attachments)
		{
			throw new NotImplementedException("GetData should not be called on the AirSyncBodyPartProperty");
		}

		public override void Unbind()
		{
			base.Unbind();
			this.attachments = null;
		}

		protected override void InternalCopyFrom(IProperty sourceProperty)
		{
			IBodyPartProperty bodyPartProperty = sourceProperty as IBodyPartProperty;
			if (bodyPartProperty == null)
			{
				throw new UnexpectedTypeException("IBodyPartProperty", sourceProperty);
			}
			List<BodyPartPreference> list = base.Options["BodyPartPreference"] as List<BodyPartPreference>;
			if (list == null || list.Count <= 0)
			{
				return;
			}
			BodyPartPreference bodyPartPreference = list[0];
			long truncationSize = bodyPartPreference.TruncationSize;
			bool allOrNone = bodyPartPreference.AllOrNone;
			long num = 0L;
			bool flag = true;
			StatusCode statusCode = StatusCode.Success;
			Stream stream;
			if (truncationSize >= 0L && allOrNone)
			{
				stream = bodyPartProperty.GetData(bodyPartPreference.Type, -1L, out num, out this.attachments);
				if (stream == null)
				{
					statusCode = StatusCode.BodyPart_ConversationTooLarge;
				}
				else if (stream.Length >= truncationSize && num > truncationSize)
				{
					stream = null;
					flag = true;
				}
				else
				{
					flag = false;
					num = stream.Length;
				}
			}
			else
			{
				stream = bodyPartProperty.GetData(bodyPartPreference.Type, truncationSize, out num, out this.attachments);
				if (stream == null)
				{
					statusCode = StatusCode.BodyPart_ConversationTooLarge;
				}
				flag = (truncationSize >= 0L && (stream != null && stream.Length >= truncationSize && num > truncationSize));
			}
			base.XmlNode = base.XmlParentNode.OwnerDocument.CreateElement(base.AirSyncTagNames[0], base.Namespace);
			base.XmlParentNode.AppendChild(base.XmlNode);
			XmlNode xmlNode = base.XmlNode;
			string elementName = "Status";
			int num2 = (int)statusCode;
			base.AppendChildNode(xmlNode, elementName, num2.ToString(CultureInfo.InvariantCulture));
			base.AppendChildNode(base.XmlNode, "Type", ((int)bodyPartPreference.Type).ToString(CultureInfo.InvariantCulture));
			base.AppendChildNode(base.XmlNode, "EstimatedDataSize", num.ToString(CultureInfo.InvariantCulture));
			if (flag)
			{
				base.AppendChildNode(base.XmlNode, "Truncated", "1");
			}
			if (stream != null && stream.Length > 0L)
			{
				base.AppendChildNode(base.XmlNode, "Data", stream, -1L, XmlNodeType.Text);
			}
			int preview = bodyPartPreference.Preview;
			if (preview == 0)
			{
				return;
			}
			string text = ((IBodyPartProperty)sourceProperty).Preview;
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			if (text.Length > preview)
			{
				text = text.Remove(preview);
			}
			text = text.TrimEnd(new char[0]);
			if (!string.IsNullOrEmpty(text))
			{
				base.AppendChildNode(base.XmlNode, "Preview", text);
			}
		}

		private IEnumerable<AirSyncAttachmentInfo> attachments;
	}
}
