using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Flags]
	[XmlType(TypeName = "SearchResultType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum SearchResultType
	{
		StatisticsOnly = 1,
		PreviewOnly = 2
	}
}
