using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class MessageOptions : MessagingBase, IMessageOptions, IMessagingBase<MessageOptionsConfiguration, SetMessageOptionsConfiguration>, IEditObjectService<MessageOptionsConfiguration, SetMessageOptionsConfiguration>, IGetObjectService<MessageOptionsConfiguration>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxMessageConfiguration?Identity@R:Self")]
		public PowerShellResults<MessageOptionsConfiguration> GetObject(Identity identity)
		{
			return base.GetObject<MessageOptionsConfiguration>(identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxMessageConfiguration?Identity@R:Self+Set-MailboxMessageConfiguration?Identity@W:Self")]
		public PowerShellResults<MessageOptionsConfiguration> SetObject(Identity identity, SetMessageOptionsConfiguration properties)
		{
			return base.SetObject<MessageOptionsConfiguration, SetMessageOptionsConfiguration>(identity, properties);
		}
	}
}
