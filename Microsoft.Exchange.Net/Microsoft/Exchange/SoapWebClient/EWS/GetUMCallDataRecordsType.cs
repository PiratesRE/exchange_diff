using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class GetUMCallDataRecordsType : BaseRequestType
	{
		public DateTime StartDateTime;

		[XmlIgnore]
		public bool StartDateTimeSpecified;

		public DateTime EndDateTime;

		[XmlIgnore]
		public bool EndDateTimeSpecified;

		public int Offset;

		[XmlIgnore]
		public bool OffsetSpecified;

		public int NumberOfRecords;

		[XmlIgnore]
		public bool NumberOfRecordsSpecified;

		public string UserLegacyExchangeDN;

		public UMCDRFilterByType FilterBy;
	}
}
