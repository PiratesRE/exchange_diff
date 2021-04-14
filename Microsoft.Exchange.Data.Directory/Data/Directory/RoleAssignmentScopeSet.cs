using System;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal class RoleAssignmentScopeSet : SimpleScopeSet<RbacScope>
	{
		public RoleAssignmentScopeSet(RbacScope recipientReadScope, RbacScope recipientWriteScope, RbacScope configReadScope, RbacScope configWriteScope) : base(recipientReadScope, recipientWriteScope, configReadScope, configWriteScope)
		{
		}
	}
}
