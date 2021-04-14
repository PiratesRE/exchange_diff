using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxTransport.StoreDriver
{
	internal class StoreDriverConfigurationLoadException : LocalizedException
	{
		internal StoreDriverConfigurationLoadException(string errorString) : base(new LocalizedString(errorString))
		{
		}
	}
}
