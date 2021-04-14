using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal abstract class UpdateSendAsPermission : WebServiceParameters
	{
		public UpdateSendAsPermission()
		{
			base["ExtendedRights"] = "send as";
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Organization";
			}
		}

		public string User
		{
			get
			{
				return (string)base["User"];
			}
			set
			{
				base["User"] = value;
			}
		}

		public string Identity
		{
			get
			{
				return (string)base["Identity"];
			}
			set
			{
				base["Identity"] = value;
			}
		}
	}
}
