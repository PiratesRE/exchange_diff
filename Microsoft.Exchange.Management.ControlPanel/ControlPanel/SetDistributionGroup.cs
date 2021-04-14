using System;
using System.Management.Automation;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetDistributionGroup : SetDistributionGroupBase<SetGroup, UpdateDistributionGroupMember>
	{
		public override string RbacScope
		{
			get
			{
				return "@W:Organization";
			}
		}

		public bool IgnoreNamingPolicy
		{
			get
			{
				return base.ParameterIsSpecified("IgnoreNamingPolicy") && ((SwitchParameter)base["IgnoreNamingPolicy"]).ToBool();
			}
			set
			{
				base["IgnoreNamingPolicy"] = new SwitchParameter(value);
			}
		}
	}
}
