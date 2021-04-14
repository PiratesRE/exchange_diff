using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Search
{
	[XmlType(TypeName = "SortDirectionType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum SortDirection
	{
		Ascending,
		Descending
	}
}
