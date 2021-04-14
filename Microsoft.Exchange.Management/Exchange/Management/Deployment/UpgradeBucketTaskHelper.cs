using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	internal sealed class UpgradeBucketTaskHelper
	{
		internal static int GetMailboxCount(ExchangeUpgradeBucket bucket)
		{
			int num = 0;
			List<ADObjectId> list = new List<ADObjectId>(bucket.Organizations.Count);
			foreach (ADObjectId adobjectId in bucket.Organizations)
			{
				int? mailboxCountFromCache = UpgradeBucketTaskHelper.MailboxCountCache.GetMailboxCountFromCache(adobjectId);
				if (mailboxCountFromCache != null)
				{
					num += mailboxCountFromCache.Value;
				}
				else
				{
					ADObjectIdResolutionHelper.ResolveDN(adobjectId);
				}
			}
			list.Sort((ADObjectId x, ADObjectId y) => x.PartitionGuid.CompareTo(y.PartitionGuid));
			ADObjectId adobjectId2 = null;
			ITenantConfigurationSession tenantConfigurationSession = null;
			foreach (ADObjectId adobjectId3 in list)
			{
				if (adobjectId2 == null || !adobjectId2.PartitionGuid.Equals(adobjectId3.PartitionGuid))
				{
					tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAllTenantsObjectId(adobjectId3), 59, "GetMailboxCount", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\UpgradeBucketTaskHelper.cs");
				}
				ExchangeConfigurationUnit exchangeConfigurationUnit = tenantConfigurationSession.Read<ExchangeConfigurationUnit>(adobjectId3);
				if (exchangeConfigurationUnit != null)
				{
					int mailboxCount = UpgradeBucketTaskHelper.MailboxCountCache.GetMailboxCount(exchangeConfigurationUnit.OrganizationId, tenantConfigurationSession);
					num += mailboxCount;
				}
				adobjectId2 = adobjectId3;
			}
			return num;
		}

		internal static void ValidateSourceAndTargetVersions(string sourceVersion, string targetVersion, Task.ErrorLoggerDelegate errorLogger)
		{
			string[] array = sourceVersion.Split(new char[]
			{
				'.'
			});
			string[] array2 = targetVersion.Split(new char[]
			{
				'.'
			});
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != "*" && array2[i] != "*")
				{
					if (int.Parse(array[i]) < int.Parse(array2[i]))
					{
						return;
					}
					if (int.Parse(array[i]) > int.Parse(array2[i]))
					{
						errorLogger(new RecipientTaskException(Strings.ExchangeUpgradeBucketSourceVersionBiggerThanTarget(sourceVersion, targetVersion)), ExchangeErrorCategory.Client, null);
					}
				}
				else
				{
					errorLogger((array[i] == "*") ? new RecipientTaskException(Strings.ExchangeUpgradeBucketTargetIncludedInSource(sourceVersion, targetVersion)) : new RecipientTaskException(Strings.ExchangeUpgradeBucketSourceIncludedInTarget(sourceVersion, targetVersion)), ExchangeErrorCategory.Client, null);
				}
			}
		}

		internal static void ValidateOrganizationAddition(ITopologyConfigurationSession configSession, OrganizationId organizationId, ExchangeUpgradeBucket exchangeUpgradeBucket, Task.ErrorLoggerDelegate errorLogger)
		{
			if (!exchangeUpgradeBucket.MaxMailboxes.IsUnlimited && !exchangeUpgradeBucket.Organizations.Contains(organizationId.ConfigurationUnit))
			{
				int mailboxCount = UpgradeBucketTaskHelper.MailboxCountCache.GetMailboxCount(organizationId, configSession);
				int mailboxCount2 = UpgradeBucketTaskHelper.GetMailboxCount(exchangeUpgradeBucket);
				int num = exchangeUpgradeBucket.MaxMailboxes.Value - mailboxCount2;
				if (mailboxCount > num)
				{
					errorLogger(new RecipientTaskException(Strings.ExchangeUpgradeBucketNotEnoughCapacity(exchangeUpgradeBucket.ToString(), num.ToString(), mailboxCount.ToString())), ExchangeErrorCategory.Client, organizationId);
				}
			}
		}

		internal static void ValidateOrganizationVersion(ExchangeConfigurationUnit configurationUnit, ExchangeUpgradeBucket exchangeUpgradeBucket, Task.ErrorLoggerDelegate errorLogger)
		{
			string text = configurationUnit.IsUpgradingOrganization ? exchangeUpgradeBucket.TargetVersion : exchangeUpgradeBucket.SourceVersion;
			if (!UpgradeBucketTaskHelper.ValidateExchangeObjectVersion(configurationUnit.AdminDisplayVersion, text))
			{
				errorLogger(new RecipientTaskException(Strings.ExchangeUpgradeBucketInvalidOrganizationVersion(configurationUnit.AdminDisplayVersion.ToString(), text)), ExchangeErrorCategory.Client, null);
			}
		}

		private static bool ValidateExchangeObjectVersion(ExchangeObjectVersion version, string versionPattern)
		{
			string[] array = versionPattern.Split(new char[]
			{
				'.'
			});
			int[] array2 = new int[]
			{
				(int)version.ExchangeBuild.Major,
				(int)version.ExchangeBuild.Minor,
				(int)version.ExchangeBuild.Build,
				(int)version.ExchangeBuild.BuildRevision
			};
			int num = 0;
			while (num < array.Length && !(array[num] == "*"))
			{
				if (int.Parse(array[num]) != array2[num])
				{
					return false;
				}
				num++;
			}
			return true;
		}

		private static readonly UpgradeBucketTaskHelper.OrganizationMailboxCountCache MailboxCountCache = new UpgradeBucketTaskHelper.OrganizationMailboxCountCache();

		private sealed class OrganizationMailboxCountCache
		{
			public int? GetMailboxCountFromCache(ADObjectId organizationId)
			{
				UpgradeBucketTaskHelper.OrganizationMailboxCountCache.OrganizationCountCacheEntry organizationCountCacheEntry;
				if (this.organizationCache.TryGetValue(organizationId, out organizationCountCacheEntry) && ExDateTime.Now - organizationCountCacheEntry.WhenRead < UpgradeBucketTaskHelper.OrganizationMailboxCountCache.CacheLifeTime)
				{
					return new int?(organizationCountCacheEntry.MailboxCount);
				}
				return null;
			}

			public int GetMailboxCount(OrganizationId organizationId, IConfigurationSession configSession)
			{
				int? mailboxCountFromCache = this.GetMailboxCountFromCache(organizationId.ConfigurationUnit);
				if (mailboxCountFromCache == null)
				{
					mailboxCountFromCache = new int?(SystemAddressListMemberCount.GetCount(configSession, organizationId, "All Mailboxes(VLV)", false));
					this.organizationCache[organizationId.ConfigurationUnit] = new UpgradeBucketTaskHelper.OrganizationMailboxCountCache.OrganizationCountCacheEntry
					{
						WhenRead = ExDateTime.Now,
						MailboxCount = mailboxCountFromCache.Value
					};
				}
				return mailboxCountFromCache.Value;
			}

			private static readonly TimeSpan CacheLifeTime = new TimeSpan(6, 0, 0);

			private readonly Dictionary<ADObjectId, UpgradeBucketTaskHelper.OrganizationMailboxCountCache.OrganizationCountCacheEntry> organizationCache = new Dictionary<ADObjectId, UpgradeBucketTaskHelper.OrganizationMailboxCountCache.OrganizationCountCacheEntry>();

			private struct OrganizationCountCacheEntry
			{
				public ExDateTime WhenRead;

				public int MailboxCount;
			}
		}
	}
}
