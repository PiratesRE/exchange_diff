using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal enum RemoveFavoriteMetadata
	{
		[DisplayName("RF.PID")]
		PersonaId,
		[DisplayName("RF.NOC")]
		NumberOfContacts
	}
}
