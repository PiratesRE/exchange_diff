using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class Conversations : MessagingBase, IConversations, IMessagingBase<ConversationsConfiguration, SetConversationsConfiguration>, IEditObjectService<ConversationsConfiguration, SetConversationsConfiguration>, IGetObjectService<ConversationsConfiguration>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxMessageConfiguration?Identity@R:Self")]
		public PowerShellResults<ConversationsConfiguration> GetObject(Identity identity)
		{
			return base.GetObject<ConversationsConfiguration>(identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxMessageConfiguration?Identity@R:Self+Set-MailboxMessageConfiguration?Identity@W:Self")]
		public PowerShellResults<ConversationsConfiguration> SetObject(Identity identity, SetConversationsConfiguration properties)
		{
			return base.SetObject<ConversationsConfiguration, SetConversationsConfiguration>(identity, properties);
		}
	}
}
