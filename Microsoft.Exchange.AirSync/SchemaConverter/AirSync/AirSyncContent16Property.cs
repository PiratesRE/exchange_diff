using System;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	internal class AirSyncContent16Property : AirSyncContent14Property, IContent16Property, IContent14Property, IContentProperty, IMIMEDataProperty, IMIMERelatedProperty, IProperty
	{
		public AirSyncContent16Property(string xmlNodeNamespace, string airSyncTagName, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, requiresClientSupport)
		{
		}

		public string BodyString
		{
			get
			{
				XmlNode xmlNode = base.XmlNode.SelectSingleNode("X:Data", base.NamespaceManager);
				if (xmlNode == null)
				{
					return null;
				}
				return xmlNode.InnerText;
			}
		}

		protected override bool ClientAccepts(IContentProperty srcProperty, BodyType type, out long estimatedDataSize, out long truncationSize)
		{
			IContent16Property content16Property = srcProperty as IContent16Property;
			if (content16Property == null)
			{
				throw new UnexpectedTypeException("IContent16Property", content16Property);
			}
			estimatedDataSize = 0L;
			bool flag;
			if (!base.IsAcceptable(type, out truncationSize, out flag))
			{
				return false;
			}
			this.dataString = content16Property.BodyString;
			estimatedDataSize = (long)this.dataString.Length;
			if (truncationSize < 0L || estimatedDataSize <= truncationSize)
			{
				base.Truncated = false;
			}
			else
			{
				if (flag)
				{
					this.dataString = null;
					return false;
				}
				this.dataString = this.dataString.Remove((int)truncationSize);
				base.Truncated = true;
			}
			return true;
		}

		protected override void CopyData()
		{
			if (!string.IsNullOrEmpty(this.dataString))
			{
				base.AppendChildNode(base.XmlNode, "Data", this.dataString);
				return;
			}
			base.CopyData();
		}

		private string dataString;
	}
}
