using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class Suggestion
	{
		public DateTime MeetingTime;

		public bool IsWorkTime;

		public SuggestionQuality SuggestionQuality;

		[XmlArrayItem(typeof(IndividualAttendeeConflictData))]
		[XmlArrayItem(typeof(TooBigGroupAttendeeConflictData))]
		[XmlArrayItem(typeof(GroupAttendeeConflictData))]
		[XmlArrayItem(typeof(UnknownAttendeeConflictData))]
		public AttendeeConflictData[] AttendeeConflictDataArray;
	}
}
