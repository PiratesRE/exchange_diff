using System;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal class CorrelatedMonitorInfo
	{
		internal CorrelatedMonitorInfo(string identity, string matchString = null, CorrelatedMonitorInfo.MatchMode matchMode = CorrelatedMonitorInfo.MatchMode.Wildcard)
		{
			this.ParseIdentityAndAssignProperties(identity);
			this.identityStr = this.GetIdentityAsString();
			this.MatchString = (matchString ?? string.Empty);
			this.ModeOfMatch = matchMode;
		}

		internal string Identity
		{
			get
			{
				return this.identityStr;
			}
		}

		internal Component Component { get; private set; }

		internal string MonitorName { get; private set; }

		internal string TargetResource { get; private set; }

		internal string MatchString { get; private set; }

		internal CorrelatedMonitorInfo.MatchMode ModeOfMatch { get; private set; }

		internal void ParseIdentityAndAssignProperties(string identityStr)
		{
			string[] array = identityStr.Split(new char[]
			{
				'\\'
			});
			int num = array.Length;
			if (num < 2)
			{
				throw new InvalidOperationException(string.Format("Invalid monitor identity string {0}", identityStr));
			}
			string key = array[0];
			Component component = null;
			if (!ExchangeComponent.WellKnownComponents.TryGetValue(key, out component))
			{
				throw new InvalidOperationException(string.Format("Invalid health set name specified in the monitor identity {0}", identityStr));
			}
			this.Component = component;
			this.MonitorName = array[1];
			this.TargetResource = ((array.Length > 2) ? array[2] : string.Empty);
		}

		private string GetIdentityAsString()
		{
			if (string.IsNullOrEmpty(this.TargetResource))
			{
				return string.Format("{0}\\{1}", this.Component.Name, this.MonitorName);
			}
			return string.Format("{0}\\{1}\\{2}", this.Component.Name, this.MonitorName, this.TargetResource);
		}

		private readonly string identityStr;

		internal enum MatchMode
		{
			Wildcard,
			RegEx
		}
	}
}
