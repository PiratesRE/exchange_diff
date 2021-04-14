using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class EMailSignature : MessagingBase, IEMailSignature, IMessagingBase<EMailSignatureConfiguration, SetEMailSignatureConfiguration>, IEditObjectService<EMailSignatureConfiguration, SetEMailSignatureConfiguration>, IGetObjectService<EMailSignatureConfiguration>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxMessageConfiguration?Identity@R:Self")]
		public PowerShellResults<EMailSignatureConfiguration> GetObject(Identity identity)
		{
			return base.GetObject<EMailSignatureConfiguration>(identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxMessageConfiguration?Identity@R:Self+Set-MailboxMessageConfiguration?Identity@W:Self")]
		public PowerShellResults<EMailSignatureConfiguration> SetObject(Identity identity, SetEMailSignatureConfiguration properties)
		{
			return base.SetObject<EMailSignatureConfiguration, SetEMailSignatureConfiguration>(identity, properties);
		}
	}
}
