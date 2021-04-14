using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Ceres.CoreServices.Admin;
using Microsoft.Ceres.CoreServices.Tools.Management.Client;
using Microsoft.Ceres.SearchCore.Admin;
using Microsoft.Ceres.SearchCore.Admin.Clustering;
using Microsoft.Ceres.SearchCore.Admin.Config;
using Microsoft.Ceres.SearchCore.Admin.Model;
using Microsoft.Ceres.SearchCore.Services.GenerationController;
using Microsoft.Ceres.SearchCore.Services.Indexes.FastServerIndex;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Fast
{
	internal class IndexManager : FastManagementClient, IIndexManager
	{
		protected IndexManager()
		{
			base.DiagnosticsSession.ComponentName = "IndexManager";
			base.DiagnosticsSession.Tracer = ExTraceGlobals.IndexManagementTracer;
			this.InitializeIndexModels();
			base.ConnectManagementAgents();
		}

		public static IndexManager Instance
		{
			get
			{
				if (Interlocked.CompareExchange<Hookable<IndexManager>>(ref IndexManager.hookableInstance, null, null) == null)
				{
					lock (IndexManager.lockObject)
					{
						if (IndexManager.hookableInstance == null)
						{
							Hookable<IndexManager> hookable = Hookable<IndexManager>.Create(true, new IndexManager());
							Thread.MemoryBarrier();
							IndexManager.hookableInstance = hookable;
						}
					}
				}
				return IndexManager.hookableInstance.Value;
			}
		}

		protected virtual IAdminServiceManagementAgent AdminService
		{
			get
			{
				return this.adminService;
			}
		}

		protected virtual IIndexClusterManagementAgent ClusterService
		{
			get
			{
				return this.clusterService;
			}
		}

		protected virtual IIndexManagement IndexService
		{
			get
			{
				return this.indexService;
			}
		}

		protected override int ManagementPortOffset
		{
			get
			{
				return 3;
			}
		}

		protected NodeManagementClient NodeClient
		{
			get
			{
				return NodeManagementClient.Instance;
			}
		}

		public void TriggerMasterMerge(string indexName)
		{
			base.PerformFastOperation(delegate()
			{
				using (SystemClient systemClient = this.ConnectSystem())
				{
					IGenerationControllerAgent managementAgentClient = systemClient.GetManagementAgentClient<IGenerationControllerAgent>("AdminNode1", "AdminNode1.GenerationController");
					IFastServerAgent managementAgentClient2 = systemClient.GetManagementAgentClient<IFastServerAgent>("IndexNode1", indexName + "Single.FastServer.FSIndex");
					managementAgentClient.TriggerCheckpoint(indexName);
					managementAgentClient2.TriggerMasterMerge();
				}
			}, "Trigger MasterMerge using the IFastServerAgent");
		}

		public void UpdateConfiguration()
		{
			base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "UpdateConfiguration called", new object[0]);
			base.PerformFastOperation(delegate()
			{
				if (!this.AdminService.IsPendingReconfiguration)
				{
					this.AdminService.UpdateConfiguration();
				}
			}, "UpdateConfiguration");
			base.PerformFastOperation(delegate()
			{
				if (this.CheckForNoPendingConfigurationUpdate())
				{
					this.NodeClient.CheckForAllNodesHealthy(true);
					base.ConnectManagementAgents();
					return;
				}
				throw new UpdateConfigurationFailedException();
			}, "Post UpdateConfiguration Checks");
		}

		public void CreateCatalog(string indexName, string databasePath, bool databaseCopyActive, RefinerUsage refinersToEnable)
		{
			base.DiagnosticsSession.TraceDebug<string, string>("Add-IndexSystem - IndexName:{0}, Mode:{1}", indexName, this.IndexSystemMode(databaseCopyActive, false));
			List<string> nodeNames = null;
			if (!string.IsNullOrEmpty(databasePath) && !Directory.Exists(databasePath))
			{
				throw new DatabasePathDoesNotExist(databasePath);
			}
			base.PerformFastOperation(delegate()
			{
				nodeNames = this.NodeClient.GetNodeNamesBasedOnRole("Index");
			}, "CreateCatalog");
			if (nodeNames != null && nodeNames.Count > 0)
			{
				nodeNames.Sort();
				this.CreateIndexSystem(indexName, nodeNames[0], databasePath, databaseCopyActive, refinersToEnable);
				return;
			}
			throw new NoFASTNodesFoundException();
		}

		public void RemoveCatalog(string indexName)
		{
			base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "Remove-IndexSystem - IndexName:{0}", new object[]
			{
				indexName
			});
			base.PerformFastOperation(delegate()
			{
				this.WaitForOperationFinished(this.IndexService.DeleteIndexSystem(indexName));
			}, "DeleteIndexSystem");
		}

		public void ResetCatalog(string indexName)
		{
			base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "Reset-IndexSystem - IndexName:{0}", new object[]
			{
				indexName
			});
			base.PerformFastOperation(delegate()
			{
				this.WaitForOperationFinished(this.IndexService.ResetIndexSystem(indexName));
			}, "ResetIndexSystem");
		}

		public bool EnsureCatalog(string indexName, bool databaseCopyActive, bool suspended, RefinerUsage refinersToEnable)
		{
			base.DiagnosticsSession.TraceDebug<string, string>("Attempt to Update-IndexSystem - IndexName:{0}, Mode:{1}", indexName, this.IndexSystemMode(databaseCopyActive, suspended));
			if (this.UpdateIndexSystem(indexName, delegate(IndexSystemModel model)
			{
				IndexSystemOptions options = new IndexSystemOptions(model.Options)
				{
					JournalBufferSize = FastManagementClient.Config.JournalBufferSize,
					ImportIndex = false,
					TriggerReseedOnDocsumLookupFailure = FastManagementClient.Config.TriggerReseedOnDocsumLookupFailure,
					InvalidateOnSeedingReadFailure = FastManagementClient.Config.InvalidateOnSeedingReadFailure
				};
				model.Options = options;
				model.Schema = FastIndexSystemSchema.GetIndexSystemSchema(refinersToEnable);
				this.SetIndexSystemIndex(model, databaseCopyActive);
				if (databaseCopyActive)
				{
					model.Suspended = false;
					return;
				}
				model.Suspended = suspended;
			}))
			{
				base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "Update-IndexSystem - IndexName:{0}, Mode:{1}", new object[]
				{
					indexName,
					this.IndexSystemMode(databaseCopyActive, suspended)
				});
				return true;
			}
			return false;
		}

		public string GetTransportIndexSystem()
		{
			foreach (string text in this.GetCatalogs())
			{
				if (FastIndexVersion.GetIndexSystemDatabaseGuid(text) != Guid.Empty)
				{
					return text;
				}
			}
			return null;
		}

		public bool SuspendCatalog(string indexName)
		{
			base.DiagnosticsSession.TraceDebug<string>("Attempt to Suspend-IndexSystem - IndexName:{0}", indexName);
			if (this.UpdateIndexSystem(indexName, delegate(IndexSystemModel model)
			{
				model.Suspended = true;
			}))
			{
				base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "Suspend-IndexSystem - IndexName:{0}", new object[]
				{
					indexName
				});
				return true;
			}
			return false;
		}

		public bool ResumeCatalog(string indexName)
		{
			base.DiagnosticsSession.TraceDebug<string>("Attempt to Resume-IndexSystem - IndexName:{0}", indexName);
			if (this.UpdateIndexSystem(indexName, delegate(IndexSystemModel model)
			{
				model.Suspended = false;
			}))
			{
				base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "Resume-IndexSystem - IndexName:{0}", new object[]
				{
					indexName
				});
				return true;
			}
			return false;
		}

		public void FlushCatalog(string indexName)
		{
			base.DiagnosticsSession.TraceDebug<string>("Attempt to Flush-IndexSystem:{0}", indexName);
			base.PerformFastOperation(delegate()
			{
				this.WaitForOperationFinished(this.IndexService.ForceCheckpoint(indexName));
			}, "Flush-IndexSystem");
		}

		public bool CatalogExists(string indexName)
		{
			base.DiagnosticsSession.TraceDebug<string>("IndexSystem-Exists? - IndexName:{0}", indexName);
			return this.PerformFastOperation<bool>(() => this.IndexService.IndexSystemNames.Contains(indexName), "CatalogExists");
		}

		public CatalogState GetCatalogState(string indexName, out string seedingSource, out int? failureCode, out string failureReason)
		{
			seedingSource = null;
			failureCode = null;
			failureReason = null;
			IndexClusterMemberInfo clusterMemberInfo = this.GetClusterMemberInfo(indexName);
			CatalogState indexingState = this.GetIndexingState(indexName, clusterMemberInfo, null);
			if (indexingState == CatalogState.Seeding)
			{
				seedingSource = clusterMemberInfo.IndexingState.SeedingState.SeedingSource;
			}
			if (clusterMemberInfo != null && clusterMemberInfo.IndexingState != null && clusterMemberInfo.IndexingState.Reason != null)
			{
				failureCode = new int?(clusterMemberInfo.IndexingState.Reason.Code);
				failureReason = clusterMemberInfo.IndexingState.Reason.Reason;
			}
			return indexingState;
		}

		public HashSet<string> GetCatalogs()
		{
			base.DiagnosticsSession.TraceDebug("Get-IndexSystems", new object[0]);
			return this.PerformFastOperation<HashSet<string>>(() => new HashSet<string>(this.IndexService.IndexSystemNames), "GetCatalogs");
		}

		public XElement GetIndexSystemsDiagnostics()
		{
			XElement xelement = new XElement("IndexSystems");
			foreach (string text in this.GetCatalogs())
			{
				IndexSystemModel indexSystemModel = this.IndexService.GetIndexSystemModel(text);
				IndexClusterMemberInfo clusterMemberInfo = this.GetClusterMemberInfo(text);
				CatalogState indexingState = this.GetIndexingState(text, clusterMemberInfo, indexSystemModel);
				string content = null;
				string content2 = null;
				string content3 = null;
				string content4 = null;
				int? num = null;
				string content5 = null;
				if (indexSystemModel != null)
				{
					content = this.GetRootDirectory(text);
					if (clusterMemberInfo != null)
					{
						content2 = string.Format("{0}.{1}.{2}", indexSystemModel.Name, indexSystemModel.SequenceNumber, clusterMemberInfo.Name);
						if (clusterMemberInfo.IndexingState != null && clusterMemberInfo.IndexingState.Reason != null)
						{
							num = new int?(clusterMemberInfo.IndexingState.Reason.Code);
							content5 = clusterMemberInfo.IndexingState.Reason.Reason;
						}
					}
				}
				if (indexingState == CatalogState.Seeding)
				{
					content3 = clusterMemberInfo.IndexingState.SeedingState.SeedingSource;
					content4 = clusterMemberInfo.IndexingState.SeedingState.SeedingTarget;
				}
				XElement xelement2 = new XElement("IndexSystem");
				xelement2.Add(new XElement("Name", text));
				xelement2.Add(new XElement("IndexingState", indexingState));
				xelement2.Add(new XElement("RootDirectory", content));
				xelement2.Add(new XElement("Directory", content2));
				xelement2.Add(new XElement("SeedingSource", content3));
				xelement2.Add(new XElement("SeedingTarget", content4));
				xelement2.Add(new XElement("FailureCode", num));
				xelement2.Add(new XElement("FailureReason", content5));
				xelement.Add(xelement2);
			}
			return xelement;
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<IndexManager>(this);
		}

		public string GetRootDirectory(string indexSystemName)
		{
			IndexSystemModel indexSystemModel = this.IndexService.GetIndexSystemModel(indexSystemName);
			if (indexSystemModel == null)
			{
				return null;
			}
			string path;
			if (indexSystemModel.Options.RootDirectory.StartsWith("/root/"))
			{
				path = indexSystemModel.Options.RootDirectory.Substring("/root/".Length);
			}
			else
			{
				path = indexSystemModel.Options.RootDirectory;
			}
			return Path.GetFullPath(path);
		}

		public bool IsCatalogSuspended(Guid mdbGuid)
		{
			string indexSystemName = FastIndexVersion.GetIndexSystemName(mdbGuid);
			IndexSystemModel indexSystemModel = this.GetIndexSystemModel(indexSystemName);
			return indexSystemModel.Suspended;
		}

		internal static IDisposable SetInstanceTestHook(IndexManager mockIndexManager)
		{
			if (IndexManager.hookableInstance == null)
			{
				IndexManager instance = IndexManager.Instance;
			}
			return IndexManager.hookableInstance.SetTestHook(mockIndexManager);
		}

		internal bool CheckForNoPendingConfigurationUpdate()
		{
			bool result = false;
			for (int i = 0; i < 120; i++)
			{
				if (!this.PerformFastOperation<bool>(() => this.AdminService.IsPendingReconfiguration, "CheckIsPendingReconfiguration"))
				{
					result = true;
					break;
				}
				Thread.Sleep(1000);
			}
			return result;
		}

		internal string[] GetIndexSystemModelIndexesOptions(string indexName)
		{
			IndexSystemModel indexSystemModel = this.GetIndexSystemModel(indexName);
			IIndexSystemIndex[] indexes = indexSystemModel.Indexes;
			int num = 0;
			if (num >= indexes.Length)
			{
				return null;
			}
			IIndexSystemIndex indexSystemIndex = indexes[num];
			return indexSystemIndex.Options;
		}

		internal IndexSystemModel GetIndexSystemModel(string indexName)
		{
			return this.PerformFastOperation<IndexSystemModel>(() => this.IndexService.GetIndexSystemModel(indexName), "GetIndexSystemModel");
		}

		protected override void InternalConnectManagementAgents(WcfManagementClient client)
		{
			this.adminService = client.GetManagementAgent<IAdminServiceManagementAgent>("AdminService");
			this.clusterService = client.GetManagementAgent<IIndexClusterManagementAgent>("IndexClusterManager");
			this.indexService = client.GetManagementAgent<IIndexManagement>("IndexController");
		}

		private void InitializeIndexModels()
		{
			List<string> list = new List<string>(4);
			if (FastManagementClient.Config.EnableIndexPartsCache)
			{
				list.Add("disk_parts_memfs_max_total=" + FastManagementClient.Config.IndexPartsMaxCacheSize);
				list.Add("disk_parts_memfs_max_one=" + FastManagementClient.Config.MaxCacheSizePerIndexPart);
			}
			this.defaultModelIndexes = this.GetIndexSystemIndex(list);
			list.Add("query_mode=full");
			this.activeModelIndexes = this.GetIndexSystemIndex(list);
			list.Clear();
			list.Add("query_mode=simple");
			this.passiveModelIndexes = this.GetIndexSystemIndex(list);
		}

		private IIndexSystemIndex[] GetIndexSystemIndex(List<string> additionalOptionsToAdd)
		{
			List<string> list = new List<string>(IndexManager.defaultOptions);
			if (FastManagementClient.Config.MergeOverrideFolderUpdateGroup)
			{
				list.AddRange(IndexManager.overrideOptions);
			}
			if (FastManagementClient.Config.CachePreWarmingEnabled)
			{
				list.Add("cache_distribution=" + FastManagementClient.Config.CachePreWarmingSubCacheSizes);
			}
			if (additionalOptionsToAdd != null)
			{
				list.AddRange(additionalOptionsToAdd);
			}
			if (FastManagementClient.Config.EnableSingleValueRefiners)
			{
				list.Add("support_single_value_refiners=1");
				list.Add("support_default_values_for_single_value_refiners=1");
			}
			return new IndexSystemIndex[]
			{
				new IndexSystemIndex
				{
					Name = "FSIndex",
					Type = 1,
					Options = list.ToArray()
				}
			};
		}

		private CatalogState GetIndexingState(string indexName, IndexClusterMemberInfo indexClusterMemberInfo, IndexSystemModel indexModel)
		{
			if (indexClusterMemberInfo == null)
			{
				if (indexModel == null)
				{
					indexModel = this.GetIndexSystemModel(indexName);
				}
				if (indexModel == null || !indexModel.Suspended)
				{
					return CatalogState.Unknown;
				}
				return CatalogState.Suspended;
			}
			else
			{
				if (indexClusterMemberInfo.IndexingState == null)
				{
					return CatalogState.Unknown;
				}
				if (indexClusterMemberInfo.IndexingState.IsInvalid)
				{
					return CatalogState.Failed;
				}
				if (indexClusterMemberInfo.IndexingState.SeedingState.SeedingSource != null)
				{
					return CatalogState.Seeding;
				}
				return CatalogState.Healthy;
			}
		}

		private IndexClusterMemberInfo GetClusterMemberInfo(string indexName)
		{
			return this.PerformFastOperation<IndexClusterMemberInfo>(delegate()
			{
				string text;
				if (!this.indexClusterMap.TryGetValue(indexName, out text))
				{
					foreach (string text2 in this.ClusterService.Clusters)
					{
						if (text2.StartsWith(indexName, StringComparison.OrdinalIgnoreCase))
						{
							text = text2;
							this.indexClusterMap[indexName] = text;
							break;
						}
					}
				}
				if (string.IsNullOrEmpty(text))
				{
					return null;
				}
				IndexClusterMemberInfo result;
				using (IEnumerator<IndexClusterMemberInfo> enumerator2 = this.ClusterService.Members(text).GetEnumerator())
				{
					result = (enumerator2.MoveNext() ? enumerator2.Current : null);
				}
				return result;
			}, "GetClusterMemberInfo");
		}

		private void CreateIndexSystem(string indexSystemName, string indexNodeName, string databasePath, bool databaseCopyActive, RefinerUsage refinersToEnable)
		{
			base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "Creating index system {0} at {1}", new object[]
			{
				indexSystemName,
				databasePath ?? "default location"
			});
			IndexSystemModel model = null;
			base.PerformFastOperation(delegate()
			{
				model = this.IndexService.CreateIndexSystemModel(indexSystemName, "fastsearch");
				if (!string.IsNullOrEmpty(databasePath))
				{
					IndexSystemOptions options = new IndexSystemOptions(model.Options)
					{
						RootDirectory = "/root/" + databasePath,
						JournalBufferSize = FastManagementClient.Config.JournalBufferSize,
						ImportIndex = true,
						TriggerReseedOnDocsumLookupFailure = FastManagementClient.Config.TriggerReseedOnDocsumLookupFailure,
						InvalidateOnSeedingReadFailure = FastManagementClient.Config.InvalidateOnSeedingReadFailure
					};
					model.Options = options;
				}
				model.Schema = FastIndexSystemSchema.GetIndexSystemSchema(refinersToEnable);
				this.SetIndexSystemIndex(model, databaseCopyActive);
			}, "CreateIndexSystem");
			base.PerformFastOperation(delegate()
			{
				this.WaitForOperationFinished(this.IndexService.CreateIndexSystem(model, indexNodeName));
			}, "CreateIndexSystem");
		}

		private void SetIndexSystemIndex(IndexSystemModel model, bool databaseCopyActive)
		{
			if (FastManagementClient.Config.PassiveCatalogEnabled)
			{
				model.Indexes = (databaseCopyActive ? this.activeModelIndexes : this.passiveModelIndexes);
				return;
			}
			model.Indexes = this.defaultModelIndexes;
		}

		private string IndexSystemMode(bool databaseCopyActive, bool suspended)
		{
			if (suspended)
			{
				return "suspended";
			}
			if (!FastManagementClient.Config.PassiveCatalogEnabled)
			{
				return "default";
			}
			if (!databaseCopyActive)
			{
				return "passive";
			}
			return "active";
		}

		private bool UpdateIndexSystem(string indexName, Action<IndexSystemModel> action)
		{
			return this.PerformFastOperation<bool>(delegate()
			{
				IndexSystemModel indexSystemModel = this.IndexService.GetIndexSystemModel(indexName);
				if (indexSystemModel == null)
				{
					return false;
				}
				action(indexSystemModel);
				indexSystemModel.LastUpdated = 0L;
				int num = this.IndexService.UpdateIndexSystem(indexSystemModel);
				this.WaitForOperationFinished(num);
				bool boolReturnValue = this.IndexService.GetBoolReturnValue(num);
				this.DiagnosticsSession.TraceDebug<string, string, bool>("{0} index system {1}: {2}", boolReturnValue ? "Updated" : "Did not update", indexName, boolReturnValue);
				return boolReturnValue;
			}, "UpdateIndexSystem");
		}

		private void WaitForOperationFinished(int handle)
		{
			while (!this.IndexService.IsFinished(handle))
			{
				Thread.Sleep(100);
			}
		}

		internal const string SystemName = "Fsis";

		internal const string IndexRole = "Index";

		private const string IndexTemplate = "fastsearch";

		private const string RootDirectoryPrefix = "/root/";

		private const string PropertyFieldFormat = "&quot;{0}&quot;";

		private const int UpdateConfigurationRetryCount = 120;

		private const int ConfigurationRetryInterval = 1000;

		private static readonly string[] defaultOptions = new string[]
		{
			"cachemem_size=" + FastManagementClient.Config.MemCacheSize,
			"doc_sum_index_in_memory=0",
			"indextime_gen_extids=0",
			"restart_on_latent_refinable_change=1",
			"use_context_lengths=0",
			"merges_concurrent_max=" + FastManagementClient.Config.MaxConcurrentMerges,
			"max_failed_merges=" + FastManagementClient.Config.MaxFailedMerges,
			"master_merges_concurrent_max=" + FastManagementClient.Config.MaxConcurrentMasterMerges,
			"master_merge_trigger_ratio_periodic=" + FastManagementClient.Config.MasterMergeTriggerRatioPeriodic,
			"master_merge_trigger_ratio_daily=" + FastManagementClient.Config.MasterMergeTriggerRatioDaily,
			"master_merge_trigger_ratio_weekly=" + FastManagementClient.Config.MasterMergeTriggerRatioWeekly,
			"master_merge_trigger_minute_of_day=" + FastManagementClient.Config.MasterMergeTriggerMinuteOfDay,
			"master_merge_trigger_minute_of_week=" + FastManagementClient.Config.MasterMergeTriggerMinuteOfWeek,
			"merge_levels_values_default=" + FastManagementClient.Config.MergeLevelsValuesDefault,
			"expected_doccount_mergeunit=" + FastManagementClient.Config.ExpectedDocCountMergeUnit,
			"failed_merges_before_reseed=" + FastManagementClient.Config.MaxFailedMerges,
			"failed_checkpoints_before_reseed=" + FastManagementClient.Config.FailedCheckpointsBeforeReseed,
			"failed_generations_before_reseed=" + FastManagementClient.Config.FailedGenerationsBeforeReseed
		};

		private static readonly string[] overrideOptions = new string[]
		{
			"merge_override1_updategroups=FolderUpdateGroup",
			"merge_override1_levels=" + FastManagementClient.Config.OverrideFolderUpdateGroupMergeLevels,
			"merge_override1_unit=" + FastManagementClient.Config.OverrideFolderUpdateGroupMergeUnit,
			"merge_override1_master_merge_ratio=" + FastManagementClient.Config.OverrideFolderUpdateGroupMasterMergeRatio
		};

		private static object lockObject = new object();

		private static Hookable<IndexManager> hookableInstance;

		private volatile IAdminServiceManagementAgent adminService;

		private volatile IIndexClusterManagementAgent clusterService;

		private volatile IIndexManagement indexService;

		private ConcurrentDictionary<string, string> indexClusterMap = new ConcurrentDictionary<string, string>();

		private IIndexSystemIndex[] defaultModelIndexes;

		private IIndexSystemIndex[] activeModelIndexes;

		private IIndexSystemIndex[] passiveModelIndexes;
	}
}
