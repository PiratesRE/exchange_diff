using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class MarkAsJunkResponseMessageType : ResponseMessageType
	{
		public ItemIdType MovedItemId
		{
			get
			{
				return this.movedItemIdField;
			}
			set
			{
				this.movedItemIdField = value;
			}
		}

		private ItemIdType movedItemIdField;
	}
}
