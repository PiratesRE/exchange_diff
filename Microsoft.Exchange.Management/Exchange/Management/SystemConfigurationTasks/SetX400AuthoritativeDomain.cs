using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "X400AuthoritativeDomain", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetX400AuthoritativeDomain : SetSystemConfigurationObjectTask<X400AuthoritativeDomainIdParameter, X400AuthoritativeDomain>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetAcceptedDomain(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			if (TopologyProvider.IsAdamTopology())
			{
				base.WriteError(new CannotRunOnEdgeException(), ErrorCategory.InvalidOperation, null);
			}
			base.InternalValidate();
			NewX400AuthoritativeDomain.ValidateNoDuplicates(this.DataObject, this.ConfigurationSession, new Task.TaskErrorLoggingDelegate(base.WriteError));
		}
	}
}
