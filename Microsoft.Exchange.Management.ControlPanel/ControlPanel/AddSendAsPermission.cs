using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class AddSendAsPermission : UpdateSendAsPermission
	{
		public AddSendAsPermission()
		{
			base["AccessRights"] = "ExtendedRight";
		}

		public override string AssociatedCmdlet
		{
			get
			{
				return "Add-ADPermission";
			}
		}
	}
}
