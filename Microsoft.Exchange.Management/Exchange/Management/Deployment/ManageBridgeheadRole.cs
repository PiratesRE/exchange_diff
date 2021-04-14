using System;
using System.Management.Automation;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public abstract class ManageBridgeheadRole : ManageRole
	{
		protected ManageBridgeheadRole()
		{
			this.StartTransportService = true;
			this.DisableAMFiltering = false;
		}

		[Parameter(Mandatory = false)]
		public bool StartTransportService
		{
			get
			{
				return (bool)base.Fields["StartTransportService"];
			}
			set
			{
				base.Fields["StartTransportService"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DisableAMFiltering
		{
			get
			{
				return (bool)base.Fields["DisableAMFiltering"];
			}
			set
			{
				base.Fields["DisableAMFiltering"] = value;
			}
		}
	}
}
