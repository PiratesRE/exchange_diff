using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class RealRegistry : RegistryManipulator
	{
		public RealRegistry(string root, SafeHandle handle) : base(root, handle)
		{
			bool flag = true;
			string apiName = string.Empty;
			try
			{
				apiName = "OpenSubKey";
				this.rootKey = Registry.LocalMachine.OpenSubKey(root, true);
				if (this.rootKey == null)
				{
					apiName = "CreateSubKey";
					this.rootKey = Registry.LocalMachine.CreateSubKey(root);
				}
				flag = false;
			}
			catch (SecurityException innerException)
			{
				throw new AmRegistryException(apiName, innerException);
			}
			catch (IOException innerException2)
			{
				throw new AmRegistryException(apiName, innerException2);
			}
			finally
			{
				if (flag)
				{
					base.SuppressDisposeTracker();
				}
			}
		}

		public RegistryKey RootKey
		{
			get
			{
				return this.rootKey;
			}
		}

		public override string[] GetSubKeyNames(string keyName)
		{
			string[] array = null;
			try
			{
				using (RegistryKey registryKey = this.OpenKey(keyName))
				{
					if (registryKey != null)
					{
						array = registryKey.GetSubKeyNames();
					}
				}
			}
			catch (SecurityException innerException)
			{
				throw new AmRegistryException("GetSubKeyNames", innerException);
			}
			catch (UnauthorizedAccessException innerException2)
			{
				throw new AmRegistryException("GetSubKeyNames", innerException2);
			}
			catch (IOException innerException3)
			{
				throw new AmRegistryException("GetSubKeyNames", innerException3);
			}
			return array ?? new string[0];
		}

		public override string[] GetValueNames(string keyName)
		{
			string[] array = null;
			try
			{
				using (RegistryKey registryKey = this.OpenKey(keyName))
				{
					if (registryKey != null)
					{
						array = registryKey.GetValueNames();
					}
				}
			}
			catch (SecurityException innerException)
			{
				throw new AmRegistryException("GetValueNames", innerException);
			}
			catch (UnauthorizedAccessException innerException2)
			{
				throw new AmRegistryException("GetValueNames", innerException2);
			}
			catch (IOException innerException3)
			{
				throw new AmRegistryException("GetValueNames", innerException3);
			}
			return array ?? new string[0];
		}

		public override void SetValue(string keyName, RegistryValue value)
		{
			RegistryKey registryKey = null;
			try
			{
				registryKey = this.OpenKey(keyName, true);
				if (registryKey == null)
				{
					this.CreateKey(keyName);
					registryKey = this.OpenKey(keyName, true);
				}
				registryKey.SetValue(value.Name, value.Value, value.Kind);
			}
			catch (UnauthorizedAccessException innerException)
			{
				throw new AmRegistryException("SetValue", innerException);
			}
			catch (SecurityException innerException2)
			{
				throw new AmRegistryException("SetValue", innerException2);
			}
			catch (IOException innerException3)
			{
				throw new AmRegistryException("SetValue", innerException3);
			}
			finally
			{
				if (registryKey != null)
				{
					registryKey.Close();
				}
			}
		}

		public override RegistryValue GetValue(string keyName, string valueName)
		{
			RegistryValue result = null;
			try
			{
				using (RegistryKey registryKey = this.OpenKey(keyName))
				{
					if (registryKey != null)
					{
						result = new RegistryValue(valueName, registryKey.GetValue(valueName), registryKey.GetValueKind(valueName));
					}
				}
			}
			catch (UnauthorizedAccessException innerException)
			{
				throw new AmRegistryException("GetValue", innerException);
			}
			catch (SecurityException innerException2)
			{
				throw new AmRegistryException("GetValue", innerException2);
			}
			catch (IOException innerException3)
			{
				throw new AmRegistryException("GetValue", innerException3);
			}
			return result;
		}

		public override void DeleteValue(string keyName, string valueName)
		{
			try
			{
				using (RegistryKey registryKey = this.OpenKey(keyName, true))
				{
					if (registryKey != null)
					{
						registryKey.DeleteValue(valueName);
					}
				}
			}
			catch (UnauthorizedAccessException innerException)
			{
				throw new AmRegistryException("DeleteValue", innerException);
			}
			catch (SecurityException innerException2)
			{
				throw new AmRegistryException("DeleteValue", innerException2);
			}
		}

		public override void DeleteKey(string keyName)
		{
			try
			{
				this.rootKey.DeleteSubKeyTree(keyName);
			}
			catch (ArgumentException)
			{
				using (this.OpenKey(keyName))
				{
				}
			}
			catch (UnauthorizedAccessException innerException)
			{
				throw new AmRegistryException("DeleteSubKeyTree", innerException);
			}
			catch (SecurityException innerException2)
			{
				throw new AmRegistryException("DeleteSubKeyTree", innerException2);
			}
		}

		public override void CreateKey(string keyName)
		{
			try
			{
				using (this.rootKey.CreateSubKey(keyName))
				{
				}
			}
			catch (SecurityException innerException)
			{
				throw new AmRegistryException("CreateSubKey", innerException);
			}
			catch (IOException innerException2)
			{
				throw new AmRegistryException("CreateSubKey", innerException2);
			}
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<RealRegistry>(this);
		}

		public override void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private RegistryKey OpenKey(string keyName)
		{
			return this.OpenKey(keyName, false);
		}

		private RegistryKey OpenKey(string keyName, bool writable)
		{
			RegistryKey result;
			try
			{
				result = this.rootKey.OpenSubKey(keyName, writable);
			}
			catch (SecurityException innerException)
			{
				throw new AmRegistryException("OpenSubKey", innerException);
			}
			return result;
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.rootKey.Close();
				this.Handle.Close();
			}
			base.Dispose();
		}

		private RegistryKey rootKey;
	}
}
