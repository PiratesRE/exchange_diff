using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "SyncUser", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetSyncUser : SetADUserBase<NonMailEnabledUserIdParameter, SyncUser>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter SoftDeletedUser { get; set; }

		protected override IConfigDataProvider CreateSession()
		{
			if (this.SoftDeletedUser.IsPresent)
			{
				base.SessionSettings.IncludeSoftDeletedObjects = true;
			}
			return base.CreateSession();
		}

		protected override void ResolveLocalSecondaryIdentities()
		{
			bool includeSoftDeletedObjects = base.TenantGlobalCatalogSession.SessionSettings.IncludeSoftDeletedObjects;
			try
			{
				base.TenantGlobalCatalogSession.SessionSettings.IncludeSoftDeletedObjects = this.SoftDeletedUser;
				base.ResolveLocalSecondaryIdentities();
			}
			finally
			{
				base.TenantGlobalCatalogSession.SessionSettings.IncludeSoftDeletedObjects = includeSoftDeletedObjects;
			}
		}

		protected override bool ShouldCheckAcceptedDomains()
		{
			return false;
		}
	}
}
