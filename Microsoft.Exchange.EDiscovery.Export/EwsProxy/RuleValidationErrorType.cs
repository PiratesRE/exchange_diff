using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class RuleValidationErrorType
	{
		public RuleFieldURIType FieldURI
		{
			get
			{
				return this.fieldURIField;
			}
			set
			{
				this.fieldURIField = value;
			}
		}

		public RuleValidationErrorCodeType ErrorCode
		{
			get
			{
				return this.errorCodeField;
			}
			set
			{
				this.errorCodeField = value;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.errorMessageField;
			}
			set
			{
				this.errorMessageField = value;
			}
		}

		public string FieldValue
		{
			get
			{
				return this.fieldValueField;
			}
			set
			{
				this.fieldValueField = value;
			}
		}

		private RuleFieldURIType fieldURIField;

		private RuleValidationErrorCodeType errorCodeField;

		private string errorMessageField;

		private string fieldValueField;
	}
}
