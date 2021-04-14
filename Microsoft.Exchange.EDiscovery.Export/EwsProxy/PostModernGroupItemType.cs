using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class PostModernGroupItemType : BaseRequestType
	{
		public EmailAddressType ModernGroupEmailAddress
		{
			get
			{
				return this.modernGroupEmailAddressField;
			}
			set
			{
				this.modernGroupEmailAddressField = value;
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

		private EmailAddressType modernGroupEmailAddressField;

		private NonEmptyArrayOfAllItemsType itemsField;
	}
}
