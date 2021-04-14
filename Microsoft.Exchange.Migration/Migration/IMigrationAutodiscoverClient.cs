using System;
using Microsoft.Exchange.Migration.DataAccessLayer;

namespace Microsoft.Exchange.Migration
{
	internal interface IMigrationAutodiscoverClient
	{
		AutodiscoverClientResponse GetUserSettings(ExchangeOutlookAnywhereEndpoint endpoint, string emailAddress);

		AutodiscoverClientResponse GetUserSettings(string userName, string encryptedPassword, string userDomain, string emailAddress);
	}
}
