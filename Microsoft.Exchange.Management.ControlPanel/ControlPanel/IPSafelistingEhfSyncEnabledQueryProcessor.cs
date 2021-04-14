using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class IPSafelistingEhfSyncEnabledQueryProcessor : SafelistingUIModeQueryProcessor
	{
		protected override SafelistingUIMode SafelistingUIMode
		{
			get
			{
				return SafelistingUIMode.Ecp;
			}
		}

		protected override string RbacRoleName
		{
			get
			{
				return "IPSafelistingEhfSyncEnabledRole";
			}
		}

		public const string RoleName = "IPSafelistingEhfSyncEnabledRole";
	}
}
