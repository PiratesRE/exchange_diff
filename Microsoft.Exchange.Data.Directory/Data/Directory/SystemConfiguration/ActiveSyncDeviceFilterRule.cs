using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class ActiveSyncDeviceFilterRule : XMLSerializableBase
	{
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }

		[XmlAttribute(AttributeName = "characteristic")]
		public DeviceAccessCharacteristic Characteristic { get; set; }

		[XmlAttribute(AttributeName = "operator")]
		public DeviceFilterOperator Operator { get; set; }

		[XmlText]
		public string Value { get; set; }

		public ActiveSyncDeviceFilterRule() : this(null, DeviceAccessCharacteristic.DeviceType, DeviceFilterOperator.Equals, null)
		{
		}

		public ActiveSyncDeviceFilterRule(string name, DeviceAccessCharacteristic charactersitic, DeviceFilterOperator filterOperator, string value)
		{
			this.Name = name;
			this.Characteristic = charactersitic;
			this.Operator = filterOperator;
			this.Value = value;
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode() ^ this.Characteristic.GetHashCode() ^ this.Operator.GetHashCode() ^ this.Value.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			ActiveSyncDeviceFilterRule activeSyncDeviceFilterRule = obj as ActiveSyncDeviceFilterRule;
			return activeSyncDeviceFilterRule != null && (string.Equals(this.Name, activeSyncDeviceFilterRule.Name, StringComparison.OrdinalIgnoreCase) && string.Equals(this.Value, activeSyncDeviceFilterRule.Value, StringComparison.OrdinalIgnoreCase) && this.Characteristic == activeSyncDeviceFilterRule.Characteristic) && this.Operator == activeSyncDeviceFilterRule.Operator;
		}
	}
}
