using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Monitors;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Rps.Probes;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rps
{
	public sealed class RPSDiscovery : MaintenanceWorkItem
	{
		internal static string CafeProbeBaseName
		{
			get
			{
				if (!LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsCafeRoleInstalled)
				{
					return RPSDiscovery.RPSDeeptTestString;
				}
				return RPSDiscovery.RPSCTPString;
			}
		}

		internal bool ShallSkipKerberosProbe
		{
			get
			{
				if (this.shallSkipKerberosProbe == null)
				{
					this.CheckMonitoringAccountKerberosAuth();
				}
				return this.shallSkipKerberosProbe.Value;
			}
		}

		internal static Component Component
		{
			get
			{
				if (!LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsCafeRoleInstalled)
				{
					return ExchangeComponent.RpsProtocol;
				}
				return ExchangeComponent.Rps;
			}
		}

		internal static string BuildWorkItemName(string baseName, string workItemTypeName, RPSDiscovery.RPSVirtualDirectory psVdirType)
		{
			return string.Format("{0}{2}{1}", baseName, workItemTypeName, psVdirType);
		}

		internal static ProbeDefinition CreateRPSCafeLogonProbeDefinition(AuthenticationMechanism rpsAuthType, string account, string accountPassword, int probeRecurrenceIntervalSeconds)
		{
			ProbeDefinition probeDefinition = new ProbeDefinition();
			RPSDiscovery.RPSVirtualDirectory psVdirType = (rpsAuthType == AuthenticationMechanism.Default || rpsAuthType == AuthenticationMechanism.Kerberos) ? RPSDiscovery.RPSVirtualDirectory.PS : RPSDiscovery.RPSVirtualDirectory.PSLiveID;
			probeDefinition.AssemblyPath = Assembly.GetExecutingAssembly().Location;
			probeDefinition.TypeName = typeof(RPSLogonProbe).FullName;
			probeDefinition.Name = RPSDiscovery.BuildWorkItemName(RPSDiscovery.CafeProbeBaseName, RPSDiscovery.ProbeString, psVdirType);
			probeDefinition.ServiceName = RPSDiscovery.Component.Name;
			probeDefinition.RecurrenceIntervalSeconds = probeRecurrenceIntervalSeconds;
			probeDefinition.TimeoutSeconds = Math.Min(180, probeRecurrenceIntervalSeconds);
			probeDefinition.MaxRetryAttempts = 3;
			probeDefinition.TargetResource = string.Format("{0}.{1}", rpsAuthType, account);
			probeDefinition.Account = account;
			probeDefinition.AccountPassword = accountPassword;
			probeDefinition.AccountDisplayName = account;
			probeDefinition.Attributes["AuthenticationType"] = rpsAuthType.ToString();
			probeDefinition.Attributes["TrustAnySslCertificate"] = true.ToString();
			probeDefinition.Attributes["MaximumConnectionRedirectionCount"] = "0";
			switch (rpsAuthType)
			{
			case AuthenticationMechanism.Default:
				probeDefinition.Endpoint = string.Format("https://{0}/powershell", Dns.GetHostEntry("LocalHost").HostName);
				break;
			case AuthenticationMechanism.Basic:
				probeDefinition.Endpoint = RPSDiscovery.RPSPSLiveIdURL;
				break;
			default:
				if (rpsAuthType != AuthenticationMechanism.Kerberos)
				{
					throw new ArgumentException(string.Format("Unhandled AuthenticationMechanism value = {0}", rpsAuthType), "rpsAuthType");
				}
				probeDefinition.Endpoint = string.Format("http://{0}/powershell", Dns.GetHostEntry("LocalHost").HostName);
				break;
			}
			return probeDefinition;
		}

		internal static ProbeDefinition CreateRPSBackEndLogonProbeDefinition(AccessTokenType tokenType, MailboxDatabaseInfo dbInfo, int probeRecurrenceIntervalSeconds)
		{
			RPSDiscovery.RPSVirtualDirectory psVdirType = (tokenType == AccessTokenType.CertificateSid || tokenType == AccessTokenType.Windows) ? RPSDiscovery.RPSVirtualDirectory.PSProxy : RPSDiscovery.RPSVirtualDirectory.PSLiveIDProxy;
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = Assembly.GetExecutingAssembly().Location;
			probeDefinition.TypeName = typeof(RPSBackEndLogonProbe).FullName;
			probeDefinition.Name = RPSDiscovery.BuildWorkItemName(RPSDiscovery.RPSDeeptTestString, RPSDiscovery.ProbeString, psVdirType);
			probeDefinition.ServiceName = RPSDiscovery.Component.Name;
			probeDefinition.RecurrenceIntervalSeconds = probeRecurrenceIntervalSeconds;
			probeDefinition.TimeoutSeconds = Math.Min(180, probeRecurrenceIntervalSeconds);
			probeDefinition.MaxRetryAttempts = 0;
			probeDefinition.TargetResource = string.Format("{0}.{1}", tokenType, dbInfo.MonitoringAccount);
			probeDefinition.Account = dbInfo.MonitoringAccount + "@" + dbInfo.MonitoringAccountDomain;
			probeDefinition.AccountPassword = dbInfo.MonitoringAccountPassword;
			probeDefinition.AccountDisplayName = dbInfo.MonitoringAccount;
			probeDefinition.Attributes["AccessTokenType"] = tokenType.ToString();
			switch (tokenType)
			{
			case AccessTokenType.Windows:
				probeDefinition.Endpoint = RPSDiscovery.RPSPSProxyURL;
				return probeDefinition;
			case AccessTokenType.LiveId:
				break;
			case AccessTokenType.LiveIdBasic:
				probeDefinition.Endpoint = RPSDiscovery.RPSPSLiveIdProxyURL;
				return probeDefinition;
			default:
				switch (tokenType)
				{
				case AccessTokenType.CertificateSid:
					probeDefinition.Endpoint = RPSDiscovery.RPSPSProxyURL;
					return probeDefinition;
				case AccessTokenType.RemotePowerShellDelegated:
					probeDefinition.Endpoint = RPSDiscovery.RPSPSLiveIdProxyURL;
					return probeDefinition;
				}
				break;
			}
			throw new ApplicationException(string.Format("Unhandled AccessTokenType:{0} to create RPSBackEndLogonProbe", tokenType));
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Entering DoWork", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 241);
			try
			{
				if (LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsCafeRoleInstalled && !LocalEndpointManager.IsDataCenter)
				{
					this.AddRPSCafeLogonProbe(false);
					this.AddRPSCafeMonitorsAndResponders(false);
				}
				if (LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled && LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsClientAccessRoleInstalled)
				{
					this.AddRPSBackEndLogonProbe();
					this.AddRPSBackEndMonitorAndResponder();
					if (!LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsCafeRoleInstalled)
					{
						this.AddRPSCafeLogonProbe(true);
						this.AddRPSCafeMonitorsAndResponders(true);
					}
					this.AddTipProbes();
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceError(ExTraceGlobals.RPSTracer, base.TraceContext, "RPSDiscovery.DoWork() failed: " + ex.ToString(), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 283);
				throw;
			}
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Leaving DoWork", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 287);
		}

		private void CreateRPSCafeLogonProbe(AuthenticationMechanism rpsAuthType, string account, string accountPassword, int probeRecurrenceIntervalSeconds)
		{
			ProbeDefinition probeDefinition = RPSDiscovery.CreateRPSCafeLogonProbeDefinition(rpsAuthType, account, accountPassword, probeRecurrenceIntervalSeconds);
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RPSTracer, base.TraceContext, "Create {0}", probeDefinition.Name, null, "CreateRPSCafeLogonProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 301);
		}

		private void AddRPSBackEndLogonProbe()
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Entering AddRPSBackEndLogonProbe", null, "AddRPSBackEndLogonProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 309);
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance.MailboxDatabaseEndpoint == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count == 0)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.RPSTracer, base.TraceContext, "no mailbox database found on this server", null, "AddRPSBackEndLogonProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 316);
				return;
			}
			int probeRecurrenceIntervalSeconds = 30 * instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count;
			MailboxDatabaseInfo mailboxDatabaseInfo = null;
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo2 in instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend)
			{
				if (string.IsNullOrEmpty(mailboxDatabaseInfo2.MonitoringAccount))
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.RPSTracer, base.TraceContext, "Ignore empty monitoring account for " + mailboxDatabaseInfo2.MailboxDatabaseName, null, "AddRPSBackEndLogonProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 328);
				}
				else
				{
					mailboxDatabaseInfo = mailboxDatabaseInfo2;
					if (this.ShallSkipKerberosProbe)
					{
						WTFDiagnostics.TraceInformation(ExTraceGlobals.RPSTracer, base.TraceContext, "Skip adding kerberos logon probe according system settings", null, "AddRPSBackEndLogonProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 335);
					}
					else
					{
						ProbeDefinition definition = RPSDiscovery.CreateRPSBackEndLogonProbeDefinition(AccessTokenType.Windows, mailboxDatabaseInfo2, probeRecurrenceIntervalSeconds);
						base.Broker.AddWorkDefinition<ProbeDefinition>(definition, base.TraceContext);
						WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RPSTracer, base.TraceContext, "Add Kerberos RPSBackEndLogonProbe for account :{0}", mailboxDatabaseInfo2.MonitoringAccount, null, "AddRPSBackEndLogonProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 341);
					}
					if (string.IsNullOrWhiteSpace(mailboxDatabaseInfo2.MonitoringAccountSid))
					{
						WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.RPSTracer, base.TraceContext, "Skip adding certificate logon probe due to missing mandatory fields Sid={0} Partition={1}", mailboxDatabaseInfo2.MonitoringAccountSid, mailboxDatabaseInfo2.MonitoringAccountPartitionId, null, "AddRPSBackEndLogonProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 346);
					}
					else
					{
						ProbeDefinition definition2 = RPSDiscovery.CreateRPSBackEndLogonProbeDefinition(AccessTokenType.CertificateSid, mailboxDatabaseInfo2, probeRecurrenceIntervalSeconds);
						base.Broker.AddWorkDefinition<ProbeDefinition>(definition2, base.TraceContext);
						WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RPSTracer, base.TraceContext, "Add Certificate RPSBackEndLogonProbe for account :{0}", mailboxDatabaseInfo2.MonitoringAccount, null, "AddRPSBackEndLogonProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 352);
					}
					if (LocalEndpointManager.IsDataCenter)
					{
						if (string.IsNullOrWhiteSpace(mailboxDatabaseInfo2.MonitoringAccountPuid) || string.IsNullOrWhiteSpace(mailboxDatabaseInfo2.MonitoringAccountPartitionId) || string.IsNullOrWhiteSpace(mailboxDatabaseInfo2.MonitoringAccountSid))
						{
							WTFDiagnostics.TraceInformation<string, string, string>(ExTraceGlobals.RPSTracer, base.TraceContext, "Skip adding LiveId logon probe due to missing mandatory fields Sid={0} Partition={1} Puid={2}", mailboxDatabaseInfo2.MonitoringAccountSid, mailboxDatabaseInfo2.MonitoringAccountPartitionId, mailboxDatabaseInfo2.MonitoringAccountPuid, null, "AddRPSBackEndLogonProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 361);
						}
						else
						{
							ProbeDefinition definition3 = RPSDiscovery.CreateRPSBackEndLogonProbeDefinition(AccessTokenType.LiveIdBasic, mailboxDatabaseInfo2, probeRecurrenceIntervalSeconds);
							base.Broker.AddWorkDefinition<ProbeDefinition>(definition3, base.TraceContext);
							WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RPSTracer, base.TraceContext, "Add LiveId RPSBackEndLogonProbe for account :{0}", mailboxDatabaseInfo2.MonitoringAccount, null, "AddRPSBackEndLogonProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 367);
						}
					}
				}
			}
			if (LocalEndpointManager.IsDataCenter && mailboxDatabaseInfo != null)
			{
				this.AddRPSBackEndDelegatedProbe(mailboxDatabaseInfo);
			}
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Leaving AddRPSBackEndLogonProbe", null, "AddRPSBackEndLogonProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 378);
		}

		private void AddRPSBackEndMonitorAndResponder()
		{
			MonitorStateTransition[] stateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, 1800)
			};
			RPSDiscovery.RPSVirtualDirectory[] array;
			if (LocalEndpointManager.IsDataCenter)
			{
				array = new RPSDiscovery.RPSVirtualDirectory[]
				{
					RPSDiscovery.RPSVirtualDirectory.PSProxy,
					RPSDiscovery.RPSVirtualDirectory.PSLiveIDProxy
				};
			}
			else
			{
				array = new RPSDiscovery.RPSVirtualDirectory[]
				{
					RPSDiscovery.RPSVirtualDirectory.PSProxy
				};
			}
			foreach (RPSDiscovery.RPSVirtualDirectory vdirType in array)
			{
				MonitorDefinition monitorDefinition = this.CreateRPSMonitorDefinition(vdirType, stateTransitions, true);
				monitorDefinition.ServicePriority = 1;
				monitorDefinition.ScenarioDescription = "Validate RPS health is not impacted by BE connectivity issues";
				base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
				base.Broker.AddWorkDefinition<ResponderDefinition>(this.CreateResetIISAppPoolResponderDefinition(vdirType), base.TraceContext);
				base.Broker.AddWorkDefinition<ResponderDefinition>(this.CreateEscalateResponderDefinition(vdirType), base.TraceContext);
			}
		}

		private void AddRPSBackEndDelegatedProbe(MailboxDatabaseInfo dbInfo)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Entering AddRPSBackEndDelegatedProbe", null, "AddRPSBackEndDelegatedProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 419);
			ADSessionSettings sessionSettings = ADSessionSettings.FromTenantAcceptedDomain(dbInfo.MonitoringAccountDomain);
			ITenantRecipientSession tenantRecipientSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(false, ConsistencyMode.IgnoreInvalid, sessionSettings, 424, "AddRPSBackEndDelegatedProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs");
			QueryFilter filter = new ExistsFilter(ADGroupSchema.LinkedPartnerGroupAndOrganizationId);
			ADRawEntry[] array = tenantRecipientSession.FindRecipient(null, QueryScope.SubTree, filter, null, 1, new PropertyDefinition[]
			{
				ADGroupSchema.LinkedPartnerGroupAndOrganizationId
			});
			if (array == null || array.Length < 1)
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RPSTracer, base.TraceContext, "Cannot find linked partner group in organization {0}. Skip adding delegated RPSBackEndLogonProbe", dbInfo.MonitoringAccountDomain, null, "AddRPSBackEndDelegatedProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 429);
				return;
			}
			LinkedPartnerGroupInformation linkedPartnerGroupInformation = (LinkedPartnerGroupInformation)array[0][ADGroupSchema.LinkedPartnerGroupAndOrganizationId];
			IIdentity delegatedIdentity = DelegatedPrincipal.GetDelegatedIdentity("mbx1@source2.org", linkedPartnerGroupInformation.LinkedPartnerOrganizationId, dbInfo.MonitoringAccountDomain, "mbx1", new string[]
			{
				linkedPartnerGroupInformation.LinkedPartnerGroupId
			});
			ProbeDefinition probeDefinition = RPSDiscovery.CreateRPSBackEndLogonProbeDefinition(AccessTokenType.RemotePowerShellDelegated, dbInfo, 300);
			probeDefinition.Attributes["DelegatedData"] = delegatedIdentity.Name;
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			WTFDiagnostics.TraceInformation<LinkedPartnerGroupInformation>(ExTraceGlobals.RPSTracer, base.TraceContext, "Add delegated RPSBackEndLogonProbe for LinkedPartnerGroup :{0}", linkedPartnerGroupInformation, null, "AddRPSBackEndDelegatedProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 441);
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Leaving AddRPSBackEndDelegatedProbe", null, "AddRPSBackEndDelegatedProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 442);
		}

		private void AddRPSCafeCertLogonProbe()
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Entering AddRPSCafeCertLogonProbe", null, "AddRPSCafeCertLogonProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 450);
			if (LocalEndpointManager.IsDataCenter)
			{
				string forwardSyncPartnerCertificateThumbprint = this.GetForwardSyncPartnerCertificateThumbprint();
				if (!string.IsNullOrEmpty(forwardSyncPartnerCertificateThumbprint))
				{
					this.CreateRPSCafeLogonProbe(AuthenticationMechanism.Default, forwardSyncPartnerCertificateThumbprint, null, this.ShallSkipKerberosProbe ? 120 : 300);
				}
			}
			else
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.RPSTracer, base.TraceContext, "Skip adding certificate RPSProbe in enterprise server", null, "AddRPSCafeCertLogonProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 467);
			}
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Leaving AddRPSCafeCertLogonProbe", null, "AddRPSCafeCertLogonProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 470);
		}

		private void AddRPSCafeLogonProbe(bool testAgainstBackendServer)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Entering AddRPSCafeLogonProbe", null, "AddRPSCafeLogonProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 481);
			ICollection<MailboxDatabaseInfo> collection = null;
			int probeRecurrenceIntervalSeconds = 0;
			if (LocalEndpointManager.Instance != null)
			{
				if (testAgainstBackendServer)
				{
					collection = LocalEndpointManager.Instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend;
					probeRecurrenceIntervalSeconds = 120 * LocalEndpointManager.Instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count;
					WTFDiagnostics.TraceInformation(ExTraceGlobals.RPSTracer, base.TraceContext, "RPSDiscovery.AddRPSCafeLogonProbe: Use MailboxDatabaseInfoCollectionForBackend while testing legacy powershell vdir", null, "AddRPSCafeLogonProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 496);
				}
				else
				{
					collection = LocalEndpointManager.Instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForCafe;
					probeRecurrenceIntervalSeconds = 120 * LocalEndpointManager.Instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForCafe.Count;
					WTFDiagnostics.TraceInformation(ExTraceGlobals.RPSTracer, base.TraceContext, "RPSDiscovery.AddRPSCafeLogonProbe: Use MailboxDatabaseInfoCollectionForCafe", null, "AddRPSCafeLogonProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 502);
				}
			}
			if (collection.Count == 0)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.RPSTracer, base.TraceContext, "RPSDiscovery.AddRPSCafeLogonProbe: no mailbox database found on this server", null, "AddRPSCafeLogonProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 508);
			}
			else if (this.ShallSkipKerberosProbe)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.RPSTracer, base.TraceContext, "RPSDiscovery.AddRPSCafeLogonProbe: Skip adding Kerberos Auth probes", null, "AddRPSCafeLogonProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 512);
			}
			else
			{
				foreach (MailboxDatabaseInfo mailboxDatabaseInfo in collection)
				{
					if (string.IsNullOrWhiteSpace(mailboxDatabaseInfo.MonitoringAccountPassword))
					{
						WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RPSTracer, base.TraceContext, "RPSDiscovery.AddRPSCafeLogonProbe: Ignore mailbox database {0} because it does not have monitoring mailbox", mailboxDatabaseInfo.MailboxDatabaseName, null, "AddRPSCafeLogonProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 520);
					}
					else
					{
						string account = mailboxDatabaseInfo.MonitoringAccount + "@" + mailboxDatabaseInfo.MonitoringAccountDomain;
						this.CreateRPSCafeLogonProbe(AuthenticationMechanism.Kerberos, account, mailboxDatabaseInfo.MonitoringAccountPassword, probeRecurrenceIntervalSeconds);
					}
				}
			}
			this.AddRPSCafeCertLogonProbe();
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Leaving AddRPSCafeLogonProbe", null, "AddRPSCafeLogonProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 532);
		}

		private void AddRPSCafeMonitorsAndResponders(bool testAgainstBackendServer)
		{
			MonitorStateTransition[] stateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, 1200)
			};
			base.Broker.AddWorkDefinition<MonitorDefinition>(this.CreateRPSMonitorDefinition(RPSDiscovery.RPSVirtualDirectory.PS, stateTransitions, false), base.TraceContext);
			if (testAgainstBackendServer)
			{
				base.Broker.AddWorkDefinition<ResponderDefinition>(this.CreateResetIISAppPoolResponderDefinition(RPSDiscovery.RPSVirtualDirectory.PS), base.TraceContext);
			}
			base.Broker.AddWorkDefinition<ResponderDefinition>(this.CreateEscalateResponderDefinition(RPSDiscovery.RPSVirtualDirectory.PS), base.TraceContext);
		}

		internal string GetForwardSyncPartnerCertificateThumbprint()
		{
			string text = null;
			try
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 569, "GetForwardSyncPartnerCertificateThumbprint", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs");
				ServiceEndpoint endpoint = topologyConfigurationSession.GetEndpointContainer().GetEndpoint("ForwardSyncRpsEndPoint");
				text = TlsCertificateInfo.FindFirstCertWithSubjectDistinguishedName(endpoint.CertificateSubject).Thumbprint;
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RPSTracer, base.TraceContext, "Found ForwardSync partner certificate, thumbprint = {0}", text, null, "GetForwardSyncPartnerCertificateThumbprint", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 575);
			}
			catch (ServiceEndpointNotFoundException arg)
			{
				WTFDiagnostics.TraceError<ServiceEndpointNotFoundException>(ExTraceGlobals.RPSTracer, base.TraceContext, "Failed to read ForwordSync partner certificate, Exception={0}", arg, null, "GetForwardSyncPartnerCertificateThumbprint", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 579);
			}
			catch (ArgumentException arg2)
			{
				WTFDiagnostics.TraceError<ArgumentException>(ExTraceGlobals.RPSTracer, base.TraceContext, "Failed to read ForwordSync partner certificate, Exception={0}", arg2, null, "GetForwardSyncPartnerCertificateThumbprint", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 583);
			}
			return text;
		}

		private T CreateBasicWorkDefinition<T>(Type workItemType) where T : WorkDefinition, new()
		{
			T result = Activator.CreateInstance<T>();
			result.AssemblyPath = workItemType.Assembly.Location;
			result.TypeName = workItemType.FullName;
			result.ServiceName = RPSDiscovery.Component.Name;
			return result;
		}

		private MonitorDefinition CreateRPSMonitorDefinition(RPSDiscovery.RPSVirtualDirectory vdirType, MonitorStateTransition[] stateTransitions, bool isBackEndMonitor)
		{
			string name;
			string sampleMask;
			if (vdirType == RPSDiscovery.RPSVirtualDirectory.PS || vdirType == RPSDiscovery.RPSVirtualDirectory.PSLiveID)
			{
				name = RPSDiscovery.BuildWorkItemName(RPSDiscovery.CafeProbeBaseName, RPSDiscovery.MonitorString, vdirType);
				sampleMask = RPSDiscovery.BuildWorkItemName(RPSDiscovery.CafeProbeBaseName, RPSDiscovery.ProbeString, vdirType);
			}
			else
			{
				name = RPSDiscovery.BuildWorkItemName(RPSDiscovery.RPSDeeptTestString, RPSDiscovery.MonitorString, vdirType);
				sampleMask = RPSDiscovery.BuildWorkItemName(RPSDiscovery.RPSDeeptTestString, RPSDiscovery.ProbeString, vdirType);
			}
			TimeSpan monitoringInterval;
			TimeSpan recurrenceInterval;
			TimeSpan secondaryMonitoringInterval;
			if (isBackEndMonitor)
			{
				monitoringInterval = TimeSpan.FromMinutes(1.0);
				recurrenceInterval = TimeSpan.FromMinutes(1.0);
				secondaryMonitoringInterval = TimeSpan.FromMinutes(5.0);
			}
			else
			{
				monitoringInterval = TimeSpan.FromMinutes(4.0);
				recurrenceInterval = TimeSpan.FromMinutes(4.0);
				secondaryMonitoringInterval = TimeSpan.FromMinutes(20.0);
			}
			MonitorDefinition monitorDefinition = OverallPercentSuccessByStateAttribute1Monitor.CreateDefinition(name, sampleMask, RPSDiscovery.Component.Name, RPSDiscovery.Component, 90.0, monitoringInterval, recurrenceInterval, secondaryMonitoringInterval, "", true);
			monitorDefinition.TimeoutSeconds = Math.Min(monitorDefinition.TimeoutSeconds, (int)recurrenceInterval.TotalSeconds);
			monitorDefinition.MonitorStateTransitions = stateTransitions;
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate RPS health is not impacted by any issues";
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RPSTracer, base.TraceContext, "Create {0}", monitorDefinition.Name, null, "CreateRPSMonitorDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 657);
			return monitorDefinition;
		}

		private void UpdateResponderDefinition(ResponderDefinition responder, RPSDiscovery.RPSVirtualDirectory vdirType, string responderType)
		{
			responder.Enabled = true;
			switch (vdirType)
			{
			case RPSDiscovery.RPSVirtualDirectory.PS:
				responder.Name = RPSDiscovery.BuildWorkItemName(RPSDiscovery.CafeProbeBaseName, responderType, vdirType);
				responder.Attributes["AppPoolName"] = "MSExchangePowerShellFrontEndAppPool";
				responder.AlertMask = RPSDiscovery.BuildWorkItemName(RPSDiscovery.CafeProbeBaseName, RPSDiscovery.MonitorString, vdirType);
				break;
			case RPSDiscovery.RPSVirtualDirectory.PSLiveID:
				responder.Name = RPSDiscovery.BuildWorkItemName(RPSDiscovery.CafeProbeBaseName, responderType, vdirType);
				responder.Attributes["AppPoolName"] = "MSExchangePowerShellLiveIDFrontEndAppPool";
				responder.AlertMask = RPSDiscovery.BuildWorkItemName(RPSDiscovery.CafeProbeBaseName, RPSDiscovery.MonitorString, vdirType);
				break;
			case RPSDiscovery.RPSVirtualDirectory.PSProxy:
				responder.Name = RPSDiscovery.BuildWorkItemName(RPSDiscovery.RPSDeeptTestString, responderType, vdirType);
				responder.Attributes["AppPoolName"] = "MSExchangePowerShellAppPool";
				responder.AlertMask = RPSDiscovery.BuildWorkItemName(RPSDiscovery.RPSDeeptTestString, RPSDiscovery.MonitorString, vdirType);
				break;
			case RPSDiscovery.RPSVirtualDirectory.PSLiveIDProxy:
				responder.Name = RPSDiscovery.BuildWorkItemName(RPSDiscovery.RPSDeeptTestString, responderType, vdirType);
				responder.Attributes["AppPoolName"] = "MSExchangePowerShellLiveIDAppPool";
				responder.AlertMask = RPSDiscovery.BuildWorkItemName(RPSDiscovery.RPSDeeptTestString, RPSDiscovery.MonitorString, vdirType);
				break;
			default:
				throw new ApplicationException("Unhandled RPSVirtualDirectory type :" + vdirType);
			}
			responder.AlertTypeId = responder.AlertMask;
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RPSTracer, base.TraceContext, "Create {0}", responder.Name, null, "UpdateResponderDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 697);
		}

		private ResponderDefinition CreateResetIISAppPoolResponderDefinition(RPSDiscovery.RPSVirtualDirectory vdirType)
		{
			ResponderDefinition responderDefinition = this.CreateBasicWorkDefinition<ResponderDefinition>(typeof(ResetIISAppPoolResponder));
			responderDefinition.RecurrenceIntervalSeconds = 60;
			responderDefinition.TimeoutSeconds = 60;
			responderDefinition.WaitIntervalSeconds = 900;
			responderDefinition.TargetHealthState = ServiceHealthStatus.Degraded;
			this.UpdateResponderDefinition(responderDefinition, vdirType, RPSDiscovery.ResetIISAppPoolString);
			return responderDefinition;
		}

		private ResponderDefinition CreateEscalateResponderDefinition(RPSDiscovery.RPSVirtualDirectory vdirType)
		{
			ResponderDefinition responderDefinition = RpsEscalateResponder.CreateDefinition("Name", RPSDiscovery.Component.Name, "AlertTypeId", "AlertMask", vdirType.ToString(), ServiceHealthStatus.Unrecoverable, RPSDiscovery.Component.Service, RPSDiscovery.Component.EscalationTeam, Strings.RpsFailedEscalationMessage(vdirType.ToString()), Strings.RpsFailedEscalationMessage(vdirType.ToString()) + Environment.NewLine + "{Probe.Exception}", true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.RecurrenceIntervalSeconds = 60;
			responderDefinition.TimeoutSeconds = 60;
			responderDefinition.WaitIntervalSeconds = 3600;
			this.UpdateResponderDefinition(responderDefinition, vdirType, RPSDiscovery.EscalateString);
			return responderDefinition;
		}

		private void AddTipProbes()
		{
			if (!LocalEndpointManager.IsDataCenter)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.RPSTracer, base.TraceContext, "Skip adding TIP probes in non-datacenter environment", null, "AddTipProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 751);
				return;
			}
			string text = "RPSTIPProbe";
			string text2 = "RPSTIPMonitor";
			string name = "RPSTIPResponder";
			Type[] array = new Type[]
			{
				typeof(MailboxTipProbe),
				typeof(MailUserTipProbe),
				typeof(MailContactTipProbe),
				typeof(ContactTipProbe),
				typeof(AutogroupTipProbe),
				typeof(MailboxPermissionTipProbe),
				typeof(RecipientTipProbe),
				typeof(CASMailboxTipProbe),
				typeof(LinkedUserTipProbe),
				typeof(DistributionGroupProbe),
				typeof(DistributionGroupMemberProbe),
				typeof(DynamicDistributionGroupProbe),
				typeof(GroupProbe),
				typeof(AddressListTipProbe),
				typeof(GlobalAddressListTipProbe),
				typeof(UserTipProbe)
			};
			foreach (Type probeType in array)
			{
				ProbeDefinition definition = this.CreateProbeDefinition(probeType, text);
				base.Broker.AddWorkDefinition<ProbeDefinition>(definition, base.TraceContext);
			}
			MonitorDefinition monitorDefinition = OverallPercentSuccessByStateAttribute1Monitor.CreateDefinition(text2, text, RPSDiscovery.Component.Name, RPSDiscovery.Component, 90.0, TimeSpan.FromMinutes(10.0), TimeSpan.FromMinutes(10.0), TimeSpan.FromMinutes(20.0), string.Empty, false);
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate RPS health is not impacted by any issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition definition2 = EscalateResponder.CreateDefinition(name, RPSDiscovery.Component.Name, text2, text2, string.Empty, ServiceHealthStatus.Unrecoverable, RPSDiscovery.Component.EscalationTeam, "RPS TIP Probe Failed", "{Probe.Exception}", false, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition2, base.TraceContext);
		}

		private ProbeDefinition CreateProbeDefinition(Type probeType, string probeBaseName)
		{
			return new ProbeDefinition
			{
				AssemblyPath = Assembly.GetExecutingAssembly().Location,
				TypeName = probeType.FullName,
				Name = probeBaseName,
				ServiceName = ExchangeComponent.Rps.Name,
				Enabled = false,
				TimeoutSeconds = 600,
				RecurrenceIntervalSeconds = 600,
				TargetResource = probeType.Name
			};
		}

		private void CheckMonitoringAccountKerberosAuth()
		{
			this.shallSkipKerberosProbe = new bool?(false);
			if (LocalEndpointManager.IsDataCenter)
			{
				MailboxDatabaseInfo mailboxDatabaseInfo = null;
				if (LocalEndpointManager.Instance.MailboxDatabaseEndpoint != null && LocalEndpointManager.Instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend != null)
				{
					mailboxDatabaseInfo = LocalEndpointManager.Instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.FirstOrDefault<MailboxDatabaseInfo>();
				}
				if (mailboxDatabaseInfo != null)
				{
					bool flag = false;
					string text = mailboxDatabaseInfo.MonitoringAccount + "@" + mailboxDatabaseInfo.MonitoringAccountDomain;
					try
					{
						using (new WindowsIdentity(text))
						{
							flag = true;
						}
					}
					catch (Exception arg)
					{
						WTFDiagnostics.TraceDebug<string, Exception>(ExTraceGlobals.RPSTracer, base.TraceContext, "new WindowsIdentity('{0}') failed, Exception={1}", text, arg, null, "CheckMonitoringAccountKerberosAuth", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 858);
					}
					if (!flag)
					{
						this.shallSkipKerberosProbe = new bool?(true);
						WTFDiagnostics.TraceInformation(ExTraceGlobals.RPSTracer, base.TraceContext, "Set ShallSkipKerberosProbe = true", null, "CheckMonitoringAccountKerberosAuth", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 865);
					}
				}
			}
		}

		internal static readonly string RPSPSProxyURL = "https://localhost:444/powershell";

		internal static readonly string RPSPSLiveIdProxyURL = "https://localhost:444/powershell-liveid";

		internal static readonly string RPSPSURL = "https://localhost/powershell";

		internal static readonly string RPSPSLiveIdURL = "https://localhost/powershell-liveid";

		internal static readonly string RPSDeeptTestString = "RpsDeepTest";

		internal static readonly string RPSCTPString = "RpsCtp";

		internal static readonly string ProbeString = "Probe";

		internal static readonly string MonitorString = "Monitor";

		internal static readonly string ResetIISAppPoolString = "Restart";

		internal static readonly string EscalateString = "Escalate";

		private bool? shallSkipKerberosProbe;

		internal enum RPSVirtualDirectory
		{
			PS,
			PSLiveID,
			PSProxy,
			PSLiveIDProxy
		}
	}
}
