using System;
using System.Globalization;
using System.Net.Security;
using System.ServiceModel;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.DiagnosticsAggregation;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.DiagnosticsAggregation;
using Microsoft.Exchange.ServiceHost;
using Microsoft.Exchange.Servicelets.DiagnosticsAggregation.Messages;
using Microsoft.Exchange.Transport.DiagnosticsAggregationService;

namespace Microsoft.Exchange.Servicelets.DiagnosticsAggregation
{
	public class DiagnosticsAggregationServicelet : Servicelet
	{
		internal static ExEventLog EventLog
		{
			get
			{
				return DiagnosticsAggregationServicelet.eventLog;
			}
		}

		internal static DiagnosticsAggregationLog Log
		{
			get
			{
				return DiagnosticsAggregationServicelet.log;
			}
		}

		internal static Server LocalServer
		{
			get
			{
				if (DiagnosticsAggregationServicelet.localServer == null)
				{
					throw new InvalidOperationException("LocalServer cannot be accessed before setting it");
				}
				return DiagnosticsAggregationServicelet.localServer;
			}
		}

		internal static DiagnosticsAggregationServiceletConfig Config
		{
			get
			{
				return DiagnosticsAggregationServicelet.config;
			}
		}

		internal static TransportConfigContainer TransportSettings
		{
			get
			{
				if (DiagnosticsAggregationServicelet.transportSettings == null)
				{
					throw new InvalidOperationException("TransportSettings cannot be accessed before initializing it");
				}
				return DiagnosticsAggregationServicelet.transportSettings;
			}
		}

		public override void Work()
		{
			ExTraceGlobals.DiagnosticsAggregationTracer.TraceDebug(0L, "Starting DiagnosticsAggregationServicelet.");
			if (DiagnosticsAggregationServicelet.config == null)
			{
				DiagnosticsAggregationServicelet.config = new DiagnosticsAggregationServiceletConfig();
			}
			if (DiagnosticsAggregationServicelet.config.Enabled)
			{
				this.HostService();
				return;
			}
			ExTraceGlobals.DiagnosticsAggregationTracer.TraceWarning(0L, "DiagnosticsAggregationServicelet is not enabled.");
			DiagnosticsAggregationServicelet.EventLog.LogEvent(MSExchangeDiagnosticsAggregationEventLogConstants.Tuple_DiagnosticsAggregationServiceletIsDisabled, null, new object[0]);
		}

		internal static NetTcpBinding GetTcpBinding()
		{
			return new NetTcpBinding
			{
				MaxReceivedMessageSize = (long)((int)ByteQuantifiedSize.FromMB(10UL).ToBytes()),
				Security = 
				{
					Transport = 
					{
						ProtectionLevel = ProtectionLevel.EncryptAndSign,
						ClientCredentialType = TcpClientCredentialType.Windows
					},
					Message = 
					{
						ClientCredentialType = MessageCredentialType.Windows
					}
				}
			};
		}

		internal static LocalQueuesDataProvider GetLocalQueuesDataProvider()
		{
			if (DiagnosticsAggregationServicelet.localQueuesDataProvider == null)
			{
				throw new InvalidOperationException("localQueuesDataProvider has not been instantiated yet");
			}
			return DiagnosticsAggregationServicelet.localQueuesDataProvider;
		}

		internal static GroupQueuesDataProvider GetGroupQueuesDataProvider()
		{
			if (DiagnosticsAggregationServicelet.groupQueuesDataProvider == null)
			{
				throw new InvalidOperationException("groupQueuesDataProvider has not been instantiated yet");
			}
			return DiagnosticsAggregationServicelet.groupQueuesDataProvider;
		}

		private static void OnLocalServerChanged(ADNotificationEventArgs args)
		{
			ExTraceGlobals.DiagnosticsAggregationTracer.TraceDebug(0L, "LocalServer changed");
			DiagnosticsAggregationServicelet.Log.Log(DiagnosticsAggregationEvent.Information, "LocalServer changed", new object[0]);
			DiagnosticsAggregationServicelet.GetLocalServer();
		}

		private static ADOperationResult GetTransportSettings()
		{
			return ADNotificationAdapter.TryRunADOperation(delegate()
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 219, "GetTransportSettings", "f:\\15.00.1497\\sources\\dev\\Management\\src\\ServiceHost\\Servicelets\\DiagnosticsAggregation\\Program\\DiagnosticsAggregationServicelet.cs");
				TransportConfigContainer transportConfigContainer = topologyConfigurationSession.FindSingletonConfigurationObject<TransportConfigContainer>();
				if (transportConfigContainer != null)
				{
					DiagnosticsAggregationServicelet.transportSettings = transportConfigContainer;
					return;
				}
				throw new TenantTransportSettingsNotFoundException("First Org");
			});
		}

		private static ADOperationResult GetLocalServer()
		{
			return ADNotificationAdapter.TryRunADOperation(delegate()
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 246, "GetLocalServer", "f:\\15.00.1497\\sources\\dev\\Management\\src\\ServiceHost\\Servicelets\\DiagnosticsAggregation\\Program\\DiagnosticsAggregationServicelet.cs");
				DiagnosticsAggregationServicelet.localServer = topologyConfigurationSession.FindLocalServer();
			});
		}

		private static bool TryReadAdObjects()
		{
			ADOperationResult adoperationResult = DiagnosticsAggregationServicelet.GetLocalServer();
			if (!adoperationResult.Succeeded)
			{
				Exception exception = adoperationResult.Exception;
				DiagnosticsAggregationServicelet.EventLog.LogEvent(MSExchangeDiagnosticsAggregationEventLogConstants.Tuple_DiagnosticsAggregationServiceletLoadFailed, null, new object[]
				{
					exception
				});
				DiagnosticsAggregationServicelet.Log.Log(DiagnosticsAggregationEvent.ServiceletError, "Getting Local server failed. Details {0}", new object[]
				{
					exception
				});
				ExTraceGlobals.DiagnosticsAggregationTracer.TraceError<Exception>(0L, "Encountered an error while getting local server object. Details {0}.", exception);
				return false;
			}
			ADOperationResult adoperationResult2 = DiagnosticsAggregationServicelet.GetTransportSettings();
			if (!adoperationResult2.Succeeded)
			{
				Exception exception2 = adoperationResult2.Exception;
				DiagnosticsAggregationServicelet.EventLog.LogEvent(MSExchangeDiagnosticsAggregationEventLogConstants.Tuple_DiagnosticsAggregationServiceletLoadFailed, null, new object[]
				{
					exception2
				});
				DiagnosticsAggregationServicelet.Log.Log(DiagnosticsAggregationEvent.ServiceletError, "Getting transportsettings failed. Details {0}", new object[]
				{
					exception2
				});
				ExTraceGlobals.DiagnosticsAggregationTracer.TraceError<Exception>(0L, "Encountered an error while getting TransportSettings configuration. Details {0}.", exception2);
				return false;
			}
			return true;
		}

		private void HostService()
		{
			ExTraceGlobals.DiagnosticsAggregationTracer.TraceDebug(0L, "DiagnosticsAggregationServicelet HostService started.");
			DiagnosticsAggregationServicelet.log.Start(DiagnosticsAggregationServicelet.config);
			if (!DiagnosticsAggregationServicelet.TryReadAdObjects())
			{
				return;
			}
			ServiceHost serviceHost = null;
			try
			{
				this.RegisterForChangeNotifications();
				if (!this.TryHostDiagnosticsWebService(false, out serviceHost))
				{
					return;
				}
				this.StartPeriodicDagAggregation();
				while (!base.StopEvent.WaitOne(TimeSpan.FromMinutes(1.0)))
				{
					if (DiagnosticsAggregationServicelet.transportSettingsChanged)
					{
						DiagnosticsAggregationServicelet.transportSettingsChanged = false;
						TransportConfigContainer transportConfigContainer = DiagnosticsAggregationServicelet.TransportSettings;
						if (DiagnosticsAggregationServicelet.GetTransportSettings() != ADOperationResult.Success)
						{
							DiagnosticsAggregationServicelet.Log.Log(DiagnosticsAggregationEvent.ServiceletError, "Fetching transport settings failed", new object[0]);
						}
						else if (transportConfigContainer.DiagnosticsAggregationServicePort != DiagnosticsAggregationServicelet.TransportSettings.DiagnosticsAggregationServicePort)
						{
							DiagnosticsAggregationServicelet.Log.Log(DiagnosticsAggregationEvent.Information, "Webservice port is changed from {0} to {1}. Hosting the webservice with the new bindings.", new object[]
							{
								transportConfigContainer.DiagnosticsAggregationServicePort,
								DiagnosticsAggregationServicelet.TransportSettings.DiagnosticsAggregationServicePort
							});
							ServiceHost serviceHost2 = null;
							if (this.TryHostDiagnosticsWebService(true, out serviceHost))
							{
								ServiceHost client = serviceHost;
								serviceHost = serviceHost2;
								WcfUtils.DisposeWcfClientGracefully(client, false);
							}
							else
							{
								DiagnosticsAggregationServicelet.Log.Log(DiagnosticsAggregationEvent.ServiceletError, "Hosting the webservice with new bindings did not succeed.", new object[0]);
							}
						}
						else
						{
							DiagnosticsAggregationServicelet.Log.Log(DiagnosticsAggregationEvent.Information, "Webservice port did not change.", new object[0]);
						}
					}
				}
			}
			finally
			{
				if (DiagnosticsAggregationServicelet.groupQueuesDataProvider != null)
				{
					DiagnosticsAggregationServicelet.groupQueuesDataProvider.Stop();
				}
				if (DiagnosticsAggregationServicelet.localQueuesDataProvider != null)
				{
					DiagnosticsAggregationServicelet.localQueuesDataProvider.Stop();
				}
				WcfUtils.DisposeWcfClientGracefully(serviceHost, false);
				this.UnregisterADNotifications();
				DiagnosticsAggregationServicelet.log.Stop();
			}
			ExTraceGlobals.DiagnosticsAggregationTracer.TraceDebug(0L, "HostService Stopped.");
		}

		private void StartPeriodicDagAggregation()
		{
			DiagnosticsAggregationServicelet.localQueuesDataProvider = new LocalQueuesDataProvider(DiagnosticsAggregationServicelet.log, DiagnosticsAggregationServicelet.LocalServer.Id);
			DiagnosticsAggregationServicelet.groupQueuesDataProvider = new GroupQueuesDataProvider(DiagnosticsAggregationServicelet.log);
			DiagnosticsAggregationServicelet.localQueuesDataProvider.Start();
			DiagnosticsAggregationServicelet.groupQueuesDataProvider.Start();
		}

		private bool TryHostDiagnosticsWebService(bool rehosting, out ServiceHost host)
		{
			host = null;
			Exception ex = null;
			int diagnosticsAggregationServicePort = DiagnosticsAggregationServicelet.TransportSettings.DiagnosticsAggregationServicePort;
			try
			{
				host = new ServiceHost(typeof(DiagnosticsAggregationServiceImpl), new Uri[0]);
				string text = string.Format(CultureInfo.InvariantCulture, DiagnosticsAggregationHelper.DiagnosticsAggregationEndpointFormat, new object[]
				{
					"localhost",
					diagnosticsAggregationServicePort
				});
				host.AddServiceEndpoint(typeof(IDiagnosticsAggregationService), DiagnosticsAggregationServicelet.GetTcpBinding(), text);
				this.AddMetadataEndpointInDebugBuild(host);
				host.Open();
				DiagnosticsAggregationServicelet.Log.Log(DiagnosticsAggregationEvent.Information, string.Format(CultureInfo.InvariantCulture, "listening at {0}", new object[]
				{
					text
				}), new object[0]);
			}
			catch (InvalidOperationException ex2)
			{
				ex = ex2;
			}
			catch (CommunicationException ex3)
			{
				ex = ex3;
			}
			catch (TimeoutException ex4)
			{
				ex = ex4;
			}
			finally
			{
				if (ex != null && host != null)
				{
					WcfUtils.DisposeWcfClientGracefully(host, false);
				}
			}
			if (ex != null)
			{
				ExTraceGlobals.DiagnosticsAggregationTracer.TraceError<Exception>(0L, "HostService Failed {0}.", ex);
				if (rehosting)
				{
					DiagnosticsAggregationServicelet.EventLog.LogEvent(MSExchangeDiagnosticsAggregationEventLogConstants.Tuple_DiagnosticsAggregationRehostingFailed, null, new object[]
					{
						diagnosticsAggregationServicePort,
						ex
					});
				}
				else
				{
					DiagnosticsAggregationServicelet.EventLog.LogEvent(MSExchangeDiagnosticsAggregationEventLogConstants.Tuple_DiagnosticsAggregationServiceletLoadFailed, null, new object[]
					{
						ex
					});
				}
				DiagnosticsAggregationServicelet.Log.Log(DiagnosticsAggregationEvent.ServiceletError, ex.ToString(), new object[0]);
				return false;
			}
			return true;
		}

		private void AddMetadataEndpointInDebugBuild(ServiceHost host)
		{
		}

		private void RegisterForChangeNotifications()
		{
			if (DiagnosticsAggregationServicelet.serverConfigNotificationCookie != null)
			{
				throw new InvalidOperationException("Cannot register for transportserver AD change notifications twice");
			}
			ADNotificationAdapter.TryRegisterChangeNotification<Server>(DiagnosticsAggregationServicelet.localServer.Id, new ADNotificationCallback(DiagnosticsAggregationServicelet.OnLocalServerChanged), 5, out DiagnosticsAggregationServicelet.serverConfigNotificationCookie);
			if (DiagnosticsAggregationServicelet.transportSettingsNotificationCookie != null)
			{
				throw new InvalidOperationException("Cannot register for transportsettings AD change notifications twice");
			}
			ADNotificationAdapter.TryRegisterChangeNotification<TransportConfigContainer>(DiagnosticsAggregationServicelet.TransportSettings.Id, new ADNotificationCallback(this.OnTransportSettingsChanged), 5, out DiagnosticsAggregationServicelet.transportSettingsNotificationCookie);
		}

		private void OnTransportSettingsChanged(ADNotificationEventArgs args)
		{
			DiagnosticsAggregationServicelet.Log.Log(DiagnosticsAggregationEvent.Information, "Transportsettings changed", new object[0]);
			DiagnosticsAggregationServicelet.transportSettingsChanged = true;
		}

		private void UnregisterADNotifications()
		{
			if (DiagnosticsAggregationServicelet.serverConfigNotificationCookie != null)
			{
				ADNotificationAdapter.TryRunADOperation(delegate()
				{
					ADNotificationAdapter.UnregisterChangeNotification(DiagnosticsAggregationServicelet.serverConfigNotificationCookie);
				}, 0);
				DiagnosticsAggregationServicelet.serverConfigNotificationCookie = null;
			}
			if (DiagnosticsAggregationServicelet.transportSettingsNotificationCookie != null)
			{
				ADNotificationAdapter.TryRunADOperation(delegate()
				{
					ADNotificationAdapter.UnregisterChangeNotification(DiagnosticsAggregationServicelet.transportSettingsNotificationCookie);
				}, 0);
				DiagnosticsAggregationServicelet.transportSettingsNotificationCookie = null;
			}
		}

		private static readonly ExEventLog eventLog = new ExEventLog(ExTraceGlobals.DiagnosticsAggregationTracer.Category, "MSExchange DiagnosticsAggregation");

		private static readonly DiagnosticsAggregationLog log = new DiagnosticsAggregationLog();

		private static LocalQueuesDataProvider localQueuesDataProvider;

		private static GroupQueuesDataProvider groupQueuesDataProvider;

		private static Server localServer;

		private static TransportConfigContainer transportSettings;

		private static ADNotificationRequestCookie serverConfigNotificationCookie;

		private static DiagnosticsAggregationServiceletConfig config;

		private static ADNotificationRequestCookie transportSettingsNotificationCookie;

		private static bool transportSettingsChanged;
	}
}
