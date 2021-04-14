using System;
using System.Collections.Generic;
using System.Timers;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.Common
{
	public class ConfigurationOverrides : IDisposable
	{
		public ConfigurationOverrides()
		{
			double configDouble = Configuration.GetConfigDouble("ConfigurationOverridesRefreshTimerInterval", 0.0, double.MaxValue, 3600000.0);
			this.refreshTimer = new Timer(configDouble);
			this.refreshTimer.Elapsed += this.RefreshEvent;
			this.refreshTimer.Start();
		}

		public static bool Equals(Dictionary<string, string> left, Dictionary<string, string> right)
		{
			if (object.ReferenceEquals(left, right))
			{
				return true;
			}
			if (left == null || right == null)
			{
				return false;
			}
			if (left.Count != right.Count)
			{
				return false;
			}
			foreach (KeyValuePair<string, string> keyValuePair in left)
			{
				string a;
				if (!right.TryGetValue(keyValuePair.Key, out a) || a != keyValuePair.Value)
				{
					return false;
				}
			}
			return true;
		}

		public void Dispose()
		{
			if (this.disposed)
			{
				return;
			}
			this.disposed = true;
			if (this.refreshTimer != null)
			{
				this.refreshTimer.Dispose();
			}
			GC.SuppressFinalize(this);
		}

		public void Refresh()
		{
			Logger.LogInformationMessage("Refreshing configuration overrides.", new object[0]);
			Dictionary<string, string> dictionary = this.Read();
			if (ConfigurationOverrides.Equals(dictionary, Configuration.Overrides))
			{
				Logger.LogInformationMessage("Overrides unchanged.", new object[0]);
				return;
			}
			Logger.LogInformationMessage("Overrides changed.", new object[0]);
			Configuration.Overrides = dictionary;
		}

		protected virtual Dictionary<string, string> Read()
		{
			return new Dictionary<string, string>();
		}

		private void RefreshEvent(object sender, ElapsedEventArgs e)
		{
			this.Refresh();
		}

		private readonly Timer refreshTimer;

		private bool disposed;
	}
}
