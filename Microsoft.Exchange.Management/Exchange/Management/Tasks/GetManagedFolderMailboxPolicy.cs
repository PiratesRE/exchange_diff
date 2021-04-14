using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("get", "ManagedFolderMailboxPolicy", DefaultParameterSetName = "Identity")]
	public sealed class GetManagedFolderMailboxPolicy : GetMailboxPolicyBase<ManagedFolderMailboxPolicy>
	{
		protected override void WriteResult<T>(IEnumerable<T> dataObjects)
		{
			TaskLogger.LogEnter();
			foreach (T t in dataObjects)
			{
				IConfigurable configurable = t;
				ManagedFolderMailboxPolicy managedFolderMailboxPolicy = (ManagedFolderMailboxPolicy)configurable;
				if (!managedFolderMailboxPolicy.AreDefaultManagedFolderLinksUnique(base.DataSession as IConfigurationSession))
				{
					this.WriteWarning(Strings.WarningMisconfiguredElcMailboxPolicy(managedFolderMailboxPolicy.Name));
				}
				else
				{
					base.WriteResult(managedFolderMailboxPolicy);
				}
			}
			TaskLogger.LogExit();
		}
	}
}
