using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "RemovedMailbox")]
	public sealed class GetRemovedMailbox : GetTenantADObjectWithIdentityTaskBase<RemovedMailboxIdParameter, RemovedMailbox>
	{
		protected override ObjectId RootId
		{
			get
			{
				return this.rootId;
			}
		}

		[Parameter]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		[Parameter]
		public Unlimited<uint> ResultSize
		{
			get
			{
				return base.InternalResultSize;
			}
			set
			{
				base.InternalResultSize = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return MailboxTaskHelper.GetSessionForDeletedObjects(base.DomainController, base.CurrentOrganizationId, base.ExecutingUserOrganizationId);
		}

		protected override bool ShouldSupportPreResolveOrgIdBasedOnIdentity()
		{
			return true;
		}

		protected override OrganizationId ResolveCurrentOrganization()
		{
			if (this.Organization != null)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.ExecutingUserOrganizationId, null, false);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, base.NetCredential, sessionSettings, ConfigScopes.TenantSubTree, 99, "ResolveCurrentOrganization", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\mailbox\\GetRemovedMailbox.cs");
				tenantOrTopologyConfigurationSession.UseConfigNC = false;
				ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Organization, tenantOrTopologyConfigurationSession, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())));
				this.rootId = adorganizationalUnit.OrganizationId.OrganizationalUnit;
				return adorganizationalUnit.OrganizationId;
			}
			OrganizationId organizationId = base.ResolveCurrentOrganization();
			this.rootId = organizationId.OrganizationalUnit;
			return organizationId;
		}

		protected override IEnumerable<RemovedMailbox> GetPagedData()
		{
			base.WriteVerbose(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(base.DataSession, typeof(RemovedMailbox), this.InternalFilter, null, this.DeepSearch));
			RemovedMailboxIdParameter removedMailboxIdParameter = new RemovedMailboxIdParameter("*");
			return removedMailboxIdParameter.GetObjects<RemovedMailbox>(this.RootId, base.DataSession);
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return (RemovedMailbox)dataObject;
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			base.WriteResult(this.ConvertDataObjectToPresentationObject(dataObject));
		}

		private ADObjectId rootId;
	}
}
