using System;
using System.ComponentModel;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Sqm;
using Microsoft.Win32;

namespace Microsoft.Exchange.ManagementGUI
{
	internal class ManagementGuiSqmSession : SqmSession
	{
		public static ManagementGuiSqmSession Instance
		{
			get
			{
				if (ManagementGuiSqmSession.instance == null)
				{
					ManagementGuiSqmSession.instance = new ManagementGuiSqmSession();
				}
				return ManagementGuiSqmSession.instance;
			}
		}

		public ManagementGuiSqmSession() : base(SqmAppID.Admin, SqmSession.Scope.Process)
		{
			base.Open();
		}

		protected override void OnCreate()
		{
			base.OnCreate();
			if (base.Enabled)
			{
				this.worker = new BackgroundWorker();
				this.worker.DoWork += delegate(object param0, DoWorkEventArgs param1)
				{
					try
					{
						this.osInfo = Environment.OSVersion.VersionString;
					}
					catch (InvalidOperationException)
					{
					}
					try
					{
						Version installedVersion = ConfigurationContext.Setup.InstalledVersion;
						if (installedVersion != null)
						{
							this.versionInfo = installedVersion.ToString();
						}
					}
					catch (TaskException)
					{
					}
					try
					{
						if (this.ldapHostName == null)
						{
							using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\EdgeTransportRole\\AdamSettings\\MSExchange"))
							{
								if (registryKey != null)
								{
									object value = registryKey.GetValue("LdapPort");
									if (value != null)
									{
										this.ldapHostName = Environment.MachineName + ":" + value.ToString() + "/";
									}
								}
								else
								{
									this.ldapHostName = string.Empty;
								}
							}
						}
						if (this.ldapHostName != null)
						{
							string arg;
							using (DirectoryEntry directoryEntry = new DirectoryEntry(string.Format("LDAP://{0}RootDse", this.ldapHostName)))
							{
								arg = (string)directoryEntry.Properties["configurationNamingContext"].Value;
							}
							this.configNCUri = string.Format("LDAP://{0}{1}", this.ldapHostName, arg);
						}
						if (string.IsNullOrEmpty(this.ldapHostName) && !string.IsNullOrEmpty(this.configNCUri))
						{
							using (DirectorySearcher directorySearcher = new DirectorySearcher(new DirectoryEntry(this.configNCUri)))
							{
								directorySearcher.Filter = "(objectClass=msExchExchangeServer)";
								directorySearcher.PropertiesToLoad.Clear();
								SearchResultCollection searchResultCollection = directorySearcher.FindAll();
								if (searchResultCollection != null)
								{
									this.orgSize = (uint)searchResultCollection.Count;
								}
							}
						}
					}
					catch (SecurityException)
					{
					}
					catch (UnauthorizedAccessException)
					{
					}
					catch (COMException)
					{
					}
					catch (ActiveDirectoryObjectNotFoundException)
					{
					}
					catch (ActiveDirectoryOperationException)
					{
					}
					catch (ActiveDirectoryServerDownException)
					{
					}
					this.staticConfiguationCollected = true;
				};
				this.worker.RunWorkerAsync();
			}
		}

		protected override void OnClosing()
		{
			base.OnClosing();
			if (this.staticConfiguationCollected)
			{
				ManagementGuiSqmSession.Instance.SetDataPoint(SqmDataID.DATAID_OS, this.osInfo);
				ManagementGuiSqmSession.Instance.SetDataPoint(SqmDataID.DATAID_EMC_VERSION, this.versionInfo);
				ManagementGuiSqmSession.Instance.SetDataPoint(SqmDataID.DATAID_ORGANIZATION_SIZE, this.orgSize);
			}
			if (this.worker != null)
			{
				this.worker.Dispose();
				this.worker = null;
			}
		}

		private string ldapHostName;

		private string configNCUri;

		private BackgroundWorker worker;

		private static ManagementGuiSqmSession instance;

		private string osInfo = "Unknown";

		private string versionInfo = "Unknown";

		private uint orgSize = 2147483646U;

		private bool staticConfiguationCollected;

		public enum GuiElementCategory : uint
		{
			ResultPane = 1U,
			Page,
			Form
		}
	}
}
