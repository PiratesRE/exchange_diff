using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[Serializable]
	public class GetInboxRulesResponseType : ResponseMessageType
	{
		public bool OutlookRuleBlobExists;

		[XmlIgnore]
		public bool OutlookRuleBlobExistsSpecified;

		[XmlArrayItem("Rule", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public RuleType[] InboxRules;
	}
}
