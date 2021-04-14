using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[XmlInclude(typeof(TentativelyAcceptItemType))]
	[XmlInclude(typeof(AcceptItemType))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[XmlInclude(typeof(DeclineItemType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class MeetingRegistrationResponseObjectType : WellKnownResponseObjectType
	{
		public DateTime ProposedStart;

		[XmlIgnore]
		public bool ProposedStartSpecified;

		public DateTime ProposedEnd;

		[XmlIgnore]
		public bool ProposedEndSpecified;
	}
}
