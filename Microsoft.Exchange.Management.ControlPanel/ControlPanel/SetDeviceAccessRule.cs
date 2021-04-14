using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetDeviceAccessRule : SetObjectProperties
	{
		public sealed override string AssociatedCmdlet
		{
			get
			{
				return "Set-ActiveSyncDeviceAccessRule";
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
