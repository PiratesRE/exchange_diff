using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlInclude(typeof(UpdateDelegateType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[XmlInclude(typeof(RemoveDelegateType))]
	[XmlInclude(typeof(AddDelegateType))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlInclude(typeof(GetDelegateType))]
	[Serializable]
	public abstract class BaseDelegateType : BaseRequestType
	{
		public EmailAddressType Mailbox;
	}
}
