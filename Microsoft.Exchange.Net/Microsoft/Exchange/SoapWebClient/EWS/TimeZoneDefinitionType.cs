using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class TimeZoneDefinitionType
	{
		[XmlArrayItem("Period", IsNullable = false)]
		public PeriodType[] Periods;

		[XmlArrayItem("TransitionsGroup", IsNullable = false)]
		public ArrayOfTransitionsType[] TransitionsGroups;

		public ArrayOfTransitionsType Transitions;

		[XmlAttribute]
		public string Id;

		[XmlAttribute]
		public string Name;
	}
}
