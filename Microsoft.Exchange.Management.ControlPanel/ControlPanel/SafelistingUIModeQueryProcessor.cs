using System;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal abstract class SafelistingUIModeQueryProcessor : EcpCmdletQueryProcessor
	{
		protected abstract SafelistingUIMode SafelistingUIMode { get; }

		protected abstract string RbacRoleName { get; }

		internal override bool? IsInRoleCmdlet(ExchangeRunspaceConfiguration rbacConfiguration)
		{
			IpSafeListings ipSafeListings = new IpSafeListings();
			PowerShellResults<IpSafeListing> @object = ipSafeListings.GetObject(null);
			if (@object.SucceededWithValue)
			{
				foreach (IpSafeListing ipSafeListing in @object.Output)
				{
					if (ipSafeListing.SafelistingUIMode == this.SafelistingUIMode)
					{
						return new bool?(true);
					}
				}
				return new bool?(false);
			}
			base.LogCmdletError(@object, this.RbacRoleName);
			return null;
		}
	}
}
