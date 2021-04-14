using System;
using System.Linq;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class DatabaseInfoCache : LazyLookupTimeoutCache<Guid, DatabaseInfo>
	{
		public DatabaseInfoCache(IConfigurationSession adConfigurationSession, TimeSpan cacheTimeout) : base(2, 1000, false, cacheTimeout)
		{
			this.adConfigurationSession = adConfigurationSession;
		}

		protected override DatabaseInfo CreateOnCacheMiss(Guid key, ref bool shouldAdd)
		{
			ADRawEntry database = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				database = this.adConfigurationSession.Find(this.adConfigurationSession.ConfigurationNamingContext, QueryScope.SubTree, new AndFilter(new QueryFilter[]
				{
					new MailboxDatabase().ImplicitFilter,
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, new ADObjectId(key))
				}), null, 1, DatabaseInfoCache.RequiredProperties).FirstOrDefault<ADRawEntry>();
			});
			if (!adoperationResult.Succeeded || database == null)
			{
				throw new CallFailedException(string.Format("Failed to look up database information for Database Guid {0}", key), adoperationResult.Exception);
			}
			SecurityDescriptor securityDescriptor = (SecurityDescriptor)database[ADObjectSchema.NTSecurityDescriptor];
			if (securityDescriptor != null)
			{
				return new DatabaseInfo(securityDescriptor);
			}
			throw new CallFailedException(string.Format("Security descriptor not available for database {0}", key));
		}

		private static readonly PropertyDefinition[] RequiredProperties = new PropertyDefinition[]
		{
			ADObjectSchema.Guid,
			MailboxDatabaseSchema.Name,
			ADObjectSchema.NTSecurityDescriptor
		};

		private readonly IConfigurationSession adConfigurationSession;
	}
}
