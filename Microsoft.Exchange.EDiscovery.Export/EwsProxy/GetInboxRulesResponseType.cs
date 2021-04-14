using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class GetInboxRulesResponseType : ResponseMessageType
	{
		public bool OutlookRuleBlobExists
		{
			get
			{
				return this.outlookRuleBlobExistsField;
			}
			set
			{
				this.outlookRuleBlobExistsField = value;
			}
		}

		[XmlIgnore]
		public bool OutlookRuleBlobExistsSpecified
		{
			get
			{
				return this.outlookRuleBlobExistsFieldSpecified;
			}
			set
			{
				this.outlookRuleBlobExistsFieldSpecified = value;
			}
		}

		[XmlArrayItem("Rule", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public RuleType[] InboxRules
		{
			get
			{
				return this.inboxRulesField;
			}
			set
			{
				this.inboxRulesField = value;
			}
		}

		private bool outlookRuleBlobExistsField;

		private bool outlookRuleBlobExistsFieldSpecified;

		private RuleType[] inboxRulesField;
	}
}
