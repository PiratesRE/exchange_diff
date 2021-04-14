using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[Serializable]
	public class AlternateMailboxCollectionSetting : UserSetting
	{
		[XmlArray(IsNullable = true)]
		public AlternateMailbox[] AlternateMailboxes
		{
			get
			{
				return this.alternateMailboxesField;
			}
			set
			{
				this.alternateMailboxesField = value;
			}
		}

		private AlternateMailbox[] alternateMailboxesField;
	}
}
