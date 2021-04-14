using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class AddCloudSendAsPermission : UpdateCloudSendAsPermission
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Add-RecipientPermission";
			}
		}
	}
}
