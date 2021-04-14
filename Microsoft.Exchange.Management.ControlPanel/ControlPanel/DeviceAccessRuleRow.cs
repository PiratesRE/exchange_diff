using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class DeviceAccessRuleRow : BaseRow
	{
		public DeviceAccessRuleRow(ActiveSyncDeviceAccessRule rule) : base(rule)
		{
			this.ActiveSyncDeviceAccessRule = rule;
		}

		public ActiveSyncDeviceAccessRule ActiveSyncDeviceAccessRule { get; set; }

		[DataMember]
		public string Name
		{
			get
			{
				return this.ActiveSyncDeviceAccessRule.Name;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string QueryString
		{
			get
			{
				return this.ActiveSyncDeviceAccessRule.QueryString;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string CharacteristicDescription
		{
			get
			{
				switch (this.ActiveSyncDeviceAccessRule.Characteristic)
				{
				case DeviceAccessCharacteristic.DeviceType:
					return Strings.DeviceAccessRulesDeviceType;
				case DeviceAccessCharacteristic.DeviceModel:
					return Strings.DeviceAccessRulesDeviceModel;
				case DeviceAccessCharacteristic.DeviceOS:
					return Strings.DeviceAccessRulesDeviceOS;
				case DeviceAccessCharacteristic.UserAgent:
					return Strings.DeviceAccessRulesUserAgent;
				default:
					throw new FaultException(Strings.InvalidDeviceAccessCharacteristic);
				}
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string AccessLevelDescription
		{
			get
			{
				switch (this.ActiveSyncDeviceAccessRule.AccessLevel)
				{
				case DeviceAccessLevel.Allow:
					return Strings.DeviceAccessRulesAllow;
				case DeviceAccessLevel.Block:
					return Strings.DeviceAccessRulesBlock;
				case DeviceAccessLevel.Quarantine:
					return Strings.DeviceAccessRulesQuarantine;
				default:
					throw new FaultException(Strings.InvalidAccessLevel);
				}
			}
			private set
			{
				throw new NotSupportedException();
			}
		}
	}
}
