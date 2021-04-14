using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class DisableAppType : BaseRequestType
	{
		public string ID
		{
			get
			{
				return this.idField;
			}
			set
			{
				this.idField = value;
			}
		}

		public DisableReasonType DisableReason
		{
			get
			{
				return this.disableReasonField;
			}
			set
			{
				this.disableReasonField = value;
			}
		}

		private string idField;

		private DisableReasonType disableReasonField;
	}
}
