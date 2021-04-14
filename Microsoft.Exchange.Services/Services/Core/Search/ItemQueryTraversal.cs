using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Search
{
	[XmlType(TypeName = "ItemQueryTraversalType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public enum ItemQueryTraversal
	{
		Shallow,
		SoftDeleted = 2,
		Associated = 1
	}
}
