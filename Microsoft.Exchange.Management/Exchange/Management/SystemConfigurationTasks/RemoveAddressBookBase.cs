using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class RemoveAddressBookBase<TIdParameter> : RemoveSystemConfigurationObjectTask<TIdParameter, AddressBookBase> where TIdParameter : ADIdParameter, new()
	{
		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!base.HasErrors && base.DataObject.IsTopContainer)
			{
				TIdParameter identity = this.Identity;
				base.WriteError(new InvalidOperationException(Strings.ErrorInvalidOperationOnContainer(identity.ToString())), ErrorCategory.InvalidOperation, base.DataObject.Identity);
			}
			TaskLogger.LogExit();
		}

		protected abstract bool HandleRemoveWithAssociatedAddressBookPolicies();

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			AddressBookBase dataObject = base.DataObject;
			if (dataObject.CheckForAssociatedAddressBookPolicies() && !this.HandleRemoveWithAssociatedAddressBookPolicies())
			{
				return;
			}
			ADObjectId id = base.DataObject.Id;
			base.InternalProcessRecord();
			if (!base.HasErrors)
			{
				try
				{
					ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.DataObject.OrganizationId, base.ExecutingUserOrganizationId, false);
					IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.TenantGlobalCatalogSession.DomainController, true, ConsistencyMode.PartiallyConsistent, sessionSettings, 99, "InternalProcessRecord", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\AddressBook\\RemoveAddressBook.cs");
					tenantOrRootOrgRecipientSession.UseGlobalCatalog = true;
					UpdateAddressBookBase<AddressListIdParameter>.UpdateRecipients(base.DataObject, new ADObjectId[]
					{
						base.DataObject.Id
					}, base.DomainController, tenantOrRootOrgRecipientSession, new Task.TaskWarningLoggingDelegate(this.WriteWarning), new WriteProgress(base.WriteProgress), this);
				}
				catch (DataSourceTransientException ex)
				{
					TIdParameter identity = this.Identity;
					this.WriteWarning(Strings.ErrorReadMatchingRecipients(identity.ToString(), base.DataObject.LdapRecipientFilter, ex.Message));
					TaskLogger.Trace("Exception is raised while reading recipients: {0}", new object[]
					{
						ex.ToString()
					});
				}
				catch (DataSourceOperationException ex2)
				{
					TIdParameter identity2 = this.Identity;
					this.WriteWarning(Strings.ErrorReadMatchingRecipients(identity2.ToString(), base.DataObject.LdapRecipientFilter, ex2.Message));
					TaskLogger.Trace("Exception is raised while reading recipients matching filter: {0}", new object[]
					{
						ex2.ToString()
					});
				}
			}
			TaskLogger.LogExit();
		}
	}
}
