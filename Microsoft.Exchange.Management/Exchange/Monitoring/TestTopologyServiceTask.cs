using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management.Automation;
using System.ServiceModel;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.TopologyDiscovery;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[Cmdlet("Test", "TopologyService", SupportsShouldProcess = true)]
	public sealed class TestTopologyServiceTask : Task
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter MonitoringContext
		{
			get
			{
				return (SwitchParameter)(base.Fields["MonitoringContext"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["MonitoringContext"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Default")]
		public ServerIdParameter Server
		{
			get
			{
				return ((ServerIdParameter)base.Fields["Server"]) ?? new ServerIdParameter();
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string TargetDomainController
		{
			get
			{
				return (string)base.Fields["TargetDomainController"];
			}
			set
			{
				base.Fields["TargetDomainController"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public TopologyServiceServerRole ADServerRole
		{
			get
			{
				return (TopologyServiceServerRole)(base.Fields["ADServerRole"] ?? TopologyServiceServerRole.GlobalCatalog);
			}
			set
			{
				base.Fields["ADServerRole"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public TopologyServiceOperationTypeEnum OperationType
		{
			get
			{
				return (TopologyServiceOperationTypeEnum)(base.Fields["OperationType"] ?? TopologyServiceOperationTypeEnum.Test);
			}
			set
			{
				base.Fields["OperationType"] = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		public string PartitionFqdn
		{
			get
			{
				return (string)base.Fields["PartitionFqdn"];
			}
			set
			{
				base.Fields["PartitionFqdn"] = value;
			}
		}

		internal IConfigurationSession SystemConfigurationSession
		{
			get
			{
				if (this.systemConfigurationSession == null)
				{
					this.systemConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 266, "SystemConfigurationSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Monitoring\\ActiveDirectory\\TestTopologyServiceTask.cs");
				}
				return this.systemConfigurationSession;
			}
		}

		protected override bool IsKnownException(Exception e)
		{
			return base.IsKnownException(e) || MonitoringHelper.IsKnownExceptionForMonitoring(e);
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				base.InternalValidate();
				if (!base.HasErrors)
				{
					this.ServerObject = TestTopologyServiceTask.EnsureSingleObject<Server>(() => this.Server.GetObjects<Server>(null, this.SystemConfigurationSession));
					if (this.ServerObject == null)
					{
						throw new CasHealthMailboxServerNotFoundException(this.Server.ToString());
					}
					if (this.PartitionFqdn == null)
					{
						PartitionId[] array = Globals.IsDatacenter ? ADAccountPartitionLocator.GetAllAccountPartitionIds() : new PartitionId[]
						{
							PartitionId.LocalForest
						};
						this.PartitionFqdn = array[new Random().Next(0, array.Length)].ForestFQDN;
					}
					if (this.TargetDomainController == null && (this.OperationType == TopologyServiceOperationTypeEnum.ReportServerDown || this.OperationType == TopologyServiceOperationTypeEnum.SetConfigDC))
					{
						throw new TopologyServiceMissingDC(this.OperationType.ToString());
					}
				}
			}
			catch (LocalizedException exception)
			{
				this.WriteError(exception, ErrorCategory.OperationStopped, this, true);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override void InternalBeginProcessing()
		{
			if (this.MonitoringContext)
			{
				this.monitoringData = new MonitoringData();
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalBeginProcessing();
			TaskLogger.LogEnter();
			try
			{
				TopologyServiceOutcome sendToPipeline = new TopologyServiceOutcome(this.Server.ToString(), this.OperationType.ToString());
				this.PerformTopologyServiceTest(ref sendToPipeline, this.OperationType);
				base.WriteObject(sendToPipeline);
			}
			catch (LocalizedException e)
			{
				this.HandleException(e);
			}
			finally
			{
				if (this.MonitoringContext)
				{
					base.WriteObject(this.monitoringData);
				}
				TaskLogger.LogExit();
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageTestTopologyServiceIdentity(this.OperationType.ToString());
			}
		}

		private void PerformTopologyServiceTest(ref TopologyServiceOutcome result, TopologyServiceOperationTypeEnum operationType)
		{
			bool flag = true;
			using (TopologyServiceClient topologyServiceClient = TopologyServiceClient.CreateClient(this.Server.ToString()))
			{
				string error = string.Empty;
				StringBuilder stringBuilder = new StringBuilder();
				TopologyServiceError topologyServiceError = TopologyServiceError.None;
				Stopwatch stopwatch = Stopwatch.StartNew();
				try
				{
					base.WriteVerbose(Strings.TopologyServiceOperation(operationType.ToString()));
					IList<TopologyVersion> list;
					IList<ServerInfo> serversForRole;
					switch (operationType)
					{
					case TopologyServiceOperationTypeEnum.GetAllTopologyVersions:
						list = topologyServiceClient.GetAllTopologyVersions();
						using (IEnumerator<TopologyVersion> enumerator = list.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								TopologyVersion topologyVersion = enumerator.Current;
								stringBuilder.Append(Strings.ToplogyserviceTopologyVersion(topologyVersion.PartitionFqdn, topologyVersion.Version) + Environment.NewLine);
							}
							goto IL_264;
						}
						break;
					case TopologyServiceOperationTypeEnum.GetTopologyVersion:
						break;
					case TopologyServiceOperationTypeEnum.SetConfigDC:
						topologyServiceClient.SetConfigDC(this.PartitionFqdn, this.TargetDomainController);
						goto IL_264;
					case TopologyServiceOperationTypeEnum.ReportServerDown:
						goto IL_13A;
					case TopologyServiceOperationTypeEnum.GetServersForRole:
						serversForRole = topologyServiceClient.GetServersForRole(this.PartitionFqdn, new List<string>(), (ADServerRole)this.ADServerRole, 20, false);
						using (IEnumerator<ServerInfo> enumerator2 = serversForRole.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								ServerInfo serverInfo = enumerator2.Current;
								stringBuilder.Append(Strings.TopologyServiceADServerInfo(serverInfo.Fqdn) + Environment.NewLine);
							}
							goto IL_264;
						}
						goto IL_1DA;
					case TopologyServiceOperationTypeEnum.Test:
						goto IL_1DA;
					default:
						goto IL_264;
					}
					list = topologyServiceClient.GetTopologyVersions(new List<string>
					{
						this.PartitionFqdn
					});
					using (IEnumerator<TopologyVersion> enumerator3 = list.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							TopologyVersion topologyVersion2 = enumerator3.Current;
							stringBuilder.Append(Strings.ToplogyserviceTopologyVersion(topologyVersion2.PartitionFqdn, topologyVersion2.Version) + Environment.NewLine);
						}
						goto IL_264;
					}
					IL_13A:
					topologyServiceClient.ReportServerDown(this.PartitionFqdn, this.TargetDomainController, (ADServerRole)this.ADServerRole);
					goto IL_264;
					IL_1DA:
					serversForRole = topologyServiceClient.GetServersForRole(this.PartitionFqdn, new List<string>(), (ADServerRole)this.ADServerRole, 20, false);
					foreach (ServerInfo serverInfo2 in serversForRole)
					{
						stringBuilder.Append(Strings.TopologyServiceADServerInfo(serverInfo2.Fqdn) + Environment.NewLine);
					}
					if (serversForRole.Count > 0)
					{
						flag = true;
					}
					else
					{
						flag = false;
						error = Strings.TopologyServiceNoServersReturned(this.PartitionFqdn);
					}
					IL_264:;
				}
				catch (CommunicationException ex)
				{
					flag = false;
					error = ex.Message;
					topologyServiceError = TopologyServiceError.CommunicationException;
				}
				catch (Exception ex2)
				{
					flag = false;
					error = ex2.Message;
					topologyServiceError = TopologyServiceError.OtherException;
				}
				stopwatch.Stop();
				result.Update(flag ? TopologyServiceResultEnum.Success : TopologyServiceResultEnum.Failure, stopwatch.Elapsed, error, stringBuilder.ToString());
				if (this.MonitoringContext)
				{
					this.monitoringData.Events.Add(new MonitoringEvent(TestTopologyServiceTask.CmdletMonitoringEventSource, (int)((flag ? 1000 : 2000) + this.OperationType), flag ? EventTypeEnumeration.Success : EventTypeEnumeration.Error, flag ? Strings.TopologyServiceSuccess(this.OperationType.ToString()) : (Strings.TopologyServiceFailed(this.OperationType.ToString(), error) + " " + topologyServiceError)));
				}
			}
		}

		private bool IsExplicitlySet(string param)
		{
			return base.Fields.Contains(param);
		}

		private TimeSpan TotalLatency { get; set; }

		private Server ServerObject { get; set; }

		internal static T EnsureSingleObject<T>(Func<IEnumerable<T>> getObjects) where T : class
		{
			T t = default(T);
			foreach (T t2 in getObjects())
			{
				if (t != null)
				{
					throw new DataValidationException(new ObjectValidationError(Strings.MoreThanOneObjects(typeof(T).ToString()), null, null));
				}
				t = t2;
			}
			return t;
		}

		private void HandleException(LocalizedException e)
		{
			if (!this.MonitoringContext)
			{
				this.WriteError(e, ErrorCategory.OperationStopped, this, true);
				return;
			}
			this.monitoringData.Events.Add(new MonitoringEvent(TestTopologyServiceTask.CmdletMonitoringEventSource, 3006, EventTypeEnumeration.Error, Strings.LiveIdConnectivityExceptionThrown(e.ToString())));
		}

		private const string ServerParam = "Server";

		private const string PartitionFqdnParam = "PartitionFqdn";

		private const string MonitoringContextParam = "MonitoringContext";

		private const string OperationParam = "OperationType";

		private const string TargetDomainControllerParam = "TargetDomainController";

		private const string ADServerRoleParam = "ADServerRole";

		private const int FailedEventIdBase = 2000;

		private const int SuccessEventIdBase = 1000;

		internal const string TopologyService = "TopologyService";

		private MonitoringData monitoringData;

		private IConfigurationSession systemConfigurationSession;

		public static readonly string CmdletMonitoringEventSource = "MSExchange Monitoring TopologyService";

		public static readonly string PerformanceCounter = "TopologyService Latency";

		public enum ScenarioId
		{
			PlaceHolderNoException = 1006,
			ExceptionThrown = 3006,
			AllTransactionsSucceeded = 3001
		}
	}
}
