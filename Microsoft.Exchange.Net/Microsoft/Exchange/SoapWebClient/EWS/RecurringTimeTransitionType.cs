using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlInclude(typeof(RecurringDateTransitionType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[XmlInclude(typeof(RecurringDayTransitionType))]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public abstract class RecurringTimeTransitionType : TransitionType
	{
		[XmlElement(DataType = "duration")]
		public string TimeOffset;

		public int Month;
	}
}
