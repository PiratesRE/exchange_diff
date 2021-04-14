using System;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.Common.ConfigurableParameters;
using Microsoft.Exchange.Cluster.Common.Registry;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.Shared
{
	internal class RegistryStateAccess : StateAccessor, IDisposeTrackable, IDisposable
	{
		public RegistryStateAccess(string registryKey)
		{
			this.m_registryKeyStr = registryKey;
			this.m_disposeTracker = this.GetDisposeTracker();
			this.m_key = this.TryOpenKey(registryKey);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<RegistryStateAccess>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.m_disposeTracker != null)
			{
				this.m_disposeTracker.Suppress();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.m_key != null)
				{
					this.m_key.Close();
					this.m_key = null;
				}
				if (this.m_disposeTracker != null)
				{
					this.m_disposeTracker.Dispose();
					this.m_disposeTracker = null;
				}
			}
			base.Dispose(disposing);
		}

		private IRegistryKey TryOpenKey(string registryKey)
		{
			Exception ex;
			return SharedDependencies.RegistryKeyProvider.TryOpenKey(registryKey, ref ex);
		}

		protected override Exception ReadValueInternal<T>(string valueName, T defaultValue, out object value)
		{
			object tmpValue = null;
			value = defaultValue;
			if (this.m_key == null)
			{
				RegistryParameterKeyNotOpenedException ex = new RegistryParameterKeyNotOpenedException(this.m_registryKeyStr);
				return new RegistryParameterReadException(valueName, ex.Message, ex);
			}
			Exception ex2 = RegistryUtil.RunRegistryFunction(delegate()
			{
				tmpValue = this.m_key.GetValue(valueName, defaultValue);
			});
			if (ex2 == null)
			{
				value = tmpValue;
			}
			else
			{
				ex2 = new RegistryParameterReadException(valueName, ex2.Message, ex2);
			}
			return ex2;
		}

		protected override Exception SetValueInternal(string valueName, string value)
		{
			return this.RunSetValueFunc(valueName, delegate
			{
				this.m_key.SetValue(valueName, value, RegistryValueKind.String);
			});
		}

		protected override Exception SetValueInternal(string valueName, int value)
		{
			return this.RunSetValueFunc(valueName, delegate
			{
				this.m_key.SetValue(valueName, value, RegistryValueKind.DWord);
			});
		}

		private Exception RunSetValueFunc(string valueName, Action setValueFunc)
		{
			if (this.m_key == null)
			{
				RegistryParameterKeyNotOpenedException ex = new RegistryParameterKeyNotOpenedException(this.m_registryKeyStr);
				return new RegistryParameterWriteException(valueName, ex.Message, ex);
			}
			Exception ex2 = RegistryUtil.RunRegistryFunction(setValueFunc);
			if (ex2 != null)
			{
				ex2 = new RegistryParameterWriteException(valueName, ex2.Message, ex2);
			}
			return ex2;
		}

		private readonly string m_registryKeyStr;

		private IRegistryKey m_key;

		private DisposeTracker m_disposeTracker;
	}
}
