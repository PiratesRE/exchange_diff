using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	internal class UserContextStatistics
	{
		public bool CookieCreated { get; set; }

		public bool Created { get; set; }

		public UserContextCreationError Error { get; set; }

		public int AcquireLatency { get; set; }

		public int LoadTime { get; set; }

		public int ExchangePrincipalCreationTime { get; set; }

		public int MiniRecipientCreationTime { get; set; }

		public int SKUCapabilityTestTime { get; set; }
	}
}
