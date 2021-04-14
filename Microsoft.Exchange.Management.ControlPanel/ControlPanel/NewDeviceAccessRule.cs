using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class NewDeviceAccessRule : SetObjectProperties
	{
		public sealed override string AssociatedCmdlet
		{
			get
			{
				return "New-ActiveSyncDeviceAccessRule";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@C:OrganizationConfig";
			}
		}

		[DataMember]
		public DeviceAccessRuleQueryString DeviceTypeQueryString { get; set; }

		[DataMember]
		public DeviceAccessRuleQueryString DeviceModelQueryString { get; set; }

		[DataMember]
		public string AccessLevel
		{
			get
			{
				return (string)base[ActiveSyncDeviceAccessRuleSchema.AccessLevel];
			}
			set
			{
				base[ActiveSyncDeviceAccessRuleSchema.AccessLevel] = value;
			}
		}
	}
}
