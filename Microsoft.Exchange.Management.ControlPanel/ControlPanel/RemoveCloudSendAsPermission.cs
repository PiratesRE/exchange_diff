using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class RemoveCloudSendAsPermission : UpdateCloudSendAsPermission
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Remove-RecipientPermission";
			}
		}
	}
}
