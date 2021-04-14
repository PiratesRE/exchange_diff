using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "UploadItemsResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class UploadItemsResponseMessage : ResponseMessage
	{
		public UploadItemsResponseMessage()
		{
		}

		internal UploadItemsResponseMessage(ServiceResultCode code, ServiceError error, XmlNode itemId) : base(code, error)
		{
			this.ItemId = itemId;
		}

		[XmlNamespaceDeclarations]
		public XmlSerializerNamespaces Namespaces
		{
			get
			{
				return ResponseMessage.namespaces;
			}
			set
			{
			}
		}

		[XmlAnyElement("ItemId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public XmlNode ItemId
		{
			get
			{
				return this.itemId;
			}
			set
			{
				if (value == null)
				{
					this.itemId = null;
					return;
				}
				XmlNode xmlNode = null;
				foreach (object obj in value.ChildNodes)
				{
					XmlNode xmlNode2 = (XmlNode)obj;
					if (xmlNode2.LocalName == "ItemId")
					{
						xmlNode = xmlNode2;
					}
				}
				this.itemId = xmlNode;
			}
		}

		private XmlNode itemId;
	}
}
