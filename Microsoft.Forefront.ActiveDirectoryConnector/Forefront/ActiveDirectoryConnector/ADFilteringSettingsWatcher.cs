using System;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Filtering;
using Microsoft.Forefront.ActiveDirectoryConnector.Events;
using Microsoft.Internal.ManagedWPP;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.ActiveDirectoryConnector
{
	[Guid("48527140-FBF1-4BDB-8BDD-D2A76A3B8C4B")]
	[ComVisible(true)]
	public class ADFilteringSettingsWatcher : IADFilteringSettingsWatcher
	{
		private static ITopologyConfigurationSession SharedAdConfigurationSession
		{
			get
			{
				if (ADFilteringSettingsWatcher.adConfigurationSession == null)
				{
					lock (ADFilteringSettingsWatcher.protectionObject)
					{
						if (ADFilteringSettingsWatcher.adConfigurationSession == null)
						{
							ADFilteringSettingsWatcher.adConfigurationSession = ADHelpers.CreateDefaultReadOnlyTopologyConfigurationSession();
						}
					}
				}
				return ADFilteringSettingsWatcher.adConfigurationSession;
			}
		}

		public void Start()
		{
			try
			{
				if (!this.RegisterConfigurationChangeHandlers())
				{
					throw new ApplicationException("Could not setup AD Change Handler");
				}
				this.ServerConfigUpdate(null);
				if (Tracing.tracer.Level >= 5 && (Tracing.tracer.Flags & 2048) != 0)
				{
					WPP_1cd3dee55f704f6905d1e53a161baad7.WPP_i(1, 10, this.GetHashCode());
				}
				ADFilteringSettingsWatcher.eventLogger.LogEvent(ADConnectorEventLogConstants.Tuple_ADFilteringSettingWatcherStarted, null, null);
			}
			catch (Exception ex)
			{
				if (Tracing.tracer.Level >= 2 && (Tracing.tracer.Flags & 2048) != 0)
				{
					WPP_1cd3dee55f704f6905d1e53a161baad7.WPP_is(1, 11, this.GetHashCode(), TraceProvider.MakeStringArg(ex.ToString()));
				}
				EventNotificationItem.Publish(ExchangeComponent.AMADError.Name, "FIPS.ADFilteringSettingsWatcherStartFailed", null, ex.ToString(), ResultSeverityLevel.Error, false);
				ADFilteringSettingsWatcher.eventLogger.LogEvent(ADConnectorEventLogConstants.Tuple_ADFilteringSettingWatcherStartException, null, new object[]
				{
					ex
				});
				throw;
			}
		}

		public void Stop()
		{
			try
			{
				this.UnRegisterConfigurationChangeHandlers();
				if (Tracing.tracer.Level >= 5 && (Tracing.tracer.Flags & 2048) != 0)
				{
					WPP_1cd3dee55f704f6905d1e53a161baad7.WPP_i(1, 12, this.GetHashCode());
				}
				ADFilteringSettingsWatcher.eventLogger.LogEvent(ADConnectorEventLogConstants.Tuple_ADFilteringSettingWatcherStopped, null, null);
			}
			catch (Exception ex)
			{
				if (Tracing.tracer.Level >= 2 && (Tracing.tracer.Flags & 2048) != 0)
				{
					WPP_1cd3dee55f704f6905d1e53a161baad7.WPP_is(1, 13, this.GetHashCode(), TraceProvider.MakeStringArg(ex.ToString()));
				}
				ADFilteringSettingsWatcher.eventLogger.LogEvent(ADConnectorEventLogConstants.Tuple_ADFilteringSettingWatcherStopException, null, new object[]
				{
					ex
				});
				throw;
			}
		}

		public int GetProcessId()
		{
			int result;
			try
			{
				int id;
				using (Process currentProcess = Process.GetCurrentProcess())
				{
					id = currentProcess.Id;
				}
				if (Tracing.tracer.Level >= 5 && (Tracing.tracer.Flags & 2048) != 0)
				{
					WPP_1cd3dee55f704f6905d1e53a161baad7.WPP_is(1, 14, this.GetHashCode(), TraceProvider.MakeStringArg(id.ToString()));
				}
				ADFilteringSettingsWatcher.eventLogger.LogEvent(ADConnectorEventLogConstants.Tuple_ADFilteringSettingWatcherProcessId, null, new object[]
				{
					id
				});
				result = id;
			}
			catch (Exception ex)
			{
				if (Tracing.tracer.Level >= 2 && (Tracing.tracer.Flags & 2048) != 0)
				{
					WPP_1cd3dee55f704f6905d1e53a161baad7.WPP_is(1, 15, this.GetHashCode(), TraceProvider.MakeStringArg(ex.ToString()));
				}
				ADFilteringSettingsWatcher.eventLogger.LogEvent(ADConnectorEventLogConstants.Tuple_ADFilteringSettingWatcherGetProcessIdException, null, new object[]
				{
					ex
				});
				throw;
			}
			return result;
		}

		private bool RegisterConfigurationChangeHandlers()
		{
			if (Tracing.tracer.Level >= 5 && (Tracing.tracer.Flags & 2048) != 0)
			{
				WPP_1cd3dee55f704f6905d1e53a161baad7.WPP_i(1, 16, this.GetHashCode());
			}
			if (this.serverNotificationCookie != null)
			{
				return true;
			}
			ADOperationResult adoperationResult;
			Server server = this.ReadServerConfig(out adoperationResult);
			if (server != null)
			{
				adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					this.serverNotificationCookie = ADNotificationAdapter.RegisterChangeNotification<Server>(server.Id, new ADNotificationCallback(this.ServerConfigUpdate));
					if (Tracing.tracer.Level >= 5 && (Tracing.tracer.Flags & 2048) != 0)
					{
						WPP_1cd3dee55f704f6905d1e53a161baad7.WPP_is(1, 17, this.GetHashCode(), TraceProvider.MakeStringArg(server.Id.ToString()));
					}
					ADFilteringSettingsWatcher.eventLogger.LogEvent(ADConnectorEventLogConstants.Tuple_ADFilteringSettingWatcherChangeHandlersRegistered, null, new object[]
					{
						server.Id
					});
				});
				return adoperationResult.Succeeded;
			}
			if (Tracing.tracer.Level >= 2 && (Tracing.tracer.Flags & 2048) != 0)
			{
				WPP_1cd3dee55f704f6905d1e53a161baad7.WPP_i(1, 18, this.GetHashCode());
			}
			ADFilteringSettingsWatcher.eventLogger.LogEvent(ADConnectorEventLogConstants.Tuple_ADFilteringSettingWatcherRegisterConfigurationChangeHandlersReadServerConfigFailed, null, null);
			return false;
		}

		private void UnRegisterConfigurationChangeHandlers()
		{
			ADNotificationListener.Stop();
			if (this.serverNotificationCookie != null)
			{
				if (Tracing.tracer.Level >= 5 && (Tracing.tracer.Flags & 2048) != 0)
				{
					WPP_1cd3dee55f704f6905d1e53a161baad7.WPP_i(1, 19, this.GetHashCode());
				}
				ADNotificationAdapter.UnregisterChangeNotification(this.serverNotificationCookie);
				ADFilteringSettingsWatcher.eventLogger.LogEvent(ADConnectorEventLogConstants.Tuple_ADFilteringSettingWatcherChangeHandlersUnRegistered, null, null);
			}
		}

		private void ServerConfigUpdate(ADNotificationEventArgs args)
		{
			try
			{
				if (Tracing.tracer.Level >= 5 && (Tracing.tracer.Flags & 2048) != 0)
				{
					WPP_1cd3dee55f704f6905d1e53a161baad7.WPP_i(1, 20, this.GetHashCode());
				}
				ADFilteringSettingsWatcher.eventLogger.LogEvent(ADConnectorEventLogConstants.Tuple_ADFilteringSettingWatcherServerConfigUpdateNotification, null, null);
				ADOperationResult adoperationResult;
				Server server = this.ReadServerConfig(out adoperationResult);
				if (server != null)
				{
					FilteringSettings filteringSettings = this.filteringSettings;
					if (this.GetFilteringSettingsFromServerConfig(server))
					{
						if (!this.filteringSettings.Equals(filteringSettings) || args == null)
						{
							this.SetFilteringSettingsToFips();
						}
						else
						{
							ADFilteringSettingsWatcher.eventLogger.LogEvent(ADConnectorEventLogConstants.Tuple_ADFilteringSettingWatcherServerConfigUpdateNoChanges, null, null);
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (Tracing.tracer.Level >= 5 && (Tracing.tracer.Flags & 2048) != 0)
				{
					WPP_1cd3dee55f704f6905d1e53a161baad7.WPP_is(1, 21, this.GetHashCode(), TraceProvider.MakeStringArg(ex.ToString()));
				}
				ADFilteringSettingsWatcher.eventLogger.LogEvent(ADConnectorEventLogConstants.Tuple_ADFilteringSettingWatcherServerConfigUpdateException, null, new object[]
				{
					ex
				});
				throw;
			}
		}

		private Server ReadServerConfig(out ADOperationResult opResult)
		{
			if (Tracing.tracer.Level >= 5 && (Tracing.tracer.Flags & 2048) != 0)
			{
				WPP_1cd3dee55f704f6905d1e53a161baad7.WPP_i(1, 22, this.GetHashCode());
			}
			Server result = null;
			if (!ADNotificationAdapter.TryReadConfiguration<Server>(delegate()
			{
				Server result2;
				try
				{
					result2 = ADFilteringSettingsWatcher.SharedAdConfigurationSession.FindLocalServer();
				}
				catch (LocalServerNotFoundException)
				{
					result2 = null;
				}
				return result2;
			}, out result, out opResult))
			{
				ADFilteringSettingsWatcher.eventLogger.LogEvent(ADConnectorEventLogConstants.Tuple_ADFilteringSettingWatcherReadServerConfigFailed, null, null);
				return null;
			}
			return result;
		}

		private bool GetFilteringSettingsFromServerConfig(Server server)
		{
			try
			{
				this.filteringSettings.MalwareFilteringUpdateFrequency = (int)server[ServerSchema.MalwareFilteringUpdateFrequency];
				this.filteringSettings.MalwareFilteringUpdateTimeout = (int)server[ServerSchema.MalwareFilteringUpdateTimeout];
				this.filteringSettings.MalwareFilteringPrimaryUpdatePath = (string)server[ServerSchema.MalwareFilteringPrimaryUpdatePath];
				this.filteringSettings.MalwareFilteringSecondaryUpdatePath = (string)server[ServerSchema.MalwareFilteringSecondaryUpdatePath];
			}
			catch (InvalidCastException ex)
			{
				if (Tracing.tracer.Level >= 2 && (Tracing.tracer.Flags & 2048) != 0)
				{
					WPP_1cd3dee55f704f6905d1e53a161baad7.WPP_is(1, 23, this.GetHashCode(), TraceProvider.MakeStringArg(ex.ToString()));
				}
				ADFilteringSettingsWatcher.eventLogger.LogEvent(ADConnectorEventLogConstants.Tuple_ADFilteringSettingWatcherGetFilteringSettingsFromServerConfigException, null, new object[]
				{
					ex
				});
				return false;
			}
			return true;
		}

		private bool SetFilteringSettingsToFips()
		{
			RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();
			PSSnapInException ex = null;
			runspaceConfiguration.AddPSSnapIn("Microsoft.Forefront.Filtering.Management.PowerShell", out ex);
			if (ex != null)
			{
				if (Tracing.tracer.Level >= 2 && (Tracing.tracer.Flags & 2048) != 0)
				{
					WPP_1cd3dee55f704f6905d1e53a161baad7.WPP_is(1, 24, this.GetHashCode(), TraceProvider.MakeStringArg(ex.Message));
				}
				ADFilteringSettingsWatcher.eventLogger.LogEvent(ADConnectorEventLogConstants.Tuple_ADFilteringSettingWatcherServerConfigUpdateErrorAddingSnapin, null, new object[]
				{
					ex.Message
				});
				return false;
			}
			using (Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration))
			{
				runspace.Open();
				using (Pipeline pipeline = runspace.CreatePipeline())
				{
					Command command = new Command("Set-EngineUpdateCommonSettings");
					command.Parameters.Add("EngineDownloadTimeout", this.filteringSettings.MalwareFilteringUpdateTimeout);
					command.Parameters.Add("PrimaryUpdatePath", this.filteringSettings.MalwareFilteringPrimaryUpdatePath);
					command.Parameters.Add("SecondaryUpdatePath", this.filteringSettings.MalwareFilteringSecondaryUpdatePath);
					command.Parameters.Add("UpdateFrequency", TimeSpan.FromMinutes((double)this.filteringSettings.MalwareFilteringUpdateFrequency));
					pipeline.Commands.Add(command);
					try
					{
						pipeline.Invoke();
						if (Tracing.tracer.Level >= 5 && (Tracing.tracer.Flags & 2048) != 0)
						{
							WPP_1cd3dee55f704f6905d1e53a161baad7.WPP_is(1, 25, this.GetHashCode(), TraceProvider.MakeStringArg(command.ToString()));
						}
						ADFilteringSettingsWatcher.eventLogger.LogEvent(ADConnectorEventLogConstants.Tuple_ADFilteringSettingWatcherSetFilteringSettingsToFips, null, new object[]
						{
							this.filteringSettings.MalwareFilteringUpdateTimeout,
							this.filteringSettings.MalwareFilteringPrimaryUpdatePath,
							this.filteringSettings.MalwareFilteringSecondaryUpdatePath,
							TimeSpan.FromMinutes((double)this.filteringSettings.MalwareFilteringUpdateFrequency)
						});
					}
					catch (RuntimeException ex2)
					{
						if (Tracing.tracer.Level >= 2 && (Tracing.tracer.Flags & 2048) != 0)
						{
							WPP_1cd3dee55f704f6905d1e53a161baad7.WPP_iss(1, 26, this.GetHashCode(), TraceProvider.MakeStringArg(command.ToString()), TraceProvider.MakeStringArg(ex2.ToString()));
						}
						ADFilteringSettingsWatcher.eventLogger.LogEvent(ADConnectorEventLogConstants.Tuple_ADFilteringSettingWatcherServerConfigUpdateErrorSettingFilteringServiceSettings, null, new object[]
						{
							command,
							ex2
						});
						return false;
					}
				}
			}
			return true;
		}

		private static ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.ADConnectorTracer.Category, "Filtering ADConnector");

		private static ITopologyConfigurationSession adConfigurationSession;

		private static object protectionObject = new object();

		private ADNotificationRequestCookie serverNotificationCookie;

		private FilteringSettings filteringSettings;
	}
}
