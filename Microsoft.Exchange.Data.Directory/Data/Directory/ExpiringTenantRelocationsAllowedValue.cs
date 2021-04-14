using System;
using Microsoft.Exchange.Common.Cache;

namespace Microsoft.Exchange.Data.Directory
{
	internal class ExpiringTenantRelocationsAllowedValue : ExpiringValue<bool, ExpiringTenantRelocationsAllowedValue.TenantRelocationsAllowedExpirationWindowProvider>
	{
		public ExpiringTenantRelocationsAllowedValue(bool value) : base(value)
		{
		}

		internal class TenantRelocationsAllowedExpirationWindowProvider : IExpirationWindowProvider<bool>
		{
			TimeSpan IExpirationWindowProvider<bool>.GetExpirationWindow(bool unused)
			{
				return ExpiringTenantRelocationsAllowedValue.TenantRelocationsAllowedExpirationWindowProvider.expirationWindow;
			}

			private static readonly TimeSpan expirationWindow = TimeSpan.FromHours(8.0);
		}
	}
}
