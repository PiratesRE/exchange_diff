using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "GlobalAddressList", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveGlobalAddressList : RemoveAddressBookBase<GlobalAddressListIdParameter>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveGlobalAddressList(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!base.HasErrors && (bool)base.DataObject[AddressBookBaseSchema.IsDefaultGlobalAddressList])
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorInvalidOperationOnDefaultGAL(this.Identity.ToString())), ErrorCategory.InvalidOperation, base.DataObject.Identity);
			}
			TaskLogger.LogExit();
		}

		protected override bool HandleRemoveWithAssociatedAddressBookPolicies()
		{
			base.WriteError(new InvalidOperationException(Strings.ErrorRemoveGlobalAddressListWithAssociatedAddressBookPolicies(base.DataObject.Name)), ErrorCategory.InvalidOperation, base.DataObject.Identity);
			return false;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			Guid guid = base.DataObject.Guid;
			OrganizationId organizationId = base.DataObject.OrganizationId;
			base.InternalProcessRecord();
			if (!base.HasErrors)
			{
				try
				{
					IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 365, "InternalProcessRecord", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\AddressBook\\RemoveAddressBook.cs");
					ADObjectId adobjectId = new ADObjectId("CN=Microsoft Exchange,CN=Services," + tenantOrTopologyConfigurationSession.ConfigurationNamingContext.DistinguishedName, Guid.Empty);
					ExchangeConfigurationContainerWithAddressLists exchangeConfigurationContainerWithAddressLists = tenantOrTopologyConfigurationSession.Read<ExchangeConfigurationContainerWithAddressLists>(adobjectId);
					if (exchangeConfigurationContainerWithAddressLists != null)
					{
						bool flag = false;
						foreach (ADObjectId adobjectId2 in exchangeConfigurationContainerWithAddressLists.DefaultGlobalAddressList)
						{
							if (adobjectId2.ObjectGuid == guid)
							{
								exchangeConfigurationContainerWithAddressLists.DefaultGlobalAddressList.Remove(adobjectId2);
								flag = true;
								break;
							}
						}
						if (exchangeConfigurationContainerWithAddressLists.LinkedAddressBookRootAttributesPresent())
						{
							foreach (ADObjectId adobjectId3 in exchangeConfigurationContainerWithAddressLists.DefaultGlobalAddressList2)
							{
								if (adobjectId3.ObjectGuid == guid)
								{
									exchangeConfigurationContainerWithAddressLists.DefaultGlobalAddressList2.Remove(adobjectId3);
									flag = true;
									break;
								}
							}
						}
						if (flag)
						{
							tenantOrTopologyConfigurationSession.Save(exchangeConfigurationContainerWithAddressLists);
						}
					}
					else
					{
						this.WriteWarning(Strings.ErrorExchangeConfigurationContainerNotFound(adobjectId.ToString()));
					}
				}
				catch (DataSourceTransientException)
				{
					this.WriteWarning(Strings.VerboseFailedRemoveGalDNInExchangeContainer(this.Identity.ToString()));
				}
			}
			TaskLogger.LogExit();
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return GlobalAddressList.FromDataObject((AddressBookBase)dataObject);
		}
	}
}
