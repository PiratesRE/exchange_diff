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
	public class UpdateInboxRulesRequestType : BaseRequestType
	{
		public string MailboxSmtpAddress
		{
			get
			{
				return this.mailboxSmtpAddressField;
			}
			set
			{
				this.mailboxSmtpAddressField = value;
			}
		}

		public bool RemoveOutlookRuleBlob
		{
			get
			{
				return this.removeOutlookRuleBlobField;
			}
			set
			{
				this.removeOutlookRuleBlobField = value;
			}
		}

		[XmlIgnore]
		public bool RemoveOutlookRuleBlobSpecified
		{
			get
			{
				return this.removeOutlookRuleBlobFieldSpecified;
			}
			set
			{
				this.removeOutlookRuleBlobFieldSpecified = value;
			}
		}

		[XmlArrayItem("SetRuleOperation", typeof(SetRuleOperationType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("DeleteRuleOperation", typeof(DeleteRuleOperationType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("CreateRuleOperation", typeof(CreateRuleOperationType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public RuleOperationType[] Operations
		{
			get
			{
				return this.operationsField;
			}
			set
			{
				this.operationsField = value;
			}
		}

		private string mailboxSmtpAddressField;

		private bool removeOutlookRuleBlobField;

		private bool removeOutlookRuleBlobFieldSpecified;

		private RuleOperationType[] operationsField;
	}
}
