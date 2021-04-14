using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal sealed class AddressBookUtilities
	{
		private AddressBookUtilities()
		{
		}

		internal static bool IsTenantAddressList(IConfigurationSession session, ADObjectId id)
		{
			string accountOrResourceForestFqdn = session.SessionSettings.GetAccountOrResourceForestFqdn();
			return ADSession.IsTenantIdentity(id, accountOrResourceForestFqdn);
		}

		internal static void SyncGlobalAddressLists(ExchangeConfigurationContainerWithAddressLists ecc, Task.TaskWarningLoggingDelegate writeWarning)
		{
			bool flag = false;
			IEnumerable<ADObjectId> enumerable = ecc.DefaultGlobalAddressList.Except(ecc.DefaultGlobalAddressList2);
			foreach (ADObjectId item in enumerable)
			{
				if (!flag)
				{
					writeWarning(Strings.WarningFixedMissingGALEntry);
					flag = true;
				}
				ecc.DefaultGlobalAddressList2.Add(item);
			}
		}
	}
}
