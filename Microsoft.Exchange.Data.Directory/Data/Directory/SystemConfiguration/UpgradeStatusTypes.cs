using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum UpgradeStatusTypes
	{
		None,
		NotStarted,
		InProgress,
		Warning,
		Error,
		Cancelled,
		Complete,
		ForceComplete
	}
}
