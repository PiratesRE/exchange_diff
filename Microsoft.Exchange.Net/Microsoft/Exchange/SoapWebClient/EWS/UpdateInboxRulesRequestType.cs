using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DebuggerStepThrough]
	[Serializable]
	public class UpdateInboxRulesRequestType : BaseRequestType
	{
		public string MailboxSmtpAddress;

		public bool RemoveOutlookRuleBlob;

		[XmlIgnore]
		public bool RemoveOutlookRuleBlobSpecified;

		[XmlArrayItem("CreateRuleOperation", typeof(CreateRuleOperationType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("DeleteRuleOperation", typeof(DeleteRuleOperationType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("SetRuleOperation", typeof(SetRuleOperationType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public RuleOperationType[] Operations;
	}
}
