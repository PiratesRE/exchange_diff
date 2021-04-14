using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.InfoWorker.Availability
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetUserAvailabilityResponse : IExchangeWebMethodResponse
	{
		[DataMember]
		[XmlArrayItem(Type = typeof(FreeBusyResponse), Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages", IsNullable = false)]
		[XmlArray(IsNullable = false)]
		public FreeBusyResponse[] FreeBusyResponseArray
		{
			get
			{
				return this.freeBusyResponseArray;
			}
			set
			{
				this.freeBusyResponseArray = value;
			}
		}

		[DataMember]
		[XmlElement(IsNullable = false)]
		public SuggestionsResponse SuggestionsResponse
		{
			get
			{
				return this.suggestionsResponse;
			}
			set
			{
				this.suggestionsResponse = value;
			}
		}

		internal static GetUserAvailabilityResponse CreateFrom(AvailabilityQueryResult queryResult)
		{
			if (queryResult == null)
			{
				return null;
			}
			GetUserAvailabilityResponse getUserAvailabilityResponse = new GetUserAvailabilityResponse();
			if (queryResult.FreeBusyResults != null && queryResult.FreeBusyResults.Length > 0)
			{
				int num = queryResult.FreeBusyResults.Length;
				getUserAvailabilityResponse.FreeBusyResponseArray = new FreeBusyResponse[num];
				for (int i = 0; i < num; i++)
				{
					FreeBusyQueryResult freeBusyQueryResult = queryResult.FreeBusyResults[i];
					if (freeBusyQueryResult != null)
					{
						getUserAvailabilityResponse.FreeBusyResponseArray[i] = FreeBusyResponse.CreateFrom(freeBusyQueryResult, i);
					}
				}
			}
			getUserAvailabilityResponse.SuggestionsResponse = SuggestionsResponse.CreateFrom(queryResult.DailyMeetingSuggestions, queryResult.MeetingSuggestionsException);
			return getUserAvailabilityResponse;
		}

		public ResponseType GetResponseType()
		{
			return ResponseType.GetUserAvailabilityResponseMessage;
		}

		ResponseCodeType IExchangeWebMethodResponse.GetErrorCodeToLog()
		{
			ResponseCodeType responseCodeType = ResponseCodeType.NoError;
			if (this.FreeBusyResponseArray != null)
			{
				foreach (FreeBusyResponse freeBusyResponse in this.FreeBusyResponseArray)
				{
					if (freeBusyResponse != null && freeBusyResponse.ResponseMessage != null && freeBusyResponse.ResponseMessage.ResponseCode != ResponseCodeType.NoError)
					{
						responseCodeType = freeBusyResponse.ResponseMessage.ResponseCode;
						break;
					}
				}
			}
			if (responseCodeType == ResponseCodeType.NoError && this.SuggestionsResponse != null && this.SuggestionsResponse.ResponseMessage != null)
			{
				responseCodeType = this.SuggestionsResponse.ResponseMessage.ResponseCode;
			}
			return responseCodeType;
		}

		private FreeBusyResponse[] freeBusyResponseArray;

		private SuggestionsResponse suggestionsResponse;
	}
}
