using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ProvisioningCacheTasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PCDiagParamSets
	{
		private PCDiagParamSets()
		{
		}

		public const string GlobalCache = "GlobalCache";

		public const string OrganizationCache = "OrganizationCache";
	}
}
