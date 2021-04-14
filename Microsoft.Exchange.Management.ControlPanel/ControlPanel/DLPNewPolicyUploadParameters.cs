using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class DLPNewPolicyUploadParameters : DLPPolicyUploadParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "New-DLPPolicy";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Organization";
			}
		}

		[DataMember]
		public string Name
		{
			get
			{
				return (string)base["Name"];
			}
			set
			{
				base["Name"] = value;
			}
		}

		[DataMember]
		public string Description
		{
			get
			{
				return (string)base["Description"];
			}
			set
			{
				base["Description"] = value;
			}
		}
	}
}
