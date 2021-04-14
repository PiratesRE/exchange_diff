using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.InfoWorker.Common.MeetingSuggestions;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.InfoWorker.Availability
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SuggestionsResponse
	{
		[DataMember]
		[XmlElement(IsNullable = false)]
		public ResponseMessage ResponseMessage
		{
			get
			{
				return this.responseMessage;
			}
			set
			{
				this.responseMessage = value;
			}
		}

		[XmlArray(IsNullable = false)]
		[DataMember]
		[XmlArrayItem(Type = typeof(SuggestionDayResult), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public SuggestionDayResult[] SuggestionDayResultArray
		{
			get
			{
				return this.suggestionDayResultArray;
			}
			set
			{
				this.suggestionDayResultArray = value;
			}
		}

		internal static SuggestionsResponse CreateFrom(SuggestionDayResult[] suggestionDayResultArray, LocalizedException suggestionsException)
		{
			if (suggestionDayResultArray == null && suggestionsException == null)
			{
				return null;
			}
			return new SuggestionsResponse
			{
				SuggestionDayResultArray = suggestionDayResultArray,
				ResponseMessage = ResponseMessageBuilder.ResponseMessageFromExchangeException(suggestionsException)
			};
		}

		private SuggestionsResponse()
		{
		}

		private SuggestionDayResult[] suggestionDayResultArray;

		private ResponseMessage responseMessage;
	}
}
