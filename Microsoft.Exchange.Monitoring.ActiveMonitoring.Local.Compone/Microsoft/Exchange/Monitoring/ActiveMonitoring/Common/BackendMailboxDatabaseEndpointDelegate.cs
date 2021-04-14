using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class BackendMailboxDatabaseEndpointDelegate : MailboxDatabaseEndpointDelegate
	{
		public override void Initialize(List<MailboxDatabaseInfo> validMailboxDatabases, List<MailboxDatabaseInfo> unverifiedMailBoxDataBases)
		{
			DatabaseCopy[] mailboxDatabaseCopies = DirectoryAccessor.Instance.GetMailboxDatabaseCopies();
			bool flag = true;
			if (flag)
			{
				List<Task> list = new List<Task>();
				foreach (DatabaseCopy state in mailboxDatabaseCopies)
				{
					Task item = Task.Factory.StartNew(delegate(object dbCopy)
					{
						this.InitializeMailboxForDatabaseCopy((DatabaseCopy)dbCopy, validMailboxDatabases, unverifiedMailBoxDataBases);
					}, state);
					list.Add(item);
				}
				Task.WaitAll(list.ToArray());
				return;
			}
			foreach (DatabaseCopy copy in mailboxDatabaseCopies)
			{
				this.InitializeMailboxForDatabaseCopy(copy, validMailboxDatabases, unverifiedMailBoxDataBases);
			}
		}

		public override bool DetectChange(List<MailboxDatabaseInfo> mailboxDatabases)
		{
			DatabaseCopy[] mailboxDatabaseCopies = DirectoryAccessor.Instance.GetMailboxDatabaseCopies();
			if (mailboxDatabaseCopies != null && mailboxDatabaseCopies.Length != mailboxDatabases.Count + this.ignoredDatabaseCount)
			{
				return true;
			}
			if (mailboxDatabaseCopies == null && mailboxDatabases.Count > 0)
			{
				return true;
			}
			foreach (DatabaseCopy databaseCopy in mailboxDatabaseCopies)
			{
				MailboxDatabase mailboxDatabaseFromCopy = DirectoryAccessor.Instance.GetMailboxDatabaseFromCopy(databaseCopy);
				if (mailboxDatabaseFromCopy != null)
				{
					MailboxDatabaseInfo match = this.FindMatch(mailboxDatabaseFromCopy, mailboxDatabases);
					if (this.DatabaseChanged(mailboxDatabaseFromCopy, match))
					{
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.Append("BackendMailboxDatabaseEndpointDelegate.DetectChange: Detected a new database copy\r\n\r\n");
						stringBuilder.AppendFormat("Database copy name: {0}\r\n", databaseCopy.Identity);
						stringBuilder.AppendFormat("Database copy GUID: {0}\r\n", databaseCopy.Guid);
						stringBuilder.AppendFormat("Database name: {0}\r\n", mailboxDatabaseFromCopy.Name ?? string.Empty);
						stringBuilder.AppendFormat("Database GUID: {0}\r\n", mailboxDatabaseFromCopy.Guid);
						WTFDiagnostics.TraceInformation(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, stringBuilder.ToString(), null, "DetectChange", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\BackendMailboxDatabaseEndpointDelegate.cs", 141);
						return true;
					}
				}
			}
			return false;
		}

		protected bool DatabaseChanged(MailboxDatabase database, MailboxDatabaseInfo match)
		{
			return !DatabaseTasksHelper.IsMailboxDatabaseExcludedFromMonitoring(database) && match == null;
		}

		private MailboxDatabaseInfo FindMatch(MailboxDatabase database, List<MailboxDatabaseInfo> list)
		{
			MailboxDatabaseInfo result = null;
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in list)
			{
				if (mailboxDatabaseInfo.MailboxDatabaseGuid == database.Guid)
				{
					result = mailboxDatabaseInfo;
					break;
				}
			}
			return result;
		}

		private void InitializeMailboxForDatabaseCopy(DatabaseCopy copy, List<MailboxDatabaseInfo> validMailboxDatabases, List<MailboxDatabaseInfo> unverifiedMailBoxDataBases)
		{
			string databaseName = copy.DatabaseName;
			WTFDiagnostics.TraceInformation<Guid, string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Checking mailbox database copy {0} for database {1}", copy.Guid, databaseName, null, "InitializeMailboxForDatabaseCopy", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\BackendMailboxDatabaseEndpointDelegate.cs", 209);
			MailboxDatabase mailboxDatabaseFromCopy = DirectoryAccessor.Instance.GetMailboxDatabaseFromCopy(copy);
			if (mailboxDatabaseFromCopy == null || DatabaseTasksHelper.IsMailboxDatabaseExcludedFromMonitoring(mailboxDatabaseFromCopy))
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("BackendMailboxDatabaseEndpointDelegate.Initialize: Ignoring database copy because it is either excluded from monitoring or not really a mailbox database copy. ");
				stringBuilder.AppendFormat("Database copy name: '{0}' ", copy.Identity);
				stringBuilder.AppendFormat("Database copy GUID: '{0}' ", copy.Guid);
				WTFDiagnostics.TraceInformation<ObjectId>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Ignore database copy '{0}' because it is either excluded from monitoring or not really a mailbox database copy", copy.Identity, null, "InitializeMailboxForDatabaseCopy", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\BackendMailboxDatabaseEndpointDelegate.cs", 221);
				Interlocked.Increment(ref this.ignoredDatabaseCount);
				return;
			}
			MailboxDatabaseInfo mailboxDatabaseInfo = new MailboxDatabaseInfo();
			mailboxDatabaseInfo.MailboxDatabaseName = mailboxDatabaseFromCopy.Name;
			mailboxDatabaseInfo.MailboxDatabaseGuid = mailboxDatabaseFromCopy.Guid;
			lock (this.token)
			{
				unverifiedMailBoxDataBases.Add(mailboxDatabaseInfo);
			}
			bool flag2 = false;
			string text = null;
			try
			{
				string monitoringMailboxName = this.GetMonitoringMailboxName(databaseName);
				string password = null;
				ADUser aduser = DirectoryAccessor.Instance.SearchMonitoringMailbox(monitoringMailboxName, null, ref mailboxDatabaseFromCopy);
				if (aduser == null)
				{
					aduser = DirectoryAccessor.Instance.CreateMonitoringMailbox(monitoringMailboxName, mailboxDatabaseFromCopy, out password);
				}
				else
				{
					if (aduser.MailboxProvisioningConstraint != null)
					{
						if (!string.IsNullOrWhiteSpace(aduser.MailboxProvisioningConstraint.Value))
						{
							goto IL_1A9;
						}
					}
					try
					{
						ADUser aduser2 = DirectoryAccessor.Instance.StampProvisioningConstraint(aduser.UserPrincipalName);
						aduser = aduser2;
					}
					catch (Exception ex)
					{
						WTFDiagnostics.TraceError<string, string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Tried and failed to stamp provisioning constraint on mailbox '{0}' Exception: {1}", aduser.UserPrincipalName, ex.ToString(), null, "InitializeMailboxForDatabaseCopy", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\BackendMailboxDatabaseEndpointDelegate.cs", 267);
					}
				}
				IL_1A9:
				if (aduser != null)
				{
					MailboxDatabaseEndpointDelegate.FillOutMonitoringAccountInfo(mailboxDatabaseInfo, aduser);
					mailboxDatabaseInfo.SystemMailboxGuid = DirectoryAccessor.Instance.GetSystemMailboxGuid(mailboxDatabaseFromCopy.Guid);
					MailboxDatabaseEndpointDelegate.ValidateMonitoringMailboxBasicTests(mailboxDatabaseInfo);
					this.GetAndVerifyMonitoringMailboxPassword(aduser, mailboxDatabaseInfo, password);
					if (mailboxDatabaseInfo.AuthenticationResult == LiveIdAuthResult.Success)
					{
						StringBuilder stringBuilder2 = new StringBuilder();
						stringBuilder2.Append("BackendMailboxDatabaseEndpointDelegate.Initialize: Adding the following database to the endpoint: ");
						stringBuilder2.AppendFormat("Database copy name: '{0}' ", copy.Identity);
						stringBuilder2.AppendFormat("Database copy GUID: '{0}' ", copy.Guid);
						stringBuilder2.AppendFormat("MailboxDatabaseName: '{0}' ", mailboxDatabaseInfo.MailboxDatabaseName ?? string.Empty);
						stringBuilder2.AppendFormat("MailboxDatabaseGuid: '{0}' ", mailboxDatabaseInfo.MailboxDatabaseGuid);
						stringBuilder2.AppendFormat("MonitoringAccount: '{0}' ", mailboxDatabaseInfo.MonitoringAccount ?? string.Empty);
						stringBuilder2.AppendFormat("MonitoringAccountDisplayName: '{0}' ", mailboxDatabaseInfo.MonitoringAccountDisplayName ?? string.Empty);
						stringBuilder2.AppendFormat("MonitoringAccountDomain: '{0}' ", mailboxDatabaseInfo.MonitoringAccountDomain ?? string.Empty);
						stringBuilder2.AppendFormat("MonitoringAccountUserPrincipalName: '{0}'", mailboxDatabaseInfo.MonitoringAccountUserPrincipalName ?? string.Empty);
						WTFDiagnostics.TraceInformation(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, stringBuilder2.ToString(), null, "InitializeMailboxForDatabaseCopy", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\BackendMailboxDatabaseEndpointDelegate.cs", 304);
						lock (this.token)
						{
							validMailboxDatabases.Add(mailboxDatabaseInfo);
						}
						flag2 = true;
					}
				}
			}
			catch (Exception ex2)
			{
				text = ex2.ToString();
			}
			if (!flag2)
			{
				StringBuilder stringBuilder3 = new StringBuilder();
				stringBuilder3.Append("BackendMailboxDatabaseEndpointDelegate.Initialize: Failed to verify mailbox. ");
				stringBuilder3.AppendFormat("Database copy name: '{0}' ", copy.Identity);
				stringBuilder3.AppendFormat("Database copy GUID: '{0}' ", copy.Guid);
				stringBuilder3.AppendFormat("MailboxDatabaseName: '{0}' ", mailboxDatabaseInfo.MailboxDatabaseName ?? string.Empty);
				stringBuilder3.AppendFormat("MailboxDatabaseGuid: '{0}' ", mailboxDatabaseInfo.MailboxDatabaseGuid);
				stringBuilder3.AppendFormat("MonitoringAccount: '{0}' ", mailboxDatabaseInfo.MonitoringAccount ?? string.Empty);
				stringBuilder3.AppendFormat("MonitoringAccountDisplayName: '{0}' ", mailboxDatabaseInfo.MonitoringAccountDisplayName ?? string.Empty);
				stringBuilder3.AppendFormat("MonitoringAccountDomain: '{0}' ", mailboxDatabaseInfo.MonitoringAccountDomain ?? string.Empty);
				stringBuilder3.AppendFormat("MonitoringAccountUserPrincipalName: '{0}' ", mailboxDatabaseInfo.MonitoringAccountUserPrincipalName ?? string.Empty);
				stringBuilder3.AppendFormat("Exception text: {0}", text ?? "No exception thrown to this frame. Look at preceding log entries for exceptions caught in previous frames.");
				WTFDiagnostics.TraceError(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, stringBuilder3.ToString(), null, "InitializeMailboxForDatabaseCopy", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\BackendMailboxDatabaseEndpointDelegate.cs", 333);
			}
		}

		private void GetAndVerifyMonitoringMailboxPassword(ADUser adUser, MailboxDatabaseInfo dbInfo, string password)
		{
			WTFDiagnostics.TraceInformation<string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, "GetAndVerifyMonitoringMailboxPassword for mailbox '{0}'", adUser.UserPrincipalName, null, "GetAndVerifyMonitoringMailboxPassword", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\BackendMailboxDatabaseEndpointDelegate.cs", 349);
			LiveIdAuthResult liveIdAuthResult = LiveIdAuthResult.InvalidCreds;
			string text = null;
			if (MailboxDatabaseEndpointDelegate.RunningInDatacenter && password != null)
			{
				liveIdAuthResult = base.AuthenticateTestForDataCenterMailbox(adUser.UserPrincipalName, password, this.traceContext, out text);
				if (liveIdAuthResult == LiveIdAuthResult.Success)
				{
					WTFDiagnostics.TraceInformation<string, string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, "GetAndVerifyMonitoringMailboxPassword: Password '{0}' generated during mailbox creation successfully validated for mailbox '{1}'.", password, adUser.UserPrincipalName, null, "GetAndVerifyMonitoringMailboxPassword", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\BackendMailboxDatabaseEndpointDelegate.cs", 363);
				}
				else
				{
					WTFDiagnostics.TraceInformation<string, string, LiveIdAuthResult, string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, "GetAndVerifyMonitoringMailboxPassword: Password '{0}' generated during mailbox creation failed validation for mailbox '{1}'. Authentication result: {2}. Error: {3}", password, adUser.UserPrincipalName, liveIdAuthResult, text, null, "GetAndVerifyMonitoringMailboxPassword", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\BackendMailboxDatabaseEndpointDelegate.cs", 368);
					password = null;
				}
			}
			if (MailboxDatabaseEndpointDelegate.RunningInDatacenter && password == null)
			{
				password = base.GetStoredMonitoringMailboxPassword(adUser, this.traceContext);
				if (string.IsNullOrEmpty(password))
				{
					password = null;
				}
				else
				{
					WTFDiagnostics.TraceInformation<string, string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, "GetAndVerifyMonitoringMailboxPassword: Retrieved stored password '{0}' for mailbox '{1}'.", password, adUser.UserPrincipalName, null, "GetAndVerifyMonitoringMailboxPassword", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\BackendMailboxDatabaseEndpointDelegate.cs", 386);
					liveIdAuthResult = base.AuthenticateTestForDataCenterMailbox(adUser.UserPrincipalName, password, this.traceContext, out text);
					if (liveIdAuthResult == LiveIdAuthResult.Success)
					{
						WTFDiagnostics.TraceInformation<string, string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, "GetAndVerifyMonitoringMailboxPassword: Stored password '{0}' successfully validated for mailbox '{1}'.", password, adUser.UserPrincipalName, null, "GetAndVerifyMonitoringMailboxPassword", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\BackendMailboxDatabaseEndpointDelegate.cs", 394);
					}
					else
					{
						WTFDiagnostics.TraceInformation<string, string, LiveIdAuthResult, string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, "GetAndVerifyMonitoringMailboxPassword: Stored password '{0}' failed validation for mailbox '{1}'. Authentication result: {2}. Error: {3}", password, adUser.UserPrincipalName, liveIdAuthResult, text, null, "GetAndVerifyMonitoringMailboxPassword", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\BackendMailboxDatabaseEndpointDelegate.cs", 399);
					}
				}
			}
			if (password == null || liveIdAuthResult != LiveIdAuthResult.Success)
			{
				password = DirectoryAccessor.Instance.GetMonitoringMailboxCredential(adUser);
				if (password != null)
				{
					WTFDiagnostics.TraceInformation<string, string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, "GetAndVerifyMonitoringMailboxPassword: GetMonitoringMailboxCredential returned new password '{0}' for mailbox '{1}'", password, adUser.UserPrincipalName, null, "GetAndVerifyMonitoringMailboxPassword", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\BackendMailboxDatabaseEndpointDelegate.cs", 414);
					if (MailboxDatabaseEndpointDelegate.RunningInDatacenter)
					{
						liveIdAuthResult = base.AuthenticateTestForDataCenterMailbox(adUser.UserPrincipalName, password, this.traceContext, out text);
						if (liveIdAuthResult == LiveIdAuthResult.Success)
						{
							WTFDiagnostics.TraceInformation<string, string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, "GetAndVerifyMonitoringMailboxPassword: New password '{0}' successfully validated for mailbox '{1}'.", password, adUser.UserPrincipalName, null, "GetAndVerifyMonitoringMailboxPassword", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\BackendMailboxDatabaseEndpointDelegate.cs", 423);
						}
						else
						{
							WTFDiagnostics.TraceError<string, string, LiveIdAuthResult, string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, "GetAndVerifyMonitoringMailboxPassword: New password '{0}' failed validation for mailbox '{1}'. Authentication result: {2}. Error: {3}", password, adUser.UserPrincipalName, liveIdAuthResult, text, null, "GetAndVerifyMonitoringMailboxPassword", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\BackendMailboxDatabaseEndpointDelegate.cs", 428);
						}
					}
					else
					{
						WTFDiagnostics.TraceInformation(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, "GetAndVerifyMonitoringMailboxPassword: Skipping LiveID validation test as we are using Active Directory for authentication.", null, "GetAndVerifyMonitoringMailboxPassword", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\BackendMailboxDatabaseEndpointDelegate.cs", 434);
						liveIdAuthResult = LiveIdAuthResult.Success;
					}
				}
				else
				{
					WTFDiagnostics.TraceError<string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, this.traceContext, "GetAndVerifyMonitoringMailboxPassword: Failed to obtain any password for mailbox '{0}'.", adUser.UserPrincipalName, null, "GetAndVerifyMonitoringMailboxPassword", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\BackendMailboxDatabaseEndpointDelegate.cs", 443);
					liveIdAuthResult = LiveIdAuthResult.InvalidCreds;
					text = "Failed to generate or apply new password to mailbox.";
				}
			}
			dbInfo.MonitoringAccountPassword = password;
			dbInfo.AuthenticationResult = liveIdAuthResult;
			dbInfo.AuthenticationError = text;
		}

		private string GetMonitoringMailboxName(string databaseName)
		{
			string hostName = Dns.GetHostName();
			return string.Format("HealthMailbox-{0}-{1}", hostName, databaseName.Replace(' ', '-'));
		}

		private int ignoredDatabaseCount;

		private TracingContext traceContext = TracingContext.Default;

		private object token = new object();
	}
}
