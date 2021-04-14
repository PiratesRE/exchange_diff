using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class ADConsumer : ICacheConsumer
	{
		internal ADConsumer(ADObjectId id, ITopologyConfigurationSession configSession, OrganizationCache cache)
		{
			this.id = id;
			this.configSession = configSession;
			this.cache = cache;
		}

		object ICacheConsumer.Id
		{
			get
			{
				return this.id;
			}
		}

		protected ADObjectId Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		internal IConfigurationSession ConfigSession
		{
			get
			{
				return this.configSession;
			}
		}

		internal OrganizationCache Cache
		{
			get
			{
				return this.cache;
			}
		}

		protected void OnChange(ADNotificationEventArgs args)
		{
			this.cache.InvalidateCache(this.id);
		}

		internal static readonly ITopologyConfigurationSession ADSystemConfigurationSessionInstance = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 41, "ADSystemConfigurationSessionInstance", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\ActiveDirectory\\ADConsumer.cs");

		private readonly IConfigurationSession configSession;

		private readonly OrganizationCache cache;

		private ADObjectId id;
	}
}
