using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class DeviceClassPickerFilter : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-ActiveSyncDeviceClass";
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
		public string DeviceType { get; set; }

		[DataMember]
		public bool GroupDeviceType { get; set; }

		public string Filter
		{
			get
			{
				return (string)base["Filter"];
			}
			set
			{
				base["Filter"] = value;
			}
		}
	}
}
