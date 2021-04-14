using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class NewDeviceAccessRuleData : BaseRow
	{
		public NewDeviceAccessRuleData(MobileDevice device) : base(device)
		{
			this.device = device;
		}

		[DataMember]
		public DeviceAccessRuleQueryString DeviceTypeQueryString
		{
			get
			{
				return new DeviceAccessRuleQueryString
				{
					QueryString = this.device.DeviceType
				};
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
				return new DeviceAccessRuleQueryString
				{
					IsWildcard = true,
					QueryString = Strings.DeviceModelPickerAll
				};
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		private readonly MobileDevice device;
	}
}
