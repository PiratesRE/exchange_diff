using System;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	internal sealed class RegistrySubKey
	{
		public RegistrySubKey(RegistryKey hiveKey, string subkeyPath)
		{
			this.hiveKey = hiveKey;
			this.subkeyPath = subkeyPath;
		}

		private RegistrySubKey()
		{
		}

		public RegistryKey HiveKey
		{
			get
			{
				return this.hiveKey;
			}
		}

		public string SubkeyPath
		{
			get
			{
				return this.subkeyPath;
			}
		}

		public RegistryKey Open()
		{
			return this.hiveKey.OpenSubKey(this.subkeyPath);
		}

		public RegistryKey Open(bool writable)
		{
			return this.hiveKey.OpenSubKey(this.subkeyPath, writable);
		}

		public RegistryKey Create()
		{
			return this.hiveKey.CreateSubKey(this.subkeyPath);
		}

		public void DeleteTreeIfExist()
		{
			Utils.DeleteRegSubKeyTreeIfExist(this.hiveKey, this.subkeyPath);
		}

		private RegistryKey hiveKey;

		private readonly string subkeyPath;
	}
}
