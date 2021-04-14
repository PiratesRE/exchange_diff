using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class EhfAdminCenterEnabledQueryProcessor : SafelistingUIModeQueryProcessor
	{
		protected override SafelistingUIMode SafelistingUIMode
		{
			get
			{
				return SafelistingUIMode.EhfAC;
			}
		}

		protected override string RbacRoleName
		{
			get
			{
				return "EhfAdminCenterEnabledRole";
			}
		}

		public const string RoleName = "EhfAdminCenterEnabledRole";
	}
}
