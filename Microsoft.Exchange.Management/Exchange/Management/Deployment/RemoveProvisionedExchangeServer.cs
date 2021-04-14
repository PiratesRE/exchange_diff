using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Remove", "ProvisionedExchangeServer")]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class RemoveProvisionedExchangeServer : RemoveSystemConfigurationObjectTask<ServerIdParameter, Server>
	{
		protected override void InternalProcessRecord()
		{
			if (base.DataObject.IsProvisionedServer)
			{
				((IConfigurationSession)base.DataSession).DeleteTree(base.DataObject, delegate(ADTreeDeleteNotFinishedException de)
				{
					base.WriteVerbose(de.LocalizedString);
				});
				return;
			}
			base.WriteError(new CannotRemoveNonProvisionedServerException(this.Identity.ToString()), ErrorCategory.InvalidOperation, null);
		}
	}
}
