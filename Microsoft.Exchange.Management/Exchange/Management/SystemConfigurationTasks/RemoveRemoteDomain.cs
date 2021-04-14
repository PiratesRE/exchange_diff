using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "RemoteDomain", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public class RemoveRemoteDomain : RemoveSystemConfigurationObjectTask<RemoteDomainIdParameter, DomainContentConfig>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveRemoteDomain(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			if (Server.IsSubscribedGateway(base.GlobalConfigSession))
			{
				base.WriteError(new CannotRunOnSubscribedEdgeException(), ErrorCategory.InvalidOperation, null);
			}
			base.InternalValidate();
		}

		protected override void InternalProcessRecord()
		{
			if (base.DataObject.DomainName != null && base.DataObject.DomainName.Equals(SmtpDomainWithSubdomains.StarDomain))
			{
				base.WriteError(new CannotRemoveDefaultRemoteDomainException(), ErrorCategory.InvalidOperation, this.Identity);
				return;
			}
			base.InternalProcessRecord();
			FfoDualWriter.DeleteFromFfo<DomainContentConfig>(this, base.DataObject);
		}
	}
}
