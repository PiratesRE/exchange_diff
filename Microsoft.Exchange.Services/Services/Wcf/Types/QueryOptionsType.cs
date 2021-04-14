using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[Flags]
	[XmlType(TypeName = "QueryOptionsType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public enum QueryOptionsType
	{
		None = 0,
		Suggestions = 1,
		Results = 2,
		Refiners = 4,
		SearchTerms = 8,
		ExplicitSearch = 16,
		SuggestionsPrimer = 32,
		AllowFuzzing = 64
	}
}
