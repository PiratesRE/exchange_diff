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
	public class PhoneEntityType : EntityType
	{
		public string OriginalPhoneString
		{
			get
			{
				return this.originalPhoneStringField;
			}
			set
			{
				this.originalPhoneStringField = value;
			}
		}

		public string PhoneString
		{
			get
			{
				return this.phoneStringField;
			}
			set
			{
				this.phoneStringField = value;
			}
		}

		public string Type
		{
			get
			{
				return this.typeField;
			}
			set
			{
				this.typeField = value;
			}
		}

		private string originalPhoneStringField;

		private string phoneStringField;

		private string typeField;
	}
}
