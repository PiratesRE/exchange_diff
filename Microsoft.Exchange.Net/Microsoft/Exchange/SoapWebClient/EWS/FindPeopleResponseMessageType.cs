using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class FindPeopleResponseMessageType : ResponseMessageType
	{
		[XmlArrayItem("Persona", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public PersonaType[] People;

		public int TotalNumberOfPeopleInView;

		[XmlIgnore]
		public bool TotalNumberOfPeopleInViewSpecified;

		public int FirstMatchingRowIndex;

		[XmlIgnore]
		public bool FirstMatchingRowIndexSpecified;

		public int FirstLoadedRowIndex;

		[XmlIgnore]
		public bool FirstLoadedRowIndexSpecified;
	}
}
