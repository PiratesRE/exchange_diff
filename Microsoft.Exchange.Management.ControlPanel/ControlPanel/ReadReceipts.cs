using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class ReadReceipts : MessagingBase, IReadReceipts, IMessagingBase<ReadReceiptsConfiguration, SetReadReceiptsConfiguration>, IEditObjectService<ReadReceiptsConfiguration, SetReadReceiptsConfiguration>, IGetObjectService<ReadReceiptsConfiguration>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxMessageConfiguration?Identity@R:Self")]
		public PowerShellResults<ReadReceiptsConfiguration> GetObject(Identity identity)
		{
			return base.GetObject<ReadReceiptsConfiguration>(identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxMessageConfiguration?Identity@R:Self+Set-MailboxMessageConfiguration?Identity@W:Self")]
		public PowerShellResults<ReadReceiptsConfiguration> SetObject(Identity identity, SetReadReceiptsConfiguration properties)
		{
			return base.SetObject<ReadReceiptsConfiguration, SetReadReceiptsConfiguration>(identity, properties);
		}
	}
}
