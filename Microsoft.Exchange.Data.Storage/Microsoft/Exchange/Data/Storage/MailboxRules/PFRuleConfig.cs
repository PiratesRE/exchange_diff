using System;
using Microsoft.Exchange.Data.Directory.IsMemberOfProvider;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PFRuleConfig : IRuleConfig
	{
		public static PFRuleConfig Instance
		{
			get
			{
				return PFRuleConfig.instance;
			}
		}

		public IsMemberOfResolver<string> IsMemberOfResolver
		{
			get
			{
				if (this.isMemberOfResolver == null)
				{
					this.isMemberOfResolver = new IsMemberOfResolver<string>(new PFRuleConfig.PFIsMemberOfResolverConfig(), new IsMemberOfResolverPerformanceCounters("PublicFolderRules"), new IsMemberOfResolverADAdapter<string>.LegacyDNResolver(false));
				}
				return this.isMemberOfResolver;
			}
		}

		public object SCLJunkThreshold
		{
			get
			{
				return this.sclJunkThreshold;
			}
		}

		private const int DefaultSCLJunkThreshold = 4;

		private static PFRuleConfig instance = new PFRuleConfig();

		private IsMemberOfResolver<string> isMemberOfResolver;

		private object sclJunkThreshold = 4;

		private class PFIsMemberOfResolverConfig : IsMemberOfResolverConfig
		{
			public bool Enabled
			{
				get
				{
					return true;
				}
			}

			public TimeSpan ExpandedGroupsCleanupInterval
			{
				get
				{
					return TimeSpan.FromHours(1.0);
				}
			}

			public TimeSpan ExpandedGroupsExpirationInterval
			{
				get
				{
					return TimeSpan.FromHours(3.0);
				}
			}

			public long ExpandedGroupsMaxSize
			{
				get
				{
					return 536870912L;
				}
			}

			public TimeSpan ExpandedGroupsPurgeInterval
			{
				get
				{
					return TimeSpan.FromMinutes(5.0);
				}
			}

			public TimeSpan ExpandedGroupsRefreshInterval
			{
				get
				{
					return TimeSpan.FromMinutes(10.0);
				}
			}

			public TimeSpan ResolvedGroupsCleanupInterval
			{
				get
				{
					return TimeSpan.FromHours(1.0);
				}
			}

			public TimeSpan ResolvedGroupsExpirationInterval
			{
				get
				{
					return TimeSpan.FromHours(3.0);
				}
			}

			public long ResolvedGroupsMaxSize
			{
				get
				{
					return 33554432L;
				}
			}

			public TimeSpan ResolvedGroupsPurgeInterval
			{
				get
				{
					return TimeSpan.FromMinutes(5.0);
				}
			}

			public TimeSpan ResolvedGroupsRefreshInterval
			{
				get
				{
					return TimeSpan.FromMinutes(10.0);
				}
			}
		}
	}
}
