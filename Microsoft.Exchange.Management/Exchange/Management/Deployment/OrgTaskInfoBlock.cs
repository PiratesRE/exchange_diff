using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public class OrgTaskInfoBlock : TaskInfoBlock
	{
		public OrgTaskInfoEntry Global
		{
			get
			{
				if (this.global == null)
				{
					this.global = new OrgTaskInfoEntry();
				}
				return this.global;
			}
			set
			{
				this.global = value;
			}
		}

		public OrgTaskInfoEntry Tenant
		{
			get
			{
				if (this.tenant == null)
				{
					this.tenant = new OrgTaskInfoEntry();
				}
				return this.tenant;
			}
			set
			{
				this.tenant = value;
			}
		}

		internal override string GetTask(InstallationCircumstances circumstance)
		{
			switch (circumstance)
			{
			case InstallationCircumstances.Standalone:
				return this.Global.Task;
			case InstallationCircumstances.TenantOrganization:
				return this.Tenant.Task;
			default:
				return string.Empty;
			}
		}

		private OrgTaskInfoEntry global;

		private OrgTaskInfoEntry tenant;
	}
}
