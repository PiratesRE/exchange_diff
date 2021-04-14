using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public class MailboxDatabaseInfo
	{
		internal LiveIdAuthResult AuthenticationResult { get; set; }

		internal string AuthenticationError { get; set; }

		internal string OriginatingServer { get; set; }

		public string MailboxDatabaseName { get; set; }

		public Guid MailboxDatabaseGuid { get; set; }

		public string MonitoringAccount { get; set; }

		public string MonitoringAccountDisplayName { get; set; }

		public string MonitoringAccountDomain { get; set; }

		public SmtpAddress MonitoringAccountPrimarySmtpAddress { get; set; }

		public string MonitoringAccountPassword
		{
			get
			{
				if (this.AuthenticationResult != LiveIdAuthResult.Success)
				{
					throw new MailboxNotValidatedException(this.password);
				}
				return this.password;
			}
			set
			{
				this.password = value;
			}
		}

		public string MonitoringAccountSid { get; set; }

		public string MonitoringAccountPuid { get; set; }

		public string MonitoringAccountPartitionId { get; set; }

		public OrganizationId MonitoringAccountOrganizationId { get; set; }

		public string MonitoringAccountLegacyDN { get; set; }

		public Guid MonitoringAccountMailboxGuid { get; set; }

		public Guid MonitoringAccountMailboxArchiveGuid { get; set; }

		public string MonitoringAccountUserPrincipalName { get; set; }

		public string MonitoringAccountExchangeLoginName { get; set; }

		public string MonitoringAccountWindowsLoginName { get; set; }

		public string MonitoringMailboxLegacyExchangeDN { get; set; }

		public Guid SystemMailboxGuid { get; set; }

		public string MonitoringAccountSipAddress { get; set; }

		public string HostingServer { get; set; }

		public DateTime TimeVerified { get; set; }

		public MailboxDatabaseInfo()
		{
			if (!VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).ActiveMonitoring.DirectoryAccessor.Enabled || !LocalEndpointManager.IsDataCenter)
			{
				this.AuthenticationResult = LiveIdAuthResult.Success;
			}
			else
			{
				this.AuthenticationResult = LiveIdAuthResult.AuthFailure;
			}
			this.TimeVerified = DateTime.UtcNow;
		}

		public void WriteToRegistry(RegistryKey roleKey)
		{
			WTFDiagnostics.TraceInformation<string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, TracingContext.Default, "Writing dbInfo for database {0}", this.MailboxDatabaseName ?? "null", null, "WriteToRegistry", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MailboxDatabaseInfo.cs", 415);
			string mailboxDatabaseName = this.MailboxDatabaseName;
			WTFDiagnostics.TraceInformation<string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, TracingContext.Default, "Creating registry subkey '{0}'", mailboxDatabaseName ?? "null", null, "WriteToRegistry", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MailboxDatabaseInfo.cs", 419);
			using (RegistryKey registryKey = roleKey.CreateSubKey(mailboxDatabaseName, RegistryKeyPermissionCheck.ReadWriteSubTree))
			{
				if (!string.IsNullOrEmpty(this.MonitoringAccountUserPrincipalName))
				{
					WTFDiagnostics.TraceInformation<string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, TracingContext.Default, "MonitoringAccountUserPrincipalName: {0}", this.MonitoringAccountUserPrincipalName ?? "null", null, "WriteToRegistry", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MailboxDatabaseInfo.cs", 426);
					registryKey.SetValue("UserPrincipalName", this.MonitoringAccountUserPrincipalName, RegistryValueKind.String);
				}
				if (!string.IsNullOrEmpty(this.MonitoringAccountDisplayName))
				{
					WTFDiagnostics.TraceInformation<string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, TracingContext.Default, "MonitoringAccountDisplayName: {0}", this.MonitoringAccountDisplayName ?? "null", null, "WriteToRegistry", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MailboxDatabaseInfo.cs", 433);
					registryKey.SetValue("DisplayName", this.MonitoringAccountDisplayName, RegistryValueKind.String);
				}
				if (!string.IsNullOrEmpty(this.MonitoringAccount))
				{
					WTFDiagnostics.TraceInformation<string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, TracingContext.Default, "MonitoringAccount: {0}", this.MonitoringAccount ?? "null", null, "WriteToRegistry", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MailboxDatabaseInfo.cs", 441);
					registryKey.SetValue("MonitoringAccount", this.MonitoringAccount, RegistryValueKind.String);
				}
				if (!string.IsNullOrEmpty(this.MonitoringAccountDomain))
				{
					WTFDiagnostics.TraceInformation<string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, TracingContext.Default, "MonitoringAccountDomain: {0}", this.MonitoringAccountDomain ?? "null", null, "WriteToRegistry", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MailboxDatabaseInfo.cs", 449);
					registryKey.SetValue("MonitoringAccountDomain", this.MonitoringAccountDomain, RegistryValueKind.String);
				}
				if (!string.IsNullOrEmpty(this.OriginatingServer))
				{
					WTFDiagnostics.TraceInformation<string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, TracingContext.Default, "OriginatingServer: {0}", this.OriginatingServer ?? "null", null, "WriteToRegistry", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MailboxDatabaseInfo.cs", 456);
					registryKey.SetValue("DomainController", this.OriginatingServer, RegistryValueKind.String);
				}
				WTFDiagnostics.TraceInformation<string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, TracingContext.Default, "AuthenticationResult: {0}", this.AuthenticationResult.ToString(), null, "WriteToRegistry", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MailboxDatabaseInfo.cs", 461);
				registryKey.SetValue("AuthenticationResult", this.AuthenticationResult.ToString(), RegistryValueKind.String);
				if (!string.IsNullOrEmpty(this.AuthenticationError))
				{
					WTFDiagnostics.TraceInformation<string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, TracingContext.Default, "AuthenticationError: {0}", this.AuthenticationError ?? "null", null, "WriteToRegistry", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MailboxDatabaseInfo.cs", 467);
					registryKey.SetValue("AuthenticationError", this.AuthenticationError, RegistryValueKind.String);
				}
				if (!string.IsNullOrEmpty(this.HostingServer))
				{
					WTFDiagnostics.TraceInformation<string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, TracingContext.Default, "HostingServer: {0}", this.HostingServer ?? "null", null, "WriteToRegistry", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MailboxDatabaseInfo.cs", 474);
					registryKey.SetValue("HostingServer", this.HostingServer, RegistryValueKind.String);
				}
				if (this.password != null && LocalEndpointManager.IsDataCenter)
				{
					WTFDiagnostics.TraceInformation<string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, TracingContext.Default, "Password: {0}", this.password ?? "null", null, "WriteToRegistry", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MailboxDatabaseInfo.cs", 482);
					registryKey.SetValue("Password", this.password, RegistryValueKind.String);
				}
				registryKey.SetValue("TimeVerified", this.TimeVerified.ToString("u"), RegistryValueKind.String);
			}
		}

		public void UpdateMailboxAuthenticationRegistryInfo(string rootRegistryPath, string roleRegistrySubkey)
		{
			WTFDiagnostics.TraceInformation<string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, TracingContext.Default, "Updating Mailbox Authentication info on registry for database {0}", this.MailboxDatabaseName ?? "null", null, "UpdateMailboxAuthenticationRegistryInfo", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MailboxDatabaseInfo.cs", 498);
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(rootRegistryPath))
			{
				if (registryKey != null)
				{
					using (RegistryKey registryKey2 = registryKey.OpenSubKey(roleRegistrySubkey))
					{
						if (registryKey2 != null)
						{
							using (RegistryKey registryKey3 = registryKey.OpenSubKey(this.MailboxDatabaseName))
							{
								if (registryKey3 != null)
								{
									try
									{
										registryKey3.SetValue("AuthenticationResult", this.AuthenticationResult.ToString(), RegistryValueKind.String);
										if (!string.IsNullOrEmpty(this.AuthenticationError))
										{
											registryKey3.SetValue("AuthenticationError", this.AuthenticationError, RegistryValueKind.String);
										}
										registryKey3.SetValue("TimeVerified", this.TimeVerified.ToString("u"), RegistryValueKind.String);
									}
									catch (Exception arg)
									{
										WTFDiagnostics.TraceError<Exception>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, TracingContext.Default, "Error while updating registry. Caught exception: {0}", arg, null, "UpdateMailboxAuthenticationRegistryInfo", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MailboxDatabaseInfo.cs", 528);
									}
								}
							}
						}
					}
				}
			}
		}

		public const string TimeVerifiedName = "TimeVerified";

		public const string UserPrincipalNameRegistryValue = "UserPrincipalName";

		public const string AuthenticationResultRegistryValue = "AuthenticationResult";

		public const string AuthenticationErrorRegistryValue = "AuthenticationError";

		private const string DisplayNameRegistryValue = "DisplayName";

		private const string MonitoringAccountRegistryValue = "MonitoringAccount";

		private const string MonitoringAccountDomainRegistryValue = "MonitoringAccountDomain";

		private const string DomainControllerRegistryValue = "DomainController";

		private const string HostingServerRegistryValue = "HostingServer";

		private const string PasswordRegistryValue = "Password";

		private string password;
	}
}
