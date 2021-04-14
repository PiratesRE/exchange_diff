using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Cluster.Common.Registry;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmFaultInjectHelper
	{
		internal AmFaultInjectHelper()
		{
			this.Init();
		}

		internal bool IsEnabled
		{
			get
			{
				return this.m_isEnabled && !this.IsTempDisabled;
			}
		}

		internal bool IsTempDisabled
		{
			get
			{
				bool isTempDisabled = this.m_isTempDisabled;
				int num = 0;
				this.ReadProperty<int>("Software\\Microsoft\\Exchange\\ActiveManager\\FaultInject", "TempDisabled", out num, 0);
				this.m_isTempDisabled = (num > 0);
				if (isTempDisabled != this.m_isTempDisabled)
				{
					if (this.m_isTempDisabled)
					{
						AmTrace.Debug("**** AM fault injector is temporarily disabled ****", new object[0]);
					}
					else
					{
						AmTrace.Debug("**** AM fault injector is re-enabled ****", new object[0]);
					}
				}
				return this.m_isTempDisabled;
			}
		}

		internal void Init()
		{
			int num = 0;
			this.ReadProperty<int>("Software\\Microsoft\\Exchange\\ActiveManager\\FaultInject", "Enabled", out num, 0);
			this.m_isEnabled = (num > 0);
			if (this.m_isEnabled)
			{
				AmTrace.Debug("**** AM fault injector is enabled ****", new object[0]);
				if (this.IsTempDisabled)
				{
					AmTrace.Debug("**** But it is temporarily disabled by the TempDisabled setting ****", new object[0]);
				}
			}
		}

		internal void SleepIfRequired(string propertyName)
		{
			this.SleepIfRequired(null, propertyName);
		}

		internal void SleepIfRequired(Guid dbGuid, string propertyName)
		{
			this.SleepIfRequired(dbGuid.ToString(), propertyName);
		}

		internal void SleepIfRequired(string subKeyName, string propertyName)
		{
			if (!this.IsEnabled)
			{
				return;
			}
			string text = "Software\\Microsoft\\Exchange\\ActiveManager\\FaultInject";
			if (!string.IsNullOrEmpty(subKeyName))
			{
				text = text + "\\" + subKeyName;
			}
			int num = 0;
			int num2 = 0;
			while (!this.IsTempDisabled)
			{
				bool flag = this.ReadProperty<int>(text, propertyName, out num2, 0);
				if (!flag && !string.IsNullOrEmpty(subKeyName))
				{
					flag = this.ReadProperty<int>("Software\\Microsoft\\Exchange\\ActiveManager\\FaultInject", propertyName, out num2, 0);
				}
				if (!flag || num >= num2)
				{
					break;
				}
				if (num % 30 == 0)
				{
					if (num == 0)
					{
						AmTrace.Debug("Sleep induced at:", new object[0]);
						AmTrace.Debug(new StackTrace(true).ToString(), new object[0]);
						AmTrace.Debug("Starting to sleeping for {0}\\{1}: (elasped={2}, max={3})", new object[]
						{
							subKeyName,
							propertyName,
							num,
							num2
						});
					}
					else
					{
						AmTrace.Debug("Sleeping for {0}\\{1}: (elasped={2}, max={3})", new object[]
						{
							subKeyName,
							propertyName,
							num,
							num2
						});
					}
				}
				Thread.Sleep(1000);
				num++;
			}
			if (num > 0)
			{
				AmTrace.Debug("Finished sleeping for {0}\\{1}: (elasped={2}, max={3})", new object[]
				{
					subKeyName,
					propertyName,
					num,
					num2
				});
			}
		}

		internal void GenerateMapiExceptionIfRequired(Guid dbGuid, AmServerName serverName)
		{
			if (!this.IsEnabled)
			{
				return;
			}
			int num = 0;
			this.ReadDbOperationProperty<int>(dbGuid, serverName, "GenerateMapiError", out num, 0);
			if (num == 0)
			{
				return;
			}
			AmTrace.Debug("AmInject: GenerateMapiError enabled for {0},{1}", new object[]
			{
				dbGuid,
				AmServerName.IsNullOrEmpty(serverName) ? "<null>" : serverName.NetbiosName
			});
			int num2 = 0;
			int num3 = 0;
			bool flag = this.ReadDbOperationProperty<int>(dbGuid, serverName, "MapiHResult", out num2, 0);
			bool flag2 = this.ReadDbOperationProperty<int>(dbGuid, serverName, "MapiLowLevelError", out num3, 0);
			if (flag && flag2)
			{
				AmTrace.Debug("AmInject: Generating mapi exception (hr={0}, ec={1})", new object[]
				{
					num2,
					num3
				});
				MapiExceptionHelper.ThrowIfError(string.Format("Database operation failed with Mapi error. (hr={0}, ec={1})", num2, num3), num2, num3);
			}
		}

		internal bool ReadDbOperationProperty<T>(Guid dbGuid, AmServerName serverName, string propertyName, out T foundValue, T defaultValue)
		{
			string str = "Software\\Microsoft\\Exchange\\ActiveManager\\FaultInject";
			string text = str + "\\" + dbGuid.ToString();
			string text2 = string.Empty;
			if (!AmServerName.IsNullOrEmpty(serverName))
			{
				text2 = text + "\\" + serverName.NetbiosName;
			}
			bool flag = false;
			foundValue = defaultValue;
			if (!string.IsNullOrEmpty(text2))
			{
				flag = this.ReadProperty<T>(text2, propertyName, out foundValue, defaultValue);
			}
			if (!flag)
			{
				flag = this.ReadProperty<T>(text, propertyName, out foundValue, defaultValue);
			}
			if (!flag)
			{
				flag = this.ReadProperty<T>("Software\\Microsoft\\Exchange\\ActiveManager\\FaultInject", propertyName, out foundValue, defaultValue);
			}
			return flag;
		}

		private bool ReadProperty<T>(string keyName, string propertyName, out T foundValue, T defaultValue)
		{
			bool result = false;
			foundValue = defaultValue;
			Exception ex = null;
			using (IRegistryKey registryKey = SharedDependencies.RegistryKeyProvider.TryOpenKey(keyName, ref ex))
			{
				if (registryKey != null)
				{
					object value = registryKey.GetValue(propertyName, defaultValue);
					if (value != null)
					{
						result = true;
						foundValue = (T)((object)value);
					}
				}
			}
			return result;
		}

		private const string RootKey = "Software\\Microsoft\\Exchange\\ActiveManager\\FaultInject";

		private const string EnabledProperty = "Enabled";

		private const string TempDisabledProperty = "TempDisabled";

		private const string GenerateMapiErrorProperty = "GenerateMapiError";

		private const string MapiHResultProperty = "MapiHResult";

		private const string MapiLowLevelErorProperty = "MapiLowLevelError";

		private bool m_isEnabled;

		private bool m_isTempDisabled;
	}
}
