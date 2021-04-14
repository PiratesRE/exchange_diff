using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "ConversationActionTypeType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public enum ConversationActionType
	{
		AlwaysCategorize,
		AlwaysDelete,
		AlwaysMove,
		Delete,
		Move,
		Copy,
		SetReadState,
		SetRetentionPolicy,
		UpdateAlwaysCategorizeRule,
		Flag,
		SetClutterState
	}
}
