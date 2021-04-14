using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmStandaloneDbState : AmDbState
	{
		internal AmStandaloneDbState()
		{
			this.InitializeHandles();
		}

		protected override void InitializeHandles()
		{
			RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveManager");
			this.m_regDbHandle = registryKey.CreateSubKey("DbState");
			this.m_dbgOptionHandle = registryKey.CreateSubKey("DebugOption");
			registryKey.Close();
		}

		protected override void CloseHandles()
		{
			if (this.m_regDbHandle != null)
			{
				this.m_regDbHandle.Close();
				this.m_regDbHandle = null;
			}
			if (this.m_dbgOptionHandle != null)
			{
				this.m_dbgOptionHandle.Close();
				this.m_dbgOptionHandle = null;
			}
		}

		protected override void WriteInternal(string guidStr, string stateInfoStr, AmServerName activeServerName)
		{
			try
			{
				this.m_regDbHandle.SetValue(guidStr, stateInfoStr);
				this.m_regDbHandle.Flush();
			}
			catch (IOException ex)
			{
				int hrforException = Marshal.GetHRForException(ex);
				AmTrace.Error("WriteInternal({0}, {1}): m_regDbHandle.SetValue failed with error {2} (hr={3})", new object[]
				{
					guidStr,
					stateInfoStr,
					ex.Message,
					hrforException
				});
				if (hrforException != 1018)
				{
					throw new AmRegistryException("m_regDbHandle.SetValue", ex);
				}
				throw;
			}
		}

		protected override Guid[] ReadDatabaseGuids(bool isBestEffort)
		{
			Guid[] result = null;
			try
			{
				string[] subKeyNames = this.m_regDbHandle.GetSubKeyNames();
				result = base.ConvertGuidStringsToGuids(subKeyNames);
			}
			catch (IOException ex)
			{
				int hrforException = Marshal.GetHRForException(ex);
				AmTrace.Error("ReadDatabaseGuids({0}): m_regDbHandle.GetSubKeyNames failed with error {1} (hr={2})", new object[]
				{
					isBestEffort,
					ex.Message,
					hrforException
				});
				if (!isBestEffort)
				{
					throw new AmRegistryException("m_regDbHandle.GetSubKeyNames", ex);
				}
			}
			return result;
		}

		protected override AmDbStateInfo[] ReadAllInternal(bool isBestEffort)
		{
			AmDbStateInfo[] array = null;
			Guid[] array2 = this.ReadDatabaseGuids(isBestEffort);
			if (array2 != null)
			{
				array = new AmDbStateInfo[array2.Length];
				for (int i = 0; i < array2.Length; i++)
				{
					array[i] = base.Read(array2[i], isBestEffort);
				}
			}
			return array;
		}

		protected override bool ReadInternal(string guidStr, out string stateInfoStr)
		{
			bool result = false;
			object obj = null;
			try
			{
				obj = this.m_regDbHandle.GetValue(guidStr);
				if (obj == null)
				{
					AmTrace.Info("Subkeys count = {0}", new object[]
					{
						this.m_regDbHandle.SubKeyCount
					});
				}
			}
			catch (IOException ex)
			{
				int hrforException = Marshal.GetHRForException(ex);
				AmTrace.Error("ReadInternal({0}): m_regDbHandle.GetValue failed with error {1} (hr={2})", new object[]
				{
					guidStr,
					ex.Message,
					hrforException
				});
				if (hrforException != 1018)
				{
					throw new AmRegistryException("m_regDbHandle.GetValue", ex);
				}
				throw;
			}
			if (obj != null)
			{
				stateInfoStr = (string)obj;
				result = true;
			}
			else
			{
				stateInfoStr = null;
			}
			return result;
		}

		protected override void DeleteInternal(string guidStr)
		{
			try
			{
				this.m_regDbHandle.DeleteValue(guidStr);
			}
			catch (ArgumentException ex)
			{
				AmTrace.Error("DeleteInternal({0}): m_regDbHandle.DeleteValue failed with error {1}", new object[]
				{
					guidStr,
					ex.Message
				});
			}
			catch (IOException ex2)
			{
				AmTrace.Error("DeleteInternal({0}): m_regDbHandle.DeleteValue failed with error {1}", new object[]
				{
					guidStr,
					ex2.Message
				});
				throw new AmRegistryException("m_regDbHandle.DeleteValue", ex2);
			}
		}

		protected override void SetLastLogPropertyInternal(string name, string value)
		{
		}

		protected override bool GetLastLogPropertyInternal(string name, out string value)
		{
			value = string.Empty;
			return false;
		}

		protected override T GetDebugOptionInternal<T>(string serverName, string propertyName, T defaultValue, out bool doesValueExist)
		{
			T result = defaultValue;
			doesValueExist = false;
			try
			{
				if (serverName == null)
				{
					object value = this.m_dbgOptionHandle.GetValue(propertyName, defaultValue);
					if (value != null)
					{
						result = (T)((object)value);
						doesValueExist = true;
					}
				}
				else
				{
					using (RegistryKey registryKey = this.m_dbgOptionHandle.OpenSubKey(serverName))
					{
						if (registryKey != null)
						{
							object value2 = registryKey.GetValue(propertyName, defaultValue);
							if (value2 != null)
							{
								result = (T)((object)value2);
								doesValueExist = true;
							}
						}
					}
				}
			}
			catch (IOException ex)
			{
				AmTrace.Error("GetDebugOptionInternal({0}, {1}): m_dbgOptionHandle.GetValue or m_dbgOptionHandle.OpenSubKey failed with error {2}", new object[]
				{
					serverName,
					propertyName,
					ex.Message
				});
				throw new AmRegistryException("m_dbgOptionHandle.GetValue or m_dbgOptionHandle.OpenSubKey", ex);
			}
			return result;
		}

		protected override bool SetDebugOptionInternal<T>(string serverName, string propertyName, T propertyValue)
		{
			try
			{
				if (serverName == null)
				{
					this.m_dbgOptionHandle.SetValue(propertyName, propertyValue);
				}
				else
				{
					using (RegistryKey registryKey = this.m_dbgOptionHandle.CreateSubKey(serverName))
					{
						if (registryKey != null)
						{
							registryKey.SetValue(propertyName, propertyValue);
						}
					}
				}
			}
			catch (IOException ex)
			{
				AmTrace.Error("SetDebugOptionInternal({0}, {1}): m_dbgOptionHandle.SetValue or m_dbgOptionHandle.CreateSubKey failed with error {2}", new object[]
				{
					serverName,
					propertyName,
					ex.Message
				});
				throw new AmRegistryException("m_dbgOptionHandle.SetValue or m_dbgOptionHandle.CreateSubKey", ex);
			}
			return true;
		}

		private const string ExchangeRootKeyName = "SOFTWARE\\Microsoft\\ExchangeServer\\v15";

		private const string AmRootKeyName = "ActiveManager";

		private RegistryKey m_regDbHandle;

		private RegistryKey m_dbgOptionHandle;
	}
}
