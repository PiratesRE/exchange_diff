using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[XmlType(TypeName = "SearchSuggestionType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class SearchSuggestionType
	{
		public SearchSuggestionType()
		{
		}

		internal SearchSuggestionType(string suggestedQuery, double weight, SuggestionSourceType suggestionSource)
		{
			this.SuggestedQuery = suggestedQuery;
			this.Weight = weight;
			this.SuggestionSource = suggestionSource;
		}

		[DataMember]
		public string SuggestedQuery { get; set; }

		[DataMember]
		public double Weight { get; set; }

		[DataMember]
		public SuggestionSourceType SuggestionSource { get; set; }
	}
}
