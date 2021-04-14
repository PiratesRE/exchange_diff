using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetActiveSyncMailboxPolicyParams : BaseActiveSyncMailboxPolicyParams
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-MobileMailboxPolicy";
			}
		}

		public override void ProcessPolicyParams(ActiveSyncMailboxPolicyObject originalPolicy)
		{
			if (originalPolicy == null)
			{
				throw new ArgumentNullException("originalPolicy");
			}
			base.ProcessPolicyParams(originalPolicy);
		}
	}
}
