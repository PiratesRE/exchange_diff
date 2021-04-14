using System;
using System.Threading;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Config;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.MailboxLoadBalance.Anchor
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AutomaticLoadBalanceCacheComponent : CacheProcessorBase
	{
		public AutomaticLoadBalanceCacheComponent(LoadBalanceAnchorContext context, WaitHandle stopEvent) : base(context, stopEvent)
		{
			this.settings = context.Settings;
		}

		internal override string Name
		{
			get
			{
				return "AutomaticLoadBalancingProcessor";
			}
		}

		internal override bool ShouldProcess()
		{
			return this.settings.AutomaticLoadBalancingEnabled && !this.settings.LoadBalanceBlocked;
		}

		internal override bool Process(JobCache data)
		{
			bool retry = false;
			base.Context.Logger.LogVerbose("Starting automatic load balancing.", new object[0]);
			LoadBalanceAnchorContext context = base.Context as LoadBalanceAnchorContext;
			if (context == null)
			{
				base.Context.Logger.LogError(null, "Context is null or not from an expected type.", new object[0]);
				return false;
			}
			if (!this.settings.AutomaticLoadBalancingEnabled)
			{
				base.Context.Logger.LogWarning("Automatic load balancing is no longer enabled.", new object[0]);
				return false;
			}
			foreach (CacheEntryBase cacheEntryBase in data.Get())
			{
				if (!cacheEntryBase.Validate())
				{
					base.Context.Logger.LogWarning("Invalid cache entry found. Skipped.", new object[0]);
				}
				else if (!cacheEntryBase.IsLocal || !cacheEntryBase.IsActive)
				{
					base.Context.Logger.LogWarning("Inactive or non local cache entry found. Skipped.", new object[0]);
				}
				else if (!this.settings.LoadBalanceBlocked)
				{
					CommonUtils.ProcessKnownExceptions(delegate
					{
						new AutomaticLoadBalancer(context).LoadBalanceForest();
					}, delegate(Exception exception)
					{
						retry = true;
						this.Context.Logger.LogError(exception, "Failed to load balance forest.", new object[0]);
						return true;
					});
				}
			}
			return retry;
		}

		private readonly ILoadBalanceSettings settings;
	}
}
