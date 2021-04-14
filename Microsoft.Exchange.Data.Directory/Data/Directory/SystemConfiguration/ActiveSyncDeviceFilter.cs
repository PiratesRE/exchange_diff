using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class ActiveSyncDeviceFilter : XMLSerializableBase
	{
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }

		[XmlElement("ApplyForAllDevices")]
		public bool ApplyForAllDevices { get; set; }

		[XmlArray("Rules")]
		[XmlArrayItem("Rule")]
		public List<ActiveSyncDeviceFilterRule> Rules { get; set; }

		public ActiveSyncDeviceFilter() : this(null, null)
		{
		}

		public ActiveSyncDeviceFilter(string name, List<ActiveSyncDeviceFilterRule> rules)
		{
			this.Name = name;
			this.Rules = rules;
		}

		public ActiveSyncDeviceFilter(string name, bool applyForAllDevices)
		{
			this.Name = name;
			this.ApplyForAllDevices = applyForAllDevices;
		}

		public override int GetHashCode()
		{
			int hashCode = this.Name.GetHashCode() ^ this.ApplyForAllDevices.GetHashCode();
			if (this.Rules != null)
			{
				this.Rules.ForEach(delegate(ActiveSyncDeviceFilterRule rule)
				{
					hashCode ^= rule.GetHashCode();
				});
			}
			return hashCode;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			ActiveSyncDeviceFilter activeSyncDeviceFilter = obj as ActiveSyncDeviceFilter;
			return activeSyncDeviceFilter != null && (this.Rules != null || activeSyncDeviceFilter.Rules == null) && (this.Rules == null || activeSyncDeviceFilter.Rules != null) && (string.Equals(this.Name, activeSyncDeviceFilter.Name, StringComparison.OrdinalIgnoreCase) && this.ApplyForAllDevices == activeSyncDeviceFilter.ApplyForAllDevices) && ((this.Rules == null && activeSyncDeviceFilter.Rules == null) || (this.Rules.Count == activeSyncDeviceFilter.Rules.Count && !this.Rules.Except(activeSyncDeviceFilter.Rules).Any<ActiveSyncDeviceFilterRule>()));
		}
	}
}
