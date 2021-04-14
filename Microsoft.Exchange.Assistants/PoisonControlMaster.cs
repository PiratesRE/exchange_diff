using System;
using Microsoft.Win32;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class PoisonControlMaster
	{
		public PoisonControlMaster(string registryKeyBasePath)
		{
			if (string.IsNullOrEmpty(registryKeyBasePath))
			{
				return;
			}
			string text = registryKeyBasePath + "\\PoisonControl";
			this.registryKey = Registry.LocalMachine.OpenSubKey(text, true);
			if (this.registryKey == null)
			{
				this.registryKey = Registry.LocalMachine.CreateSubKey(text, RegistryKeyPermissionCheck.ReadWriteSubTree);
				this.registryKey.SetValue("Enabled", this.enabled ? 1 : 0);
				this.registryKey.SetValue("MaxCrashCount", this.poisonCrashCount);
				return;
			}
			object value = this.registryKey.GetValue("Enabled");
			if (value is int)
			{
				this.enabled = ((int)value != 0);
			}
			value = this.registryKey.GetValue("MaxCrashCount");
			if (value is int)
			{
				this.poisonCrashCount = (int)value;
				this.toxicCrashCount = this.poisonCrashCount + 1;
			}
		}

		public int PoisonCrashCount
		{
			get
			{
				return this.poisonCrashCount;
			}
		}

		public int ToxicCrashCount
		{
			get
			{
				return this.toxicCrashCount;
			}
		}

		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
		}

		public RegistryKey RegistryKey
		{
			get
			{
				return this.registryKey;
			}
		}

		private const string RegistryNameEnabled = "Enabled";

		private const string RegistryNameMaxCrashCount = "MaxCrashCount";

		private RegistryKey registryKey;

		private bool enabled = true;

		private int poisonCrashCount = 2;

		private int toxicCrashCount = 3;
	}
}
