using System;

namespace Microsoft.Exchange.Data.Storage.StoreConfigurableType
{
	internal delegate IReadableUserConfiguration GetReadableUserConfigurationDelegate(MailboxSession session, string configuration, UserConfigurationTypes type, bool createIfNonexisting);
}
