using System;
using System.Xml.Serialization;
using Microsoft.Exchange.InfoWorker.Common.MeetingSuggestions;

namespace Microsoft.Exchange.InfoWorker.Common.Availability.Proxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class SuggestionsResponse
	{
		[XmlElement(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public ResponseMessage ResponseMessage
		{
			get
			{
				return this.responseMessageField;
			}
			set
			{
				this.responseMessageField = value;
			}
		}

		[XmlArrayItem(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
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

		private SuggestionDayResult[] suggestionDayResultArray;

		private ResponseMessage responseMessageField;
	}
}
