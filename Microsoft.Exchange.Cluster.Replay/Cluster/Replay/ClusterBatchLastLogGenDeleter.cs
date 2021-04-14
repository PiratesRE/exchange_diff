using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ClusterBatchLastLogGenDeleter
	{
		public ClusterBatchLastLogGenDeleter(IAmCluster cluster, List<Database> databases, ILogTraceHelper logger)
		{
			this.m_cluster = cluster;
			this.m_databases = databases;
			this.m_logger = logger;
		}

		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ClusterTracer;
			}
		}

		public void DeleteTimeStamps()
		{
			if (this.m_databases == null || this.m_databases.Count == 0)
			{
				ClusterBatchLastLogGenDeleter.Tracer.TraceDebug((long)this.GetHashCode(), "No databases specified, so skipping cluster batch timestamp deletion.");
				return;
			}
			AmClusterHandle amClusterHandle;
			if (this.m_cluster == null)
			{
				amClusterHandle = ClusapiMethods.OpenCluster(null);
			}
			else
			{
				amClusterHandle = this.m_cluster.Handle;
			}
			if (amClusterHandle.IsInvalid)
			{
				Exception ex = new ClusterApiException("OpenCluster", new Win32Exception());
				ClusterBatchLastLogGenDeleter.Tracer.TraceError<Exception>((long)this.GetHashCode(), "The cluster handle is invalid! Exception: {0}", ex);
				throw ex;
			}
			try
			{
				this.DeleteTimeStampsInternal(amClusterHandle);
			}
			finally
			{
				if (this.m_cluster == null)
				{
					amClusterHandle.Close();
				}
			}
		}

		private void DeleteTimeStampsInternal(AmClusterHandle clusterHandle)
		{
			using (IDistributedStoreKey clusterKey = DistributedStore.Instance.GetClusterKey(clusterHandle, null, null, DxStoreKeyAccessMode.Write, false))
			{
				using (IDistributedStoreKey distributedStoreKey = clusterKey.OpenKey("ExchangeActiveManager", DxStoreKeyAccessMode.Write, true, null))
				{
					if (distributedStoreKey != null)
					{
						using (IDistributedStoreKey distributedStoreKey2 = distributedStoreKey.OpenKey("LastLog", DxStoreKeyAccessMode.Write, true, null))
						{
							if (distributedStoreKey2 != null)
							{
								using (IDistributedStoreBatchRequest distributedStoreBatchRequest = distributedStoreKey2.CreateBatchUpdateRequest())
								{
									foreach (Database database in this.m_databases)
									{
										string name = database.Name;
										string text = database.Guid.ToString();
										string propertyName = AmDbState.ConstructLastLogTimeStampProperty(text);
										string value = distributedStoreKey2.GetValue(propertyName, null, null);
										if (value != null)
										{
											ClusterBatchLastLogGenDeleter.Tracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "LastLogGeneration time stamp for database [{0} ({1})] found with value '{2}'.", name, text, value);
											this.m_logger.AppendLogMessage("Deleting LastLogGeneration time stamp from cluster registry for database [{0} ({1})] with existing value: '{2}'.", new object[]
											{
												name,
												text,
												value
											});
											distributedStoreBatchRequest.DeleteValue(propertyName);
										}
										else
										{
											ClusterBatchLastLogGenDeleter.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "LastLogGeneration time stamp for database [{0} ({1})] does not exist.", name, text);
										}
									}
									distributedStoreBatchRequest.Execute(null);
									goto IL_151;
								}
							}
							ClusterBatchLastLogGenDeleter.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "ActiveManager LastLog key '{0}\\{1}' does not exist in the cluster registry. Skipping deletion.", "ExchangeActiveManager", "LastLog");
							IL_151:
							goto IL_178;
						}
					}
					ClusterBatchLastLogGenDeleter.Tracer.TraceDebug<string>((long)this.GetHashCode(), "ActiveManager root key '{0}' does not exist in the cluster registry. Skipping deletion.", "ExchangeActiveManager");
					IL_178:;
				}
			}
		}

		private const string AmRootKeyName = "ExchangeActiveManager";

		private const string LastLogKeyName = "LastLog";

		private readonly List<Database> m_databases;

		private readonly IAmCluster m_cluster;

		private ILogTraceHelper m_logger;
	}
}
