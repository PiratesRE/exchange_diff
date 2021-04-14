using System;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	internal class ExpiringTenantRelocationStateValue : ExpiringValue<TenantRelocationState, ExpiringTenantRelocationStateValue.TenantRelocationStateExpirationWindowProvider>
	{
		public ExpiringTenantRelocationStateValue(TenantRelocationState value) : base(value)
		{
		}

		internal class TenantRelocationStateExpirationWindowProvider : IExpirationWindowProvider<TenantRelocationState>
		{
			internal static TimeSpan DefaultExpirationWindow
			{
				get
				{
					return TimeSpan.FromMinutes((double)TenantRelocationConfigImpl.GetConfig<int>("DefaultRelocationCacheExpirationTimeInMinutes"));
				}
			}

			internal static TimeSpan AggressiveExpirationWindow
			{
				get
				{
					return TimeSpan.FromMinutes((double)TenantRelocationConfigImpl.GetConfig<int>("AggressiveRelocationCacheExpirationTimeInMinutes"));
				}
			}

			internal static TimeSpan ModerateExpirationWindow
			{
				get
				{
					return TimeSpan.FromMinutes((double)TenantRelocationConfigImpl.GetConfig<int>("ModerateRelocationCacheExpirationTimeInMinutes"));
				}
			}

			TimeSpan IExpirationWindowProvider<TenantRelocationState>.GetExpirationWindow(TenantRelocationState value)
			{
				return ExpiringTenantRelocationStateValue.TenantRelocationStateExpirationWindowProvider.GetExpirationWindow(value);
			}

			internal static TimeSpan GetExpirationWindow(TenantRelocationState value)
			{
				switch (value.SourceForestState)
				{
				case TenantRelocationStatus.NotStarted:
					return ExpiringTenantRelocationStateValue.TenantRelocationStateExpirationWindowProvider.DefaultExpirationWindow;
				case TenantRelocationStatus.Synchronization:
				case TenantRelocationStatus.Lockdown:
					return ExpiringTenantRelocationStateValue.TenantRelocationStateExpirationWindowProvider.AggressiveExpirationWindow;
				case TenantRelocationStatus.Retired:
					return ExpiringTenantRelocationStateValue.TenantRelocationStateExpirationWindowProvider.ModerateExpirationWindow;
				default:
					throw new NotSupportedException(value.SourceForestState.ToString());
				}
			}
		}
	}
}
