using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Search
{
	[XmlType(TypeName = "ItemChoiceType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IncludeInSchema = false)]
	public enum QueryType
	{
		Items,
		Groups
	}
}
