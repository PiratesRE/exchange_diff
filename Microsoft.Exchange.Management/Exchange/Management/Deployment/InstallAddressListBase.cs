using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public abstract class InstallAddressListBase : InstallAddressBookContainer
	{
		protected override ADObjectId RdnContainerToOrganization
		{
			get
			{
				return AddressList.RdnAlContainerToOrganization;
			}
		}

		protected void PostExchange(ADObjectId alContainer)
		{
			IConfigurationSession configurationSession = base.CreateGlobalWritableConfigSession();
			bool skipRangedAttributes = configurationSession.SkipRangedAttributes;
			configurationSession.SkipRangedAttributes = true;
			try
			{
				ExchangeConfigurationContainerWithAddressLists exchangeConfigurationContainerWithAddressLists = configurationSession.GetExchangeConfigurationContainerWithAddressLists();
				if (exchangeConfigurationContainerWithAddressLists.LinkedAddressBookRootAttributesPresent())
				{
					exchangeConfigurationContainerWithAddressLists.AddressBookRoots2.Add(alContainer);
				}
				base.WriteVerbose(TaskVerboseStringHelper.GetSaveObjectVerboseString(exchangeConfigurationContainerWithAddressLists, configurationSession, typeof(ExchangeConfigurationContainer)));
				configurationSession.Save(exchangeConfigurationContainerWithAddressLists);
				exchangeConfigurationContainerWithAddressLists.ResetChangeTracking();
				if (!AddressBookUtilities.IsTenantAddressList(configurationSession, alContainer))
				{
					exchangeConfigurationContainerWithAddressLists.AddressBookRoots.Add(alContainer);
					base.WriteVerbose(TaskVerboseStringHelper.GetSaveObjectVerboseString(exchangeConfigurationContainerWithAddressLists, configurationSession, typeof(ExchangeConfigurationContainer)));
					configurationSession.Save(exchangeConfigurationContainerWithAddressLists);
				}
			}
			finally
			{
				configurationSession.SkipRangedAttributes = skipRangedAttributes;
				base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(configurationSession));
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || CannedAddressListsFilterHelper.IsKnownException(exception);
		}
	}
}
