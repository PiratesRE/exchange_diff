using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class GetUMCallSummaryResponseMessageType : ResponseMessageType
	{
		[XmlArrayItem("UMReportRawCounters", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public UMReportRawCountersType[] UMReportRawCountersCollection
		{
			get
			{
				return this.uMReportRawCountersCollectionField;
			}
			set
			{
				this.uMReportRawCountersCollectionField = value;
			}
		}

		private UMReportRawCountersType[] uMReportRawCountersCollectionField;
	}
}
