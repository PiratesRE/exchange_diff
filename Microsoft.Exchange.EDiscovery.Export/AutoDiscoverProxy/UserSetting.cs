using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy
{
	[XmlInclude(typeof(DocumentSharingLocationCollectionSetting))]
	[XmlInclude(typeof(WebClientUrlCollectionSetting))]
	[XmlInclude(typeof(StringSetting))]
	[XmlInclude(typeof(ProtocolConnectionCollectionSetting))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[XmlInclude(typeof(AlternateMailboxCollectionSetting))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[DesignerCategory("code")]
	[Serializable]
	public class UserSetting
	{
		[XmlElement(IsNullable = true)]
		public string Name
		{
			get
			{
				return this.nameField;
			}
			set
			{
				this.nameField = value;
			}
		}

		private string nameField;
	}
}
