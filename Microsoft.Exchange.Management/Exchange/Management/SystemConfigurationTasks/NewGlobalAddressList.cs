using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "GlobalAddressList", SupportsShouldProcess = true, DefaultParameterSetName = "PrecannedFilter")]
	public sealed class NewGlobalAddressList : NewAddressBookBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewGlobalAddressList(base.Name.ToString());
			}
		}

		protected override ADObjectId GetContainerId()
		{
			return base.CurrentOrgContainerId.GetDescendantId(GlobalAddressList.RdnGalContainerToOrganization);
		}

		private void PostNewAddressBookBase()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.GlobalAddressListAttrbutes.Enabled)
			{
				bool flag = true;
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 625, "PostNewAddressBookBase", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\AddressBook\\NewAddressBook.cs");
				bool skipRangedAttributes = tenantOrTopologyConfigurationSession.SkipRangedAttributes;
				tenantOrTopologyConfigurationSession.SkipRangedAttributes = false;
				try
				{
					ExchangeConfigurationContainerWithAddressLists exchangeConfigurationContainerWithAddressLists = tenantOrTopologyConfigurationSession.GetExchangeConfigurationContainerWithAddressLists();
					try
					{
						if (exchangeConfigurationContainerWithAddressLists.LinkedAddressBookRootAttributesPresent())
						{
							AddressBookUtilities.SyncGlobalAddressLists(exchangeConfigurationContainerWithAddressLists, new Task.TaskWarningLoggingDelegate(this.WriteWarning));
							exchangeConfigurationContainerWithAddressLists.DefaultGlobalAddressList2.Add(this.DataObject.Id);
						}
						tenantOrTopologyConfigurationSession.Save(exchangeConfigurationContainerWithAddressLists);
						exchangeConfigurationContainerWithAddressLists.ResetChangeTracking();
						flag = false;
						if (!AddressBookUtilities.IsTenantAddressList(tenantOrTopologyConfigurationSession, this.DataObject.Id))
						{
							try
							{
								exchangeConfigurationContainerWithAddressLists.DefaultGlobalAddressList.Add(this.DataObject.Id);
								tenantOrTopologyConfigurationSession.Save(exchangeConfigurationContainerWithAddressLists);
							}
							catch (AdminLimitExceededException innerException)
							{
								if (!exchangeConfigurationContainerWithAddressLists.LinkedAddressBookRootAttributesPresent())
								{
									throw new ADOperationException(Strings.ErrorTooManyGALsCreated, innerException);
								}
								this.WriteWarning(Strings.WarningTooManyLegacyGALsCreated);
							}
						}
					}
					catch (DataSourceTransientException exception)
					{
						base.WriteError(exception, ErrorCategory.WriteError, this.DataObject.Id);
					}
				}
				finally
				{
					tenantOrTopologyConfigurationSession.SkipRangedAttributes = skipRangedAttributes;
					if (flag)
					{
						try
						{
							base.DataSession.Delete(this.DataObject);
						}
						catch (DataSourceTransientException ex)
						{
							TaskLogger.Trace("Exception is raised in rollback action (deleting the new Global Address List object): {0}", new object[]
							{
								ex.ToString()
							});
						}
					}
				}
			}
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable result)
		{
			TaskLogger.LogEnter(new object[]
			{
				result.Identity
			});
			this.PostNewAddressBookBase();
			base.WriteResult(new GlobalAddressList((AddressBookBase)result));
			TaskLogger.LogExit();
		}

		protected override string ClonableTypeName
		{
			get
			{
				return typeof(GlobalAddressList).FullName;
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return GlobalAddressList.FromDataObject((AddressBookBase)dataObject);
		}
	}
}
