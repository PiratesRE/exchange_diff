using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class SetClientExtensionType : BaseRequestType
	{
		[XmlArrayItem("Action", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public SetClientExtensionActionType[] Actions;
	}
}
