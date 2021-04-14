using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Search
{
	[XmlType(TypeName = "StandardGroupByType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum StandardGroupBys
	{
		[XmlEnum(Name = "ConversationTopic")]
		ConversationTopic
	}
}
