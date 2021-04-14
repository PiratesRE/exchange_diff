using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class GetTenantADObjectWithIdentityTaskBase<TIdentity, TDataObject> : GetObjectWithIdentityTaskBase<TIdentity, TDataObject> where TIdentity : IIdentityParameter where TDataObject : IConfigurable, new()
	{
		[Parameter]
		public new Fqdn DomainController
		{
			get
			{
				return base.DomainController;
			}
			set
			{
				base.DomainController = value;
			}
		}

		internal ADSessionSettings SessionSettings
		{
			get
			{
				return this.sessionSettings;
			}
		}

		protected virtual AccountPartitionIdParameter AccountPartition
		{
			get
			{
				return (AccountPartitionIdParameter)base.Fields["AccountPartition"];
			}
			set
			{
				base.Fields["AccountPartition"] = value;
			}
		}

		protected override int PageSize
		{
			get
			{
				if (base.InternalResultSize.IsUnlimited)
				{
					return 1000;
				}
				return (int)Math.Min(base.InternalResultSize.Value - base.WriteObjectCount, 1000U);
			}
		}

		internal override void AdjustPageSize(IPageInformation pageInfo)
		{
			ADGenericPagedReader<TDataObject> adgenericPagedReader = pageInfo as ADGenericPagedReader<TDataObject>;
			if (adgenericPagedReader != null)
			{
				adgenericPagedReader.PageSize = this.PageSize;
			}
		}

		protected override void InternalStateReset()
		{
			if (this.AccountPartition != null)
			{
				PartitionId partitionId = RecipientTaskHelper.ResolvePartitionId(this.AccountPartition, new Task.TaskErrorLoggingDelegate(base.WriteError));
				this.sessionSettings = ADSessionSettings.FromAllTenantsPartitionId(partitionId);
			}
			else
			{
				this.ResolveCurrentOrgIdBasedOnIdentity(this.Identity);
				this.sessionSettings = ADSessionSettings.FromCustomScopeSet(base.ScopeSet, base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			}
			base.InternalStateReset();
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			base.CurrentOrganizationId = this.ResolveCurrentOrganization();
			TaskLogger.LogExit();
		}

		protected virtual OrganizationId ResolveCurrentOrganization()
		{
			this.ResolveCurrentOrgIdBasedOnIdentity(this.Identity);
			return base.CurrentOrganizationId ?? base.ExecutingUserOrganizationId;
		}

		protected override void InternalEndProcessing()
		{
			base.InternalEndProcessing();
			this.sessionSettings = null;
		}

		internal const uint LdapDefaultPageSize = 1000U;

		private ADSessionSettings sessionSettings;
	}
}
