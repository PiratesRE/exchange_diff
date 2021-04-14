using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Cluster;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "DatabaseAvailabilityGroup")]
	public sealed class GetDatabaseAvailabilityGroup : GetSystemConfigurationObjectTask<DatabaseAvailabilityGroupIdParameter, DatabaseAvailabilityGroup>
	{
		[Parameter]
		public SwitchParameter Status
		{
			get
			{
				return (SwitchParameter)(base.Fields["Status"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Status"] = value;
			}
		}

		protected override bool IsKnownException(Exception e)
		{
			return DagTaskHelper.IsKnownException(this, e) || base.IsKnownException(e);
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 78, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Cluster\\GetDatabaseAvailabilityGroup.cs");
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			DatabaseAvailabilityGroup databaseAvailabilityGroup = (DatabaseAvailabilityGroup)dataObject;
			if (this.Status && !databaseAvailabilityGroup.IsDagEmpty())
			{
				List<ADObjectId> list = new List<ADObjectId>(8);
				List<ADObjectId> list2 = new List<ADObjectId>(8);
				AmCluster amCluster = null;
				try
				{
					amCluster = AmCluster.OpenDagClus(databaseAvailabilityGroup);
				}
				catch (ClusterException exception)
				{
					base.WriteError(exception, ErrorCategory.InvalidArgument, null);
				}
				if (amCluster != null)
				{
					using (amCluster)
					{
						foreach (IAmClusterNode amClusterNode in amCluster.EnumerateNodes())
						{
							using (amClusterNode)
							{
								AmNodeState state = amClusterNode.GetState(false);
								if (AmClusterNode.IsNodeUp(state))
								{
									ADObjectId adobjectId = DagTaskHelper.FindServerAdObjectIdInDag(databaseAvailabilityGroup, amClusterNode.Name);
									if (adobjectId != null)
									{
										list.Add(adobjectId);
										if (state == AmNodeState.Paused)
										{
											list2.Add(adobjectId);
										}
									}
									else
									{
										this.WriteWarning(Strings.WarningClusterNodeNotFoundInDag(amClusterNode.Name.Fqdn, databaseAvailabilityGroup.Name));
									}
								}
							}
						}
						databaseAvailabilityGroup.OperationalServers = list.ToArray();
						databaseAvailabilityGroup.ServersInMaintenance = list2.ToArray();
						DagNetworkConfiguration dagNetworkConfig = DagNetworkRpc.GetDagNetworkConfig(databaseAvailabilityGroup);
						databaseAvailabilityGroup.ReplicationPort = dagNetworkConfig.ReplicationPort;
						foreach (DatabaseAvailabilityGroupNetwork databaseAvailabilityGroupNetwork in dagNetworkConfig.Networks)
						{
							databaseAvailabilityGroup.NetworkNames.Add(databaseAvailabilityGroupNetwork.Name);
						}
						if (DagHelper.IsQuorumTypeFileShareWitness(amCluster))
						{
							IAmClusterGroup amClusterGroup = amCluster.FindCoreClusterGroup();
							if (amClusterGroup == null)
							{
								databaseAvailabilityGroup.WitnessShareInUse = new WitnessShareUsage?(WitnessShareUsage.InvalidConfiguration);
								this.WriteWarning(Strings.WarningClusterGroupNotFormed(databaseAvailabilityGroup.Name));
								goto IL_306;
							}
							using (amClusterGroup)
							{
								IEnumerable<AmClusterResource> enumerable = null;
								try
								{
									enumerable = amClusterGroup.EnumerateResourcesOfType("File Share Witness");
									AmClusterResource amClusterResource = enumerable.ElementAtOrDefault(0);
									if (amClusterResource == null)
									{
										databaseAvailabilityGroup.WitnessShareInUse = new WitnessShareUsage?(WitnessShareUsage.InvalidConfiguration);
										this.WriteWarning(Strings.WarningFswNotFound(databaseAvailabilityGroup.Name));
									}
									else if (amClusterResource.GetState() == AmResourceState.Failed)
									{
										databaseAvailabilityGroup.WitnessShareInUse = new WitnessShareUsage?(WitnessShareUsage.InvalidConfiguration);
										this.WriteWarning(Strings.WarningFswFailed(databaseAvailabilityGroup.Name));
									}
									else
									{
										string privateProperty = amClusterResource.GetPrivateProperty<string>("SharePath");
										UncFileSharePath a;
										if (UncFileSharePath.TryParse(privateProperty, out a))
										{
											if (a == databaseAvailabilityGroup.FileShareWitness)
											{
												databaseAvailabilityGroup.WitnessShareInUse = new WitnessShareUsage?(WitnessShareUsage.Primary);
											}
											else if (a == databaseAvailabilityGroup.AlternateFileShareWitness)
											{
												databaseAvailabilityGroup.WitnessShareInUse = new WitnessShareUsage?(WitnessShareUsage.Alternate);
											}
											else
											{
												databaseAvailabilityGroup.WitnessShareInUse = new WitnessShareUsage?(WitnessShareUsage.InvalidConfiguration);
												this.WriteWarning(Strings.WarningFswNotPrimaryOrAlternate(databaseAvailabilityGroup.Name));
											}
										}
										else
										{
											databaseAvailabilityGroup.WitnessShareInUse = new WitnessShareUsage?(WitnessShareUsage.InvalidConfiguration);
											this.WriteWarning(Strings.WarningFswNotValidPath(databaseAvailabilityGroup.Name, privateProperty));
										}
									}
								}
								finally
								{
									if (enumerable != null)
									{
										foreach (AmClusterResource amClusterResource2 in enumerable)
										{
											using (amClusterResource2)
											{
											}
										}
									}
								}
								goto IL_306;
							}
						}
						databaseAvailabilityGroup.WitnessShareInUse = new WitnessShareUsage?(WitnessShareUsage.None);
						IL_306:;
					}
					this.UpdatePam(databaseAvailabilityGroup);
					try
					{
						DeferredFailoverEntry[] serversInDeferredRecovery = DagTaskHelper.GetServersInDeferredRecovery(databaseAvailabilityGroup);
						databaseAvailabilityGroup.ServersInDeferredRecovery = serversInDeferredRecovery;
					}
					catch (AmServerException ex)
					{
						this.WriteWarning(Strings.PAMOtherError(ex.Message));
					}
				}
			}
			base.WriteResult(databaseAvailabilityGroup);
			TaskLogger.LogExit();
		}

		private void UpdatePam(DatabaseAvailabilityGroup dag)
		{
			try
			{
				AmServerName primaryActiveManagerNode = DagTaskHelper.GetPrimaryActiveManagerNode(dag);
				if (primaryActiveManagerNode != null)
				{
					dag.PrimaryActiveManager = DagTaskHelper.FindServerAdObjectIdInDag(dag, primaryActiveManagerNode);
				}
			}
			catch (RpcException ex)
			{
				this.WriteWarning(Strings.PAMRPCError(ex.Message));
			}
			catch (AmFailedToDeterminePAM amFailedToDeterminePAM)
			{
				this.WriteWarning(Strings.PAMOtherError(amFailedToDeterminePAM.Message));
			}
			catch (AmServerException ex2)
			{
				this.WriteWarning(Strings.PAMOtherError(ex2.Message));
			}
			catch (AmServerTransientException ex3)
			{
				this.WriteWarning(Strings.PAMOtherError(ex3.Message));
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}
	}
}
