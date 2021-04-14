using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "GetNonIndexableItemStatisticsResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class GetNonIndexableItemStatisticsResponse : ResponseMessage
	{
		public GetNonIndexableItemStatisticsResponse()
		{
		}

		internal GetNonIndexableItemStatisticsResponse(ServiceResultCode code, ServiceError error, NonIndexableItemStatisticResult[] results) : base(code, error)
		{
			this.NonIndexableItemStatisticsResult = results;
		}

		[DataMember(Name = "NonIndexableItemStatistics", IsRequired = false)]
		[XmlArrayItem(ElementName = "NonIndexableItemStatistic", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(NonIndexableItemStatisticResult))]
		[XmlArray(ElementName = "NonIndexableItemStatistics", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public NonIndexableItemStatisticResult[] NonIndexableItemStatisticsResult { get; set; }
	}
}
