using System;

namespace Microsoft.Forefront.ActiveDirectoryConnector
{
	public struct FilteringSettings
	{
		public int MalwareFilteringUpdateFrequency;

		public int MalwareFilteringUpdateTimeout;

		public string MalwareFilteringPrimaryUpdatePath;

		public string MalwareFilteringSecondaryUpdatePath;
	}
}
