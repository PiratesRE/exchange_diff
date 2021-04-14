using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class MessageFormat : MessagingBase, IMessageFormat, IMessagingBase<MessageFormatConfiguration, SetMessageFormatConfiguration>, IEditObjectService<MessageFormatConfiguration, SetMessageFormatConfiguration>, IGetObjectService<MessageFormatConfiguration>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxMessageConfiguration?Identity@R:Self")]
		public PowerShellResults<MessageFormatConfiguration> GetObject(Identity identity)
		{
			return base.GetObject<MessageFormatConfiguration>(identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxMessageConfiguration?Identity@R:Self+Set-MailboxMessageConfiguration?Identity@W:Self")]
		public PowerShellResults<MessageFormatConfiguration> SetObject(Identity identity, SetMessageFormatConfiguration properties)
		{
			return base.SetObject<MessageFormatConfiguration, SetMessageFormatConfiguration>(identity, properties);
		}
	}
}
