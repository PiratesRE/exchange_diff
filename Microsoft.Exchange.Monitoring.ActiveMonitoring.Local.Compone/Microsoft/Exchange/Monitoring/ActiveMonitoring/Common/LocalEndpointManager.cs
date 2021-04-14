using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class LocalEndpointManager
	{
		private LocalEndpointManager()
		{
			CertificateValidationManager.RegisterCallback("DefaultAMComponent", new RemoteCertificateValidationCallback(LocalEndpointManager.ValidateRemoteCertificate));
			if (!LocalEndpointManager.UseMaintenanceWorkItem)
			{
				foreach (Type type in this.endpointDiscriptors.Keys)
				{
					this.TryInitializeEndpoint(type);
				}
			}
		}

		public static LocalEndpointManager Instance
		{
			get
			{
				if (LocalEndpointManager.instance == null)
				{
					lock (LocalEndpointManager.locker)
					{
						if (LocalEndpointManager.instance == null)
						{
							LocalEndpointManager.instance = new LocalEndpointManager();
						}
					}
				}
				return LocalEndpointManager.instance;
			}
		}

		public static bool UseMaintenanceWorkItem
		{
			get
			{
				return LocalEndpointManager.useMaintenanceWorkItem;
			}
			set
			{
				LocalEndpointManager.useMaintenanceWorkItem = value;
			}
		}

		public static bool IsDataCenter
		{
			get
			{
				return LocalEndpointManager.isDataCenter;
			}
		}

		public static bool IsDataCenterDedicated
		{
			get
			{
				return LocalEndpointManager.isDataCenterDedicated;
			}
		}

		public IEnumerable<MaintenanceDefinition> EndpointWorkitems
		{
			get
			{
				return this.endpointDiscriptors.Values;
			}
		}

		public MailboxDatabaseEndpoint MailboxDatabaseEndpoint
		{
			get
			{
				return this.GetEndpoint(typeof(MailboxDatabaseEndpoint), false) as MailboxDatabaseEndpoint;
			}
		}

		public ExchangeServerRoleEndpoint ExchangeServerRoleEndpoint
		{
			get
			{
				return this.GetEndpoint(typeof(ExchangeServerRoleEndpoint), false) as ExchangeServerRoleEndpoint;
			}
		}

		public WindowsServerRoleEndpoint WindowsServerRoleEndpoint
		{
			get
			{
				return this.GetEndpoint(typeof(WindowsServerRoleEndpoint), false) as WindowsServerRoleEndpoint;
			}
		}

		public UnifiedMessagingCallRouterEndpoint UnifiedMessagingCallRouterEndpoint
		{
			get
			{
				return this.GetEndpoint(typeof(UnifiedMessagingCallRouterEndpoint), false) as UnifiedMessagingCallRouterEndpoint;
			}
		}

		public UnifiedMessagingServiceEndpoint UnifiedMessagingServiceEndpoint
		{
			get
			{
				return this.GetEndpoint(typeof(UnifiedMessagingServiceEndpoint), false) as UnifiedMessagingServiceEndpoint;
			}
		}

		public OfflineAddressBookEndpoint OfflineAddressBookEndpoint
		{
			get
			{
				return this.GetEndpoint(typeof(OfflineAddressBookEndpoint), false) as OfflineAddressBookEndpoint;
			}
		}

		public SubjectListEndpoint SubjectListEndpoint
		{
			get
			{
				return this.GetEndpoint(typeof(SubjectListEndpoint), false) as SubjectListEndpoint;
			}
		}

		public MonitoringEndpoint MonitoringEndpoint
		{
			get
			{
				return this.GetEndpoint(typeof(MonitoringEndpoint), false) as MonitoringEndpoint;
			}
		}

		public RecoveryActionsEnabledEndpoint RecoveryActionsEnabledEndpoint
		{
			get
			{
				return this.GetEndpoint(typeof(RecoveryActionsEnabledEndpoint), false) as RecoveryActionsEnabledEndpoint;
			}
		}

		public ScopeMappingLocalEndpoint ScopeMappingLocalEndpoint
		{
			get
			{
				return this.GetEndpoint(typeof(ScopeMappingLocalEndpoint), false) as ScopeMappingLocalEndpoint;
			}
		}

		public bool RestartThrottlingAllowed
		{
			get
			{
				return this.restartThrottleAllowed;
			}
			set
			{
				this.restartThrottleAllowed = value;
			}
		}

		public void SetEndpoint(Type type, IEndpoint endpoint, bool validate = true)
		{
			if (validate && !this.endpointDiscriptors.ContainsKey(type))
			{
				throw new ArgumentException("Invalid endpoint type: " + type.Name);
			}
			this.endpoints[type] = endpoint;
			if (this.endpoints.Count == this.endpointDiscriptors.Count)
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.LocalEndpointManagerTracer, this.traceContext, "LocalEndpointManager.SetEndpoint: Signal startup notification {0}", LocalDataAccess.EndpointManagerNotificationId, null, "SetEndpoint", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\LocalEndpointManager.cs", 482);
				StartupNotification.InsertStartupNotification(LocalDataAccess.EndpointManagerNotificationId);
			}
		}

		public IEndpoint GetEndpoint(Type type, bool throwIfEndpointContainsException = false)
		{
			IEndpoint endpoint;
			if (!this.endpoints.TryGetValue(type, out endpoint))
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.LocalEndpointManagerTracer, this.traceContext, "Cannot find endpoint type '{0}' in this.endpoints.", type.FullName, null, "GetEndpoint", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\LocalEndpointManager.cs", 500);
				throw new EndpointManagerEndpointUninitializedException(Strings.EndpointManagerEndpointUninitialized(type.FullName));
			}
			if (endpoint != null && endpoint.Exception != null)
			{
				WTFDiagnostics.TraceInformation<string, bool, string>(ExTraceGlobals.LocalEndpointManagerTracer, this.traceContext, "Endpoint type '{0}' contains an exception (throwIfEndpointContainsException={1}): {2}", type.FullName, throwIfEndpointContainsException, endpoint.Exception.ToString(), null, "GetEndpoint", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\LocalEndpointManager.cs", 508);
				if (throwIfEndpointContainsException)
				{
					throw endpoint.Exception;
				}
			}
			return endpoint;
		}

		public bool IsEndpointInitialized(Type type)
		{
			return this.endpoints.ContainsKey(type);
		}

		private static bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
		{
			return true;
		}

		private void TryInitializeEndpoint(Type type)
		{
			WTFDiagnostics.TraceFunction<string>(ExTraceGlobals.LocalEndpointManagerTracer, this.traceContext, "Initializing monitoring endpoint: {0}", type.FullName, null, "TryInitializeEndpoint", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\LocalEndpointManager.cs", 562);
			this.endpoints[type] = null;
			try
			{
				IEndpoint endpoint = (IEndpoint)Activator.CreateInstance(type);
				endpoint.Initialize();
				this.endpoints[type] = endpoint;
			}
			catch (Exception arg)
			{
				WTFDiagnostics.TraceError<string, Exception>(ExTraceGlobals.LocalEndpointManagerTracer, this.traceContext, "Monitoring endpoint {0} initialization failed with exception: {1}", type.FullName, arg, null, "TryInitializeEndpoint", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\LocalEndpointManager.cs", 579);
			}
		}

		private static readonly bool isDataCenter = Datacenter.IsMicrosoftHostedOnly(true);

		private static readonly bool isDataCenterDedicated = Datacenter.IsDatacenterDedicated(true);

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private readonly Dictionary<Type, MaintenanceDefinition> endpointDiscriptors = new Dictionary<Type, MaintenanceDefinition>
		{
			{
				typeof(MailboxDatabaseEndpoint),
				new MaintenanceDefinition
				{
					AssemblyPath = LocalEndpointManager.AssemblyPath,
					TypeName = typeof(EndpointMaintenance<MailboxDatabaseEndpoint>).FullName,
					Name = EndpointMaintenance<MailboxDatabaseEndpoint>.GetDefaultName(),
					ServiceName = ExchangeComponent.Monitoring.Name,
					RecurrenceIntervalSeconds = 900,
					TimeoutSeconds = 900,
					MaxRestartRequestAllowedPerHour = 2,
					Enabled = true
				}
			},
			{
				typeof(ExchangeServerRoleEndpoint),
				new MaintenanceDefinition
				{
					AssemblyPath = LocalEndpointManager.AssemblyPath,
					TypeName = typeof(EndpointMaintenance<ExchangeServerRoleEndpoint>).FullName,
					Name = EndpointMaintenance<ExchangeServerRoleEndpoint>.GetDefaultName(),
					ServiceName = ExchangeComponent.Monitoring.Name,
					RecurrenceIntervalSeconds = 0,
					TimeoutSeconds = 30,
					Enabled = true
				}
			},
			{
				typeof(WindowsServerRoleEndpoint),
				new MaintenanceDefinition
				{
					AssemblyPath = LocalEndpointManager.AssemblyPath,
					TypeName = typeof(EndpointMaintenance<WindowsServerRoleEndpoint>).FullName,
					Name = EndpointMaintenance<WindowsServerRoleEndpoint>.GetDefaultName(),
					ServiceName = ExchangeComponent.Monitoring.Name,
					RecurrenceIntervalSeconds = 0,
					TimeoutSeconds = 120,
					Enabled = true
				}
			},
			{
				typeof(UnifiedMessagingCallRouterEndpoint),
				new MaintenanceDefinition
				{
					AssemblyPath = LocalEndpointManager.AssemblyPath,
					TypeName = typeof(EndpointMaintenance<UnifiedMessagingCallRouterEndpoint>).FullName,
					Name = EndpointMaintenance<UnifiedMessagingCallRouterEndpoint>.GetDefaultName(),
					ServiceName = ExchangeComponent.UMCallRouter.Name,
					RecurrenceIntervalSeconds = 600,
					TimeoutSeconds = 300,
					MaxRestartRequestAllowedPerHour = 2,
					Enabled = true
				}
			},
			{
				typeof(UnifiedMessagingServiceEndpoint),
				new MaintenanceDefinition
				{
					AssemblyPath = LocalEndpointManager.AssemblyPath,
					TypeName = typeof(EndpointMaintenance<UnifiedMessagingServiceEndpoint>).FullName,
					Name = EndpointMaintenance<UnifiedMessagingServiceEndpoint>.GetDefaultName(),
					ServiceName = ExchangeComponent.UMProtocol.Name,
					RecurrenceIntervalSeconds = 600,
					TimeoutSeconds = 300,
					MaxRestartRequestAllowedPerHour = 2,
					Enabled = true
				}
			},
			{
				typeof(OfflineAddressBookEndpoint),
				new MaintenanceDefinition
				{
					AssemblyPath = LocalEndpointManager.AssemblyPath,
					TypeName = typeof(EndpointMaintenance<OfflineAddressBookEndpoint>).FullName,
					Name = EndpointMaintenance<OfflineAddressBookEndpoint>.GetDefaultName(),
					ServiceName = ExchangeComponent.Oab.Name,
					RecurrenceIntervalSeconds = 600,
					TimeoutSeconds = 300,
					MaxRestartRequestAllowedPerHour = 2,
					Enabled = true
				}
			},
			{
				typeof(SubjectListEndpoint),
				new MaintenanceDefinition
				{
					AssemblyPath = LocalEndpointManager.AssemblyPath,
					TypeName = typeof(EndpointMaintenance<SubjectListEndpoint>).FullName,
					Name = EndpointMaintenance<SubjectListEndpoint>.GetDefaultName(),
					ServiceName = ExchangeComponent.Monitoring.Name,
					RecurrenceIntervalSeconds = 900,
					TimeoutSeconds = 150,
					MaxRestartRequestAllowedPerHour = 2,
					Enabled = true
				}
			},
			{
				typeof(MonitoringEndpoint),
				new MaintenanceDefinition
				{
					AssemblyPath = LocalEndpointManager.AssemblyPath,
					TypeName = typeof(EndpointMaintenance<MonitoringEndpoint>).FullName,
					Name = EndpointMaintenance<MonitoringEndpoint>.GetDefaultName(),
					ServiceName = ExchangeComponent.Monitoring.Name,
					RecurrenceIntervalSeconds = 300,
					TimeoutSeconds = 150,
					MaxRestartRequestAllowedPerHour = 2,
					Enabled = true
				}
			},
			{
				typeof(RecoveryActionsEnabledEndpoint),
				new MaintenanceDefinition
				{
					AssemblyPath = LocalEndpointManager.AssemblyPath,
					TypeName = typeof(EndpointMaintenance<RecoveryActionsEnabledEndpoint>).FullName,
					Name = EndpointMaintenance<RecoveryActionsEnabledEndpoint>.GetDefaultName(),
					ServiceName = ExchangeComponent.Monitoring.Name,
					RecurrenceIntervalSeconds = 300,
					TimeoutSeconds = 150,
					MaxRestartRequestAllowedPerHour = 2,
					Enabled = true
				}
			},
			{
				typeof(OverrideEndpoint),
				new MaintenanceDefinition
				{
					AssemblyPath = LocalEndpointManager.AssemblyPath,
					TypeName = typeof(EndpointMaintenance<OverrideEndpoint>).FullName,
					Name = EndpointMaintenance<OverrideEndpoint>.GetDefaultName(),
					ServiceName = ExchangeComponent.Monitoring.Name,
					RecurrenceIntervalSeconds = 300,
					TimeoutSeconds = 150,
					MaxRestartRequestAllowedPerHour = 3,
					Enabled = true
				}
			},
			{
				typeof(ScopeMappingLocalEndpoint),
				new MaintenanceDefinition
				{
					AssemblyPath = LocalEndpointManager.AssemblyPath,
					TypeName = typeof(EndpointMaintenance<ScopeMappingLocalEndpoint>).FullName,
					Name = EndpointMaintenance<ScopeMappingLocalEndpoint>.GetDefaultName(),
					ServiceName = ExchangeComponent.Monitoring.Name,
					RecurrenceIntervalSeconds = 0,
					TimeoutSeconds = 300,
					MaxRestartRequestAllowedPerHour = 3,
					Enabled = true
				}
			}
		};

		private static bool useMaintenanceWorkItem = false;

		private static LocalEndpointManager instance = null;

		private static object locker = new object();

		private ConcurrentDictionary<Type, IEndpoint> endpoints = new ConcurrentDictionary<Type, IEndpoint>();

		private bool restartThrottleAllowed = true;

		private TracingContext traceContext = TracingContext.Default;
	}
}
