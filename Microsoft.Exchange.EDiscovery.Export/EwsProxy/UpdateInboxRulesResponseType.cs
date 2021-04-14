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
	public class UpdateInboxRulesResponseType : ResponseMessageType
	{
		[XmlArrayItem("RuleOperationError", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public RuleOperationErrorType[] RuleOperationErrors
		{
			get
			{
				return this.ruleOperationErrorsField;
			}
			set
			{
				this.ruleOperationErrorsField = value;
			}
		}

		private RuleOperationErrorType[] ruleOperationErrorsField;
	}
}
