using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Directory
{
	internal class ServerComponentStateManager
	{
		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ServerComponentStateManagerTracer;
			}
		}

		private static ServerComponentStateManager Instance
		{
			get
			{
				if (ServerComponentStateManager.sm_instance == null)
				{
					ServerComponentStateManager.sm_instance = new ServerComponentStateManager();
				}
				return ServerComponentStateManager.sm_instance;
			}
		}

		public static string GetComponentId(ServerComponentEnum serverComponent)
		{
			return serverComponent.ToString();
		}

		public static bool IsValidComponent(string componentId)
		{
			ServerComponentEnum serverComponent;
			return Enum.TryParse<ServerComponentEnum>(componentId, true, out serverComponent) && ServerComponentStateManager.IsValidComponent(serverComponent);
		}

		public static bool IsValidComponent(ServerComponentEnum serverComponent)
		{
			return serverComponent != ServerComponentEnum.None && serverComponent != ServerComponentEnum.TestComponent && serverComponent != ServerComponentEnum.ServerWideSettings;
		}

		public static ServiceState GetDefaultState(string componentId)
		{
			ServerComponentEnum serverComponent;
			if (!Enum.TryParse<ServerComponentEnum>(componentId, true, out serverComponent))
			{
				throw new ArgumentException(DirectoryStrings.ComponentNameInvalid);
			}
			return ServerComponentStateManager.GetDefaultState(serverComponent);
		}

		public static ServiceState GetDefaultState(ServerComponentEnum serverComponent)
		{
			ServerComponentStateManager.ComponentStateData componentStateData;
			if (!ServerComponentStateManager.defaultComponentStates.TryGetValue(serverComponent, out componentStateData))
			{
				return ServiceState.Active;
			}
			if (!Datacenter.IsMicrosoftHostedOnly(true))
			{
				return componentStateData.OnPremState;
			}
			return componentStateData.DatacenterState;
		}

		public static bool IsOnline(ServerComponentEnum serverComponent)
		{
			return ServerComponentStateManager.IsOnline(serverComponent, true);
		}

		public static bool IsOnline(ServerComponentEnum serverComponent, bool onlineByDefault)
		{
			ServiceState effectiveState = ServerComponentStateManager.GetEffectiveState(serverComponent, onlineByDefault);
			return effectiveState == ServiceState.Active || effectiveState == ServiceState.Draining;
		}

		public static ServiceState GetEffectiveState(ServerComponentEnum serverComponent)
		{
			ServiceState defaultState = ServerComponentStateManager.GetDefaultState(serverComponent);
			return ServerComponentStateManager.GetEffectiveState(serverComponent, defaultState != ServiceState.Inactive);
		}

		public static ServiceState GetEffectiveState(ServerComponentEnum serverComponent, bool onlineByDefault)
		{
			MultiValuedProperty<string> adStates = null;
			try
			{
				adStates = ServerComponentStateManager.Instance.GetAdStates();
			}
			catch (ServerComponentApiException arg)
			{
				ServerComponentStateManager.Tracer.TraceError<ServerComponentApiException>(0L, "GetEffectiveState ignoring failure to get adStates. Ex was {0}", arg);
			}
			ServiceState compState = ServiceState.Inactive;
			ServerComponentStateManager.RunLocalRegistryOperation(delegate
			{
				compState = ServerComponentStates.ReadEffectiveComponentState(null, adStates, ServerComponentStates.GetComponentId(serverComponent), onlineByDefault ? ServiceState.Active : ServiceState.Inactive);
			});
			return compState;
		}

		public static void SetOffline(ServerComponentEnum serverComponent)
		{
			Exception ex = ServerComponentStateManager.RunLocalRegistryOperationNoThrow(delegate
			{
				ServerComponentStates.UpdateLocalState(null, ServerComponentRequest.HealthApi.ToString(), serverComponent.ToString(), ServiceState.Inactive);
			});
			if (ex != null)
			{
				ServerComponentStateManager.Tracer.TraceError<ServerComponentEnum, Exception>(0L, "SetOffline({0}) failed: {1}", serverComponent, ex);
				throw new ServerComponentApiException(DirectoryStrings.ServerComponentLocalRegistryError(ex.ToString()), ex);
			}
		}

		public static void SetOnline(ServerComponentEnum serverComponent)
		{
			Exception ex = ServerComponentStateManager.RunLocalRegistryOperationNoThrow(delegate
			{
				ServerComponentStates.UpdateLocalState(null, ServerComponentRequest.HealthApi.ToString(), serverComponent.ToString(), ServiceState.Active);
			});
			if (ex != null)
			{
				ServerComponentStateManager.Tracer.TraceError<ServerComponentEnum, Exception>(0L, "SetOnline({0}) failed: {1}", serverComponent, ex);
				throw new ServerComponentApiException(DirectoryStrings.ServerComponentLocalRegistryError(ex.ToString()), ex);
			}
		}

		public static void SyncAdState()
		{
			MultiValuedProperty<string> adStates = null;
			Exception ex = ServerComponentStateManager.RunADOperationNoThrow(delegate
			{
				adStates = ServerComponentStateManager.ReadLocalServerComponentStatesFromAD();
				ServerComponentStateManager.UseAdStates(adStates);
			});
			if (ex != null)
			{
				ServerComponentStateManager.Tracer.TraceError<Exception>(0L, "SyncAdState failed to read from AD: {0}", ex);
				throw new ServerComponentApiException(DirectoryStrings.ServerComponentReadADError(ex.ToString()), ex);
			}
			Exception ex2 = ServerComponentStateManager.RunLocalRegistryOperationNoThrow(delegate
			{
				ServerComponentStates.SyncComponentStates(adStates);
			});
			if (ex2 != null)
			{
				ServerComponentStateManager.Tracer.TraceError<Exception>(0L, "SyncAdState failed in ServerComponentStates.SyncComponentStates: {0}", ex2);
				throw new ServerComponentApiException(DirectoryStrings.ServerComponentLocalRegistryError(ex2.ToString()), ex2);
			}
		}

		internal static void UseAdStates(MultiValuedProperty<string> adStates)
		{
			ServerComponentStateManager.Instance.SetAdStates(adStates);
		}

		public static Exception RunLocalRegistryOperationNoThrow(Action codeToRun)
		{
			Exception result = null;
			try
			{
				codeToRun();
			}
			catch (UnauthorizedAccessException ex)
			{
				result = ex;
			}
			catch (SecurityException ex2)
			{
				result = ex2;
			}
			catch (IOException ex3)
			{
				result = ex3;
			}
			return result;
		}

		private static void RunLocalRegistryOperation(Action codeToRun)
		{
			Exception ex = ServerComponentStateManager.RunLocalRegistryOperationNoThrow(codeToRun);
			if (ex != null)
			{
				throw new ServerComponentApiException(DirectoryStrings.ServerComponentLocalRegistryError(ex.ToString()), ex);
			}
		}

		private static Exception RunADOperationNoThrow(Action codeToRun)
		{
			Exception result = null;
			try
			{
				codeToRun();
			}
			catch (ADExternalException ex)
			{
				result = ex;
			}
			catch (ADOperationException ex2)
			{
				result = ex2;
			}
			catch (ADTransientException ex3)
			{
				result = ex3;
			}
			return result;
		}

		private static MultiValuedProperty<string> ReadLocalServerComponentStatesFromAD()
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 480, "ReadLocalServerComponentStatesFromAD", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\ServerComponentStateManager.cs");
			string machineName = Environment.MachineName;
			Server server = topologyConfigurationSession.FindServerByName(machineName);
			if (server != null)
			{
				return server.ComponentStates;
			}
			topologyConfigurationSession.UseConfigNC = false;
			topologyConfigurationSession.UseGlobalCatalog = true;
			ADComputer adcomputer = topologyConfigurationSession.FindComputerByHostName(machineName);
			if (adcomputer == null)
			{
				throw new LocalServerNotFoundException(machineName);
			}
			return adcomputer.ComponentStates;
		}

		private MultiValuedProperty<string> GetAdStates()
		{
			if (!this.m_lastKnownADComponentStatesAreSet)
			{
				this.LazyInit();
			}
			return this.m_lastKnownADComponentStates;
		}

		private void SetAdStates(MultiValuedProperty<string> adStates)
		{
			this.m_lastKnownADComponentStates = adStates;
			this.m_lastKnownADComponentStatesAreSet = true;
		}

		private void LazyInit()
		{
			int num = 30;
			try
			{
				if (!Monitor.TryEnter(this.syncLock, TimeSpan.FromSeconds((double)num)))
				{
					throw new ServerComponentApiException(DirectoryStrings.ServerComponentReadTimeout(num));
				}
				if (!this.m_lastKnownADComponentStatesAreSet)
				{
					MultiValuedProperty<string> adStates = null;
					Exception ex = ServerComponentStateManager.RunADOperationNoThrow(delegate
					{
						adStates = ServerComponentStateManager.ReadLocalServerComponentStatesFromAD();
					});
					if (ex != null)
					{
						ServerComponentStateManager.Tracer.TraceError<Exception>(0L, "LazyInit failed: {0}", ex);
					}
					this.SetAdStates(adStates);
				}
			}
			finally
			{
				if (Monitor.IsEntered(this.syncLock))
				{
					Monitor.Exit(this.syncLock);
				}
			}
		}

		private const int MaxWaitForLazyInitInSecs = 30;

		private static readonly string serverFqdn = ComputerInformation.DnsFullyQualifiedDomainName;

		private static readonly string ServerWideOfflineComponentId = ServerComponentEnum.ServerWideOffline.ToString();

		private static readonly Dictionary<ServerComponentEnum, ServerComponentStateManager.ComponentStateData> defaultComponentStates = new Dictionary<ServerComponentEnum, ServerComponentStateManager.ComponentStateData>
		{
			{
				ServerComponentEnum.HubTransport,
				new ServerComponentStateManager.ComponentStateData
				{
					OnPremState = ServiceState.Active,
					DatacenterState = ServiceState.Inactive
				}
			},
			{
				ServerComponentEnum.FrontendTransport,
				new ServerComponentStateManager.ComponentStateData
				{
					OnPremState = ServiceState.Active,
					DatacenterState = ServiceState.Inactive
				}
			},
			{
				ServerComponentEnum.ForwardSyncDaemon,
				new ServerComponentStateManager.ComponentStateData
				{
					OnPremState = ServiceState.Inactive,
					DatacenterState = ServiceState.Inactive
				}
			},
			{
				ServerComponentEnum.ProvisioningRps,
				new ServerComponentStateManager.ComponentStateData
				{
					OnPremState = ServiceState.Inactive,
					DatacenterState = ServiceState.Inactive
				}
			}
		};

		private static ServerComponentStateManager sm_instance;

		private bool m_lastKnownADComponentStatesAreSet;

		private MultiValuedProperty<string> m_lastKnownADComponentStates;

		private object syncLock = new object();

		private struct ComponentStateData
		{
			public ServiceState OnPremState { get; set; }

			public ServiceState DatacenterState { get; set; }
		}
	}
}
