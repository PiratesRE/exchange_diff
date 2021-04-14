using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal class DumpClusterTopology : IDisposable
	{
		public DumpClusterTopology(string nameCluster) : this(nameCluster, null)
		{
		}

		public DumpClusterTopology(string nameCluster, ITaskOutputHelper output)
		{
			this.m_ownsClusterHandle = true;
			base..ctor();
			this.m_nodeNameToConnect = nameCluster;
			this.m_output = output;
			this.m_indentlevel = 0U;
			Exception ex = null;
			try
			{
				if (string.IsNullOrEmpty(nameCluster))
				{
					this.WriteLine("DumpClusterTopology: Opening local cluster.", new object[0]);
					this.m_cluster = AmCluster.Open();
				}
				else
				{
					this.WriteLine("DumpClusterTopology: Opening remote cluster {0}.", new object[]
					{
						nameCluster
					});
					AmServerName serverName = new AmServerName(nameCluster);
					this.m_cluster = AmCluster.OpenByName(serverName);
				}
			}
			catch (ClusterException ex2)
			{
				ex = ex2;
			}
			catch (AmCommonTransientException ex3)
			{
				ex = ex3;
			}
			catch (AmCommonException ex4)
			{
				ex = ex4;
			}
			catch (TransientException ex5)
			{
				ex = ex5;
			}
			if (ex != null)
			{
				this.WriteLine("DumpClusterTopology: Failed opening with {0}", new object[]
				{
					ex
				});
			}
		}

		public DumpClusterTopology(AmCluster cluster, ITaskOutputHelper output)
		{
			this.m_ownsClusterHandle = true;
			base..ctor();
			this.m_nodeNameToConnect = "<existing cluster handle>";
			this.m_output = output;
			this.m_indentlevel = 0U;
			this.m_cluster = cluster;
			this.m_ownsClusterHandle = false;
		}

		private void DumpResourceIpAddress(AmClusterResource resource, string typeName)
		{
			this.m_indentlevel += 1U;
			this.WriteLine("Address = [{0}]", new object[]
			{
				resource.GetPrivateProperty<string>("Address")
			});
			if (SharedHelper.StringIEquals(typeName, "IP Address"))
			{
				this.m_indentlevel += 1U;
				this.WriteLine("EnableDhcp = [{0}]", new object[]
				{
					resource.GetPrivateProperty<int>("EnableDhcp")
				});
				this.m_indentlevel -= 1U;
			}
			string networkNameFromIpResource = AmClusterResourceHelper.GetNetworkNameFromIpResource(null, resource);
			this.m_indentlevel += 1U;
			this.WriteLine("Network = [{0}]", new object[]
			{
				networkNameFromIpResource
			});
			this.m_indentlevel -= 1U;
			this.m_indentlevel -= 1U;
		}

		private void DumpResourceNetName(AmClusterResource resource)
		{
			this.m_indentlevel += 1U;
			this.WriteLine("NetName = [{0}]", new object[]
			{
				resource.GetPrivateProperty<string>("Name")
			});
			int privateProperty = resource.GetPrivateProperty<int>("RequireKerberos");
			if (privateProperty != 0)
			{
				this.m_indentlevel += 1U;
				this.WriteLine("RequireKerberos = 1. Computer account created on DC '{0}'.", new object[]
				{
					resource.GetPrivateProperty<string>("CreatingDC")
				});
				this.m_indentlevel -= 1U;
			}
			this.m_indentlevel -= 1U;
		}

		private void DumpResource(AmClusterResource resource)
		{
			string typeName = resource.GetTypeName();
			List<string> list = new List<string>(resource.EnumeratePossibleOwnerNames());
			string text = string.Join(",", list.ToArray());
			this.WriteLine("Resource: {0} [{1}, type = {2}, PossibleOwners = {3} ]", new object[]
			{
				resource.Name,
				resource.GetState(),
				typeName,
				text
			});
			if (SharedHelper.StringIEquals(typeName, "IP Address") || SharedHelper.StringIEquals(typeName, "IPv6 Address"))
			{
				this.DumpResourceIpAddress(resource, typeName);
				return;
			}
			if (SharedHelper.StringIEquals(typeName, "Network Name"))
			{
				this.DumpResourceNetName(resource);
			}
		}

		private void DumpGroup(AmClusterGroup group)
		{
			string text;
			if (group.IsQuorumGroup() && group.IsClusterGroup())
			{
				text = "Cluster Quorum/Main Group";
			}
			else if (group.IsQuorumGroup())
			{
				text = "Cluster Quorum Group";
			}
			else if (group.IsClusterGroup())
			{
				text = "Cluster Main Group";
			}
			else
			{
				text = "not a CMS";
			}
			this.WriteLine("group: {0} [{1}]", new object[]
			{
				group.Name,
				text
			});
			this.m_indentlevel += 1U;
			this.WriteLine("OwnerNode: {0}", new object[]
			{
				group.OwnerNode
			});
			this.WriteLine("State: {0}", new object[]
			{
				group.State
			});
			this.m_indentlevel += 1U;
			foreach (AmClusterResource amClusterResource in group.EnumerateResources())
			{
				using (amClusterResource)
				{
					this.DumpResource(amClusterResource);
				}
			}
			this.m_indentlevel -= 1U;
			this.m_indentlevel -= 1U;
		}

		private void DumpGroups()
		{
			this.WriteLine("Groups", new object[0]);
			this.m_indentlevel += 1U;
			foreach (IAmClusterGroup amClusterGroup in this.m_cluster.EnumerateGroups())
			{
				using (amClusterGroup)
				{
					this.DumpGroup((AmClusterGroup)amClusterGroup);
				}
			}
			this.m_indentlevel -= 1U;
		}

		private void DumpNodes()
		{
			this.WriteLine("Nodes", new object[0]);
			this.m_indentlevel += 1U;
			foreach (IAmClusterNode amClusterNode in this.m_cluster.EnumerateNodes())
			{
				using (amClusterNode)
				{
					this.WriteLine("node: {0} [ state = {1} ]", new object[]
					{
						amClusterNode.Name,
						amClusterNode.State
					});
				}
			}
			this.m_indentlevel -= 1U;
		}

		public int DumpInternal()
		{
			int result = 0;
			this.WriteLine("Dumping the cluster by connecting to: {0}.", new object[]
			{
				this.m_nodeNameToConnect
			});
			if (this.m_cluster == null)
			{
				this.WriteLine("DumpClusterTopology: Not dumping, since the cluster could not be contacted.", new object[0]);
				result = 53;
			}
			else
			{
				this.WriteLine("The cluster's name is: {0}.", new object[]
				{
					this.m_cluster.Name
				});
				this.DumpGroups();
				this.DumpNodes();
				this.DumpNets();
			}
			return result;
		}

		public int Dump()
		{
			int result = 0;
			try
			{
				result = this.DumpInternal();
			}
			catch (LocalizedException ex)
			{
				this.WriteLine("A localized exception was caught while dumping the cluster! {0}", new object[]
				{
					ex.Message
				});
				if (this.m_output != null)
				{
					this.m_output.WriteErrorSimple(ex);
				}
			}
			return result;
		}

		private void DumpNets()
		{
			this.WriteLine("Subnets", new object[0]);
			this.m_indentlevel += 1U;
			foreach (AmClusterNetwork amClusterNetwork in this.m_cluster.EnumerateNetworks())
			{
				using (amClusterNetwork)
				{
					DatabaseAvailabilityGroupSubnetId databaseAvailabilityGroupSubnetId = ExchangeSubnet.ExtractSubnetId(amClusterNetwork);
					this.WriteLine("Name({0}), Mask({1}), Role({2})", new object[]
					{
						amClusterNetwork.Name,
						databaseAvailabilityGroupSubnetId,
						amClusterNetwork.GetNativeRole()
					});
					this.m_indentlevel += 1U;
					foreach (AmClusterNetInterface amClusterNetInterface in amClusterNetwork.EnumerateNetworkInterfaces())
					{
						using (amClusterNetInterface)
						{
							this.WriteLine("NIC {0} on Node {1} in State={2}", new object[]
							{
								amClusterNetInterface.GetAddress(),
								amClusterNetInterface.GetNodeName(),
								amClusterNetInterface.GetState(false)
							});
						}
					}
					this.m_indentlevel -= 1U;
				}
			}
			this.m_indentlevel -= 1U;
		}

		public int DumpOnlyResources()
		{
			this.WriteLine("Sorry, DumpOnlyResources() is not implemented yet.", new object[0]);
			throw new NotImplementedException();
		}

		private static void MyWriteVerbose(Task task, LocalizedString localizedString)
		{
			TaskLogger.Log(localizedString);
			task.WriteVerbose(localizedString);
		}

		private void WriteLine(string format, params object[] args)
		{
			if (this.m_output != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(' ', (int)(4U * this.m_indentlevel));
				stringBuilder.AppendFormat(format, args);
				this.m_output.AppendLogMessage("{0}", new object[]
				{
					stringBuilder.ToString()
				});
				return;
			}
			for (uint num = 0U; num < this.m_indentlevel; num += 1U)
			{
				Console.Write("  ");
			}
			Console.WriteLine(format, args);
		}

		public void Dispose()
		{
			if (!this.m_fDisposed)
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}
		}

		public void Dispose(bool disposing)
		{
			lock (this)
			{
				if (!this.m_fDisposed)
				{
					if (disposing && this.m_ownsClusterHandle && this.m_cluster != null)
					{
						this.m_cluster.Dispose();
					}
					this.m_fDisposed = true;
				}
			}
		}

		private readonly AmCluster m_cluster;

		private readonly bool m_ownsClusterHandle;

		private readonly string m_nodeNameToConnect;

		private uint m_indentlevel;

		private ITaskOutputHelper m_output;

		private bool m_fDisposed;
	}
}
