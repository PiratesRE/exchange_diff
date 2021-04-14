using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "X400AuthoritativeDomain", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveX400AuthoritativeDomain : RemoveSystemConfigurationObjectTask<X400AuthoritativeDomainIdParameter, X400AuthoritativeDomain>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveAcceptedDomain(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (TopologyProvider.IsAdamTopology())
			{
				base.WriteError(new CannotRunOnEdgeException(), ErrorCategory.InvalidOperation, null);
			}
			base.InternalValidate();
			TaskLogger.LogExit();
		}
	}
}
