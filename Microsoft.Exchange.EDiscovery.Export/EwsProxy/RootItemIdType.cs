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
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class RootItemIdType : BaseItemIdType
	{
		[XmlAttribute]
		public string RootItemId
		{
			get
			{
				return this.rootItemIdField;
			}
			set
			{
				this.rootItemIdField = value;
			}
		}

		[XmlAttribute]
		public string RootItemChangeKey
		{
			get
			{
				return this.rootItemChangeKeyField;
			}
			set
			{
				this.rootItemChangeKeyField = value;
			}
		}

		private string rootItemIdField;

		private string rootItemChangeKeyField;
	}
}
