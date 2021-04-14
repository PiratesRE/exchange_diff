using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class RemoveSendAsPermission : UpdateSendAsPermission
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Remove-ADPermission";
			}
		}
	}
}
