using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class AddDistributionGroupToImListResponseMessageType : ResponseMessageType
	{
		public ImGroupType ImGroup
		{
			get
			{
				return this.imGroupField;
			}
			set
			{
				this.imGroupField = value;
			}
		}

		private ImGroupType imGroupField;
	}
}
