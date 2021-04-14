using System;
using System.Threading;
using Microsoft.Win32;

namespace Microsoft.Exchange.Diagnostics
{
	internal class RegistryKeyChangeWatcher : IDisposeTrackable, IDisposable
	{
		public RegistryKeyChangeWatcher(string key, string value, Action<string> successCallback, Action<string> errorCallback)
		{
			this.key = key;
			this.value = value;
			this.successCallback = successCallback;
			this.errorCallback = errorCallback;
			this.timer = new Timer(new TimerCallback(this.ReadRegistryKey), null, 0, -1);
			this.disposeTracker = this.GetDisposeTracker();
		}

		public void Dispose()
		{
			if (this.timer != null)
			{
				this.timer.Dispose();
			}
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTrackerFactory.Get(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		private void ReadRegistryKey(object state)
		{
			try
			{
				using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32))
				{
					if (registryKey != null)
					{
						using (RegistryKey registryKey2 = registryKey.OpenSubKey(this.key))
						{
							if (registryKey2 != null)
							{
								object objA = registryKey2.GetValue(this.value);
								if (this.successCallback != null && !object.Equals(objA, this.registryKeyCurrentValue))
								{
									this.registryKeyCurrentValue = objA;
									this.successCallback(this.registryKeyCurrentValue.ToString());
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				this.errorCallback(ex.Message);
			}
			this.timer.Change(RegistryKeyChangeWatcher.timerElapsedPeriod, -1);
		}

		private static readonly int timerElapsedPeriod = 300000;

		private readonly Timer timer;

		private readonly string key;

		private readonly Action<string> successCallback;

		private readonly Action<string> errorCallback;

		private readonly string value;

		private object registryKeyCurrentValue;

		private DisposeTracker disposeTracker;
	}
}
