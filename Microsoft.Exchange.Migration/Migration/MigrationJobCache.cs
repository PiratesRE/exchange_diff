using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.Management.Migration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration.DataAccessLayer;
using Microsoft.Exchange.Migration.Logging;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Migration
{
	internal class MigrationJobCache : IMigrationJobCache, IDiagnosable
	{
		internal MigrationJobCache()
		{
			this.sharedDataLock = new object();
			this.cacheEntries = new Dictionary<string, MigrationCacheEntry>(12, StringComparer.OrdinalIgnoreCase);
			this.cacheUpdated = new AutoResetEvent(false);
		}

		internal AutoResetEvent CacheUpdated
		{
			get
			{
				return this.cacheUpdated;
			}
		}

		public string GetDiagnosticComponentName()
		{
			return "MigrationJobCache";
		}

		public XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement xml = null;
			MigrationDiagnosticArgument arguments = new MigrationDiagnosticArgument(parameters.Argument);
			MigrationApplication.RunOperationWithCulture(CultureInfo.InvariantCulture, delegate
			{
				xml = this.InternalGetDiagnosticInfo(arguments);
			});
			return xml;
		}

		public bool Add(string mailboxLegacyDn, Guid mdbGuid, OrganizationId organizationId, bool refresh)
		{
			MigrationUtil.ThrowOnNullArgument(mailboxLegacyDn, "mailboxLegacyDN");
			MigrationUtil.ThrowOnGuidEmptyArgument(mdbGuid, "mdbGuid");
			TenantPartitionHint tenantPartitionHint = TenantPartitionHint.FromOrganizationId(organizationId);
			MigrationCacheEntry migrationCacheEntry;
			if (!refresh && this.TryGet(mailboxLegacyDn, out migrationCacheEntry))
			{
				if (migrationCacheEntry.NextProcessTime == null)
				{
					return true;
				}
				MigrationLogger.Log(MigrationEventType.Information, "add: cacheentry {0} already exists, but has a next process time {1}- so re-adding to clear it out", new object[]
				{
					migrationCacheEntry,
					migrationCacheEntry.NextProcessTime
				});
			}
			using (IMigrationDataProvider migrationDataProvider = MigrationDataProvider.CreateProviderForMigrationMailbox(tenantPartitionHint, mailboxLegacyDn))
			{
				MigrationSession migrationSession;
				if (!MigrationApplication.TryGetEnabledMigrationSession(migrationDataProvider, true, out migrationSession))
				{
					MigrationLogger.Log(MigrationEventType.Warning, "add: cacheentry {0} with dbid {1} org {2} doesn't have a valid session associated with it.", new object[]
					{
						mailboxLegacyDn,
						mdbGuid,
						tenantPartitionHint
					});
					return false;
				}
				if (migrationSession.TotalJobCount <= 0)
				{
					MigrationLogger.Log(MigrationEventType.Warning, "add: cacheentry {0} with dbid {1} org {2} and session {3} doesn't have any migration job", new object[]
					{
						mailboxLegacyDn,
						mdbGuid,
						tenantPartitionHint,
						migrationSession
					});
					return false;
				}
				if (migrationDataProvider.MdbGuid != mdbGuid)
				{
					MigrationLogger.Log(MigrationEventType.Warning, "Did the arbitration mailbox for org {0} change databases?  old {1}, new {2}", new object[]
					{
						mdbGuid,
						migrationDataProvider.MdbGuid,
						tenantPartitionHint
					});
					mdbGuid = migrationDataProvider.MdbGuid;
				}
			}
			this.SyncWithStore();
			MigrationCacheEntry migrationCacheEntry2;
			if (this.TryGet(mailboxLegacyDn, out migrationCacheEntry2))
			{
				if (migrationCacheEntry2.MdbGuid == mdbGuid && object.Equals(migrationCacheEntry2.OrganizationId, organizationId.OrganizationalUnit) && migrationCacheEntry2.NextProcessTime == null)
				{
					MigrationLogger.Log(MigrationEventType.Verbose, "CacheEntry {0} already exists in cache, skipping add", new object[]
					{
						mailboxLegacyDn
					});
					return true;
				}
				MigrationLogger.Log(MigrationEventType.Verbose, "Overwriting cacheentry {0} with dbid {1} org {2} with one dbid {3} orgid {4}", new object[]
				{
					migrationCacheEntry2.MigrationMailboxLegacyDN,
					migrationCacheEntry2.MdbGuid,
					migrationCacheEntry2.TenantPartitionHint,
					mdbGuid,
					tenantPartitionHint
				});
				MigrationJobCache.RemoveFromStore(migrationCacheEntry2);
			}
			using (IMigrationDataProvider migrationDataProvider2 = MigrationDataProvider.CreateProviderForSystemMailbox(mdbGuid))
			{
				migrationCacheEntry = MigrationCacheEntry.Create(migrationDataProvider2, mailboxLegacyDn, mdbGuid, tenantPartitionHint);
			}
			lock (this.sharedDataLock)
			{
				this.cacheEntries[migrationCacheEntry.MigrationMailboxLegacyDN] = migrationCacheEntry;
			}
			this.cacheUpdated.Set();
			return true;
		}

		public void Remove(MigrationCacheEntry cacheEntry)
		{
			MigrationUtil.ThrowOnNullArgument(cacheEntry, "cacheEntry");
			MigrationJobCache.RemoveFromStore(cacheEntry);
			string migrationMailboxLegacyDN = cacheEntry.MigrationMailboxLegacyDN;
			MigrationCacheEntry migrationCacheEntry;
			if (!this.TryGet(migrationMailboxLegacyDN, out migrationCacheEntry))
			{
				MigrationLogger.Log(MigrationEventType.Warning, "CacheEntry {0} not found in cache, will re-sync cache", new object[]
				{
					migrationMailboxLegacyDN
				});
				if (!this.SyncWithStore() || !this.TryGet(migrationMailboxLegacyDN, out migrationCacheEntry))
				{
					MigrationLogger.Log(MigrationEventType.Warning, "CacheEntry {0} not found in cache, after re-syncing cache.  treating as entry already removed", new object[]
					{
						migrationMailboxLegacyDN
					});
					return;
				}
			}
			if (migrationCacheEntry.StoreObjectId != cacheEntry.StoreObjectId)
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "CacheEntry {0} with id {1} has been overwritten with id {2}", new object[]
				{
					migrationMailboxLegacyDN,
					cacheEntry.StoreObjectId,
					migrationCacheEntry.StoreObjectId
				});
				return;
			}
			lock (this.sharedDataLock)
			{
				this.cacheEntries.Remove(migrationMailboxLegacyDN);
			}
		}

		public bool SyncWithStore()
		{
			Dictionary<string, MigrationCacheEntry> dictionary = new Dictionary<string, MigrationCacheEntry>(this.cacheEntries.Count);
			foreach (MigrationCacheEntry migrationCacheEntry in this.GetMigrationCacheEntriesForServer())
			{
				MigrationCacheEntry migrationCacheEntry2;
				if (!dictionary.TryGetValue(migrationCacheEntry.MigrationMailboxLegacyDN, out migrationCacheEntry2))
				{
					dictionary.Add(migrationCacheEntry.MigrationMailboxLegacyDN, migrationCacheEntry);
				}
				else
				{
					MigrationCacheEntry migrationCacheEntry3 = migrationCacheEntry;
					if (migrationCacheEntry2.LastUpdated < migrationCacheEntry.LastUpdated)
					{
						dictionary[migrationCacheEntry.MigrationMailboxLegacyDN] = migrationCacheEntry;
						migrationCacheEntry3 = migrationCacheEntry2;
						migrationCacheEntry2 = migrationCacheEntry;
					}
					MigrationLogger.Log(MigrationEventType.Warning, "Found multiple occurrences of {0} one from {1} with dbid {2} the older, other from {3} with dbid {4}", new object[]
					{
						migrationCacheEntry2.MigrationMailboxLegacyDN,
						migrationCacheEntry2.LastUpdated,
						migrationCacheEntry2.MdbGuid,
						migrationCacheEntry3.LastUpdated,
						migrationCacheEntry3.MdbGuid
					});
					MigrationJobCache.RemoveFromStore(migrationCacheEntry3);
				}
			}
			bool result = false;
			MigrationLogger.Log(MigrationEventType.Verbose, "Found {0} new cacheEntries had {1} old cacheEntries", new object[]
			{
				dictionary.Count,
				this.cacheEntries.Count
			});
			lock (this.sharedDataLock)
			{
				foreach (KeyValuePair<string, MigrationCacheEntry> keyValuePair in dictionary)
				{
					if (!this.cacheEntries.ContainsKey(keyValuePair.Key))
					{
						result = true;
						break;
					}
				}
				this.cacheEntries = dictionary;
			}
			return result;
		}

		public List<MigrationCacheEntry> Get()
		{
			List<MigrationCacheEntry> list;
			lock (this.sharedDataLock)
			{
				list = new List<MigrationCacheEntry>(this.cacheEntries.Count);
				foreach (KeyValuePair<string, MigrationCacheEntry> keyValuePair in this.cacheEntries)
				{
					list.Add(keyValuePair.Value);
				}
			}
			return list;
		}

		private static XElement GetJobItemDiagnosticInfo(IMigrationDataProvider dataProvider, MigrationJob job, MigrationDiagnosticArgument argument, string jobItemIdentifier, string elementName)
		{
			XElement xelement = new XElement(elementName);
			IEnumerable<MigrationJobItem> enumerable = null;
			int value;
			if (!argument.TryGetArgument<int>("maxsize", out value))
			{
				value = 500;
			}
			if (!string.IsNullOrEmpty(jobItemIdentifier))
			{
				enumerable = MigrationJobItem.GetByIdentifier(dataProvider, null, jobItemIdentifier, null);
			}
			else if (argument.HasArgument("slotid"))
			{
				Guid argument2 = argument.GetArgument<Guid>("slotid");
				enumerable = MigrationJobItem.GetBySlotId(dataProvider, argument2, new int?(value));
			}
			else if (argument.HasArgument("verbose"))
			{
				enumerable = MigrationJobItem.GetAll(dataProvider, job, new int?(value));
			}
			else if (argument.HasArgument("status"))
			{
				string value2 = null;
				IEnumerable<MigrationUserStatus> enumerable2;
				if (argument.TryGetArgument<string>("status", out value2))
				{
					MigrationUserStatus migrationUserStatus = (MigrationUserStatus)Enum.Parse(typeof(MigrationUserStatus), value2, false);
					enumerable2 = new MigrationUserStatus[]
					{
						migrationUserStatus
					};
				}
				else
				{
					enumerable2 = Enum.GetValues(typeof(MigrationUserStatus)).Cast<MigrationUserStatus>();
				}
				XElement xelement2 = new XElement("counts");
				foreach (MigrationUserStatus migrationUserStatus2 in enumerable2)
				{
					int count = MigrationJobItem.GetCount(dataProvider, (job != null) ? job.JobId : Guid.Empty, new MigrationUserStatus[]
					{
						migrationUserStatus2
					});
					xelement2.Add(new XElement(migrationUserStatus2.ToString(), count));
				}
				xelement.Add(xelement2);
			}
			else
			{
				xelement.Add(new XElement("totalCount", MigrationJobItem.GetCount(dataProvider, (job != null) ? job.JobId : Guid.Empty, new MigrationUserStatus[0])));
			}
			if (enumerable != null)
			{
				int num = 0;
				IEnumerator<MigrationJobItem> enumerator = enumerable.GetEnumerator();
				while (enumerator.MoveNext())
				{
					XElement content = argument.RunDiagnosticOperation(() => enumerator.Current.GetDiagnosticInfo(dataProvider, argument));
					num++;
					xelement.Add(content);
				}
				xelement.AddFirst(new XElement("count", num));
			}
			return xelement;
		}

		private static XElement GetJobDiagnosticInfo(TenantPartitionHint tenantPartitionHint, string mailboxLegacyDn, MigrationDiagnosticArgument argument, string jobItemIdentifier)
		{
			return argument.RunDiagnosticOperation(delegate
			{
				XElement diagnosticInfo;
				using (IMigrationDataProvider migrationDataProvider = MigrationDataProvider.CreateProviderForMigrationMailbox(tenantPartitionHint, mailboxLegacyDn))
				{
					MigrationSession migrationSession = MigrationSession.Get(migrationDataProvider);
					diagnosticInfo = migrationSession.GetDiagnosticInfo(migrationDataProvider, argument);
					IEnumerable<MigrationEndpointBase> enumerable = null;
					string jobName;
					IEnumerable<MigrationJob> enumerable2;
					if (argument.TryGetArgument<string>("batch", out jobName))
					{
						enumerable2 = MigrationJob.GetByName(migrationDataProvider, migrationSession.Config, jobName);
						List<MigrationEndpointBase> list = new List<MigrationEndpointBase>();
						foreach (MigrationJob migrationJob in enumerable2)
						{
							if (migrationJob.SourceEndpoint != null)
							{
								list.Add(migrationJob.SourceEndpoint);
							}
							if (migrationJob.TargetEndpoint != null)
							{
								list.Add(migrationJob.TargetEndpoint);
							}
						}
						enumerable = list;
					}
					else
					{
						enumerable2 = MigrationJob.Get(migrationDataProvider, migrationSession.Config);
					}
					int num = 0;
					foreach (MigrationJob migrationJob2 in enumerable2)
					{
						num++;
						XElement diagnosticInfo2 = migrationJob2.GetDiagnosticInfo(migrationDataProvider, argument);
						diagnosticInfo2.Add(MigrationJobCache.GetJobItemDiagnosticInfo(migrationDataProvider, migrationJob2, argument, jobItemIdentifier, "MigrationJobItemsCollection"));
						diagnosticInfo.Add(diagnosticInfo2);
					}
					if (num <= 0)
					{
						diagnosticInfo.Add(new XElement("MigrationJob"));
					}
					if (argument.HasArgument("verbose"))
					{
						using (IMigrationDataProvider providerForFolder = migrationDataProvider.GetProviderForFolder(MigrationFolderName.CorruptedItems))
						{
							diagnosticInfo.Add(MigrationJobCache.GetJobItemDiagnosticInfo(providerForFolder, null, argument, jobItemIdentifier, "CorruptedItemsCollection"));
						}
					}
					XElement xelement = new XElement("endpoints");
					if (enumerable == null && argument.HasArgument("endpoints"))
					{
						enumerable = MigrationEndpointBase.Get(MigrationEndpointId.Any, migrationDataProvider, true);
					}
					bool flag = false;
					if (enumerable != null)
					{
						foreach (MigrationEndpointBase migrationEndpointBase in enumerable)
						{
							xelement.Add(migrationEndpointBase.GetDiagnosticInfo(migrationDataProvider, argument));
							flag = true;
						}
					}
					if (!flag)
					{
						xelement.Add(new XText("No endpoints."));
					}
					diagnosticInfo.Add(xelement);
					string reportId;
					if (argument.TryGetArgument<string>("reportid", out reportId))
					{
						using (IMigrationDataProvider providerForFolder2 = migrationDataProvider.GetProviderForFolder(MigrationFolderName.SyncMigrationReports))
						{
							MigrationReportItem migrationReportItem = MigrationReportItem.Get(providerForFolder2, new MigrationReportId(reportId));
							diagnosticInfo.Add(migrationReportItem.GetDiagnosticInfo(providerForFolder2, argument));
							goto IL_2E6;
						}
					}
					if (argument.HasArgument("reports"))
					{
						using (IMigrationDataProvider providerForFolder3 = migrationDataProvider.GetProviderForFolder(MigrationFolderName.SyncMigrationReports))
						{
							foreach (MigrationReportItem migrationReportItem2 in MigrationReportItem.GetAll(providerForFolder3))
							{
								diagnosticInfo.Add(migrationReportItem2.GetDiagnosticInfo(providerForFolder3, argument));
							}
						}
					}
					IL_2E6:;
				}
				return diagnosticInfo;
			});
		}

		private static void RemoveFromStore(MigrationCacheEntry cacheEntry)
		{
			MigrationUtil.ThrowOnNullArgument(cacheEntry, "cacheEntry");
			try
			{
				using (IMigrationDataProvider migrationDataProvider = MigrationDataProvider.CreateProviderForSystemMailbox(cacheEntry.MdbGuid))
				{
					cacheEntry.Delete(migrationDataProvider);
				}
			}
			catch (MigrationMailboxNotFoundOnServerException exception)
			{
				MigrationLogger.Log(MigrationEventType.Warning, exception, "migration cacheEntry doesn't exist here", new object[0]);
			}
			catch (MigrationPermanentException exception2)
			{
				MigrationLogger.Log(MigrationEventType.Error, exception2, "trouble removing migration cache entry", new object[0]);
			}
			catch (StoragePermanentException exception3)
			{
				MigrationLogger.Log(MigrationEventType.Error, exception3, "trouble removing migration cache entry", new object[0]);
			}
		}

		private XElement InternalGetDiagnosticInfo(MigrationDiagnosticArgument argument)
		{
			XElement xelement = new XElement(this.GetDiagnosticComponentName());
			string text = null;
			string argument2 = argument.GetArgument<string>("user");
			string argument3 = argument.GetArgument<string>("partition");
			if (MigrationServiceFactory.Instance.IsMultiTenantEnabled())
			{
				text = argument.GetArgument<string>("organization");
			}
			if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(argument2) && string.IsNullOrEmpty(argument3))
			{
				ICollection<MigrationCacheEntry> collection = this.Get();
				XElement xelement2 = new XElement("MigrationCacheEntryCollection", new XElement("count", collection.Count));
				xelement.Add(xelement2);
				int num = 0;
				foreach (MigrationCacheEntry migrationCacheEntry in collection)
				{
					XElement diagnosticInfo = migrationCacheEntry.GetDiagnosticInfo(null, argument);
					xelement2.Add(diagnosticInfo);
					XElement jobDiagnosticInfo = MigrationJobCache.GetJobDiagnosticInfo(migrationCacheEntry.TenantPartitionHint, migrationCacheEntry.MigrationMailboxLegacyDN, argument, null);
					if (jobDiagnosticInfo != null)
					{
						diagnosticInfo.Add(jobDiagnosticInfo);
					}
					else
					{
						num++;
					}
				}
				xelement2.Add(new XElement("jobsNotFoundCount", num));
			}
			else
			{
				OrganizationId organizationId = null;
				if (!string.IsNullOrEmpty(text))
				{
					SmtpDomain smtpDomain = null;
					try
					{
						smtpDomain = new SmtpDomain(text);
					}
					catch (FormatException)
					{
						xelement.Add(new XElement("error", "Specified domain is invalid: " + text));
					}
					if (smtpDomain != null)
					{
						organizationId = DomainToOrganizationIdCache.Singleton.Get(smtpDomain);
						if (organizationId == null)
						{
							xelement.Add(new XElement("error", "Organization not found: " + text));
						}
					}
				}
				else if (!string.IsNullOrEmpty(argument2))
				{
					ExchangePrincipal exchangePrincipal = null;
					try
					{
						ADSessionSettings adSettings = ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(SmtpAddress.Parse(argument2).Domain);
						exchangePrincipal = ExchangePrincipal.FromProxyAddress(adSettings, argument2, RemotingOptions.AllowCrossSite);
					}
					catch (ObjectNotFoundException)
					{
					}
					if (exchangePrincipal != null)
					{
						organizationId = exchangePrincipal.MailboxInfo.OrganizationId;
					}
					else
					{
						xelement.Add(new XElement("error", "User not found: " + argument2));
					}
				}
				if (organizationId == null)
				{
					organizationId = OrganizationId.ForestWideOrgId;
				}
				IEnumerable<string> enumerable = null;
				TenantPartitionHint tenantPartitionHint = TenantPartitionHint.FromOrganizationId(organizationId);
				string localServerFqdn = MigrationServiceFactory.Instance.GetLocalServerFqdn();
				if (!string.IsNullOrEmpty(argument3))
				{
					Guid guid;
					QueryFilter optionalFilter;
					if (Guid.TryParse(argument3, out guid))
					{
						optionalFilter = new ComparisonFilter(ComparisonOperator.Equal, ADMailboxRecipientSchema.ExchangeGuid, guid);
					}
					else
					{
						optionalFilter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, argument3);
					}
					enumerable = MigrationHelperBase.GetMigrationMailboxLegacyDN(tenantPartitionHint, localServerFqdn, optionalFilter).ToArray<string>();
				}
				else
				{
					try
					{
						enumerable = MigrationHelperBase.GetMigrationMailboxLegacyDN(tenantPartitionHint, localServerFqdn, null).ToArray<string>();
					}
					catch (MigrationMailboxNotFoundException)
					{
						string text2;
						string a;
						MigrationHelperBase.GetManagementMailboxData(tenantPartitionHint, out text2, out a);
						if (string.Equals(a, localServerFqdn, StringComparison.OrdinalIgnoreCase))
						{
							enumerable = new string[]
							{
								text2
							};
						}
					}
				}
				bool flag = false;
				if (enumerable != null)
				{
					foreach (string mailboxLegacyDn in enumerable)
					{
						flag = true;
						MigrationCacheEntry migrationCacheEntry2 = null;
						XElement xelement3;
						if (this.TryGet(mailboxLegacyDn, out migrationCacheEntry2))
						{
							xelement3 = migrationCacheEntry2.GetDiagnosticInfo(null, argument);
						}
						else
						{
							xelement3 = new XElement("MigrationCacheEntry");
							string text3 = "no cache entry found";
							if (organizationId != OrganizationId.ForestWideOrgId)
							{
								text3 = text3 + " for: " + organizationId;
							}
							xelement3.Add(new XElement("warning", text3));
						}
						XElement jobDiagnosticInfo2 = MigrationJobCache.GetJobDiagnosticInfo(tenantPartitionHint, mailboxLegacyDn, argument, argument2);
						if (jobDiagnosticInfo2 != null)
						{
							xelement3.Add(jobDiagnosticInfo2);
						}
						xelement.Add(xelement3);
					}
				}
				if (!flag)
				{
					xelement.Add(new XElement("error", string.Format("no migration mailboxes found on '{0}'", MigrationServiceFactory.Instance.GetLocalServerFqdn())));
				}
			}
			if (argument.ArgumentCount == 0)
			{
				string content = string.Empty;
				if (MigrationServiceFactory.Instance.IsMultiTenantEnabled())
				{
					content = string.Format(CultureInfo.InvariantCulture, "Supported arguments: \"{0},{1},{2}=admin@organization.com,{3}=organization.com,{4}=mailbox\", where {0} - prints job items, {1} prints message properties, {2} - prints data for specified user, {3} - prints data for specified organization (ignored if {2} is specified), {4} - prints data for specified partition", new object[]
					{
						"verbose",
						"storage",
						"user",
						"organization",
						"partition"
					});
				}
				else
				{
					content = string.Format(CultureInfo.InvariantCulture, "Supported arguments: \"{0},{1},{2}=admin@organization.com,{3}=mailbox\", where {0} - prints job items, {1} prints message properties, {2} - prints data for specified user, {3} - prints data for specified partition", new object[]
					{
						"verbose",
						"storage",
						"user",
						"partition"
					});
				}
				xelement.Add(new XElement("help", content));
			}
			return xelement;
		}

		private bool TryGet(string mailboxLegacyDn, out MigrationCacheEntry cacheEntry)
		{
			bool result;
			lock (this.sharedDataLock)
			{
				result = this.cacheEntries.TryGetValue(mailboxLegacyDn, out cacheEntry);
			}
			return result;
		}

		private IEnumerable<MigrationCacheEntry> GetMigrationCacheEntriesForServer()
		{
			List<Guid> databaseGuids = new List<Guid>();
			try
			{
				using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create("Client=MSExchangeSimpleMigration", MigrationServiceFactory.Instance.GetLocalServerFqdn(), null, null, null))
				{
					MdbStatus[] array = exRpcAdmin.ListMdbStatus(true);
					if (array != null)
					{
						foreach (MdbStatus mdbStatus in array)
						{
							if ((mdbStatus.Status & MdbStatusFlags.Online) != MdbStatusFlags.Offline && (mdbStatus.Status & MdbStatusFlags.InRecoverySG) == MdbStatusFlags.Offline)
							{
								databaseGuids.Add(mdbStatus.MdbGuid);
							}
						}
					}
				}
			}
			catch (MapiRetryableException ex)
			{
				MigrationLogger.Log(MigrationEventType.Warning, "error when looking for local databases {0}", new object[]
				{
					ex
				});
				throw new MigrationTransientException(Strings.MigrationLocalDatabasesNotFound, ex);
			}
			catch (MapiPermanentException ex2)
			{
				MigrationLogger.Log(MigrationEventType.Error, "error when looking for local databases {0}", new object[]
				{
					ex2
				});
				throw new MigrationPermanentException(Strings.MigrationLocalDatabasesNotFound, ex2);
			}
			if (databaseGuids.Count == 0)
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "GetMigrationCacheEntriesForServer: No mounted databases on server: {0}", new object[]
				{
					MigrationServiceFactory.Instance.GetLocalServerFqdn()
				});
			}
			else
			{
				foreach (Guid databaseGuid in databaseGuids)
				{
					string debugInfo = null;
					List<MigrationCacheEntry> databaseEntries = new List<MigrationCacheEntry>();
					try
					{
						using (IMigrationDataProvider migrationDataProvider = MigrationDataProvider.CreateProviderForSystemMailbox(databaseGuid))
						{
							debugInfo = migrationDataProvider.MailboxName;
							foreach (MigrationCacheEntry item in MigrationCacheEntry.GetMigrationCacheEntries(migrationDataProvider, databaseGuid))
							{
								databaseEntries.Add(item);
							}
						}
					}
					catch (MigrationMailboxNotFoundOnServerException ex3)
					{
						MigrationLogger.Log(MigrationEventType.Verbose, "GetMigrationCacheEntriesForServer: Error accessing system mailbox: {0} {1}", new object[]
						{
							debugInfo,
							databaseGuid,
							ex3
						});
						databaseEntries = null;
					}
					catch (MigrationTransientException ex4)
					{
						MigrationApplication.NotifyOfTransientException(ex4, string.Format("GetMigrationCacheEntriesForServer: Error accessing system mailbox: {0} {1}", debugInfo, databaseGuid));
						databaseEntries = null;
					}
					catch (StorageTransientException ex5)
					{
						MigrationApplication.NotifyOfTransientException(ex5, string.Format("GetMigrationCacheEntriesForServer: Error accessing system mailbox: {0} {1}", debugInfo, databaseGuid));
						databaseEntries = null;
					}
					catch (MigrationPermanentException ex6)
					{
						MigrationApplication.NotifyOfPermanentException(ex6, string.Format("GetMigrationCacheEntriesForServer: Error accessing system mailbox: {0} {1}", debugInfo, databaseGuid));
						databaseEntries = null;
					}
					catch (StoragePermanentException ex7)
					{
						MigrationApplication.NotifyOfPermanentException(ex7, string.Format("GetMigrationCacheEntriesForServer: Error accessing system mailbox: {0} {1}", debugInfo, databaseGuid));
						databaseEntries = null;
					}
					if (databaseEntries != null)
					{
						foreach (MigrationCacheEntry cacheEntry in databaseEntries)
						{
							yield return cacheEntry;
						}
					}
				}
			}
			yield break;
		}

		public const int ExpectedNumOrgsPerDatabase = 2;

		public const int ExpectedNumDBsPerServer = 6;

		public const int MaxNumberDiagnosticJobItems = 500;

		private readonly AutoResetEvent cacheUpdated;

		private readonly object sharedDataLock;

		private Dictionary<string, MigrationCacheEntry> cacheEntries;
	}
}
