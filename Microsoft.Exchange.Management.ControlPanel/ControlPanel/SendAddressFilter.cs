using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SendAddressFilter : SelfMailboxParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-SendAddress";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@R:Self";
			}
		}

		[DataMember]
		public string AddressId
		{
			get
			{
				return (string)base["AddressId"];
			}
			set
			{
				base["AddressId"] = value.Trim();
			}
		}
	}
}
