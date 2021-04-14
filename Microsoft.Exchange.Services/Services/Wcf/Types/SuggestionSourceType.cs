using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Flags]
	[XmlType(TypeName = "SuggestionSourceType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum SuggestionSourceType
	{
		None = 0,
		RecentSearches = 1,
		Spelling = 2,
		Synonyms = 4,
		Nicknames = 8,
		TopN = 16,
		Fuzzy = 26,
		All = 31
	}
}
