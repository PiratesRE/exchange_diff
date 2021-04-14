using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[Serializable]
	public class RuleOperationErrorType
	{
		public int OperationIndex
		{
			get
			{
				return this.operationIndexField;
			}
			set
			{
				this.operationIndexField = value;
			}
		}

		[XmlArrayItem("Error", IsNullable = false)]
		public RuleValidationErrorType[] ValidationErrors
		{
			get
			{
				return this.validationErrorsField;
			}
			set
			{
				this.validationErrorsField = value;
			}
		}

		private int operationIndexField;

		private RuleValidationErrorType[] validationErrorsField;
	}
}
