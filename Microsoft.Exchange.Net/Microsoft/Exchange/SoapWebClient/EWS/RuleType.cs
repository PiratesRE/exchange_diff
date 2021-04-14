using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class RuleType
	{
		public string RuleId;

		public string DisplayName;

		public int Priority;

		public bool IsEnabled;

		public bool IsNotSupported;

		[XmlIgnore]
		public bool IsNotSupportedSpecified;

		public bool IsInError;

		[XmlIgnore]
		public bool IsInErrorSpecified;

		public RulePredicatesType Conditions;

		public RulePredicatesType Exceptions;

		public RuleActionsType Actions;
	}
}
