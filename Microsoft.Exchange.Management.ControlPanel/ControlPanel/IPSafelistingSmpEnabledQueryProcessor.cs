using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class IPSafelistingSmpEnabledQueryProcessor : SafelistingUIModeQueryProcessor
	{
		protected override SafelistingUIMode SafelistingUIMode
		{
			get
			{
				return SafelistingUIMode.Smp;
			}
		}

		protected override string RbacRoleName
		{
			get
			{
				return "IPSafelistingSmpEnabledRole";
			}
		}

		public const string RoleName = "IPSafelistingSmpEnabledRole";
	}
}
