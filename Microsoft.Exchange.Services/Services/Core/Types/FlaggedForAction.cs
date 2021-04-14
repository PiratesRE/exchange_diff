using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "FlaggedForActionType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public enum FlaggedForAction
	{
		Any,
		Call,
		DoNotForward,
		FollowUp,
		Forward = 5,
		FYI = 4,
		NoResponseNecessary = 6,
		Read,
		Reply,
		ReplyToAll,
		Review
	}
}
