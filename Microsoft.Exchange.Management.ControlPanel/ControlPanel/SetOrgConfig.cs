using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetOrgConfig : SetObjectProperties
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-OrganizationConfig";
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
		public string GroupNamingPolicy
		{
			internal get
			{
				return (string)base["DistributionGroupNamingPolicy"];
			}
			set
			{
				base["DistributionGroupNamingPolicy"] = value;
			}
		}

		[DataMember]
		public IEnumerable<string> DistributionGroupNameBlockedWordsList
		{
			get
			{
				return (IEnumerable<string>)base["DistributionGroupNameBlockedWordsList"];
			}
			set
			{
				base["DistributionGroupNameBlockedWordsList"] = value;
			}
		}
	}
}
