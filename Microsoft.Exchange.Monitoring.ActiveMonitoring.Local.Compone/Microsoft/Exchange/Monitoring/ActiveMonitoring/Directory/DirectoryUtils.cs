using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Interop.ActiveDS;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory
{
	public class DirectoryUtils
	{
		public static ProbeResult GetLastProbeResult(ProbeWorkItem probe, IProbeWorkBroker broker, CancellationToken cancellationToken)
		{
			ProbeResult lastProbeResult = null;
			if (broker != null)
			{
				IOrderedEnumerable<ProbeResult> query = from r in broker.GetProbeResults(probe.Definition, probe.Result.ExecutionStartTime.AddSeconds((double)(-3 * probe.Definition.RecurrenceIntervalSeconds)))
				orderby r.ExecutionStartTime descending
				select r;
				Task<int> task = broker.AsDataAccessQuery<ProbeResult>(query).ExecuteAsync(delegate(ProbeResult r)
				{
					if (lastProbeResult == null)
					{
						lastProbeResult = r;
					}
				}, cancellationToken, DirectoryUtils.traceContext);
				task.Wait(cancellationToken);
				return lastProbeResult;
			}
			if (ExEnvironment.IsTest)
			{
				return null;
			}
			throw new ArgumentNullException("broker");
		}

		public static bool IsRidMaster()
		{
			bool result;
			using (Domain computerDomain = Domain.GetComputerDomain())
			{
				result = computerDomain.RidRoleOwner.Name.StartsWith(Environment.MachineName, StringComparison.InvariantCultureIgnoreCase);
			}
			return result;
		}

		public static bool IsPrimaryActiveManager()
		{
			bool result = false;
			if (ExEnvironment.IsTest)
			{
				return true;
			}
			try
			{
				IADServer localServer = CachedAdReader.Instance.LocalServer;
				if (localServer == null || localServer.DatabaseAvailabilityGroup == null)
				{
					return false;
				}
				IADDatabaseAvailabilityGroup localDAG = CachedAdReader.Instance.LocalDAG;
				if (localDAG != null)
				{
					AmServerName primaryActiveManagerNode = DagTaskHelper.GetPrimaryActiveManagerNode(localDAG);
					if (primaryActiveManagerNode != null)
					{
						result = primaryActiveManagerNode.IsLocalComputerName;
					}
				}
			}
			catch (Exception)
			{
			}
			return result;
		}

		public static int GetRidsLeft()
		{
			int result;
			using (DirectoryEntry directoryEntry = new DirectoryEntry())
			{
				using (DirectoryEntry directoryEntry2 = new DirectoryEntry("LDAP://CN=RID Manager$,CN=System," + directoryEntry.Properties["distinguishedName"].Value.ToString()))
				{
					IADsLargeInteger iadsLargeInteger = (IADsLargeInteger)directoryEntry2.Properties["rIDAvailablePool"].Value;
					result = iadsLargeInteger.HighPart - iadsLargeInteger.LowPart;
				}
			}
			return result;
		}

		public static bool GetCredentials(out string username, out string password, out string domain, ProbeWorkItem probe)
		{
			username = string.Empty;
			password = string.Empty;
			domain = string.Empty;
			bool result;
			try
			{
				LocalEndpointManager instance = LocalEndpointManager.Instance;
				ICollection<MailboxDatabaseInfo> collection;
				if (instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
				{
					collection = instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend;
				}
				else
				{
					collection = instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForCafe;
				}
				if (collection.Count == 0)
				{
					probe.Result.StateAttribute2 = DirectoryUtils.ExceptionType.None.ToString();
					result = false;
				}
				else
				{
					Random random = new Random();
					int index = random.Next(0, collection.Count);
					MailboxDatabaseInfo mailboxDatabaseInfo = ((List<MailboxDatabaseInfo>)collection)[index];
					if (string.IsNullOrEmpty(mailboxDatabaseInfo.MonitoringAccount) || string.IsNullOrEmpty(mailboxDatabaseInfo.MonitoringAccountPassword) || string.IsNullOrEmpty(mailboxDatabaseInfo.MonitoringAccountDomain))
					{
						probe.Result.StateAttribute2 = DirectoryUtils.ExceptionType.None.ToString();
						result = false;
					}
					else
					{
						username = mailboxDatabaseInfo.MonitoringAccount + "@" + mailboxDatabaseInfo.MonitoringAccountDomain;
						password = mailboxDatabaseInfo.MonitoringAccountPassword;
						domain = mailboxDatabaseInfo.MonitoringAccountDomain;
						result = true;
					}
				}
			}
			catch (Exception ex)
			{
				probe.Result.StateAttribute2 = DirectoryUtils.ExceptionType.None.ToString();
				string stateAttribute = string.Format("GetCredentials get exception for user {0} with psd {1} in domain {2}: {3}", new object[]
				{
					username,
					password,
					domain,
					ex.ToString()
				});
				probe.Result.StateAttribute4 = stateAttribute;
				result = false;
			}
			return result;
		}

		public static bool GetLiveIdProbeCredentials(out string username, out string password, out string domain, ProbeWorkItem probe)
		{
			username = string.Empty;
			password = string.Empty;
			domain = string.Empty;
			bool result;
			try
			{
				LocalEndpointManager instance = LocalEndpointManager.Instance;
				ICollection<MailboxDatabaseInfo> databaseCollection = null;
				if (instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
				{
					databaseCollection = instance.MailboxDatabaseEndpoint.UnverifiedMailboxDatabaseInfoCollectionForBackendLiveIdAuthenticationProbe;
				}
				else
				{
					databaseCollection = instance.MailboxDatabaseEndpoint.UnverifiedMailboxDatabaseInfoCollectionForCafeLiveIdAuthenticationProbe;
				}
				if (databaseCollection.Count == 0)
				{
					probe.Result.StateAttribute2 = DirectoryUtils.ExceptionType.None.ToString();
					result = false;
				}
				else
				{
					Random rand = new Random();
					int randomIndex = rand.Next(0, databaseCollection.Count);
					MailboxDatabaseInfo randomDatabase = ((List<MailboxDatabaseInfo>)databaseCollection)[randomIndex];
					bool flag = DirectoryGeneralUtils.Retry(delegate(bool lastAttempt)
					{
						if (randomDatabase.AuthenticationResult != LiveIdAuthResult.UserNotFoundInAD && randomDatabase.AuthenticationResult != LiveIdAuthResult.ExpiredCreds && randomDatabase.AuthenticationResult != LiveIdAuthResult.InvalidCreds && randomDatabase.AuthenticationResult != LiveIdAuthResult.RecoverableAuthFailure && randomDatabase.AuthenticationResult != LiveIdAuthResult.AmbigiousMailboxFoundFailure && !string.IsNullOrEmpty(randomDatabase.MonitoringAccount) && !string.IsNullOrEmpty(randomDatabase.MonitoringAccountDomain))
						{
							return true;
						}
						randomIndex = rand.Next(0, databaseCollection.Count);
						randomDatabase = ((List<MailboxDatabaseInfo>)databaseCollection)[randomIndex];
						return false;
					}, databaseCollection.Count, TimeSpan.FromMilliseconds(1.0));
					if (flag)
					{
						username = randomDatabase.MonitoringAccountUserPrincipalName;
						domain = randomDatabase.MonitoringAccountDomain;
						try
						{
							password = randomDatabase.MonitoringAccountPassword;
						}
						catch (MailboxNotValidatedException ex)
						{
							password = ex.Password;
						}
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			catch (Exception ex2)
			{
				probe.Result.StateAttribute2 = DirectoryUtils.ExceptionType.None.ToString();
				string stateAttribute = string.Format("GetCredentials get exception for user {0} with psd {1} in domain {2}: {3}", new object[]
				{
					username,
					password,
					domain,
					ex2.ToString()
				});
				probe.Result.StateAttribute4 = stateAttribute;
				result = false;
			}
			return result;
		}

		public static string GenerateRandomString(int len)
		{
			if (len > 0 && len <= 32)
			{
				return Guid.NewGuid().ToString("N").Substring(0, len);
			}
			return Guid.NewGuid().ToString("N");
		}

		public static void InvokeBaseResponderMethodIfRequired(ResponderWorkItem responder, Action<CancellationToken> baseDoResponderWork, TracingContext traceContext, CancellationToken cancellationToken)
		{
			IResponderWorkBroker broker = (IResponderWorkBroker)responder.Broker;
			string exceptionType;
			responder.Definition.Attributes.TryGetValue("ExceptionType", out exceptionType);
			if (string.IsNullOrEmpty(exceptionType))
			{
				baseDoResponderWork(cancellationToken);
				return;
			}
			IDataAccessQuery<ResponderResult> lastSuccessfulResponderResult = broker.GetLastSuccessfulResponderResult(responder.Definition);
			Task<ResponderResult> task = lastSuccessfulResponderResult.ExecuteAsync(cancellationToken, traceContext);
			task.Continue(delegate(ResponderResult lastResponderResult)
			{
				DateTime startTime = DateTime.MinValue;
				if (lastResponderResult != null)
				{
					startTime = lastResponderResult.ExecutionStartTime;
				}
				IDataAccessQuery<MonitorResult> lastSuccessfulMonitorResult = broker.GetLastSuccessfulMonitorResult(responder.Definition.AlertMask, startTime, responder.Result.ExecutionStartTime);
				Task<MonitorResult> task2 = lastSuccessfulMonitorResult.ExecuteAsync(cancellationToken, traceContext);
				task2.Continue(delegate(MonitorResult lastMonitorResult)
				{
					if (lastMonitorResult != null && lastMonitorResult.IsAlert)
					{
						string stateAttribute = lastMonitorResult.StateAttribute2;
						if (!string.IsNullOrEmpty(stateAttribute) && stateAttribute.IndexOf(exceptionType, StringComparison.InvariantCultureIgnoreCase) >= 0)
						{
							baseDoResponderWork(cancellationToken);
						}
					}
				}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
		}

		public static string GetDomainControllerSite(DirectoryEntry dcEntry)
		{
			string text = string.Empty;
			if (dcEntry != null && dcEntry.Properties.Contains("serverReferenceBL") && dcEntry.Properties["serverReferenceBL"].Value != null)
			{
				string text2 = dcEntry.Properties["serverReferenceBL"].Value.ToString();
				if (text2 != null)
				{
					string[] array = Regex.Split(text2, "CN=");
					if (array.Length > 3)
					{
						text = array[3];
						text = text.Substring(0, text.Length - 1);
					}
				}
			}
			return text;
		}

		public static void CheckSharedConfigurationTenants()
		{
			List<string> mailboxServerVersions = DirectoryUtils.GetMailboxServerVersions();
			PartitionId[] allAccountPartitionIds = ADAccountPartitionLocator.GetAllAccountPartitionIds();
			bool flag = false;
			bool flag2 = false;
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			string item = string.Empty;
			List<string> list = new List<string>();
			if (allAccountPartitionIds != null && mailboxServerVersions.Count > 0)
			{
				stringBuilder.Append("SCT is not found for the following Version/Offer.\n\nPartitionId, Version, Offer\n");
				PartitionId[] array = allAccountPartitionIds;
				int i = 0;
				while (i < array.Length)
				{
					PartitionId partitionId = array[i];
					ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAllTenantsPartitionId(partitionId), 732, "CheckSharedConfigurationTenants", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\DirectoryUtils.cs");
					AndFilter filter = new AndFilter(new QueryFilter[]
					{
						new ComparisonFilter(ComparisonOperator.Equal, OrganizationSchema.EnableAsSharedConfiguration, true),
						new ComparisonFilter(ComparisonOperator.Equal, ExchangeConfigurationUnitSchema.OrganizationStatus, OrganizationStatus.Active)
					});
					ExchangeConfigurationUnit[] array2 = tenantConfigurationSession.Find<ExchangeConfigurationUnit>(null, QueryScope.SubTree, filter, null, 0);
					if (array2 != null)
					{
						foreach (ExchangeConfigurationUnit exchangeConfigurationUnit in array2)
						{
							SharedConfigurationInfo sharedConfigurationInfo = exchangeConfigurationUnit.SharedConfigurationInfo;
							ServerVersion currentVersion = sharedConfigurationInfo.CurrentVersion;
							item = string.Format("{0}.{1}.{2}.{3}_{4}", new object[]
							{
								currentVersion.Major,
								currentVersion.Minor,
								currentVersion.Build,
								currentVersion.Revision,
								sharedConfigurationInfo.OfferId
							});
							if (!list.Contains(item))
							{
								list.Add(item);
							}
						}
						using (List<string>.Enumerator enumerator = mailboxServerVersions.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								string text = enumerator.Current;
								foreach (string text2 in DirectoryUtils.SupportedOffers)
								{
									item = string.Format("{0}_{1}", text, text2);
									if (!list.Contains(item))
									{
										flag = true;
										stringBuilder.AppendFormat("{0},{1},{2}\n", partitionId.ForestFQDN, text.ToString(), text2);
									}
								}
							}
							goto IL_23F;
						}
						goto IL_219;
					}
					goto IL_219;
					IL_23F:
					i++;
					continue;
					IL_219:
					flag2 = true;
					stringBuilder2.AppendFormat("No SCTs found in this AccountPartition: {0}.  Expected SCTs to be created for versions:  {1}\n", partitionId.ForestFQDN, string.Join(",", mailboxServerVersions.ToArray()));
					goto IL_23F;
				}
			}
			StringBuilder stringBuilder3 = new StringBuilder();
			if (flag)
			{
				stringBuilder3.AppendFormat("{0}\n", stringBuilder.ToString());
			}
			if (flag2)
			{
				stringBuilder3.Append(stringBuilder2.ToString());
			}
			if (flag || flag2)
			{
				throw new Exception(stringBuilder3.ToString());
			}
		}

		public static List<string> GetMailboxServerVersions()
		{
			List<string> list = new List<string>();
			ADTopologyConfigurationSession adtopologyConfigurationSession = new ADTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet());
			BitMaskAndFilter filter = new BitMaskAndFilter(ServerSchema.CurrentServerRole, 2UL);
			ADPagedReader<MiniServer> adpagedReader = adtopologyConfigurationSession.FindPagedMiniServer(null, QueryScope.SubTree, filter, null, 1, null);
			foreach (MiniServer miniServer in adpagedReader)
			{
				string item = string.Format("{0}.{1}.{2}.{3}", new object[]
				{
					miniServer.AdminDisplayVersion.Major,
					miniServer.AdminDisplayVersion.Minor,
					miniServer.AdminDisplayVersion.Build,
					miniServer.AdminDisplayVersion.Revision
				});
				if (!list.Contains(item) && miniServer.AdminDisplayVersion.Major == 15)
				{
					list.Add(item);
				}
			}
			return list;
		}

		public static int GetADConnectivityProbeThresholdByEnviornment(int readADConnectivityThreshold)
		{
			if (ExEnvironment.IsTest)
			{
				return 10000;
			}
			return readADConnectivityThreshold;
		}

		public static bool StartLocalService(string serviceName)
		{
			bool result = false;
			TimeSpan timeout = TimeSpan.FromSeconds(30.0);
			try
			{
				using (ServiceController serviceController = new ServiceController(serviceName))
				{
					if (serviceController != null)
					{
						if (serviceController.Status == ServiceControllerStatus.Running)
						{
							throw new Exception(string.Format("Serivce  {0} not found on local server", serviceName));
						}
						serviceController.Start();
						serviceController.WaitForStatus(ServiceControllerStatus.Running, timeout);
						result = (serviceController.Status == ServiceControllerStatus.Running);
					}
				}
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		public static bool StopLocalService(string serviceName)
		{
			bool result = false;
			TimeSpan timeout = TimeSpan.FromSeconds(30.0);
			try
			{
				using (ServiceController serviceController = new ServiceController(serviceName))
				{
					if (serviceController != null)
					{
						if (serviceController.Status == ServiceControllerStatus.Stopped)
						{
							throw new Exception(string.Format("Serivce  {0} not found on local server", serviceName));
						}
						serviceController.Stop();
						serviceController.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
						result = (serviceController.Status == ServiceControllerStatus.Stopped);
					}
				}
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		public static ServiceControllerStatus GetLocalServiceStatus(string serviceName)
		{
			ServiceControllerStatus status;
			using (ServiceController serviceController = new ServiceController(serviceName))
			{
				if (serviceController == null)
				{
					throw new Exception(string.Format("Serivce  {0} not found on local server", serviceName));
				}
				status = serviceController.Status;
			}
			return status;
		}

		public static void GetServiceIntoExpectedStatus(string serviceName, ServiceControllerStatus expectStatus, int retryCount)
		{
			DirectoryGeneralUtils.Retry(delegate(bool lastAttempt)
			{
				ServiceControllerStatus expectStatus2 = expectStatus;
				if (expectStatus2 != ServiceControllerStatus.Stopped)
				{
					if (expectStatus2 == ServiceControllerStatus.Running)
					{
						DirectoryUtils.StartLocalService(serviceName);
					}
				}
				else
				{
					DirectoryUtils.StopLocalService(serviceName);
				}
				return expectStatus == DirectoryUtils.GetLocalServiceStatus(serviceName);
			}, retryCount, TimeSpan.FromSeconds(10.0));
		}

		internal static string GetRegKeyValue(string path, string key, string defaultValue)
		{
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(path))
				{
					if (registryKey != null)
					{
						string value = registryKey.GetValue(key) as string;
						if (!string.IsNullOrWhiteSpace(value))
						{
						}
					}
				}
			}
			catch (Exception)
			{
			}
			return defaultValue;
		}

		public static string GetExchangeBinPath()
		{
			string result;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup"))
			{
				if (registryKey == null)
				{
					result = string.Empty;
				}
				else
				{
					object value = registryKey.GetValue("MsiInstallPath");
					registryKey.Close();
					if (value == null)
					{
						result = string.Empty;
					}
					else
					{
						result = Path.Combine(value.ToString(), "Bin");
					}
				}
			}
			return result;
		}

		internal static void Logger(ProbeWorkItem workitem, StxLogType logType, Action method)
		{
			string errorString = string.Empty;
			bool status = false;
			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				method();
				status = true;
			}
			catch (Exception ex)
			{
				errorString = ex.ToString();
				throw;
			}
			finally
			{
				stopwatch.Stop();
				StxLoggerBase.GetLoggerInstance(logType).BeginAppend(Dns.GetHostName(), status, stopwatch.Elapsed, 0, errorString, workitem.Result.StateAttribute1, workitem.Result.StateAttribute2, workitem.Result.StateAttribute3, workitem.Result.StateAttribute4);
			}
		}

		internal static string GetReplicationXml(string DomainControllerName, string partition)
		{
			string path = "LDAP://" + DomainControllerName + "/" + partition;
			DirectoryEntry directoryEntry = null;
			string result = string.Empty;
			try
			{
				directoryEntry = new DirectoryEntry(path);
				directoryEntry.RefreshCache();
				string[] propertyNames = new string[]
				{
					"msDS-NCReplInboundNeighbors"
				};
				directoryEntry.RefreshCache(propertyNames);
				StringBuilder stringBuilder = new StringBuilder();
				if (directoryEntry.Properties["msDS-NCReplInboundNeighbors"].Value != null && directoryEntry.Properties["msDS-NCReplInboundNeighbors"].Count > 0)
				{
					int count = directoryEntry.Properties["msDS-NCReplInboundNeighbors"].Count;
					if (count == 1)
					{
						stringBuilder.Append(directoryEntry.Properties["msDS-NCReplInboundNeighbors"].Value.ToString());
					}
					else
					{
						object[] array = (object[])directoryEntry.Properties["msDS-NCReplInboundNeighbors"].Value;
						foreach (object obj in array)
						{
							stringBuilder.Append(obj.ToString());
						}
					}
					result = string.Format("<REPL>{0}</REPL>", stringBuilder.ToString());
				}
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("Could not make an LDAP connection to the Domain Controller {0}.  Got exception {1}", DomainControllerName, ex.ToString()));
			}
			finally
			{
				try
				{
					if (directoryEntry != null)
					{
						directoryEntry.Dispose();
					}
				}
				catch
				{
				}
			}
			return result;
		}

		internal static bool VerifyDomainControllerLDAPRead(string dcName, string dnNameToRead)
		{
			string path = string.Format("LDAP://{0}/{1}", dcName, dnNameToRead);
			bool result = false;
			try
			{
				using (DirectoryEntry directoryEntry = new DirectoryEntry(path))
				{
					if (directoryEntry != null)
					{
						directoryEntry.Properties["distinguishedName"].Value.ToString();
						result = true;
					}
				}
			}
			catch (Exception)
			{
			}
			return result;
		}

		internal static string ListToString(List<string> listToConvert)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string arg in listToConvert)
			{
				stringBuilder.AppendFormat("{0},", arg);
			}
			return stringBuilder.ToString().TrimEnd(new char[]
			{
				','
			});
		}

		internal static string GetDomainControllerOUFormatString(string connectToDC)
		{
			string str = string.Empty;
			string str2 = string.Empty;
			using (DirectoryEntry directoryEntry = new DirectoryEntry())
			{
				if (directoryEntry.Properties.Contains("distinguishedName") && directoryEntry.Properties["distinguishedName"].Value != null)
				{
					str = "OU=Domain Controllers," + directoryEntry.Properties["distinguishedName"].Value.ToString();
					str2 = string.Format("LDAP://{0}/CN=", connectToDC);
					return str2 + "{0}," + str;
				}
			}
			throw new Exception("Could not instantiate DirectoryEntry object which is required to create a LDAP format string.");
		}

		public const int DefaultADConnectivityTestThreshold = 10000;

		public const int DefaultServiceStartTimeoutInSecs = 30;

		public const int DefaultServiceStartRetryInterval = 10;

		public const int DefaultLoopMaxRetryCount = 2;

		public const string RidThresholdAttribute = "RidsLeftThreshold";

		public const string RidsLeftLimitAttribute = "RidsLeftLimit";

		public const string RidsLeftLimitLowValueAttribute = "RidsLeftLimitLowValue";

		public const string RidsLeftLimitSDFAttribute = "RidsLeftLimitSDF";

		public const string ExceptionTypAttribute = "ExceptionType";

		public const string ADConnectivityThresholdAttribute = "ADConnectivityThreshold";

		public const string KDCStartOnProvisionDCEnabledAttribute = "KDCStartOnProvisionDCEnabled";

		public const string KDCStopOnMMDCEnabledAttribute = "KDCStopOnMMDCEnabled";

		public const string ServiceStartStopRetryCountAttribute = "ServiceStartStopRetryCount";

		public const string LiveIdProbeLatencyThresholdAttribute = "LiveIdProbeLatencyThreshold";

		public const string ReplicationThresholdInMinsAttribute = "ReplicationThresholdInMins";

		public const string PercentageOfDCsThresholdExcludedForADHealthAttribute = "PercentageOfDCsThresholdExcludedForADHealth";

		public const string ApplicationLog = "Application";

		public const string MSExchangeADAccessSource = "MSExchange ADAccess";

		public const string MSExchangeISSource = "MSExchangeIS*";

		public const string MSExchangeLiveIdBasicAuthenticationSource = "MSExchange LiveIdBasicAuthentication";

		public const string DirectoryService = "Directory Service";

		public const string SystemLog = "System";

		public const string NTDSGeneral = "NTDS General";

		public const string NTDSSDPROP = "NTDS SDPROP";

		public const string NTDSReplication = "NTDS Replication";

		public const string NTDSDatabase = "NTDS Database";

		public const string NTDSBackup = "NTDS Backup";

		public const string ActiveDirectoryDomainService = "*ActiveDirectory_DomainService";

		public const string NTDSISAM = "NTDS ISAM";

		public const string Kerberos = "Microsoft-Windows-Security-Kerberos";

		public const string Adaptec = "Adaptec Storage Manager Agent";

		public const string NTDSKCC = "NTDS KCC";

		public const string SharedConfigTenantRecovery = "MSExchange Shared Configuration Tenant Recovery";

		public const string SharedConfigTenantStateMonitor = "MSExchange Shared Configuration Tenant State Monitor";

		public const string MSExchDCMMSource = "MSExchange Domain Controller Maintenance Mode";

		public const string MSExchProvisionDCSource = "MSExchange Monitoring Provisioned DCs";

		public const string MSExchZombieDCSource = "MSExchange Monitoring ZombieDCs";

		public const string MSExchFSMOSource = "MSExchange FSMO Roles";

		public const string BPASource = "BPA";

		public const string MSExchangeProtectedServiceHost = "MSExchangeProtectedServiceHost";

		public const string All = "*";

		public const string AdminDescriptionProperty = "adminDescription";

		public const string ADHealthProperty = "msExchExtensionAttribute45";

		public const string SupportedProgramId = "MSOnline";

		public const string ProvisioningFlagProperty = "msExchProvisioningFlags";

		public static List<string> SupportedOffers = new List<string>
		{
			"BPOS_Basic_CustomDomain_Hydrated",
			"BPOS_L_Hydrated",
			"BPOS_S_Hydrated",
			"BPOS_M_Hydrated",
			"BPOS_L_Pilot_Hydrated",
			"BPOS_M_Pilot_Hydrated",
			"BPOS_S_Pilot_Hydrated"
		};

		private static readonly TracingContext traceContext = TracingContext.Default;

		public enum ExceptionType
		{
			None,
			AuthenticationFailureNotServiceIssue,
			ProtectedServiceHostIssue,
			KDCNotRunningOnProvisionedDC,
			KDCNotStoppedOnMaintenanceDC
		}

		public enum ResponderChainType
		{
			Default,
			LiveId,
			DomainController,
			EscalateOnly,
			NonUrgentEscalate,
			Scheduled,
			PutDCInMM,
			PutMultipleDCInMM,
			RenameNTDSPowerOff,
			DoMT,
			TraceAndEscalate,
			TraceAndPutInMM,
			KDCNotRightStatus,
			None
		}
	}
}
