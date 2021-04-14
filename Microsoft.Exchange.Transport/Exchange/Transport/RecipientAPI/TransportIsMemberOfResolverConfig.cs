using System;
using Microsoft.Exchange.Data.Directory.IsMemberOfProvider;

namespace Microsoft.Exchange.Transport.RecipientAPI
{
	internal class TransportIsMemberOfResolverConfig : IsMemberOfResolverConfig
	{
		public TransportIsMemberOfResolverConfig(TransportAppConfig.IsMemberOfResolverConfiguration configuration)
		{
			this.configuration = configuration;
		}

		public bool Enabled
		{
			get
			{
				return this.configuration.Enabled;
			}
		}

		public long ResolvedGroupsMaxSize
		{
			get
			{
				return (long)this.configuration.ResolvedGroupsCacheConfiguration.MaxSize.ToBytes();
			}
		}

		public TimeSpan ResolvedGroupsExpirationInterval
		{
			get
			{
				return this.configuration.ResolvedGroupsCacheConfiguration.ExpirationInterval;
			}
		}

		public TimeSpan ResolvedGroupsCleanupInterval
		{
			get
			{
				return this.configuration.ResolvedGroupsCacheConfiguration.CleanupInterval;
			}
		}

		public TimeSpan ResolvedGroupsPurgeInterval
		{
			get
			{
				return this.configuration.ResolvedGroupsCacheConfiguration.PurgeInterval;
			}
		}

		public TimeSpan ResolvedGroupsRefreshInterval
		{
			get
			{
				return this.configuration.ResolvedGroupsCacheConfiguration.RefreshInterval;
			}
		}

		public long ExpandedGroupsMaxSize
		{
			get
			{
				return (long)this.configuration.ExpandedGroupsCacheConfiguration.MaxSize.ToBytes();
			}
		}

		public TimeSpan ExpandedGroupsExpirationInterval
		{
			get
			{
				return this.configuration.ExpandedGroupsCacheConfiguration.ExpirationInterval;
			}
		}

		public TimeSpan ExpandedGroupsCleanupInterval
		{
			get
			{
				return this.configuration.ExpandedGroupsCacheConfiguration.CleanupInterval;
			}
		}

		public TimeSpan ExpandedGroupsPurgeInterval
		{
			get
			{
				return this.configuration.ExpandedGroupsCacheConfiguration.PurgeInterval;
			}
		}

		public TimeSpan ExpandedGroupsRefreshInterval
		{
			get
			{
				return this.configuration.ExpandedGroupsCacheConfiguration.RefreshInterval;
			}
		}

		private readonly TransportAppConfig.IsMemberOfResolverConfiguration configuration;
	}
}
