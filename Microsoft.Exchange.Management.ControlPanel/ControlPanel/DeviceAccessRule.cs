using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	[KnownType(typeof(DeviceAccessRule))]
	public class DeviceAccessRule : DeviceAccessRuleRow
	{
		public DeviceAccessRule(ActiveSyncDeviceAccessRule rule) : base(rule)
		{
			switch (base.ActiveSyncDeviceAccessRule.Characteristic)
			{
			case DeviceAccessCharacteristic.DeviceType:
				this.deviceTypeQueryString = new DeviceAccessRuleQueryString
				{
					QueryString = base.ActiveSyncDeviceAccessRule.QueryString
				};
				this.deviceModelQueryString = new DeviceAccessRuleQueryString
				{
					IsWildcard = true,
					QueryString = Strings.DeviceModelPickerAll
				};
				return;
			case DeviceAccessCharacteristic.DeviceModel:
				this.deviceTypeQueryString = new DeviceAccessRuleQueryString
				{
					IsWildcard = true,
					QueryString = Strings.DeviceTypePickerAll
				};
				this.deviceModelQueryString = new DeviceAccessRuleQueryString
				{
					QueryString = base.ActiveSyncDeviceAccessRule.QueryString
				};
				return;
			case DeviceAccessCharacteristic.DeviceOS:
			case DeviceAccessCharacteristic.UserAgent:
				this.deviceTypeQueryString = new DeviceAccessRuleQueryString
				{
					IsWildcard = true,
					QueryString = Strings.DeviceTypePickerAll
				};
				this.deviceModelQueryString = new DeviceAccessRuleQueryString
				{
					IsWildcard = true,
					QueryString = Strings.DeviceTypePickerAll
				};
				return;
			default:
				throw new FaultException(Strings.InvalidDeviceAccessCharacteristic);
			}
		}

		[DataMember]
		public DeviceAccessRuleQueryString DeviceTypeQueryString
		{
			get
			{
				return this.deviceTypeQueryString;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public DeviceAccessRuleQueryString DeviceModelQueryString
		{
			get
			{
				return this.deviceModelQueryString;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string AccessLevel
		{
			get
			{
				return base.ActiveSyncDeviceAccessRule.AccessLevel.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		private DeviceAccessRuleQueryString deviceTypeQueryString;

		private DeviceAccessRuleQueryString deviceModelQueryString;
	}
}
