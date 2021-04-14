using System;
using Microsoft.Exchange.Common.Cache;

namespace Microsoft.Exchange.Data.Directory
{
	internal class ExpiringRidMasterNameValue : ExpiringValue<string, ExpiringRidMasterNameValue.RidMasterNameExpirationWindowProvider>
	{
		public ExpiringRidMasterNameValue(string value) : base(value)
		{
		}

		internal class RidMasterNameExpirationWindowProvider : IExpirationWindowProvider<string>
		{
			TimeSpan IExpirationWindowProvider<string>.GetExpirationWindow(string unused)
			{
				return ExpiringRidMasterNameValue.RidMasterNameExpirationWindowProvider.expirationWindow;
			}

			private static readonly TimeSpan expirationWindow = TimeSpan.FromHours(1.0);
		}
	}
}
