using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class ContactsViewType : BasePagingType
	{
		[XmlAttribute]
		public string InitialName
		{
			get
			{
				return this.initialNameField;
			}
			set
			{
				this.initialNameField = value;
			}
		}

		[XmlAttribute]
		public string FinalName
		{
			get
			{
				return this.finalNameField;
			}
			set
			{
				this.finalNameField = value;
			}
		}

		private string initialNameField;

		private string finalNameField;
	}
}
