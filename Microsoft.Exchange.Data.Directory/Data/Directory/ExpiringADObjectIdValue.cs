using System;
using Microsoft.Exchange.Common.Cache;

namespace Microsoft.Exchange.Data.Directory
{
	internal class ExpiringADObjectIdValue : ExpiringValue<ADObjectId, ExpiringADObjectIdValue.ADObjectIdExpirationWindowProvider>
	{
		internal ExpiringADObjectIdValue(ADObjectId value) : base(value)
		{
		}

		internal class ADObjectIdExpirationWindowProvider : IExpirationWindowProvider<ADObjectId>
		{
			TimeSpan IExpirationWindowProvider<ADObjectId>.GetExpirationWindow(ADObjectId unused)
			{
				return ExpiringADObjectIdValue.ADObjectIdExpirationWindowProvider.expirationWindow;
			}

			private static readonly TimeSpan expirationWindow = TimeSpan.FromHours(3.0);
		}
	}
}
