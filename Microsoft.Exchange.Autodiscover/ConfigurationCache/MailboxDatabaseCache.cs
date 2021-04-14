using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Autodiscover.ConfigurationCache
{
	internal class MailboxDatabaseCache : SimpleConfigCache<MailboxDatabase, ADObjectId>
	{
	}
}
