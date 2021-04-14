using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class GroupMailboxConfigurationReport
	{
		public GroupMailboxConfigurationReport()
		{
			this.ConfigurationActionLatencyStatistics = new Dictionary<GroupMailboxConfigurationAction, LatencyStatistics>();
			this.Warnings = new List<LocalizedString>();
		}

		public Dictionary<GroupMailboxConfigurationAction, LatencyStatistics> ConfigurationActionLatencyStatistics { get; private set; }

		public List<LocalizedString> Warnings { get; private set; }

		public bool IsConfigurationExecuted { get; internal set; }

		public int FoldersCreatedCount { get; internal set; }

		public int FoldersPrivilegedCount { get; internal set; }

		public LatencyStatistics? GetElapsedTime(GroupMailboxConfigurationAction action)
		{
			if (!this.ConfigurationActionLatencyStatistics.ContainsKey(action))
			{
				return null;
			}
			return new LatencyStatistics?(this.ConfigurationActionLatencyStatistics[action]);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(300);
			foreach (KeyValuePair<GroupMailboxConfigurationAction, LatencyStatistics> keyValuePair in this.ConfigurationActionLatencyStatistics)
			{
				stringBuilder.Append(keyValuePair.Key);
				stringBuilder.Append("ElapsedTimeMs=");
				stringBuilder.Append(keyValuePair.Value.ElapsedTime.TotalMilliseconds.ToString("n0"));
				stringBuilder.Append(", ");
			}
			stringBuilder.Append("FoldersCreatedCount=");
			stringBuilder.Append(this.FoldersCreatedCount);
			stringBuilder.Append(", FoldersPrivilegedCount=");
			stringBuilder.Append(this.FoldersPrivilegedCount);
			stringBuilder.Append(", Warnings={");
			stringBuilder.Append(string.Join<LocalizedString>(",", this.Warnings));
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}
	}
}
