using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class RoleAssignmentPolicyRow : BaseRow
	{
		[DataMember]
		public string Name { get; private set; }

		public RoleAssignmentPolicyRow(RoleAssignmentPolicy policy) : base(policy)
		{
			this.Name = policy.Name;
		}
	}
}
