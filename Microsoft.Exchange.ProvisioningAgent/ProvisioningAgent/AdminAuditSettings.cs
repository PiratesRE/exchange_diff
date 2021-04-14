using System;
using System.IO;
using System.Security;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.ProvisioningAgent;
using Microsoft.Win32;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal class AdminAuditSettings
	{
		public static AdminAuditSettings Instance
		{
			get
			{
				return AdminAuditSettings.instance;
			}
		}

		public bool BypassForwardSync { get; private set; }

		public int SessionCacheSize { get; private set; }

		public TimeSpan SessionExpirationTime { get; private set; }

		public int MaxNumberOfMailboxSessionsPerMailbox { get; private set; }

		private AdminAuditSettings()
		{
			this.LoadSettings();
			this.CheckTestDomain();
		}

		private void LoadSettings()
		{
			this.BypassForwardSync = AdminAuditSettings.DefaultBypassForwardSync;
			this.SessionCacheSize = AdminAuditSettings.DefaultSessionCacheSize;
			this.SessionExpirationTime = AdminAuditSettings.DefaultSessionExpirationTime;
			this.MaxNumberOfMailboxSessionsPerMailbox = AdminAuditSettings.DefaultMaxNumberOfMailboxSessionsPerMailbox;
			Exception ex = null;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(AdminAuditSettings.AdminAuditLogKeyRoot))
				{
					if (registryKey == null)
					{
						return;
					}
					this.BypassForwardSync = ((int)registryKey.GetValue(AdminAuditSettings.BypassForwardSyncValueName, AdminAuditSettings.DefaultBypassForwardSync) != 0);
					int num = (int)registryKey.GetValue(AdminAuditSettings.SessionCacheSizeValueName, AdminAuditSettings.DefaultSessionCacheSize);
					if (num > 0)
					{
						this.SessionCacheSize = num;
					}
					num = (int)registryKey.GetValue(AdminAuditSettings.MaxNumberOfMailboxSessionsPerMailboxName, AdminAuditSettings.DefaultMaxNumberOfMailboxSessionsPerMailbox);
					if (num > 0)
					{
						this.MaxNumberOfMailboxSessionsPerMailbox = num;
					}
					this.SessionExpirationTime = TimeSpan.Parse((string)registryKey.GetValue(AdminAuditSettings.SessionExpirationTimeValueName, AdminAuditSettings.DefaultSessionExpirationTime.ToString()));
				}
			}
			catch (SecurityException ex2)
			{
				ex = ex2;
			}
			catch (InvalidCastException ex3)
			{
				ex = ex3;
			}
			catch (FormatException ex4)
			{
				ex = ex4;
			}
			catch (IOException ex5)
			{
				ex = ex5;
			}
			catch (UnauthorizedAccessException ex6)
			{
				ex = ex6;
			}
			if (ex != null)
			{
				ExTraceGlobals.AdminAuditLogTracer.TraceError<Exception>(0L, "Error occured when reading settings from registry. Exception: {0}", ex);
			}
		}

		private void CheckTestDomain()
		{
			ADObjectId rootDomainNamingContextForLocalForest = ADSession.GetRootDomainNamingContextForLocalForest();
			string text = rootDomainNamingContextForLocalForest.DomainId.ToCanonicalName().ToLower();
			if (text.Contains("extest.microsoft.com"))
			{
				this.SessionCacheSize = Math.Min(this.SessionCacheSize, AdminAuditSettings.TestDomainMaxCacheSize);
			}
		}

		private static readonly string AuditLogKeyName = "AuditLog";

		private static readonly string AdminAuditLogKeyName = "AdminAuditLog";

		private static readonly string AdminAuditLogKeyRoot = string.Format("SOFTWARE\\Microsoft\\ExchangeServer\\{0}\\{1}\\{2}", "v15", AdminAuditSettings.AuditLogKeyName, AdminAuditSettings.AdminAuditLogKeyName);

		private static readonly string BypassForwardSyncValueName = "BypassForwardSync";

		private static readonly string SessionCacheSizeValueName = "SessionCacheSize";

		private static readonly string SessionExpirationTimeValueName = "SessionExpirationTime";

		private static readonly string MaxNumberOfMailboxSessionsPerMailboxName = "MaxNumberOfMailboxSessionsPerMailbox";

		private static readonly bool DefaultBypassForwardSync = false;

		private static readonly int DefaultSessionCacheSize = 1000;

		private static readonly TimeSpan DefaultSessionExpirationTime = TimeSpan.FromMinutes(1.0);

		private static readonly int DefaultMaxNumberOfMailboxSessionsPerMailbox = 5;

		private static readonly int TestDomainMaxCacheSize = 3;

		private static AdminAuditSettings instance = new AdminAuditSettings();
	}
}
