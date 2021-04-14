using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.HA
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	public class ThrottlingData
	{
		[CLSCompliant(false)]
		public ulong DataProtectionHealth
		{
			get
			{
				return this.dataProtectionHealth;
			}
		}

		[CLSCompliant(false)]
		public ulong DataAvailabilityHealth
		{
			get
			{
				return this.dataAvailabilityHealth;
			}
		}

		[CLSCompliant(false)]
		public void Update(ulong dataProtectionHealth, ulong dataAvailabilityHealth)
		{
			this.dataProtectionHealth = ((dataProtectionHealth < (ulong)-1) ? dataProtectionHealth : ((ulong)-1));
			this.dataAvailabilityHealth = ((dataAvailabilityHealth < (ulong)-1) ? dataAvailabilityHealth : ((ulong)-1));
		}

		internal void MarkFailed()
		{
			this.Update(52428800UL, 1048576000UL);
		}

		internal void MarkHealthy()
		{
			this.Update(0UL, 0UL);
		}

		internal const long MiB = 1048576L;

		internal const uint MaxCopyQueue = 50U;

		internal const uint MaxReplayQueue = 1000U;

		private ulong dataProtectionHealth;

		private ulong dataAvailabilityHealth;
	}
}
