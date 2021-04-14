using System;

namespace Microsoft.Exchange.Rpc.ActiveManager
{
	[Serializable]
	internal sealed class AmDeferredRecoveryEntry
	{
		public AmDeferredRecoveryEntry(string serverFqdn, string recoveryDueTimeInUtc)
		{
		}

		public string ServerFqdn
		{
			get
			{
				return this.m_serverFqdn;
			}
		}

		public string RecoveryDueTimeInUtc
		{
			get
			{
				return this.m_recoveryDueTimeInUtc;
			}
		}

		private readonly string m_serverFqdn = serverFqdn;

		private readonly string m_recoveryDueTimeInUtc = recoveryDueTimeInUtc;
	}
}
