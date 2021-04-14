using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal abstract class UpdateCloudSendAsPermission : WebServiceParameters
	{
		public UpdateCloudSendAsPermission()
		{
			base["AccessRights"] = RecipientAccessRight.SendAs;
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Organization";
			}
		}

		public string Trustee
		{
			get
			{
				return (string)base["Trustee"];
			}
			set
			{
				base["Trustee"] = value;
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
