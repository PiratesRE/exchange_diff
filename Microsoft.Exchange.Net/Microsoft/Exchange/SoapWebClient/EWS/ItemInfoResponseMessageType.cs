using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DesignerCategory("code")]
	[XmlInclude(typeof(UpdateItemInRecoverableItemsResponseMessageType))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[XmlInclude(typeof(UpdateItemResponseMessageType))]
	[Serializable]
	public class ItemInfoResponseMessageType : ResponseMessageType
	{
		public ArrayOfRealItemsType Items;
	}
}
