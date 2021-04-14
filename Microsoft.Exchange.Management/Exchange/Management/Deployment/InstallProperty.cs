using System;

namespace Microsoft.Exchange.Management.Deployment
{
	internal sealed class InstallProperty
	{
		private InstallProperty()
		{
		}

		public static readonly string VersionString = "VersionString";

		public static readonly string InstallSource = "InstallSource";

		public static readonly string LocalPackage = "LocalPackage";

		public static readonly string PackageName = "PackageName";
	}
}
