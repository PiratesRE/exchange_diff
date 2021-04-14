using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Cluster;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal class DagNetworkConfigDataProvider : IConfigDataProvider
	{
		public string TargetServer
		{
			get
			{
				return this.m_targetServer;
			}
		}

		public DatabaseAvailabilityGroup TargetDag
		{
			get
			{
				return this.m_dag;
			}
		}

		public DagNetworkConfiguration NetworkConfig
		{
			get
			{
				return this.m_netConfig;
			}
		}

		public DagNetworkConfigDataProvider(IConfigurationSession adSession, string targetServer, DatabaseAvailabilityGroup dag)
		{
			this.m_adSession = adSession;
			this.m_targetServer = targetServer;
			this.m_dag = dag;
		}

		public IConfigurable Read<T>(ObjectId identity) where T : IConfigurable, new()
		{
			return this.m_dagNet;
		}

		public IEnumerable<T> FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where T : IConfigurable, new()
		{
			return (IEnumerable<T>)this.Find(filter, rootId, deepSearch, sortBy);
		}

		public IConfigurable[] Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy) where T : IConfigurable, new()
		{
			return this.Find(filter, rootId, deepSearch, sortBy).ToArray();
		}

		private List<DatabaseAvailabilityGroupNetwork> Find(QueryFilter queryFilter, ObjectId rootId, bool deepSearch, SortBy sortBy)
		{
			List<DatabaseAvailabilityGroupNetwork> list = new List<DatabaseAvailabilityGroupNetwork>();
			DagNetworkQueryFilter dagNetworkQueryFilter = null;
			if (queryFilter != null)
			{
				dagNetworkQueryFilter = (queryFilter as DagNetworkQueryFilter);
			}
			IEnumerable<DatabaseAvailabilityGroup> enumerable;
			if (this.TargetDag != null)
			{
				enumerable = new DatabaseAvailabilityGroup[]
				{
					this.TargetDag
				};
			}
			else
			{
				string identity = "*";
				if (dagNetworkQueryFilter != null && dagNetworkQueryFilter.NamesToMatch != null && dagNetworkQueryFilter.NamesToMatch.DagName != null)
				{
					identity = dagNetworkQueryFilter.NamesToMatch.DagName;
				}
				DatabaseAvailabilityGroupIdParameter databaseAvailabilityGroupIdParameter = DatabaseAvailabilityGroupIdParameter.Parse(identity);
				enumerable = databaseAvailabilityGroupIdParameter.GetObjects<DatabaseAvailabilityGroup>(null, this.m_adSession);
			}
			if (enumerable != null)
			{
				Regex regex = null;
				if (dagNetworkQueryFilter != null && dagNetworkQueryFilter.NamesToMatch != null && dagNetworkQueryFilter.NamesToMatch.NetName != null)
				{
					string pattern = Wildcard.ConvertToRegexPattern(dagNetworkQueryFilter.NamesToMatch.NetName);
					regex = new Regex(pattern, RegexOptions.IgnoreCase);
				}
				foreach (DatabaseAvailabilityGroup databaseAvailabilityGroup in enumerable)
				{
					DagNetworkConfiguration dagNetworkConfiguration = (this.m_targetServer == null) ? DagNetworkRpc.GetDagNetworkConfig(databaseAvailabilityGroup) : DagNetworkRpc.GetDagNetworkConfig(this.m_targetServer);
					if (dagNetworkConfiguration != null && dagNetworkConfiguration.Networks != null)
					{
						this.m_netConfig = dagNetworkConfiguration;
						foreach (DatabaseAvailabilityGroupNetwork databaseAvailabilityGroupNetwork in dagNetworkConfiguration.Networks)
						{
							if (regex == null || regex.IsMatch(databaseAvailabilityGroupNetwork.Name))
							{
								DagNetworkObjectId identity2 = new DagNetworkObjectId(databaseAvailabilityGroup.Name, databaseAvailabilityGroupNetwork.Name);
								databaseAvailabilityGroupNetwork.SetIdentity(identity2);
								databaseAvailabilityGroupNetwork.ResetChangeTracking();
								list.Add(databaseAvailabilityGroupNetwork);
							}
						}
					}
				}
			}
			return list;
		}

		private DatabaseAvailabilityGroup GetDagByName(string dagName)
		{
			if (this.m_dag == null || this.m_dag.Name != dagName)
			{
				this.m_dag = DagTaskHelper.ReadDagByName(dagName, this.m_adSession);
			}
			return this.m_dag;
		}

		public DagNetworkConfiguration ReadNetConfig(DatabaseAvailabilityGroup dag)
		{
			this.m_netConfig = DagNetworkRpc.GetDagNetworkConfig(dag);
			return this.m_netConfig;
		}

		public void Save(IConfigurable instance)
		{
			DatabaseAvailabilityGroupNetwork databaseAvailabilityGroupNetwork = (DatabaseAvailabilityGroupNetwork)instance;
			DagNetworkObjectId dagNetworkObjectId = (DagNetworkObjectId)databaseAvailabilityGroupNetwork.Identity;
			SetDagNetworkRequest setDagNetworkRequest = new SetDagNetworkRequest();
			if (dagNetworkObjectId == null)
			{
				setDagNetworkRequest.Name = databaseAvailabilityGroupNetwork.Name;
			}
			else
			{
				setDagNetworkRequest.Name = dagNetworkObjectId.NetName;
			}
			if (databaseAvailabilityGroupNetwork.IsChanged(DatabaseAvailabilityGroupNetworkSchema.Name))
			{
				setDagNetworkRequest.NewName = databaseAvailabilityGroupNetwork.Name;
			}
			if (databaseAvailabilityGroupNetwork.IsChanged(DatabaseAvailabilityGroupNetworkSchema.Description))
			{
				setDagNetworkRequest.Description = databaseAvailabilityGroupNetwork.Description;
			}
			if (databaseAvailabilityGroupNetwork.IsChanged(DatabaseAvailabilityGroupNetworkSchema.ReplicationEnabled))
			{
				setDagNetworkRequest.ReplicationEnabled = databaseAvailabilityGroupNetwork.ReplicationEnabled;
			}
			if (databaseAvailabilityGroupNetwork.IsChanged(DatabaseAvailabilityGroupNetworkSchema.IgnoreNetwork))
			{
				setDagNetworkRequest.IgnoreNetwork = databaseAvailabilityGroupNetwork.IgnoreNetwork;
			}
			if (databaseAvailabilityGroupNetwork.IsChanged(DatabaseAvailabilityGroupNetworkSchema.Subnets))
			{
				setDagNetworkRequest.SubnetListIsSet = true;
				foreach (DatabaseAvailabilityGroupNetworkSubnet databaseAvailabilityGroupNetworkSubnet in databaseAvailabilityGroupNetwork.Subnets)
				{
					setDagNetworkRequest.Subnets.Add(databaseAvailabilityGroupNetworkSubnet.SubnetId, null);
				}
			}
			this.GetDagByName(dagNetworkObjectId.DagName);
			DagNetworkRpc.SetDagNetwork(this.m_dag, setDagNetworkRequest);
			DagNetworkObjectId identity = new DagNetworkObjectId(this.m_dag.Name, databaseAvailabilityGroupNetwork.Name);
			databaseAvailabilityGroupNetwork.SetIdentity(identity);
			databaseAvailabilityGroupNetwork.ResetChangeTracking();
			this.m_dagNet = databaseAvailabilityGroupNetwork;
		}

		public void Delete(IConfigurable instance)
		{
			DatabaseAvailabilityGroupNetwork databaseAvailabilityGroupNetwork = (DatabaseAvailabilityGroupNetwork)instance;
			DagNetworkObjectId dagNetworkObjectId = (DagNetworkObjectId)databaseAvailabilityGroupNetwork.Identity;
			this.GetDagByName(dagNetworkObjectId.DagName);
			RemoveDagNetworkRequest removeDagNetworkRequest = new RemoveDagNetworkRequest();
			removeDagNetworkRequest.Name = dagNetworkObjectId.NetName;
			DagNetworkRpc.RemoveDagNetwork(this.m_dag, removeDagNetworkRequest);
		}

		public string Source
		{
			get
			{
				if (this.m_dag != null)
				{
					return this.m_dag.Name;
				}
				return null;
			}
		}

		private IConfigurationSession m_adSession;

		private readonly string m_targetServer;

		private DatabaseAvailabilityGroup m_dag;

		private DatabaseAvailabilityGroupNetwork m_dagNet;

		private DagNetworkConfiguration m_netConfig;
	}
}
