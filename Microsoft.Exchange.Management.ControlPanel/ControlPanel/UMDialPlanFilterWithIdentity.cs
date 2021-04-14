using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UMDialPlanFilterWithIdentity : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-UMDialPlan";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@R:Organization";
			}
		}

		[DataMember]
		public Identity DialPlanIdentity { get; set; }

		[DataMember]
		public bool IsInternational { get; set; }
	}
}
