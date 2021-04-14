using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.Management.Tasks.MailboxSearch;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Get", "MailboxSearch", DefaultParameterSetName = "Identity")]
	public sealed class GetMailboxSearch : GetTenantADObjectWithIdentityTaskBase<EwsStoreObjectIdParameter, MailboxDiscoverySearch>
	{
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

		[Parameter(Mandatory = false)]
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

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "InPlaceHoldIdentity")]
		public string InPlaceHoldIdentity
		{
			get
			{
				return (string)base.Fields["InPlaceHoldIdentity"];
			}
			set
			{
				base.Fields["InPlaceHoldIdentity"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter ShowDeletionInProgressSearches
		{
			get
			{
				return (SwitchParameter)(base.Fields["ShowDeletionInProgressSearches"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ShowDeletionInProgressSearches"] = value;
			}
		}

		protected override Unlimited<uint> DefaultResultSize
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return new DiscoverySearchDataProvider(base.CurrentOrganizationId);
		}

		protected override void InternalValidate()
		{
			if (this.Identity != null && this.ShowDeletionInProgressSearches == true)
			{
				base.WriteError(new MailboxSearchTaskException(Strings.InvalidOperationIdentityWithShowDeletion), ErrorCategory.InvalidOperation, null);
			}
		}

		protected override void InternalProcessRecord()
		{
			if (this.InPlaceHoldIdentity != null)
			{
				MailboxDiscoverySearch mailboxDiscoverySearch = ((DiscoverySearchDataProvider)base.DataSession).FindByInPlaceHoldIdentity(this.InPlaceHoldIdentity);
				if (mailboxDiscoverySearch == null)
				{
					base.WriteError(new ManagementObjectNotFoundException(Strings.MailboxSearchObjectWithHoldIdentityNotFound(this.InPlaceHoldIdentity)), ExchangeErrorCategory.Context, null);
				}
				this.WriteResult(mailboxDiscoverySearch);
				return;
			}
			if (this.Identity != null)
			{
				string text = this.Identity.ToString();
				MailboxDataProvider mailboxDataProvider = Utils.GetMailboxDataProvider(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, new Task.TaskErrorLoggingDelegate(base.WriteError));
				if (Utils.IsLegacySearchObjectIdentity(text))
				{
					MailboxDiscoverySearch mailboxDiscoverySearch2 = ((DiscoverySearchDataProvider)base.DataSession).FindByLegacySearchObjectIdentity(text);
					if (mailboxDiscoverySearch2 != null)
					{
						this.WriteResult(mailboxDiscoverySearch2);
						return;
					}
					LocalizedString? localizedString;
					IEnumerable<SearchObject> dataObjects = base.GetDataObjects<SearchObject>(new SearchObjectIdParameter(text), mailboxDataProvider, this.RootId, base.OptionalIdentityData, out localizedString);
					foreach (SearchObject searchObject in dataObjects)
					{
						base.WriteResult(new MailboxSearchObject(searchObject, searchObject.SearchStatus ?? new SearchStatus()));
					}
					if (!base.HasErrors && base.WriteObjectCount == 0U)
					{
						base.WriteError(new ManagementObjectNotFoundException(localizedString ?? base.GetErrorMessageObjectNotFound(text, null, null)), (ErrorCategory)1003, null);
						return;
					}
				}
				else
				{
					SearchObject e14SearchObjectByName = Utils.GetE14SearchObjectByName(this.Identity.ToString(), mailboxDataProvider);
					if (e14SearchObjectByName == null)
					{
						base.InternalProcessRecord();
						return;
					}
					base.WriteResult(new MailboxSearchObject(e14SearchObjectByName, e14SearchObjectByName.SearchStatus ?? new SearchStatus()));
					return;
				}
			}
			else
			{
				base.InternalProcessRecord();
				MailboxDataProvider mailboxDataProvider2 = Utils.GetMailboxDataProvider(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, new Task.TaskErrorLoggingDelegate(base.WriteError));
				try
				{
					foreach (SearchObject searchObject2 in mailboxDataProvider2.FindPaged<SearchObject>(null, null, true, null, (int)(this.ResultSize.IsUnlimited ? 0U : (this.ResultSize.Value - base.WriteObjectCount))))
					{
						base.WriteResult(new MailboxSearchObject(searchObject2, searchObject2.SearchStatus ?? new SearchStatus()));
					}
				}
				catch (TenantAccessBlockedException exception)
				{
					base.WriteError(exception, (ErrorCategory)1003, null);
				}
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			MailboxDiscoverySearch mailboxDiscoverySearch = dataObject as MailboxDiscoverySearch;
			if (MailboxDiscoverySearch.IsInProgressState(mailboxDiscoverySearch.Status) || MailboxDiscoverySearch.IsInDeletionState(mailboxDiscoverySearch.Status))
			{
				Utils.CreateMailboxDiscoverySearchRequest((DiscoverySearchDataProvider)base.DataSession, mailboxDiscoverySearch.Name, ActionRequestType.UpdateStatus, base.ExchangeRunspaceConfig.GetRbacContext().ToString());
			}
			if (!MailboxDiscoverySearch.IsInDeletionState(mailboxDiscoverySearch.Status) || this.Identity != null || true == this.ShowDeletionInProgressSearches)
			{
				base.WriteResult(new MailboxSearchObject(mailboxDiscoverySearch, ((DiscoverySearchDataProvider)base.DataSession).OrganizationId));
			}
		}

		protected override OrganizationId ResolveCurrentOrganization()
		{
			if (this.Organization != null)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, ADSessionSettings.RescopeToSubtree(sessionSettings), 257, "ResolveCurrentOrganization", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Search\\GetMailboxSearch.cs");
				tenantOrTopologyConfigurationSession.UseConfigNC = false;
				ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Organization, tenantOrTopologyConfigurationSession, null, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())));
				return adorganizationalUnit.OrganizationId;
			}
			return base.CurrentOrganizationId ?? base.ExecutingUserOrganizationId;
		}
	}
}
