using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.UM.Rpc;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Get", "UMActiveCalls", DefaultParameterSetName = "Server")]
	public sealed class GetUMActiveCalls : Task
	{
		[Parameter]
		public Fqdn DomainController
		{
			get
			{
				return (Fqdn)base.Fields["DomainController"];
			}
			set
			{
				base.Fields["DomainController"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Server", ValueFromPipeline = true)]
		[ValidateNotNullOrEmpty]
		public ServerIdParameter Server
		{
			get
			{
				return (ServerIdParameter)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "ServerInstance")]
		public UMServer InstanceServer
		{
			get
			{
				return (UMServer)base.Fields["InstanceServer"];
			}
			set
			{
				base.Fields["InstanceServer"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "DialPlan")]
		public UMDialPlanIdParameter DialPlan
		{
			get
			{
				return (UMDialPlanIdParameter)base.Fields["DialPlan"];
			}
			set
			{
				base.Fields["DialPlan"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "UMIPGateway")]
		[ValidateNotNullOrEmpty]
		public UMIPGatewayIdParameter IPGateway
		{
			get
			{
				return (UMIPGatewayIdParameter)base.Fields["IPGateway"];
			}
			set
			{
				base.Fields["IPGateway"] = value;
			}
		}

		protected override void InternalValidate()
		{
			Dictionary<Guid, bool> dictionary = new Dictionary<Guid, bool>();
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!base.HasErrors)
			{
				this.rootOrgConfigSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(this.DomainController, true, ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 123, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\um\\GetUMActiveCalls.cs");
				if (this.DialPlan != null)
				{
					IEnumerable<UMDialPlan> objects = this.DialPlan.GetObjects<UMDialPlan>(null, this.rootOrgConfigSession);
					using (IEnumerator<UMDialPlan> enumerator = objects.GetEnumerator())
					{
						if (!enumerator.MoveNext())
						{
							base.WriteError(new LocalizedException(Strings.NonExistantDialPlan(this.DialPlan.ToString())), ErrorCategory.InvalidData, this.DialPlan);
						}
						this.adResults = enumerator.Current;
						if (enumerator.MoveNext())
						{
							base.WriteError(new LocalizedException(Strings.MultipleDialplansWithSameId(this.DialPlan.ToString())), ErrorCategory.InvalidData, this.DialPlan);
						}
					}
					UMDialPlan dialPlan = (UMDialPlan)this.adResults;
					if (Utility.DialPlanHasIncompatibleServers(dialPlan, this.rootOrgConfigSession, dictionary))
					{
						this.WriteWarning(Strings.InvalidServerVersionInDialPlan(((UMDialPlan)this.adResults).Name));
					}
				}
				else
				{
					if (this.IPGateway != null)
					{
						IEnumerable<UMIPGateway> objects2 = this.IPGateway.GetObjects<UMIPGateway>(null, this.rootOrgConfigSession);
						using (IEnumerator<UMIPGateway> enumerator2 = objects2.GetEnumerator())
						{
							if (!enumerator2.MoveNext())
							{
								base.WriteError(new LocalizedException(Strings.NonExistantIPGateway(this.IPGateway.ToString())), ErrorCategory.InvalidData, this.IPGateway);
							}
							this.adResults = enumerator2.Current;
							if (enumerator2.MoveNext())
							{
								base.WriteError(new LocalizedException(Strings.MultipleIPGatewaysWithSameId(this.IPGateway.ToString())), ErrorCategory.InvalidData, this.IPGateway);
							}
						}
						if (CommonConstants.UseDataCenterCallRouting)
						{
							goto IL_36F;
						}
						UMIPGateway umipgateway = (UMIPGateway)this.adResults;
						using (MultiValuedProperty<UMHuntGroup>.Enumerator enumerator3 = umipgateway.HuntGroups.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								UMHuntGroup umhuntGroup = enumerator3.Current;
								if (umhuntGroup.UMDialPlan != null && !dictionary.ContainsKey(umhuntGroup.UMDialPlan.ObjectGuid))
								{
									dictionary[umhuntGroup.UMDialPlan.ObjectGuid] = true;
									IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.DomainController, true, ConsistencyMode.FullyConsistent, ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(umhuntGroup.UMDialPlan), 189, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\um\\GetUMActiveCalls.cs");
									UMDialPlan dialPlan2 = tenantOrTopologyConfigurationSession.Read<UMDialPlan>(umhuntGroup.UMDialPlan);
									if (Utility.DialPlanHasIncompatibleServers(dialPlan2, this.rootOrgConfigSession, dictionary))
									{
										this.WriteWarning(Strings.InvalidServerVersionInGateway(umipgateway.Name));
									}
								}
							}
							goto IL_36F;
						}
					}
					if (this.Server != null)
					{
						IEnumerable<Server> objects3 = this.Server.GetObjects<Server>(null, this.rootOrgConfigSession);
						using (IEnumerator<Server> enumerator4 = objects3.GetEnumerator())
						{
							if (!enumerator4.MoveNext())
							{
								base.WriteError(new LocalizedException(Strings.NonExistantServer(this.Server.ToString())), ErrorCategory.InvalidData, this.Server);
							}
							this.adResults = enumerator4.Current;
							if (enumerator4.MoveNext())
							{
								base.WriteError(new LocalizedException(Strings.MultipleServersWithSameId(this.Server.ToString())), ErrorCategory.InvalidData, this.Server);
							}
						}
						Server target = (Server)this.adResults;
						this.ValidateServerIsCompatible(target);
					}
					else if (this.InstanceServer != null)
					{
						Server target2 = (Server)this.InstanceServer.DataObject;
						this.ValidateServerIsCompatible(target2);
					}
				}
			}
			IL_36F:
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			string serverName = string.Empty;
			try
			{
				TaskLogger.LogEnter();
				if (this.IPGateway != null)
				{
					UMIPGateway umipgateway = (UMIPGateway)this.adResults;
					List<UMServer> compatibleUMRpcServerList = Utility.GetCompatibleUMRpcServerList(null, this.rootOrgConfigSession);
					using (List<UMServer>.Enumerator enumerator = compatibleUMRpcServerList.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							UMServer umserver = enumerator.Current;
							serverName = umserver.Name;
							ActiveCalls[] dataObjects = this.InvokeGetActiveCalls(serverName, null, umipgateway.Address.ToString());
							this.WriteResults(dataObjects);
						}
						goto IL_175;
					}
				}
				if (this.DialPlan != null)
				{
					UMDialPlan umdialPlan = (UMDialPlan)this.adResults;
					List<UMServer> compatibleUMRpcServerList2 = Utility.GetCompatibleUMRpcServerList(umdialPlan, this.rootOrgConfigSession);
					using (List<UMServer>.Enumerator enumerator2 = compatibleUMRpcServerList2.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							UMServer umserver2 = enumerator2.Current;
							serverName = umserver2.Name;
							ActiveCalls[] dataObjects2 = this.InvokeGetActiveCalls(serverName, umdialPlan.Name, null);
							this.WriteResults(dataObjects2);
						}
						goto IL_175;
					}
				}
				if (this.Server != null)
				{
					Server server = (Server)this.adResults;
					serverName = server.Name;
					ActiveCalls[] dataObjects3 = this.InvokeGetActiveCalls(serverName, null, null);
					this.WriteResults(dataObjects3);
				}
				else if (this.InstanceServer != null)
				{
					serverName = this.InstanceServer.Name;
					ActiveCalls[] dataObjects4 = this.InvokeGetActiveCalls(serverName, null, null);
					this.WriteResults(dataObjects4);
				}
				else
				{
					serverName = "localhost";
					ActiveCalls[] dataObjects5 = this.InvokeGetActiveCalls(serverName, null, null);
					this.WriteResults(dataObjects5);
				}
				IL_175:
				TaskLogger.LogExit();
			}
			catch (RpcException e)
			{
				this.WriteRpcError(e, serverName);
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		private ActiveCalls[] InvokeGetActiveCalls(string serverName, string dialPlan, string ipGateway)
		{
			ActiveCalls[] result = null;
			try
			{
				ExTraceGlobals.RpcTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "GetUMActiveCalls: Executing rpc request on server {0}. DialPlan:{1} IPGateway:{2}", serverName, dialPlan, ipGateway);
				using (UMClientRpc umclientRpc = new UMClientRpc(serverName))
				{
					bool isDialPlan = !string.IsNullOrEmpty(dialPlan);
					bool isIpGateway = !string.IsNullOrEmpty(ipGateway);
					result = umclientRpc.GetUmActiveCallList(isDialPlan, dialPlan, isIpGateway, ipGateway);
				}
				ExTraceGlobals.RpcTracer.TraceDebug<string>((long)this.GetHashCode(), "GetUMActiveCalls: Rpc succeeded on server {0}.", serverName);
			}
			catch (RpcException ex)
			{
				ExTraceGlobals.RpcTracer.TraceError<string, int, RpcException>((long)this.GetHashCode(), "GetUMActiveCalls: Rpc failed on server {0}. ErrorCode:{1} CallStack:{2}", serverName, ex.ErrorCode, ex);
				throw;
			}
			return result;
		}

		private void WriteResults(IConfigurable[] dataObjects)
		{
			TaskLogger.LogEnter(dataObjects);
			if (dataObjects != null)
			{
				foreach (IConfigurable sendToPipeline in dataObjects)
				{
					base.WriteObject(sendToPipeline);
				}
			}
			TaskLogger.LogExit();
		}

		private void WriteRpcError(RpcException e, string serverName)
		{
			LocalizedException exception;
			ErrorCategory category;
			if (e.ErrorCode == 1753)
			{
				exception = new LocalizedException(Strings.RpcNotRegistered(serverName), e);
				category = ErrorCategory.ResourceUnavailable;
			}
			else if (e.ErrorCode == 1722)
			{
				exception = new LocalizedException(Strings.RpcUnavailable(serverName), e);
				category = ErrorCategory.InvalidOperation;
			}
			else
			{
				Win32Exception ex = new Win32Exception(e.ErrorCode);
				exception = new LocalizedException(Strings.GenericRPCError(ex.Message, serverName), ex);
				category = ErrorCategory.InvalidOperation;
			}
			base.WriteError(exception, category, null);
		}

		private void ValidateServerIsCompatible(Server target)
		{
			if (!CommonUtil.IsServerCompatible(target))
			{
				base.WriteError(new LocalizedException(Strings.InvalidServerVersionForUMRpcTask(target.Name)), ErrorCategory.InvalidData, target);
			}
		}

		private ITopologyConfigurationSession rootOrgConfigSession;

		private IConfigurable adResults;
	}
}
