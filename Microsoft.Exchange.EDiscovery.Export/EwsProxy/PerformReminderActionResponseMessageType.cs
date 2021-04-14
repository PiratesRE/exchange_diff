using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class PerformReminderActionResponseMessageType : ResponseMessageType
	{
		[XmlArrayItem("ItemId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public ItemIdType[] UpdatedItemIds
		{
			get
			{
				return this.updatedItemIdsField;
			}
			set
			{
				this.updatedItemIdsField = value;
			}
		}

		private ItemIdType[] updatedItemIdsField;
	}
}
