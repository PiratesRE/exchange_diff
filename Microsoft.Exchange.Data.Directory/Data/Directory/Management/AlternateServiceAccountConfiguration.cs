using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Security;
using System.Security.Principal;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class AlternateServiceAccountConfiguration
	{
		private AlternateServiceAccountConfiguration(string machineName)
		{
			this.machineName = machineName;
		}

		public ReadOnlyCollection<AlternateServiceAccountCredential> EffectiveCredentials
		{
			get
			{
				return this.AllCredentials;
			}
		}

		public override string ToString()
		{
			IList<AlternateServiceAccountCredential> effectiveCredentials = this.EffectiveCredentials;
			return DirectoryStrings.AlternateServiceAccountConfigurationDisplayFormat((effectiveCredentials.Count > 0) ? effectiveCredentials[0].ToString() : DirectoryStrings.AlternateServiceAccountCredentialNotSet, (effectiveCredentials.Count > 1) ? effectiveCredentials[1].ToString() : DirectoryStrings.AlternateServiceAccountCredentialNotSet, (effectiveCredentials.Count > 2) ? DirectoryStrings.AlternateServiceAccountConfigurationDisplayFormatMoreDataAvailable : string.Empty);
		}

		internal static bool TestOnlyUseAlternateKeyAndDisableAccountCheck { private get; set; }

		internal static RegistryWatcher CreateRegistryWatcher()
		{
			RegistryWatcher watcher = null;
			new AlternateServiceAccountConfiguration(null).DoRegistryOperation(false, delegate(RegistryKey key)
			{
				watcher = new RegistryWatcher(AlternateServiceAccountConfiguration.RootRegistryKeyName, true);
			}, new Func<string, LocalizedString>(DirectoryStrings.FailedToReadAlternateServiceAccountConfigFromRegistry));
			return watcher;
		}

		internal static void EnsureCanDoCryptoOperations()
		{
			using (WindowsIdentity current = WindowsIdentity.GetCurrent())
			{
				if (!current.IsSystem && !AlternateServiceAccountConfiguration.TestOnlyUseAlternateKeyAndDisableAccountCheck)
				{
					throw new InvalidOperationException("Only processes running under LocalSystem account can manage Alternate Service Account settings for a computer.");
				}
			}
		}

		internal static AlternateServiceAccountConfiguration LoadFromRegistry(string machineName)
		{
			return AlternateServiceAccountConfiguration.LoadFromRegistry(machineName, false);
		}

		internal static AlternateServiceAccountConfiguration LoadWithPasswordsFromRegistry()
		{
			return AlternateServiceAccountConfiguration.LoadFromRegistry(null, true);
		}

		internal ReadOnlyCollection<AlternateServiceAccountCredential> AllCredentials
		{
			get
			{
				return new ReadOnlyCollection<AlternateServiceAccountCredential>(this.credentials);
			}
		}

		internal void ApplyPasswords(SecureString[] passwords)
		{
			if (this.credentials.Count != passwords.Length)
			{
				throw new ArgumentException("Credential and password counts don't match", "passwords");
			}
			for (int i = 0; i < this.credentials.Count; i++)
			{
				this.credentials[i].ApplyPassword(passwords[i]);
			}
		}

		internal AlternateServiceAccountCredential AddCredential(PSCredential credential)
		{
			AlternateServiceAccountCredential newCredential = AlternateServiceAccountCredential.Create(TimeSpan.FromMilliseconds((double)(10 * this.credentials.Count)), credential);
			this.DoRegistryOperation(false, delegate(RegistryKey key)
			{
				newCredential.SaveToRegistry(key);
			}, new Func<string, LocalizedString>(DirectoryStrings.FailedToWriteAlternateServiceAccountConfigToRegistry));
			this.credentials.Insert(0, newCredential);
			return newCredential;
		}

		internal bool RemoveAllCredentials()
		{
			bool result = this.AllCredentials.Count > 0;
			this.DoRegistryOperation(true, delegate(RegistryKey unused)
			{
				using (RegistryKey registryKey = this.OpenHive())
				{
					registryKey.DeleteSubKeyTree(AlternateServiceAccountConfiguration.RootRegistryKeyName);
				}
			}, new Func<string, LocalizedString>(DirectoryStrings.FailedToWriteAlternateServiceAccountConfigToRegistry));
			foreach (AlternateServiceAccountCredential alternateServiceAccountCredential in this.credentials)
			{
				alternateServiceAccountCredential.Dispose();
			}
			this.credentials.Clear();
			return result;
		}

		internal void RemoveCredential(AlternateServiceAccountCredential credential)
		{
			this.DoRegistryOperation(false, delegate(RegistryKey key)
			{
				credential.Remove(key);
			}, new Func<string, LocalizedString>(DirectoryStrings.FailedToWriteAlternateServiceAccountConfigToRegistry));
			this.credentials.Remove(credential);
			credential.Dispose();
		}

		private static string RootRegistryKeyName
		{
			get
			{
				if (!AlternateServiceAccountConfiguration.TestOnlyUseAlternateKeyAndDisableAccountCheck)
				{
					return "SYSTEM\\CurrentControlSet\\Services\\MSExchangeServiceHost\\ServiceAccounts";
				}
				return "SYSTEM\\CurrentControlSet\\Services\\MSExchangeServiceHost\\TestOnly_ServiceAccounts";
			}
		}

		private static AlternateServiceAccountConfiguration LoadFromRegistry(string machineName, bool decryptPasswords)
		{
			AlternateServiceAccountConfiguration result = new AlternateServiceAccountConfiguration(machineName);
			result.DoRegistryOperation(true, delegate(RegistryKey rootKey)
			{
				result.credentials.AddRange(AlternateServiceAccountCredential.LoadFromRegistry(rootKey, decryptPasswords));
			}, new Func<string, LocalizedString>(DirectoryStrings.FailedToReadAlternateServiceAccountConfigFromRegistry));
			result.credentials.Sort();
			return result;
		}

		private void DoRegistryOperation(bool isReadOnly, Action<RegistryKey> action, Func<string, LocalizedString> errorMessage)
		{
			string rootRegistryKeyName = AlternateServiceAccountConfiguration.RootRegistryKeyName;
			try
			{
				using (RegistryKey registryKey = this.OpenHive())
				{
					using (RegistryKey registryKey2 = isReadOnly ? registryKey.OpenSubKey(rootRegistryKeyName) : registryKey.CreateSubKey(rootRegistryKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree))
					{
						if (registryKey2 != null)
						{
							action(registryKey2);
						}
						else if (!isReadOnly)
						{
							throw new DataSourceTransientException(errorMessage(rootRegistryKeyName));
						}
					}
				}
			}
			catch (IOException innerException)
			{
				throw new DataSourceTransientException(errorMessage(rootRegistryKeyName), innerException);
			}
			catch (SecurityException innerException2)
			{
				throw new DataSourceOperationException(errorMessage(rootRegistryKeyName), innerException2);
			}
			catch (UnauthorizedAccessException innerException3)
			{
				throw new DataSourceOperationException(errorMessage(rootRegistryKeyName), innerException3);
			}
		}

		private RegistryKey OpenHive()
		{
			if (this.machineName == null)
			{
				return Registry.LocalMachine;
			}
			return RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, this.machineName);
		}

		private const string ServiceAccountConfigurationRegkey = "SYSTEM\\CurrentControlSet\\Services\\MSExchangeServiceHost\\ServiceAccounts";

		private const string TestOnlyServiceAccountConfigurationRegkey = "SYSTEM\\CurrentControlSet\\Services\\MSExchangeServiceHost\\TestOnly_ServiceAccounts";

		private readonly List<AlternateServiceAccountCredential> credentials = new List<AlternateServiceAccountCredential>();

		[NonSerialized]
		private readonly string machineName;
	}
}
