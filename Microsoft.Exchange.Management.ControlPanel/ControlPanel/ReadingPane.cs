using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class ReadingPane : MessagingBase, IReadingPane, IMessagingBase<ReadingPaneConfiguration, SetReadingPaneConfiguration>, IEditObjectService<ReadingPaneConfiguration, SetReadingPaneConfiguration>, IGetObjectService<ReadingPaneConfiguration>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxMessageConfiguration?Identity@R:Self")]
		public PowerShellResults<ReadingPaneConfiguration> GetObject(Identity identity)
		{
			return base.GetObject<ReadingPaneConfiguration>(identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxMessageConfiguration?Identity@R:Self+Set-MailboxMessageConfiguration?Identity@W:Self")]
		public PowerShellResults<ReadingPaneConfiguration> SetObject(Identity identity, SetReadingPaneConfiguration properties)
		{
			return base.SetObject<ReadingPaneConfiguration, SetReadingPaneConfiguration>(identity, properties);
		}
	}
}
