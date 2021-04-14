using System;

namespace Microsoft.Exchange.Data.Storage.StoreConfigurableType
{
	internal delegate UserConfiguration GetUserConfigurationDelegate(MailboxSession session, string configuration, UserConfigurationTypes type, bool createIfNonexisting);
}
