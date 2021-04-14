using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DebuggerStepThrough]
	[Serializable]
	public class ValidateUMPinType : BaseRequestType
	{
		public PinInfoType PinInfo
		{
			get
			{
				return this.pinInfoField;
			}
			set
			{
				this.pinInfoField = value;
			}
		}

		public string UserUMMailboxPolicyGuid
		{
			get
			{
				return this.userUMMailboxPolicyGuidField;
			}
			set
			{
				this.userUMMailboxPolicyGuidField = value;
			}
		}

		private PinInfoType pinInfoField;

		private string userUMMailboxPolicyGuidField;
	}
}
