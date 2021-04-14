using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetUMCallDataRecordsResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetUMCallDataRecordsResponseMessage : ResponseMessage
	{
		public GetUMCallDataRecordsResponseMessage()
		{
		}

		internal GetUMCallDataRecordsResponseMessage(ServiceResultCode code, ServiceError error, GetUMCallDataRecordsResponseMessage response) : base(code, error)
		{
			if (response != null)
			{
				this.CallDataRecords = response.CallDataRecords;
			}
		}

		[XmlArray(ElementName = "CallDataRecords")]
		[DataMember]
		[XmlArrayItem(ElementName = "CDRData", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public CDRData[] CallDataRecords { get; set; }

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetUMCallDataRecordsResponseMessage;
		}
	}
}
