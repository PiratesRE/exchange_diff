using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "TeamMailboxLifecycleStateType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public enum TeamMailboxLifecycleState
	{
		Active,
		Closed,
		Unlinked,
		PendingDelete
	}
}
