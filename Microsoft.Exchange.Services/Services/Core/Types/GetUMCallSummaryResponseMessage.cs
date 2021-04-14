using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetUMCallSummaryResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetUMCallSummaryResponseMessage : ResponseMessage
	{
		public GetUMCallSummaryResponseMessage()
		{
		}

		internal GetUMCallSummaryResponseMessage(ServiceResultCode code, ServiceError error, GetUMCallSummaryResponseMessage response) : base(code, error)
		{
			if (response != null)
			{
				this.UMReportRawCountersCollection = response.UMReportRawCountersCollection;
			}
		}

		[XmlArrayItem(ElementName = "UMReportRawCounters", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[XmlArray(ElementName = "UMReportRawCountersCollection")]
		[DataMember]
		public UMReportRawCounters[] UMReportRawCountersCollection { get; set; }

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetUMCallSummaryResponseMessage;
		}
	}
}
