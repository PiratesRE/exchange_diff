using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class AddRetentionPolicyTag : SetObjectProperties
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-RetentionPolicyTag";
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
		public Identity[] OptionalInMailbox { get; set; }
	}
}
