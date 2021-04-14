using System;
using Microsoft.Exchange.Configuration.Authorization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class OrganizationCacheQueryProcessor<T> : RbacQuery.RbacQueryProcessor, INamedQueryProcessor
	{
		public string Name { get; private set; }

		public OrganizationCacheQueryProcessor(string roleName, string key, Func<T, bool> predicate = null)
		{
			if (string.IsNullOrEmpty(roleName))
			{
				throw new ArgumentNullException("roleName");
			}
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("key");
			}
			this.Name = roleName;
			this.key = key;
			this.predicate = predicate;
		}

		public sealed override bool CanCache
		{
			get
			{
				return false;
			}
		}

		public sealed override bool? TryIsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
		{
			T t;
			bool flag = OrganizationCache.TryGetValue<T>(this.key, out t);
			bool value = flag && ((this.predicate != null) ? this.predicate(t) : ((bool)((object)t)));
			return new bool?(value);
		}

		public static readonly OrganizationCacheQueryProcessor<bool> XPremiseEnt = new OrganizationCacheQueryProcessor<bool>("XPremiseEnt", "EntHasTargetDeliveryDomain", null);

		public static readonly OrganizationCacheQueryProcessor<bool> XPremiseDC = new OrganizationCacheQueryProcessor<bool>("XPremiseDC", "DCIsDirSyncRunning", null);

		private readonly string key;

		private readonly Func<T, bool> predicate;
	}
}
