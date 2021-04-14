using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class PartitionDataAggregator
	{
		public static MRSRequest FindFirstMRSRequestLinkedToDatabase(ADObjectId databaseId)
		{
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ExistsFilter(MRSRequestSchema.MRSRequestType),
				new ComparisonFilter(ComparisonOperator.Equal, MRSRequestSchema.MailboxMoveStorageMDB, databaseId),
				new NotFilter(new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, MRSRequestSchema.MailboxMoveStatus, RequestStatus.Completed),
					new ComparisonFilter(ComparisonOperator.Equal, MRSRequestSchema.MailboxMoveStatus, RequestStatus.CompletedWithWarning)
				}))
			});
			return PartitionDataAggregator.FindFirstMatchingConfigurationObject<MRSRequest>(filter, true, true, true, ConsistencyMode.PartiallyConsistent, true);
		}

		public static ADUser FindFirstMoveRequestLinkedToDatabase(ADObjectId databaseId)
		{
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.UserMailbox),
					new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.MailUser)
				}),
				new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.MailboxMoveTargetMDB, databaseId)
			});
			return PartitionDataAggregator.FindFirstMatchingRecipientObject<ADUser>(filter, true, true, true, ConsistencyMode.PartiallyConsistent, true, true);
		}

		public static ADUser FindFirstUserLinkedToDatabase(ADObjectId databaseId)
		{
			QueryFilter filter = PartitionDataAggregator.CreateUsersLinkedToDatabaseFilter(databaseId);
			return PartitionDataAggregator.FindFirstMatchingRecipientObject<ADUser>(filter, true, true, true, ConsistencyMode.PartiallyConsistent, true, true);
		}

		public static IEnumerable<TResult> FindAllUsersLinkedToDatabase<TResult>(ADObjectId databaseId) where TResult : ADObject, new()
		{
			QueryFilter filter = PartitionDataAggregator.CreateUsersLinkedToDatabaseFilter(databaseId);
			return PartitionDataAggregator.FindPagedAllMatchingObjects<TResult>(filter, true, true, true, ConsistencyMode.PartiallyConsistent, true, true, true);
		}

		public static ADUser FindFirstUserOrMoveRequestLinkedToDatabase(ADObjectId databaseId)
		{
			QueryFilter queryFilter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.MailboxMoveTargetMDB, databaseId),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.UserMailbox)
			});
			QueryFilter queryFilter2 = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.MailboxMoveTargetMDB, databaseId),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.MailUser)
			});
			QueryFilter queryFilter3 = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADMailboxRecipientSchema.Database, databaseId),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.UserMailbox)
			});
			QueryFilter queryFilter4 = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.ArchiveDatabaseRaw, databaseId),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.UserMailbox)
			});
			QueryFilter queryFilter5 = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.ArchiveDatabaseRaw, databaseId),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.MailUser)
			});
			QueryFilter queryFilter6 = new OrFilter(new QueryFilter[]
			{
				queryFilter3,
				queryFilter4,
				queryFilter5,
				queryFilter,
				queryFilter2
			});
			queryFilter6 = new AndFilter(new QueryFilter[]
			{
				queryFilter6,
				PartitionDataAggregator.CreateMonitoringMailboxFilter()
			});
			return PartitionDataAggregator.FindFirstMatchingRecipientObject<ADUser>(queryFilter6, true, true, true, ConsistencyMode.PartiallyConsistent, true, true);
		}

		public static void RunOperationOnAllAccountPartitions(bool readOnly, PartitionDataAggregator.OperationDelegate operation)
		{
			foreach (PartitionId partitionId in ADAccountPartitionLocator.GetAllAccountPartitionIds())
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromAllTenantsPartitionId(partitionId);
				IRecipientSession recipientSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(readOnly, ConsistencyMode.IgnoreInvalid, sessionSettings, 185, "RunOperationOnAllAccountPartitions", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\PartitionDataAggregator.cs");
				operation(recipientSession);
			}
		}

		public static IEnumerable<ADUser> GetUMEnabledUsersInDatabase(MailboxDatabase database)
		{
			QueryFilter filter = UMMailbox.GetUMEnabledUserQueryFilter(database);
			foreach (PartitionId partitionId in ADAccountPartitionLocator.GetAllAccountPartitionIds(true))
			{
				ADSessionSettings settings = ADSessionSettings.FromAllTenantsPartitionId(partitionId);
				ITenantRecipientSession session = DirectorySessionFactory.Default.CreateTenantRecipientSession(null, null, LcidMapper.DefaultLcid, true, ConsistencyMode.IgnoreInvalid, null, settings, 199, "GetUMEnabledUsersInDatabase", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\PartitionDataAggregator.cs");
				ADPagedReader<ADRecipient> recipientReader = session.FindPaged(null, QueryScope.SubTree, filter, null, 0);
				foreach (ADRecipient recipient in recipientReader)
				{
					ADUser user = recipient as ADUser;
					if (user != null)
					{
						yield return user;
					}
				}
			}
			yield break;
		}

		public static MiniRecipient GetMiniRecipientFromUserId(SecurityIdentifier sid)
		{
			return PartitionDataAggregator.GetMiniRecipientFromUserId(sid, null, ConsistencyMode.PartiallyConsistent);
		}

		public static MiniRecipient GetMiniRecipientFromUserId(SecurityIdentifier sid, IEnumerable<PropertyDefinition> properties, ConsistencyMode consistencyMode)
		{
			MiniRecipient miniRecipient = null;
			foreach (PartitionId partitionId in ADAccountPartitionLocator.GetAllAccountPartitionIds())
			{
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, consistencyMode, ADSessionSettings.FromAllTenantsPartitionId(partitionId), 249, "GetMiniRecipientFromUserId", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\PartitionDataAggregator.cs");
				try
				{
					miniRecipient = tenantOrRootOrgRecipientSession.FindMiniRecipientBySid<MiniRecipient>(sid, properties);
				}
				catch (NonUniqueRecipientException)
				{
				}
				if (miniRecipient != null)
				{
					break;
				}
			}
			return miniRecipient;
		}

		public static ADRawEntry FindUserBySid(SecurityIdentifier sId, IList<PropertyDefinition> properties, ref IRecipientSession recipientSession)
		{
			ADRawEntry adrawEntry = null;
			foreach (PartitionId partitionId in ADAccountPartitionLocator.GetAllAccountPartitionIds())
			{
				ITenantRecipientSession tenantRecipientSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromAllTenantsPartitionId(partitionId), 286, "FindUserBySid", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\PartitionDataAggregator.cs");
				adrawEntry = tenantRecipientSession.FindUserBySid(sId, properties);
				if (adrawEntry != null)
				{
					recipientSession = tenantRecipientSession;
					break;
				}
			}
			return adrawEntry;
		}

		public static IEnumerable<MsoTenantCookieContainer> FindTenantCookieContainers(QueryFilter filter)
		{
			return PartitionDataAggregator.FindPaged<MsoTenantCookieContainer>(filter, false, null);
		}

		public static IEnumerable<TenantRelocationRequest> FindPagedRelocationRequests()
		{
			IEnumerable<TenantRelocationRequest> requestsInFlight = PartitionDataAggregator.FindPaged<TenantRelocationRequest>(TenantRelocationRequest.InFlightRelocationRequestsFilter, true, new SortBy(TenantRelocationRequestSchema.RelocationSyncStartTime, SortOrder.Ascending));
			IEnumerable<TenantRelocationRequest> requestsJustStarted = PartitionDataAggregator.FindPaged<TenantRelocationRequest>(TenantRelocationRequest.JustStartedRelocationRequestsFilter, true, null);
			foreach (IEnumerable<TenantRelocationRequest> iterator in new IEnumerable<TenantRelocationRequest>[]
			{
				requestsInFlight,
				requestsJustStarted
			})
			{
				foreach (TenantRelocationRequest request in iterator)
				{
					yield return request;
				}
			}
			yield break;
		}

		public static IEnumerable<TenantRelocationRequest> FindPagedStaleLockedRelocationRequests(ExDateTime olderThan)
		{
			QueryFilter staleLockedRelocationRequestsFilter = TenantRelocationRequest.GetStaleLockedRelocationRequestsFilter(olderThan, true);
			return PartitionDataAggregator.FindPaged<TenantRelocationRequest>(staleLockedRelocationRequestsFilter, false, null);
		}

		public static IEnumerable<TenantRelocationRequest> FindPagedRelocationRequestsWithUnclassifiedPermanentError()
		{
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				TenantRelocationRequest.TenantRelocationRequestFilter,
				new ComparisonFilter(ComparisonOperator.Equal, TenantRelocationRequestSchema.RelocationLastError, 255)
			});
			return PartitionDataAggregator.FindPaged<TenantRelocationRequest>(filter, false, null);
		}

		public static ADUser[] FindAllArbitrationMailboxes(string legDN)
		{
			List<ADUser> list = new List<ADUser>();
			foreach (PartitionId partitionId in ADAccountPartitionLocator.GetAllAccountPartitionIds())
			{
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromAllTenantsPartitionId(partitionId), 367, "FindAllArbitrationMailboxes", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\PartitionDataAggregator.cs");
				ADUser[] collection = tenantOrRootOrgRecipientSession.FindPaged<ADUser>(RecipientFilterHelper.DiscoveryMailboxFilterForAuditLog(legDN), null, true, null, 0).ToArray<ADUser>();
				list.AddRange(collection);
			}
			return list.ToArray();
		}

		public static IEnumerable<ExchangeConfigurationUnit> FindForwardSyncMonitoringTenants()
		{
			QueryFilter filter = new TextFilter(ADObjectSchema.Name, "a830edad9050849EXPrv", MatchOptions.Prefix, MatchFlags.IgnoreCase);
			IEnumerable<Container> tenantContainers = PartitionDataAggregator.FindPaged<Container>(filter, false, null);
			foreach (Container tenantContainer in tenantContainers)
			{
				ExchangeConfigurationUnit cu = tenantContainer.Session.Read<ExchangeConfigurationUnit>(tenantContainer.Id.GetChildId("Configuration"));
				if (cu != null)
				{
					yield return cu;
				}
			}
			yield break;
		}

		private static QueryFilter CreateMonitoringMailboxFilter()
		{
			return new ComparisonFilter(ComparisonOperator.NotEqual, ADRecipientSchema.RecipientTypeDetails, RecipientTypeDetails.MonitoringMailbox);
		}

		private static QueryFilter CreateUsersLinkedToDatabaseFilter(ADObjectId databaseId)
		{
			QueryFilter queryFilter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADMailboxRecipientSchema.Database, databaseId),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.UserMailbox)
			});
			QueryFilter queryFilter2 = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.ArchiveDatabaseRaw, databaseId),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.UserMailbox)
			});
			QueryFilter queryFilter3 = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.ArchiveDatabaseRaw, databaseId),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.MailUser)
			});
			QueryFilter queryFilter4 = new OrFilter(new QueryFilter[]
			{
				queryFilter,
				queryFilter2,
				queryFilter3
			});
			return new AndFilter(new QueryFilter[]
			{
				queryFilter4,
				PartitionDataAggregator.CreateMonitoringMailboxFilter()
			});
		}

		private static T FindFirstMatchingRecipientObject<T>(QueryFilter filter, bool includeAccountForestRootOrg, bool includeResourceForest, bool useGC, ConsistencyMode consistencyMode, bool includeSoftDeletedObject, bool includeSecondaryPartitions) where T : ADObject, new()
		{
			return PartitionDataAggregator.FindFirstMatchingObject<T>(filter, includeAccountForestRootOrg, includeResourceForest, useGC, consistencyMode, true, includeSoftDeletedObject, includeSecondaryPartitions);
		}

		private static T FindFirstMatchingConfigurationObject<T>(QueryFilter filter, bool includeAccountForestRootOrg, bool includeResourceForest, bool useGC, ConsistencyMode consistencyMode, bool includeSecondaryPartitions) where T : ADObject, new()
		{
			return PartitionDataAggregator.FindFirstMatchingObject<T>(filter, includeAccountForestRootOrg, includeResourceForest, useGC, consistencyMode, false, false, includeSecondaryPartitions);
		}

		private static IEnumerable<T> FindPagedAllMatchingObjects<T>(QueryFilter filter, bool includeAccountForestRootOrg, bool includeResourceForest, bool useGC, ConsistencyMode consistencyMode, bool useRecipientSession, bool includeSoftDeletedObject, bool includeSecondaryPartitions) where T : ADObject, new()
		{
			IEnumerable<ADSessionSettings> sessionSettingsCollection = PartitionDataAggregator.CreateSessionSettingsCollection(includeAccountForestRootOrg, includeResourceForest, includeSecondaryPartitions);
			foreach (ADSessionSettings sessionSettings in sessionSettingsCollection)
			{
				if (includeSoftDeletedObject)
				{
					sessionSettings.IncludeSoftDeletedObjects = true;
				}
				IDirectorySession session = PartitionDataAggregator.GetDirectorySession(consistencyMode, useRecipientSession, sessionSettings);
				session.UseGlobalCatalog = useGC;
				ADPagedReader<T> reader = session.FindPaged<T>(null, QueryScope.SubTree, filter, null, 0, null);
				foreach (T result in reader)
				{
					yield return result;
				}
			}
			yield break;
		}

		private static T FindFirstMatchingObject<T>(QueryFilter filter, bool includeAccountForestRootOrg, bool includeResourceForest, bool useGC, ConsistencyMode consistencyMode, bool useRecipientSession, bool includeSoftDeletedObject, bool includeSecondaryPartitions) where T : ADObject, new()
		{
			IEnumerable<ADSessionSettings> enumerable = PartitionDataAggregator.CreateSessionSettingsCollection(includeAccountForestRootOrg, includeResourceForest, includeSecondaryPartitions);
			foreach (ADSessionSettings adsessionSettings in enumerable)
			{
				if (includeSoftDeletedObject)
				{
					adsessionSettings.IncludeSoftDeletedObjects = true;
				}
				IDirectorySession directorySession = PartitionDataAggregator.GetDirectorySession(consistencyMode, useRecipientSession, adsessionSettings);
				directorySession.UseGlobalCatalog = useGC;
				T[] array = directorySession.Find<T>(null, QueryScope.SubTree, filter, null, 1);
				if (array != null && array.Length != 0)
				{
					return array[0];
				}
			}
			return default(T);
		}

		private static IEnumerable<ADSessionSettings> CreateSessionSettingsCollection(bool includeAccountForestRootOrg, bool includeResourceForest, bool includeSecondaryPartitions = false)
		{
			List<ADSessionSettings> list = new List<ADSessionSettings>();
			bool flag = false;
			if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
			{
				PartitionId[] allAccountPartitionIds = ADAccountPartitionLocator.GetAllAccountPartitionIds(includeSecondaryPartitions);
				list.AddRange(allAccountPartitionIds.Select(new Func<PartitionId, ADSessionSettings>(ADSessionSettings.FromAllTenantsPartitionId)));
				if (includeAccountForestRootOrg)
				{
					list.AddRange(allAccountPartitionIds.Select(new Func<PartitionId, ADSessionSettings>(ADSessionSettings.FromAccountPartitionRootOrgScopeSet)));
					flag = allAccountPartitionIds.Contains(PartitionId.LocalForest);
					if (ExEnvironment.IsTest && !flag)
					{
						flag = allAccountPartitionIds.Any((PartitionId id) => id.ForestFQDN.Contains(PartitionId.LocalForest.ForestFQDN));
					}
				}
			}
			if (includeResourceForest && !flag)
			{
				list.Add(ADSessionSettings.FromRootOrgScopeSet());
			}
			return list;
		}

		private static IDirectorySession GetDirectorySession(ConsistencyMode consistencyMode, bool useRecipientSession, ADSessionSettings sessionSettings)
		{
			if (useRecipientSession)
			{
				return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, consistencyMode, sessionSettings, 612, "GetDirectorySession", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\PartitionDataAggregator.cs");
			}
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, consistencyMode, sessionSettings, 618, "GetDirectorySession", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\PartitionDataAggregator.cs");
		}

		private static IEnumerable<T> FindPaged<T>(QueryFilter filter, bool includeRetiredTenants = false, SortBy sortBy = null) where T : ADConfigurationObject, new()
		{
			PartitionId[] allAccountPartitionIds = ADAccountPartitionLocator.GetAllAccountPartitionIds();
			int i = 0;
			while (i < allAccountPartitionIds.Length)
			{
				PartitionId partitionId = allAccountPartitionIds[i];
				ADPagedReader<T> tenantReader;
				try
				{
					ADSessionSettings adsessionSettings = ADSessionSettings.FromAllTenantsPartitionId(partitionId);
					if (includeRetiredTenants)
					{
						adsessionSettings.TenantConsistencyMode = TenantConsistencyMode.IncludeRetiredTenants;
					}
					ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.PartiallyConsistent, adsessionSettings, 641, "FindPaged", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\PartitionDataAggregator.cs");
					tenantReader = tenantConfigurationSession.FindPaged<T>(null, QueryScope.SubTree, filter, sortBy, 0);
				}
				catch (OrgContainerNotFoundException)
				{
					if (Globals.IsDatacenter)
					{
						goto IL_10B;
					}
					throw;
				}
				goto IL_AF;
				IL_10B:
				i++;
				continue;
				IL_AF:
				foreach (T request in tenantReader)
				{
					yield return request;
				}
				goto IL_10B;
			}
			yield break;
		}

		public delegate void OperationDelegate(IRecipientSession recipientSession);
	}
}
