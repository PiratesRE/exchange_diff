using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "ConversationNodeSortOrder", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public enum ConversationNodeSortOrder
	{
		TreeOrderAscending,
		TreeOrderDescending,
		DateOrderAscending,
		DateOrderDescending
	}
}
