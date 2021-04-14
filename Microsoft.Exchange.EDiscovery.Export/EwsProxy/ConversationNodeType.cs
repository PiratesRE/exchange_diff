using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[Serializable]
	public class ConversationNodeType
	{
		public string InternetMessageId
		{
			get
			{
				return this.internetMessageIdField;
			}
			set
			{
				this.internetMessageIdField = value;
			}
		}

		public string ParentInternetMessageId
		{
			get
			{
				return this.parentInternetMessageIdField;
			}
			set
			{
				this.parentInternetMessageIdField = value;
			}
		}

		public NonEmptyArrayOfAllItemsType Items
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

		private string internetMessageIdField;

		private string parentInternetMessageIdField;

		private NonEmptyArrayOfAllItemsType itemsField;
	}
}
