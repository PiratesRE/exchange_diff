using System;
using System.ComponentModel;
using System.Configuration;
using System.Security;
using System.Security.Principal;
using System.ServiceModel;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Replay.Dumpster;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Serialization;
using Microsoft.Exchange.Data.Storage.Cluster;
using Microsoft.Exchange.HA.Services;
using Microsoft.Mapi;

namespace Microsoft.Exchange.HA.SupportApi
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, IncludeExceptionDetailInFaults = true)]
	internal class SupportApiService : IInternalSupportApi
	{
		private static bool AuthorizeRequest(WindowsIdentity wid)
		{
			IdentityReferenceCollection groups = wid.Groups;
			foreach (IdentityReference left in groups)
			{
				if (left == SupportApiService.s_localAdminsSid)
				{
					return true;
				}
			}
			return false;
		}

		private static void ThrowNotAuthorized()
		{
			throw new SecurityException("Not authorized to access the SupportApi");
		}

		private static void CheckSecurity()
		{
			WindowsIdentity windowsIdentity = ServiceSecurityContext.Current.PrimaryIdentity as WindowsIdentity;
			if (windowsIdentity == null)
			{
				SupportApiService.ThrowNotAuthorized();
			}
			if (!SupportApiService.AuthorizeRequest(windowsIdentity))
			{
				SupportApiService.ThrowNotAuthorized();
			}
		}

		public void DisconnectCopier(Guid dbGuid)
		{
			LogCopier.TestDisconnectCopier(dbGuid);
		}

		public void ConnectCopier(Guid dbGuid)
		{
			LogCopier.TestConnectCopier(dbGuid);
		}

		public void SetFailedAndSuspended(Guid dbGuid, bool fSuspendCopy, uint errorEventId, string failedMsg)
		{
			ReplicaInstanceManager replicaInstanceManager = Dependencies.ReplayCoreManager.ReplicaInstanceManager;
			replicaInstanceManager.RequestSuspendAndFail_SupportApi(dbGuid, fSuspendCopy, errorEventId, failedMsg, "Suspended by the SupportApi SetFailedAndSuspended() test call.", false);
		}

		public void TriggerShutdownSwitchover()
		{
			ActiveManagerCore.AttemptServerSwitchoverOnShutdown();
		}

		public void IgnoreGranularCompletions(Guid dbGuid)
		{
			LogCopier.TestIgnoreGranularCompletions(dbGuid);
		}

		public void ReloadRegistryParameters()
		{
			RegistryParameters.TestLoadRegistryParameters();
		}

		public void TriggerLogSourceCorruption(Guid dbGuid, bool granular, bool granularRepairFails, int countOfLogsBeforeCorruption)
		{
		}

		public void SetCopyProperty(Guid dbGuid, string propName, string propVal)
		{
			char[] separator = new char[]
			{
				'.'
			};
			string[] array = propName.Split(separator, 2);
			if (array.Length > 1)
			{
				if (SharedHelper.StringIEquals(array[0], "MonitoredDatabase"))
				{
					MonitoredDatabase.SetCopyProperty(dbGuid, array[1], propVal);
					return;
				}
				if (SharedHelper.StringIEquals(array[0], "LogCopier"))
				{
					LogCopier.SetCopyProperty(dbGuid, array[1], propVal);
					return;
				}
			}
			string message = string.Format("SetCopyProperty doesn't recognize '{0}'", propName);
			throw new ArgumentException(message);
		}

		public static SupportApiService StartListening(out Exception exception)
		{
			exception = null;
			SupportApiService supportApiService = null;
			try
			{
				supportApiService = new SupportApiService();
				int num = 2014;
				NetTcpBinding netTcpBinding = new NetTcpBinding();
				netTcpBinding.PortSharingEnabled = true;
				string uriString = string.Format("net.tcp://localhost:{0}/Microsoft.Exchange.HA.SupportApi", num);
				Uri uri = new Uri(uriString);
				supportApiService.m_host = new ServiceHost(supportApiService, new Uri[]
				{
					uri
				});
				supportApiService.m_host.AddServiceEndpoint(typeof(IInternalSupportApi), netTcpBinding, string.Empty);
				supportApiService.m_host.Open();
				return supportApiService;
			}
			catch (CommunicationException ex)
			{
				exception = ex;
			}
			catch (ConfigurationException ex2)
			{
				exception = ex2;
			}
			catch (InvalidOperationException ex3)
			{
				exception = ex3;
			}
			ReplayCrimsonEvents.SupportApiFailedToStart.LogPeriodic<string>(Environment.MachineName, DiagCore.DefaultEventSuppressionInterval, exception.ToString());
			if (supportApiService != null && supportApiService.m_host != null)
			{
				supportApiService.m_host.Abort();
			}
			return null;
		}

		public void StopListening()
		{
			this.m_host.Close();
		}

		private byte[] SerializeException(Exception exception)
		{
			return Serialization.ObjectToBytes(exception);
		}

		private Exception DoAction(Action action)
		{
			Exception result = null;
			try
			{
				SupportApiService.CheckSecurity();
				action();
			}
			catch (ADTransientException ex)
			{
				result = ex;
			}
			catch (ADExternalException ex2)
			{
				result = ex2;
			}
			catch (ADOperationException ex3)
			{
				result = ex3;
			}
			catch (AmServiceShuttingDownException ex4)
			{
				result = ex4;
			}
			catch (MapiPermanentException ex5)
			{
				result = ex5;
			}
			catch (MapiRetryableException ex6)
			{
				result = ex6;
			}
			catch (DataSourceTransientException ex7)
			{
				result = ex7;
			}
			catch (DataSourceOperationException ex8)
			{
				result = ex8;
			}
			catch (HaRpcServerBaseException ex9)
			{
				result = ex9;
			}
			catch (TransientException ex10)
			{
				result = ex10;
			}
			catch (Win32Exception ex11)
			{
				result = ex11;
			}
			catch (SecurityException ex12)
			{
				result = ex12;
			}
			return result;
		}

		public void TriggerConfigUpdater()
		{
			Dependencies.ConfigurationUpdater.RunConfigurationUpdater(true, ReplayConfigChangeHints.RunConfigUpdaterRpc);
		}

		public void TriggerDumpster(Guid dbGuid, DateTime inspectorTime)
		{
			IADToplogyConfigurationSession iadtoplogyConfigurationSession = ADSessionFactory.CreatePartiallyConsistentRootOrgSession(true);
			IADDatabase db = iadtoplogyConfigurationSession.FindDatabaseByGuid(dbGuid);
			IADServer server = iadtoplogyConfigurationSession.FindServerByName(Environment.MachineName);
			IADDatabaseAvailabilityGroup dag = iadtoplogyConfigurationSession.FindDagByServer(server);
			ReplayConfiguration configuration = RemoteReplayConfiguration.TaskGetReplayConfig(dag, db, server);
			DumpsterRedeliveryWrapper.MarkRedeliveryRequired(configuration, inspectorTime, 0L, 0L);
		}

		public void TriggerDumpsterEx(Guid dbGuid, bool fTriggerSafetyNet, DateTime failoverTimeUtc, DateTime startTimeUtc, DateTime endTimeUtc, long lastLogGenBeforeActivation, long numLogsLost)
		{
			IADToplogyConfigurationSession iadtoplogyConfigurationSession = ADSessionFactory.CreatePartiallyConsistentRootOrgSession(true);
			IADDatabase db = iadtoplogyConfigurationSession.FindDatabaseByGuid(dbGuid);
			IADServer server = iadtoplogyConfigurationSession.FindServerByName(Environment.MachineName);
			IADDatabaseAvailabilityGroup dag = iadtoplogyConfigurationSession.FindDagByServer(server);
			ReplayConfiguration configuration = RemoteReplayConfiguration.TaskGetReplayConfig(dag, db, server);
			DumpsterRedeliveryWrapper.MarkRedeliveryRequired(configuration, failoverTimeUtc, startTimeUtc, endTimeUtc, lastLogGenBeforeActivation, numLogsLost);
		}

		public void DoDumpsterRedeliveryIfRequired(Guid dbGuid)
		{
			IADToplogyConfigurationSession iadtoplogyConfigurationSession = ADSessionFactory.CreatePartiallyConsistentRootOrgSession(true);
			IADDatabase db = iadtoplogyConfigurationSession.FindDatabaseByGuid(dbGuid);
			IADServer server = iadtoplogyConfigurationSession.FindServerByName(Environment.MachineName);
			IADDatabaseAvailabilityGroup dag = iadtoplogyConfigurationSession.FindDagByServer(server);
			ReplayConfiguration replayConfig = RemoteReplayConfiguration.TaskGetReplayConfig(dag, db, server);
			DumpsterRedeliveryWrapper.DoRedeliveryIfRequired(replayConfig);
		}

		public void TriggerServerLocatorRestart()
		{
			ServerLocatorManager.Instance.RestartServiceHost(this, null);
		}

		public void TriggerTruncation(Guid dbGuid)
		{
			ReplicaInstanceManager replicaInstanceManager = Dependencies.ReplayCoreManager.ReplicaInstanceManager;
			ReplicaInstance replicaInstance;
			if (replicaInstanceManager.TryGetReplicaInstance(dbGuid, out replicaInstance))
			{
				LogTruncater component = replicaInstance.GetComponent<LogTruncater>();
				component.TimerCallback(null);
				return;
			}
			throw new ArgumentException("Unable to find a ReplicaInstance with a DB Guid of " + dbGuid, "dbGuid");
		}

		public bool Ping()
		{
			return true;
		}

		private static SecurityIdentifier s_localAdminsSid = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);

		private ServiceHost m_host;
	}
}
