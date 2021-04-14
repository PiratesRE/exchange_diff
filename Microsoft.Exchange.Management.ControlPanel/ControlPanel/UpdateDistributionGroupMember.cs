using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UpdateDistributionGroupMember : UpdateDistributionGroupMemberBase
	{
		public override string RbacScope
		{
			get
			{
				return "@W:Organization";
			}
		}
	}
}
