using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class RemoveMailboxPolicyBase<T> : RemoveSystemConfigurationObjectTask<MailboxPolicyIdParameter, T> where T : MailboxPolicy, new()
	{
		protected abstract bool HandleRemoveWithAssociatedUsers();

		protected override void InternalProcessRecord()
		{
			T dataObject = base.DataObject;
			if (dataObject.CheckForAssociatedUsers() && !this.HandleRemoveWithAssociatedUsers())
			{
				return;
			}
			base.InternalProcessRecord();
		}
	}
}
