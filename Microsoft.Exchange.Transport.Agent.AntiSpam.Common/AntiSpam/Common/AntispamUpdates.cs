using System;
using Microsoft.Win32;

namespace Microsoft.Exchange.Transport.Agent.AntiSpam.Common
{
	[Serializable]
	public class AntispamUpdates
	{
		public AntispamUpdateMode UpdateMode
		{
			get
			{
				return this.updateMode;
			}
			set
			{
				this.updateMode = value;
			}
		}

		public string LatestContentFilterVersion
		{
			get
			{
				return this.contentFilterVersion;
			}
			set
			{
				this.contentFilterVersion = value;
			}
		}

		public bool SpamSignatureUpdatesEnabled
		{
			get
			{
				return this.spamSignatureUpdatesEnabled;
			}
			set
			{
				this.spamSignatureUpdatesEnabled = value;
			}
		}

		public string LatestSpamSignatureVersion
		{
			get
			{
				return this.spamSignatureVersion;
			}
			set
			{
				this.spamSignatureVersion = value;
			}
		}

		public bool IPReputationUpdatesEnabled
		{
			get
			{
				return this.ipReputationUpdatesEnabled;
			}
			set
			{
				this.ipReputationUpdatesEnabled = value;
			}
		}

		public string LatestIPReputationVersion
		{
			get
			{
				return this.ipReputationVersion;
			}
			set
			{
				this.ipReputationVersion = value;
			}
		}

		public OptInStatus MicrosoftUpdate
		{
			get
			{
				return this.microsoftUpdate;
			}
			set
			{
				this.microsoftUpdate = value;
			}
		}

		internal void LoadConfiguration(string computerName)
		{
			using (RegistryKey registryKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, computerName))
			{
				using (RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Hygiene"))
				{
					if (registryKey2 != null)
					{
						this.UpdateMode = (AntispamUpdateMode)registryKey2.GetValue("Mode", AntispamUpdateMode.Disabled);
						this.LatestContentFilterVersion = (registryKey2.GetValue("SmartDatVersion", string.Empty) as string);
						this.LatestIPReputationVersion = (registryKey2.GetValue("SmartDRPVersion", string.Empty) as string);
						this.LatestSpamSignatureVersion = (registryKey2.GetValue("SmartFNGVersion", string.Empty) as string);
						this.IPReputationUpdatesEnabled = ((int)registryKey2.GetValue("SmartDRPEnabled", 0) != 0);
						this.SpamSignatureUpdatesEnabled = ((int)registryKey2.GetValue("SmartFNGEnabled", 0) != 0);
						this.MicrosoftUpdate = (OptInStatus)registryKey2.GetValue("OptIn", OptInStatus.NotConfigured);
					}
				}
			}
		}

		internal void SaveConfiguration(string computerName)
		{
			using (RegistryKey registryKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, computerName))
			{
				using (RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Hygiene", true) ?? registryKey.CreateSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Hygiene"))
				{
					registryKey2.SetValue("Mode", (int)this.UpdateMode, RegistryValueKind.DWord);
					registryKey2.SetValue("SmartDRPEnabled", this.IPReputationUpdatesEnabled ? 1 : 0, RegistryValueKind.DWord);
					registryKey2.SetValue("SmartFNGEnabled", this.SpamSignatureUpdatesEnabled ? 1 : 0, RegistryValueKind.DWord);
					registryKey2.SetValue("OptIn", (int)this.MicrosoftUpdate, RegistryValueKind.DWord);
				}
			}
		}

		internal bool IsPremiumSKUInstalled()
		{
			this.LoadConfiguration(string.Empty);
			return this.UpdateMode == AntispamUpdateMode.Automatic && this.SpamSignatureUpdatesEnabled;
		}

		private const string DefaultKeyName = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Hygiene";

		private const string ModeValue = "Mode";

		private const string ContentFilterVersionValue = "SmartDatVersion";

		private const string IPReputationVersionValue = "SmartDRPVersion";

		private const string SpamSignatureVersionValue = "SmartFNGVersion";

		private const string IPReputationUpdatesEnabledValue = "SmartDRPEnabled";

		private const string SpamSignatureUpdatesEnabledValue = "SmartFNGEnabled";

		private const string OptInValue = "OptIn";

		private AntispamUpdateMode updateMode;

		private string contentFilterVersion;

		private bool spamSignatureUpdatesEnabled;

		private string spamSignatureVersion;

		private bool ipReputationUpdatesEnabled;

		private string ipReputationVersion;

		private OptInStatus microsoftUpdate;
	}
}
