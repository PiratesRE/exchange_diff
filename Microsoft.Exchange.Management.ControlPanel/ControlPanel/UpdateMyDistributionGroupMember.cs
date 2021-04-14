using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UpdateMyDistributionGroupMember : UpdateDistributionGroupMemberBase
	{
		public override string RbacScope
		{
			get
			{
				return "@W:MyDistributionGroups";
			}
		}
	}
}
