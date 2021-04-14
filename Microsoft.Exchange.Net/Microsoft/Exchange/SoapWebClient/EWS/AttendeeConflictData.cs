using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(IndividualAttendeeConflictData))]
	[XmlInclude(typeof(TooBigGroupAttendeeConflictData))]
	[XmlInclude(typeof(UnknownAttendeeConflictData))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlInclude(typeof(GroupAttendeeConflictData))]
	[Serializable]
	public abstract class AttendeeConflictData
	{
	}
}
