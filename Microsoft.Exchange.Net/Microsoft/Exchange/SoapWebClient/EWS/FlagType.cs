using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class FlagType
	{
		public FlagStatusType FlagStatus;

		public DateTime StartDate;

		[XmlIgnore]
		public bool StartDateSpecified;

		public DateTime DueDate;

		[XmlIgnore]
		public bool DueDateSpecified;

		public DateTime CompleteDate;

		[XmlIgnore]
		public bool CompleteDateSpecified;
	}
}
