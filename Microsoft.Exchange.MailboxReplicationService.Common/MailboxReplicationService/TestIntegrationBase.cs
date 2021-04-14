using System;
using System.Security;
using System.Threading;
using Microsoft.Win32;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class TestIntegrationBase
	{
		protected TestIntegrationBase(string regKeyName, bool autoRefresh)
		{
			this.regKeyName = regKeyName;
			this.lastRefreshTimestamp = DateTime.MinValue;
			bool enabled = this.Enabled;
			if (!autoRefresh)
			{
				this.lastRefreshTimestamp = DateTime.MaxValue;
			}
		}

		public bool Enabled
		{
			get
			{
				if (this.lastRefreshTimestamp < DateTime.UtcNow - TestIntegrationBase.AutoRefreshInterval)
				{
					this.lastRefreshTimestamp = DateTime.UtcNow;
					using (RegistryKey registryKey = this.OpenTestKey(false))
					{
						this.testIntegrationEnabled = (this.GetParamValueInt(registryKey, "TestIntegrationEnabled", 0, 1) == 1);
					}
				}
				return this.testIntegrationEnabled;
			}
		}

		public void ForceRefresh()
		{
			this.lastRefreshTimestamp = DateTime.MinValue;
		}

		public int GetIntValue(string valueName, int defaultValue, int minValue, int maxValue)
		{
			if (!this.Enabled)
			{
				return defaultValue;
			}
			int result;
			using (RegistryKey registryKey = this.OpenTestKey(false))
			{
				result = (this.GetParamValueInt(registryKey, valueName, minValue, maxValue) ?? defaultValue);
			}
			return result;
		}

		public int GetIntValueAndDecrement(string valueName, int defaultValue, int minValue, int maxValue)
		{
			if (!this.Enabled)
			{
				return defaultValue;
			}
			int num;
			using (RegistryKey registryKey = this.OpenTestKey(true))
			{
				num = (this.GetParamValueInt(registryKey, valueName, minValue, maxValue) ?? defaultValue);
				if (num > 0)
				{
					registryKey.SetValue(valueName, num - 1);
				}
			}
			return num;
		}

		public string GetStrValue(string valueName)
		{
			if (!this.Enabled)
			{
				return null;
			}
			string paramValueStr;
			using (RegistryKey registryKey = this.OpenTestKey(false))
			{
				paramValueStr = this.GetParamValueStr(registryKey, valueName);
			}
			return paramValueStr;
		}

		public Guid GetGuidValue(string valueName)
		{
			string strValue = this.GetStrValue(valueName);
			Guid result;
			if (!string.IsNullOrEmpty(strValue) && Guid.TryParse(strValue, out result))
			{
				return result;
			}
			return Guid.Empty;
		}

		public void Barrier(string valueKeyName, Action abortDelegate)
		{
			if (!this.Enabled)
			{
				return;
			}
			DateTime t = DateTime.UtcNow + TestIntegrationBase.MaxBarrierDelay;
			bool flag = false;
			using (RegistryKey registryKey = this.OpenTestKey(true))
			{
				try
				{
					while (DateTime.UtcNow < t)
					{
						if (!this.Enabled || this.GetParamValueInt(registryKey, valueKeyName, 0, 1) != 1)
						{
							return;
						}
						if (abortDelegate != null)
						{
							abortDelegate();
						}
						CommonUtils.CheckForServiceStopping();
						MrsTracer.Common.Debug("Waiting at breakpoint {0}", new object[]
						{
							valueKeyName
						});
						if (!flag)
						{
							registryKey.SetValue("CurrentBreakpoint", valueKeyName);
							flag = true;
						}
						Thread.Sleep(TestIntegrationBase.BarrierPollInterval);
					}
					MrsTracer.Common.Debug("Breakpoint {0} timed out, unblocking execution", new object[]
					{
						valueKeyName
					});
				}
				finally
				{
					if (flag)
					{
						registryKey.DeleteValue("CurrentBreakpoint", false);
					}
				}
			}
		}

		protected RegistryKey OpenTestKey(bool writable)
		{
			RegistryKey result = null;
			try
			{
				result = Registry.LocalMachine.OpenSubKey(this.regKeyName, writable);
			}
			catch (SecurityException)
			{
			}
			catch (ArgumentException)
			{
			}
			return result;
		}

		protected int? GetParamValueInt(RegistryKey key, string valueName, int minValue, int maxValue)
		{
			if (key == null)
			{
				return null;
			}
			object value = key.GetValue(valueName);
			if (value == null || !(value is int))
			{
				return null;
			}
			int num = (int)value;
			if (num < minValue)
			{
				num = minValue;
			}
			else if (num > maxValue)
			{
				num = maxValue;
			}
			return new int?(num);
		}

		protected bool GetFlagValue(string flagName)
		{
			return this.GetFlagValue(flagName, false);
		}

		protected bool GetFlagValue(string flagName, bool defaultValue)
		{
			if (!this.Enabled)
			{
				return defaultValue;
			}
			bool result;
			using (RegistryKey registryKey = this.OpenTestKey(false))
			{
				int? paramValueInt = this.GetParamValueInt(registryKey, flagName, 0, 1);
				if (paramValueInt == null)
				{
					result = defaultValue;
				}
				else
				{
					result = (paramValueInt.Value != 0);
				}
			}
			return result;
		}

		private string GetParamValueStr(RegistryKey key, string valueName)
		{
			if (key == null)
			{
				return null;
			}
			object value = key.GetValue(valueName);
			if (value == null || !(value is string))
			{
				return null;
			}
			return (string)value;
		}

		public const string TestIntegrationEnabledName = "TestIntegrationEnabled";

		public const string CurrentBreakpointName = "CurrentBreakpoint";

		private static readonly TimeSpan MaxBarrierDelay = TimeSpan.FromHours(1.0);

		private static readonly TimeSpan BarrierPollInterval = TimeSpan.FromSeconds(1.0);

		private static readonly TimeSpan AutoRefreshInterval = TimeSpan.FromSeconds(60.0);

		private readonly string regKeyName;

		private bool testIntegrationEnabled;

		private DateTime lastRefreshTimestamp;
	}
}
