using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	[KnownType(typeof(DeviceClassPickerObject))]
	public class DeviceClassPickerObject : BaseRow
	{
		public DeviceClassPickerObject(ActiveSyncDeviceClass deviceClass) : base(deviceClass)
		{
			this.ActiveSyncDeviceClass = deviceClass;
			this.deviceTypeQueryString = new DeviceAccessRuleQueryString
			{
				QueryString = deviceClass.DeviceType
			};
			this.deviceModelQueryString = new DeviceAccessRuleQueryString
			{
				QueryString = deviceClass.DeviceModel
			};
		}

		public DeviceClassPickerObject(DeviceAccessRuleQueryString deviceType, DeviceAccessRuleQueryString deviceModel)
		{
			this.deviceTypeQueryString = deviceType;
			this.deviceModelQueryString = deviceModel;
		}

		private ActiveSyncDeviceClass ActiveSyncDeviceClass { get; set; }

		[DataMember]
		public DeviceAccessRuleQueryString DeviceType
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
		public DeviceAccessRuleQueryString DeviceModel
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
		public string DeviceTypeString
		{
			get
			{
				return this.deviceTypeQueryString.QueryString;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DeviceModelString
		{
			get
			{
				return this.deviceModelQueryString.QueryString;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		public static DeviceAccessRuleQueryString AllDeviceTypeQueryString = new DeviceAccessRuleQueryString
		{
			IsWildcard = true,
			QueryString = Strings.DeviceTypePickerAll
		};

		public static DeviceAccessRuleQueryString AllDeviceModelQueryString = new DeviceAccessRuleQueryString
		{
			IsWildcard = true,
			QueryString = Strings.DeviceModelPickerAll
		};

		private readonly DeviceAccessRuleQueryString deviceTypeQueryString;

		private readonly DeviceAccessRuleQueryString deviceModelQueryString;
	}
}
