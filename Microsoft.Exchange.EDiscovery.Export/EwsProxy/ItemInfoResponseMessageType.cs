using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[XmlInclude(typeof(UpdateItemInRecoverableItemsResponseMessageType))]
	[XmlInclude(typeof(UpdateItemResponseMessageType))]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[Serializable]
	public class ItemInfoResponseMessageType : ResponseMessageType
	{
		public ArrayOfRealItemsType Items
		{
			get
			{
				return this.itemsField;
			}
			set
			{
				this.itemsField = value;
			}
		}

		private ArrayOfRealItemsType itemsField;
	}
}
