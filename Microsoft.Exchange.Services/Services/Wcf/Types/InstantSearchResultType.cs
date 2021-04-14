using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[Flags]
	[XmlType(TypeName = "InstantSearchResultType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public enum InstantSearchResultType
	{
		None = 0,
		Suggestions = 1,
		ItemResults = 2,
		ConversationResults = 4,
		Refiners = 8,
		SearchTerms = 16,
		Errors = 32,
		QueryStatistics = 64,
		CalendarItemResults = 128,
		PersonaResults = 256,
		SuggestionsPrimer = 512
	}
}
