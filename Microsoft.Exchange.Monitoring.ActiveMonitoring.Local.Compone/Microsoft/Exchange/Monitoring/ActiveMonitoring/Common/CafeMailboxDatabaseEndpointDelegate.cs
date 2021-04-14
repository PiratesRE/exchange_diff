using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class CafeMailboxDatabaseEndpointDelegate : MailboxDatabaseEndpointDelegate
	{
		public override void Initialize(List<MailboxDatabaseInfo> validMailboxDatabases, List<MailboxDatabaseInfo> unverifiedMailBoxDataBases)
		{
			WTFDiagnostics.TraceFunction(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Initialize mailbox database monitoring endpoint for Cafe", null, "Initialize", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\CafeMailboxDatabaseEndpointDelegate.cs", 60);
			int cafeMailboxes = Settings.CafeMailboxes;
			MailboxDatabase[] array = null;
			int num = 0;
			for (int i = 1; i <= cafeMailboxes; i++)
			{
				MailboxDatabase database = null;
				string monitoringMailboxName = this.GetMonitoringMailboxName(i);
				ADUser aduser = DirectoryAccessor.Instance.SearchMonitoringMailbox(monitoringMailboxName, null, ref database);
				if (aduser == null)
				{
					if (array == null)
					{
						array = DirectoryAccessor.Instance.GetCandidateMailboxDatabases(cafeMailboxes);
					}
					if (array != null && array.Length > 0)
					{
						database = array[num];
						num = (num + 1) % array.Length;
					}
					if (database != null)
					{
						aduser = this.CreateMonitoringMailbox(monitoringMailboxName, database);
					}
				}
				if (aduser != null && database != null)
				{
					if (!unverifiedMailBoxDataBases.Exists((MailboxDatabaseInfo element) => element.MailboxDatabaseGuid == database.Guid))
					{
						MailboxDatabaseInfo mailboxDatabaseInfo = new MailboxDatabaseInfo();
						mailboxDatabaseInfo.MailboxDatabaseName = database.Name;
						mailboxDatabaseInfo.MailboxDatabaseGuid = database.Guid;
						lock (this.token)
						{
							unverifiedMailBoxDataBases.Add(mailboxDatabaseInfo);
						}
						bool flag2 = false;
						string text = null;
						try
						{
							MailboxDatabaseEndpointDelegate.FillOutMonitoringAccountInfo(mailboxDatabaseInfo, aduser);
							mailboxDatabaseInfo.SystemMailboxGuid = DirectoryAccessor.Instance.GetSystemMailboxGuid(database.Guid);
							MailboxDatabaseEndpointDelegate.ValidateMonitoringMailboxBasicTests(mailboxDatabaseInfo);
							this.GetAndVerifyMonitoringMailboxPassword(aduser, mailboxDatabaseInfo);
							if (mailboxDatabaseInfo.AuthenticationResult == LiveIdAuthResult.Success)
							{
								StringBuilder stringBuilder = new StringBuilder();
								stringBuilder.Append("CafeMailboxDatabaseEndpointDelegate.Initialize: Adding the following database to the endpoint: ");
								stringBuilder.AppendFormat("Front-end server GUID: '{0}' ", DirectoryAccessor.Instance.Server.Guid);
								stringBuilder.AppendFormat("MailboxDatabaseName: '{0}' ", mailboxDatabaseInfo.MailboxDatabaseName ?? string.Empty);
								stringBuilder.AppendFormat("MailboxDatabaseGuid: '{0}' ", mailboxDatabaseInfo.MailboxDatabaseGuid);
								stringBuilder.AppendFormat("MonitoringAccount: '{0}' ", mailboxDatabaseInfo.MonitoringAccount ?? string.Empty);
								stringBuilder.AppendFormat("MonitoringAccountDisplayName: '{0}' ", mailboxDatabaseInfo.MonitoringAccountDisplayName ?? string.Empty);
								stringBuilder.AppendFormat("MonitoringAccountDomain: '{0}' ", mailboxDatabaseInfo.MonitoringAccountDomain ?? string.Empty);
								stringBuilder.AppendFormat("MonitoringAccountUserPrincipalName: '{0}'", mailboxDatabaseInfo.MonitoringAccountUserPrincipalName ?? string.Empty);
								WTFDiagnostics.TraceInformation(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, stringBuilder.ToString(), null, "Initialize", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\CafeMailboxDatabaseEndpointDelegate.cs", 150);
								lock (this.token)
								{
									validMailboxDatabases.Add(mailboxDatabaseInfo);
								}
								flag2 = true;
							}
						}
						catch (Exception ex)
						{
							text = ex.ToString();
						}
						if (!flag2)
						{
							StringBuilder stringBuilder2 = new StringBuilder();
							stringBuilder2.Append("CafeMailboxDatabaseEndpointDelegate.Initialize: Failed to verify the dedicated CAFE monitoring mailbox. ");
							stringBuilder2.AppendFormat("Front-end server GUID: '{0}' ", DirectoryAccessor.Instance.Server.Guid);
							stringBuilder2.AppendFormat("MailboxDatabaseName: '{0}' ", mailboxDatabaseInfo.MailboxDatabaseName ?? string.Empty);
							stringBuilder2.AppendFormat("MailboxDatabaseGuid: '{0}' ", mailboxDatabaseInfo.MailboxDatabaseGuid);
							stringBuilder2.AppendFormat("MonitoringAccount: '{0}' ", mailboxDatabaseInfo.MonitoringAccount ?? string.Empty);
							stringBuilder2.AppendFormat("MonitoringAccountDisplayName: '{0}' ", mailboxDatabaseInfo.MonitoringAccountDisplayName ?? string.Empty);
							stringBuilder2.AppendFormat("MonitoringAccountDomain: '{0}' ", mailboxDatabaseInfo.MonitoringAccountDomain ?? string.Empty);
							stringBuilder2.AppendFormat("MonitoringAccountUserPrincipalName: '{0}' ", mailboxDatabaseInfo.MonitoringAccountUserPrincipalName ?? string.Empty);
							stringBuilder2.AppendFormat("Exception text: {0}", text ?? "No exception thrown to this frame. Look at preceding log entries for exceptions caught in previous frames.");
							WTFDiagnostics.TraceError(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, stringBuilder2.ToString(), null, "Initialize", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\CafeMailboxDatabaseEndpointDelegate.cs", 178);
						}
					}
				}
				else
				{
					StringBuilder stringBuilder3 = new StringBuilder();
					stringBuilder3.AppendFormat("CafeMailboxDatabaseEndpointDelegate.Initialize: Failed to find or create CAFE monitoring mailbox '{0}'", monitoringMailboxName);
					WTFDiagnostics.TraceError(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, stringBuilder3.ToString(), null, "Initialize", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\CafeMailboxDatabaseEndpointDelegate.cs", 185);
				}
			}
		}

		public override bool DetectChange(List<MailboxDatabaseInfo> memoryDatabases)
		{
			return false;
		}

		private void GetAndVerifyMonitoringMailboxPassword(ADUser adUser, MailboxDatabaseInfo dbInfo)
		{
			WTFDiagnostics.TraceInformation<string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, "GetAndVerifyMonitoringMailboxPassword for mailbox '{0}'", adUser.UserPrincipalName, null, "GetAndVerifyMonitoringMailboxPassword", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\CafeMailboxDatabaseEndpointDelegate.cs", 210);
			string text = null;
			LiveIdAuthResult liveIdAuthResult = LiveIdAuthResult.InvalidCreds;
			string text2 = null;
			if (MailboxDatabaseEndpointDelegate.RunningInDatacenter)
			{
				text = base.GetStoredMonitoringMailboxPassword(adUser, this.traceContext);
				if (string.IsNullOrEmpty(text))
				{
					text = null;
				}
				else
				{
					WTFDiagnostics.TraceInformation<string, string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, "GetAndVerifyMonitoringMailboxPassword: Retrieved stored password '{0}' for mailbox '{1}'.", text, adUser.UserPrincipalName, null, "GetAndVerifyMonitoringMailboxPassword", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\CafeMailboxDatabaseEndpointDelegate.cs", 227);
					liveIdAuthResult = base.AuthenticateTestForDataCenterMailbox(adUser.UserPrincipalName, text, this.traceContext, out text2);
					if (liveIdAuthResult == LiveIdAuthResult.Success)
					{
						WTFDiagnostics.TraceInformation<string, string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, "GetAndVerifyMonitoringMailboxPassword: Stored password '{0}' successfully validated for mailbox '{1}'.", text, adUser.UserPrincipalName, null, "GetAndVerifyMonitoringMailboxPassword", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\CafeMailboxDatabaseEndpointDelegate.cs", 235);
					}
					else
					{
						WTFDiagnostics.TraceInformation<string, string, LiveIdAuthResult, string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, "GetAndVerifyMonitoringMailboxPassword: Stored password '{0}' failed validation for mailbox '{1}'. Authentication result: {2}. Error: {3}", text, adUser.UserPrincipalName, liveIdAuthResult, text2, null, "GetAndVerifyMonitoringMailboxPassword", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\CafeMailboxDatabaseEndpointDelegate.cs", 240);
					}
				}
			}
			if (text == null || liveIdAuthResult != LiveIdAuthResult.Success)
			{
				string arg;
				text = this.ResetMonitoringMailboxPassword(adUser, out arg);
				if (text != null)
				{
					WTFDiagnostics.TraceInformation<string, string, string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, "GetAndVerifyMonitoringMailboxPassword: Server '{0}' returned new password '{1}' for mailbox '{2}'", arg, text, adUser.UserPrincipalName, null, "GetAndVerifyMonitoringMailboxPassword", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\CafeMailboxDatabaseEndpointDelegate.cs", 256);
					if (MailboxDatabaseEndpointDelegate.RunningInDatacenter)
					{
						liveIdAuthResult = base.AuthenticateTestForDataCenterMailbox(adUser.UserPrincipalName, text, this.traceContext, out text2);
						if (liveIdAuthResult == LiveIdAuthResult.Success)
						{
							WTFDiagnostics.TraceInformation<string, string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, "GetAndVerifyMonitoringMailboxPassword: New password '{0}' successfully validated for mailbox '{1}'.", text, adUser.UserPrincipalName, null, "GetAndVerifyMonitoringMailboxPassword", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\CafeMailboxDatabaseEndpointDelegate.cs", 265);
						}
						else
						{
							WTFDiagnostics.TraceError<string, string, string, LiveIdAuthResult, string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, "GetAndVerifyMonitoringMailboxPassword: Back-end server '{0}' gave us password '{1}' for monitoring mailbox '{2}' but it did not work. Result: {3}. Error: {4}", arg, text, adUser.UserPrincipalName, liveIdAuthResult, text2, null, "GetAndVerifyMonitoringMailboxPassword", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\CafeMailboxDatabaseEndpointDelegate.cs", 270);
						}
					}
					else
					{
						WTFDiagnostics.TraceInformation(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, "GetAndVerifyMonitoringMailboxPassword: Skipping LiveID validation test as we are using Active Directory for authentication.", null, "GetAndVerifyMonitoringMailboxPassword", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\CafeMailboxDatabaseEndpointDelegate.cs", 276);
						liveIdAuthResult = LiveIdAuthResult.Success;
					}
				}
				else
				{
					WTFDiagnostics.TraceError<string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, "GetAndVerifyMonitoringMailboxPassword: Failed to obtain any password for mailbox '{0}'.", adUser.UserPrincipalName, null, "GetAndVerifyMonitoringMailboxPassword", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\CafeMailboxDatabaseEndpointDelegate.cs", 285);
					liveIdAuthResult = LiveIdAuthResult.InvalidCreds;
					text2 = "Failed to generate or apply new password to mailbox.";
				}
			}
			dbInfo.MonitoringAccountPassword = text;
			dbInfo.AuthenticationResult = liveIdAuthResult;
			dbInfo.AuthenticationError = text2;
		}

		private bool GetActiveServerForDatabase(Guid databaseGuid, out Server server)
		{
			server = null;
			string serverFqdnForDatabase = DirectoryAccessor.Instance.GetServerFqdnForDatabase(databaseGuid);
			if (serverFqdnForDatabase == null)
			{
				return false;
			}
			server = DirectoryAccessor.Instance.GetExchangeServerByName(serverFqdnForDatabase);
			return server != null && string.Compare(server.SerialNumber, CafeMailboxDatabaseEndpointDelegate.versionCutoff.ToString(true), true) >= 0;
		}

		private bool GetActiveServerForMonitoringMailbox(ADUser monitoringMailbox, out Server server)
		{
			Guid objectGuid = monitoringMailbox.Database.ObjectGuid;
			return this.GetActiveServerForDatabase(objectGuid, out server);
		}

		private string ResetMonitoringMailboxPassword(ADUser monitoringMailbox, out string serverName)
		{
			string result = null;
			Server server;
			if (this.GetActiveServerForMonitoringMailbox(monitoringMailbox, out server))
			{
				serverName = server.Fqdn;
				try
				{
					WTFDiagnostics.TraceDebug<string, string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Request credential from server {0} for monitoring mailbox {1}", serverName, monitoringMailbox.UserPrincipalName, null, "ResetMonitoringMailboxPassword", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\CafeMailboxDatabaseEndpointDelegate.cs", 358);
					using (ActiveMonitoringRpcClient activeMonitoringRpcClient = new ActiveMonitoringRpcClient(serverName))
					{
						activeMonitoringRpcClient.RequestCredential(monitoringMailbox.Database.ObjectGuid, monitoringMailbox.UserPrincipalName, ref result);
					}
					return result;
				}
				catch (RpcException arg)
				{
					WTFDiagnostics.TraceError<string, string, RpcException>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Request credential from server {0} for monitoring mailbox {1} failed with error {2}", serverName, monitoringMailbox.UserPrincipalName, arg, null, "ResetMonitoringMailboxPassword", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\CafeMailboxDatabaseEndpointDelegate.cs", 370);
					return null;
				}
			}
			serverName = Environment.MachineName;
			result = DirectoryAccessor.Instance.GetMonitoringMailboxCredential(monitoringMailbox);
			return result;
		}

		private ADUser CreateMonitoringMailbox(string displayName, MailboxDatabase database)
		{
			Server server;
			if (this.GetActiveServerForDatabase(database.Guid, out server))
			{
				string fqdn = server.Fqdn;
				try
				{
					WTFDiagnostics.TraceDebug<string, string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Request create monitoring mailbox from server {0} for monitoring mailbox {1}", fqdn, displayName, null, "CreateMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\CafeMailboxDatabaseEndpointDelegate.cs", 409);
					using (ActiveMonitoringRpcClient activeMonitoringRpcClient = new ActiveMonitoringRpcClient(fqdn))
					{
						activeMonitoringRpcClient.CreateMonitoringMailbox(displayName, database.Guid);
					}
					return DirectoryAccessor.Instance.SearchMonitoringMailbox(displayName, null, ref database);
				}
				catch (RpcException arg)
				{
					WTFDiagnostics.TraceError<string, string, RpcException>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Request create monitoring mailbox from server {0} for monitoring mailbox {1} failed with error {2}", fqdn, displayName, arg, null, "CreateMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\CafeMailboxDatabaseEndpointDelegate.cs", 421);
					return null;
				}
			}
			string text = null;
			return DirectoryAccessor.Instance.CreateMonitoringMailbox(displayName, database, out text);
		}

		private string GetMonitoringMailboxName(int serialNumber)
		{
			if (serialNumber < 1 || serialNumber > 999)
			{
				throw new ArgumentException("Mailbox serial number must be between 001 and 999");
			}
			string hostName = Dns.GetHostName();
			string arg = serialNumber.ToString("D3");
			return string.Format("HealthMailbox-{0}-{1}", hostName, arg);
		}

		private static readonly ServerVersion versionCutoff = new ServerVersion(15, 0, 830, 0);

		private TracingContext traceContext = TracingContext.Default;

		private object token = new object();
	}
}
